using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Cashbag.ClientProxy;
using BPiaoBao.Cashbag.Domain.Models;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.FrePasser;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using BPiaoBao.DomesticTicket.EFRepository;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using JoveZhao.Framework.DDD.Events;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework.Expand;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBaoTPos.Domain.Models;
using BPiaoBaoTPos.Domain.Services;
using BPiaoBao.TPos.ClientProxy;
using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.TPos;
using JoveZhao.Framework.ScheduleTask;
using JoveZhao.Framework.EFRepository.UnitOfWork;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
//using SPiaoBao.IAppServices.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;
using BPiaoBao.Common.Enums;
using BPiaoBao.SystemSetting.EFRepository;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using BPiaoBao.SystemSetting.Domain.Models.Notice;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices;
using JoveZhao.Framework;

namespace BPiaoBao.UnitTest.DomesticTicket
{
    public class BootStrapper
    {
        public static void Boot()
        {
            ConfigureDependencies();
            AutoMapperInitialize();
            InitSystemSwitch.Init();
            ScheduleTaskServices.RegisterTask(new TimeSendMessage(), new IntervalSchedule(TimeSpan.Parse("02:00:00"), DateTime.Now));
            ScheduleTaskServices.RegisterTask(new UserBehavior(), new IntervalSchedule(TimeSpan.Parse("00:00:10"), DateTime.Now));
            ScheduleTaskServices.RegisterTask(new OrderTimeSendMessage(), new IntervalSchedule(TimeSpan.Parse("00:00:20"), DateTime.Now));
            ScheduleTaskServices.Start();
            System.Threading.Thread t = new System.Threading.Thread(ReviceMessage);
            t.Start();

        }
        public static void ReviceMessage()
        {
        }
        public static void ConfigureDependencies()
        {
            ObjectFactory.Configure(x =>
            {
                #region SystemSetting
                //IUnitOfWork
                x.For<IUnitOfWork>().Use<EFUnitOfWork<SystemDbContext>>().Name = EnumModule.SystemSetting.ToString();
                x.For<IUnitOfWorkRepository>().Use<EFUnitOfWorkRepositoryAdapter<SystemDbContext>>().Name = EnumModule.SystemSetting.ToString();

                //repository
                x.For<IBusinessmanRepository>().Use<BusinessmanRepository>();
                x.For<ILoginLogRepository>().Use<BPiaoBao.SystemSetting.EFRepository.LoginLogRepository>();
                x.For<ITicketSumRepository>().Use<TicketSumRepository>();
                x.For<IRoleRepository>().Use<RoleRepository>();
                x.For<IBehaviorStatRepository>().Use<BehaviorStatRepository>();
                x.For<ISmsTemplateRepository>().Use<SmsTemplateRepository>();
                x.For<ISMSChargeSetRepository>().Use<SMSChargeSetRepository>();
                x.For<INoticeRepository>().Use<NoticeRepository>();
                #endregion

                #region DomesticTicket
                x.For<IUnitOfWork>().Use<EFUnitOfWork<TicketDbContext>>().Name = EnumModule.DomesticTicket.ToString();
                x.For<IUnitOfWorkRepository>().Use<EFUnitOfWorkRepositoryAdapter<TicketDbContext>>().Name = EnumModule.DomesticTicket.ToString();
                x.For<IOrderRepository>().Use<OrderRepository>();
                x.For<ITicketRepository>().Use<TicketRepository>();
                x.For<IPlatformRefundOrderRepository>().Use<PlatformRefundOrderRepository>();
                x.For<IAfterSaleOrderRepository>().Use<AfterSaleOrderRepository>();
                x.For<IPaymentClientProxy>().Use<CashbagPaymentClientProxy>();
                x.For<IAccountClientProxy>().Use<AccountClientProxy>();
                x.For<IFundClientProxy>().Use<FundClientProxy>();
                x.For<IFinancialClientProxy>().Use<FinancialClientProxy>();
                x.For<IDeductionRepository>().Use<DeductionRepository>();
                x.For<IFlightDestineService>().Use<FlightDestineService>();
                x.For<IOrderService>().Use<OrderService>();
                x.For<IPidService>().Use<PidService>();
                x.For<IFrePasserRepository>().Use<FrePasserRepository>();
                x.For<ILocalPolicyRepository>().Use<LocalPolicyRepository>();
                x.For<IInsuranceConfigRepository>().Use<InsuranceConfigRepository>();
                x.For<IInsuranceDepositLogRepository>().Use<InsuranceDepositLogRepository>();
                x.For<IInsuranceOrderRepository>().Use<InsuranceOrderRepository>();
                x.For<IInsurancePurchaseByBussinessmanRepository>().Use<InsurancePurchaseByBussinessmanRepository>();
                //x.For<IInsuranceRecordRepository>().Use<InsuranceRecordRepository>();
                x.Scan(p =>
                {
                    p.AssembliesFromApplicationBaseDirectory();
                    p.AddAllTypesOf<BaseOrderState>().NameBy(o =>
                    {

                        var os = o.GetCustomAttributes(typeof(OrderStateAttribute), true).FirstOrDefault();
                        if (os == null)
                            throw new Exception("没有找到定义OrderStateAttribute的" + o.FullName);
                        return (os as OrderStateAttribute).OrderState.ToString();
                    });
                    p.AddAllTypesOf<IAggregationBuilder>();
                    p.AddAllTypesOf<IPlatformNotify>();
                    p.AddAllTypesOf<IPlatform>().NameBy(o =>
                    {
                        var os = o.GetCustomAttributes(typeof(PlatformAttribute), true).FirstOrDefault();
                        if (os == null)
                            throw new Exception("没有找到定义PlatformAttribute的" + o.FullName);
                        return (os as PlatformAttribute).Code;
                    });
                    p.AddAllTypesOf<IDomainEvent>();
                    p.AddAllTypesOf<IDomainEventHandler<RefundEvent>>();
                    p.AddAllTypesOf<IDomainEventHandler<UserLoginEvent>>();
                    p.AddAllTypesOf<IDomainEventHandler<OrderStatusChangedEvent>>();
                    p.AddAllTypesOf<IDomainEventHandler<TicketEvent>>();
                });
                #endregion

                #region TPOS
                x.For<IBusinessmanClientProxy>().Use<BusinessmanClientProxy>();
                x.For<IPosClientProxy>().Use<PosClientProxy>();
                x.For<IPosStatClientProxy>().Use<PosStatClientProxy>();
                #endregion

                #region TravelPaper
                x.For<ITravelGrantRecordRepository>().Use<TravelGrantRecordRepository>();
                x.For<ITravelPaperRepository>().Use<TravelPaperRepository>();
                #endregion
            });
        }
        private static SkyWayDto ConsUsing(SkyWay skyWay)
        {
            return new SkyWayDto
            {
                ToCity = skyWay.ToCityCode,
                FromCity = skyWay.FromCityCode
            };
        }
        public static void AutoMapperInitialize()
        {
          


        
        }
    }
    




}
