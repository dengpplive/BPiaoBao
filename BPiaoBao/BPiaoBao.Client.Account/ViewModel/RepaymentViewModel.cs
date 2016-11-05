using System.Globalization;
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
    /// 还款视图模型
    /// </summary>
    public class RepaymentViewModel : BaseVM
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
                //刷新现金余额
                homeVm.RefreshAccountInfo();
                //刷新信用账户
                homeVm.RefreshRepay();
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
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

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否在忙碌状态
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

                if (_repayMoneyByBankCommand != null)
                    _repayMoneyByBankCommand.RaiseCanExecuteChanged();
                if (_repayMoneyByCashAccountCommand != null)
                    _repayMoneyByCashAccountCommand.RaiseCanExecuteChanged();
                if (_repayMoneyByPlatformCommand != null)
                    _repayMoneyByPlatformCommand.RaiseCanExecuteChanged();
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
        /// 支付密码
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

        #region RepayMoney

        /// <summary>
        /// The <see cref="RepayMoney" /> property's name.
        /// </summary>
        private const string RepayMoneyPropertyName = "RepayMoney";

        private decimal _repayMoney;

        /// <summary>
        /// 还款金额
        /// </summary>
        public decimal RepayMoney
        {
            get { return _repayMoney; }

            set
            {
                if (_repayMoney == value) return;

                RaisePropertyChanging(RepayMoneyPropertyName);
                //repayMoney = value;
                var temp = decimal.Parse(value.ToString("F2"));
                _repayMoney = temp;
                RaisePropertyChanged(RepayMoneyPropertyName);
            }
        }

        #endregion

        #region Message

        ///// <summary>
        ///// The <see cref="Message" /> property's name.
        ///// </summary>
        //public const string MessagePropertyName = "Message";

        //private string message = null;

        ///// <summary>
        ///// 消息
        ///// </summary>
        //public string Message
        //{
        //    get { return message; }

        //    set
        //    {
        //        if (message == value) return;

        //        RaisePropertyChanging(MessagePropertyName);
        //        message = value;
        //        RaisePropertyChanged(MessagePropertyName);
        //    }
        //}

        #endregion

        #endregion

        #region 公开命令

        #region RepayMoneyByQuikAliPayCommand

        private RelayCommand _repayMoneyByQuikAliPayCommand;

        /// <summary>
        /// 支付宝快支付还款
        /// </summary>
        public RelayCommand RepayMoneyByQuikAliPayCommand
        {
            get
            {
                return _repayMoneyByQuikAliPayCommand ?? (_repayMoneyByQuikAliPayCommand = new RelayCommand(ExecuteRepayMoneyByQuikAliPayCommand, CanExecuteRepayMoneyByQuikAliPayCommand));
            }
        }

        private void ExecuteRepayMoneyByQuikAliPayCommand()
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                service.AlipaySignRepay(_repayMoney, _password);
                //支付成功
                Initialize();
                //清空
                CleanTxt();
                UIManager.ShowMessage("还款成功！");
                //Message = "还款成功";
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteRepayMoneyByQuikAliPayCommand()
        {
            return !isBusy && !String.IsNullOrWhiteSpace(_password) && RepayMoney > 0;
        }

        #endregion

        #region RepayMoneyByCashAccountCommand

        private RelayCommand _repayMoneyByCashAccountCommand;

        /// <summary>
        /// 现金账户还款
        /// </summary>
        public RelayCommand RepayMoneyByCashAccountCommand
        {
            get
            {
                return _repayMoneyByCashAccountCommand ?? (_repayMoneyByCashAccountCommand = new RelayCommand(ExecuteRepayMoneyByCashAccountCommand, CanExecuteRepayMoneyByCashAccountCommand));
            }
        }

        private void ExecuteRepayMoneyByCashAccountCommand()
        {
            IsBusy = true;
            //Message = null;
            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                service.RepayMoneyByCashAccount(_repayMoney.ToString(CultureInfo.InvariantCulture), _password);
                //支付成功
                Initialize();
                //清空
                CleanTxt();
                UIManager.ShowMessage("还款成功！");
                //Message = "还款成功";
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteRepayMoneyByCashAccountCommand()
        {
            return !isBusy && !String.IsNullOrWhiteSpace(_password) && RepayMoney > 0;
        }

        #endregion

        #region RepayMoneyByBankCommand

        private RelayCommand<string> _repayMoneyByBankCommand;

        /// <summary>
        /// 银行卡还款
        /// </summary>
        public RelayCommand<string> RepayMoneyByBankCommand
        {
            get
            {
                return _repayMoneyByBankCommand ?? (_repayMoneyByBankCommand = new RelayCommand<string>(ExecuteRepayMoneyByBankCommand, CanExecuteRepayMoneyByBankCommand));
            }
        }

        private void ExecuteRepayMoneyByBankCommand(string code)
        {
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                var uri = service.RepayMoneyByBank(_repayMoney.ToString(CultureInfo.InvariantCulture), code);
                LocalUIManager.OpenDefaultBrowser(uri);

                var result = UIManager.ShowPayWindow();
                if (result != null && result.Value)
                    Initialize();//刷新余额

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteRepayMoneyByBankCommand(string code)
        {
            return !isBusy && !String.IsNullOrWhiteSpace(code) && RepayMoney > 0;
        }

        #endregion

        #region RepayMoneyByPlatformCommand

        private RelayCommand<string> _repayMoneyByPlatformCommand;

        /// <summary>
        /// 第三方平台还款
        /// </summary>
        public RelayCommand<string> RepayMoneyByPlatformCommand
        {
            get
            {
                return _repayMoneyByPlatformCommand ?? (_repayMoneyByPlatformCommand = new RelayCommand<string>(ExecuteRepayMoneyByPlatformCommand, CanExecuteRepayMoneyByPlatformCommand));
            }
        }

        private void ExecuteRepayMoneyByPlatformCommand(string code)
        {
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                var uri = service.RepayMoneyByPlatform(_repayMoney.ToString(CultureInfo.InvariantCulture), code);
                LocalUIManager.OpenDefaultBrowser(uri);

                var result = UIManager.ShowPayWindow();
                if (result != null && result.Value)
                    Initialize();//刷新余额

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteRepayMoneyByPlatformCommand(string code)
        {
            return !isBusy && !String.IsNullOrWhiteSpace(code) && RepayMoney > 0;
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
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.BillRePayDetailCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {
                PluginService.Run(Main.ProjectCode, Main.BillRePayDetailCode);
            }  
          
        }

        #endregion

        #endregion

        protected override void ExecutePageLoadCommand()
        {
            CleanTxt();
        }

        private void CleanTxt()
        {
            RepayMoney = 0;
            Password = null;
            //Message = null;
        }
    }
}
