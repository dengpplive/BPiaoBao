using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.Web.SupplierManager.Models;
using JoveZhao.Framework.Expand;
using PnrAnalysis;
using BPiaoBao.Common;
using BPiaoBao.AppServices;
using System.Xml.Linq;
using BPiaoBao.AppServices.DataContracts.SystemSetting;


namespace BPiaoBao.Web.SupplierManager.Controllers
{

    public class BusinessmanController : BaseController
    {

        public ActionResult BuyerIndexTest()
        {
            var model = new
            {
                searchForm = new
                {
                    code = "fsfs",
                    businessmanName = string.Empty,
                    startTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    endTime = DateTime.Now.AddDays(20)
                },
                editForm = new RequestBuyer
                {
                    ContactWay = new ContactWayDataObject()
                },
                urls = new
                {
                    search = "/Businessman/BuyerList",
                    add = "/Businessman/AddBuyer",
                    edit = "/Businessman/EditBuyer",
                    export = "/Businessman/ExportExcel",
                    upload = "/Businessman/FileUpload"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 采购商列表
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyerIndex()
        {
            var model = new
            {
                searchForm = new
                {
                    code = string.Empty,
                    businessmanName = string.Empty,
                    startTime = string.Empty,
                    endTime = string.Empty
                },
                editForm = new RequestBuyer
                {
                    ContactWay = new ContactWayDataObject()

                },
                urls = new
                {
                    search = "/Businessman/BuyerList",
                    add = "/Businessman/AddBuyer",
                    edit = "/Businessman/EditBuyer",
                    editinfo = "/Businessman/BuyerInfo",
                    exportexcel = "/Businessman/ExportExcel",
                    upload = "/Businessman/FileUpload",
                    setenable = "/Businessman/BusinessmanSetEnable",
                    getlabel = "/Businessman/GetLabel",
                    setlabel = "/Businessman/SetLabel",
                    reset = "/Businessman/ResetPwd"
                }
            };
            return View(model);
        }
        [HttpPost]
        public JsonResult ResetPwd(string code)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.ResetBuyerAdminPwd(code);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult BusinessmanSetEnable(string code, bool isEnable)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.BusinessmanSetEnable(code, isEnable);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult BuyerList(string code, string businessmanName, DateTime? startTime, DateTime? endTime, int page = 1, int rows = 15)
        {
            PagedList<ResponseBuyer> jsonList = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                jsonList = p.GetBusinessmanBuyerByCode(code, businessmanName, startTime, endTime, page, rows);

            });
            return Json(new { total = jsonList.Total, rows = jsonList.Rows }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddBuyer(RequestBuyer buyer)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => p.OpenBuyer(buyer));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult EditBuyer(RequestBuyer buyer)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => p.EditBuyer(buyer));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult BuyerInfo(string code)
        {
            RequestBuyer detail = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                detail = p.GetEditBuyerInfo(code);
            });
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
        //导出excel
        public ActionResult ExportExcel(string code, string name, DateTime? startTime, DateTime? endTime, string exportType)
        {
            ExportExcelContext export = new ExportExcelContext(exportType);
            DataTable dt = new DataTable("商户信息");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
                 new KeyValuePair<string,Type>("商户号",typeof(string)),
                 new KeyValuePair<string,Type>("商户名称",typeof(string)),
                 new KeyValuePair<string,Type>("业务员",typeof(string)),
                 new KeyValuePair<string,Type>("业务员电话",typeof(string)),
                 new KeyValuePair<string,Type>("联系人",typeof(string)),
                 new KeyValuePair<string,Type>("联系电话",typeof(string)),
                 new KeyValuePair<string,Type>("联系地址",typeof(string)),
                 new KeyValuePair<string,Type>("状态",typeof(string)),
                 new KeyValuePair<string,Type>("创建时间",typeof(string))
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.GetBusinessmanBuyerByCode(code, name, startTime, endTime, 1, 1000).Rows.ForEach(n =>
                {
                    dt.Rows.Add(n.Code,
                            n.Name,
                            n.ContactName,
                            n.Phone,
                            n.ContactWay.Contact,
                            n.ContactWay.Tel,
                            n.ContactWay.Address,
                            n.IsEnable ? "正常" : "禁用",
                            n.CreateTime);
                });
            });
            return File(export.GetMemoryStream(dt), "application/ms-excel", HttpUtility.UrlEncode(string.Format("{1}.{0}", export.TypeName, dt.TableName + DateTime.Now.ToString("yyyyMMdd")), System.Text.Encoding.UTF8));
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase Filedata)
        {
            if (Filedata != null)
            {
                try
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileUpload");
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    string fileName = Path.GetFileName(Filedata.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    string saveName = Guid.NewGuid().ToString() + fileExtension;//保存文件名称
                    Filedata.SaveAs(Path.Combine(filePath, saveName));
                    return Json(new { Success = true, Url = Url.Content("~/FileUpload/" + saveName) });
                }
                catch (Exception e)
                {
                    return Json(new { Success = false, Message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = false, Message = "请选择要上传的文件!" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult OperatorIndex()
        {
            var model = new
            {
                searchForm = new
                {
                    account = string.Empty,
                    realName = string.Empty,
                    phone = string.Empty,
                    status = string.Empty,
                    roleId = string.Empty
                },
                editForm = new RequestOperator(),
                urls = new
                {
                    search = "/Businessman/OperatorList",
                    add = "/Businessman/AddConsoOperator",
                    editinfo = "/Businessman/GetOperatorInfo",
                    setenable = "/Businessman/SetEnableStatus",
                    reset = "/Businessman/ResetPassWord",
                    getRoleInfo = "/Businessman/GetRoleInfo",
                    getRoleList = "/Businessman/RoleList"

                } 
            };
            return View(model);
        }
        /// <summary>
        /// 员工密码重置
        /// </summary>
        /// <param name="Id">员工ID</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ResetPassWord(int Id)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.ResetPassWord(Id);
            });
            return Json(null, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorList(string account, string realName, string phone, string status, int? roleId)
        {
            List<ResponseOperator> list = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                list = p.GetOperators(account,realName,phone,status,roleId);
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddConsoOperator(RequestOperator requestOperator)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                if (requestOperator.Id == default(int))
                    p.AddConsoOperator(requestOperator);
                else
                    p.EditConsoOperator(requestOperator);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetOperatorInfo(int id)
        {
            RequestOperator rp = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                rp = p.GetOperatorInfo(id);
            });
            return Json(rp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SetEnableStatus(int id, int status)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.SetEnableStatus(id, status);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }

        //角色
        public ActionResult RoleIndex()
        {
            var model = new
            {
                searchForm = new
                {
                    roleName = string.Empty
                },
                editForm = new RequestRole(),
                urls = new
                {
                    search = "/Businessman/RoleList",
                    add = "/Businessman/AddRole",
                    editinfo = "/Businessman/GetRoleInfo",
                    delectinfo = "/Businessman/DeleteRoleInfo",
                }
            };
            return View(model);
        }


        public JsonResult DeleteRoleInfo(int ID)
        {
            bool result = false;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                result = p.DeleteRole(ID);
            });
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public JsonResult RoleList(string roleName)
        {
            List<ResponseRole> jsonList = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                jsonList = p.GetRole(roleName);
            });
            return Json(jsonList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddRole(RequestRole role)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                if (role.ID == default(int))
                    p.AddRole(role);
                else
                    p.EditRole(role);

            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetRoleInfo(int id)
        {
            RequestRole role = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                role = p.GetRoleInfo(id);
            });
            return Json(role, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 运营商个人信息
        /// </summary>
        /// <returns></returns>
        public ActionResult CarrierIndex()
        {

            CarrierDataObject dataObject = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                dataObject = p.GetCarrierDetail();
            });
            var carrayCodeModel = new PnrResource().CarrayDictionary.CarrayList.Select(p => new
            {
                AirCode = p.AirCode,
                AirName = p.AirCode + "-" + p.Carry.AirName
            }).ToList();
            carrayCodeModel.Insert(0, new { AirCode = "ALL", AirName = "ALL-所有航空公司" });
            return View(new { DataObject = dataObject, CarrayCodeModel = carrayCodeModel });
        }
        [HttpPost]
        public JsonResult SubmitCarrier(CarrierDataObject dataObject)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.SubmitCarrier(dataObject);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public ActionResult GetLabel()
        {
            string label = string.Empty;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                label = p.GetLabel();
            });
            return Content(label);
        }
        [HttpPost]
        public JsonResult SetLabel(SetLabel setLabel)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.SetLabel(setLabel);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }


        public JsonResult UpdatePayPwd(string newPwd, string onceNewPwd, string smsCode)
        {
            var msg = new RspMessageModel();
            if (!newPwd.Equals(onceNewPwd))
            {
                msg.Success = 0;
                msg.Message = "两次输入密码不一致";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            CommunicateManager.Invoke<IAccountService>(p =>
            {
                p.SetPayPassword(newPwd, smsCode);
                msg.Success = 1;
                msg.Message = "设置支付密码成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取短信
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSMSValidate()
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IAccountService>(p =>
            {
                p.GetValidateCode();
                msg.Success = 1;
                msg.Message = "获取短信成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #region 默认政策
        public ActionResult SetDefaultPolicy()
        {
            List<DefaultPolicyDataObject> list = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => list = p.GetDefaultPolicy());
            return View(list);
        }
        [HttpPost]
        public ActionResult SetDefaultPolicy(List<DefaultPolicyDataObject> list)
        {
            if (list != null)
            {
                var result = list.GroupBy(x => x.CarrayCode).Select(p => new { Code = p.Key, Count = p.Count() }).Where(p => p.Count > 1).FirstOrDefault();
                if (result != null)
                    throw new Exception(string.Format("{0}只能添加一个", result.Code));
            }
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => p.SetDefaultPolicy(list));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 供应商操作
        [HttpPost]
        public ActionResult AddSupplier(RequestSupplier requestSupplier)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => p.AddSupplier(requestSupplier));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult UpdateSupplier(RequestSupplier requestSupplier)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => p.UpdateSupplier(requestSupplier));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public JsonResult GetEditInfo(string code)
        {
            RequestSupplier supplier = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => supplier = p.EditFind(code));
            return Json(supplier, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 供应商管理
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierIndex()
        {
            var model = new
            {
                editForm = new RequestSupplier()
                {
                    CarrierSettings = new List<CarrierSettingDataObject>(),
                    ContactWay = new ContactWayDataObject(),
                    Pids = new List<PIDDataObject>()
                },
                searchForm = new
                {
                    code = string.Empty,
                    businessmanName = string.Empty,
                    startTime = string.Empty,
                    endTime = string.Empty
                },
                urls = new
                {
                    search = "/Businessman/SupperlierSearch",
                    add = "/Businessman/AddSupplier",
                    edit = "/Businessman/UpdateSupplier",
                    editinfo = "/Businessman/GetEditInfo",
                    setenable = "/Businessman/BusinessmanSetEnable"
                }
            };
            return View(model);
        }
        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <returns></returns>
        public JsonResult SupperlierSearch(string code, string businessmanName, DateTime? startTime, DateTime? endTime)
        {
            List<ResponseSupplier> list = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                list = p.FindSupplier(code, businessmanName, startTime, endTime);
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult SupplierDetail(string code)
        {
            SupplierDataObject supplierDetail = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => supplierDetail = p.FindSupplierByCode(code));
            return PartialView(supplierDetail);
        }
        #endregion

        #region 自动出票设置
        public ActionResult AutoTicketSet()
        {
            AutoIssueTicketViewModel model = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => model = p.GetAutoIssueTicket());
            return View(model);
        }
        [HttpPost]
        public JsonResult AutoTicketSetSave(AutoIssueTicketViewModel vm)
        {
            CommunicateManager.Invoke<IConsoBusinessmanService>(p => p.AutoIssueTicketSave(vm));
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        #endregion

        /// <summary>
        /// 组织tree-josn数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetJsonDate()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Menu.config";
            XElement root = XElement.Load(path);
            int Share=0;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                Share = p.GetCurrentUser().Type == "Supplier" ? 2 : 1;
            });
            var list = root.Descendants("Module").Select(p => new
            {
                id = int.Parse(p.Attribute("Code").Value),
                text = p.Attribute("Name").Value,
                state = "closed",
                Share = int.Parse(p.Attribute("Share").Value),
                children = p.Elements("Menus").Select(n => new
                {
                    id = int.Parse(n.Attribute("Code").Value),
                    text = n.Attribute("Name").Value,
                    state = "closed",
                    Share = int.Parse(n.Attribute("Share").Value),
                    children = n.Elements("Menu").Select(m => new
                    {
                        id = int.Parse(m.Attribute("Code").Value),
                        text = m.Attribute("Name").Value,
                        Share = int.Parse(m.Attribute("Share").Value)
                    }).Where(x => x.Share == 0 || x.Share == Share).ToList()
                }).Where(x => x.Share == 0 || x.Share == Share).ToList()
            }).Where(x => x.Share == 0 || x.Share == Share).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SetCustomerInfoIndex()
        {
            CustomerDto dataObject = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                dataObject = p.GetConsoCustomerInfo();
            }, e =>
            {
                throw e;
            });

            var urls = new
            {
                SaveUrl = "/Businessman/SetCustomerInfo"
            };
            return View(new { DataObject = dataObject, Urls = urls });
        }
        [HttpPost]
        public JsonResult SetCustomerInfo(CustomerDto dto)
        {
            if (dto.AdvisoryQQ == null) dto.AdvisoryQQ = new List<KeyAndValueDto>();
            if (dto.HotlinePhone == null) dto.HotlinePhone = new List<KeyAndValueDto>();
            for (int i = dto.AdvisoryQQ.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(dto.AdvisoryQQ[i].Key) &&
                string.IsNullOrWhiteSpace(dto.AdvisoryQQ[i].Value))
                {
                    dto.AdvisoryQQ.RemoveAt(i);
                }
            }

            for (int i = dto.HotlinePhone.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrWhiteSpace(dto.HotlinePhone[i].Key) &&
                string.IsNullOrWhiteSpace(dto.HotlinePhone[i].Value))
                {
                    dto.HotlinePhone.RemoveAt(i);
                }
            }
            if (string.IsNullOrWhiteSpace(dto.CustomPhone))
            {
                throw new Exception("必须填写客服服务。");
            }
            if (!dto.CustomPhone.IsMatch(StringExpend.PhonePattern))
            {
                throw new Exception(dto.CustomPhone + "不是合法的电话。");
            }
            if (dto.AdvisoryQQ.Count(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value)) > 0)
                throw new Exception("QQ及QQ描述均必须填写。");
            if (dto.HotlinePhone.Count(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value)) > 0)
                throw new Exception("电话及电话描述均必须填写。");

            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.SetConsoCustomerInfo(dto);
            }, e =>
            {
                throw e;
            });
            return Json("保存成功。");
        }
    }
}
