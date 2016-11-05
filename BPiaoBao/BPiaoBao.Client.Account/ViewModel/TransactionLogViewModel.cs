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
    /// 交易记录视图模型
    /// </summary>
    public class TransactionLogViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionLogViewModel"/> class.
        /// </summary>
        public TransactionLogViewModel()
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
                ExecuteQueryCommand();//第一次默认加载数据
        }

        #endregion

        #region 公开属性

        #region BargainLogs

        /// <summary>
        /// The <see cref="BargainLogs" /> property's name.
        /// </summary>
        private const string BargainLogsPropertyName = "BargainLogs";

        private ObservableCollection<BargainLogDto> _bargainLogs = new ObservableCollection<BargainLogDto>();

        /// <summary>
        /// 交易记录
        /// </summary>
        public ObservableCollection<BargainLogDto> BargainLogs
        {
            get { return _bargainLogs; }

            set
            {
                if (_bargainLogs == value) return;

                RaisePropertyChanging(BargainLogsPropertyName);
                _bargainLogs = value;
                RaisePropertyChanged(BargainLogsPropertyName);
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

        /// <summary>
        /// 检查是否可以执行命令
        /// </summary>
        /// <returns></returns>
        protected override bool CanExecuteQueryCommand()
        {
            return !IsBusy;
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            BargainLogs.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var dataPack = service.GetBargainLog(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                TotalCount = dataPack.TotalCount;

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<BargainLogDto>(BargainLogs.Add), item);
                }
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion
    }
}
