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
    public class BuyDetailViewModel : ViewModelBase
    {
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

        private DateTime? _startTime;

        public DateTime? StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime == value) return;
                _startTime = value;
                RaisePropertyChanged("StartTime");
            }
        }
        private DateTime? _endTime;

        public DateTime? EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime == value) return;
                _endTime = value;
                RaisePropertyChanged("EndTime");
            }
        }


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
        public ObservableCollection<BuyDetailDto> List { get; private set; }
        public ICommand PagerCommand { get; private set; }
        private static BuyDetailViewModel _instace;
        private BuyDetailViewModel()
        {
            List = new ObservableCollection<BuyDetailDto>();
            CurrentPageIndex = 1;
            PageSize = 15;
            EndTime = DateTime.Now;
            StartTime = EndTime.Value.AddMonths(-1);
            if (IsInDesignMode)
                return;

            PagerCommand = new RelayCommand(Init);
        }
        public static BuyDetailViewModel CreateInstance()
        {
            if (_instace == null)
                _instace = new BuyDetailViewModel();
            _instace.Init();
            return _instace;
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
                var dataPack = p.BuyRecordByPage(CurrentPageIndex, PageSize, StartTime, endTime, OutTradeNo);
                TotalCount = dataPack.TotalCount;
                dataPack.List.ForEach(x => DispatcherHelper.UIDispatcher.Invoke(new Action<BuyDetailDto>(List.Add), x));
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
         
        private string _outTradeNo = "";

        public string OutTradeNo
        {
            get { return _outTradeNo.Trim(); }
            set
            {
                if (_outTradeNo == value) return;
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNo);
            }
        }
    }
}
