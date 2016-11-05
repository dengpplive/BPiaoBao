using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;

namespace BPiaoBao.Common
{
    /// <summary>
    /// 消息队列管理【专用队列】
    /// </summary>
    public class MessageQueueManager
    {
        private const string messagePath = @".\Private$\OrderMessage";
        public static void SendMessage(string orderId,int orderType)
        {
            MessageQueue mq = null;
            if (MessageQueue.Exists(messagePath))
                mq = new MessageQueue(messagePath);
            else
                mq = MessageQueue.Create(messagePath);
            mq.Send(new OrderMessage { OrderID = orderId, OrderType = orderType });
        }
        
    }
    [Serializable]
    public class OrderMessage
    {
        /// <summary>
        /// 0:出票订单 1:售后订单
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
    }
}
