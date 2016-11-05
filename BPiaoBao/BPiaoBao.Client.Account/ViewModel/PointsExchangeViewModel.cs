using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 积分兑换界面视图模型
    /// </summary>
    public class PointsExchangeViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsExchangeViewModel"/> class.
        /// </summary>
        public PointsExchangeViewModel()
        {
            if (IsInDesignMode)
                return;
            AccountInfo = ViewModelLocator.Home.AccountInfo;
            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            RefreshAccountInfo();
        }

        //刷新账户信息
        private void RefreshAccountInfo()
        {
            IsBusy = true;
            Action action = () => ViewModelLocator.Home.RefreshAccountInfo();

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                IsBusy = false;
            });
        }

        #endregion

        #region 公开属性

        #region IsExchangeSuccess

        /// <summary>
        /// The <see cref="IsExchangeSuccess" /> property's name.
        /// </summary>
        private const string IsExchangeSuccessPropertyName = "IsExchangeSuccess";

        private bool _isExchangeSuccess;

        /// <summary>
        /// 是否兑换成功
        /// </summary>
        public bool IsExchangeSuccess
        {
            get { return _isExchangeSuccess; }

            set
            {
                if (_isExchangeSuccess == value) return;

                RaisePropertyChanging(IsExchangeSuccessPropertyName);
                _isExchangeSuccess = value;
                RaisePropertyChanged(IsExchangeSuccessPropertyName);
            }
        }

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

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否繁忙
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

        #region Points

        /// <summary>
        /// The <see cref="Points" /> property's name.
        /// </summary>
        private const string PointsPropertyName = "Points";

        private decimal _points;

        /// <summary>
        /// 兑换的积分
        /// </summary>
        public decimal Points
        {
            get { return _points; }

            set
            {
                if (_points == value) return;

                RaisePropertyChanging(PointsPropertyName);
                _points = value;
                RaisePropertyChanged(PointsPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region ExchangeCommand

        private RelayCommand _exchangeCommand;

        /// <summary>
        /// 兑换命令
        /// </summary>
        public RelayCommand ExchangeCommand
        {
            get
            {
                return _exchangeCommand ?? (_exchangeCommand = new RelayCommand(ExecuteExchangeCommand, CanExecuteExchangeCommand));
            }
        }

        /// <summary>
        /// Executes the exchange command.
        /// </summary>
        private void ExecuteExchangeCommand()
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.ExchangeSource(_points);
                UIManager.ShowMessage("兑换成功");
                IsExchangeSuccess = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                IsBusy = false;
            });
        }

        private bool CanExecuteExchangeCommand()
        {
            return true;
        }

        #endregion

        #endregion
    }
}
