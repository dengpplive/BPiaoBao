using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 改签支付视图模型
    /// </summary>
    public class AfterSalePayViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IAccountService>(service =>
                {
                    AccountInfo = service.GetAccountInfo();
                }, UIManager.ShowErr);

                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var temp = service.FindAll(SourceOrder.OrderID, null, null, null, null, null, 0, 1);
                    if (temp != null && temp.TotalCount > 0)
                        Order = temp.List[0];
                    //获取改签后的航程信息
                    var tem = service.GetAfterSaleOrderDetail(SourceOrder.OrderID, SourceOrder.Id);
                    ResponseChangeOrder = tem as ResponseChangeOrder;
                }, UIManager.ShowErr);
                if (ResponseChangeOrder != null)
                {
                    var tempList = from p in ResponseChangeOrder.SkyWay
                                   join x in ResponseChangeOrder.SkyWays
                                   on p.SkyWayId equals x.SkyWayId
                                   select new SkyWayViewModel
                                   {
                                       CarrayCode = x.CarrayCode,
                                       CarrayShortName = x.CarrayShortName,
                                       FlightNumber = x.FlightNumber,
                                       FromCity = x.FromCity,
                                       FromCityCode = x.FromCityCode,
                                       FromTerminal = x.FromTerminal,
                                       NewFlightNumber = p.NewFlightNumber,
                                       NewSeat = p.NewSeat,
                                       NewStartDateTime = p.NewStartDateTime,
                                       NewToDateTime = p.NewToDateTime,
                                       Seat = x.Seat,
                                       StartDateTime = x.StartDateTime,
                                       ToCity = x.ToCity,
                                       ToCityCode = x.ToCityCode,
                                       ToDateTime = x.ToDateTime,
                                       ToTerminal = x.ToTerminal
                                   };
                    ChangeSkyWayList = tempList.ToList();
                }
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region 公开属性

        public ResponseAfterSaleOrder SourceOrder { get; set; }

        #region ChangeSkyWayList

        /// <summary>
        /// The <see cref="ChangeSkyWayList" /> property's name.
        /// </summary>
        private const string ChangeSkyWayListPropertyName = "ChangeSkyWayList";

        private List<SkyWayViewModel> _changeSkyWayList = new List<SkyWayViewModel>();

        /// <summary>
        /// desc
        /// </summary>
        public List<SkyWayViewModel> ChangeSkyWayList
        {
            get { return _changeSkyWayList; }

            set
            {
                if (_changeSkyWayList == value) return;

                RaisePropertyChanging(ChangeSkyWayListPropertyName);
                _changeSkyWayList = value;
                RaisePropertyChanged(ChangeSkyWayListPropertyName);
            }
        }

        #endregion

        #region ResponseChangeOrder

        /// <summary>
        /// The <see cref="ResponseChangeOrder" /> property's name.
        /// </summary>
        private const string ResponseChangeOrderPropertyName = "ResponseChangeOrder";

        private ResponseChangeOrder _responseChangeOrder;

        /// <summary>
        /// 改签订单
        /// </summary>
        public ResponseChangeOrder ResponseChangeOrder
        {
            get { return _responseChangeOrder; }

            set
            {

                if (_responseChangeOrder == value) return;
                RaisePropertyChanging(ResponseChangeOrderPropertyName);
                _responseChangeOrder = value;
                RaisePropertyChanged(ResponseChangeOrderPropertyName);
            }
        }

        #endregion

        #region Order

        /// <summary>
        /// The <see cref="Order" /> property's name.
        /// </summary>
        private const string OrderPropertyName = "Order";

        private OrderDto _order;

        /// <summary>
        /// 订单对象
        /// </summary>
        public OrderDto Order
        {
            get { return _order; }

            set
            {
                if (_order == value) return;

                RaisePropertyChanging(OrderPropertyName);
                _order = value;
                RaisePropertyChanged(OrderPropertyName);
            }
        }

        #endregion

        #region IsPaid

        /// <summary>
        /// The <see cref="IsPaid " /> property's name.
        /// </summary>
        private const string IsPaidPropertyName = "IsPaid";

        private bool _isPaid;

        /// <summary>
        /// 是否已经支付
        /// </summary>
        public bool IsPaid
        {
            get { return _isPaid; }

            set
            {
                if (_isPaid == value) return;

                RaisePropertyChanging(IsPaidPropertyName);
                _isPaid = value;
                RaisePropertyChanged(IsPaidPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否繁忙
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

        #region AccountInfo

        /// <summary>
        /// The <see cref="AccountInfo" /> property's name.
        /// </summary>
        private const string AccountInfoPropertyName = "AccountInfo";

        private AccountInfoDto _accountInfo;

        /// <summary>
        /// 账户信息
        /// </summary>
        public AccountInfoDto AccountInfo
        {
            get { return _accountInfo; }

            set
            {
                if (_accountInfo == value) return;

                RaisePropertyChanging(AccountInfoPropertyName);
                _accountInfo = value;
                RaisePropertyChanged(AccountInfoPropertyName);
            }
        }

        #endregion

        #region IsPaying

        /// <summary>
        /// The <see cref="IsPaying" /> property's name.
        /// </summary>
        private const string IsPayingPropertyName = "IsPaying";

        private bool _isPaying;

        /// <summary>
        /// 是否正在支付
        /// </summary>
        public bool IsPaying
        {
            get { return _isPaying; }

            set
            {
                if (_isPaying == value) return;

                RaisePropertyChanging(IsPayingPropertyName);
                _isPaying = value;
                RaisePropertyChanged(IsPayingPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region PayOrderByBankCommand

        private RelayCommand<string> _payOrderByBankCommand;

        /// <summary>
        /// 使用银行支付
        /// </summary>
        public RelayCommand<string> PayOrderByBankCommand
        {
            get
            {
                return _payOrderByBankCommand ?? (_payOrderByBankCommand = new RelayCommand<string>(ExecutePayOrderByBankCommand, CanExecutePayOrderByBankCommand));
            }
        }

        private void ExecutePayOrderByBankCommand(string bankCode)
        {
            IsPaying = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var uri = service.SaleOrderPayByBank(SourceOrder.Id, bankCode);
                if (uri == "True")
                {
                    var info = service.QueryAfterSaleOrderPayStatus(SourceOrder.Id);
                    if (info == "已支付")
                    {
                        UIManager.ShowMessage("订单已支付");
                        IsPaid = true;
                    }
                    return;
                }
                LocalUIManager.OpenDefaultBrowser(uri);
                var isOk = UIManager.ShowPayWindow();
                if (isOk == null || !isOk.Value) return;
                IsPaid = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsPaying = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecutePayOrderByBankCommand(string bankCode)
        {
            return !IsBusy && !_isPaying && bankCode != null && SourceOrder != null;
        }

        #endregion

        #region PayOrderByPlatformCommand

        private RelayCommand<string> _payOrderByPlatformCommand;

        /// <summary>
        /// 使用支付平台支付
        /// </summary>
        public RelayCommand<string> PayOrderByPlatformCommand
        {
            get
            {
                return _payOrderByPlatformCommand ?? (_payOrderByPlatformCommand = new RelayCommand<string>(ExecutePayOrderByPlatformCommand, CanExecutePayOrderByPlatformCommand));
            }
        }

        private void ExecutePayOrderByPlatformCommand(string code)
        {
            IsPaying = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var uri = service.SaleOrderPayByPlatform(SourceOrder.Id, code);
                if (uri == "True")
                {
                    var info = service.QueryAfterSaleOrderPayStatus(SourceOrder.Id);
                    if (info == "已支付")
                    {
                        UIManager.ShowMessage("订单已支付");
                        IsPaid = true;
                    }
                    return;
                }
                LocalUIManager.OpenDefaultBrowser(uri);
                var isOk = UIManager.ShowPayWindow();
                if (isOk == null || !isOk.Value) return;
                IsPaid = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsPaying = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecutePayOrderByPlatformCommand(string code)
        {
            return !IsBusy && !_isPaying && code != null && SourceOrder != null;
        }

        #endregion

        #region PayOrderByCashbagAccountCommand

        private RelayCommand<string> _payOrderByCashbagAccountCommand;

        /// <summary>
        /// Gets the PayOrderByCashbagAccountCommand.
        /// </summary>
        public RelayCommand<string> PayOrderByCashbagAccountCommand
        {
            get
            {
                return _payOrderByCashbagAccountCommand ?? (_payOrderByCashbagAccountCommand = new RelayCommand<string>(ExecutePayOrderByCashbagAccountCommand, CanExecutePayOrderByCashbagAccountCommand));
            }
        }

        private void ExecutePayOrderByCashbagAccountCommand(string password)
        {
            IsPaying = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var info = service.QueryAfterSaleOrderPayStatus(SourceOrder.Id);
                if (info == "已支付")
                {
                    UIManager.ShowMessage("订单已支付");
                    IsPaid = true;
                    return;
                }
                service.SaleOrderPayByCashbagAccount(SourceOrder.Id, password);
                UIManager.ShowMessage("支付成功！");
                IsPaid = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsPaying = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecutePayOrderByCashbagAccountCommand(string password)
        {
            return !IsBusy && !IsPaying && !String.IsNullOrWhiteSpace(password) && SourceOrder != null;
        }

        #endregion

        #region PayOrderByCreditAccountCommand

        private RelayCommand<string> _payOrderByCreditAccountCommand;

        /// <summary>
        /// Gets the PayOrderByCreditAccountCommand.
        /// </summary>
        public RelayCommand<string> PayOrderByCreditAccountCommand
        {
            get
            {
                return _payOrderByCreditAccountCommand ?? (_payOrderByCreditAccountCommand = new RelayCommand<string>(ExecutePayOrderByCreditAccountCommand, CanExecutePayOrderByCreditAccountCommand));
            }
        }

        private void ExecutePayOrderByCreditAccountCommand(string password)
        {
            IsPaying = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var info = service.QueryAfterSaleOrderPayStatus(SourceOrder.Id);
                if (info == "已支付")
                {
                    UIManager.ShowMessage("订单已支付");
                    IsPaid = true;
                    return;
                }
                service.SaleOrderPayByCreditAccount(SourceOrder.Id, password);
                UIManager.ShowMessage("支付成功！");
                IsPaid = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsPaying = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecutePayOrderByCreditAccountCommand(string password)
        {
            return !IsBusy && !IsPaying && !String.IsNullOrWhiteSpace(password) && !IsPaid;
        }

        #endregion

        #region PayByQuikAliPayCommand

        private RelayCommand<string> _payOrderByQuikAliPayCommand;

        /// <summary>
        /// Gets the PayOrderByCreditAccountCommand.
        /// </summary>
        public RelayCommand<string> PayByQuikAliPayCommand
        {
            get
            {
                return _payOrderByQuikAliPayCommand ?? (_payOrderByQuikAliPayCommand = new RelayCommand<string>(ExecutePayByQuikAliPayCommand, CanExecutePayByQuikAliPayCommand));
            }
        }

        private void ExecutePayByQuikAliPayCommand(string password)
        {
            IsPaying = true;
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var uri = service.SaleOrderPayByQuikAliPay(SourceOrder.Id, password);
                if (uri == "True")
                {
                    UIManager.ShowMessage("订单已支付");
                    IsPaid = true;
                    return;
                }
                UIManager.ShowMessage("支付成功！");
                IsPaid = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsPaying = false; IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecutePayByQuikAliPayCommand(string password)
        {
            return !IsBusy && !IsPaying && !String.IsNullOrWhiteSpace(password) && !IsPaid;
        }

        #endregion
        #endregion

    }
}
