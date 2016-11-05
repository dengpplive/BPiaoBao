using BPiaoBao.Common;
using BPiaoBaoTPos.Domain.Models;
using BPiaoBaoTPos.Domain.Services;
using JoveZhao.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.TPos.ClientProxy
{
    public class BusinessmanClientProxy : IBusinessmanClientProxy
    {
        private string WebUrlTPos = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "TPos/";
        public Tuple<IEnumerable<BPiaoBaoTPos.Domain.Models.BusinessmanInfo>, int> GetPosBusinessman(string code, string key, string businessmanName, string posNo, int startIndex, int count)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "GetTPOSCompanyList", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("POSNumber", posNo);
            dictionary.Add("CompanyName", businessmanName);
            var page = Math.Ceiling((double)startIndex / count) + 1;
            dictionary.Add("CurrentPage", page.ToString());
            dictionary.Add("PageSize", count.ToString());
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            List<BusinessmanInfo> list = new List<BusinessmanInfo>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                BusinessmanInfo businessinfo = new BusinessmanInfo()
                {
                    Id = item.CompanyID,
                    BusinessmanName = item.CompanyName,
                    TotalPosCount = item.Count,
                    PosRate = item.POSRate,
                    Bank = new BusinessmanInfo.BankInfo()
                    {
                        BankName = item.BankName,
                        CardNo = item.BankCardNumber
                    },
                    CreateTime = item.SignDate
                };
                list.Add(businessinfo);
            }
            Tuple<IEnumerable<BusinessmanInfo>, int> tuple = new Tuple<IEnumerable<BusinessmanInfo>, int>(list, totalcount);
            return tuple;
        }

        public void AddBusinessman(string code, string key, string OperationUser, BPiaoBaoTPos.Domain.Models.BusinessmanInfo businessmanInfo)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "Create", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("CompanyName", businessmanInfo.BusinessmanName);
            dictionary.Add("Address", businessmanInfo.Address);
            dictionary.Add("ContactUser", businessmanInfo.LinkMan);
            dictionary.Add("Moblie", businessmanInfo.LinkPhone);
            dictionary.Add("Phone", businessmanInfo.LinkTel);
            dictionary.Add("AccountName", businessmanInfo.Bank.Cardholder);
            dictionary.Add("BankCardNumber", businessmanInfo.Bank.CardNo);
            dictionary.Add("BankAddress", businessmanInfo.Bank.Address.Subbranch);
            dictionary.Add("BankName", businessmanInfo.Bank.BankName);
            dictionary.Add("BankProvince", businessmanInfo.Bank.Address.Province);
            dictionary.Add("BankCity", businessmanInfo.Bank.Address.City);
            dictionary.Add("POSRate", businessmanInfo.PosRate.ToString());
            dictionary.Add("OperationUser", OperationUser);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
        }
        public void DeleteBusinessman(string code, string key, string OperationUser, string Id)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "Remove", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("CompanyID", Id);
            dictionary.Add("OperationUser", OperationUser);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
        }
        public void UpdateBusinessman(string code, string key, string OperationUser, BPiaoBaoTPos.Domain.Models.BusinessmanInfo businessmanInfo)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "Update", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("MerchantID", businessmanInfo.Id);
            dictionary.Add("Address", businessmanInfo.Address);
            dictionary.Add("ContactUser", businessmanInfo.LinkMan);
            dictionary.Add("Moblie", businessmanInfo.LinkPhone);
            dictionary.Add("Phone", businessmanInfo.LinkTel);
            dictionary.Add("AccountName", businessmanInfo.Bank.Cardholder);
            dictionary.Add("AccountID", businessmanInfo.Bank.BankId);
            dictionary.Add("BankCardNumber", businessmanInfo.Bank.CardNo);
            dictionary.Add("BankAddress", businessmanInfo.Bank.Address.Subbranch);
            dictionary.Add("BankName", businessmanInfo.Bank.BankName);
            dictionary.Add("BankProvince", businessmanInfo.Bank.Address.Province);
            dictionary.Add("BankCity", businessmanInfo.Bank.Address.City);
            dictionary.Add("POSRate", businessmanInfo.PosRate.ToString());
            dictionary.Add("OperationUser", OperationUser);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
        }

        public BusinessmanInfo GetBusinessmanInfo(string code, string key, string Id)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "QueryMerchant", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("MerchantID", Id);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            var businessinfo = new BusinessmanInfo()
            {
                Id = result.result.CompanyID,
                Address = result.result.Address,
                LinkMan = result.result.ContactUser,
                LinkPhone = result.result.Moblie,
                LinkTel = result.result.Phone,
                PosRate = result.result.POSRate,
                BusinessmanName = result.result.CompanyName,
                CreateTime = result.result.CreateDate,
                Bank = new BusinessmanInfo.BankInfo()
                {
                    Address = new BusinessmanInfo.BankAddress()
                    {
                        Province = result.result.BankProvince,
                        City = result.result.BankCity,
                        Subbranch = result.result.BankAddress
                    },
                    BankName = result.result.BankName,
                    Cardholder = result.result.AccountName,
                    CardNo = result.result.BankCardNumber,
                    BankId = result.result.AccountID
                },
                TotalPosCount = result.result.POSCount
            };
            return businessinfo;
        }

        public void AssignPos(string code, string key, string OperationUser, string Id, string[] posNoList)
        {
            string posNo = string.Empty;
            for (int i = 0; i < posNoList.Length; i++)
            {
                posNo += posNoList[i] + ",";
            }
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "Distribution", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("MerchantID", Id);
            dictionary.Add("POSNumbers", posNo.TrimEnd(','));
            dictionary.Add("OperationUser", OperationUser);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
        }


        public void RetrievePos(string code, string key, string Id, string PosNo, string OperationUser)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "Retrieve", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("MerchantID", Id);
            dictionary.Add("POSNumber", PosNo);
            dictionary.Add("OperationUser", OperationUser);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
        }


       
    }
}
