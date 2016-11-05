using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;


namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{

    public class OrderDto
    {
        public OrderDto()
        {
            this.IsAuthSuc = true;
            this.AuthInfo = string.Empty;
        }
        public string RealRemark { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 外部(接口)订单号
        /// </summary>
        public string OutOrderId
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
        /// 订单佣金(总和)
        /// </summary>
        public decimal OrderCommissionTotalMoney
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
        /// 显示Pnr编码
        /// </summary>
        private string _PnrCode = string.Empty; //PNR显示隐藏功能实现处理
        public string ShowPnrCode
        {

            get
            {
                //if (OrderSource == EnumOrderSource.ChdPnrImport || OrderSource == EnumOrderSource.PnrImport || OrderSource == EnumOrderSource.PnrContentImport || OrderSource == EnumOrderSource.LineOrder) return _PnrCode;
                if (OrderStatus == 3 || OrderStatus == 5) return _PnrCode;
                //return "******";
                return OrderSource == EnumOrderSource.WhiteScreenDestine ? "******" : _PnrCode;
            }
            set { _PnrCode = PnrCode; }
        }
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode { get; set; }
        /// <summary>
        /// 0.成人订单 1儿童订单
        /// </summary>
        public string OrderType
        {
            get;
            set;
        }
        /// <summary>
        /// 编码类型 0普通编码 1团编码
        /// </summary>
        public string PnrType
        {
            get;
            set;
        }
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode
        {
            get;
            set;
        }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 锁定名称
        /// </summary>
        public string LockAccount { get; set; }
        public string Remark
        {
            get;
            set;
        }
        public int? OrderStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 获取订单状态（描述）
        /// </summary>
        public string OrderStatusStr
        {
            get;
            set;
        }
        public virtual PolicyDto Policy
        {
            get;
            set;
        }
        /// <summary>
        /// 协调是否完成
        /// </summary>
        public bool? IsCompleted { get; set; }
        /// <summary>
        /// 支付信息
        /// </summary>
        public virtual OrderPayDto PayInfo { get; set; }
        /// <summary>
        /// 是否有售后信息
        /// </summary>
        public bool HasAfterSale { get; set; }
        /// <summary>
        /// 编码来源 0本地生成 1编码内容导入
        /// </summary>
        public EnumPnrSource PnrSource { get; set; }

        /// <summary>
        /// 原订单号（升舱换开，或者儿童关联的成人订单号）
        /// </summary>
        public string OldOrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 订单来源
        /// </summary>
        public EnumOrderSource OrderSource { get; set; }
        /// <summary>
        /// 是否带有婴儿
        /// </summary>
        public bool HaveBabyFlag { get; set; }
        /// <summary>
        /// 是否换编码出票
        /// </summary> 
        public bool IsChangePnrTicket { get; set; }
        /// <summary>
        /// 多个价格(高低价格) true低价格(默认) false高价格
        /// </summary>
        public bool IsLowPrice
        {
            get;
            set;
        }

        public virtual IList<SkyWayDto> SkyWays
        {
            get;
            set;
        }
        public virtual IList<PassengerDto> Passengers
        {
            get;
            set;
        }
        public string PNRContent { get; set; }
        /// 预定编码Office
        /// </summary>
        public string YdOffice { get; set; }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CpOffice { get; set; }
        public int? Time
        {
            get;
            set;
        }
        public string CarrierCode { get; set; }
        /// <summary>
        /// 是否是代理人
        /// </summary>
        public bool IsSupplier { get; set; }
        /// <summary>
        /// 共享运营
        /// </summary>
        public bool IsCarrier { get; set; }
        /// <summary>
        /// 当前商户Code
        /// </summary>
        public string CurrentCode { get; set; }
        /// <summary>
        /// 出票金额
        /// </summary>
        public decimal CPMoney { get; set; }
        private string _ShowPassengerType = string.Empty;

        /// </summary>
        /// 订单类型标识 儿童 婴儿
        /// </summary>
        public string PassengerType
        {
            get
            {
                string passengertype = string.Empty;
                if (Passengers != null)
                {
                    if (Passengers.Count(p => p.PassengerType == EnumPassengerType.Child) > 0)
                        passengertype = "儿童";
                    else if (Passengers.Count(p => p.PassengerType == EnumPassengerType.Baby) > 0)
                        passengertype = "婴儿";
                }
                return passengertype;
            }
            set { _ShowPassengerType = value; }
        }
        /// <summary>
        /// Pnr内容导入的自动授权是否成功
        /// </summary>
        public bool IsAuthSuc
        {
            get;
            set;
        }
        /// <summary>
        /// 授权指令
        /// </summary>
        public string AuthInfo
        {
            get;
            set;
        }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime NowDateTime
        {
            get { return DateTime.Now; }
        }
        /// <summary>
        /// 当前用户角色
        /// </summary>
        public string UserRole
        {
            get;
            set;
        }
    }
}
