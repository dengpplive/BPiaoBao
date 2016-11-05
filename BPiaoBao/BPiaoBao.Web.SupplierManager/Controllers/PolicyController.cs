using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using JoveZhao.Framework;
using PnrAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JoveZhao.Framework.Expand;
using BPiaoBao.Common.Enums;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using System.IO;
using System.Data;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class PolicyController : BaseController
    {
        public ActionResult Index()
        {
            var model = new
            {
                searchForm = new SearchPolicy(),
                editForm = new RequestPartPolicy()
                {
                    PassengeDate = new ClientDateLimit(),
                    IssueDate = new ClientDateLimit()
                },
                fullForm = new RequestPolicy()
                {
                    PassengeDate = new ClientDateLimit()
                    {
                        StartTime = DateTime.Now.Date,
                        EndTime = DateTime.Now.AddYears(1).Date
                    },
                    IssueDate = new ClientDateLimit
                    {
                        StartTime = DateTime.Now.Date,
                        EndTime = DateTime.Now.AddYears(1).Date
                    }
                },
                urls = new
                {
                    search = "/Policy/SearchPolicy",
                    batchdelete = "/Policy/BatchDelete",
                    batchreview = "/Policy/BatchReview",
                    batchcancelhangup = "/Policy/BatchCancelHangUp",
                    batchhangup = "/Policy/BatchHangUp",
                    getpartinfo = "/Policy/GetPartEditInfo",
                    getseats = "/Policy/GetSeats",
                    partsave = "/Policy/PartSave",
                    add = "/Policy/CreatePolicy",
                    editinfo = "/Policy/GetEditPolicy"
                }
            };
            return View(model);
        }

        #region 特价政策
        /// <summary>
        /// 特价政策
        /// </summary>
        /// <returns></returns>
        public ActionResult SpeciaIndex()
        {
            var model = new
            {
                searchForm = new SearchSpecialPolicy(),
                fullForm = new RequestSpecialPolicy()
                {
                    PassengeDate = new ClientDateLimit()
                    {
                        StartTime = DateTime.Now.Date,
                        EndTime = DateTime.Now.AddYears(1).Date
                    },
                    IssueDate = new ClientDateLimit
                    {
                        StartTime = DateTime.Now.Date,
                        EndTime = DateTime.Now.AddYears(1).Date
                    }
                },
                urls = new
                {
                    search = "/Policy/SearchSpeciaPolicy",
                    batchdelete = "/Policy/BatchDelete",
                    batchhangup = "/Policy/BatchHangUp",
                    batchreview = "/Policy/BatchReview",
                    batchcancelhangup = "/Policy/BatchCancelHangUp",
                    getpartinfo = "/Policy/GetPartEditInfo",
                    add = "/Policy/CreateSpecialPolicy",
                    editinfo = "/Policy/GetEditPolicyInfo"
                }
            };
            return View(model);
        }
        public JsonResult GetEditPolicyInfo(Guid id)
        {
            ResponseOperPolicy model = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => model = p.EditSpeciaFind(id));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchSpeciaPolicy(SearchSpecialPolicy search, int page, int rows)
        {
            PagedList<ResponseSpecialPolicy> pagedList = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                pagedList = p.FindSpeciaPolicyByPager(search, page, rows) ?? new PagedList<ResponseSpecialPolicy>();
            });
            return Json(new { total = pagedList.Total, rows = pagedList.Rows }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateSpecialPolicy(RequestSpecialPolicy policy)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                if (policy.ID == default(Guid))
                    p.AddSpeaiaPolicy(policy);
                else
                    p.UpdateSpeaiaPolicy(policy);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 政策详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult ShowSpecialDetail(Guid id)
        {
            ResponseOperPolicy fullPolicy = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => fullPolicy = p.FindSpecialPolicyInfo(id));
            return PartialView(fullPolicy);
        }

        #endregion

        /// <summary>
        /// 批量解挂
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchSpecialHangUp(Guid[] ids)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                p.BatchHangUp(ids);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }



        #region 政策对比
        public ActionResult RedujstIndex()
        {
            var model = new
            {
                searchForm = new
                {
                    carrayCode = string.Empty,
                    fromCityCodes = string.Empty,
                    toCityCode = string.Empty,
                    localPolicyType = string.Empty,
                    startDate = string.Empty,
                    endDate = string.Empty,
                    seat = string.Empty
                },
                editForm = new ResponseLocalPolicy(),
                urls = new
                {
                    search = "/Policy/PolicyContrast",
                    adjust = "/Policy/AdjustPoint",
                    info = "/Policy/GetPolicyInfo"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 政策对比
        /// </summary>
        /// <param name="carrayCode">承运人</param>
        /// <param name="fromCityCodes">出发城市</param>
        /// <param name="toCityCode">到达城市</param>
        /// <param name="localPolicyType">政策类型</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="seat">舱位</param>
        /// <param name="page">页数</param>
        /// <param name="rows">一页数量</param>
        /// <returns></returns>
        public JsonResult PolicyContrast(string carrayCode, string fromCityCodes, string toCityCode, string localPolicyType, DateTime? startDate, DateTime? endDate, string seat, int page, int rows)
        {
            PagedList<ResponseLocalNormalPolicy> pagedList = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
                {
                    pagedList = p.PolicyContrast(carrayCode, fromCityCodes, toCityCode, localPolicyType, startDate, endDate, seat, page, rows) ?? new PagedList<ResponseLocalNormalPolicy>();
                });

            return Json(new { total = pagedList.Total, rows = pagedList.Rows }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPolicyInfo(string guid)
        {
            ResponseLocalPolicy pageInfo = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => pageInfo = p.Find(Guid.Parse(guid), 0));
            return Json(pageInfo, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 修改返点
        /// </summary>
        /// <param name="policyId">政策ID</param>
        /// <param name="newPoint">新的返点数</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AdjustPoint(string policyId, decimal newPoint)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                p.AdjustPoint(Guid.Parse(policyId), newPoint);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        } 
        #endregion

        public JsonResult SearchPolicy(SearchPolicy search, int page, int rows)
        {
            PagedList<ResponsePolicy> pagedList = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                pagedList = p.FindPolicyByPager(search, page, rows);
            });
            if (pagedList == null)
                pagedList = new PagedList<ResponsePolicy>();
            return Json(new { total = pagedList.Total, rows = pagedList.Rows }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetIndex()
        {
            return View();
        }
        public JsonResult ttt()
        {
            var list = new PnrAnalysis.PnrResource().CityDictionary.CityList.Select(p => new { Code = p.city.Code, CityName = p.city.Name }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReleasePolicy()
        {
            return View();
        }
        public JsonResult GetEditPolicy(Guid id)
        {
            RequestPolicy model = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => model = p.EditFind(id));
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreatePolicy(RequestNormalPolicy policy)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                if (policy.ID == default(Guid))
                    p.AddLocalPolicy(policy);
                else
                    p.UpdateLocalPolicy(policy);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 导入政策
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportPolicy()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ImportPolicy(HttpPostedFileBase file)
        {
            if (file == null)
                return Json(new { msg = "上传文件为空!" }, JsonRequestBehavior.DenyGet);
            DataTable dt = ImportExcel.RenderFromExcel(file.InputStream);
            if (dt == null || dt.Rows.Count == 0)
                return Json(new { msg = "无数据!" }, JsonRequestBehavior.DenyGet);
            HttpContext.Cache.Insert(System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"], dt, null, DateTime.Now.AddMinutes(20), TimeSpan.Zero);
            HttpContext.Cache[System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"]] = dt;

            return Json(new { msg = dt.Columns.Count>22?"0":"1"}, JsonRequestBehavior.DenyGet);
        }
        public JsonResult PreViewPolicy()
        {
            if (HttpContext.Cache[System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"]] == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            DataTable dt = HttpContext.Cache[System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"]] as DataTable;
            if (dt != null && dt.Rows.Count > 0)
                return Json(new { total = dt.Rows.Count, rows = dt }, JsonRequestBehavior.AllowGet);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Import()
        {
            if (HttpContext.Cache[System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"]] == null)
                throw new CustomException(400, "请选择导入文件");
            DataTable dt = HttpContext.Cache[System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"]] as DataTable;
            List<RequestSpecialPolicy> specialList = new List<RequestSpecialPolicy>();
            List<RequestNormalPolicy> localList = new List<RequestNormalPolicy>();
            foreach (DataRow row in dt.Rows)
            {
                if (!row.Table.Columns.Contains("特价类型") || (row.Table.Columns.Contains("特价类型") & string.IsNullOrEmpty(row["特价类型"].ToString())))
                {

                    RequestNormalPolicy requestPolicy = new RequestNormalPolicy()
                    {
                        ReleaseType = (EnumReleaseType)int.Parse(!string.IsNullOrEmpty(row["发布类型"].ToString()) ? row["发布类型"].ToString() : "0"),
                        TravelType = (EnumTravelType)int.Parse(!string.IsNullOrEmpty(row["行程类型"].ToString())? row["行程类型"].ToString() : "0"),
                        FromCityCodes = !string.IsNullOrEmpty(row["出发城市"].ToString()) ? row["出发城市"].ToString().ToUpper() : string.Empty,
                        ToCityCodes = !string.IsNullOrEmpty(row["到达城市"].ToString()) ? row["到达城市"].ToString().ToUpper() : string.Empty,
                        Low = !string.IsNullOrEmpty(row["是否低开"].ToString()) ? Convert.ToBoolean(int.Parse(row["是否低开"].ToString())) : false,
                        ChangeCode = !string.IsNullOrEmpty(row["是否换编码"].ToString()) ? Convert.ToBoolean(int.Parse(row["是否换编码"].ToString())) : false,
                        Share = !string.IsNullOrEmpty(row["适用共享航班"].ToString()) ? Convert.ToBoolean(int.Parse(row["适用共享航班"].ToString())) : false,
                        WeekLimit = !string.IsNullOrEmpty(row["班期限制"].ToString()) ? row["班期限制"].ToString() : string.Empty,
                        Apply = !string.IsNullOrEmpty(row["适用航班"].ToString()) ? (EnumApply)int.Parse(row["适用航班"].ToString()) : EnumApply.All,
                        ApplyFlights = !string.IsNullOrEmpty(row["航班"].ToString()) ? row["航班"].ToString() : string.Empty,
                        CarrayCode = !string.IsNullOrEmpty(row["航空公司"].ToString()) ? row["航空公司"].ToString().ToUpper() : string.Empty,
                        Seats = !string.IsNullOrEmpty(row["舱位"].ToString()) ? row["舱位"].ToString() : string.Empty,
                        Office = !string.IsNullOrEmpty(row["Office"].ToString()) ? row["Office"].ToString() : string.Empty,
                        LocalPolicyType = !string.IsNullOrEmpty(row["政策类型"].ToString()) ? row["政策类型"].ToString() : string.Empty,
                        LocalPoint = !string.IsNullOrEmpty(row["本地返点"].ToString()) ? Convert.ToDecimal(row["本地返点"]) : default(decimal),
                        Different = !string.IsNullOrEmpty(row["异地返点"].ToString()) ? Convert.ToDecimal(row["异地返点"]) : default(decimal),
                        PassengeDate = new ClientDateLimit
                        {
                            StartTime = !string.IsNullOrEmpty(row["乘机开始日期"].ToString()) ? Convert.ToDateTime(row["乘机开始日期"]) : DateTime.Now.Date,
                            EndTime = !string.IsNullOrEmpty(row["乘机结束日期"].ToString()) ? Convert.ToDateTime(row["乘机结束日期"]) : DateTime.Now.AddYears(1).Date
                        },
                        IssueDate = new ClientDateLimit
                        {
                            StartTime = !string.IsNullOrEmpty(row["出票开始日期"].ToString()) ? Convert.ToDateTime(row["出票开始日期"]) : DateTime.Now.Date,
                            EndTime = !string.IsNullOrEmpty(row["出票结束日期"].ToString()) ? Convert.ToDateTime(row["出票结束日期"]) : DateTime.Now.AddYears(1).Date
                        },
                        IssueTicketWay = (EnumIssueTicketWay)int.Parse(row["出票方式"].ToString() != "" ? row["出票方式"].ToString() : "0"),
                        Remark = !string.IsNullOrEmpty(row["出发城市"].ToString()) ? row["政策备注"].ToString() : string.Empty
                    };
                    localList.Add(requestPolicy);
                }
                else
                {
                    RequestSpecialPolicy requestPolicy = new RequestSpecialPolicy()
                    {
                        ReleaseType = (EnumReleaseType)int.Parse(!string.IsNullOrEmpty(row["发布类型"].ToString()) ? row["发布类型"].ToString() : "0"),
                        TravelType = (EnumTravelType)int.Parse(!string.IsNullOrEmpty(row["行程类型"].ToString()) ? row["行程类型"].ToString() : "0"),
                        FromCityCodes = !string.IsNullOrEmpty(row["出发城市"].ToString()) ? row["出发城市"].ToString().ToUpper() : string.Empty,
                        ToCityCodes = !string.IsNullOrEmpty(row["到达城市"].ToString()) ? row["到达城市"].ToString().ToUpper() : string.Empty,
                        Low = !string.IsNullOrEmpty(row["是否低开"].ToString()) ? Convert.ToBoolean(int.Parse(row["是否低开"].ToString())) : false,
                        ChangeCode = !string.IsNullOrEmpty(row["是否换编码"].ToString()) ? Convert.ToBoolean(int.Parse(row["是否换编码"].ToString())) : false,
                        Share = !string.IsNullOrEmpty(row["适用共享航班"].ToString()) ? Convert.ToBoolean(int.Parse(row["适用共享航班"].ToString())) : false,
                        WeekLimit = !string.IsNullOrEmpty(row["班期限制"].ToString()) ? row["班期限制"].ToString() : string.Empty,
                        Apply = !string.IsNullOrEmpty(row["适用航班"].ToString()) ? (EnumApply)int.Parse(row["适用航班"].ToString()) : EnumApply.All,
                        ApplyFlights = !string.IsNullOrEmpty(row["航班"].ToString()) ? row["航班"].ToString() : string.Empty,
                        CarrayCode = !string.IsNullOrEmpty(row["航空公司"].ToString()) ? row["航空公司"].ToString().ToUpper() : string.Empty,
                        Seats = !string.IsNullOrEmpty(row["舱位"].ToString()) ? row["舱位"].ToString() : string.Empty,
                        Office = !string.IsNullOrEmpty(row["Office"].ToString()) ? row["Office"].ToString() : string.Empty,
                        LocalPolicyType = !string.IsNullOrEmpty(row["政策类型"].ToString()) ? row["政策类型"].ToString() : string.Empty,
                        LocalPoint = !string.IsNullOrEmpty(row["本地返点"].ToString()) ? Convert.ToDecimal(row["本地返点"]) : default(decimal),
                        Different = !string.IsNullOrEmpty(row["异地返点"].ToString()) ? Convert.ToDecimal(row["异地返点"]) : default(decimal),
                        PassengeDate = new ClientDateLimit
                        {
                            StartTime = !string.IsNullOrEmpty(row["乘机开始日期"].ToString()) ? Convert.ToDateTime(row["乘机开始日期"]) : DateTime.Now.Date,
                            EndTime = !string.IsNullOrEmpty(row["乘机结束日期"].ToString()) ? Convert.ToDateTime(row["乘机结束日期"]) : DateTime.Now.AddYears(1).Date
                        },
                        IssueDate = new ClientDateLimit
                        {
                            StartTime = !string.IsNullOrEmpty(row["出票开始日期"].ToString()) ? Convert.ToDateTime(row["出票开始日期"]) : DateTime.Now.Date,
                            EndTime = !string.IsNullOrEmpty(row["出票结束日期"].ToString()) ? Convert.ToDateTime(row["出票结束日期"]) : DateTime.Now.AddYears(1).Date
                        },
                        IssueTicketWay = (EnumIssueTicketWay)int.Parse(!string.IsNullOrEmpty(row["出票方式"].ToString()) ? row["出票方式"].ToString() : "0"),
                        Remark = !string.IsNullOrEmpty(row["政策备注"].ToString()) ? row["政策备注"].ToString() : string.Empty,
                        Type = (FixedOnSaleType)int.Parse(!string.IsNullOrEmpty(row["固定特价类型"].ToString()) ? row["固定特价类型"].ToString() : "0"),
                        FixedSeatPirce = !string.IsNullOrEmpty(row["特价金额"].ToString()) ? Convert.ToDecimal(row["特价金额"]) : default(decimal),
                        SpecialType = (SpeciaType)int.Parse(!string.IsNullOrEmpty(row["特价类型"].ToString()) ? row["特价类型"].ToString() : "0"),
                    };
                    specialList.Add(requestPolicy);
                }
            }
            HttpContext.Cache.Remove(System.Web.HttpContext.Current.Request.Cookies["auth"].Values["token"]);
            if (specialList.Count>0)
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => p.ImportPolicy(specialList));
            if (localList.Count >0)
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => p.ImportPolicy(localList));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">删除ID列表</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchDelete(Guid[] ids)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                p.BatchDelete(ids);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchReview(Guid[] ids)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                p.BatchReview(ids);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 批量挂起
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchCancelHangUp(Guid[] ids)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                p.BatchCancelHangUp(ids);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 批量解挂
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BatchHangUp(Guid[] ids)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                p.BatchHangUp(ids);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 部分修改数据获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetPartEditInfo(Guid id)
        {
            RequestPartPolicy requestPolicy = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                requestPolicy = p.EditPartFind(id);
            });
            return Json(requestPolicy, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 部分修改保存
        /// </summary>
        /// <param name="requestPartPolicy"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult PartSave(RequestPartPolicy requestPartPolicy)
        {
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => p.PartUpdateLocalPolicy(requestPartPolicy));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 根据航空公司二字码获取舱位
        /// </summary>
        /// <param name="carraycode"></param>
        /// <returns></returns>
        public JsonResult GetSeats(string carraycode)
        {
            CabinData cabinData = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p =>
            {
                cabinData = p.GetBaseCabinData(carraycode);
            });
            return Json(cabinData.CabinList, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 政策详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult ShowDetail(Guid id)
        {
            ResponseFullPolicy fullPolicy = null;
            CommunicateManager.Invoke<IConsoLocalPolicyService>(p => fullPolicy = p.Find(id));
            return PartialView(fullPolicy);
        }
        /// <summary>
        /// 政策模版下载
        /// </summary>
        /// <returns></returns>
        public ActionResult TemplateDown()
        {
            return File("~/Content/Template.xls", "application/ms-excel", HttpUtility.UrlEncode("政策模版.xls", System.Text.Encoding.UTF8));
        }
        /// <summary>
        /// 特价政策模板下班
        /// </summary>
        /// <returns></returns>
        public ActionResult SpecialTemplateDown()
        {
            return File("~/Content/SpecialTemplate.xls", "application/ms-excel", HttpUtility.UrlEncode("特价政策模版.xls", System.Text.Encoding.UTF8));
        }

    }
    public class RadioButtonMeta
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string Text { get; set; }
    }
    public class EnumData
    {
        public int EnumValue { get; set; }
        public string EnumText { get; set; }
        public string EnumDescription { get; set; }
    }
}
