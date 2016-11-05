using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper.QueryableExtensions;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
 
namespace BPiaoBao.AppServices.DomesticTicket
{
    public partial class OrderService : IConsoOrderService
    {

        public PagedList<ResponseConsoOrder> FindOrder(string orderid, string pnr, string passengerName, string code, int currentIndex, int pageSize)
        {
            var currentCode = AuthManager.GetCurrentUser().Code;
            var query = this.orderRepository.FindAllNoTracking(p => p.Policy.Code == currentCode && p.Policy.PolicySourceType != EnumPolicySourceType.Interface && (p.OrderStatus == EnumOrderStatus.WaitIssue || p.OrderStatus == EnumOrderStatus.WaitReimburseWithRepelIssue || p.OrderStatus == EnumOrderStatus.ApplyBabyFail || p.OrderStatus==EnumOrderStatus.AutoIssueFail));
            if (!string.IsNullOrWhiteSpace(orderid))
                query = query.Where(p => p.OrderId == orderid.Trim());
            if (!string.IsNullOrWhiteSpace(pnr))
                query = query.Where(p => p.PnrCode == pnr.Trim());
            if (!string.IsNullOrWhiteSpace(passengerName))
                query = query.Where(p => p.Passengers.Any(x => x.PassengerName == passengerName.Trim()));
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(p => p.BusinessmanCode == code.Trim());
            List<Order> orderList = query.OrderByDescending(p => p.CreateTime).Skip((currentIndex - 1) * pageSize).Take(pageSize).ToList();
            var list = AutoMapper.Mapper.Map<List<Order>, List<ResponseConsoOrder>>(orderList);
            return new PagedList<ResponseConsoOrder>()
            {
                Total = query.Count(),
                Rows = list
            };
        }

        /// <summary>
        /// 待处理售后订单查询
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="pnr">pnr</param>
        /// <param name="passengerName">乘机人</param>
        /// <param name="code">商户号</param>
        /// <param name="payNum">交易号</param>
        /// <param name="startDate">查询时间-开始</param>
        /// <param name="endDate">查询时间-结束</param>
        /// <param name="PolicyType">政策类型</param>
        /// <param name="AfterSaleType">申请类型</param>、
        /// <param name="lockAccount">锁定当前账户</param>
        /// <param name="currentIndex">当前页</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public PagedList<ResponseConsoSaleOrder> FindSaleOrder(string orderid, string pnr, string passengerName, string code, string payNum, EnumTfgProcessStatus? status, DateTime? startDate, DateTime? endDate, string PolicyType, string AfterSaleType,string lockAccont, int currentIndex, int pageSize)
        {
            var currentCode = AuthManager.GetCurrentUser().Code;
            var query = this.afterSaleOrderRepository.FindAllNoTracking(p => p.Order.Policy.Code == currentCode && p.Order.Policy.PolicySourceType != EnumPolicySourceType.Interface && (p.ProcessStatus != EnumTfgProcessStatus.Processed && p.ProcessStatus != EnumTfgProcessStatus.RepelProcess));
            if (!string.IsNullOrWhiteSpace(orderid))
                query = query.Where(p => p.OrderID == orderid.Trim());
            if (!string.IsNullOrWhiteSpace(pnr))
                query = query.Where(p => p.Order.PnrCode == pnr.Trim());
            if (!string.IsNullOrWhiteSpace(passengerName))
                query = query.Where(p => p.Passenger.Any(x => x.Passenger.PassengerName == passengerName.Trim()));
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(p => p.Order.BusinessmanCode == code.Trim());
            if (!string.IsNullOrWhiteSpace(payNum))
                query = query.Where(p => p.Order.OrderPay.PaySerialNumber == payNum.Trim());
            if (status.HasValue)
                query = query.Where(p => p.ProcessStatus == status.Value);
            if (startDate.HasValue)
                query = query.Where(p=>p.CreateTime >= startDate);
            if (endDate.HasValue)
                query = query.Where(p=>p.CreateTime <= endDate);
            if (!string.IsNullOrEmpty(PolicyType))
                query = query.Where(p => p.Order.Policy.PolicyType == PolicyType);
            if (!string.IsNullOrEmpty(lockAccont))
                query = query.Where(p=>p.LockCurrentAccount == lockAccont);
            switch (AfterSaleType)
            {
                case "改签": query = query.OfType<ChangeOrder>(); break;
                case "废票": query = query.OfType<AnnulOrder>(); break;
                case "退票": query = query.OfType<BounceOrder>(); break;
                case "其他修改": query = query.OfType<ModifyOrder>(); break;
            }
            List<AfterSaleOrder> rsales = query.OrderByDescending(p => p.CreateTime).Skip((currentIndex - 1) * pageSize).Take(pageSize).ToList();
            var list = AutoMapper.Mapper.Map<List<AfterSaleOrder>, List<ResponseConsoSaleOrder>>(rsales);
           
            return new PagedList<ResponseConsoSaleOrder>()
                {
                Total = query.Count(),
                Rows = list
            };
        }

