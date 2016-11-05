using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using NPOI.SS.Formula.Functions;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class BankCardController : BaseController
    {
        //银行卡管理
        // GET: /BankCard/

        public ActionResult Index()
        {
            CashCompanyInfoDto _cashCompanyInfo = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                _cashCompanyInfo = service.GetCompanyInfo();
            });

            var model = new
            {
                searchForm = new
                {
                    Name = string.Empty,
                    BankBranch = string.Empty,
                    CardNo = string.Empty,
                    Owner = string.Empty,

                },
                editForm = new BankCardModel() { Owner = _cashCompanyInfo != null ? _cashCompanyInfo.Contact : "" },

                urls = new
                {
                    search = "/BankCard/QueryBankCards",
                    add = "/BankCard/Add",
                    edit="/BankCard/Edit",
                    deleteBank = "/BankCard/Delete",
                    setDefaultBankCard = "/BankCard/SetDefaultBankCard",
                    getCity = "/BankCard/GetCity"
                },
                otherParas = new
                {
                    Banks = BankData.GetAllBanks(),
                    Provinces = CityData.GetAllState()

                }

            };
            return View(model);

        }

        public JsonResult QueryBankCards()
        {
            List<BankCardDto> list = new List<BankCardDto>();
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                list = service.GetBank();
            });
            List<BankCardModel> banks = new List<BankCardModel>();



            foreach (var cardDto in list)
            {
                var model = new BankCardModel();
                model.BankBranch = cardDto.BankBranch;
                model.BankId = cardDto.BankId;
                model.CardNo = cardDto.CardNo;
                model.City = cardDto.City;
                model.Name = cardDto.Name;
                model.Owner = cardDto.Owner;
                model.Province = cardDto.Province;
                model.IsDefault = cardDto.IsDefault;
                int length = model.CardNo.Length;
                if (length > 4)
                {
                    var start = model.CardNo.Length - 4;
                    model.CardNoShow = model.CardNo.Substring(0, 4) + "****" + model.CardNo.Substring(start);
                }
                else
                {
                    model.CardNoShow = model.CardNo;
                }

                model.NameCardNoShow = model.Name + "  " + model.CardNoShow;
                banks.Add(model);
            }

            IEnumerable<BankCardModel> query = from m in banks 
                                               orderby m.IsDefault descending 
                                               select m;

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(BankCardModel backCardModel)
        {

            var dto = new BankCardDto();
            dto.Name = backCardModel.Name;
            dto.BankBranch = backCardModel.BankBranch.Trim();
            dto.Owner = backCardModel.Owner;
            dto.CardNo = backCardModel.CardNo.Trim();
            dto.Province = backCardModel.Province;
            dto.City = backCardModel.City;

            var msg = new RspMessageModel();

            CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.AddBank(dto);
                msg.Success = 1;
                msg.Message = "添加银行卡成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Edit(BankCardModel backCardModel)
        {

            var dto = new BankCardDto();
            dto.Name = backCardModel.Name;
            dto.BankBranch = backCardModel.BankBranch.Trim();
            dto.Owner = backCardModel.Owner;
            dto.CardNo = backCardModel.CardNo.Trim();
            dto.Province = backCardModel.Province;
            dto.City = backCardModel.City;
            dto.BankId = backCardModel.BankId;
            dto.IsDefault = backCardModel.IsDefault;
            var msg = new RspMessageModel();

            CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.ModifyBank(dto);
                msg.Success = 1;
                msg.Message = "修改银行卡成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Delete(string id)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.RemoveBank(id);
                msg.Success = 1;
                msg.Message = "删除银行卡成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetDefaultBankCard(string id)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.SetDefaultBank(id);
                msg.Success = 1;
                msg.Message = "设为默认银行卡成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCity(string areaName)
        {
            List<CityModel> list = CityData.GetCity(areaName);

            return Json(list, JsonRequestBehavior.AllowGet);
        }



    }
}
