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

    }
}
