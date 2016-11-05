using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.ZKInsurance;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    /// <summary>
    /// 中科保险接口
    /// </summary>
    public class _ZKInsurancePlatform:_IInsurancePlatform
    {
        private ZKInsurance.JtywInsServiceSoapClient _client = new JtywInsServiceSoapClient();

        private string _username =
            InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_ZK.UserName;
        private string _password =
              InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_ZK.Password;
        private string _productcode =
           InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_ZK.ProductCode;

        /// <summary>
        /// <summary>
        /// 投保
        /// </summary> 
        /// <param name="serialNumber">流水号</param>
        /// <param name="beginDate">保险生效时间，格式：yyyy-MM-dd HH:mm:ss</param>
        /// <param name="endDate">保险截止时间，格式：yyyy-MM-dd HH:mm:ss</param>
        /// <param name="name">投保人姓名</param>
        /// <param name="idType">投保人证件类型</param>
        /// <param name="idNo">被保险人证件号</param>
        /// <param name="sexType">投保人性别</param>
        /// <param name="birthDay">投保人生日</param>
        /// <param name="mobile">投保人手机</param> 
        /// <param name="insuranceCount">投保份数（默认1份）</param>
        /// <param name="fligthNumber">航班号(如：3U8823)</param>
        /// <param name="flightStartDate">航班日期</param>
        /// <param name="toCityName">到达城市</param>
        /// <returns>成功则返回<流水号,保单号></returns>
        public Tuple<string, string> Buy_Insurance(string serialNumber,DateTime beginDate, DateTime endDate, string name, string idType, string idNo,
            string sexType, DateTime? birthDay, string mobile,int insuranceCount, string fligthNumber = null, DateTime? flightStartDate = null,
            string toCityName = null)
        {
           
            try
            {
                // 保险接口
                #region 组合保单信息（投保）

                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version='1.0' encoding='GBK'?>");
                sb.Append("<CssXml>");
                sb.Append("<Header>");
                //用户名，必填
                sb.Append("<Username>" + _username + "</Username>");
                //密码，必填
                sb.Append("<Password>" + _password + "</Password>");
                sb.Append("</Header>");
                sb.Append("<Request>");
                //保单信息
                sb.Append("<Policy>");
                //交易类型，1：投保 必填
                sb.Append("<TranType>1</TranType>");
                //投保方式，1：单证投保
                sb.Append("<PostType>1</PostType>");
                //投保基本信息
                sb.Append("<PolicyBaseInfo>");
                //产品编码，必填
                sb.Append("<ProductCode>" + _productcode + "</ProductCode>");
                //保险生效时间，格式：yyyy-MM-dd HH:mm:ss
                sb.Append("<BeginDate>" + beginDate + "</BeginDate>");
                //保险截止时间，格式：yyyy-MM-dd HH:mm:ss
                sb.Append("<EndDate>" +endDate + "</EndDate>");
                sb.Append("</PolicyBaseInfo>");
                //投保人信息
                sb.Append("<PolicyHolderInfo>");
                //投保人姓名
                sb.Append("<Name>" + name + "</Name>");
                //投保人证件类型
                sb.Append("<IDType>" + idType + "</IDType>");
                //投保人证件号
                sb.Append("<IDNo>" + idNo + "</IDNo>");
                //投保人性别
                sb.Append("<Sex>" + sexType + "</Sex>");
                //投保人生日
                sb.Append("<Birthday>" + birthDay + "</Birthday>");
                //投保人手机
                sb.Append("<Mobile>" + mobile + "</Mobile>");
                sb.Append("</PolicyHolderInfo>");
                //被保险人信息
                sb.Append("<InsurantInfo>");
                //被保险人姓名
                sb.Append("<Name>" + name + "</Name>");
                //被保险人证件类型
                sb.Append("<IDType>" + idType + "</IDType>");
                //被保险人证件号
                sb.Append("<IDNo>" + idNo + "</IDNo>");
                //被保险人性别
                sb.Append("<Sex>" + sexType + "</Sex>");
                //被保险人生日
                sb.Append("<Birthday>" + birthDay + "</Birthday>");
                //被保险人手机
                sb.Append("<Mobile>" + mobile + "</Mobile>");
                sb.Append("</InsurantInfo>");
                sb.Append("</Policy>");
                sb.Append("</Request>");
                sb.Append("</CssXml>");

                #endregion
                string strXml = sb.ToString();

                byte[] bytes = Encoding.Default.GetBytes(strXml);
                string str = Convert.ToBase64String(bytes);

                System.Xml.XmlNode xmlNode = _client.PolicyOperate_Test(str);
                var xn = xmlNode.InnerXml;
                if (string.IsNullOrEmpty(xn))
                {
                    return null;
                }
                xn = "<?xml version='1.0' encoding='GBK'?><Return>" + xn + "</Return>";
                 var model = XmlSerializerHelper.Deserialize<_ZKReturn>(xn);
                Tuple<string, string> t = null;
                if (model.Message.Value == "OK")
                {
                   t=new Tuple<string, string>(serialNumber, model.Table.PolicyNo);
                    Logger.WriteLog(LogType.INFO, "中科保险       投保        保单号：" + model.Table.PolicyNo);
                }
                else
                {
                    throw new CustomException(100011, model.Message.Value);
                }

                return t;

                //if (xn.Contains("<VALUE>OK</VALUE>"))
                //{
                //    //<Table><PolicyNo>000320744522158</PolicyNo><LocalPolicyNo>d99a21aa208f4b23bb6202fa087940a4</LocalPolicyNo></Table><MESSAGE><VALUE>OK</VALUE><TIME>2014/3/31 16:01:04</TIME></MESSAGE>
                //    xn = xn.Replace("<PolicyNo>", "|").Replace("</PolicyNo>", "|");
                //    if (xn.Split('|').Length > 2)
                //    {
                //        msg = xn.Split('|')[1];
                //        Logger.WriteLog(LogType.INFO, "中科保险       投保        保单号：" + msg);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, "系统异常 \r\n\r\n", ex);
            }

            return null;
        }


       
        /// <summary>
        /// 退保
        /// </summary>
        /// <param name="insuranceNo">保单号</param>
        /// <param name="serialNumber">流水号</param>
        /// <returns></returns>
        public Tuple<string, string> Refund_Insurance(string insuranceNo, string serialNumber = null)
        {
            
            try
            {
                // 保险接口
                #region 组合保单信息（退保）

                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version='1.0' encoding='GBK'?>");
                sb.Append("<CssXml>");

                sb.Append("<Header>");
                sb.Append("<Username>" + _username + "</Username>");//用户名，必填
                sb.Append("<Password>" + _password + "</Password>");//密码，必填
                sb.Append("</Header>");

                sb.Append("<Request>");
                sb.Append("<Policy>"); //保单信息

                sb.Append("<TranType>2</TranType>");//交易类型，1：投保 必填
                sb.Append("<PostType>1</PostType>");//投保方式，1：单证投保

                sb.Append("<PolicyBaseInfo>");//投保基本信息
                sb.Append("<ProductCode>" + _productcode + "</ProductCode>");//产品编码，必填
                sb.Append("<PolicyNo>" + insuranceNo + "</PolicyNo>");//保单号
                sb.Append("</PolicyBaseInfo>");

                sb.Append("</Policy>");
                sb.Append("</Request>");

                sb.Append("</CssXml>");

                #endregion

                string strXml = sb.ToString();

                byte[] bytes = Encoding.Default.GetBytes(strXml);
                string str = Convert.ToBase64String(bytes);

                System.Xml.XmlNode xmlNode = _client.PolicyOperate(str);
                string xn = xmlNode.InnerXml;
                if (string.IsNullOrEmpty(xn))
                {
                    return null;
                }
                xn = "<?xml version='1.0' encoding='GBK'?><Return>" + xn + "</Return>";
                var model = XmlSerializerHelper.Deserialize<_ZKReturn>(xn);
                Tuple<string, string> t = null;
                if (model.Message.Value == "OK")
                {
                    t = new Tuple<string, string>(serialNumber, model.Table.PolicyNo);
                    Logger.WriteLog(LogType.INFO, "中科保险       退保        保单号：" + insuranceNo);
             
                }
                else
                {
                    throw new CustomException(100011, model.Message.Value);
                }

                return t;
                // 解析投保信息后的信息
                //<MESSAGE><VALUE>OK</VALUE><TIME>2014/3/31 16:10:45</TIME></MESSAGE> 
                //if (xn.Contains("<VALUE>OK</VALUE>"))
                //{
                //    msg = "1";
                //    Logger.WriteLog(LogType.INFO, "中科保险       退保        保单号：" + msg);
                //}
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, "系统异常 \r\n\r\n", ex);
            }

            return null;
        }
    }
}
