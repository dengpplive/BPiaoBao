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
    public class IntegralController : BaseController
    {
        //积分账户
        // GET: /Integral/

        public ActionResult Index()
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
                    search = "/Integral/QueryIntegral",
                    exchangeScore = "/Integral/ExchangeScore",
                    getFinancialScore = "/Integral/GetFinancialScore",
                    ScoreConvertLogIndex = "/Integral/ScoreConvertLogIndex"
                },
                financialScore = GetScore()

            };
            return View(model);
        }

        public ActionResult ScoreConvertLogIndex()
        {
            var model = new
            {
                searchForm = new
                {
                    startTime = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"),
                    endTime = DateTime.Now.ToString("yyyy-MM-dd")

                },
                editForm = new { },

                urls = new
                {
                    search = "/Integral/GetScoreConvertLog"
                   
                } 

            };
            return View(model);
        }

        /// <summary>
        /// 积分兑换记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult GetScoreConvertLog(DateTime? startTime, DateTime? endTime, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<ScoreConvertLogDto> dpList = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                dpList = service.GetScoreConvertLog(startTime, endTime, (page - 1) * rows, rows);

            });
            if (dpList != null)
            {
                return Json(new { total = dpList.TotalCount, rows = dpList.List }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, rows = new List<ScoreConvertLogDto>() }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 积分兑换
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public JsonResult ExchangeScore(int score)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.ExchangeSource(score);
                msg.Success = 1;
                msg.Message = "兑换积分操作成功";

            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFinancialScore()
        {
            return Json(GetScore(), JsonRequestBehavior.AllowGet); ;
        }

        private decimal GetScore()
        {
            decimal financialScore = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                financialScore = service.GetAccountInfo().ScoreInfo.FinancialScore;

            });
            return financialScore;
        }
        /// <summary>
        /// 积分账户明细
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult QueryIntegral(DateTime? startTime, DateTime? endTime,string outTradeNo, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<BalanceDetailDto> dpList = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                dpList = service.GetScoreAccountDetails(startTime, endTime, (page - 1) * rows, rows,outTradeNo.Trim());

            });
            if (dpList != null)
            {
                return Json(new { total = dpList.TotalCount, rows = dpList.List }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, rows = new List<BalanceDetailDto>() }, JsonRequestBehavior.AllowGet);
        }

    }
}
