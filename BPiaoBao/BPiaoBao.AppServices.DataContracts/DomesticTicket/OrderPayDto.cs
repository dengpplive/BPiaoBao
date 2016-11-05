using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class OrderPayDto
    {
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PaySerialNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 代付流水号
        /// </summary>
        public string PaidSerialNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal PaidMoney
        {
            get;
            set;
        }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayMoney
        {
            get;
            set;
        }
        /// <summary>
        /// 具体支付方式代号
        /// </summary>
        public string PayMethodCode { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethod { get; set; }
        /// <summary>
        /// 代付方式
        /// </summary>
        public string PaidMethod { get; set; }

        /// <summary>
        /// 支付状态 0未支付 1已支付
        /// </summary>       
        public EnumPayStatus PayStatus { get; set; }

        /// <summary>
        /// 代付状态 0未代付 1已代付
        /// </summary>       
        public EnumPaidStatus PaidStatus { get; set; }

        /// <summary>
        /// 交易手续费
        /// </summary>
        public decimal TradePoundage
        {
            get;
            set;
        }
        /// <summary>
        /// 系统分润
        /// </summary>
        public decimal SystemFee
        {
            get;
            set;
        }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 代付时间
        /// </summary>
        public DateTime? PaidDateTime
        {
            get;
            set;
        }

        public virtual List<PayBillDetailDto> PayBillDetailDtos
        {
            get;
            set;
        }
    }
}
