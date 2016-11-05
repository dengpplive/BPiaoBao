using AutoMapper;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Cashbag.Domain.Models;
using BPiaoBao.Cashbag.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPiaoBao.AppServices.Cashbag
{
    public class AccountService : BaseService, IAccountService
    {
        IAccountClientProxy accountClientProxy;
        public AccountService(IAccountClientProxy accountClientProxy)
        {
            this.accountClientProxy = accountClientProxy;
        }

        readonly string _code = AuthManager.GetCurrentUser().CashbagCode;
        readonly string _key = AuthManager.GetCurrentUser().CashbagKey;

        #region 记录
        public DataPack<CashOutLogDto> FindCashOutLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var data = accountClientProxy.GetCashOutLog(_code, _key, startTime, endTime, startIndex, count, outTradeNo);
            var result = new DataPack<CashOutLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new CashOutLogDto
                {
                    CashOutMoney = p.CashOutMoney,
                    CashOutStatus = p.CashOutStatus,
                    CashOutTime = p.CashOutTime,
                    SerialNum = p.SerialNum,
                    ClearDateTime = p.ClearDateTime,
                    BankNo = p.BankNo,
                    FeeAmount = p.FeeAmount,
                    OutTradeNo = p.OutTradeNo,
                    ReceivingType = p.ReceivingType
                }).ToList()
            };
            return result;
        }

        public DataPack<FinancialLogDto> FindFinancialLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var data = accountClientProxy.GetFinancialLog(_code, _key, startTime, endTime, startIndex, count, outTradeNo);

            var result = new DataPack<FinancialLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new FinancialLogDto
                {
                    BuyTime = p.BuyTime,
                    FinancialLogStatus = p.FinancialLogStatus,
                    FinancialMoney = p.FinancialMoney,
                    ProductName = p.ProductName,
                    SerialNum = p.SerialNum,
                    AbortTime = p.AbortTime,
                    StartDateTime = p.StartDateTime,
                    CashSource = p.CashSource,
                    PointAmount = p.PointAmount,
                    PreEndDateTime = p.PreEndDateTime,
                    AuditDate = p.AuditDate,
                    OutTradeNo = p.OutTradeNo,
                    Remark = p.Remark
                }).ToList()
            };
            return result;
        }

        public DataPack<RechargeLogDto> FindRechargeLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var data = accountClientProxy.GetRechargeLogs(_code, _key, startTime, endTime, startIndex, count, outTradeNo);

            var result = new DataPack<RechargeLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new RechargeLogDto
                {
                    CashSource = p.CashSource,
                    RechargeMoney = p.RechargeMoney,
                    RechargeStatus = p.RechargeStatus,
                    RechargeTime = p.RechargeTime,
                    SerialNum = p.SerialNum,
                    TypeName = p.TypeName,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return result;
        }

        public DataPack<RepaymentLogDto> FindRepaymentLog(DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var data = accountClientProxy.GetRepaymentLog(_code, _key, startTime, endTime, startIndex, count);

            var result = new DataPack<RepaymentLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new RepaymentLogDto
                {
                    RepaymentNotes = p.RepaymentNotes,
                    CashSource = p.CashSource,
                    RepaymentMoney = p.RepaymentMoney,
                    RepaymentStatus = p.RepaymentStatus,
                    RepaymentTime = p.RepaymentTime,
                    SerialNum = p.SerialNum,
                    ShouldAmount = p.ShouldAmount,
                    BillAmount = p.BillAmount,
                    BillTime = p.BillTime,
                    TotalAmount = p.TotalAmount,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return result;
        }

        public DataPack<TransferAccountsLogDto> FindTransferAccountsLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var data = accountClientProxy.GetTransferAccountsLog(_code, _key, startTime, endTime, startIndex, count, outTradeNo);

            var result = new DataPack<TransferAccountsLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new TransferAccountsLogDto
                {
                    SerialNum = p.SerialNum,
                    InComeOrExpenses = p.TransferNotes,
                    TransferAccountsMoney = p.TransferAccountsMoney,
                    TransferAccountsStatus = p.TransferAccountsStatus,
                    TargetAccount = p.TargetAccount,
                    TransferAccountsTime = p.TransferAccountsTime,
                    TransferAccountsType = p.TransferAccountsType,
                    Type = p.Type,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return result;
        }

        public DataPack<BargainLogDto> GetBargainLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var data = accountClientProxy.GetBargainLog(_code, _key, startTime, endTime, startIndex, count, outTradeNo);
            var rs = new DataPack<BargainLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new BargainLogDto
                {
                    SerialNum = p.SerialNum,
                    Status = p.Status,
                    CashSource = p.CashSource,
                    BargainTime = p.BargainTime,
                    Money = p.Money,
                    OrderId = p.OrderId,
                    CreateTime = p.CreateTime,
                    OutTradeNo = p.OutTradeNo,
                    TradeType = p.TradeType,
                    Notes = p.Notes
                }).ToList()
            };
            return rs;
        }

        public DataPack<ScoreConvertLogDto> GetScoreConvertLog(DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var data = accountClientProxy.GetScoreConvertLog(_code, _key, startTime, endTime, startIndex, count);
            var rs = new DataPack<ScoreConvertLogDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new ScoreConvertLogDto
                {
                    CreateDate = p.CreateDate,
                    LeaveAmount = p.LeaveAmount,
                    PointAmount = p.PointAmount
                }).ToList()
            };
            return rs;
        }

        #endregion
        #region 明细
        //public DataContracts.DataPack<DataContracts.Cashbag.BalanceDetailDto> GetReadyAccountDetails(DateTime? startTime, DateTime? endTime, int startIndex, int count)
        //{
        //    var data = accountClientProxy.GetReadyAccountDetails(code, key, startTime, endTime, startIndex, count);
        //    var rs = new DataPack<BalanceDetailDto>()
        //    {
        //        TotalCount = data.Item2,
        //        List = data.Item1.Select(p => new BalanceDetailDto()
        //        {
        //            SerialNum = p.SerialNum,
        //            Amount = p.Amount,
        //            CreateAmount = p.CreateAmount,
        //            LeaveAmount = p.LeaveAmount,
        //            OperationType = p.OperationType,
        //            PayType = p.PayType,
        //            Remark = p.Remark,
        //            OutOrderNo = p.OutOrderNo,
        //            OutTradeNo = p.OutTradeNo
        //        }).ToList()
        //    };
        //    return rs;
        //}

        public DataPack<BalanceDetailDto> GetReadyAccountDetails(DateTime? startTime, DateTime? endTime, string outTradeNo, string orderNo,
            int startIndex, int count)
        {
            var data = accountClientProxy.GetReadyAccountDetails(_code, _key, startTime, endTime, outTradeNo, orderNo, startIndex, count);
            var rs = new DataPack<BalanceDetailDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new BalanceDetailDto
                {
                    SerialNum = p.SerialNum,
                    Amount = p.Amount,
                    CreateAmount = p.CreateAmount,
                    LeaveAmount = p.LeaveAmount,
                    OperationType = p.OperationType,
                    PayType = p.PayType,
                    Remark = p.Remark,
                    OutOrderNo = p.OutOrderNo,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return rs;
        }

        public DataPack<BalanceDetailDto> GetCreditAccountDetails(DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var data = accountClientProxy.GetCreditAccountDetails(_code, _key, startTime, endTime, startIndex, count);
            var rs = new DataPack<BalanceDetailDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new BalanceDetailDto
                {
                    SerialNum = p.SerialNum,
                    Amount = p.Amount,
                    CreateAmount = p.CreateAmount,
                    LeaveAmount = p.LeaveAmount,
                    OperationType = p.OperationType,
                    PayType = p.PayType,
                    Remark = p.Remark,
                    OutOrderNo = p.OutOrderNo,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return rs;
        }

        public DataPack<BalanceDetailDto> GetScoreAccountDetails(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null)
        {
            var data = accountClientProxy.GetScoreAccountDetails(_code, _key, startTime, endTime, startIndex, count, outTradeNo);
            var rs = new DataPack<BalanceDetailDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new BalanceDetailDto
                {
                    SerialNum = p.SerialNum,
                    Amount = p.Amount,
                    CreateAmount = p.CreateAmount,
                    LeaveAmount = p.LeaveAmount,
                    OperationType = p.OperationType,
                    PayType = p.PayType,
                    Remark = p.Remark,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return rs;
        }

        #endregion
        #region 账单
        public DataPack<BillListDto> GetBill(DateTime? startTime, DateTime? endTime, string status, int startIndex, int count)
        {
            var data = accountClientProxy.GetBill(_code, _key, startTime, endTime, status, startIndex, count);
            var rs = new DataPack<BillListDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new BillListDto
                {
                    Amount = p.Amount,
                    CreateDate = p.CreateDate,
                    FeeAmount = p.FeeAmount,
                    LateAmount = p.LateAmount,
                    RepayAmount = p.RepayAmount,
                    ShouldRepayAmount = p.ShouldRepayAmount,
                    Status = p.Status,
                    BillAmount = p.BillAmount,
                    ShouldRepayDate = p.ShouldRepayDate,
                    CreditDayStr = p.CreditDayStr
                }).ToList()
            };
            return rs;
        }

        public DataPack<BillDetailListDto> GetBillDetail(DateTime? startTime, DateTime? endTime, string payNo, string amountMin, string amountMax, int startIndex, int count, string outTradeNo)
        {
            var data = accountClientProxy.GetBillDetail(_code, _key, startTime, endTime, payNo, amountMin, amountMax, startIndex, count, outTradeNo);
            var rs = new DataPack<BillDetailListDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new BillDetailListDto
                {
                    Amount = p.Amount,
                    CreateDate = p.CreateDate,
                    BillDate = p.BillDate,
                    Notes = p.Notes,
                    PayNo = p.PayNo,
                    OutOrderNo = p.OutOrderNo,
                    OutTradeNo = p.OutTradeNo
                }).ToList()
            };
            return rs;
        }

        public DataPack<RePayDetailListDto> GetRePayDetail(DateTime? startTime, DateTime? endTime, DateTime? repayStartDate, DateTime? repayEndDate, string payNo, string status, int startIndex, int count, string outTradeNo)
        {
            var data = accountClientProxy.GetRePayDetail(_code, _key, startTime, endTime, repayStartDate, repayEndDate, payNo, status, startIndex, count, outTradeNo);
            var rs = new DataPack<RePayDetailListDto>
            {
                TotalCount = data.Item2,
                List = data.Item1.Select(p => new RePayDetailListDto
                {
                    Amount = p.Amount,
                    CreateDate = p.CreateDate,
                    BillDate = p.BillDate,
                    Notes = p.Notes,
                    PayNo = p.PayNo,
                    Status = p.Status,
                    TotalAmount = p.TotalAmount,
                    OutOrderNo = p.OutOrderNo,
                    OutTradeNo = p.OutTradeNo,
                    RepayType = p.RepayType
                }).ToList()
            };
            return rs;
        }

        #endregion
        #region  信用账户开通（申请，查询）
        public void GrantApply(string ca, string ra, string wa, string ha, string ba, string ma, string ea, string ia)
        {
            accountClientProxy.GrantApply(_code, _key, ca, ra, wa, ha, ba, ma, ea, ia);
        }

        public GrantInfoDto GetGrantInfo()
        {
            var data = accountClientProxy.GetGrantInfo(_code, _key);
            var grantinfo = new GrantInfoDto
            {
                message = data.message,
                Applystatus = data.Applystatus,
                GrantArray = new List<GrantArrayDto>()
            };
            if (data.GrantArray == null) return grantinfo;
            foreach (var item in data.GrantArray)
            {
                grantinfo.GrantArray.Add(new GrantArrayDto
                {
                    Key = item.Key,
                    ImageUrl = item.ImageUrl
                });
            }
            return grantinfo;
        }
        #endregion
        #region 银行卡
        public List<BankCardDto> GetBank()
        {
            var list = accountClientProxy.GetBank(_code, _key).Select(p => new BankCardDto
            {
                City = p.City,
                Province = p.Province,
                Name = p.Name,
                BankBranch = p.BankBranch,
                CardNo = p.CardNo,
                Owner = p.Owner,
                BankId = p.BankId,
                IsDefault = p.IsDefault
            }).ToList();
            return list;
        }
        public void AddBank(BankCardDto bank)
        {
            var bankcard = new BankCard
            {
                BankBranch = bank.BankBranch,
                BankId = bank.BankId,
                CardNo = bank.CardNo,
                City = bank.City,
                IsDefault = bank.IsDefault,
                Name = bank.Name,
                Owner = bank.Owner,
                Province = bank.Province
            };
            accountClientProxy.AddBank(_code, _key, bankcard);
        }
        public void ModifyBank(BankCardDto bank)
        {
            var bankcard = new BankCard
            {
                BankBranch = bank.BankBranch,
                BankId = bank.BankId,
                CardNo = bank.CardNo,
                City = bank.City,
                IsDefault = bank.IsDefault,
                Name = bank.Name,
                Owner = bank.Owner,
                Province = bank.Province
            };
            accountClientProxy.ModifyBank(_code, _key, bankcard);
        }
        public void SetDefaultBank(string bankId)
        {
            accountClientProxy.SetDefaultBank(_code, _key, bankId);
        }
        public void RemoveBank(string bankId)
        {
            accountClientProxy.RemoveBank(_code, _key, bankId);
        }

        public List<BankInfoDto> GetBankListInfo()
        {
            var list = accountClientProxy.GetBankListInfo(_code, _key).Select(p => new BankInfoDto
            {
                Code = p.Code,
                Name = p.Name,
                ImageUri = p.ImageUri
            }).ToList();
            return list;
        }
        #endregion
        #region 其它信息
        public AccountInfoDto GetAccountInfo()
        {
            var data = accountClientProxy.GetAccountInfo(_code, _key);
            var account = new AccountInfoDto
            {
                ReadyInfo = new ReadyAccountDto
                {
                    ReadyBalance = data.ReadyInfo.ReadyBalance,
                    FreezeAmount = data.ReadyInfo.FreezeAmount
                },
                CreditInfo = new CreditAccountDto
                {
                    CreditBalance = data.CreditInfo.CreditBalance,
                    CreditQuota = data.CreditInfo.CreditQuota,
                    TempQuota = data.CreditInfo.TempQuota,
                    Status = data.CreditInfo.Status
                },
                ScoreInfo = new ScoreAccountDto
                {
                    FinancialScore = data.ScoreInfo.FinancialScore
                },
                FinancialInfo = new FinancialAccountDto
                {
                    FinancialMoney = data.FinancialInfo.FinancialMoney,
                }
            };
            account.FinancialInfo.FinancialProducts = new List<CurrentFinancialProductDto>();
            foreach (var p in data.FinancialInfo.FinancialProducts)
            {
                account.FinancialInfo.FinancialProducts.Add(new CurrentFinancialProductDto
                {
                    BuyTime = p.BuyTime,
                    FinancialMoney = p.FinancialMoney,
                    ProductName = p.ProductName,
                    SerialNum = p.SerialNum,
                    ImageUrl = p.ImageUrl,
                    Content = p.Content,
                    ProductID = p.ProductID,
                    ReturnRate = p.ReturnRate,
                    Day = p.Day,
                    StarDate = p.StarDate,
                    PreProfit = p.PreProfit,
                    BuyDay = p.BuyDay,
                    MinRate = p.MinRate,
                    PreEndDate = p.PreEndDate,
                    Status = p.Status,
                    TradeID = p.TradeID,
                    CanSettleInAdvance = p.CanSettleInAdvance

                });
            }
            return account;
        }
        public CashCompanyInfoDto GetCompanyInfo()
        {
            var rs = accountClientProxy.GetCompanyInfo(_code, _key);
            var cpyinfo = new CashCompanyInfoDto
            {
                Contact = rs.Contact,
                CpyName = rs.CpyName
            };
            return cpyinfo;
        }
        public RepayInfoDto GetRepayInfo()
        {
            var data = accountClientProxy.GetRepayInfo(_code, _key);
            var repayinfo = new RepayInfoDto
            {
                CreditAmount = data.CreditAmount,
                AvailableAmount = data.AvailableAmount,
                OweRentMoney = data.OweRentMoney,
                LateAmount = data.LateAmount,
                ShouldRepayMoney = data.ShouldRepayMoney
            };
            return repayinfo;
        }
        #endregion

        #region 支付密码修改,短信验证码
        public string GetValidateCode()
        {
            return accountClientProxy.GetValidateCode(_code, _key);
        }

        public void SetPayPassword(string newpwd, string smsPwd)
        {
            accountClientProxy.SetPayPassword(_code, _key, newpwd, smsPwd);
        }
        #endregion




        public void ExchangeSource(decimal source)
        {
            accountClientProxy.ExchangeSource(_code, _key, source);
        }

        #region 新增修改用户
        public CashCompanyInfoDto AddCompany(CashCompanyInfoDto cashcpyinfo)
        {
            var data = accountClientProxy.AddCompany(_code, _key, Mapper.Map<CashCompanyInfo>(cashcpyinfo));
            return Mapper.Map<CashCompanyInfoDto>(data);
        }

        public void UpdateCompany(CashCompanyInfoDto cashcpyinfo)
        {
            accountClientProxy.UpdateCompany(_code, _key, Mapper.Map<CashCompanyInfo>(cashcpyinfo));
        }


        #endregion

        #region 支付宝代扣获取签约地址,绑定，解绑，查询
        [ExtOperationInterceptor("获取支付宝签约地址")]
        public string GetAlipaySign(string alipayAccount)
        {
            return accountClientProxy.GetAliPaySign(_code, _key, alipayAccount);
        }
        [ExtOperationInterceptor("绑定支付宝快捷支付")]
        public string AlipayBind(string alipayAccount, string payPwd)
        {
            return accountClientProxy.AlipayBind(_code, _key, alipayAccount, payPwd);
        }
        [ExtOperationInterceptor("解绑支付宝快捷支付")]
        public string AlipayUnBind(string payPwd)
        {
            return accountClientProxy.AlipayUnBind(_code, _key, payPwd);
        }
        [ExtOperationInterceptor("查询支付宝快捷支付")]
        public QuickPayDto GetBindAccount()
        {
            var temp = accountClientProxy.GetBindAccount(_code, _key);
            return new QuickPayDto() { AlipayAccount = temp.Item1, TenpayAccount = temp.Item2 };
        }

        #endregion
    }
}
