using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 售后订单【聚合根】
    /// </summary>
    public abstract class AfterSaleOrder : EntityBase, IAggregationRoot
    {
        public int Id { get; set; }
        /// <summary>
        /// 售后类型
        /// </summary>
        public abstract string AfterSaleType { get; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public EnumTfgProcessStatus ProcessStatus { get; set; }
        /// <summary>
        /// 乘机人
        /// </summary>
        public virtual ICollection<AfterSalePassenger> Passenger { get; set; }
        /// <summary>
        /// 日志
        /// </summary>
        public virtual ICollection<OrderLog> Logs { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string CreateMan { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 产生金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcessDate { get; set; }
        /// <summary>
        /// 处理备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
        public virtual Order Order { get; set; }
        /// <summary>
        /// 锁定帐号
        /// </summary>
        public string LockCurrentAccount { get; set; }
        /// <summary>
        /// 协调是否完成
        /// </summary>
        public bool? IsCoorCompleted { get; set; }
        /// <summary>
        /// 【完成时间】
        /// </summary>
        public DateTime? CompletedTime { get; set; }
        /// <summary>
        /// 接口订单号
        /// </summary>
        public string OutOrderId { get; set; }
        /// <summary>
        /// 理由ID
        /// </summary>
        public int ReasonID { get; set; }
        public virtual ICollection<CoordinationLog> CoordinationLogs { get; set; }

        public decimal TotalMoney
        {
            get
            {
                return this.Passenger.Sum(p => p.Passenger.PayMoney);

            }
        }
        /// <summary>
        /// 出票售后金额
        /// </summary>
        public decimal AfterCPMoney
        {
            get
            {
                return this.Passenger.Sum(x => x.Passenger.CPMoney);
            }
        }
        public decimal AfterMoney
        {
            get {
                return this.AfterCPMoney - this.Passenger.Sum(x => x.RetirementPoundage);
            }
        }

        /// <summary>
        /// 控台分销分组ID
        /// </summary>
        public string StationBuyGroupID { get; set; }

        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }
        public void WriteLog(OrderLog orderLog)
        {
            if (this.Logs == null)
                this.Logs = new List<OrderLog>();
            this.Logs.Add(orderLog);
        }
        /// <summary>
        /// 申请验证规则
        /// </summary>
        public virtual void CheckRule(List<AfterSaleOrder> aftersaleOrders)
        {
            /*
             1,是否可以申请售后
             */
            if (aftersaleOrders != null && aftersaleOrders.Count != 0)
            {
                foreach (var item in aftersaleOrders)
                {
                    foreach (var currentitem in this.Passenger)
                    {
                        item.Passenger.Where(p => p.PassengerId == currentitem.PassengerId).ToList().ForEach(n =>
                        {
                            if ((item is AnnulOrder || item is BounceOrder) && item.ProcessStatus == EnumTfgProcessStatus.Processed)
                                throw new CustomException(500, string.Format("{0}已经{1},申请失败!", n.Passenger.PassengerName, item.AfterSaleType));
                            if (n.Status == EnumTfgPassengerStatus.Apply || n.Status == EnumTfgPassengerStatus.Processing)
                                throw new CustomException(500, string.Format("{0}已申请{1}", n.Passenger.PassengerName, item.AfterSaleType));
                        });
                    }
                }
            }

            /*
             2,可以申请-->更改乘机人状态
             */
            this.Passenger.ToList().ForEach(p => p.Status = EnumTfgPassengerStatus.Apply);
        }

    }

}
