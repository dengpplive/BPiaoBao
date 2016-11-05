using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class ChooseOrderIdViewModel : BaseVM
    {

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManagerViewModel"/> class.
        /// </summary>
        public ChooseOrderIdViewModel()
        {
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.IssueAndCompleted, EnumClientOrderStatus.IssueAndCompleted.ToEnumDesc()));
            SelectedOrderStatus = EnumClientOrderStatus.IssueAndCompleted;
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

        #endregion

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

        #region PNRCode

        /// <summary>
        /// The <see cref="PnrCode" /> property's name.
        /// </summary>
        private const string PnrCodePropertyName = "PNRCode";

        private string _pnrCode = "";

        /// <summary>
        /// PNR编码
        /// </summary>
        public string PnrCode
        {
            get { return _pnrCode.Trim(); }

            set
            {
                if (_pnrCode == value) return;

                RaisePropertyChanging(PnrCodePropertyName);
                _pnrCode = value;
                RaisePropertyChanged(PnrCodePropertyName);
            }
        }

        #endregion

        #region PassengerName

        /// <summary>
        /// The <see cref="PassengerName" /> property's name.
        /// </summary>
        private const string PassengerNamePropertyName = "PassengerName";

        private string _passengerName = "";

        /// <summary>
        /// 乘机人姓名
        /// </summary>
        public string PassengerName
        {
            get { return _passengerName.Trim(); }

            set
            {
                if (_passengerName == value) return;

                RaisePropertyChanging(PassengerNamePropertyName);
                _passengerName = value;
                RaisePropertyChanged(PassengerNamePropertyName);
            }
        }

        #endregion

        #region SelectedOrderStatus

        /// <summary>
        /// The <see cref="SelectedOrderStatus" /> property's name.
        /// </summary>
        private const string SelectedOrderStatusPropertyName = "SelectedOrderStatus";

        private EnumClientOrderStatus? _orderStatus;

        /// <summary>
        /// 选中的订单状态
        /// </summary>
        public EnumClientOrderStatus? SelectedOrderStatus
        {
            get { return _orderStatus; }

            set
            {
                if (_orderStatus == value) return;

                RaisePropertyChanging(SelectedOrderStatusPropertyName);
                _orderStatus = value;
                RaisePropertyChanged(SelectedOrderStatusPropertyName);
            }
        }

        #endregion

        #region AllOrderStatus

        /// <summary>
        /// The <see cref="AllOrderStatus" /> property's name.
        /// </summary>
        private const string AllOrderStatusPropertyName = "AllOrderStatus";

        private ObservableCollection<KeyValuePair<EnumClientOrderStatus?, String>> _allOrderStatus = new ObservableCollection<KeyValuePair<EnumClientOrderStatus?, string>>();

        /// <summary>
        /// 所有订单状态
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumClientOrderStatus?, String>> AllOrderStatus
        {
            get { return _allOrderStatus; }

            set
            {
                if (_allOrderStatus == value) return;

                RaisePropertyChanging(AllOrderStatusPropertyName);
                _allOrderStatus = value;
                RaisePropertyChanged(AllOrderStatusPropertyName);
            }
        }

        #endregion

        #region StartCreateTime

        /// <summary>
        /// The <see cref="StartCreateTime" /> property's name.
        /// </summary>
        private const string StartCreateTimePropertyName = "StartCreateTime";

        private DateTime? _startCreateTime;

        /// <summary>
        /// 创建时开始间 
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

        #region EndCreateTime

        /// <summary>
        /// The <see cref="EndCreateTime" /> property's name.
        /// </summary>
        private const string EndCreateTimePropertyName = "EndCreateTime";

        private DateTime? _endCreateTime;

        /// <summary>
        /// 创建时开始间 
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

        #region StartFlightTime

        /// <summary>
        /// The <see cref="StartFlightTime" /> property's name.
        /// </summary>
        private const string StartFlightTimePropertyName = "StartFlightTime";

        private DateTime? _startFlightTime;

        /// <summary>
        /// 航班开始日期 
        /// </summary>
        public DateTime? StartFlightTime
        {
            get
            {
                return _startFlightTime;
            }

            set
            {
                if (_startFlightTime == value) return;

                RaisePropertyChanging(StartFlightTimePropertyName);
                _startFlightTime = value;
                RaisePropertyChanged(StartFlightTimePropertyName);
            }
        }

        #endregion

        #region EndFlightTime

        /// <summary>
        /// The <see cref="EndFlightTime" /> property's name.
        /// </summary>
        private const string EndFlightTimePropertyName = "EndFlightTime";

        private DateTime? _endFlightTime;

        /// <summary>
        /// 航班结束日期 
        /// </summary>
        public DateTime? EndFlightTime
        {
            get
            {
                return _endFlightTime;
            }

            set
            {
                if (_endFlightTime == value) return;

                RaisePropertyChanging(EndFlightTimePropertyName);
                _endFlightTime = value;
                RaisePropertyChanged(EndFlightTimePropertyName);
            }
        }

        #endregion

        #region OutTradeNo

        /// <summary>
        /// The <see cref="OutTradeNo" /> property's name.
        /// </summary>
        private const string OutTradeNoPropertyName = "OutTradeNo";

        private string _outTradeNo = "";

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo
        {
            get { return _outTradeNo.Trim(); }

            set
            {
                if (_outTradeNo == value) return;

                RaisePropertyChanging(OutTradeNoPropertyName);
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNoPropertyName);
            }
        }

        #endregion

        #region IsSelected

        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        private const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelected;

        /// <summary>
        /// 是否选择完成
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (_isSelected == value) return;

                RaisePropertyChanging(IsSelectedPropertyName);
                _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }

        #endregion

        #region Orders

        /// <summary>
        /// The <see cref="Orders" /> property's name.
        /// </summary>
        private const string OrdersPropertyName = "Orders";

        private ObservableCollection<ResponseOrder> _orders = new ObservableCollection<ResponseOrder>();

        /// <summary>
        /// 显示的订单
        /// </summary>
        public ObservableCollection<ResponseOrder> Orders
        {
            get { return _orders; }

            set
            {
                if (_orders == value) return;

                RaisePropertyChanging(OrdersPropertyName);
                _orders = value;
                RaisePropertyChanged(OrdersPropertyName);
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
        #endregion

        #region 公开事件

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
            if (StartCreateTime != null && EndCreateTime != null && StartCreateTime.Value.CompareTo(EndCreateTime.Value) > 0)
            {
                UIManager.ShowMessage("创建日期选择开始日期大于结束日期");
                return;
            }
            if (StartFlightTime != null && EndFlightTime != null && StartFlightTime.Value.CompareTo(EndFlightTime.Value) > 0)
            {
                UIManager.ShowMessage("航班日期选择开始日期大于结束日期");
                return;
            }
            IsBusy = true;
            Orders.Clear();
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                int? status = null;
                if (SelectedOrderStatus.HasValue)
                    status = (int)SelectedOrderStatus.Value;
                DateTime? endTime = null;
                if (EndCreateTime != null) endTime = EndCreateTime.Value.Date.AddDays(1).AddSeconds(-1);
                DateTime? flightendTime = null;
                if (EndFlightTime != null) flightendTime = EndFlightTime.Value.Date.AddDays(1).AddSeconds(-1);
                var data = service.GetOrderBySearch(OrderId, PnrCode, PassengerName, StartCreateTime, endTime, status, (CurrentPageIndex - 1) * PageSize, PageSize, OutTradeNo, StartFlightTime, flightendTime);
                if (data.List == null)
                    return;
                TotalCount = data.TotalCount;

                foreach (var item in data.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<ResponseOrder>(Orders.Add), item);
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

        #region ChooseOrderIdCommand

        private RelayCommand<ResponseOrder> _chooseOrderIdCommand;

        /// <summary>
        /// 选择
        /// </summary>
        public RelayCommand<ResponseOrder> ChooseOrderIdCommand
        {
            get
            {
                return _chooseOrderIdCommand ?? (_chooseOrderIdCommand = new RelayCommand<ResponseOrder>(ExecuteChooseOrderId, CanExecuteChooseOrderId));
            }
        }

        private void ExecuteChooseOrderId(ResponseOrder order)
        {
            OrderId = order.OrderId;
            IsSelected = true;
        }

        private bool CanExecuteChooseOrderId(ResponseOrder arg)
        {
            return !isBusy;
        }

        #endregion

        #endregion

    }
}
