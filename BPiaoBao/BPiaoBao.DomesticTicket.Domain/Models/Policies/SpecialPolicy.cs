using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Policies
{
    public class SpecialPolicy : LocalPolicy
    {
        /// <summary>
        /// 特价类型
        /// </summary>
        public SpeciaType SpecialType { get; set; }
        /// <summary>
        ///  固定特价类型
        /// </summary>
        public FixedOnSaleType Type { get; set; }
        /// <summary>
        /// 固定舱位价位
        /// </summary>
        public decimal FixedSeatPirce { get; set; }
    }
}
