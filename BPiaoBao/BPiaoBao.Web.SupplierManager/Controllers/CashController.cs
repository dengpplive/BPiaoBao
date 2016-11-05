using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Cashbag.Domain.Models;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using JoveZhao.Framework;
using NPOI.SS.Formula.Functions;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class CashController : BaseController
    {
        //现金账户
        // GET: /Cash/

        public ActionResult Index()
        {

            var model = new
            {
                searchForm = new
                {
                    startTime = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"),
                    endTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    outTradeNo = string.Empty


                },
                editForm = new { },
                urls = new
                {
                    search = "/Cash/QueryCash",//查询
                    getOverage = "/Cash/GetOverage",//查询余额
                    deposit = "/Cash/Deposit",//充值
                    transfer = "/Cash/Transfer",//转账
                    //  depositLog = "/Cash/DepositLog",//充值记录
                    withdraw = "/Cash/Withdraw",//提现  
                    //  withdrawLog = "/Cash/WithdrawLog"//提现记录
                    exportexcel = "/Cash/ExportCashExcel"//导出
                },
                readyAccount = new ReadyAccountDto()

            };
            return View(model);
        }



        /// <summary>
        /// 查询现金账户
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param> 
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public JsonResult QueryCash(DateTime? startTime, DateTime? endTime, string outTradeNo, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<BalanceDetailDto> dpList = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {

                dpList = service.GetReadyAccountDetails(startTime, endTime, outTradeNo.Trim(), null, (page - 1) * rows,
                    rows);
            });
            if (dpList == null)
            {

                return Json(new { total = 0, rows = new List<BalanceDetailDto>() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = dpList.TotalCount, rows = dpList.List }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取现金账户信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetOverage()
        {
            ReadyAccountDto readyAccount = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                readyAccount = service.GetAccountInfo().ReadyInfo;

            });
            return Json(readyAccount, JsonRequestBehavior.AllowGet);
        }

        #region 充值

        /// <summary>
        /// 获取可用金额
        /// </summary>
        /// <returns></returns>
        public JsonResult GetReadyBalance()
        {
            decimal _readyBalance = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                _readyBalance = service.GetAccountInfo().ReadyInfo.ReadyBalance;

            });

            return Json(_readyBalance, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 充值页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Deposit()
        {
            decimal _readyBalance = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                _readyBalance = service.GetAccountInfo().ReadyInfo.ReadyBalance;

            });
            var model = new
            {
                searchForm = new { },
                editForm = new { },
                urls = new
                {
                    depositLog = "/Cash/DepositLog",//充值页面记录URL
                    rechargeByBank = "/Cash/RechargeByBank",//银行卡充值
                    rechargeByPlatform = "/Cash/RechargeByPlatform",//第三方平台充值
                    getReadyBalance = "/Cash/GetReadyBalance"//获取可用金额

                },
                readyBalance = _readyBalance,
                banks = BankData.GetAllBanks()

            };
            return View(model);
        }

        /// <summary>
        /// 银行卡充值
        /// </summary>
        /// <param name="money"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult RechargeByBank(decimal money, string code)
        {
            string postUrl = "";
            CommunicateManager.Invoke<IFundService>(service =>
            {
                postUrl = service.RechargeByBank(money, code);
            });
            return Json(postUrl, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 第三方平台充值
        /// </summary>
        /// <param name="money"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public JsonResult RechargeByPlatform(decimal money, string code)
        {
            string postUrl = "";
            CommunicateManager.Invoke<IFundService>(service =>
            {
                postUrl = service.RechargeByPlatform(money, code);
            });
            return Json(postUrl, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 充值记录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DepositLog()
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
                    startTime = DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"),
                    endTime = DateTime.Now.ToString("yyyy-MM-dd"),
                    outTradeNo = string.Empty

                },
                editForm = new { },
                urls = new
                {
                    search = "/Cash/QueryDepositLog",//查询充值记录URL 

                },
                readyBalance = _readyBalance

            };
            return View(model);
        }

        /// <summary>
        /// 查询充值记录
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryDepositLog(DateTime? startTime, DateTime? endTime, string outTradeNo, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<RechargeLogDto> dpList = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                dpList = service.FindRechargeLog(startTime, endTime, (page - 1) * rows, rows, outTradeNo.Trim());

            });
            if (dpList != null)
            {
                return Json(new { total = dpList.TotalCount, rows = dpList.List }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, rows = new List<RechargeLogDto>() }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 结算
        /// <summary>
        ///结算页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Withdraw()
        {
            decimal _readyBalance = 0;

            CashCompanyInfoDto _cashCompanyInfo = null;
            FeeRuleInfoDto dto = null;
            CommunicateManager.Invoke<IFundService>(p => dto = p.GetFeeRule());
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                _readyBalance = service.GetAccountInfo().ReadyInfo.ReadyBalance;
                _cashCompanyInfo = service.GetCompanyInfo();

            });


            var model = new
            {
                searchForm = new
                {
                },
                editForm = new BankCardModel()
                {
                    Owner = _cashCompanyInfo != null ? _cashCompanyInfo.Contact : ""
                },
                feeRuleInfo = dto,
                urls = new
                {
                    addBankCard = "/BankCard/Add",
                    getCity = "/BankCard/GetCity",
                    queryBankCards = "/BankCard/QueryBankCards",

                    withdrawLog = "/Cash/WithdrawLog",//结算记录页面URL 
                    cashOut = "/Cash/CashOut",//结算
                    getAvailableMoney = "/Cash/GetAvailableMoney",//获取最高结算金额
                    getFeeAmount = "/Cash/GetFeeAmount"//获取手续费
                },
                cashOutParas = new
                {
                    money = 0,
                    bankId = string.Empty,
                    password = string.Empty,
                    isNextDayToAccount = dto.MorrowEnable ? "1" : (dto.TodayEnable ? "0" : "")//次日到账

                },
                otherParas = new
                {
                    Banks = BankData.GetAllBanks(),
                    Provinces = CityData.GetAllState(),

                },
                readyBalance = _readyBalance,
                bankCards = new List<BankCardDto>()

            };
            return View(model);
        }

        /// <summary>
        /// 获取最高结算金额
        /// </summary>
        /// <param name="isNextDayToAccout"></param>
        /// <returns></returns>
        public JsonResult GetAvailableMoney(string isNextDayToAccout)
        {
            decimal availableMoney = 0;//最高结算金额
            CommunicateManager.Invoke<IFundService>(service =>
            {
                availableMoney = decimal.Parse(service.GetApplicationMaxAmount(isNextDayToAccout));
            });
            return Json(availableMoney, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取手续费
        /// </summary>
        /// <param name="money"></param>
        /// <param name="isNextDayToAccout"></param>
        /// <returns></returns>
        public JsonResult GetFeeAmount(decimal money, string isNextDayToAccout)
        {
            decimal feeAmount = 0;//手续费
            CommunicateManager.Invoke<IFundService>(service =>
            {
                feeAmount = decimal.Parse(service.GetFeeAmount(money.ToString(), isNextDayToAccout));
            });
            return Json(feeAmount, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 结算操作
        /// </summary>
        /// <param name="money"></param>
        /// <param name="bankId"></param>
        /// <param name="password"></param>
        /// <param name="isNextDayToAccount"></param>
        /// <returns></returns>
        public JsonResult CashOut(decimal money, string bankId, string password, string isNextDayToAccount)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IFundService>(service =>
            {
                service.CashOut(money, bankId, password, isNextDayToAccount);
                msg.Success = 1;
                msg.Message = "申请结算成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 结算记录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WithdrawLog()
        {
            DateTime? _startTime = Convert.ToDateTime(DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"));
            DateTime? _endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

            var model = new
            {
                searchForm = new
                {
                    startTime = _startTime.Value.ToString("yyyy-MM-dd"),
                    endTime = _endTime.Value.ToString("yyyy-MM-dd"),
                    outTradeNo = string.Empty
                },
                editForm = new { },
                urls = new
                {
                    search = "/Cash/QueryWithdrawLog",//查询结算记录URL
                    getWithdrawMoneyJson = "/Cash/GetWithdrawMoneyJson"
                },
                withdrawMoney = GetWithdrawMoney(_startTime, _endTime)

            };
            return View(model);
        }

        /// <summary>
        /// 得到时间内的结算金额
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private decimal GetWithdrawMoney(DateTime? startTime, DateTime? endTime)
        {
            decimal withdrawMoney = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var result = service.FindCashOutLog(startTime, endTime, 0, 5000);
                if (result != null && result.TotalCount > 0)
                {
                    foreach (var m in result.List)
                    {
                        withdrawMoney += m.CashOutMoney;
                    }
                }
            });
            return withdrawMoney;
        }

        public JsonResult GetWithdrawMoneyJson(DateTime? startTime, DateTime? endTime)
        {
            var withdrawMoney = GetWithdrawMoney(startTime, endTime);
            return Json(withdrawMoney, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询结算记录
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryWithdrawLog(DateTime? startTime, DateTime? endTime, string outTradeNo, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<CashOutLogDto> dpList = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                dpList = service.FindCashOutLog(startTime, endTime, (page - 1) * rows, rows, outTradeNo.Trim());

            });
            if (dpList != null)
            {
                var list = new List<CashOutLogModel>();
                foreach (var m in dpList.List)
                {
                    var model = new CashOutLogModel();
                    model.BankNo = m.BankNo;
                    model.CashOutMoney = m.CashOutMoney;
                    model.CashOutStatus = m.CashOutStatus;
                    model.CashOutTime = m.CashOutTime;
                    model.ClearDateTime = m.ClearDateTime;
                    model.FeeAmount = m.FeeAmount;
                    model.SerialNum = m.SerialNum;
                    model.OutTradeNo = m.OutTradeNo;
                    int length = model.BankNo.Length;
                    if (length > 4)
                    {
                        var start = model.BankNo.Length - 4;
                        model.BankCardNoShowStar = model.BankNo.Substring(0, 4) + "****" + model.BankNo.Substring(start);
                    }

                    list.Add(model);
                }
                return Json(new { total = dpList.TotalCount, rows = list }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, rows = new List<CashOutLogModel>() }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public FileResult ExportCashExcel(DateTime? startTime, DateTime? endTime, string outTradeNo, string orderNo)
        {
            ExportExcelContext export = new ExportExcelContext("Excel2003");
            DataTable dt = new DataTable("账户明细");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
               
                 //new KeyValuePair<string,Type>("流水号",typeof(string)),
                 new KeyValuePair<string,Type>("交易号",typeof(string)),
                 new KeyValuePair<string,Type>("日期",typeof(DateTime)),
                 new KeyValuePair<string,Type>("收支(元)",typeof(decimal)), 
                 //new KeyValuePair<string,Type>("支付方式 ",typeof(string)), 
                 new KeyValuePair<string,Type>("账户余额(元) ",typeof(decimal)),
                 new KeyValuePair<string,Type>("备注 ",typeof(string)), 
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                endTime = endTime != null ? endTime.Value.AddDays(1).AddSeconds(-1) : endTime;
                service.GetReadyAccountDetails(startTime, endTime, outTradeNo, orderNo, 0, 65535).List.ForEach(m =>
                {
                    dt.Rows.Add(
                        // m.SerialNum, 
                    m.OutTradeNo,
                    m.CreateAmount,
                    m.Amount,
                        //m.PayType,
                    m.LeaveAmount,
                    m.OperationType
                     );
                });
            });

            return File(export.GetMemoryStream(dt), "application/ms-excel", HttpUtility.UrlEncode(string.Format("{1}.{0}", export.TypeName, dt.TableName + DateTime.Now.ToString("yyyyMMdd")), Encoding.UTF8));
        }

        #endregion

        #region 转账
        /// <summary>
        ///转账页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Transfer()
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
                urls = new
                {
                    transferLog = "/Cash/TransferLog",//转账记录页面URL 
                    transferMoney = "/Cash/TransferMoney",//转账
                    getName = "/Cash/GetTargetAccountName",//获取商户姓名
                },
                tranferParas = new
                {
                    money = 0,
                    code = string.Empty,
                    password = string.Empty,
                    notes = string.Empty,
                },
                readyBalance = _readyBalance,
            };
            return View(model);
        }
        /// <summary>
        /// 转账操作
        /// </summary>
        /// <param name="code"></param>
        /// <param name="money"></param>
        /// <param name="password"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public JsonResult TransferMoney(string code, string money, string password, string notes)
        {
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IFundService>(service =>
            {
                service.InnerTransfer(code, money, password, notes);
                msg.Success = 1;
                msg.Message = "转账成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 转账记录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult TransferLog()
        {
            DateTime? _startTime = Convert.ToDateTime(DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"));
            DateTime? _endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

            var model = new
            {
                searchForm = new
                {
                    startTime = _startTime.Value.ToString("yyyy-MM-dd"),
                    endTime = _endTime.Value.ToString("yyyy-MM-dd"),
                    outTradeNo = string.Empty
                },
                editForm = new { },
                urls = new
                {
                    search = "/Cash/QueryTransferLog",//查询转账记录URL
                    getTransferMoneyJson = "/Cash/GetTransferMoneyJson"
                },
                transferMoney = GetTransferMoney(_startTime, _endTime)

            };
            return View(model);
        }
        /// <summary>
        /// 得到时间内的转账金额
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private decimal GetTransferMoney(DateTime? startTime, DateTime? endTime)
        {
            decimal transferMoney = 0;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var result = service.FindTransferAccountsLog(startTime, endTime, 0, 5000);
                if (result != null && result.TotalCount > 0)
                {
                    foreach (var m in result.List)
                    {
                        transferMoney += m.TransferAccountsMoney;
                    }
                }
            });
            return transferMoney;
        }
        public JsonResult GetTransferMoneyJson(DateTime? startTime, DateTime? endTime)
        {
            var transferMoney = GetTransferMoney(startTime, endTime);
            return Json(transferMoney, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询转账记录
        /// </summary>
        /// <returns></returns>
        public JsonResult QueryTransferLog(DateTime? startTime, DateTime? endTime, string outTradeNo, int page = 1, int rows = 10)
        {
            endTime = endTime.Value.AddDays(1).AddSeconds(-1);
            DataPack<TransferAccountsLogDto> dpList = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                dpList = service.FindTransferAccountsLog(startTime, endTime, (page - 1) * rows, rows, outTradeNo.Trim());
            });
            if (dpList != null)
            {
                var list = new List<TransferAccountsLogDto>();
                foreach (var m in dpList.List)
                {
                    var model = new TransferAccountsLogDto();
                    model.InComeOrExpenses = m.InComeOrExpenses;
                    model.OutTradeNo = m.OutTradeNo;
                    model.SerialNum = m.SerialNum;
                    model.TargetAccount = m.TargetAccount;
                    model.TransferAccountsMoney = m.TransferAccountsMoney;
                    model.TransferAccountsStatus = m.TransferAccountsStatus;
                    model.TransferAccountsTime = m.TransferAccountsTime;
                    model.TransferAccountsType = m.TransferAccountsType;
                    model.Type = m.Type;
                    list.Add(model);
                }
                return Json(new { total = dpList.TotalCount, rows = list }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { total = 0, rows = new List<TransferAccountsLogDto>() }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 获取商户姓名
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string QueryTargetAccountName(string code)
        {
            string name = string.Empty;
            CommunicateManager.Invoke<IFundService>(service =>
            {
                name = service.GetTargetAccountName(code);
            });
            return name;
        }
        public JsonResult GetTargetAccountName(string code)
        {
            var targetAccountName = QueryTargetAccountName(code);
            return Json(targetAccountName, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
