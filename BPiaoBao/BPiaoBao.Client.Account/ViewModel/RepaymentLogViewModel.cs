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
    /// 还款日志视图模型
    /// </summary>
    public class RepaymentLogViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="RepaymentLogViewModel"/> class.
        /// </summary>
        public RepaymentLogViewModel()
        {
            CurrentPageIndex = 1;
            PageSize = 15;
            EndTime = DateTime.Now;
            StartTime = EndTime.Value.AddMonths(-1);
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

        #region DepositLogs

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string RepaymentLogsPropertyName = "RepaymentLogs";

        private ObservableCollection<RepaymentLogDto> _repaymentLogs = new ObservableCollection<RepaymentLogDto>();

        /// <summary>
        /// 显示的订单
        /// </summary>
        public ObservableCollection<RepaymentLogDto> RepaymentLogs
        {
            get { return _repaymentLogs; }

            set
            {
                if (_repaymentLogs == value) return;

                RaisePropertyChanging(RepaymentLogsPropertyName);
                _repaymentLogs = value;
                RaisePropertyChanged(RepaymentLogsPropertyName);
            }
        }

        #endregion

        #endregion

        #region QueryCommand

        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            RepaymentLogs.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                var dataPack = p.FindRepaymentLog(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize);
                TotalCount = dataPack.TotalCount;

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<RepaymentLogDto>(RepaymentLogs.Add), item);
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
