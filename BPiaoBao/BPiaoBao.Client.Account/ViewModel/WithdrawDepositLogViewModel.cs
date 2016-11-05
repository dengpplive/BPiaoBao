using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 清算日志视图模型
    /// </summary>
    public class WithdrawDepositLogViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="WithdrawDepositLogViewModel"/> class.
        /// </summary>
        public WithdrawDepositLogViewModel()
        {
            CurrentPageIndex = 1;
            PageSize = 15;
            //EndTime = DateTime.Now;
            //StartTime = EndTime.Value.AddMonths(-1);
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
                ExecuteQueryCommand();//第一次默认加载数据
        }

        #endregion

        #region WithdrawDepositLogs

        /// <summary>
        /// The <see cref="WithdrawDepositLogs" /> property's name.
        /// </summary>
        private const string WithdrawDepositLogsPropertyName = "WithdrawDepositLogs";

        private ObservableCollection<CashOutLogDto> _withdrawDepositLogs = new ObservableCollection<CashOutLogDto>();

        /// <summary>
        /// 显示的订单
        /// </summary>
        public ObservableCollection<CashOutLogDto> WithdrawDepositLogs
        {
            get { return _withdrawDepositLogs; }

            set
            {
                if (_withdrawDepositLogs == value) return;

                RaisePropertyChanging(WithdrawDepositLogsPropertyName);
                _withdrawDepositLogs = value;
                RaisePropertyChanged(WithdrawDepositLogsPropertyName);
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

        #region QueryCommand

        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            WithdrawDepositLogs.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                var dataPack = p.FindCashOutLog(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                TotalCount = dataPack.TotalCount;

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<CashOutLogDto>(WithdrawDepositLogs.Add), item);
                }
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        protected override bool CanExecuteQueryCommand()
        {
            return !IsBusy;
        }

        #endregion
    }
}
