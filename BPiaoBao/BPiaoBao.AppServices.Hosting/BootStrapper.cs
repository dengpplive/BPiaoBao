using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Cashbag.ClientProxy;
using BPiaoBao.Cashbag.Domain.Models;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using BPiaoBao.DomesticTicket.Domain.Models.FrePasser;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using BPiaoBao.DomesticTicket.EFRepository;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.Logs;
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
using SPiaoBao.IAppServices.DomesticTicket;
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
using System.Messaging;
using JoveZhao.Framework;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using BPiaoBao.DomesticTicket.Domain.Models.RefundEvent;


namespace BPiaoBao.AppServices.Hosting
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
            ScheduleTaskServices.RegisterTask(new OrderTimeSendMessage(), new IntervalSchedule(TimeSpan.Parse("00:10:00"), DateTime.Now));
            SystemConsoSwitch.QTSettingList.ForEach(x =>
            {
                    ScheduleTaskServices.RegisterTask(new GetQTInfo(x), new IntervalSchedule(TimeSpan.Parse(x.Timeout), DateTime.Now));
            });

            ScheduleTaskServices.RegisterTask(new OpenTicketTask(), new DailySchedule(1, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 0, 0)));
            ScheduleTaskServices.Start();
            System.Threading.Thread t = new System.Threading.Thread(ReviceMessage);
            t.Start();
            QueueLogsManager.Init();
        }
        /// <summary>
        /// 总表任务
        /// </summary>
        public static void ReviceMessage()
        {
            try
            {
                string messagePath = @".\Private$\OrderMessage";
                MessageQueue mq = null;
                if (MessageQueue.Exists(messagePath))
                    mq = new MessageQueue(messagePath);
                else
                    mq = MessageQueue.Create(messagePath);
                while (true)
                {
                    Message message = mq.Receive();
                    message.Formatter = new XmlMessageFormatter(new Type[] { typeof(OrderMessage) });
                    if (message != null)
                    {
                        try
                        {
                            OrderMessage orderMessage = message.Body as OrderMessage;
                            if (orderMessage.OrderType == 0)
                                new TicketEventHelper().FindIssueTicket(orderMessage.OrderID);
                            else if (orderMessage.OrderType == 1)
                                new TicketEventHelper().FindAfterSale(orderMessage.OrderID);
                            else if (orderMessage.OrderType == 2)
                                new TicketEventHelper().RealIssueTicket(orderMessage.OrderID);
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, "队列执行", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "队列错误", e);
            }
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
                x.For<IMyMessageRepository>().Use<MyMessageRepository>();
                x.For<IAriChangeRepository>().Use<AriChangeRepository>();
                x.For<IOPENScanRepository>().Use<OPENScanRepository>();
                x.For<IOperationLogRepository>().Use<OperationLogRepository>();
                x.For<ICustomerInfoRepository>().Use<CustomerInfoRepository>();
                x.For<IStationBuyGroupRepository>().Use<StationBuyGroupRepository>();
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
                x.For<IPlatformPointGroupRepository>().Use<PlatformPointGroupRepository>();
                x.For<IPlatformPointGroupRuleRepository>().Use<PlatformPointGroupRuleRepository>();
                x.For<IRefundReasonRepository>().Use<RefundReasonRepository>();
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
                    p.AddAllTypesOf<IDomainEventHandler<RefundTicketEvent>>();
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
            #region DomesticTicket

            Mapper.CreateMap<OrderLog, OrderLogDto>();
            Mapper.CreateMap<BounceLine, ResponseBounLine>()
                .ForMember(x => x.Status, y => y.MapFrom(p => p.Status.ToEnumDesc()));

            Mapper.CreateMap<SkyWay, SkyWayDto>()
                .ForMember(x => x.FromCity, y => y.ResolveUsing<BPiaoBao.AppServices.DomesticTicket.CityMapper.FromCityResolver>())
                .ForMember(x => x.ToCity, y => y.ResolveUsing<BPiaoBao.AppServices.DomesticTicket.CityMapper.ToCityResolver>())
                .ForMember(x => x.SkyWayId, y => y.ResolveUsing(p => p.Id));

            Mapper.CreateMap<OrderPay, OrderPayDto>()
                .ForMember(x => x.PayMethod, y => y.ResolveUsing(p => p.PayMethod.ToEnumDesc()))
                .ForMember(x => x.PayStatus, y => y.ResolveUsing(p => p.PayStatus))
                .ForMember(x => x.PaidStatus, y => y.ResolveUsing(p => p.PaidStatus));

            Mapper.CreateMap<AfterSalePassenger, ResponseAfterSalePassenger>()
                .ForMember(x => x.AfterPassengerID, y => y.MapFrom(m => m.Id))
                .ForMember(x => x.ABFee, y => y.ResolveUsing(p => p.Passenger.ABFee))
                .ForMember(x => x.CardNo, y => y.ResolveUsing(p => p.Passenger.CardNo))
                .ForMember(x => x.Id, y => y.ResolveUsing(p => p.PassengerId))
                .ForMember(x => x.Mobile, y => y.ResolveUsing(p => p.Passenger.Mobile))
                .ForMember(x => x.PassengerName, y => y.ResolveUsing(p => p.Passenger.PassengerName))
                .ForMember(x => x.PassengerType, y => y.ResolveUsing(p => p.Passenger.PassengerType))
                .ForMember(x => x.RetirementPoundage, y => y.MapFrom(p => p.RetirementPoundage))
                .ForMember(x => x.RQFee, y => y.ResolveUsing(p => p.Passenger.RQFee))
                .ForMember(x => x.SeatPrice, y => y.ResolveUsing(p => p.Passenger.SeatPrice))
                .ForMember(x => x.TicketNumber, y => y.ResolveUsing(p => p.Passenger.TicketNumber))
                .ForMember(x => x.TicketStatus, y => y.ResolveUsing(p => p.Passenger.TicketStatus))
                .ForMember(x => x.TravelNumber, y => y.ResolveUsing(p => p.Passenger.TravelNumber))
                .ForMember(x => x.PassengerTripStatus, y => y.ResolveUsing(p => p.PassengerTripStatus))
                .ForMember(x => x.IsInsuranceRefund, y => y.ResolveUsing(p => p.Passenger.IsInsuranceRefund));
            Mapper.CreateMap<RequestAfterSaleOrder, AfterSaleOrder>()
                .Include<RequestAnnulOrder, AnnulOrder>()
                .Include<RequestBounceOrder, BounceOrder>()
                .Include<RequestChangeOrder, ChangeOrder>()
                .Include<RequestModifyOrder, ModifyOrder>();
            Mapper.CreateMap<AfterSaleOrder, ResponseAfterSaleOrder>()
                .Include<AnnulOrder, ResponseAnnulOrder>()
                .Include<BounceOrder, ResponseBounceOrder>()
                .Include<ChangeOrder, ResponseChangeOrder>()
                .Include<ModifyOrder, ResponseModifyOrder>()
                .ForMember(x => x.Description, y => y.ResolveUsing(p => p.Description))
                .ForMember(x => x.Logs, y => y.ResolveUsing(p => p.Logs))
                .ForMember(x => x.Money, y => y.ResolveUsing(p => p.Money))
                .ForMember(x => x.Passenger, y => y.ResolveUsing(p => p.Passenger))
                .ForMember(x => x.ProcessStatus, y => y.ResolveUsing(p => p.ProcessStatus))
                .ForMember(x => x.CreateMan, y => y.ResolveUsing(p => p.CreateMan))
                .ForMember(x => x.CreateTime, y => y.ResolveUsing(p => p.CreateTime))
                .ForMember(x => x.SkyWays, y => y.ResolveUsing(p => p.Order.SkyWays))
                .ForMember(x => x.OrderPay, y => y.ResolveUsing(p => p.Order.OrderPay))
                .ForMember(x => x.PolicyFrom, y => y.ResolveUsing(p => p.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface ? p.Order.Policy.PlatformCode : p.Order.Policy.Code))
                .ForMember(x => x.PolicySourceType, y => y.ResolveUsing(p => p.Order.Policy.PolicySourceType))
                .ForMember(x => x.TicketDate, y => y.ResolveUsing(p => p.Order.IssueTicketTime))
                .ForMember(x => x.Code, y => y.ResolveUsing(p => p.Order.BusinessmanCode))
                .ForMember(x => x.PNR, y => y.ResolveUsing(p => p.Order.PnrCode))
                .ForMember(x => x.PNRContent, y => y.ResolveUsing(p => p.Order.PnrContent))
                .ForMember(x => x.HasAfterSale, y => y.ResolveUsing(p => p.Order.HasAfterSale))
                .ForMember(x => x.Passenger, y => y.ResolveUsing(p => p.Passenger.ToList()))
                .ForMember(x => x.YdOffice, y => y.MapFrom(m => m.Order.YdOffice))
                .ForMember(x => x.PayNum, y => y.MapFrom(m => m.Order.OrderPay.PaySerialNumber))
                .ForMember(x => x.XCPMoney, y => y.MapFrom(m => m.AfterCPMoney))
                .ForMember(x => x.SMoney, y => y.MapFrom(m => m.AfterMoney))
                .ForMember(x => x.Issue_CarrierCode, y => y.MapFrom(m => m.Order.Policy.CarrierCode))
                .ForMember(x => x.Order_CarrierCode, y => y.MapFrom(m => m.Order.CarrierCode))
                .ForMember(x => x.Reason, y => y.ResolveUsing(p => p.Reason));

            Mapper.CreateMap<AfterSaleSkyWay, ResponseAfterSaleSkyWay>()
                .ForMember(x => x.NewFlightNumber, y => y.MapFrom(p => p.FlightNumber))
                .ForMember(x => x.NewSeat, y => y.MapFrom(p => p.Seat))
                .ForMember(x => x.NewStartDateTime, y => y.MapFrom(p => p.FlyDate))
                .ForMember(x => x.NewToDateTime, y => y.MapFrom(p => p.ToDate));
            Mapper.CreateMap<RequestAfterSaleSkyWay, AfterSaleSkyWay>()
                .ForMember(x => x.FlightNumber, y => y.ResolveUsing(p => p.NewFlightNumber))
                .ForMember(x => x.Seat, y => y.ResolveUsing(p => p.NewSeat))
                .ForMember(x => x.ToDate, y => y.ResolveUsing(p => p.NewToDateTime))
                .ForMember(x => x.FlyDate, y => y.ResolveUsing(p => p.NewStartDateTime));


            Mapper.CreateMap<AnnulOrder, ResponseAnnulOrder>();
            Mapper.CreateMap<RequestAnnulOrder, AnnulOrder>()
                .ForMember(x => x.Passenger, y => y.ResolveUsing(p => p.Passengers
                    .Select(q => new AfterSalePassenger() { PassengerId = q }).ToList()));

            Mapper.CreateMap<BounceOrder, ResponseBounceOrder>();
            Mapper.CreateMap<RequestBounceOrder, BounceOrder>()
                .ForMember(x => x.Passenger, y => y.ResolveUsing(p => p.Passengers
                    .Select(q => new AfterSalePassenger() { PassengerId = q }).ToList()));


            Mapper.CreateMap<ChangeOrder, ResponseChangeOrder>();
            Mapper.CreateMap<RequestChangeOrder, ChangeOrder>()
                .ForMember(x => x.Passenger, y => y.ResolveUsing(p => p.Passengers
                    .Select(q => new AfterSalePassenger() { PassengerId = q }).ToList()));

            Mapper.CreateMap<ModifyOrder, ResponseModifyOrder>();
            Mapper.CreateMap<RequestModifyOrder, ModifyOrder>()
                .ForMember(x => x.Passenger, y => y.ResolveUsing(p => p.Passengers
                    .Select(q => new AfterSalePassenger() { PassengerId = q }).ToList()));




            Mapper.CreateMap<PlatformRefundOrder, ResponsePlatformRefundOrder>();

            Mapper.CreateMap<TravelGrantRecord, TravelGrantRecordDto>();
            Mapper.CreateMap<TravelPaper, TravelPaperDto>();
            Mapper.CreateMap<TravelPaperLog, TravelPaperLogDto>();
            //Mapper.CreateMap<IssueTicketInformation, TicketInfoSummaryEntity>();
            //Mapper.CreateMap<RefundTicketInformation, TicketInfoSummaryEntity>();
            //Mapper.CreateMap<InvalidTicketInformation, TicketInfoSummaryEntity>();
            //Mapper.CreateMap<ChangeTicketInformation, TicketInfoSummaryEntity>();
            Mapper.CreateMap<TicketSalesStatisticsDto, TicketSalesStatisticsEntity>();
            Mapper.CreateMap<TicketSalesStatistics, TicketSalesStatisticsEntity>();
            Mapper.CreateMap<TicketSalesCarrayCode, TicketSalesStatisticsEntity>();
            Mapper.CreateMap<FrePasser, FrePasserDto>();
            Mapper.CreateMap<Passenger, PassengerDto>();
            Mapper.CreateMap<InsuranceSearchRecord, ResponseInsurance>()
                .ForMember(x => x.BuyTime, y => y.ResolveUsing(p => p.BuyTime))
                .ForMember(x => x.InsuranceLimitStartTime, y => y.ResolveUsing(p => p.InsuranceLimitStartTime))
                .ForMember(x => x.InsuranceLimitEndTime, y => y.ResolveUsing(p => p.InsuranceLimitEndTime))
                .ForMember(x => x.PassengerName, y => y.ResolveUsing(p => p.PassengerName))
                .ForMember(x => x.Mobile, y => y.ResolveUsing(p => p.Mobile))
                .ForMember(x => x.CardNo, y => y.ResolveUsing(p => p.CardNo))
                .ForMember(x => x.ToCityCode, y => y.ResolveUsing(p => p.ToCity))
                .ForMember(x => x.FromCityCode, y => y.ResolveUsing(p => p.FromCity))
                .ForMember(x => x.StartDateTime, y => y.ResolveUsing(p => p.StartDateTime))
                .ForMember(x => x.ToDateTime, y => y.ResolveUsing(p => p.ToDateTime))
                .ForMember(x => x.FromCityCode, y => y.ResolveUsing(p => p.FromCityCode))
                .ForMember(x => x.ToCityCode, y => y.ResolveUsing(p => p.ToCityCode))
                .ForMember(x => x.FromCity, y => y.ResolveUsing(p => p.FromCity))
                .ForMember(x => x.ToCity, y => y.ResolveUsing(p => p.ToCity));

            Mapper.CreateMap<InsuranceConfig, InsuranceConfigData>();
            Mapper.CreateMap<InsuranceDepositLog, ResponseInsuranceDepositLog>();
            Mapper.CreateMap<InsurancePurchaseByBussinessman, ResponseInsurancePurchaseByBussinessman>();


            Mapper.CreateMap<ClientDateLimit, DateLimit>();


            Mapper.CreateMap<LocalNormalPolicy, RequestNormalPolicy>().ReverseMap();
            Mapper.CreateMap<SpecialPolicy, RequestSpecialPolicy>().ReverseMap();
            Mapper.CreateMap<LocalNormalPolicy, ResponseLocalNormalPolicy>()
                .ForMember(p => p.IsExist, opt => opt.ResolveUsing<ExistResolver>());
            Mapper.CreateMap<LocalPolicy, ResponseFullPolicy>()
                .Include<LocalNormalPolicy, ReponseNormalFullPolicy>()
                .Include<SpecialPolicy, ReponseSpecialFullPolicy>();
            Mapper.CreateMap<LocalNormalPolicy, ReponseNormalFullPolicy>();
            Mapper.CreateMap<SpecialPolicy, ReponseSpecialFullPolicy>();


            Mapper.CreateMap<SpecialPolicy, ResponseOperPolicy>();
            Mapper.CreateMap<LocalPolicy, RequestSpecialPolicy>();


            Mapper.CreateMap<RequestPolicy, LocalPolicy>()
                  .Include<RequestSpecialPolicy, SpecialPolicy>()
                .ForMember(p => p.ID, opt => opt.Ignore());
            Mapper.CreateMap<RequestPartPolicy, LocalPolicy>().ReverseMap();






            Mapper.CreateMap<DateLimit, ClientDateLimit>();
            Mapper.CreateMap<LocalPolicy, BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects.ResponsePolicy>();


            Mapper.CreateMap<LocalPolicy, ResponseSpecialPolicy>();
            Mapper.CreateMap<DateLimit, StringDateLimit>().ConvertUsing(new StringTypeConvert());
            Mapper.CreateMap<LocalPolicy, ResponseLocalPolicy>()
                 .Include<SpecialPolicy, ResponseSpecialPolicy>()
                 .ForMember(p => p.IsExist, opt => opt.ResolveUsing<ExistResolver>());
            Mapper.CreateMap<SpecialPolicy, ResponseOperPolicy>();
            Mapper.CreateMap<SpecialPolicy, ResponseSpecialPolicy>();
            Mapper.CreateMap<RequestSpecialPolicy, SpecialPolicy>();
            Mapper.CreateMap<TicketSum, ReponseTicketSum>();
            Mapper.CreateMap<Ticket_Conso, TicketSumDetailDto>();
            Mapper.CreateMap<Ticket_Carrier, ResponseTicket>();
            Mapper.CreateMap<Ticket_Supplier, ResponseTicket>();
            Mapper.CreateMap<Ticket_Buyer, ReponseTicketSum>();
            Mapper.CreateMap<AirChange, ResponeAirChange>();
            Mapper.CreateMap<AirChange, ResponseAirQtInfo>();
            Mapper.CreateMap<AirChangeCoordion, AirChangeCoordionDto>();
            Mapper.CreateMap<AirChange, ResponeAirPnrInfo>();

            Mapper.CreateMap<PointGroupDetailRuleDataObject, PlatformPointGroupDetailRule>().ReverseMap();
            Mapper.CreateMap<PlatformPointGroup, PlatformPointGroupDataObject>().ReverseMap();
            Mapper.CreateMap<PlatformPointGroupRule, PlatformPointGroupRuleDataObject>()
                .ForMember(p => p.PointGroupName, x => x.MapFrom(m => m.PlatformPointGroup.GroupName));
            Mapper.CreateMap<PlatformPointGroupRuleDataObject, PlatformPointGroupRule>()
                .ForMember(p => p.PlatformPointGroup, x => x.Ignore());


            #endregion

            #region TPOS
            Mapper.CreateMap<AccountStat, AccountStatDataObject>();
            Mapper.CreateMap<BusinessmanReport, BusinessmanReportDataObject>();
            Mapper.CreateMap<BusinessmanTrade, BusinessmanTradeDataObject>();
            Mapper.CreateMap<PosAssignLog, PosAssignLogDataObject>();
            Mapper.CreateMap<PosInfo, PosInfoDataObject>();
            Mapper.CreateMap<TradeDetail, TradeDetailDataObject>();
            Mapper.CreateMap<TradeStat, TradeStatDataObject>();
            Mapper.CreateMap<BusinessmanInfo, ResponseBusinessmanInfo>()
                .ForMember(x => x.Cardholder, y => y.ResolveUsing(p => p.Bank.Cardholder))
                .ForMember(x => x.BankName, y => y.ResolveUsing(p => p.Bank.BankName))
                .ForMember(x => x.CardNo, y => y.ResolveUsing(p => p.Bank.CardNo))
                .ForMember(x => x.Province, y => y.ResolveUsing(p => p.Bank.Address.Province))
                .ForMember(x => x.City, y => y.ResolveUsing(p => p.Bank.Address.City))
                .ForMember(x => x.BankId, y => y.ResolveUsing(p => p.Bank.BankId))
                .ForMember(x => x.Subbranch, y => y.ResolveUsing(p => p.Bank.Address.Subbranch));
            Mapper.CreateMap<RequestBusinessmanInfo.RequestBankInfo, BusinessmanInfo.BankInfo>();
            Mapper.CreateMap<RequestBusinessmanInfo.RequestBankAddress, BusinessmanInfo.BankAddress>();
            Mapper.CreateMap<RequestBusinessmanInfo, BusinessmanInfo>();

            #endregion


            #region 系统设置
            Mapper.CreateMap<BounceLine, BPiaoBao.AppServices.DataContracts.DomesticTicket.ResponseBounceLine>()
                .ForMember(p => p.Status, opt => opt.MapFrom(x => x.Status.ToEnumDesc()));
            #region 系统设置后台
            Mapper.CreateMap<BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.AttachmentDataObject, Attachment>();
            Mapper.CreateMap<BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.ContactWayDataObject, ContactWay>();
            Mapper.CreateMap<Attachment, BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.AttachmentDataObject>();
            Mapper.CreateMap<ContactWay, BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.ContactWayDataObject>();
            Mapper.CreateMap<Businessman, ResponseBusinessMan>();
            Mapper.CreateMap<ContactWay, ContactWayDto>();
            Mapper.CreateMap<Buyer, RequestBuyer>();
            Mapper.CreateMap<RequestBusinessman, Businessman>()
                .Include<RequestBuyer, Buyer>();
            Mapper.CreateMap<RequestBuyer, Buyer>();
            Mapper.CreateMap<RequestSupplier, Supplier>()
                .ForMember(p => p.SupPids, opt => opt.MapFrom(x => x.Pids));
            Mapper.CreateMap<Supplier, RequestSupplier>()
                .ForMember(p => p.Pids, opt => opt.MapFrom(x => x.SupPids));
            Mapper.CreateMap<Businessman, BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.ResponseBusinessman>()
                .Include<Buyer, ResponseBuyer>()
                .Include<Supplier, ResponseSupplier>();
            Mapper.CreateMap<Businessman, ResponseDetailBusinessman>()
                .Include<Buyer, ResponseDetailBuyer>();
            Mapper.CreateMap<Buyer, ResponseDetailBuyer>();
            Mapper.CreateMap<Operator, ResponseOperator>()
                .ForMember(p => p.OperatorState, opt => opt.MapFrom(x => x.OperatorState.ToEnumDesc()))
                .ForMember(p => p.RoleName, opt => opt.MapFrom(x => x.Role != null ? x.Role.RoleName : string.Empty));
            Mapper.CreateMap<RequestOperator, Operator>();
            Mapper.CreateMap<Operator, RequestOperator>();
            Mapper.CreateMap<Role, ResponseRole>();
            Mapper.CreateMap<RequestRole, Role>();
            Mapper.CreateMap<Role, RequestRole>();
            Mapper.CreateMap<WorkBusinessman, WorkBusinessmanDataObject>();
            Mapper.CreateMap<WorkBusinessmanDataObject, WorkBusinessman>();
            Mapper.CreateMap<PID, BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.PIDDataObject>();
            Mapper.CreateMap<BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.PIDDataObject, PID>();
            Mapper.CreateMap<CarrierSetting, BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.CarrierSettingDataObject>();
            Mapper.CreateMap<BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects.CarrierSettingDataObject, CarrierSetting>();
            Mapper.CreateMap<Carrier, CarrierDataObject>();
            Mapper.CreateMap<STRequestSupplier, Supplier>();
            Mapper.CreateMap<Supplier, ResponseDetailSupplier>();
            Mapper.CreateMap<Supplier, CarrierDataObject>()
                .ForMember(p => p.NormalWork, opt => opt.MapFrom(x => x.SupNormalWork))
                .ForMember(p => p.RestWork, opt => opt.MapFrom(x => x.SupRestWork))
                .ForMember(p => p.Pids, opt => opt.MapFrom(x => x.SupPids));


            Mapper.CreateMap<DeductionGroup, ResponseDeduction>();
            Mapper.CreateMap<RequestAdjustDetail, AdjustDetail>();
            Mapper.CreateMap<RequestDeductionRule, DeductionRule>();
            Mapper.CreateMap<RequestDeduction, DeductionGroup>();

            Mapper.CreateMap<AdjustDetail, RequestAdjustDetail>()
                .ForMember(p => p.AdjustTypeDescription, opt => opt.MapFrom(y => y.AdjustType.ToEnumDesc()));
            Mapper.CreateMap<DeductionRule, RequestDeductionRule>()
                .ForMember(p => p.DeductionTypeDescription, opt => opt.MapFrom(y => y.DeductionType.ToEnumDesc()));
            Mapper.CreateMap<DeductionGroup, RequestDeduction>();
            Mapper.CreateMap<DefaultPolicy, DefaultPolicyDataObject>().ReverseMap();
            Mapper.CreateMap<SmsTemplateDataObject, SMSTemplate>();
            Mapper.CreateMap<SMSTemplate, ResponseSmsTemplate>();
            Mapper.CreateMap<GiveDetailDataObj, GiveDetail>();
            Mapper.CreateMap<GiveDetail, GiveDetailDto>();
            Mapper.CreateMap<SMSChargeSetDataObj, SMSChargeSet>();
            Mapper.CreateMap<SMSChargeSet, SMSChargeSetDataObj>();
            Mapper.CreateMap<SendDetailDataObj, SendDetail>();
            Mapper.CreateMap<Notice, NoticeDto>();
            Mapper.CreateMap<Notice, RequestNoticeDto>();
            Mapper.CreateMap<Notice, NoticeDataObj>();
            Mapper.CreateMap<RequestNotice, Notice>();
            Mapper.CreateMap<RequestNoticeDto, Notice>();
            Mapper.CreateMap<NoticeDataObj, Notice>();
            Mapper.CreateMap<NoticeAttachment, NoticeAttachmentDto>();
            Mapper.CreateMap<NoticeAttachment, NoticeAttachmentDataDto>();
            Mapper.CreateMap<NoticeAttachmentDto, NoticeAttachment>();
            Mapper.CreateMap<NoticeAttachmentDataDto, NoticeAttachment>();

            Mapper.CreateMap<SupplierDataObject, Supplier>()
                .ForMember(p => p.SupPids, opt => opt.MapFrom(x => x.Pids))
                .ForMember(p => p.SupNormalWork, opt => opt.MapFrom(x => x.NormalWork))
                .ForMember(p => p.SupRestWork, opt => opt.MapFrom(x => x.RestWork));
            Mapper.CreateMap<Supplier, SupplierDataObject>()
                .ForMember(p => p.Pids, opt => opt.MapFrom(x => x.SupPids))
                .ForMember(p => p.NormalWork, opt => opt.MapFrom(x => x.SupNormalWork))
                .ForMember(p => p.RestWork, opt => opt.MapFrom(x => x.SupRestWork));
            Mapper.CreateMap<Supplier, ResponseSupplier>();
            Mapper.CreateMap<Buyer, ResponseBuyer>();


            Mapper.CreateMap<OrderPay, OrderPayDataObject>();
            Mapper.CreateMap<Passenger, BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects.PassengerDataObject>();
            Mapper.CreateMap<AfterSalePassenger, BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects.AfterPassengerDataObject>()
                .ForMember(p => p.PassengerName, x => x.MapFrom(m => m.Passenger.PassengerName));
            Mapper.CreateMap<SkyWay, SkyDataObject>();
            Mapper.CreateMap<Order, ResponseConsoOrder>()
                .ForMember(p => p.OrderMoney, opt => opt.ResolveUsing<OrderMoneyResolver>())
                .ForMember(p => p.BusinessmanCode, opt => opt.ResolveUsing<BuyerCodeResolver>())
                .ForMember(p => p.IsSelfIssueTicket, opt => opt.ResolveUsing<IsSelfIssueTicket>());

            Mapper.CreateMap<Policy, PolicyDataObject>()
                .ForMember(p => p.Code, x => x.ResolveUsing<IssueTicketCodeResolver>());
            Mapper.CreateMap<Policy,PolicyDto>();
            Mapper.CreateMap<Order, ConsoOrderDataObject>()
                .ForMember(p => p.BusinessmanCode, x => x.ResolveUsing<BuyerCodeResolver>());
            Mapper.CreateMap<AfterSaleOrder, ResponseConsoSaleOrder>()
                .Include<BounceOrder, ResponseConsoSaleBounceOrder>()
                .ForMember(p => p.Money, x => x.ResolveUsing<AfterMoneyResolver>())
                .ForMember(p => p.TotalMoney, x => x.ResolveUsing<AfterCPMoneyResolver>());
            Mapper.CreateMap<BounceOrder, ResponseConsoSaleBounceOrder>();
            Mapper.CreateMap<CoordinationLog, ConsoOrderCoordination>();
            Mapper.CreateMap<MyMessage, MyMessageDto>();
            Mapper.CreateMap<OperationLog, OperationLogDto>();
            Mapper.CreateMap<OperationLogDto, OperationLog>();
            Mapper.CreateMap<RefundReason, ResponseRefund>();
         
            #endregion

            #region 系统设置控台
            Mapper.CreateMap<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ContactWayDataObject, ContactWay>();
            Mapper.CreateMap<ContactWay, BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ContactWayDataObject>();
            Mapper.CreateMap<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.PIDDataObject, PID>();
            Mapper.CreateMap<PID, BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.PIDDataObject>();
            Mapper.CreateMap<RequestCarrier, Carrier>();
            Mapper.CreateMap<Carrier, ResponseDetailCarrier>();
            Mapper.CreateMap<OPENScan, ResponseOPENScan>();
            Mapper.CreateMap<RequestOPENScan, OPENScan>();
            Mapper.CreateMap<Supplier, SupplierDataObj>();
            #endregion
            #endregion

        }
       

        /// <summary>
        /// 总表任务
        /// </summary>
        //public static void ReviceMessage()
        //{
        //    try
        //    {
        //        string messagePath = @".\Private$\OrderMessage";
        //        MessageQueue mq = null;
        //        if (MessageQueue.Exists(messagePath))
        //            mq = new MessageQueue(messagePath);
        //        else
        //            mq = MessageQueue.Create(messagePath);
        //        while (true)
        //        {
        //            Message message = mq.Receive();
        //            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(OrderMessage) });
        //            if (message != null)
        //            {
        //                try
        //                {
        //                    OrderMessage orderMessage = message.Body as OrderMessage;
        //                    if (orderMessage.OrderType == 0)
        //                        new TicketEventHelper().FindIssueTicket(orderMessage.OrderID);
        //                    else if (orderMessage.OrderType == 1)
        //                        new TicketEventHelper().FindAfterSale(orderMessage.OrderID);
        //                    else if (orderMessage.OrderType == 2)
        //                        new TicketEventHelper().RealIssueTicket(orderMessage.OrderID);
        //                }
        //                catch (Exception e)
        //                {
        //                    Logger.WriteLog(LogType.ERROR, "队列执行", e);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.WriteLog(LogType.ERROR, "队列错误", e);
        //    }
        //}
    }

    #region AutoMapper值转换器
    public class AfterMoneyResolver : ValueResolver<AfterSaleOrder, decimal>
    {
        protected override decimal ResolveCore(AfterSaleOrder source)
        {
            if (source is ChangeOrder)
                return source.Money;
            if (source.ProcessStatus == EnumTfgProcessStatus.UnProcess || source.ProcessStatus == EnumTfgProcessStatus.Processing || source.ProcessStatus == EnumTfgProcessStatus.RepelProcess)
                return 0;
            bool _selfParent = AuthManager.GetCurrentUser().Code == source.Order.CarrierCode ? true : false;
            return _selfParent ? source.Money : source.AfterMoney;
        }
    }
    public class AfterCPMoneyResolver : ValueResolver<AfterSaleOrder, decimal>
    {
        protected override decimal ResolveCore(AfterSaleOrder source)
        {
            bool _selfParent = AuthManager.GetCurrentUser().Code == source.Order.CarrierCode ? true : false;
            return _selfParent ? source.TotalMoney : source.AfterCPMoney;
        }
    }
    public class BuyerCodeResolver : ValueResolver<Order, string>
    {
        protected override string ResolveCore(Order source)
        {
            bool _selfParent = AuthManager.GetCurrentUser().Code == source.CarrierCode ? true : false;
            return _selfParent ? source.BusinessmanCode : string.Empty;
        }
    }
    public class IssueTicketCodeResolver : ValueResolver<Policy, string>
    {
        protected override string ResolveCore(Policy source)
        {
            bool _selfParent = AuthManager.GetCurrentUser().Code == source.Order.CarrierCode ? true : false;
            return _selfParent ? source.Code : string.Empty;
        }
    }
    public class IsSelfIssueTicket : ValueResolver<Order, bool>
    {
        protected override bool ResolveCore(Order source)
        {
            if (source.Policy == null)
                return false;
            return AuthManager.GetCurrentUser().Code == source.Policy.Code;
        }
    }

    public class OrderMoneyResolver : ValueResolver<Order, decimal>
    {
        protected override decimal ResolveCore(Order source)
        {
            bool _selfParent = AuthManager.GetCurrentUser().Code == source.CarrierCode ? true : false;
            return _selfParent ? source.OrderMoney : source.CPMoney;
        }
    }

    public class ExistResolver : ValueResolver<LocalPolicy, bool>
    {
        protected override bool ResolveCore(LocalPolicy source)
        {
            return AuthManager.GetCurrentUser().Code == source.Code ? true : false;
        }
    }

    /// <summary>
    /// DateTime-String 转换
    /// </summary>
    public class StringTypeConvert : TypeConverter<DateLimit, StringDateLimit>
    {
        protected override StringDateLimit ConvertCore(DateLimit source)
        {
            return new StringDateLimit { StartTime = source.StartTime.ToString("yyyy-MM-dd"), EndTime = source.EndTime.ToString("yyyy-MM-dd") };
        }
    }


    #endregion



}