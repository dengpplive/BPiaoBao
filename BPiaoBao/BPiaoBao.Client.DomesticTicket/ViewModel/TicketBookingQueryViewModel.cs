using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 机票预订查询结果视图
    /// </summary>
    public class TicketBookingQueryViewModel : BaseVM
    {
        #region
        public const string ToPnrViewMessageCommission = "ToPnrViewMessage_Commission";
        public const string ToChoosePolicyViewMessageCommission = "ToChoosePolicyViewMessage_Commission";
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public TicketBookingQueryViewModel()
        {

            Initialize();
            //_timer = new Timer((o) =>
            //{
            //    _timer.Change(Timeout.Infinite, Timeout.Infinite);
            //    DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            //    {
            //        ExecuteWeekCheckedCommnad();
            //    }));

            //});
            Messenger.Default.Register<Visibility>(this, PNRViewModel.ToTicketBookingQueryViewMessageCommission, p =>
            {
                IsShowCommissionColumn = p;
            });

        }
        #endregion

        #region 初始数据
        /// <summary>
        /// 初始数据
        /// </summary>
        public override void Initialize()
        {
            if (IsInDesignMode)
                return;
            ListView = null;
            if (CanExecuteQueryCommand())
            {
                QueryFlightInfoResult();
            }
            ExecuteShowWeekPanelCommand();
            ExecuteSwitchFlightTypeCommand(FlightType.ToString());

        }

        #endregion

        #region 公开属性


        public const string ListProtertyName = "List";//航班信息
        public const string FlightDataRowsProtertyName = "FlightDataRows";

        public const string IsShowWeekPanelProtertyName = "IsShowWeekPanel";
        public const string IsShowConGridProtertyName = "IsShowConGrid";

        private bool _isShowWeekPanel = true;
        private bool _isShowConGrid;


        ////////////////////////////////////////////// 
        public const string FlightTipInfoProtertyName = "FlightTipInfo";//航班提示信息
        public const string FlightTypePropertyName = "FlightType";//航程类型
        public const string FromCityPropertyName = "FromCity"; //出发城市
        public const string ToCityPropertyName = "ToCity"; //到达城市
        public const string TakeDateProtertyName = "TakeDate"; //出发时间
        public const string BackDateProtertyName = "BackDate"; //返回时间 
        public const string CarrayProtertyName = "Carray"; //承运人 
        public const string SelectedCarrayProtertyName = "SelectedCarray"; //承运人列表
        public const string MidCityProtertyName = "MidCity";//中转城市 
        public const string MidDateProtertyName = "MidDate";//中转日期 

        public const string IsShowOneProtertyName = "IsShowOne";//单程
        public const string IsShowTwoProtertyName = "IsShowTwo";//往返
        public const string IsShowConnWayProtertyName = "IsShowConnWay";//联程

        //  public const string SelectedDateTimeProterty = "SelectedDateTime";//选中的当前日期

        public const string IsAllDayProtertyName = "IsAllDay";
        public const string IsSwDayProtertyName = "IsSwDay";
        public const string IsXwDayProtertyName = "IsXwDay";
        public const string IsWsDayProtertyName = "IsWsDay";

        public const string IsShowCommissionColumnProtertyName = "IsShowCommissionColumn";
        public const string IsShareProtertyName = "IsShare";
        private List<FlightInfoModel> _list;

        private List<FlightInfoModel[]> _flightDataRows;

        private string _flightTipInfo;
        private FlightTypeEnum _flightType = FlightTypeEnum.SingleWay;//默认为单程
        private CityNewModel _fromCity;
        private CityNewModel _toCity;
        private DateTime _takeDate = DateTime.Now.Date;
        private DateTime _backDate = DateTime.Now.Date;
        private List<CarrayModel> _carray;
        private CarrayModel _selecttedCarray;
        private CityNewModel _midCity;
        private DateTime _midDate = DateTime.Now;

        private bool _isShowOne = true;
        private bool _isShowTwo;
        private bool _isShowConnWay;

        private bool _isAllDay = true;
        private bool _isSwDay;
        private bool _isXwDay;
        private bool _isWsDay;

        private Visibility _isShowCommissionColumn = Visibility.Visible;
        private bool _isShare;
        #region 视图

        private ICollectionView _listView;
        public const string CListViewPropertyName = "ListView";
        /// <summary>
        /// 视图
        /// </summary>        
        [DisplayName(@"视图")]
        public ICollectionView ListView
        {
            get { return _listView; }

            set
            {
                if (_listView == value) return;

                RaisePropertyChanging(CListViewPropertyName);
                _listView = value;
                RaisePropertyChanged(CListViewPropertyName);
            }
        }
        #endregion

        public List<FlightInfoModel> List
        {
            get { return _list; }
            set
            {
                if (_list == value)
                {
                    return;
                }

                if (value == null)
                {
                    ListView = null;
                }
                else
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                    {
                        ListView = CollectionViewSource.GetDefaultView(value);
                        ListView.SortDescriptions.Add(new SortDescription(FlightInfoModel.StartDateTimeProtertyName, ListSortDirection.Ascending));
                    }));
                }

                RaisePropertyChanging(ListProtertyName);
                _list = value;
                RaisePropertyChanged(ListProtertyName);
            }
        }

        public List<FlightInfoModel[]> FlightDataRows
        {
            get { return _flightDataRows; }
            set
            {
                if (_flightDataRows == value)
                {
                    return;
                }
                RaisePropertyChanging(FlightDataRowsProtertyName);
                _flightDataRows = value;
                RaisePropertyChanged(FlightDataRowsProtertyName);
            }
        }

        public string FlightTipInfo
        {
            get { return _flightTipInfo; }
            set
            {
                if (_flightTipInfo == value)
                {
                    return;
                }
                RaisePropertyChanging(FlightTipInfoProtertyName);
                _flightTipInfo = value;
                RaisePropertyChanged(FlightTipInfoProtertyName);

            }
        }

        public FlightTypeEnum FlightType
        {
            get { return _flightType; }
            set
            {
                if (_flightType == value)
                {
                    return;
                }
                RaisePropertyChanging(FlightTypePropertyName);
                _flightType = value;
                RaisePropertyChanged(FlightTypePropertyName);
            }
        }


        public CityNewModel FromCity
        {
            get { return _fromCity; }
            set
            {
                //if (_fromCity == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(FromCityPropertyName);
                _fromCity = value;
                RaisePropertyChanged(FromCityPropertyName);
            }
        }

        public CityNewModel ToCity
        {
            get { return _toCity; }
            set
            {
                //if (_toCity == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(ToCityPropertyName);
                _toCity = value;
                RaisePropertyChanged(ToCityPropertyName);
            }
        }

        public DateTime TakeDate
        {
            get { return _takeDate; }
            set
            {
                //if (_takeDate == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(TakeDateProtertyName);
                _takeDate = value;
                RaisePropertyChanged(TakeDateProtertyName);
                ViewModelLocator.TicketBookingManager.TakeDate = value;
            }
        }

        #region DefaultTakeDate

        /// <summary>
        /// DefaultTakeDate
        /// </summary>        
        [DisplayName(@"DefaultTakeDate")]
        public DateTime DefaultTakeDate
        {
            get { return _takeDate; }

        }

        private void OnDefaultTakeDateChanged()
        {
            const string cDefaultTakeDatePropertyName = "DefaultTakeDate";
            RaisePropertyChanged(cDefaultTakeDatePropertyName);
        }
        #endregion


        public DateTime BackDate
        {
            get { return _backDate; }
            set
            {
                //if (_backDate == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(BackDateProtertyName);
                _backDate = value;
                RaisePropertyChanged(BackDateProtertyName);
            }
        }

        public CarrayModel SelectedCarray
        {
            get { return _selecttedCarray; }
            set
            {
                //if (_selecttedCarray == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(SelectedCarrayProtertyName);
                _selecttedCarray = value;
                RaisePropertyChanged(SelectedCarrayProtertyName);
            }
        }
        public List<CarrayModel> Carray
        {
            get { return _carray; }
            set
            {
                if (_carray == value)
                {
                    return;
                }
                RaisePropertyChanging(CarrayProtertyName);
                _carray = value;
                RaisePropertyChanged(CarrayProtertyName);
            }
        }


        public CityNewModel MidCity
        {
            get { return _midCity; }
            set
            {
                //if (_midCity == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(MidCityProtertyName);
                _midCity = value;
                RaisePropertyChanged(MidCityProtertyName);
            }
        }
        public DateTime MidDate
        {
            get { return _midDate; }
            set
            {
                //if (_midDate == value)
                //{
                //    return;
                //}
                RaisePropertyChanging(MidDateProtertyName);
                _midDate = value;
                RaisePropertyChanged(MidDateProtertyName);
            }
        }

        public bool IsShowOne
        {
            get { return _isShowOne; }
            set
            {
                if (_isShowOne == value)
                {
                    return;
                }

                RaisePropertyChanging(IsShowOneProtertyName);
                _isShowOne = value;
                RaisePropertyChanged(IsShowOneProtertyName);

            }
        }

        public bool IsShowTwo
        {
            get { return _isShowTwo; }
            set
            {
                if (_isShowTwo == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowTwoProtertyName);
                _isShowTwo = value;
                RaisePropertyChanged(IsShowTwoProtertyName);
            }
        }

        public bool IsShowConnWay
        {
            get { return _isShowConnWay; }
            set
            {
                if (_isShowConnWay == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowConnWayProtertyName);
                _isShowConnWay = value;
                RaisePropertyChanged(IsShowConnWayProtertyName);
            }
        }

        public bool IsShowWeekPanel
        {
            get { return _isShowWeekPanel; }
            set
            {
                if (_isShowWeekPanel == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowWeekPanelProtertyName);
                _isShowWeekPanel = value;
                RaisePropertyChanged(IsShowWeekPanelProtertyName);
            }
        }

        public bool IsShowConGrid
        {
            get { return _isShowConGrid; }
            set
            {
                if (_isShowConGrid == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowConGridProtertyName);
                _isShowConGrid = value;
                RaisePropertyChanged(IsShowConGridProtertyName);
            }
        }

        public bool IsAllDay
        {
            get { return _isAllDay; }
            set
            {
                if (_isAllDay == value)
                {
                    return;
                }
                RaisePropertyChanging(IsAllDayProtertyName);
                _isAllDay = value;
                RaisePropertyChanged(IsAllDayProtertyName);
            }
        }

        public bool IsSwDay
        {
            get { return _isSwDay; }
            set
            {
                if (_isSwDay == value)
                {
                    return;
                }
                RaisePropertyChanging(IsSwDayProtertyName);
                _isSwDay = value;
                RaisePropertyChanged(IsSwDayProtertyName);
            }
        }

        public bool IsXwDay
        {
            get { return _isXwDay; }
            set
            {
                if (_isXwDay == value)
                {
                    return;
                }
                RaisePropertyChanging(IsXwDayProtertyName);
                _isXwDay = value;
                RaisePropertyChanged(IsXwDayProtertyName);
            }
        }

        public bool IsWsDay
        {
            get { return _isWsDay; }
            set
            {
                if (_isWsDay == value)
                {
                    return;
                }
                RaisePropertyChanging(IsWsDayProtertyName);
                _isWsDay = value;
                RaisePropertyChanged(IsWsDayProtertyName);
            }
        }

        /// <summary>
        /// 是否显示佣金列
        /// </summary>
        public Visibility IsShowCommissionColumn
        {
            get { return _isShowCommissionColumn; }
            set
            {
                if (_isShowCommissionColumn == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowCommissionColumnProtertyName);
                _isShowCommissionColumn = value;
                RaisePropertyChanged(IsShowCommissionColumnProtertyName);

            }
        }
        /// <summary>
        /// 是否显示共享航班
        /// </summary>
        public bool IsShare
        {
            get { return _isShare; }
            set
            {
                if (_isShare == value)
                {
                    return;
                }

                RaisePropertyChanging(IsShareProtertyName);
                _isShare = value;
                RaisePropertyChanged(IsShareProtertyName);

            }
        }

        //private bool _isLoading;
        ///// <summary>
        ///// 正在Loading
        ///// </summary>
        //public bool IsLoading
        //{
        //    get { return _isLoading; }
        //    set
        //    {
        //        if (_isLoading == value) return;
        //        RaisePropertyChanging("IsLoading");
        //        _isLoading = value;
        //        RaisePropertyChanged("IsLoading");
        //    }
        //}

        #endregion

        #region 公开命令

        #region ShowWeekPanelCommand

        private RelayCommand _showWeekPanelCommand;//显示周导航

        public RelayCommand ShowWeekPanelCommand
        {
            get
            {
                return
                    _showWeekPanelCommand ?? (_showWeekPanelCommand =
                        new RelayCommand(ExecuteShowWeekPanelCommand, CanExecuteShowWeekPanelCommand));
            }
        }

        private void ExecuteShowWeekPanelCommand()
        {
            IsShowWeekPanel = FlightType == FlightTypeEnum.SingleWay;

            IsShowConGrid = false;
            OnDefaultTakeDateChanged();
        }

        private bool CanExecuteShowWeekPanelCommand()
        {
            return !IsBusy;
        }
        #endregion

        #region ShowConditionPanelCommand

        private RelayCommand _showConditionPanelCommand;//显示搜索条件

        public RelayCommand ShowConditionPanelCommand
        {
            get
            {
                return _showConditionPanelCommand ?? (_showConditionPanelCommand = new RelayCommand(ExecuteShowConditionPanelCommand, CanExecuteShowConditionPanelCommand));
            }
        }

        private void ExecuteShowConditionPanelCommand()
        {


            IsShowWeekPanel = false;
            IsShowConGrid = true;
            switch (FlightType)
            {
                case FlightTypeEnum.SingleWay:
                    IsShowOne = true;
                    IsShowTwo = false;
                    IsShowConnWay = false;
                    break;
                case FlightTypeEnum.DoubleWay:
                    IsShowOne = false;
                    IsShowTwo = true;
                    IsShowConnWay = false;
                    break;
                case FlightTypeEnum.ConnWay:
                    IsShowOne = false;
                    IsShowTwo = false;
                    IsShowConnWay = true;
                    break;
            }


        }

        private bool CanExecuteShowConditionPanelCommand()
        {
            return !IsBusy;
        }
        #endregion

        #region SwitchFlightTypeCommand

        private RelayCommand<string> _switchFlightTypeCommand;

        public RelayCommand<string> SwitchFlightTypeCommand
        {
            get
            {
                return _switchFlightTypeCommand ??
                       (_switchFlightTypeCommand = new RelayCommand<string>(ExecuteSwitchFlightTypeCommand, CanExecuteSwitchFlightTypeCommand));
            }
        }

        private FlightTypeEnum _tempFlightType;
        private void ExecuteSwitchFlightTypeCommand(string fType)
        {

            var type = EnumHelper.GetInstance<FlightTypeEnum>(fType);
            switch (type)
            {

                case FlightTypeEnum.SingleWay:
                    IsShowOne = true;
                    IsShowTwo = false;
                    IsShowConnWay = false;
                    break;
                case FlightTypeEnum.DoubleWay:
                    IsShowOne = false;
                    IsShowTwo = true;
                    IsShowConnWay = false;
                    break;
                case FlightTypeEnum.ConnWay:
                    IsShowOne = false;
                    IsShowTwo = false;
                    IsShowConnWay = true;
                    MidDate = TakeDate;
                    break;

            }
            // FlightType = type;
            _tempFlightType = type;
            // QueryFlightInfoResult();

        }

        private bool CanExecuteSwitchFlightTypeCommand(string fType)
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(fType);
        }

        #endregion

        #region SwitchCityCommand

        private RelayCommand _switchCityCommand;

        /// <summary>
        /// 切换城市命令
        /// </summary>
        public RelayCommand SwitchCityCommand
        {
            get
            {
                return _switchCityCommand ?? (_switchCityCommand = new RelayCommand(ExecuteSwitchCityCommand, CanExecuteSwitchCityCommand));
            }
        }

        private void ExecuteSwitchCityCommand()
        {
            var temp = FromCity;
            FromCity = ToCity;
            ToCity = temp;
        }

        private bool CanExecuteSwitchCityCommand()
        {
            return !IsBusy;
        }
        #endregion

        #region QueryCommand

        private RelayCommand _queryCommand;

        public RelayCommand QueryCommand
        {
            get
            {
                return _queryCommand ?? (_queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        private void ExecuteQueryCommand()
        {
            ListView = null;
            FlightType = _tempFlightType;
            QueryFlightInfoResult();
        }
        private bool CanExecuteQueryCommand()
        {
            return !IsBusy && FromCity != null;
        }
        #endregion

        #region BookingCommand

        private RelayCommand<FlightInfoModel> _bookingCommand;

        public RelayCommand<FlightInfoModel> BookingCommand
        {
            get
            {
                return _bookingCommand ?? (_bookingCommand = new RelayCommand<FlightInfoModel>(ExecuteBookingCommand, CanExecuteBookingCommand));

            }
        }

        private void ExecuteBookingCommand(FlightInfoModel flight)
        {
            LocalUIManager.ShowTicketBooking(new[] { flight }, null, null);
        }

        private bool CanExecuteBookingCommand(FlightInfoModel flight)
        {
            return !IsBusy && flight != null;
        }
        #endregion

        #region QueryTicketBackCommand

        private RelayCommand<FlightInfoModel> _queryTicketBackCommand;

        public RelayCommand<FlightInfoModel> QueryTicketBackCommand
        {
            get { return _queryTicketBackCommand ?? (new RelayCommand<FlightInfoModel>(ExecuteQueryTicketBackCommand, CanExecuteQueryTicketBackCommand)); }
        }

        private void ExecuteQueryTicketBackCommand(FlightInfoModel flightInfoModel)
        {
            var backRows = FlightDataRows[1];
            switch (FlightType)
            {
                case FlightTypeEnum.DoubleWay:
                    {
                        //往返
                        var backArray = backRows.Where(p => p.CarrayCode == flightInfoModel.CarrayCode).ToArray();
                        LocalUIManager.ShowTicketBookingBack(flightInfoModel, backArray, IsShowCommissionColumn, null);
                    }
                    break;
                case FlightTypeEnum.ConnWay:
                    {
                        //联程
                        var backArray = backRows.Where(p => p.StartDateTime > flightInfoModel.ToDateTime && p.CarrayCode == flightInfoModel.CarrayCode).ToArray();
                        LocalUIManager.ShowTicketBookingBack(flightInfoModel, backArray, IsShowCommissionColumn, null);
                    }
                    break;
            }
        }

        private bool CanExecuteQueryTicketBackCommand(FlightInfoModel flightInfoModel)
        {
            return !IsBusy && flightInfoModel != null;
        }
        #endregion

        #region BookingSubCommand

        private RelayCommand<Site> _bookingSubCommand;

        public RelayCommand<Site> BookingSubCommand
        {
            get { return _bookingSubCommand ?? (new RelayCommand<Site>(ExecuteBookingSubCommand, CanExecuteBookingSubCommand)); }
        }

        private void ExecuteBookingSubCommand(Site site)
        {
            FlightInfoModel model = List.FirstOrDefault(m => m.CusNo == site.CusNo);
            if (model == null) return;
            if (FlightTypeEnum.SingleWay == _queryFlightTypeCache)
            {
                LocalUIManager.ShowTicketBooking(new[] { model }, site, null);
                model.DefaultSite = model.SiteList.Any(p => p.PolicySpecialType != EnumPolicySpecialType.Normal) ? model.SiteList.First(p => p.PolicySpecialType != EnumPolicySpecialType.Normal) : model.SiteList.First();
            }
            else
            {
                var backRows = FlightDataRows[1];
                switch (_queryFlightTypeCache)
                {
                    case FlightTypeEnum.DoubleWay:
                        {
                            //往返
                            var backArray = backRows.Where(p => p.CarrayCode == model.CarrayCode).ToArray();
                            model.DefaultSite = site;
                            LocalUIManager.ShowTicketBookingBack(model, backArray, IsShowCommissionColumn, null);
                            model.DefaultSite = model.SiteList.First();
                        }
                        break;
                    case FlightTypeEnum.ConnWay:
                        {
                            //联程
                            var backArray = backRows.Where(p => (p.StartDateTime > model.ToDateTime && p.CarrayCode == model.CarrayCode)).ToArray();
                            model.DefaultSite = site;
                            LocalUIManager.ShowTicketBookingBack(model, backArray, IsShowCommissionColumn, null);
                            model.DefaultSite = model.SiteList.First();
                        }
                        break;
                }
            }
        }

        private bool CanExecuteBookingSubCommand(Site site)
        {
            return !IsBusy && site != null;
        }
        #endregion

        #region WeekCheckedCommnad

        private RelayCommand _weekCheckedCommnad;

        public RelayCommand WeekCheckedCommnad
        {
            get { return _weekCheckedCommnad ?? (new RelayCommand(ExecuteWeekCheckedCommnad, CanExecuteWeekCheckedCommnad)); }
        }

        private void ExecuteWeekCheckedCommnad()
        {
            ListView = null;
            //BackDate = TakeDate;
            MidDate = TakeDate;
            IsShowOne = true;
            IsShowTwo = false;
            IsShowConGrid = false;
            QueryFlightInfoResult();

        }

        private bool CanExecuteWeekCheckedCommnad()
        {
            return !IsBusy;
        }

        //private System.Threading.Timer _timer = null;

        //public void DelayExecuteWeekChecked()
        //{
        //    _timer.Change(300, 300);
        //}

        #endregion

        #region SelectFlyTimeCommand

        private RelayCommand<string> _selectFlyTimeCommand;

        public RelayCommand<string> SelectFlyTimeCommand
        {
            get { return _selectFlyTimeCommand ?? (new RelayCommand<string>(ExecuteSelectFlyTimeCommand, CanExecuteSelectFlyTimeCommand)); }

        }

        private void ExecuteSelectFlyTimeCommand(string chk)
        {


            var time = TakeDate.ToString("yyyy-MM-dd");

            DateTime? t1 = null;
            DateTime? t2 = null;

            var temp = chk.Split('-');
            var ck = temp[0];
            var isChecked = temp[1] == "1";


            if (ck == "1" && isChecked)
            {
                IsSwDay = false;
                IsXwDay = false;
                IsSwDay = false;
            }

            if ((ck == "2" && isChecked) || (ck == "3" && isChecked) || (ck == "4" && isChecked))
            {
                IsAllDay = false;
                switch (ck)
                {
                    case "2":
                        IsSwDay = true;
                        break;
                    case "3":
                        IsXwDay = true;
                        break;
                    case "4":
                        IsWsDay = true;
                        break;
                }
            }
            else if ((ck == "2" && !isChecked) || (ck == "3" && !isChecked) || (ck == "4" && !isChecked))
            {
                switch (ck)
                {
                    case "2":
                        IsSwDay = false;
                        break;
                    case "3":
                        IsXwDay = false;
                        break;
                    case "4":
                        IsWsDay = false;
                        break;
                }
            }


            if (!IsAllDay)
            {
                if (IsSwDay && !IsXwDay && !IsWsDay)
                {//上午
                    t1 = Convert.ToDateTime(time + " 01:00");
                    t2 = Convert.ToDateTime(time + " 12:59");
                }
                else if (!IsSwDay && IsXwDay && !IsWsDay)
                {//下午
                    t1 = Convert.ToDateTime(time + " 13:00");
                    t2 = Convert.ToDateTime(time + " 17:59");
                }
                else if (!IsSwDay && !IsXwDay && IsWsDay)
                {//晚上
                    t1 = Convert.ToDateTime(time + " 18:00");
                    t2 = Convert.ToDateTime(time + " 23:59");
                }
                //if (IsSwDay && IsXwDay && IsWsDay)
                //{
                //    t1 = Convert.ToDateTime(time + " 08:00");
                //    t2 = Convert.ToDateTime(time + " 23:59");
                //}
                //else if (IsSwDay && IsXwDay && !IsWsDay)
                //{
                //    t1 = Convert.ToDateTime(time + " 08:00");
                //    t2 = Convert.ToDateTime(time + " 18:00");
                //}
                //else if (IsSwDay && !IsXwDay && !IsWsDay)
                //{
                //    t1 = Convert.ToDateTime(time + " 08:00");
                //    t2 = Convert.ToDateTime(time + " 12:00");
                //}
                //else if (!IsSwDay && IsXwDay && IsWsDay)
                //{
                //    t1 = Convert.ToDateTime(time + " 12:00");
                //    t2 = Convert.ToDateTime(time + " 23:59");
                //}
                //else if (!IsSwDay && IsXwDay && !IsWsDay)
                //{
                //    t1 = Convert.ToDateTime(time + " 12:00");
                //    t2 = Convert.ToDateTime(time + " 18:00");
                //}
                //else if (!IsSwDay && !IsXwDay && IsWsDay)
                //{
                //    t1 = Convert.ToDateTime(time + " 18:00");
                //    t2 = Convert.ToDateTime(time + " 23:59");
                //}
            }


            if (List == null || _FlightTempAllInfos == null)
            {
                return;
            }

            if (t1 == null)
            {

                List = _FlightTempAllInfos[0].ToList();
                if (FlightType != FlightTypeEnum.SingleWay && _FlightTempAllInfos.Count > 1)
                {
                    FlightDataRows[1] = _FlightTempAllInfos[1];
                }
            }
            else
            {
                List = _FlightTempAllInfos[0].Where(p => p.StartDateTime <= t2 && p.StartDateTime >= t1).ToList();
                if (FlightType != FlightTypeEnum.SingleWay && _FlightTempAllInfos.Count > 1)
                {

                    FlightDataRows[1] =
                        _FlightTempAllInfos[1].Where(p => p.StartDateTime <= t2 && p.StartDateTime >= t1).ToArray();

                }
            }
            //DispatcherHelper.UIDispatcher.Invoke(new Action(() => ListView.SortDescriptions.Add(new SortDescription(FlightInfoModel.StartDateTimeProtertyName, ListSortDirection.Ascending))));
            Messenger.Default.Send(true, "ticketbookingquerysort");
        }

        private bool CanExecuteSelectFlyTimeCommand(string chk)
        {
            return !isBusy && !string.IsNullOrWhiteSpace(chk);
        }
        #endregion

        #region QueryStopTextRemarkCommand

        private RelayCommand<FlightInfoModel> _queryStopTextRemarkCommand;

        public RelayCommand<FlightInfoModel> QueryStopTextRemarkCommand
        {
            get
            {
                return _queryStopTextRemarkCommand ?? (new RelayCommand<FlightInfoModel>(ExecuteQueryStopTextRemarkCommand, CanExecuteQueryStopTextRemarkCommand));
            }
        }

        private void ExecuteQueryStopTextRemarkCommand(FlightInfoModel model)
        {
            //查询航班经停止信息

            Action action = () => CommunicateManager.Invoke<IFlightDestineService>(service =>
            {
                var m = service.GetLegStop(LoginInfo.Code, model.CarrayCode + model.FlightNumber, model.StartDateTime);

                var leg = new LegStopModel
                {
                    FlightCodeNumber = m.CarrayCodeFlightNum,
                    StartDate = m.StopDate + " " + m.FromTime,
                    ToDate = m.StopDate + " " + m.ToTime,
                    MiddleDate = m.StopDate + " " + m.MiddleTime1,
                    FromCity = m.FromCity,
                    ToCity = m.ToCity,
                    MiddleCity = m.MiddleCity
                };

                model.LegStopModel = leg;



            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteQueryStopTextRemarkCommand(FlightInfoModel model)
        {
            return model.LegStopModel == null;
        }
        #endregion

        #region GetSpecialFromSiteCommand

        private RelayCommand<Site> _getSpecialFromSiteCommand;

        public RelayCommand<Site> GetSpecialFromSiteCommand
        {
            get { return _getSpecialFromSiteCommand ?? (new RelayCommand<Site>(ExecuteGetSpecialFromSiteCommand, CanExecuteGetSpecialFromSiteCommand)); }
        }

        private void ExecuteGetSpecialFromSiteCommand(Site site)
        {
            var model = List.FirstOrDefault(m => m.CusNo == site.CusNo);
            if (model == null) return;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IPidService>(service =>
            {
                var pricelist = service.sendSSPat(LoginInfo.Code, model.CarrayCode, model.FlightNumber, site.SeatCode,
                    TakeDate.ToString("yyyy-MM-dd"), model.FromCityCode, model.ToCityCode, model.StartDateTime.ToString("HH:mm"),
                    model.ToDateTime.ToString("HH:mm"));
                var sitelist = new List<Site>();
                if (!pricelist.Any())
                {
                    if (site == model.DefaultSite)
                    {
                        model.DefaultSite.IsGotSpecial = true;
                        model.DefaultSite.IsReceivedSpecial = false;
                        if (model.IsOpened) model.IsOpened = false;//ui恢复
                        RefreshUi();//不做刷新ui处理；否则下拉框会收缩造成ui缺陷；
                    }
                    foreach (var item in model.SiteList)
                    {
                        if (item == site)
                        {
                            item.IsGotSpecial = true;
                            item.IsReceivedSpecial = false;
                        }
                        sitelist.Add(item);
                    }
                    model.SiteList = sitelist;
                    model.IsSiteListChanged = true;
                }
                else
                {
                    var info = pricelist.FirstOrDefault();
                    if (info == null) return;
                    decimal taxFee;
                    decimal rqFee;
                    decimal seatPrice;
                    decimal.TryParse(info.TAX, out taxFee);
                    decimal.TryParse(info.RQFare, out rqFee);
                    decimal.TryParse(info.Fare, out seatPrice);
                    foreach (var item in model.SiteList)
                    {
                        if (item == site)
                        {
                            item.TaxFee = taxFee;
                            item.RQFee = rqFee;
                            item.SeatPrice = seatPrice;
                            item.TicketPrice = taxFee + rqFee + seatPrice;
                            item.Commission = Math.Floor(site.PolicyPoint / 100 * site.SeatPrice);
                            item.Discount = item.SeatPrice > 0 && item.SpecialYPrice > 0 ? Convert.ToInt32((item.SeatPrice / item.SpecialYPrice) * 100) : item.Discount;
                            item.IsGotSpecial = true;
                            item.IsReceivedSpecial = true;
                        }
                        sitelist.Add(item);
                    }
                    model.SiteList = sitelist;
                    model.IsSiteListChanged = true;

                    if (site == model.DefaultSite)
                    {
                        model.TaxFee = model.DefaultSite.TaxFee = taxFee;
                        model.RQFee = model.DefaultSite.RQFee = rqFee;
                        model.DefaultSite.SeatPrice = seatPrice;
                        model.DefaultSite.TicketPrice = taxFee + rqFee + seatPrice;
                        model.DefaultSite.Commission = Math.Floor(model.DefaultSite.PolicyPoint / 100 * model.DefaultSite.SeatPrice);
                        model.DefaultSite.Discount = model.DefaultSite.SeatPrice > 0 && model.DefaultSite.SpecialYPrice > 0 ? Convert.ToInt32((model.DefaultSite.SeatPrice / model.DefaultSite.SpecialYPrice) * 100) : model.DefaultSite.Discount;
                        model.DefaultSite.IsGotSpecial = true;
                        model.DefaultSite.IsReceivedSpecial = true;
                        if (model.IsOpened) model.IsOpened = false;//ui恢复
                        RefreshUi();//不做刷新ui处理；否则下拉框会收缩造成ui缺陷；
                    }
                }
            }, UIManager.ShowErr)));
        }
        private bool CanExecuteGetSpecialFromSiteCommand(Site site)
        {
            return !IsBusy && site != null;
        }
        #endregion

        #region GetSpecialFromModelCommand

        private RelayCommand<FlightInfoModel> _getSpecialFromModelCommand;

        public RelayCommand<FlightInfoModel> GetSpecialFromModelCommand
        {
            get { return _getSpecialFromModelCommand ?? (new RelayCommand<FlightInfoModel>(ExecuteGetSpecialFromModelCommand, CanExecuteGetSpecialFromModelCommand)); }
        }

        private void ExecuteGetSpecialFromModelCommand(FlightInfoModel model)
        {
            if (model == null) return;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IPidService>(service =>
            {
                var pricelist = service.sendSSPat(LoginInfo.Code, model.CarrayCode, model.FlightNumber, model.DefaultSite.SeatCode,
                    TakeDate.ToString("yyyy-MM-dd"), model.FromCityCode, model.ToCityCode, model.StartDateTime.ToString("HH:mm"),
                    model.ToDateTime.ToString("HH:mm"));
                if (!pricelist.Any())
                {
                    foreach (var m in model.SiteList.Where(m => m.CusNo == model.DefaultSite.CusNo))
                    {
                        m.IsGotSpecial = true;
                        m.IsReceivedSpecial = false;
                        break;
                    }
                    model.DefaultSite.IsGotSpecial = true;
                    model.DefaultSite.IsReceivedSpecial = false;
                }
                else
                {
                    var info = pricelist.FirstOrDefault();
                    if (info == null) return;
                    decimal taxFee;
                    decimal rqFee;
                    decimal seatPrice;
                    decimal.TryParse(info.TAX, out taxFee);
                    decimal.TryParse(info.RQFare, out rqFee);
                    decimal.TryParse(info.Fare, out seatPrice);
                    foreach (var m in model.SiteList.Where(m => m.CusNo == model.DefaultSite.CusNo))
                    {
                        m.TaxFee = taxFee;
                        m.RQFee = rqFee;
                        m.SeatPrice = seatPrice;
                        m.TicketPrice = taxFee + rqFee + seatPrice;
                        m.Commission = Math.Floor(m.PolicyPoint / 100 * m.SeatPrice);
                        m.Discount = m.SeatPrice > 0 && m.SpecialYPrice > 0 ? Convert.ToInt32((m.SeatPrice / m.SpecialYPrice) * 100) : m.Discount;
                        m.IsGotSpecial = true;
                        m.IsReceivedSpecial = true;
                        break;
                    }
                    model.TaxFee = model.DefaultSite.TaxFee = taxFee;
                    model.RQFee = model.DefaultSite.RQFee = rqFee;
                    model.DefaultSite.SeatPrice = seatPrice;
                    model.DefaultSite.TicketPrice = taxFee + rqFee + seatPrice;
                    model.DefaultSite.Commission = Math.Floor(model.DefaultSite.PolicyPoint / 100 * model.DefaultSite.SeatPrice);
                    model.DefaultSite.Discount = model.DefaultSite.SeatPrice > 0 && model.DefaultSite.SpecialYPrice > 0 ? Convert.ToInt32((model.DefaultSite.SeatPrice / model.DefaultSite.SpecialYPrice) * 100) : model.DefaultSite.Discount;
                    model.DefaultSite.IsGotSpecial = true;
                    model.DefaultSite.IsReceivedSpecial = true;
                }
                RefreshUi();
            }, UIManager.ShowErr)));
        }

        private bool CanExecuteGetSpecialFromModelCommand(FlightInfoModel model)
        {
            return !IsBusy && model != null;
        }

        #endregion

        #region LeaveCommand

        private RelayCommand<FlightInfoModel> _leaveCommand;

        /// <summary>
        /// 执行鼠标离开popup命令
        /// </summary>
        public RelayCommand<FlightInfoModel> LeaveCommand
        {
            get
            {
                return _leaveCommand ?? (_leaveCommand = new RelayCommand<FlightInfoModel>(ExecuteLeaveCommand));
            }
        }

        private void ExecuteLeaveCommand(FlightInfoModel model)
        {
            if (model == null) return;
            if (model.IsSiteListChanged) model.IsSiteListChanged = false;
        }
        #endregion
        #endregion

        #region 私有方法

        /// <summary>
        /// 数据变更时刷新ui
        /// </summary>
        private void RefreshUi()
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                ListView = CollectionViewSource.GetDefaultView(List);
                ListView.SortDescriptions.Add(new SortDescription(FlightInfoModel.StartDateTimeProtertyName, ListSortDirection.Ascending));
            }));
        }

        private void QueryFlightInfoResult()
        {
            if (!ValidateParams())
            {
                return;
            }
            IsAllDay = true;
            IsSwDay = false;
            IsXwDay = false;
            IsWsDay = false;

            _queryFlightTypeCache = FlightType;

            switch (FlightType)
            {
                case FlightTypeEnum.SingleWay:
                    FlightTipInfo = FromCity.Info.Name + "--" + ToCity.Info.Name + " | 单程 " + TakeDate.ToString("yyyy-MM-dd");
                    break;
                case FlightTypeEnum.DoubleWay:
                    FlightTipInfo = FromCity.Info.Name + "--" + ToCity.Info.Name + " | 往返  " + TakeDate.ToString("yyyy-MM-dd") + " / " + BackDate.ToString("yyyy-MM-dd");
                    break;
                case FlightTypeEnum.ConnWay:
                    FlightTipInfo = FromCity.Info.Name + "--" + MidCity.Info.Name + "--" + ToCity.Info.Name + " | 联程  " + TakeDate.ToString("yyyy-MM-dd") + " / " + MidDate.ToString("yyyy-MM-dd");
                    break;
            }

            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFlightDestineService>(service =>
            {
                Logger.WriteLog(LogType.DEBUG, string.Format("{2}【{0}-{1}】", FromCity, ToCity, DateTime.Now));
                _FlightTempAllInfos.Clear();
                switch (FlightType)
                {
                    case FlightTypeEnum.SingleWay:
                        var dataOne = service.QueryOnewayFlight(FromCity.Info.Code, ToCity.Info.Code, TakeDate,
                            IsShare, SelectedCarray == null ? null : SelectedCarray.AirCode);
                        List = ConvertToFlightInfoModels(dataOne);
                        _FlightTempAllInfos.Add(List.ToArray());
                        break;

                    case FlightTypeEnum.DoubleWay:
                        var dataTwo = service.QueryTwowayFlight(FromCity.Info.Code, ToCity.Info.Code, TakeDate,
                            BackDate, IsShare, SelectedCarray == null ? null : SelectedCarray.AirCode);
                        FlightDataRows = ConvertToFlightInfosModels(dataTwo);
                        List = FlightDataRows[0].ToList();
                        _FlightTempAllInfos.Add(FlightDataRows[0]);
                        _FlightTempAllInfos.Add(FlightDataRows[1]);
                        break;
                    case FlightTypeEnum.ConnWay:
                        var dataConnWay = service.QueryConnwayFlight(FromCity.Info.Code, MidCity.Info.Code,
                            ToCity.Info.Code, TakeDate, MidDate, IsShare, SelectedCarray == null ? null : SelectedCarray.AirCode);
                        FlightDataRows = ConvertToFlightInfosModels(dataConnWay);
                        List = FlightDataRows[0].ToList();
                        _FlightTempAllInfos.Add(FlightDataRows[0]);
                        _FlightTempAllInfos.Add(FlightDataRows[1]);
                        break;
                }

            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool ValidateParams()
        {

            if (FromCity == null)
            {
                UIManager.ShowMessage("请输入出发城市名称");
                return false;
            }
            if (ToCity == null)
            {
                UIManager.ShowMessage("请输入到达城市名称");
                return false;
            }

            if (TakeDate.Date < DateTime.Today)
            {
                UIManager.ShowMessage("时间不能小于今天");

                return false;
            }

            if (FlightType == FlightTypeEnum.DoubleWay)
            {
                if (DateTime.Compare(TakeDate.Date, BackDate.Date) > 0)
                {
                    UIManager.ShowMessage("返回日期不能够小于出发日期");

                    return false;
                }
            }
            if (FlightType == FlightTypeEnum.ConnWay)
            {
                if (MidCity == null)
                {
                    UIManager.ShowMessage("请输入中转城市名称");
                    return false;
                }
                if (DateTime.Compare(TakeDate.Date, MidDate.Date) > 0)
                {
                    UIManager.ShowMessage("中转日期不能够小于出发日期");

                    return false;
                }
            }
            return true;


        }

        /// <summary>
        /// 航班信息（数组转数组集合）转换
        /// </summary>
        /// <param name="flights"></param>
        /// <returns></returns>
        private List<FlightInfoModel[]> ConvertToFlightInfosModels(FlightResponse[] flights)
        {
            var list = new List<FlightInfoModel[]>();
            if (flights.Length <= 0) return list;
            var model1 = ConvertToFlightInfoModels(flights[0]).ToArray();
            list.Add(model1);
            if (flights.Length <= 1) return list;
            var model2 = ConvertToFlightInfoModels(flights[1]).ToArray();
            list.Add(model2);

            return list;

        }

        /// <summary>
        /// 航班信息（实体转集合）转换
        /// </summary>
        /// <param name="flight"></param>
        /// <returns></returns>
        private List<FlightInfoModel> ConvertToFlightInfoModels(FlightResponse flight)
        {
            var list = new List<FlightInfoModel>();
            if (flight.List.Count <= 0) return list;
            var k = 1;
            foreach (var m in flight.List)
            {
                var fm = new FlightInfoModel
                {
                    CarrayCode = m.SkyWay.CarrayCode,
                    Model = m.SkyWay.Model,
                    CarrayShortName = m.SkyWay.CarrayShortName,
                    FlightNumber = m.SkyWay.FlightNumber,
                    FromAirPortrName = m.SkyWay.FromAirPortrName,
                    FromCity = m.SkyWay.FromCity,
                    FromCityCode = m.SkyWay.FromCityCode,
                    FromTerminal = m.SkyWay.FromTerminal,
                    RQFee = m.SkyWay.RQFee,
                    StartDateTime = m.SkyWay.StartDateTime,
                    TaxFee = m.SkyWay.TaxFee,
                    ToAirPortrName = m.SkyWay.ToAirPortrName,
                    ToCity = m.SkyWay.ToCity,
                    ToCityCode = m.SkyWay.ToCityCode,
                    ToDateTime = m.SkyWay.ToDateTime,
                    ToTerminal = m.SkyWay.ToTerminal,
                    FlightType = FlightType,
                    CusNo = k,
                    IsStop = m.SkyWay.IsStop,
                    StopText = m.SkyWay.IsStop ? "经停" : "",
                    IsShareFlight = m.SkyWay.IsShareFlight,
                };
                if (fm.IbeRQFee == 0) fm.IbeRQFee = m.SkyWay.RQFee;
                if (fm.IbeTaxFee == 0) fm.IbeTaxFee = m.SkyWay.TaxFee;
                if (m.SeatList != null && m.SeatList.Length > 0)
                {
                    //var minSeat = m.SeatList.Min(p => p.SeatPrice);

                    var siteList = m.SeatList.Select(seat => new Site
                    {
                        Discount = seat.Discount,
                        PolicyPoint = seat.PolicyPoint,
                        SeatCode = seat.SeatCode,
                        SeatCount = seat.SeatCount,
                        SeatPrice = seat.SeatPrice,
                        TicketPrice = seat.TicketPrice,
                        RQFee = m.SkyWay.RQFee,
                        TaxFee = m.SkyWay.TaxFee,
                        CusNo = k,
                        PolicySpecialType = seat.PolicySpecialType,
                        SpecialYPrice = seat.YPrice,
                        IbeSeatResponse = seat,
                        FlightType = fm.FlightType
                    }).ToList();

                    siteList = (
                        from p in siteList
                        orderby p.SeatPrice
                        select p
                        ).ToList();
                    fm.DefaultSite = siteList.Any(p => p.PolicySpecialType != EnumPolicySpecialType.Normal) ? siteList.First(p => p.PolicySpecialType != EnumPolicySpecialType.Normal) : siteList.First();
                    fm.SiteList = siteList;
                    //for (int i = 0; i < m.SeatList.Length; i++)
                    //{
                    //    if (i == 0)
                    //    {
                    //        var defaultSite = new Site
                    //        {
                    //            Discount = m.SeatList[0].Discount,
                    //            PolicyPoint = m.SeatList[0].PolicyPoint,
                    //            SeatCode = m.SeatList[0].SeatCode,
                    //            SeatCount = m.SeatList[0].SeatCount,
                    //            SeatPrice = m.SeatList[0].SeatPrice,
                    //            TicketPrice = m.SeatList[0].TicketPrice,
                    //            RQFee = m.SkyWay.RQFee,
                    //            TaxFee = m.SkyWay.TaxFee,
                    //            CusNo = k
                    //        };
                    //        fm.DefaultSite = defaultSite;
                    //        siteList.Add(defaultSite);
                    //    }
                    //    var morSite = new Site
                    //    {
                    //        Discount = m.SeatList[i].Discount,
                    //        PolicyPoint = m.SeatList[i].PolicyPoint,
                    //        SeatCode = m.SeatList[i].SeatCode,
                    //        SeatCount = m.SeatList[i].SeatCount,
                    //        SeatPrice = m.SeatList[i].SeatPrice,
                    //        TicketPrice = m.SeatList[i].TicketPrice,
                    //        RQFee = m.SkyWay.RQFee,
                    //        TaxFee = m.SkyWay.TaxFee,
                    //        CusNo = k
                    //    };
                    //    siteList.Add(morSite);

                    //}
                }
                else
                {
                    fm.DefaultSite = new Site();
                    fm.SiteList = new List<Site>();
                }

                list.Add(fm);
                k++;
            }


            return list;

        }


        #endregion

        #region 私有属性
        private List<FlightInfoModel[]> _FlightTempAllInfos = new List<FlightInfoModel[]>();

        /// <summary>
        /// 查询缓存
        /// </summary>
        private FlightTypeEnum _queryFlightTypeCache;
        #endregion
    }
}
