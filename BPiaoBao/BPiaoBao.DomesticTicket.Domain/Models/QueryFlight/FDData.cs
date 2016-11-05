using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// FD运价
    /// </summary>
    public class FDData
    {
        public FDData()
        {
            this.YFdRow = new Dictionary<string, FdRow>();
            this.FdRow = new List<FdRow>();
        }
        public QueryParam QueryParam
        {
            get;
            set;
        }
        /// <summary>
        /// Y舱信息
        /// </summary>
        public Dictionary<string, FdRow> YFdRow { get; set; }
        /// <summary>
        /// 舱位价格信息
        /// </summary>
        public List<FdRow> FdRow { get; set; }
    }

    public class FdRow
    {
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 舱位 
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 舱位价(运价)
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// 里程
        /// </summary>
        public int Mileage { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 成人燃油费
        /// </summary>
        public decimal ADultFuleFee { get; set; }
        /// <summary>
        /// 儿童燃油费
        /// </summary>
        public decimal ChildFuleFee { get; set; }
    }


}
