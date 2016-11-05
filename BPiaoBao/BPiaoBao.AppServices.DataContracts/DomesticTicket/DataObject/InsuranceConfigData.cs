using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    /// <summary>
    /// 配置保单实体(后台调用)
    /// </summary>
    public class InsuranceConfigData
    {/// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 剩余张数
        /// </summary>
        public int LeaveCount { get; set; }


        /// <summary>
        /// 单价
        /// </summary>
        public decimal SinglePrice { get; set; }

        /// <summary>
        /// 保险开关
        /// </summary>
        public bool IsOpen { get; set; }


        /// <summary>
        /// 商户Code
        /// </summary>
        public string BusinessmanCode { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessmanName { get; set; }

    }


    public class CtrlInsuranceInterDto
    {
        public List<CtrlInsuranceBase> CtrlInsuranceInter { get; set; }
        public bool IsEnabled { get; set; }
    }
    public class CtrlInsuranceDto
    {
        public List<CtrlInsuranceConfig> CtrlInsurance { get; set; }
        public bool IsEnabled { get; set; }

        public decimal SinglePrice { get; set; }

    }
    public class CtrlInsuranceBase
    {
        public bool IsCurrent { get; set; }

        public string Value { get; set; }

       // public string Url { get; set; }
    }

    public class CtrlInsuranceConfig : CtrlInsuranceBase
    {
        public decimal LeaveCount { get; set; }
         
    }


    public class CtrlInsuranceRefundDto
    {
        public bool IsEnabled { get; set; }

        public decimal SinglePrice { get; set; }
    }
}
