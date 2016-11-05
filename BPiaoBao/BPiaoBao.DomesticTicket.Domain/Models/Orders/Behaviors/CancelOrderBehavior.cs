using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 取消订单操作
    /// </summary>
    [Behavior("CancelOrder")]
    public class CancelOrderBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            string operatorName = getParame("operatorName").ToString();

            #region 接口取消订单信息
            string Message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(order.OutOrderId)
                    && order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                {
                    var plateform = ObjectFactory.GetNamedInstance<IPlatform>(order.Policy.PlatformCode);
                    StringBuilder sbPassenger = new StringBuilder();
                    foreach (var item in order.Passengers)
                    {
                        sbPassenger.Append(item.PassengerName + "^");
                    }
                    plateform.CancelOrder(order.Policy.AreaCity, order.OutOrderId, order.PnrCode, "", sbPassenger.ToString().Trim(new char[] { '^' }));
                    Message = "接口信息：取消成功";
                }
            }
            catch (Exception ex)
            {
                Message = "接口信息：" + ex.Message;
            }
            #endregion

            order.ChangeStatus(EnumOrderStatus.OrderCanceled);
            order.WriteLog(new OrderLog()
            {
                OperationContent = "订单号" + order.OrderId + "于" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "取消订单," + Message,
                OperationDatetime = System.DateTime.Now,
                OperationPerson = operatorName,
                IsShowLog = false
            });

            order.WriteLog(new OrderLog()
            {
                OperationContent = "订单号" + order.OrderId + "于" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "取消订单",
                OperationDatetime = System.DateTime.Now,
                OperationPerson = operatorName,
                IsShowLog = true
            });
            return null;
        }
    }
}
