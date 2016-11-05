using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers.BTicket
{
    public class TicketSumController : BaseController
    {
        private readonly IStationOrderService _service;
        private static readonly ChannelFactory<IStationOrderService> _cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name);

        public TicketSumController()
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                _service = _cf.CreateChannel();
            }
        }
        public ActionResult TicketSumTable()
        {
            return View();
        }
        public ActionResult TicketSumTableSupplier()
        {
            return View();
        }
        ///// <summary>
        ///// 机票信息汇总
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult GetTicketSummary(TicketSumQueryEntity queryCond)
        //{
        //    List<TicketInfoSummaryEntity> TicketInfoSummaryList = _service.GetTicketInfoSummaryList(queryCond.OrderId, queryCond.OutOrderId, queryCond.PNR, queryCond.TicketNumber,
        //   queryCond.PlatformCode, queryCond.PolicyType,
        //   queryCond.CarrayCode, queryCond.FromCity, queryCond.ToCity,
        //   queryCond.TicketStatus, queryCond.BusinessmanName, queryCond.BusinessmanCode, "", queryCond.OperatorAccount, null,
        //   null, queryCond.StartIssueRefundTime, queryCond.EndIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59), null, null, queryCond.PayWay);
        //    return Json(TicketInfoSummaryList);

        //}

        ///// <summary>
        ///// 机票销售统计
        ///// </summary>
        ///// <returns></returns>
        //public JsonResult GetTicketSalesStatistics(TicketSumQueryEntity queryCond)
        //{
        //    List<TicketSalesStatisticsEntity> TicketSalesList = _service.GetTicketSalesStatisticsList(queryCond.OrderId, queryCond.OutOrderId, queryCond.PNR, queryCond.TicketNumber,
        //     queryCond.PlatformCode, queryCond.PolicyType,
        //     queryCond.CarrayCode, queryCond.FromCity, queryCond.ToCity,
        //     queryCond.TicketStatus, queryCond.BusinessmanName, queryCond.BusinessmanCode, "", queryCond.OperatorAccount, null,
        //     null, queryCond.StartIssueRefundTime, queryCond.EndIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59), null, null, queryCond.PayWay);
        //    TicketSalesSumEntity tsum = new TicketSalesSumEntity();
        //    tsum.rows = TicketSalesList;
        //    tsum.total = TicketSalesList.Count();
        //    return Json(tsum);

        //}

        /// <summary>
        /// 机票明细
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTicketInformationDetail(TicketDetailSearch queryCond)
        {
            queryCond.endIssueRefundTime = queryCond.endIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            PagedList<ResponseTicket> list = null;
            CommunicateManager.Invoke<IConsoOrderService>(p=>
                    list = p.GetConsoTicketSumDetail(queryCond)
                );
            return Json(new { total = list.Total, rows = list.Rows }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult TicketCountOfBuyer()
        {
            return View();
        }

        public JsonResult GetTicketCount(TicketCountOfBuyer queryCond)
        {
            queryCond.EndTime = queryCond.EndTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            PagedList<ResponseTicketCount> list = null;
            CommunicateManager.Invoke<IConsoOrderService>(p=>
                list = p.GetBuyerTicketCount(queryCond)
                );
            return Json(new {total=list.Total,rows=list.Rows },JsonRequestBehavior.AllowGet);
        }

        #region 导excel------------------------------------------------------

        ///// <summary>
        ///// 汇总导出
        ///// </summary>
        ///// <returns></returns>
        //public FileResult ExportExcelSummary(string OrderId, string PNR, DateTime? StartIssueRefundTime, DateTime? EndIssueRefundTime, string TicketNumber,
        //    string PlatformCode, string PolicyType, string CarrayCode, string FromCity, string ToCity, int? TicketStatus, string BusinessmanCode,
        //    string OperatorAccount, int? PayWay)
        //{
        //    ExportExcelContext export = new ExportExcelContext("Excel2003");
        //    DataTable dt = new DataTable("机票信息汇总报表");
        //    List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
        //    {
                 
        //         new KeyValuePair<string,Type>("机票状态",typeof(string)),
        //         new KeyValuePair<string,Type>("数量",typeof(int)),
        //         new KeyValuePair<string,Type>("票价",typeof(decimal)),
        //         new KeyValuePair<string,Type>("税费",typeof(decimal)),
        //         new KeyValuePair<string,Type>("佣金",typeof(decimal)),
        //         new KeyValuePair<string,Type>("交易金额",typeof(decimal)),
        //         new KeyValuePair<string,Type>("退款金额",typeof(decimal))
        //    };
        //    headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));

        //    _service.GetTicketInfoSummaryList(OrderId, "", PNR, TicketNumber, PlatformCode, PolicyType, CarrayCode,
        //                                     FromCity, ToCity, TicketStatus, "", BusinessmanCode, "", OperatorAccount,
        //                                     null, null, StartIssueRefundTime, EndIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59), null, null, PayWay).ForEach(p =>
        //    {
        //        dt.Rows.Add(
        //            p.TicketStatus,
        //            p.TicketCount,
        //            p.TicketPrice,
        //            p.TaxFee,
        //            p.Commission,
        //            p.ShouldMoney,
        //            p.ShouldRefundMoney
        //            );
        //    });
        //    return File(export.GetMemoryStream(dt), "application/ms-excel",HttpUtility.UrlEncode(string.Format(dt.TableName + ".{0}", export.TypeName),Encoding.UTF8));
        //}

        ///// <summary>
        ///// 机票销售统计导出
        ///// <returns></returns>
        //public FileResult ExportExcelTicketSalesStatistics(string OrderId, string PNR, DateTime? StartIssueRefundTime, DateTime? EndIssueRefundTime, string TicketNumber,
        //   string PlatformCode, string PolicyType, string CarrayCode, string FromCity, string ToCity, int? TicketStatus, string BusinessmanCode,
        //   string OperatorAccount, int? PayWay)
        //{
        //    ExportExcelContext export = new ExportExcelContext("Excel2003");
        //    DataTable dt = new DataTable("机票销售统计报表");
        //    List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
        //    {
        //         new KeyValuePair<string,Type>("出票地",typeof(string)),
        //         new KeyValuePair<string,Type>("出票数量",typeof(int)),
        //         new KeyValuePair<string,Type>("出票票价",typeof(decimal)),
        //         new KeyValuePair<string,Type>("出票税费",typeof(decimal)),
        //         new KeyValuePair<string,Type>("出票佣金",typeof(decimal)),
        //         new KeyValuePair<string,Type>("信用支付",typeof(decimal)),
        //         new KeyValuePair<string,Type>("现金账户",typeof(decimal)),
        //         new KeyValuePair<string,Type>("第三方支付",typeof(decimal)),
        //         new KeyValuePair<string,Type>("出票交易额",typeof(decimal)),
        //         new KeyValuePair<string,Type>("退票数量",typeof(int)),
        //         new KeyValuePair<string,Type>("退票票价",typeof(decimal)),
        //         new KeyValuePair<string,Type>("退票税费",typeof(decimal)),
        //         new KeyValuePair<string,Type>("退票佣金",typeof(decimal)),
        //         new KeyValuePair<string,Type>("退票交易额",typeof(decimal)),
        //         new KeyValuePair<string,Type>("废票数量",typeof(int)),
        //         new KeyValuePair<string,Type>("废票票价",typeof(decimal)),
        //         new KeyValuePair<string,Type>("废票税费",typeof(decimal)),
        //         new KeyValuePair<string,Type>("废票佣金",typeof(decimal)),
        //         new KeyValuePair<string,Type>("废票交易额",typeof(decimal)),
        //    };
        //    headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));

        //    _service.GetTicketSalesStatisticsList(OrderId, "", PNR, TicketNumber, PlatformCode, PolicyType, CarrayCode,
        //                                   FromCity, ToCity, TicketStatus, "", BusinessmanCode, "", OperatorAccount,
        //                                   null, null, StartIssueRefundTime, EndIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59), null, null, PayWay).ForEach(p =>
        //    {
        //        dt.Rows.Add(
        //           p.PolicyCode,
        //           p.IssueTicketCount,
        //           p.IssueTicketPrice,
        //           p.IssueTicketTaxFee,
        //           p.IssueTicketCommission,
        //           p.IssueCreditShouldMoney,
        //           p.IssueAccountShouldMoney,
        //           p.IssuePlatFormShouldMoney,
        //           p.IssueTicketShouldMoney,
        //           p.RefundTicketCount,
        //           p.RefundTicketPrice,
        //           p.RefundTicketTaxFee,
        //           p.RefundTicketCommission,
        //           p.RefundTicketShouldMoney,
        //           p.InvalidTicketCount,
        //           p.InvalidTicketPrice,
        //           p.InvalidTicketTaxFee,
        //           p.InvalidTicketCommission,
        //           p.InvalidTicketShouldMoney
        //            );
        //    });
        //    return File(export.GetMemoryStream(dt), "application/ms-excel",HttpUtility.UrlEncode(string.Format(dt.TableName + ".{0}", export.TypeName),System.Text.Encoding.UTF8));
        //}
        /// <summary>
        /// 机票明细导出(运营)
        /// <returns></returns>
        public FileResult ExportExcelTicketDetail(string OrderId, string PNR, DateTime? StartIssueRefundTime, DateTime? EndIssueRefundTime, string TicketNumber,
           string PlatformCode, string PolicyType, string CarrayCode, string FromCity, string ToCity, string TicketStatus, string BusinessmanCode,
           string OperatorAccount, int? PayWay)
        {
            ExportExcelContext export = new ExportExcelContext("Excel2003");
            DataTable dt = new DataTable("机票明细报表");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
                 new KeyValuePair<string,Type>("商户名称",typeof(string)),
                 new KeyValuePair<string,Type>("政策来源",typeof(string)),
                 new KeyValuePair<string,Type>("订单号",typeof(string)),
                 new KeyValuePair<string,Type>("Pnr编码",typeof(string)),
                 new KeyValuePair<string,Type>("大编码",typeof(string)),
                 new KeyValuePair<string,Type>("票号",typeof(string)),
                 new KeyValuePair<string,Type>("实收金额",typeof(decimal)),
                 new KeyValuePair<string,Type>("机票状态",typeof(string)),
                 new KeyValuePair<string,Type>("(出/退)票时间",typeof(string)),
                 new KeyValuePair<string,Type>("起飞时间",typeof(string)),
                 new KeyValuePair<string,Type>("承运人",typeof(string)),
                 new KeyValuePair<string,Type>("航班号",typeof(string)),
                 new KeyValuePair<string,Type>("舱位",typeof(string)),
                 new KeyValuePair<string,Type>("航程",typeof(string)),
                 new KeyValuePair<string,Type>("乘机人",typeof(string)),
                 new KeyValuePair<string,Type>("舱位价",typeof(decimal)),
                 new KeyValuePair<string,Type>("机建费",typeof(decimal)),
                 new KeyValuePair<string,Type>("燃油费",typeof(decimal)),
                 new KeyValuePair<string,Type>("票面价",typeof(decimal)),
                 new KeyValuePair<string,Type>("政策点数",typeof(decimal)),
                 new KeyValuePair<string,Type>("扣点",typeof(decimal))
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));
            TicketDetailSearch ticketquery = new TicketDetailSearch()
            {
                orderId = OrderId,
                pnr = PNR,
                ticketNumber = TicketNumber,
                platformCode = PlatformCode,
                policyType = PolicyType,
                carrayCode = CarrayCode,
                fromCity = FromCity,
                toCity = ToCity,
                ticketStatus = TicketStatus,
                businessmanCode = BusinessmanCode,
                operatorAccount = OperatorAccount,
                startIssueRefundTime = StartIssueRefundTime,
                endIssueRefundTime = EndIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59),
                PayWay = PayWay,
                page = 1,
                rows = 10000
            };
            PagedList<ResponseTicket> dp =null;
            CommunicateManager.Invoke<IConsoOrderService>(p =>
               dp = p.GetConsoTicketSumDetail(ticketquery)
            );
            dp.Rows.ForEach(p =>
            {
                dt.Rows.Add(
                   p.Code,
                   p.PolicyFrom,
                   p.OrderID,
                   p.PNR,
                   p.BigCode,
                   p.TicketNum,
                   p.Money,
                   p.TicketState,
                   p.CreateDate,
                   p.StartTime,
                   p.CarryCode,
                   p.FlightNum,
                   p.Seat,
                   p.Voyage,
                   p.PassengerName,
                   p.SeatPrice,
                   p.ABFee,
                   p.RQFee,
                   p.PMFee,
                   p.PolicyPoint,
                   p.Point
                    );
            });
            return File(export.GetMemoryStream(dt), "application/ms-excel",HttpUtility.UrlEncode(string.Format(dt.TableName + ".{0}", export.TypeName),System.Text.Encoding.UTF8));

        }

        /// <summary>
        /// 机票明细导出(供应)
        /// <returns></returns>
        public FileResult ExportExcelSupplierTicketDetail(string OrderId, string PNR, DateTime? StartIssueRefundTime, DateTime? EndIssueRefundTime, string TicketNumber,
           string PlatformCode, string PolicyType, string CarrayCode, string FromCity, string ToCity, string TicketStatus, string BusinessmanCode,
           string OperatorAccount, int? PayWay)
        {
            ExportExcelContext export = new ExportExcelContext("Excel2003");
            DataTable dt = new DataTable("机票明细报表");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
                 new KeyValuePair<string,Type>("订单号",typeof(string)),
                 new KeyValuePair<string,Type>("Pnr编码",typeof(string)),
                 new KeyValuePair<string,Type>("大编码",typeof(string)),
                 new KeyValuePair<string,Type>("票号",typeof(string)),
                 new KeyValuePair<string,Type>("实收金额",typeof(decimal)),
                 new KeyValuePair<string,Type>("机票状态",typeof(string)),
                 new KeyValuePair<string,Type>("(出/退)票时间",typeof(string)),
                 new KeyValuePair<string,Type>("起飞时间",typeof(string)),
                 new KeyValuePair<string,Type>("承运人",typeof(string)),
                 new KeyValuePair<string,Type>("航班号",typeof(string)),
                 new KeyValuePair<string,Type>("舱位",typeof(string)),
                 new KeyValuePair<string,Type>("航程",typeof(string)),
                 new KeyValuePair<string,Type>("乘机人",typeof(string)),
                 new KeyValuePair<string,Type>("舱位价",typeof(decimal)),
                 new KeyValuePair<string,Type>("机建费",typeof(decimal)),
                 new KeyValuePair<string,Type>("燃油费",typeof(decimal)),
                 new KeyValuePair<string,Type>("票面价",typeof(decimal)),
                 new KeyValuePair<string,Type>("政策点数",typeof(decimal))
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));
            TicketDetailSearch ticketquery = new TicketDetailSearch()
            {
                orderId = OrderId,
                pnr = PNR,
                ticketNumber = TicketNumber,
                platformCode = PlatformCode,
                policyType = PolicyType,
                carrayCode = CarrayCode,
                fromCity = FromCity,
                toCity = ToCity,
                ticketStatus = TicketStatus,
                businessmanCode = BusinessmanCode,
                operatorAccount = OperatorAccount,
                startIssueRefundTime = StartIssueRefundTime,
                endIssueRefundTime = EndIssueRefundTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59),
                PayWay = PayWay,
                page = 1,
                rows = 10000
            };
            PagedList<ResponseTicket> dp = null;
            CommunicateManager.Invoke<IConsoOrderService>(p =>
               dp = p.GetConsoTicketSumDetail(ticketquery)
            );
            dp.Rows.ForEach(p =>
            {
                dt.Rows.Add(
                   p.OrderID,
                   p.PNR,
                   p.BigCode,
                   p.TicketNum,
                   p.Money,
                   p.TicketState,
                   p.CreateDate,
                   p.StartTime,
                   p.CarryCode,
                   p.FlightNum,
                   p.Seat,
                   p.Voyage,
                   p.PassengerName,
                   p.SeatPrice,
                   p.ABFee,
                   p.RQFee,
                   p.PMFee,
                   p.PolicyPoint
                    );
            });
            return File(export.GetMemoryStream(dt), "application/ms-excel", HttpUtility.UrlEncode(string.Format(dt.TableName + ".{0}", export.TypeName), System.Text.Encoding.UTF8));

        }
        #endregion
    }
}
