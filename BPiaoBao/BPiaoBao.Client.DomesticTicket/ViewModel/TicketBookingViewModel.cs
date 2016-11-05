using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using BPiaoBao.Client.DomesticTicket.Model;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
using ProjectHelper.Utils;
using EnumPassengerType = BPiaoBao.Common.Enums.EnumPassengerType;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.Client.UIExt.Model;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class TicketBookingViewModel : BaseVM
    {

        /// <summary>
        ///记录新增加常旅客的行索引
        /// </summary>
        private int _passerIndex;
        /// <summary>
        /// 关联成人订单号
        /// </summary>
        public string OldOrderId = string.Empty;
        /// <summary>
        /// 选择常旅客后常旅客信息临时集合
        /// </summary>
        private List<FrePasserDto> _frePassers = new List<FrePasserDto>();
        public List<FrePasserDto> FrePassers
        {
            get { return _frePassers; }
            set
            {
                if (_frePassers == value) return;
                RaisePropertyChanging("FrePassers");
                _frePassers = value;
                //选择乘客信息后去掉第一行空数据信息
                if (_frePassers.Count == 0) return;
                var passer = PassengerInformationItems[0].Passer;
                if (passer == null)
                {
                    PassengerInformationItems.RemoveAt(0);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(passer.Name) &&
                        string.IsNullOrWhiteSpace(PassengerInformationItems[0].Telephone) &&
                        string.IsNullOrWhiteSpace(PassengerInformationItems[0].BusinessCard))
                        PassengerInformationItems.RemoveAt(0);
                }
                //循环添加到集合
                foreach (var item in _frePassers)
                {
                    var data = new PassengerInformationViewModel(Items.First().StartDateTime)
                    {
                        AgeType = AgeTypeConvertor(item.PasserType),
                        IdType = IDTypeConvertor(item.CertificateType)
                    };

                    #region 出生日期
                    switch (data.AgeType)
                    {
                        //case AgeType.Child:
                        //    if (data.IDType == EnumIDType.NormalId)
                        //    {
                        //        if (item.CertificateNo.Length == 18)
                    //        {
                        //            #region IDtoBirthday
                        //            string CardID = item.CertificateNo;
                        //            string year = CardID.Substring(6, 4);
                        //            string month = CardID.Substring(10, 2);
                        //            string date = CardID.Substring(12, 2);
                        //            string result = year + "-" + month + "-" + date;
                        //            #endregion
                        //            DateTime birth;
                        //            if (DateTime.TryParse(result, out birth)) data.Birthday = birth;
                    //        }
                        //    }
                        //    else if (data.IDType == EnumIDType.BirthDate)
                    //        data.Birthday = Convert.ToDateTime(item.CertificateNo);
                        //    break;
                        //case AgeType.Baby:
                        //    data.Birthday = Convert.ToDateTime(item.CertificateNo);
                        //    break;
                        //case AgeType.All:
                        //case AgeType.Adult:
                        default:
                    data.Birthday = item.Birth;
                            break;
                    } 
                    #endregion
                    #region 性别
                    switch (item.SexType)
                    {
                        case "男":
                            data.SexType = EnumSexType.Male;
                            break;
                        case "女":
                            data.SexType = EnumSexType.Female;
                            break;
                        case "未知":
                            data.SexType = EnumSexType.UnKnown;
                            break;
                    }
                    #endregion
                    data.BusinessCard = item.AirCardNo;
                    data.Name = item.Name;
                    data.Id = item.CertificateNo;
                    data.Telephone = item.Mobile;
                    data.PasserIndex = _passerIndex;
                    data.Passer = new PasserModel { Name = item.Name, Index = _passerIndex };

                    //祛除重复选择项
                    if (_passengerInformationItems.FirstOrDefault(p => p.Id == item.CertificateNo && p.Telephone == item.Mobile && p.Passer != null && p.Passer.Name == item.Name) == null)
                        _passengerInformationItems.Add(data);
                    else
                        _passengerInformationItems.Remove(data);

                    _passerIndex++;
                }

                RaisePropertyChanged("FrePassers");
            }
        }
        /// <summary>
        /// 添加到常旅客信息临时集合
        /// </summary>
        private List<PassengerInformationViewModel> _list = new List<PassengerInformationViewModel>();

        private static readonly List<KeyValuePair<dynamic, string>> CAgeTypeItems;
        private FlightInfoModel[] _flightItems;

        #region 静态变量
        public const string CCloseWindow = "820AB95A-2CE1-4B04-AE9A-EDD18BD30ECD";
        public const string CSearchPnr = "89B27274-E2D2-49CC-84B5-80CD3212502C";
        #endregion

        #region 构造函数
        static TicketBookingViewModel()
        {
            CAgeTypeItems = EnumHelper.GetEnumKeyValuesPassger(typeof(AgeType));

        }

        public TicketBookingViewModel(FlightInfoModel[] flights,Site site = null)
        {

            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Male, UIExt.Helper.EnumHelper.GetDescription(EnumSexType.Male)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Female, UIExt.Helper.EnumHelper.GetDescription(EnumSexType.Female)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.UnKnown,UIExt.Helper.EnumHelper.GetDescription(EnumSexType.UnKnown)));
            _flightItems = flights;

            CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                CurrentUserInfo = service.GetCurrentUserInfo();
            }, UIManager.ShowErr);

            if (CurrentUserInfo == null)
            {
                CurrentUserInfo = new CurrentUserInfoDto();
            }

            Items = new ObservableCollection<FlightInformationItemViewModel>();
            PassengerInformationItems = new ObservableCollection<PassengerInformationViewModel>();
            AgeTypeItems = CollectionViewSource.GetDefaultView(CAgeTypeItems);

            //AgeTypeItems.Filter = (o) =>
            //{
            //    if (o is KeyValuePair<dynamic, string>)
            //    {
            //        var item = (KeyValuePair<dynamic, string>)o;
            //        if (IsAdult)
            //        {
            //            return item.Key != AgeType.Child;
            //        }
            //        else
            //        {
            //            return item.Key == AgeType.Child;
            //        }
            //    }
            //    return false;
            //};

            Reset();

            foreach (var flight in flights)
            {
                if (site != null) flight.DefaultSite = site;
                Items.Add(new FlightInformationItemViewModel(flight));
            }

            AddNew();
        }

        #endregion

        #region 公开属性
        #region Items

        private const string CItemsPropertyName = "Items";

        private ObservableCollection<FlightInformationItemViewModel> _items;

        /// <summary>
        /// Items
        /// </summary>
        public ObservableCollection<FlightInformationItemViewModel> Items
        {
            get { return _items; }

            set
            {
                if (_items == value) return;
                RaisePropertyChanging(CItemsPropertyName);
                _items = value;
                RaisePropertyChanged(CItemsPropertyName);
            }
        }
        #endregion

        #region 乘客信息

        private const string CPassengerInformationItemsPropertyName = "PassengerInformationItems";

        private ObservableCollection<PassengerInformationViewModel> _passengerInformationItems = new ObservableCollection<PassengerInformationViewModel>();

        /// <summary>
        /// 乘客信息
        /// </summary>

        [DisplayName(@"乘客信息")]
        public ObservableCollection<PassengerInformationViewModel> PassengerInformationItems
        {
            get { return _passengerInformationItems; }

            set
            {
                if (_passengerInformationItems == value) return;
                RaisePropertyChanging(CPassengerInformationItemsPropertyName);
                _passengerInformationItems = value;
                RaisePropertyChanged(CPassengerInformationItemsPropertyName);
            }
        }
        #endregion

        #region CurrentItem

        private const string CCurrentItemPropertyName = "CurrentItem";

        private PassengerInformationViewModel _currentItem;

        /// <summary>
        /// CurrentItem
        /// </summary>

        [DisplayName(@"CurrentItem")]
        public PassengerInformationViewModel CurrentItem
        {
            get { return _currentItem; }

            set
            {
                if (_currentItem == value) return;
                RaisePropertyChanging(CCurrentItemPropertyName);
                _currentItem = value;
                RaisePropertyChanged(CCurrentItemPropertyName);
            }
        }
        #endregion

        #region 是否转换

        private const string CIsTransformPropertyName = "IsTransform";

        private bool _isTransform;

        /// <summary>
        /// 是否转换
        /// </summary>

        [DisplayName(@"是否转换")]
        public bool IsTransform
        {
            get { return _isTransform; }

            set
            {
                if (_isTransform == value) return;
                RaisePropertyChanging(CIsTransformPropertyName);
                _isTransform = value;
                RaisePropertyChanged(CIsTransformPropertyName);
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
        /// 是否在繁忙
        /// </summary>
        public virtual bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        #endregion

        #region PnrType
        public PnrType PnrType
        {
            get { return _isAdult ? PnrType.Adult : PnrType.Child; }
        }
        #endregion

        #region IsAdult

        private const string CIsAdultPropertyName = "IsAdult";

        private bool _isAdult = true;

        /// <summary>
        /// IsAdult
        /// </summary>        
        [DisplayName(@"IsAdult")]
        public bool IsAdult
        {
            get { return _isAdult; }

            set
            {
                if (_isAdult == value) return;
                RaisePropertyChanging(CIsAdultPropertyName);
                _isAdult = value;
                Reset();
                RaisePropertyChanged(CIsAdultPropertyName);
            }
        }
        #endregion

        #region IsUsualPassenger 是否添加到常旅客

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        private const string IsUsualPassengerPropertyName = "IsUsualPassenger";

        private bool _isUsualPassenger;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public virtual bool IsUsualPassenger
        {
            get { return _isUsualPassenger; }

            set
            {
                if (_isUsualPassenger == value) return;

                RaisePropertyChanging(IsUsualPassengerPropertyName);
                _isUsualPassenger = value;
                RaisePropertyChanged(IsUsualPassengerPropertyName);
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

        #region SelectedSexType

        ///// <summary>
        ///// The <see cref="SelectedSexType" /> property's name.
        ///// </summary>
        //public const string SelectedSexTypePropertyName = "SelectedSexType";

        //private EnumSexType? selectedSexType = EnumSexType.Male;

        ///// <summary>
        ///// 选中的性别
        ///// </summary>
        //public EnumSexType? SelectedSexType
        //{
        //    get { return selectedSexType; }

        //    set
        //    {
        //        if (selectedSexType == value) return;

        //        RaisePropertyChanging(SelectedSexTypePropertyName);
        //        selectedSexType = value;
        //        RaisePropertyChanged(SelectedSexTypePropertyName);
        //    }
        //}

        #endregion

        #region IsLowPrice
        /// <summary>
        /// The <see cref="IsLowPrice" /> property's name.
        /// </summary>
        private const string IsLowPricePropertyName = "IsLowPrice";
        private bool _isLowPrice = true; //默认选中
        /// <summary>
        /// 使用低价格
        /// </summary>
        public bool IsLowPrice
        {
            get { return _isLowPrice; }

            set
            {
                if (_isLowPrice == value) return;
                RaisePropertyChanging(IsLowPricePropertyName);
                _isLowPrice = value;
                RaisePropertyChanged(IsLowPricePropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令
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
            AddNew();
        }

        private bool CanExecuteAddCommandCommand()
        {
            return !IsBusy;
        }
        #endregion

        #region RemoveCommand 删除

        private RelayCommand<object> _removeCommand;
        //private SynchronizationContext _SynchronizationContext;

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
            var data = obj as PassengerInformationViewModel;
            if (data == null) return;
            _passengerInformationItems.Remove(data);
            CanExecuteRemoveCommandCommand(data);
        }

        private bool CanExecuteRemoveCommandCommand(object obj)
        {
            return _passengerInformationItems.Count > 1;
        }
        #endregion

        #region OkCommand 生产定单

        private RelayCommand _okCommand;
        //private FlightInfoModel[] flights;
        private Task _activeTask;

        /// <summary>
        /// 生产定单
        /// </summary>
        public RelayCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand(ExecuteOkCommandCommand, CanExecuteOkCommandCommand));
            }
        }

        private void ExecuteOkCommandCommand()
        {
            try
            {
                Check();
                //乘机人姓名生僻字判断
                var isstop = false;
                DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IFlightDestineService>(service =>
                {
                    foreach (var item in _passengerInformationItems.Select(item => service.HasRare(item.Name)).Where(rback => rback.IsRare))
                    {
                        isstop = item.IsRare;
                        throw new Exception("乘机人姓名里含有生僻字(" + item.RareFontString + ")，请用拼音代替");
                    }
                }, UIManager.ShowErr)));
                if (isstop) return;
            }
            catch (Exception ex)
            {
                UIManager.ShowErr(ex);
                return;
            }
            IsBusy = true;
            OkCommand.RaiseCanExecuteChanged();
            //Func<string> action = () =>
            Func<PolicyPack> action = () =>
            {
                //string v = null;
                var pp = new PolicyPack();
                var special = false;
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var skyWay = Items.Select(p => new DestineSkyWayRequest
                    {
                        CarrayCode = p.FlightInfoModel.CarrayCode,
                        FlightNumber = p.FlightInfoModel.FlightNumber,
                        FromCityCode = p.FlightInfoModel.FromCityCode,
                        Seat = p.FlightInfoModel.DefaultSite.SeatCode,
                        StartDate = p.FlightInfoModel.StartDateTime,
                        ToCityCode = p.FlightInfoModel.ToCityCode,
                        EndDate = p.FlightInfoModel.ToDateTime,
                        FromTerminal = p.FromTerminal,
                        ToTerminal = p.ToTerminal,
                        FlightModel = p.FlightInfoModel.Model,
                        Discount = p.Discount,
                        SeatPrice = p.IbeSeatPrice,
                        TaxFee = p.IbeTaxFee,
                        RQFee = p.IbeRQFee,
                        //SeatPrice = p.SeatPrice,
                        //TaxFee = p.TaxFee,
                        //RQFee = p.RQFee,.
                        PolicySpecialType = p.PolicySpecialType,
                        SpecialPrice = p.SeatPrice,
                        SpecialTax = p.TaxFee,
                        SpecialFuelFee = p.RQFee,
                        SpecialYPrice = p.YPrice
                    }).ToArray();
                    //单程,往返，联程特价标示判断
                    special = skyWay.Any(p => p.PolicySpecialType != EnumPolicySpecialType.Normal);

                    var passengers = _passengerInformationItems.Select(p =>
                    {
                        p.Id = p.IdType == EnumIDType.BirthDate && p.Birthday != null ? p.Birthday.Value.ToString("yyyy-MM-dd") : p.Id;
                        return new PassengerRequest
                        {
                            CardNo = p.Id ?? "",
                            ChdBirthday = p.AgeType != AgeType.Adult && p.Birthday != null ? p.Birthday.Value.Date : DateTime.Now.Date.AddYears(-12),
                            MemberCard = p.BusinessCard ?? "",
                            PassengerName = p.Passer.Name.Trim(),
                            PassengerType = (int)(EnumPassengerType)p.AgeType,
                            LinkPhone = p.AgeType == AgeType.Adult && p.Telephone != null ? p.Telephone.Trim() : string.Empty,
                            IdType = p.IdType,
                            SexType = p.SexType,
                            Birth = p.Birthday
                        };
                    }).ToArray();

                    try
                    {
                        //v = service.Destine(new DestineRequest()
                        //             {
                        //                 Passengers = Passengers,
                        //                 PnrType = (int)PnrType,
                        //                 SkyWay = SkyWay,
                        //                 Tel = CurrentUserInfo.Tel.Trim(),
                        //             }).TrimEx();
                        pp = service.Destine(new DestineRequest
                        {
                            Passengers = passengers,
                            SkyWay = skyWay,
                            Tel = CurrentUserInfo.Tel.Trim(),
                            //关联成人订单号(只有单独儿童时关联)
                            OldOrderId = passengers.Count(p => p.PassengerType == 2) > 0 ? OldOrderId : string.Empty,
                            IsChangePnr = IsTransform,
                            IsLowPrice = IsLowPrice,
                            PolicySpecialType = special ? skyWay[0].PolicySpecialType : EnumPolicySpecialType.Normal,
                            SpecialFuelFee = special ? skyWay[0].SpecialFuelFee : skyWay[0].RQFee,
                            SpecialPrice = special ? skyWay[0].SpecialPrice : skyWay[0].SeatPrice,
                            SpecialTax = special ? skyWay[0].SpecialTax : skyWay[0].TaxFee,
                            SpecialYPrice = skyWay[0].SpecialYPrice,
                            IbeRQFee = skyWay[0].RQFee,
                            IbeTaxFee = skyWay[0].TaxFee,
                            IbeSeatPrice = skyWay[0].SeatPrice
                        }, EnumDestineSource.WhiteScreenDestine);
                        if (!pp.INFPnrIsSame) UIManager.ShowMessage("成人编码中婴儿项丢失，请补全婴儿信息");
                        //政策列表显示
                        LocalUIManager.ShowPolicyList(pp, null);
                    }
                    catch (Exception ex)
                    {
                        UIManager.ShowErr(ex);
                        return;
                    }
                    DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                    {

                        //执行常旅客的添加操作

                        var fpdList = (from item in _list
                            let f = new FrePasserDto
                            {
                                AirCardNo = item.BusinessCard ?? "", 
                                CertificateNo = item.Id ?? "",
                                Mobile = item.Telephone ?? "",
                                Name = item.Name ?? "", 
                                CertificateType = UIExt.Helper.EnumHelper.GetDescription(item.IdType), 
                                PasserType = UIExt.Helper.EnumHelper.GetDescription(item.AgeType), 
                                SexType = UIExt.Helper.EnumHelper.GetDescription(item.SexType),
                                Birth = item.Birthday
                            }
                            where PassengerInformationItems.Contains(item)
                            select f).ToList();
                        Action action2 = () =>
                        {
                            IsBusy = true;

                            CommunicateManager.Invoke<IFrePasserService>(service2 =>
                            {
                                if (fpdList.Count > 0) service2.Import(fpdList);
                            }, UIManager.ShowErr);
                        };

                        Task.Factory.StartNew(action2).ContinueWith(task =>
                        {
                            Action setBusyAction = () => { IsBusy = false; };
                            DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                        });


                        //发送关闭查看返程窗口消息 
                        Messenger.Default.Send(true, TicketBookingBackViewModel.c_CloseTicketBookingBackWindow);
                        //发送关闭预定机票窗口消息 
                        Messenger.Default.Send(true, CCloseWindow);
                      if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
                        {
                            var uc = PluginManager.FindItem(Main.ProjectCode, Main.PnrControlCode);
                            if (uc != null)
                            {
                                var control = uc.GetContent() as Control;
                                this.FullWidowExt.SetContent(control);
                            }
                        }
                        else
                        {
                            PluginService.Run(Main.ProjectCode, Main.PnrControlCode);
                        }  
                        
                        //Messenger.Default.Send<string>(v, cSearchPNR);

                    }));
                    Debug.WriteLine("");
                },  UIManager.ShowErr);
                //return v;
                if (!string.IsNullOrWhiteSpace(OldOrderId))
                    pp.OrderId = pp.ChdOrderId;
                return pp;
            };

            try
            {
                if (_activeTask != null)
                {
                    _activeTask.Wait();
                }
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message,exx);
            }

            //_activeTask = Task.Factory.StartNew<string>(action)
            _activeTask = Task.Factory.StartNew(action)
               .ContinueWith(task =>
               {
                   DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                   {
                       IsBusy = false;
                       OkCommand.RaiseCanExecuteChanged();
                   }
                   ));
               });

        }

        private void Check()
        {
            //手机号，证件号重复check
            for (int i = 0; i < _passengerInformationItems.Count; i++)
            {
                var p = _passengerInformationItems[i];
                p.Check();
                //如果乘机人手机号不空则验证格式
                if (!string.IsNullOrEmpty(p.Telephone)) Guard.CheckMobilePhoneNum(p.Telephone,p.Name);
                for (int j = i + 1; j < _passengerInformationItems.Count; j++)
                {
                    var next = _passengerInformationItems[j];
                    //乘机人相同电话号码判断
                    if (!string.IsNullOrWhiteSpace(p.Telephone) && !string.IsNullOrWhiteSpace(next.Telephone) && p.Telephone == next.Telephone)
                    {
                        throw new Exception(string.Format("手机号码不能重复：{0}", p.Telephone));
                    }
                    //成人时相同ID判断
                    if (p.Id == next.Id && p.AgeType == AgeType.Adult && next.AgeType == AgeType.Adult)
                    {
                        throw new Exception(string.Format("证件号不能重复：{0}", p.Id));
                    }
                    //乘机人姓名相同提示处理
                    if (p.Name == next.Name)
                        throw new Exception("订单中包含姓名相同的乘机人");
                }
            }
            //一个成人只能带2个婴儿，儿童和婴儿不能一起预订，
            if (IsAdult)
            {
                //成人数目
                var countAdult = _passengerInformationItems.Count(p => p.AgeType == AgeType.Adult);
                //婴儿数目
                var countBaby = _passengerInformationItems.Count(p => p.AgeType == AgeType.Baby);
                //儿童数目
                var countChild = _passengerInformationItems.Count(p => p.AgeType == AgeType.Child);
                //特价政策暂不支持儿童
                //if (Items.Any(p => p.FlightInfoModel.DefaultSite.PolicySpecialType != EnumPolicySpecialType.Normal))
                //{
                //    if (countChild > 0) throw new Exception("特价政策暂不支持儿童");
                //}

                //成人和婴儿预订时的判断
                if (countAdult > 0 && countBaby > 0)
                {
                    if (countBaby > countAdult)
                        throw new Exception("一个成人最多只能带1个婴儿");
                }
                //成人和儿童预订时的判断
                if (countAdult > 0 && countChild > 0)
                {
                    if (countChild > 2 * countAdult)
                        throw new Exception("一个成人最多只能带2个儿童");
                }
                //婴儿和儿童预订时的判断
                if (countChild > 0 && countBaby > 0 && countAdult == 0)
                {
                    throw new Exception("婴儿和儿童不能一起预定");
                }
                //多个儿童预定时的判断
                if (countChild > 0 && countAdult == 0 && countBaby == 0)
                {
                    if (countChild > 2)
                        throw new Exception("只能有2个单独儿童预定");
                    LocalUIManager.ShowRelationOrderNoWindow(this);
                    if (string.IsNullOrWhiteSpace(OldOrderId))
                        throw new Exception("未填写关联成人订单号");
                }
                //多个婴儿预定时的判断
                if (countBaby > 0 && countAdult == 0 && countChild == 0)
                {
                    throw new Exception("不能单独预订婴儿票");
                }

            }
            //乘机人总人数判断
            if (_passengerInformationItems.Count > 9)
                throw new Exception("乘机人总数不能超过9人");
            if (_passengerInformationItems.Count(p => !string.IsNullOrWhiteSpace(p.Telephone)) == 0)
                throw new Exception("所有乘机人中至少有一个手机号码");
            Guard.CheckMobilePhoneNum(CurrentUserInfo.Tel, "联系人");
        }

        private bool CanExecuteOkCommandCommand()
        {
            return !IsBusy;
        }
        #endregion

        #region OpenUsualPassengersCommand 选择常旅客信息

        private RelayCommand _openUsualPassengersCommand;

        /// <summary>
        /// 打开常用乘客信息窗口命令
        /// </summary>
        public RelayCommand OpenUsualPassengersCommand
        {
            get
            {
                return _openUsualPassengersCommand ?? (_openUsualPassengersCommand = new RelayCommand(ExecuteOpenUsualPassengersCommand, CanExecuteOpenUsualPassengersCommand));
            }
        }

        private void ExecuteOpenUsualPassengersCommand()
        {
            LocalUIManager.ShowUsualPassengers(this);
        }

        private bool CanExecuteOpenUsualPassengersCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region SelectCommand 选中常旅客

        private RelayCommand<PassengerInformationViewModel> _selectCommand;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand<PassengerInformationViewModel> SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand<PassengerInformationViewModel>(ExecuteSelectCommand, CanExecuteSelectCommand));
            }
        }

        private void ExecuteSelectCommand(PassengerInformationViewModel model)
        {
            _list.Add(model);
        }

        private bool CanExecuteSelectCommand(PassengerInformationViewModel model)
        {
            return !IsBusy;
        }
        #endregion

        #region UnSelectCommand 取消选中常旅客

        private RelayCommand<PassengerInformationViewModel> _unselectCommand;

        /// <summary>
        /// 执行取消选择命令
        /// </summary>
        public RelayCommand<PassengerInformationViewModel> UnSelectCommand
        {
            get
            {
                return _unselectCommand ?? (_unselectCommand = new RelayCommand<PassengerInformationViewModel>(ExecuteUnSelectCommand, CanExecuteUnSelectCommand));
            }
        }

        private void ExecuteUnSelectCommand(PassengerInformationViewModel model)
        {
            _list.Remove(model);
        }

        private bool CanExecuteUnSelectCommand(PassengerInformationViewModel model)
        {
            return !IsBusy;
        }
        #endregion

        #region SelectedCommand 模糊查询后事件

        private RelayCommand<PasserModel> _selectedCommand;

        /// <summary>
        /// 模糊查询后事件
        /// </summary>
        public RelayCommand<PasserModel> SelectedCommand
        {
            get
            {
                return _selectedCommand ?? (_selectedCommand = new RelayCommand<PasserModel>(ExecuteSelectedCommand));
            }
        }

        private void ExecuteSelectedCommand(PasserModel model)
        {
            if (model == null || model.Index <= -1) return;
            var data =
                _passengerInformationItems.Where(p => p.PasserIndex == model.Index)
                    .ToList()
                    .FirstOrDefault();
            if (data == null) return;
            data.Name = model.Name; 
            if (data.Passer == null)
            {
                data.Passer = new PasserModel { Name = model.Name, Index = model.Index, isCusCreate = true };
            }
            else if (data.Passer != null && !data.Passer.isCusCreate)
            {
                data.Passer.AirCardNo = model.AirCardNo;
                data.Passer.CertificateNo = model.CertificateNo;
                data.Passer.CertificateType = model.CertificateType;
                data.Passer.Mobile = model.Mobile;
                data.Passer.Name = model.Name;
                data.Passer.PasserType = model.PasserType;
                data.Passer.SexType = model.SexType;
                data.Passer.Birth = model.Birth;
                data.Id = model.CertificateNo;
                data.AgeType = AgeTypeConvertor(model.PasserType);
                data.IdType = IDTypeConvertor(model.CertificateType);
                data.Telephone = model.Mobile;
                data.BusinessCard = model.AirCardNo;
                data.SexType = data.Passer.SexType;
                data.Birthday = data.Passer.Birth;
            }
        }

        #endregion
        #endregion

        private void Reset()
        {
            AgeTypeItems.Refresh();

            //var type = IsAdult ? AgeType.Adult : AgeType.Child;
            //_PassengerInformationItems.ForEach(p => p.AgeType = type);
        }

        /// <summary>
        /// 添加新行
        /// </summary>
        /// <returns></returns>
        private void AddNew()
        {
            var data = new PassengerInformationViewModel(Items.First().StartDateTime)
            {
                Passer = new PasserModel {Index = _passerIndex},
                IdType = EnumIDType.NormalId,
                SexType = EnumSexType.UnKnown,
                PasserIndex = _passerIndex
            };
            _passengerInformationItems.Add(data);
            _passerIndex++;
        }
        public ICollectionView AgeTypeItems { get; set; }
        //public ICollectionView IDTypeItems { get; set; }
        public CurrentUserInfoDto CurrentUserInfo { get; set; }


        /// <summary>
        /// 乘客类型枚举转换
        /// </summary>
        /// <param name="ageTypeString"></param>
        /// <returns></returns>
        private AgeType AgeTypeConvertor(string ageTypeString)
        {
            var ageType = AgeType.Adult;
            switch (ageTypeString)
            {
                case "成人":
                    ageType = AgeType.Adult;
                    break;
                case "儿童":
                    ageType = AgeType.Child;
                    break;
                case "婴儿":
                    ageType = AgeType.Baby;
                    break;
            }

            return ageType;
        }

        /// <summary>
        /// 身份证枚举转换
        /// </summary>
        /// <param name="idTypeString"></param>
        /// <returns></returns>
        private EnumIDType IDTypeConvertor(string idTypeString)
        {
            var idType = EnumIDType.NormalId;
            switch (idTypeString)
            {
                case "身份证":
                    idType = EnumIDType.NormalId;
                    break;
                case "护照":
                    idType = EnumIDType.Passport;
                    break;
                case "军官证":
                    idType = EnumIDType.MilitaryId;
                    break;
                case "出生日期":
                    idType = EnumIDType.BirthDate;
                    break;
                case "其它有效证件":
                    idType = EnumIDType.Other;
                    break;
            }
            return idType;
        }
    }
}
