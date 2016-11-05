using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers.BTicket
{
    public class TicketOrderController : BaseController
    {
        #region
        //
        // GET: /TicketOrder/
        public ActionResult FindAllOrder()
        {
            var model = new
            {
                searchForm = new AllOrderSearch(),
                urls = new
                {
                    search = "/TicketOrder/FindAllOrderSearch",
                    passengers = "/TicketOrder/GetPassengerInfo",
                    _updatepassengers = "/TicketOrder/UpdatePassenger",
                    _getcoordination = "/TicketOrder/GetOrderCoordination",
                    _addcoordination = "/TicketOrder/AddOrderCoordination"
                }
            };
            return View(model);
        }
        public JsonResult FindAllOrderSearch(AllOrderSearch allOrderSearch,int Page,int Rows)
        {
            PagedList<ResponseConsoOrder> pageList = null;
            CommunicateManager.Invoke<IConsoOrderService>(p => pageList = p.FindConsoAllOrder(allOrderSearch,Page,Rows));
            return Json(new { total = pageList.Total, rows = pageList.Rows }, JsonRequestBehavior.DenyGet);
        }
        public ActionResult FindAllSaleOrder()
        {
            var model = new
            {
                searchForm = new
                {
                    OrderID = string.Empty,
                    PNR = string.Empty,
                    PassengerName = string.Empty,
                    BusinessmanCode = string.Empty,
                    PaySerialNumber = string.Empty,
                    ProcessStatus = string.Empty
                },
                urls = new
                {
                    search = "/TicketOrder/FindAllSaleOrderSearch",
                    _getcoordination = "/TicketOrder/GetSaleOrderCoordination",
                    _addcoordination = "/TicketOrder/AddSaleOrderCoordination"
                }
            };
            return View(model);
        }
        public JsonResult FindAllSaleOrderSearch(AllSaleOrderSearch allSaleOrderSearch)
        {
            PagedList<ResponseConsoSaleOrder> pageList = null;
            CommunicateManager.Invoke<IConsoOrderService>(p => pageList = p.FindConsoAllSaleOrder(allSaleOrderSearch));
            return Json(new { total = pageList.Total, rows = pageList.Rows }, JsonRequestBehavior.DenyGet);
        }


        public ActionResult AllOrderQuery()
        {
            return View();
        }
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="queryCond"></param>
        /// <returns></returns>
        public JsonResult FindAll(TicketOrderQueryEntity queryCond)
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                var _service = cf.CreateChannel();

                List<int> listStatus = new List<int>();

                if (queryCond.OrderStatus.HasValue)
                {
                    listStatus.Add(queryCond.OrderStatus.Value);
                }
                if (queryCond.ToDateTime.HasValue)
                {
                    queryCond.ToDateTime = queryCond.ToDateTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                if (string.IsNullOrEmpty(queryCond.OrderId) && string.IsNullOrEmpty(queryCond.PNR) && string.IsNullOrEmpty(queryCond.PassengerName) && string.IsNullOrEmpty(queryCond.TicketNumber))
                {
                    if (!queryCond.StartCreateTime.HasValue)
                        queryCond.StartCreateTime = DateTime.Now.AddDays(-7);

                    if (queryCond.EndDateTime.HasValue)
                    {
                        queryCond.EndDateTime = queryCond.EndDateTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    else
                    {
                        queryCond.EndDateTime = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                }

                DataPack<OrderDto> dataPack;

                if (listStatus.Count == 0)
                {
                    dataPack = _service.FindCarrierAll(queryCond.PaySerialNumber, queryCond.OrderId, queryCond.PNR, queryCond.PassengerName, queryCond.TicketNumber, queryCond.FromCity, queryCond.ToCity, queryCond.StartDateTime, queryCond.ToDateTime, queryCond.BusinessmanCode, queryCond.StartCreateTime, queryCond.EndDateTime, queryCond.CarrayCode, queryCond.PlatformCode, null, (queryCond.page - 1) * queryCond.rows, queryCond.rows);
                }
                else
                {
                    dataPack = _service.FindCarrierAll(queryCond.PaySerialNumber, queryCond.OrderId, queryCond.PNR, queryCond.PassengerName, queryCond.TicketNumber, queryCond.FromCity, queryCond.ToCity, queryCond.StartDateTime, queryCond.ToDateTime, queryCond.BusinessmanCode, queryCond.StartCreateTime, queryCond.EndDateTime, queryCond.CarrayCode, queryCond.PlatformCode, listStatus.ToArray(), (queryCond.page - 1) * queryCond.rows, queryCond.rows);
                }
                return Json(new
                {
                    total = dataPack.TotalCount,
                    rows = dataPack.List
                });
            }
        }
        public ActionResult GetBuyerInfo(string code)
        {
            ResponseDetailBuyer buyer = null;
            CommunicateManager.Invoke<BPiaoBao.AppServices.ConsoContracts.SystemSetting.IConsoBusinessmanService>(p => buyer = p.GetBuyerInfo(code));
            return PartialView(buyer);
        }
        /// <summary>
        /// 订单详情页面
        /// </summary>
        public ActionResult OrderDetail(string orderId)
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                var _service = cf.CreateChannel();
                OrderDetailDto orderDetail = _service.GetCarrierOrderDetail(orderId);
                orderDetail.OrderStatusStr = GetDesc(((EnumOrderStatus)orderDetail.OrderStatus));
                return View(orderDetail);
            }

        }
        /// <summary>
        /// 售后订单详情页面
        /// </summary>
        public JsonResult AfterSaleOrderDetail(int Id)
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                var _service = cf.CreateChannel();
                //ResponseAfterSaleOrder orderDetail = _service.AfterSaleOrderDetail(Id);
                //orderDetail.OrderStatusStr = GetDesc(((EnumOrderStatus)orderDetail.OrderStatus));
                //return View(orderDetail);
                ResponseAfterSaleOrder after = _service.AfterSaleOrderDetail(Id);
                if (after is ResponseAnnulOrder)
                {
                    return Json(new
                    {
                        Detail = after,
                        Type = 2
                    });
                }
                if (after is ResponseBounceOrder)
                {
                    return Json(new
                    {
                        Detail = after,
                        Type = 1
                    });
                }
                if (after is ResponseChangeOrder)
                {
                    return Json(new
                    {
                        Detail = after,
                        Type = 3
                    });
                }
                if (after is ResponseModifyOrder)
                {
                    return Json(new
                    {
                        Detail = after,
                        Type = 4
                    });
                }
                return Json("");
            }

        }
        /// <summary>
        /// 获取及时pnr信息
        /// </summary>
        /// <param name="businessCode"></param>
        /// <param name="officeNo"></param>
        /// <returns></returns>
        public JsonResult GetNewPnrInfo(string businessCode, string Pnr, string Ydoffice)
        {
            using (var cf = new ChannelFactory<IPidService>(typeof(IPidService).Name))
            {
                var client = cf.CreateChannel();
                var msg = client.GetPnrAndTickeNumInfo(businessCode, Pnr, Ydoffice);
                var rs = msg == null ? "未获取到PNR内容" : msg;
                return Json(rs);
            }

        }
        public string GetDesc(Enum @enum)
        {
            var strValue = @enum.ToString();
            var attributes = @enum.GetType().GetField(strValue).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes == null || attributes.Length == 0 ? strValue : (attributes[0] as DescriptionAttribute).Description;
        }
        public ActionResult GetTFGOrderList()
        {
            return View();
        }

        public JsonResult GetRefund_Abolish_ChangeOrderList(TicketOrderQueryEntity queryCond)
        {
            DataPack<ResponseAfterSaleOrder> dataPack;
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                var _service = cf.CreateChannel();

                EnumAfterSaleOrder? OrderTypeEnum = null;
                EnumTfgProcessStatus? statusEnum = null;
                if (queryCond.ProcessStatus != null)
                    statusEnum = (EnumTfgProcessStatus)queryCond.ProcessStatus;
                if (queryCond.OrderType != null)
                    OrderTypeEnum = (EnumAfterSaleOrder)queryCond.OrderType;

                dataPack = _service.GetCarrierAfterSaleOrder(queryCond.PaySerialNumber, queryCond.page, queryCond.rows, queryCond.PNR, queryCond.BusinessmanCode, queryCond.PlatformCode, queryCond.OrderId, OrderTypeEnum, statusEnum, queryCond.PassengerName);

            }
            if (dataPack == null)
                dataPack = new DataPack<ResponseAfterSaleOrder>();
            return Json(new { total = dataPack.TotalCount, rows = dataPack.List });
        }
        public JsonResult GetRefundDetails(int id)
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                var _service = cf.CreateChannel();
                var refunddetail = _service.RefundDetails(id);
                return Json(refunddetail);
            }
        }
        #endregion

        #region 订单处理
        public ActionResult OrderIndex()
        {
            string lockName = string.Empty;
            if (Request.Cookies["User"] != null)
            {
                lockName = Request.Cookies["User"].Values["account"];
            };  
            var model = new
            {
                searchForm = new
                {
                    orderid = string.Empty,
                    pnr = string.Empty,
                    passengerName = string.Empty,
                    code = string.Empty
                },
                lockName = lockName,
                urls = new
                {
                    search = "/TicketOrder/OrderSearchByPager",
                    _lock = "/TicketOrder/LockOrder",
                    _unlock = "/TicketOrder/UnlockOrder",
                    _compositenum = "/TicketOrder/CompositeNumber",
                    _autocomposite = "/TicketOrder/AutoComposite",
                    _autorefund = "/TicketOrder/AutoRefund",
                    _paystatus = "/TicketOrder/QueryPayStatus",
                    _getcoordination = "/TicketOrder/GetOrderCoordination",
                    _addcoordination = "/TicketOrder/AddOrderCoordination",
                    _exam = "/TicketOrder/OrderExamine",
                    _unexam = "/TicketOrder/UnExamineOrder"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 婴儿订单审核
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="seatPrice"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderExamine(string orderid, decimal seatPrice)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.ExamineBabyOrder(orderid, seatPrice));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 婴儿订单拒绝审核
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UnExamineOrder(string orderid, string remark)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.UnExamine(orderid, remark));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 查询待处理订单
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="pnr"></param>
        /// <param name="passengerName"></param>
        /// <param name="code"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult OrderSearchByPager(string orderid, string pnr, string passengerName, string code, int page, int rows)
        {
            PagedList<ResponseConsoOrder> pagedList = null;
            CommunicateManager.Invoke<IConsoOrderService>(p => pagedList = p.FindOrder(orderid, pnr, passengerName, code, page, rows));
            return Json(new { total = pagedList.Total, rows = pagedList.Rows }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 锁定
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LockOrder(string orderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.LockOrder(orderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UnlockOrder(string orderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.UnLockOrder(orderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 手动出票
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CompositeNumber(string orderid, List<PassengerTicketDto> list, string remark, string PnrCode)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.HandIssueTicket(orderid, list, remark, PnrCode));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 自动复合票号
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AutoComposite(string orderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.AutoCompositeTicket(orderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 拒绝出票自动退款
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AutoRefund(string orderid, string remark)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.AutoRefund(orderid, remark));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 支付状态查询
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QueryPayStatus(string orderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.QueryPayStatus(orderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult UpdateOrder(BPiaoBao.AppServices.StationContracts.StationMap.OrderDataObject order)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.UpdateOrderPay(order));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult UpdatePassenger(string orderid, List<BPiaoBao.AppServices.StationContracts.StationMap.PassengerDataObject> list)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.UpdatePassenger(orderid, list));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetOrderPayInfo(string orderid)
        {
            BPiaoBao.AppServices.StationContracts.StationMap.OrderDataObject model = null;
            CommunicateManager.Invoke<IStationOrderService>(p => model = p.GetOrderPayInfo(orderid));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPassengerInfo(string orderid)
        {
            List<BPiaoBao.AppServices.StationContracts.StationMap.PassengerDataObject> list = null;
            CommunicateManager.Invoke<IStationOrderService>(p => list = p.GetPassengerInfo(orderid));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取订单协调
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public JsonResult GetOrderCoordination(string orderid)
        {
            List<ConsoOrderCoordination> list = null;
            CommunicateManager.Invoke<IConsoOrderService>(p => list = p.GetOrderCoordination(orderid));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加订单协调
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="isCompleted"></param>
        /// <param name="Type"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddOrderCoordination(string orderid, bool isCompleted, string Type, string Content)
        {
            CommunicateManager.Invoke<IConsoOrderService>(p => p.AddOrderCoordination(orderid, isCompleted, Type, Content));
            return Json(null, JsonRequestBehavior.DenyGet);
        }

        //public JsonResult GetCoordination(string orderId)
        //{
        //    BPiaoBao.AppServices.DataContracts.DomesticTicket.CoordinationDto dto = null;
        //    CommunicateManager.Invoke<IStationOrderService>(p => dto = p.GetCoordinationDto(orderId));
        //    return Json(dto.CoordinationLogs, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCoordinationAfterSale(int orderId)
        //{
        //    BPiaoBao.AppServices.DataContracts.DomesticTicket.CoordinationDto dto = null;
        //    CommunicateManager.Invoke<IStationOrderService>(p => dto = p.GetCoordinationAfterSale(orderId));
        //    return Json(dto.CoordinationLogs, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult AddCoordination(string orderId, string type, string coordinationContent, bool isCompleted)
        //{
        //    CommunicateManager.Invoke<IStationOrderService>(p => p.AddCoordinationDto(orderId, type, coordinationContent, isCompleted));
        //    return Json(null, JsonRequestBehavior.DenyGet);
        //}

        //[HttpPost]
        //public JsonResult AddCoordinationAfterSale(int orderId, string type, string coordinationContent, bool isCompleted)
        //{
        //    CommunicateManager.Invoke<IStationOrderService>(p => p.AddCoordination(orderId, type, coordinationContent, isCompleted));
        //    return Json(null, JsonRequestBehavior.DenyGet);
        //}
        #endregion

        #region 售后处理订单
        public ActionResult SaleOrderIndex()
        {
            string lockName = string.Empty;
            if (Request.Cookies["User"] != null)
            {
                lockName = Request.Cookies["User"].Values["account"];
            };
            var model = new
            {
                searchForm = new
                {
                    orderid = string.Empty,
                    pnr = string.Empty,
                    code = string.Empty,
                    passengerName = string.Empty,
                    payNum = string.Empty,
                    status = string.Empty,
                    policyType =  string.Empty,
                    startDate = string.Empty,
                    endDate = string.Empty,
                    afterSaleType = string.Empty,
                    lockAccount = string.Empty
                },
                lockName = lockName,
                urls = new
                {
                    search = "/TicketOrder/SaleOrderBySearch",
                    _lock = "/TicketOrder/LockSaleOrder",
                    _unlock = "/TicketOrder/UnlockSaleOrder",
                    _process = "/TicketOrder/Process",
                    _unprocess = "/TicketOrder/UnProcess",
                    _refund = "/TicketOrder/SaleOrderRefund",
                    _completed = "/TicketOrder/SaleOrderCompleted",
                    _detail = "/TicketOrder/SaleOrderDetail",
                    _qrefund = "/TicketOrder/RefundDetail",
                    _refundstatus = "/TicketOrder/SaleRefundQuery",
                    _sigrefund = "/TicketOrder/SaleSignalRefund",
                    _afterpassenger = "/TicketOrder/GetAfterPassenger",
                    _getcoordination = "/TicketOrder/GetSaleOrderCoordination",
                    _addcoordination = "/TicketOrder/AddSaleOrderCoordination"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 获取售后订单协调
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        public JsonResult GetSaleOrderCoordination(int saleorderid)
        {
            List<ConsoOrderCoordination> list = null;
            CommunicateManager.Invoke<IConsoOrderService>(p => list = p.GetSaleOrderCoordination(saleorderid));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加售后订单协调
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <param name="isCompleted"></param>
        /// <param name="Type"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddSaleOrderCoordination(int saleorderid, bool isCompleted, string Type, string Content)
        {
            CommunicateManager.Invoke<IConsoOrderService>(p => p.AddSaleOrderCoordination(saleorderid, isCompleted, Type, Content));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 售后列表售后
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="pnr"></param>
        /// <param name="code"></param>
        /// <param name="passengerName"></param>
        /// <param name="payNum"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult SaleOrderBySearch(string orderid, string pnr, string code, string passengerName, string payNum, EnumTfgProcessStatus? status, DateTime? startDate, DateTime? endDate, string policyType, string afterSaleType, string lockAccount, int page, int rows)
        {
            PagedList<ResponseConsoSaleOrder> list = null;
            CommunicateManager.Invoke<IConsoOrderService>(p => list = p.FindSaleOrder(orderid, pnr, passengerName, code, payNum, status, startDate, endDate, policyType, afterSaleType, lockAccount, page, rows));
            return Json(new { rows = list.Rows, total = list.Total }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 退款明细
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        public PartialViewResult RefundDetail(int saleorderid)
        {
            ViewBag.SaleOrderId = saleorderid;
            List<BPiaoBao.AppServices.StationContracts.StationMap.ResponseBounLine> list = null;
            CommunicateManager.Invoke<IStationOrderService>(p => list = p.RefundDetails(saleorderid));
            return PartialView(list);
        }
        /// <summary>
        /// 售后退款查询
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <param name="refundid"></param>
        /// <returns></returns>
        [HttpPost]
        public ContentResult SaleRefundQuery(int saleorderid, string refundno)
        {
            string refundStr = string.Empty;
            CommunicateManager.Invoke<IStationOrderService>(p => refundStr = p.RefundQuery(saleorderid, refundno));
            return Content(refundStr);
        }
        /// <summary>
        /// 单笔退款请求
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <param name="refundno"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaleSignalRefund(int saleorderid, string refundno)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.SingleRefund(saleorderid, refundno));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 售后订单详情
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        public PartialViewResult SaleOrderDetail(int saleorderid, int isProcess = -1)
        {
            if (isProcess == -1)
                ViewBag.Process = true;//查看详情
            else
                ViewBag.Process = false;
            ResponseAfterSaleOrder model = null;
            CommunicateManager.Invoke<IStationOrderService>(p => model = p.AfterSaleOrderDetail(saleorderid));
            return PartialView(model);
        }
        /// <summary>
        /// 锁定售后订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LockSaleOrder(int saleorderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.LockAccount(saleorderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 解锁售后订单
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UnlockSaleOrder(int saleorderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.UnlockAccount(saleorderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 处理售后订单
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <param name="dic"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Process(int saleorderid, Dictionary<int, decimal> dic, string remark)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.Process(saleorderid, dic, remark));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 拒绝处理
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UnProcess(int saleorderid, string reason)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.UnProcess(saleorderid, reason));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 售后订单退款
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaleOrderRefund(int saleorderid)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.SaleOrderRefund(saleorderid));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetAfterPassenger(int saleorderid)
        {
            List<BPiaoBao.AppServices.StationContracts.StationMap.AfterPassengerDataObject> list = null;
            CommunicateManager.Invoke<IStationOrderService>(p => list = p.GetAfterPassengerInfo(saleorderid));
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 售后订单完成
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaleOrderCompleted(int saleorderid, Dictionary<int, string> dic)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.Completed(saleorderid, dic));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}
