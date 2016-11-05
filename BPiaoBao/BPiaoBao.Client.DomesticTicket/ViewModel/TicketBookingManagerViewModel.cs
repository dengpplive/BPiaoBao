using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 机票预订管理视图模型
    /// </summary>
    public class TicketBookingManagerViewModel : BaseVM
    {

        #region 属性
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public TicketBookingManagerViewModel()
        {
            InitViewModalData();
        }
        #endregion

        public override void Initialize()
        {
            base.Initialize();
            UpdateChart();
        }

        #region 构造函数
        /// <summary>
        /// 初始ViewModel数据
        /// </summary>
        private void InitViewModalData()
        {
            UpdateChart();

            var dropDownList = GetACarrayModels();
            Carray = dropDownList;
        }

        private void UpdateChart()
        {
            if (CanExecuteQueryChartStatisticsCommand())
            {
                ExecuteQueryChartStatisticsCommand();
            }
        }


        #endregion

        #region 公开属性
        public const string FlightTypePropertyName = "FlightType";//航程类型
        public const string FromCityPropertyName = "FromCity"; //出发城市
        public const string ToCityPropertyName = "ToCity"; //到达城市
        public const string TakeDateProtertyName = "TakeDate"; //出发时间
        public const string BackDateProtertyName = "BackDate"; //返回时间 
        public const string CarrayProtertyName = "Carray"; //承运人列表
        public const string SelectedCarrayProtertyName = "SelectedCarray"; //承运人列表
        public const string MidCityProtertyName = "MidCity";//中转城市 
        public const string MidDateProtertyName = "MidDate";//中转日期
        public const string ChartModelPropertyName = "ChartModel";//图标数据
        public const string ChartDataPropertyName = "ChartData";//报表数据

        public const string IsShowOneProtertyName = "IsShowOne";//单程
        public const string IsShowTwoProtertyName = "IsShowTwo";//往返程
        public const string IsShowConnWayProtertyName = "IsShowConnWay";//联程


        private FlightTypeEnum _flightType = FlightTypeEnum.SingleWay;//默认为单程
        private CityNewModel _fromCity;
        private CityNewModel _toCity;
        private DateTime _takeDate = DateTime.Now.Date;
        private DateTime _backDate = DateTime.Now.Date;
        private List<CarrayModel> _carray;
        private CarrayModel _selecttedCarray;
        private CityNewModel _midCity;
        private DateTime _midDate = DateTime.Now.Date;
        private Collection<Current15DayDataDto.DataStatistics> _chartModel = new Collection<Current15DayDataDto.DataStatistics>();
        private Current15DayDataDto _chartData;

        private bool _isShowOne = true;
        private bool _isShowTwo;
        private bool _isShowConnWay;

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
            }
        }

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

        public Collection<Current15DayDataDto.DataStatistics> ChartModel
        {
            get { return _chartModel; }
            set
            {
                if (_chartModel == value)
                {
                    return;
                }
                RaisePropertyChanging(ChartModelPropertyName);
                _chartModel = value;
                RaisePropertyChanged(ChartModelPropertyName);
            }
        }


        public Current15DayDataDto ChartData
        {
            get { return _chartData; }
            set
            {
                if (_chartData == value)
                {
                    return;
                }
                RaisePropertyChanging(ChartDataPropertyName);
                _chartData = value;
                RaisePropertyChanged(ChartDataPropertyName);
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
        #endregion

        #region 公开命令

        #region QueryChartStatisticsCommand

        private RelayCommand _queryChartStatisticsCommand;

        /// <summary>
        /// 查询图标统计命令
        /// </summary>
        public RelayCommand QueryChartStatisticsCommand
        {
            get
            {
                return _queryChartStatisticsCommand ?? (_queryChartStatisticsCommand = new RelayCommand(ExecuteQueryChartStatisticsCommand, CanExecuteQueryChartStatisticsCommand));
            }
        }

        private void ExecuteQueryChartStatisticsCommand()
        {
            //ChartModel.Clear();
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var data = service.Query15DayStatistics();
                var temp = new Collection<Current15DayDataDto.DataStatistics>();

                for (int i = 0; i < 15; i++)
                {
                    //生成15天数据
                    temp.Add(new Current15DayDataDto.DataStatistics
                    {
                        Day = DateTime.Today.AddDays(-15 + i + 1)
                    });
                }

                if (data.DataStatisticsList != null)
                    foreach (var item in data.DataStatisticsList)
                    {
                        var exist = temp.FirstOrDefault(m => m.Day.Year == item.Day.Year && m.Day.DayOfYear == item.Day.DayOfYear);
                        if (exist != null)
                            exist.TradeTotalMoney += item.TradeTotalMoney;
                    }



                ChartModel = temp;

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteQueryChartStatisticsCommand()
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
                       (_switchFlightTypeCommand = new RelayCommand<string>(ExecuteSwitchFlightTypeCommand));
            }
        }

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
                    BackDate = TakeDate;
                    break;
                case FlightTypeEnum.ConnWay:
                    IsShowOne = false;
                    IsShowTwo = false;
                    IsShowConnWay = true;
                    MidDate = TakeDate;
                    break;

            }
            FlightType = type;

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
                return _switchCityCommand ?? (_switchCityCommand = new RelayCommand(ExecuteSwitchCityCommand));
            }
        }

        private void ExecuteSwitchCityCommand()
        {
            var temp = FromCity;
            FromCity = ToCity;
            ToCity = temp;
        }


        #endregion


        #region switchToQueryResultViewCommand

        private RelayCommand _switchToQueryResultViewCommand;

        /// <summary>
        /// 查询操作
        /// </summary>
        public RelayCommand SwitchToQueryResultViewCommand
        {
            get
            {
                return _switchToQueryResultViewCommand ?? (_switchToQueryResultViewCommand = new RelayCommand(ExecuteSwitchToQueryResultViewCommand));
            }
        }

        private void ExecuteSwitchToQueryResultViewCommand()
        {
            if (!ValidateParams())
            {
                return;
            }

            ViewModelLocator.TicketBookingQuery.FlightType = FlightType;
            ViewModelLocator.TicketBookingQuery.FromCity = FromCity;
            ViewModelLocator.TicketBookingQuery.ToCity = ToCity;
            ViewModelLocator.TicketBookingQuery.MidCity = MidCity;
            ViewModelLocator.TicketBookingQuery.MidDate = MidDate;
            ViewModelLocator.TicketBookingQuery.TakeDate = TakeDate;
            ViewModelLocator.TicketBookingQuery.BackDate = BackDate;
            ViewModelLocator.TicketBookingQuery.Carray = Carray;
            ViewModelLocator.TicketBookingQuery.SelectedCarray = SelectedCarray;
            switch (FlightType)
            {
                case FlightTypeEnum.SingleWay:
                    ViewModelLocator.TicketBookingQuery.FlightTipInfo = FromCity.Info.Name + "--" + ToCity.Info.Name + " | 单程 " + TakeDate.ToString("yyyy-MM-dd");
                    break;
                case FlightTypeEnum.DoubleWay:
                    ViewModelLocator.TicketBookingQuery.FlightTipInfo = FromCity.Info.Name + "--" + ToCity.Info.Name + " | 往返  " + TakeDate.ToString("yyyy-MM-dd") + " / " + BackDate.ToString("yyyy-MM-dd");
                    break;
                case FlightTypeEnum.ConnWay:
                    ViewModelLocator.TicketBookingQuery.FlightTipInfo = FromCity.Info.Name + "--" + MidCity.Info.Name + "--" + ToCity.Info.Name + " | 联程  " + TakeDate.ToString("yyyy-MM-dd") + " / " + MidDate.ToString("yyyy-MM-dd");
                    break;
            }
            ViewModelLocator.TicketBookingQuery.Initialize();
            SwitchView(Main.TicketBookingQueryResultCode);
        }

        #endregion



        #endregion

        #region 私有方法
        /// <summary>
        /// 切换视图
        /// </summary>
        /// <param name="code"></param>
        private void SwitchView(string code)
        {
           if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, code);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            { 
                PluginService.Run(Main.ProjectCode, code);
            }
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
        /// 得到所有航空公司列表
        /// </summary>
        /// <returns></returns>
        private List<CarrayModel> GetACarrayModels()
        {
            var list = Agent.GetCarryInfos();
            list.Insert(0, new CarrayModel { AirCode = null, AirName = "全部", AirShortName = "全部" });
            return list;
        }



        #endregion
    }
}
