using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Domain.Services;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework.DDD.Events;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using PnrAnalysis.Model;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 订单信息
    /// </summary>
    public class Order : EntityBase, IAggregationRoot
    {
        public Order()
        {
            this.OrderSource = EnumOrderSource.PnrContentImport;
            this.PnrSource = EnumPnrSource.ImportPnrContent;
        }
        #region 属性
        /// <summary>
        /// 票面总价(舱位价+机建费+燃油费)
        /// </summary>
        public decimal TicketPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 婴儿票面总价(舱位价+机建费+燃油费)
        /// </summary>
        public decimal INFTicketPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 0.成人订单 1儿童订单,2:婴儿
        /// </summary>
        public int OrderType
        {
            get;
            set;
        }
        /// <summary>
        /// 编码类型 0普通编码 1团编码
        /// </summary>
        public int PnrType
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
        public string PnrCode { get; set; }
        /// <summary>
        /// 新PNR编码
        /// </summary>
        public string NewPnrCode { get; set; }
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode { get; set; }

        /// <summary>
        /// Pnr内容
        /// </summary>
        public string PnrContent { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode
        {
            get;
            set;
        }
        /// <summary>
        /// 商户名
        /// </summary>
        public string BusinessmanName
        {
            get;
            set;
        }
        /// <summary>
        /// 操作员号
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 锁定账号
        /// </summary>
        public string LockAccount { get; set; }
        /// <summary>
        /// 接口订单号(代付订单号)
        /// </summary>
        public string OutOrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        public EnumOrderStatus OrderStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 订单备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 订单出票时间
        /// </summary>
        public DateTime? IssueTicketTime { get; set; }


        /// <summary>
        /// 政策信息
        /// </summary>
        public virtual Policy Policy
        {
            get;
            set;
        }


        /// <summary>
        /// 支付信息
        /// </summary>
        public virtual OrderPay OrderPay
        {
            get;
            set;
        }
        /// <summary>
        /// 航段
        /// </summary>
        public virtual IList<SkyWay> SkyWays
        {
            get;
            set;
        }
        /// <summary>
        /// 乘客
        /// </summary>
        public virtual IList<Passenger> Passengers
        {
            get;
            set;
        }
        /// <summary>
        /// 订单日志
        /// </summary>
        public virtual IList<OrderLog> OrderLogs
        {
            get;
            set;
        }
        /// <summary>
        /// 成人订单关联儿童人数 默认0
        /// </summary>
        public int AssocChdCount
        {
            get;
            set;
        }
        /// <summary>
        /// 自动出票次数从0开始
        /// </summary>
        public int? AutoEtdzCallCount
        {
            get;
            set;
        }
        /// <summary>
        /// B2b自动出票失败 调用的接口方法名 默认为空
        /// </summary>
        public string B2bFailLastCallMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 协调日志
        /// </summary>
        public virtual IList<CoordinationLog> CoordinationLogs
        {
            get;
            set;
        }
        //退款剩余交易
        public decimal RefundedTradeMoney { get; set; }

        public decimal RefundedServiceMoney { get; set; }
        /// <summary>
        /// 协调状态
        /// </summary>
        public bool? CoordinationStatus { get; set; }
        /// <summary>
        /// 售后信息
        /// </summary>
        //public virtual ICollection<AfterSaleOrder> AfterSaleOrders
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 是否有售后信息
        /// </summary>
        public bool HasAfterSale { get; set; }
        /// <summary>
        /// 售后产生总金额
        /// </summary>
        public decimal AfterSaleTotalMoney { get; set; }
        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 编码来源 0本地生成 1编码内容导入
        /// </summary>
        public EnumPnrSource PnrSource { get; set; }

        /// <summary>
        /// 预定编码Office
        /// </summary>
        public string YdOffice { get; set; }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CpOffice { get; set; }

        /// <summary>
        /// 出票方的金额
        /// </summary>
        public decimal CPMoney
        {
            get;
            set;
        }
        /// <summary>
        /// true 婴儿订单没有P到价格 false 获取到价格
        /// </summary>
        public bool InfNotGetPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 多个价格(高低价格) true低价格(默认) false高价格
        /// </summary>
        public bool IsLowPrice
        {
            get;
            set;
        }

        /// <summary>
        /// 控台分销分组ID
        /// </summary>
        public string StationBuyGroupID { get; set; }

        #endregion

        public BaseOrderState State
        {
            get
            {
                try
                {
                    var orderState = ObjectFactory.GetNamedInstance<BaseOrderState>(OrderStatus.ToString());
                    orderState.Order = this;
                    return orderState;
                }
                catch (Exception e)
                {
                    throw new CustomException(500, "该订单状态已发生变化，不能继续操作");
                }
            }
        }


        #region 行为
        public void ChangeStatus(EnumOrderStatus status)
        {
            if (this.OrderStatus != status)
            {
                var s = this.OrderStatus;
                this.OrderStatus = status;
                //引发订单状态改变的事件
                DomainEvents.Raise(new OrderStatusChangedEvent() { Order = this, OldStatus = s, NewStatus = status });
            }
        }

        public void WriteLog(OrderLog log)
        {
            if (this.OrderLogs == null)
            {
                this.OrderLogs = new List<OrderLog>();
            }
            this.OrderLogs.Add(log);
        }

        public void WriteCoordinationContent(CoordinationLog log)
        {
            if (this.CoordinationLogs == null)
            {
                this.CoordinationLogs = new List<CoordinationLog>();
            }
            this.CoordinationLogs.Add(log);
        }

        public void PayToPaid(string operatorName, EnumPayMethod? payMethod, string PayMethodCode, string serialNumber, string isNotify)
        {
            DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
            //支付成功后，修改日志。改变状态，进行代付
            this.OrderPay.PayDateTime = DateTime.Now;
            this.OrderPay.PaySerialNumber = serialNumber;
            this.OrderPay.PayMethod = payMethod.Value;
            this.OrderPay.PayMethodCode = PayMethodCode;
            this.OrderPay.PayStatus = EnumPayStatus.OK;
            this.ChangeStatus(EnumOrderStatus.PayWaitCreatePlatformOrder);
            //this.ChangeStatus(EnumOrderStatus.WaitAndPaid);
            //domesticService.SetInsuranceStatus(this.OrderId, EnumInsuranceStatus.PayOK);//设置保险单状态
            this.WriteLog(new OrderLog()
            {
                OperationPerson = operatorName,
                OperationDatetime = DateTime.Now,
                OperationContent = "支付方式:" + payMethod.ToEnumDesc() + ",支付成功",
                IsShowLog = true
            });
            StringBuilder sbLog = new StringBuilder();
            try
            {
                sbLog.AppendFormat("支付到代付 时间:{0} OrderId={1} serialNumber={2} isNotify={3} 支付方式:{4} operatorName={5}\r\n",
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), this.OrderId, serialNumber, isNotify, payMethod.ToEnumDesc(), operatorName);
                //设置其他相同PNR订单无效
                domesticService.SetOrderStatusInvalid(operatorName, this.PnrCode, this.OrderId);
                #region 代付
                //接口的单子进行代付
                if (this.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                {
                    //没有支付成功也没有代付成功 再调用代付接口
                    //if (this.OrderPay.PayStatus == EnumPayStatus.OK
                    //    && this.OrderPay.PaidStatus == EnumPaidStatus.NoPaid)
                    //{                   
                    //    //调用代付
                    //    var behavior = this.State.GetBehaviorByCode("PaidOrder");
                    //    behavior.SetParame("areaCity", this.Policy.AreaCity);
                    //    behavior.SetParame("PlatformCode", this.Policy.PlatformCode);
                    //    behavior.SetParame("operatorName", operatorName);
                    //    behavior.SetParame("isNotify", isNotify);
                    //    behavior.Execute();
                    //}
                }
                else
                {
                    //修改为已支付 等待出票
                    this.ChangeStatus(EnumOrderStatus.WaitIssue);
                    BPiaoBao.Common.WebMessageManager.GetInstance().Send(EnumMessageCommand.PayWaitIssueTicket, this.Policy.Code, string.Format("订单{0}已支付完成，请及时出票", this.OrderId));


                }
                #endregion
            }
            catch (Exception ex)
            {
                sbLog.Append(" 异常信息=" + ex.Message + "\r\n");
            }
            finally
            {
                Logger.WriteLog(LogType.INFO, sbLog.ToString());
            }
        }


        #endregion

        protected override string GetIdentity()
        {
            return this.OrderId;
        }
    }
}
