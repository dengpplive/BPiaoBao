using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    /// <summary>
    /// 第三方保险接口领域服务
    /// </summary>
    public class InsurancePlatformDomainService
    {

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
        /// <param name="birthDay">投保人生日(yyyyMMdd)</param>
        /// <param name="mobile">投保人手机</param>
        /// <param name="insuranceCount">投保份数（默认1份）</param>
        /// <param name="fligthNumber">航班号(如：3U8823)</param>
        /// <param name="flightStartDate">航班日期</param>
        /// <param name="toCityName">到达城市</param> 
        /// <returns>成功则返回保单号,否则-1</returns>
        public static string Buy_Insurance(string serialNumber,DateTime beginDate, DateTime endDate, string name, EnumIDType idType, string idNo,
            EnumSexType sexType, DateTime? birthDay, string mobile,int insuranceCount=1, string fligthNumber = null, DateTime? flightStartDate = null,
            string toCityName = null)
        {
            Tuple<string,string> tuple =null;
            var collect = InsuranceSection.GetInsuranceConfigurationSection().CtrlInsuranceCollection;
            if (!collect.IsEnabled)
            {
                throw new CustomException(100012, "保险平台未启用");
            }
            var curInsChanel = "";
            foreach (var m in from InsuranceElement m in collect where m.IsCurrent select m)
            {
                curInsChanel = m.Value;
                break;
            }
            if (curInsChanel == "中科")
            { 
                var idTypeStr = idType == EnumIDType.NormalId ? "I" : "O";
                var sexTypeStr = sexType == EnumSexType.Male ? "M" : "F";
                var iInsurancePlatform = new _ZKInsurancePlatform();
                tuple = iInsurancePlatform.Buy_Insurance(serialNumber, beginDate, endDate, name, idTypeStr, idNo, sexTypeStr, birthDay, mobile,insuranceCount);
            }
            else if (curInsChanel == "中国太平")
            {
               
                 
                var idTypeStr = "";
                if (idType == EnumIDType.NormalId)
                {
                    idTypeStr = "1";
                }
                else if (idType == EnumIDType.MilitaryId)
                {
                    idTypeStr = "2";
                }
                else if (idType == EnumIDType.Passport)
                {
                    idTypeStr = "3";
                }
                else
                {
                    idTypeStr = "9";
                    
                }
                var sexTypeStr = sexType == EnumSexType.Male ? "1" : "2";
                var iInsurancePlatform = new _TPRSInsurancePlatform();
                tuple = iInsurancePlatform.Buy_Insurance(serialNumber, beginDate, endDate, name, idTypeStr, idNo, sexTypeStr,
                    birthDay, mobile, insuranceCount, fligthNumber, flightStartDate, toCityName);

            }
            else
            {
                throw new CustomException(100013, "当前启用的保险通道有误");
            }
            return tuple == null ? "-1" : tuple.Item2;
        }


        /// <summary>
        /// 退保
        /// </summary>
        /// <param name="insuranceNo">保单号</param>
        /// <param name="serailNumebr">中介方流水号</param>
        /// <returns>成功则返回保单号,否则-1</returns>
        public static  string Refund_Insurance(string insuranceNo, string serailNumebr)
        {
            Tuple<string, string> tuple = null;
            var collect = InsuranceSection.GetInsuranceConfigurationSection().CtrlInsuranceCollection;
            if (!collect.IsEnabled)
            {
                throw new CustomException(100012, "保险平台未启用");
            }
            var curInsChanel = "";
            foreach (var m in from InsuranceElement m in collect where m.IsCurrent select m)
            {
                curInsChanel = m.Value;
                break;
            }
            if (curInsChanel == "中科")
            {
                
                var iInsurancePlatform = new _ZKInsurancePlatform();
                tuple = iInsurancePlatform.Refund_Insurance(insuranceNo);
            }
            else if (curInsChanel == "中国太平")
            { 
                var iInsurancePlatform = new _TPRSInsurancePlatform();
                tuple = iInsurancePlatform.Refund_Insurance(insuranceNo, serailNumebr);

            }
            else
            {
                throw new CustomException(100013, "当前启用的保险通道有误");
            }
            return tuple == null ? "-1" : tuple.Item2;
        }
    }
}