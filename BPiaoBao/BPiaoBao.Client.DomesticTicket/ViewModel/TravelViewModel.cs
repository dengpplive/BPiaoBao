using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class TravelViewModel : BaseVM
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TravelViewModel()
        {
            #region 行程单状态
            _allTravelStatus.Add(new KeyValuePair<EnumPassengerTripStatus?, string>(null, "请选择"));
            _allTravelStatus.Add(new KeyValuePair<EnumPassengerTripStatus?, string>(EnumPassengerTripStatus.HasCreate, EnumHelper.GetDescription(EnumPassengerTripStatus.HasCreate)));
            _allTravelStatus.Add(new KeyValuePair<EnumPassengerTripStatus?, string>(EnumPassengerTripStatus.HasVoid, EnumHelper.GetDescription(EnumPassengerTripStatus.HasVoid)));
            _allTravelStatus.Add(new KeyValuePair<EnumPassengerTripStatus?, string>(EnumPassengerTripStatus.NoCreate, EnumHelper.GetDescription(EnumPassengerTripStatus.NoCreate))); 
            #endregion

            Initialize();
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #region 公开属性

        #region OrderId

        /// <summary>
        /// The <see cref="OrderId" /> property's name.
        /// </summary>
        private const string OrderIdPropertyName = "OrderId";

        private string _orderId = "";

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId
        {
            get { return _orderId.Trim(); }

            set
            {
                if (_orderId == value) return;

                RaisePropertyChanging(OrderIdPropertyName);
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        #endregion

        #region useOffice

        /// <summary>
        /// The <see cref="UseOffice" /> property's name.
        /// </summary>
        private const string UseOfficePropertyName = "useOffice";

        private string _useOffice = "";

        /// <summary>
        /// office
        /// </summary>
        public string UseOffice
        {
            get { return _useOffice.Trim(); }

            set
            {
                if (_useOffice == value) return;

                RaisePropertyChanging(UseOfficePropertyName);
                _useOffice = value;
                RaisePropertyChanged(UseOfficePropertyName);
            }
        }

        #endregion

        #region TicketNumber

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string StartTicketNumberPropertyName = "startTicketNumber";

        private string _sartTicketNumber = "";

        /// <summary>
        /// 票号
        /// </summary>
        public string StartTicketNumber
        {
            get { return _sartTicketNumber.Trim(); }

            set
            {
                if (_sartTicketNumber == value) return;

                RaisePropertyChanging(StartTicketNumberPropertyName);
                _sartTicketNumber = value;
                RaisePropertyChanged(StartTicketNumberPropertyName);
            }
        }

        private const string EndTicketNumberPropertyName = "endTicketNumber";

        private string _endTicketNumber = "";

        /// <summary>
        /// 票号
        /// </summary>
        public string EndTicketNumber
        {
            get { return _endTicketNumber.Trim(); }

            set
            {
                if (_endTicketNumber == value) return;

                RaisePropertyChanging(EndTicketNumberPropertyName);
                _endTicketNumber = value;
                RaisePropertyChanged(EndTicketNumberPropertyName);
            }
        }
        #endregion

        #region TripNumber

        /// <summary>
        /// The <see cref="StartTripNumber" /> property's name.
        /// </summary>
        private const string StartTripNumberPropertyName = "startTripNumber";

        private string _startTripNumber = "";

        /// <summary>
        /// 行程单号
        /// </summary>
        public string StartTripNumber
        {
            get { return _startTripNumber.Trim(); }

            set
            {
                if (_startTripNumber == value) return;

                RaisePropertyChanging(StartTripNumberPropertyName);
                _startTripNumber = value;
                RaisePropertyChanged(StartTripNumberPropertyName);
            }
        }

        private const string EndTripNumberPropertyName = "endTripNumber";

        private string _endTripNumber = "";

        /// <summary>
        /// 行程单号
        /// </summary>
        public string EndTripNumber
        {
            get { return _endTripNumber.Trim(); }

            set
            {
                if (_endTripNumber == value) return;

                RaisePropertyChanging(EndTripNumberPropertyName);
                _endTripNumber = value;
                RaisePropertyChanged(EndTripNumberPropertyName);
            }
        }
        #endregion

        #region SelectedTravelStatus

        /// <summary>
        /// The <see cref="SelectedTravelStatus" /> property's name.
        /// </summary>
        private const string SelectedTravelStatusPropertyName = "SelectedTravelStatus";

        private EnumPassengerTripStatus? _selectedTravelStatus;

        /// <summary>
        /// 选中的行程单状态
        /// </summary>
        public EnumPassengerTripStatus? SelectedTravelStatus
        {
            get { return _selectedTravelStatus; }

            set
            {
                if (_selectedTravelStatus == value) return;

                RaisePropertyChanging(SelectedTravelStatusPropertyName);
                _selectedTravelStatus = value;
                RaisePropertyChanged(SelectedTravelStatusPropertyName);
            }
        }

        #endregion

        #region AllTravelStatus

        /// <summary>
        /// The <see cref="AllTravelStatus" /> property's name.
        /// </summary>
        private const string AllTravelStatusPropertyName = "AllTravelStatus";

        private ObservableCollection<KeyValuePair<EnumPassengerTripStatus?, String>> _allTravelStatus = new ObservableCollection<KeyValuePair<EnumPassengerTripStatus?, string>>();

        /// <summary>
        /// 所有行程单状态
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumPassengerTripStatus?, String>> AllTravelStatus
        {
            get { return _allTravelStatus; }

            set
            {
                if (_allTravelStatus == value) return;

                RaisePropertyChanging(AllTravelStatusPropertyName);
                _allTravelStatus = value;
                RaisePropertyChanged(AllTravelStatusPropertyName);
            }
        }

        #endregion

        #region Travels

        /// <summary>
        /// The <see cref="Travels" /> property's name.
        /// </summary>
        private const string TravelsPropertyName = "Travels";

        private ObservableCollection<TravelPaperDto> _travels = new ObservableCollection<TravelPaperDto>();

        /// <summary>
        /// 显示的行程单
        /// </summary>
        public ObservableCollection<TravelPaperDto> Travels
        {
            get { return _travels; }

            set
            {
                if (_travels == value) return;

                RaisePropertyChanging(TravelsPropertyName);
                _travels = value;
                RaisePropertyChanged(TravelsPropertyName);
            }
        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否正在忙碌
        /// </summary>
        public new bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_queryCommand != null)
                    _queryCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 翻页

        #region PageSize

        /// <summary>
        /// The <see cref="PageSize" /> property's name.
        /// </summary>
        private const string PageSizePropertyName = "PageSize";

        private int _pageSize = 20;

        /// <summary>
        /// 翻页
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }

            set
            {
                if (_pageSize == value) return;

                RaisePropertyChanging(PageSizePropertyName);
                _pageSize = value;
                RaisePropertyChanged(PageSizePropertyName);
            }
        }

        #endregion

        #region CurrentPageIndex

        /// <summary>
        /// The <see cref="CurrentPageIndex" /> property's name.
        /// </summary>
        private const string CurrentPageIndexPropertyName = "CurrentPageIndex";

        private int _currentPageIndex = 1;

        /// <summary>
        /// 当前索引页
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }

            set
            {
                if (_currentPageIndex == value) return;

                RaisePropertyChanging(CurrentPageIndexPropertyName);
                _currentPageIndex = value;
                RaisePropertyChanged(CurrentPageIndexPropertyName);
            }
        }

        #endregion

        #region TotalCount

        /// <summary>
        /// The <see cref="TotalCount" /> property's name.
        /// </summary>
        private const string TotalCountPropertyName = "TotalCount";

        private int _totalCount;

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }

            set
            {
                if (_totalCount == value) return;

                RaisePropertyChanging(TotalCountPropertyName);
                _totalCount = value;
                RaisePropertyChanged(TotalCountPropertyName);
            }
        }

        #endregion

        #endregion

        #region startVoidTime

        /// <summary>
        /// The <see cref="StartVoidTime" /> property's name.
        /// </summary>
        private const string StartVoidTimePropertyName = "startVoidTime";

        private DateTime? _startVoidTime;

        /// <summary>
        /// 作废时间 
        /// </summary>
        public DateTime? StartVoidTime
        {
            get
            {
                return _startVoidTime;
            }
            set
            {
                if (_startVoidTime == value) return;

                RaisePropertyChanging(StartVoidTimePropertyName);
                _startVoidTime = value;
                RaisePropertyChanged(StartVoidTimePropertyName);
            }
        }

        #endregion

        #region endVoidTime

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string EndVoidTimePropertyName = "endVoidTime";

        private DateTime? _endVoidTime;

        /// <summary>
        /// 作废时间
        /// </summary>
        public DateTime? EndVoidTime
        {
            get
            {
                return _endVoidTime;
            }
            set
            {
                if (_endVoidTime == value) return;

                RaisePropertyChanging(EndVoidTimePropertyName);
                _endVoidTime = value;
                RaisePropertyChanged(EndVoidTimePropertyName);
            }
        }

        #endregion

        #region startGrantTime

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string StartGrantTimePropertyName = "startGrantTime";

        private DateTime? _startGrantTime;

        /// <summary>
        /// 发放时间 
        /// </summary>
        public DateTime? StartGrantTime
        {
            get
            {
                return _startGrantTime;
            }
            set
            {
                if (_startGrantTime == value) return;

                RaisePropertyChanging(StartGrantTimePropertyName);
                _startGrantTime = value;
                RaisePropertyChanged(StartGrantTimePropertyName);
            }
        }

        #endregion

        #region endGrantTime

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string EndGrantTimePropertyName = "endGrantTime";

        private DateTime? _endGrantTime;

        /// <summary>
        /// 发放时间
        /// </summary>
        public DateTime? EndGrantTime
        {
            get
            {
                return _endGrantTime;
            }
            set
            {
                if (_endGrantTime == value) return;

                RaisePropertyChanging(EndGrantTimePropertyName);
                _endGrantTime = value;
                RaisePropertyChanged(EndGrantTimePropertyName);
            }
        }

        #endregion

        #region startCreateTime

        /// <summary>
        /// The <see cref="StartCreateTime" /> property's name.
        /// </summary>
        private const string StartCreateTimePropertyName = "startCreateTime";

        private DateTime? _startCreateTime;

        /// <summary>
        /// 使用时间 
        /// </summary>
        public DateTime? StartCreateTime
        {
            get
            {
                return _startCreateTime;
            }
            set
            {
                if (_startCreateTime == value) return;

                RaisePropertyChanging(StartCreateTimePropertyName);
                _startCreateTime = value;
                RaisePropertyChanged(StartCreateTimePropertyName);
            }
        }

        #endregion

        #region endCreateTime

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string EndCreateTimePropertyName = "endCreateTime";

        private DateTime? _endCreateTime;

        /// <summary>
        /// 使用时间 
        /// </summary>
        public DateTime? EndCreateTime
        {
            get
            {
                return _endCreateTime;
            }
            set
            {
                if (_endCreateTime == value) return;

                RaisePropertyChanging(EndCreateTimePropertyName);
                _endCreateTime = value;
                RaisePropertyChanged(EndCreateTimePropertyName);
            }
        }

        #endregion

        #endregion     .
        
        #region 公开命令
        
        #region QueryCommand

        private RelayCommand _queryCommand;

        /// <summary>
        /// 查询命令
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return _queryCommand ?? (_queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        protected virtual void ExecuteQueryCommand()
        {
            #region 状态
            var tripStatus = new int?[0];
            if (SelectedTravelStatus.HasValue)
            {
                switch (SelectedTravelStatus.Value)
                {
                    case EnumPassengerTripStatus.NoCreate:
                        tripStatus = new int?[] { 0, 3, 4 };
                        break;
                    case EnumPassengerTripStatus.HasCreate:
                        tripStatus = new int?[] { 1 };
                        break;
                    case EnumPassengerTripStatus.HasVoid:
                        tripStatus = new int?[] { 2, 5 };
                        break;
                }
            } 
            #endregion

            //if (int.Parse(startTicketNumber) > int.Parse(endTicketNumber) && !string.IsNullOrEmpty(startTicketNumber) && !string.IsNullOrEmpty(endTicketNumber))
            //{ UIManager.ShowMessage("票号段选择开始票号大于结束票号"); return; }
            if (StartCreateTime > EndCreateTime || StartGrantTime > EndGrantTime || StartVoidTime > EndVoidTime)
            { UIManager.ShowMessage("选择日期时开始日期大于结束日期"); return; }

            IsBusy = true;
            Travels.Clear();
            Action action = () => CommunicateManager.Invoke<ITravelPaperService>(service =>
            {
                var data = service.FindTravelPaper(null,null,UseOffice,StartTripNumber,EndTripNumber,StartTicketNumber,EndTicketNumber,StartCreateTime,EndCreateTime,StartVoidTime,EndVoidTime,StartGrantTime,EndGrantTime,null,null,null,null,CurrentPageIndex,PageSize,true,tripStatus,OrderId);
                if (data.List == null)
                    return;
                TotalCount = data.TotalCount;

                foreach (var item in data.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<TravelPaperDto>(Travels.Add), item);
                }
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        protected virtual bool CanExecuteQueryCommand()
        {
            return !isBusy;
        }

        #endregion

        #region ClearCommand 清空

        private RelayCommand _clearCommand;

        /// <summary>
        /// 导入命令
        /// </summary>
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand));
            }
        }

        private void ExecuteClearCommand()
        {
            OrderId = string.Empty;
            UseOffice = string.Empty;
            StartTripNumber = string.Empty;
            EndTripNumber = string.Empty;
            StartTicketNumber = string.Empty;
            EndTicketNumber = string.Empty;
            StartCreateTime = null;
            EndCreateTime = null;
            StartGrantTime = null;
            EndGrantTime = null;
            StartVoidTime = null;
            EndVoidTime = null;
            SelectedTravelStatus = null;
        }

        private bool CanExecuteClearCommand()
        {
            return !IsBusy;
        }

        #endregion

        #endregion
    }
}
