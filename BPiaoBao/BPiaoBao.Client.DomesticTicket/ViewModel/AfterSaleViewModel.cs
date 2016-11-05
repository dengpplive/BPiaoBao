using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 退废改视图模型
    /// </summary>
    public class AfterSaleViewModel : BaseVM
    {
        #region 构造函数

        ///// <summary>
        ///// 初始化数据
        ///// </summary>
        //public override void Initialize()
        //{
        //    IsBusy = true;
        //    Action action = () =>
        //    {
        //        CommunicateManager.Invoke<IOrderService>(service =>
        //        {
        //            AfterSaleOrder = service.GetAfterSaleOrderById(order.OrderId);
        //        }, UIManager.ShowErr);
        //    };

        //    Task.Factory.StartNew(action).ContinueWith((task) =>
        //    {
        //        Action setBusyAction = () => { IsBusy = false; };
        //        DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
        //    });
        //}

        #endregion

        #region 公开属性

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否在忙
        ///// </summary>
        //public bool IsBusy
        //{
        //    get { return isBusy; }

        //    set
        //    {
        //        if (isBusy == value) return;

        //        RaisePropertyChanging(IsBusyPropertyName);
        //        isBusy = value;
        //        RaisePropertyChanged(IsBusyPropertyName);
        //    }
        //}

        #endregion

        #region Order

        ///// <summary>
        ///// The <see cref="Order" /> property's name.
        ///// </summary>
        //public const string OrderPropertyName = "Order";

        //private OrderDto order = null;

        ///// <summary>
        ///// 订单对象
        ///// </summary>
        //public OrderDto Order
        //{
        //    get { return order; }

        //    set
        //    {
        //        if (order == value) return;

        //        RaisePropertyChanging(OrderPropertyName);
        //        order = value;
        //        RaisePropertyChanged(OrderPropertyName);
        //    }
        //}

        #endregion

        #region AfterSaleOrder

        /// <summary>
        /// The <see cref="AfterSaleOrder" /> property's name.
        /// </summary>
        private const string AfterSaleOrderPropertyName = "AfterSaleOrder";

        private IEnumerable<ResponseAfterSaleOrder> _afterSaleOrder;

        /// <summary>
        /// 售后列表
        /// </summary>
        public IEnumerable<ResponseAfterSaleOrder> AfterSaleOrder
        {
            get { return _afterSaleOrder; }

            set
            {
                if (_afterSaleOrder == value) return;

                RaisePropertyChanging(AfterSaleOrderPropertyName);
                _afterSaleOrder = value;
                RaisePropertyChanged(AfterSaleOrderPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开属性

        #region OpenInfoCommand

        private RelayCommand<ResponseAfterSaleOrder> _openInfoCommand;

        /// <summary>
        /// 打开售后详情
        /// </summary>
        public RelayCommand<ResponseAfterSaleOrder> OpenInfoCommand
        {
            get
            {
                return _openInfoCommand ?? (_openInfoCommand = new RelayCommand<ResponseAfterSaleOrder>(ExecuteOpenInfoCommand, CanExecuteOpenInfoCommand));
            }
        }

        private void ExecuteOpenInfoCommand(ResponseAfterSaleOrder saleOrder)
        {
            LocalUIManager.ShowAfterSaleInfo(saleOrder.OrderID, saleOrder.Id);
        }

        private bool CanExecuteOpenInfoCommand(ResponseAfterSaleOrder saleOrder)
        {
            return true;
        }

        #endregion

        #region PayCommand

        private RelayCommand<ResponseAfterSaleOrder> _payCommand;

        /// <summary>
        /// 支付命令
        /// </summary>
        public RelayCommand<ResponseAfterSaleOrder> PayCommand
        {
            get
            {
                return _payCommand ?? (_payCommand = new RelayCommand<ResponseAfterSaleOrder>(ExecutePayCommand, CanExecutePayCommand));
            }
        }

        private void ExecutePayCommand(ResponseAfterSaleOrder order)
        {
            LocalUIManager.ShowAfterSalePay(order, isOk => Initialize());
        }

        private bool CanExecutePayCommand(ResponseAfterSaleOrder order)
        {
            return true;
        }

        #endregion

        #endregion

        internal void LoadOrderInfo(string orderId)
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                AfterSaleOrder = service.GetAfterSaleOrderById(orderId);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }
    }
}
