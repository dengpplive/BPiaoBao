using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 订单行为接口
    /// </summary>
    public abstract class BaseOrderBehavior
    {
        Dictionary<string, object> parames = new Dictionary<string, object>();
        internal object getParame(string name)
        {
            var o = parames[name];
            if (o == null)
                throw new OrderCommException("没有找到name为" + name + "的订单行为参数");
            return o;
        }
        internal T getParame<T>(string name)
        {
            return (T)getParame(name);
        }
        internal Order order { get; set; }

        public virtual string BehaviorCode { get; set; }
        public virtual void SetParame(OrderBehaviorParameter parameter)
        {
            this.parames.Add(parameter.Name, parameter.Value);
        }
        public virtual void SetParame(string name, object value)
        {
            this.parames.Add(name, value);
        }

        public abstract object Execute();
    }

    public class BehaviorAttribute : Attribute
    {
        public string BehaviorCode { get; set; }
        public BehaviorAttribute(string behaviorCode)
        {
            this.BehaviorCode = behaviorCode;
        }
    }
    public struct OrderBehaviorParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
