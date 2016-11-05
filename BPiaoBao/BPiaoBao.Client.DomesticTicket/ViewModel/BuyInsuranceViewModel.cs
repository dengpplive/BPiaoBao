using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class BuyInsuranceViewModel : ViewModelBase
    {
        /// <summary>
        /// 购买保险实体参数
        /// </summary>
        RequestPurchaseFromCarrier repurchase = new RequestPurchaseFromCarrier();

        #region 公开属性

        #region Count 保险份数

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        private const string CountPropertyName = "Count";

        private int _count;//默认

        /// <summary>
        /// 保险份数
        /// </summary>
        public int Count
        {
            get
            {
                //保险份数不足判断
                return _count > InsuranceLeaveCount ? InsuranceLeaveCount : _count;
            }

            set
            {
                if (_count == value) return;

                RaisePropertyChanging(CountPropertyName);
                _count = value;
                //保险份数不足判断
                if (_count > InsuranceLeaveCount) { UIManager.ShowMessage("最多购买保险份数为"+ InsuranceLeaveCount +"份"); _count = InsuranceLeaveCount; }
                RaisePropertyChanged(CountPropertyName);
                //保险总额
                SumPrice = UnexpectedPrice * Count;

            }
        }

        #endregion

        #region IsPaid

        /// <summary>
        /// The <see cref="IsPaid" /> property's name.
        /// </summary>
        private const string IsPaidPropertyName = "IsPaid";

        private bool _isPaid;

        /// <summary>
        /// 是否已经支付
        /// </summary>
        public bool IsPaid
        {
            get { return _isPaid; }

            set
            {
                if (_isPaid == value) return;

                RaisePropertyChanging(IsPaidPropertyName);
                _isPaid = value;
                RaisePropertyChanged(IsPaidPropertyName);
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
        /// 是否正在加载
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

        #region AccountInfo

        /// <summary>
        /// The <see cref="AccountInfo" /> property's name.
        /// </summary>
        public const string AccountInfoPropertyName = "AccountInfo";

        private AccountInfoDto _accountInfo;

        /// <summary>
        /// 账户信息
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

        #region InsuranceLeaveCount

        /// <summary>
        /// The <see cref="InsuranceLeaveCount" /> property's name.
        /// </summary>
        private const string InsuranceLeaveCountPropertyName = "InsuranceLeaveCount";

        private int _insuranceLeaveCount;

        /// <summary>
        /// 保险剩余张数
        /// </summary>
        public int InsuranceLeaveCount
        {
            get { return _insuranceLeaveCount; }

            set
            {
                if (_insuranceLeaveCount == value) return;

                RaisePropertyChanging(InsuranceLeaveCountPropertyName);
                _insuranceLeaveCount = value;
                RaisePropertyChanged(InsuranceLeaveCountPropertyName);
            }
        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        private const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy;

        /// <summary>
        /// 是否正在繁忙
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

        #region UnexpectedPrice

        /// <summary>
        /// The <see cref="UnexpectedPrice" /> property's name.
        /// </summary>
        private const string UnexpectedPricePropertyName = "UnexpectedPrice";

        private decimal _unexpectedPrice;

        /// <summary>
        /// 保险价格
        /// </summary>
        public decimal UnexpectedPrice
        {
            get { return _unexpectedPrice; }

            set
            {
                if (_unexpectedPrice == value) return;

                RaisePropertyChanging(UnexpectedPricePropertyName);
                _unexpectedPrice = value;
                RaisePropertyChanged(UnexpectedPricePropertyName);
            }
        }

        #endregion

        #region SumPrice

        /// <summary>
        /// The <see cref="SumPrice" /> property's name.
        /// </summary>
        private const string SumPricePropertyName = "SumPrice";

        private decimal _sumPrice;

        /// <summary>
        /// 结算总金额
        /// </summary>
        public decimal SumPrice
        {
            get { return _sumPrice; }

            set
            {
                if (_sumPrice == value) return;

                RaisePropertyChanging(SumPricePropertyName);
                _sumPrice = value;
                RaisePropertyChanged(SumPricePropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region InitlizeCommand

        private RelayCommand _initlizeCommand;

        /// <summary>
        /// 初始化命令.
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
            IsLoading = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IInsuranceService>(service =>
                {
                    #region 保险相关设置
                    var re = service.GetCurentInsuranceCfgInfo();
                    if (!re.IsOpenCurrenCarrierInsurance || !re.IsOpenUnexpectedInsurance) return;
                    UnexpectedPrice = re.UnexpectedSinglePrice;
                    InsuranceLeaveCount = re.LeaveCount;

                    #endregion
                }, UIManager.ShowErr);

                CommunicateManager.Invoke<IAccountService>(service =>
                {
                    AccountInfo = service.GetAccountInfo();
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteInitlizeCommand()
        {
            return !IsLoading;
        }

        #endregion

        #region PayOrderByCashbagAccountCommand

        private RelayCommand<string> _payOrderByCashbagAccountCommand;

        /// <summary>
        /// Gets the PayOrderByCashbagAccountCommand.
        /// </summary>
        public RelayCommand<string> PayOrderByCashbagAccountCommand
        {
            get
            {
                return _payOrderByCashbagAccountCommand ?? (_payOrderByCashbagAccountCommand = new RelayCommand<string>(ExecutePayOrderByCashbagAccountCommand, CanExecutePayOrderByCashbagAccountCommand));
            }
        }

        private void ExecutePayOrderByCashbagAccountCommand(string password)
        {
            BuyInsuranceAction(password,0);
        }

        private bool CanExecutePayOrderByCashbagAccountCommand(string password)
        {
            return !_isBusy && !String.IsNullOrWhiteSpace(password);
        }

        #endregion

        #region PayOrderByCreditAccountCommand

        private RelayCommand<string> _payOrderByCreditAccountCommand;

        /// <summary>
        /// Gets the PayOrderByCreditAccountCommand.
        /// </summary>
        public RelayCommand<string> PayOrderByCreditAccountCommand
        {
            get
            {
                return _payOrderByCreditAccountCommand ?? (_payOrderByCreditAccountCommand = new RelayCommand<string>(ExecutePayOrderByCreditAccountCommand, CanExecutePayOrderByCreditAccountCommand));
            }
        }

        private void ExecutePayOrderByCreditAccountCommand(string password)
        {
            BuyInsuranceAction(password,1);
        }

        private bool CanExecutePayOrderByCreditAccountCommand(string password)
        {
            return !_isBusy && !String.IsNullOrWhiteSpace(password) && !IsPaid;
        }

        #endregion

        #endregion

        /// <summary>
        /// 购买保险操作
        /// </summary>
        /// <param name="password"></param>
        /// <param name="payMethod"></param>
        private void BuyInsuranceAction(string password,int payMethod)
        {
            //没有购买保险操作
            if (Count == 0)
            {
                var dialog = UIManager.ShowMessageDialog("您还没有购买保险，确定需要购买？");
                if (dialog != null && !(bool)dialog) IsPaid = true; return;
            }
            if (repurchase == null) return;
            repurchase.buyCount = Count;
            repurchase.pwd = password;
            repurchase.payMethod = payMethod == 0 ? Common.Enums.EnumPayMethod.Account : Common.Enums.EnumPayMethod.Credit;
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                service.PurchaseInsuranceFromCarrier(repurchase);
                UIManager.ShowMessage("支付成功！");
                IsPaid = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }
    }
}
