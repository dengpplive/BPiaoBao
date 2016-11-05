using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Util;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    public class _TPRSInsurancePlatform : _IInsurancePlatform
    {
        private string _dlbh =
            InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_TPRS.DLBH;
        private string _tbbxddh =
           InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_TPRS.TBBXDDH;
        private string _key =
                   InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_TPRS.Key;

        private string _url = InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_TPRS.Url;

        private decimal _jsbf = InsuranceSection.GetInsuranceConfigurationSection().Insurance_Platform_Config.Insurance_TPRS.JSBF;

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
        public Tuple<string, string> Buy_Insurance(string serialNumber, DateTime beginDate, DateTime endDate, string name, string idType, string idNo,
            string sexType, DateTime? birthDay, string mobile, int insuranceCount = 1, string fligthNumber = null, DateTime? flightStartDate = null, string toCityName = null)
        {
            var curDate = DateTime.Now;
            var birthDayStr = "";
            if (birthDay.HasValue)
            {
                birthDayStr = birthDay.Value.ToString("yyyyMMdd");
                if (idType == "9" && string.IsNullOrEmpty(idNo))
                {
                    idNo = birthDayStr;
                }
            }

            string strHead = "<?xml version=\"1.0\" encoding=\"GBK\"?>";
            string strRE = "-1";
            StringBuilder sb = new StringBuilder();
            sb.Append("<REQUEST>");
            sb.Append("<DIST>");
            sb.Append("<DLBH>" + _dlbh + "</DLBH>");
            sb.Append("<TBBXDDH>" + serialNumber + "</TBBXDDH>");
            sb.Append("</DIST>");
            sb.Append("<BUSI>");
            sb.Append("<DZHM></DZHM>");
            sb.Append("<TBRQ>" + curDate.ToString("yyyyMMdd") + "</TBRQ>");
            sb.Append("<TBSJ>" + curDate.ToString("HHmmss") + "</TBSJ>");
            //客户类型   1-个人;2-企业-->
            sb.Append("<TBKHLX>1</TBKHLX>");
            sb.Append("<TBR>");
            //姓名-->
            sb.Append("<TBRXM>" + name + "</TBRXM>");
            //性别-->
            sb.Append("<TBRXB>" + sexType + "</TBRXB>");
            //出生日期-->
            sb.Append("<TBRSR>" + birthDayStr + "</TBRSR>");
            //证件类型-->
            sb.Append("<TBRZJLX>" + idType + "</TBRZJLX>");
            //证件号码-->
            sb.Append("<TBRZJH>" + idNo + "</TBRZJH>");
            //手机-->
            sb.Append("<TBRSJH>" + mobile + "</TBRSJH>");
            //电子邮件-->
            sb.Append("<TBRYXDZ>bx@51cbc.cn</TBRYXDZ>");
            // 投保人地址 --> 
            sb.Append("</TBR>");
            sb.Append("<COMP>");
            //单位名称-->
            sb.Append("<DWMC>信誉金融</DWMC>");
            //联系人姓名-->
            sb.Append("<LXRMC></LXRMC>");
            //联系人手机-->
            sb.Append("<LXRSJH></LXRSJH>");
            //联系人电子邮件-->
            sb.Append("<LXRDZYJ>bx@51cbc.cn</LXRDZYJ>");
            sb.Append("</COMP>");
            //被保人信息区--> 
            sb.Append("<BBR>");
            //姓名-->
            sb.Append("<BBXRXM>" + name + "</BBXRXM>");
            //性别-->
            sb.Append("<BBXRXB>" + sexType + "</BBXRXB>");
            //出生日期-->
            sb.Append("<BBXRSR>" + birthDayStr + "</BBXRSR>");
            //证件类型-->
            sb.Append("<ZJLX>" + idType + "</ZJLX>");
            //证件号码-->
            sb.Append("<BBXRZJH>" + idNo + "</BBXRZJH>");
            //手机-->
            sb.Append("<BBXRSJH>" + mobile + "</BBXRSJH>");
            //电子邮件-->
            sb.Append("<BBXRYXDZ>bx@51cbc.cn</BBXRYXDZ>");
            //职业代码--> 
            //受益人信息区-->
            sb.Append("<BENE>");
            sb.Append("<SYRXM></SYRXM>");
            sb.Append("</BENE>");
            sb.Append("</BBR>");
            sb.Append("<TRIP>");
            //航班号-->
            sb.Append("<HBH>" + fligthNumber + "</HBH>");
            //航班日期-->
            if (flightStartDate == null)
            {
                sb.Append("<HBRQ></HBRQ>");

            }
            else
            {
                sb.Append("<HBRQ>" + flightStartDate.Value.ToString("yyyyMMdd") + "</HBRQ>");
            }
            //旅行目的地-->
            sb.Append("<LXMDD>" + toCityName + "</LXMDD>");
            sb.Append("</TRIP>");
            sb.Append("<PT>");
            sb.Append("<XZDM>1</XZDM>");
            sb.Append("<XZMC>太平游意外伤害保险</XZMC>");
            sb.Append("<TBSL>" + insuranceCount + "</TBSL>");
            sb.Append("<JSBF>" + _jsbf * insuranceCount + "</JSBF>");
            sb.Append("<BXJE></BXJE>");
            sb.Append("<BXZRQSSJ>" + beginDate.ToString("yyyyMMdd") + "</BXZRQSSJ>");
            sb.Append("<BXZRZZSJ>" + endDate.ToString("yyyyMMdd") + "</BXZRZZSJ>");
            sb.Append("<JHDM>" + _tbbxddh + "</JHDM>");
            sb.Append("<CZDM>1</CZDM>");
            sb.Append("<BDZT>1</BDZT>");
            sb.Append("</PT>");

            sb.Append("</BUSI>");
            sb.Append("</REQUEST>");

            var str = strHead + sb.ToString() + _key;

            //MD5加密
            var strXml = MD5Encrypt(str);

            var param = "dlbh=" + _dlbh + "&xmlstr=" + strHead + sb.ToString() + "&md5=" + strXml;

            Logger.WriteLog(LogType.DEBUG, string.Format("发送保险投保单地址：{0}{1}", _url, param));

            strRE = HttpWebRequestHelper.Post(_url, param);

            Logger.WriteLog(LogType.DEBUG, string.Format("返回发送保险投保单地址响应：{0}", strRE));

            if (strRE.Contains("hf_md5"))
            {
                Logger.WriteLog(LogType.ERROR, "MD5验证不通过。返回：" + strRE);
                throw new CustomException(10001, "MD5验证不通过");

            }

            var model = XmlSerializerHelper.Deserialize<_TPRSReturn>(strRE);
            Tuple<string, string> t = null;
            if (model != null && model.Busi.RejectDesc == "交易成功")
            {
                Logger.WriteLog(LogType.DEBUG,
                    "太平人寿保险   投保   投保单号：" + model.Busi.Main.Tbdh + "   保单号：" + model.Busi.Main.Bdh);
                t = new Tuple<string, string>(model.Busi.Trans, model.Busi.Main.Bdh);
            }
            else
            {
                if (model != null)
                {
                    Logger.WriteLog(LogType.DEBUG, string.Format("发送保险投保单异常。原因：{0}", model.Busi.RejectDesc));
                    throw new CustomException(10001, model.Busi.RejectDesc);
                }
                else
                {
                    Logger.WriteLog(LogType.ERROR, "投保保险接口返回异常数据不能够解析。返回：" + strRE);
                    throw new CustomException(10001, "投保保险接口返回异常数据不能够解析");

                }

            }
            return t;
        }

        /// <summary>
        /// 退保
        /// </summary>
        /// <param name="insuranceNo">保单号码</param> 
        /// <param name="serailNumebr">流水号</param>
        /// <returns>成功则返回<流水号,保单号></returns>
        public Tuple<string, string> Refund_Insurance(string insuranceNo, string serailNumebr = null)
        {
            var curDate = DateTime.Now;

            string strHead = "<?xml version=\"1.0\" encoding=\"GBK\"?>";

            string strRE = "-1";

            StringBuilder sb = new StringBuilder();
            sb.Append("<REQUEST>");
            sb.Append("<DIST>");
            sb.Append("<DLBH>" + _dlbh + "</DLBH>");
            sb.Append("<TBBXDDH>" + serailNumebr + "</TBBXDDH>");
            sb.Append("</DIST>");
            sb.Append("<BUSI>");
            sb.Append("<TBRQ>" + curDate.ToString("yyyyMMdd") + "</TBRQ>");
            sb.Append("<TBSJ>" + curDate.ToString("HHmmss") + "</TBSJ>");
            sb.Append("<CZDM>2</CZDM>");
            //保单号码-->
            sb.Append("<BDH>" + insuranceNo + "</BDH>");
            //原交易流水号-->
            sb.Append("<OLD_TBBXDDH>" + insuranceNo + "</OLD_TBBXDDH>");
            sb.Append("</BUSI>");
            sb.Append("</REQUEST>");

            var str = strHead + sb.ToString() + _key;
            //MD5加密 
            var strXml = MD5Encrypt(str);

            var param = "dlbh=" + _dlbh + "&xmlstr=" + strHead + sb.ToString() + "&md5=" + strXml;

            Logger.WriteLog(LogType.DEBUG, string.Format("发送保险退保单地址：{0}{1}", _url, param));

            strRE = HttpWebRequestHelper.Post(_url, param);

            Logger.WriteLog(LogType.DEBUG, string.Format("返回发送保险退保单响应：{0}", strRE));

            if (strRE.Contains("hf_md5"))
            {
                Logger.WriteLog(LogType.ERROR, "MD5验证不通过。返回：" + strRE);
                throw new CustomException(10001, "验证不通过");

            }

            var model = XmlSerializerHelper.Deserialize<_TPRSReturn>(strRE);
            Tuple<string, string> t = null;
            if (model != null && model.Busi.RejectDesc == "交易成功")
            {
                Logger.WriteLog(LogType.DEBUG,
                    "太平人寿保险   退保   投保单号：" + model.Busi.Main.Tbdh + "   保单号：" + model.Busi.Main.Bdh);
                t = new Tuple<string, string>(model.Busi.Trans, model.Busi.Main.Bdh);
            }
            else
            {
                if (model != null)
                {
                    Logger.WriteLog(LogType.DEBUG, string.Format("发送保险退保单异常。原因：{0}", model.Busi.RejectDesc));
                    throw new CustomException(10001, model.Busi.RejectDesc);
                }
                else
                {
                    Logger.WriteLog(LogType.ERROR, "退保保险接口返回异常数据不能够解析。返回：" + strRE);
                    throw new CustomException(10001, "退保保险接口返回异常数据不能够解析");

                }
            }
            return t;
        }

        /// <summary>
        /// 获取保险记录支付用订单号
        /// </summary>
        /// <returns></returns>
        private string GetPayNo()
        {
            //每次生成随机数的时候都使用机密随机数生成器来生成种子，
            //这样即使在很短的时间内也可以保证生成的随机数不同
            var rand = new Random(GetRandomSeed());
            var exNum = rand.Next(1000, 9999);
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), exNum);
        }

        /// <summary>
        /// 得到随机数种子
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string MD5Encrypt(string str)
        {
            var sb = new StringBuilder();
            var md5 = new MD5CryptoServiceProvider();
            var s = md5.ComputeHash(Encoding.GetEncoding("GBK").GetBytes(str));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                sb.Append(s[i].ToString("X2"));

            }
            return sb.ToString().ToLower();
        }

    }
}
