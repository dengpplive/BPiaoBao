using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 售后乘机人
    /// </summary>
    public class AfterSalePassenger : EntityBase
    {
        public int Id { get; set; }
        /// <summary>
        /// 原乘机人编号
        /// </summary>
        public int PassengerId { get; set; }
        public virtual Passenger Passenger { get; private set; }
        /// <summary>
        /// 产生金额
        /// </summary>
        public Decimal RetirementMoney { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal RetirementPoundage { get; set; }
        /// <summary>
        /// 是否退款[false：未退，true:已退]
        /// </summary>
        public bool IsRefund { get; set; }
        /// <summary>
        /// 售后乘机人状态
        /// </summary>
        public EnumTfgPassengerStatus Status { get; set; }
        /// <summary>
        /// 行程单状态
        /// </summary>
        public EnumPassengerTripStatus PassengerTripStatus { get; set; }
        /// <summary>
        /// 改签后的行程单号
        /// </summary>
        public string AfterSaleTravelNum { get; set; }
        /// <summary>
        /// 改签后的票号
        /// </summary>
        public string AfterSaleTravelTicketNum
        {
            get;
            set;
        }

        protected override string GetIdentity()
        {
            return Id.ToString();
        }
    }
}
