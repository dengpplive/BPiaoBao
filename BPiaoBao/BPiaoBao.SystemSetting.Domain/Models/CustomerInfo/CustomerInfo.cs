using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo
{
    public class CustomerInfo : EntityBase, IAggregationRoot
    {
        protected override string GetIdentity()
        {
            return Id.ToString();
        }
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 运营商编码
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 客服服务电话
        /// </summary>
        public string CustomPhone { get; set; }
        /// <summary>
        /// 热线电话
        /// </summary>
        public virtual List<PhoneInfo> HotlinePhone { get; set; }
        /// <summary>
        /// 在线qq
        /// </summary>
        public virtual List<QQInfo> AdvisoryQQ { get; set; }

    }
}
