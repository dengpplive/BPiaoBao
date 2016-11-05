using AutoMapper;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Services;
using PnrAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public static class OrderMapper
    {
        private static string GetPayMethod(string PayMethod)
        {
            if (!string.IsNullOrEmpty(PayMethod))
            {
                switch (PayMethod)
                {
                    case "Account":
                        PayMethod = "现金账户";
                        break;
                    case "Credit":
                        PayMethod = "信用账户";
                        break;
                    case "Bank":
                        PayMethod = "银行卡";
                        break;
                    //case "Platform":
                    //    PayMethod = "支付平台";
                    //    break;
                    case "Alipay":
                    case "AliPay":
                        PayMethod = "支付宝";
                        break;
                    case "Tenpay":
                    case "TenPay":
                        PayMethod = "财付通";
                        break;
                    default:
                        break;
                }
            }
            return PayMethod;
        }
        public static PolicyDto ToPolicyDto(this Policy policy, Order order)
        {
            DataBill databill = new DataBill();
            decimal PayMoney = databill.GetPayPrice(policy.SeatPrice, policy.ABFee, policy.RQFee, policy.PolicyPoint, policy.ReturnMoney);
            decimal Commission = databill.GetCommission(policy.PolicyPoint, policy.SeatPrice, policy.ReturnMoney);
            var _p = new PolicyDto()
                  {
                      Commission = Commission,
                      AreaCity = policy.AreaCity,
                      Id = policy.PolicyId,
                      PlatformCode = policy.PlatformCode,
                      PlatformName = policy.PlatformCode,
                      Point = policy.PolicyPoint,
                      DownPoint = policy.DownPoint,
                      PaidPoint = policy.PaidPoint,
                      OriginalPolicyPoint = policy.OriginalPolicyPoint,
                      ReturnMoney = policy.ReturnMoney,
                      IsChangePNRCP = policy.IsChangePNRCP,
                      IssueTicketWay = ((int)policy.EnumIssueTicketWay).ToString(),
                      IsSp = policy.IsSp,
                      IsLow = policy.IsLow,
                      PolicyType = policy.PolicyType,
                      WorkTime = policy.WorkTime.ToString(),
                      ReturnTicketTime = policy.ReturnTicketTime.ToString(),
                      AnnulTicketTime = policy.AnnulTicketTime.ToString(),
                      TFGTime = FormatPNR.GetIntersectionTimeSlot(policy.ReturnTicketTime.ToString(), policy.AnnulTicketTime.ToString()),
                      CPOffice = policy.CPOffice,
                      IssueSpeed = policy.IssueSpeed,
                      Remark = policy.Remark,
                      PolicySourceType = EnumItemManager.GetDesc(policy.PolicySourceType),
                      CarryCode = policy.CarryCode,
                      PolicyOwnUserRole = ((int)policy.PolicyOwnUserRole).ToString(),
                      Code = policy.Code,
                      Name = policy.Name,
                      CashbagCode = policy.CashbagCode,
                      Rate = policy.Rate,
                      CarrierCode = policy.CarrierCode,
                      SeatPrice = policy.SeatPrice,
                      ABFee = policy.ABFee,
                      RQFee = policy.RQFee,
                      TicketPrice = policy.SeatPrice + policy.ABFee + policy.RQFee,
                      PayMoney = PayMoney,
                      DefaultPolicySource = order.OrderType,
                      PolicySpecialType = policy.PolicySpecialType,
                      SpecialPriceOrDiscount = policy.SpecialPriceOrDiscount,
                      TodayGYCode = policy.TodayGYCode
                  };
            var currentUser = AuthManager.GetCurrentUser();
            if (currentUser.Type == "Buyer")
            {
                _p.ShowPolicySource = EnumItemManager.GetDesc(policy.PolicySourceType);
            }
            else if (currentUser.Type == "Carrier")
            {
                if (policy.PolicySourceType != EnumPolicySourceType.Local)
                    _p.ShowPolicySource = "系统";
                else if (currentUser.Code == policy.Code)
                    _p.ShowPolicySource = policy.PolicyType;
                else
                    _p.ShowPolicySource = policy.Code;
            }
            else if (currentUser.Type == "Supplier")
            {
                _p.ShowPolicySource = policy.PolicyType;
            }
            else
            {
                if (policy.PolicySourceType == EnumPolicySourceType.Interface)
                    _p.ShowPolicySource = policy.PlatformCode;
                else
                    _p.ShowPolicySource = policy.Code;

            }
            return _p;
        }
        public static OrderDto ToOrderDto(this Order p, OrderDto result = null)
        {
            var temp = p.OrderLogs.Where(x => x.OperationContent.Contains("拒绝出票")).FirstOrDefault();
            DataBill databill = new DataBill();
            PnrResource pnrResource = new PnrResource();
            if (result == null) result = new OrderDto();
            result.HasAfterSale = p.HasAfterSale;
            result.OutOrderId = p.OutOrderId;
            result.OrderId = p.OrderId;
            result.IsSupplier = AuthManager.GetCurrentUser().Type == "Supplier" ? true : false;
            result.IsCarrier = AuthManager.GetCurrentUser().Code == p.CarrierCode ? true : false;
            result.CurrentCode = AuthManager.GetCurrentUser().Code;
            result.UserRole = AuthManager.GetCurrentUser().Type;
            result.CPMoney = p.CPMoney;
            result.OrderMoney = p.OrderMoney;
            result.Remark = p.Remark;
            result.PnrCode = p.PnrCode;
            result.CarrierCode = p.CarrierCode;
            result.BigCode = p.BigCode;
            result.PNRContent = p.PnrContent;
            result.OrderStatus = (int)p.OrderStatus;
            result.BusinessmanName = p.BusinessmanName;
            result.BusinessmanCode = p.BusinessmanCode;
            result.CreateTime = p.CreateTime;
            result.LockAccount = p.LockAccount;
            result.IsCompleted = p.CoordinationStatus;
            result.OrderCommissionTotalMoney = p.OrderCommissionTotalMoney;
            result.PnrSource = p.PnrSource;
            result.IsChangePnrTicket = p.IsChangePnrTicket;
            result.IsLowPrice = p.IsLowPrice;
            result.HaveBabyFlag = p.HaveBabyFlag;
            result.RealRemark = temp != null ? temp.OperationContent : string.Empty;
            result.OrderSource = p.OrderSource;
            result.OldOrderId = p.OldOrderId;
            result.YdOffice = p.YdOffice;
            result.CpOffice = p.CpOffice;
            result.OrderType = p.OrderType.ToString();
            result.PayInfo = new OrderPayDto();
            if (p.OrderPay != null)
            {
                result.PayInfo.PayMethodCode = p.OrderPay.PayMethodCode;
                result.PayInfo.PayDateTime = p.OrderPay.PayDateTime;
                result.PayInfo.PaidMethod = p.OrderPay.PaidMethod == null ? null : p.OrderPay.PaidMethod.ToString();
                result.PayInfo.PayDateTime = p.OrderPay.PayDateTime;
                result.PayInfo.PayMethod = GetPayMethod(p.OrderPay.PayMethod == null ? null : p.OrderPay.PayMethod.ToString());
                result.PayInfo.PaidMoney = p.OrderPay.PaidMoney;
                result.PayInfo.PayMoney = p.OrderPay.PayMoney;
                result.PayInfo.PaidSerialNumber = p.OrderPay.PaidSerialNumber;
                result.PayInfo.PaySerialNumber = p.OrderPay.PaySerialNumber;
                result.PayInfo.SystemFee = p.OrderPay.SystemFee;
                result.PayInfo.TradePoundage = p.OrderPay.TradePoundage;
                result.PayInfo.PaidStatus = p.OrderPay.PaidStatus;
                result.PayInfo.PayStatus = p.OrderPay.PayStatus;
                result.PayInfo.PayBillDetailDtos = p.OrderPay.PayBillDetails.Select(x => new PayBillDetailDto()
                {
                    ID = x.ID,
                    Code = x.Code,
                    CashbagCode = x.CashbagCode,
                    AdjustType = x.AdjustType,
                    Money = x.Money,
                    Name = x.Name,
                    OpType = x.OpType.ToEnumDesc(),
                    Point = x.Point,
                    Remark = x.Remark
                }).ToList();
                if (p.OrderPay.PaidDateTime.HasValue)
                {
                    DateTime ticketTime = DateTime.Now;
                    if (p.IssueTicketTime.HasValue)
                        ticketTime = p.IssueTicketTime.Value;
                    result.Time = (int)ticketTime.Subtract(p.OrderPay.PaidDateTime.Value).TotalMinutes;

                }
            }
            result.Policy = new PolicyDto();
            if (p.Policy != null)
            {
                result.Policy = p.Policy.ToPolicyDto(p);
            }

            result.Passengers = p.Passengers
                                      .Select(x => new PassengerDto()
                                      {
                                          Id = x.Id,
                                          PassengerName = x.PassengerName,
                                          CardNo = x.CardNo,
                                          TaxFee = x.ABFee,
                                          RQFee = x.RQFee,
                                          SeatPrice = x.SeatPrice,
                                          Mobile = x.Mobile,
                                          PassengerType = x.PassengerType,//.ToEnumDesc(),
                                          TicketNumber = x.TicketNumber,
                                          TravelNumber = x.TravelNumber,
                                          PassengerTripStatus = x.PassengerTripStatus,
                                          PayMoney = x.PayMoney,
                                          BuyInsuranceCount = x.BuyInsuranceCount,
                                          BuyInsurancePrice = x.BuyInsurancePrice,
                                          IsInsuranceRefund = x.IsInsuranceRefund,
                                          InsuranceRefunrPrice = x.InsuranceRefunrPrice,
                                          IdType = x.IdType,
                                          SexType = x.SexType,
                                          Birth = x.Birth,
                                          CPMoney = x.CPMoney
                                      }).ToList();
            result.SkyWays = p.SkyWays
                                   .Select(k => new SkyWayDto()
                                   {
                                       SkyWayId = k.Id,
                                       FromCityCode = k.FromCityCode,
                                       ToCityCode = k.ToCityCode,
                                       FromCity = pnrResource.GetCityInfo(k.FromCityCode) == null ? "" : pnrResource.GetCityInfo(k.FromCityCode).city.Name,
                                       ToCity = pnrResource.GetCityInfo(k.ToCityCode) == null ? "" : pnrResource.GetCityInfo(k.ToCityCode).city.Name,
                                       CarrayShortName = pnrResource.GetAirInfo(k.CarrayCode) == null ? "" : pnrResource.GetAirInfo(k.CarrayCode).Carry.AirShortName,
                                       CarrayCode = k.CarrayCode,
                                       StartDateTime = k.StartDateTime,
                                       ToDateTime = k.ToDateTime,
                                       FlightNumber = k.FlightNumber,
                                       FromTerminal = k.FromTerminal,
                                       Seat = k.Seat,
                                       ToTerminal = k.ToTerminal,
                                       Discount = k.Discount.HasValue ? k.Discount.Value : 0m,
                                       FlightModel = k.FlightModel
                                   }).ToList();


            return result;
        }

        public static OrderDetailDto ToOrderDetail(this Order p, bool? onlyShow = null)
        {
            var result = new OrderDetailDto();
            p.ToOrderDto(result);


            result.OrderLogs = new List<OrderLogDto>();
            if (p.OrderLogs != null && p.OrderLogs.Count > 0)
            {
                var logQuery = p.OrderLogs.AsQueryable();
                if (onlyShow.HasValue)
                    logQuery = logQuery.Where(q => q.IsShowLog == onlyShow.Value);
                result.OrderLogs = logQuery.Select(l => new OrderLogDto()
                {
                    Id = l.Id,
                    OperationContent = l.OperationContent,
                    OperationDatetime = l.OperationDatetime,
                    OperationPerson = l.OperationPerson,
                    Remark = l.Remark
                }).ToList();
            }
            return result;
        }

        public static CoordinationDto ToOrderCoordination(this Order p)
        {
            var result = new CoordinationDto();
            p.ToOrderDto(result);
            result.CoordinationLogs = new List<CoordinationLogDto>();
            if (p.CoordinationLogs != null && p.CoordinationLogs.Count > 0)
            {
                var logQuery = p.CoordinationLogs.AsQueryable();
                result.CoordinationLogs = logQuery.Select(l => new CoordinationLogDto()
                {
                    AddDatetime = l.AddDatetime,
                    Content = l.Content,
                    Type = l.Type,
                    OperationPerson = l.OperationPerson
                }).OrderBy(t => t.AddDatetime).ToList();
            }
            return result;
        }
        public static int? ToClientStatus(this int p)
        {
            int? OrderStatus = null;
            switch (p)
            {
                case (int)EnumOrderStatus.WaitChoosePolicy:
                    OrderStatus = (int)EnumClientOrderStatus.WaitChoosePolicy;
                    break;
                case (int)EnumOrderStatus.NewOrder:
                    OrderStatus = (int)EnumClientOrderStatus.NewOrder;
                    break;
                case (int)EnumClientOrderStatus.PayWaitCreatePlatformOrder:
                case (int)EnumOrderStatus.CreatePlatformFail:
                case (int)EnumOrderStatus.WaitAndPaid:
                case (int)EnumOrderStatus.WaitIssue:
                case (int)EnumOrderStatus.AutoIssueFail:
                    OrderStatus = (int)EnumClientOrderStatus.WaitIssue;
                    break;
                case (int)EnumOrderStatus.OrderCanceled:
                    OrderStatus = (int)EnumClientOrderStatus.OrderCanceled;
                    break;
                case (int)EnumOrderStatus.IssueAndCompleted:
                    OrderStatus = (int)EnumClientOrderStatus.IssueAndCompleted;
                    break;
                case (int)EnumOrderStatus.WaitReimburseWithPlatformRepelIssue:
                case (int)EnumOrderStatus.WaitReimburseWithRepelIssue:
                    OrderStatus = (int)EnumClientOrderStatus.WaitReimburseWithRepelIssue;
                    break;
                case (int)EnumOrderStatus.RepelIssueAndCompleted:
                    OrderStatus = (int)EnumClientOrderStatus.RepelIssueAndCompleted;
                    break;
                case (int)EnumOrderStatus.Invalid:
                    OrderStatus = (int)EnumClientOrderStatus.Invalid;
                    break;
                case (int)EnumOrderStatus.RepelIssueRefunding:
                    OrderStatus = (int)EnumClientOrderStatus.RepelIssueRefunding;
                    break;
                case (int)EnumClientOrderStatus.ApplyBabyFail:
                    OrderStatus = (int)EnumClientOrderStatus.ApplyBabyFail;
                    break;
                case (int)EnumClientOrderStatus.RepelApplyBaby:
                    OrderStatus = (int)EnumClientOrderStatus.RepelApplyBaby;
                    break;
                case (int)EnumClientOrderStatus.PaymentInWaiting:
                    OrderStatus = (int)EnumClientOrderStatus.PaymentInWaiting;
                    break;
            }
            return OrderStatus;
        }





        public static ResponseAnnulOrder ToResponseAnnulOrder(this AnnulOrder p)
        {
            return Mapper.Map<AnnulOrder, ResponseAnnulOrder>(p);
        }
        public static AnnulOrder ToRequestAnnulOrder(this RequestAnnulOrder p)
        {
            return Mapper.Map<RequestAnnulOrder, AnnulOrder>(p);
        }
        public static OrderLogDto ToOrderLogDto(this OrderLog p)
        {
            return Mapper.Map<OrderLog, OrderLogDto>(p);
        }
        public static BounceOrder ToRequestBounceOrder(this RequestBounceOrder p)
        {
            return Mapper.Map<RequestBounceOrder, BounceOrder>(p);
        }
        public static ResponseBounceOrder ToResponseBounceOrder(this BounceOrder p)
        {
            return Mapper.Map<BounceOrder, ResponseBounceOrder>(p);
        }
        public static ChangeOrder ToRequestChangeOrder(this RequestChangeOrder p)
        {
            return Mapper.Map<RequestChangeOrder, ChangeOrder>(p);
        }
        public static ModifyOrder ToRequestModifyOrder(this RequestModifyOrder p)
        {
            return Mapper.Map<RequestModifyOrder, ModifyOrder>(p);
        }
        public static ResponseChangeOrder ToResponseChangeOrder(this ChangeOrder p)
        {
            return Mapper.Map<ChangeOrder, ResponseChangeOrder>(p);
        }
        public static AfterSaleOrder ToRequestAfterSaleOrder(this RequestAfterSaleOrder p)
        {
            return Mapper.Map<RequestAfterSaleOrder, AfterSaleOrder>(p);
        }
        public static ResponseAfterSaleOrder ToResponseAfterSaleOrder(this AfterSaleOrder p)
        {
            var model = Mapper.Map<AfterSaleOrder, ResponseAfterSaleOrder>(p);
            var currentUser = AuthManager.GetCurrentUser();
            if (!string.IsNullOrEmpty(currentUser.Type) && currentUser.Type != "Buyer")
            {
                if (!(p is ChangeOrder))
                {
                    model.Money = model.SMoney;
                }
                if (p.Order.CarrierCode != AuthManager.GetCurrentUser().Code)
                {
                    model.TotalMoney = p.AfterCPMoney;
                }
            }
            return model;
        }
        public static ResponseModifyOrder ToResponseModifySaleOrder(this ModifyOrder p)
        {
            return Mapper.Map<ModifyOrder, ResponseModifyOrder>(p);
        }
    }
}
