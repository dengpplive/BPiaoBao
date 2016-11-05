using System.Globalization;
using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 主页视图模型
    /// </summary>
    public class HomeViewModel : BaseVM
    {
        #region 成员变量

        //private volatile bool isRefreshAccountInfo = false;//是否正在刷新账户信息

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
            Messenger.Default.Register<bool>(this, "HomeRefresh", p =>
            {
                if (p)
                {
                    Initialize();
                }
            });
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteInitlizeCommand())
                ExecuteInitlizeCommand();
        }

        /// <summary>
        /// 初始化数据，在完成时触发回调
        /// </summary>
        /// <param name="call"></param>
        public void Initialize(Action call)
        {
            IsBusy = true;

            Action action = () =>
            {
                try
                {
                    //RefreshRepay(true);
                    RefreshAccountInfo(true);
                    //RefreshGrantInfo(true);
                    RefreshBankCards(true);
                    RefreshAllFinance(true);
                    RefreshFindFinancialLog(true);
                }
                catch (Exception ex)
                {
                    UIManager.ShowErr(ex);
                }
            };
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                if (call != null)
                    call();
            });
        }

        #endregion

        #region 公开命令

        #region SwitchToCashViewCommand

        private RelayCommand _switchToCashViewCommand;

        /// <summary>
        /// 切换到现金账户
        /// </summary>
        public RelayCommand SwitchToCashViewCommand
        {
            get
            {
                return _switchToCashViewCommand ?? (_switchToCashViewCommand = new RelayCommand(ExecuteSwitchToCashViewCommand));
            }
        }

        private void ExecuteSwitchToCashViewCommand()
        {
            SwitchView(Main.CashCode);
        }

        #endregion

        #region SwitchToCreditViewCommand

        private RelayCommand _switchToCreditViewCommand;

        /// <summary>
        /// 切换到信用账户
        /// </summary>
        public RelayCommand SwitchToCreditViewCommand
        {
            get
            {
                return _switchToCreditViewCommand ?? (_switchToCreditViewCommand = new RelayCommand(ExecuteSwitchToCreditViewCommand));
            }
        }

        private void ExecuteSwitchToCreditViewCommand()
        {
            SwitchView(Main.CreditCode);
        }

        #endregion

        #region SwitchFinanceViewCommand

        private RelayCommand _switchFinanceViewCommand;

        /// <summary>
        /// 切换到理财账户
        /// </summary>
        public RelayCommand SwitchFinanceViewCommand
        {
            get
            {
                return _switchFinanceViewCommand ?? (_switchFinanceViewCommand = new RelayCommand(ExecuteSwitchFinanceViewCommand));
            }
        }

        private void ExecuteSwitchFinanceViewCommand()
        {
            SwitchView(Main.FinanceCode);
        }

        #endregion

        #region SwitchAllFinanceViewCommand

        private RelayCommand _switchAllFinanceViewCommand;

        /// <summary>
        /// 切换到所有理财产品
        /// </summary>
        public RelayCommand SwitchAllFinanceViewCommand
        {
            get
            {
                return _switchAllFinanceViewCommand ?? (_switchAllFinanceViewCommand = new RelayCommand(ExecuteSwitchAllFinanceViewCommand, CanExecuteSwitchAllFinanceViewCommand));
            }
        }

        private void ExecuteSwitchAllFinanceViewCommand()
        {
            SwitchView(Main.AllFinanceCode);
        }

        private bool CanExecuteSwitchAllFinanceViewCommand()
        {
            return true;
        }

        #endregion

        #region SwitchToFinanceLogViewCommand

        private RelayCommand _switchToFinanceLogViewCommand;

        public RelayCommand SwitchToFinanceLogViewCommand
        {
            get
            {
                return _switchToFinanceLogViewCommand ?? (_switchToFinanceLogViewCommand = new RelayCommand(ExecuteSwitchFinanceLogViewCommand));
            }
        }

        private void ExecuteSwitchFinanceLogViewCommand()
        {
            SwitchView(Main.FinanceLogCode);
        }

        #endregion

        #region SwitchPointsViewCommand

        private RelayCommand _switchPointsViewCommand;

        /// <summary>
        /// 切换到积分视图
        /// </summary>
        public RelayCommand SwitchPointsViewCommand
        {
            get
            {
                return _switchPointsViewCommand ?? (_switchPointsViewCommand = new RelayCommand(ExecuteSwitchPointsViewCommand));
            }
        }

        private void ExecuteSwitchPointsViewCommand()
        {
            SwitchView(Main.PointCode);
        }

        #endregion

        #region SwitchBankCardCommand

        private RelayCommand _switchBankcardCommand;

        /// <summary>
        /// 切换到银行卡命令
        /// </summary>
        public RelayCommand SwitchBankCardCommand
        {
            get
            {
                return _switchBankcardCommand ?? (_switchBankcardCommand = new RelayCommand(ExecuteSwitchBankCardCommand));
            }
        }

        private void ExecuteSwitchBankCardCommand()
        {
            SwitchView(Main.BankCardCode);
        }

        #endregion

        #region SwitchToApplyingForCreditCommand

        private RelayCommand _switchToApplyingForCreditCommand;

        /// <summary>
        /// 切换到信用申请界面
        /// </summary>
        public RelayCommand SwitchToApplyingForCreditCommand
        {
            get
            {
                return _switchToApplyingForCreditCommand ?? (_switchToApplyingForCreditCommand = new RelayCommand(ExecuteSwitchToApplyingForCreditCommand));
            }
        }

        private void ExecuteSwitchToApplyingForCreditCommand()
        {
            SwitchView(Main.ApplyingForCreditCode);
        }

        #endregion

        #region InitlizeCommand

        private RelayCommand _initlizeCommand;

        /// <summary>
        /// 初始化命令
        /// </summary>
        public RelayCommand InitlizeCommand
        {
            get
            {
                return _initlizeCommand ?? (_initlizeCommand = new RelayCommand(ExecuteInitlizeCommand, CanExecuteInitlizeCommand));
            }
        }

        private void ExecuteInitlizeCommand()
        {
            Initialize(null);
        }

        private bool CanExecuteInitlizeCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region RollOutCommand

        public ICommand RollOutCommand
        {
            get
            {
                return new RelayCommand<string>(param =>
                {
                    var currentProduct = AccountInfo.FinancialInfo.FinancialProducts.ToList().SingleOrDefault(p => p.SerialNum == param);
                    if (currentProduct == null)
                    {
                        UIManager.ShowMessage("加载产品信息错误,请稍候再试");
                        return;
                    }
                    LocalUIManager.OpenRollOutWindow(new RollOutViewModel(currentProduct));
                });
            }
        }

        #endregion

        #region CheckCreditCommand

        //private RelayCommand checkCreditCommand;

        ///// <summary>
        ///// 检查是否有信用权限，没有权限跳转到申请页面
        ///// </summary>
        //public RelayCommand CheckCreditCommand
        //{
        //    get
        //    {
        //        return checkCreditCommand ?? (checkCreditCommand = new RelayCommand(ExecuteCheckCreditCommand, CanExecuteCheckCreditCommand));
        //    }
        //}

        //private void ExecuteCheckCreditCommand()
        //{
        //    Action switchAction = new Action(() =>
        //    {
        //        if (!isRefreshAccountInfo)
        //            RefreshAccountInfo();

        //        while (isRefreshAccountInfo)
        //        {
        //            //正在刷新账户信息，等待刷新完成
        //            Thread.Sleep(300);
        //        }
        //        //没开通信用账户
        //        if (!HasOpenedCredit)
        //        {
        //            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
        //            {
        //                //切换到信用申请
        //                PluginService.Run(Main.ProjectCode, Main.ApplyingForCreditCode);
        //            }));
        //        }
        //    });

        //    Task.Factory.StartNew(switchAction);
        //}

        //private bool CanExecuteCheckCreditCommand()
        //{
        //    return true;
        //}

        #endregion

        #region BuyCommand

        /// <summary>
        /// 购买理财产品命令
        /// </summary>
        public ICommand BuyCommand
        {
            get
            {
                return new RelayCommand<string>(p =>
                {
                    var model = AllProducts.SingleOrDefault(x => x.ProductId == p);
                    if (model == null)
                    {
                        UIManager.ShowMessage("获取产品信息失败，请稍后再试!");
                        return;
                    }
                    LocalUIManager.OpenProductBuyWindow(new ProductBuyViewModel(model));
                });
            }
        }

        #endregion

        #region OpenProductsInfoCommand

        private RelayCommand<FinancialProductDto> _openProductsInfoCommand;

        /// <summary>
        /// 理财产品详情信息
        /// </summary>
        public RelayCommand<FinancialProductDto> OpenProductsInfoCommand
        {
            get
            {
                return _openProductsInfoCommand ?? (_openProductsInfoCommand = new RelayCommand<FinancialProductDto>(ExecuteOpenProductsInfoCommand, CanExecuteOpenProductsInfoCommand));
            }
        }

        private void ExecuteOpenProductsInfoCommand(FinancialProductDto info)
        {
            LocalUIManager.ShowFinancialProductInfo(info.ProductId);
        }

        private bool CanExecuteOpenProductsInfoCommand(FinancialProductDto info)
        {
            return info != null;
        }

        #endregion

        #region OpenProductsInfoByAccountCommand

        private RelayCommand<CurrentFinancialProductDto> _openProductsInfoByAccountCommand;

        /// <summary>
        /// 通过已购买理财产品打开界面
        /// </summary>
        public RelayCommand<CurrentFinancialProductDto> OpenProductsInfoByAccountCommand
        {
            get
            {
                return _openProductsInfoByAccountCommand ?? (_openProductsInfoByAccountCommand = new RelayCommand<CurrentFinancialProductDto>(ExecuteOpenProductsInfoByAccountCommand, CanExecuteOpenProductsInfoByAccountCommand));
            }
        }

        private void ExecuteOpenProductsInfoByAccountCommand(CurrentFinancialProductDto product)
        {
            LocalUIManager.ShowFinancialProductInfo(product.ProductID);
        }

        private bool CanExecuteOpenProductsInfoByAccountCommand(CurrentFinancialProductDto product)
        {
            return true;
        }

        #endregion

        #endregion

        #region 公开属性

        #region HasOpenedCredit

        /// <summary>
        /// The <see cref="HasOpenedCredit" /> property's name.
        /// </summary>
        private const string HasOpenedCreditPropertyName = "HasOpenedCredit";

        private bool _hasOpenedCredit;

        /// <summary>
        /// 是否已经开通信用账户
        /// </summary>
        public bool HasOpenedCredit
        {
            get { return _hasOpenedCredit; }

            set
            {
                if (_hasOpenedCredit == value) return;

                RaisePropertyChanging(HasOpenedCreditPropertyName);
                _hasOpenedCredit = value;
                RaisePropertyChanged(HasOpenedCreditPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否正在忙
        ///// </summary>
        //public bool IsBusy
        //{
        //    get { return isBusy; }

        //    set
        //    {
        //        if (isBusy == value) return;

        //        RaisePropertyChanging(IsBusyPropertyName);
        //        isBusy = value;
        //        RaisePropertyChanged(IsBusyPropertyName);
        //    }
        //}

        #endregion

        #region AccountInfo

        /// <summary>
        /// The <see cref="AccountInfo" /> property's name.
        /// </summary>
        private const string AccountInfoPropertyName = "AccountInfo";

        private AccountInfoDto _accountInfo;

        /// <summary>
        /// 账户详情
        /// </summary>
        public AccountInfoDto AccountInfo
        {
            get { return _accountInfo; }

            set
            {
                if (_accountInfo == value) return;

                RaisePropertyChanging(AccountInfoPropertyName);
                _accountInfo = value;
                RaisePropertyChanged(AccountInfoPropertyName);
            }
        }

        #endregion

        #region RepayInfo

        /// <summary>
        /// The <see cref="RepayInfo" /> property's name.
        /// </summary>
        private const string RepayInfoPropertyName = "RepayInfo";

        private RepayInfoDto _replayInfo;

        /// <summary>
        /// 还款信息
        /// </summary>
        public RepayInfoDto RepayInfo
        {
            get { return _replayInfo; }

            set
            {
                if (_replayInfo == value) return;

                RaisePropertyChanging(RepayInfoPropertyName);
                _replayInfo = value;
                RaisePropertyChanged(RepayInfoPropertyName);
            }
        }

        #endregion

        #region FinancialLogs

        /// <summary>
        /// The <see cref="FinancialLogs" /> property's name.
        /// </summary>
        private const string FinancialLogsPropertyName = "FinancialLogs";

        private ObservableCollection<FinancialLogDto> _financialLogs = new ObservableCollection<FinancialLogDto>();

        /// <summary>
        /// 显示的订单
        /// </summary>
        public ObservableCollection<FinancialLogDto> FinancialLogs
        {
            get { return _financialLogs; }

            set
            {
                if (_financialLogs == value) return;

                RaisePropertyChanging(FinancialLogsPropertyName);
                _financialLogs = value;
                RaisePropertyChanged(FinancialLogsPropertyName);
            }
        }

        #endregion

        #region AllProducts

        /// <summary>
        /// The <see cref="AllProducts" /> property's name.
        /// </summary>
        private const string AllProductsPropertyName = "AllProducts";

        private ObservableCollection<FinancialProductDto> _allProducts = new ObservableCollection<FinancialProductDto>();

        /// <summary>
        /// 所有理财产品
        /// </summary>
        public ObservableCollection<FinancialProductDto> AllProducts
        {
            get { return _allProducts; }

            set
            {
                if (_allProducts == value) return;

                RaisePropertyChanging(AllProductsPropertyName);
                _allProducts = value;
                RaisePropertyChanged(AllProductsPropertyName);
            }
        }

        #endregion

        #endregion

        #region 私有方法

        private void SwitchView(string code)
        {
            if (this.FullWidowExt != null && this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, code);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, code);
            } 
          
          
        }
        #endregion

        #region 公开方法
        /// <summary>
        /// 刷新还款数据
        /// </summary>
        internal void RefreshRepay(bool throwEx = false)
        {
            if (HasOpenedCredit == false)
            {
                return;
            }
            RepayInfo = null;

            CommunicateManager.Invoke<IAccountService>(service =>
            {
                RepayInfo = service.GetRepayInfo();
            }, ex =>
            {
                if (throwEx)
                    throw ex;
                Logger.WriteLog(LogType.WARN, "获取还款信息失败", ex);
            });
        }
        

        /// <summary>
        /// 刷新账户信息
        /// </summary>
        internal void RefreshAccountInfo(bool throwEx = false)
        {
            //isRefreshAccountInfo = true;
            AccountInfo = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                AccountInfo = service.GetAccountInfo();
                if (AccountInfo != null && AccountInfo.CreditInfo != null)
                {
                    HasOpenedCredit = AccountInfo.CreditInfo.Status;
                }
                //isRefreshAccountInfo = false;

            }, ex =>
            {
                //isRefreshAccountInfo = false;

                if (throwEx)
                    throw ex;
                Logger.WriteLog(LogType.WARN, "获取账户信息失败", ex);
            });
        }

        /// <summary>
        /// 刷新银行卡信息
        /// </summary>
        internal void RefreshBankCards(bool throwEx = false)
        {
            var bankVm = ViewModelLocator.BankCard;
            //初始化银行列表,如果错误不弹出消息框
            bankVm.RefreshBankCards();
        }

        /// <summary>
        /// 刷新理财日志
        /// </summary>
        public void RefreshFindFinancialLog(bool throwEx = false)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(FinancialLogs.Clear));

            CommunicateManager.Invoke<IAccountService>(service =>
            {
                DataPack<FinancialLogDto> dataPack = service.FindFinancialLog(null, null, 0, 15);

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<FinancialLogDto>(FinancialLogs.Add), item);
                }
            }, ex =>
            {
                if (throwEx)
                    throw ex;
                Logger.WriteLog(LogType.WARN, "获取理财信息失败", ex);
            });
        }

        /// <summary>
        /// 刷新所有理财产品
        /// </summary>
        /// <param name="throwEx"></param>
        private void RefreshAllFinance(bool throwEx = false)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(AllProducts.Clear));

            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                var result = p.GetAllProduct();
                if (result != null)
                    result.ForEach(x => DispatcherHelper.UIDispatcher.Invoke(new Action<FinancialProductDto>(AllProducts.Add), x));
            },
            ex =>
            {
                if (throwEx)
                    throw ex;
                Logger.WriteLog(LogType.WARN, "获取理财产品失败", ex);
            });
        }

        #endregion
    }
    public class RollOutViewModel : ViewModelBase
    {
        public RollOutViewModel(CurrentFinancialProductDto dto)
        {
            if (IsInDesignMode)
                return;

            CurrentFinancialProduct = dto;
            CommunicateManager.Invoke<IFinancialService>(p =>
            {
                ExpectProfit = p.GetExpectProfit(dto.TradeID.ToString(CultureInfo.InvariantCulture));
            }, UIManager.ShowErr);
        }
        public CurrentFinancialProductDto CurrentFinancialProduct { get; private set; }

        private string _payPassword;
        public string PayPassword
        {
            get { return _payPassword; }
            set
            {
                if (_payPassword == value) return;
                _payPassword = value;
                RaisePropertyChanged("PayPassword");
            }
        }

        #region ExpectProfit

        /// <summary>
        /// The <see cref="ExpectProfit" /> property's name.
        /// </summary>
        private const string ExpectProfitPropertyName = "ExpectProfit";

        private ExpectProfitDto _expectProfit;

        /// <summary>
        /// 收益
        /// </summary>
        public ExpectProfitDto ExpectProfit
        {
            get { return _expectProfit; }

            set
            {
                if (_expectProfit == value) return;

                RaisePropertyChanging(ExpectProfitPropertyName);
                _expectProfit = value;
                RaisePropertyChanged(ExpectProfitPropertyName);
            }
        }

        #endregion

        private RelayCommand _confirmRollOutCommand;

        public RelayCommand ConfirmRollOutCommand
        {
            get { return _confirmRollOutCommand ?? (_confirmRollOutCommand = new RelayCommand(ExecuteRollOutCommand)); }
        }
        //确认转出
        private void ExecuteRollOutCommand()
        {
            if (string.IsNullOrEmpty(PayPassword))
            {
                UIManager.ShowMessage("请输入支付密码!");
                return;
            }
            CommunicateManager.Invoke<IFinancialService>(p => p.AbortFinancial(CurrentFinancialProduct.TradeID.ToString(CultureInfo.InvariantCulture), PayPassword), UIManager.ShowErr);
            Messenger.Default.Send(true, "RollOutClose");
        }

        public RelayCommand CancelRollOutCommand
        {
            get
            {
                return new RelayCommand(() => Messenger.Default.Send(true, "RollOutClose"));
            }
        }
    }
}
