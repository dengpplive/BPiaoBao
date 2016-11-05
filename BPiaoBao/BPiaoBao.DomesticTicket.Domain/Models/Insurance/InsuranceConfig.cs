using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{

    /// <summary>
    /// 运营商保险配置
    /// </summary>
    public class InsuranceConfig : EntityBase, IAggregationRoot
    {
        /// <summary>
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

        /// <summary>
        /// 配置类型
        /// </summary>
        public EnumInsuranceConfigType ConfigType { get; set; }


     
        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }

        
    }



}
