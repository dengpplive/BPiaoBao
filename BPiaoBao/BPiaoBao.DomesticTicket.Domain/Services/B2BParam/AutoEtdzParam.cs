using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Services.B2BParam
{
    public class AutoEtdzParam
    {
        public AutoEtdzParam()
        {
        }

        private int _UseAutoType = 0;
        /// <summary>
        /// 开始使用自动方式 默认使用OrderEx 1使用查政策方式
        /// </summary>
        public int UseAutoType
        {
            get { return _UseAutoType; }
            set { _UseAutoType = value; }
        }
        private string _FlatformOrderId = string.Empty;
        /// <summary>
        /// 第三方合作商（平台订单号） 订单号(my票宝订单号)
        /// </summary>
        public string FlatformOrderId
        {
            get { return _FlatformOrderId; }
            set { _FlatformOrderId = value; }
        }
        private string _CarryCode = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string CarryCode
        {
            get { return _CarryCode; }
            set { _CarryCode = value; }
        }
        private string _B2BAccount = string.Empty;
        /// <summary>
        /// B2B网站登陆账号
        /// </summary>
        public string B2BAccount
        {
            get { return _B2BAccount; }
            set { _B2BAccount = value; }
        }
        private string _B2BPwd = string.Empty;

        /// <summary>
        /// B2B网站登陆密码
        /// </summary>
        public string B2BPwd
        {
            get { return _B2BPwd; }
            set { _B2BPwd = value; }
        }
        private string _Pnr = string.Empty;
        /// <summary>
        /// 小编码
        /// </summary>
        public string Pnr
        {
            get { return _Pnr; }
            set { _Pnr = value; }
        }
        private string _BigPnr = string.Empty;
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigPnr
        {
            get { return _BigPnr; }
            set { _BigPnr = value; }
        }
        private bool _IsMulPrice = false;
        /// <summary>
        /// 编码中是否有多个出票价格
        /// </summary>
        public bool IsMulPrice
        {
            get { return _IsMulPrice; }
            set { _IsMulPrice = value; }
        }
        private bool _IsLimitScope = false;
        /// <summary>
        /// 多个价格是否采用限制金额范围取政策 否则采用指定的票面支付总价获取政策
        /// </summary>
        public bool IsLimitScope
        {
            get { return _IsLimitScope; }
            set { _IsLimitScope = value; }
        }

        private decimal _OldPolicyPoint = 0m;
        /// <summary>
        /// 原始政策点数(代理人在平台投放的政策点数)
        /// </summary>
        public decimal OldPolicyPoint
        {
            get { return _OldPolicyPoint; }
            set { _OldPolicyPoint = value; }
        }

        private string _LastCallMethod = string.Empty;
        /// <summary>
        /// 上次全自动失败调用接口方法名
        /// </summary>
        public string LastCallMethod
        {
            get { return _LastCallMethod; }
            set { _LastCallMethod = value; }
        }

        private PayInfo _PayInfo = new PayInfo();
        /// <summary>
        /// 自动支付信息
        /// </summary>
        public PayInfo PayInfo
        {
            get { return _PayInfo; }
            set { _PayInfo = value; }
        }

        private UrlInfo _UrlInfo = new UrlInfo();
        /// <summary>
        /// Url信息
        /// </summary>
        public UrlInfo UrlInfo
        {
            get { return _UrlInfo; }
            set { _UrlInfo = value; }
        }

        private Dictionary<string, string> _ExParam = new Dictionary<string, string>();
        /// <summary>
        /// 扩展参数
        /// </summary>
        public Dictionary<string, string> ExParam
        {
            get { return _ExParam; }
            set { _ExParam = value; }
        }

        private string _RequestUrl = string.Empty;
        /// <summary>
        /// 记录接口内部请求URL数据 不用传值
        /// </summary>
        public string RequestUrl
        {
            get { return _RequestUrl; }
            set { _RequestUrl = value; }
        }
        private bool _ParamIsPass = false;
        /// <summary>
        /// 请求参数是否通过
        /// </summary>
        public bool ParamIsPass
        {
            get { return _ParamIsPass; }
            set { _ParamIsPass = value; }
        }
    }
    public class PayInfo
    {
        private string _PayAccount = string.Empty;

        /// <summary>
        /// 支付宝自动支付账号
        /// </summary>
        public string PayAccount
        {
            get { return _PayAccount; }
            set { _PayAccount = value; }
        }
        private decimal _SeatTotalPrice = 0m;
        /// <summary>
        /// 舱位价总和 （舱位价）*出票乘客人数
        /// </summary>
        public decimal SeatTotalPrice
        {
            get { return _SeatTotalPrice; }
            set { _SeatTotalPrice = value; }
        }
        private decimal _TaxTotalPrice = 0m;
        /// <summary>
        /// 税费总和 机建和燃油费总和 （机建+燃油）*出票乘客人数
        /// </summary>
        public decimal TaxTotalPrice
        {
            get { return _TaxTotalPrice; }
            set { _TaxTotalPrice = value; }
        }
        private decimal _PayTotalPrice = 0m;
        /// <summary>
        /// 票面支付总和(舱位价+机建+燃油)*出票乘客人数
        /// </summary>
        public decimal PayTotalPrice
        {
            get { return _PayTotalPrice; }
            set { _PayTotalPrice = value; }
        }

        private decimal _OnlyAdultSeatPrice = 0m;
        /// <summary>
        /// 单独成人票面价 （舱位价）
        /// </summary>
        public decimal OnlyAdultSeatPrice
        {
            get { return _OnlyAdultSeatPrice; }
            set { _OnlyAdultSeatPrice = value; }
        }
        private decimal _OnlyAdultTaxPrice = 0m;
        /// <summary>
        /// 单独成人税费
        /// </summary>
        public decimal OnlyAdultTaxPrice
        {
            get { return _OnlyAdultTaxPrice; }
            set { _OnlyAdultTaxPrice = value; }
        }

        private decimal _AppointPayPrice = 0m;
        /// <summary>
        /// 使用指定的支付价格支付 （多个价格时）
        /// </summary>
        public decimal AppointPayPrice
        {
            get { return _AppointPayPrice; }
            set { _AppointPayPrice = value; }
        }
    }

    public class UrlInfo
    {
        private string _AlipayAutoCPUrl = string.Empty;
        /// <summary>
        /// 请求本票通自动出票的Url地址 不可空
        /// </summary>
        public string AlipayAutoCPUrl
        {
            get { return _AlipayAutoCPUrl; }
            set { _AlipayAutoCPUrl = value; }
        }
        private string _AlipayTicketNotifyUrl = string.Empty;
        /// <summary>
        /// 本票通出票通知Url 不可空
        /// </summary>
        public string AlipayTicketNotifyUrl
        {
            get { return _AlipayTicketNotifyUrl; }
            set { _AlipayTicketNotifyUrl = value; }
        }
        private string _AlipayPayNotifyUrl = string.Empty;
        /// <summary>
        /// 本票通支付通知Url 可空
        /// </summary>
        public string AlipayPayNotifyUrl
        {
            get { return _AlipayPayNotifyUrl; }
            set { _AlipayPayNotifyUrl = value; }
        }
        private string _AlipayPayReturnUrl = string.Empty;
        /// <summary>
        /// 本票通支付同步返回Url地址 可空
        /// </summary>
        public string AlipayPayReturnUrl
        {
            get { return _AlipayPayReturnUrl; }
            set { _AlipayPayReturnUrl = value; }
        }
    }
}
