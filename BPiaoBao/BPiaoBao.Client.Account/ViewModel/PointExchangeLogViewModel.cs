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
    /// 积分兑换视图模型
    /// </summary>
    public class PointExchangeLogViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="PointExchangeLogViewModel"/> class.
        /// </summary>
        public PointExchangeLogViewModel()
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

        #region ScoreConvertLogs

        /// <summary>
        /// The <see cref="ScoreConvertLogs" /> property's name.
        /// </summary>
        private const string ScoreConvertLogsPropertyName = "ScoreConvertLogs";

        private ObservableCollection<ScoreConvertLogDto> _scoreConvertLogs = new ObservableCollection<ScoreConvertLogDto>();

        /// <summary>
        /// 积分兑换
        /// </summary>
        public ObservableCollection<ScoreConvertLogDto> ScoreConvertLogs
        {
            get { return _scoreConvertLogs; }

            set
            {
                if (_scoreConvertLogs == value) return;

                RaisePropertyChanging(ScoreConvertLogsPropertyName);
                _scoreConvertLogs = value;
                RaisePropertyChanged(ScoreConvertLogsPropertyName);
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
            return !IsBusy;
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            ScoreConvertLogs.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var dataPack = service.GetScoreConvertLog(StartTime, EndTime, (CurrentPageIndex - 1) * PageSize, PageSize);
                TotalCount = dataPack.TotalCount;

                foreach (var item in dataPack.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<ScoreConvertLogDto>(ScoreConvertLogs.Add), item);
                }
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion

        #endregion
    }
}
