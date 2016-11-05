using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using JoveZhao.Framework;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class TravelPaperController : Controller
    {
        #region 行程单分配记录
        // GET: /TravelPaper/GrantTravelPaper
        public ActionResult TravelPaperGrantRecord()
        {
            var model = new
            {
                searchForm = new
                {
                    Name = string.Empty,
                    StartTime = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd"),
                    EndTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    Office = string.Empty
                },
                editForm = new
                {
                    BuyBusinessman = string.Empty,
                    startTripNumber = string.Empty,
                    endTripNumber = string.Empty,
                    tripCount = 0,
                    useOffice = string.Empty,
                    iataCode = string.Empty,
                    ticketCompanyName = string.Empty,
                    TripRemark = string.Empty
                },
                urls = new
                {
                    search = "/TravelPaper/SearchGrantRecordList",
                    add = "/TravelPaper/AddTravelPaperInfo"
                }
            };
            return View(model);
        }
        public JsonResult SearchGrantRecordList(string Name, string Office, DateTime? StartTime, DateTime? EndTime, int page = 1, int rows = 10)
        {
            JsonResult result = null;
            try
            {
                using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
                {
                    var client = cf.CreateChannel();
                    DataPack<TravelGrantRecordDto> datapackList = client.FindTravelRecord(Name, Office, StartTime, EndTime, page, rows, true);
                    result = Json(new { total = datapackList.TotalCount, rows = datapackList.List }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                result = Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return result;
        }
        /// <summary>
        /// 商户分类数据导出
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportStatistics(string BusinessmanCode, string BusinessmanName, string exportType)
        {
            ExportExcelContext export = new ExportExcelContext(exportType);
            DataTable dt = new DataTable("行程单商户分类");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {                
                 new KeyValuePair<string,Type>("商户名称",typeof(string)),
                 new KeyValuePair<string,Type>("商户号",typeof(string)),
                 new KeyValuePair<string,Type>("全部行程单",typeof(string)),
                 new KeyValuePair<string,Type>("已使用",typeof(string)),
                 new KeyValuePair<string,Type>("未使用",typeof(string)),
                 new KeyValuePair<string,Type>("可使用",typeof(string)),
                 new KeyValuePair<string,Type>("已作废",typeof(string))                 
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));
            using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
            {
                var client = cf.CreateChannel();
                TravelPaperStaticsDto travelPaperStaticsDto = client.FindTravelPaperStatistics(BusinessmanCode, BusinessmanName);
                travelPaperStaticsDto.ItemStaticsList.Add(travelPaperStaticsDto.Total);
                travelPaperStaticsDto.ItemStaticsList.ForEach(p =>
                {
                    dt.Rows.Add(
                        p.UseBusinessmanName,
                        p.UseBusinessmanCode,
                        p.TotalCount,
                        p.TotalUse,
                        p.TotalNoUse,
                        p.TotalValidateUse,
                        p.TotalVoid
                        );
                });
            }
            return File(export.GetMemoryStream(dt), "application/ms-excel", HttpUtility.UrlEncode(string.Format("商户分类查询_{0}.{1}", System.DateTime.Now.ToString("yyy-MM-ddHHMMss"), export.TypeName), System.Text.Encoding.UTF8));
        }

        public JsonResult AddTravelPaperInfo(string BuyBusinessman,
            string startTripNumber, string endTripNumber, int tripCount, string useOffice, string iataCode,
            string ticketCompanyName, string TripRemark)
        {
            string BusinessmanCode = string.Empty;
            string BusinessmanName = string.Empty;
            int rowCount = -1;
            string Msg = string.Empty;
            bool State = false;
            try
            {
                using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
                {
                    var client = cf.CreateChannel();
                    rowCount = client.AddTravelPaper(BuyBusinessman, startTripNumber, endTripNumber, useOffice, iataCode, ticketCompanyName, TripRemark);
                    if (rowCount > 0)
                    {
                        State = true;
                        Msg = "分配成功" + rowCount + "条！";
                    }
                    else
                    {
                        Msg = "分配成功0条,行程单号段【" + startTripNumber + "-" + endTripNumber + "】中存在已分配的行程单号！";
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            return Json(new { Msg = Msg, rowCount = rowCount, State = State }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 行程单已分配管理
        // GET: /TravelPaper/FindTravelPaper
        public ActionResult FindTravelPaper()
        {
            var model = new
            {
                searchForm = new
                {
                    BuyBusinessmanCode = string.Empty,
                    BuyBusinessmanName = string.Empty
                },
                editForm = new
                {
                },
                urls = new
                {
                    search = "/TravelPaper/SearchStatistics",
                    exportexcel = "/TravelPaper/ImportStatistics"
                }
            };
            return View(model);
        }
        //采购详情
        public ActionResult CodeDetailList(string BuyBusinessmanCode)
        {
            var model = new
            {
                searchForm = new
                {
                    UseBusinessmanCode = BuyBusinessmanCode,
                    startTripNumber = string.Empty,
                    endTripNumber = string.Empty,
                    startTicketNumber = string.Empty,
                    endTicketNumber = string.Empty,
                    useOffice = string.Empty,
                    TripStatus = -1,
                    startGrantTime = string.Empty,
                    endGrantTime = string.Empty,
                    startCreateTime = string.Empty,
                    endCreateTime = string.Empty,
                    startVoidTime = string.Empty,
                    endVoidTime = string.Empty
                },
                editForm = new
                {
                    BuyBusinessman = string.Empty,
                    startTripNumber = string.Empty,
                    endTripNumber = string.Empty,
                    tripCount = 0,
                    useOffice = string.Empty,
                    iataCode = string.Empty,
                    ticketCompanyName = string.Empty,
                    TripRemark = string.Empty
                },
                urls = new
                {
                    search = "/TravelPaper/GetBuyerTravelDetail?UseBusinessmanCode=" + BuyBusinessmanCode,
                    exportexcel = "/TravelPaper/ImportTravelDetail_BuyerType"
                }
            };
            return View(model);
        }
        //行程单详情
        public JsonResult GetBuyerTravelDetail(string UseBusinessmanCode, string UseBusinessmanName,
            string startTripNumber, string endTripNumber,
            string startTicketNumber, string endTicketNumber, string useOffice, int? TripStatus,
            DateTime? startGrantTime, DateTime? endGrantTime,
            DateTime? startCreateTime, DateTime? endCreateTime,
            DateTime? startVoidTime, DateTime? endVoidTime,
            DateTime? startBlankRecoveryTime, DateTime? endBlankRecoveryTime,
            string PageSource,
            int page = 1, int rows = 10
           )
        {
            using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
            {
                var client = cf.CreateChannel();
                DataPack<TravelPaperDto> datapackList = client.FindTravelPaper(UseBusinessmanCode, UseBusinessmanName, useOffice, startTripNumber, endTripNumber, startTicketNumber, endTicketNumber
               , startCreateTime, endCreateTime, startVoidTime, endVoidTime, startGrantTime, endGrantTime, startBlankRecoveryTime, endBlankRecoveryTime, PageSource, TripStatus, page, rows, true);
                return Json(new { total = datapackList.TotalCount, rows = datapackList.List }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SearchStatistics(string BuyBusinessmanName, string BuyBusinessmanCode)
        {
            using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
            {
                var client = cf.CreateChannel();
                TravelPaperStaticsDto travelPaperStaticsDto = client.FindTravelPaperStatistics(BuyBusinessmanCode, BuyBusinessmanName);
                travelPaperStaticsDto.ItemStaticsList.Add(travelPaperStaticsDto.Total);
                return Json(travelPaperStaticsDto.ItemStaticsList, JsonRequestBehavior.AllowGet);
            }
        }
        //修改Office   
        // [HttpPost]
        public JsonResult UpdateOffice(string useOffice, string Ids)
        {
            int count = -1;
            using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
            {
                var client = cf.CreateChannel();
                if (!string.IsNullOrEmpty(Ids))
                {
                    List<int> SelectIds = new List<int>();
                    string[] strArr = Ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in strArr)
                    {
                        SelectIds.Add(int.Parse(item));
                    }
                    count = client.UpdateOffice(useOffice, SelectIds);
                }
            }
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 批量回收 空白和作废行程单
        /// </summary>
        /// <returns></returns>
        ///        
        // [HttpPost]
        public JsonResult PatchRecovery(string type, string selTripIds)
        {
            int count = -1;
            string Msg = string.Empty;
            try
            {
                using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
                {
                    var client = cf.CreateChannel();
                    List<int> idsList = new List<int>();
                    if (!string.IsNullOrEmpty(selTripIds))
                    {
                        string[] strArry = selTripIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < strArry.Length; i++)
                        {
                            idsList.Add(int.Parse(strArry[i]));
                        }
                        if (idsList.Count > 0)
                        {
                            if (type == "1")//空白回收
                            {
                                count = client.RecoveryBlackTravelPaper(idsList);
                            }
                            else if (type == "2")//作废回收
                            {
                                count = client.RecoveryVoidTravelPaper(idsList);
                            }
                        }
                        else
                        {
                            if (type == "1")//空白回收
                            {
                                Msg = "请选择状态为”空白回收,未分配“的行程单！";
                            }
                            else if (type == "2")//作废回收
                            {
                                Msg = "请选择状态为“已作废,未回收”的行程单！";
                            }
                        }
                    }
                    else
                    {
                        if (type == "1")//空白回收
                        {
                            Msg = "请选择状态为”空白回收,未分配“的行程单！";
                        }
                        else if (type == "2")//作废回收
                        {
                            Msg = "请选择状态为“已作废,未回收”的行程单！";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            return Json(new { count = count, Msg = Msg }, JsonRequestBehavior.AllowGet);
        }


        #endregion






        #region  空白行程单
        //全部
        public ActionResult TravelPaper1()
        {
            var model = new
            {
                searchForm = new
                {
                    UseBusinessmanCode = string.Empty,
                    UseBusinessmanName = string.Empty,
                    startTripNumber = string.Empty,
                    endTripNumber = string.Empty,
                    PageSource = "black",
                    TripStatus = string.Empty,
                    startBlankRecoveryTime = string.Empty,
                    endBlankRecoveryTime = string.Empty,
                    startGrantTime = string.Empty,
                    endGrantTime = string.Empty,
                    useOffice = string.Empty
                },
                editForm = new
                {
                    BuyBusinessman = string.Empty,
                    tripCount = 0,
                    selTripIds = string.Empty,
                    useOffice = string.Empty,
                    iataCode = string.Empty,
                    ticketCompanyName = string.Empty,
                    TripRemark = string.Empty
                },
                urls = new
                {
                    search = "/TravelPaper/GetBuyerTravelDetail?PageSource=black",
                    add = "/TravelPaper/AddBlackTravelPaperInfo",
                    exportexcel = "/TravelPaper/ImportTravelDetail?PageSource=black"
                }
            };
            return View(model);
        }


        /// <summary>
        /// 发放空白行程单
        /// </summary>
        /// <returns></returns>
        public JsonResult AddBlackTravelPaperInfo(string BuyBusinessman, string useOffice, string iataCode,
            string ticketCompanyName, string TripRemark, string selTripIds

            )
        {
            string BusinessmanCode = string.Empty;
            string BusinessmanName = string.Empty;
            int rowCount = -1;
            string Msg = string.Empty;
            bool State = false;
            try
            {
                List<int> selIds = new List<int>();
                if (!string.IsNullOrEmpty(selTripIds))
                {
                    string[] strArr = selTripIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in strArr)
                    {
                        selIds.Add(int.Parse(item));
                    }
                    if (selIds.Count > 0)
                    {
                        using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
                        {
                            var client = cf.CreateChannel();
                            rowCount = client.GrantBlankRecoveryTravelPaper(BuyBusinessman, useOffice, iataCode, ticketCompanyName, TripRemark, selIds);
                            if (rowCount > 0)
                            {
                                State = true;
                                Msg = "发放空白行程单,成功" + rowCount + "条！";
                            }
                            else
                            {
                                Msg = "发放空白行程单,成功0条！";
                            }
                        }
                    }
                    else
                    {
                        Msg = "请选择发放的行程单号！";
                    }
                }
                else
                {
                    Msg = "请选择发放的行程单号！";
                }
            }
            catch (Exception ex)
            {
                Msg = ex.Message;
            }
            return Json(new { Msg = Msg, rowCount = rowCount, State = State }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region  已分配行程单综合查询
        public ActionResult TravelPaper2()
        {
            var model = new
            {
                searchForm = new
                {
                    UseBusinessmanCode = string.Empty,
                    UseBusinessmanName = string.Empty,
                    startTripNumber = string.Empty,
                    endTripNumber = string.Empty,
                    startTicketNumber = string.Empty,
                    endTicketNumber = string.Empty,
                    TripStatus = 0,
                    startCreateTime = string.Empty,
                    endCreateTime = string.Empty,
                    startGrantTime = string.Empty,
                    endGrantTime = string.Empty
                },
                editForm = new
                {
                    BuyBusinessman = string.Empty,
                    tripCount = 0,
                    selTripIds = string.Empty,
                    useOffice = string.Empty,
                    iataCode = string.Empty,
                    ticketCompanyName = string.Empty,
                    TripRemark = string.Empty
                },
                urls = new
                {
                    search = "/TravelPaper/GetBuyerTravelDetail",
                    add = "/TravelPaper/AddBlackTravelPaperInfo",
                    exportexcel = "/TravelPaper/ImportTravelDetail_All"
                }
            };
            return View(model);
        }

        public ActionResult ImportTravelDetail_BuyerType(string UseBusinessmanCode, string UseBusinessmanName,
            string startTripNumber, string endTripNumber,
            string startTicketNumber, string endTicketNumber, string useOffice, int? TripStatus,
             DateTime? startGrantTime, DateTime? endGrantTime,
            DateTime? startCreateTime, DateTime? endCreateTime,
            DateTime? startVoidTime, DateTime? endVoidTime,
           string exportType, int page = 1, int rows = 10)
        {
            return ImportTravelDetail(UseBusinessmanCode, UseBusinessmanName,
              startTripNumber, endTripNumber,
              startTicketNumber, endTicketNumber, useOffice, TripStatus,
              startGrantTime, endGrantTime,
              startCreateTime, endCreateTime,
              startVoidTime, endVoidTime,
             exportType, "0", page, rows);
        }
        public ActionResult ImportTravelDetail_All(string UseBusinessmanCode, string UseBusinessmanName,
            string startTripNumber, string endTripNumber,
            string startTicketNumber, string endTicketNumber, string useOffice, int? TripStatus,
             DateTime? startGrantTime, DateTime? endGrantTime,
            DateTime? startCreateTime, DateTime? endCreateTime,
            DateTime? startVoidTime, DateTime? endVoidTime,
           string exportType, int page = 1, int rows = 10)
        {
            return ImportTravelDetail(UseBusinessmanCode, UseBusinessmanName,
              startTripNumber, endTripNumber,
              startTicketNumber, endTicketNumber, useOffice, TripStatus,
              startGrantTime, endGrantTime,
              startCreateTime, endCreateTime,
              startVoidTime, endVoidTime,
             exportType, "1", page, rows);
        }
        /// <summary>
        /// 空白行程单详情导出  空白行程单管理
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportTravelDetail(string UseBusinessmanCode, string UseBusinessmanName,
            string startTripNumber, string endTripNumber,
            string startTicketNumber, string endTicketNumber, string useOffice, int? TripStatus,
             DateTime? startGrantTime, DateTime? endGrantTime,
            DateTime? startCreateTime, DateTime? endCreateTime,
            DateTime? startVoidTime, DateTime? endVoidTime,
           string exportType, string BuyerType, int page = 1, int rows = 10)
        {
            DateTime defaultTime = DateTime.Parse("1900-01-01");
            ExportExcelContext export = new ExportExcelContext(exportType);
            DataTable table = new DataTable();
            if (string.IsNullOrEmpty(BuyerType))
            {
                table.TableName = "空白行程单管理";
                List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
                {                
                     new KeyValuePair<string,Type>("商户名称",typeof(string)),
                     new KeyValuePair<string,Type>("商户号",typeof(string)),
                     new KeyValuePair<string,Type>("Office",typeof(string)),
                     new KeyValuePair<string,Type>("行程单号",typeof(string)),
                     new KeyValuePair<string,Type>("行程单状态",typeof(string)),
                     new KeyValuePair<string,Type>("回收时间",typeof(string)),
                     new KeyValuePair<string,Type>("分配时间",typeof(string))                 
                };
                headArray.ForEach(p => table.Columns.Add(p.Key, p.Value));
            }
            else if (BuyerType == "0")// 采购详情
            {
                table.TableName = "行程单详情";
                List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
                {                
                     new KeyValuePair<string,Type>("领用商户名",typeof(string)),
                     new KeyValuePair<string,Type>("行程单号",typeof(string)),
                     new KeyValuePair<string,Type>("Office号",typeof(string)),
                     new KeyValuePair<string,Type>("票号",typeof(string)),
                     new KeyValuePair<string,Type>("状态",typeof(string)),
                     new KeyValuePair<string,Type>("分配时间",typeof(string)),
                     new KeyValuePair<string,Type>("回收时间",typeof(string))  ,               
                     new KeyValuePair<string,Type>("创建打印时间",typeof(string)) ,
                     new KeyValuePair<string,Type>("作废时间",typeof(string)) ,
                     new KeyValuePair<string,Type>("备注",typeof(string))
                };
                headArray.ForEach(p => table.Columns.Add(p.Key, p.Value));
            }
            else if (BuyerType == "1")//行程单综合查询
            {
                table.TableName = "行程单综合查询";
                List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
                {                
                     new KeyValuePair<string,Type>("商户名",typeof(string)),
                     new KeyValuePair<string,Type>("商户号",typeof(string)),
                     new KeyValuePair<string,Type>("Office",typeof(string)),
                     new KeyValuePair<string,Type>("票号",typeof(string)),
                     new KeyValuePair<string,Type>("行程单号",typeof(string)),
                     new KeyValuePair<string,Type>("状态",typeof(string)),
                     new KeyValuePair<string,Type>("回收时间",typeof(string))  ,   
                     new KeyValuePair<string,Type>("分配时间",typeof(string)),                              
                     new KeyValuePair<string,Type>("创建时间",typeof(string)) ,
                     new KeyValuePair<string,Type>("作废时间",typeof(string))                     
                };
                headArray.ForEach(p => table.Columns.Add(p.Key, p.Value));

            }
            using (ChannelFactory<ITravelPaperService> cf = new ChannelFactory<ITravelPaperService>(typeof(ITravelPaperService).Name))
            {
                var client = cf.CreateChannel();
                DataPack<TravelPaperDto> datapackList = client.FindTravelPaper(UseBusinessmanCode, UseBusinessmanName, useOffice, startTripNumber, endTripNumber, startTicketNumber, endTicketNumber
                    , startCreateTime, endCreateTime, startVoidTime, endVoidTime, startGrantTime, endGrantTime, null, null, (string.IsNullOrEmpty(BuyerType) ? "black" : ""), TripStatus, page, rows, false);
                List<TravelPaperDto> TravelPaperList = datapackList.List;
                if (string.IsNullOrEmpty(BuyerType))
                {
                    TravelPaperList.ForEach(p =>
                    {
                        table.Rows.Add(
                         p.BusinessmanName,
                         p.BusinessmanCode,
                         p.UseOffice,
                         p.TripNumber,
                         EnumItemManager.GetDesc(p.TripStatus),
                         p.BlankRecoveryTime > defaultTime ? p.BlankRecoveryTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                         p.GrantTime > defaultTime ? p.GrantTime.ToString("yyyy-MM-dd HH:mm:ss") : ""
                     );
                    });
                }
                else if (BuyerType == "0")// 采购详情
                {
                    TravelPaperList.ForEach(p =>
                    {
                        table.Rows.Add(
                        p.BusinessmanName,
                        p.TripNumber,
                        p.UseOffice,
                        p.TicketNumber,
                        EnumItemManager.GetDesc(p.TripStatus),
                        p.GrantTime > defaultTime ? p.GrantTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.BlankRecoveryTime > defaultTime ? p.BlankRecoveryTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.PrintTime > defaultTime ? p.PrintTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.InvalidTime > defaultTime ? p.InvalidTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.UseTripRemark
                     );
                    });
                }
                else if (BuyerType == "1")// 综合查询
                {
                    TravelPaperList.ForEach(p =>
                    {
                        table.Rows.Add(
                        p.BusinessmanName,
                        p.BusinessmanCode,
                        p.UseOffice,
                        p.TicketNumber,
                        p.TripNumber,
                        EnumItemManager.GetDesc(p.TripStatus),
                        p.BlankRecoveryTime > defaultTime ? p.BlankRecoveryTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.GrantTime > defaultTime ? p.GrantTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.PrintTime > defaultTime ? p.PrintTime.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        p.InvalidTime > defaultTime ? p.InvalidTime.ToString("yyyy-MM-dd HH:mm:ss") : ""
                     );
                    });
                }
            }
            return File(export.GetMemoryStream(table), "application/ms-excel", HttpUtility.UrlEncode(string.Format("{0}_{1}.{2}", table.TableName, System.DateTime.Now.ToString("yyy-MM-ddHHMMss"), export.TypeName), System.Text.Encoding.UTF8));
        }
        #endregion
        /// <summary>
        /// 获取行程单详情
        /// </summary>
        /// <param name="TripNumber"></param>
        /// <returns></returns>
        public JsonResult GetTripNumberDetail(string TripNumber)
        {
            TravelPaperDto travelPaperDto = null;
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<ITravelPaperService>(service =>
            {
                travelPaperDto = service.QueryTripNumberDetail(TripNumber);
                msg.Success = 1;
                msg.Message = "成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(new { model = travelPaperDto, msg = msg }, JsonRequestBehavior.AllowGet);
        }

    }
}
