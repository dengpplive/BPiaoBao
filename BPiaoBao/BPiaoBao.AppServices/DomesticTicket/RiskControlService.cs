using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using Cashbag.Integrated;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class RiskControlService : BaseService, IRiskControl
    {
        IOrderRepository orderRepository;
        IBusinessmanRepository businessmanRepository;
        public RiskControlService(IOrderRepository orderRepository, IBusinessmanRepository businessmanRepository)
        {
            this.orderRepository = orderRepository;
            this.businessmanRepository = businessmanRepository;

        }
        [ExtOperationInterceptor("取最近三个月的流水信息")]
        public DataList<SellSerial> GetSerial(string cashbagCode, int startIndex, int count)
        {
            var bm = businessmanRepository.FindAll(p => p.CashbagCode == cashbagCode).FirstOrDefault();
            var Slist = new DataList<SellSerial>();
            if (bm != null)
            {
                var query = orderRepository.FindAll(p =>
                    p.BusinessmanCode == bm.Code &&
                    p.OrderStatus == EnumOrderStatus.IssueAndCompleted &&
                    p.IssueTicketTime > DateTime.Now.AddMonths(-3) &&
                    p.IssueTicketTime <= DateTime.Now
                    );

                Slist.TotalCount = query.Count();
                Slist.List = query.OrderBy(p => p.IssueTicketTime).Skip(startIndex).Take(count).Select(p => new SellSerial()
                {
                    OrderId = p.OrderId,
                    Money = p.OrderMoney,
                    SellTime = (DateTime)p.IssueTicketTime,
                    Remark = p.Remark

                }).ToList();
            }
            return Slist;
        }

        [ExtOperationInterceptor("取库存信息")]
        public DataInventory GetInventory(string cashbagCode)
        {
            var bm = businessmanRepository.FindAll(p => p.CashbagCode == cashbagCode).FirstOrDefault();
            DataInventory datainventory = new DataInventory();
            if (bm != null)
            {
                var query = orderRepository.FindAll(p =>
                    p.BusinessmanCode == bm.Code &&
                    p.OrderStatus == EnumOrderStatus.IssueAndCompleted &&
                    p.SkyWays.Where(s => s.StartDateTime > DateTime.Now).Count() > 0
                    );

                int ordercount = query.Count();
                datainventory.TotalMoney = query.Select(p => p.OrderMoney).Sum();
                datainventory.Info = "该商户已出票未起飞的订单共计:" + ordercount + ",订单总金额:" + datainventory.TotalMoney;
            }
            return datainventory;
        }
    }
}
