using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using BPiaoBao.AppServices.Contracts.Cashbag;

namespace BPiaoBao.Client.UIExt.Model
{
    public class BankData
    {
        private static List<BankInfo> _banks;

        private static List<BankInfo> _payPlatforms;

        public static List<BankInfo> Banks
        {
            get { return _banks ?? (_banks = GetBanks()); }
        }

        public static List<BankInfo> PayPlatforms
        {
            get
            {
                return _payPlatforms ?? (_payPlatforms = new List<BankInfo>()
                {
                    //new BankInfo(20003,"ChinaPay","汇付","/BPiaoBao.Client.UIExt;component/Image/PayPlatformsImages/ChinaPnrPay.png"),
                    new BankInfo(20001, "Alipay", "支付宝",
                        "/BPiaoBao.Client.UIExt;component/Image/PayPlatformsImages/Alipay.png"),
                    //new BankInfo(20002,"BillPay","快钱","/BPiaoBao.Client.UIExt;component/Image/PayPlatformsImages/BillPay.png"),
                    new BankInfo(20004, "Tenpay", "财付通",
                        "/BPiaoBao.Client.UIExt;component/Image/PayPlatformsImages/Tenpay.png")
                });
            }
        }
        public static List<BankInfo> GetBanks()
        {

            //Assembly assembly = Assembly.Load("BPiaoBao.Client.UIExt");
            //Stream stream = assembly.GetManifestResourceStream("BPiaoBao.Client.UIExt.Data.BankData.xml");
            //XDocument xdoc = XDocument.Load(stream);
            //var list = xdoc.Element("Root").Elements("Bank").Select(p => new BankInfo(
            //    int.Parse(p.Attribute("Num").Value),
            //    p.Attribute("Code").Value,
            //    p.Attribute("Name").Value,
            //    p.Attribute("Uri").Value
            //    )).ToList();

            var list = new List<BankInfo>();
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var data = service.GetBankListInfo();
                list = data.Select(p => new BankInfo(0, p.Code, p.Name, p.ImageUri)).ToList();

            }, UIManager.ShowErr);
            return list;
        }
        /// <summary>
        /// 通过银行Name获取银行信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BankInfo GetBankByName(string name)
        {
            var list = GetBanks();
            return list.FirstOrDefault(p => p.Name.Equals(name));
        }

        /// <summary>
        ///通过银行Code获取银行信息
        /// </summary>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        public static BankInfo GetBankInfoByCode(string bankCode)
        {
            var list = GetBanks();
            return list.FirstOrDefault(p => string.Equals(p.Code,bankCode,StringComparison.CurrentCultureIgnoreCase));
        }
    }
    public class BankInfo
    {
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
