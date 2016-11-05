using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using JoveZhao.Framework;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 结算视图模型
    /// </summary>
    public class WithdrawDepositViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="WithdrawDepositViewModel"/> class.
        /// </summary>
        public WithdrawDepositViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsLoading = true;

            Action action = () =>
            {
                //支付成功
                var homeVm = ViewModelLocator.Home;
                //刷新数据
                homeVm.RefreshAccountInfo();
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });

            ExecutePageLoadCommand();
        }

        /// <summary>
        /// 页面加载后触发
        /// </summary>
        protected override void ExecutePageLoadCommand()
        {
            //银行卡视图模型
            var bankCardVm = ViewModelLocator.BankCard;
            if (bankCardVm.BankCards == null)
                return;

            var defaultCard = bankCardVm.BankCards.FirstOrDefault(m => m.IsDefault);
            SelectedBankCard = defaultCard;

            if (IsBusy) return;
            SetDefaultMoney();
            //Message = null;
            Password = null;
            GetFeeRules();
        }

        private void SetDefaultMoney()
        {
            Money = 0;
        }

        #endregion

        #region 公开属性

        #region IsLoading

        /// <summary>
        /// The <see cref="IsLoading" /> property's name.
        /// </summary>
        private const string IsLoadingPropertyName = "IsLoading";

        private bool _isLoading;

        /// <summary>
        /// 是否在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (_isLoading == value) return;

                RaisePropertyChanging(IsLoadingPropertyName);
                _isLoading = value;
                RaisePropertyChanged(IsLoadingPropertyName);
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
        ///// 是否在忙
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

        #region SelectedBankCard

        /// <summary>
        /// The <see cref="SelectedBankCard" /> property's name.
        /// </summary>
        private const string SelectedBankCardPropertyName = "SelectedBankCard";

        private BankCardDto _selectedBankCard;

        /// <summary>
        /// 选中的银行卡
        /// </summary>
        public BankCardDto SelectedBankCard
        {
            get { return _selectedBankCard; }

            set
            {
                if (_selectedBankCard == value) return;

                RaisePropertyChanging(SelectedBankCardPropertyName);
                _selectedBankCard = value;
                RaisePropertyChanged(SelectedBankCardPropertyName);
            }
        }

        #endregion

        #region Password

        /// <summary>
        /// The <see cref="Password" /> property's name.
        /// </summary>
        private const string PasswordPropertyName = "Password";

        private string _password;

        /// <summary>
        /// 支付pwd
        /// </summary>
        public string Password
        {
            get { return _password; }

            set
            {
                if (_password == value) return;

                RaisePropertyChanging(PasswordPropertyName);
                _password = value;
                RaisePropertyChanged(PasswordPropertyName);
            }
        }

        #endregion

        #region Money

        /// <summary>
        /// The <see cref="Money" /> property's name.
        /// </summary>
        private const string MoneyPropertyName = "Money";

        private decimal _money;

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money
        {
            get { return _money; }

            set
            {
                if (_money == value) return;

                RaisePropertyChanging(MoneyPropertyName);
                //money = value;
                var temp = decimal.Parse(value.ToString("F2"));
                _money = temp;

                GetFeeAmount();

                RaisePropertyChanged(MoneyPropertyName);
            }
        }

        #endregion

        #region IsNextDayToAccount

        /// <summary>
        /// The <see cref="IsNextDayToAccount" /> property's name.
        /// </summary>
        private const string IsNextDayToAccountPropertyName = "IsNextDayToAccount";

        private bool _isNextDayToAccount;// = true;

        /// <summary>
        /// 是否是次日到账
        /// </summary>
        public bool IsNextDayToAccount
        {
            get { return _isNextDayToAccount; }

            set
            {
                if (_isNextDayToAccount == value) return;

                RaisePropertyChanging(IsNextDayToAccountPropertyName);
                _isNextDayToAccount = value;
                RaisePropertyChanged(IsNextDayToAccountPropertyName);

                GetFeeAmount();
            }
        }

        #endregion

        #region IsHoliday

        /// <summary>
        /// The <see cref="IsHoliday" /> property's name.
        /// </summary>
        private const string IsHolidayPropertyName = "IsHoliday";

        private bool _isHoliday;

        /// <summary>
        /// 是否当天节假日
        /// </summary>
        public bool IsHoliday
        {
            get { return _isHoliday; }

            set
            {
                if (_isHoliday == value) return;
                RaisePropertyChanging(IsHolidayPropertyName);
                _isHoliday = value;
                RaisePropertyChanged(IsHolidayPropertyName);
            }
        }

        #endregion

        #region TodayVisible

        /// <summary>
        /// The <see cref="TodayVisible" /> property's name.
        /// </summary>
        private const string TodayVisiblePropertyName = "TodayVisible";

        private bool _todayVisible;

        /// <summary>
        /// 是否显示当日到账
        /// </summary>
        public bool TodayVisible
        {
            get { return _todayVisible; }

            set
            {
                if (_todayVisible == value) return;
                RaisePropertyChanging(TodayVisiblePropertyName);
                _todayVisible = value;
                RaisePropertyChanged(TodayVisiblePropertyName);
            }
        }

        #endregion

        #region TodayEnable

        /// <summary>
        /// The <see cref="TodayEnable" /> property's name.
        /// </summary>
        private const string TodayEnablePropertyName = "TodayEnable";

        private bool _todayEnable;

        /// <summary>
        /// 是否可用当日到账
        /// </summary>
        public bool TodayEnable
        {
            get { return _todayEnable; }

            set
            {
                if (_todayEnable == value) return;
                RaisePropertyChanging(TodayEnablePropertyName);
                _todayEnable = value;
                RaisePropertyChanged(TodayEnablePropertyName);
            }
        }

        #endregion

        #region TodayLast

        /// <summary>
        /// The <see cref="TodayLast" /> property's name.
        /// </summary>
        private const string TodayLastPropertyName = "TodayLast";

        private string _todayLast;

        /// <summary>
        /// 当日最迟提交时间
        /// </summary>
        public string TodayLast
        {
            get { return _todayLast; }
            set
            {
                if (_todayLast == value) return;
                RaisePropertyChanging(TodayLastPropertyName);
                _todayLast = value;
                if (_todayLast != null) _todayLast = string.Format("（请于{0}前提交）", _todayLast);
                RaisePropertyChanged(TodayLastPropertyName);
            }
        }

        #endregion

        #region TodayLastVisible

        /// <summary>
        /// The <see cref="TodayLastVisible" /> property's name.
        /// </summary>
        private const string TodayLastVisiblePropertyName = "TodayLastVisible";

        private bool _todayLastVisible;

        /// <summary>
        /// 是否显示当日最迟提交时间
        /// </summary>
        public bool TodayLastVisible
        {
            get { return _todayLastVisible; }
            set
            {
                if (_todayLastVisible == value) return;
                RaisePropertyChanging(TodayLastVisiblePropertyName);
                _todayLastVisible = value;
                RaisePropertyChanged(TodayLastVisiblePropertyName);
            }
        }

        #endregion

        #region MorrowEnable

        /// <summary>
        /// The <see cref="MorrowEnable" /> property's name.
        /// </summary>
        private const string MorrowEnablePropertyName = "MorrowEnable";

        private bool _morrowEnable;

        /// <summary>
        /// 是否显示次日到账
        /// </summary>
        public bool MorrowEnable
        {
            get { return _morrowEnable; }

            set
            {
                if (_morrowEnable == value) return;

                RaisePropertyChanging(MorrowEnablePropertyName);
                _morrowEnable = value;
                RaisePropertyChanged(MorrowEnablePropertyName);
            }
        }

        #endregion

        #region MorrowLast

        /// <summary>
        /// The <see cref="MorrowLast" /> property's name.
        /// </summary>
        private const string MorrowLastPropertyName = "MorrowLast";

        private string _morrowLast;

        /// <summary>
        /// 次日最迟提交时间
        /// </summary>
        public string MorrowLast
        {
            get { return _morrowLast; }
            set
            {
                if (_morrowLast == value) return;
                RaisePropertyChanging(MorrowLastPropertyName);
                _morrowLast = value;
                if (_morrowLast != null) _morrowLast = string.Format("（请于{0}前提交）", _morrowLast);
                RaisePropertyChanged(MorrowLastPropertyName);
            }
        }

        #endregion

        #region IsGetFeeAmount

        /// <summary>
        /// The <see cref="IsGetFeeAmount" /> property's name.
        /// </summary>
        private const string IsGetFeeAmountPropertyName = "IsGetFeeAmount";

        private bool _isGetFeeAmount;

        /// <summary>
        /// 是否正在获取手续费
        /// </summary>
        public bool IsGetFeeAmount
        {
            get { return _isGetFeeAmount; }

            set
            {
                if (_isGetFeeAmount == value) return;

                RaisePropertyChanging(IsGetFeeAmountPropertyName);
                _isGetFeeAmount = value;
                RaisePropertyChanged(IsGetFeeAmountPropertyName);
            }
        }

        #endregion

        #region FeeAmount

        /// <summary>
        /// The <see cref="FeeAmount" /> property's name.
        /// </summary>
        private const string FeeAmountPropertyName = "FeeAmount";

        private string _feeAmount;

        /// <summary>
        /// 手续费
        /// </summary>
        public string FeeAmount
        {
            get { return _feeAmount; }

            set
            {
                if (_feeAmount == value) return;

                var action = new Action(() =>
                {
                    RaisePropertyChanging(FeeAmountPropertyName);
                    _feeAmount = value;
                    RaisePropertyChanged(FeeAmountPropertyName);
                });

                var hasAccess = DispatcherHelper.UIDispatcher.CheckAccess();

                if (hasAccess)
                    action();
                else
                    DispatcherHelper.UIDispatcher.Invoke(action);
            }
        }

        #endregion

        #region TotalMoney

        /// <summary>
        /// The <see cref="TotalMoney" /> property's name.
        /// </summary>
        private const string TotalMoneyPropertyName = "TotalMoney";

        private decimal _totalMoney;

        /// <summary>
        /// 付款总额
        /// </summary>
        public decimal TotalMoney
        {
            get { return _totalMoney; }

            set
            {
                if (_totalMoney == value) return;

                RaisePropertyChanging(TotalMoneyPropertyName);
                _totalMoney = value;
                RaisePropertyChanged(TotalMoneyPropertyName);
            }
        }

        #endregion

        #region Description

        /// <summary>
        /// The <see cref="Description" /> property's name.
        /// </summary>
        private const string DescriptionPropertyName = "Description";

        private string _description;

        /// <summary>
        /// 描述信文字
        /// </summary>
        public string Description
        {
            get { return _description; }

            set
            {
                if (_description == value) return;

                RaisePropertyChanging(DescriptionPropertyName);
                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region CashOutCommand

        private RelayCommand _cashOutCommand;

        /// <summary>
        /// 结算命令
        /// </summary>
        public RelayCommand CashOutCommand
        {
            get
            {
                return _cashOutCommand ?? (_cashOutCommand = new RelayCommand(ExecuteCashOutCommand, CanExecuteCashOutCommand));
            }
        }

        private void ExecuteCashOutCommand()
        {
            IsBusy = true;
            //Message = null;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                service.CashOut(Money, _selectedBankCard.BankId.ToString(CultureInfo.InvariantCulture), Password, _isNextDayToAccount ? "1" : "0");
                //支付成功
                var homeVm = ViewModelLocator.Home;
                //刷新数据
                homeVm.RefreshAccountInfo();

                UIManager.ShowMessage("结算申请成功");
                //Message = "结算成功";//String.Format("结算成功 金额：【￥{0}】，时间{1}", outMoney, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                SetDefaultMoney();
                Password = null;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteCashOutCommand()
        {
            var can = !IsBusy && Money > 0 && SelectedBankCard != null && !String.IsNullOrEmpty(Password) && !IsGetFeeAmount;
            return can;
        }

        #endregion

        #region SwtichToLogViewCommand

        private RelayCommand _switchToLogViewCommand;

        /// <summary>
        /// 切换到最近日志
        /// </summary>
        public RelayCommand SwtichToLogViewCommand
        {
            get
            {
                return _switchToLogViewCommand ?? (_switchToLogViewCommand = new RelayCommand(ExecuteSwtichToLogViewCommand));
            }
        }

        private void ExecuteSwtichToLogViewCommand()
        {
          if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.WithdrawDepositLogCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {
                PluginService.Run(Main.ProjectCode, Main.WithdrawDepositLogCode);
            }  
           
        }

        #endregion

        #region AddBankCardCommand

        private RelayCommand _addBankCardCommand;

        /// <summary>
        /// 新增银行卡
        /// </summary>
        public RelayCommand AddBankCardCommand
        {
            get
            {
                return _addBankCardCommand ?? (_addBankCardCommand = new RelayCommand(ExecuteAddBankCardCommand, CanExecuteAddBankCardCommand));
            }
        }

        private void ExecuteAddBankCardCommand()
        {
            LocalUIManager.AddBank(resut =>
            {
                if (resut == null || !resut.Value) return;
                //添加完成刷新界面
                var homeVm = ViewModelLocator.Home;
                //刷新还款数据
                if (homeVm.InitlizeCommand.CanExecute(null))
                    homeVm.InitlizeCommand.Execute(null);
            });
        }

        private bool CanExecuteAddBankCardCommand()
        {
            return true;
        }

        #endregion

        #endregion

        #region 私有方法

        private void GetFeeAmount()
        {
            if (_isGetFeeAmount)
                return;

            IsGetFeeAmount = true;
            Description = FeeAmount = null;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                //if (ViewModelLocator.Home == null || ViewModelLocator.Home.AccountInfo == null)
                //    return;

                //var availableMoney = decimal.Parse(service.GetApplicationMaxAmount(isNextDayToAccount ? "1" : "0")); // ViewModelLocator.Home.AccountInfo.ReadyInfo.ReadyBalance - decimal.Parse(FeeAmount);

                //bool showDesc = false;
                //if (availableMoney <= 0)
                //{
                //    Description = "余额不足，不能结算";
                //}
                //else if (Money > availableMoney)
                //{
                //    Money = availableMoney;
                //    showDesc = true;
                //}

                FeeAmount = Money == 0 ? "0" : service.GetFeeAmount(_money.ToString(CultureInfo.InvariantCulture), _isNextDayToAccount ? "1" : "0");

                if (FeeAmount == null) return;
                TotalMoney = _money - decimal.Parse(FeeAmount);
                if (TotalMoney < 0)
                {
                    TotalMoney = 0;
                    UIManager.ShowMessage("结算金额不能小于手续费");
                }

                //if (showDesc)
                //    Description = String.Format("最多可结算金额：{0}元，剩余{1}元作为手续费", Money, FeeAmount);

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsGetFeeAmount = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }
        /// <summary>
        /// 获取手续费相关规则信息
        /// </summary>
        private void GetFeeRules()
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                var temp = service.GetFeeRule();
                IsHoliday = temp.IsHoliday;
                IsNextDayToAccount = MorrowEnable = temp.MorrowEnable;
                MorrowLast = temp.MorrowLast;
                TodayLast = temp.TodayLast;
                TodayVisible = temp.TodayEnable;
                TodayEnable = temp.TodayEnable && !IsHoliday; //如若当天节假日当日到账不可用
                TodayLastVisible = !IsNextDayToAccount && TodayEnable; //当日到账最迟提交时间显示判断(次日到账未选择且当日到账可用)
            }, ex => Logger.WriteLog(LogType.WARN, "获取手续费规则失败", ex));

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion
    }
}
