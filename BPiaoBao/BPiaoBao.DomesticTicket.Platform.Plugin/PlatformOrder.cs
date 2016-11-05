using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public class PlatformOrder
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal TotlePaidPirce
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位价总和
        /// </summary>
        public decimal TotaSeatlPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 区域
        /// </summary>
        public string AreaCity { get; set; }

        /// <summary>
        /// Pnr编码
        /// </summary>
        public string PnrCode { get; set; }

        /// <summary>
        /// 代付方式
        /// </summary>
        public EnumPaidMethod PaidMethod { get; set; }
        /// <summary>
        /// 接口订单号(代付订单号)
        /// </summary>
        public string OutOrderId
        {
            get;
            set;
        }
    }
}
