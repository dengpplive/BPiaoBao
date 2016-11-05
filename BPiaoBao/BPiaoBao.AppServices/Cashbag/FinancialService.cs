using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.Cashbag.Domain.Services;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.Cashbag
{
    public class FinancialService : BaseService, IFinancialService
    {
        IFinancialClientProxy financialProxy;

        public FinancialService(IFinancialClientProxy financialProxy)
        {
            this.financialProxy = financialProxy;
        }

        string code = AuthManager.GetCurrentUser().CashbagCode;
        string key = AuthManager.GetCurrentUser().CashbagKey;
        public List<FinancialProductDto> GetAllProduct()
        {
            BehaviorStatService.SaveBehaviorStat(DataContracts.SystemSetting.EnumBehaviorOperate.AccessCount);
            var temp = financialProxy.GetAllProduct(code,key);
            if (temp != null)
            {
                var result = temp.Select(p => new FinancialProductDto
                   {
                       Abstract = p.Abstract,
                       Day = p.Day,
                       Description = p.Description,
                       EndDate = p.EndDate,
                       IconUrl = p.IconUrl,
                       LimitAmount = p.LimitAmount,
                       CurrentAmount = p.CurrentAmount,
                       MaxAmount = p.MaxAmount,
                       Name = p.Name,
                       ProductId = p.ProductId,
                       ReturnRate = p.ReturnRate,
                       ValidDate = p.ValidDate,
                       InterestRate = p.InterestRate,
                       CanSettleInAdvance = p.CanSettleInAdvance
                   });
                if (result == null)
                    return null;
                return result.ToList();
            }

            return null;
        }
        public void BuyFinancialProductByCashAccount(string productID, decimal money, string pwd)
        {
            financialProxy.BuyFinancialProductByCashAccount(code, key, productID, money, pwd);
        }

        public string BuyFinancialProductByBank(string productID, decimal money, string bankName)
        {
            return financialProxy.BuyFinancialProductByBank(code, key, productID, money, bankName);
        }

        public string BuyFinancialProductByPlatform(string productID, decimal money, string payPlatform)
        {
            return financialProxy.BuyFinancialProductByPlatform(code, key, productID, money, payPlatform);
        }

        public void AbortFinancial(string tradeID, string pwd)
        {
            financialProxy.AbortFinancial(code, key, tradeID, pwd);
        }


        public ExpectProfitDto GetExpectProfit(string tradeID)
        {
            var data = financialProxy.GetExpectProfit(code, key, tradeID);
            var ep = new ExpectProfitDto()
            {
                Profit = data.Profit,
                NormalProfit = data.NormalProfit
            };
            return ep;
        }


        public FinancialProductDto GetSingleProductInfo(string productID)
        {
            var p = financialProxy.GetSingleProductInfo(code, key, productID);
            var fpd = new FinancialProductDto()
            {
                Abstract = p.Abstract,
                Day = p.Day,
                Description = p.Description,
                EndDate = p.EndDate,
                IconUrl = p.IconUrl,
                LimitAmount = p.LimitAmount,
                Name = p.Name,
                ProductId = p.ProductId,
                ReturnRate = p.ReturnRate,
                ValidDate = p.ValidDate,
                InterestRate = p.InterestRate,
                CanSettleInAdvance = p.CanSettleInAdvance
            };
            return fpd;
        }


      
        public IEnumerable<FinancialProductDto> GetShelfProducts(string quantity)
        {
            var temp = financialProxy.GetShelfProducts(code,key,quantity);
            if (temp != null)
            {
                var result = temp.Select(p => new FinancialProductDto
                {
                    Abstract = p.Abstract,
                    Day = p.Day,
                    Description = p.Description,
                    EndDate = p.EndDate,
                    IconUrl = p.IconUrl,
                    LimitAmount = p.LimitAmount,
                    CurrentAmount = p.CurrentAmount,
                    MaxAmount = p.MaxAmount,
                    Name = p.Name,
                    ProductId = p.ProductId,
                    ReturnRate = p.ReturnRate,
                    ValidDate = p.ValidDate,
                    InterestRate = p.InterestRate,
                    CanSettleInAdvance = p.CanSettleInAdvance
                });
                if (result == null)
                    return null;
                return result.ToList();
            }

            return null;
        }

        public IEnumerable<FinancialProductDto> GetActiveProduct(string quantity)
        {
            var temp = financialProxy.GetActiveProduct(code, key, quantity);
            if (temp != null)
            {
                var result = temp.Select(p => new FinancialProductDto
                {
                    Abstract = p.Abstract,
                    Day = p.Day,
                    Description = p.Description,
                    EndDate = p.EndDate,
                    IconUrl = p.IconUrl,
                    LimitAmount = p.LimitAmount,
                    CurrentAmount = p.CurrentAmount,
                    MaxAmount = p.MaxAmount,
                    Name = p.Name,
                    ProductId = p.ProductId,
                    ReturnRate = p.ReturnRate,
                    ValidDate = p.ValidDate,
                    InterestRate = p.InterestRate,
                    CanSettleInAdvance = p.CanSettleInAdvance
                });
                if (result == null)
                    return null;
                return result.ToList();
            }

            return null;
        }
    }
}
