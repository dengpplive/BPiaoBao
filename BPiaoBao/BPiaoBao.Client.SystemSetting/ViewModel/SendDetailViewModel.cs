using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class SendDetailViewModel : ViewModelBase
    {
        private static SendDetailViewModel _instance;
        private bool _isWait;
        public bool IsWait
        {
            get { return _isWait; }
            set
            {
                if (_isWait == value) return;
                _isWait = value;
                RaisePropertyChanged("IsWait");
            }
        }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (_currentPageIndex == value) return;
                _currentPageIndex = value;
                RaisePropertyChanged("CurrentPageIndex");
            }
        }

        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                if (_totalCount == value) return;
                _totalCount = value;
                RaisePropertyChanged("TotalCount");
            }
        }

        public int PageSize { get; private set; }
        public ObservableCollection<SendDetailDto> List { get; private set; }
        public ICommand PagerCommand { get; private set; }
      
        private SendDetailViewModel()
        {
            List = new ObservableCollection<SendDetailDto>();
            CurrentPageIndex = 1;
            PageSize = 15;
            EndTime = DateTime.Now;
            StartTime = EndTime.Value.AddMonths(-1);

            if (IsInDesignMode)
                return;

            PagerCommand = new RelayCommand(Init);
        }
        public static SendDetailViewModel CreateInstance()
        {
            if (_instance == null)
                _instance = new SendDetailViewModel();
            _instance.Init();
            return _instance;
        }
        private bool _result = true;
        internal void Init()
        {
            if (!_result) return;
            _result = false;
            IsWait = true;
            List.Clear();
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                var endTime = EndTime.HasValue ? EndTime.Value.AddDays(1) : EndTime;
                var dataPack = p.SendRecordByPage(CurrentPageIndex, PageSize, StartTime, endTime);
                TotalCount = dataPack.TotalCount;
                dataPack.List.ForEach(x => DispatcherHelper.UIDispatcher.Invoke(new Action<SendDetailDto>(List.Add), x));
            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(param =>
            {
                Action setIsWait = () =>
                {
                    IsWait = false;
                    _result = true;
                };
                DispatcherHelper.UIDispatcher.Invoke(setIsWait);
            });
        }
    }
}
