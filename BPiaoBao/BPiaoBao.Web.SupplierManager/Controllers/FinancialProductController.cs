using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class FinancialProductController : BaseController
    {
        //理财产品
        // GET: /FinancialProduct/

        public ActionResult Index()
        {
            decimal _readyBalance = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                _readyBalance = service.GetAccountInfo().ReadyInfo.ReadyBalance;

            });
            var model = new
            {
                searchForm = new
                {

                },
                editForm = new { },
                urls = new
                {
                    searchOnSale = "/FinancialProduct/QueryOnSale",
                    searchSaleOut = "/FinancialProduct/QuerySaleOut",
                    queryAllFinancialProduct="/FinancialProduct/QueryAllFinancialProduct", 
                    buy = "/FinancialProduct/BuyProduct"
                },
                readyBalance = _readyBalance,
                financialProduct = new FinancialProductModel()
            };
            return View(model);
        }

       
        /// <summary>
        /// 购买理财产品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonResult BuyProduct(FinancialProductModel model)
        {
            var msg = new RspMessageModel();

            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                p.BuyFinancialProductByCashAccount(model.ProductId, model.Money, model.Password);
                msg.Success = 1;
                msg.Message = "购买理财产品成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryAllFinancialProduct()
        {
            IEnumerable<FinancialProductDto> onSales = null;
            IEnumerable<FinancialProductDto> salesOut = null;

            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                var result = p.GetActiveProduct(null);
                onSales = result;
            }); 
            CommunicateManager.Invoke<IFinancialService>(p =>
            {

                var result = p.GetShelfProducts("5");
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        item.MaxAmount = 0;
                        item.CurrentAmount = 0;
                    }
                    salesOut = result;
                }
            });
            return Json(new { Sales = onSales, SalesOut = salesOut }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 可购买的理财产品
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryOnSale()
        {
            IEnumerable<FinancialProductDto> onSales = null;
            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                var result = p.GetActiveProduct(null);
                onSales = result;
            });
            return Json(onSales, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 已满额的理财产品
        /// </summary>
        /// <returns></returns>
        public JsonResult QuerySaleOut()
        {
            IEnumerable<FinancialProductDto> salesOut = null;
            CommunicateManager.Invoke<IFinancialService>(p =>
            {

                var result = p.GetShelfProducts("5");
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        item.MaxAmount = 0;
                        item.CurrentAmount = 0;
                    }
                    salesOut = result;
                }
            });
            return Json(salesOut, JsonRequestBehavior.AllowGet);
        }

    }
}
