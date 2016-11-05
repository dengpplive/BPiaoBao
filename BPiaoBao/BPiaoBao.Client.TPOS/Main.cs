using BPiaoBao.Client.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.TPOS
{
    [Plugin(ProjectCode, ProjectName, "pack://application:,,,/BPiaoBao.Client.TPOS;component/View/Images/project.png", 3, HomeCode)]
    public class Main : IPlugin
    {
        /// <summary>
        /// 获取模块菜单
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<MainMenuItem> Parts
        {
            get
            {
                return new List<MainMenuItem>() { 

                    new MainMenuItem{
                        Name=HomeName,
                        MenuCode=HomeCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.TPOS;component/View/Images/Home/ico.jpg"},

                    new MainMenuItem{
                        Name=MerchantManagerName,
                        MenuCode=MerchantManagerCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.TPOS;component/View/Images/merchant/ico.jpg"},

                    new MainMenuItem{
                        Name=POSManagerName,
                        MenuCode=POSManagerCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.TPOS;component/View/Images/pos/ico.png"},

                    new MainMenuItem{
                        Name="交易查询",
                        MenuCode="tposTransaction",
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.TPOS;component/View/Images/transaction/ico.jpg",
                        Children=new List<MainMenuItem>{
                                 new MainMenuItem{
                                     Name=TransactionQueryName,
                                     MenuCode=TransactionQueryCode,                                     
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name=TransactionSummaryName,
                                    MenuCode=TransactionSummaryCode,                                    
                                    Description=null
                                 }
                            }
                        }
                  };
            }
        }

        public const string ProjectCode = "TPOS";
        public const string ProjectName = "TPOS业务";

        public const string HomeCode = "tposHome";
        public const string HomeName = "我的账户";

        public const string MerchantManagerCode = "tposMerchant";
        public const string MerchantManagerName = "POS商户管理";

        public const string POSManagerCode = "tposManager";
        public const string POSManagerName = "POS机管理";

        public const string TransactionQueryCode = "tposTransactionQuery";
        public const string TransactionQueryName = "交易查询";

        public const string TransactionSummaryCode = "tposTransactionSummary";
        public const string TransactionSummaryName = "交易汇总";


    }
}
