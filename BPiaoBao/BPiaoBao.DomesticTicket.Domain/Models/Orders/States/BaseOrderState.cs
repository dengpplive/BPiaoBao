using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{

    public abstract class BaseOrderState
    {
        public abstract Type[] GetBahaviorTypes();
        public Order Order { get; set; }
        public BaseOrderBehavior GetBehaviorByCode(string behaviorCode)
        {
            var t = GetBahaviorTypes().FirstOrDefault(p =>
                p.GetCustomAttributes(typeof(BehaviorAttribute), true).Where(
                    q => (q as BehaviorAttribute).BehaviorCode == behaviorCode).Count() > 0
                    );
            if (t == null)
                throw new OrderCommException(string.Format("当前状态:{0},不允许此操作,请刷新!", this.Order.OrderStatus.ToEnumDesc()));


            var behavior = Activator.CreateInstance(t) as BaseOrderBehavior;
            if (behavior == null)
                throw new OrderCommException("错误的订单行为");
            behavior.order = Order;
            return behavior;
        }
    }
    public class OrderStateAttribute : Attribute
    {
        public EnumOrderStatus OrderState { get; set; }
        public OrderStateAttribute(EnumOrderStatus orderState)
        {
            this.OrderState = orderState;
        }
    }

    ///// <summary>
    ///// 订单状态接口
    ///// </summary>
    //public interface IOrderState
    //{
    //    Order Order { get; set; }
    //    /// <summary>
    //    /// 当前状态
    //    /// </summary>
    //    EnumOrderStatus Status { get; }
    //    /// <summary>
    //    /// 可否支付
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanPay();
    //    /// <summary>
    //    /// 可否取消
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanCancel( );
    //    /// <summary>
    //    /// 取消订单
    //    /// </summary>
    //    /// <param name="order"></param>
    //    void Cancel( );
    //    /// <summary>
    //    /// 可否出票
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanIssue( );
    //    /// <summary>
    //    /// 可否代付
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanAidPay( );
    //    /// <summary>
    //    /// 可否退款
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanReimburse( );
    //    /// <summary>
    //    /// 可否拒绝出票
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanRepelIssue( );
    //    /// <summary>
    //    /// 可否出票并完成
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanIssueAndCompleted( );
    //    /// <summary>
    //    /// 可否拒绝出票并完成
    //    /// </summary>
    //    /// <returns></returns>
    //    bool CanRepelIssueAndCompleted( );
    //}
}
