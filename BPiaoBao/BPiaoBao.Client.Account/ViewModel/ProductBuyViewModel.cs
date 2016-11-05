using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace BPiaoBao.Client.Account.ViewModel
{
    public class ProductBuyViewModel : ViewModelBase, IDataErrorInfo
    {
        #region 成员变量

        private DispatcherTimer _timer;
        private TimeSpan _countdownTimeSpan;

        #endregion

        #region 构造函数

        public ProductBuyViewModel(FinancialProductDto currentProduct)
        {
            IsBusy = true;

            CurrentProduct = currentProduct;
            BuyPrice = currentProduct.LimitAmount;
            var action = new Action(() =>
            {
                RefreshRecieveMoney(true);

                CommunicateManager.Invoke<IBusinessmanService>(service =>
                {
                    var serverTime = service.GetServerTime();

                    CountdownTimeSpan = currentProduct.EndDate - serverTime;
                    //小于等于5天显示支付
                    IsShowCountdown = _countdownTimeSpan <= TimeSpan.FromDays(5);
                }, UIManager.ShowErr);
            });


            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    IsBusy = false;
                }));
            });
        }

        /// <summary>
        /// 刷新余额
        /// </summary>
        private void RefreshRecieveMoney(bool showErr = false)
        {
           
            //从现金帐户里去获取可用余额
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                RecieveMoney = service.GetAccountInfo().ReadyInfo.ReadyBalance;

            }, ex =>
                {
                    if (showErr)
                        UIManager.ShowErr(ex);
                });
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CountdownTimeSpan = _countdownTimeSpan.Add(TimeSpan.FromSeconds(-1));
        }

        /// <summary>
        /// Unregisters this instance from the Messenger class.
        /// <para>To cleanup additional resources, override this method, clean
        /// up and then call base.Cleanup().</para>
        /// </summary>
        public override void Cleanup()
        {
            if (_timer != null)
                _timer.Stop();
            _timer = null;
            base.Cleanup();
        }

        #endregion

        #region 公开属性

        private decimal _recieveMoney;

        public decimal RecieveMoney
        {
            get { return _recieveMoney; }
            set
            {
                if (_recieveMoney != value)
                {
                    _recieveMoney = value;
                    RaisePropertyChanged("RecieveMoney");
                }
            }
        }

        /// <summary>
        /// 购买产品价格
        /// </summary>
        private decimal _buyPrice;
        public decimal BuyPrice
        {
            get { return _buyPrice; }
            set
            {
                if (_buyPrice == value) return;
                var temp = decimal.Parse(value.ToString("F2"));
                _buyPrice = temp;
                RaisePropertyChanged("BuyPrice");

                if (DateTime.Today > CurrentProduct.EndDate)
                    GainsPoints = 0;
                else
                {
                    //总金额*年利率/365 *周期*100
                    var result = BuyPrice * CurrentProduct.ReturnRate / 365 * CurrentProduct.Day * 100;
                    GainsPoints = int.Parse(result.ToString("f0"));
                }
            }
        }

        #region CurrentProduct

        /// <summary>
        /// The <see cref="CurrentProduct" /> property's name.
        /// </summary>
        private const string CurrentProductPropertyName = "CurrentProduct";

        private FinancialProductDto _currentProduct;

        /// <summary>
        /// 当前理财产品
        /// </summary>
        public FinancialProductDto CurrentProduct
        {
            get { return _currentProduct; }

            set
            {
                if (_currentProduct == value) return;

                RaisePropertyChanging(CurrentProductPropertyName);
                _currentProduct = value;
                RaisePropertyChanged(CurrentProductPropertyName);

                if (value != null)
                    ToDay = DateTime.Today.AddDays(value.Day);
            }
        }

        #endregion

        public bool IsCanBuyCommand
        {
            get
            {
                return BuyPrice >= CurrentProduct.LimitAmount;
            }
        }

        public string Error
        {
            get { return string.Empty; }
        }

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        private const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy;

        /// <summary>
        /// 是否在忙
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        #endregion

        #region ToDay

        /// <summary>
        /// The <see cref="ToDay" /> property's name.
        /// </summary>
        private const string ToDayPropertyName = "ToDay";

        private DateTime _toDay;

        /// <summary>
        /// 到期日
        /// </summary>
        public DateTime ToDay
        {
            get { return _toDay; }

            set
            {
                if (_toDay == value) return;

                RaisePropertyChanging(ToDayPropertyName);
                _toDay = value;
                RaisePropertyChanged(ToDayPropertyName);
            }
        }

        #endregion

        public string this[string columnName]
        {
            get
            {
                if (columnName != "BuyPrice") return string.Empty;
                return _buyPrice < CurrentProduct.LimitAmount ? "购买金额不能小于最低额度!" : string.Empty;
            }
        }

        #region IsAccept

        /// <summary>
        /// The <see cref="IsAccept" /> property's name.
        /// </summary>
        private const string IsAcceptPropertyName = "IsAccept";

        private bool _isAccept;

        /// <summary>
        /// 是否接受协议
        /// </summary>
        public bool IsAccept
        {
            get { return _isAccept; }

            set
            {
                if (_isAccept == value) return;

                RaisePropertyChanging(IsAcceptPropertyName);
                _isAccept = value;
                RaisePropertyChanged(IsAcceptPropertyName);
            }
        }

        #endregion

        #region GainsPoints

        /// <summary>
        /// The <see cref="GainsPoints" /> property's name.
        /// </summary>
        private const string GainsPointsPropertyName = "GainsPoints";

        private int _gainsPoints;

        /// <summary>
        /// 当前金额，预期收益积分
        /// </summary>
        public int GainsPoints
        {
            get { return _gainsPoints; }

            set
            {
                if (_gainsPoints == value) return;

                RaisePropertyChanging(GainsPointsPropertyName);
                _gainsPoints = value;
                RaisePropertyChanged(GainsPointsPropertyName);
            }
        }

        #endregion

        #region Countdown

        /// <summary>
        /// The <see cref="Countdown" /> property's name.
        /// </summary>
        private const string CountdownPropertyName = "Countdown";

        private string _countdown;

        /// <summary>
        /// 理财倒计时
        /// </summary>
        public string Countdown
        {
            get { return _countdown; }

            set
            {
                if (_countdown == value) return;

                RaisePropertyChanging(CountdownPropertyName);
                _countdown = value;
                RaisePropertyChanged(CountdownPropertyName);
            }
        }

        #endregion

        public TimeSpan CountdownTimeSpan
        {
            get { return _countdownTimeSpan; }
            set
            {
                _countdownTimeSpan = value;
                Countdown = String.Format("{0}天{1}时{2}分{3}秒", _countdownTimeSpan.Days, _countdownTimeSpan.Hours, _countdownTimeSpan.Minutes, _countdownTimeSpan.Seconds);
            }
        }

        #region IsShowCountdown

        /// <summary>
        /// The <see cref="IsShowCountdown" /> property's name.
        /// </summary>
        private const string IsShowCountdownPropertyName = "IsShowCountdown";

        private bool _isShowCountdown;

        /// <summary>
        /// 是否显示倒计时
        /// </summary>
        public bool IsShowCountdown
        {
            get { return _isShowCountdown; }

            set
            {
                if (_isShowCountdown == value) return;

                RaisePropertyChanging(IsShowCountdownPropertyName);
                _isShowCountdown = value;
                RaisePropertyChanged(IsShowCountdownPropertyName);

                if (value)
                {
                    if (_timer != null)
                        return;
                    DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                    {
                        _timer = new DispatcherTimer();
                        _timer.Interval = TimeSpan.FromSeconds(1);
                        _timer.Tick += timer_Tick;
                        _timer.Start();
                    }));
                }
                else
                {
                    if (_timer == null)
                        return;
                    _timer.Stop();
                    _timer.Tick -= timer_Tick;
                    _timer = null;
                }
            }
        }

        #endregion

        #endregion

        #region 公开命令

        /// <summary>
        /// 账户支付命令
        /// </summary>
        public ICommand PayAccountCommand
        {
            get
            {
                return new RelayCommand<object>(param =>
                {
                    var pb = param as System.Windows.Controls.PasswordBox;
                    if (pb == null) return;
                    var password = pb.Password;
                    if (string.IsNullOrEmpty(password))
                    {
                        UIManager.ShowMessage("请输入支付密码");
                        pb.Focus();
                        return;
                    }

                    if (!IsAccept)
                    {
                        UIManager.ShowMessage("请先接受协议");
                        return;
                    }

                    CommunicateManager.Invoke<IFinancialService>(p =>
                    {
                        p.BuyFinancialProductByCashAccount(CurrentProduct.ProductId, BuyPrice, password);
                        var result = MessageBoxExt.Show("提示", "购买成功!", MessageImageType.Info);
                        if (result.HasValue == false || result.Value)
                            Messenger.Default.Send(true, "CloseProductBuy");
                    }, UIManager.ShowErr);
                }, param => IsCanBuyCommand);
            }
        }
        /// <summary>
        /// 银行卡支付命令
        /// </summary>
        public ICommand PayCommand
        {
            get
            {
                return new RelayCommand<string>(param =>
                {
                    if (string.IsNullOrEmpty(param))
                    {
                        UIManager.ShowMessage("请选择银行支付");
                        return;
                    }

                    if (!IsAccept)
                    {
                        UIManager.ShowMessage("请先接受协议");
                        return;
                    }

                    CommunicateManager.Invoke<IFinancialService>(p =>
                    {
                        var resultUrl = p.BuyFinancialProductByBank(CurrentProduct.ProductId, BuyPrice, param);
                        LocalUIManager.OpenBrowser(resultUrl);
                    }, UIManager.ShowErr);
                }, param => IsCanBuyCommand);
            }
        }
        /// <summary>
        /// 支付平台支付命令
        /// </summary>
        public ICommand PayPlatformCommand
        {
            get
            {
                return new RelayCommand<string>(param =>
                {
                    if (string.IsNullOrEmpty(param))
                    {
                        UIManager.ShowMessage("请选择支付平台");
                        return;
                    }

                    if (!IsAccept)
                    {
                        UIManager.ShowMessage("请先接受协议");
                        return;
                    }

                    CommunicateManager.Invoke<IFinancialService>(p =>
                    {
                        string resultUrl = p.BuyFinancialProductByPlatform(CurrentProduct.ProductId, BuyPrice, param);
                        LocalUIManager.OpenBrowser(resultUrl);
                    }, UIManager.ShowErr);

                }, param => IsCanBuyCommand);
            }
        }

        #region OpenAgreementCommand

        private RelayCommand _openAgreementCommand;

        /// <summary>
        /// 打开协议命令
        /// </summary>
        public RelayCommand OpenAgreementCommand
        {
            get
            {
                return _openAgreementCommand ?? (_openAgreementCommand = new RelayCommand(ExecuteOpenAgreementCommand, CanExecuteOpenAgreementCommand));
            }
        }

        private void ExecuteOpenAgreementCommand()
        {
            //string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //UIManager.ShowWeb("购买协议", String.Format("{0}/FinanceAgreement.html", runDir));

            UIManager.ShowWeb("购买协议", "http://www.51cbc.cn/financialAgreement.html");


            //pack://siteoforigin:,,,/FinanceAgreement.html
        }

        private bool CanExecuteOpenAgreementCommand()
        {
            return true;
        }

        #endregion

        #region OpenRechargeCommand

        private RelayCommand _openRechargeCommand;

        /// <summary>
        /// 打开充值界面
        /// </summary>
        public RelayCommand OpenRechargeCommand
        {
            get
            {
                return _openRechargeCommand ?? (_openRechargeCommand = new RelayCommand(ExecuteOpenRechargeCommand, CanExecuteOpenRechargeCommand));
            }
        }

        private void ExecuteOpenRechargeCommand()
        {
            LocalUIManager.ShowRecharge(() =>
            {
                IsBusy = true;

                //刷新金额
                Action action = () => RefreshRecieveMoney();

                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    var setBusy = new Action(() =>
                    {
                        IsBusy = false;
                    });
                    DispatcherHelper.UIDispatcher.Invoke(setBusy);
                });
            });
        }

        private bool CanExecuteOpenRechargeCommand()
        {
            return true;
        }

        #endregion

        #endregion
    }
}
