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
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class AfterSaleOrderViewModel : BaseVM
    {

        #region OrderId

        /// <summary>
        /// The <see cref="OrderId" /> property's name.
        /// </summary>
        private const string OrderIdPropertyName = "OrderId";

        private string _orderId;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId
        {
            get { return _orderId; }

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

        private string _pnrCode;

        /// <summary>
        /// PNR编码
        /// </summary>
        public string PnrCode
        {
            get { return _pnrCode; }

            set
            {
                if (_pnrCode == value) return;

                RaisePropertyChanging(PnrCodePropertyName);
                _pnrCode = value;
                RaisePropertyChanged(PnrCodePropertyName);
            }
        }

        #endregion

        #region AllOrderStatus

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string AllOrderStatusPropertyName = "AllOrderStatus";

        private ObservableCollection<KeyValuePair<EnumTfgProcessStatus?, String>> _allOrderStatus = new ObservableCollection<KeyValuePair<EnumTfgProcessStatus?, string>>();

        /// <summary>
        /// 所有退废改订单状态
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumTfgProcessStatus?, String>> AllOrderStatus
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

        #region SelectedOrderStatus

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string SelectedOrderStatusPropertyName = "SelectedOrderStatus";

        private EnumTfgProcessStatus? _orderStatus;

        /// <summary>
        /// 选中的订单状态
        /// </summary>
        public EnumTfgProcessStatus? SelectedOrderStatus
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

        #region AllOrderType

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string AllOrderTypePropertyName = "AllOrderType";

        private ObservableCollection<KeyValuePair<EnumAfterSaleOrder?, String>> _allOrderType = new ObservableCollection<KeyValuePair<EnumAfterSaleOrder?, string>>();

        /// <summary>
        /// 退废改订单类型
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumAfterSaleOrder?, String>> AllOrderType
        {
            get { return _allOrderType; }

            set
            {
                if (_allOrderType == value) return;

                RaisePropertyChanging(AllOrderTypePropertyName);
                _allOrderType = value;
                RaisePropertyChanged(AllOrderTypePropertyName);
            }
        }

        #endregion

        #region SelectedOrderType

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string SelectedOrderTypePropertyName = "SelectedOrderType";

        private EnumAfterSaleOrder? _orderType;

        /// <summary>
        /// 选中的订单状态
        /// </summary>
        public EnumAfterSaleOrder? SelectedOrderType
        {
            get { return _orderType; }

            set
            {
                if (_orderType == value) return;

                RaisePropertyChanging(SelectedOrderTypePropertyName);
                _orderType = value;
                RaisePropertyChanged(SelectedOrderTypePropertyName);
            }
        }

        #endregion

        #region Orders

        /// <summary>
        /// The <see cref="Orders" /> property's name.
        /// </summary>
        private const string OrdersPropertyName = "Orders";

        private ObservableCollection<ResponseAfterSaleOrder> _orders = new ObservableCollection<ResponseAfterSaleOrder>();

        /// <summary>
        /// 显示的退废改订单
        /// </summary>
        public ObservableCollection<ResponseAfterSaleOrder> Orders
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

        #region PayNum

        /// <summary>
        /// The <see cref="PayNum" /> property's name.
        /// </summary>
        private const string PayNumPropertyName = "PayNum";

        private string _payNum;

        /// <summary>
        /// 交易号
        /// </summary>
        public string PayNum
        {
            get { return _payNum; }

            set
            {
                if (_payNum == value) return;

                RaisePropertyChanging(PayNumPropertyName);
                _payNum = value;
                RaisePropertyChanged(PayNumPropertyName);
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public AfterSaleOrderViewModel()
        {
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(null, "请选择"));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.Processed, EnumHelper.GetDescription(EnumTfgProcessStatus.Processed)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.Processing, EnumHelper.GetDescription(EnumTfgProcessStatus.Processing)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.ProcessingWaitPay, EnumHelper.GetDescription(EnumTfgProcessStatus.ProcessingWaitPay)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.ProcessingWaitRefund, EnumHelper.GetDescription(EnumTfgProcessStatus.ProcessingWaitRefund)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.Refunding, EnumHelper.GetDescription(EnumTfgProcessStatus.Refunding)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.RepelProcess, EnumHelper.GetDescription(EnumTfgProcessStatus.RepelProcess)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.UnProcess, EnumHelper.GetDescription(EnumTfgProcessStatus.UnProcess)));
            _allOrderStatus.Add(new KeyValuePair<EnumTfgProcessStatus?, string>(EnumTfgProcessStatus.WaitIssue, EnumHelper.GetDescription(EnumTfgProcessStatus.WaitIssue)));

            _allOrderType.Add(new KeyValuePair<EnumAfterSaleOrder?, string>(null, "请选择"));
            _allOrderType.Add(new KeyValuePair<EnumAfterSaleOrder?, string>(EnumAfterSaleOrder.Annul, EnumHelper.GetDescription(EnumAfterSaleOrder.Annul)));
            _allOrderType.Add(new KeyValuePair<EnumAfterSaleOrder?, string>(EnumAfterSaleOrder.Bounce, EnumHelper.GetDescription(EnumAfterSaleOrder.Bounce)));
            _allOrderType.Add(new KeyValuePair<EnumAfterSaleOrder?, string>(EnumAfterSaleOrder.Change, EnumHelper.GetDescription(EnumAfterSaleOrder.Change)));
            _allOrderType.Add(new KeyValuePair<EnumAfterSaleOrder?, string>(EnumAfterSaleOrder.Modify, EnumHelper.GetDescription(EnumAfterSaleOrder.Modify)));

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
            if (StartCreateTime != null && EndCreateTime != null && StartCreateTime.Value.CompareTo(EndCreateTime.Value) > 0)
            {
                UIManager.ShowMessage("申请日期选择开始日期大于结束日期");
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
                if (SelectedOrderStatus.HasValue)
                    _orderStatus = SelectedOrderStatus.Value;
                if (SelectedOrderType.HasValue)
                    _orderType = SelectedOrderType.Value;
                DateTime? endTime = null;
                if (EndCreateTime != null) endTime = EndCreateTime.Value.Date.AddDays(1).AddSeconds(-1);
                DateTime? flighendTime = null;
                if (EndFlightTime != null) flighendTime = EndFlightTime.Value.Date.AddDays(1).AddSeconds(-1);
                var data = service.GetSaleOrderBySearch(_currentPageIndex, _pageSize, _pnrCode, _orderId, _orderType, _orderStatus, StartCreateTime, endTime, PayNum, StartFlightTime, flighendTime);
                if (data.List == null)
                    return;
                TotalCount = data.TotalCount;

                foreach (var item in data.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<ResponseAfterSaleOrder>(Orders.Add), item);
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

        #region OpenSendMsgCommand

        private RelayCommand<ResponseAfterSaleOrder> _openSendMsgCommand;

        /// <summary>
        /// 发送打开命令窗口命令
        /// </summary>
        public RelayCommand<ResponseAfterSaleOrder> OpenSendMsgCommand
        {
            get
            {
                return _openSendMsgCommand ?? (_openSendMsgCommand = new RelayCommand<ResponseAfterSaleOrder>(ExecuteOpenSendMsgCommand));
            }
        }

        private void ExecuteOpenSendMsgCommand(ResponseAfterSaleOrder order)
        {
            LocalUIManager.ShowSendMsg(order.OrderID, order.Id);
        }

        #endregion

        #region PayCommand

        private RelayCommand<ResponseAfterSaleOrder> _payCommand;

        /// <summary>
        /// 立即支付命令
        /// </summary>
        public RelayCommand<ResponseAfterSaleOrder> PayCommand
        {
            get
            {
                return _payCommand ?? (_payCommand = new RelayCommand<ResponseAfterSaleOrder>(ExecutePayCommand, CanExecutePayCommand));
            }
        }

        private void ExecutePayCommand(ResponseAfterSaleOrder model)
        {
            //LocalUIManager.ShowAfterSalePay(model, isOk => RefreshOrders(model));
            IsBusy = true;
            Action action = () => DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var info = service.QueryAfterSaleOrderPayStatus(model.Id);
                    if (info == "已支付")
                    {
                        UIManager.ShowMessage("订单已支付");
                        RefreshOrders(model);
                        return;
                    }
                    LocalUIManager.ShowAfterSalePay(model, isOk => RefreshOrders(model));
                }, UIManager.ShowErr)));

            Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    Action setBusyAction = () => { IsBusy = false; };
                    DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                });
        }

        private bool CanExecutePayCommand(ResponseAfterSaleOrder model)
        {
            return !IsBusy;
        }

        #endregion

        #region OpenOrderInfoCommand

        private RelayCommand<ResponseAfterSaleOrder> _openOrderInfoCommand;

        /// <summary>
        /// 打开订单详情页面命令
        /// </summary>
        public RelayCommand<ResponseAfterSaleOrder> OpenOrderInfoCommand
        {
            get
            {
                return _openOrderInfoCommand ?? (_openOrderInfoCommand = new RelayCommand<ResponseAfterSaleOrder>(ExecuteOpenOrderInfoCommand, CanExecuteOpenOrderInfoCommand));
            }
        }

        private void ExecuteOpenOrderInfoCommand(ResponseAfterSaleOrder order)
        {
            LocalUIManager.ShowAfterSaleInfo(order.OrderID, order.Id);
        }

        private bool CanExecuteOpenOrderInfoCommand(ResponseAfterSaleOrder order)
        {
            return true;
        }

        #endregion

        #endregion

        //刷新指定订单
        private void RefreshOrders(ResponseAfterSaleOrder order)
        {
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var data = service.GetSaleOrderBySearch(1, 1, "", "", null, null, null, null, "", null, null, order.Id);
                if (data.List == null)
                    return;

                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        var index = Orders.IndexOf(Orders.FirstOrDefault(p => p.Id == order.Id));
                        Orders[index] = data.List[0];
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //throw;
                    }
                }));

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

    }
}
