using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    /// <summary>
    /// 基础舱位数据
    /// </summary>
    public class CabinData
    {
        /// <summary>
        /// 航空公司二字码对应的基础舱位
        /// </summary>
        public List<CabinRow> CabinList = new List<CabinRow>();
    }
    /// <summary>
    /// 基础舱位数据
    /// </summary>
    public class CabinRow
    {
        /// <summary>
        /// 航空公司
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 基本折扣
        /// </summary>
        public decimal Rebate { get; set; }
    }
}
