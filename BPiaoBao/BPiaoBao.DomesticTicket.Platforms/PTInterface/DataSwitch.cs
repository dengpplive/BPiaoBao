using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using PnrAnalysis.Model;

namespace BPiaoBao.DomesticTicket.Platforms.PTInterface
{
    public class DataSwitch
    {
        #region 公共方法
        /// <summary>
        /// DataSet装换到字符串显示 自定义显示格式
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string DataSetToString(DataSet ds)
        {
            StringBuilder sbLog = new StringBuilder();
            if (ds != null && ds.Tables.Count > 0)
            {
                List<string> PropertyList = new List<string>();
                foreach (DataTable table in ds.Tables)
                {
                    DataTableToString(PropertyList, table, "\r\n");
                }
                if (PropertyList.Count > 0)
                {
                    sbLog.Append(string.Join("\r\n", PropertyList.ToArray()));
                }
            }
            return sbLog.ToString();
        }
        public string DataTableToString(DataTable table, string SplitChar)
        {
            StringBuilder sbRowData = new StringBuilder();
            if (table != null && table.Rows.Count > 0)
            {
                DataRowCollection rcs = table.Rows;
                DataColumnCollection dcs = table.Columns;
                foreach (DataRow dr in rcs)
                {
                    object obj = null;
                    foreach (DataColumn dc in dcs)
                    {
                        obj = dr[dc.ColumnName] == DBNull.Value ? "" : dr[dc.ColumnName].ToString();
                        sbRowData.Append(obj + SplitChar);
                    }
                }
            }
            return sbRowData.ToString();
        }
        private void DataTableToString(List<string> PropertyList, DataTable table, string SplitChar)
        {
            if (table != null && table.Rows.Count > 0)
            {
                DataRowCollection rcs = table.Rows;
                DataColumnCollection dcs = table.Columns;
                int i = 0;
                foreach (DataRow dr in rcs)
                {
                    i++;
                    PropertyList.Add("表名:" + table.TableName + "" + "第" + i + "行数据:");
                    StringBuilder sbRowData = new StringBuilder();
                    object obj = null;
                    foreach (DataColumn dc in dcs)
                    {
                        obj = dr[dc.ColumnName] == DBNull.Value ? "null" : dr[dc.ColumnName].ToString();
                        sbRowData.Append(dc.ColumnName + "=" + obj + SplitChar);
                    }
                    PropertyList.Add(sbRowData.ToString());
                }
            }
        }
        /// <summary>
        ///  DataSet装换到字符串显示 返回XML格式
        /// </summary>
        /// <returns></returns>
        public string DataSetToXml(DataSet ds)
        {
            StringBuilder sbLog = new StringBuilder();
            if (ds != null && ds.Tables.Count > 0)
            {
                MemoryStream ms = null;
                XmlTextWriter XmlWt = null;
                try
                {
                    ms = new MemoryStream();
                    //根据ms实例化XmlWt
                    XmlWt = new XmlTextWriter(ms, Encoding.Default);
                    //获取ds中的数据
                    ds.WriteXml(XmlWt);
                    int count = (int)ms.Length;
                    byte[] temp = new byte[count];
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Read(temp, 0, count);
                    string returnValue = Encoding.Default.GetString(temp).Trim();
                    sbLog.Append(returnValue);
                }
                catch (System.Exception ex)
                {
                }
                finally
                {
                    //释放资源
                    if (XmlWt != null)
                    {
                        XmlWt.Close();
                        ms.Close();
                        ms.Dispose();
                    }
                }

            }
            return sbLog.ToString();
        }
        /// <summary>
        /// xml转换到DataSet
        /// </summary>
        /// <param name="Content"></param>
        /// <returns></returns>
        public DataSet XmlToDataSet(string Content)
        {
            DataSet ds = new DataSet();
            try
            {
                if (Content != "")
                {
                    if (!Content.Trim().ToLower().StartsWith("<?xml"))
                    {
                        Content = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Content;
                    }
                    StringReader rea = new StringReader(Content);
                    XmlTextReader xmlReader = new XmlTextReader(rea);
                    ds.ReadXml(xmlReader);
                }
            }
            catch (Exception)
            {
            }
            return ds;
        }
        /// <summary>
        /// 获取客户端请求IP地址 即浏览器IP
        /// </summary>
        /// <returns></returns>
        public string getIP()
        {
            string ip = "";
            try
            {
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
            }
            catch
            {
            }
            return ip;
        }
        #endregion

