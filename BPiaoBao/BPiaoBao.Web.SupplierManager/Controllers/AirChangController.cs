using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class AirChangController : BaseController
    {
        private readonly IStationOrderService _service;
        private static readonly ChannelFactory<IStationOrderService> _cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name);
        public AirChangController()
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                _service = _cf.CreateChannel();
            }
        }
        //
        // GET: /AriChang/
        /// <summary>
        /// 设置航变信息
        /// </summary>
        /// <returns></returns>
        public ActionResult SetAriChangIndex()
        {
            return View();
        }

        public JsonResult TimeOurInfo()
        {
            QTSetting model = null;
            CommunicateManager.Invoke<IMemoryService>(p => model = p.GetAirChangeTimeOutInfo());
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        public JsonResult AirChaneTimeOut(string qtBeginTime, string qtEndTime, string qtTimeOut,bool? IsOpen)
        {
            CommunicateManager.Invoke<IMemoryService>(p => p.SetAirChangTimeOutInfo(qtBeginTime, qtEndTime, qtTimeOut,IsOpen.Value));
            return Json(null, JsonRequestBehavior.DenyGet);
        }



        /// <summary>
        /// 航变信息查询
        /// </summary>
        /// <returns></returns>
        public ActionResult AriChangIndex()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetAirChangeInfoList(DateTime? startDate, DateTime? endDate, string startTime, string endTime, string PNR, string Passenger, bool? statue, int page, int rows)
        {
            BPiaoBao.AppServices.StationContracts.StationMap.PagedList<ResponeAirChange> pagedList = null;
            CommunicateManager.Invoke<IStationOrderService>(p => pagedList = p.GetAirChangeList(startDate, endDate, startTime, endTime, PNR, Passenger, statue, page, rows));
            return Json(new { total = pagedList.Total, rows = pagedList.Rows }, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 查询QT信息
        /// </summary>
        /// <returns></returns>
        public JsonResult QTMessage(int Id)
        {
            ResponseAirQtInfo model = null;
            CommunicateManager.Invoke<IStationOrderService>(p => model = p.GetQtInfo(Id));
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        public JsonResult CreateCoordion(int Id, string content, bool status, EnumAriChangNotifications type)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.CreateAirChangeCoordion(type, status, content, Id));
            return Json(null, JsonRequestBehavior.DenyGet);
        }

        public ViewResult OrderDetail(string orderId)
        {
            using (var cf = new ChannelFactory<IStationOrderService>(typeof(IStationOrderService).Name))
            {
                var _service = cf.CreateChannel();
                OrderDetailDto orderDetail = _service.GetCarrierOrderDetail(orderId);
                orderDetail.OrderStatusStr = GetDesc(((EnumOrderStatus)orderDetail.OrderStatus));
                return View(orderDetail);
            }
        }
        public string GetDesc(Enum @enum)
        {
            var strValue = @enum.ToString();
            var attributes = @enum.GetType().GetField(strValue).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes == null || attributes.Length == 0 ? strValue : (attributes[0] as DescriptionAttribute).Description;
        }
        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MerchantInfo(string Code)
        {
            ResponseBusinessMan model = null;
            CommunicateManager.Invoke<IStationOrderService>(p => model = p.GetBuseInfo(Code));
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        public JsonResult GetAirChangeCoordion(int Id)
        {
            List<AirChangeCoordionDto> list = null;
            CommunicateManager.Invoke<IStationOrderService>(p => list = p.GetAirChangeCoordion(Id) ?? new List<AirChangeCoordionDto>());
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PnrInfo(string pnr, int Id)
        {
            ResponeAirPnrInfo model = null;
            CommunicateManager.Invoke<IStationOrderService>(p => model = p.GetPnrInfo(pnr, Id));
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 查询操作详情
        /// </summary>
        /// <returns></returns>
        public JsonResult OperateDetail(int Id)
        {
            ResponseOperateDetail model = null;
            CommunicateManager.Invoke<IStationOrderService>(p => model = p.GetOperateDetail(Id));
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 协调操作信息保存
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResult SaveOperate(EnumAriChangNotifications type, bool status, string content, int Id)
        {
            CommunicateManager.Invoke<IStationOrderService>(p => p.CreateAirChangeCoordion(type, status, content, Id));
            return Json(null, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 航变信息导出EXCEL表
        /// </summary>
        /// <param name="qtDate"></param>
        /// <param name="qtDateTime"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public FileResult ExportExcelAriChang(DateTime? startDate, DateTime? endDate, string startTime, string endTime, string PNR, string Passenger, bool? statue)
        {
            ExportExcelContext export = new ExportExcelContext("Excel2003");
            DataTable dt = new DataTable("航变信息");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
                 new KeyValuePair<string,Type>("qt日期",typeof(string)),
                 new KeyValuePair<string,Type>("qt时间",typeof(string)),
                 new KeyValuePair<string,Type>("qt条数",typeof(string)),
                 new KeyValuePair<string,Type>("商户信息",typeof(string)),
                 new KeyValuePair<string,Type>("PNR",typeof(string)),
                 new KeyValuePair<string,Type>("订单号",typeof(string)),
                 new KeyValuePair<string,Type>("CTCT",typeof(string)),
                 new KeyValuePair<string,Type>("乘机人",typeof(string)),
                 new KeyValuePair<string,Type>("出票",typeof(string)),
                 new KeyValuePair<string,Type>("通知方式",typeof(string)),
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));

            BPiaoBao.AppServices.StationContracts.StationMap.PagedList<ResponeAirChange> list = _service.GetAirChangeList(startDate, endDate, startTime, endTime, PNR, Passenger, statue, 1, 10000);
            list.Rows.ForEach(
                x =>
                {
                    dt.Rows.Add(
                        x.QTDate.ToString("yyyy-MM-dd"),
                        x.QTDate.ToString("HH:mm"),
                        x.QTCount,
                        x.BusinessmanName,
                        x.PNR,
                        x.OrderId,
                        x.CTCT,
                        string.Join("|", x.PassengerName),
                        x.OfficeNum,
                        ((EnumAriChangNotifications)x.NotifyWay).ToEnumDesc()
                        );
                });
            return File(export.GetMemoryStream(dt), "application/ms-excel", HttpUtility.UrlEncode(string.Format(dt.TableName + ".{0}", export.TypeName), System.Text.Encoding.UTF8));

        }
    }
}
