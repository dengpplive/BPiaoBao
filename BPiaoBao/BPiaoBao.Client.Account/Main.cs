using BPiaoBao.Client.Account.Properties;
using BPiaoBao.Client.Account.View;
using BPiaoBao.Client.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BPiaoBao.Client.Account
{
    /// <summary>
    /// 我的账户接口
    /// <summary>
    [Plugin(Main.ProjectCode, "我的账户", "pack://application:,,,/BPiaoBao.Client.Account;component/Image/moduleLogo.png", 2, HomeCode)]
    public class Main : IPlugin
    {
        static Main()
        {
            Application.Current.Resources.MergedDictionaries.Add(SharedDictionaryManager.SharedDictionary);
        }

        public Main()
        {

        }

        #region 常量

        public const string HomeName = "主页";
        public const string HomeCode = "account1";

        public const string Transfer = "转账";
        public const string TransferCode = "account2";

        public const string TransferLog = "转账记录";
        public const string TransferLogCode = "account3";

        public const string Operation = "资金操作";
        public const string OperationCode = "account4";

        public const string Deposit = "充值";
        public const string DepositCode = "account5";

        public const string WithdrawDeposit = "结算";
        public const string WithdrawDepositCode = "account6";

        public const string Repayment = "还款";
        public const string RepaymentCode = "account7";

        public const string BankCard = "银行卡";
        public const string BankCardCode = "account8";

        public const string Finance = "理财";
        public const string FinanceCode = "account9";

        public const string OperationLog = "历史记录";
        public const string OperationLogCode = "account10";

        public const string DepositLog = "充值记录";
        public const string DepositLogCode = "account11";

        public const string WithdrawDepositLog = "结算记录";
        public const string WithdrawDepositLogCode = "account12";

        public const string FinanceLog = "理财记录";
        public const string FinanceLogCode = "account13";

        public const string RepaymentLog = "还款记录";
        public const string RepaymentLogCode = "account14";

        public const string TransactionLog = "交易记录";
        public const string TransactionLogCode = "account15";

        public const string PointExchangeLog = "积分兑换";
        public const string PointExchangeLogCode = "account16";

        public const string CashCode = "account17";
        public const string CreditCode = "account18";

        public const string PointCode = "account20";
        public const string ProjectCode = "account22";

        public const string ApplyingForCreditCode = "account23";
        public const string BillsCode = "account24";
        public const string BillDetailCode = "account25";
        public const string BillRePayDetailCode = "account26";
        public const string AllFinance = "理财";
        public const string AllFinanceCode = "account27";

        #endregion
        /// <summary>
        /// 获取模块菜单
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<MainMenuItem> Parts
        {
            get
            {
                if (BPiaoBao.Client.UIExt.Communicate.LoginInfo.IsAdmin)
                {
                    return new List<MainMenuItem>() {
                    new MainMenuItem{
                                     Name=HomeName,
                                     MenuCode=HomeCode,                                     
                                     Description=null,
                                     Icon="pack://application:,,,/BPiaoBao.Client.Account;component/Image/home.png",
                                 },
                     new MainMenuItem{
                        Name=Operation,
                        MenuCode=OperationCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.Account;component/Image/cashOperation.png",
                        Children=new List<MainMenuItem>{                                
                                 new MainMenuItem{
                                     Name=Deposit,
                                     MenuCode=DepositCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                     Name=AllFinance,
                                     MenuCode=AllFinanceCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name=WithdrawDeposit,
                                    MenuCode=WithdrawDepositCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=Transfer,
                                    MenuCode=TransferCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=Repayment,
                                    MenuCode=RepaymentCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=BankCard,
                                    MenuCode=BankCardCode,
                                    Description=null
                                 },                               
                            }
                        },                                                                  
                        new MainMenuItem{
                        Name=OperationLog,
                        MenuCode=OperationLogCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.Account;component/Image/operationLog.png",
                        Children=new List<MainMenuItem>{
                                 new MainMenuItem{
                                     Name=DepositLog,
                                     MenuCode=DepositLogCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name=WithdrawDepositLog,
                                    MenuCode=WithdrawDepositLogCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=TransferLog,
                                    MenuCode=TransferLogCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=FinanceLog,
                                    MenuCode=FinanceLogCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=RepaymentLog,
                                    MenuCode=BillRePayDetailCode,
                                    Description=null

                                 },
                                 //new MainMenuItem{
                                 //   Name=Resources.AllLog,
                                 //   MenuCode=Resources.AllLogCode,
                                 //   UserControl=new Lazy<IPart>(()=>new AllLogControl()),
                                 //   Description=null
                                 //}
                                 new MainMenuItem{
                                    Name=TransactionLog,
                                    MenuCode=TransactionLogCode,
                                    Description=null
                                 },
                                 //new MainMenuItem{
                                 //   Name=PointExchangeLog,
                                 //   MenuCode=PointExchangeLogCode,
                                 //   Description=null
                                 // }
                            }
                        }
                };
                }
                else
                {
                    return new List<MainMenuItem>() {
                    new MainMenuItem{
                                     Name=HomeName,
                                     MenuCode=HomeCode,                                     
                                     Description=null,
                                     Icon="pack://application:,,,/BPiaoBao.Client.Account;component/Image/home.png",
                                 },
                     new MainMenuItem{
                        Name=Operation,
                        MenuCode=OperationCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.Account;component/Image/cashOperation.png",
                        Children=new List<MainMenuItem>{                                
                                 new MainMenuItem{
                                     Name=Deposit,
                                     MenuCode=DepositCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                     Name=AllFinance,
                                     MenuCode=AllFinanceCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name=WithdrawDeposit,
                                    MenuCode=WithdrawDepositCode,
                                    Description=null
                                 },
                                 //new MainMenuItem{
                                 //   Name=Transfer,
                                 //   MenuCode=TransferCode,
                                 //   Description=null
                                 //},
                                 new MainMenuItem{
                                    Name=Repayment,
                                    MenuCode=RepaymentCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=BankCard,
                                    MenuCode=BankCardCode,
                                    Description=null
                                 },                               
                            }
                        },                                                                  
                        new MainMenuItem{
                        Name=OperationLog,
                        MenuCode=OperationLogCode,
                        Description=null,
                        Icon="pack://application:,,,/BPiaoBao.Client.Account;component/Image/operationLog.png",
                        Children=new List<MainMenuItem>{
                                 new MainMenuItem{
                                     Name=DepositLog,
                                     MenuCode=DepositLogCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name=WithdrawDepositLog,
                                    MenuCode=WithdrawDepositLogCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=TransferLog,
                                    MenuCode=TransferLogCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=FinanceLog,
                                    MenuCode=FinanceLogCode,
                                    Description=null
                                 },
                                 new MainMenuItem{
                                    Name=RepaymentLog,
                                    MenuCode=BillRePayDetailCode,
                                    Description=null

                                 },
                                 //new MainMenuItem{
                                 //   Name=Resources.AllLog,
                                 //   MenuCode=Resources.AllLogCode,
                                 //   UserControl=new Lazy<IPart>(()=>new AllLogControl()),
                                 //   Description=null
                                 //}
                                 new MainMenuItem{
                                    Name=TransactionLog,
                                    MenuCode=TransactionLogCode,
                                    Description=null
                                 },
                                 //new MainMenuItem{
                                 //   Name=PointExchangeLog,
                                 //   MenuCode=PointExchangeLogCode,
                                 //   Description=null
                                 // }
                            }
                        }
                };
                }
            }
        }



        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void Initlize()
        {
            //什么都不用做，只是为了触发静态构造函数，引用常量是不会触发的
        }
    }
}
