using System.Globalization;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 售后详情视图模型
    /// </summary>
    public class AfterSaleInfoViewModel : BaseVM
    {
        #region 成员变量

        private int afterId;

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterSaleInfoViewModel"/> class.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <param name="afterId">The after identifier.</param>
        public AfterSaleInfoViewModel(string orderId, int afterId)
        {
            OrderId = orderId;
            this.afterId = afterId;
            Id = this.afterId.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var temp = service.GetAfterSaleOrderDetail(_orderId, afterId);
                    ResponseAnnulOrder = temp as ResponseAnnulOrder;
                    ResponseBounceOrder = temp as ResponseBounceOrder;
                    ResponseChangeOrder = temp as ResponseChangeOrder; 
                    AfterSaleInfo = temp;
                }, UIManager.ShowErr);
                if (ResponseChangeOrder != null)
                {
                    IsHaveBounceLines = false;
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
                                       NewToDateTime=p.NewToDateTime,
                                       Seat = x.Seat,
                                       StartDateTime = x.StartDateTime,
                                       ToCity = x.ToCity,
                                       ToCityCode = x.ToCityCode,
                                       ToDateTime = x.ToDateTime,
                                       ToTerminal = x.ToTerminal
                                   };
                    ChangeSkyWayList = tempList.ToList();
                    var list = new List<ResponseChangeOrder>
                    {
                        new ResponseChangeOrder
                        {
                            OutPayNo = ResponseChangeOrder.OutPayNo,
                            PayWay = ResponseChangeOrder.PayWay,
                            PayTime = ResponseChangeOrder.PayTime
                        }
                    };
                    ChangeOrderPayInfoList = list;
                }
                if (ResponseAnnulOrder!= null)
                {
                    IsHaveBounceLines = true;
                    BounceLinesList = _responseAnnulOrder.BounceLines;
                }
                if (ResponseBounceOrder == null) return;
                IsHaveBounceLines = true;
                BounceLinesList = ResponseBounceOrder.BounceLines;
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

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

        #region AfterSaleInfo

        /// <summary>
        /// The <see cref="AfterSaleInfo" /> property's name.
        /// </summary>
        private const string AfterSaleInfoPropertyName = "AfterSaleInfo";

        private ResponseAfterSaleOrder _afterSaleInfo;

        /// <summary>
        /// 售后详情
        /// </summary>
        public ResponseAfterSaleOrder AfterSaleInfo
        {
            get { return _afterSaleInfo; }

            set
            {
                if (_afterSaleInfo == value) return;

                RaisePropertyChanging(AfterSaleInfoPropertyName);
                _afterSaleInfo = value;
                RaisePropertyChanged(AfterSaleInfoPropertyName);
            }
        }

        #endregion

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

        #region Id

        /// <summary>
        /// The <see cref="OrderId" /> property's name.
        /// </summary>
        private const string IdPropertyName = "Id";

        private string _id;

        /// <summary>
        /// 售后订单号
        /// </summary>
        public string Id
        {
            get { return _id; }

            set
            {
                if (_id == value) return;

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        #endregion

        #region 废票

        #region IsAnnulOrder

        /// <summary>
        /// The <see cref="IsAnnulOrder" /> property's name.
        /// </summary>
        private const string IsAnnulOrderPropertyName = "IsAnnulOrder";

        private bool _isAnnulOrder = true;

        /// <summary>
        /// 是否废票
        /// </summary>
        public bool IsAnnulOrder
        {
            get { return _isAnnulOrder; }

            set
            {
                if (_isAnnulOrder == value) return;

                RaisePropertyChanging(IsAnnulOrderPropertyName);
                _isAnnulOrder = value;
                RaisePropertyChanged(IsAnnulOrderPropertyName);
            }
        }

        #endregion

        #region ResponseAnnulOrder

        /// <summary>
        /// The <see cref="ResponseAnnulOrder" /> property's name.
        /// </summary>
        private const string ResponseAnnulOrderPropertyName = "ResponseAnnulOrder";

        private ResponseAnnulOrder _responseAnnulOrder;

        /// <summary>
        /// 废票信息
        /// </summary>
        public ResponseAnnulOrder ResponseAnnulOrder
        {
            get { return _responseAnnulOrder; }

            set
            {
                IsAnnulOrder = value != null;
                if (_responseAnnulOrder == value) return;

                RaisePropertyChanging(ResponseAnnulOrderPropertyName);
                _responseAnnulOrder = value;
                RaisePropertyChanged(ResponseAnnulOrderPropertyName);
            }
        }

        #endregion

        #endregion

        #region 退票

        #region IsBounceOrder

        /// <summary>
        /// The <see cref="IsBounceOrder" /> property's name.
        /// </summary>
        private const string IsBounceOrderPropertyName = "IsBounceOrder";

        private bool _isBounceOrder = true;

        /// <summary>
        /// 是否是退票
        /// </summary>
        public bool IsBounceOrder
        {
            get { return _isBounceOrder; }

            set
            {
                if (_isBounceOrder == value) return;

                RaisePropertyChanging(IsBounceOrderPropertyName);
                _isBounceOrder = value;
                RaisePropertyChanged(IsBounceOrderPropertyName);
            }
        }

        #endregion

        #region ResponseBounceOrder

        /// <summary>
        /// The <see cref="ResponseBounceOrder" /> property's name.
        /// </summary>
        private const string ResponseBounceOrderPropertyName = "ResponseBounceOrder";

        private ResponseBounceOrder _responseBounceOrder;

        /// <summary>
        /// 退票信息
        /// </summary>
        public ResponseBounceOrder ResponseBounceOrder
        {
            get { return _responseBounceOrder; }

            set
            {
                IsBounceOrder = value != null;

                if (_responseBounceOrder == value) return;
                RaisePropertyChanging(ResponseBounceOrderPropertyName);
                _responseBounceOrder = value;
                RaisePropertyChanged(ResponseBounceOrderPropertyName);
            }
        }

        #endregion

        #endregion

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

        #region 改签

        #region IsChangeOrder

        /// <summary>
        /// The <see cref="IsChangeOrder" /> property's name.
        /// </summary>
        private const string IsChangeOrderPropertyName = "IsChangeOrder";

        private bool _isChangeOrder = true;

        /// <summary>
        /// 是否是改签订单
        /// </summary>
        public bool IsChangeOrder
        {
            get { return _isChangeOrder; }

            set
            {
                if (_isChangeOrder == value) return;

                RaisePropertyChanging(IsChangeOrderPropertyName);
                _isChangeOrder = value;
                RaisePropertyChanged(IsChangeOrderPropertyName);
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
                IsChangeOrder = value != null;

                if (_responseChangeOrder == value) return;
                RaisePropertyChanging(ResponseChangeOrderPropertyName);
                _responseChangeOrder = value;
                RaisePropertyChanged(ResponseChangeOrderPropertyName);
            }
        }

        #endregion

        #region 改签支付信息

        /// <summary>
        /// The <see cref="ChangeOrderPayInfoList" /> property's name.
        /// </summary>
        public const string ChangeOrderPayInfoListPropertyName = "ChangeOrderPayInfoList";

        private List<ResponseChangeOrder> _changeOrderPayInfoList;

        /// <summary>
        /// 改签订单
        /// </summary>
        public List<ResponseChangeOrder> ChangeOrderPayInfoList
        {
            get { return _changeOrderPayInfoList; }

            set
            {

                if (_changeOrderPayInfoList == value) return;
                RaisePropertyChanging(ChangeOrderPayInfoListPropertyName);
                _changeOrderPayInfoList = value;
                RaisePropertyChanged(ChangeOrderPayInfoListPropertyName);
            }
        }

        #endregion

        #endregion

        #region 是否是退/废票

        /// <summary>
        /// The <see cref="IsHaveBounceLines" /> property's name.
        /// </summary>
        private const string IsHaveBounceLinesPropertyName = "IsHaveBounceLines";

        private bool _isHaveBounceLines = true;

        /// <summary>
        /// 是否有退款明细
        /// </summary>
        public bool IsHaveBounceLines
        {
            get { return _isHaveBounceLines; }

            set
            {
                if (_isHaveBounceLines == value) return;

                RaisePropertyChanging(IsHaveBounceLinesPropertyName);
                _isHaveBounceLines = value;
                RaisePropertyChanged(IsHaveBounceLinesPropertyName);
            }
        } 

        #endregion

        #region 退款明细

        /// <summary>
        /// The <see cref="BounceLinesList" /> property's name.
        /// </summary>
        private const string BounceLinesListPropertyName = "BounceLinesList";

        private List<ResponseBounceLine> _bounceLinesList;

        /// <summary>
        ///退款明细
        /// </summary>
        public List<ResponseBounceLine> BounceLinesList
        {
            get { return _bounceLinesList; }

            set
            {
                if (_bounceLinesList == value) return;

                RaisePropertyChanging(BounceLinesListPropertyName);
                _bounceLinesList = value;
                RaisePropertyChanged(BounceLinesListPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令
        #region OpenOrderInfoCommand

        private RelayCommand<string> _openTravelCommand;

        /// <summary>
        /// 打开订单详情页面命令
        /// </summary>
        public RelayCommand<string> OpenTravelCommand
        {
            get
            {
                return _openTravelCommand ?? (_openTravelCommand = new RelayCommand<string>(ExecuteOpenTravelCommand));
            }
        }

        private void ExecuteOpenTravelCommand(string pid)
        {
            
            ResponseAfterSalePassenger passenger = AfterSaleInfo.Passenger.FirstOrDefault(p => p.Id.ToString(CultureInfo.InvariantCulture) == pid);
            if (passenger != null && passenger.Status == EnumTfgPassengerStatus.ChangeTicketed)
            {
                LocalUIManager.ShowPrintTravel(null, null, _responseChangeOrder, passenger, 1,null,this, dialogResult =>
                {
                    if (dialogResult == null || !dialogResult.Value) return;
                    Initialize();
                });
            }
            else
            {
                UIManager.ShowMessage("改签完成状态下才能打印行程单");
            }
        }

        #endregion 
        #endregion
    }
}
