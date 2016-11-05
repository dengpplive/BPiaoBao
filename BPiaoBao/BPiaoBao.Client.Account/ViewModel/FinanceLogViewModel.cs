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
    /// 理财视图模型
    /// </summary>
    public class FinanceLogViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="FinanceLogViewModel"/> class.
        /// </summary>
        public FinanceLogViewModel()
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

        #region 公开属性

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

        #region QueryCommand

        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            FinancialLogs.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                var dataPack = p.FindFinancialLog(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                TotalCount = dataPack.TotalCount;

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<FinancialLogDto>(FinancialLogs.Add), item);
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

        #endregion

    }
}
