using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NPOI.DDF;
using NPOI.SS.Formula.Functions;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 订单管理视图模型
    /// </summary>
    public class OrderManagerViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManagerViewModel"/> class.
        /// </summary>
        public OrderManagerViewModel()
        {
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(null, "请选择"));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.Invalid, EnumHelper.GetDescription(EnumClientOrderStatus.Invalid)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.IssueAndCompleted, EnumHelper.GetDescription(EnumClientOrderStatus.IssueAndCompleted)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.PaymentInWaiting, EnumHelper.GetDescription(EnumClientOrderStatus.PaymentInWaiting)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.NewOrder, EnumHelper.GetDescription(EnumClientOrderStatus.NewOrder)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.OrderCanceled, EnumHelper.GetDescription(EnumClientOrderStatus.OrderCanceled)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.RepelIssueAndCompleted, EnumHelper.GetDescription(EnumClientOrderStatus.RepelIssueAndCompleted)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.WaitChoosePolicy, EnumHelper.GetDescription(EnumClientOrderStatus.WaitChoosePolicy)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.WaitIssue, EnumHelper.GetDescription(EnumClientOrderStatus.WaitIssue)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.WaitReimburseWithRepelIssue, EnumHelper.GetDescription(EnumClientOrderStatus.WaitReimburseWithRepelIssue)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.RepelIssueRefunding, EnumHelper.GetDescription(EnumClientOrderStatus.RepelIssueRefunding)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.ApplyBabyFail, EnumHelper.GetDescription(EnumClientOrderStatus.ApplyBabyFail)));
            _allOrderStatus.Add(new KeyValuePair<EnumClientOrderStatus?, string>(EnumClientOrderStatus.RepelApplyBaby, EnumHelper.GetDescription(EnumClientOrderStatus.RepelApplyBaby)));

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
                if (_choosePolicyCommand != null)
                    _choosePolicyCommand.RaiseCanExecuteChanged();
                if (_cancelOrderCommand != null)
                    _cancelOrderCommand.RaiseCanExecuteChanged();
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
                var data = service.GetOrderBySearch(OrderId, PnrCode, PassengerName, StartCreateTime, endTime, status, (CurrentPageIndex - 1) * PageSize, PageSize, OutTradeNo,StartFlightTime,flightendTime);
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

        #region OpenOrderInfoCommand

        private RelayCommand<ResponseOrder> _openOrderInfoCommand;

        /// <summary>
        /// 打开订单详情页面命令
        /// </summary>
        public RelayCommand<ResponseOrder> OpenOrderInfoCommand
        {
            get
            {
                return _openOrderInfoCommand ?? (_openOrderInfoCommand = new RelayCommand<ResponseOrder>(ExecuteOpenOrderInfoCommand, CanExecuteOpenOrderInfoCommand));
            }
        }

        private void ExecuteOpenOrderInfoCommand(ResponseOrder order)
        {
            var oo = new OrderDetailDto();
            CommunicateManager.Invoke<IOrderService>(service =>
            {
                oo = service.GetClientOrderDetail(order.OrderId);
            }, UIManager.ShowErr);
            try
            {
                var flag = oo.Policy.PolicySourceType == "接口" ? 1 : 0;
                LocalUIManager.ShowOrderInfo(order.OrderId, null, flag);
            }
            catch (Exception exx)
            {
               Logger.WriteLog(LogType.ERROR,exx.Message,exx);
            }
        }

        private bool CanExecuteOpenOrderInfoCommand(ResponseOrder order)
        {
            return true;
        }

        #endregion

        #region OpenAfterSaleCommand

        private RelayCommand<ResponseOrder> _openAfterSaleCommand;

        /// <summary>
        /// 打开售后命令
        /// </summary>
        public RelayCommand<ResponseOrder> OpenAfterSaleCommand
        {
            get
            {
                return _openAfterSaleCommand ?? (_openAfterSaleCommand = new RelayCommand<ResponseOrder>(ExecuteOpenAfterSaleCommand, CanExecuteOpenAfterSaleCommand));
            }
        }

        private void ExecuteOpenAfterSaleCommand(ResponseOrder order)
        {
            LocalUIManager.ShowAfterSale(order.OrderId);
        }

        private bool CanExecuteOpenAfterSaleCommand(ResponseOrder order)
        {
            return true;
        }

        #endregion

        #region OpenCoordinationCommand

        private RelayCommand<ResponseOrder> _openCoordinationCommand;

        /// <summary>
        /// 打开协调命令
        /// </summary>
        public RelayCommand<ResponseOrder> OpenCoordinationCommand
        {
            get
            {
                return _openCoordinationCommand ?? (_openCoordinationCommand = new RelayCommand<ResponseOrder>(ExecuteOpenCoordinationCommand, CanExecuteOpenCoordinationCommand));
            }
        }

        private void ExecuteOpenCoordinationCommand(ResponseOrder order)
        {
            LocalUIManager.ShowCoordination(order.OrderId);
        }

        private bool CanExecuteOpenCoordinationCommand(ResponseOrder order)
        {
            return true;
        }

        #endregion

        #region OpenSendMsgCommand

        private RelayCommand<ResponseOrder> _openSendMsgCommand;

        /// <summary>
        /// 发送打开命令窗口命令
        /// </summary>
        public RelayCommand<ResponseOrder> OpenSendMsgCommand
        {
            get
            {
                return _openSendMsgCommand ?? (_openSendMsgCommand = new RelayCommand<ResponseOrder>(ExecuteOpenSendMsgCommand));
            }
        }

        private void ExecuteOpenSendMsgCommand(ResponseOrder order)
        {
            LocalUIManager.ShowSendMsg(order.OrderId);
        }

        #endregion

        #region ChoosePolicyCommand

        private RelayCommand<ResponseOrder> _choosePolicyCommand;

        /// <summary>
        /// 选择政策
        /// </summary>
        public RelayCommand<ResponseOrder> ChoosePolicyCommand
        {
            get
            {
                return _choosePolicyCommand ?? (_choosePolicyCommand = new RelayCommand<ResponseOrder>(ExecuteChoosePolicy, CanExecuteChoosePolicy));
            }
        }

        private void ExecuteChoosePolicy(ResponseOrder order)
        {
            //if (order.ClientOrderStatus == EnumOrderStatus.Invalid) { UIManager.ShowMessage("订单已经失效"); ExecuteQueryCommand(); return; } 
            if (order.Passengers.Count == order.Passengers.Count(p => p.PassengerType == EnumPassengerType.Baby)) { UIManager.ShowMessage("婴儿不能选择政策"); return; }
            if (order.Policy != null && order.Policy.PolicySpecialType != null && order.Policy.PolicySpecialType != EnumPolicySpecialType.Normal) { UIManager.ShowMessage("特价订单不能选择政策"); return; }
            IsBusy = true;
            Func<PolicyPack> func = () =>
            {
                PolicyPack result = null;
                CommunicateManager.Invoke<IOrderService>(
                    service => { result = service.GetPolicyByOrderId(order.OrderId); }, UIManager.ShowErr);

                return result;
            };

            Task.Factory.StartNew(func).ContinueWith(task =>
            {
                IsBusy = false;

                var model = task.Result;
                if (model == null)
                    return;
                LocalUIManager.ShowPolicyList(model, () => RefreshOrders(order));
            });
        }

        private bool CanExecuteChoosePolicy(ResponseOrder arg)
        {
            return !isBusy;
        }

        #endregion

        #region CancelOrderCommand

        private RelayCommand<ResponseOrder> _cancelOrderCommand;

        /// <summary>
        /// Gets the CancelOrderCommand.
        /// </summary>
        public RelayCommand<ResponseOrder> CancelOrderCommand
        {
            get
            {
                return _cancelOrderCommand ?? (_cancelOrderCommand = new RelayCommand<ResponseOrder>(ExecuteCancelOrderCommand, CanExecuteCancelOrderCommand));
            }
        }

        private void ExecuteCancelOrderCommand(ResponseOrder order)
        {

            //    var dialogResult = UIManager.ShowMessageDialog("提醒：你正在进行取消订单操作！");
            //    if (dialogResult == null || !dialogResult.Value)
            //        return;
            //    if (order.ClientOrderStatus == EnumOrderStatus.Invalid) { UIManager.ShowMessage("订单已经失效"); ExecuteQueryCommand(); return; }



            bool isChecked;
            var dialogResult = UIManager.ShowCancelOrderWindow(order.PnrSource == EnumPnrSource.CreatePnr, out isChecked);
            if (dialogResult == null || !dialogResult.Value)
                return;
            Logger.WriteLog(LogType.INFO, "取消订单时，勾选是否同时取消编码状态：" + isChecked);


            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service => service.CancelOrder(order.OrderId, isChecked), UIManager.ShowErr);

            //Task.Factory.StartNew(action).ContinueWith(task =>
            //{
            //    Action setBusyAction = () => { IsBusy = false; };
            //    DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            //    RefreshOrders(order);
            //});

            Task.Factory.StartNew(action).ContinueWith(task => RefreshOrders(order));
        }

        private bool CanExecuteCancelOrderCommand(ResponseOrder order)
        {
            return !isBusy;
        }

        #endregion

        #region PayCommand

        private RelayCommand<ResponseOrder> _payCommand;

        /// <summary>
        /// 立即支付命令
        /// </summary>
        public RelayCommand<ResponseOrder> PayCommand
        {
            get
            {
                return _payCommand ?? (_payCommand = new RelayCommand<ResponseOrder>(ExecutePayCommand, CanExecutePayCommand));
            }
        }

        private void ExecutePayCommand(ResponseOrder model)
        {
            if (String.IsNullOrEmpty(model.OrderId))
                return;
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                //var isPay = service.PnrIsPay(model.OrderId);
                string result = service.QueryPayStatus(model.OrderId, "Pay");
                if (result != "已支付")
                {
                    LocalUIManager.ShowPayInsuranceAndRefund(model.OrderId, dialogResult => RefreshOrders(model));
                }
                else
                {
                    UIManager.ShowMessage("该订单已支付");
                }
            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                RefreshOrders(model);
            });
        }

        private bool CanExecutePayCommand(ResponseOrder model)
        {
            return !IsBusy;
        }

        #endregion

        #region ReissueCommand

        private RelayCommand<ResponseOrder> _reissueCommand;

        /// <summary>
        /// 退改签订单命令
        /// </summary>
        public RelayCommand<ResponseOrder> ReissueCommand
        {
            get
            {
                return _reissueCommand ?? (_reissueCommand = new RelayCommand<ResponseOrder>(ExecuteReissueCommand, CanExecuteReissueCommand));
            }
        }

        private void ExecuteReissueCommand(ResponseOrder order)
        {
            LocalUIManager.ShowReissue(order, isOk =>
            {
                if (isOk == null || !isOk.Value) return;
                RefreshOrders(order);
            });
        }

        private bool CanExecuteReissueCommand(ResponseOrder order)
        {
            return true;
        }

        #endregion

        #region AuthCommand

        private RelayCommand<ResponseOrder> _authCommand;

        /// <summary>
        /// 授权提示命令
        /// </summary>
        public RelayCommand<ResponseOrder> AuthCommand
        {
            get
            {
                return _authCommand ?? (_authCommand = new RelayCommand<ResponseOrder>(ExecuteAuthCommand, CanExecuteAuthCommand));
            }
        }

        private void ExecuteAuthCommand(ResponseOrder model)
        {

            if (String.IsNullOrEmpty(model.OrderId))
                return;
            CommunicateManager.Invoke<IOrderService>(service =>
            {
                var oo = service.GetClientOrderDetail(model.OrderId);
                UIManager.ShowMessage("授权指令:RMK TJ AUTH " + oo.Policy.CPOffice);
            }, UIManager.ShowErr);

        }

        private bool CanExecuteAuthCommand(ResponseOrder model)
        {
            return true;
        }

        #endregion

        #region QueryPayStatusCommand

        private RelayCommand<ResponseOrder> _queryPayStatusCommand;

        /// <summary>
        /// 支付状态查询命令
        /// </summary>
        public RelayCommand<ResponseOrder> QueryPayStatusCommand
        {
            get
            {
                return _queryPayStatusCommand ?? (_queryPayStatusCommand = new RelayCommand<ResponseOrder>(ExecuteQueryPayStatusCommand, CanExecuteQueryPayStatusCommand));
            }
        }

        private void ExecuteQueryPayStatusCommand(ResponseOrder model)
        {
            if (String.IsNullOrEmpty(model.OrderId)) return;
            //CommunicateManager.Invoke<IOrderService>(service => service.QueryPayStatus(model.OrderId), UIManager.ShowErr);
            Logger.WriteLog(LogType.INFO, "订单号：" + model.OrderId + "执行支付查询");
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service => UIManager.ShowMessage(service.QueryPayStatus(model.OrderId)), UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                RefreshOrders(model);
            });
        }

        private bool CanExecuteQueryPayStatusCommand(ResponseOrder model)
        {
            return !isBusy;
        }

        #endregion
        #endregion

        //刷新指定订单
        private void RefreshOrders(ResponseOrder order)
        {
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var data = service.GetOrderBySearch(order.OrderId, null, null, null, null, null, 0, 1, OutTradeNo, StartFlightTime, EndFlightTime);
                if (data.List == null)
                    return;

                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        var index = Orders.IndexOf(Orders.FirstOrDefault(p => p.OrderId == order.OrderId));
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
