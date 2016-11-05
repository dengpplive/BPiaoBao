using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    /// <summary>
    /// 当前运营商保险配置信息
    /// </summary>
   public  class ResponseCurrentCarrierInsurance
    {
       /// <summary>
        /// 航意险剩余数量
       /// </summary>
       public int LeaveCount { get; set; }

       /// <summary>
       /// 航意险保险单价
       /// </summary>
       public decimal UnexpectedSinglePrice { get; set; }

       /// <summary>
       /// 当前保险公司名称
       /// </summary>
       public string InsuranceCompany { get; set; }

       /// <summary>
       /// 急速退保险单价
       /// </summary>
       public decimal RefundSinglePrice { get; set; }


       /// <summary>
       /// 是否开启急速退
       /// </summary>
       public bool IsOpenRefundInsurance { get; set; }

       /// <summary>
       /// 控台上是否开启航意险配置
       /// </summary>
       public bool IsOpenUnexpectedInsurance { get; set; }

       /// <summary>
       /// 当前运营商是否打开保险配置
       /// </summary>
       public bool IsOpenCurrenCarrierInsurance { get; set; }

       ///// <summary>
       ///// 保险接口总开关是否开启
       ///// </summary>
       //public bool IsOpenGlobalInsurance { get; set; }

      
        
    }
}
