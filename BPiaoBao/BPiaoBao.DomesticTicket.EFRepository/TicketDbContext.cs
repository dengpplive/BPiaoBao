using BPiaoBao.DomesticTicket.Domain.Models.FrePasser;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using System.Data.Entity.ModelConfiguration.Conventions;
using BPiaoBao.DomesticTicket.EFRepository.OrdersMap;
using BPiaoBao.DomesticTicket.EFRepository.InsurancesMap;
using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using BPiaoBao.DomesticTicket.Domain.Models;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using BPiaoBao.DomesticTicket.EFRepository.PlatformPointGroupMap;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class TicketDbContext : DbContext
    {
        static TicketDbContext()
        {
            Database.SetInitializer<TicketDbContext>(null);
        }

        public TicketDbContext()
            : base("name=ticket")
        {
            
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLog> OrderLogs { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<SkyWay> SkyWays { get; set; }
        public DbSet<Policy> Policys { get; set; }
        public DbSet<OrderPay> OrderPays { get; set; }
        public DbSet<PlatformRefundOrder> RefundOrders { get; set; }
        public DbSet<RefundReason> RefundReason { get; set; }


        public DbSet<AfterSaleOrder> AfterSaleOrders { get; set; }
        public DbSet<AfterSalePassenger> AfterSalePassengers { get; set; }
        public DbSet<AfterSaleSkyWay> AfterSaleSkyWays { get; set; }

        public DbSet<CoordinationLog> CoordinationLogs { get; set; }
        public DbSet<TicketSum> TicketSums { get; set; }
        public DbSet<TravelPaper> TravelPapers { get; set; }
        public DbSet<TravelGrantRecord> TravelNums { get; set; }
        public DbSet<TravelPaperLog> TravelPaperLogs { get; set; }
        public DbSet<DeductionGroup> DeductionGroups { get; set; }
        public DbSet<DeductionRule> DeductionRules { get; set; }
        public DbSet<AdjustDetail> AdjustDetails { get; set; }
        public DbSet<PayBillDetail> PayBillDetails { get; set; }
        public DbSet<DeductionDetail> DeductionDetails { get; set; }
        public DbSet<InsuranceConfig> InsuranceConfig { get; set; }
        public DbSet<InsuranceDepositLog> InsuranceDepositLog { get; set; }
        public DbSet<LocalPolicy> LocalPolicy { get; set; }
        public DbSet<InsuranceOrder> InsuranceOrders { get; set; }
        public DbSet<InsuranceRecord> InsuranceRecords { get; set; }
        public DbSet<InsurancePurchaseByBussinessman> InsurancePurchaseByBussinessmans { get; set; }

        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<AirChange> AirChange { get; set; }
        public DbSet<AirChangeCoordion> AirChangeCoordion { get; set; }
        public DbSet<PlatformPointGroup> PlatformPointGroups { get; set; }
        public DbSet<PlatformPointGroupRule> PlatformPointGroupRules { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.ComplexType<DateLimit>();
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderLogMap());
            modelBuilder.Configurations.Add(new PassengerMap());
            modelBuilder.Configurations.Add(new SkyWayMap());
            modelBuilder.Configurations.Add(new PolicyMap());
            modelBuilder.Configurations.Add(new OrderPayMap());
            modelBuilder.Configurations.Add(new AfterSaleOrderMap());
            modelBuilder.Configurations.Add(new AfterSalePassengerMap());
            modelBuilder.Configurations.Add(new AfterSaleSkyWayMap());
            modelBuilder.Configurations.Add(new AnnulOrderMap());
            modelBuilder.Configurations.Add(new BounceOrderMap());
            modelBuilder.Configurations.Add(new PlatformRefundOrderMap());
            modelBuilder.Configurations.Add(new CoordinationLogMap());
            modelBuilder.Configurations.Add(new TicketSumMap());
            modelBuilder.Configurations.Add(new TravelPaperMap());
            modelBuilder.Configurations.Add(new TravelGrantRecordMap());
            modelBuilder.Configurations.Add(new TravelPaperLogMap());
            modelBuilder.Configurations.Add(new DeductionGroupMap());
            modelBuilder.Configurations.Add(new DeductionDetailMap());
            modelBuilder.Configurations.Add(new PayBillDetailMap());
            modelBuilder.Configurations.Add(new DeductionRuleMap());
            modelBuilder.Configurations.Add(new AdjustDetailMap());
            modelBuilder.Configurations.Add(new FrePasserMap());
            modelBuilder.Configurations.Add(new InsuranceConfigMap());
            modelBuilder.Configurations.Add(new InsuranceDepositLogMap());
            modelBuilder.Configurations.Add(new LocalPolicyMap());
            modelBuilder.Configurations.Add(new InsuranceRecordMap());
            modelBuilder.Configurations.Add(new InsuranceOrderMap());
            modelBuilder.Configurations.Add(new InsurancePurchaseByBussinessmanMap());
            modelBuilder.Configurations.Add(new TicketMap());
            modelBuilder.Configurations.Add(new TicketConsoMap());
            modelBuilder.Configurations.Add(new TicketCarrierMap());
            modelBuilder.Configurations.Add(new AirChangeMap());
            modelBuilder.Configurations.Add(new PointGroupMap());
            modelBuilder.Configurations.Add(new PlatformPointGroupRuleMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
