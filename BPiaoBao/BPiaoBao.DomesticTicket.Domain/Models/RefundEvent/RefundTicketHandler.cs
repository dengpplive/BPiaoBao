using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.RefundEvent
{
    public class RefundTicketHandler : IDomainEventHandler<RefundTicketEvent>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IAfterSaleOrderRepository saleOrderRepository;
        private readonly IRefundReasonRepository refundReasonRepository;
        public RefundTicketHandler()
        {
            unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
            saleOrderRepository = ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
            refundReasonRepository = ObjectFactory.GetInstance<IRefundReasonRepository>();
        }


        public void Handle(RefundTicketEvent domainEvent)
        {
            var saleOrder = this.saleOrderRepository.FindAll(p => p.Id == domainEvent.SaleOrderId).FirstOrDefault();
            if (saleOrder == null)
            {
                Logger.WriteLog(LogType.INFO, string.Format("没有找到售后订单：{0}", domainEvent.SaleOrderId));
                return;
            }
            string platCode = saleOrder.Order.Policy.PlatformCode;
            var platform = BPiaoBao.DomesticTicket.Platform.Plugin.PlatformFactory.GetPlatformByCode(platCode);
            if (platform == null)
            {
                Logger.WriteLog(LogType.INFO, string.Format("没有找到平台Code:{0},售后订单:{1}", platCode, domainEvent.SaleOrderId));
                return;
            }
            var reason = refundReasonRepository.FindAllNoTracking(p => p.ID == saleOrder.ReasonID).FirstOrDefault();

            RefundArgs refundArgs = new RefundArgs
            {
                Guid = reason.Guid.ToString(),
                OrderId = saleOrder.OrderID,
                areaCity=saleOrder.Order.Policy.AreaCity,
                OutOrderId = saleOrder.Order.OutOrderId,
                Passengers = saleOrder.Passenger.Select(p => new RefundPassenger
                {
                    PassengerName = p.Passenger.PassengerName,
                    PassengerType = p.Passenger.PassengerType,
                    TicketNo = p.Passenger.TicketNumber,
                    CardNo = p.Passenger.CardNo,
                    Amount=p.Passenger.CPMoney
                }).ToList(),
                PnrCode = saleOrder.Order.PnrCode,
                Reason = saleOrder.Reason,
                RefundMoneyType = reason.RefundType,
                Remark = saleOrder.Remark,
                Sky = saleOrder.Order.SkyWays.Select(p => new RefundSky
                {
                    FromCityCode = p.FromCityCode,
                    ToCityCode = p.ToCityCode
                }).ToList()
            };
            if (saleOrder is BounceOrder)
            {
                refundArgs.AttachmentUrl = (saleOrder as BounceOrder).AttachmentUrl;
                refundArgs.IsVoluntary = (saleOrder as BounceOrder).IsVoluntary;
                refundArgs.RefundType = 0;
            }
            else if (saleOrder is AnnulOrder)
            {
                refundArgs.AttachmentUrl = (saleOrder as AnnulOrder).AttachmentUrl;
                refundArgs.RefundType = 1;
            }
            RefundTicketResult result = platform.BounceOrAnnulTicket(refundArgs);
            if (result.Result)
                saleOrder.ProcessStatus = EnumTfgProcessStatus.Processing;
            this.unitOfWorkRepository.PersistUpdateOf(saleOrder);
            this.unitOfWork.Commit();
                
        }



    }
}
