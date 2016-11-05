using System.ComponentModel;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 退改签视图模型
    /// </summary>
    public class ReissueViewModel : BaseVM
    {
        private enum RequestMode
        {
            /// <summary>
            /// 退票申请
            /// </summary>
            BounceOrder,

            /// <summary>
            /// 废票申请
            /// </summary>
            AnnulOrder,

            /// <summary>
            /// 改签申请
            /// </summary>
            ChangeOrder,

            /// <summary>
            /// 婴儿申请 
            /// </summary>
            ApplyBabyOrder,

            /// <summary>
            /// 其他理由申请
            /// </summary>
            Modify
        }

        #region 成员变量

        private RequestMode _currentMode = RequestMode.BounceOrder;//当前申请模式 

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="ReissueViewModel"/> class.
        /// </summary>
        public ReissueViewModel()
        {
            AnnulReasons.Add("我方已经作废行程单/未创建行程单，并且取消位置，申请卖家废票！");
            AnnulReasons.Add("一个编码贴有多个票号，申请卖家废票！");

            ChangeReasons.Add("乘客需要改签升舱。");
            ChangeReasons.Add("同等舱位更改时间");
            ChangeReasons.Add("航班延误，机场关闭，申请免费改签。");

            ModifyReasons.Add("修改乘机人证件号");
            ModifyReasons.Add("修改乘机人联系方式");
            ModifyReasons.Add("时限项修改为票号（仅用于B2B客票操作时提交）");
            ModifyReasons.Add("申请票号挂起");
            ModifyReasons.Add("申请票号解挂");

            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Male, EnumHelper.GetDescription(EnumSexType.Male)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Female, EnumHelper.GetDescription(EnumSexType.Female)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.UnKnown, EnumHelper.GetDescription(EnumSexType.UnKnown)));
            
            BabyInformationItemsItems = new ObservableCollection<BabyModel>();

            IsVoluntary = true;
            IsSameSpaceChange = true;
        }

        #endregion

        #region 公开属性

        #region IsVoluntary 是否自愿申请退票

        /// <summary>
        /// The <see cref="IsVoluntary" /> property's name.
        /// </summary>
        private const string IsVoluntaryPropertyName = "IsVoluntary";

        private bool _isVoluntary;

        /// <summary>
        /// 是否自愿申请退票(仅退票可用）
        /// </summary>
        public bool IsVoluntary
        {
            get { return _isVoluntary; }

            set
            {
                BounceReasons.Clear();
                if (value)
                {
                    BounceReasons.Add("客票为换开状态，申请自愿退票。");
                    BounceReasons.Add("已作废/未创建行程单，已取消位置。");
                }
                else
                {
                    BounceReasons.Add("因航空公司原因导致客人非自愿降舱,【申请全退】。");
                    BounceReasons.Add("客票不退，申请退回多收票款。即：只退款，不退票,【申请退回多收票款】。");
                    BounceReasons.Add("客户重复购买同一类型的机票（都是B2B或都是BSP），同终端出票，只退款，不退票,【申请全退】。");
                    BounceReasons.Add("同一航空公司因前段航班延误/取消，导致后段航班无法登机，已取消位置,【申请全退】。");
                    BounceReasons.Add("按航空公司规定，同一日期和航班，同终端出票，名其中一个字音同字不同，已取消位置,【申请全退】。");
                    BounceReasons.Add("同一编码在两个平台重复支付，同一供应商出票，只退款，不退票,【申请全退】。");
                    BounceReasons.Add("非供应商出票，只退款，不退票,【申请全退】。");
                    BounceReasons.Add("同一航空公司，在平台同一供应商出票，满足升舱全退条件，已取消位置,【申请全退】。");
                    BounceReasons.Add("同一编码在平台重复支付，同一供应商出票，只退款，不退票,【申请全退】。");
                    BounceReasons.Add("卖方当天已废票，已取消位置,【当废票处理】。");
                    BounceReasons.Add("供应商操作不规范，导致乘客不能正常登机，只退款，不退票,【申请全退】。");
                    BounceReasons.Add("因航空公司原因导致客人非自愿降舱，只退款，不退票,【申请退回多收票款】。");
                    BounceReasons.Add("供应商操作不规范，导致客人无法登机,【申请全退】。");
                    BounceReasons.Add("客户重复购买同一类型的机票（都是B2B或都是BSP），同终端出票,【申请全退】。");
                    BounceReasons.Add("PNR包含航班延误/取消信息，或我方已传真相关证明，已取消位置作废行程单,【申请全退】。");
                    BounceReasons.Add("已邮寄市级以上医院证明（诊断书、病例、旅客不能登机的证明、发票）原件，已取消位置,【申请全退】。");
                }

                if (_isVoluntary == value) return;

                RaisePropertyChanging(IsVoluntaryPropertyName);
                _isVoluntary = value;
                RaisePropertyChanged(IsVoluntaryPropertyName);

                RequestBounceOrder.IsVoluntary = value;
            }
        }

        #endregion

        #region IsSameSpaceChange

        /// <summary>
        /// The <see cref="IsSameSpaceChange" /> property's name.
        /// </summary>
        private const string IsSameSpaceChangePropertyName = "IsSameSpaceChange";

        private bool _isSameSpaceChange;

        /// <summary>
        /// 是否是同舱位改签
        /// </summary>
        public bool IsSameSpaceChange
        {
            get { return _isSameSpaceChange; }

            set
            {
                if (_isSameSpaceChange == value) return;

                RaisePropertyChanging(IsSameSpaceChangePropertyName);
                _isSameSpaceChange = value;
                RaisePropertyChanged(IsSameSpaceChangePropertyName);

                //恢复可能修改过的数据

                foreach (var item in _reissueSkyWays)
                    item.Recover();
            }
        }

        #endregion

        #region ReissueSkyWays

        /// <summary>
        /// The <see cref="ReissueSkyWays" /> property's name.
        /// </summary>
        private const string ReissueSkyWaysPropertyName = "ReissueSkyWays";

        private ObservableCollection<ReissueSkyWayObject> _reissueSkyWays = new ObservableCollection<ReissueSkyWayObject>();

        /// <summary>
        /// 改签航班信息
        /// </summary>
        public ObservableCollection<ReissueSkyWayObject> ReissueSkyWays
        {
            get { return _reissueSkyWays; }

            set
            {
                if (_reissueSkyWays == value) return;

                RaisePropertyChanging(ReissueSkyWaysPropertyName);
                _reissueSkyWays = value;
                RaisePropertyChanged(ReissueSkyWaysPropertyName);
            }
        }

        #endregion

        #region BounceReasons 退票理由

        /// <summary>
        /// The <see cref="BounceReasons" /> property's name.
        /// </summary>
        private const string BounceReasonsPropertyName = "BounceReasons";

        private ObservableCollection<string> _bounceReasons = new ObservableCollection<string>();

        /// <summary>
        /// 退票申请原因
        /// </summary>
        public ObservableCollection<string> BounceReasons
        {
            get { return _bounceReasons; }

            set
            {
                if (_bounceReasons == value) return;

                RaisePropertyChanging(BounceReasonsPropertyName);
                _bounceReasons = value;
                RaisePropertyChanged(BounceReasonsPropertyName);
            }
        }

        #endregion

        #region AnnulReasons 废票理由

        /// <summary>
        /// The <see cref="AnnulReasons" /> property's name.
        /// </summary>
        private const string AnnulReasonsPropertyName = "AnnulReasons";

        private List<string> _annulReasons = new List<string>();

        /// <summary>
        /// 废票申请理由
        /// </summary>
        public List<string> AnnulReasons
        {
            get { return _annulReasons; }

            set
            {
                if (_annulReasons == value) return;

                RaisePropertyChanging(AnnulReasonsPropertyName);
                _annulReasons = value;
                RaisePropertyChanged(AnnulReasonsPropertyName);
            }
        }

        #endregion

        #region ChangeReasons 改签理由

        /// <summary>
        /// The <see cref="ChangeReasons" /> property's name.
        /// </summary>
        private const string ChangeReasonsPropertyName = "ChangeReasons";

        private List<string> _changeReasons = new List<string>();

        /// <summary>
        /// 改签申请理由
        /// </summary>
        public List<string> ChangeReasons
        {
            get { return _changeReasons; }

            set
            {
                if (_changeReasons == value) return;

                RaisePropertyChanging(ChangeReasonsPropertyName);
                _changeReasons = value;
                RaisePropertyChanged(ChangeReasonsPropertyName);
            }
        }

        #endregion

        #region ModifyReasons 其他理由

        /// <summary>
        /// The <see cref="ModifyReasons" /> property's name.
        /// </summary>
        private const string ModifyReasonsPropertyName = "ModifyReasons";

        private List<string> _modifyReasons = new List<string>();

        /// <summary>
        /// 其他理由
        /// </summary>
        public List<string> ModifyReasons
        {
            get { return _modifyReasons; }

            set
            {
                if (_modifyReasons == value) return;

                RaisePropertyChanging(ModifyReasonsPropertyName);
                _modifyReasons = value;
                RaisePropertyChanged(ModifyReasonsPropertyName);
            }
        }

        #endregion

        #region ModifyReasons 多退少补理由

        /// <summary>
        /// The <see cref="BouncePriceDifferenceReasons" /> property's name.
        /// </summary>
        private const string BouncePriceDifferenceReasonsPropertyName = "BouncePriceDifferenceReasons";

        private ICollectionView _bouncePriceDifferenceReasons;

        /// <summary>
        /// 多退少补理由
        /// </summary>
        public ICollectionView BouncePriceDifferenceReasons
        {
            get { return _bouncePriceDifferenceReasons; }

            set
            {
                if (_bouncePriceDifferenceReasons == value) return;

                RaisePropertyChanging(BouncePriceDifferenceReasonsPropertyName);
                _bouncePriceDifferenceReasons = value;
                RaisePropertyChanged(BouncePriceDifferenceReasonsPropertyName);
            }
        }

        #endregion

        #region Order 订单对象

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

                Passenger.Clear();
                if (_order != null && _order.Passengers != null)
                    foreach (var item in _order.Passengers)
                    {
                        Passenger.Add(new PassengerModel
                        {
                            Id = item.Id,
                            IDNumer = item.CardNo,
                            Name = item.PassengerName,
                            PhoneNum = item.Mobile,       
                            IsInsuranceRefund = item.IsInsuranceRefund,
                            PassengerTripStatus = item.PassengerTripStatus
                        });
                    }

                ReissueSkyWays.Clear();
                if (_order != null && _order.SkyWays != null)
                    foreach (var item in _order.SkyWays)
                    {
                        ReissueSkyWays.Add(new ReissueSkyWayObject(item));
                    }

                //只有一个乘客默认选中
                if (Passenger.Count == 1)
                    Passenger[0].IsChecked = true;
                if (_order != null) HasRefund = _order.Policy.PolicySourceType == "接口";//退废申请接口订单显示极速退相关提示
                AddNewRow();
            }
        }

        #endregion

        #region Passenger 乘机人

        /// <summary>
        /// The <see cref="Passenger" /> property's name.
        /// </summary>
        private const string PassengerPropertyName = "Passenger";

        private ObservableCollection<PassengerModel> _passenger = new ObservableCollection<PassengerModel>();

        /// <summary>
        /// 乘机人
        /// </summary>
        public ObservableCollection<PassengerModel> Passenger
        {
            get { return _passenger; }

            set
            {
                if (_passenger == value) return;

                RaisePropertyChanging(PassengerPropertyName);
                _passenger = value;
                RaisePropertyChanged(PassengerPropertyName);
            }
        }

        #endregion

        #region RequestAnnulOrder 废票提交对象

        /// <summary>
        /// The <see cref="RequestAnnulOrder" /> property's name.
        /// </summary>
        private const string RequestAnnulOrderPropertyName = "RequestAnnulOrder";

        private RequestAnnulOrder _requestAnnulOrder = new RequestAnnulOrder();

        /// <summary>
        /// 废票
        /// </summary>
        public RequestAnnulOrder RequestAnnulOrder
        {
            get { return _requestAnnulOrder; }

            set
            {
                if (_requestAnnulOrder == value) return;

                RaisePropertyChanging(RequestAnnulOrderPropertyName);
                _requestAnnulOrder = value;
                RaisePropertyChanged(RequestAnnulOrderPropertyName);
            }
        }

        #endregion

        #region RequestBounceOrder 退票提交对象

        /// <summary>
        /// The <see cref="RequestBounceOrder" /> property's name.
        /// </summary>
        private const string RequestBounceOrderPropertyName = "RequestBounceOrder";

        private RequestBounceOrder _requestBounceOrder = new RequestBounceOrder();

        /// <summary>
        /// 退票
        /// </summary>
        public RequestBounceOrder RequestBounceOrder
        {
            get { return _requestBounceOrder; }

            set
            {
                if (_requestBounceOrder == value) return;

                RaisePropertyChanging(RequestBounceOrderPropertyName);
                _requestBounceOrder = value;
                RaisePropertyChanged(RequestBounceOrderPropertyName);
            }
        }

        #endregion

        #region RequestChangeOrder 改签提交对象

        /// <summary>
        /// The <see cref="RequestChangeOrder" /> property's name.
        /// </summary>
        private const string RequestChangeOrderPropertyName = "RequestChangeOrder";

        private RequestChangeOrder _requestChangeOrder = new RequestChangeOrder();

        /// <summary>
        /// 改签
        /// </summary>
        public RequestChangeOrder RequestChangeOrder
        {
            get { return _requestChangeOrder; }

            set
            {
                if (_requestChangeOrder == value) return;

                RaisePropertyChanging(RequestChangeOrderPropertyName);
                _requestChangeOrder = value;
                RaisePropertyChanged(RequestChangeOrderPropertyName);
            }
        }

        #endregion

        #region ApplyBabyOrder 婴儿提交对象

        /// <summary>
        /// The <see cref="ApplyBabyOrder" /> property's name.
        /// </summary>
        private const string ApplyBabyOrderPropertyName = "ApplyBabyOrder";

        private ApplyBabyDataObject _applyBabyDataObject = new ApplyBabyDataObject();

        /// <summary>
        /// 婴儿
        /// </summary>
        public ApplyBabyDataObject ApplyBabyOrder
        {
            get { return _applyBabyDataObject; }

            set
            {
                if (_applyBabyDataObject == value) return;

                RaisePropertyChanging(ApplyBabyOrderPropertyName);
                _applyBabyDataObject = value;
                RaisePropertyChanged(ApplyBabyOrderPropertyName);
            }
        }

        #endregion

        #region ApplyBabyOrder 婴儿信息

        private const string BabyDataObjectInformationItemsPropertyName = "BabyInformationItems";

        private ObservableCollection<BabyModel> _babyInformationItems = new ObservableCollection<BabyModel>();

        /// <summary>
        /// 婴儿信息
        /// </summary>
        public ObservableCollection<BabyModel> BabyInformationItemsItems
        {
            get { return _babyInformationItems; }

            set
            {
                if (_babyInformationItems == value) return;
                RaisePropertyChanging(BabyDataObjectInformationItemsPropertyName);
                _babyInformationItems = value;
                RaisePropertyChanged(BabyDataObjectInformationItemsPropertyName);
            }
        }
        #endregion

        #region RequestModifyOrder 其他提交对象

        #region RequestModifyOrder

        /// <summary>
        /// The <see cref="RequestModifyOrder" /> property's name.
        /// </summary>
        private const string RequestModifyOrderPropertyName = "RequestModifyOrder";

        private RequestModifyOrder _requestModifyOrder = new RequestModifyOrder();

        /// <summary>
        /// 其他提交对象
        /// </summary>
        public RequestModifyOrder RequestModifyOrder
        {
            get { return _requestModifyOrder; }

            set
            {
                if (_requestModifyOrder == value) return;

                RaisePropertyChanging(RequestModifyOrderPropertyName);
                _requestModifyOrder = value;
                RaisePropertyChanged(RequestModifyOrderPropertyName);
            }
        }

        #endregion

        #endregion

        #region IsBusy 是否在繁忙

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否在繁忙
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

        #region IsSubmitted 是否已经提交申请

        /// <summary>
        /// The <see cref="IsSubmitted" /> property's name.
        /// </summary>
        private const string IsSubmittedPropertyName = "IsSubmitted";

        private bool _isSubmitted;

        /// <summary>
        /// 是否已经提交
        /// </summary>
        public bool IsSubmitted
        {
            get { return _isSubmitted; }

            set
            {
                if (_isSubmitted == value) return;

                RaisePropertyChanging(IsSubmittedPropertyName);
                _isSubmitted = value;
                RaisePropertyChanged(IsSubmittedPropertyName);
            }
        }

        #endregion

        #region IsUploading 是否正在上传图片

        /// <summary>
        /// The <see cref="IsUploading" /> property's name.
        /// </summary>
        private const string IsUploadingPropertyName = "IsUploading";

        private bool _isUploading;

        /// <summary>
        /// 是否正在上传
        /// </summary>
        public bool IsUploading
        {
            get { return _isUploading; }

            set
            {
                if (_isUploading == value) return;

                RaisePropertyChanging(IsUploadingPropertyName);
                _isUploading = value;
                RaisePropertyChanged(IsUploadingPropertyName);
            }
        }

        #endregion

        #region HasFile 是否有上传文件

        /// <summary>
        /// The <see cref="HasFile" /> property's name.
        /// </summary>
        private const string HasFilePropertyName = "HasFile";

        private bool _hasFile;

        /// <summary>
        /// 是否有文件
        /// </summary>
        public bool HasFile
        {
            get { return _hasFile; }

            set
            {
                if (_hasFile == value) return;

                RaisePropertyChanging(HasFilePropertyName);
                _hasFile = value;
                RaisePropertyChanged(HasFilePropertyName);
            }
        }

        #endregion

        #region CurrentImageUri 当前上传的文件路径

        /// <summary>
        /// The <see cref="CurrentImageUri" /> property's name.
        /// </summary>
        private const string CurrentImageUriPropertyName = "CurrentImageUri";

        private string _currentImageUri;

        /// <summary>
        /// 当前模式附件地址
        /// </summary>
        public string CurrentImageUri
        {
            get { return _currentImageUri; }

            set
            {
                if (_currentImageUri == value) return;

                RaisePropertyChanging(CurrentImageUriPropertyName);
                _currentImageUri = value;
                RaisePropertyChanged(CurrentImageUriPropertyName);
                HasFile = value != null;
            }
        }

        #endregion

        #region HasRefund 是否显示极速退相关提示

        /// <summary>
        /// The <see cref="HasRefund" /> property's name.
        /// </summary>
        private const string HasRefundPropertyName = "HasRefund";

        private bool _hasRefund;

        /// <summary>
        /// 是否显示极速退提示
        /// </summary>
        public bool HasRefund
        {
            get { return _hasRefund; }

            set
            {
                if (_hasRefund == value) return;

                RaisePropertyChanging(HasRefundPropertyName);
                _hasRefund = value;
                RaisePropertyChanged(HasRefundPropertyName);
            }
        }

        #endregion

        #region AllSexTypes 所有性别信息

        /// <summary>
        /// The <see cref="AllSexTypes" /> property's name.
        /// </summary>
        private const string AllSexTypesPropertyName = "AllSexTypes";

        private ObservableCollection<KeyValuePair<EnumSexType?, String>> _allInsextypes = new ObservableCollection<KeyValuePair<EnumSexType?, string>>();

        /// <summary>
        /// 所有性别
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumSexType?, String>> AllSexTypes
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
        #endregion

        #region 公开命令

        #region SubmitCommand

        private RelayCommand _submitCommand;

        /// <summary>
        /// Gets the SubmitCommand.
        /// </summary>
        public RelayCommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new RelayCommand(ExecuteSubmitCommand, CanExecuteSubmitCommand));
            }
        }

        private void ExecuteSubmitCommand()
        {
            RequestAfterSaleOrder uploadModel = null;
            switch (_currentMode)
            {
                    //退票申请
                case RequestMode.BounceOrder:
                    if (Passenger.Where(p => p.IsChecked).Count(p => p.IsInsuranceRefund) > 0 &&
                        Passenger.Where(p => p.IsChecked).Count(p => p.IsInsuranceRefund == false) > 0)
                    {
                        UIManager.ShowMessage("不能同时选择未购和已购买极速退服务的乘客");
                        return;
                    }
                    if (Passenger.Any(p => p.IsChecked && p.PassengerTripStatus == EnumPassengerTripStatus.HasCreate))
                    {
                        UIManager.ShowMessage("选择的乘机人里有未作废的行程单不能进行退票申请");
                        return;
                    }
                    uploadModel = _requestBounceOrder;
                    if (_requestBounceOrder.AttachmentUrl != null &&
                        !_requestBounceOrder.AttachmentUrl.StartsWith("http"))
                    {
                        var result = UIManager.ShowMessageDialog("附件还没有上传，确认不上传附件？");
                        if (result == null || !result.Value)
                            return;
                        _requestBounceOrder.AttachmentUrl = null;
                    }
                    break;
                    //废票申请
                case RequestMode.AnnulOrder:
                    if (Passenger.Any(p => p.IsChecked && p.PassengerTripStatus == EnumPassengerTripStatus.HasCreate))
                    {
                        UIManager.ShowMessage("选择的乘机人里有未作废的行程单不能进行废票申请");
                        return;
                    }
                    uploadModel = _requestAnnulOrder;
                    if (_requestAnnulOrder.AttachmentUrl != null && !_requestAnnulOrder.AttachmentUrl.StartsWith("http"))
                    {
                        var result = UIManager.ShowMessageDialog("附件还没有上传，确认不上传附件？");
                        if (result == null || !result.Value)
                            return;
                        _requestAnnulOrder.AttachmentUrl = null;
                    }
                    break;
                    //改签申请
                case RequestMode.ChangeOrder:
                    uploadModel = _requestChangeOrder;
                    var isChanged = ReissueSkyWays.Any(m => m.IsChanged());
                    if (!isChanged)
                    {
                        UIManager.ShowMessage("航程信息并未修改");
                        return;
                    }
                    _requestChangeOrder.SkyWay = new List<RequestAfterSaleSkyWay>();
                    var isValidate = true;
                    foreach (var skywayModel in from item in _reissueSkyWays
                        where item.IsChanged()
                        select new RequestAfterSaleSkyWay
                        {
                            NewFlightNumber = item.FlightNumber,
                            NewSeat = item.Seat,
                            NewStartDateTime = item.StartDateTime,
                            SkyWayId = item.SkyWayId,
                            NewToDateTime = item.ToDateTime

                        })
                    {
                        if (skywayModel.NewStartDateTime.Year == 1)
                        {
                            isValidate = false;
                            break;
                        }
                        if (skywayModel.NewToDateTime.Year == 1)
                        {
                            isValidate = false;
                            break;
                        }
                        _requestChangeOrder.SkyWay.Add(skywayModel);
                    }
                    if (!isValidate)
                    {
                        UIManager.ShowMessage("输入改签日期格式有误，请重新输入");
                        return;
                    }
                    break;
                    //婴儿申请
                case RequestMode.ApplyBabyOrder:
                    ApplyBabyOrder.RelationOrderId = Order.OrderId;
                    ApplyBabyOrder.BabyList = new List<BabyDataObject>();
                    foreach (var model in BabyInformationItemsItems)
                    {
                        if (!model.CheckInput() || model.BornDate == null) return;
                            ApplyBabyOrder.BabyList.Add(new BabyDataObject
                            {
                                BabyName = model.BabyName,
                                BornDate = model.BornDate.Value.Date,
                                SexType = model.SexType
                            });
                    }
                    if (ApplyBabyOrder.BabyList.Count == 0) return;
                    IsBusy = true;
                    Action act = () => CommunicateManager.Invoke<IOrderService>(service =>
                    {
                        service.ApplyBaby(ApplyBabyOrder);
                        UIManager.ShowMessage("申请成功");
                        IsSubmitted = true;
                    }, UIManager.ShowErr);

                    Task.Factory.StartNew(act).ContinueWith(task =>
                    {
                        Action setBusyAction = () => { IsBusy = false; };
                        DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                    });
                    break;
                //其它申请
                case RequestMode.Modify:
                    uploadModel = _requestModifyOrder;
                    if (uploadModel != null && (String.IsNullOrEmpty(uploadModel.Description) || String.IsNullOrWhiteSpace(uploadModel.Description)))
                    {
                        UIManager.ShowMessage("请输入申请备注");
                        return;
                    }
                    break;
                default: return;
            }

            if (uploadModel != null && (String.IsNullOrEmpty(uploadModel.Reason) || String.IsNullOrWhiteSpace(uploadModel.Reason)))
            {
                UIManager.ShowMessage("请选择申请理由");
                return;
            }

            if (uploadModel == null) return;
            uploadModel.Passengers = GetPassengerIds();
            if (uploadModel.Passengers == null || uploadModel.Passengers.Length == 0)
            {
                UIManager.ShowMessage("请选择退改签乘机人");
                return;
            }

            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                service.ApplySaleOrder(Order.OrderId, uploadModel);
                UIManager.ShowMessage("申请成功");
                IsSubmitted = true;

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private int[] GetPassengerIds()
        {
            if (_passenger == null || _passenger.Count == 0)
                return null;

            var result = new List<int>();
            result.AddRange(_passenger.Where(m => m.IsChecked).Select(m => m.Id));
            return result.ToArray();
        }

        private bool CanExecuteSubmitCommand()
        {
            return !IsBusy && !_isUploading;
        }

        #endregion

        #region SelectModeCommand

        private RelayCommand<string> _selectModeCommand;

        /// <summary>
        /// 选择模式
        /// </summary>
        public RelayCommand<string> SelectModeCommand
        {
            get
            {
                return _selectModeCommand ?? (_selectModeCommand = new RelayCommand<string>(ExecuteSelectModeCommand, CanExecuteSelectModeCommand));
            }
        }

        /// <summary>
        /// 选择当前模式
        /// </summary>
        /// <param name="mode">The mode.</param>
        private void ExecuteSelectModeCommand(string mode)
        {
            switch (mode)
            {
                //退票申请
                case "RequestBounceOrder":
                    _currentMode = RequestMode.BounceOrder;
                    CurrentImageUri = _requestBounceOrder.AttachmentUrl;
                    break;
                //废票申请
                case "RequestAnnulOrder":
                    _currentMode = RequestMode.AnnulOrder;
                    CurrentImageUri = _requestAnnulOrder.AttachmentUrl;
                    break;
                //改签申请
                case "RequestChangeOrder":
                    _currentMode = RequestMode.ChangeOrder;
                    CurrentImageUri = null;
                    break;
                //婴儿申请
                case "ApplyBabyOrder":
                    _currentMode = RequestMode.ApplyBabyOrder;
                    _currentImageUri = null;
                    break;
                //其他理由
                case "Modify":
                    _currentMode = RequestMode.Modify;
                    CurrentImageUri = null;
                    break;
                default: return;
            }
        }

        private bool CanExecuteSelectModeCommand(string mode)
        {
            return true;
        }

        #endregion

        #region SelectFileCommand

        private RelayCommand _selectFileCommand;

        /// <summary>
        /// 选择文件命令
        /// </summary>
        public RelayCommand SelectFileCommand
        {
            get
            {
                return _selectFileCommand ?? (_selectFileCommand = new RelayCommand(ExecuteSelectFileCommand, CanExecuteSelectFileCommand));
            }
        }

        private void ExecuteSelectFileCommand()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "所有文件|*.png;*.jpg;*.jpeg|PNG文件|*.png|JPG文件|*.jpg;*.jpeg",
                Multiselect = false
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == null || !dialogResult.Value)
                return;

            if (openFileDialog.FileNames == null)
                return;

            var filePath = openFileDialog.FileNames[0];

            SetUri(filePath);
        }

        private bool CanExecuteSelectFileCommand()
        {
            return true;
        }

        #endregion

        #region UploadCommand

        private RelayCommand _uploadCommand;

        /// <summary>
        /// 上传命令
        /// </summary>
        public RelayCommand UploadCommand
        {
            get
            {
                return _uploadCommand ?? (_uploadCommand = new RelayCommand(ExecuteUploadCommand, CanExecuteUploadCommand));
            }
        }

        private void ExecuteUploadCommand()
        {
            IsUploading = true;
            Action action = () =>
            {
                var result = UploadHelper.UploadFile(_currentImageUri);
                if (result.Value != null)
                {
                    UIManager.ShowMessage(result.Value);
                    return;
                }

                SetUri(result.Key);
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                var setBusy = new Action(() => { IsUploading = false; });
                DispatcherHelper.UIDispatcher.Invoke(setBusy);
            });
        }

        private bool CanExecuteUploadCommand()
        {
            return !IsUploading && !String.IsNullOrEmpty(_currentImageUri);
        }

        #endregion

        #region OpenFileCommand

        private RelayCommand _openFileCommand;

        /// <summary>
        /// 打开文件命令
        /// </summary>
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand = new RelayCommand(ExecuteOpenFileCommand, CanExecuteOpenFileCommand));
            }
        }

        private void ExecuteOpenFileCommand()
        {
            if (CurrentImageUri == null || CurrentImageUri == null)
                return;

            try
            {
                //System.Diagnostics.Process.Start(CurrentImageUri);
                UIManager.OpenDefaultBrower(_currentImageUri);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.INFO, "打开文件失败", ex);
                UIManager.ShowErr(ex);
            }
        }

        private bool CanExecuteOpenFileCommand()
        {
            return true;
        }

        #endregion

        #region RemoveFileCommand

        private RelayCommand _removeFileCommand;

        /// <summary>
        /// 移除文件命令
        /// </summary>
        public RelayCommand RemoveFileCommand
        {
            get
            {
                return _removeFileCommand ?? (_removeFileCommand = new RelayCommand(ExecuteRemoveFileCommand, CanExecuteRemoveFileCommand));
            }
        }

        private void ExecuteRemoveFileCommand()
        {
            CurrentImageUri = null;
            switch (_currentMode)
            {
                case RequestMode.AnnulOrder:
                    _requestAnnulOrder.AttachmentUrl = null; break;
                case RequestMode.BounceOrder:
                    _requestBounceOrder.AttachmentUrl = null; break;
            }
        }

        private bool CanExecuteRemoveFileCommand()
        {
            return true;
        }

        #endregion

        #region AddCommand 添加

        private RelayCommand _addCommand;

        /// <summary>
        /// 添加
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(ExecuteAddCommandCommand, CanExecuteAddCommandCommand));
            }
        }

        private void ExecuteAddCommandCommand()
        {
            AddNewRow();
        }

        private bool CanExecuteAddCommandCommand()
        {
            return !IsBusy;
        }
        #endregion

        #region RemoveCommand 删除

        private RelayCommand<object> _removeCommand;

        /// <summary>
        /// 删除
        /// </summary>
        public RelayCommand<object> RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = new RelayCommand<object>(ExecuteRemoveCommandCommand, CanExecuteRemoveCommandCommand));
            }
        }

        private void ExecuteRemoveCommandCommand(object obj)
        {
            var data = obj as BabyModel;
            if (data == null) return;
            BabyInformationItemsItems.Remove(data);
            CanExecuteRemoveCommandCommand(data);
        }

        private bool CanExecuteRemoveCommandCommand(object obj)
        {
            return BabyInformationItemsItems.Count > 1;
        }
        #endregion
        #endregion

        #region 私有方法

        private void SetUri(string filePath)
        {
            switch (_currentMode)
            {
                //退票申请
                case RequestMode.BounceOrder:
                    RequestBounceOrder.AttachmentUrl = filePath;
                    break;
                //废票申请
                case RequestMode.AnnulOrder:
                    RequestAnnulOrder.AttachmentUrl = filePath;
                    break;
            }

            CurrentImageUri = filePath;
        }

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

        /// <summary>
        /// 婴儿信息添加新行
        /// </summary>
        private void AddNewRow()
        {
            var flightDateTime = DateTime.Now.Date;
            if (Order != null && Order.SkyWays != null) flightDateTime = Order.SkyWays.Min(p => p.StartDateTime);
            BabyInformationItemsItems.Add(new BabyModel { BabyName = string.Empty, SexType = EnumSexType.Male, BornDate = flightDateTime, DisplayDateEnd = flightDateTime, DisplayDateStart = flightDateTime.Date.AddYears(-2).AddDays(1)});
        }
    }

    /// <summary>
    /// 退改签航班信息
    /// </summary>
    public class ReissueSkyWayObject : ObservableObject
    {
        private SkyWayDto sourceModel;

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="ReissueSkyWayObject"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ReissueSkyWayObject(SkyWayDto item)
        {
            sourceModel = item;

            Recover();
        }

        #endregion

        /// <summary>
        /// 原航班编号
        /// </summary>
        public int SkyWayId { get; set; }

        #region FromCity

        /// <summary>
        /// The <see cref="FromCity" /> property's name.
        /// </summary>
        private const string FromCityPropertyName = "FromCity";

        private string _fromCity;

        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCity
        {
            get { return _fromCity; }

            set
            {
                if (_fromCity == value) return;

                RaisePropertyChanging(FromCityPropertyName);
                _fromCity = value;
                RaisePropertyChanged(FromCityPropertyName);
            }
        }

        #endregion

        #region ToCity

        /// <summary>
        /// The <see cref="ToCity" /> property's name.
        /// </summary>
        private const string ToCityPropertyName = "ToCity";

        private string _toCity;

        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCity
        {
            get { return _toCity; }

            set
            {
                if (_toCity == value) return;

                RaisePropertyChanging(ToCityPropertyName);
                _toCity = value;
                RaisePropertyChanged(ToCityPropertyName);
            }
        }

        #endregion

        #region StartDateTime

        /// <summary>
        /// The <see cref="StartDateTime" /> property's name.
        /// </summary>
        private const string StartDateTimePropertyName = "StartDateTime";

        private DateTime _startDateTime;

        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime
        {
            get { return _startDateTime; }

            set
            {
                if (value.Year == 1) return;
                if (_startDateTime == value) return;

                RaisePropertyChanging(StartDateTimePropertyName);
                _startDateTime = value;
                RaisePropertyChanged(StartDateTimePropertyName);
            }
        }

        #endregion

        #region ToDateTime

        /// <summary>
        /// The <see cref="ToDateTime" /> property's name.
        /// </summary>
        private const string ToDateTimePropertyName = "ToDateTime";

        private DateTime _toDateTime;

        /// <summary>
        /// 抵达时间
        /// </summary>
        public DateTime ToDateTime
        {
            get { return _toDateTime; }

            set
            {
                if (value.Year == 1) return;
                if (_toDateTime == value) return;

                RaisePropertyChanging(ToDateTimePropertyName);
                _toDateTime = value;
                RaisePropertyChanged(ToDateTimePropertyName);
            }
        }

        #endregion

        #region FlightNumber

        /// <summary>
        /// The <see cref="FlightNumber" /> property's name.
        /// </summary>
        private const string FlightNumberPropertyName = "FlightNumber";

        private string _flightNumber;

        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber
        {
            get { return _flightNumber; }

            set
            {
                if (_flightNumber == value) return;

                RaisePropertyChanging(FlightNumberPropertyName);
                _flightNumber = value;
                RaisePropertyChanged(FlightNumberPropertyName);
            }
        }

        #endregion

        #region Seat

        /// <summary>
        /// The <see cref="Seat" /> property's name.
        /// </summary>
        private const string SeatPropertyName = "Seat";

        private string _seat;

        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat
        {
            get { return _seat; }

            set
            {
                if (_seat == value) return;

                RaisePropertyChanging(SeatPropertyName);
                _seat = value;
                RaisePropertyChanged(SeatPropertyName);
            }
        }

        #endregion

        #region Discount

        /// <summary>
        /// The <see cref="Discount" /> property's name.
        /// </summary>
        private const string DiscountPropertyName = "Discount";

        private decimal _discount;

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount
        {
            get { return _discount; }

            set
            {
                if (_discount == value) return;

                RaisePropertyChanging(DiscountPropertyName);
                _discount = value;
                RaisePropertyChanged(DiscountPropertyName);
            }
        }

        #endregion

        #region Carrier

        /// <summary>
        /// The <see cref="Carrier" /> property's name.
        /// </summary>
        private const string CarrierPropertyName = "Carrier";

        private string _carrier;

        /// <summary>
        /// 承运人
        /// </summary>
        public string Carrier
        {
            get { return _carrier; }

            set
            {
                if (_carrier == value) return;

                RaisePropertyChanging(CarrierPropertyName);
                _carrier = value;
                RaisePropertyChanged(CarrierPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 恢复数据
        /// </summary>
        internal void Recover()
        {
            if (sourceModel == null)
            {
                Carrier = FlightNumber = FromCity = Seat = ToCity = null;
                return;
            }

            Carrier = sourceModel.CarrayCode;
            FlightNumber = sourceModel.FlightNumber;
            FromCity = sourceModel.FromCity;
            Seat = sourceModel.Seat;
            SkyWayId = sourceModel.SkyWayId;
            StartDateTime = sourceModel.StartDateTime;
            ToCity = sourceModel.ToCity;
            ToDateTime = sourceModel.ToDateTime;
        }

        /// <summary>
        /// 数据是否发生变化
        /// </summary>
        /// <returns></returns>
        internal bool IsChanged()
        {
            var isChanged = Carrier != sourceModel.CarrayCode ||
            FlightNumber != sourceModel.FlightNumber ||
            FromCity != sourceModel.FromCity ||
            Seat != sourceModel.Seat ||
            SkyWayId != sourceModel.SkyWayId ||
            StartDateTime != sourceModel.StartDateTime ||
            ToCity != sourceModel.ToCity ||
            ToDateTime != sourceModel.ToDateTime;

            return isChanged;
        }
    }
}
