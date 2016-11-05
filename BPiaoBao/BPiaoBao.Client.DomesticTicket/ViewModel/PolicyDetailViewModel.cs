using System.Configuration;
using System.Globalization;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JoveZhao.Framework;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 政策支付详情页 viewmode
    /// </summary>
    public class PolicyDetailViewModel : ViewModelBase
    {
        #region 支付超时
        private System.Timers.Timer _timer;
        /// <summary>
        /// 分钟
        /// </summary>
        private int _minutes;
        /// <summary>
        /// 秒数
        /// </summary>
        private int _seconds;
        /// <summary>
        /// 截止支付时间
        /// </summary>
        private DateTime DeadTime { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyDetailViewModel"/> class.
        /// RelayCommand valueCommand, decimal boughtInsuranceMoney, decimal boughtRefundMoney
        /// </summary>
        public PolicyDetailViewModel()
        {
            //_boughtInsuranceMoney = boughtInsuranceMoney;
            //_boughtRefundMoney = boughtRefundMoney;
            //_valueCommand = valueCommand;
            //timer
            _timer = new System.Timers.Timer { Interval = 1000 };
            _timer.Elapsed += timer_Elapsed;
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Male, EnumHelper.GetDescription(EnumSexType.Male)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Female, EnumHelper.GetDescription(EnumSexType.Female)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.UnKnown, EnumHelper.GetDescription(EnumSexType.UnKnown)));
            c_AgeTypeItems = ProjectHelper.Utils.EnumHelper.GetEnumKeyValuesPassger(typeof(EnumPassengerType));
            AgeTypeItems = CollectionViewSource.GetDefaultView(c_AgeTypeItems);
            //c_IDTypeItems = (List<KeyValuePair<dynamic, string>>)ProjectHelper.Utils.EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType));
            //IDTypeItems = CollectionViewSource.GetDefaultView(c_IDTypeItems);
            IsShowCommissinInfo = LocalUIManager.DefaultShowhiddenColumn;
            IsShowTicketPrice = !LocalUIManager.DefaultShowhiddenColumn;
            if (_payInsuranceModels == null) _payInsuranceModels = new List<PayInsuranceModel>();
            //获取保险相关设置
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                #region 保险相关设置
                var re = service.GetCurentInsuranceCfgInfo(true);
                IsOpenCurrenCarrierInsurance = re.IsOpenCurrenCarrierInsurance && re.IsOpenUnexpectedInsurance;
                if (IsOpenCurrenCarrierInsurance)
                {
                    _unexpectedSinglePrice = re.UnexpectedSinglePrice;
                    InsuranceLeaveCount = re.LeaveCount;
                }
                IsOpenRefundInsurance = re.IsOpenRefundInsurance;
                if (IsOpenRefundInsurance) _refundSingle = re.RefundSinglePrice;
                #endregion
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_seconds > 0)
                _seconds = _seconds - 1;
            LastSeconds = _seconds < 10 ? "0" + _seconds : _seconds.ToString(CultureInfo.InvariantCulture);
            if (_seconds == 0 && _minutes > 0)
            {
                _minutes = _minutes - 1;
                LastTimes = _minutes < 10 ? "0" + _minutes + ":" : _minutes + ":";
                _seconds = 60;
            }
            if (_minutes != 0 || _seconds != 0) return;
            if (_order.OrderType != "2") IsOverTime = true; //非线下婴儿订单才更改超时标识
            _timer.Stop();
            //执行更改订单的状态信息
            //ChangeOrderStatus();
        }

        /// <summary>
        /// 置订单状态为失效
        /// </summary>
        //private void ChangeOrderStatus()
        //{
        //    IsBusy = true;
        //    Action action2 = () => CommunicateManager.Invoke<IOrderService>(service => service.UpdateOrderStatus(Order.OrderId), UIManager.ShowErr);

        //    Task.Factory.StartNew(action2).ContinueWith(task =>
        //    {
        //        Action setBusyAction = () => { IsBusy = false; };
        //        DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
        //    });
        //}

        #endregion

        #region 公开属性

        #region AllInsuranceCount
        private const string AllInsuranceCountPropertyName = "AllInsuranceCount";

        private int _allInsuranceCount;
        public int AllInsuranceCount
        {
            get { return _allInsuranceCount; }
            set
            {
                if (_allInsuranceCount == value) return;
                RaisePropertyChanging(AllInsuranceCountPropertyName);
                _allInsuranceCount = value;
                RaisePropertyChanged(AllInsuranceCountPropertyName);
            }
        }
        #endregion

        #region IsSingleFlight

        /// <summary>
        /// The <see cref="IsSingleFlight" /> property's name.
        /// </summary>
        private const string IsSingleFlightPropertyName = "IsSingleFlight";

        private bool _isSingleFlight;

        /// <summary>
        /// 是否是单程
        /// </summary>
        public bool IsSingleFlight
        {
            get { return _isSingleFlight; }

            set
            {
                if (_isSingleFlight == value) return;

                RaisePropertyChanging(IsSingleFlightPropertyName);
                _isSingleFlight = value;
                RaisePropertyChanged(IsSingleFlightPropertyName);
            }
        }
        #endregion

        #region PayInsuranceModels

        private const string PayInsuranceModelsPropertyName = "PayInsuranceModels";
        private List<PayInsuranceModel> _payInsuranceModels;
        public List<PayInsuranceModel> PayInsuranceModels
        {
            get { return _payInsuranceModels; }
            set
            {
                if (_payInsuranceModels == value) return;
                RaisePropertyChanging(PayInsuranceModelsPropertyName);
                _payInsuranceModels = value;
                RaisePropertyChanged(PayInsuranceModelsPropertyName);
            }
        }

        #endregion

        #region IsShowCommissinInfo

        /// <summary>
        /// The <see cref="IsShowCommissinInfo" /> property's name.
        /// </summary>
        private const string IsShowCommissinInfoPropertyName = "IsShowCommissinInfo";

        private bool _isShowCommissinInfo;

        /// <summary>
        /// 是否隐藏佣金/返点
        /// </summary>
        public bool IsShowCommissinInfo
        {
            get { return _isShowCommissinInfo; }

            set
            {
                if (_isShowCommissinInfo == value) return;

                RaisePropertyChanging(IsShowCommissinInfoPropertyName);
                _isShowCommissinInfo = value;
                RaisePropertyChanged(IsShowCommissinInfoPropertyName);
            }
        }

        #endregion

        #region IsShowTicketPrice

        /// <summary>
        /// The <see cref="IsShowTicketPrice" /> property's name.
        /// </summary>
        private const string IsShowTicketPricePropertyName = "IsShowTicketPrice";

        private bool _isShowTicketPrice;

        /// <summary>
        /// 是否隐藏面价格
        /// </summary>
        public bool IsShowTicketPrice
        {
            get { return _isShowTicketPrice; }

            set
            {
                if (_isShowTicketPrice == value) return;

                RaisePropertyChanging(IsShowTicketPricePropertyName);
                _isShowTicketPrice = value;
                RaisePropertyChanged(IsShowTicketPricePropertyName);
            }
        }

        #endregion

        #region IsShowTimer

        /// <summary>
        /// The <see cref="IsShowTimer" /> property's name.
        /// </summary>
        private const string IsShowTimerPropertyName = "IsShowTimer";

        private bool _isShowTimer;

        /// <summary>
        /// 是否显示倒计时信息
        /// </summary>
        public bool IsShowTimer
        {
            get { return _isShowTimer; }

            set
            {
                if (_isShowTimer == value) return;

                RaisePropertyChanging(IsShowTimerPropertyName);
                _isShowTimer = value;
                RaisePropertyChanged(IsShowTimerPropertyName);
            }
        }

        #endregion

        #region IsShowPay

        /// <summary>
        /// The <see cref="IsShowPay" /> property's name.
        /// </summary>
        public const string IsShowPayPropertyName = "IsShowPay";

        private bool _isShowPay = true;

        /// <summary>
        /// 是否显示其他支付方式
        /// </summary>
        public bool IsShowPay
        {
            get { return _isShowPay; }

            set
            {
                if (_isShowPay == value) return;

                RaisePropertyChanging(IsShowPayPropertyName);
                _isShowPay = value;
                RaisePropertyChanged(IsShowPayPropertyName);
            }
        }

        #endregion

        #region IsShowOldOrderID

        /// <summary>
        /// The <see cref="IsShowOldOrderId" /> property's name.
        /// </summary>
        public const string IsShowOldOrderIdPropertyName = "IsShowOldOrderID";

        private bool _isShowOldOrderId;

        /// <summary>
        /// 是否显示成人订单号
        /// </summary>
        public bool IsShowOldOrderId
        {
            get { return _isShowOldOrderId; }

            set
            {
                if (_isShowOldOrderId == value) return;

                RaisePropertyChanging(IsShowOldOrderIdPropertyName);
                _isShowOldOrderId = value;
                RaisePropertyChanged(IsShowOldOrderIdPropertyName);
            }
        }

        #endregion

        #region Order

        /// <summary>
        /// The <see cref="Order" /> property's name.
        /// </summary>
        public const string OrderPropertyName = "Order";

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
                IsOpenRefundInsurance = _order.Policy.PolicySourceType == "接口" && _order.PayInfo.PayStatus != EnumPayStatus.OK && IsOpenRefundInsurance; //极速退按钮隐藏
                IsShowPay = _order.PayInfo.PayStatus != EnumPayStatus.OK; //其他支付方式隐藏
                IsShowOldOrderId = _order.OrderType == "1";//关联成人订单号隐藏    
                //TicketPrice = order.Policy.TicketPrice * order.Passengers.Count;//票面价总金额
                _order.Passengers.ToList().ForEach(p => { TicketPrice += p.SeatPrice + p.TaxFee + p.RQFee; });
                ExecuteSumPriceCommand("");
                #region 支付信息
                //UnexpectedPrice = BoughtInsuranceMoney = Order.Passengers.Where(p => p.IsBuyInsurance).Sum(p=>p.BuyInsuranceCount * p.BuyInsurancePrice);
                //RefundSinglePrice = BoughtRefundMoney = Order.Passengers.Where(p => p.IsInsuranceRefund).Sum(p => p.InsuranceRefunrPrice);
                //SumPrice = UnexpectedPrice + Order.PayInfo.PayMoney; //机票支付信息里包含有极速退，所以显示金额不包含极速退服务信息
                #endregion
                #region 倒计时处理
                IsShowTimer = Order.PayInfo.PayStatus == EnumPayStatus.NoPay && _order.OrderType != "2";
                DeadTime = Order.CreateTime.AddMinutes(30);//默认支付限定30分钟
                var serverDateTime = DateTime.Now;
                try
                {
                    CommunicateManager.Invoke<IDateTimeService>(service =>
                    {
                        serverDateTime = service.GetCurrentSystemDateTime();
                    }, UIManager.ShowErr);
                }
                catch (Exception e)
                {
                    Logger.WriteLog(LogType.ERROR, e.Message, e);
                }
                if (serverDateTime > DeadTime && _order.OrderType != "2")
                {
                    IsOverTime = true;
                    //执行更改订单的状态信息
                    //ChangeOrderStatus();
                }
                else
                {
                    IsOverTime = false;
                    var ts = DeadTime - serverDateTime;
                    var totls = (int)ts.TotalSeconds;
                    _minutes = totls / 60;
                    _seconds = totls % 60;
                    LastTimes = _minutes < 10 ? "0" + _minutes + ":" : _minutes + ":";
                    LastSeconds = _seconds < 10 ? "0" + _seconds : _seconds.ToString(CultureInfo.InvariantCulture);
                    _timer.Start();
                }
                #endregion
                #region 保险和极速退
                IsSingleFlight = !(_order.SkyWays.Count > 1);
                if (IsOpenCurrenCarrierInsurance)//根据保险相关筛选乘机人信息
                {
                    _order.Passengers = _order.PayInfo.PayStatus == EnumPayStatus.OK ? _order.Passengers.Where(p => p.IsBuyInsurance == false).ToList() : _order.Passengers;
                    _order.Passengers.ToList().ForEach(x =>
                    {
                        x.BuyInsurancePrice = _unexpectedSinglePrice;
                        //x.BuyInsuranceCount = order.SkyWays.Count;
                        //x.SkyWays = order.SkyWays.ToList();
                        var sws = new List<SkyWayDto>();
                        _order.SkyWays.ToList().ForEach(p => sws.Add(new SkyWayDto
                        {
                            CarrayCode = p.CarrayCode,
                            CarrayShortName = p.CarrayShortName,
                            Discount = p.Discount,
                            FlightModel = p.FlightModel,
                            FlightNumber = p.FlightNumber,
                            FromCity = p.FromCity,
                            FromCityCode = p.FromCityCode,
                            FromTerminal = p.FromTerminal,
                            InsuranceCount = p.InsuranceCount,
                            Seat = p.Seat,
                            SkyWayId = p.SkyWayId,
                            StartDateTime = p.StartDateTime,
                            ToCity = p.ToCity,
                            ToCityCode = p.ToCityCode,
                            ToDateTime = p.ToDateTime,
                            ToTerminal = p.ToTerminal
                        }));
                        x.SkyWays = sws;
                        //x.SkyWays.ToList().ForEach(z => z.InsuranceCount = 1);
                    });

                    var payinsurance = new List<PayInsuranceModel>();


                    _order.Passengers.ToList().ForEach(x =>
                    {
                        var payinsuranceskyways = new List<PayInsuranceSkyWayModel>();

                        _order.SkyWays.ToList().ForEach(y => payinsuranceskyways.Add(new PayInsuranceSkyWayModel { SkyWayId = y.SkyWayId, InsuranceCount = 0 }));
                        payinsurance.Add(new PayInsuranceModel { PassengerId = x.Id, PassengerName = x.PassengerName, PayInsuranceSkyWayModels = payinsuranceskyways, SexType = x.SexType, Birthday = x.Birth, ID = x.CardNo, PassengerType = x.PassengerType, IDType = x.IdType, Tel = x.Mobile });
                    });

                    PayInsuranceModels = payinsurance;
                    ExecuteValueCommand();
                }
                if (IsOpenRefundInsurance)//默认乘机人都购买极速退服务
                {
                    //order.Passengers.ToList().ForEach(x => { x.IsInsuranceRefund = true; x.InsuranceRefunrPrice = RefundSingle; });
                    RefundSinglePrice = Order.Passengers.Sum(p => p.InsuranceRefunrPrice);
                    ExecuteSumPriceCommand("");
                    //SumPrice = RefundSinglePrice + Order.PayInfo.PayMoney;
                    //UIManager.ShowMessage("极速退服务处理时间为09:00—21:00, 且不支持非自愿退票");
                    //ExecuteCountRefundCommand();
                }
                #endregion
                RaisePropertyChanged(OrderPropertyName);
            }
        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        private const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy;

        /// <summary>
        /// 是否正在繁忙
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_payOrderByBankCommand != null)
                    _payOrderByBankCommand.RaiseCanExecuteChanged();
                if (_payOrderByPlatformCommand != null)
                    _payOrderByPlatformCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IsLoading

        /// <summary>
        /// The <see cref="IsLoading" /> property's name.
        /// </summary>
        private const string IsLoadingPropertyName = "IsLoading";

        private bool _isLoading;

        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (_isLoading == value) return;

                RaisePropertyChanging(IsLoadingPropertyName);
                _isLoading = value;
                RaisePropertyChanged(IsLoadingPropertyName);
            }
        }

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

        #region IsPaid

        /// <summary>
        /// The <see cref="IsPaid" /> property's name.
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

        #region IsOverTime

        /// <summary>
        /// The <see cref="IsOverTime" /> property's name.
        /// </summary>
        private const string IsOverTimePropertyName = "IsOverTime";

        private bool _isOverTime;

        /// <summary>
        /// 支付超时标识
        /// </summary>
        public bool IsOverTime
        {
            get { return _isOverTime; }

            set
            {

                RaisePropertyChanging(IsOverTimePropertyName);
                _isOverTime = value;
                RaisePropertyChanged(IsOverTimePropertyName);
            }
        }

        #endregion
        /// <summary>
        /// 支付超时标识
        /// </summary>
        //private bool IsOverTime = false;
        /// <summary>
        /// 选择购买标识
        /// 0表示购买航意险
        /// 1表示购买极速退
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 选择购买保险乘客集合
        /// </summary>
        public List<PassengerDto> InPassers = new List<PassengerDto>();
        /// <summary>
        /// 选择购买极速退服务乘客集合
        /// </summary>
        public List<PassengerDto> RePassers = new List<PassengerDto>();
        /// <summary>
        /// 购买保险和极速退乘机人集合
        /// </summary>
        public List<PassengerDto> AllPassengers = new List<PassengerDto>();
        /// <summary>
        /// 运营商保险数额度
        /// </summary>
        //public int InsuranceLeaveCount { get; set; }
        #region InsuranceLeaveCount

        /// <summary>
        /// The <see cref="InsuranceLeaveCount" /> property's name.
        /// </summary>
        private const string InsuranceLeaveCountPropertyName = "InsuranceLeaveCount";

        private int _insuranceLeaveCount;

        /// <summary>
        /// 保险数额度
        /// </summary>
        public int InsuranceLeaveCount
        {
            get { return _insuranceLeaveCount; }

            set
            {
                if (_insuranceLeaveCount == value) return;

                RaisePropertyChanging(InsuranceLeaveCountPropertyName);
                _insuranceLeaveCount = value;
                RaisePropertyChanged(InsuranceLeaveCountPropertyName);
            }
        }

        #endregion

        #region IsOpenCurrenCarrierInsurance

        /// <summary>
        /// The <see cref="IsOpenCurrenCarrierInsurance" /> property's name.
        /// </summary>
        private const string IsOpenCurrenCarrierInsurancePropertyName = "IsOpenCurrenCarrierInsurance";

        private bool _isOpenCurrenCarrierInsurance;

        /// <summary>
        /// 是否显示保险相关
        /// </summary>
        public bool IsOpenCurrenCarrierInsurance
        {
            get { return _isOpenCurrenCarrierInsurance; }

            set
            {
                if (_isOpenCurrenCarrierInsurance == value) return;

                RaisePropertyChanging(IsOpenCurrenCarrierInsurancePropertyName);
                _isOpenCurrenCarrierInsurance = value;
                IsShowGrid = _isOpenCurrenCarrierInsurance;
                RaisePropertyChanged(IsOpenCurrenCarrierInsurancePropertyName);
            }
        }

        #endregion

        #region IsOpenRefundInsurance

        /// <summary>
        /// The <see cref="IsOpenRefundInsurance" /> property's name.
        /// </summary>
        private const string IsOpenRefundInsurancePropertyName = "IsOpenRefundInsurance";

        private bool _isOpenRefundInsurance;

        /// <summary>
        /// 是否显示极速退相关
        /// </summary>
        public bool IsOpenRefundInsurance
        {
            get { return _isOpenRefundInsurance; }

            set
            {
                if (_isOpenRefundInsurance == value) return;

                RaisePropertyChanging(IsOpenRefundInsurancePropertyName);
                _isOpenRefundInsurance = value;
                RaisePropertyChanged(IsOpenRefundInsurancePropertyName);
            }
        }

        #endregion

        #region Money
        /// <summary>
        /// 极速退款价格
        /// </summary>
        private decimal _refundSingle;
        /// <summary>
        /// 航意险每份单价
        /// </summary>
        private decimal _unexpectedSinglePrice;
        /// <summary>
        /// 保单总份数
        /// </summary>
        private int _buyInsuranceAllCount;
        /// <summary>
        /// 已经购买的保险金额
        /// </summary>
        //private decimal _boughtInsuranceMoney;
        /// <summary>
        /// 已经购买的极速退金额
        /// </summary>
        //private decimal _boughtRefundMoney;
        #endregion

        #region UnexpectedPrice

        /// <summary>
        /// The <see cref="UnexpectedPrice" /> property's name.
        /// </summary>
        private const string UnexpectedPricePropertyName = "UnexpectedPrice";

        private decimal _unexpectedPrice;

        /// <summary>
        /// 航意险总金额
        /// </summary>
        public decimal UnexpectedPrice
        {
            get { return _unexpectedPrice; }

            set
            {
                if (_unexpectedPrice == value) return;

                RaisePropertyChanging(UnexpectedPricePropertyName);
                _unexpectedPrice = value;
                RaisePropertyChanged(UnexpectedPricePropertyName);
            }
        }

        #endregion

        #region RefundSinglePrice

        /// <summary>
        /// The <see cref="RefundSinglePrice" /> property's name.
        /// </summary>
        private const string RefundSinglePricePropertyName = "RefundSinglePrice";

        private decimal _refundSinglePrice;

        /// <summary>
        /// 极速退险金额
        /// </summary>
        public decimal RefundSinglePrice
        {
            get { return _refundSinglePrice; }

            set
            {
                if (_refundSinglePrice == value) return;

                RaisePropertyChanging(RefundSinglePricePropertyName);
                _refundSinglePrice = value;
                RaisePropertyChanged(RefundSinglePricePropertyName);
            }
        }

        #endregion

        #region TicketPrice

        /// <summary>
        /// The <see cref="SumPrice0" /> property's name.
        /// </summary>
        private const string TicketPricePropertyName = "TicketPrice";

        private decimal _ticketPrice;

        /// <summary>
        /// 票面价总金额
        /// </summary>
        public decimal TicketPrice
        {
            get { return _ticketPrice; }

            set
            {
                if (_ticketPrice == value) return;

                RaisePropertyChanging(TicketPricePropertyName);
                _ticketPrice = value;
                RaisePropertyChanged(TicketPricePropertyName);
            }
        }

        #endregion

        #region SumPrice0

        /// <summary>
        /// The <see cref="SumPrice0" /> property's name.
        /// </summary>
        private const string SumPrice0PropertyName = "SumPrice0";

        private decimal _sumPrice0;

        /// <summary>
        /// 结算总金额
        /// </summary>
        public decimal SumPrice0
        {
            get { return _sumPrice0; }

            set
            {
                if (_sumPrice0 == value) return;

                RaisePropertyChanging(SumPrice0PropertyName);
                _sumPrice0 = value;
                RaisePropertyChanged(SumPrice0PropertyName);
            }
        }

        #endregion

        #region SumPrice

        /// <summary>
        /// The <see cref="SumPrice" /> property's name.
        /// </summary>
        private const string SumPricePropertyName = "SumPrice";

        private decimal _sumPrice;

        /// <summary>
        /// 结算总金额
        /// </summary>
        public decimal SumPrice
        {
            get { return _sumPrice; }

            set
            {
                if (_sumPrice == value) return;

                RaisePropertyChanging(SumPricePropertyName);
                _sumPrice = value;
                RaisePropertyChanged(SumPricePropertyName);
            }
        }

        #endregion

        #region LastSeconds

        /// <summary>
        /// The <see cref="LastSeconds" /> property's name.
        /// </summary>
        private const string LastSecondsPropertyName = "LastSeconds";

        private string _lastSeconds = "00";

        /// <summary>
        /// 秒数
        /// </summary>
        public string LastSeconds
        {
            get { return _lastSeconds; }

            set
            {
                if (_lastSeconds == value) return;

                RaisePropertyChanging(LastSecondsPropertyName);
                _lastSeconds = value;
                RaisePropertyChanged(LastSecondsPropertyName);
            }
        }

        #endregion

        #region LastTimes

        /// <summary>
        /// The <see cref="LastTimes" /> property's name.
        /// </summary>
        private const string LastTimesPropertyName = "LastTimes";

        private string _lastTimes = "00:";

        /// <summary>
        /// 分钟
        /// </summary>
        public string LastTimes
        {
            get { return _lastTimes; }

            set
            {
                if (_lastTimes == value) return;

                RaisePropertyChanging(LastTimesPropertyName);
                _lastTimes = value;
                RaisePropertyChanged(LastTimesPropertyName);
            }
        }

        #endregion

        #region IsShowGrid

        /// <summary>
        /// The <see cref="IsShowGrid" /> property's name.
        /// </summary>
        private const string IsShowGridPropertyName = "IsShowGrid";

        private bool _isShowGrid;

        /// <summary>
        /// 是否显示乘机人列表
        /// </summary>
        public bool IsShowGrid
        {
            get { return _isShowGrid; }

            set
            {
                if (_isShowGrid == value) return;

                RaisePropertyChanging(IsShowGridPropertyName);
                _isShowGrid = value;
                RaisePropertyChanged(IsShowGridPropertyName);
            }
        }

        #endregion

        #region AllSexTypes

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string AllSexTypesPropertyName = "AllSexTypes";

        private ObservableCollection<KeyValuePair<EnumSexType?, String>> _allInsextypes = new ObservableCollection<KeyValuePair<EnumSexType?, string>>();

        /// <summary>
        /// 所有性别
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumSexType?, String>> AllInsextypes
        {
            get { return _allInsextypes; }

            set
            {
                if (_allInsextypes == value) return;

                RaisePropertyChanging(AllSexTypesPropertyName);
                _allInsextypes = value;
                RaisePropertyChanged(AllSexTypesPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 乘机人类型
        /// </summary>
        public ICollectionView AgeTypeItems { get; set; }
        private readonly List<KeyValuePair<dynamic, string>> c_AgeTypeItems;
        /// <summary>
        /// 证件类型集合
        /// </summary>
        public ICollectionView IDTypeItems { get; set; }
        private readonly List<KeyValuePair<dynamic, string>> c_IDTypeItems;

        #region IsDone

        /// <summary>
        /// The <see cref="IsDone" /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

        /// <summary>
        /// 是否已经确认投保
        /// </summary>
        public bool IsDone
        {
            get { return _isDone; }

            set
            {
                if (_isDone == value) return;

                RaisePropertyChanging(IsDonePropertyName);
                _isDone = value;
                RaisePropertyChanged(IsDonePropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令
        #region PayOrderByQuikPayCommand

        private RelayCommand<string> _payOrderByQuikPayCommand;

        /// <summary>
        /// 快捷支付
        /// </summary>
        public RelayCommand<string> PayOrderByQuikPayCommand
        {
            get
            {
                return _payOrderByQuikPayCommand ?? (_payOrderByQuikPayCommand = new RelayCommand<string>(ExecutePayOrderByQuikPayCommand, CanExecutePayOrderByQuikPayCommand));
            }
        }

        private void ExecutePayOrderByQuikPayCommand(string password)
        {
            BindInsuranceToOder();
            #region 订单倒计时
            if (IsOverTime) return;
            #endregion
            #region 保险相关
            AllPassengers = Order.Passengers.ToList();
            _buyInsuranceAllCount = AllPassengers.Count; AllPassengers.Where(p => p.IsInsuranceRefund).ToList().ForEach(p => p.InsuranceRefunrPrice = _refundSingle);
            #endregion
            //执行正常订单支付流程
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                //保单处理
                if (_buyInsuranceAllCount > 0)
                {
                    service.SaveInsurance(new RequestInsurance { BuyInsuranceAllCount = AllInsuranceCount, OrderId = Order.OrderId, UnexpectedPassenger = AllPassengers });
                    SendRefreshInsuranceCountMessage();
                }
                CommunicateManager.Invoke<IOrderService>(p =>
                {
                    try
                    {
                        //如果极速退服务费不为零
                        if (RefundSinglePrice > 0) p.UpdateOrderPayMoney(_order.OrderId, _order.PayInfo.PayMoney + RefundSinglePrice);
                        //调用服务接口执行支付操作
                        string info = p.PayOrderByQuikAliPay(_order.OrderId, password);
                        if (info == "True")
                        {
                            UIManager.ShowMessage(p.QueryPayStatus(_order.OrderId));
                            IsPaid = true;
                            return;
                        }
                        UIManager.ShowMessage("支付成功！");
                        IsPaid = true;
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(LogType.ERROR, "支付异常。原因：" + e.Message);
                        //todo 出现异常 调用保险服务撤销保险记录
                        service.DeleteInsurance(_order.OrderId);
                        throw;
                    }

                }, UIManager.ShowErr);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecutePayOrderByQuikPayCommand(string password)
        {
            return !_isBusy && !String.IsNullOrWhiteSpace(password);
        }

        #endregion

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
            BindInsuranceToOder();
            #region 订单倒计时
            if (IsOverTime)
            {
                return;
            }
            #endregion

            AllPassengers = Order.Passengers.ToList();
            _buyInsuranceAllCount = AllPassengers.Count;
            AllPassengers.Where(p => p.IsInsuranceRefund).ToList().ForEach(p => p.InsuranceRefunrPrice = _refundSingle);
            //执行正常订单支付流程
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                //保单处理
                if (_buyInsuranceAllCount > 0)
                {
                    service.SaveInsurance(new RequestInsurance { BuyInsuranceAllCount = AllInsuranceCount, OrderId = Order.OrderId, UnexpectedPassenger = AllPassengers });
                    SendRefreshInsuranceCountMessage();
                }
                CommunicateManager.Invoke<IOrderService>(p =>
                {
                    try
                    {
                        //如果极速退服务费不为零
                        if (RefundSinglePrice > 0)
                        {
                            p.UpdateOrderPayMoney(_order.OrderId, _order.PayInfo.PayMoney + RefundSinglePrice);
                        }
                        string uri = p.PayOrderByBank(_order.OrderId, bankCode);
                        if (uri == "True")
                        {
                            UIManager.ShowMessage(p.QueryPayStatus(_order.OrderId));
                            IsPaid = true;
                            return;
                        }
                        LocalUIManager.OpenDefaultBrowser(uri);
                        bool? isOk = UIManager.ShowPayWindow();
                        if (isOk != null && isOk.Value)
                            IsPaid = true;
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(LogType.ERROR, "支付异常。原因：" + e.Message);
                        //todo 出现异常 调用保险服务撤销保险记录
                        service.DeleteInsurance(_order.OrderId);
                        throw;
                    }

                }, UIManager.ShowErr);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });

        }

        private bool CanExecutePayOrderByBankCommand(string bankCode)
        {
            return !_isBusy && bankCode != null;
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
            BindInsuranceToOder();
            #region 订单倒计时
            if (IsOverTime)
            {
                return;
            }
            #endregion

            AllPassengers = Order.Passengers.ToList();
            _buyInsuranceAllCount = AllPassengers.Count; AllPassengers.Where(p => p.IsInsuranceRefund).ToList().ForEach(p => p.InsuranceRefunrPrice = _refundSingle);
            //执行正常订单支付流程
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                //保单处理
                if (_buyInsuranceAllCount > 0)
                {
                    service.SaveInsurance(new RequestInsurance { BuyInsuranceAllCount = AllInsuranceCount, OrderId = Order.OrderId, UnexpectedPassenger = AllPassengers });
                    SendRefreshInsuranceCountMessage();
                }
                CommunicateManager.Invoke<IOrderService>(p =>
                {
                    try
                    {
                        //如果极速退服务费不为零
                        if (RefundSinglePrice > 0)
                        {
                            p.UpdateOrderPayMoney(_order.OrderId, _order.PayInfo.PayMoney + RefundSinglePrice);
                        }
                        string uri = p.PayOrderByPlatform(_order.OrderId, code);
                        if (uri == "True")
                        {
                            UIManager.ShowMessage(p.QueryPayStatus(_order.OrderId));
                            IsPaid = true;
                            return;
                        }
                        LocalUIManager.OpenDefaultBrowser(uri);
                        bool? isOk = UIManager.ShowPayWindow();
                        if (isOk != null && isOk.Value)
                            IsPaid = true;
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(LogType.ERROR, "支付异常。原因：" + e.Message);
                        //todo 出现异常 调用保险服务撤销保险记录
                        service.DeleteInsurance(_order.OrderId);
                        throw;
                    }

                }, UIManager.ShowErr);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });

        }

        private bool CanExecutePayOrderByPlatformCommand(string code)
        {
            return !_isBusy && code != null;
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
            BindInsuranceToOder();
            if (_order.PayInfo.PayStatus == EnumPayStatus.OK) //如果该订单状态为已支付
            {
            }
            else
            {
                #region 订单倒计时
                if (IsOverTime)
                {
                    return;
                }
                #endregion

                AllPassengers = Order.Passengers.ToList();
                _buyInsuranceAllCount = AllPassengers.Count; AllPassengers.Where(p => p.IsInsuranceRefund).ToList().ForEach(p => p.InsuranceRefunrPrice = _refundSingle);
                //执行正常订单支付流程
                IsBusy = true;
                Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
                {
                    //保单处理
                    if (_buyInsuranceAllCount > 0)
                    {
                        service.SaveInsurance(new RequestInsurance { BuyInsuranceAllCount = AllInsuranceCount, OrderId = Order.OrderId, UnexpectedPassenger = AllPassengers });
                        SendRefreshInsuranceCountMessage();
                    }
                    CommunicateManager.Invoke<IOrderService>(p =>
                    {
                        try
                        {
                            var info = p.QueryPayStatus(_order.OrderId, "pay");
                            if (info == "已支付")
                            {
                                UIManager.ShowMessage("订单已支付");
                                IsPaid = true;
                                return;
                            }
                            //如果极速退服务费不为零
                            if (RefundSinglePrice > 0)
                            {
                                p.UpdateOrderPayMoney(_order.OrderId, _order.PayInfo.PayMoney + RefundSinglePrice);
                            }
                            p.PayOrderByCashbagAccount(_order.OrderId, password);
                            UIManager.ShowMessage("支付成功！");
                            IsPaid = true;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, "支付异常。原因：" + e.Message);
                            //todo 出现异常 调用保险服务撤销保险记录
                            service.DeleteInsurance(_order.OrderId);
                            throw;
                        }

                    }, UIManager.ShowErr);
                }, UIManager.ShowErr);

                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    Action setBusyAction = () => { IsBusy = false; };
                    DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                });
            }
        }

        private bool CanExecutePayOrderByCashbagAccountCommand(string password)
        {
            return !_isBusy && !String.IsNullOrWhiteSpace(password);
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
            BindInsuranceToOder();
            if (_order.PayInfo.PayStatus == EnumPayStatus.OK) //如果该订单状态为已支付
            {
            }
            else
            {
                #region 订单倒计时
                if (IsOverTime)
                {
                    return;
                }
                #endregion

                AllPassengers = Order.Passengers.ToList();
                _buyInsuranceAllCount = AllPassengers.Count; AllPassengers.Where(p => p.IsInsuranceRefund).ToList().ForEach(p => p.InsuranceRefunrPrice = _refundSingle);
                //执行正常订单支付流程
                IsBusy = true;
                Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
                {
                    //保单处理
                    if (_buyInsuranceAllCount > 0)
                    {
                        service.SaveInsurance(new RequestInsurance { BuyInsuranceAllCount = AllInsuranceCount, OrderId = Order.OrderId, UnexpectedPassenger = AllPassengers });
                        SendRefreshInsuranceCountMessage();
                    }
                    CommunicateManager.Invoke<IOrderService>(p =>
                    {
                        try
                        {
                            var info = p.QueryPayStatus(_order.OrderId, "pay");
                            if (info == "已支付")
                            {
                                UIManager.ShowMessage("订单已支付");
                                IsPaid = true;
                                return;
                            }
                            //如果极速退服务费不为零
                            if (RefundSinglePrice > 0)
                            {
                                p.UpdateOrderPayMoney(_order.OrderId, _order.PayInfo.PayMoney + RefundSinglePrice);
                            }
                            p.PayOrderByCreditAccount(_order.OrderId, password);
                            UIManager.ShowMessage("支付成功！");
                            IsPaid = true;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, "支付异常。原因：" + e.Message);
                            //todo 出现异常 调用保险服务撤销保险记录
                            service.DeleteInsurance(_order.OrderId);
                            throw;
                        }

                    }, UIManager.ShowErr);
                }, UIManager.ShowErr);

                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    Action setBusyAction = () => { IsBusy = false; };
                    DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                });
            }
        }

        private bool CanExecutePayOrderByCreditAccountCommand(string password)
        {
            return !_isBusy && !String.IsNullOrWhiteSpace(password) && !IsPaid;
        }

        #endregion

        #region CofirmBuyInsuranceCommand

        private RelayCommand<string> _cofirmBuyInsuranceCommand;

        /// <summary>
        /// Gets the CofirmBuyInsuranceCommand.
        /// </summary>
        public RelayCommand<string> CofirmBuyInsuranceCommand
        {
            get
            {
                return _cofirmBuyInsuranceCommand ?? (_cofirmBuyInsuranceCommand = new RelayCommand<string>(ExecuteCofirmBuyInsuranceCommand, CanExecuteCofirmBuyInsuranceCommand));
            }
        }

        private void ExecuteCofirmBuyInsuranceCommand(string parm)
        {
            if (!CheckBirthandId()) return;
            BindInsuranceToOder();
            if (_order.PayInfo.PayStatus == EnumPayStatus.OK) //如果该订单状态为已支付
            {
                InPassers = Order.Passengers.ToList();
                //没有购买保险操作
                if (InPassers.Count(p => p.IsBuyInsurance) == 0)
                {
                    var dialog = UIManager.ShowMessageDialog("您还没有购买保险，确定需要购买？");
                    if (dialog != null && !(bool)dialog) IsPaid = true; return;
                }
                //保险份数不足判断
                if (InPassers.Sum(p => p.BuyInsuranceCount) > InsuranceLeaveCount) { UIManager.ShowMessage("最多购买保险份数为" + InsuranceLeaveCount + "份"); return; }
                //执行单独接口实现保单支付
                IsBusy = true;
                Action action2 = () => CommunicateManager.Invoke<IInsuranceService>(service =>
                {
                    service.BuyInsuranceByCashOrCredit(Order.OrderId, InPassers, 0, parm);
                    SendRefreshInsuranceCountMessage();
                    UIManager.ShowMessage("购买成功！");
                    IsPaid = true;
                }, UIManager.ShowErr);

                Task.Factory.StartNew(action2).ContinueWith(task =>
                {
                    Action setBusyAction = () => { IsBusy = false; };
                    DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                });
            }
        }

        private bool CanExecuteCofirmBuyInsuranceCommand(string parm)
        {
            return !_isBusy;
        }

        #endregion

        #region SumPriceCommand

        private RelayCommand<string> _sumPriceCommand;

        /// <summary>
        /// Gets the CofirmBuyInsuranceCommand.
        /// </summary>
        public RelayCommand<string> SumPriceCommand
        {
            get
            {
                return _sumPriceCommand ?? (_sumPriceCommand = new RelayCommand<string>(ExecuteSumPriceCommand, CanExecuteSumPriceCommand));
            }
        }

        private void ExecuteSumPriceCommand(string parm)
        {
            if (IsShowTicketPrice) SumPrice0 = TicketPrice + RefundSinglePrice;
            else SumPrice = Order.PayInfo.PayMoney + RefundSinglePrice;
        }

        private bool CanExecuteSumPriceCommand(string parm)
        {
            return !_isBusy;
        }

        #endregion

        #region InitlizeCommand

        private RelayCommand _initlizeCommand;

        /// <summary>
        /// 初始化命令.
        /// </summary>
        public RelayCommand InitlizeCommand
        {
            get
            {
                return _initlizeCommand ?? (_initlizeCommand = new RelayCommand(ExecuteInitlizeCommand, CanExecuteInitlizeCommand));
            }
        }

        private void ExecuteInitlizeCommand()
        {
            IsLoading = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                AccountInfo = service.GetAccountInfo();
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteInitlizeCommand()
        {
            return !IsLoading;
        }

        #endregion

        #region OpenCommand 弹出窗口

        private RelayCommand _openCommand;

        /// <summary>
        /// 打开弹出窗口
        /// </summary>
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(ExecuteOpenCommand, CanExecuteOpenCommand));
            }
        }

        private void ExecuteOpenCommand()
        {
            //if(InPassers.Count == 0 && Order != null)InPassers = Order.Passengers.Where(p=>p.IsBuyInsurance == false).ToList();
            //flag = 0;
            //LocalUIManager.ShowPassengers(this, (dialogResult) =>
            //{
            //    if (dialogResult != null && dialogResult.Value)
            //    {
            //        foreach (var item in InPassers)
            //        {
            //            if (item.BuyInsuranceCount > 0)
            //                item.BuyInsurancePrice = UnexpectedSinglePrice;
            //        }
            //        AllPassengers.RemoveAll(p=>p.IsBuyInsurance);
            //        AllPassengers.AddRange(InPassers.Where(p => p.IsBuyInsurance));
            //    }
            //    //计算显示金额信息
            //    BuyInsuranceAllCount = AllPassengers.Count;
            //    UnexpectedPrice = BoughtInsuranceMoney + InPassers.Where(p => p.IsBuyInsurance).Sum(p => p.BuyInsuranceCount * p.BuyInsurancePrice);
            //    SumPrice = BoughtRefundMoney > 0 ? UnexpectedPrice + Order.PayInfo.PayMoney : UnexpectedPrice + RefundSinglePrice + Order.PayInfo.PayMoney;

            //});
            if (InsuranceLeaveCount == 0) { UIManager.ShowMessage("请先向运营商购买保险"); return; }
            LocalUIManager.ShowConfirmPayInsurance(this, dialogResult =>
                {
                    if (!(dialogResult.HasValue && dialogResult.Value))
                    { _payInsuranceModels.ForEach(x => x.PayInsuranceSkyWayModels.ToList().ForEach(z => z.InsuranceCount = 0)); }
                });
        }

        private bool CanExecuteOpenCommand()
        {
            return !IsBusy;
        }

        private RelayCommand _openPassengersCommand2;

        /// <summary>
        /// 打开选择乘客信息窗口命令(极速退)
        /// </summary>
        public RelayCommand OpenPassengersCommand2
        {
            get
            {
                return _openPassengersCommand2 ?? (_openPassengersCommand2 = new RelayCommand(ExecuteOpenPassengersCommand2, CanExecuteOpenPassengersCommand2));
            }
        }

        private void ExecuteOpenPassengersCommand2()
        {
            //if (RePassers.Count == 0 && Order != null) RePassers = Order.Passengers.Where(p=>p.IsInsuranceRefund == false).ToList();
            //flag = 1;
            //LocalUIManager.ShowPassengers(this, (dialogResult) =>
            //{
            //    if (dialogResult != null && dialogResult.Value)
            //    {
            //        foreach (var item in RePassers)
            //        {
            //            if (item.IsInsuranceRefund)
            //                item.InsuranceRefunrPrice = RefundSingle;
            //        }
            //        AllPassengers.RemoveAll(p=>p.IsInsuranceRefund);
            //        AllPassengers.AddRange(RePassers.Where(p => p.IsInsuranceRefund));
            //    }
            //    //计算显示金额信息
            //    BuyInsuranceAllCount = AllPassengers.Count;
            //    RefundSinglePrice = BoughtRefundMoney + RefundSingle * RePassers.Count(p => p.IsInsuranceRefund);
            //    SumPrice = BoughtRefundMoney > 0 ? UnexpectedPrice + Order.PayInfo.PayMoney : RefundSinglePrice + UnexpectedPrice + Order.PayInfo.PayMoney;

            //});
        }

        private bool CanExecuteOpenPassengersCommand2()
        {
            return !IsBusy;
        }

        #endregion

        #region QuickBackCommand 极速退服务协议

        private RelayCommand _quickBackCommand;

        /// <summary>
        /// 打开极速退服务协议窗口命令
        /// </summary>
        public RelayCommand QuickBackCommand
        {
            get
            {
                return _quickBackCommand ?? (_quickBackCommand = new RelayCommand(ExecuteQuickBackCommand, CanExecuteQuickBackCommand));
            }
        }

        private void ExecuteQuickBackCommand()
        {
            UIManager.ShowWeb("买票宝极速退款服务协议", "http://www.51cbc.cn/faq/jst.htm");
        }

        private bool CanExecuteQuickBackCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region ShowGirdCommand 展开

        private RelayCommand _showGirdCommand;

        /// <summary>
        /// 展开
        /// </summary>
        public RelayCommand ShowGridCommand
        {
            get
            {
                return _showGirdCommand ?? (_showGirdCommand = new RelayCommand(ExecuteShowGirdCommand, CanExecuteShowGirdCommand));
            }
        }

        private void ExecuteShowGirdCommand()
        {
            IsShowGrid = true;
        }

        private bool CanExecuteShowGirdCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region HideGirdCommand 折叠

        private RelayCommand _hideGirdCommand;

        /// <summary>
        /// 折叠
        /// </summary>
        public RelayCommand HideGridCommand
        {
            get
            {
                return _hideGirdCommand ?? (_hideGirdCommand = new RelayCommand(ExecuteHideGirdCommand, CanHideGirdCommand));
            }
        }

        private void ExecuteHideGirdCommand()
        {
            IsShowGrid = false;
        }

        private bool CanHideGirdCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region ValueCommand 保险份数变化事件

        private RelayCommand _valueCommand;

        /// <summary>
        /// 保险份数变化事件
        /// </summary>
        public RelayCommand ValueCommand
        {
            get
            {
                return _valueCommand ?? (_valueCommand = new RelayCommand(ExecuteValueCommand, CanValueCommand));
            }
        }

        private void ExecuteValueCommand()
        {
            //UnexpectedPrice = Order.Passengers.Sum(p => p.BuyInsuranceCount * p.BuyInsurancePrice);
            //SumPrice = UnexpectedPrice + RefundSinglePrice + Order.PayInfo.PayMoney;
            //if (Order.Passengers.Sum(p => p.BuyInsuranceCount) > InsuranceLeaveCount) UIManager.ShowMessage("最多购买保险份数为" + InsuranceLeaveCount + "份");
            AllInsuranceCount = 0;
            PayInsuranceModels.ForEach(x =>
            {
                if (x.PayInsuranceSkyWayModels.Sum(m => m.InsuranceCount) > 0)
                    AllInsuranceCount += x.PayInsuranceSkyWayModels.Sum(m => m.InsuranceCount);
            });
        }

        private bool CanValueCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region CountRefundCommand 极速退选择变化事件

        private RelayCommand _countRefundCommand;

        /// <summary>
        /// 极速退份数变化事件
        /// </summary>
        public RelayCommand CountRefundCommand
        {
            get
            {
                return _countRefundCommand ?? (_countRefundCommand = new RelayCommand(ExecuteCountRefundCommand, CanCountRefundCommand));
            }
        }

        private void ExecuteCountRefundCommand()
        {
            RefundSinglePrice = Order.Passengers.Count(p => p.IsInsuranceRefund) * _refundSingle;
            //SumPrice = UnexpectedPrice + RefundSinglePrice + Order.PayInfo.PayMoney;
            //SumPrice = RefundSinglePrice + Order.PayInfo.PayMoney;
            ExecuteSumPriceCommand("");
            if (Order.Passengers.Count(p => p.IsInsuranceRefund) > 0)
            {
                if (Order.Passengers.Count(p => p.IsInsuranceRefund) >= 2) return;
                UIManager.ShowMessage("极速退服务处理时间为09:00—21:00, 且不支持非自愿退票；此产品与机票无关，一旦购买均不退款");
            }
        }

        private bool CanCountRefundCommand()
        {
            return !IsBusy;
        }
        //取消选择的事件
        private RelayCommand _countRefundCommand0;

        /// <summary>
        /// 极速退份数变化事件
        /// </summary>
        public RelayCommand CountRefundCommand0
        {
            get
            {
                return _countRefundCommand0 ?? (_countRefundCommand0 = new RelayCommand(ExecuteCountRefundCommand0, CanCountRefundCommand0));
            }
        }

        private void ExecuteCountRefundCommand0()
        {
            RefundSinglePrice = Order.Passengers.Count(p => p.IsInsuranceRefund) * _refundSingle;
            //SumPrice = RefundSinglePrice + Order.PayInfo.PayMoney;
            ExecuteSumPriceCommand("");
        }

        private bool CanCountRefundCommand0()
        {
            return !IsBusy;
        }

        #endregion

        #region ConfirmInsuranceCommand 确认投保

        private RelayCommand _confirmInsuranceCommand;

        /// <summary>
        /// 确认投保命令
        /// </summary>
        public RelayCommand ConfirmInsuranceCommand
        {
            get
            {
                return _confirmInsuranceCommand ?? (_confirmInsuranceCommand = new RelayCommand(ExecuteConfirmInsuranceCommand, CanExecuteConfirmInsuranceCommand));
            }
        }

        private void ExecuteConfirmInsuranceCommand()
        {
            if (!CheckBirthandId()) return;
            if (AllInsuranceCount == 0)
            {
                //没有购买保险操作
                var dialog = UIManager.ShowMessageDialog("您还没有购买保险，确定需要购买？");
                if (dialog != null && (bool)dialog) return;
            }
            else
            {
                //保险份数不足判断
                if (AllInsuranceCount > InsuranceLeaveCount) { UIManager.ShowMessage("最多购买保险份数为" + InsuranceLeaveCount + "份"); return; }
            }
            IsDone = true;
        }

        private bool CanExecuteConfirmInsuranceCommand()
        {
            return !IsBusy;
        }

        #endregion

        #endregion

        internal void LoadOrderInfo(string orderId)
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var temp = service.FindAll(orderId, null, null, null, null, null, 0, 1);
                if (temp != null && temp.TotalCount > 0)
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                    {
                        Order = temp.List[0];

                    }));
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private void SendRefreshInsuranceCountMessage()
        {
            Messenger.Default.Send(true, "buyinsurancemessage");
        }

        /// <summary>
        /// 保险份数赋值
        /// </summary>
        private void BindInsuranceToOder()
        {
            //Console.Write("                    ");
            //Console.Write("\n赋值");
            if (AllInsuranceCount == 0) return;//保险选择份数为零时判断执行
            Order.Passengers.ToList().ForEach(o =>
            {
                o.BuyInsuranceCount = 0;
                PayInsuranceModels.ForEach(p =>
                {
                    if (o.Id != p.PassengerId) return;
                    o.CardNo = p.ID;
                    o.Birth = p.Birthday;
                    o.IdType = p.IDType;
                    o.SexType = p.SexType;
                    o.PassengerType = p.PassengerType;
                    o.Mobile = p.Tel;
                    //Console.Write("\n" + p.PassengerName + "            ");
                    //Console.Write("    HasCode:" + o.SkyWays.GetHashCode() + "            ");
                    o.SkyWays.ForEach(s => p.PayInsuranceSkyWayModels.ForEach(ps =>
                    {
                        if (s.SkyWayId != ps.SkyWayId) return;
                        var temp = s;
                        temp.InsuranceCount = ps.InsuranceCount;
                        s = temp;
                        o.BuyInsuranceCount += ps.InsuranceCount;
                        // Console.Write(s.SkyWayId + "           " + s.InsuranceCount + "\t\t");
                    }));
                });
            });
            //Console.Write("                    ");
            //Console.Write("\n\n赋值后\n");
            //Order.Passengers.ToList().ForEach(p =>
            //{
            //    Console.Write(p.PassengerName + "            ");
            //    Console.Write("    HasCode:" + p.SkyWays.GetHashCode() + "            ");
            //    p.SkyWays.ToList().ForEach(x =>
            //    {
            //        Console.Write(x.SkyWayId + "           " + x.InsuranceCount + "\t\t");
            //    });
            //    Console.Write("\n");

            //});
        }

        /// <summary>
        /// 投保时验证出生日期和证件号
        /// </summary>
        /// <returns></returns>
        private bool CheckBirthandId()
        {
            var iscontinue = true;
            foreach (var item in PayInsuranceModels)
            {
                if (item.SexType == EnumSexType.UnKnown)
                {
                    UIManager.ShowMessage(string.Format("请选择{0}的性别", item.PassengerName)); iscontinue = false; break;
                }
                if (item.Birthday == null)
                {
                    UIManager.ShowMessage(string.Format("请输入{0}的出生日期", item.PassengerName)); iscontinue = false; break;
                }
                if (item.IDType == EnumIDType.NormalId && item.ID.Trim().Length != 18)
                {
                    UIManager.ShowMessage(string.Format("{0}的证件号输入格式有误", item.PassengerName)); iscontinue = false; break;
                }
                if (item.IDType != EnumIDType.BirthDate && string.IsNullOrWhiteSpace(item.ID))
                {
                    UIManager.ShowMessage(string.Format("{0}的证件号不能为空", item.PassengerName)); iscontinue = false; break;
                }
                if (!string.IsNullOrWhiteSpace(item.Tel) && !System.Text.RegularExpressions.Regex.IsMatch(item.Tel.Trim(), @"^1[1-9]\d{9}$"))
                {
                    UIManager.ShowMessage(string.Format("{0}的手机号码输入不正确", item.PassengerName)); iscontinue = false; break;
                }
            }
            return iscontinue;
        }

    }
}