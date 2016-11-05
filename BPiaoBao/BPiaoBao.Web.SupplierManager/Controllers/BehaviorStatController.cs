using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class BehaviorStatController : BaseController
    {
        //
        // GET: /BehaviorStat/

        public ActionResult Index()
        {

            var model = new
            {
                searchForm = new BehaviorStatModel
                {

                    StartDateTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    EndDateTime = DateTime.Now.ToString("yyyy-MM-dd")
                },
                urls = new
                {
                    search = "/BehaviorStat/Query",//查询 
                    queryDeatils = "/BehaviorStat/QueryBussinessDeatials",
                    exportexcel = "/BehaviorStat/Export"
                }

            };
            return View(model);
        }


        public JsonResult Query(BehaviorStatModel query)
        {
            RequestQueryBehaviorStatQuery queryCon = new RequestQueryBehaviorStatQuery();
            queryCon.BusinessmanCode = query.BusinessmanCode;
            queryCon.BusinessmanName = query.BusinessmanName;
            queryCon.BusinessmanType = query.BusinessmanType;
            queryCon.StartDateTime = Convert.ToDateTime(query.StartDateTime);
            queryCon.EndDateTime = Convert.ToDateTime(query.EndDateTime);
            queryCon.PageIndex = query.page;
            queryCon.PageSize = query.rows;
            queryCon.Sort = query.sort;
            queryCon.Order = query.order;

            DataPack<ResponseBehaviorStat> dpList = null;
            CommunicateManager.Invoke<IBehaviorStatService>(service =>
            {
                dpList = service.Query(queryCon);

            });
            if (dpList == null)
            {

                return Json(new { total = 0, rows = new List<ResponseBehaviorStat>() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = dpList.TotalCount, rows = dpList.List }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult QueryBussinessDeatials(string code)
        {
            ResponseDetailBuyer rsp = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                rsp = p.GetBuyerInfo(code);


            });
            return Json(rsp, JsonRequestBehavior.AllowGet);

        }


        public FileResult Export(BehaviorStatModel query)
        {
            ExportExcelContext export = new ExportExcelContext("Excel2003");
            DataTable dt = new DataTable("用户行为");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
               
               //  new KeyValuePair<string,Type>("查询日期",typeof(string)),
                 new KeyValuePair<string,Type>("业务经理",typeof(string)),
                // new KeyValuePair<string,Type>("操作员",typeof(string)),
                 new KeyValuePair<string,Type>("商户号",typeof(string)),
                 new KeyValuePair<string,Type>("公司名称",typeof(string)),
                 new KeyValuePair<string,Type>("商户类型",typeof(string)), 
                 new KeyValuePair<string,Type>("登录次数",typeof(int)), 
                 new KeyValuePair<string,Type>("（机票）查询次数",typeof(int)),
                 new KeyValuePair<string,Type>("（机票）导入次数 ",typeof(int)), 
                 new KeyValuePair<string,Type>("（机票）出票量 ",typeof(int)), 
                 new KeyValuePair<string,Type>("（机票）退票量 ",typeof(int)), 
                 new KeyValuePair<string,Type>("（机票）废票量 ",typeof(int)), 
                 new KeyValuePair<string,Type>("（理财）访问次数 ",typeof(int)), 
                 new KeyValuePair<string,Type>("（理财）理财笔数 ",typeof(int)), 
                 new KeyValuePair<string,Type>("（信用）使用笔数 ",typeof(int)), 
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));

            var queryCon = new RequestQueryBehaviorStatQuery();
            queryCon.BusinessmanCode = query.BusinessmanCode;
            queryCon.BusinessmanName = query.BusinessmanName;
            queryCon.BusinessmanType = query.BusinessmanType;
            queryCon.StartDateTime = Convert.ToDateTime(query.StartDateTime);
            queryCon.EndDateTime = Convert.ToDateTime(query.EndDateTime);
            queryCon.PageIndex = 1;
            queryCon.PageSize = 65535; 
            queryCon.Sort = query.sort;
            queryCon.Order = query.order;

            CommunicateManager.Invoke<IBehaviorStatService>(service =>
            {
                service.Query(queryCon).List.ForEach(m =>
                {
                    var businessType = "";
                    if (m.BusinessmanType.ToLower().Equals("buyer"))
                    {
                        businessType = "采购商";
                    }
                    else if (m.BusinessmanType.ToLower().Equals("supplier"))
                    {
                        businessType = "供应商";
                    }
                    else
                    {
                        businessType = "运营商";
                    }
                    dt.Rows.Add(
                        //  m.OpDateTime.ToString("yyyy-MM-dd"),
                        m.ContactName,
                        // m.OperatorName,
                        m.BusinessmanCode,
                        m.BusinessmanName,
                        businessType,
                        m.LoginCount,
                        m.QueryCount,
                        m.ImportCount,
                        m.OutTicketCount,
                        m.BackTicketCount,
                        m.AbolishTicketCount,
                        m.AccessCount,
                        m.FinancingCount,
                        m.UseCount
                        );
                });

            });
            return File(export.GetMemoryStream(dt), "application/ms-excel", HttpUtility.UrlEncode(string.Format("{1}.{0}", export.TypeName, dt.TableName + query.StartDateTime + "至" + query.EndDateTime), Encoding.UTF8));


        }
    }
}
