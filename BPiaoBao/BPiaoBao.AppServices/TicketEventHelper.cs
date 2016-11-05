using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
using BPiaoBao.DomesticTicket.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;
using JoveZhao.Framework.Expand;
using PnrAnalysis;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.Common;
using StructureMap;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.AppServices
{
    public class TicketEventHelper
    {
        private readonly PnrResource pnrResource = new PnrResource();
        private readonly DataBill databill = new DataBill();
        private readonly IBusinessmanRepository businessmanRepository = ObjectFactory.GetInstance<IBusinessmanRepository>();
        private readonly IOrderRepository orderRepository = ObjectFactory.GetInstance<IOrderRepository>();
        private readonly IAfterSaleOrderRepository afterSaleOrderRepository = ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
        private readonly IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        private readonly IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        private readonly CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
        public void FindIssueTicket(string orderid)
        {
            List<Ticket> list = new List<Ticket>();
            try
            {
                var order = this.orderRepository.FindAllNoTracking(p => p.OrderId.Equals(orderid)).FirstOrDefault();

                string strFlightNumber = string.Empty;
                string strTravel = string.Empty;
                string strSeat = string.Empty;
                string flyDateTime = string.Empty;
                string carray = string.Empty;
                order.SkyWays.ToList().ForEach(sp =>
                {
                    strFlightNumber += sp.CarrayCode + sp.FlightNumber + "/";
                    PnrAnalysis.CityInfo fromCityInfo = pnrResource.GetCityInfo(sp.FromCityCode);
                    PnrAnalysis.CityInfo toCityCodeInfo = pnrResource.GetCityInfo(sp.ToCityCode);
                    strTravel += (fromCityInfo != null ? fromCityInfo.city.Name : "") + "-" + (toCityCodeInfo != null ? toCityCodeInfo.city.Name : "") + "/";
                    strSeat += sp.Seat + "/";
                    carray += sp.CarrayCode + "/";
                    flyDateTime += sp.StartDateTime.ToString("yyyy-MM-dd HH:mm") + "/";
                });

                var passengerList = order.Passengers.ToList();
                var pList = order.OrderPay.PayBillDetails.Where(p => p.OpType != EnumOperationType.Insurance && p.OpType != EnumOperationType.InsuranceServer).GroupBy(x => x.Code).Select(p => new { Code = p.Key, Money = p.Sum(x => x.Money), BabyMoney = p.Sum(x => x.InfMoney), Point = p.Sum(x => x.Point) }).ToList();
                var consoList = order.OrderPay.PayBillDetails.Where(p => p.OpType == EnumOperationType.ParterProfitServer || p.OpType == EnumOperationType.ParterServer).GroupBy(x => x.Code).Select(p => new { Money = p.Sum(x => x.Money), BabyMoney = p.Sum(x => x.InfMoney) }).FirstOrDefault();
                var blModel = order.OrderPay.PayBillDetails.Where(p => p.OpType == EnumOperationType.ParterRetainServer).FirstOrDefault();
                int adultCount = passengerList.Count(p => p.PassengerType != EnumPassengerType.Baby);
                int babyCount = passengerList.Count(p => p.PassengerType == EnumPassengerType.Baby);
                decimal babyMoney = 0;
                if (babyCount > 0)
                    babyMoney = passengerList.Where(p => p.PassengerType == EnumPassengerType.Baby).FirstOrDefault().PayMoney;
                decimal recordPaidMoney = 0;
                decimal recordPayFeeMoney = 0;
                int currentPassengerIndex = 1;
                decimal consoTransactionMoney = 0;
                decimal tempTPaidMoney = order.Policy.PolicySourceType == EnumPolicySourceType.Interface ? (order.OrderPay.PayBillDetails.Where(x => x.OpType == EnumOperationType.Receivables).FirstOrDefault().Money - order.OrderPay.PaidMoney) : 0;
                decimal blMoney = 0;
                if (blModel != null)
                    blMoney = blModel.Money;
                if (consoList != null)
                    consoTransactionMoney = consoList.Money;
                decimal payFee = Math.Round(order.OrderMoney * SystemConsoSwitch.Rate * -1, 2);
                passengerList.ForEach(p =>
                {
                    decimal tempPaidMoney = GetPaidMoney(order.OrderPay.PaidMoney, recordPaidMoney, passengerList.Count, currentPassengerIndex);

                    #region Ticket_Buyer
                    list.Add(new Ticket_Buyer
                     {
                         PayNumber = order.OrderPay.PaySerialNumber,
                         Code = order.BusinessmanCode,
                         ABFee = p.ABFee,
                         BigCode = order.BigCode,
                         CarryCode = carray.Trim('/'),
                         CommissionMoney = Math.Floor(order.Policy.PolicyPoint / 100 * p.SeatPrice),
                         CurrentOrderID = order.OrderId,
                         FlightNum = strFlightNumber.Trim('/'),
                         OrderID = order.OrderId,
                         OrderMoney = p.PayMoney,
                         PassengerName = p.PassengerName,
                         PayMethod = GetPayMethod(order.OrderPay.PayMethodCode),
                         PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                         PNR = order.PnrCode,
                         PolicyPoint = p.PassengerType == EnumPassengerType.Baby ? 0 : order.Policy.PolicyPoint,
                         RetirementPoundage = 0,
                         RQFee = p.RQFee,
                         Seat = strSeat.Trim('/'),
                         SeatPrice = p.SeatPrice,
                         StartTime = flyDateTime.Trim('/'),
                         TicketNum = p.TicketNumber,
                         TicketState = "出票",
                         Voyage = strTravel.Trim('/'),
                         CreateDate = order.IssueTicketTime.Value
                     });
                    #endregion
                    #region Ticket_Conso

                    decimal consoMoney = 0;
                    var pay = pList.Where(m => m.Code.Equals(setting.CashbagCode)).FirstOrDefault();
                    if (pay != null)
                        consoMoney = p.PassengerType == EnumPassengerType.Baby ? pay.BabyMoney / babyCount : (pay.Money - pay.BabyMoney) / adultCount;

                    list.Add(new Ticket_Conso
                    {
                        PayNumber = order.OrderPay.PaySerialNumber,
                        PaidMethod = order.OrderPay.PaidMethod,
                        ABFee = p.ABFee,
                        BigCode = order.BigCode,
                        CarryCode = carray.Trim('/'),
                        CurrentOrderID = order.OrderId,
                        FlightNum = strFlightNumber.Trim('/'),
                        OrderID = order.OrderId,
                        OrderMoney = p.PayMoney,
                        PassengerName = p.PassengerName,
                        PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                        PNR = order.PnrCode,
                        RetirementPoundage = 0,
                        RQFee = p.RQFee,
                        Seat = strSeat.Trim('/'),
                        SeatPrice = p.SeatPrice,
                        StartTime = flyDateTime.Trim('/'),
                        TicketNum = p.TicketNumber,
                        TicketState = "出票",
                        Voyage = strTravel.Trim('/'),
                        CreateDate = order.IssueTicketTime.Value,
                        Code = order.BusinessmanCode,
                        CarrierCode = order.CarrierCode,
                        Paymethod = GetPayMethod(order.OrderPay.PayMethodCode),
                        IssueTicketCode = order.Policy.Code,
                        PolicyFrom = order.Policy.PolicySourceType == EnumPolicySourceType.Interface ? order.Policy.PlatformCode : order.Policy.Code,
                        PaidMoney = tempPaidMoney,//currentPassengerIndex==1? order.OrderPay.PaidMoney:0
                        PaidPoint = order.Policy.OriginalPolicyPoint,
                        Money = Math.Abs(consoMoney),
                        PayFee = currentPassengerIndex == 1 ? payFee : 0,// payFee*-1支付手续费：订单总金额*费率/乘机人数量
                        TransactionFee = currentPassengerIndex == 1 ? consoTransactionMoney : 0,//consoTransactionMoney交易手续费：若为婴儿,则直接婴儿费用/数量,反之总接受的服务费-婴儿金额/成人数目
                        InCome = currentPassengerIndex == 1 ? blMoney + consoTransactionMoney + payFee + tempTPaidMoney : 0 //收益:保留+交易手续费-支付手续费

                    });
                    #endregion
                    #region Ticket_Carrier
                    //采购上级
                    decimal carrierMoney = 0;
                    var carrierPay = pList.Where(m => m.Code.Equals(order.CarrierCode)).FirstOrDefault();
                    if (carrierPay != null && carrierPay.Money != 0)
                    {
                        if (order.CarrierCode == order.Policy.Code)
                            carrierMoney = p.PassengerType == EnumPassengerType.Baby ? babyMoney - Math.Abs(carrierPay.BabyMoney / babyCount) : (carrierPay.Money - (babyCount > 0 ? ((babyMoney - Math.Abs(carrierPay.BabyMoney / babyCount)) * babyCount) : 0)) / adultCount;
                        else
                            carrierMoney = p.PassengerType == EnumPassengerType.Baby ? carrierPay.BabyMoney / babyCount : (carrierPay.Money - carrierPay.BabyMoney) / adultCount;


                        list.Add(new Ticket_Carrier
                        {
                            PayNumber = order.OrderPay.PaySerialNumber,
                            CarrierCode = order.CarrierCode,
                            ABFee = p.ABFee,
                            BigCode = order.BigCode,
                            CarryCode = carray.Trim('/'),
                            CurrentOrderID = order.OrderId,
                            FlightNum = strFlightNumber.Trim('/'),
                            OrderID = order.OrderId,
                            PassengerName = p.PassengerName,
                            PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                            PNR = order.PnrCode,
                            RetirementPoundage = 0,
                            RQFee = p.RQFee,
                            Seat = strSeat.Trim('/'),
                            SeatPrice = p.SeatPrice,
                            StartTime = flyDateTime.Trim('/'),
                            TicketNum = p.TicketNumber,
                            TicketState = "出票",
                            Voyage = strTravel.Trim('/'),
                            CreateDate = order.IssueTicketTime.Value,
                            Code = order.BusinessmanCode,
                            IssueTicketCode = order.Policy.Code,
                            PolicyFrom = order.Policy.PolicySourceType == EnumPolicySourceType.Local ? order.Policy.Code : order.Policy.PolicySourceType.ToEnumDesc(),
                            PolicyPoint = p.PassengerType == EnumPassengerType.Baby ? 0 : order.Policy.OriginalPolicyPoint,
                            Money = Math.Abs(carrierMoney),
                            Point = carrierPay.Point
                        });
                    }
                    if (order.Policy.PolicySourceType == EnumPolicySourceType.Share && order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        decimal shareCarrierMoney = 0;
                        var shareCarrierPay = pList.Where(m => m.Code.Equals(order.Policy.Code)).FirstOrDefault();
                        if (shareCarrierPay != null)
                            shareCarrierMoney = p.PassengerType == EnumPassengerType.Baby ? babyMoney - Math.Abs(shareCarrierPay.BabyMoney / babyCount) : (shareCarrierPay.Money - (babyCount > 0 ? ((babyMoney - Math.Abs(shareCarrierPay.BabyMoney / babyCount)) * babyCount) : 0)) / adultCount;
                        list.Add(new Ticket_Carrier
                        {
                            PayNumber = order.OrderPay.PaySerialNumber,
                            CarrierCode = order.Policy.Code,
                            ABFee = p.ABFee,
                            BigCode = order.BigCode,
                            CarryCode = carray.Trim('/'),
                            CurrentOrderID = order.OrderId,
                            FlightNum = strFlightNumber.Trim('/'),
                            OrderID = order.OrderId,
                            PassengerName = p.PassengerName,
                            PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                            PNR = order.PnrCode,
                            RetirementPoundage = 0,
                            RQFee = p.RQFee,
                            Seat = strSeat.Trim('/'),
                            SeatPrice = p.SeatPrice,
                            StartTime = flyDateTime.Trim('/'),
                            TicketNum = p.TicketNumber,
                            TicketState = "出票",
                            Voyage = strTravel.Trim('/'),
                            CreateDate = order.IssueTicketTime.Value,
                            Code = order.BusinessmanCode,
                            IssueTicketCode = order.Policy.Code,
                            PolicyFrom = order.Policy.PolicySourceType == EnumPolicySourceType.Local ? order.Policy.Code : order.Policy.PolicySourceType.ToEnumDesc(),
                            PolicyPoint = p.PassengerType == EnumPassengerType.Baby ? 0 : order.Policy.OriginalPolicyPoint,
                            Point = shareCarrierPay.Point,
                            Money = shareCarrierMoney
                        });
                    }
                    #endregion
                    #region Ticket_Supplier
                    if (order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Supplier)
                    {
                        decimal supplierMoney = 0;
                        var supplierPay = pList.Where(m => m.Code.Equals(order.Policy.Code)).FirstOrDefault();
                        if (supplierPay != null)
                            supplierMoney = p.PassengerType == EnumPassengerType.Baby ? babyMoney - Math.Abs(supplierPay.BabyMoney / babyCount) : (supplierPay.Money - (babyCount > 0 ? ((babyMoney - Math.Abs(supplierPay.BabyMoney / babyCount)) * babyCount) : 0)) / adultCount;
                        list.Add(new Ticket_Supplier
                        {
                            PayNumber = order.OrderPay.PaySerialNumber,
                            IssueTicketCode = order.Policy.Code,
                            ABFee = p.ABFee,
                            BigCode = order.BigCode,
                            CarryCode = carray.Trim('/'),
                            CurrentOrderID = order.OrderId,
                            FlightNum = strFlightNumber.Trim('/'),
                            OrderID = order.OrderId,
                            PassengerName = p.PassengerName,
                            PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                            PNR = order.PnrCode,
                            PolicyPoint = p.PassengerType == EnumPassengerType.Baby ? 0 : order.Policy.OriginalPolicyPoint,
                            RetirementPoundage = 0,
                            RQFee = p.RQFee,
                            Seat = strSeat.Trim('/'),
                            SeatPrice = p.SeatPrice,
                            StartTime = flyDateTime.Trim('/'),
                            TicketNum = p.TicketNumber,
                            TicketState = "出票",
                            Voyage = strTravel.Trim('/'),
                            CreateDate = order.IssueTicketTime.Value,
                            Money = supplierMoney
                        });
                        if (order.Policy.PolicySourceType == EnumPolicySourceType.Share)
                        {
                            decimal supplierParentMoney = 0;
                            var supplierParentPay = pList.Where(m => m.Code.Equals(order.Policy.CarrierCode)).FirstOrDefault();
                            if (supplierParentPay != null)
                            {
                                supplierParentMoney = p.PassengerType == EnumPassengerType.Baby ? supplierParentPay.BabyMoney / babyCount : (supplierParentPay.Money - supplierParentPay.BabyMoney) / adultCount;
                                list.Add(new Ticket_Carrier
                                {
                                    PayNumber = order.OrderPay.PaySerialNumber,
                                    CarrierCode = order.Policy.CarrierCode,
                                    ABFee = p.ABFee,
                                    BigCode = order.BigCode,
                                    CarryCode = carray.Trim('/'),
                                    CurrentOrderID = order.OrderId,
                                    FlightNum = strFlightNumber.Trim('/'),
                                    OrderID = order.OrderId,
                                    PassengerName = p.PassengerName,
                                    PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                                    PNR = order.PnrCode,
                                    RetirementPoundage = 0,
                                    RQFee = p.RQFee,
                                    Seat = strSeat.Trim('/'),
                                    SeatPrice = p.SeatPrice,
                                    StartTime = flyDateTime.Trim('/'),
                                    TicketNum = p.TicketNumber,
                                    TicketState = "出票",
                                    Voyage = strTravel.Trim('/'),
                                    CreateDate = order.IssueTicketTime.Value,
                                    Code = order.BusinessmanCode,
                                    IssueTicketCode = order.Policy.Code,
                                    PolicyFrom = order.Policy.PolicySourceType == EnumPolicySourceType.Local ? order.Policy.Code : order.Policy.PolicySourceType.ToEnumDesc(),
                                    PolicyPoint = p.PassengerType == EnumPassengerType.Baby ? 0 : order.Policy.OriginalPolicyPoint,
                                    Point = supplierParentPay.Point,
                                    Money = supplierParentMoney
                                });
                            }
                        }
                    }
                    #endregion
                    currentPassengerIndex++;
                    recordPaidMoney += tempPaidMoney;
                    recordPayFeeMoney += payFee;
                    BehaviorStatService.SaveBehaviorStat(order.BusinessmanCode, DataContracts.SystemSetting.EnumBehaviorOperate.OutTicketCount);
                });
                //修改保险状态
                var insuranceFac = ObjectFactory.GetInstance<InsuranceDomainService>();
                insuranceFac.GetInsuranceNos(order);
                list.ForEach(p => unitOfWorkRepository.PersistCreationOf(p));
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, string.Format("总表错误:{0}", orderid), e);
            }

        }
        public void FindAfterSale(string afterSaleOrderId)
        {
            List<Ticket> list = new List<Ticket>();
            try
            {
                int saleId = int.Parse(afterSaleOrderId);
                var afterSaleOrder = this.afterSaleOrderRepository.FindAll(p => p.Id == saleId).FirstOrDefault();
                string strFlightNumber = string.Empty;
                string strTravel = string.Empty;
                string strSeat = string.Empty;
                string flyDateTime = string.Empty;
                string payWay = string.Empty;
                string carray = string.Empty;
                if (afterSaleOrder is ChangeOrder)
                {
                    ChangeOrder changeOrder = afterSaleOrder as ChangeOrder;
                    payWay = changeOrder.PayWay.ToEnumDesc();
                    changeOrder.SkyWay.ToList().ForEach(sp =>
                    {
                        strFlightNumber += sp.SkyWay.CarrayCode + sp.FlightNumber + "/";
                        PnrAnalysis.CityInfo fromCityInfo = pnrResource.GetCityInfo(sp.SkyWay.FromCityCode);
                        PnrAnalysis.CityInfo toCityCodeInfo = pnrResource.GetCityInfo(sp.SkyWay.ToCityCode);
                        strTravel += (fromCityInfo != null ? fromCityInfo.city.Name : "") + "-" + (toCityCodeInfo != null ? toCityCodeInfo.city.Name : "") + "/";
                        strSeat += sp.Seat + "/";
                        carray += sp.SkyWay.CarrayCode + "/";
                        flyDateTime += sp.FlyDate.Value.ToString("yyyy-MM-dd HH:mm") + "/";
                    });
                }
                else if (afterSaleOrder is AnnulOrder || afterSaleOrder is BounceOrder)
                {
                    payWay = afterSaleOrder.Order.OrderPay.PayMethod.ToEnumDesc();
                    afterSaleOrder.Order.SkyWays.ForEach(sp =>
                    {
                        strFlightNumber += sp.CarrayCode + sp.FlightNumber + "/";
                        PnrAnalysis.CityInfo fromCityInfo = pnrResource.GetCityInfo(sp.FromCityCode);
                        PnrAnalysis.CityInfo toCityCodeInfo = pnrResource.GetCityInfo(sp.ToCityCode);
                        strTravel += (fromCityInfo != null ? fromCityInfo.city.Name : "") + "-" + (toCityCodeInfo != null ? toCityCodeInfo.city.Name : "") + "/";
                        strSeat += sp.Seat + "/";
                        carray += sp.CarrayCode + "/";
                        flyDateTime += sp.StartDateTime.ToString("yyyy-MM-dd HH:mm") + "/";
                    });
                }
                var passengerList = afterSaleOrder.Passenger.ToList();
                List<BounceLine> bouncelines = new List<BounceLine>();
                var payBillList = afterSaleOrder.Order.OrderPay.PayBillDetails.Where(p => p.OpType != EnumOperationType.PayMoney && p.OpType != EnumOperationType.Insurance && p.OpType != EnumOperationType.InsuranceServer).Select(p => new { CashBagCode = p.CashbagCode, Code = p.Code }).ToList();
                if (afterSaleOrder is BounceOrder)
                    bouncelines.AddRange((afterSaleOrder as BounceOrder).BounceLines.Where(p => p.BusArgs != null).ToList());
                else if (afterSaleOrder is AnnulOrder)
                    bouncelines.AddRange((afterSaleOrder as AnnulOrder).BounceLines.Where(p => p.BusArgs != null).ToList());


                decimal payFee = (afterSaleOrder is ChangeOrder) ? Math.Round(afterSaleOrder.Money * SystemConsoSwitch.Rate, 2) :
                    GetPayFee(afterSaleOrder.Money, afterSaleOrder.Order.OrderMoney, afterSaleOrder.Order.RefundedServiceMoney, afterSaleOrder.Order.RefundedTradeMoney);

                //控台退收
                decimal consoMoney = 0;
                //交易手续费
                decimal consoTransactionMoney = 0;
                //退废保留
                decimal consoTransactionBlMoney = 0;
                //收益
                decimal inCome = 0;
                if ((afterSaleOrder is AnnulOrder) || (afterSaleOrder is BounceOrder))
                {
                    ///查询交易手续费列表
                    bouncelines.ForEach(x =>
                    {
                        var parseList = x.BusArgs.Split('|').Select(m => new
                          {
                              Code = payBillList.Where(c => c.CashBagCode == m.Split('^')[0]).Select(yy => yy.Code).FirstOrDefault(),
                              Money = Decimal.Parse(m.Split('^')[1]),
                              Descript = m.Split('^')[2]
                          }).ToList();
                        consoTransactionMoney += parseList.Where(y => y.Code.Equals(setting.CashbagCode) && (y.Descript.Equals("系统退款") || y.Descript.Equals("系统分润手续费退款"))).Sum(y => y.Money);
                        consoTransactionBlMoney += parseList.Where(y => y.Code.Equals(setting.CashbagCode) && y.Descript.Equals("保留")).Sum(y => y.Money);
                        consoMoney += parseList.Where(y => y.Code.Equals(setting.CashbagCode)).Sum(y => y.Money);
                    });
                }
                else if (afterSaleOrder is ChangeOrder)
                {
                    var tuple = PayArgs((afterSaleOrder as ChangeOrder));
                    consoMoney = tuple.Item1.Where(x => x.Code.Equals(setting.CashbagCode)).Sum(x => x.Money);
                    consoTransactionMoney = tuple.Item2;
                }

                if (afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                    inCome = afterSaleOrder.Money - payFee;
                else
                    inCome = consoTransactionMoney + consoTransactionBlMoney - payFee;
                bool isFirst = true;
                passengerList.ForEach(p =>
                {
                    List<TicketDetail> bounceOrAnnulList = null;
                    if (afterSaleOrder is AnnulOrder || afterSaleOrder is BounceOrder)
                    {
                        var bModel = bouncelines.Where(n => n.PassgenerName == p.Passenger.PassengerName).FirstOrDefault();
                        if (bModel != null && !string.IsNullOrEmpty(bModel.BusArgs))
                        {
                            bounceOrAnnulList = bouncelines.Where(n => n.PassgenerName == p.Passenger.PassengerName).FirstOrDefault().BusArgs.Split('|').Select(m => new
                            {
                                Code = payBillList.Where(c => c.CashBagCode == m.Split('^')[0]).Select(yy => yy.Code).FirstOrDefault(),
                                Money = Decimal.Parse(m.Split('^')[1])
                            }).GroupBy(x => x.Code).Select(sp => new TicketDetail { Code = sp.Key, Money = sp.Sum(x => x.Money) }).ToList();
                        }
                    }
                    else if (afterSaleOrder is ChangeOrder)
                    {
                        var tuple = PayArgs((afterSaleOrder as ChangeOrder));
                        bounceOrAnnulList = tuple.Item1;
                    }
                    if (bounceOrAnnulList == null)
                        bounceOrAnnulList = new List<TicketDetail>();
                    #region Ticket_Buyer
                    list.Add(new Ticket_Buyer
                    {
                        PayNumber = (afterSaleOrder is ChangeOrder) ? (afterSaleOrder as ChangeOrder).OutPayNo : afterSaleOrder.Order.OrderPay.PaySerialNumber,
                        Code = afterSaleOrder.Order.BusinessmanCode,
                        ABFee = p.Passenger.ABFee,
                        BigCode = afterSaleOrder.Order.BigCode,
                        CarryCode = carray.Trim('/'),
                        CommissionMoney = 0,// Math.Floor(afterSaleOrder.Order.Policy.PolicyPoint / 100 * p.Passenger.SeatPrice),
                        CurrentOrderID = afterSaleOrder.Id.ToString(),
                        FlightNum = strFlightNumber.Trim('/'),
                        OrderID = afterSaleOrder.OrderID,
                        OrderMoney = p.RetirementMoney,
                        RetirementPoundage = p.RetirementPoundage,
                        PassengerName = p.Passenger.PassengerName,
                        PayMethod = (afterSaleOrder is ChangeOrder) ? GetPayMethod((afterSaleOrder as ChangeOrder).PayWay) : GetPayMethod(afterSaleOrder.Order.OrderPay.PayMethodCode),
                        PMFee = p.Passenger.SeatPrice + p.Passenger.RQFee + p.Passenger.ABFee,
                        PNR = afterSaleOrder.Order.PnrCode,
                        PolicyPoint = 0,// afterSaleOrder.Order.Policy.PolicyPoint,
                        RQFee = p.Passenger.RQFee,
                        Seat = strSeat.Trim('/'),
                        SeatPrice = p.Passenger.SeatPrice,
                        StartTime = flyDateTime.Trim('/'),
                        TicketNum = p.Passenger.TicketNumber,
                        TicketState = afterSaleOrder.AfterSaleType,
                        Voyage = strTravel.Trim('/'),
                        CreateDate = afterSaleOrder.CompletedTime.Value
                    });
                    #endregion
                    #region Ticket_Conso
                    list.Add(new Ticket_Conso
                    {
                        PayNumber = (afterSaleOrder is ChangeOrder) ? (afterSaleOrder as ChangeOrder).OutPayNo : afterSaleOrder.Order.OrderPay.PaySerialNumber,
                        ABFee = p.Passenger.ABFee,
                        BigCode = afterSaleOrder.Order.BigCode,
                        CarryCode = carray.Trim('/'),
                        CurrentOrderID = afterSaleOrder.Id.ToString(),
                        FlightNum = strFlightNumber.Trim('/'),
                        OrderID = afterSaleOrder.Order.OrderId,
                        OrderMoney = p.RetirementMoney,
                        PassengerName = p.Passenger.PassengerName,
                        PMFee = p.Passenger.SeatPrice + p.Passenger.RQFee + p.Passenger.ABFee,
                        PNR = afterSaleOrder.Order.PnrCode,
                        RetirementPoundage = p.RetirementPoundage,
                        RQFee = p.Passenger.RQFee,
                        Seat = strSeat.Trim('/'),
                        SeatPrice = p.Passenger.SeatPrice,
                        StartTime = flyDateTime.Trim('/'),
                        TicketNum = p.Passenger.TicketNumber,
                        TicketState = afterSaleOrder.AfterSaleType,
                        Voyage = strTravel.Trim('/'),
                        Paymethod = (afterSaleOrder is ChangeOrder) ? GetPayMethod((afterSaleOrder as ChangeOrder).PayWay) : GetPayMethod(afterSaleOrder.Order.OrderPay.PayMethodCode),
                        CreateDate = afterSaleOrder.CompletedTime.Value,
                        Code = afterSaleOrder.Order.BusinessmanCode,
                        CarrierCode = afterSaleOrder.Order.CarrierCode,
                        IssueTicketCode = afterSaleOrder.Order.Policy.Code,
                        PolicyFrom = afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface ? afterSaleOrder.Order.Policy.PlatformCode : afterSaleOrder.Order.Policy.Code,
                        PaidMoney = 0,
                        PaidPoint = 0,
                        Money = isFirst ? ((afterSaleOrder is ChangeOrder) ? consoMoney : consoMoney * -1) : 0,
                        PayFee = isFirst ? ((afterSaleOrder is ChangeOrder) ? payFee * -1 : payFee) : 0,
                        TransactionFee = isFirst ? ((afterSaleOrder is ChangeOrder) ? consoTransactionMoney : consoTransactionMoney * -1) : 0,
                        InCome = isFirst ? ((afterSaleOrder is ChangeOrder) ? inCome : inCome * -1) : 0
                    });
                    #endregion
                    #region Ticket_Carrier
                    //采购上级
                    decimal carrierMoney = 0;
                    var carrierPay = bounceOrAnnulList.Where(m => m.Code.Equals(afterSaleOrder.Order.CarrierCode)).FirstOrDefault();
                    if (carrierPay != null && carrierPay.Money != 0)
                    {
                        carrierMoney = carrierPay.Money;
                        list.Add(new Ticket_Carrier
                        {
                            PayNumber = (afterSaleOrder is ChangeOrder) ? (afterSaleOrder as ChangeOrder).OutPayNo : afterSaleOrder.Order.OrderPay.PaySerialNumber,
                            CarrierCode = afterSaleOrder.Order.CarrierCode,
                            ABFee = p.Passenger.ABFee,
                            BigCode = afterSaleOrder.Order.BigCode,
                            CarryCode = carray.Trim('/'),
                            CurrentOrderID = afterSaleOrder.Id.ToString(),
                            FlightNum = strFlightNumber.Trim('/'),
                            OrderID = afterSaleOrder.OrderID,
                            PassengerName = p.Passenger.PassengerName,
                            PMFee = p.Passenger.SeatPrice + p.Passenger.RQFee + p.Passenger.ABFee,
                            PNR = afterSaleOrder.Order.PnrCode,
                            RetirementPoundage = p.RetirementPoundage,
                            RQFee = p.Passenger.RQFee,
                            Seat = strSeat.Trim('/'),
                            SeatPrice = p.Passenger.SeatPrice,
                            StartTime = flyDateTime.Trim('/'),
                            TicketNum = p.Passenger.TicketNumber,
                            TicketState = afterSaleOrder.AfterSaleType,
                            Voyage = strTravel.Trim('/'),
                            CreateDate = afterSaleOrder.CompletedTime.Value,
                            Code = afterSaleOrder.Order.BusinessmanCode,
                            IssueTicketCode = afterSaleOrder.Order.Policy.Code,
                            PolicyFrom = afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Local ? afterSaleOrder.Order.Policy.Code : afterSaleOrder.Order.Policy.PolicySourceType.ToEnumDesc(),
                            PolicyPoint = 0,
                            Money = afterSaleOrder.AfterSaleType != "改签" ? carrierMoney * -1 : carrierMoney,
                            Point = 0
                        });
                    }
                    if (afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Share && afterSaleOrder.Order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        decimal shareCarrierMoney = 0;
                        var shareCarrierPay = bounceOrAnnulList.Where(m => m.Code.Equals(afterSaleOrder.Order.Policy.Code)).FirstOrDefault();
                        if (shareCarrierPay != null)
                            shareCarrierMoney = shareCarrierPay.Money;
                        list.Add(new Ticket_Carrier
                        {
                            PayNumber = (afterSaleOrder is ChangeOrder) ? (afterSaleOrder as ChangeOrder).OutPayNo : afterSaleOrder.Order.OrderPay.PaySerialNumber,
                            CarrierCode = afterSaleOrder.Order.Policy.Code,
                            ABFee = p.Passenger.ABFee,
                            BigCode = afterSaleOrder.Order.BigCode,
                            CarryCode = carray.Trim('/'),
                            CurrentOrderID = afterSaleOrder.Id.ToString(),
                            FlightNum = strFlightNumber.Trim('/'),
                            OrderID = afterSaleOrder.OrderID,
                            PassengerName = p.Passenger.PassengerName,
                            PMFee = p.Passenger.SeatPrice + p.Passenger.RQFee + p.Passenger.ABFee,
                            PNR = afterSaleOrder.Order.PnrCode,
                            RetirementPoundage = p.RetirementPoundage,
                            RQFee = p.Passenger.RQFee,
                            Seat = strSeat.Trim('/'),
                            SeatPrice = p.Passenger.SeatPrice,
                            StartTime = flyDateTime.Trim('/'),
                            TicketNum = p.Passenger.TicketNumber,
                            TicketState = afterSaleOrder.AfterSaleType,
                            Voyage = strTravel.Trim('/'),
                            CreateDate = afterSaleOrder.CompletedTime.Value,
                            Code = afterSaleOrder.Order.BusinessmanCode,
                            IssueTicketCode = afterSaleOrder.Order.Policy.Code,
                            PolicyFrom = afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Local ? afterSaleOrder.Order.Policy.Code : afterSaleOrder.Order.Policy.PolicySourceType.ToEnumDesc(),
                            PolicyPoint = 0,
                            Money = afterSaleOrder.AfterSaleType != "改签" ? shareCarrierMoney * -1 : shareCarrierMoney,
                            Point = 0
                        });
                    }
                    #endregion
                    #region Ticket_Supplier
                    if (afterSaleOrder.Order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Supplier)
                    {
                        decimal supplierMoney = 0;
                        var supplierPay = bounceOrAnnulList.Where(m => m.Code.Equals(afterSaleOrder.Order.Policy.Code)).FirstOrDefault();
                        if (supplierPay != null)
                            supplierMoney = supplierPay.Money;
                        list.Add(new Ticket_Supplier
                        {
                            PayNumber = (afterSaleOrder is ChangeOrder) ? (afterSaleOrder as ChangeOrder).OutPayNo : afterSaleOrder.Order.OrderPay.PaySerialNumber,
                            IssueTicketCode = afterSaleOrder.Order.Policy.Code,
                            ABFee = p.Passenger.ABFee,
                            BigCode = afterSaleOrder.Order.BigCode,
                            CarryCode = carray.Trim('/'),
                            CurrentOrderID = afterSaleOrder.Id.ToString(),
                            FlightNum = strFlightNumber.Trim('/'),
                            OrderID = afterSaleOrder.OrderID,
                            PassengerName = p.Passenger.PassengerName,
                            PMFee = p.Passenger.SeatPrice + p.Passenger.RQFee + p.Passenger.ABFee,
                            PNR = afterSaleOrder.Order.PnrCode,
                            PolicyPoint = 0,
                            RetirementPoundage = 0,
                            RQFee = p.Passenger.RQFee,
                            Seat = strSeat.Trim('/'),
                            SeatPrice = p.Passenger.SeatPrice,
                            StartTime = flyDateTime.Trim('/'),
                            TicketNum = p.Passenger.TicketNumber,
                            TicketState = afterSaleOrder.AfterSaleType,
                            Voyage = strTravel.Trim('/'),
                            CreateDate = afterSaleOrder.CompletedTime.Value,
                            Money = afterSaleOrder.AfterSaleType != "改签" ? supplierMoney * -1 : supplierMoney
                        });
                        if (afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Share)
                        {
                            decimal supplierParentMoney = 0;
                            var supplierParentPay = bounceOrAnnulList.Where(m => m.Code.Equals(afterSaleOrder.Order.Policy.CarrierCode)).FirstOrDefault();
                            if (supplierParentPay != null)
                            {
                                supplierParentMoney = supplierParentPay.Money;
                                list.Add(new Ticket_Carrier
                                {
                                    PayNumber = afterSaleOrder.Order.OrderPay.PaySerialNumber,
                                    CarrierCode = afterSaleOrder.Order.Policy.CarrierCode,
                                    ABFee = p.Passenger.ABFee,
                                    BigCode = afterSaleOrder.Order.BigCode,
                                    CarryCode = carray.Trim('/'),
                                    CurrentOrderID = afterSaleOrder.Id.ToString(),
                                    FlightNum = strFlightNumber.Trim('/'),
                                    OrderID = afterSaleOrder.OrderID,
                                    PassengerName = p.Passenger.PassengerName,
                                    PMFee = p.Passenger.SeatPrice + p.Passenger.RQFee + p.Passenger.ABFee,
                                    PNR = afterSaleOrder.Order.PnrCode,
                                    RetirementPoundage = p.RetirementPoundage,
                                    RQFee = p.Passenger.RQFee,
                                    Seat = strSeat.Trim('/'),
                                    SeatPrice = p.Passenger.SeatPrice,
                                    StartTime = flyDateTime.Trim('/'),
                                    TicketNum = p.Passenger.TicketNumber,
                                    TicketState = afterSaleOrder.AfterSaleType,
                                    Voyage = strTravel.Trim('/'),
                                    CreateDate = afterSaleOrder.CompletedTime.Value,
                                    Code = afterSaleOrder.Order.BusinessmanCode,
                                    IssueTicketCode = afterSaleOrder.Order.Policy.Code,
                                    PolicyFrom = afterSaleOrder.Order.Policy.PolicySourceType == EnumPolicySourceType.Local ? afterSaleOrder.Order.Policy.Code : afterSaleOrder.Order.Policy.PolicySourceType.ToEnumDesc(),
                                    PolicyPoint = 0,
                                    Money = afterSaleOrder.AfterSaleType != "改签" ? supplierParentMoney * -1 : supplierParentMoney,
                                    Point = 0
                                });
                            }
                        }
                    }
                    #endregion
                    isFirst = false;
                });
                afterSaleOrder.Order.RefundedTradeMoney -= afterSaleOrder.Money;
                afterSaleOrder.Order.RefundedServiceMoney -= payFee;
                unitOfWorkRepository.PersistUpdateOf(afterSaleOrder);
                Logger.WriteLog(LogType.ERROR, list != null ? list.Count.ToString() : "list is null");
                list.ForEach(p => unitOfWorkRepository.PersistCreationOf(p));
                Logger.WriteLog(LogType.ERROR, "1");
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, string.Format("售后订单总表错误：{0}", afterSaleOrderId), e);
                MessageQueueManager.SendMessage(afterSaleOrderId, 1);
            }
        }
        public void RealIssueTicket(string orderid)
        {
            List<Ticket> list = new List<Ticket>();
            try
            {
                var order = this.orderRepository.FindAllNoTracking(p => p.OrderId.Equals(orderid)).FirstOrDefault();

                string strFlightNumber = string.Empty;
                string strTravel = string.Empty;
                string strSeat = string.Empty;
                string flyDateTime = string.Empty;
                string carray = string.Empty;
                order.SkyWays.ToList().ForEach(sp =>
                {
                    strFlightNumber += sp.CarrayCode + sp.FlightNumber + "/";
                    PnrAnalysis.CityInfo fromCityInfo = pnrResource.GetCityInfo(sp.FromCityCode);
                    PnrAnalysis.CityInfo toCityCodeInfo = pnrResource.GetCityInfo(sp.ToCityCode);
                    strTravel += (fromCityInfo != null ? fromCityInfo.city.Name : "") + "-" + (toCityCodeInfo != null ? toCityCodeInfo.city.Name : "") + "/";
                    strSeat += sp.Seat + "/";
                    carray += sp.CarrayCode + "/";
                    flyDateTime += sp.StartDateTime.ToString("yyyy-MM-dd HH:mm") + "/";
                });

                var passengerList = order.Passengers.ToList();

                passengerList.ForEach(p =>
                {
                    #region Ticket_Buyer
                    list.Add(new Ticket_Buyer
                    {
                        PayNumber = order.OrderPay.PaySerialNumber,
                        Code = order.BusinessmanCode,
                        ABFee = p.ABFee,
                        BigCode = order.BigCode,
                        CarryCode = carray.Trim('/'),
                        CommissionMoney = Math.Floor(order.Policy.PolicyPoint / 100 * p.SeatPrice),
                        CurrentOrderID = order.OrderId,
                        FlightNum = strFlightNumber.Trim('/'),
                        OrderID = order.OrderId,
                        OrderMoney = p.PayMoney,
                        PassengerName = p.PassengerName,
                        PayMethod = GetPayMethod(order.OrderPay.PayMethodCode),
                        PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                        PNR = order.PnrCode,
                        PolicyPoint = p.PassengerType == EnumPassengerType.Baby ? 0 : order.Policy.PolicyPoint,
                        RetirementPoundage = 0,
                        RQFee = p.RQFee,
                        Seat = strSeat.Trim('/'),
                        SeatPrice = p.SeatPrice,
                        StartTime = flyDateTime.Trim('/'),
                        TicketNum = p.TicketNumber,
                        TicketState = "拒绝出票",
                        Voyage = strTravel.Trim('/'),
                        CreateDate = DateTime.Now
                    });
                    #endregion
                    #region Ticket_Conso
                    list.Add(new Ticket_Conso
                    {
                        PaidMethod = order.OrderPay.PaidMethod,
                        PayNumber = order.OrderPay.PaySerialNumber,
                        ABFee = p.ABFee,
                        BigCode = order.BigCode,
                        CarryCode = carray.Trim('/'),
                        CurrentOrderID = order.OrderId,
                        FlightNum = strFlightNumber.Trim('/'),
                        OrderID = order.OrderId,
                        OrderMoney = p.PayMoney,
                        PassengerName = p.PassengerName,
                        PMFee = p.SeatPrice + p.RQFee + p.ABFee,
                        PNR = order.PnrCode,
                        RetirementPoundage = 0,
                        RQFee = p.RQFee,
                        Seat = strSeat.Trim('/'),
                        SeatPrice = p.SeatPrice,
                        StartTime = flyDateTime.Trim('/'),
                        TicketNum = p.TicketNumber,
                        TicketState = "拒绝出票",
                        Voyage = strTravel.Trim('/'),
                        CreateDate = DateTime.Now,
                        Code = order.BusinessmanCode,
                        CarrierCode = order.CarrierCode,
                        Paymethod = GetPayMethod(order.OrderPay.PayMethodCode),
                        IssueTicketCode = order.Policy.Code,
                        PolicyFrom = order.Policy.PolicySourceType == EnumPolicySourceType.Interface ? order.Policy.PlatformCode : order.Policy.Code,
                        PaidMoney = 0,
                        PaidPoint = 0,
                        Money = 0,
                        PayFee = 0,
                        TransactionFee = 0,
                        InCome = 0

                    });
                    #endregion
                });
                //修改保险状态
                list.ForEach(p => unitOfWorkRepository.PersistCreationOf(p));
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, string.Format("总表错误:{0}", orderid), e);
            }

        }


        private decimal GetPaidMoney(decimal paidMoney, decimal recordPaidMoney, int passengerCount, int currentPassengerIndex)
        {
            if (passengerCount == currentPassengerIndex)
                return paidMoney - recordPaidMoney;
            return Math.Round((decimal)paidMoney / passengerCount, 2);
        }

        private Tuple<List<TicketDetail>, decimal> PayArgs(ChangeOrder model)
        {

            List<TicketDetail> list = new List<TicketDetail>();
            CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
            model.Passenger.ForEach(ps =>
            {
                //产生金额
                // decimal retirementMoney = model.Passenger.Where(p => p.Id == afterPassengerId).FirstOrDefault().RetirementMoney;
                decimal retirementMoney = ps.RetirementMoney;
                //合作者的信息

                StringBuilder args = new StringBuilder();
                var dataBill = new DataBill();
                if (model.Order.Policy.PolicySourceType == EnumPolicySourceType.Local)
                {
                    if (model.Order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        list.Add(new TicketDetail { Code = model.Order.Policy.Code, Money = (retirementMoney - databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2)) });
                    }
                    else
                    {
                        var carrier = GetCarrierCashBagCode(model.Order.BusinessmanCode);

                        //供应实收
                        list.Add(new TicketDetail
                        {
                            Code = model.Order.Policy.Code,
                            Money = retirementMoney - dataBill.NewRound(retirementMoney * model.Order.Policy.Rate, 2)
                        });
                        //运营实收
                        list.Add(new TicketDetail
                        {
                            Code = carrier.Code,
                            Money = databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2) - dataBill.NewRound(retirementMoney * carrier.Rate, 2)
                        });
                    }
                }
                else if (model.Order.Policy.PolicySourceType == EnumPolicySourceType.Share)
                {

                    if (model.Order.Policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        list.Add(new TicketDetail
                        {
                            Code = model.Order.Policy.Code,
                            Money = (retirementMoney - databill.NewRound(retirementMoney * model.Order.Policy.Rate, 2))
                        });
                    }
                    else
                    {
                        var supper = this.businessmanRepository.FindAll(p => p.Code == model.Order.Policy.Code).OfType<Supplier>().FirstOrDefault();
                        if (supper == null)
                            throw new CustomException(400, "未获取信息");
                        var carrier = this.businessmanRepository.FindAllNoTracking(p => p.Code == supper.CarrierCode).OfType<Carrier>().FirstOrDefault();
                        if (carrier == null)
                            throw new CustomException(400, "未获取信息");
                        //供应实收
                        list.Add(new TicketDetail
                        {
                            Code = model.Order.Policy.Code,
                            Money = (retirementMoney - dataBill.NewRound(retirementMoney * model.Order.Policy.Rate, 2))
                        });
                        //运营实收
                        list.Add(new TicketDetail
                        {
                            Code = carrier.Code,
                            Money = dataBill.NewRound(retirementMoney * model.Order.Policy.Rate, 2) - dataBill.NewRound(retirementMoney * carrier.Rate, 2)
                        });
                    }
                }
                else if (model.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                {
                    list.Add(new TicketDetail { Code = setting.CashbagCode, Money = model.Money });
                }
            });
            var partnerMoney = model.Money - list.Sum(x => x.Money);
            if (partnerMoney != 0)
                list.Add(new TicketDetail { Code = setting.CashbagCode, Money = partnerMoney });
            return Tuple.Create<List<TicketDetail>, decimal>(list.GroupBy(x => x.Code).Select(p => new TicketDetail { Code = p.Key, Money = p.Sum(x => x.Money) }).ToList(), partnerMoney);
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
        public string GetPayMethod(string methodCode)
        {
            string result = string.Empty;
            switch (methodCode)
            {
                case "AliPay":
                    result = "支付宝";
                    break;
                case "现金账户":
                    result = "现金账户";
                    break;
                case "信用账户":
                    result = "信用账户";
                    break;
                default:
                    result = "财付通";
                    break;
            }
            return result;
        }
        public string GetPayMethod(EnumPayMethod? payMethod)
        {
            if (!payMethod.HasValue)
                return string.Empty;
            if (payMethod.Value == EnumPayMethod.Bank)
                return EnumPayMethod.TenPay.ToEnumDesc();
            return payMethod.ToEnumDesc();
        }
        /// <summary>
        /// 退款手续费计算
        /// </summary>
        /// <param name="refundMoney">退款金额</param>
        /// <param name="orderMoney">订单金额</param>
        /// <param name="totalPayFee">总手续费</param>
        /// <param name="refundTradeMoney">剩余退款金额</param>
        /// <returns></returns>
        private decimal GetPayFee(decimal refundMoney, decimal orderMoney, decimal refundedPayFee, decimal refundTradeMoney)
        {
            //订单总手续费
            decimal payFee = Math.Round(orderMoney * SystemConsoSwitch.Rate, 2);

            if (refundMoney == refundTradeMoney)
                return refundedPayFee;
            return Math.Round((refundMoney / orderMoney) * payFee, 2);
        }
    }
}
