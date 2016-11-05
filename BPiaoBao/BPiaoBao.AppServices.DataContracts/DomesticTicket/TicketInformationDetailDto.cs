using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 机票信息明细
    /// </summary>
    public class TicketInformationDetailDto
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 政策来源
        /// </summary>
        public string PlatformCode
        {
            get;
            set;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 接口订单号(代付订单号)
        /// </summary>
        public string OutOrderId
        {
            get;
            set;
        }
        /// <summary>
        /// Pnr编码
        /// </summary>
        public string PnrCode
        {
            get;
            set;
        }

        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode
        {
            get;
            set;
        }

        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TravelNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney
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
        /// 状态(机票)
        /// </summary>
        public string TicketStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 起飞时间
        /// </summary>
        public string StartDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 出票时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 承运人 航空公司二字码
        /// </summary>
        public string CarrayCode
        {
            get;
            set;
        }
        /// <summary>
        /// 航班号 8889
        /// </summary>
        public string FlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string Seat
        {
            get;
            set;
        }

        /// <summary>
        /// 航程 成都-北京
        /// </summary>
        public string Travel
        {
            get;
            set;
        }
        /// <summary>
        /// 乘客姓名 
        /// </summary>
        public string PassengerName
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFee
        {
            get;
            set;
        }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee
        {
            get;
            set;
        }
        /// <summary>
        /// 票面价 (舱位价+机建费+燃油费)
        /// </summary>
        public decimal TicketPrice
        {
            get;
            set;
        }


        /// <summary>
        /// 原政策（接口政策）
        /// </summary>
        public decimal OldPointPoint
        {
            get;
            set;
        }

        /// <summary>
        ///返点（采购政策）
        /// </summary>
        public decimal PointPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商户号
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 运营商费用
        /// </summary>
        public decimal CarrierMoney { get; set; }
        /// <summary>
        /// 合作开发者费用
        /// </summary>
        public decimal ParentMoney { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public string PolicySourceType { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal RealMoney { get; set; }
    }
}
