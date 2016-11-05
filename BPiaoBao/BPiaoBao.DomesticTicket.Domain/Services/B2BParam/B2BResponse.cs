using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace BPiaoBao.DomesticTicket.Domain.Services.B2BParam
{
    public class B2BResponse
    {
        private string lastCallMethod = string.Empty;
        /// <summary>
        /// 上次失败调用接口方法名
        /// </summary>
        public string LastCallMethod
        {
            get { return lastCallMethod; }
            set { lastCallMethod = value; }
        }

        private string _RetuenXML = string.Empty;
        /// <summary>
        /// 请求返回来的XML数据
        /// </summary>
        public string RetuenXML
        {
            get { return _RetuenXML; }
            set { _RetuenXML = value; }
        }

        private string _Remark = string.Empty;
        /// <summary>
        /// 本票通错误描述
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        private DataSet _DsResult = null;
        /// <summary>
        /// 本票通返回XML的数据集
        /// </summary>
        public DataSet DsResult
        {
            get { return _DsResult; }
            set { _DsResult = value; }
        }
        private bool _Status = false;
        /// <summary>
        /// 操作状态 true 成功 false失败
        /// </summary>
        public bool Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        private TicketNofityInfo _TicketNofityInfo = new TicketNofityInfo();
        /// <summary>
        /// 异步出票返回信息 出票通知信息
        /// </summary>
        public TicketNofityInfo TicketNofityInfo
        {
            get { return _TicketNofityInfo; }
            set { _TicketNofityInfo = value; }
        }

        private PayNotifyInfo _PayNotifyInfo = null;
        /// <summary>
        /// 支付通知信息
        /// </summary>
        public PayNotifyInfo PayNotifyInfo
        {
            get { return _PayNotifyInfo; }
            set { _PayNotifyInfo = value; }
        }
    }

    public class NotifyInfo
    {
        private bool _code = false;
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private string _Pnr = string.Empty;
        /// <summary>
        /// 编码
        /// </summary>
        public string Pnr
        {
            get { return _Pnr; }
            set { _Pnr = value; }
        }
        private string _OrderStatus = string.Empty;
        /// <summary>
        /// 航空公司订单状态
        /// </summary>
        public string OrderStatus
        {
            get { return _OrderStatus; }
            set { _OrderStatus = value; }
        }
        private string _PayStatus = string.Empty;
        /// <summary>
        /// 航空公司订单支付状态
        /// </summary>
        public string PayStatus
        {
            get { return _PayStatus; }
            set { _PayStatus = value; }
        }
        private decimal _PayPrice = 0m;
        /// <summary>
        /// 支付价格
        /// </summary>
        public decimal PayPrice
        {
            get { return _PayPrice; }
            set { _PayPrice = value; }
        }
        private string _TradeNo = string.Empty;
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string TradeNo
        {
            get { return _TradeNo; }
            set { _TradeNo = value; }
        }
        private string _FlatformOrderId = string.Empty;
        /// <summary>
        /// 第三方合作商 订单号(my票宝订单号)
        /// </summary>
        public string FlatformOrderId
        {
            get { return _FlatformOrderId; }
            set { _FlatformOrderId = value; }
        }
        private string _Message = string.Empty;
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }
    }

    /// <summary>
    /// 出票的返回信息
    /// </summary>
    public class TicketNofityInfo : NotifyInfo
    {
        private string _OrderNo = string.Empty;
        /// <summary>
        /// 航空公司订单号
        /// </summary>
        public string OrderNo
        {
            get { return _OrderNo; }
            set { _OrderNo = value; }
        }
        private List<AutoTicketInfo> _AutoTicketList = new List<AutoTicketInfo>();
        /// <summary>
        /// 出票成功后 异步取回的票号信息
        /// </summary>
        public List<AutoTicketInfo> AutoTicketList
        {
            get { return _AutoTicketList; }
            set { _AutoTicketList = value; }
        }
    }

    public class AutoTicketInfo
    {
        /// <summary>
        /// 乘机人名称
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber { get; set; }
    }


    /// <summary>
    /// 航空公司支付成功后的支付通知信息
    /// </summary>
    public class PayNotifyInfo : NotifyInfo
    {
        private string _OutTradeNo = string.Empty;
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string OutTradeNo
        {
            get { return _OutTradeNo; }
            set { _OutTradeNo = value; }
        }
    }
}
