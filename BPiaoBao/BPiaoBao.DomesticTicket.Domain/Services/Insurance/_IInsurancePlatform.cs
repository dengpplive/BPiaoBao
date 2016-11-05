using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    public interface _IInsurancePlatform
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
        /// <param name="birthDay">投保人生日</param>
        /// <param name="mobile">投保人手机</param> 
        /// <param name="insuranceCount">投保份数（默认1份）</param>
        /// <param name="fligthNumber">航班号(如：3U8823)</param>
        /// <param name="flightStartDate">航班日期</param>
        /// <param name="toCityName">到达城市</param>
        /// <returns>成功则返回<流水号,保单号></returns>
        Tuple<string,string> Buy_Insurance(string serialNumber,DateTime beginDate, DateTime endDate, string name, string idType, string idNo,
            string sexType, DateTime? birthDay, string mobile,int insuranceCount=1,string fligthNumber = null, DateTime? flightStartDate = null,
            string toCityName = null);


        /// <summary>
        /// 退保
        /// </summary>
        /// <param name="insuranceNo">保单号码</param> 
        /// <param name="serialNumber">流水号</param>
        /// <returns>成功则返回<流水号,保单号></returns>
        Tuple<string, string> Refund_Insurance(string insuranceNo, string serialNumber = null);
    }
}
