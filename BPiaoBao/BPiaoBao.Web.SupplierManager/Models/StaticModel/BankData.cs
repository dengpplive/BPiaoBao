using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Web.SupplierManager.CommonHelpers;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class BankData
    {




        /// <summary>
        /// 得到可选择的静态银行数据
        /// </summary>
        /// <returns></returns>
        public static List<BankInfo> GetAllBanks()
        {
            //string path = AppDomain.CurrentDomain.BaseDirectory + @"App_Data\BankData.xml";
            //var stream = new FileStream(path, FileMode.Open);
            //XDocument xdoc = XDocument.Load(stream);
            //var list = xdoc.Element("Root").Elements("Bank").Select(p => new BankInfo(
            //    int.Parse(p.Attribute("Num").Value),
            //    p.Attribute("Code").Value,
            //    p.Attribute("Name").Value,
            //    p.Attribute("Uri").Value
            //    )).ToList(); 
            //stream.Close();
            var list = new List<BankInfo>();
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var data = service.GetBankListInfo();
                list = data.Select(p => new BankInfo(0, p.Code, p.Name, p.ImageUri)).ToList();
            });
            return list;
        }


        /// <summary>
        /// 通过Code获取银行信息
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public static BankInfo GetBankInfoByName(string bankName)
        {
            var banks = GetAllBanks();
            var bank =
                banks.FirstOrDefault(p => string.Equals(p.Name, bankName, StringComparison.CurrentCultureIgnoreCase));
            return bank;

        }

        /// <summary>
        /// 通过Code获取银行信息
        /// </summary>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        public static BankInfo GetBankInfoByCode(string bankCode)
        {
            var banks = GetAllBanks();
            var bank =
                banks.FirstOrDefault(p => string.Equals(p.Code, bankCode, StringComparison.CurrentCultureIgnoreCase));
            return bank;

        }
    }

    public class BankInfo
    {
        public BankInfo() { }
        public BankInfo(int num, string code, string name, string uri)
        {
            this.Num = num;
            this.Code = code;
            this.Name = name;
            this.Uri = uri;
        }
        public int Num { get; private set; }
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Uri { get; private set; }
    }
}