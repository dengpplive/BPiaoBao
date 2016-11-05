using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 积分账户 视图模型
    /// </summary>
    public class PointsViewModel : PageBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointsViewModel"/> class.
        /// </summary>
        public PointsViewModel()
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
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();

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

        #region 公开属性

        #region BalanceDetails

        /// <summary>
        /// The <see cref="BalanceDetails" /> property's name.
        /// </summary>
        private const string BalanceDetailsPropertyName = "BalanceDetails";

        private ObservableCollection<BalanceDetailDto> _balanceDetail = new ObservableCollection<BalanceDetailDto>();

        /// <summary>
        /// 现金账户明细
        /// </summary>
        public ObservableCollection<BalanceDetailDto> BalanceDetails
        {
            get { return _balanceDetail; }

            set
            {
                if (_balanceDetail == value) return;

                RaisePropertyChanging(BalanceDetailsPropertyName);
                _balanceDetail = value;
                RaisePropertyChanged(BalanceDetailsPropertyName);
            }
        }

        #endregion

        #region IsLoadingList

        /// <summary>
        /// The <see cref="IsLoadingList" /> property's name.
        /// </summary>
        private const string IsLoadingListPropertyName = "IsLoadingList";

        private bool _isLoadingList;

        /// <summary>
        /// 是否正在加载列表
        /// </summary>
        public bool IsLoadingList
        {
            get { return _isLoadingList; }

            set
            {
                if (_isLoadingList == value) return;

                RaisePropertyChanging(IsLoadingListPropertyName);
                _isLoadingList = value;
                RaisePropertyChanged(IsLoadingListPropertyName);
            }
        }

        #endregion

        #region OutTradeNo

        /// <summary>
        /// The <see cref="OutTradeNo" /> property's name.
        /// </summary>
        private const string OutTradeNoPropertyName = "OutTradeNo";

        private string _outTradeNo;

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo
        {
            get { return _outTradeNo; }

            set
            {
                if (_outTradeNo == value) return;

                RaisePropertyChanging(OutTradeNoPropertyName);
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNoPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region 查询

        /// <summary>
        /// 检查是否可以执行命令
        /// </summary>
        /// <returns></returns>
        protected override bool CanExecuteQueryCommand()
        {
            return !IsLoadingList;
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            IsLoadingList = IsBusy = true;
            BalanceDetails.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var result = service.GetScoreAccountDetails(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    TotalCount = result.TotalCount;
                }));

                if (result.List == null) return;
                foreach (var item in result.List)
                    DispatcherHelper.UIDispatcher.Invoke(new Action<BalanceDetailDto>(BalanceDetails.Add), item);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoadingList = IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region ExchangeCommand

        private RelayCommand _exchangeCommand;

        /// <summary>
        /// 兑换积分
        /// </summary>
        public RelayCommand ExchangeCommand
        {
            get
            {
                return _exchangeCommand ?? (_exchangeCommand = new RelayCommand(ExecuteExchangeCommand, CanExecuteExchangeCommand));
            }
        }

        private void ExecuteExchangeCommand()
        {
            LocalUIManager.ShowPointsExchange(isExchangeSuccess =>
            {
                if (!isExchangeSuccess) return;
                RefreshAccountInfo();
                ExecuteQueryCommand();
            });
        }

        private bool CanExecuteExchangeCommand()
        {
            if (IsInDesignMode)
                return true;

            var can = ViewModelLocator.Home != null && ViewModelLocator.Home.AccountInfo != null &&
                ViewModelLocator.Home.AccountInfo.ScoreInfo.FinancialScore > 0;
            return can;
        }

        #endregion

        #endregion
    }
}
