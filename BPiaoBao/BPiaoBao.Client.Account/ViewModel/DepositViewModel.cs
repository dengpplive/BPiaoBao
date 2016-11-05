using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 充值视图模型
    /// </summary>
    public class DepositViewModel : BaseVM
    {
        #region 构造函数

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
        }

        #endregion

        #region 公开属性

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public new bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_rechargeByBankCommand != null)
                    _rechargeByBankCommand.RaiseCanExecuteChanged();
                if (_rechargeByPlatformCommand != null)
                    _rechargeByPlatformCommand.RaiseCanExecuteChanged();
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
        /// 充值金额
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
                RaisePropertyChanged(MoneyPropertyName);
            }
        }

        #endregion

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

        #endregion

        #region 公开命令

        #region RechargeByBankCommand

        private RelayCommand<string> _rechargeByBankCommand;

        /// <summary>
        /// 通过银行充值
        /// </summary>
        public RelayCommand<string> RechargeByBankCommand
        {
            get
            {
                return _rechargeByBankCommand ?? (_rechargeByBankCommand = new RelayCommand<string>(ExecuteRechargeByBankCommand, CanExecuteRechargeByBankCommand));
            }
        }

        private void ExecuteRechargeByBankCommand(string code)
        {
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                var uri = service.RechargeByBank(_money, code);
                LocalUIManager.OpenDefaultBrowser(uri);
                var result = UIManager.ShowPayWindow();
                if (result != null && result.Value)
                    Initialize();//刷新余额

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteRechargeByBankCommand(string code)
        {
            return !isBusy && !String.IsNullOrWhiteSpace(code) && _money > 0;
        }

        #endregion

        #region RechargeByPlatformCommand

        private RelayCommand<string> _rechargeByPlatformCommand;

        /// <summary>
        /// 通过第三方充值
        /// </summary>
        public RelayCommand<string> RechargeByPlatformCommand
        {
            get
            {
                return _rechargeByPlatformCommand ?? (_rechargeByPlatformCommand = new RelayCommand<string>(ExecuteRechargeByPlatformCommand, CanExecuteRechargeByPlatformCommand));
            }
        }

        private void ExecuteRechargeByPlatformCommand(string code)
        {
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                var uri = service.RechargeByPlatform(_money, code);
                LocalUIManager.OpenDefaultBrowser(uri);

                var result = UIManager.ShowPayWindow();
                if (result != null && result.Value)
                    Initialize();//刷新余额

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteRechargeByPlatformCommand(string code)
        {
            return !isBusy && !String.IsNullOrWhiteSpace(code) && _money > 0;
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
            if (this.FullWidowExt != null && this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.DepositLogCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.DepositLogCode);
            }

        }

        #endregion

        #region RechargeByQuikAliPayCommand

        private RelayCommand<string> _rechargeByQuikAliPayCommand;

        /// <summary>
        /// 支付宝快捷支付充值
        /// </summary>
        public RelayCommand<string> RechargeByQuikAliPayCommand
        {
            get
            {
                return _rechargeByQuikAliPayCommand ?? (_rechargeByQuikAliPayCommand = new RelayCommand<string>(ExecuteRechargeByQuikAliPayCommand, CanExecuteRechargeByQuikAliPayCommand));
            }
        }

        private void ExecuteRechargeByQuikAliPayCommand(string pwd)
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                service.AlipaySignRecharge(_money, pwd);
                UIManager.ShowMessage("充值成功！");
                Initialize();//刷新余额
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteRechargeByQuikAliPayCommand(string pwd)
        {
            return !isBusy && !string.IsNullOrEmpty(pwd) && _money > 0;
        }

        #endregion

        #endregion
    }
}
