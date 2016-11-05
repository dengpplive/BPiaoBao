using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common.Enums;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class InsuranceController : BaseController
    {
        //
        // GET: /Insurance/
        #region 保险设置主页
        public ActionResult SetIndex()
        {
            bool _isOpenInsuracne = true;//保险是否开启
            decimal _readyBalance = 0;//现金帐户余额
            decimal _sysCurrentLeaveCount = 0;//系统剩余保险数量
            decimal _singlePrice = 0;//保险单价
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                _readyBalance = service.GetAccountInfo().ReadyInfo.ReadyBalance;

            });
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                var m = service.GetCtrlInsurance();
                _isOpenInsuracne = m.IsEnabled;
                _singlePrice = m.SinglePrice;
                foreach (var ins in m.CtrlInsurance)
                {
                    if (ins.IsCurrent)
                    {
                        _sysCurrentLeaveCount = ins.LeaveCount;
                    }
                }

            });
            //var dto = GetInsuranceConfigInfo();
            var model = new
            {
                searchForm = new
               {
                   startTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"),
                   endTime = DateTime.Now.ToString("yyyy-MM-dd"),
                   insuranceAllDepositType = GetAllEnumInsuranceDepositType(),
                   insuranceDepositType = ""
               },
                urls = new
                {
                    getCurrentUserInsuranceConfigInfo = "/Insurance/GetCurrentUserInsuranceConfigInfo",
                    setInsuranceConfig = "/Insurance/SetInsuranceConfig",//设置保险
                    search = "/Insurance/QueryBuyInsuranceLog",//查询保险记录
                    buyInsuracne = "/Insurance/BuyInsurance",//购买保险
                    giveInsuracne = "/Insurance/GiveInsurance",//赠送保险,
                    querySellInsuranceLog = "/Insurance/QuerySellInsuranceLog",//销售记录
                    checkBusinessCode = "/Insurance/CheckBusinessCode"//检测当前商户是否合法
                },
                //editForm = dto ?? new InsuranceConfigData(){ },
                buyInsuranceParams = new
                {
                    sysCurrentLeaveCount = _sysCurrentLeaveCount,
                    singlePrice = _singlePrice,
                    readyBalance = _readyBalance,
                    isOpenInsuracne = _isOpenInsuracne
                }
            };
            return View(model);
        }

        /// <summary>
        /// 得到当前用户保险配置信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCurrentUserInsuranceConfigInfo()
        {
            InsuranceConfigData model = null;
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                model = service.QueryInsuranceConfig();
            });
            return Json(model, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// 设置保险配置
        /// </summary>
        /// <param name="isOpen"></param>
        /// <param name="leaveCount"></param>
        /// <param name="singlePrice"></param>
        /// <returns></returns>
        public JsonResult SetInsuranceConfig(bool isOpen, int leaveCount, string singlePrice)
        {
            var msg = new RspMessageModel();
            try
            {
                CommunicateManager.Invoke<IInsuranceService>(service =>
                {
                    var req = new InsuranceConfigData();
                    decimal price;
                    if (!decimal.TryParse(singlePrice, out price))
                    {
                        throw new Exception("单价过大或者过小，或者不是有效的数值。");
                    }
                    req.IsOpen = isOpen;
                    req.SinglePrice = price;
                    req.LeaveCount = leaveCount;
                    service.SaveInsuranceConfig(req);
                    msg.Success = 1;
                    msg.Message = "保险设置成功";
                }, (p =>
                {
                    msg.Success = 0;
                    msg.Message = p.Message;
                }));
            }catch{}
            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 购买保险

        /// <summary>
        /// 购买保险
        /// </summary>
        /// <param name="buyCount"></param>
        /// <param name="singlePrice"></param>
        /// <param name="totalPrice"></param>
        /// <param name="leaveCount"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult BuyInsurance(int buyCount, decimal singlePrice, decimal totalPrice, int leaveCount, string pwd)
        {
            var msg = new RspMessageModel();

            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                var m = service.GetCtrlInsurance();
                if (m.IsEnabled)
                {

                    service.BuyInsuranceByCashByCarrier(buyCount, pwd);
                    msg.Success = 1;
                    msg.Message = "支付成功";
                }
                else
                {
                    msg.Success = 0;
                    msg.Message = "保险功能未开启";
                }

            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 购买保险记录
        /// <summary>
        /// 购买保险记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult QueryBuyInsuranceLog(DateTime? startTime, DateTime? endTime, string insuranceDepositType, int page = 1, int rows = 10)
        {
            DataPack<ResponseInsuranceDepositLog> data = null;
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                var log = new RequestQueryInsuranceDepositLog();
                log.IsCtrlStationCall = false;
                log.BuyStartTime = startTime;
                log.BuyEndTime = endTime;
                if (string.IsNullOrWhiteSpace(insuranceDepositType))
                {
                    log.InsuranceDepositType = null;
                }
                else
                {
                    log.InsuranceDepositType = EnumHelper.GetInstance<EnumInsuranceDepositType>(insuranceDepositType);
                }
                data = service.QueryInsuranceDepositLog(log, page, rows);
            });
            return Json(data == null ? new { total = 0, rows = new List<ResponseInsuranceDepositLog>() } : new { total = data.TotalCount, rows = data.List }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 赠送保险

        /// <summary>
        /// 赠送保险
        /// </summary>
        /// <param name="businessCode"></param>
        /// <param name="giveCount"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResult GiveInsurance(string businessCode, int giveCount, string remark)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                //todo 调用赠送保险服务接口 
                service.OfferInsuranceToBuyer(new RequestOfferInsuranceToBuyer { BuyerCode = businessCode, OfferCount = giveCount, Remark = remark });
                msg.Success = 1;
                msg.Message = "赠送保险成功";

            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region 销售记录(包括销售和赠送记录)

        /// <summary>
        /// 销售记录(包括销售和赠送记录)
        /// </summary>
        /// <param name="businessCode"></param>
        /// <param name="tradeNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult QuerySellInsuranceLog(string businessCode, string tradeNo, DateTime? startTime,
            DateTime? endTime, int page = 1, int rows = 10)
        {
            DataPack<ResponseInsurancePurchaseByBussinessman> data = null;
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                //调用销售保险服务接口
                data = service.QueryInsurancePurchaseByBussinessman(new RequestQueryInsurancePurchaseByBussinessman { BuyStartTime = startTime, BuyEndTime = endTime, BuyerCode = businessCode, TradeNo = tradeNo, RequestFrom = 1 }, page, rows);
            });
            return Json(data == null ? new { total = 0, rows = new List<ResponseInsurancePurchaseByBussinessman>() } : new { total = data.TotalCount, rows = data.List }, JsonRequestBehavior.AllowGet);
   
        }


        #endregion

        #region 检测商户号是否合法
        /// <summary>
        /// 检测商户号是否合法
        /// </summary>
        /// <param name="businessCode"></param>
        /// <returns></returns>
        public JsonResult CheckBusinessCode(string businessCode)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IConsoBusinessmanService>(service =>
            {
                string name = "";
                //name = service.GetBusinessmanName(businessCode);
                var buyer = service.GetBusinessmanBuyerByCode(businessCode, null, null, null, 1).Rows.FirstOrDefault();
                if (buyer != null)
                {
                    msg.Success = 1;
                    msg.Message = buyer.Name;
                }
                else
                {
                    msg.Success = 0;
                    msg.Message = "分销商不存在。";
                }
            });
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 保险查询主页
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryIndex()
        {
            var startTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            var endTime = DateTime.Now.ToString("yyyy-MM-dd");
            var reqInsurance = new QueryInsurance();
            reqInsurance.FlyStartTime = startTime;
            reqInsurance.FlyEndTime = endTime;
            reqInsurance.InsuranceLimitStartTime = startTime;
            reqInsurance.InsuranceLimitEndTime = endTime;
            reqInsurance.EnumInsuranceStatus = "0";
            reqInsurance.BuyEndTime = endTime;
            reqInsurance.BuyStartTime = startTime;
            reqInsurance.FlightTripFrom = "";
            reqInsurance.FlightTripTo = "";
            reqInsurance.InsuranceCompany = "";
            reqInsurance.InsuranceNo = "";
            reqInsurance.Mobile = "";
            reqInsurance.OrderId = "";
            reqInsurance.PassengerName = "";
            var model = new
            {
                urls = new
                {
                    search = "/Insurance/QueryInsurance"//查询保单信息
                },
                searchForm = reqInsurance,
                otherParas = new
                {
                    InsuranceAllEnumSatus = GetAllInsuranceEnumSatus(),
                    InsuranceAllCompany = GetAllInsuranceCompany(),
                    GetInsuranceManualUrl = "/Insurance/GetInsuranceManual"
                }

            };
            return View(model);
        }


        /// <summary>
        /// 查询保单
        /// </summary>
        /// <param name="insuranceNo"></param>
        /// <param name="orderId"></param>
        /// <param name="passengerName"></param>
        /// <param name="mobile"></param>
        /// <param name="enumInsuranceStatus"></param>
        /// <param name="flightTripFrom"></param>
        /// <param name="flightTripTo"></param>
        /// <param name="flyStartTime"></param>
        /// <param name="flyEndTime"></param>
        /// <param name="buyStartTime"></param>
        /// <param name="buyEndTime"></param>
        /// <param name="insuranceLimitStartTime"></param>
        /// <param name="insuranceLimitEndTime"></param>
        /// <param name="insuranceCompany"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult QueryInsurance(string insuranceNo, string orderId, string passengerName,
            string mobile, string enumInsuranceStatus, string flightTripFrom, string flightTripTo,
            DateTime? flyStartTime, DateTime? flyEndTime, DateTime? buyStartTime, DateTime? buyEndTime,
            DateTime? insuranceLimitStartTime, DateTime? insuranceLimitEndTime, string insuranceCompany, int page = 1,
            int rows = 10)
        {
            var req = new RequestQueryInsurance();
            req.IsCtrlStationCall = false;
            req.InsuranceNo = insuranceNo;
            req.OrderId = orderId;
            req.PassengerName = passengerName;
            req.Mobile = mobile;
            if (string.IsNullOrWhiteSpace(enumInsuranceStatus) || enumInsuranceStatus == "0")
                req.EnumInsuranceStatus = null;
            else
                req.EnumInsuranceStatus = EnumHelper.GetInstance<EnumInsuranceStatus>(enumInsuranceStatus);
            req.FlightTripFrom = flightTripFrom;
            req.FlightTripTo = flightTripTo;
            req.FlyStartTime = flyStartTime;
            req.FlyEndTime = flyEndTime;
            req.BuyStartTime = buyStartTime;
            req.BuyEndTime = buyEndTime;
            req.InsuranceLimitStartTime = insuranceLimitStartTime;
            req.InsuranceLimitEndTime = insuranceLimitEndTime;
            req.InsuranceCompany = insuranceCompany;
            DataPack<ResponseInsurance> data = null;
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                data = service.QueryInsurance(req, page, rows);

            });
            return Json(data == null ? new { total = 0, rows = new List<ResponseInsurance>() } : new { total = data.TotalCount, rows = data.List }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insuranceOrderId"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult GetInsuranceManual(string insuranceOrderId, string recordId)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                //todo 调用赠送保险服务接口 
                service.GetInsuranceNoManual(insuranceOrderId, recordId);
                msg.Success = 1;
                msg.Message = "手动出单成功。";

            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg);
            //return Json(true);
        }

        #region 私有方法
        /// <summary>
        /// 获取当前用户保险设置
        /// </summary>
        /// <returns></returns>
        private InsuranceConfigData GetInsuranceConfigInfo()
        {
            InsuranceConfigData model = null;
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                model = service.QueryInsuranceConfig();
            });
            return model;
        }

        /// <summary>
        /// 获取所有保单状态
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<dynamic, string>> GetAllInsuranceEnumSatus()
        {
            var list = EnumHelper.GetEnumKeyValues(typeof(EnumInsuranceStatus));
            list.Insert(0, new KeyValuePair<dynamic, string>("", "全部"));
            return list;
        }

        /// <summary>
        /// 获取所有运营商批发保险记录
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<dynamic, string>> GetAllEnumInsuranceDepositType()
        {
            var list = EnumHelper.GetEnumKeyValues(typeof(EnumInsuranceDepositType));
            list.Insert(0, new KeyValuePair<dynamic, string>("", "全部"));
            return list;
        }


        /// <summary>
        /// 得到所有保险公司
        /// </summary>
        /// <returns></returns>
        private List<RspKeyValue> GetAllInsuranceCompany()
        {
            List<RspKeyValue> list = null;
            CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                var result = service.GetCtrlInsurance();
                list = result.CtrlInsurance.Select(p => new RspKeyValue()
                {
                    Key = p.Value,
                    Value = p.Value
                }).ToList();
                list.Insert(0, new RspKeyValue() { Key = "", Value = "全部" });

            });
            return list;
        }

        #endregion
    }
}
