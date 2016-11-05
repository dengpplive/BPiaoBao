using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using PnrAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    /// <summary>
    /// 扣点组设置
    /// </summary>
    public class DeductionController : BaseController
    {
        public ActionResult Index()
        {
            var model = new
            {
                searchForm = new
                {
                    name = string.Empty
                },
                editForm = new RequestDeduction
                {
                    DeductionRules = new List<RequestDeductionRule>()
                },
                urls = new
                {
                    add = "/Deduction/SaveDeduction",
                    search = "/Deduction/GetDeductionByList",
                    deleteurl = "/Deduction/DeleteDeduction",
                    editinfo = "/Deduction/GetDeductionByID",
                    dislist = "/Deduction/GetDistributionInfo",
                    distribution = "/Deduction/Distribution",
                    getair = "/Deduction/GetAllCarrierCode",
                    getdistribution = "/Deduction/GetSelectedDistribution"
                },
                label = GetCarrierLabel()
            };

            return View(model);
        }
        public JsonResult GetSelectedDistribution(int id)
        {
            List<string> list = new List<string>();
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                list = p.GetSelectedDistribution(id);
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCarrierCode()
        {
            var model = new PnrResource().CarrayDictionary.CarrayList.Select(p => new
            {
                AirCode = p.AirCode,
                AirName = p.AirCode + "-" + p.Carry.AirName
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取分配采购上列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDistributionInfo()
        {
            List<SampleListBuyer> list = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                list = p.GetDistributionBuyer();
            });
            if (list == null)
                list = new List<SampleListBuyer>();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 分配采购商
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Distribution(List<SampleBuyer> list)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.DistributionBuyer(list);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDeductionByList(string name, int page = 1, int rows = 10)
        {
            PagedList<ResponseDeduction> list = null;
            CommunicateManager.Invoke<IConsoDeductionGroupService>(p =>
            {
                list = p.FindDeductionByPager(name, page, rows);

            });
            if (list == null)
                list = new PagedList<ResponseDeduction>();
            return Json(new { total = list.Total, rows = list.Rows }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetDeductionByID(int id)
        {
            RequestDeduction model = null;
            CommunicateManager.Invoke<IConsoDeductionGroupService>(p =>
            {
                model = p.GetDeductionByID(id);
            });
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 提交保存组
        /// </summary>
        /// <param name="deduction"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveDeduction(RequestDeduction deduction)
        {
            CommunicateManager.Invoke<IConsoDeductionGroupService>(p =>
            {
                if (deduction.ID == default(int))
                    p.AddDeductionGroup(deduction);
                else
                    p.EditDeductionGroup(deduction);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 删除指定扣点组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteDeduction(int id)
        {
            CommunicateManager.Invoke<IConsoDeductionGroupService>(p =>
            {
                p.DeleteDeduction(id);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public string GetCarrierLabel()
        {
            string label = string.Empty;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                label = p.GetLabel();
            });
            return label;
        }
    }
}
