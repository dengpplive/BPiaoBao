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
    /// 转账日志记录
    /// </summary>
    public class TransferLogViewModel : PageBaseViewModel
    {
        #region 构造函数

        public TransferLogViewModel()
        {
            CurrentPageIndex = 1;
            PageSize = 15;

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

        #region TransferLogs

        /// <summary>
        /// The <see cref="TransferLogs" /> property's name.
        /// </summary>
        private const string TransferLogsPropertyName = "TransferLogs";

        private ObservableCollection<TransferAccountsLogDto> _transferLogs = new ObservableCollection<TransferAccountsLogDto>();

        /// <summary>
        /// 显示的订单
        /// </summary>
        public ObservableCollection<TransferAccountsLogDto> TransferLogs
        {
            get { return _transferLogs; }

            set
            {
                if (_transferLogs == value) return;

                RaisePropertyChanging(TransferLogsPropertyName);
                _transferLogs = value;
                RaisePropertyChanged(TransferLogsPropertyName);
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
            TransferLogs.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                var dataPack = p.FindTransferAccountsLog(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                TotalCount = dataPack.TotalCount;

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<TransferAccountsLogDto>(TransferLogs.Add), item);
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