        #region 百拓方法
        /// <summary>
        /// 百拓接口参数
        /// </summary>
        public string PseudoCityCode = "DFW";
        public string ISOCountry = "US";
        public string ISOCurrency = "USD";
        public string AirlineVendorID = "AA";
        public string AirportCode = "IDA";
        public string Url = "http://provider1.org/OTAEngine";
        /// <summary>
        /// 百拓实时获取政策
        /// </summary>
        /// <param name="_baiTuoAccout">账号</param>
        /// <param name="_baiTuoPassword">密码</param>
        /// <param name="_baiTuoAg">密钥</param>
        /// <param name="DepartureDateTime">去程出发时间 如：2014-09-30T07:05:00 </param>
        /// <param name="FlightNumber">去程航班号 如：ZH9793</param>
        /// <param name="ResBookDesigCode">去程舱位</param>
        /// <param name="DepartureAirport">出发城市三字码</param>
        /// <param name="ArrivalAirport">到达城市三字码</param>
        /// <param name="TripType">行程类型：1，单程 2，往返</param>
        /// <param name="FlightNumberBack">回程航班号 如：CZ8010</param>
        /// <param name="ResBookDesigCodeBack">回程舱位</param>
        /// <param name="DepartureDateTimeBack">回程出发时间 如：2014-09-30T07:05:00</param>
        /// <returns>返回处理好的XML节点</returns>
        public XmlElement BaiTuoSSPolicySend(string _baiTuoAccout, string _baiTuoPassword, string _baiTuoAg, string DepartureDateTime, string FlightNumber, string ResBookDesigCode, string DepartureAirport, string ArrivalAirport, string TripType, string FlightNumberBack, string ResBookDesigCodeBack, string DepartureDateTimeBack)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe3 = doc.CreateElement("OTA_AirFareRQ");
            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", "DFW");
                xe2.SetAttribute("ISOCountry", "US");
                xe2.SetAttribute("ISOCurrency", "USD");
                xe2.SetAttribute("AirlineVendorID", "AA");
                xe2.SetAttribute("AirportCode", "IAD");
                XmlElement xe4 = doc.CreateElement("RequestorID");
                xe4.SetAttribute("URL", Url);
                xe4.SetAttribute("Type", "6");
                xe4.SetAttribute("ID", _baiTuoPassword);
                xe2.AppendChild(xe4);
                xe1.AppendChild(xe2);
                XmlElement xe5 = doc.CreateElement("FlightSegmentFares");
                XmlElement xe6 = doc.CreateElement("FlightSegment");
                xe6.SetAttribute("DepartureDateTime", ConvertDateTime(DepartureDateTime.Replace(' ', 'T')));
                xe6.SetAttribute("FlightNumber", FlightNumber);
                xe6.SetAttribute("ResBookDesigCode", ResBookDesigCode);
                xe6.SetAttribute("DepartureAirport", DepartureAirport);
                xe6.SetAttribute("ArrivalAirport", ArrivalAirport);
                xe6.SetAttribute("ReturnPolicyType", "2");
                if (TripType != "1")
                {
                    xe6.SetAttribute("TripType", TripType);
                    xe6.SetAttribute("FlightNumberBack", FlightNumberBack);
                    xe6.SetAttribute("ResBookDesigCodeBack", ResBookDesigCodeBack);
                    xe6.SetAttribute("DepartureDateTimeBack", ConvertDateTime(DepartureDateTimeBack.Replace(' ', 'T')));
                }
                xe5.AppendChild(xe6);
                xe3.AppendChild(xe1);
                xe3.AppendChild(xe5);
                xe3.InnerXml = "<OTA_AirFareRQ>" + xe3.InnerXml + "</OTA_AirFareRQ>";
            }
            catch
            {
                return xe3;
            }
            return xe3;
        }


        /// <summary>
        /// 百拓订单生成传入参数XML配置
        /// </summary>
        /// <param name="values">数组对象1:政策编号2:行程类型 1单程 2往返3:是否有保险 0没有 1有4:本地订单编号5:出发时间6:到达时间7:航班号8:机型9:出发城市三字码10:到达城市三字码11:舱位12:票面价13:结算价14:基建费15:燃油费16:联系人17：联系电话</param>
        /// <param name="dt">乘机人信息DatatTable集合</param>
        /// <returns>返回拼接好的XMLElement对象</returns>
        public XmlElement BaiTuoCreateOrderSend(string _baiTuoAccout, string _baiTuoPassword, string _baiTuoAg, string[] values, DataTable dt)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("POS");
            XmlElement xe2 = doc.CreateElement("Source");
            XmlElement xe14 = doc.CreateElement("ORDER_CREATE_RQ");
            try
            {
                //在节点中添加属性与对应的值
                xe2.SetAttribute("AgentSine", _baiTuoAg);
                xe2.SetAttribute("AgentUserName", _baiTuoAccout);
                xe2.SetAttribute("PseudoCityCode", "2U8");
                xe2.SetAttribute("ISOCountry", "CN");
                xe2.SetAttribute("ISOCurrency", "USD");
                xe2.SetAttribute("AirlineVendorID", "AA");
                xe2.SetAttribute("AirportCode", "IAD");
                XmlElement xe3 = doc.CreateElement("RequestorID");
                xe3.SetAttribute("URL", Url);
                xe3.SetAttribute("Type", "4");
                xe3.SetAttribute("ID", _baiTuoPassword);

                XmlElement xe4 = doc.CreateElement("OrderInfo");
                xe4.SetAttribute("PolicyId", values[0]);
                xe4.SetAttribute("InternationalTicket", "0");
                xe4.SetAttribute("TripTypeCode", values[1]);
                xe4.SetAttribute("OrderSrc", "1");
                xe4.SetAttribute("InsuranceType", values[2]);
                xe4.SetAttribute("LocalOrderID", values[3]);

                XmlElement xe5 = doc.CreateElement("FlightSegments");
                if (values[4].Split('#').Length == 2)
                {
                    if (values[8].Split('/')[1].ToString() != "")
                    {
                        for (int x = 0; x < values[8].Split('/').Length; x++)
                        {
                            XmlElement xe6 = doc.CreateElement("FlightInfo");
                            xe6.SetAttribute("DepartureDatetime", ConvertDateTime(values[4].Split('#')[x].ToString()));
                            xe6.SetAttribute("ArrivalDatetime", ConvertDateTime(values[5].Split('#')[x].ToString()));
                            xe6.SetAttribute("FlightNumber", values[6].Split('/')[x].ToString());
                            xe6.SetAttribute("PlaneStyle", values[7].Split('/')[x].ToString());

                            XmlElement xe7 = doc.CreateElement("DepartureAirport");
                            xe7.SetAttribute("LocationCode", values[8].Split('/')[x].ToString());
                            XmlElement xe8 = doc.CreateElement("ArrivalAirport");
                            xe8.SetAttribute("LocationCode", values[9].Split('/')[x].ToString());
                            XmlElement xe9 = doc.CreateElement("Cabin");
                            xe9.SetAttribute("Code", values[10].Split('/')[x].ToString());
                            xe9.SetAttribute("Price", Convert.ToString(values[11].Split('/')[x].ToString().Split('.')[0].ToString()));
                            xe9.SetAttribute("AgentPrice", Convert.ToString(Math.Round(double.Parse(values[12].Split('/')[x].ToString()), 2)));
                            xe9.SetAttribute("Tax", Convert.ToString(values[13].Split('/')[x].ToString().Split('.')[0].ToString()));
                            xe9.SetAttribute("YQTax", Convert.ToString(values[14].Split('/')[x].ToString().Split('.')[0].ToString()));
                            xe9.SetAttribute("SubCode", "");
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                XmlElement xe10 = doc.CreateElement("RPH");
                                xe10.SetAttribute("personName", dt.Rows[i]["PassengerName"].ToString());
                                xe10.SetAttribute("PNR", dt.Rows[i]["PNR"].ToString());
                                xe10.SetAttribute("Insurance", dt.Rows[i]["Insurance"].ToString());
                                xe10.SetAttribute("BigPNR", values[17].ToString());
                                xe9.AppendChild(xe10);
                            }
                            xe6.AppendChild(xe7);
                            xe6.AppendChild(xe8);
                            xe6.AppendChild(xe9);
                            xe5.AppendChild(xe6);
                        }
                    }
                    else
                    {
                        XmlElement xe6 = doc.CreateElement("FlightInfo");
                        xe6.SetAttribute("DepartureDatetime", ConvertDateTime(values[4].Split('#')[0].ToString()));
                        xe6.SetAttribute("ArrivalDatetime", ConvertDateTime(values[5].Split('#')[0].ToString()));
                        xe6.SetAttribute("FlightNumber", values[6].Split('/')[0].ToString());
                        xe6.SetAttribute("PlaneStyle", values[7].Split('/')[0].ToString());

                        XmlElement xe7 = doc.CreateElement("DepartureAirport");
                        xe7.SetAttribute("LocationCode", values[8].Split('/')[0].ToString());
                        XmlElement xe8 = doc.CreateElement("ArrivalAirport");
                        xe8.SetAttribute("LocationCode", values[9].Split('/')[0].ToString());
                        XmlElement xe9 = doc.CreateElement("Cabin");
                        xe9.SetAttribute("Code", values[10].Split('/')[0].ToString());
                        xe9.SetAttribute("Price", Convert.ToString(values[11].Split('/')[0].ToString().Split('.')[0].ToString()));
                        xe9.SetAttribute("AgentPrice", Convert.ToString(Math.Round(double.Parse(values[12].Split('/')[0].ToString()), 2)));
                        xe9.SetAttribute("Tax", Convert.ToString(values[13].Split('/')[0].ToString().Split('.')[0].ToString()));
                        xe9.SetAttribute("YQTax", Convert.ToString(values[14].Split('/')[0].ToString().Split('.')[0].ToString()));
                        xe9.SetAttribute("SubCode", "");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            XmlElement xe10 = doc.CreateElement("RPH");
                            xe10.SetAttribute("personName", dt.Rows[i]["PassengerName"].ToString());
                            xe10.SetAttribute("PNR", dt.Rows[i]["PNR"].ToString());
                            xe10.SetAttribute("Insurance", "0");
                            xe10.SetAttribute("BigPNR", values[17].ToString());
                            xe9.AppendChild(xe10);
                        }
                        xe6.AppendChild(xe7);
                        xe6.AppendChild(xe8);
                        xe6.AppendChild(xe9);
                        xe5.AppendChild(xe6);
                    }
                }
                else
                {
                    XmlElement xe6 = doc.CreateElement("FlightInfo");
                    xe6.SetAttribute("DepartureDatetime", ConvertDateTime(values[4].Split('#')[0].ToString()));
                    xe6.SetAttribute("ArrivalDatetime", ConvertDateTime(values[5].Split('#')[0].ToString()));
                    xe6.SetAttribute("FlightNumber", values[6].Split('/')[0].ToString());
                    xe6.SetAttribute("PlaneStyle", values[7].Split('/')[0].ToString());

                    XmlElement xe7 = doc.CreateElement("DepartureAirport");
                    xe7.SetAttribute("LocationCode", values[8].Split('/')[0].ToString());
                    XmlElement xe8 = doc.CreateElement("ArrivalAirport");
                    xe8.SetAttribute("LocationCode", values[9].Split('/')[0].ToString());
                    XmlElement xe9 = doc.CreateElement("Cabin");
                    xe9.SetAttribute("Code", values[10].Split('/')[0].ToString());
                    xe9.SetAttribute("Price", Convert.ToString(values[11].Split('/')[0].ToString().Split('.')[0].ToString()));
                    string AgentPrice = Convert.ToString(Math.Round(double.Parse(values[12].Split('/')[0].ToString()), 2));
                    if (AgentPrice.Split('.').Length < 2)
                    {
                        xe9.SetAttribute("AgentPrice", Convert.ToString(AgentPrice + ".00"));
                    }
                    else
                    {
                        if (AgentPrice.Split('.')[1].Length == 1)
                        {
                            AgentPrice = AgentPrice + "0";
                        }
                        xe9.SetAttribute("AgentPrice", AgentPrice);
                    }
                    xe9.SetAttribute("Tax", Convert.ToString(values[13].Split('/')[0].ToString().Split('.')[0].ToString()));
                    xe9.SetAttribute("YQTax", Convert.ToString(values[14].Split('/')[0].ToString().Split('.')[0].ToString()));
                    xe9.SetAttribute("SubCode", "");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        XmlElement xe10 = doc.CreateElement("RPH");
                        xe10.SetAttribute("personName", dt.Rows[i]["PassengerName"].ToString());
                        xe10.SetAttribute("PNR", dt.Rows[i]["PNR"].ToString());
                        xe10.SetAttribute("Insurance", "0");
                        xe10.SetAttribute("BigPNR", values[17].ToString());
                        xe9.AppendChild(xe10);
                    }
                    xe6.AppendChild(xe7);
                    xe6.AppendChild(xe8);
                    xe6.AppendChild(xe9);
                    xe5.AppendChild(xe6);
                }
                XmlElement xe11 = doc.CreateElement("TravelerInfo");
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    XmlElement xe12 = doc.CreateElement("personInfo");
                    XmlElement xe20 = doc.CreateElement("personName");
                    XmlElement xe21 = doc.CreateElement("personType");
                    XmlElement xe22 = doc.CreateElement("CertifyID");
                    XmlElement xe23 = doc.CreateElement("IdNumber");
                    XmlElement xe24 = doc.CreateElement("NationalityId");
                    XmlElement xe25 = doc.CreateElement("Gender");
                    XmlElement xe26 = doc.CreateElement("Birthday");
                    xe20.InnerText = dt.Rows[j]["PassengerName"].ToString();
                    xe21.InnerText = dt.Rows[j]["PassengerType"].ToString();
                    xe22.InnerText = dt.Rows[j]["CType"].ToString();
                    xe23.InnerText = dt.Rows[j]["Cid"].ToString();
                    xe24.InnerText = dt.Rows[j]["NationalityId"].ToString();
                    xe25.InnerText = dt.Rows[j]["Gender"].ToString();
                    xe26.InnerText = dt.Rows[j]["Birthday"].ToString();
                    xe12.AppendChild(xe20);
                    xe12.AppendChild(xe21);
                    xe12.AppendChild(xe22);
                    xe12.AppendChild(xe23);
                    xe12.AppendChild(xe24);
                    xe12.AppendChild(xe25);
                    xe12.AppendChild(xe26);
                    xe11.AppendChild(xe12);
                }
                XmlElement xe13 = doc.CreateElement("Remark");
                xe13.SetAttribute("SysRemark", "合作伙伴接口政策实时预定！");
                XmlElement xe15 = doc.CreateElement("LinkmanInfo");
                xe15.SetAttribute("Name", values[15].ToString());
                xe15.SetAttribute("MobilePhone", values[16].ToString());
                xe2.AppendChild(xe3);
                xe1.AppendChild(xe2);
                xe14.AppendChild(xe1);
                xe4.AppendChild(xe5);
                xe4.AppendChild(xe11);
                xe4.AppendChild(xe15);
                xe4.AppendChild(xe13);
                xe14.AppendChild(xe4);
            }
            catch
            {
                return xe14;
            }
            return xe14;
        }
        /// <summary>
        /// 构造乘客信息
        /// </summary>
        /// <param name="SsrCardType"></param>
        /// <param name="pnrMode"></param>
        /// <returns></returns>
        public DataTable StructPassenger(string SsrCardType, PnrAnalysis.PnrModel pnrMode)
        {
            //实例化创建好的DataTable对象
            DataTable dt = CreateDataTable();

            //循环将乘机人信息赋值到构造好的空DataTable对象中
            foreach (PassengerInfo Passenger in pnrMode._PassengerList)
            {
                DataRow dr = dt.NewRow();
                //乘客姓名
                dr["PassengerName"] = Passenger.PassengerName;
                //乘客PNR
                dr["PNR"] = pnrMode.Pnr;
                //乘客类型
                dr["PassengerType"] = Passenger.PassengerType;
                //证件类型
                dr["CType"] = ReturnPassengerCType(SsrCardType);
                //证件号码
                dr["Cid"] = Passenger.SsrCardID;
                //国籍
                dr["NationalityId"] = "45";
                //性别
                dr["Gender"] = "0";
                //出生日期
                dr["Birthday"] = "1999-1-1";
                //购买保险标识
                dr["Insurance"] = "0";

                //添加到构造好的DataTable中
                dt.Rows.Add(dr);
            }

            //返回DataTable
            return dt;
        }
        /// <summary>
        /// 返回乘客的证件类型在百拓中相对应的证件类型
        /// </summary>
        /// <param name="Type">乘机人证件类型编号</param>
        /// <returns>返回处理后的证件类型编号</returns>
        private string ReturnPassengerCType(string Type)
        {
            string Message = "0";
            switch (Type)
            {
                case "1":
                    Message = "0";
                    break;
                case "2":
                    Message = "3";
                    break;
            }
            return Message;
        }
        /// <summary>
        /// 创建百拓机票生成乘机人信息DataTable集合
        /// </summary>
        /// <returns>返回创建好的DataTable集合</returns>
        public DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            //乘机人姓名
            dt.Columns.Add("PassengerName");
            //PNR
            dt.Columns.Add("PNR");
            //乘客类型
            dt.Columns.Add("PassengerType");
            //证件类型
            dt.Columns.Add("CType");
            //证件号码
            dt.Columns.Add("Cid");
            //国籍
            dt.Columns.Add("NationalityId");
            //性别
            dt.Columns.Add("Gender");
            //出生日期
            dt.Columns.Add("Birthday");
            //购买保险标识
            dt.Columns.Add("Insurance");

            return dt;
        }
        /// <summary>
        /// 根据订单编号构建数据对象兵赋值
        /// </summary>
        /// <param name="OrderId">订单编号</param>
        /// <returns>返回构建与赋值好的string类型数组对象</returns>
        public string[] StructOrder(string PolicyId, string LocalOrderId, decimal SpacePrice, decimal ABFee, decimal FuelFee, decimal PolicyPoint, string LinkMan, string LinkManPhone, PnrAnalysis.PnrModel pnrModel)
        {
            //声明数组对象参数信息
            string[] OrderArray = new string[18];
            //构造数组中参数的值
            OrderArray[0] = PolicyId;
            OrderArray[1] = pnrModel.TravelType.ToString();
            OrderArray[2] = "0";
            OrderArray[3] = LocalOrderId;
            #region 行程参数声明并赋值

            //起飞时间
            string FromTime = "";
            //到达时间
            string ToTime = "";
            //起飞城市三字码
            string FromCityCode = "";
            //到达城市三字码
            string ToCityCode = "";
            //航班号
            string FlightCode = "";
            //机型
            string AircraftModel = "";
            //舱位
            string Space = "";
            //销售价
            string PMFee = "";
            //本地结算价
            string JSFee = "";
            //基建费
            string strABFee = "";
            //燃油费
            string strFuelFee = "";
            foreach (PnrAnalysis.Model.LegInfo SkyWay in pnrModel._LegList)
            {
                FromTime = FromTime + SkyWay.FlyDate1 + "T" + SkyWay.FlyStartTime.Insert(2, ":") + ":00" + "/";
                ToTime = ToTime + SkyWay.FlyDateE + "T" + SkyWay.FlyEndTime.Insert(2, ":") + ":00" + "/";

                FromCityCode = FromCityCode + SkyWay.FromCode + "/";
                ToCityCode = ToCityCode + SkyWay.ToCode + "/";
                FlightCode = FlightCode + SkyWay.AirCodeFlightNum.Replace("*", "") + "/";
                //AircraftModel = AircraftModel + SkyWay.Aircraft + "/";
                AircraftModel = AircraftModel + "/";//
                Space = Space + SkyWay.Seat + "/";
                decimal PMFees = SpacePrice;
                string s = PMFees.ToString().Split('.')[0].ToString();
                int s1 = int.Parse(s.Substring(s.Length - 1, 1));
                if (s1 > 0 && s1 <= 4)
                {
                    PMFees = PMFees - s1;
                }
                else if (s1 >= 5)
                {
                    PMFees = PMFees - s1;
                    PMFees = PMFees + 10;
                }
                PMFee = PMFee + PMFees.ToString() + "/";
                if (PolicyPoint > 1)
                {
                    PolicyPoint = PolicyPoint / 100;
                }
                JSFee = JSFee + Convert.ToString(PMFees * (1 - PolicyPoint)) + "/";
                strABFee = strABFee + ABFee.ToString() + "/";
                strFuelFee = strFuelFee + FuelFee.ToString() + "/";
            }

            #endregion

            OrderArray[4] = FromTime.Replace("/", "#").Substring(0, FromTime.Length - 1);
            OrderArray[5] = ToTime.Replace("/", "#").Substring(0, ToTime.Length - 1);

            OrderArray[6] = FlightCode.Substring(0, FlightCode.Length - 1);
            OrderArray[7] = AircraftModel.Substring(0, AircraftModel.Length - 1);
            OrderArray[8] = FromCityCode.Substring(0, FromCityCode.Length - 1);
            OrderArray[9] = ToCityCode.Substring(0, ToCityCode.Length - 1);
            OrderArray[10] = Space.Substring(0, Space.Length - 1);
            OrderArray[11] = PMFee.Substring(0, PMFee.Length - 1);
            OrderArray[12] = JSFee.Substring(0, JSFee.Length - 1);
            OrderArray[13] = strABFee.Substring(0, strABFee.Length - 1);
            OrderArray[14] = strFuelFee.Substring(0, strFuelFee.Length - 1);
            OrderArray[15] = LinkMan;
            OrderArray[16] = LinkManPhone;
            OrderArray[17] = pnrModel._BigPnr;

            return OrderArray;
        }
        public string ConvertDateTime(string strDateTime)
        {
            try
            {
                if (strDateTime.Contains("T"))
                {
                    strDateTime = strDateTime.Replace("T", " ");
                }
                DateTime dt = DateTime.Parse(strDateTime);
                strDateTime = dt.ToString("yyyy-MM-dd") + "T" + dt.ToString("HH:mm:ss");
            }
            catch (Exception)
            {
            }
            return strDateTime;
        }


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="oldstr">要加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        private string GetMD5str(string oldstr)
        {
            ASCIIEncoding asc = new ASCIIEncoding();
            byte[] result = Encoding.Default.GetBytes(oldstr);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }
        /// <summary>
        /// 构造百拓支付订单信息接口页面
        /// </summary>
        /// <param name="OrderId">本地机票订单编号</param>
        /// <param name="Type">1，支付宝；2，快钱</param>
        /// <returns>返回构造好的GET提交方式页面信息</returns>
        public string BaiTuoPaySend(string _baiTuoAccout, string _baiTuoAg, string OutOrderId, decimal TotalPMFee, decimal PayFee, string NotifyUrl, string Type)
        {
            string SendURL = "";
            try
            {
                PayFee = Math.Round(PayFee, 2);
                string CoAgsinCode = GetMD5str(_baiTuoAg + OutOrderId);
                if (Type == "1")
                {
                    SendURL = "http://co.baitour.com/copayment/PortFlightPayment.aspx?ForderId=" + OutOrderId + "&CoUserName=" + _baiTuoAccout + "&CoAgentCode=" + CoAgsinCode + "&UserFares=" + TotalPMFee.ToString() + "&CoShouldPay=" + PayFee.ToString() + "&opUrl=" + NotifyUrl + "&bankId=AUTOALIPAY";
                }
                else
                {
                    SendURL = "http://co.baitour.com/copayment/PortFlightPayment.aspx?ForderId=" + OutOrderId + "&CoUserName=" + _baiTuoAccout + "&CoAgentCode=" + CoAgsinCode + "&UserFares=" + TotalPMFee.ToString() + "&CoShouldPay=" + PayFee.ToString() + "&opUrl=" + NotifyUrl + "&bankId=AUTOBILLPAY";
                }
            }
            catch
            {
                SendURL = "";
            }
            if (SendURL != "")
            {
                return SendURL;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 退费改返回结果数据
        /// </summary>
        /// <param name="Status"></param>
        /// <param name="Result"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        public DataSet CreateDataSet(string Status, string Result, string OutOrderId, string Platform)
        {
            DataSet ds = new DataSet();
            ds.DataSetName = "ApplyTF";
            DataTable table = new DataTable();
            table.TableName = Platform;
            table.Columns.Add("status", typeof(string));
            table.Columns.Add("msg", typeof(string));
            table.Columns.Add("OutOrderId", typeof(string));
            DataRow dr = table.NewRow();
            dr["status"] = Status;
            dr["msg"] = Result;
            dr["OutOrderId"] = OutOrderId;
            table.Rows.Add(dr);
            ds.Tables.Add(table);
            return ds;
        }



        /// <summary>
        /// 构造申请退费票XML节点
        /// </summary>
        /// <returns></returns>
        public XmlNode CreateApplyRefundOrder(string _baiTuoAccout, string _baiTuoPassword, string _baiTuoAg, string PNR, string RefundType, string IsHaveTakeOff, string OutOrderId, string strAppLaySky, string strApplayPasName)
        {
            XmlDocument doc = new XmlDocument();
            //创建父节点
            XmlElement xe1 = doc.CreateElement("Order_Refund_RQ");
            XmlElement xe2 = doc.CreateElement("RefundOrderInfo");
            XmlElement xe3 = doc.CreateElement("RefundFlightSegments");
            XmlElement xe4 = doc.CreateElement("Remark");
            try
            {
                //string RefundType = InParam.m_ApplayInParam.ApplayType == 3 ? (InParam.m_ApplayInParam.RefundReasonType == 0 ? "2" : "0") : "1";
                //string IsHaveTakeOff = DateTime.Compare(System.DateTime.Now, InParam.m_ApplayInParam.ApplaySkyList[0].FromDate) > 0 ? "1" : "0"; ;
                //在节点中添加属性与对应的值
                xe1.SetAttribute("AgentSine", _baiTuoAg);
                xe1.SetAttribute("AgentUserName", _baiTuoAccout);
                xe1.SetAttribute("AgentPwd", _baiTuoPassword);

                xe2.SetAttribute("ForderformID", OutOrderId);//百拓订单号
                xe2.SetAttribute("InternationalTicket", "0");//国内国际 0：国内 1：国际
                xe2.SetAttribute("RefundType", RefundType);// 退废票类型 0 自愿退票 1 申请废票 2 非自愿退票  3 取消订单（支付后）
                xe2.SetAttribute("UserFare", "0");//接口合作方应退客户金额
                xe2.SetAttribute("RefundSrc", "1");//订单来源 【0 代表退给接口 1 代表退给客人 只有在没有确定退票费率的情况下考虑 】
                xe2.SetAttribute("UserFetchInFare", "0");//平台退票时，从供应商退款时留下的钱
                xe2.SetAttribute("IsHaveTakeOff", IsHaveTakeOff);//标识是否是起飞后 1代表已起飞
                xe2.SetAttribute("HaveTakeOffUrl", "");//起飞后客户传来的附件地址               
                xe2.SetAttribute("RefundPortorderId", "");//退废票新生成的订单号 可不填

                string[] skyWayArr = strAppLaySky.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                string[] SkyItem = null;
                foreach (string strSkyItem in skyWayArr)
                {
                    SkyItem = strSkyItem.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (SkyItem.Length >= 2)
                    {
                        XmlElement xe5 = doc.CreateElement("RefundFlightSegment");
                        xe5.SetAttribute("DepartureAirport", SkyItem[0]);//出发城市
                        xe5.SetAttribute("ArrivalAirport", SkyItem[1]);//到达城市  
                        string[] PasNameArr = strApplayPasName.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                        if (PasNameArr.Length > 0)
                        {
                            foreach (string pasName in PasNameArr)
                            {
                                XmlElement xe6 = doc.CreateElement("PersonInfo");
                                xe6.SetAttribute("PersonName", pasName);
                                xe6.SetAttribute("PNR", PNR);
                                xe5.AppendChild(xe6);
                            }
                        }
                        xe3.AppendChild(xe5);
                    }
                }

                //将节点附加到父节点中
                xe1.AppendChild(xe2);
                xe2.AppendChild(xe3);
                xe2.AppendChild(xe4);
            }
            catch
            {
                return xe1;
            }
            return xe1;
        }
        /// <summary>
        /// 构造查询订单详情的XML
        /// </summary>
        /// <param name="AgentCode"></param>
        /// <param name="AgentUserName"></param>
        /// <param name="AgentPwd"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public XmlNode CreateSelectOrderInfo(string AgentCode, string AgentUserName, string AgentPwd, string OrderID)
        {
            return null;
        }
        #endregion

        /// <summary>
        /// 构造生成订单返回结果
        /// </summary>
        /// <param name="PlatformName">平台名称</param>
        /// <param name="Status">状态</param>
        /// <param name="PTOrderId">生成成功订单号</param>
        /// <param name="TotlePirce">支付(代付)价格</param>
        /// <returns></returns>
        public DataTable CreateOrderReturnData(string PlatformName)
        {
            DataTable table = new DataTable();
            table.TableName = PlatformName;
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("Pnr", typeof(string));
            table.Columns.Add("OutOrderId", typeof(string));
            table.Columns.Add("PaidTotlePirce", typeof(string));

            table.Columns.Add("SeatPrice", typeof(string));
            table.Columns.Add("ABFare", typeof(string));
            table.Columns.Add("RQFare", typeof(string));
            table.Columns.Add("TotalPrice", typeof(string));
            table.Columns.Add("INFSeatPrice", typeof(string));
            table.Columns.Add("INFABFare", typeof(string));
            table.Columns.Add("INFRQFare", typeof(string));
            table.Columns.Add("INFTotalPrice", typeof(string));


            table.Columns.Add("SeatTotalPrice", typeof(string));
            table.Columns.Add("ABTotalFare", typeof(string));
            table.Columns.Add("RQTotalFare", typeof(string));

            table.Columns.Add("PnrType", typeof(string));

            DataRow dr = table.NewRow();
            table.Rows.Add(dr);
            dr["Status"] = "F";
            dr["Message"] = "";
            dr["OutOrderId"] = "";
            dr["Pnr"] = "";
            dr["PaidTotlePirce"] = "0";

            dr["SeatPrice"] = "0";
            dr["ABFare"] = "0";
            dr["RQFare"] = "0";
            dr["TotalPrice"] = "0";

            dr["INFSeatPrice"] = "0";
            dr["INFABFare"] = "0";
            dr["INFRQFare"] = "0";
            dr["INFTotalPrice"] = "0";


            dr["SeatTotalPrice"] = "0";
            dr["ABTotalFare"] = "0";
            dr["RQTotalFare"] = "0";
            dr["PnrType"] = "1";

            return table;
        }

        /// <summary>
        /// 构造支付返回结果
        /// </summary>
        /// <param name="PlatformName">平台名称</param>
        /// <param name="Status">状态</param>
        /// <param name="PTOrderId">生成成功订单号</param>
        /// <param name="TotlePirce">支付(代付)价格</param>
        /// <returns></returns>
        public DataTable CreatePayReturnData(string PlatformName)
        {
            DataTable table = new DataTable();
            table.TableName = PlatformName;
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("OutOrderId", typeof(string));
            DataRow dr = table.NewRow();
            table.Rows.Add(dr);
            dr["Status"] = "F";
            dr["Message"] = "";
            dr["OutOrderId"] = "";
            return table;
        }

        /// <summary>
        /// 构造政策消息
        /// </summary>
        /// <param name="PlatformName"></param>
        /// <returns></returns>
        public DataTable CreatePolicy(string PlatformName)
        {
            DataTable table = new DataTable();
            table.TableName = PlatformName;
            table.Columns.Add("Pnr", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("PnrType", typeof(string));
            table.Columns.Add("SeatPrice", typeof(string));
            table.Columns.Add("ABFare", typeof(string));
            table.Columns.Add("RQFare", typeof(string));
            table.Columns.Add("TotalPrice", typeof(string));
            table.Columns.Add("INFSeatPrice", typeof(string));
            table.Columns.Add("INFABFare", typeof(string));
            table.Columns.Add("INFRQFare", typeof(string));
            table.Columns.Add("INFTotalPrice", typeof(string));

            DataRow dr = table.NewRow();
            table.Rows.Add(dr);
            dr["Status"] = "F";
            dr["Pnr"] = "";
            dr["Message"] = "";
            dr["PnrType"] = "1";
            dr["SeatPrice"] = "0";
            dr["ABFare"] = "0";
            dr["RQFare"] = "0";
            dr["TotalPrice"] = "0";
            dr["INFSeatPrice"] = "0";
            dr["INFABFare"] = "0";
            dr["INFRQFare"] = "0";
            dr["INFTotalPrice"] = "0";
            return table;
        }
    }
}
