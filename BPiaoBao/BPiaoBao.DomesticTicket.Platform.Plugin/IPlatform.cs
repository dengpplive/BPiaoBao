
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using PnrAnalysis;



namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public interface IPlatform
    {
        bool IsClosed { get; }
        string Code { get; }
        string GetPlatformName();
        List<PlatformPolicy> GetPoliciesByPnrContent(string pnrContent, bool IsLowPrice, PnrData pnrData);
        PlatformOrder CreateOrder(string pnrContent, bool IsLowPrice, string areaCity, string policyId, string RateId, string localOrderId, decimal policyPoint, decimal ReturnMoney, BPiaoBao.Common.PnrData pnrData);
        void Pay(string areaCity, PlatformOrder order);
        bool CanCancelOrder { get; }
        void CancelOrder(string areaCity, string outOrderId, string pnr, string CancelRemark, string passengerName);
        string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr);
        Dictionary<string, string> AutoCompositeTicket(string areaCity, string orderId, string outOrderId, string pnr);
        bool IsPaid(string areaCity, string orderId, string outOrderId, string pnr);
        RefundTicketResult BounceOrAnnulTicket(RefundArgs refundArgs);

    }
    public class RefundTicketResult {
        /// <summary>
        /// 提交状态【true：成功，false：失败】
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 返回结果描述
        /// </summary>
        public string Descript { get; set; }
    }

    public class RefundArgs
    {
        /// <summary>
        ///  订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 接口订单号
        /// </summary>
        public string OutOrderId { get; set; }
        /// <summary>
        /// 运营商城市区域
        /// </summary>
        public string areaCity { get; set; }
        /// <summary>
        /// 退废类型[0:退票，1：废票...]
        /// </summary>
        public int RefundType { get; set; }
        /// <summary>
        /// 票款类型：true：退票退款 false：退款不退票
        /// </summary>
        public bool RefundMoneyType { get; set; }
        /// <summary>
        /// 退款理由ID【517|今日|百拓|票盟】
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 退废理由
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
        /// <summary>
        /// PNR编码
        /// </summary>
        public string PnrCode { get; set; }
        /// <summary>
        /// 自否自愿[true:自愿 false：非自愿]
        /// </summary>
        public bool? IsVoluntary { get; set; }
        /// <summary>
        /// 是否取消座位 [true 取消,false 不取消]
        /// </summary>
        public bool IsCancelSeat { get; set; }
        /// <summary>
        /// 退废乘机人
        /// </summary>
        public List<RefundPassenger> Passengers { get; set; }
        /// <summary>
        /// 航段
        /// </summary>
        public List<RefundSky> Sky { get; set; }
    }
    public class RefundPassenger
    {
        public RefundPassenger() { this.Repeal = 0; }
        /// <summary>
        /// 乘机人类型
        /// </summary>
        public EnumPassengerType PassengerType { get; set; }
        /// <summary>
        /// 乘机人名称
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNo { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 乘客退/废票标识  0：不变   1：废票   2：退票  
        /// </summary>
        public int Repeal { get; set; }
        /// <summary>
        /// 单人 退废票金额
        /// </summary>
        public decimal Amount { get; set; }
    }
    public class RefundSky
    {
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCode { get; set; }
    }
}
