using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo
{
    public class QQInfo : ValueObjectBase
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// qq号码
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// qq号码描述
        /// </summary>
        public string Description { get; set; }
    }
}
