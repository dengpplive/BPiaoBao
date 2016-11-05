using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.Cashbag
{
    public class FundService : BaseService, IFundService
    {
        IFundClientProxy fundClientProxy;
        IBusinessmanRepository businessmanRepository;
        public FundService(IFundClientProxy fundClientProxy, IBusinessmanRepository businessmanRepository)
        {
            this.fundClientProxy = fundClientProxy;
            this.businessmanRepository = businessmanRepository;
        }

        string code = AuthManager.GetCurrentUser().CashbagCode;
        string key = AuthManager.GetCurrentUser().CashbagKey;
        public string RechargeByBank(decimal money, string payBank)
        {
            return fundClientProxy.RechargeByBank(code, key, money, payBank);
        }

        public string RechargeByPlatform(decimal money, string payPlatform)
        {
            return fundClientProxy.RechargeByPlatform(code, key, money, payPlatform);
        }

        public void RepayMoneyByCashAccount(string money, string pwd)
        {
            fundClientProxy.RepayMoneyByCashAccount(code, key, money, pwd);
        }

        public string RepayMoneyByBank(string money, string Bank)
        {
            return fundClientProxy.RepayMoneyByBank(code, key, money, Bank);
        }

        public string RepayMoneyByPlatform(string money, string Platform)
        {
            return fundClientProxy.RepayMoneyByPlatform(code, key, money, Platform);
        }


        public void InnerTransfer(string targetcode, string money, string pwd, string notes)
        {
            var bm = businessmanRepository.FindAll(p => p.Code == targetcode).FirstOrDefault();
            if (bm != null)
                fundClientProxy.InnerTransfer(code, key, bm.CashbagCode, money, pwd, notes);
            else
                fundClientProxy.InnerTransfer(code, key, string.Empty, money, pwd, notes);
        }

        public void CashOut(decimal money, string accountID, string pwd, string Type)
        {
            fundClientProxy.CashOut(code, key, money, accountID, pwd, Type);
        }

        public string GetFeeAmount(string money, string Type)
        {
            return fundClientProxy.GetFeeAmount(code, key, money, Type);
        }


        public FeeRuleInfoDto GetFeeRule()
        {
            var data = fundClientProxy.GetFeeRule(code, key);
            var feerule = new FeeRuleInfoDto()
            {
                //todayFee = data.todayFee,
                //todayMax = data.todayMax,
                //todayMin = data.todayMin,
                //tomorrowFee = data.tomorrowFee,
                //tomorrowMax = data.tomorrowMax,
                //tomorrowMin = data.tomorrowMin
                IsHoliday = data.IsHoliday,
                Id = data.Id,
                Name = data.Name,
                CustomerType = data.CustomerType,
                IsDefault = data.IsDefault,
                TodayEnable = data.TodayEnable,
                TodayLast = data.TodayLast,
                TodayWithdrawRateType = data.TodayWithdrawRateType,
                TodayEachFeeAmount = data.TodayEachFeeAmount,
                TodayEachRate = data.TodayEachRate,
                TodayEachFeeAmountMin = data.TodayEachFeeAmountMin,
                TodayEachFeeAmountMax = data.TodayEachFeeAmountMax,
                TodayDayAmount = data.TodayDayAmount,
                TodayEachAmount = data.TodayEachAmount,
                MorrowEnable = data.MorrowEnable,
                MorrowLast = data.MorrowLast,
                MorrowWithdrawRateType = data.MorrowWithdrawRateType,
                MorrowEachFeeAmount = data.MorrowEachFeeAmount,
                MorrowEachRate = data.MorrowEachRate,
                MorrowEachFeeAmountMin = data.MorrowEachFeeAmountMin,
                MorrowEachFeeAmountMax = data.MorrowEachFeeAmountMax,
                MorrowDayAmount = data.MorrowDayAmount,
                MorrowEachAmount = data.MorrowEachAmount
            };
            return feerule;
        }


        public string OnLineRecieveByBank(decimal money, string NotifyUrl, string payBank)
        {
            return fundClientProxy.OnLineRecieveByBank(code, key, money, NotifyUrl, payBank);
        }

        public string OnLineRecieveByPlatform(decimal money, string NotifyUrl, string payPlatform)
        {
            return fundClientProxy.OnLineRecieveByBank(code, key, money, NotifyUrl, payPlatform);
        }


        public string GetApplicationMaxAmount(string Type)
        {
            return fundClientProxy.GetApplicationMaxAmount(code, key, Type);
        }

        /// <summary>
        /// 获取商户姓名
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetTargetAccountName(string code)
        {
            var bm = businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (bm != null)
                return bm.Name;
            else
                return "该商户号不存在";
        }

        /// <summary>
        /// 获取可用临时申请额度
        /// </summary>
        /// <returns></returns>
        public decimal GetTempCreditAmount()
        {
            return fundClientProxy.GetTempCreditAmount(code, key);
        }

        /// <summary>
        ///  申请临时额度
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="tempAmount"></param>
        public void TempCreditApplication(string pwd, decimal tempAmount)
        {
            fundClientProxy.TempCreditApplication(code, key, pwd, tempAmount);
        }

        /// <summary>
        /// 申请临时额度条件相关
        /// </summary>
        /// <returns></returns>
        public TempCreditInfoDto GetTempCreditSetting()
        {
            var data = fundClientProxy.GetTempCreditSetting(code, key);
            var tempCreditInfo = new TempCreditInfoDto { Day = data.Day, Number = data.Number };
            return tempCreditInfo;
        }

 

        #region 支付宝快捷充值，还款

        public void AlipaySignRecharge(decimal money, string payPwd)
        {
            fundClientProxy.AlipaySignRecharge(code, key, money, payPwd);
        }

        public void AlipaySignRepay(decimal money, string payPwd)
        {
            fundClientProxy.AlipaySignRepay(code, key, money,payPwd);
        }

        #endregion
    }
}
