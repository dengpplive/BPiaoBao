using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using JoveZhao.Framework.Helper;
using PnrAnalysis;
using PnrAnalysis.Model;

namespace BPiaoBao.DomesticTicket.Domain.Services
{
    /// <summary>
    /// 查政策筛选 one查一条最优 all查所有根据政策或者价格选择
    /// </summary>
    public enum QueryPolicyType : int
    {
        one = 0,
        all = 1
    }
    /// <summary>
    /// B2B调用接口方法
    /// </summary>
    public enum B2BCallMethod : int
    {
        //[Description("新订单，等待选择政策")]
        NotKnow = -1,
        NewQueryPriceByPnr = 0,
        input = 1,
        Pay = 2,
        TicketOut = 3,
        TicketoutCallback = 4,
        NewQueryOrder = 5,
        OrderEx = 6
    }
    /// <summary>
    /// B2B自动出票服务
    /// </summary>
    public class AutoTicketService
    {
        /// <summary>
        /// 编码解析
        /// </summary>
        FormatPNR format = new FormatPNR();


        /// <summary>
        /// 检查本票通软件是否在运行 true 在运行 
        /// </summary>
        /// <param name="reqUrl"></param>
        /// <returns></returns>
        public bool checkclt(string reqUrl)
        {
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            bool IsRun = false;
            try
            {
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=checkclt");
                lstParam.Add("fmt=xml");
                lstParam.Add("xmlhashead=false");//返回信息是否带XML声明 true（返回xml带声明）  false （返回xml不带声明）当fmt=xml或者为空或者不传递时，返回信息带声明。
                //请求地址
                string ReqUrl = string.Format("{0}?{1}", reqUrl, string.Join("&", lstParam.ToArray()));
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(reqUrl, MethodType.GET, Encoding.Default, 60, lstParam.ToArray());
                string result = response.Data;
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                DataSet ds_checkclt = XmlToDataSet(result);

                if (ds_checkclt != null && ds_checkclt.Tables.Count > 0
                    && ds_checkclt.Tables[0].Rows.Count > 0
                    && ds_checkclt.Tables[0].Columns.Contains("message")
                    )
                {
                    string code = ds_checkclt.Tables[0].Rows[0]["code"].ToString();
                    string message = ds_checkclt.Tables[0].Rows[0]["message"].ToString();
                    if (code == "1")
                    {
                        IsRun = true;
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("checkclt", sbLog.ToString());
            }
            return IsRun;
        }
        /// <summary>
        /// 全自动出票方法
        /// </summary>
        /// <returns></returns>
        public B2BResponse B2BAutoEtdz(AutoEtdzParam autoEtdzParam)
        {
            List<string> lstParam = new List<string>();
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                if (!string.IsNullOrEmpty(strErrMsg))
                {
                    sbLog.Append("错误信息:" + strErrMsg + "\r\n");
                    b2bResponse.Remark = strErrMsg;
                }
                else
                {
                    //请求参数通过
                    autoEtdzParam.ParamIsPass = true;
                    #region 调用接口
                    sbLog.Append("开始调用接口\r\n");
                    if (string.IsNullOrEmpty(autoEtdzParam.LastCallMethod))
                    {
                        sbLog.AppendFormat("走全自动流程 UseAutoType={0}\r\n", autoEtdzParam.UseAutoType);
                        #region //走全自动流程
                        //0使用OrderEx 1使用查询政策的方法
                        if (autoEtdzParam.UseAutoType == 0)
                        {
                            #region OrderEx
                            sbLog.Append("[OrderEx]\r\n");
                            b2bResponse = OrderEx(autoEtdzParam);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_OrderEx = b2bResponse.DsResult;
                            if (ds_OrderEx != null && ds_OrderEx.Tables.Count > 0)
                            {
                                //解析成功 处理OrderEx
                                if ((ds_OrderEx.Tables["autopayinfo"].Rows[0]["code"].ToString() == "1"
                                    && ds_OrderEx.Tables["autopayinfo"].Rows[0]["paystatus"].ToString() == "1")
                                    || ds_OrderEx.Tables["autopayinfo"].Rows[0]["errorinfo"].ToString().Contains("该订单已经出票成功，平台订单号")
                                    )
                                {
                                    b2bResponse.Status = true;
                                    b2bResponse.Remark = " 请等待航空公司出票后系统自动回帖票号！";
                                    autoEtdzParam.LastCallMethod = "";
                                }
                                else
                                {
                                    //如果没有入库成功 下次调用入库接口 
                                    if (ds_OrderEx.Tables[0].Rows[0]["code"].ToString() != "1"
                                        || ds_OrderEx.Tables["autopayinfo"].Rows[0]["errorinfo"].ToString().Contains("入库失败"))
                                    {
                                        b2bResponse.LastCallMethod = "input";
                                        b2bResponse.Remark = ds_OrderEx.Tables["autopayinfo"].Rows[0]["errorinfo"].ToString();
                                    }
                                    else
                                    {
                                        //支付没有成功 调用支付接口
                                        if (ds_OrderEx.Tables["autopayinfo"].Rows[0]["paystatus"].ToString() != "1")
                                        {
                                            if (ds_OrderEx.Tables["autopayinfo"].Rows[0]["errorinfo"].ToString().Contains("已出票"))
                                            {
                                                #region TicketOut 同步返回票号出票
                                                b2bResponse = TicketOut(autoEtdzParam);
                                                sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                DataSet ds_TicketOut = b2bResponse.DsResult;
                                                b2bResponse.Remark = ds_TicketOut.Tables[0].Rows[0]["message"].ToString();
                                                b2bResponse.LastCallMethod = "TicketOut";
                                                #endregion
                                            }
                                            if (!b2bResponse.Status)
                                            {
                                                #region //调用单独支付
                                                b2bResponse = Pay(autoEtdzParam);
                                                sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                DataSet ds_Pay = b2bResponse.DsResult;
                                                if (ds_Pay != null && ds_Pay.Tables.Count > 0)
                                                {
                                                    if (ds_Pay.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                                    {
                                                        //检查是否有票号
                                                        b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                                        #region //支付成功 调用出票接口
                                                        if (!b2bResponse.Status)
                                                        {
                                                            b2bResponse = TicketoutCallback(autoEtdzParam);
                                                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                            DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                                                            if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                                                            {
                                                                if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                                                {
                                                                    //出票成功
                                                                    b2bResponse.Status = true;
                                                                    b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                                                    autoEtdzParam.LastCallMethod = "";
                                                                }
                                                                else
                                                                {
                                                                    //出票失败  
                                                                    b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                                                    b2bResponse.LastCallMethod = "TicketoutCallback";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //调用出票接口失败  
                                                                b2bResponse.LastCallMethod = "TicketoutCallback";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //调用出票接口失败  
                                                            b2bResponse.LastCallMethod = "TicketoutCallback";
                                                        }
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        if (ds_Pay.Tables[0].Rows[0]["message"].ToString().Contains("出票完成"))
                                                        {
                                                            #region TicketOut 同步返回票号出票
                                                            b2bResponse = TicketOut(autoEtdzParam);
                                                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                            DataSet ds_TicketOut = b2bResponse.DsResult;
                                                            b2bResponse.Remark = ds_TicketOut.Tables[0].Rows[0]["message"].ToString();
                                                            b2bResponse.LastCallMethod = "TicketOut";
                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            //支付失败
                                                            b2bResponse.Remark = ds_Pay.Tables[0].Rows[0]["message"].ToString();
                                                            b2bResponse.LastCallMethod = "Pay";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //支付失败                                            
                                                    b2bResponse.LastCallMethod = "Pay";
                                                }
                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            #region   //支付成功 调用出票接口
                                            //检查是否有票号
                                            b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                            if (!b2bResponse.Status)
                                            {
                                                b2bResponse = TicketoutCallback(autoEtdzParam);
                                                sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                                                if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                                                {
                                                    if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                                    {
                                                        //出票成功
                                                        b2bResponse.Status = true;
                                                        b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                                        autoEtdzParam.LastCallMethod = "";
                                                    }
                                                    else
                                                    {
                                                        //出票失败                                                           
                                                        b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                                        b2bResponse.LastCallMethod = "TicketoutCallback";
                                                    }
                                                }
                                                else
                                                {
                                                    //调用出票接口失败   
                                                    b2bResponse.LastCallMethod = "TicketoutCallback";
                                                }
                                            }
                                            else
                                            {
                                                //调用出票接口失败 
                                                b2bResponse.LastCallMethod = "TicketoutCallback";
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //失败
                                b2bResponse.Remark = "解析失败(OrderEx)";
                            }
                            #endregion
                        }
                        else
                        {
                            #region//查询政策 one
                            b2bResponse = NewQueryPriceByPnr(autoEtdzParam, QueryPolicyType.one);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_NewQueryPriceByPnr = b2bResponse.DsResult;
                            if (ds_NewQueryPriceByPnr != null && ds_NewQueryPriceByPnr.Tables.Count > 0)
                            {
                                if (ds_NewQueryPriceByPnr.Tables.Contains("pnrinfo")
                                    && ds_NewQueryPriceByPnr.Tables.Contains("policy")
                                    && ds_NewQueryPriceByPnr.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1"
                                    && ds_NewQueryPriceByPnr.Tables["policy"].Rows.Count > 0)
                                {
                                    //查询政策成功 
                                    #region //获取一条最优政策
                                    decimal policynum = autoEtdzParam.OldPolicyPoint;
                                    decimal ticketPrice = autoEtdzParam.PayInfo.SeatTotalPrice;
                                    decimal totaltax = autoEtdzParam.PayInfo.TaxTotalPrice;
                                    decimal payprice = autoEtdzParam.PayInfo.PayTotalPrice;
                                    DataView dataView = ds_NewQueryPriceByPnr.Tables["policy"].DefaultView;
                                    dataView.Sort = " policynum desc ";
                                    DataTable table = dataView.ToTable();
                                    DataRow dr = table.Rows[0];
                                    decimal.TryParse(dr["policynum"].ToString(), out policynum);
                                    decimal.TryParse(dr["ticketprice"].ToString(), out ticketPrice);
                                    decimal.TryParse(dr["totaltax"].ToString(), out totaltax);
                                    decimal.TryParse(dr["payprice"].ToString(), out payprice);

                                    //如果指定了舱位价就匹配 价格
                                    decimal tempticketprice = 0m;
                                    DataRowCollection drs = table.Rows;
                                    foreach (DataRow drPolicy in drs)
                                    {
                                        decimal.TryParse(drPolicy["ticketprice"].ToString(), out tempticketprice);
                                        if (autoEtdzParam.PayInfo.SeatTotalPrice == tempticketprice)
                                        {
                                            decimal.TryParse(drPolicy["policynum"].ToString(), out policynum);
                                            decimal.TryParse(drPolicy["ticketprice"].ToString(), out ticketPrice);
                                            decimal.TryParse(drPolicy["totaltax"].ToString(), out totaltax);
                                            decimal.TryParse(drPolicy["payprice"].ToString(), out payprice);
                                            break;
                                        }
                                    }
                                    autoEtdzParam.OldPolicyPoint = policynum;
                                    autoEtdzParam.PayInfo.SeatTotalPrice = ticketPrice;
                                    autoEtdzParam.PayInfo.TaxTotalPrice = totaltax;
                                    autoEtdzParam.PayInfo.PayTotalPrice = payprice;
                                    #endregion

                                    #region 入库

                                    b2bResponse = Input(autoEtdzParam);
                                    sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                    sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                    DataSet ds_Input = b2bResponse.DsResult;
                                    if (ds_Input != null && ds_Input.Tables.Count > 0)
                                    {
                                        if (ds_Input.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                        {
                                            //检查是否有票号
                                            b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                            if (!b2bResponse.Status)
                                            {
                                                //入库 调用支付接口
                                                #region //调用单独支付
                                                b2bResponse = Pay(autoEtdzParam);
                                                sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                DataSet ds_Pay = b2bResponse.DsResult;
                                                if (ds_Pay != null && ds_Pay.Tables.Count > 0)
                                                {
                                                    if (ds_Pay.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                                    {
                                                        //检查是否有票号
                                                        b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                                        if (!b2bResponse.Status)
                                                        {
                                                            //支付成功 调用出票接口
                                                            b2bResponse = TicketoutCallback(autoEtdzParam);
                                                            DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                                                            if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                                                            {
                                                                if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                                                {
                                                                    //出票成功
                                                                    b2bResponse.Status = true;
                                                                    b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                                                    autoEtdzParam.LastCallMethod = "";
                                                                }
                                                                else
                                                                {
                                                                    //出票失败                                                           
                                                                    b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                                                    b2bResponse.LastCallMethod = "TicketoutCallback";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //调用出票接口失败                                                                                                              
                                                                b2bResponse.LastCallMethod = "TicketoutCallback";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //支付失败
                                                            //b2bResponse.Remark = ds_Pay.Tables[0].Rows[0]["message"].ToString();
                                                            //b2bResponse.LastCallMethod = "Pay";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //支付失败
                                                        b2bResponse.Remark = ds_Pay.Tables[0].Rows[0]["message"].ToString();
                                                        b2bResponse.LastCallMethod = "Pay";
                                                    }
                                                }
                                                else
                                                {
                                                    //支付失败                                              
                                                    b2bResponse.LastCallMethod = "Pay";
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                //支付失败                                              
                                                b2bResponse.LastCallMethod = "Pay";
                                            }
                                        }
                                        else
                                        {
                                            //入库失败                                            
                                            b2bResponse.Remark = ds_Input.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                            b2bResponse.LastCallMethod = "input";
                                        }
                                    }
                                    else
                                    {
                                        //入库解析失败
                                        b2bResponse.LastCallMethod = "input";
                                    }
                                    #endregion
                                }
                                else
                                {
                                    //查询政策失败 为货到
                                    b2bResponse.Remark = ds_NewQueryPriceByPnr.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                    b2bResponse.LastCallMethod = "NewQueryPriceByPnr";
                                }
                            }
                            else
                            {
                                //查询政策失败
                                b2bResponse.Remark = ds_NewQueryPriceByPnr.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                b2bResponse.LastCallMethod = "NewQueryPriceByPnr";
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region //调用具体的接口 走后续流程
                        string LastCallMethod = autoEtdzParam.LastCallMethod;
                        sbLog.AppendFormat("LastCallMethod={0}\r\n", LastCallMethod);
                        if (LastCallMethod == "NewQueryPriceByPnr")
                        {
                            #region//查询航空公司政策和价格 one
                            b2bResponse = NewQueryPriceByPnr(autoEtdzParam, QueryPolicyType.all);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_NewQueryPriceByPnr = b2bResponse.DsResult;
                            if (ds_NewQueryPriceByPnr != null && ds_NewQueryPriceByPnr.Tables.Count > 0)
                            {
                                if (ds_NewQueryPriceByPnr.Tables.Contains("pnrinfo")
                                    && ds_NewQueryPriceByPnr.Tables.Contains("policy")
                                    && ds_NewQueryPriceByPnr.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1"
                                    && ds_NewQueryPriceByPnr.Tables["policy"].Rows.Count > 0)
                                {
                                    //查询政策成功 
                                    #region //获取一条最优政策
                                    decimal policynum = autoEtdzParam.OldPolicyPoint;
                                    decimal ticketPrice = autoEtdzParam.PayInfo.SeatTotalPrice;
                                    decimal totaltax = autoEtdzParam.PayInfo.TaxTotalPrice;
                                    decimal payprice = autoEtdzParam.PayInfo.PayTotalPrice;
                                    DataView dataView = ds_NewQueryPriceByPnr.Tables["policy"].DefaultView;
                                    dataView.Sort = " policynum desc ";
                                    DataTable table = dataView.ToTable();
                                    DataRow dr = table.Rows[0];
                                    decimal.TryParse(dr["policynum"].ToString(), out policynum);
                                    decimal.TryParse(dr["ticketprice"].ToString(), out ticketPrice);
                                    decimal.TryParse(dr["totaltax"].ToString(), out totaltax);
                                    decimal.TryParse(dr["payprice"].ToString(), out payprice);

                                    //如果指定了舱位价就匹配 价格
                                    decimal tempticketprice = 0m;
                                    DataRowCollection drs = table.Rows;
                                    foreach (DataRow drPolicy in drs)
                                    {
                                        decimal.TryParse(drPolicy["ticketprice"].ToString(), out tempticketprice);
                                        if (autoEtdzParam.PayInfo.SeatTotalPrice == tempticketprice)
                                        {
                                            decimal.TryParse(drPolicy["policynum"].ToString(), out policynum);
                                            decimal.TryParse(drPolicy["ticketprice"].ToString(), out ticketPrice);
                                            decimal.TryParse(drPolicy["totaltax"].ToString(), out totaltax);
                                            decimal.TryParse(drPolicy["payprice"].ToString(), out payprice);
                                            break;
                                        }
                                    }
                                    autoEtdzParam.OldPolicyPoint = policynum;
                                    autoEtdzParam.PayInfo.SeatTotalPrice = ticketPrice;
                                    autoEtdzParam.PayInfo.TaxTotalPrice = totaltax;
                                    autoEtdzParam.PayInfo.PayTotalPrice = payprice;
                                    #endregion

                                    #region 入库

                                    b2bResponse = Input(autoEtdzParam);
                                    sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                    sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                    DataSet ds_Input = b2bResponse.DsResult;
                                    if (ds_Input != null && ds_Input.Tables.Count > 0)
                                    {
                                        if (ds_Input.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                        {
                                            //入库 调用支付接口
                                            #region //入库成功 调用单独支付
                                            b2bResponse = Pay(autoEtdzParam);
                                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                            DataSet ds_Pay = b2bResponse.DsResult;
                                            if (ds_Pay != null && ds_Pay.Tables.Count > 0)
                                            {
                                                if (ds_Pay.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                                {
                                                    #region  //支付成功 调用出票接口
                                                    b2bResponse = TicketoutCallback(autoEtdzParam);
                                                    sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                    sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                    DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                                                    if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                                                    {
                                                        if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                                        {
                                                            //出票成功
                                                            b2bResponse.Status = true;
                                                            b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                                            autoEtdzParam.LastCallMethod = "";
                                                        }
                                                        else
                                                        {
                                                            //出票失败                                                           
                                                            b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                                            b2bResponse.LastCallMethod = "TicketoutCallback";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //调用出票接口失败                                                                                                              
                                                        b2bResponse.LastCallMethod = "TicketoutCallback";
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    if (ds_Pay.Tables[0].Rows[0]["message"].ToString().Contains("出票完成"))
                                                    {
                                                        #region TicketOut 同步返回票号出票
                                                        b2bResponse = TicketOut(autoEtdzParam);
                                                        sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                        sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                        DataSet ds_TicketOut = b2bResponse.DsResult;
                                                        b2bResponse.Remark = ds_TicketOut.Tables[0].Rows[0]["message"].ToString();
                                                        b2bResponse.LastCallMethod = "TicketOut";
                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        //支付失败
                                                        b2bResponse.Remark = ds_Pay.Tables[0].Rows[0]["message"].ToString();
                                                        b2bResponse.LastCallMethod = "Pay";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //支付失败                                              
                                                b2bResponse.LastCallMethod = "Pay";
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            //入库失败                                            
                                            b2bResponse.Remark = ds_Input.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                            b2bResponse.LastCallMethod = "input";
                                        }
                                    }
                                    else
                                    {
                                        //入库解析失败
                                        b2bResponse.LastCallMethod = "input";
                                    }
                                    #endregion
                                }
                                else
                                {
                                    //查询政策失败 为货到
                                    b2bResponse.Remark = ds_NewQueryPriceByPnr.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                    b2bResponse.LastCallMethod = "NewQueryPriceByPnr";
                                }
                            }
                            else
                            {
                                //查询政策失败
                                b2bResponse.Remark = ds_NewQueryPriceByPnr.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                b2bResponse.LastCallMethod = "NewQueryPriceByPnr";
                            }
                            #endregion
                        }
                        else if (LastCallMethod == "input")
                        {
                            #region //查询政策成功 入库

                            b2bResponse = Input(autoEtdzParam);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_Input = b2bResponse.DsResult;
                            if (ds_Input != null && ds_Input.Tables.Count > 0)
                            {
                                if (ds_Input.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                {
                                    //检查是否有票号
                                    b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                    if (!b2bResponse.Status)
                                    {
                                        //入库成功 调用支付接口
                                        #region //调用单独支付
                                        b2bResponse = Pay(autoEtdzParam);
                                        sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                        sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                        DataSet ds_Pay = b2bResponse.DsResult;
                                        if (ds_Pay != null && ds_Pay.Tables.Count > 0)
                                        {
                                            if (ds_Pay.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                            {
                                                //检查是否有票号
                                                b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                                if (!b2bResponse.Status)
                                                {
                                                    #region //支付成功 调用出票接口
                                                    b2bResponse = TicketoutCallback(autoEtdzParam);
                                                    DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                                                    if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                                                    {
                                                        if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                                        {
                                                            //出票成功
                                                            b2bResponse.Status = true;
                                                            b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                                            autoEtdzParam.LastCallMethod = "";
                                                        }
                                                        else
                                                        {
                                                            //出票失败                                                           
                                                            b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                                            b2bResponse.LastCallMethod = "TicketoutCallback";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //调用出票接口失败  
                                                        b2bResponse.LastCallMethod = "TicketoutCallback";
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    //调用出票接口失败  
                                                    b2bResponse.LastCallMethod = "TicketoutCallback";
                                                }
                                            }
                                            else
                                            {
                                                if (ds_Pay.Tables[0].Rows[0]["message"].ToString().Contains("出票完成"))
                                                {
                                                    #region TicketOut 同步返回票号出票
                                                    b2bResponse = TicketOut(autoEtdzParam);
                                                    sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                                    sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                                    DataSet ds_TicketOut = b2bResponse.DsResult;
                                                    b2bResponse.Remark = ds_TicketOut.Tables[0].Rows[0]["message"].ToString();
                                                    b2bResponse.LastCallMethod = "TicketOut";
                                                    #endregion
                                                }
                                                else
                                                {
                                                    //支付失败
                                                    b2bResponse.Remark = ds_Pay.Tables[0].Rows[0]["message"].ToString();
                                                    b2bResponse.LastCallMethod = "Pay";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //支付失败                                              
                                            b2bResponse.LastCallMethod = "Pay";
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        b2bResponse.LastCallMethod = "input";
                                    }
                                }
                                else
                                {
                                    //入库失败                                            
                                    b2bResponse.Remark = ds_Input.Tables["pnrinfo"].Rows[0]["message"].ToString();
                                    b2bResponse.LastCallMethod = "input";
                                }
                            }
                            else
                            {
                                //入库解析失败
                                b2bResponse.LastCallMethod = "input";
                            }
                            #endregion
                        }
                        else if (LastCallMethod == "Pay")
                        {
                            #region //入库成功 调用单独支付
                            b2bResponse = Pay(autoEtdzParam);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_Pay = b2bResponse.DsResult;
                            if (ds_Pay != null && ds_Pay.Tables.Count > 0)
                            {
                                if (ds_Pay.Tables["pnrinfo"].Rows[0]["code"].ToString() == "1")
                                {
                                    //检查是否有票号
                                    b2bResponse = SyncTicketCall(b2bResponse.RetuenXML);
                                    if (!b2bResponse.Status)
                                    {
                                        #region  //支付成功 调用出票接口
                                        b2bResponse = TicketoutCallback(autoEtdzParam);
                                        sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                        sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                        DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                                        if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                                        {
                                            if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                            {
                                                //出票成功
                                                b2bResponse.Status = true;
                                                b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                                autoEtdzParam.LastCallMethod = "";
                                            }
                                            else
                                            {
                                                //出票失败                                                           
                                                b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                                b2bResponse.LastCallMethod = "TicketoutCallback";
                                            }
                                        }
                                        else
                                        {
                                            //调用出票接口失败 
                                            b2bResponse.LastCallMethod = "TicketoutCallback";
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        //支付失败                                        
                                        b2bResponse.LastCallMethod = "Pay";
                                    }
                                }
                                else
                                {
                                    if (ds_Pay.Tables[0].Rows[0]["message"].ToString().Contains("出票完成"))
                                    {
                                        #region TicketOut 同步返回票号出票
                                        b2bResponse = TicketOut(autoEtdzParam);
                                        sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                                        sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                                        DataSet ds_TicketOut = b2bResponse.DsResult;
                                        b2bResponse.Remark = ds_TicketOut.Tables[0].Rows[0]["message"].ToString();
                                        b2bResponse.LastCallMethod = "TicketOut";
                                        #endregion
                                    }
                                    else
                                    {
                                        //支付失败
                                        b2bResponse.Remark = ds_Pay.Tables[0].Rows[0]["message"].ToString();
                                        b2bResponse.LastCallMethod = "Pay";
                                    }
                                }
                            }
                            else
                            {
                                //支付失败                                              
                                b2bResponse.LastCallMethod = "Pay";
                            }
                            #endregion
                        }
                        else if (LastCallMethod == "TicketOut")
                        {
                            #region  //支付成功 调用同步出票接口
                            b2bResponse = TicketOut(autoEtdzParam);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_TicketOut = b2bResponse.DsResult;
                            if (ds_TicketOut != null && ds_TicketOut.Tables.Count > 0)
                            {
                                if (ds_TicketOut.Tables[0].Rows[0]["code"].ToString() == "1")
                                {
                                    //同步出票成功
                                    b2bResponse.Status = true;
                                    b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                    autoEtdzParam.LastCallMethod = "";
                                }
                                else
                                {
                                    //同步出票失败                                                           
                                    b2bResponse.Remark = ds_TicketOut.Tables[0].Rows[0]["message"].ToString();
                                    b2bResponse.LastCallMethod = "TicketOut";
                                }
                            }
                            else
                            {
                                //调用同步出票接口失败                                                                                                              
                                b2bResponse.LastCallMethod = "TicketOut";
                            }
                            #endregion
                        }
                        else if (LastCallMethod == "TicketoutCallback")
                        {
                            #region  //支付成功 调用异步出票接口
                            b2bResponse = TicketoutCallback(autoEtdzParam);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_TicketoutCallback = b2bResponse.DsResult;
                            if (ds_TicketoutCallback != null && ds_TicketoutCallback.Tables.Count > 0)
                            {
                                if (ds_TicketoutCallback.Tables[0].Rows[0]["code"].ToString() == "1")
                                {
                                    //异步出票成功
                                    b2bResponse.Status = true;
                                    b2bResponse.Remark = "调用出票成功,等待回帖票号...";
                                    autoEtdzParam.LastCallMethod = "";
                                }
                                else
                                {
                                    //异步出票失败                                                           
                                    b2bResponse.Remark = ds_TicketoutCallback.Tables[0].Rows[0]["message"].ToString();
                                    b2bResponse.LastCallMethod = "TicketoutCallback";
                                }
                            }
                            else
                            {
                                //调用异步出票接口失败                                                                                                              
                                b2bResponse.LastCallMethod = "TicketoutCallback";
                            }
                            #endregion
                        }
                        else if (LastCallMethod == "NewQueryOrder")
                        {
                            #region //查询航空公司订单状态
                            b2bResponse = NewQueryOrder(autoEtdzParam);
                            sbLog.AppendFormat("调用{0}\r\n", autoEtdzParam.RequestUrl);
                            sbLog.AppendFormat("返回结果：{0}\r\n\r\n", b2bResponse.RetuenXML);
                            DataSet ds_NewQueryOrder = b2bResponse.DsResult;
                            if (ds_NewQueryOrder != null && ds_NewQueryOrder.Tables.Count > 0)
                            {
                                if (ds_NewQueryOrder.Tables[0].Rows[0]["code"].ToString() == "1")
                                {
                                    //查询订单状态成功
                                    b2bResponse.Status = true;
                                    b2bResponse.Remark = ds_NewQueryOrder.Tables[0].Rows[0]["message"].ToString();
                                    autoEtdzParam.LastCallMethod = "";
                                }
                                else
                                {
                                    //查询订单状态失败
                                    b2bResponse.Remark = ds_NewQueryOrder.Tables[0].Rows[0]["status"].ToString();
                                    b2bResponse.LastCallMethod = "NewQueryOrder";
                                }
                            }
                            else
                            {
                                //查询订单状态失败
                                b2bResponse.LastCallMethod = "NewQueryOrder";
                            }
                            #endregion
                        }
                        #endregion
                    }
                    sbLog.Append("结束调用接口\r\n");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("AutoEtdz", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 全自动流程(原)
        /// </summary>
        /// <param name="autoEtdzParam"></param>
        /// <returns></returns>
        public B2BResponse OrderEx(AutoEtdzParam autoEtdzParam)
        {
            List<string> lstParam = new List<string>();
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                lstParam.Add("cmd=orderex");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));
                lstParam.Add("bigpnr=1");
                lstParam.Add("autopayflag=1");
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                lstParam.Add("pnrc=" + autoEtdzParam.Pnr);
                lstParam.Add("callbackurl=" + autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl);
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("fmt=xml");
                lstParam.Add("xmlhashead=false");
                lstParam.Add("getticketovertime=3");
                lstParam.Add("payaccount=" + autoEtdzParam.PayInfo.PayAccount);
                lstParam.Add("callbackurlforpay=" + autoEtdzParam.UrlInfo.AlipayPayNotifyUrl);

                Dictionary<string, string> exDic = autoEtdzParam.ExParam;
                foreach (KeyValuePair<string, string> item in exDic)
                {
                    if (autoEtdzParam.CarryCode.Trim().ToUpper() == "HU"
                        )
                    {
                        if (item.Key == "hu_linktel")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                        if (item.Key == "hu_linkman")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                    }
                    else if (autoEtdzParam.CarryCode.Trim().ToUpper() == "CZ")
                    {
                        if (item.Key == "cz_linktel")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                    }
                }
                //有多个价格 采用指定的价格限制 默认采用范围
                if (autoEtdzParam.IsLimitScope)
                {
                    lstParam.Add("srcticketprice=" + autoEtdzParam.PayInfo.SeatTotalPrice);
                }
                else
                {
                    lstParam.Add("checkprice=0-" + autoEtdzParam.PayInfo.PayTotalPrice);
                }
                if (!string.IsNullOrEmpty(strErrMsg))
                {
                    sbLog.Append("错误信息:" + strErrMsg + "\r\n");
                    b2bResponse.Remark = strErrMsg;
                }
                else
                {
                    //请求地址
                    string ReqUrl = string.Format("{0}?{1}", autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                    autoEtdzParam.RequestUrl = "[OrderEx]:" + ReqUrl;
                    sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                    WebHttp webHttp = new WebHttp();
                    DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                    if (response.Status != HttpStatusCode.OK)
                    {
                        //失败再次请求一次
                        response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                    }
                    string result = response.Data;
                    //<autopayinfo><code>1</code><pnr>NJD40G</pnr><air>HU</air><paystatus>0</paystatus><errorInfo>入库成功，支付信息获取失败，请重试或到航空公司网站支付</errorInfo><payprice>295.0</payprice><tradeno></tradeno><outtradeno></outtradeno></autopayinfo>
                    //<autopayinfo><code>1</code><pnr>NJD40G</pnr><air>HU</air><paystatus>0</paystatus><errorInfo>网站提示： Err701:已出票！! Err701:已出票！! Err708:出票状态应为出票时限形式!</errorInfo><payprice>0</payprice><tradeno></tradeno><outtradeno></outtradeno></autopayinfo>
                    //<autopayinfo><code>0</code><pnr>NZEWDM</pnr><errorinfo>获取登录信息失败，请稍后再试</errorinfo></autopayinfo>
                    sbLog.Append("返回结果:\r\n" + result + "\r\n");
                    b2bResponse.LastCallMethod = "OrderEx";
                    b2bResponse.RetuenXML = result;
                    b2bResponse.DsResult = XmlToDataSet(result);
                    if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Contains("autopayinfo")
                        && b2bResponse.DsResult.Tables["autopayinfo"].Rows.Count > 0
                        && b2bResponse.DsResult.Tables["autopayinfo"].Columns.Contains("errorinfo")
                        )
                    {
                        b2bResponse.Remark = b2bResponse.DsResult.Tables["autopayinfo"].Rows[0]["errorinfo"].ToString();
                    }
                    else
                    {
                        b2bResponse.Remark = "[OrderEx]:" + result;
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("OrderEx", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <returns></returns>
        public B2BResponse NewQueryOrder(AutoEtdzParam autoEtdzParam)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=newqueryorder");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);
                lstParam.Add("bigpnr=1");
                lstParam.Add("fmt=xml");
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                lstParam.Add("xmlhashead=false");
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));

                //请求地址
                string ReqUrl = string.Format("{0}?{1}", autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                autoEtdzParam.RequestUrl = "[NewQueryOrder]:" + ReqUrl;
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                if (response.Status != HttpStatusCode.OK)
                {
                    //失败再次请求一次
                    response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                }
                string result = response.Data;
                //<pnrinfo><code>1</code><pnr>MCDP0H</pnr><pnrsrcid>0140704141742444045</pnrsrcid><message></message><result><status>3</status><payprice>3225.00</payprice><tickets></tickets></result></pnrinfo>
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                b2bResponse.LastCallMethod = "NewQueryOrder";
                b2bResponse.RetuenXML = result;
                b2bResponse.DsResult = XmlToDataSet(result);
                if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Contains("pnrinfo")
                    && b2bResponse.DsResult.Tables["pnrinfo"].Rows.Count > 0
                    && b2bResponse.DsResult.Tables["pnrinfo"].Columns.Contains("message")
                    )
                {
                    string code = b2bResponse.DsResult.Tables["pnrinfo"].Rows[0]["status"].ToString();
                    string status = GetOrderStatus(code);
                    b2bResponse.Remark = code + "|" + status;
                }
                else
                {
                    b2bResponse.Remark = "[NewQueryOrder]:" + result;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("NewQueryOrder", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 查询航空公司政策
        /// </summary>
        /// <param name="autoEtdzParam"></param>
        /// <param name="qrytype"></param>
        /// <returns></returns>
        public B2BResponse NewQueryPriceByPnr(AutoEtdzParam autoEtdzParam, QueryPolicyType qrytype)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                //验证参数
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=newquerypricebypnr");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);
                lstParam.Add("fmt=xml");
                lstParam.Add("xmlhashead=false");
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                //平台订单号
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));

                //one（回传1条最优的政策，如果校验票面价就返回返点最高的政策，如果不校验票面价就返回支付价格最低的政策） 
                //all（回传所有的政策）
                lstParam.Add("qrytype=" + qrytype.ToString());

                //指定票面价（国航是根据票面价返回规则确定是校验单成人票面价还是总票面价，其他航空公司校验单成人票面价）
                //。返回与票面价一致的价格信息；未指定票面价，则返回支付价最低的价格信息。单位为（元）。
                //指定票面价
                if (autoEtdzParam.IsLimitScope)
                {
                    //返回政策点数最高的政策
                    lstParam.Add("ticketprice=" + autoEtdzParam.PayInfo.SeatTotalPrice);
                }

                //请求地址
                string ReqUrl = string.Format("{0}?{1}", autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                autoEtdzParam.RequestUrl = "[NewQueryPriceByPnr]:" + ReqUrl;
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                if (response.Status != HttpStatusCode.OK)
                {
                    //失败再次请求一次
                    response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                }
                string result = response.Data;
                //<pnrinfo><code>1</code><pnr>NFXCT8|HDFTP6</pnr><message></message><air>3U</air><policies><policy><pgid>1</pgid><pgcode>28463952</pgcode><ticketprice>1450.0</ticketprice><policynum>5.00</policynum><totaltax>160.0</totaltax><payprice>1537.5</payprice><fc>-</fc></policy></policies></pnrinfo>
                //<pnrinfo><code>0</code><pnr>NJ9280|HDMP6V</pnr><air>CA</air><pnrsrcid></pnrsrcid><message>获取登录信息失败，请稍后再试</message></pnrinfo>
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                b2bResponse.LastCallMethod = "NewQueryPriceByPnr";
                b2bResponse.RetuenXML = result;
                b2bResponse.DsResult = XmlToDataSet(result);
                if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Contains("autopayinfo")
                    && b2bResponse.DsResult.Tables["autopayinfo"].Rows.Count > 0
                    && b2bResponse.DsResult.Tables["autopayinfo"].Columns.Contains("errorinfo")
                    )
                {
                    b2bResponse.Remark = b2bResponse.DsResult.Tables["autopayinfo"].Rows[0]["errorinfo"].ToString();
                }
                else
                {
                    b2bResponse.Remark = "[NewQueryPriceByPnr]:" + result;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("NewQueryPriceByPnr", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 编码入库
        /// </summary>
        /// <param name="autoEtdzParam"></param>
        /// <returns></returns>
        public B2BResponse Input(AutoEtdzParam autoEtdzParam)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=input");
                lstParam.Add("getticketovertime=5");
                lstParam.Add("bigpnr=1");
                lstParam.Add("fmt=xml");
                lstParam.Add("xmlhashead=false");
                lstParam.Add("autopayflag=1");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);//大编码
                lstParam.Add("pnrc=" + autoEtdzParam.Pnr);//小编码
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("payaccount=" + autoEtdzParam.PayInfo.PayAccount);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));
                lstParam.Add("callbackurl=" + autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl);
                lstParam.Add("callbackurlforpay=" + autoEtdzParam.UrlInfo.AlipayPayNotifyUrl);
                //代理人在平台投放的政策点数
                lstParam.Add("srcpolicynum=" + autoEtdzParam.OldPolicyPoint);
                //不为空 就校验 当相同时才能入库，否则不能入库  
                lstParam.Add("srctotaltax=" + autoEtdzParam.PayInfo.TaxTotalPrice);
                //校验支付价格
                //最低价-最高价：
                //如果格式为最低价-最高价（例如：0-500），当支付总价格不高于最高价时才支付，最高价为空或为0则不校验；
                //限定价格：如果格式为限定价格（例如：600），当支付价格等于此价格时才支付。
                lstParam.Add("checkprice=0-" + autoEtdzParam.PayInfo.PayTotalPrice);//限定支付总价为一范围或者某一固定值
                //pgid
                //pgcode                  
                //当总票面价相同时才能入库
                //用于代理人和航空公司总票面价对比，当总票面价相同时才能入库，否则不能入库。单位为（元）。
                if (autoEtdzParam.IsMulPrice && autoEtdzParam.IsLimitScope)
                {
                    lstParam.Add("srcticketsprice=" + autoEtdzParam.PayInfo.SeatTotalPrice);
                }
                //扩展参数
                Dictionary<string, string> exDic = autoEtdzParam.ExParam;
                foreach (KeyValuePair<string, string> item in exDic)
                {
                    if (autoEtdzParam.CarryCode.Trim().ToUpper() == "ZH"
                        )
                    {
                        if (item.Key == "zh_name")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                        if (item.Key == "zh_tel")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                        if (item.Key == "zh_mobile")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                    }
                    else if (autoEtdzParam.CarryCode.Trim().ToUpper() == "KY")
                    {
                        if (item.Key == "ky_name")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                        if (item.Key == "ky_tel")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                        if (item.Key == "ky_mobile")
                        {
                            lstParam.Add(string.Format("{0}={1}", item.Key, item.Value));
                        }
                    }
                }
                //请求地址
                string ReqUrl = string.Format("{0}?{1}",
                    autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                autoEtdzParam.RequestUrl = "[Input]:" + ReqUrl;
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                if (response.Status != HttpStatusCode.OK)
                {
                    //失败再次请求一次
                    response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                }
                string result = response.Data;
                //<pnrinfo><code>0</code><pnr>NTSJ5Z|HTFQFT</pnr><message>获取登录信息失败，请稍后再试</message><pnrsrcid>0140704101430012564</pnrsrcid></pnrinfo>
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                b2bResponse.LastCallMethod = "Input";
                b2bResponse.RetuenXML = result;
                b2bResponse.DsResult = XmlToDataSet(result);
                if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Contains("pnrinfo")
                    && b2bResponse.DsResult.Tables["pnrinfo"].Rows.Count > 0
                    && b2bResponse.DsResult.Tables["pnrinfo"].Columns.Contains("message")
                    )
                {
                    b2bResponse.Remark = b2bResponse.DsResult.Tables["pnrinfo"].Rows[0]["message"].ToString();
                }
                else
                {
                    b2bResponse.Remark = "[Input]:" + result;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("Input", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 自动支付
        /// </summary>
        /// <param name="autoEtdzParam"></param>
        /// <returns></returns>
        public B2BResponse Pay(AutoEtdzParam autoEtdzParam)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=pay");
                lstParam.Add("getticketovertime=5");
                lstParam.Add("bigpnr=1");
                lstParam.Add("fmt=xml");
                lstParam.Add("xmlhashead=false");
                lstParam.Add("autopayflag=1");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);
                lstParam.Add("pnrc=" + autoEtdzParam.Pnr);//小编码
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));
                lstParam.Add("payaccount=" + autoEtdzParam.PayInfo.PayAccount);
                lstParam.Add("callbackurl=" + autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl);
                lstParam.Add("callbackurlforpay=" + autoEtdzParam.UrlInfo.AlipayPayNotifyUrl);
                //添加参数
                if (autoEtdzParam.IsMulPrice && autoEtdzParam.IsLimitScope)
                {
                    //指定支付价格
                    lstParam.Add("checkprice=" + autoEtdzParam.PayInfo.SeatTotalPrice);
                }
                else
                {
                    //指定一个支付价格的范围 自动支付最低的价格
                    lstParam.Add("checkprice=0-" + autoEtdzParam.PayInfo.PayTotalPrice);
                }

                //请求地址
                string ReqUrl = string.Format("{0}?{1}", autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                autoEtdzParam.RequestUrl = "[Pay]:" + ReqUrl;
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                if (response.Status != HttpStatusCode.OK)
                {
                    //失败再次请求一次
                    response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                }
                string result = response.Data;
                //<pnrinfo><code>0</code><pnr>MGPSMH|JZD3GV</pnr><pnrsrcid>0140704151404814360</pnrsrcid><message>未取到支付信息,请稍后再试！</message><pay><orderid></orderid><payprice>930.0</payprice><tradeno></tradeno><outtradeno></outtradeno></pay></pnrinfo>
                //<pnrinfo><code>0</code><pnr>NH9PKQ|JPKHSX</pnr><pnrsrcid>0140704142006134063</pnrsrcid><message>当前订单状态为：完成购票(已出票) 不允许支付！</message><pay><orderid></orderid><payprice>3225.0</payprice><tradeno></tradeno><outtradeno></outtradeno></pay></pnrinfo>
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                b2bResponse.LastCallMethod = "Pay";
                b2bResponse.RetuenXML = result;
                b2bResponse.DsResult = XmlToDataSet(result);
                if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Contains("pnrinfo")
                    && b2bResponse.DsResult.Tables["pnrinfo"].Rows.Count > 0
                    && b2bResponse.DsResult.Tables["pnrinfo"].Columns.Contains("message")
                    )
                {
                    b2bResponse.Remark = b2bResponse.DsResult.Tables["pnrinfo"].Rows[0]["message"].ToString();
                }
                else
                {
                    b2bResponse.Remark = "[Pay]:" + result;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("Pay", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 出票(同步) 取票号
        /// </summary>     
        /// <returns></returns>
        public B2BResponse TicketOut(AutoEtdzParam autoEtdzParam)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=ticketout");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));
                lstParam.Add("bigpnr=1");
                lstParam.Add("fmt=xml");
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("xmlhashead=false");

                //请求地址
                string ReqUrl = string.Format("{0}?{1}", autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                autoEtdzParam.RequestUrl = "[Pay]:" + ReqUrl;
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                if (response.Status != HttpStatusCode.OK)
                {
                    //失败再次请求一次
                    response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                }
                string result = response.Data;
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                b2bResponse.LastCallMethod = "TicketOut";
                b2bResponse.RetuenXML = result;
                b2bResponse.DsResult = XmlToDataSet(result);
                if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Contains("pnrinfo")
                    && b2bResponse.DsResult.Tables["pnrinfo"].Rows.Count > 0
                    && b2bResponse.DsResult.Tables["pnrinfo"].Columns.Contains("message")
                    )
                {
                    if (b2bResponse.DsResult.Tables.Contains("ticket"))
                    {
                        b2bResponse.TicketNofityInfo = new TicketNofityInfo();
                        DataRowCollection drs = b2bResponse.DsResult.Tables["ticket"].Rows;
                        if (drs.Count > 0)
                        {
                            string strPassengerName = string.Empty;
                            string strTicketNo = string.Empty;
                            foreach (DataRow dr in drs)
                            {
                                strPassengerName = HttpUtility.UrlDecode(dr["passenger"].ToString().Trim(), Encoding.Default);
                                strTicketNo = dr["tktno"].ToString().Trim();
                                if (!string.IsNullOrEmpty(strTicketNo.Replace("-", "")))
                                {
                                    AutoTicketInfo autoTicketInfo = new AutoTicketInfo();
                                    autoTicketInfo.PassengerName = strPassengerName;
                                    autoTicketInfo.TicketNumber = strTicketNo;
                                    b2bResponse.TicketNofityInfo.AutoTicketList.Add(autoTicketInfo);
                                }
                            }
                            b2bResponse.Status = true;
                        }
                    }
                    else
                    {
                        b2bResponse.Remark = b2bResponse.DsResult.Tables["pnrinfo"].Rows[0]["message"].ToString();
                    }
                }
                else
                {
                    b2bResponse.Remark = "[TicketOut]:" + result;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("TicketOut", sbLog.ToString());
            }
            return b2bResponse;
        }
        /// <summary>
        /// 出票(异步)
        /// </summary>
        /// <param name="autoEtdzParam"></param>
        /// <returns></returns>
        public B2BResponse TicketoutCallback(AutoEtdzParam autoEtdzParam)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                strErrMsg = ValdateParam(autoEtdzParam, strErrMsg);
                List<string> lstParam = new List<string>();
                lstParam.Add("cmd=ticketoutcallback");
                lstParam.Add("pnr=" + autoEtdzParam.BigPnr);
                lstParam.Add("b2buser=" + HttpUtility.UrlEncode(autoEtdzParam.B2BAccount));
                lstParam.Add("b2bpwd=" + HttpUtility.UrlEncode(autoEtdzParam.B2BPwd));
                lstParam.Add("bigpnr=1");
                lstParam.Add("fmt=xml");
                lstParam.Add("air=" + autoEtdzParam.CarryCode);
                lstParam.Add("pnrsrcid=" + autoEtdzParam.FlatformOrderId);
                lstParam.Add("xmlhashead=false");
                lstParam.Add("callbackurl=" + autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl);

                //请求地址
                string ReqUrl = string.Format("{0}?{1}", autoEtdzParam.UrlInfo.AlipayAutoCPUrl, string.Join("&", lstParam.ToArray()));
                autoEtdzParam.RequestUrl = "[TicketoutCallback]:" + ReqUrl;
                sbLog.Append("请求URL:" + ReqUrl + "\r\n");
                WebHttp webHttp = new WebHttp();
                DataResponse response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                if (response.Status != HttpStatusCode.OK)
                {
                    //失败再次请求一次
                    response = webHttp.SendRequest(ReqUrl, MethodType.GET, Encoding.Default, 60);
                }
                string result = response.Data;
                //<pnrinfo><code>1</code><pnr>MXC5J4</pnr><air>CZ</air><pnrsrcid>0140313152408060555</pnrsrcid><message>出票处理中</message></pnrinfo>
                sbLog.Append("返回结果:\r\n" + result + "\r\n");
                b2bResponse.LastCallMethod = "TicketoutCallback";
                b2bResponse.RetuenXML = result;
                b2bResponse.DsResult = XmlToDataSet(result);
                if (b2bResponse.DsResult != null && b2bResponse.DsResult.Tables.Count > 0
                    && b2bResponse.DsResult.Tables[0].Rows.Count > 0
                    && b2bResponse.DsResult.Tables[0].Columns.Contains("message")
                    )
                {
                    b2bResponse.Remark = b2bResponse.DsResult.Tables[0].Rows[0]["message"].ToString();
                }
                else
                {
                    b2bResponse.Remark = "[TicketoutCallback]:" + result;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("TicketoutCallback", sbLog.ToString());
            }
            return b2bResponse;
        }


        /// <summary>
        /// 出票回调处理 本票通异步发送的出票信息字符串 ticketXML= Request.Form["ticketnoinfo"] 
        /// </summary>
        /// <param name="ticketXML"></param>
        /// <returns></returns>
        public B2BResponse SyncTicketCall(string ticketXML)
        {
            B2BResponse b2bResponse = new B2BResponse();
            TicketNofityInfo ticketNofityInfo = b2bResponse.TicketNofityInfo;
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                //解码
                ticketXML = HttpUtility.UrlDecode(ticketXML, Encoding.GetEncoding("gb2312"));
                sbLog.AppendFormat("ticketXML={0}\r\n", ticketXML);
                b2bResponse.RetuenXML = ticketXML;
                b2bResponse.DsResult = XmlToDataSet(ticketXML);
                if (b2bResponse.DsResult != null
                    && b2bResponse.DsResult.Tables.Count > 0
                    && b2bResponse.DsResult.Tables[0].Rows.Count > 0
                     && b2bResponse.DsResult.Tables.Contains("tickets")
                    && b2bResponse.DsResult.Tables.Contains("ticket")
                    )
                {
                    if (b2bResponse.DsResult.Tables.Contains("orderinfo"))
                    {
                        //订单信息
                        DataRow dr_orderinfo = b2bResponse.DsResult.Tables["orderinfo"].Rows[0];
                        ticketNofityInfo.Code = dr_orderinfo["code"].ToString() == "1" ? true : false;
                        ticketNofityInfo.FlatformOrderId = dr_orderinfo["pnrsrcid"].ToString();
                        ticketNofityInfo.OrderNo = dr_orderinfo["orderno"].ToString();
                        ticketNofityInfo.TradeNo = dr_orderinfo["tradeno"].ToString();
                        ticketNofityInfo.OrderStatus = dr_orderinfo["orderstatus"].ToString();
                        ticketNofityInfo.PayStatus = dr_orderinfo["paystatus"].ToString();
                        ticketNofityInfo.Message = dr_orderinfo["message"].ToString();
                        ticketNofityInfo.Pnr = dr_orderinfo["pnr"].ToString();
                        decimal PayPrice = 0m;
                        decimal.TryParse(dr_orderinfo["payprice"].ToString(), out PayPrice);
                        ticketNofityInfo.PayPrice = PayPrice;
                    }
                    if (b2bResponse.DsResult.Tables.Contains("pnrinfo"))
                    {
                        DataRow dr_pnrinfo = b2bResponse.DsResult.Tables["pnrinfo"].Rows[0];
                        ticketNofityInfo.Code = dr_pnrinfo["code"].ToString() == "1" ? true : false;
                        ticketNofityInfo.Pnr = dr_pnrinfo["pnr"].ToString();
                        ticketNofityInfo.FlatformOrderId = dr_pnrinfo["pnrsrcid"].ToString();
                        ticketNofityInfo.Message = dr_pnrinfo["message"].ToString();
                    }
                    //出票信息
                    if (b2bResponse.DsResult.Tables["ticket"].Rows.Count > 0)
                    {
                        DataRowCollection drs = b2bResponse.DsResult.Tables["ticket"].Rows;
                        string strPassengerName = string.Empty;
                        string strTicketNo = string.Empty;
                        foreach (DataRow dr in drs)
                        {
                            strPassengerName = HttpUtility.UrlDecode(dr["passenger"].ToString().Trim(), Encoding.Default);
                            strTicketNo = dr["tktno"].ToString().Trim();
                            if (!string.IsNullOrEmpty(strTicketNo.Replace("-", "")))
                            {
                                AutoTicketInfo autoTicketInfo = new AutoTicketInfo();
                                autoTicketInfo.PassengerName = strPassengerName;
                                autoTicketInfo.TicketNumber = strTicketNo;
                                ticketNofityInfo.AutoTicketList.Add(autoTicketInfo);
                            }
                        }
                        b2bResponse.Status = true;
                    }
                    b2bResponse.Remark = ticketNofityInfo.Message;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("SyncTicketCall", sbLog.ToString());
            }
            return b2bResponse;
        }

        /// <summary>
        /// 支付回调处理    payreturnXML= Request.Form["paymentinfo"] 
        /// </summary>
        /// <returns></returns>
        public B2BResponse SyncPayCall(string payXML)
        {
            B2BResponse b2bResponse = new B2BResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            string strErrMsg = string.Empty;
            try
            {
                //解码
                payXML = HttpUtility.UrlDecode(payXML, Encoding.GetEncoding("gb2312"));
                sbLog.AppendFormat("payXML={0}\r\n", payXML);
                b2bResponse.RetuenXML = payXML;
                b2bResponse.DsResult = XmlToDataSet(payXML);
                if (b2bResponse.DsResult != null
                    && b2bResponse.DsResult.Tables.Count > 0
                    && b2bResponse.DsResult.Tables[0].Rows.Count > 0
                     && b2bResponse.DsResult.Tables.Contains("orderinfo")
                    )
                {
                    b2bResponse.PayNotifyInfo = new PayNotifyInfo();
                    DataRow dr_orderinfo = b2bResponse.DsResult.Tables["orderinfo"].Rows[0];

                    b2bResponse.PayNotifyInfo.Code = dr_orderinfo["code"].ToString() == "1" ? true : false;
                    b2bResponse.PayNotifyInfo.Pnr = dr_orderinfo["pnr"].ToString();
                    b2bResponse.PayNotifyInfo.OrderStatus = dr_orderinfo["orderstatus"].ToString();
                    b2bResponse.PayNotifyInfo.PayStatus = dr_orderinfo["paystatus"].ToString();
                    decimal PayPrice = 0m;
                    decimal.TryParse(dr_orderinfo["payprice"].ToString(), out PayPrice);
                    b2bResponse.PayNotifyInfo.PayPrice = PayPrice;
                    b2bResponse.PayNotifyInfo.FlatformOrderId = dr_orderinfo["pnrsrcid"].ToString();
                    b2bResponse.PayNotifyInfo.Message = dr_orderinfo["message"].ToString();
                    b2bResponse.PayNotifyInfo.OutTradeNo = dr_orderinfo["outtradeno"].ToString();
                    b2bResponse.PayNotifyInfo.TradeNo = dr_orderinfo["tradeno"].ToString();

                    b2bResponse.Status = true;
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                //记录日志
                new CommLog().WriteLog("SyncPayCall", sbLog.ToString());
            }
            return b2bResponse;
        }


        /// <summary>
        /// BSP自动出票
        /// </summary>
        /// <param name="bspParam"></param>
        /// <returns></returns>
        public BSPResponse BSPAutoIssue(BSPParam bspParam)
        {
            SendNewPID pid = new SendNewPID();
            return pid.BSPAutoIssue(bspParam);
        }




        /// <summary>
        /// 将本票通返回的XML转换成数据集
        /// </summary>       
        /// <returns></returns>
        private DataSet XmlToDataSet(string xmlContent)
        {
            DataSet ds = new DataSet();
            try
            {
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    //去掉不可见字符                        
                    xmlContent = format.RemoveHideChar(xmlContent);
                    if (!xmlContent.Trim().ToLower().StartsWith("<?xml"))
                    {
                        xmlContent = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + xmlContent;
                    }
                    StringReader rea = new StringReader(xmlContent);
                    XmlTextReader xmlReader = new XmlTextReader(rea);
                    ds.ReadXml(xmlReader);
                }
            }
            catch
            {
            }
            return ds;
        }

        private static string ValdateParam(AutoEtdzParam autoEtdzParam, string strErrMsg)
        {
            if (string.IsNullOrEmpty(autoEtdzParam.BigPnr)
             || string.IsNullOrEmpty(autoEtdzParam.Pnr))
            {
                strErrMsg = "编码或者大编码不能为空！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.CarryCode)
              || autoEtdzParam.CarryCode.Trim().Length != 2)
            {
                strErrMsg = "航空公司二字码不能为空！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.FlatformOrderId))
            {
                strErrMsg = "订单号不能为空！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.B2BAccount))
            {
                strErrMsg = "航空公司B2B网站登陆账号不能为空！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.B2BPwd))
            {
                strErrMsg = "航空公司B2B网站登陆密码不能为空！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.PayInfo.PayAccount))
            {
                strErrMsg = "航空公司B2B网站没有设置自动支付账号！";
            }
            else if (autoEtdzParam.PayInfo.PayTotalPrice <= 0)
            {
                strErrMsg = "支付总价格错误,不能小于0";
            }
            else if (autoEtdzParam.PayInfo.SeatTotalPrice <= 0)
            {
                strErrMsg = "票面总价格错误,不能小于0";
            }
            else if (autoEtdzParam.PayInfo.TaxTotalPrice <= 0)
            {
                strErrMsg = "税费总价格错误,不能小于0";
            }
            else if (autoEtdzParam.OldPolicyPoint <= 0 || autoEtdzParam.OldPolicyPoint > 100)
            {
                strErrMsg = "代理人在平台投放的政策点数错误！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.UrlInfo.AlipayAutoCPUrl))
            {
                strErrMsg = "请求本票通的地址不能为空！";
            }
            else if (string.IsNullOrEmpty(autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl)
                || !autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl.ToLower().StartsWith("http")
                )
            {
                strErrMsg = "本票通出票通知地址不能为空！";
            }
            return strErrMsg;
        }

        /// <summary>
        /// 获取航空公司订单状态（已本票通为准）
        /// </summary>
        /// <returns></returns>
        private static string GetOrderStatus(string status)
        {
            Dictionary<string, string> dicOrderStatus = new Dictionary<string, string>();
            dicOrderStatus.Add("0", "未出票");
            dicOrderStatus.Add("1", "出票完成");
            dicOrderStatus.Add("3", "等待支付");
            dicOrderStatus.Add("4", "等待出票");
            dicOrderStatus.Add("6", "差错退款");
            dicOrderStatus.Add("7", "取消入库");
            dicOrderStatus.Add("9", "状态未知");
            dicOrderStatus.Add("11", "订单作废");
            dicOrderStatus.Add("12", "议价申请中");
            var result = dicOrderStatus.Where(p => p.Key == status).FirstOrDefault();
            string reData = status;
            if (result.Value != null)
            {
                reData = result.Value;
            }
            return reData;
        }
        /// <summary>
        /// 获取支付状态（已本票通为准）
        /// </summary>
        /// <returns></returns>
        private static string GetPayStatus(string payStatus)
        {
            Dictionary<string, string> dicPayStatus = new Dictionary<string, string>();
            dicPayStatus.Add("0", "支付失败");
            dicPayStatus.Add("1", "支付成功");
            dicPayStatus.Add("9", "异常情况");
            var result = dicPayStatus.Where(p => p.Key == payStatus).FirstOrDefault();
            string reData = payStatus;
            if (result.Value != null)
            {
                reData = result.Value;
            }
            return reData;
        }
    }
}
