using System;
using System.Collections.Generic;
using System.Globalization;
using BPiaoBao.Cashbag.Domain.Models;
using BPiaoBao.Common;
using Newtonsoft.Json.Linq;
using BPiaoBao.Cashbag.Domain.Services;
namespace BPiaoBao.Cashbag.ClientProxy
{
    public class AccountClientProxy : IAccountClientProxy
    {

        private readonly string _webUrLfunds = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "History/";
        private readonly string _webUrlbank = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "Bank/";
        private readonly string _webUrlBusinesst = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "BusinessMan/";
        private readonly string _webUrlAccount = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "Account/";
        private readonly string _webUrl = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "funds/";
        private readonly string _webUrlBill = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "Bill/";

        #region 记录日志
        public Tuple<IEnumerable<RechargeLog>, int> GetRechargeLogs(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var ch = new CashbagHelper(_webUrLfunds + "Recharge", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lists = new List<RechargeLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                var rl = new RechargeLog
                {
                    SerialNum = item.SerialNum,
                    RechargeTime = item.RechargeTime,
                    TypeName = item.TypeName,
                    RechargeMoney = item.RechargeMoney,
                    CashSource = item.CashSource,
                    RechargeStatus = item.RechargeStatus,
                    OutTradeNo = item.OutTradeNo
                };
                lists.Add(rl);
            }
            var tuple = new Tuple<IEnumerable<RechargeLog>, int>(lists, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<CashOutLog>, int> GetCashOutLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var ch = new CashbagHelper(_webUrLfunds + "Withdraw", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lists = new List<CashOutLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                var rl = new CashOutLog
                {
                    SerialNum = item.SerialNum,
                    CashOutTime = item.CashOutTime,
                    CashOutMoney = item.CashOutMoney,
                    CashOutStatus = item.CashOutStatus,
                    ClearDateTime = item.ClearDateTime,
                    BankNo = item.AccountName,
                    FeeAmount = item.FeeAmount,
                    OutTradeNo = item.OutTradeNo,
                    ReceivingType = item.ReceivingType
                };
                lists.Add(rl);
            }
            var tuple = new Tuple<IEnumerable<CashOutLog>, int>(lists, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<TransferLog>, int> GetTransferAccountsLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var ch = new CashbagHelper(_webUrLfunds + "Transfer", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<TransferLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new TransferLog
                {
                    SerialNum = item.SerialNum,
                    TransferAccountsMoney = item.TransferAccountsMoney,
                    TransferAccountsStatus = item.TransferAccountsStatus,
                    TransferAccountsTime = item.TransferAccountsTime,
                    TransferAccountsType = item.TransferAccountsType,
                    TransferNotes = item.TransferNotes,
                    TargetAccount = item.TargetAccount,
                    Type = item.Type,
                    OutTradeNo = item.OutTradeNo
                });
            }
            var tuple = new Tuple<IEnumerable<TransferLog>, int>(lst, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<FinancialLog>, int> GetFinancialLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var ch = new CashbagHelper(_webUrLfunds + "Financing", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lists = new List<FinancialLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                var rl = new FinancialLog
                {
                    SerialNum = item.SerialNum,
                    BuyTime = item.BuyTime,
                    ProductName = item.ProductName,
                    FinancialMoney = item.FinancialMoney,
                    FinancialLogStatus = item.FinancialLogStatus,
                    CashSource = item.CashSource,
                    AbortTime = item.EndDateTime == null ? DateTime.Parse("0001-01-01 00:00:00") : item.EndDateTime,
                    StartDateTime = item.StartDateTime,
                    PointAmount = item.PointAmount,
                    PreEndDateTime = item.PreEndDateTime == null ? DateTime.Parse("0001-01-01 00:00:00") : item.PreEndDateTime,
                    AuditDate = item.AuditDate,
                    OutTradeNo = item.OutTradeNo,
                    Remark = item.Notes

                };
                lists.Add(rl);
            }
            var tuple = new Tuple<IEnumerable<FinancialLog>, int>(lists, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<RepaymentLog>, int> GetRepaymentLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var ch = new CashbagHelper(_webUrLfunds + "Repayment", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lists = new List<RepaymentLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                var rl = new RepaymentLog
                {
                    SerialNum = item.SerialNum,
                    RepaymentTime = item.RepaymentTime,
                    RepaymentMoney = item.Amount,
                    CashSource = item.CashSource,
                    RepaymentStatus = item.RepaymentStatus,
                    ShouldAmount = item.ShouldAmount,
                    BillAmount = item.BillAmount,
                    BillTime = item.BillTime,
                    TotalAmount = item.TotalAmount,
                    OutTradeNo = item.OutTradeNo
                };
                lists.Add(rl);
            }
            var tuple = new Tuple<IEnumerable<RepaymentLog>, int>(lists, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<BargainLog>, int> GetBargainLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var ch = new CashbagHelper(_webUrLfunds + "Bargain", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<BargainLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BargainLog
                {
                    CashSource = item.CashSource,
                    Money = item.Money ?? 0,
                    Status = item.Status,
                    SerialNum = item.SerialNum,
                    CreateTime = item.Time,
                    OrderId = item.outOrderNo,
                    BargainTime = item.PayTime,
                    OutTradeNo = item.OutTradeNo,
                    TradeType = item.TradeType,
                    Notes = item.Notes

                });
            }
            var tuple = new Tuple<IEnumerable<BargainLog>, int>(lst, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<ScoreConvertLog>, int> GetScoreConvertLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var ch = new CashbagHelper(_webUrLfunds + "ExchangePoint", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<ScoreConvertLog>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new ScoreConvertLog
                {
                    CreateDate = item.CreateDate,
                    LeaveAmount = item.LeaveAmount,
                    PointAmount = item.PointAmount
                });
            }
            var tuple = new Tuple<IEnumerable<ScoreConvertLog>, int>(lst, totalcount);
            return tuple;
        }
        #endregion

        #region 明细

        public Tuple<IEnumerable<BalanceDetail>, int> GetReadyAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, string outTradeNo,
            string orderNo, int startIndex, int count)
        {
            var ch = new CashbagHelper(_webUrlAccount + "Cash", "GET");
            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<BalanceDetail>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BalanceDetail
                {
                    SerialNum = item.OrderNo,
                    Amount = item.Amount,
                    CreateAmount = item.CreateDate,
                    LeaveAmount = item.LeaveAmount,
                    OperationType = item.OperationType,
                    PayType = item.PayType,
                    OutOrderNo = item.OutOrderNo,
                    OutTradeNo = item.OutTradeNo,
                    Remark = item.Notes
                });
            }
            var tuple = new Tuple<IEnumerable<BalanceDetail>, int>(lst, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<BalanceDetail>, int> GetCreditAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var ch = new CashbagHelper(_webUrlAccount + "Credit", "GET");

            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<BalanceDetail>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BalanceDetail
                {
                    SerialNum = item.OrderNo,
                    Amount = item.Amount,
                    CreateAmount = item.CreateDate,
                    LeaveAmount = item.LeaveAmount,
                    OperationType = item.OperationType,
                    PayType = item.PayType,
                    OutOrderNo = item.OutOrderNo,
                    OutTradeNo = item.OutTradeNo
                });
            }
            var tuple = new Tuple<IEnumerable<BalanceDetail>, int>(lst, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<BalanceDetail>, int> GetScoreAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var ch = new CashbagHelper(_webUrlAccount + "Point", "GET");

            var data = ch.GetURLEncodeData(code, key, startTime, endTime, startIndex, count, outTradeNo);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<BalanceDetail>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BalanceDetail
                {
                    SerialNum = item.OrderNo,
                    Amount = item.Amount,
                    CreateAmount = item.CreateDate,
                    OperationType = item.OperationType,
                    PayType = item.PayType,
                    LeaveAmount = item.LeaveAmount,
                    OutTradeNo = item.OutTradeNo
                });
            }
            var tuple = new Tuple<IEnumerable<BalanceDetail>, int>(lst, totalcount);
            return tuple;
        }
        #endregion

        #region 账单
        public Tuple<IEnumerable<BillList>, int> GetBill(string code, string key, DateTime? startTime, DateTime? endTime, string status, int startIndex, int count)
        {
            var ch = new CashbagHelper(_webUrlBill + "GetBill", "GET");
            var dictionary = new Dictionary<string, string> { { "code", code }, { "key", key } };
            if (startTime.HasValue)
            {
                dictionary.Add("StartDate", startTime.Value.ToString("yyyyMMddHHmmss"));
            }
            if (endTime.HasValue)
            {
                dictionary.Add("EndDate", endTime.Value.ToString("yyyyMMddHHmmss"));
            }
            if (!string.IsNullOrEmpty(status))
            {
                dictionary.Add("Status", status);
            }
            var page = Math.Ceiling((double)startIndex / count) + 1;
            dictionary.Add("CurrentPage", page.ToString(CultureInfo.InvariantCulture));
            dictionary.Add("PageSize", count.ToString(CultureInfo.InvariantCulture));
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lst = new List<BillList>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BillList
                {
                    Amount = item.Amount,
                    CreateDate = item.CreateDate,
                    LateAmount = item.LateAmount,
                    FeeAmount = item.FeeAmount,
                    ShouldRepayAmount = item.ShouldRepayAmount,
                    Status = item.Status,
                    RepayAmount = item.RepayAmount,
                    BillAmount = item.BillAmount,
                    ShouldRepayDate = item.ShouldRepayDate,
                    CreditDayStr = item.CreditDayStr
                });
            }
            var tuple = new Tuple<IEnumerable<BillList>, int>(lst, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<BillDetail>, int> GetBillDetail(string code, string key, DateTime? startTime, DateTime? endTime, string payNo, string amountMin, string amountMax, int startIndex, int count, string outTradeNo)
        {
            var ch = new CashbagHelper(_webUrlBill + "GetBillDetail", "GET");
            var dictionary = new Dictionary<string, string> { { "code", code }, { "key", key } };
            if (startTime.HasValue)
                dictionary.Add("StartDate", startTime.Value.ToString("yyyyMMddHHmmss"));
            if (endTime.HasValue)
                dictionary.Add("EndDate", endTime.Value.ToString("yyyyMMddHHmmss"));
            if (!string.IsNullOrWhiteSpace(payNo))
                dictionary.Add("PayNo", payNo.Trim());
            if (!string.IsNullOrEmpty(amountMin))
                dictionary.Add("AmountMin", amountMin);
            if (!string.IsNullOrEmpty(amountMax))
                dictionary.Add("AmountMax", amountMax);

            var page = Math.Ceiling((double)startIndex / count) + 1;
            dictionary.Add("CurrentPage", page.ToString(CultureInfo.InvariantCulture));
            dictionary.Add("PageSize", count.ToString(CultureInfo.InvariantCulture));
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (!string.IsNullOrWhiteSpace(outTradeNo))
                dictionary.Add("OutTradeNo", outTradeNo.Trim());
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lst = new List<BillDetail>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BillDetail
                {
                    Amount = item.Amount,
                    CreateDate = item.CreateDate,
                    PayNo = item.PayNo,
                    BillDate = item.BillDate,
                    Notes = item.Notes,
                    OutOrderNo = item.OutOrderNo,
                    OutTradeNo = item.OutTradeNo
                });
            }
            var tuple = new Tuple<IEnumerable<BillDetail>, int>(lst, totalcount);
            return tuple;
        }

        public Tuple<IEnumerable<BillDetail>, int> GetRePayDetail(string code, string key, DateTime? startTime, DateTime? endTime, DateTime? repayStartDate, DateTime? repayEndDate, string payNo, string status, int startIndex, int count, string outTradeNo)
        {
            var ch = new CashbagHelper(_webUrlBill + "GetRePayDetail", "GET");
            var dictionary = new Dictionary<string, string> { { "code", code }, { "key", key } };
            if (startTime.HasValue)
                dictionary.Add("StartDate", startTime.Value.ToString("yyyyMMddHHmmss"));
            if (endTime.HasValue)
                dictionary.Add("EndDate", endTime.Value.ToString("yyyyMMddHHmmss"));
            if (!string.IsNullOrWhiteSpace(payNo))
                dictionary.Add("PayNo", payNo.Trim());
            if (repayStartDate.HasValue)
                dictionary.Add("RepayStartDate", repayStartDate.Value.ToString("yyyyMMddHHmmss"));
            if (repayEndDate.HasValue)
                dictionary.Add("RepayEndDate", repayEndDate.Value.ToString("yyyyMMddHHmmss"));
            if (!string.IsNullOrEmpty(status))
                dictionary.Add("Status", status);

            var page = Math.Ceiling((double)startIndex / count) + 1;
            dictionary.Add("CurrentPage", page.ToString(CultureInfo.InvariantCulture));
            dictionary.Add("PageSize", count.ToString(CultureInfo.InvariantCulture));
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (!string.IsNullOrWhiteSpace(outTradeNo))
                dictionary.Add("OutTradeNo", outTradeNo.Trim());
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var lst = new List<BillDetail>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                lst.Add(new BillDetail
                {
                    Amount = item.Amount,
                    CreateDate = item.CreateDate,
                    PayNo = item.PayNo,
                    BillDate = item.BillDate,
                    Notes = item.Notes,
                    TotalAmount = item.TotalAmount,
                    Status = item.Status,
                    OutOrderNo = item.OutOrderNo,
                    OutTradeNo = item.OutTradeNo,
                });
            }
            var tuple = new Tuple<IEnumerable<BillDetail>, int>(lst, totalcount);
            return tuple;
        }
        #endregion

        #region 信用账户开通（申请，查询）
        public void GrantApply(string code, string key, string ca, string ra, string wa, string ha, string ba, string ma, string ea, string ia)
        {
            var ch = new CashbagHelper(_webUrl + "GrantApply", "POST");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            if (!string.IsNullOrEmpty(ca))
                dictionary.Add("ca", ca);
            if (!string.IsNullOrEmpty(ra))
                dictionary.Add("ra", ra);
            if (!string.IsNullOrEmpty(wa))
                dictionary.Add("wa", wa);
            if (!string.IsNullOrEmpty(ha))
                dictionary.Add("ha", ha);
            if (!string.IsNullOrEmpty(ba))
                dictionary.Add("ba", ba);
            if (!string.IsNullOrEmpty(ma))
                dictionary.Add("ma", ma);
            if (!string.IsNullOrEmpty(ea))
                dictionary.Add("ea", ea);
            if (!string.IsNullOrEmpty(ia))
                dictionary.Add("ia", ia);
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public GrantInfo GetGrantInfo(string code, string key)
        {
            var ch = new CashbagHelper(_webUrl + "QueryGrantApply", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var grant = new GrantInfo();
            if (result.result != null)
            {
                grant.GrantArray = new List<GrantArray>();
                var rows = JArray.FromObject(result.result);
                foreach (var item in rows)
                {
                    if (item.Key.ToString() == "") continue;
                    var grantarray = new GrantArray
                    {
                        Key = item.Key,
                        ImageUrl = item.ImageUrl
                    };
                    grant.GrantArray.Add(grantarray);
                }
            }
            grant.Applystatus = result.code;
            grant.message = result.message;
            return grant;
        }
        #endregion

        #region 银行卡
        public IEnumerable<BankCard> GetBank(string code, string key)
        {
            var ch = new CashbagHelper(_webUrlbank + "GetAccount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);

            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var lst = new List<BankCard>();
            var rows = JArray.FromObject(result.result.rows);

            foreach (var p in rows)
            {
                lst.Add(new BankCard
                {
                    City = p.City,
                    Province = p.Province,
                    Name = p.BankName,
                    BankBranch = p.Address,
                    CardNo = p.BankCardNumber,
                    Owner = p.AccountName,
                    BankId = p.AccountID,
                    IsDefault = p.IsDefault
                });
            }
            return lst;
        }

        public void AddBank(string code, string key, BankCard bank)
        {
            var ch = new CashbagHelper(_webUrlbank + "AddAccount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"AccountName", bank.Owner},
                {"BankCardNumber", bank.CardNo},
                {"Address", bank.BankBranch},
                {"BankName", bank.Name},
                {"Province", bank.Province},
                {"City", bank.City},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public void ModifyBank(string code, string key, BankCard bank)
        {
            var ch = new CashbagHelper(_webUrlbank + "ModifyAccount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"accountId", bank.BankId},
                {"BankCardNumber", bank.CardNo},
                {"Address", bank.BankBranch},
                {"BankName", bank.Name},
                {"Province", bank.Province},
                {"City", bank.City},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public void SetDefaultBank(string code, string key, string bankId)
        {
            var ch = new CashbagHelper(_webUrlbank + "SetDefaultAccount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"AccountID", bankId},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public void RemoveBank(string code, string key, string bankId)
        {
            var ch = new CashbagHelper(_webUrlbank + "RemoveAccount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"AccountID", bankId},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public IEnumerable<BankInfo> GetBankListInfo(string code, string key)
        {
            var ch = new CashbagHelper(_webUrlbank + "GetBankList", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var list = new List<BankInfo>();
            var rows = JArray.FromObject(result.result.rows);
            foreach (var row in rows)
            {
                list.Add(new BankInfo { Code = row.Code, Name = row.Name, ImageUri = row.ImageUri });
            }
            return list;
        }
        #endregion

        #region 其它信息
        public AccountInfo GetAccountInfo(string code, string key)
        {
            var ch = new CashbagHelper(_webUrlBusinesst + "GetUserInfo", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);

            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var account = new AccountInfo
            {
                ReadyInfo = new ReadyAccount
                {
                    ReadyBalance = result.result.Balance,
                    FreezeAmount = result.result.FreezeAmount ?? 0
                },
                CreditInfo = new CreditAccount
                {
                    CreditBalance = result.result.Available,
                    CreditQuota = result.result.CreditLine,
                    TempQuota = result.result.TemporaryLine,
                    Status = result.result.Status
                },
                FinancialInfo = new FinancialAccount
                {
                    FinancialMoney = result.result.TotalFinancial,
                },
                ScoreInfo = new ScoreAccount
                {
                    FinancialScore = result.result.TotalPoint ?? 0
                }
            };
            account.FinancialInfo.FinancialProducts = new List<CurrentFinancialProduct>();
            if (result.result.currentFinancialList == null) return account;
            var rows = JArray.FromObject(result.result.currentFinancialList);
            foreach (var p in rows)
            {
                var m = new CurrentFinancialProduct
                {
                    BuyTime = Convert.ToDateTime(p.BuyTime),
                    FinancialMoney = p.FinancialMoney,
                    ProductName = p.ProductName,
                    SerialNum = p.SerialNum,
                    ImageUrl = p.ImageURL,
                    Content = p.Content,
                    ProductID = p.FinancingProductID,
                    ReturnRate = p.Rate,
                    MinRate = p.MinRate,
                    Day = p.Day,
                    StarDate = Convert.ToDateTime(p.StarDate),
                    PreProfit = p.PreProfit,
                    BuyDay = p.BuyDay,
                    PreEndDate = Convert.ToDateTime(p.PreEndDate),
                    Status = p.Status,
                    TradeID = p.TradeID,
                    CanSettleInAdvance = p.CanSettleInAdvance
                };
                account.FinancialInfo.FinancialProducts.Add(m);
            }
            return account;
        }

        public CashCompanyInfo GetCompanyInfo(string code, string key)
        {
            var ch = new CashbagHelper(_webUrlBusinesst + "GetCompanyInfo", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            var cpyinfo = new CashCompanyInfo
            {
                CpyName = result.result.CompanyName,
                Contact = result.result.ContactUser
            };
            return cpyinfo;
        }

        public RepayInfo GetRepayInfo(string code, string key)
        {
            var ch = new CashbagHelper(_webUrlBusinesst + "CreditAccounts", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var rinfo = new RepayInfo
            {
                CreditAmount = result.result.CreditAmount ?? 0,
                AvailableAmount = result.result.AvailableAmount ?? 0,
                LateAmount = result.result.LateAmount ?? 0,
                OweRentMoney = result.result.OweRentMoney ?? 0,
                ShouldRepayMoney = result.result.ShouldRepayMoney ?? 0
            };
            return rinfo;
        }
        #endregion

        #region 支付密码修改,短信验证码
        public string GetValidateCode(string code, string key)
        {
            var ch = new CashbagHelper(_webUrl + "GetValidateCodeAccount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")},
                {"account", SettingSection.GetInstances().Sms.smsLKAccount},
                {"password", SettingSection.GetInstances().Sms.smsLKPwd}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }

        public void SetPayPassword(string code, string key, string newpwd, string smsPwd)
        {
            var ch = new CashbagHelper(_webUrl + "SetPayPassword", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"NewPwd", newpwd},
                {"SmsPwd", smsPwd},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }
        #endregion

        #region 手工兑换积分
        public void ExchangeSource(string code, string key, decimal source)
        {
            var ch = new CashbagHelper(_webUrlAccount + "HandMovePointToRecieve", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"source", source.ToString(CultureInfo.InvariantCulture)},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

        }
        #endregion

        #region 新增修改删除用户
        public CashCompanyInfo AddCompany(string code, string key, CashCompanyInfo cashcpyinfo)
        {

            var ch = new CashbagHelper(_webUrlBusinesst + "AddCompany", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"CompanyName", cashcpyinfo.CpyName},
                {"ContactUser", cashcpyinfo.Contact},
                {"province", cashcpyinfo.Province},
                {"city", cashcpyinfo.City},
                {"Address", cashcpyinfo.Address},
                {"ClientAccount", cashcpyinfo.ClientAccount},
                {"Moblie", cashcpyinfo.Moblie},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var cpyinfo = new CashCompanyInfo
            {
                Token = result.result.Token,
                PayAccount = result.result.PayAccount
            };
            return cpyinfo;
        }

        public void UpdateCompany(string code, string key, CashCompanyInfo cashcpyinfo)
        {
            var ch = new CashbagHelper(_webUrlBusinesst + "UpdateCompany", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"CustomerName",cashcpyinfo.CpyName},
                {"PayAccount", cashcpyinfo.PayAccount},
                {"ContactUser", cashcpyinfo.Contact},
                {"province", cashcpyinfo.Province},
                {"city", cashcpyinfo.City},
                {"Address", cashcpyinfo.Address},
                {"Moblie", cashcpyinfo.Moblie},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }
        public void DeleteCashBagBusinessman(string code, string payAccount, string key)
        {
            var ch = new CashbagHelper(_webUrlBusinesst + "Remove", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"payAccount", payAccount},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }
        #endregion


        #region 支付宝快捷绑定，解绑，查询


        public string GetAliPaySign(string code, string key, string alipayAccount)
        {
            var ch = new CashbagHelper(_webUrl + "GetAliPaySign", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"email", alipayAccount}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }
        public string AlipayBind(string code, string key, string alipayAccount, string payPwd)
        {
            var ch = new CashbagHelper(_webUrl + "AlipayBind", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"email", alipayAccount},
                {"paypwd", payPwd}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.status;
        }

        public string AlipayUnBind(string code, string key, string payPwd)
        {
            var ch = new CashbagHelper(_webUrl + "AlipayUnBind", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key}, 
                {"paypwd", payPwd}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.status;
        }

        public Tuple<string, string> GetBindAccount(string code, string key)
        {
            var ch = new CashbagHelper(_webUrl + "GetBind", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            //return new Tuple<string, string>(result.result.alipayEmail, result.result.tenpayEmail);
            string alipayEmail = result.result.alipayEmail;
            string tenpayEmail = result.result.tenpayEmail;
            return Tuple.Create(alipayEmail, tenpayEmail);
        }



        #endregion

    }
}
