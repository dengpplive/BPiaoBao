using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Services.TodayObject
{
    public class TodayTuiFeiOrderRequest
    {
        /// <summary>
        /// A:  废票  B:  退票
        /// </summary>
        public string _Type { get; set; }
        /// <summary>
        /// 订单编号（接口）
        /// </summary>
        public string _OrderNo { get; set; }
        /// <summary>
        /// 退/废票标识1：  废票   2：  退票  0：  不变
        /// 退/废票标识只与乘客对应，多个以”|”分割
        /// </summary>
        public string _Repeal { get; set; }
        /// <summary>
        /// 退/废票乘客姓名
        /// </summary>
        public string _PersonName { get; set; }
        /// <summary>
        /// 是否取消今日位置。是：  取消  否：  不取消
        /// </summary>
        public string _IsCancelSeat { get; set; }
        /// <summary>
        /// 退/废票原因（A：当日废票
        /// B:  按宠规自愿退票
        /// C:  非自愿及特殊退票
        /// D:  航班延误申请全退)
        ///输入 A/B/C/D
        /// </summary>
        public string _Cause { get; set; }
        /// <summary>
        /// 退/废票备注 
        /// </summary>
        public string _Remarks { get; set; }
        /// <summary>
        /// 退/废票人数
        /// </summary>
        public int _Rnum { get; set; }
        /// <summary>
        /// 退/废票票号，多个以”| ”分割
        /// </summary>
        public string _TicketNo { get; set; }
        /// <summary>
        /// 退/废总金额
        /// </summary>
        public decimal _Ramount { get; set; }
        /// <summary>
        /// 服务商 ID
        /// </summary>
        public string _SystemId { get; set; }
    }
}
