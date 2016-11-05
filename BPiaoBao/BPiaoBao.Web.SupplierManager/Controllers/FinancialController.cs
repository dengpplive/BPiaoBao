using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class FinancialController : BaseController
    {
        //理财账户
        // GET: /Finance/

        public ActionResult Index()
        {
            decimal FinancialMoney = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                FinancialMoney = service.GetAccountInfo().FinancialInfo.FinancialMoney;

            });
            var model = new
            {
                searchForm = new
                {

                },
                editForm = new { },

                urls = new
                {
                    search = "/Financial/QueryFinancial",
                    financialLogIndex = "/Financial/FinancialLogIndex",
                    abortFinancial = "/Financial/AbortFinancial",
                    getFinancialMoney="/Financial/GetFinancialMoney",
                    queryExpectProfit = "/Financial/QueryExpectProfit",
                    querySingleProductInfo = "/Financial/QuerySingleProductInfo",
                },
                financialMoney = FinancialMoney

            };
            return View(model);
        }
        /// <summary>
        /// 获取理财产品详情
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public JsonResult QuerySingleProductInfo(string productId)
        {
            var dto = new FinancialProductDto();
            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                dto = p.GetSingleProductInfo(productId);

            });
            return Json(dto, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查看收益
        /// </summary>
        /// <param name="tradeId"></param>
        /// <returns></returns>
        public JsonResult QueryExpectProfit(string tradeId)
        {
            var dto = new ExpectProfitDto();
            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                dto = p.GetExpectProfit(tradeId);

            });
            return Json(dto, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///查询我的理财账户总额
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFinancialMoney()
        {
            decimal FinancialMoney = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                FinancialMoney = service.GetAccountInfo().FinancialInfo.FinancialMoney;

            });
            return Json(FinancialMoney, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询我的理财产品
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryFinancial()
        {
            IList<CurrentFinancialProductDto> FinancialProducts = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                FinancialProducts = service.GetAccountInfo().FinancialInfo.FinancialProducts;

            });
            return Json(FinancialProducts, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 中止理财产品
        /// </summary>
        /// <param name="tradeID"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonResult AbortFinancial(string tradeID, string pwd)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                p.AbortFinancial(tradeID, pwd);
                msg.Success = 1;
                msg.Message = "中止理财产品";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 理财产品日志页面
        /// </summary>
        /// <returns></returns>
        public ActionResult FinancialLogIndex()
        {
            var model = new
            {
                searchForm = new
                {
                    startTime = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"),
                    endTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    outTradeNo=string.Empty
                },
                editForm = new { },

                urls = new
                {
                    search = "/Financial/QueryFinancialLog"

                }

            };
            return View(model);
        }

       /// <summary>
        /// 查询理财产品日志
       /// </summary>
       /// <param name="startTime"></param>
       /// <param name="endTime"></param>
       /// <param name="outTradeNo"></param>
       /// <param name="page"></param>
       /// <param name="rows"></param>
       /// <returns></returns>
        public JsonResult QueryFinancialLog(DateTime? startTime, DateTime? endTime,string outTradeNo, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<FinancialLogDto> dataPack = null;

            CommunicateManager.Invoke<IAccountService>(p =>
            {
                dataPack = p.FindFinancialLog(startTime, endTime, (page - 1) * rows, rows,outTradeNo.Trim());
            });

            if (dataPack != null)
            {
                return Json(new { total = dataPack.TotalCount, rows = dataPack.List }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, rows = new List<FinancialLogDto>() }, JsonRequestBehavior.AllowGet);
        }





    }
}
