using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers.SMS
{
    public class SMSController : BaseController
    {
        //
        // GET: /SMS/
        #region 短信模板
        public ActionResult SmsTemplateList()
        {
            return View();
        }
        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public JsonResult GetSmsTemplateList(int page, int rows)
        {
            PagedList<ResponseSmsTemplate> list = null;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                list = p.GetSmsTemplateList((page - 1) * rows, rows);
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据id获取模板信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSmsTemplateById(int id)
        {
            ResponseSmsTemplate rs = null;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                rs = p.GetSmsTemplatebyId(id);
            });
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <returns></returns>
        public JsonResult AddSmsTemplate(SmsTemplateDataObject smsTemplate)
        {
            smsTemplate.State = true;
            smsTemplate.IsSystemTemplate = false;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.AddSmsTemplate(smsTemplate);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteSmsTemplate(int id)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.DeleteSmsTemplate(id);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="smsTemplate"></param>
        /// <returns></returns>
        public JsonResult EditSmsTemplate(SmsTemplateDataObject smsTemplate)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.EditSmsTemplate(smsTemplate);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 禁用启用模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult SmsTemplateEnableOrDisable(int id)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.SmsTemplateEnableOrDisable(id);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        #endregion
        #region 短信记录
        /// <summary>
        /// 购买记录列表
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyDetailList()
        {
            var model = new
            {
                searchForm = new
                {
                    StartTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"),
                    EndTime = DateTime.Now.ToString("yyyy-MM-dd"),
                },
                urls = new
                {
                    search = "/SMS/GetBuyDetail"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 查询购买记录
        /// </summary>
        /// <returns></returns>
        public JsonResult GetBuyDetail(DateTime? StartTime, DateTime? EndTime, int page = 1, int rows = 10)
        {
            PagedList<BuyDetailDataObj> list = null;
            if (EndTime.HasValue)
                EndTime = EndTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                list = p.GetBuyRecordByPage("", (page - 1) * rows, rows, StartTime,EndTime);
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 短信发送列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SendDetailList()
        {
            var model = new
            {
                searchForm = new
                {
                    StartTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"),
                    EndTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    Tel = string.Empty
                },
                urls = new
                {
                    search = "/SMS/GetSendDetail"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 查询发送记录
        /// </summary>
        /// <param name="Tel"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetSendDetail(string Tel, DateTime? StartTime, DateTime? EndTime, int page = 1, int rows = 10)
        {
            PagedList<SendDetailDataObj> list = null;
            if (EndTime.HasValue)
                EndTime = EndTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                list = p.GetSendRecordByPage("",Tel, (page - 1) * rows, rows, StartTime, EndTime);
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 短信赠送
        /// <summary>
        /// 赠送记录
        /// </summary>
        /// <returns></returns>
        public ActionResult GiveDetailList()
        {
            return View();
        }
        /// <summary>
        /// 查询赠送记录
        /// </summary>
        /// <param name="GiveName"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetSmsGiveDetail(string GiveName, DateTime? StartTime, DateTime? EndTime, int page, int rows)
        {
            PagedList<GiveDetailDataObj> list = null;
            if (EndTime.HasValue)
                EndTime = EndTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                list = p.GetGiveDetailByPage(GiveName, (page - 1) * rows, rows, StartTime, EndTime);
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加赠送记录
        /// </summary>
        /// <param name="givedetail"></param>
        /// <returns></returns>
        public JsonResult AddSmsGiveDetail(GiveDetailDataObj givedetail)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.AddSmsGive(givedetail);
            });
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSmsRemainCount()
        {
            var count = "";
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
               count = p.GetSmsRemainCount();
            });
            return Json(count);
        }
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
                string name = string.Empty;
                name = service.GetBusinessmanName(businessCode);
                //var buyer = service.GetBusinessmanBuyerByCode(businessCode, null, null, null, 1).Rows.FirstOrDefault();
                if (!string.IsNullOrEmpty(name))
                {
                    msg.Success = 1;
                    msg.Message = name;
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
        #region 短信费用设置
        public ActionResult SMSChargeSetList()
        {
            return View();
        }
        /// <summary>
        /// 获取短信费用设置列表
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public JsonResult GetSmsChargeSetList(int page, int rows)
        {
            PagedList<SMSChargeSetDataObj> list = null;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                list = p.GetSmsChargeSetByPage((page - 1) * rows, rows);
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据id获取短信费用设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetSmsChargeSetById(int id)
        {
            SMSChargeSetDataObj rs = null;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                rs = p.GetSmsChargeSetById(id);
            });
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加短信费用设置
        /// </summary>
        /// <returns></returns>
        public JsonResult AddSmsChargeSet(SMSChargeSetDataObj SmsChargeset)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.AddSmsChargeSet(SmsChargeset);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 删除短信费用设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteSmsChargeSet(int id)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.DeleteSmsChargeSet(id);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 修改短信费用设置
        /// </summary>
        /// <param name="smsTemplate"></param>
        /// <returns></returns>
        public JsonResult EditSmsChargeSet(SMSChargeSetDataObj SmsChargeset)
        {
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                p.EditSmsChargeSet(SmsChargeset);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        #endregion
        #region 短信设置首页（购买)
        public ActionResult SmsIndex()
        {
            return View();
        }
        /// <summary>
        /// 获取购买短信平台费用设置
        /// </summary>
        /// <param name="PageNo"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public JsonResult GetBuySmsChargeSetList()
        {
            List<SMSChargeSetDataObj> list = null;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                list = p.GetBuySmsChargeSetByPage();
            });
            smsbuy sms = new smsbuy() { 
                smsbuylist = list
            };
            return Json(sms);
        }
        /// <summary>
        /// 购买短信
        /// </summary>
        /// <param name="chargesetid"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult BuySms(int chargesetid, string pwd)
        {
            bool rs = false;
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                rs = p.BuySmsByCashAccount(chargesetid, pwd);
            });
            return Json(rs);
        }

        #endregion
        #region 短信发送
        public ActionResult SMSSend()
        {
            return View();
        }
        public JsonResult Send(SendDetailDataObj sendobj)
        {
            string rs = "发送失败!";
            CommunicateManager.Invoke<IConsoSMSService>(p =>
            {
                rs = p.SmsSend(sendobj);
            });
            return Json(rs);
        }
        #endregion

    }
    public class smsbuy
    {
        public List<SMSChargeSetDataObj> smsbuylist { get; set; }
    }
}
