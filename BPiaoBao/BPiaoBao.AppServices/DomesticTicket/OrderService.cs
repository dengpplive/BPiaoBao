using System.ServiceModel.Channels;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using BPiaoBao.DomesticTicket.Domain.Services;
using PnrAnalysis;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.Common.Enums;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using PnrAnalysis.Model;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
using System.Linq.Expressions;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DataContracts.TPos;
using JoveZhao.Framework.DDD.Events;
using BPiaoBao.DomesticTicket.Domain.Models.RefundEvent;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public partial class OrderService : BaseService, IOrderService
    {
        DomesticService service;
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IUnitOfWork _unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository _unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        IOrderRepository orderRepository;
        ILocalPolicyRepository localPolicyRepository;
        IBusinessmanRepository businessmanRepository;
        IAfterSaleOrderRepository afterSaleOrderRepository;
        ITicketSumRepository ticketSumRepository;
        ITicketRepository ticketRepository;
        IAriChangeRepository ariChangeRepository;
        IMyMessageRepository MyMessageRepository;
        IPidService pidService;
        IRefundReasonRepository RefundReasonRepository;



        FlightService flightDestineService;
        BPiaoBao.SystemSetting.Domain.Services.Auth.CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
        DataBill databill = new DataBill();
        string collaboratorKey = SettingSection.GetInstances().Cashbag.PartnerKey;
        //PNR资源
        PnrResource pnrResource = new PnrResource();
        FormatPNR format = new FormatPNR();
        public OrderService(IOrderRepository orderRepository, IBusinessmanRepository businessmanRepository, DomesticService service, IAfterSaleOrderRepository afterSaleOrderRepository, ITicketSumRepository ticketSumRepository, IPidService pidService, IInsuranceOrderRepository iInsuranceOrderRepository, ILocalPolicyRepository localPolicyRepository, ITicketRepository ticketRepository, IAriChangeRepository ariChangeRepository, IMyMessageRepository MyMessageRepository, IRefundReasonRepository RefundReasonRepository)
        {
            this.service = service;
            //this.service = new DomesticService(orderRepository, localPolicyRepository, businessmanRepository, ObjectFactory.GetInstance<BPiaoBao.DomesticTicket.Domain.Models.Deduction.IDeductionRepository>(), iInsuranceOrderRepository);
            this.orderRepository = orderRepository;
            this.businessmanRepository = businessmanRepository;
            this.afterSaleOrderRepository = afterSaleOrderRepository;
            this.ticketSumRepository = ticketSumRepository;
            this.ticketRepository = ticketRepository;
            this.pidService = pidService;
            this.localPolicyRepository = localPolicyRepository;
            this.ariChangeRepository = ariChangeRepository;
            this.MyMessageRepository = MyMessageRepository;
            this.RefundReasonRepository = RefundReasonRepository;
            flightDestineService = new FlightService(businessmanRepository, currentUser);
        }
        private Object root = new Object();
        //预定
        [ExtOperationInterceptor("白屏编码预定")]
        public PolicyPack Destine(DestineRequest destine, EnumDestineSource destineSource)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            #region 开始预定
            CreateOrderParam OrderParam = new CreateOrderParam();
            PolicyPack policyPack = new PolicyPack();
            var orderBuilder = AggregationFactory.CreateBuiler<OrderBuilder>();
            Order AdultOrder = null;
            bool HasAdult = false;
            try
            {
                DestineResponse response = new DestineResponse();
                if (currentUser == null)
                    throw new CustomException(111, "用户未登录或者掉线，请重新登陆！");
                //儿童关联成人订单号
                if (!string.IsNullOrEmpty(destine.OldOrderId))
                {
                    //检查关联的成人订单号是否存在
                    var adultOrder = orderRepository.FindAll(p => p.OrderId == destine.OldOrderId).FirstOrDefault();
                    if (adultOrder == null)
                        throw new OrderCommException("儿童关联的成人订单号（" + destine.OldOrderId + "）不存在！");
                    if (adultOrder.OrderType != 0)
                        throw new OrderCommException("输入的订单号非成人订单号(" + destine.OldOrderId + ")，请检查！");
                    if (((int)adultOrder.OrderStatus) < 6)
                        throw new OrderCommException("关联的成人订单号(" + destine.OldOrderId + ")未出票,请检查！");
                    if (!format.IsPnr(adultOrder.PnrCode))
                        throw new OrderCommException("关联的成人订单号(" + destine.OldOrderId + ")编码不能为空或者编码格式错误,请检查！");
                    List<SkyWay> adultSkyList = adultOrder.SkyWays.OrderBy(p => p.StartDateTime).ToList();
                    //比较航段
                    if (orderBuilder.CompareSky(adultSkyList, destine.SkyWay, destine.OldOrderId))
                        throw new OrderCommException("航段信息与关联的成人航段信息不一致,请检查关联订单信息！");
                    //比较关联的儿童验证
                    int adultCount = adultOrder.Passengers.Where(p => p.PassengerType == EnumPassengerType.Adult).Count();
                    //最多关联的儿童数
                    int maxAssocChdCount = adultCount * 2;
                    int chdCount = destine.Passengers.Where(p => p.PassengerType == 2).Count() + adultOrder.AssocChdCount;
                    if (chdCount > maxAssocChdCount)
                        throw new OrderCommException("该成人订单(" + destine.OldOrderId + ")关联的儿童数已超过成人所带儿童限制！");
                    else
                    {
                        adultOrder.AssocChdCount += chdCount;//更新所带儿童数
                        unitOfWorkRepository.PersistUpdateOf(adultOrder);
                    }
                    //儿童关联成人编码
                    destine.ChdRemarkAdultPnr = adultOrder.PnrCode;
                }
                sw.Stop();
                Logger.WriteLog(LogType.DEBUG, string.Format("Destine()-->预定编码前,执行时间:{0}", sw.ElapsedMilliseconds));
                sw.Restart();
                //预定编码
                response = flightDestineService.Destine(currentUser.Code, destine);
                sw.Stop();
                Logger.WriteLog(LogType.DEBUG, string.Format("Destine()-->预定编码完成,执行时间:{0}", sw.ElapsedMilliseconds));
                sw.Restart();
                EnumPnrImportType enumPnrImportType = destineSource == EnumDestineSource.WhiteScreenDestine ? EnumPnrImportType.WhiteScreenDestine : EnumPnrImportType.MobileDestine;
                //航段
                destine.SkyWay.ForEach(p =>
                {
                    OrderParam.SkyWayDtos.Add(new SkyWayDto()
                    {
                        CarrayCode = p.CarrayCode,
                        FromCityCode = p.FromCityCode,
                        ToCityCode = p.ToCityCode,
                        FlightNumber = p.FlightNumber,
                        StartDateTime = p.StartDate,
                        ToDateTime = p.EndDate,
                        Seat = p.Seat,
                        FromTerminal = p.FromTerminal,
                        ToTerminal = p.ToTerminal,
                        Discount = p.Discount.HasValue ? p.Discount.Value : 0m,
                        FlightModel = p.FlightModel
                    });
                });
                sw.Stop();
                Logger.WriteLog(LogType.DEBUG, string.Format("Destine()-->航段,执行时间:{0}", sw.ElapsedMilliseconds));
                sw.Restart();
                #region 成人订单
                if (!string.IsNullOrEmpty(response.AdultPnr)
                    && !string.IsNullOrEmpty(response.AdultPnrContent))
                {
                    bool HaveBabyFlag = false;
                    OrderParam.PassengerDtos = new List<PassengerDto>();
                    OrderParam.pnrData = response.AdultPnrData;
                    destine.Passengers.ForEach(p =>
                    {
                        if (p.PassengerType != 2)
                        {
                            OrderParam.PassengerDtos.Add(new PassengerDto()
                            {
                                PassengerName = p.PassengerName,
                                PassengerType = p.PassengerType == 1 ? EnumPassengerType.Adult : (p.PassengerType == 2 ? EnumPassengerType.Child : EnumPassengerType.Baby),
                                CardNo = p.CardNo,
                                Mobile = p.LinkPhone,
                                IdType = p.IdType,
                                SexType = p.SexType,
                                Birth = p.Birth
                            });
                        }
                        if (p.PassengerType == 3)
                        {
                            HaveBabyFlag = true;
                        }
                    });
                    if (OrderParam.PassengerDtos.Count > 0)
                    {
                        AdultOrder = new Order();
                        AdultOrder.OrderId = GetOrderId("0");//订单号
                        EnumOrderSource OrderSource = destineSource == EnumDestineSource.WhiteScreenDestine ? EnumOrderSource.WhiteScreenDestine : EnumOrderSource.MobileDestine;
                        AdultOrder.OrderSource = OrderSource;
                        AdultOrder.PnrSource = EnumPnrSource.CreatePnr;
                        //是否含有婴儿
                        AdultOrder.HaveBabyFlag = HaveBabyFlag;
                        //成人订单
                        AdultOrder.OrderType = 0;
                        AdultOrder.IsLowPrice = destine.IsLowPrice;

                        PolicyParam policyParam = new PolicyParam();
                        policyParam.PolicySpecialType = destine.PolicySpecialType;
                        policyParam.IsLowPrice = destine.IsLowPrice;
                        policyParam.pnrData = response.AdultPnrData;
                        policyParam.code = currentUser.Code;
                        policyParam.PnrContent = response.AdultPnrContent;
                        policyParam.OrderId = AdultOrder.OrderId;
                        policyParam.OrderType = AdultOrder.OrderType;
                        policyParam.OrderSource = OrderSource;
                        policyParam.IsChangePnrTicket = destine.IsChangePnr;
                        policyParam.IsDestine = OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                        //为固定特价时 或者普通政策时 取原始Ibe的舱位价 获取政策哪儿重新计算                       
                        if (destine.PolicySpecialType == EnumPolicySpecialType.Normal
                          || destine.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial
                            )
                        {
                            policyParam.IsUseSpecial = true;
                            policyParam.defFare = destine.IbeSeatPrice.ToString();
                            policyParam.defTAX = destine.IbeTaxFee.ToString();
                            policyParam.defRQFare = destine.IbeRQFee.ToString();
                        }
                        else
                        {
                            policyParam.defFare = destine.SpecialPrice.ToString();
                            policyParam.defTAX = destine.SpecialTax.ToString();
                            policyParam.defRQFare = destine.SpecialFuelFee.ToString();
                        }
                        //有成人
                        HasAdult = true;
                        //关联成人订单号
                        if (string.IsNullOrEmpty(destine.OldOrderId))
                            destine.OldOrderId = AdultOrder.OrderId;

                        //同时执行生成订单和获取政策
                        IList<Policy> policyList = new List<Policy>();
                        var parallelExceptions = new ConcurrentQueue<Exception>();
                        Parallel.Invoke(() =>
                            {
                                try
                                {
                                    //成人订单                   
                                    AdultOrder = orderBuilder.CreateOrderByPnrContent(AdultOrder, response.AdultPnrContent, enumPnrImportType, OrderParam, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, destine.IsChangePnr);
                                }
                                catch (Exception e)
                                {
                                    parallelExceptions.Enqueue(e);
                                }
                            },
                            () =>
                            {
                                try
                                {
                                    //获取政策
                                    policyList = NewGetPolicy(policyParam);
                                }
                                catch (Exception e)
                                {
                                    parallelExceptions.Enqueue(e);
                                }
                            }
                        );
                        //抛出并行的异常信息
                        if (parallelExceptions.Count > 0)
                        {
                            Exception ex;
                            parallelExceptions.TryDequeue(out ex);
                            throw new OrderCommException(ex.Message);
                        }
                        policyPack = GetPolicy(policyList, AdultOrder);
                        policyPack.OrderId = AdultOrder.OrderId;
                        //婴儿丢失日志
                        if (HaveBabyFlag && !response.INFPnrIsSame)
                        {
                            //订单日志            
                            AdultOrder.WriteLog(new OrderLog()
                            {
                                OperationDatetime = System.DateTime.Now,
                                OperationPerson = currentUser.OperatorName,
                                Remark = "",
                                OperationContent = string.Format("编码【{0}】中婴儿信息未能添加成功,婴儿请申请线下订单！", AdultOrder.PnrCode),
                                IsShowLog = true
                            });
                        }
                    }
                }
                #endregion
                sw.Stop();
                Logger.WriteLog(LogType.DEBUG, string.Format("Destine()-->航段,成人订单:{0}", sw.ElapsedMilliseconds));
                sw.Restart();
                #region 儿童订单
                if (!string.IsNullOrEmpty(response.ChdPnr)
                   && !string.IsNullOrEmpty(response.ChdPnrContent))
                {
                    OrderParam.PassengerDtos = new List<PassengerDto>();
                    OrderParam.pnrData = response.ChdPnrData;
                    destine.Passengers.ForEach(p =>
                    {
                        if (p.PassengerType == 2)
                        {
                            OrderParam.PassengerDtos.Add(new PassengerDto()
                            {
                                PassengerName = p.PassengerName,
                                PassengerType = p.PassengerType == 1 ? EnumPassengerType.Adult : (p.PassengerType == 2 ? EnumPassengerType.Child : EnumPassengerType.Baby),
                                CardNo = p.CardNo,
                                Mobile = p.LinkPhone,
                                IdType = p.IdType,
                                SexType = p.SexType,
                                Birth = p.Birth
                            });
                        }
                    });
                    if (OrderParam.PassengerDtos.Count > 0)
                    {
                        var order = new Order();
                        order.OrderId = GetOrderId("0");//订单号
                        order.OrderSource = destineSource == EnumDestineSource.WhiteScreenDestine ? EnumOrderSource.WhiteScreenDestine : EnumOrderSource.MobileDestine;
                        order.PnrSource = EnumPnrSource.CreatePnr;
                        order.IsLowPrice = destine.IsLowPrice;
                        //儿童订单
                        order.OrderType = 1;
                        if (AdultOrder != null)
                        {
                            AdultOrder.AssocChdCount = OrderParam.PassengerDtos.Count;
                        }
                        //儿童关联成人订单号
                        order.OldOrderId = destine.OldOrderId;

                        PolicyParam policyParam = new PolicyParam();
                        policyParam.IsLowPrice = destine.IsLowPrice;
                        policyParam.pnrData = response.ChdPnrData;
                        policyParam.code = currentUser.Code;
                        policyParam.PnrContent = response.ChdPnrContent;
                        policyParam.OrderId = order.OrderId;
                        policyParam.OrderType = order.OrderType;
                        policyParam.OrderSource = order.OrderSource;
                        policyParam.IsChangePnrTicket = destine.IsChangePnr;
                        policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                        PassengerDto pasData = OrderParam.PassengerDtos.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                        if (pasData != null)
                        {
                            policyParam.defFare = pasData.SeatPrice.ToString();
                            policyParam.defTAX = pasData.TaxFee.ToString();
                            policyParam.defRQFare = pasData.RQFee.ToString();
                        }
                        //同时执行生成订单和获取政策
                        IList<Policy> policyList = new List<Policy>();
                        var parallelExceptions = new ConcurrentQueue<Exception>();
                        Parallel.Invoke(() =>
                            {
                                try
                                {
                                    //儿童订单                
                                    order = orderBuilder.CreateOrderByPnrContent(order, response.ChdPnrContent, enumPnrImportType, OrderParam, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, destine.IsChangePnr);
                                }
                                catch (Exception e)
                                {
                                    parallelExceptions.Enqueue(e);
                                }
                            },
                           () =>
                           {
                               try
                               {
                                   //获取默认政策
                                   policyList = NewGetPolicy(policyParam);
                               }
                               catch (Exception e)
                               {
                                   parallelExceptions.Enqueue(e);
                               }
                           }
                        );
                        if (parallelExceptions.Count > 0)
                        {
                            Exception ex;
                            parallelExceptions.TryDequeue(out ex);
                            throw new OrderCommException(ex.Message);
                        }

                        //处理儿童政策
                        policyPack = GetChdPolicy(policyList, policyParam, policyPack, order);
                        //儿童选择默认政策
                        if (policyPack.PolicyList.Count > 0)
                        {
                            PolicyDto policy = policyPack.PolicyList.Where(p => p.Id.StartsWith(currentUser.CarrierCode + "_")
                                && p.DefaultPolicySource == 1).LastOrDefault();
                            //没有默认政策 取ctuadmin的默认政策
                            if (policy == null)
                            {
                                policy = policyPack.PolicyList.Where(p => p.Id.StartsWith(PnrHelper.DefAccount + "_")
                                && p.DefaultPolicySource == 1).LastOrDefault();
                            }
                            if (policy != null)
                            {
                                var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
                                behavior.SetParame("platformCode", policy.PlatformCode);
                                behavior.SetParame("policyId", policy.Id);
                                behavior.SetParame("policy", policy);
                                behavior.SetParame("operatorName", currentUser.OperatorName);
                                behavior.SetParame("source", "forward");
                                behavior.Execute();
                            }
                            if (HasAdult)
                            {
                                RemoveChdDefaultPolicy(policyPack);
                            }
                        }
                        unitOfWorkRepository.PersistCreationOf(order);
                    }
                }
                #endregion
                sw.Stop();
                Logger.WriteLog(LogType.DEBUG, string.Format("Destine()-->航段,儿童订单:{0}", sw.ElapsedMilliseconds));
                sw.Restart();
                if (AdultOrder != null)
                {
                    unitOfWorkRepository.PersistCreationOf(AdultOrder);
                    policyPack.OrderId = AdultOrder.OrderId;
                }
                policyPack.INFPnrIsSame = response.INFPnrIsSame;
                policyPack.AdultPnr = response.AdultPnr;
                policyPack.ChdPnr = response.ChdPnr;
                policyPack.OrderSource = EnumOrderSource.WhiteScreenDestine;
                //生成订单         
                unitOfWork.Commit();

            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("Destine_" + destineSource.ToEnumDesc(), ex.StackTrace + ex.Message);
                throw new CustomException(111, ex.Message);
            }
            #endregion
            sw.Stop();
            Logger.WriteLog(LogType.DEBUG, string.Format("Destine()--保存到数据库,执行时间:{0}", sw.ElapsedMilliseconds));

            return policyPack;
        }
        /// <summary>
        /// 获取儿童默认政策
        /// </summary>      
        /// <returns></returns>
        private PolicyPack GetChdPolicy(IList<Policy> policyList, PolicyParam policyParam, PolicyPack policyPack, Order order)
        {
            List<PolicyDto> tempPolicyDtoList = policyPack.PolicyList;
            policyPack = GetPolicy(policyList, order);
            policyPack.ChdOrderId = order.OrderId;
            policyPack.PolicyList.AddRange(tempPolicyDtoList.ToArray());
            return policyPack;
        }
        /// <summary>
        /// 移除儿童默认政策
        /// </summary>
        /// <param name="policyPack"></param>
        /// <returns></returns>
        private PolicyPack RemoveChdDefaultPolicy(PolicyPack policyPack)
        {
            List<PolicyDto> chdPolicyList = policyPack.PolicyList.Where(p => p.DefaultPolicySource == 1).ToList();
            if (chdPolicyList.Count > 0)
            {
                foreach (PolicyDto policyDto in chdPolicyList)
                {
                    policyPack.PolicyList.Remove(policyDto);
                }
            }
            return policyPack;
        }

        /// <summary>
        /// 编码导入 生成默认订单
        /// </summary>
        /// <param name="pnrContent">编码内容</param>
        /// <param name="PnrSource">编码来源</param> 
        /// <returns></returns>
        [ExtOperationInterceptor("编码或者编码内容导入")]
        public PolicyPack ImportPnrContext(PnrImportParam PnrParam)
        {

            PolicyPack policyPack = new PolicyPack();
            try
            {
                BehaviorStatService.SaveBehaviorStat(DataContracts.SystemSetting.EnumBehaviorOperate.ImportCount);
                var orderBuilder = AggregationFactory.CreateBuiler<OrderBuilder>();
                bool IsPnr = format.IsPnr(PnrParam.PnrAndPnrContent.Trim());
                string Pnr = string.Empty;
                string PnrContent = PnrParam.PnrAndPnrContent;
                string errMsg = string.Empty;
                var parallelExceptions = new ConcurrentQueue<Exception>();
                //PNR内容导入
                if (PnrParam.PnrImportType == EnumPnrImportType.PnrContentImport)
                {
                    #region PNR内容导入
                    //构造订单
                    var order = new Order();
                    order.OrderId = GetOrderId("0");//订单号
                    order.OrderSource = EnumOrderSource.PnrContentImport;
                    order.IsLowPrice = PnrParam.IsLowPrice;
                    PolicyParam policyParam = new PolicyParam();
                    policyParam.IsLowPrice = PnrParam.IsLowPrice;
                    policyParam.code = currentUser.Code;
                    policyParam.PnrContent = PnrContent;
                    policyParam.OrderId = order.OrderId;
                    //policyParam.OrderType = order.OrderType;
                    policyParam.OrderSource = order.OrderSource;
                    policyParam.IsChangePnrTicket = PnrParam.IsChangePnrTicket;
                    policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                    //Passenger pasData = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                    //if (pasData != null)
                    //{
                    //    policyParam.defFare = pasData.SeatPrice.ToString();
                    //    policyParam.defTAX = pasData.ABFee.ToString();
                    //    policyParam.defRQFare = pasData.RQFee.ToString();
                    //}
                    //同时执行生成订单和获取政策 
                    IList<Policy> policyList = new List<Policy>();
                    Parallel.Invoke(() =>
                    {
                        try
                        {
                            order = orderBuilder.CreateOrderByPnrContent(order, PnrContent, PnrParam.PnrImportType, null, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, PnrParam.IsChangePnrTicket);
                        }
                        catch (Exception e)
                        {
                            parallelExceptions.Enqueue(e);
                        }
                    }, () =>
                    {
                        try
                        {
                            //获取政策
                            policyList = NewGetPolicy(policyParam);
                        }
                        catch (Exception e)
                        {
                            parallelExceptions.Enqueue(e);
                        }
                    }
                    );
                    if (parallelExceptions.Count > 0)
                    {
                        Exception ex;
                        parallelExceptions.TryDequeue(out ex);
                        throw new OrderCommException(ex.Message);
                    }
                    //处理
                    policyPack = GetPolicy(policyList, order);
                    if (service.IsExistNewOrder(currentUser.Code, order.PnrCode))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]的订单已存在,请在订单列表直接支付！");
                    }

                    if (service.PnrIsPay(order))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]已经支付成功,不能再次导入生成订单");
                    }
                    else
                    {
                        //设置其他相同PNR订单无效
                        service.SetOrderStatusInvalid(currentUser.OperatorName, order.PnrCode, order.OrderId);
                    }
                    unitOfWorkRepository.PersistCreationOf(order);
                    #endregion
                }
                //编码导入
                else if (PnrParam.PnrImportType == EnumPnrImportType.GenericPnrImport)
                {
                    #region 编码导入
                    if (IsPnr)
                    {
                        Pnr = PnrParam.PnrAndPnrContent.Trim();
                        string strCmd = string.Format("RT{0}", Pnr);
                        string strRTContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "", true).Replace("^", "\r");
                        if (strRTContent.Contains("授权"))
                            throw new OrderCommException(strRTContent);
                        PnrModel pModel = format.GetPNRInfo(Pnr, strRTContent, false, out errMsg);
                        //如果是团编码 发送团编码指令
                        if (pModel._PnrType == "2")
                        {
                            strCmd = string.Format("RT{0}|RTN", Pnr);
                            strRTContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "", true).Replace("^", "\r");
                        }
                        strCmd = string.Format("RT{0}|PAT:A", Pnr);
                        string strPATContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                        string strINFPATContent = string.Empty;
                        //是否有婴儿
                        if (pModel.HasINF)
                        {
                            strCmd = string.Format("RT{0}|PAT:A*IN", Pnr);
                            strINFPATContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                        }
                        PnrContent = strRTContent + "\r\n" + strPATContent + "\r\n" + strINFPATContent;
                    }


                    //构造订单
                    var order = new Order();
                    order.IsLowPrice = PnrParam.IsLowPrice;
                    order.OrderId = GetOrderId("0");//订单号
                    order.OrderSource = EnumOrderSource.PnrImport;
                    PolicyParam policyParam = new PolicyParam();
                    policyParam.IsLowPrice = PnrParam.IsLowPrice;
                    policyParam.code = currentUser.Code;
                    policyParam.PnrContent = PnrContent;
                    policyParam.OrderId = order.OrderId;
                    policyParam.OrderSource = order.OrderSource;
                    policyParam.IsChangePnrTicket = PnrParam.IsChangePnrTicket;
                    policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;

                    //同时执行生成订单和获取政策  
                    IList<Policy> policyList = new List<Policy>();
                    Parallel.Invoke(() =>
                    {
                        try
                        {
                            order = orderBuilder.CreateOrderByPnrContent(order, PnrContent, PnrParam.PnrImportType, null, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, PnrParam.IsChangePnrTicket);
                        }
                        catch (Exception e)
                        {
                            parallelExceptions.Enqueue(e);
                        }
                    }, () =>
                    {
                        try
                        {
                            //获取政策
                            policyList = NewGetPolicy(policyParam);
                        }
                        catch (Exception e)
                        {
                            parallelExceptions.Enqueue(e);
                        }
                    }
                    );
                    if (parallelExceptions.Count > 0)
                    {
                        Exception ex;
                        parallelExceptions.TryDequeue(out ex);
                        throw new OrderCommException(ex.Message);
                    }
                    //处理
                    policyPack = GetPolicy(policyList, order);
                    if (service.IsExistNewOrder(currentUser.Code, order.PnrCode))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]的订单已存在,请在订单列表直接支付！");
                    }
                    //获取政策
                    //policyPack = GetPolicy(PnrContent, order);


                    if (service.PnrIsPay(order))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]已经支付成功,不能再次导入生成订单");
                    }
                    else
                    {
                        //设置其他相同PNR订单无效
                        service.SetOrderStatusInvalid(currentUser.OperatorName, order.PnrCode, order.OrderId);
                    }
                    unitOfWorkRepository.PersistCreationOf(order);
                    #endregion
                }
                //儿童编码导入
                else if (PnrParam.PnrImportType == EnumPnrImportType.CHDPnrImport)
                {
                    #region 儿童编码导入
                    if (string.IsNullOrEmpty(PnrParam.OldOrderId))
                        throw new OrderCommException("儿童编码或者内容导入,关联的成人订单号不能为空！");
                    //检查关联的成人订单号是否存在
                    var adultOrder = orderRepository.FindAll(p => p.OrderId == PnrParam.OldOrderId).FirstOrDefault();
                    if (adultOrder == null)
                        throw new OrderCommException("儿童编码或者内容导入,关联的成人订单号（" + PnrParam.OldOrderId + "）不存在！");
                    if (adultOrder.OrderType != 0)
                        throw new OrderCommException("儿童编码或者内容导入,输入的订单号不是成人订单号(" + PnrParam.OldOrderId + ")，请检查！");
                    if (string.IsNullOrEmpty(adultOrder.PnrCode))
                        throw new OrderCommException("关联的成人订单号（" + PnrParam.OldOrderId + "）所对应的编码不能为空！");
                    if (((int)adultOrder.OrderStatus) < 6)
                        throw new OrderCommException("儿童编码或者内容导入,关联的成人订单号(" + PnrParam.OldOrderId + ")未出票,请检查！");
                    if (IsPnr)
                    {
                        Pnr = PnrParam.PnrAndPnrContent.Trim();
                        string strCmd = string.Format("RT{0}", Pnr);
                        string strRTContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "", true).Replace("^", "\r");
                        if (strRTContent.Contains("授权"))
                            throw new OrderCommException(strRTContent);

                        strCmd = string.Format("RT{0}|PAT:A*CH", Pnr);
                        string strPATContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                        if (strPATContent.Contains("CAN NOT USE *CH FOR NON CHD PASSENGER"))
                            throw new OrderCommException(Pnr + "不是儿童编码,取不到儿童价格,原因如下:" + strPATContent);
                        PnrContent = strRTContent + "\r\n" + strPATContent;
                    }

                    var order = new Order();
                    order.OrderId = GetOrderId("0");//订单号
                    order.IsLowPrice = PnrParam.IsLowPrice;
                    order.OrderSource = EnumOrderSource.ChdPnrImport;
                    PolicyParam policyParam = new PolicyParam();
                    policyParam.IsLowPrice = PnrParam.IsLowPrice;
                    policyParam.code = currentUser.Code;
                    policyParam.PnrContent = PnrContent;
                    policyParam.OrderId = order.OrderId;
                    policyParam.OrderSource = order.OrderSource;
                    policyParam.IsChangePnrTicket = PnrParam.IsChangePnrTicket;
                    policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                    //同时执行生成订单和获取政策
                    IList<Policy> policyList = new List<Policy>();
                    Parallel.Invoke(() =>
                    {
                        try
                        {
                            //构造订单
                            order = orderBuilder.CreateOrderByPnrContent(order, PnrContent, PnrParam.PnrImportType, null, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, PnrParam.IsChangePnrTicket);
                            //验证航段信息是否一致
                            List<SkyWay> adultSkyList = adultOrder.SkyWays.OrderBy(p => p.StartDateTime).ToList();
                            List<SkyWay> chdSkyList = order.SkyWays.OrderBy(p => p.StartDateTime).ToList();
                            //比较航段
                            if (orderBuilder.CompareSky(adultSkyList, chdSkyList, PnrParam.OldOrderId))// "CHDPnrImport")
                                throw new OrderCommException("儿童航段信息与关联的成人航段信息不一致,请检查编码内容航段信息！");

                            //比较关联的儿童验证
                            int adultCount = adultOrder.Passengers.Where(p => p.PassengerType == EnumPassengerType.Adult).Count();
                            //最多关联的儿童数
                            int maxAssocChdCount = adultCount * 2;
                            int chdCount = order.Passengers.Where(p => p.PassengerType == EnumPassengerType.Child).Count() + adultOrder.AssocChdCount;
                            if (chdCount > maxAssocChdCount)
                                throw new OrderCommException("该成人订单(" + PnrParam.OldOrderId + ")关联的儿童数已超过成人所带儿童限制！");
                            else
                            {
                                adultOrder.AssocChdCount += chdCount;//更新所带儿童数
                            }

                            //关联成人订单号
                            order.OldOrderId = PnrParam.OldOrderId;
                            //关联编码
                            if (!string.IsNullOrEmpty(adultOrder.PnrCode))
                            {
                                string CarrayCode = order.SkyWays[0].CarrayCode;
                                this.pidService.ChdRemarkAdultPnr(currentUser.Code, order.PnrCode, adultOrder.PnrCode, CarrayCode, order.YdOffice);
                            }
                            if (service.IsExistNewOrder(currentUser.Code, order.PnrCode))
                            {
                                throw new OrderCommException("该编码[" + order.PnrCode + "]的订单已存在,请在订单列表直接支付！");
                            }
                        }
                        catch (Exception e)
                        {
                            parallelExceptions.Enqueue(e);
                        }
                    }
                    , () =>
                    {
                        try
                        {
                            //获取默认政策
                            policyList = NewGetPolicy(policyParam);
                        }
                        catch (Exception e)
                        {
                            parallelExceptions.Enqueue(e);
                        }
                    }
                    );

                    if (parallelExceptions.Count > 0)
                    {
                        Exception ex;
                        parallelExceptions.TryDequeue(out ex);
                        throw new OrderCommException(ex.Message);
                    }
                    unitOfWorkRepository.PersistUpdateOf(adultOrder);
                    //处理
                    policyPack = GetChdPolicy(policyList, policyParam, policyPack, order);
                    //儿童选择默认政策
                    if (policyPack.PolicyList.Count > 0)
                    {
                        PolicyDto policy = policyPack.PolicyList.Where(p => p.Id.StartsWith(currentUser.CarrierCode + "_") && p.DefaultPolicySource == 1).LastOrDefault();
                        //没有默认政策 取ctuadmin的默认政策
                        if (policy == null)
                        {
                            policy = policyPack.PolicyList.Where(p => p.Id.StartsWith(PnrHelper.DefAccount + "_")
                              && p.DefaultPolicySource == 1).LastOrDefault();
                        }
                        if (policy != null)
                        {
                            var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
                            behavior.SetParame("platformCode", policy.PlatformCode);
                            behavior.SetParame("policyId", policy.Id);
                            behavior.SetParame("operatorName", currentUser.OperatorName);
                            behavior.SetParame("source", "forward");
                            behavior.SetParame("policy", policy);
                            behavior.Execute();
                        }
                    }
                    if (service.PnrIsPay(order))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]已经支付成功,不能再次导入生成订单");
                    }
                    else
                    {
                        //设置其他相同PNR订单无效
                        service.SetOrderStatusInvalid(currentUser.OperatorName, order.PnrCode, order.OrderId);
                    }
                    //添加
                    unitOfWorkRepository.PersistCreationOf(order);
                    #endregion
                }
                //升舱换开导入
                else if (PnrParam.PnrImportType == EnumPnrImportType.UpSeatChangePnrImport)
                {
                    #region 升舱换开
                    if (string.IsNullOrEmpty(PnrParam.OldOrderId))
                        throw new OrderCommException("升舱换开原订单号不能为空！");
                    //检查关联的成人订单号是否存在
                    var upgradeOrder = orderRepository.FindAll(p => p.OrderId == PnrParam.OldOrderId).FirstOrDefault();
                    if (upgradeOrder == null)
                        throw new OrderCommException("升舱换开原订单号不存在！");
                    if (((int)upgradeOrder.OrderStatus) < 6)
                        throw new OrderCommException("升舱换开原订单号(" + PnrParam.OldOrderId + ")未出票,请检查！");
                    if (IsPnr)
                    {
                        Pnr = PnrParam.PnrAndPnrContent.Trim();
                        string strCmd = string.Format("RT{0}", Pnr);
                        string strRTContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "", true).Replace("^", "\r");
                        if (strRTContent.Contains("授权"))
                            throw new OrderCommException(strRTContent);

                        PnrModel pModel = format.GetPNRInfo(Pnr, strRTContent, false, out errMsg);
                        string strPATContent = string.Empty;
                        string strINFPATContent = string.Empty;
                        if (upgradeOrder.OrderType == 0)
                        {
                            //如果是团编码 发送团编码指令
                            if (pModel._PnrType == "2")
                            {
                                strCmd = string.Format("RT{0}|RTN", Pnr);
                                strRTContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "", true).Replace("^", "\r");
                            }
                            strCmd = string.Format("RT{0}|PAT:A", Pnr);
                            strPATContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                            //是否有婴儿
                            if (pModel.HasINF)
                            {
                                strCmd = string.Format("RT{0}|PAT:A*IN", Pnr);
                                strINFPATContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                            }
                            strPATContent += "\r\n" + strINFPATContent;
                        }
                        else
                        {
                            strCmd = string.Format("RT{0}|PAT:A*CH", Pnr);
                            strPATContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                        }
                        PnrContent = strRTContent + "\r\n" + strPATContent;
                    }
                    var order = new Order();
                    order.OrderId = GetOrderId("0");//订单号
                    order.IsLowPrice = PnrParam.IsLowPrice;
                    order.OrderSource = EnumOrderSource.UpSeatChangePnrImport;
                    //构造订单
                    order = orderBuilder.CreateOrderByPnrContent(order, PnrContent, PnrParam.PnrImportType, null, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, upgradeOrder.IsChangePnrTicket);
                    //if (order.Passengers.Count != upgradeOrder.Passengers.Count)
                    //    throw new OrderCommException("升舱换开," + order.PnrCode + "中乘客人数与原订单（" + PnrParam.OldOrderId + "）乘客人数不一致,请仔细检查！");
                    if (order.SkyWays == null || order.SkyWays.Count == 0)
                        throw new OrderCommException("升舱换开," + order.PnrCode + " 航段错误！");
                    if (order.SkyWays.Count != upgradeOrder.SkyWays.Count)
                        throw new OrderCommException("升舱换开," + order.PnrCode + "中航段数与原订单（" + PnrParam.OldOrderId + "）航段数不一致,请仔细检查！");
                    //验证航段信息是否一致
                    List<SkyWay> adultSkyList = upgradeOrder.SkyWays.OrderBy(p => p.StartDateTime).ToList();
                    List<SkyWay> chdSkyList = order.SkyWays.OrderBy(p => p.StartDateTime).ToList();
                    //比较航段
                    if (orderBuilder.CompareSky(adultSkyList, chdSkyList, PnrParam.OldOrderId, "upgradeOrder"))
                        throw new OrderCommException("升舱换开,航段信息与原单的航段信息不一致！");
                    //关联订单号
                    order.OldOrderId = PnrParam.OldOrderId;

                    if (order.OrderType == 1 && upgradeOrder.OrderType == 0 && IsPnr)
                    {
                        //关联编码
                        if (!string.IsNullOrEmpty(upgradeOrder.PnrCode))
                        {
                            string CarrayCode = order.SkyWays[0].CarrayCode;
                            this.pidService.ChdRemarkAdultPnr(currentUser.Code, order.PnrCode, upgradeOrder.PnrCode, CarrayCode, order.YdOffice);
                        }
                    }


                    PolicyParam policyParam = new PolicyParam();
                    policyParam.IsLowPrice = PnrParam.IsLowPrice;
                    policyParam.code = currentUser.Code;
                    policyParam.PnrContent = PnrContent;
                    policyParam.OrderId = order.OrderId;
                    policyParam.OrderSource = order.OrderSource;
                    policyParam.IsChangePnrTicket = PnrParam.IsChangePnrTicket;
                    policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                    Passenger pasData = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                    if (pasData != null)
                    {
                        policyParam.defFare = pasData.SeatPrice.ToString();
                        policyParam.defTAX = pasData.ABFee.ToString();
                        policyParam.defRQFare = pasData.RQFee.ToString();
                    }
                    //获取政策
                    IList<Policy> policyList = NewGetPolicy(policyParam);
                    if (order.OrderType == 0)
                    {
                        //处理                                               
                        policyPack = GetPolicy(policyList, order);
                        order.ChangeStatus(EnumOrderStatus.WaitChoosePolicy);
                    }
                    else
                    {
                        //处理儿童默认政策
                        policyPack = GetChdPolicy(policyList, policyParam, policyPack, order);
                        //儿童选择默认政策
                        if (policyPack.PolicyList.Count > 0)
                        {
                            PolicyDto policy = policyPack.PolicyList.Where(p => p.Id.StartsWith(currentUser.CarrierCode + "_") && p.DefaultPolicySource == 1).LastOrDefault();
                            //没有默认政策 取ctuadmin的默认政策
                            if (policy == null)
                            {
                                policy = policyPack.PolicyList.Where(p => p.Id.StartsWith(PnrHelper.DefAccount + "_")
                               && p.DefaultPolicySource == 1).LastOrDefault();
                            }
                            if (policy != null)
                            {
                                var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
                                behavior.SetParame("platformCode", policy.PlatformCode);
                                behavior.SetParame("policyId", policy.Id);
                                behavior.SetParame("operatorName", currentUser.OperatorName);
                                behavior.SetParame("source", "forward");
                                behavior.SetParame("policy", policy);
                                behavior.Execute();
                            }
                        }
                    }
                    policyPack.OrderId = order.OrderId;

                    if (service.IsExistNewOrder(currentUser.Code, order.PnrCode))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]的订单已存在,请在订单列表直接支付！");
                    }
                    if (service.PnrIsPay(order))
                    {
                        throw new OrderCommException("该编码[" + order.PnrCode + "]已经支付成功,不能再次导入生成订单");
                    }
                    else
                    {
                        //设置其他相同PNR订单无效
                        service.SetOrderStatusInvalid(currentUser.OperatorName, order.PnrCode, order.OrderId);
                    }
                    //添加
                    unitOfWorkRepository.PersistCreationOf(order);
                    #endregion
                }
                //生成订单         
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("ImportPnrContext", ex.StackTrace + ex.Message);
                throw new CustomException(111, ex.Message);
            }
            return policyPack;
        }
        /// <summary>
        /// 通过订单号取编码内容导入生成新订单
        /// </summary>      
        /// <returns></returns>
        [ExtOperationInterceptor("通过订单号取编码内容导入生成新订单")]
        public PolicyPack ImportByOrderId(string orderId)
        {
            PolicyPack policyPack = new PolicyPack();
            try
            {
                var oldOrder = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                if (oldOrder == null)
                    throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
                var orderBuilder = AggregationFactory.CreateBuiler<OrderBuilder>();
                string PnrContent = oldOrder.PnrContent;
                if (string.IsNullOrEmpty(PnrContent))
                    throw new OrderCommException("单编号：" + orderId + "没有找到对应的编码内容，无法导入！");

                var order = new Order();
                order.OrderId = GetOrderId("0");//订单号
                order.OldOrderId = orderId;
                order.OrderSource = EnumOrderSource.PnrImport;

                PolicyParam policyParam = new PolicyParam();
                policyParam.code = currentUser.Code;
                policyParam.PnrContent = oldOrder.PnrContent;
                policyParam.OrderId = order.OrderId;
                //policyParam.OrderType = order.OrderType;
                policyParam.OrderSource = order.OrderSource;
                policyParam.IsChangePnrTicket = oldOrder.IsChangePnrTicket;
                policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                //Passenger pasData = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                //if (pasData != null)
                //{
                //    policyParam.defFare = pasData.SeatPrice.ToString();
                //    policyParam.defTAX = pasData.ABFee.ToString();
                //    policyParam.defRQFare = pasData.RQFee.ToString();
                //}
                var parallelExceptions = new ConcurrentQueue<Exception>();
                IList<Policy> policyLisy = new List<Policy>();
                Parallel.Invoke(() =>
                {
                    try
                    {
                        //构造订单
                        order = orderBuilder.CreateOrderByPnrContent(order, PnrContent, EnumPnrImportType.GenericPnrImport, null, currentUser.Code, currentUser.CarrierCode, currentUser.BusinessmanName, currentUser.OperatorName, oldOrder.IsChangePnrTicket);
                    }
                    catch (Exception e)
                    {
                        parallelExceptions.Enqueue(e);
                    }
                }, () =>
                {
                    try
                    {
                        policyLisy = NewGetPolicy(policyParam);
                    }
                    catch (Exception e)
                    {
                        parallelExceptions.Enqueue(e);
                    }
                });
                if (parallelExceptions.Count > 0)
                {
                    Exception ex;
                    parallelExceptions.TryDequeue(out ex);
                    throw new OrderCommException(ex.Message);
                }
                policyPack = GetPolicy(policyLisy, order);
                //获取政策
                //policyPack = GetPolicy(PnrContent, order);
                unitOfWorkRepository.PersistCreationOf(order);
                //生成订单         
                unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("ImportByOrderId", ex.StackTrace + ex.Message);
                throw new CustomException(111, ex.Message);
            }
            return policyPack;
        }

        /// <summary>
        /// 选择政策
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="policyId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("选择政策【ChoosePolicy(string platformCode, string policyId, string orderId)】")]
        public OrderDto ChoosePolicy(string platformCode, string policyId, string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();

            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (order.OrderStatus == EnumOrderStatus.PaymentInWaiting)
                throw new OrderCommException("该订单【" + orderId + "】正在支付中,请稍后。。。");
            if (service.PnrIsPay(order) || (order.OrderPay != null && order.OrderPay.PayStatus == EnumPayStatus.OK))
                throw new OrderCommException("该编码已经支付成功,不能再次导入生成订单");

            var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
            behavior.SetParame("platformCode", platformCode);
            behavior.SetParame("policyId", policyId);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("source", "forward");
            behavior.SetParame("policy", "");
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            OrderDto orderDto = order.ToOrderDto();
            //自动授权
            if (pidService.GetOffice(currentUser.CarrierCode).Contains(order.YdOffice.ToUpper())
                && (!string.IsNullOrEmpty(order.CpOffice))
                && order.CpOffice != order.YdOffice
                )
            {
                orderDto.IsAuthSuc = pidService.AuthToOffice(order.BusinessmanCode, order.CpOffice, order.YdOffice, order.PnrCode);
                if (!orderDto.IsAuthSuc)
                {
                    orderDto.AuthInfo = string.Format("请授权,授权指令:RMK TJ AUTH {0}", order.CpOffice);
                }
            }
            return orderDto;
        }

        /// <summary>
        /// 更改订单的状态信息(支付超时就失效)
        /// </summary>
        /// <param name="orderId"></param>
        [ExtOperationInterceptor("更改订单的状态信息(支付超时就失效)")]
        public void UpdateOrderStatus(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order.OrderStatus == EnumOrderStatus.NewOrder || order.OrderStatus == EnumOrderStatus.WaitChoosePolicy)
            {
                order.OrderStatus = EnumOrderStatus.Invalid;
                //修改
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// 更改订单的支付金额信息GG
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="totalmoney"></param>
        [ExtOperationInterceptor("更改订单的支付金额信息")]
        public void UpdateOrderPayMoney(string orderId, decimal totalmoney)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            order.OrderPay.PayMoney = totalmoney;
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        /// <summary>
        /// 支付宝快捷支付代扣支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="password"></param>
        [ExtOperationInterceptor("支付宝快捷支付代扣支付")]
        public string PayOrderByQuikAliPay(string orderId, string password)
        {
            try
            {
                string result = string.Empty;
                //加载订单信息
                var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                if (order == null) throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
                var behavior = order.State.GetBehaviorByCode("PayOrder");
                behavior.SetParame("Code", currentUser.Code);
                behavior.SetParame("cashbagCode", currentUser.CashbagCode);
                behavior.SetParame("cashbagKey", currentUser.CashbagKey);
                behavior.SetParame("operatorName", currentUser.OperatorName);
                behavior.SetParame("payType", EnumPayMethod.AliPay);
                behavior.SetParame("payPassword", password);
                behavior.SetParame("collaboratorKey", collaboratorKey);
                behavior.SetParame("platformCode", order.Policy.PlatformCode);
                behavior.SetParame("platformName", order.Policy.PolicySourceType == EnumPolicySourceType.Interface
                        ? service.GetPlatList()[order.Policy.PlatformCode].Name
                        : "系统");
                result = behavior.Execute().ToString();
                //修改订单
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
                //生成接口订单和代付            
                service.CreatePlatformOrderAndPaid(order.OrderId, currentUser.OperatorName, EnumPayMethod.AliPay.ToEnumDesc() + "支付");
                //支付成功自动出票           
                service.AutoIssue(orderId, EnumPayMethod.AliPay.ToEnumDesc() + "支付", () => MessageQueueManager.SendMessage(orderId, 0));
                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException(00001, ex.Message);
            }
        }

        /// <summary>
        /// 钱袋子现金账户支付
        /// </summary>
        /// <param name="orderId"></param>
        [ExtOperationInterceptor("钱袋子现金账户支付")]
        public void PayOrderByCashbagAccount(string orderId, string payPassword)
        {
            try
            {
                //加载订单信息
                var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();

                if (order == null)
                    throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

                var behavior = order.State.GetBehaviorByCode("PayOrder");
                behavior.SetParame("Code", currentUser.Code);
                behavior.SetParame("cashbagCode", currentUser.CashbagCode);
                behavior.SetParame("cashbagKey", currentUser.CashbagKey);
                behavior.SetParame("operatorName", currentUser.OperatorName);
                behavior.SetParame("payType", EnumPayMethod.Account);
                behavior.SetParame("payPassword", payPassword);
                behavior.SetParame("collaboratorKey", collaboratorKey);
                behavior.SetParame("platformCode", order.Policy.PlatformCode);
                behavior.SetParame("platformName",
                    order.Policy.PolicySourceType == EnumPolicySourceType.Interface
                        ? service.GetPlatList()[order.Policy.PlatformCode].Name
                        : "系统");
                behavior.Execute();
                //修改订单
                unitOfWorkRepository.PersistUpdateOf(order);

                unitOfWork.Commit();

                //生成接口订单和代付            
                service.CreatePlatformOrderAndPaid(order.OrderId, currentUser.OperatorName, EnumPayMethod.Account.ToEnumDesc() + "支付");

                //支付成功自动出票           
                service.AutoIssue(orderId, "现金账户支付", () => MessageQueueManager.SendMessage(orderId, 0));
            }
            catch (Exception ex)
            {
                throw new CustomException(00001, ex.Message);
            }
            //AutoIssueManage.Add(orderId);
        }

        /// <summary>
        /// 信用支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="paypassword"></param>
        [ExtOperationInterceptor("钱袋子信用支付")]
        public void PayOrderByCreditAccount(string orderId, string paypassword)
        {
            try
            {
                BehaviorStatService.SaveBehaviorStat(DataContracts.SystemSetting.EnumBehaviorOperate.UseCount);

                var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                if (order == null)
                    throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

                var behavior = order.State.GetBehaviorByCode("PayOrder");
                behavior.SetParame("Code", currentUser.Code);
                behavior.SetParame("cashbagCode", currentUser.CashbagCode);
                behavior.SetParame("cashbagKey", currentUser.CashbagKey);
                behavior.SetParame("operatorName", currentUser.OperatorName);
                behavior.SetParame("payType", EnumPayMethod.Credit);
                behavior.SetParame("payPassword", paypassword);
                behavior.SetParame("collaboratorKey", collaboratorKey);
                behavior.SetParame("platformCode", order.Policy.PlatformCode);
                behavior.SetParame("platformName",
                    order.Policy.PolicySourceType == EnumPolicySourceType.Interface
                        ? service.GetPlatList()[order.Policy.PlatformCode].Name
                        : "系统");
                behavior.Execute();
                //修改订单
                unitOfWorkRepository.PersistUpdateOf(order);

                unitOfWork.Commit();
                //生成接口订单和代付            
                service.CreatePlatformOrderAndPaid(order.OrderId, currentUser.OperatorName, EnumPayMethod.Credit.ToEnumDesc() + "支付");

                //支付成功自动出票           
                service.AutoIssue(orderId, "信用支付", () => MessageQueueManager.SendMessage(orderId, 0));
            }
            catch (Exception ex)
            {
                throw new CustomException(00001, ex.Message);
            }
            //AutoIssueManage.Add(orderId);
        }

        /// <summary>
        /// 银行卡支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("钱袋子银行卡支付")]
        public string PayOrderByBank(string orderId, string bankCode)
        {
            string result = string.Empty;
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

            var behavior = order.State.GetBehaviorByCode("PayOrder");
            behavior.SetParame("cashbagCode", currentUser.CashbagCode);
            behavior.SetParame("Code", currentUser.Code);
            behavior.SetParame("cashbagKey", currentUser.CashbagKey);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("payType", EnumPayMethod.Bank);
            behavior.SetParame("bank", bankCode);
            behavior.SetParame("collaboratorKey", collaboratorKey);
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            behavior.SetParame("platformName",
                order.Policy.PolicySourceType == EnumPolicySourceType.Interface
                    ? service.GetPlatList()[order.Policy.PlatformCode].Name
                    : "系统");

            result = behavior.Execute().ToString();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            return result;
        }
        /// <summary>
        /// 使用支付平台支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("钱袋子支付平台支付")]
        public string PayOrderByPlatform(string orderId, string platform)
        {
            string result = string.Empty;
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

            var behavior = order.State.GetBehaviorByCode("PayOrder");
            behavior.SetParame("Code", currentUser.Code);
            behavior.SetParame("cashbagCode", currentUser.CashbagCode);
            behavior.SetParame("cashbagKey", currentUser.CashbagKey);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("payType", EnumPayMethod.Platform);
            behavior.SetParame("platform", platform);
            behavior.SetParame("collaboratorKey", collaboratorKey);
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            behavior.SetParame("platformName",
                order.Policy.PolicySourceType == EnumPolicySourceType.Interface
                    ? service.GetPlatList()[order.Policy.PlatformCode].Name
                    : "系统");

            result = behavior.Execute().ToString();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            return result;
        }


        [ExtOperationInterceptor("订单查询【FindAll】")]
        public DataPack<OrderDto> FindAll(string PaySerialNumber, string orderId, string pnr, string passengerName, string ticketNumber, string fromCity,
            string toCity, DateTime? startDateTime, DateTime? toDateTime, string businessmanCode, DateTime? startCreateTime, DateTime? endDateTime,
            string carrayCode, string platformCode, int[] orderStatus, int[] specialquery, int? specialschk, int startIndex, int count, bool? interfaceOrder, bool? shareOrder, bool? localOrder, string IsProcess = "")
        {
            //手机端过滤失效订单处理
            var query = orderRepository.FindAll();
            //Expression<Func<int, bool>> exp = s => s == 4;
            //Expression<Func<int, bool>> exp2 = s => specialquery.Contains(s);
            //var exp3 = exp.Or(exp2);
            //Predicate<EnumOrderSource> t1 = (p) => p == EnumOrderSource.UpSeatChangePnrImport;
            //Predicate<int> t2 = (p) => specialquery.Contains(p);
            if (specialschk == 1 && specialquery != null && specialquery.Length > 0)
            {
                if (specialquery.Contains(2))
                {
                    //婴儿/儿童
                    query = query.Where(p => specialquery.Contains(p.OrderType) || (p.HaveBabyFlag && p.OrderType == 0) || p.OrderSource == EnumOrderSource.UpSeatChangePnrImport);

                }
                else
                {
                    //儿童
                    query = query.Where(p => specialquery.Contains(p.OrderType) || p.OrderSource == EnumOrderSource.UpSeatChangePnrImport);
                }

            }
            else if (specialschk == 1)
            {
                query = query.Where(p => p.OrderSource == EnumOrderSource.UpSeatChangePnrImport);
            }
            else
            {
                if (specialquery != null && specialquery.Length > 0)
                {
                    if (specialquery.Contains(2))
                    {
                        //婴儿/儿童
                        query = query.Where(p => specialquery.Contains(p.OrderType) || (p.HaveBabyFlag && p.OrderType == 0));

                    }
                    else
                    {
                        //儿童
                        query = query.Where(p => specialquery.Contains(p.OrderType));
                    }
                }

            }


            IList<IQueryable<Order>> list = new List<IQueryable<Order>>();

            if (shareOrder.HasValue && shareOrder.Value) list.Add(query.Where(q => q.Policy.PolicySourceType == EnumPolicySourceType.Share));
            if (interfaceOrder.HasValue && interfaceOrder.Value) list.Add(query.Where(q => q.Policy.PolicySourceType == EnumPolicySourceType.Interface));
            if (localOrder.HasValue && localOrder.Value) list.Add(query.Where(q => q.Policy.PolicySourceType == EnumPolicySourceType.Local));
            IQueryable<Order> result = null;
            foreach (var record in list)
            {
                if (result == null)
                {
                    result = record;
                }
                else
                {
                    result = result.Union(record);
                }
            }
            if (result != null)
            {
                query = query.Intersect(result);
            }
            //if(IsProcess == "1" && shareOrder && !interfaceOrder)
            //    query = query.Where(p => p.Policy.PolicySourceType == EnumPolicySourceType.Share);
            //if (IsProcess == "1" && interfaceOrder && !shareOrder)//控台能处理的单子
            //    query = query.Where(p => p.Policy.PolicySourceType == EnumPolicySourceType.Interface);
            //if (IsProcess == "1" && !interfaceOrder && !shareOrder && localOrder)
            //    query = query.Where(p => p.Policy.PolicySourceType == EnumPolicySourceType.Local);
            //if (IsProcess == "1" && shareOrder && interfaceOrder && localOrder)
            //    query = query.Where(p => p.Policy.PolicySourceType == EnumPolicySourceType.Share || p.Policy.PolicySourceType == EnumPolicySourceType.Interface || p.Policy.PolicySourceType == EnumPolicySourceType.Local);
            if (!string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(orderId.Trim()))
                query = query.Where(p => p.OrderId == orderId.Trim());
            if (!string.IsNullOrEmpty(PaySerialNumber) && !string.IsNullOrEmpty(PaySerialNumber.Trim()))
                query = query.Where(p => p.OrderPay.PaySerialNumber == PaySerialNumber.Trim());
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.Passengers.Where(t => t.PassengerName.Contains(passengerName.Trim())).Count() > 0);
            if (!string.IsNullOrEmpty(ticketNumber) && !string.IsNullOrEmpty(ticketNumber.Trim()))
                query = query.Where(p => p.Passengers.Where(t => t.TicketNumber.Contains(ticketNumber.Trim())).Count() > 0);

            //if (string.IsNullOrEmpty(PaySerialNumber) && string.IsNullOrEmpty(orderId) && string.IsNullOrEmpty(pnr) && string.IsNullOrEmpty(passengerName) && string.IsNullOrEmpty(ticketNumber))
            //{
            if (!string.IsNullOrEmpty(fromCity) && !string.IsNullOrEmpty(fromCity.Trim()))
                query = query.Where(p => p.SkyWays.Where(t => t.FromCityCode == fromCity.Trim()).Count() > 0);
            if (!string.IsNullOrEmpty(toCity) && !string.IsNullOrEmpty(toCity.Trim()))
                query = query.Where(p => p.SkyWays.Where(t => t.ToCityCode == toCity.Trim()).Count() > 0);
            if (startDateTime != null)
                query = query.Where(p => p.SkyWays.Where(t => t.StartDateTime > startDateTime).Count() > 0);
            if (toDateTime != null)
                query = query.Where(p => p.SkyWays.Where(t => t.StartDateTime <= toDateTime).Count() > 0);
            if (!string.IsNullOrEmpty(carrayCode) && !string.IsNullOrEmpty(carrayCode.Trim()))
                query = query.Where(p => p.SkyWays.Where(t => t.CarrayCode == carrayCode.Trim()).Count() > 0);
            if (startCreateTime != null)
            {
                query = query.Where(p => p.CreateTime >= startCreateTime);
            }
            if (endDateTime != null)
            {
                query = query.Where(p => p.CreateTime <= endDateTime);
            }

            if (!string.IsNullOrEmpty(platformCode) && !string.IsNullOrEmpty(platformCode.Trim()))
                query = query.Where(p => p.Policy.PlatformCode == platformCode.Trim());
            //}
            if (!string.IsNullOrEmpty(businessmanCode) && !string.IsNullOrEmpty(businessmanCode.Trim()))
                query = query.Where(p => p.BusinessmanCode == businessmanCode.Trim());
            if (orderStatus != null && (orderStatus.Length == 1 || orderStatus.Length == 2))
            {
                query = query.Where(p => orderStatus.Contains((int)p.OrderStatus)).OrderByDescending(o => o.CreateTime);
            }
            else if (orderStatus != null && orderStatus.Length > 2)
            {
                query = query.Where(p => orderStatus.Contains((int)p.OrderStatus)).OrderBy(o => o.OrderPay.PayDateTime);
            }
            else
            {
                query = query.Where(p => p.OrderStatus != EnumOrderStatus.Invalid).OrderByDescending(o => o.CreateTime);
            }

            var datapack = new DataPack<OrderDto>();
            datapack.TotalCount = query.Count();
            var lst = query.Skip(startIndex).Take(count).ToList();
            datapack.List = lst.Select(p => p.ToOrderDto()).ToList();
            return datapack;
        }
        [ExtOperationInterceptor("订单查询【FindAll】")]
        public DataPack<OrderDto> FindAll(string orderId, string pnr, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime,
            int? orderStatus, int startIndex, int count, bool InterfaceOrder = true, bool ShrOrder = true)
        {
            #region 条件转换
            int[] status = GetIntArrStatus(orderStatus);
            #endregion
            var result = FindAll("", orderId, pnr, passengerName, null, null, null, null, null, currentUser.Code, startCreateTime, endCreateTime, null, null, status, null, null, startIndex, count, null, null, null);
            #region 结果转换
            result.List.ForEach(p =>
            {
                if (p.OrderStatus.HasValue)
                {
                    p.OrderStatus = p.OrderStatus.Value.ToClientStatus();
                }

            });
            #endregion
            return result;
        }
        [ExtOperationInterceptor("订单查询【FindCarrierAll】")]
        public DataPack<OrderDto> FindCarrierAll(string PaySerialNumber, string orderId, string pnr, string passengerName, string ticketNumber, string fromCity,
            string toCity, DateTime? startDateTime, DateTime? toDateTime, string businessmanCode, DateTime? startCreateTime, DateTime? endDateTime,
            string carrayCode, string platformCode, int[] orderStatus, int startIndex, int count)
        {
            var currentUser = AuthManager.GetCurrentUser();
            IQueryable<Order> query = null;
            if (currentUser.Type == "Carrier")
            {
                query = orderRepository.FindAllNoTracking(p => p.OrderStatus != EnumOrderStatus.WaitChoosePolicy && (p.CarrierCode == currentUser.Code || p.Policy.Code == currentUser.Code || p.Policy.CarrierCode == currentUser.Code));
            }
            else
                query = orderRepository.FindAllNoTracking(p => p.Policy.Code == currentUser.Code);
            if (!string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(orderId.Trim()))
                query = query.Where(p => p.OrderId == orderId.Trim());
            if (!string.IsNullOrEmpty(PaySerialNumber) && !string.IsNullOrEmpty(PaySerialNumber.Trim()))
                query = query.Where(p => p.OrderPay.PaySerialNumber == PaySerialNumber.Trim());
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.Passengers.Where(t => t.PassengerName.Contains(passengerName.Trim())).Count() > 0);
            if (!string.IsNullOrEmpty(ticketNumber) && !string.IsNullOrEmpty(ticketNumber.Trim()))
                query = query.Where(p => p.Passengers.Where(t => t.TicketNumber.Contains(ticketNumber.Trim())).Count() > 0);
            if (orderStatus != null)
                query = query.Where(p => orderStatus.Contains((int)p.OrderStatus));
            else
                query = query.Where(p => p.OrderStatus != EnumOrderStatus.Invalid);
            if (string.IsNullOrEmpty(PaySerialNumber) && string.IsNullOrEmpty(orderId) && string.IsNullOrEmpty(pnr) && string.IsNullOrEmpty(passengerName) && string.IsNullOrEmpty(ticketNumber))
            {
                if (!string.IsNullOrEmpty(fromCity) && !string.IsNullOrEmpty(fromCity.Trim()))
                    query = query.Where(p => p.SkyWays.Where(t => t.FromCityCode == fromCity).Count() > 0);
                if (!string.IsNullOrEmpty(toCity) && !string.IsNullOrEmpty(toCity.Trim()))
                    query = query.Where(p => p.SkyWays.Where(t => t.ToCityCode == toCity.Trim()).Count() > 0);
                if (startDateTime != null)
                    query = query.Where(p => p.SkyWays.Where(t => t.StartDateTime > startDateTime).Count() > 0);
                if (toDateTime != null)
                    query = query.Where(p => p.SkyWays.Where(t => t.ToDateTime < toDateTime).Count() > 0);
                if (!string.IsNullOrEmpty(carrayCode) && !string.IsNullOrEmpty(carrayCode.Trim()))
                    query = query.Where(p => p.SkyWays.Where(t => t.CarrayCode == carrayCode.Trim()).Count() > 0);
                if (startCreateTime != null)
                {
                    query = query.Where(p => p.CreateTime >= startCreateTime);
                }
                if (endDateTime != null)
                {
                    query = query.Where(p => p.CreateTime <= endDateTime);
                }

                if (!string.IsNullOrEmpty(platformCode) && !string.IsNullOrEmpty(platformCode.Trim()))
                    query = query.Where(p => p.Policy.PlatformCode == platformCode.Trim());
            }
            if (!string.IsNullOrEmpty(businessmanCode) && !string.IsNullOrEmpty(businessmanCode.Trim()))
                query = query.Where(p => p.BusinessmanCode == businessmanCode.Trim());
            var datapack = new DataPack<OrderDto>();
            datapack.TotalCount = query.Count();
            var lst = query.OrderByDescending(o => o.CreateTime).Skip(startIndex).Take(count).ToList();
            datapack.List = lst.Select(p => p.ToOrderDto()).ToList();
            return datapack;
        }
        /// <summary>
        /// 查询每月出票统计
        /// </summary>
        /// <param name="startCpTime"></param>
        /// <param name="endCpTime"></param>
        /// <param name="orderStatus"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询每月出票统计【QueryByCpTime】")]
        public DataPack<DataStatisticsDto> QueryByCpTime(DateTime CpTime)
        {
            var code = currentUser.Code;
            DateTime startTime = new DateTime(CpTime.Year, CpTime.Month, 1);
            DateTime endTime = startTime.AddMonths(1).AddSeconds(-1);

            var query = orderRepository.FindAll(p => p.BusinessmanCode == code);
            query = query.Where(p => p.OrderStatus == EnumOrderStatus.IssueAndCompleted);
            query = query.Where(p => p.IssueTicketTime != null && p.IssueTicketTime >= startTime && p.IssueTicketTime <= endTime);
            var result = query.Select(p => new
            {
                p.OperatorAccount,
                IssueTicketTime = p.IssueTicketTime.Value.Day,
                p.OrderPay.PayMoney,
                p.OrderCommissionTotalMoney,
                Count = p.Passengers.Count()
            }).ToList();

            List<DataStatisticsDto> dataStatisticsDtoList = result.GroupBy(p => p.OperatorAccount).Select((m, n) =>
            {
                //查询每天的统计
                var dataStatisticsList = m.GroupBy(tp => tp.IssueTicketTime).Select((tm, tn) =>
                {
                    return new DataStatisticsDto.DataStatistics
                    {
                        IssueTicketCount = tm.Sum(tmp => tmp.Count),
                        TradeTotalMoney = tm.Sum(tmp => tmp.PayMoney),
                        CommissionTotalMoney = tm.Sum(tmp => tmp.OrderCommissionTotalMoney),
                        Day = tm.Key
                    };
                }).ToList();
                //这个月的统计
                return new DataStatisticsDto
                {
                    DataStatisticsList = dataStatisticsList,
                    OperatorAccount = m.Key,
                    TotalCommission = dataStatisticsList.Sum(dsl => dsl.CommissionTotalMoney),
                    TotalIssueTicket = dataStatisticsList.Sum(dsl => dsl.IssueTicketCount),
                    TotalTradeMoney = dataStatisticsList.Sum(dsl => dsl.TradeTotalMoney)
                };
            }).ToList();
            DataPack<DataStatisticsDto> dataPack = new DataPack<DataStatisticsDto>();
            dataPack.List = dataStatisticsDtoList;
            dataPack.TotalCount = dataStatisticsDtoList.Count;
            return dataPack;
        }

        /// <summary>
        /// 统计最近15天的交易总量
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("统计最近15天的交易总量")]
        public Current15DayDataDto Query15DayStatistics()
        {
            var code = currentUser.Code;


            DateTime currentTime = System.DateTime.Now;
            DateTime ForwdTime = currentTime.AddDays(-15);
            DateTime startTime = new DateTime(ForwdTime.Year, ForwdTime.Month, ForwdTime.Day);
            DateTime endTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 23, 59, 59);

            var query = orderRepository.FindAll(p => p.BusinessmanCode == code);
            query = query.Where(p => p.OrderStatus == EnumOrderStatus.IssueAndCompleted);
            query = query.Where(p => p.IssueTicketTime != null && p.IssueTicketTime >= startTime && p.IssueTicketTime <= endTime);

            var result = query.Select(p => new
            {
                Day = p.IssueTicketTime.Value,
                p.OrderPay.PayMoney
            }).ToList();

            Current15DayDataDto dataStatisticsDtoList = new Current15DayDataDto()
            {
                DataStatisticsList = result.GroupBy(p => p.Day).Select((tm, tn) =>
                {
                    return new Current15DayDataDto.DataStatistics
                    {
                        TradeTotalMoney = tm.Sum(tmp => tmp.PayMoney),
                        Day = tm.Key
                    };
                }).ToList()
            };
            return dataStatisticsDtoList;
        }

        /// <summary>
        /// 查询机票信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pnr"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="orderStatus"></param>
        /// <param name="startCreateTime"></param>
        /// <param name="startCreateTime"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询机票信息")]
        public DataPack<TicketDataInfoDto> FindTicketInfo(string orderId, string pnr, string ticketNumber, string passengerName, int? orderStatus, DateTime? startCreateTime, DateTime? endCreateTime, int startIndex, int count)
        {
            var code = currentUser.Code;
            var query = orderRepository.FindAll(p => p.BusinessmanCode == code);
            if (!string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(orderId.Trim()))
                query = query.Where(p => p.OrderId == orderId.Trim());
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.Passengers.Where(t => t.PassengerName.Contains(passengerName.Trim())).Count() > 0);
            if (!string.IsNullOrEmpty(ticketNumber) && !string.IsNullOrEmpty(ticketNumber.Trim()))
                query = query.Where(p => p.Passengers.Where(t => t.TicketNumber.Contains(ticketNumber.Trim())).Count() > 0);
            if (orderStatus != null && orderStatus.HasValue)
            {
                int[] status = GetIntArrStatus(orderStatus);
                if (status != null)
                {
                    query = query.Where(p => status.Contains((int)p.OrderStatus));
                }
            }
            if (startCreateTime != null)
                query = query.Where(p => p.CreateTime >= startCreateTime);
            if (endCreateTime != null)
                query = query.Where(p => p.CreateTime <= endCreateTime);

            var result = query.OrderByDescending(o => o.CreateTime).ToList();



            List<TicketDataInfoDto> TicketDataInfoDtoList = new List<TicketDataInfoDto>();
            // result.Where(p=>p.AfterSaleTotalMoney!=0 && p.HasAfterSale==true).ForEach()
            result.ForEach(p =>
             {
                 string strFlightNumber = string.Empty;
                 string strFlyDateTime = string.Empty;
                 string strTravleNumber = string.Empty;
                 string strTravel = string.Empty;
                 string strSeat = string.Empty;
                 if (p.SkyWays != null)
                 {
                     p.SkyWays.ForEach(sp =>
                     {
                         strFlightNumber += sp.CarrayCode + sp.FlightNumber + "/";
                         PnrAnalysis.CityInfo fromCityInfo = pnrResource.GetCityInfo(sp.FromCityCode);
                         PnrAnalysis.CityInfo toCityCodeInfo = pnrResource.GetCityInfo(sp.ToCityCode);
                         strTravel += (fromCityInfo != null ? fromCityInfo.city.Name : "") + "-" + (toCityCodeInfo != null ? toCityCodeInfo.city.Name : "") + "/";
                         strSeat += sp.Seat + "/";
                         if (string.IsNullOrEmpty(strFlyDateTime))
                         {
                             strFlyDateTime += sp.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                         }
                     });
                 }
                 if (p.Passengers != null)
                 {
                     p.Passengers.ForEach(pp =>
                     {
                         TicketDataInfoDtoList.Add(new TicketDataInfoDto()
                         {
                             FlightNumber = strFlightNumber.Trim(new char[] { '/' }),
                             FlyDateTime = strFlyDateTime.Trim(new char[] { '/' }),
                             Travel = strTravel.Trim(new char[] { '/' }),
                             Seat = strSeat.Trim(new char[] { '/' }),
                             OrderId = p.OrderId,
                             OrderMoney = pp.PassengerType != BPiaoBao.Common.Enums.EnumPassengerType.Baby ?
                             databill.GetPayPrice(pp.SeatPrice, pp.ABFee, pp.RQFee, p.Policy.PolicyPoint, p.Policy.ReturnMoney) :
                             databill.GetPayPrice(pp.SeatPrice, pp.ABFee, pp.RQFee, 0, 0),
                             PayMethod = p.OrderPay != null ? p.OrderPay.PayMethod.ToEnumDesc() : "",
                             Pnr = p.PnrCode,
                             ReturnPoint = p.Policy != null ? p.Policy.PolicyPoint : 0m,
                             TicketStatus = GetTicketStatus((int)pp.TicketStatus),
                             PassengerName = pp.PassengerName,
                             TicketNumber = pp.TicketNumber,
                             TravelNumber = pp.TravelNumber,
                             SeatPrice = pp.SeatPrice,
                             TaxFee = pp.ABFee,
                             RQFee = pp.RQFee,
                             BigCode = p.BigCode,
                             Commission = pp.PassengerType != BPiaoBao.Common.Enums.EnumPassengerType.Baby ? databill.GetCommission((p.Policy != null ? p.Policy.PolicyPoint : 0m), pp.SeatPrice, p.Policy.ReturnMoney) : 0,
                             IssueTicketTime = p.IssueTicketTime,
                             PayDateTime = p.OrderPay != null ? p.OrderPay.PayDateTime : null

                         });
                     });
                 }
             });
            var datapack = new DataPack<TicketDataInfoDto>();
            var resultList = TicketDataInfoDtoList.AsQueryable().Where(p => !string.IsNullOrEmpty(p.Seat));
            datapack.List = resultList.Skip(startIndex).Take(count).ToList();
            datapack.TotalCount = resultList.Count();
            return datapack;
        }
        /// <summary>
        /// 根据订单号重新获取政策
        /// </summary>
        /// <param name="OrderId">订单号</param>
        /// <returns></returns>
        [ExtOperationInterceptor("根据订单号重新获取政策")]
        public PolicyPack GetPolicyByOrderId(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            if (order.Policy != null && order.Policy.PolicySpecialType != EnumPolicySpecialType.Normal)
                throw new OrderCommException("该特价政策的订单不能重新选择政策！");
            if (service.PnrIsPay(order))
            {
                throw new OrderCommException("该编码已经支付成功,不能再次导入生成订单");
            }
            PolicyParam policyParam = new PolicyParam();
            policyParam.IsLowPrice = order.IsLowPrice;
            policyParam.code = order.BusinessmanCode;
            policyParam.PnrContent = order.PnrContent;
            policyParam.OrderId = order.OrderId;
            policyParam.OrderType = order.OrderType;
            policyParam.OrderSource = order.OrderSource;
            policyParam.IsChangePnrTicket = order.IsChangePnrTicket;
            policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
            Passenger pasData = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
            if (pasData != null)
            {
                policyParam.defFare = pasData.SeatPrice.ToString();
                policyParam.defTAX = pasData.ABFee.ToString();
                policyParam.defRQFare = pasData.RQFee.ToString();
            }
            IList<Policy> policyLisy = NewGetPolicy(policyParam);
            return GetPolicy(policyLisy, order);
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="OrderId">取消的订单号</param>
        [ExtOperationInterceptor("取消订单")]
        public void CancelOrder(string orderId, bool IsCancelPnr = false)
        {
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("订单号:{0} IsCancelPnr={1}\r\n", orderId, IsCancelPnr);
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            var behavior = order.State.GetBehaviorByCode("CancelOrder");
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.Execute();
            //取消编码
            if (!string.IsNullOrEmpty(order.PnrCode)
                && order.PnrCode.Trim().Length == 6
                && order.PnrSource == EnumPnrSource.CreatePnr)
            {
                bool isSuc = false;
                string strResult = string.Empty;
                if (pidService.CanCancel(order.BusinessmanCode, order.YdOffice, order.PnrCode))
                {
                    isSuc = pidService.CancelPnr(order.BusinessmanCode, order.YdOffice, order.PnrCode);
                }
                strResult = string.Format("{0}取消编码{1}{2}", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), order.PnrCode, (isSuc ? "成功" : "失败"));
                order.WriteLog(new OrderLog()
                {
                    OperationContent = strResult,
                    OperationDatetime = System.DateTime.Now,
                    OperationPerson = currentUser.OperatorAccount,
                    IsShowLog = true
                });
                sbLog.AppendFormat("code={0}\r\n OperatorAccount={1}", currentUser.Code, currentUser.OperatorAccount);
                sbLog.AppendFormat("结果:{0}\r\n", strResult);
                new CommLog().WriteLog("CancelPnr", sbLog.ToString());
            }
            if (!string.IsNullOrEmpty(order.OldOrderId) && order.OrderType == 1)
            {
                var oldOrder = orderRepository.FindAll(p => p.OrderId.Equals(order.OldOrderId)).FirstOrDefault();
                if (oldOrder != null)
                {
                    oldOrder.AssocChdCount -= order.Passengers.Count();
                    unitOfWorkRepository.PersistUpdateOf(oldOrder);
                }
            }
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        #region 私有方法
        private static int[] GetIntArrStatus(int? orderStatus)
        {
            int[] status = null;
            if (orderStatus.HasValue)
            {
                switch (orderStatus.Value)
                {
                    case (int)EnumClientOrderStatus.WaitChoosePolicy:
                        status = new[] { (int)EnumOrderStatus.WaitChoosePolicy };
                        break;
                    case (int)EnumClientOrderStatus.NewOrder:
                        status = new[] { (int)EnumOrderStatus.NewOrder };
                        break;
                    case (int)EnumClientOrderStatus.WaitIssue:
                        status = new[] { (int)EnumOrderStatus.WaitAndPaid, (int)EnumOrderStatus.WaitIssue, (int)EnumOrderStatus.AutoIssueFail, (int)EnumOrderStatus.CreatePlatformFail, (int)EnumOrderStatus.PayWaitCreatePlatformOrder };
                        break;
                    case (int)EnumClientOrderStatus.OrderCanceled:
                        status = new[] { (int)EnumOrderStatus.OrderCanceled };
                        break;
                    case (int)EnumClientOrderStatus.IssueAndCompleted:
                        status = new[] { (int)EnumOrderStatus.IssueAndCompleted };
                        break;
                    case (int)EnumClientOrderStatus.WaitReimburseWithRepelIssue:
                        status = new[] { (int)EnumOrderStatus.WaitReimburseWithRepelIssue, (int)EnumOrderStatus.WaitReimburseWithPlatformRepelIssue };
                        break;
                    case (int)EnumClientOrderStatus.RepelIssueAndCompleted:
                        status = new[] { (int)EnumOrderStatus.RepelIssueAndCompleted };
                        break;
                    case (int)EnumClientOrderStatus.Invalid:
                        status = new[] { (int)EnumOrderStatus.Invalid };
                        break;
                    case (int)EnumClientOrderStatus.RepelIssueRefunding:
                        status = new[] { (int)EnumOrderStatus.RepelIssueRefunding };
                        break;
                    case (int)EnumClientOrderStatus.ApplyBabyFail:
                        status = new[] { (int)EnumOrderStatus.ApplyBabyFail };
                        break;
                    case (int)EnumClientOrderStatus.RepelApplyBaby:
                        status = new[] { (int)EnumOrderStatus.RepelApplyBaby };
                        break;
                    case (int)EnumClientOrderStatus.PaymentInWaiting:
                        status = new[] { (int)EnumOrderStatus.PaymentInWaiting };
                        break;
                }
            }
            return status;
        }
        /// <summary>
        /// 获取机票状态
        /// </summary>
        /// <param name="TicketStatus"></param>
        /// <returns></returns>
        private string GetTicketStatus(int TicketStatus)
        {
            string result = string.Empty;
            if (TicketStatus == 0)
            {
                result = "出票";
            }
            else if (TicketStatus == 1)
            {
                result = "退票";
            }
            else if (TicketStatus == 2)
            {
                result = "改签";
            }
            return result;
        }

        private IList<Policy> NewGetPolicy(PolicyParam policyParam)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var list = service.GetPolicyWithDownPoint(policyParam);
            sw.Stop();
            Logger.WriteLog(LogType.DEBUG, string.Format("重选政策用时:{0}", sw.ElapsedMilliseconds));
            return list;
        }

        private PolicyPack GetPolicy(IList<Policy> policyList, Order order)
        {
            //string pnrContent = order.PnrContent;           
            //包括默认政策
            //  IList<Policy> policyList = service.GetPolicyWithDownPoint(order.BusinessmanCode, order);
            Passenger passenger = null;
            PolicyPack policyPack = new PolicyPack();
            if (order.Passengers != null && order.Passengers.Count > 0)
            {
                if (order.OrderType == 0)
                {
                    passenger = order.Passengers.FirstOrDefault(pp => pp.PassengerType == BPiaoBao.Common.Enums.EnumPassengerType.Adult && pp.PassengerType != BPiaoBao.Common.Enums.EnumPassengerType.Baby);
                    policyPack.OrderId = order.OrderId;
                }
                else
                {
                    passenger = order.Passengers.FirstOrDefault(pp => pp.PassengerType == BPiaoBao.Common.Enums.EnumPassengerType.Child);
                    policyPack.ChdOrderId = order.OrderId;
                }
            }
            policyPack.PolicyList = new List<PolicyDto>();
            if (policyList != null && policyList.Count > 0 && passenger != null)
            {
                policyList.ForEach((p) =>
                {
                    PolicyDto policyDto = p.ToPolicyDto(order);
                    policyPack.PolicyList.Add(policyDto);
                });
            }
            else
            {
                throw new OrderCommException("未获取到政策信息！");
            }
            return policyPack;
        }

        #endregion

        public override void Dispose()
        {
            unitOfWork.Dispose();
        }
        [ExtOperationInterceptor("获取订单信息")]
        public OrderDetailDto GetClientOrderDetail(string orderId)
        {
            var currentUser = AuthManager.GetCurrentUser();
            var order = orderRepository.FindAll(q => q.OrderId == orderId && q.BusinessmanCode == currentUser.Code).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为" + orderId + "的订单");

            var p = order.ToOrderDetail(true);

            if (p.OrderStatus.HasValue)
            {
                p.OrderStatus = p.OrderStatus.Value.ToClientStatus();
            }
            return p;
        }
        [ExtOperationInterceptor("申请售后订单")]
        public void ApplySaleOrder(string orderId, RequestAfterSaleOrder afterSaleOrderDto)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException(string.Format("没有找到订单号【{0}】的订单", orderId));
            var nowTime = DateTime.Now.ToString("HH:mm");
            if (afterSaleOrderDto is RequestBounceOrder)
            {
                var bounceTime = order.Policy.ReturnTicketTime;
                if (string.Compare(nowTime, bounceTime.StartTime) == -1 || string.Compare(nowTime, bounceTime.EndTime) == 1)
                    throw new OrderCommException("申请时间不在退票时间之内，订单的退票时间为" + bounceTime.ToString());
            }
            else if ((afterSaleOrderDto is RequestAnnulOrder) || (afterSaleOrderDto is RequestChangeOrder))
            {
                var annulTime = order.Policy.AnnulTicketTime;
                if (string.Compare(nowTime, annulTime.StartTime) == -1 || string.Compare(nowTime, annulTime.EndTime) == 1)
                    throw new OrderCommException("申请时间不在废改时间之内，订单的废改时间为" + annulTime.ToString());
            }
            //申请信息
            AfterSaleOrder afterSaleOrder = afterSaleOrderDto.ToRequestAfterSaleOrder();
            if (afterSaleOrder == null)
                throw new CustomException(500, "AfterSaleOrder is null");
            afterSaleOrder.Order = order;
            afterSaleOrder.OrderID = orderId;
            //获取该订单的售后订单
            var afterSaleOrders = afterSaleOrderRepository.FindAll(p => p.OrderID == orderId).ToList();
            if (order.OrderType != 2)
                afterSaleOrder.CheckRule(afterSaleOrders);

            afterSaleOrder.CreateMan = AuthManager.GetCurrentUser().OperatorName;
            afterSaleOrder.CreateTime = DateTime.Now;
            var behavior = order.State.GetBehaviorByCode("AfterSaleOrder");
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("AfterSaleOrder", afterSaleOrder);
            behavior.Execute();
            unitOfWorkRepository.PersistCreationOf(afterSaleOrder);
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            DomainEvents.Raise(new RefundTicketEvent() { SaleOrderId = afterSaleOrder.Id });
            if (order.Policy.PolicySourceType != EnumPolicySourceType.Interface)
                WebMessageManager.GetInstance().Send(EnumMessageCommand.ApplyAfterSaleOrder, order.Policy.Code, string.Format("订单{0}申请【{1}】,请及时处理", orderId, afterSaleOrder.AfterSaleType));
        }

        [ExtOperationInterceptor("获取订单售后列表信息")]
        public IEnumerable<ResponseAfterSaleOrder> GetAfterSaleOrderById(string orderId)
        {
            var list = afterSaleOrderRepository.FindAll(p => p.OrderID == orderId).ToList();
            return AutoMapper.Mapper.Map<List<AfterSaleOrder>, List<ResponseAfterSaleOrder>>(list);
        }
        [ExtOperationInterceptor("获取售后详细")]
        public ResponseAfterSaleOrder GetAfterSaleOrderDetail(string orderId, int afterSaleOrderId)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == afterSaleOrderId).FirstOrDefault();
            return model.ToResponseAfterSaleOrder();
        }
        private Carrier GetCarrierCashBagCode(string code)
        {
            var buyer = this.businessmanRepository.FindAll(p => p.Code == code).OfType<Buyer>().FirstOrDefault();
            if (buyer == null)
                throw new CustomException(500, "获取支付信息失败!");
            var carrier = this.businessmanRepository.FindAll(p => p.Code == buyer.CarrierCode).OfType<Carrier>().FirstOrDefault();
            if (carrier == null)
                throw new CustomException(500, "获取支付信息失败.");
            return carrier;
        }
        [ExtOperationInterceptor("售后订单改签现金账户支付")]
        public void SaleOrderPayByCashbagAccount(int saleorderid, string payPassword)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid && p is ChangeOrder).Select(p => p as ChangeOrder).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该售后订单信息不存在");

            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            StringBuilder args = PayArgs(model);

            //string accountPayNo = client.PaymentByCashAccount(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, payPassword, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));

            //model.CashBagCode = AuthManager.GetCurrentUser().CashbagCode;
            //model.PayWay = EnumPayMethod.Account;
            //model.OutPayNo = accountPayNo;
            //model.PayTime = DateTime.Now;
            //model.PayStatus = EnumChangePayStatus.Payed;
            //model.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
            //unitOfWorkRepository.PersistUpdateOf(model);
            //unitOfWork.Commit();
            var result = client.PaymentByCashAccount(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, payPassword, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));
            //未被支付时
            if (!result.Item1)
            {
                model.CashBagCode = AuthManager.GetCurrentUser().CashbagCode;
                model.PayWay = EnumPayMethod.Account;
                //model.OutPayNo = accountPayNo;
                model.OutPayNo = result.Item2;
                model.PayTime = DateTime.Now;
                model.PayStatus = EnumChangePayStatus.Payed;
                model.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
            }
            else
            {
                //该订单已经被在线支付时写入日志
                Logger.WriteLog(LogType.INFO, "售后订单号" + model.Id + "已经支付，交易号为" + result.Item2 + "支付方式为" + result.Item3);
                //如果查询结果已经支付，则恢复订单支付方式
                switch (result.Item3.ToLower())
                {
                    case "tenpay":
                        model.PayWay = EnumPayMethod.TenPay;
                        break;
                    case "alipay":
                        model.PayWay = EnumPayMethod.AliPay;
                        break;
                }
            }
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }

        private StringBuilder PayArgs(ChangeOrder model)
        {

            //合作者的信息
            CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
            StringBuilder args = new StringBuilder();
            var dataBill = new DataBill();
            List<TicketDetail> list = new List<TicketDetail>();
            model.Passenger.ForEach(p =>
            {
                decimal retirementMoney = p.RetirementMoney;
                if (model.Order.Policy.PolicySourceType == EnumPolicySourceType.Local)
                {
                    if (model.Order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        list.Add(new TicketDetail
                        {
                            ID = 3,
                            Code = model.Order.Policy.CashbagCode,
                            Money = retirementMoney
                        });
                        list.Add(new TicketDetail
                        {
                            ID = 4,
                            Code = model.Order.Policy.CashbagCode,
                            Money = databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2) * -1
                        });
                    }
                    else
                    {
                        var carrier = GetCarrierCashBagCode(model.Order.BusinessmanCode);
                        //供应实收
                        list.Add(new TicketDetail
                        {
                            ID = 1,
                            Code = model.Order.Policy.CashbagCode,
                            Money = retirementMoney
                        });
                        list.Add(new TicketDetail
                        {
                            ID = 2,
                            Code = model.Order.Policy.CashbagCode,
                            Money = dataBill.NewRound(retirementMoney * model.Order.Policy.Rate, 2) * -1
                        });
                        //运营实收
                        list.Add(new TicketDetail
                        {
                            ID = 3,
                            Code = carrier.CashbagCode,
                            Money = databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2)
                        });
                        list.Add(new TicketDetail
                        {
                            ID = 4,
                            Code = carrier.CashbagCode,
                            Money = dataBill.NewRound(retirementMoney * carrier.Rate, 2) * -1
                        });
                    }
                }
                else if (model.Order.Policy.PolicySourceType == EnumPolicySourceType.Share)
                {

                    if (model.Order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        list.Add(new TicketDetail
                        {
                            ID = 3,
                            Code = model.Order.Policy.CashbagCode,
                            Money = retirementMoney
                        });
                        list.Add(new TicketDetail
                        {
                            ID = 4,
                            Code = model.Order.Policy.CashbagCode,
                            Money = databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2) * -1
                        });
                    }
                    else
                    {
                        var supper = this.businessmanRepository.FindAll(x => x.Code == model.Order.Policy.Code).OfType<Supplier>().FirstOrDefault();
                        if (supper == null)
                            throw new CustomException(400, "未获取信息");
                        var carrier = this.businessmanRepository.FindAllNoTracking(x => x.Code == supper.CarrierCode).OfType<Carrier>().FirstOrDefault();
                        if (carrier == null)
                            throw new CustomException(400, "未获取信息");
                        //供应实收
                        list.Add(new TicketDetail
                        {
                            ID = 1,
                            Code = model.Order.Policy.CashbagCode,
                            Money = retirementMoney
                        });
                        list.Add(new TicketDetail
                        {
                            ID = 2,
                            Code = model.Order.Policy.CashbagCode,
                            Money = databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2) * -1
                        });
                        //运营实收
                        list.Add(new TicketDetail
                        {
                            ID = 3,
                            Code = carrier.CashbagCode,
                            Money = dataBill.NewRound(retirementMoney * model.Order.Policy.Rate, 2)
                        });
                        list.Add(new TicketDetail
                        {
                            ID = 4,
                            Code = carrier.CashbagCode,
                            Money = dataBill.NewRound(retirementMoney * carrier.Rate, 2) * -1
                        });
                    }
                }

            });
            decimal parentMoney = model.Money - list.Sum(x => x.Money);
            if (parentMoney > 0)
            {
                list.Add(new TicketDetail
                {
                    ID = 5,
                    Code = setting.CashbagCode,
                    Money = parentMoney
                });
            }
            list.GroupBy(x => new { x.ID, x.Code }).Select(x => new TicketDetail { ID = x.Key.ID, Code = x.Key.Code, Money = x.Sum(y => y.Money) }).ForEach(m =>
            {
                args.AppendFormat("{0}^{1}^{2}|", m.Code, m.Money, (m.ID == 1 ? "运营收款" : (m.ID == 2 ? "运营收款手续费" : (m.ID == 3 ? "供应收款" : (m.ID == 4 ? "供应收款手续费" : "服务费")))));
            });
            return args.Remove(args.Length - 1, 1);
        }

        [ExtOperationInterceptor("售后订单改签信用支付")]
        public void SaleOrderPayByCreditAccount(int saleorderid, string payPassword)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid && p is ChangeOrder).Select(p => p as ChangeOrder).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该售后订单信息不存在");

            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            StringBuilder args = PayArgs(model);
            //string accountPayNo = client.PaymentByCreditAccount(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, payPassword, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));
            var result = client.PaymentByCreditAccount(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, payPassword, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));
            //未被支付时
            if (!result.Item1)
            {
                model.CashBagCode = AuthManager.GetCurrentUser().CashbagCode;
                model.PayWay = EnumPayMethod.Credit;
                //model.OutPayNo = accountPayNo;
                model.OutPayNo = result.Item2;
                model.PayTime = DateTime.Now;
                model.PayStatus = EnumChangePayStatus.Payed;
                model.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
            }
            else
            {
                //该订单已经被在线支付时写入日志
                Logger.WriteLog(LogType.INFO, "售后订单号" + model.Id + "已经支付，交易号为" + result.Item2 + "支付方式为" + result.Item3);
                //如果查询结果已经支付，则恢复订单支付方式
                switch (result.Item3.ToLower())
                {
                    case "tenpay":
                        model.PayWay = EnumPayMethod.TenPay;
                        break;
                    case "alipay":
                        model.PayWay = EnumPayMethod.AliPay;
                        break;
                }
            }
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();

        }
        [ExtOperationInterceptor("售后订单改签银行卡支付")]
        public string SaleOrderPayByBank(int saleorderid, string bankCode)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid && p is ChangeOrder).Select(p => p as ChangeOrder).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该售后订单信息不存在");
            model.PayWay = EnumPayMethod.Bank;
            model.PayStatus = EnumChangePayStatus.NoPay;
            model.CashBagCode = AuthManager.GetCurrentUser().CashbagCode;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            var saleNotify = SettingSection.GetInstances().Payment.SaleNotify;
            StringBuilder args = PayArgs(model);

            return client.PaymentByBank(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, bankCode, saleNotify, AuthManager.GetCurrentUser().OperatorName, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));
        }
        [ExtOperationInterceptor("售后订单改签平台支付")]
        public string SaleOrderPayByPlatform(int saleorderid, string platform)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid && p is ChangeOrder).Select(p => p as ChangeOrder).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该售后订单信息不存在");
            //model.PayWay = EnumPayMethod.Platform; 取消平台支付方式根据平台名称具体赋值payway
            model.PayWay = platform == "Alipay" ? EnumPayMethod.AliPay : EnumPayMethod.TenPay;
            model.PayStatus = EnumChangePayStatus.NoPay;
            model.CashBagCode = AuthManager.GetCurrentUser().CashbagCode;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            var saleNotify = SettingSection.GetInstances().Payment.SaleNotify;
            StringBuilder args = PayArgs(model);
            return client.PaymentByPlatform(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, platform, saleNotify, AuthManager.GetCurrentUser().OperatorName, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));

        }

        [ExtOperationInterceptor("售后订单支付宝快捷支付")]
        public string SaleOrderPayByQuikAliPay(int saleorderid, string pwd)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid && p is ChangeOrder).Select(p => p as ChangeOrder).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该售后订单信息不存在");
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            var saleNotify = SettingSection.GetInstances().Payment.SaleNotify;
            StringBuilder args = PayArgs(model);
            var result = client.PaymentByQuikAliPay(AuthManager.GetCurrentUser().CashbagCode, AuthManager.GetCurrentUser().CashbagKey, string.Format("A_{0}", model.Id), "改签支付", model.Money, "支付宝快捷支付", saleNotify, AuthManager.GetCurrentUser().OperatorName, pwd, args.ToString(), string.Format("PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray())));
            if (result == "True") return result;
            model.CashBagCode = AuthManager.GetCurrentUser().CashbagCode;
            model.PayWay = EnumPayMethod.AliPay;
            model.OutPayNo = result;
            model.PayTime = DateTime.Now;
            model.PayStatus = EnumChangePayStatus.Payed;
            model.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            return result;
        }


        [ExtOperationInterceptor("查询订单")]
        public DataPack<ResponseOrder> GetOrderBySearch(string orderId, string pnr, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, int? orderStatus, int startIndex, int count, string OutTradeNo, DateTime? startFlightTime = null, DateTime? endFlightTime = null)
        {
            int[] status = GetIntArrStatus(orderStatus);
            return this.GetOrderBySearch(orderId, pnr, passengerName, null, null, null, startFlightTime, endFlightTime, AuthManager.GetCurrentUser().Code, startCreateTime, endCreateTime, null, null, status, startIndex, count, OutTradeNo);
        }

        [ExtOperationInterceptor("机票总表")]
        public DataPack<ReponseTicketSum> FindTicketSum(string orderId, string currentOrderId, string pnr, string ticketNumber, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, int startIndex, int count, string transactionNumber = null, EnumTicketStatus? ticketStatus = null)
        {
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Buyer>().Where(p => p.Code == currentUser.Code);
            if (!string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(orderId.Trim()))
                query = query.Where(p => p.OrderID == orderId.Trim());
            if (!string.IsNullOrEmpty(currentOrderId) && !string.IsNullOrEmpty(currentOrderId.Trim()))
                query = query.Where(p => p.CurrentOrderID == currentOrderId.Trim());
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.PNR == pnr.Trim());
            if (!string.IsNullOrEmpty(ticketNumber) && !string.IsNullOrEmpty(ticketNumber.Trim()))
                query = query.Where(p => p.TicketNum == ticketNumber.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.PassengerName == passengerName.Trim());
            if (startCreateTime.HasValue)
                query = query.Where(p => p.CreateDate >= startCreateTime.Value);
            if (endCreateTime.HasValue)
                query = query.Where(p => p.CreateDate <= endCreateTime.Value);
            if (!string.IsNullOrEmpty(transactionNumber) && !string.IsNullOrEmpty(transactionNumber.Trim()))
                query = query.Where(p => p.PayNumber == transactionNumber.Trim());
            if (ticketStatus.HasValue)
            {
                var state = ticketStatus.Value.ToEnumDesc();
                query = query.Where(p => p.TicketState == state);
            }
            var list = query.OrderByDescending(p => p.CreateDate).Skip(startIndex).Take(count).ToList();
            var dataPack = new DataPack<ReponseTicketSum>
            {
                TotalCount = query.Count(),
                List = AutoMapper.Mapper.Map<List<Ticket_Buyer>, List<ReponseTicketSum>>(list)
            };
            return dataPack;
        }

        [ExtOperationInterceptor("获取退废改订单")]
        public DataPack<ResponseAfterSaleOrder> GetSaleOrderBySearch(int currentPageIndex, int pageSize, string pnr, string orderid, EnumAfterSaleOrder? saleOrderType, EnumTfgProcessStatus? status, DateTime? startTime, DateTime? endTime, string payno, DateTime? startFlightTime = null, DateTime? endFlightTime = null, int id = 0)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var query = afterSaleOrderRepository.FindAll(p => p.Order.BusinessmanCode == code);
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.Order.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(orderid.Trim()))
                query = query.Where(p => p.OrderID == orderid.Trim());
            if (startTime.HasValue)
                query = query.Where(p => p.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateTime <= endTime.Value);
            if (startFlightTime.HasValue)
                query = query.Where(p => p.Order.SkyWays.Min(s => s.StartDateTime) >= startFlightTime.Value);
            if (endFlightTime.HasValue)
                query = query.Where(p => p.Order.SkyWays.Max(s => s.StartDateTime) <= endFlightTime.Value);
            if (saleOrderType.HasValue)
                switch (saleOrderType.Value)
                {
                    case EnumAfterSaleOrder.Annul:
                        query = query.Where(p => p is AnnulOrder);
                        break;
                    case EnumAfterSaleOrder.Bounce:
                        query = query.Where(p => p is BounceOrder);
                        break;
                    case EnumAfterSaleOrder.Change:
                        query = query.Where(p => p is ChangeOrder);
                        break;
                    case EnumAfterSaleOrder.Modify:
                        query = query.Where(p => p is ModifyOrder);
                        break;
                    default:
                        break;
                }
            if (status.HasValue)
                query = query.Where(p => p.ProcessStatus == status.Value);
            if (!string.IsNullOrWhiteSpace(payno))
                query = query.Where(p => p.Order.OrderPay.PaySerialNumber == payno);
            if (id > 0)
                query = query.Where(p => p.Id == id);
            query = query.OrderByDescending(p => p.CreateTime);
            var afterSaleOrderList = query.Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();
            var dataPack = new DataPack<ResponseAfterSaleOrder>
            {
                TotalCount = query.Count(),
                List = AutoMapper.Mapper.Map<List<AfterSaleOrder>, List<ResponseAfterSaleOrder>>(afterSaleOrderList)
            };
            return dataPack;
        }

        [ExtOperationInterceptor("验证订单中的编码是否存在已经支付成功")]
        public bool PnrIsPay(string orderId)
        {
            bool IsPay = false;
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (service.PnrIsPay(order))
            {
                IsPay = true;
                throw new OrderCommException("该订单号(" + orderId + ")中的编码(" + order.PnrCode + ")已经支付成功,不能再次进行支付！");
            }
            return IsPay;
        }

        [ExtOperationInterceptor("新的选择政策")]
        public OrderDto ChoosePolicy(PolicyDto policy, string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (order.OrderStatus == EnumOrderStatus.PaymentInWaiting)
                throw new OrderCommException("该订单【" + orderId + "】正在支付中,请稍后。。。");
            if (service.PnrIsPay(order) || (order.OrderPay != null && order.OrderPay.PayStatus == EnumPayStatus.OK))
                throw new OrderCommException("该编码已经支付成功,不能再次导入生成订单");


            var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
            behavior.SetParame("platformCode", "");
            behavior.SetParame("policyId", "");
            behavior.SetParame("policy", policy);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("source", "forward");
            behavior.Execute();
            OrderDto orderDto = order.ToOrderDto();
            //自动授权
            if ((pidService.GetOffice(currentUser.CarrierCode).Contains(order.YdOffice.ToUpper())
                || order.OrderSource == EnumOrderSource.PnrContentImport)
                && (!string.IsNullOrEmpty(order.CpOffice))
                && order.CpOffice != order.YdOffice
                )
            {
                orderDto.IsAuthSuc = pidService.AuthToOffice(order.BusinessmanCode, order.CpOffice, order.YdOffice, order.PnrCode);
                if (order.OrderSource == EnumOrderSource.PnrContentImport
                    && !string.IsNullOrEmpty(policy.CPOffice)
                    && policy.CPOffice != "null"
                    )
                {
                    orderDto.AuthInfo = string.Format("请授权,授权指令:RMK TJ AUTH {0}", policy.CPOffice);
                    order.WriteLog(new OrderLog()
                    {
                        OperationContent = orderDto.AuthInfo,
                        OperationDatetime = DateTime.Now,
                        OperationPerson = currentUser.OperatorName,
                        IsShowLog = true
                    });
                }
            }
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            return orderDto;
        }

        [ExtOperationInterceptor("线下婴儿申请")]
        public void ApplyBaby(ApplyBabyDataObject applyBaby)
        {
            var order = this.orderRepository.FindAllNoTracking(p => p.OrderType == 0 && p.OrderId.Equals(applyBaby.RelationOrderId)).FirstOrDefault();
            if (order == null)
                throw new CustomException(400, "只能关联成人订单");
            //可以申请婴儿个数
            int count = order.Passengers.Where(p => p.PassengerType == EnumPassengerType.Adult).Count() - order.Passengers.Where(p => p.PassengerType == EnumPassengerType.Baby).Count();
            if (applyBaby.BabyList.Count > count)
                throw new CustomException(500, string.Format("最多可以申请{0}个婴儿", count));

            Order newOrder = new Order();
            newOrder.OrderId = GetOrderId(string.Empty);

            #region 添加婴儿和获取婴儿价格
            newOrder.InfNotGetPrice = true;
            newOrder.OrderStatus = EnumOrderStatus.ApplyBabyFail;
            int sendCount = 0;
            //单人支付
            decimal onePayMoney = 0m;
            decimal seatPrice = 0m;
            string errMsg = string.Empty;
            string Pnr = order.PnrCode;
            string strCmd = string.Format("RT{0}", Pnr);
            try
            {
                //自动授权
                if (pidService.GetOffice(currentUser.CarrierCode).Contains(order.YdOffice.ToUpper())
                    && (!string.IsNullOrEmpty(order.CpOffice))
                    && order.CpOffice != order.YdOffice
                    )
                {

                    string recvData = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                    PnrModel pnrModel = format.GetPNRInfo(Pnr, recvData, false, out errMsg);
                    if (pnrModel != null && applyBaby.BabyList.Count > 0)
                    {
                        List<string> pNoList = pnrModel._PassengerList.Select(p => p.SerialNum).ToList();
                        string AdultNum = "1";
                        applyBaby.BabyList.ForEach(p =>
                        {
                            SkyWay sky = order.SkyWays[0];
                            LegInfo leg = pnrModel._LegList[0];
                            if ((pNoList.Count - 1) >= 0)
                            {
                                AdultNum = pNoList[pNoList.Count - 1];
                                pNoList.Remove(AdultNum);
                            }
                            bool isSuccess = this.pidService.AddINF(currentUser.Code, Pnr, p.BabyName, p.BornDate.ToString("yyyy-MM-dd"), sky.CarrayCode, sky.FromCityCode + sky.ToCityCode, sky.FlightNumber, sky.Seat, sky.StartDateTime.ToString("yyyy-MM-dd"), AdultNum, leg.SerialNum);
                            if (!isSuccess)
                            {
                                sendCount += 1;
                                newOrder.WriteLog(new OrderLog
                                {
                                    IsShowLog = true,
                                    OperationContent = p.BabyName + "注入编码失败",
                                    OperationDatetime = DateTime.Now,
                                    OperationPerson = currentUser.OperatorAccount
                                });
                            }
                        });
                        strCmd = string.Format("RT{0}", Pnr);
                        recvData = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                        pnrModel = format.GetPNRInfo(Pnr, recvData, false, out errMsg);
                        if (pnrModel.HasINF)
                        {
                            strCmd = string.Format("RT{0}|PAT:A*IN", Pnr);
                            recvData = this.pidService.SendCmd(currentUser.Code, strCmd, "", "").Replace("^", "\r");
                            PatModel patModel = format.GetPATInfo(recvData, out errMsg);
                            if (patModel.UninuePatList.Count > 0)
                            {
                                decimal.TryParse(patModel.UninuePatList[0].Fare, out seatPrice);
                                if (sendCount == 0)
                                {
                                    //取到价格
                                    newOrder.InfNotGetPrice = false;
                                    newOrder.OrderStatus = EnumOrderStatus.NewOrder;
                                    newOrder.WriteLog(new OrderLog
                                    {
                                        IsShowLog = true,
                                        OperationContent = string.Format("婴儿舱位价:{0}", seatPrice),
                                        OperationDatetime = DateTime.Now,
                                        OperationPerson = currentUser.OperatorAccount
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.DEBUG, "线下婴儿申请P价格失败", e);
            }
            #endregion

            newOrder.Passengers = new List<Passenger>();
            applyBaby.BabyList.ForEach(p =>
            {
                onePayMoney = seatPrice;
                newOrder.Passengers.Add(new Passenger
                {
                    Birth = p.BornDate,
                    CardNo = p.BornDate.ToString("yyyy-MM-dd"),
                    CPMoney = onePayMoney,
                    IdType = EnumIDType.BirthDate,
                    PassengerName = p.BabyName,
                    PassengerType = EnumPassengerType.Baby,
                    PayMoney = onePayMoney,
                    SeatPrice = seatPrice,
                    SexType = p.SexType
                });
            });
            //订单金额
            decimal orderMoney = newOrder.Passengers.Sum(p => p.PayMoney);
            //婴儿人数
            int infCount = newOrder.Passengers.Count;
            newOrder.OrderPay = new OrderPay
            {
                OrderId = newOrder.OrderId,
                PayMoney = orderMoney,
                PayBillDetails = new List<PayBillDetail>()
            };
            newOrder.Policy = new Policy()
            {
                AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                {
                    StartTime = order.Policy.AnnulTicketTime.StartTime,
                    EndTime = order.Policy.AnnulTicketTime.EndTime
                },
                PolicyOwnUserRole = order.Policy.PolicyOwnUserRole,
                Rate = order.Policy.Rate,
                CarrierCode = order.Policy.CarrierCode,
                Code = order.Policy.Code,
                CashbagCode = order.Policy.CashbagCode,
                EnumIssueTicketWay = order.Policy.EnumIssueTicketWay,
                PlatformCode = order.Policy.PlatformCode,
                Name = order.Policy.Name,
                PolicySourceType = order.Policy.PolicySourceType,
                PolicyType = order.Policy.PolicyType,
                ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                {
                    StartTime = order.Policy.ReturnTicketTime.StartTime,
                    EndTime = order.Policy.ReturnTicketTime.EndTime
                },
                WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                {
                    StartTime = order.Policy.WorkTime.StartTime,
                    EndTime = order.Policy.WorkTime.EndTime
                }
            };
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                var carrier = this.businessmanRepository.FindAllNoTracking(p => p.Code.Equals(order.CarrierCode)).OfType<Carrier>().FirstOrDefault();
                bool isIssueTicket = carrier.DefaultPolicys.Where(p => p.CarrayCode.Equals("ALL") || p.CarrayCode.Equals(order.SkyWays.Select(x => x.CarrayCode).FirstOrDefault())).Count() > 0;
                if (isIssueTicket)
                {
                    newOrder.Policy.Code = order.CarrierCode;
                    newOrder.Policy.CarrierCode = order.CarrierCode;
                    newOrder.Policy.PolicySourceType = EnumPolicySourceType.Local;
                }
                else
                {
                    newOrder.Policy.Code = "ctuadmin";
                    newOrder.Policy.CarrierCode = "ctuadmin";
                    newOrder.Policy.PolicySourceType = EnumPolicySourceType.Share;
                }
            }
            Businessman businessman = this.businessmanRepository.FindAll(p => p.Code.Trim() == newOrder.Policy.Code).FirstOrDefault();
            if (businessman is Supplier)
            {
                if (!(businessman as Supplier).CheckNormalWork())
                    throw new CustomException(500, "供应商已下班,不能继续申请操作");
            }
            if (businessman is Carrier)
            {
                if (!(businessman as Carrier).CheckNormalWork())
                    throw new CustomException(500, "运营商已下班,不能继续申请操作");
            }
            if (newOrder.OrderStatus == EnumOrderStatus.NewOrder)
            {
                CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;

                var buyer = this.businessmanRepository.FindAllNoTracking(p => p.Code.Equals(order.BusinessmanCode)).Select(p => new
                {
                    BusinessmanCode = p.Code,
                    BusinessmanName = p.Name,
                    CashbagCode = p.CashbagCode
                }).FirstOrDefault();
                //采购付款
                newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                {
                    Code = buyer.BusinessmanCode,
                    Name = buyer.BusinessmanName,
                    CashbagCode = buyer.CashbagCode,
                    Money = orderMoney,
                    OpType = EnumOperationType.PayMoney,
                    Remark = "付款"
                });

                //运营商婴儿服务费
                //decimal carrInfServerFee = Math.Abs(databill.NewRound(seatPrice * ((businessman is Supplier) ? (businessman as Supplier).SupRate : (businessman as Carrier).Rate), 2) * infCount);

                //出票方是运营
                if ((businessman is Carrier) && (businessman as Carrier) != null)
                {
                    #region 运营
                    Carrier carrier = businessman as Carrier;
                    //运营商婴儿服务费
                    decimal carrInfServerFee = Math.Abs(databill.NewRound(seatPrice * carrier.Rate, 2) * infCount);
                    //运营商收款
                    newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = businessman.Code,
                        Name = businessman.Name,
                        CashbagCode = businessman.CashbagCode,
                        Money = orderMoney,
                        OpType = EnumOperationType.Receivables,
                        Remark = "收款"
                    });
                    //运营商付款 支付票价的服务费付给合作方
                    newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = businessman.Code,
                        Name = businessman.Name,
                        CashbagCode = businessman.CashbagCode,
                        Money = -carrInfServerFee,
                        InfMoney = -carrInfServerFee,
                        OpType = EnumOperationType.IssuePayServer,
                        Remark = "服务费"
                    });
                    //合作方收款 收取运营票款服务费                          
                    newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = carrInfServerFee,
                        InfMoney = carrInfServerFee,
                        OpType = EnumOperationType.ParterServer,
                        Remark = "服务费"
                    });
                    #endregion
                }
                //出票方是供应
                else if ((businessman is Supplier) && (businessman as Supplier) != null)
                {
                    #region 供应
                    Supplier supplier = businessman as Supplier;
                    //运营收取供应婴儿服务费
                    decimal gyInfFee = Math.Abs(databill.NewRound(seatPrice * supplier.SupRate, 2)) * infCount;
                    //供应商收款
                    newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = businessman.Code,
                        Name = businessman.Name,
                        CashbagCode = businessman.CashbagCode,
                        Money = orderMoney,
                        OpType = EnumOperationType.Receivables,
                        Remark = "收款"
                    });
                    //供应商付款 支付运营商的服务费
                    newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = businessman.Code,
                        Name = businessman.Name,
                        CashbagCode = businessman.CashbagCode,
                        Money = -gyInfFee,
                        InfMoney = -gyInfFee,
                        OpType = EnumOperationType.IssuePayServer,
                        Remark = "服务费"
                    });
                    //该供应商的上级运营商
                    Carrier carrier = this.businessmanRepository.FindAll(p => p.Code.Trim() == order.Policy.CarrierCode).FirstOrDefault() as Carrier;
                    if (carrier != null)
                    {
                        //运营商收款 收取供应商的服务费
                        newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                        {
                            Code = carrier.Code,
                            Name = carrier.Name,
                            CashbagCode = carrier.CashbagCode,
                            Money = gyInfFee,
                            InfMoney = gyInfFee,
                            OpType = EnumOperationType.CarrierRecvServer,
                            Remark = "服务费"
                        });
                        //运营商婴儿服务费
                        decimal yyInfFee = Math.Abs(databill.NewRound(seatPrice * carrier.Rate, 2)) * infCount;
                        //运营商付款 运营商票款服务费                          
                        newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                        {
                            Code = carrier.Code,
                            Name = carrier.Name,
                            CashbagCode = carrier.CashbagCode,
                            Money = -yyInfFee,
                            InfMoney = -yyInfFee,
                            OpType = EnumOperationType.CarrierPayServer,
                            Remark = "服务费"
                        });
                        //合作方收款 运营商票款服务费                          
                        newOrder.OrderPay.PayBillDetails.Add(new PayBillDetail()
                        {
                            Code = setting.CashbagCode,
                            Name = "系统",
                            CashbagCode = setting.CashbagCode,
                            Money = yyInfFee,
                            InfMoney = yyInfFee,
                            OpType = EnumOperationType.ParterServer,
                            Remark = "服务费"
                        });
                    }
                    #endregion
                }

            }


            if (newOrder.SkyWays == null)
                newOrder.SkyWays = new List<SkyWay>();
            order.SkyWays.ForEach(p => newOrder.SkyWays.Add(p));

            if (!string.IsNullOrWhiteSpace(applyBaby.Remark))
            {
                newOrder.OrderLogs = new List<OrderLog>()
                {
                    new OrderLog{
                    IsShowLog=true,
                    OperationContent=applyBaby.Remark,
                    OperationDatetime=DateTime.Now,
                    OperationPerson=AuthManager.GetCurrentUser().OperatorAccount
                    }
                };
            }

            newOrder.PnrCode = Pnr;
            newOrder.BigCode = order.BigCode;
            newOrder.PnrType = order.PnrType;
            newOrder.TicketPrice = orderMoney;
            newOrder.INFTicketPrice = orderMoney;
            newOrder.BusinessmanCode = order.BusinessmanCode;
            newOrder.BusinessmanName = order.BusinessmanName;
            newOrder.CarrierCode = order.CarrierCode;
            newOrder.OperatorAccount = currentUser.OperatorAccount;
            newOrder.CreateTime = DateTime.Now;
            newOrder.YdOffice = order.YdOffice;
            newOrder.CpOffice = order.CpOffice;
            newOrder.OrderPay.PayStatus = EnumPayStatus.NoPay;
            newOrder.OrderMoney = orderMoney;
            newOrder.CPMoney = orderMoney;
            newOrder.HaveBabyFlag = true;
            newOrder.OrderSource = EnumOrderSource.LineOrder;
            newOrder.OrderType = 2;
            newOrder.RefundedTradeMoney = orderMoney;
            newOrder.RefundedServiceMoney = Math.Round(orderMoney * SystemConsoSwitch.Rate, 2);
            newOrder.OldOrderId = applyBaby.RelationOrderId;
            newOrder.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "申请线下婴儿订单",
                OperationDatetime = DateTime.Now,
                OperationPerson = currentUser.OperatorAccount
            });
            unitOfWorkRepository.PersistCreationOf(newOrder);
            unitOfWork.Commit();

        }

        private string GetOrderId(string prxFix)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long l = BitConverter.ToInt64(buffer, 0);
            return prxFix + l.ToString();
        }




    }
}