        public PagedList<ResponseConsoOrder> FindConsoAllOrder(AllOrderSearch allOrderSearch,int Page,int Rows)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var query = this.orderRepository.FindAllNoTracking(p => (p.CarrierCode.Equals(code) || p.Policy.CarrierCode.Equals(code) || p.Policy.Code.Equals(code)));// && p.OrderStatus != EnumOrderStatus.WaitChoosePolicy && p.OrderStatus != EnumOrderStatus.Invalid && p.OrderStatus != EnumOrderStatus.OrderCanceled
            if (!string.IsNullOrWhiteSpace(allOrderSearch.OrderID))
                query = query.Where(p => p.OrderId.Equals(allOrderSearch.OrderID.Trim()));
            if (!string.IsNullOrWhiteSpace(allOrderSearch.PNR))
                query = query.Where(p => p.PnrCode.Equals(allOrderSearch.PNR.Trim()));
            if (!string.IsNullOrWhiteSpace(allOrderSearch.PassengerName))
                query = query.Where(p => p.Passengers.Any(x => x.PassengerName.Equals(allOrderSearch.PassengerName.Trim())));
            if (!string.IsNullOrWhiteSpace(allOrderSearch.TicketNum))
                query = query.Where(p => p.Passengers.Any(x => x.TicketNumber.Equals(allOrderSearch.TicketNum.Trim())));
            if (!string.IsNullOrWhiteSpace(allOrderSearch.BusinessmanCode))
                query = query.Where(p => p.BusinessmanCode.Equals(allOrderSearch.BusinessmanCode.Trim()));
            if (!string.IsNullOrWhiteSpace(allOrderSearch.PaySerialNumber))
                query = query.Where(p => p.OrderPay.PaySerialNumber.Equals(allOrderSearch.PaySerialNumber.Trim()));
            if (!string.IsNullOrWhiteSpace(allOrderSearch.CarrayCode))
                query = query.Where(p => p.SkyWays.Any(x => x.CarrayCode.Equals(allOrderSearch.CarrayCode.Trim())));
            if (allOrderSearch.OrderStatus.HasValue)
                query = query.Where(p => p.OrderStatus == allOrderSearch.OrderStatus.Value);
            if (!string.IsNullOrEmpty(allOrderSearch.PolicyType))
                query = query.Where(p => p.Policy.PolicyType == allOrderSearch.PolicyType);
            if (allOrderSearch.PolicysSourceType.HasValue)
                query = query.Where(p=>p.Policy.PolicySourceType == allOrderSearch.PolicysSourceType.Value);
            List<Order> orderList = query.OrderByDescending(p => p.CreateTime).Skip((Page - 1) * Rows).Take(Rows).ToList();
            return new PagedList<ResponseConsoOrder>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<Order>, List<ResponseConsoOrder>>(orderList)
            };
        }

        public PagedList<ResponseConsoSaleOrder> FindConsoAllSaleOrder(AllSaleOrderSearch allSaleOrderSearch)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var query = this.afterSaleOrderRepository.FindAllNoTracking(p => p.Order.CarrierCode.Equals(code) || p.Order.Policy.CarrierCode.Equals(code) || p.Order.Policy.Code.Equals(code));
            if (!string.IsNullOrWhiteSpace(allSaleOrderSearch.OrderID))
                query = query.Where(p => p.OrderID.Equals(allSaleOrderSearch.OrderID.Trim()));
            if (!string.IsNullOrWhiteSpace(allSaleOrderSearch.PNR))
                query = query.Where(p => p.Order.PnrCode.Equals(allSaleOrderSearch.PNR.Trim()));
            if (!string.IsNullOrWhiteSpace(allSaleOrderSearch.BusinessmanCode))
                query = query.Where(p => p.Order.BusinessmanCode.Equals(allSaleOrderSearch.BusinessmanCode.Trim()));
            if (!string.IsNullOrWhiteSpace(allSaleOrderSearch.PaySerialNumber))
                query = query.Where(p => p.Order.OrderPay.PaySerialNumber.Equals(allSaleOrderSearch.PaySerialNumber.Trim()));
            if (allSaleOrderSearch.ProcessStatus.HasValue)
                query = query.Where(p => p.ProcessStatus == allSaleOrderSearch.ProcessStatus.Value);
            if (!string.IsNullOrWhiteSpace(allSaleOrderSearch.PassengerName))
                query = query.Where(p => p.Passenger.Any(x => x.Passenger.PassengerName.Equals(allSaleOrderSearch.PassengerName.Trim())));
            List<AfterSaleOrder> afterSaleOrderList = query.OrderByDescending(p => p.CreateTime).Skip((allSaleOrderSearch.Page - 1) * allSaleOrderSearch.Rows).Take(allSaleOrderSearch.Rows).ToList();
            return new PagedList<ResponseConsoSaleOrder>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<AfterSaleOrder>, List<ResponseConsoSaleOrder>>(afterSaleOrderList)
            };
        }


        public void AddOrderCoordination(string orderid, bool IsCompleted, string Type, string Content)
        {
            var order = this.orderRepository.FindAll(p => p.OrderId.Equals(orderid)).FirstOrDefault();
            if (order == null)
                throw new CustomException(404, string.Format("未找到订单:{0}", orderid));
            order.CoordinationLogs.Add(new CoordinationLog
            {
                AddDatetime = DateTime.Now,
                Content = Content,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount,
                Type = Type
            });
            order.CoordinationStatus = IsCompleted;
            this.unitOfWorkRepository.PersistUpdateOf(order);
            this.unitOfWork.Commit();
        }

        public List<ConsoOrderCoordination> GetOrderCoordination(string orderid)
        {
            var order = this.orderRepository.FindAll(p => p.OrderId.Equals(orderid)).FirstOrDefault();
            if (order == null)
                return new List<ConsoOrderCoordination>();
            return AutoMapper.Mapper.Map<List<CoordinationLog>, List<ConsoOrderCoordination>>(order.CoordinationLogs.ToList());
        }


        public void AddSaleOrderCoordination(int saleorderid, bool IsCompleted, string Type, string Content)
        {
            var saleOrder = this.afterSaleOrderRepository.FindAll(p => p.Id.Equals(saleorderid)).FirstOrDefault();
            if (saleOrder == null)
                throw new CustomException(404, string.Format("未找到售后订单"));
            saleOrder.CoordinationLogs.Add(new CoordinationLog
            {
                AddDatetime = DateTime.Now,
                Content = Content,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount,
                Type = Type
            });
            saleOrder.IsCoorCompleted = IsCompleted;
            this.unitOfWorkRepository.PersistUpdateOf(saleOrder);
            this.unitOfWork.Commit();
        }

        public List<ConsoOrderCoordination> GetSaleOrderCoordination(int saleorderid)
        {
            var saleOrder = this.afterSaleOrderRepository.FindAll(p => p.Id.Equals(saleorderid)).FirstOrDefault();
            if (saleOrder == null)
                throw new CustomException(404, string.Format("未找到售后订单"));
            return AutoMapper.Mapper.Map<List<CoordinationLog>, List<ConsoOrderCoordination>>(saleOrder.CoordinationLogs.ToList());
        }
        private PagedList<ResponseTicket> FindTicket_Carrier(TicketDetailSearch ticketDetailSearch)
        {
            
            DateTime startTime = DateTime.Now.AddDays(-7);
            var currentUser = AuthManager.GetCurrentUser();
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Carrier>().Where(p=>p.CarrierCode==currentUser.Code);
         
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.orderId))
                query = query.Where(p => p.OrderID == ticketDetailSearch.orderId.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.pnr))
                query = query.Where(p => p.PNR == ticketDetailSearch.pnr.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.ticketNumber))
                query = query.Where(p => p.TicketNum == ticketDetailSearch.ticketNumber.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.carrayCode))
                query = query.Where(p => p.CarryCode == ticketDetailSearch.carrayCode.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.fromCity))
                query = query.Where(p => p.Voyage.Contains(ticketDetailSearch.fromCity.Trim() + "-"));
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.toCity))
                query = query.Where(p => p.Voyage.Contains("-" + ticketDetailSearch.toCity.Trim()));
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.ticketStatus))
                query = query.Where(p => p.TicketState == ticketDetailSearch.ticketStatus);
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.businessmanCode))
                query = query.Where(p => p.Code == ticketDetailSearch.businessmanCode.Trim());
            if (ticketDetailSearch.startIssueRefundTime.HasValue)
            {
                query = query.Where(p => p.CreateDate >= ticketDetailSearch.startIssueRefundTime.Value);
            }
            else
            {
                query = query.Where(p => p.CreateDate >= startTime);
            }
            if (ticketDetailSearch.endIssueRefundTime.HasValue)
                query = query.Where(p => p.CreateDate <= ticketDetailSearch.endIssueRefundTime.Value);

            var list = new PagedList<ResponseTicket>()
            {
                Total = query.Count(),
                Rows = query.OrderByDescending(p=>p.CreateDate).Skip((ticketDetailSearch.page - 1) * ticketDetailSearch.rows).Take(ticketDetailSearch.rows).Project().To<ResponseTicket>().ToList()
            };
            return list;
        }
        private PagedList<ResponseTicket> FindTicket_Supplier(TicketDetailSearch ticketDetailSearch)
        {
            DateTime startTime = DateTime.Now.AddDays(-7);
            var currentUser = AuthManager.GetCurrentUser();
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Supplier>().Where(p => p.IssueTicketCode == currentUser.Code);

            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.orderId))
                query = query.Where(p => p.OrderID == ticketDetailSearch.orderId.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.pnr))
                query = query.Where(p => p.PNR == ticketDetailSearch.pnr.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.ticketNumber))
                query = query.Where(p => p.TicketNum == ticketDetailSearch.ticketNumber.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.carrayCode))
                query = query.Where(p => p.CarryCode == ticketDetailSearch.carrayCode.Trim());
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.fromCity))
                query = query.Where(p => p.Voyage.Contains(ticketDetailSearch.fromCity.Trim() + "-"));
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.toCity))
                query = query.Where(p => p.Voyage.Contains("-" + ticketDetailSearch.toCity.Trim()));
            if (!string.IsNullOrWhiteSpace(ticketDetailSearch.ticketStatus))
                query = query.Where(p => p.TicketState == ticketDetailSearch.ticketStatus);
            if (ticketDetailSearch.startIssueRefundTime.HasValue)
            {
                query = query.Where(p => p.CreateDate >= ticketDetailSearch.startIssueRefundTime.Value);
            }
            else
            {
                query = query.Where(p => p.CreateDate >= startTime);
            }
            if (ticketDetailSearch.endIssueRefundTime.HasValue)
                query = query.Where(p => p.CreateDate <= ticketDetailSearch.endIssueRefundTime.Value);

            var list = new PagedList<ResponseTicket>()
            {
                Total = query.Count(),
                Rows = query.OrderByDescending(p => p.CreateDate).Skip((ticketDetailSearch.page - 1) * ticketDetailSearch.rows).Take(ticketDetailSearch.rows).Project().To<ResponseTicket>().ToList()
            };
            // AutoMapper.Mapper.Map<List<Ticket_Supplier>, List<ResponseTicket>>(query.Skip((ticketDetailSearch.page - 1) * ticketDetailSearch.rows).Take(ticketDetailSearch.rows).ToList())
            return list;
        }
        /// <summary>
        /// 机票总表
        /// </summary>
        /// <param name="ticketDetailSearch"></param>
        /// <returns></returns>
        public PagedList<ResponseTicket> GetConsoTicketSumDetail(TicketDetailSearch ticketDetailSearch)
        {
            var list = new PagedList<ResponseTicket>();
            var currentUser = AuthManager.GetCurrentUser();
            if (currentUser.Type == "Carrier")
            {
                list = FindTicket_Carrier(ticketDetailSearch);
            }
            else
            {
                list = FindTicket_Supplier(ticketDetailSearch);
            }
            return list;
        }


        public PagedList<ResponseTicketCount> GetBuyerTicketCount(TicketCountOfBuyer ticketCountOfBuyer)
        {
            var currentUser = AuthManager.GetCurrentUser();
            var query = this.ticketRepository.FindAllNoTracking().OfType<Ticket_Carrier>().Where(p => p.CarrierCode == currentUser.Code && p.TicketState=="出票");
            if (ticketCountOfBuyer.StartTime.HasValue)
                query = query.Where(p => p.CreateDate >= ticketCountOfBuyer.StartTime.Value);
            if (ticketCountOfBuyer.EndTime.HasValue)
                query = query.Where(p => p.CreateDate < ticketCountOfBuyer.EndTime.Value);
            if (!string.IsNullOrWhiteSpace(ticketCountOfBuyer.BusinessmanCode))
                query = query.Where(p => p.Code == ticketCountOfBuyer.BusinessmanCode);

            ResponseTicketCount rticketcount = null;
            List<ResponseTicketCount> list = new List<ResponseTicketCount>();
            foreach (var item in query.ToLookup(t=>t.Code).ToList().AsParallel())
            {
                rticketcount = new ResponseTicketCount();
                rticketcount.Code = item.Key;
                rticketcount.TotalCount = item.Count();
                list.Add(rticketcount);
            }
            return new PagedList<ResponseTicketCount>() { 
                Total = list.Count(),
                Rows = list.Skip((ticketCountOfBuyer.Page - 1) * ticketCountOfBuyer.Rows).Take(ticketCountOfBuyer.Rows).ToList()
            };
        }
    }
}
