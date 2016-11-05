using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// PNR内容导入 视图模型
    /// </summary>
    public class PNRViewModel : ChoosePolicyViewModel
    {
        #region 成员变量

        public const string ToTicketBookingQueryViewMessageCommission = "ToTicketBookingQueryViewMessage_Commission";
        public const string ToChoosePolicyViewMessageCommission = "ToChoosePolicyViewMessage_Commission";
        /// <summary>
        /// PNR参数实体对象
        /// </summary>
        private PnrImportParam pip = new PnrImportParam();
        #endregion

        #region 构造函数

        /// <summary>
        /// 
        /// </summary>
        public PNRViewModel()
        {
            Policys = new List<PolicyDto>();

            if (IsInDesignMode)
                return;

            Messenger.Default.Register<string>(this, TicketBookingViewModel.CSearchPnr,
                p =>
                {
                    PnrCode = p;
                    Search(0);
                });

            Messenger.Default.Register<Visibility>(this, TicketBookingQueryViewModel.ToPnrViewMessageCommission,
                p =>
                {
                    IsShowCommissionColumn = p;
                });
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryChartStatisticsCommand())
                ExecuteQueryChartStatisticsCommand();
        }

        #endregion

        #region 公开属性


        #region IsShowCommissionColumn

        private const string IsShowCommissionColumnProtertyName = "IsShowCommissionColumn";

        private Visibility _isShowCommissionColumn = Visibility.Visible;

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
        #endregion

        #region PnrCode

        /// <summary>
        /// The <see cref="PnrCode" /> property's name.
        /// </summary>
        private const string PnrCodePropertyName = "PnrCode";

        private string _pnrCode;

        /// <summary>
        /// 输入的Pnr代码
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

        #region OrderID

        /// <summary>
        /// The <see cref="OrderID" /> property's name.
        /// </summary>
        private const string OrderIdPropertyName = "OrderID";

        private string _orderId;

        /// <summary>
        /// 成人(原)订单号
        /// </summary>
        /// <value>
        /// The order detail.
        /// </value>
        public string OrderID
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

        #region Policys

        ///// <summary>
        ///// The <see cref="Policys" /> property's name.
        ///// </summary>
        //public const string PolicysPropertyName = "Policys";

        //private ObservableCollection<PolicyDto> policys = null;

        ///// <summary>
        ///// PNR列表
        ///// </summary>
        //public ObservableCollection<PolicyDto> Policys
        //{
        //    get { return policys; }

        //    set
        //    {
        //        if (policys == value) return;

        //        RaisePropertyChanging(PolicysPropertyName);
        //        policys = value;
        //        RaisePropertyChanged(PolicysPropertyName);
        //    }
        //}

        #endregion

        #region IsBusy

        /// <summary>
        /// 是否正在忙
        /// </summary>
        public override bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_importPnrCommand != null)
                    _importPnrCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region SeatPrice

        /// <summary>
        /// The <see cref="SeatPrice" /> property's name.
        /// </summary>
        private const string SeatPricePropertyName = "SeatPrice";

        private decimal _seatPrice;

        /// <summary>
        /// 座舱价
        /// </summary>
        public decimal SeatPrice
        {
            get { return _seatPrice; }

            set
            {
                if (_seatPrice == value) return;

                RaisePropertyChanging(SeatPricePropertyName);
                _seatPrice = value;
                RaisePropertyChanged(SeatPricePropertyName);
            }
        }

        #endregion

        #region ChartModel

        /// <summary>
        /// The <see cref="ChartModel" /> property's name.
        /// </summary>
        private const string ChartModelPropertyName = "ChartModel";

        private Collection<Current15DayDataDto.DataStatistics> _chartModel = new Collection<Current15DayDataDto.DataStatistics>();

        /// <summary>
        /// 图标数据
        /// </summary>
        private Collection<Current15DayDataDto.DataStatistics> ChartModel
        {
            get { return _chartModel; }

            set
            {
                if (_chartModel == value) return;

                RaisePropertyChanging(ChartModelPropertyName);
                _chartModel = value;
                RaisePropertyChanged(ChartModelPropertyName);
            }
        }

        #endregion

        #region ChartData

        /// <summary>
        /// The <see cref="ChartData" /> property's name.
        /// </summary>
        private const string ChartDataPropertyName = "ChartData";

        private Current15DayDataDto _chartData;

        /// <summary>
        /// 报表数据
        /// </summary>
        public Current15DayDataDto ChartData
        {
            get { return _chartData; }

            set
            {
                if (_chartData == value) return;

                RaisePropertyChanging(ChartDataPropertyName);
                _chartData = value;
                RaisePropertyChanged(ChartDataPropertyName);
            }
        }

        #endregion

        #region IsChangePnrTicket
        /// <summary>
        /// The <see cref="IsChangePnrTicket" /> property's name.
        /// </summary>
        private const string IsChangePnrTicketPropertyName = "IsChangePnrTicket";
        private bool _isChangePnrTicket;
        /// <summary>
        /// 是否换编码出票
        /// </summary>
        public bool IsChangePnrTicket
        {
            get { return _isChangePnrTicket; }

            set
            {
                if (_isChangePnrTicket == value) return;
                RaisePropertyChanging(IsChangePnrTicketPropertyName);
                _isChangePnrTicket = value;
                RaisePropertyChanged(IsChangePnrTicketPropertyName);
            }
        }

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

        #region ImportPNRCommand

        private RelayCommand _importPnrCommand;

        /// <summary>
        /// 导入PNR命令
        /// </summary>
        public RelayCommand ImportPNRCommand
        {
            get
            {
                return _importPnrCommand ?? (_importPnrCommand = new RelayCommand(ExecuteImportPnrCommand, CanExecuteImportPnrCommand));
            }
        }

        private void ExecuteImportPnrCommand()
        {
            Search();
        }

        private void Search(int PnrSource = 1)
        {
            
            Policys.Clear();
            Action action = () =>
            {
                IsBusy = true;

                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var pnr = Trim(PnrCode); 
                    //只有儿童编码和升舱换开时赋值成人(原)订单号
                    pip.OldOrderId = pip.PnrImportType == Common.Enums.EnumPnrImportType.UpSeatChangePnrImport || pip.PnrImportType == Common.Enums.EnumPnrImportType.CHDPnrImport ? OrderID : string.Empty;
                    pip.PnrAndPnrContent = pnr;
                    pip.IsChangePnrTicket = IsChangePnrTicket;
                    pip.IsLowPrice = IsLowPrice;
                    var result = service.ImportPnrContext(pip);//service.ImportPnrContext(pnr, PnrSource);
                    //只有儿童编码导入时返回儿童订单号
                    OrderId = pip.PnrImportType == Common.Enums.EnumPnrImportType.CHDPnrImport ? result.ChdOrderId : result.OrderId;
                    //SeatPrice = result.SeatPrice;
                    //PNR导入方式如果是升舱换开直接跳转到支付页面
                    //if (pip.PnrImportType == Common.Enums.EnumPnrImportType.UpSeatChangePnrImport)
                    //{
                    //    LocalUIManager.ShowPolicy(result.OrderId, null);
                    //    return;
                    //}
                    if (result.PolicyList == null || result.PolicyList.Count == 0) return;

                    //导入成功，清空pnr
                    PnrCode = null;

                    //foreach (var item in result.PolicyList)
                    //{
                    //    DispatcherHelper.UIDispatcher.Invoke(new Action<PolicyDto>(Policys.Add), item);
                    //}
                    //非白屏预定和手机预定来源的订单政策过滤到动态特价
                    Policys = (result.OrderSource != EnumOrderSource.WhiteScreenDestine && result.OrderSource != EnumOrderSource.MobileDestine) 
                        ? result.PolicyList.Where(p => p.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial).ToList()
                        : result.PolicyList;
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private string Trim(string pnr)
        {
            try
            {
                var sb = new StringBuilder();
                var reader = new System.IO.StringReader(pnr);
                var temp = reader.ReadLine();
                while (temp != null)
                {
                    temp = temp.TrimEnd();
                    sb.AppendLine(temp);

                    temp = reader.ReadLine();
                }

                return sb.ToString();
            }
            catch
            {
                return null;
            }
        }

        private bool CanExecuteImportPnrCommand()
        {
            return !isBusy && PnrCode != null;
        }

        #endregion

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
                    temp.Add(new Current15DayDataDto.DataStatistics()
                    {
                        Day = DateTime.Today.AddDays(-15 + i + 1)
                    });
                }

                if (data.DataStatisticsList != null)
                    foreach (var item in data.DataStatisticsList)
                    {
                        var exist = temp.FirstOrDefault(m => m.Day.Year == item.Day.Year && m.Day.DayOfYear == item.Day.DayOfYear);
                        if (exist != null)
                            exist.TradeTotalMoney = item.TradeTotalMoney;
                    }

                //int test = 0;

                //foreach (var item in temp)
                //    item.TradeTotalMoney = test++;

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
            return true;
        }

        #endregion

        #region OpenUriCommand

        private RelayCommand<string> _openUriCommand;

        /// <summary>
        /// 打开uri命令
        /// </summary>
        public RelayCommand<string> OpenUriCommand
        {
            get
            {
                return _openUriCommand ?? (_openUriCommand = new RelayCommand<string>(ExecuteOpenUriCommand, CanExecuteOpenUriCommand));
            }
        }

        private void ExecuteOpenUriCommand(string uri)
        {
            LocalUIManager.OpenDefaultBrowser(uri);
        }

        private bool CanExecuteOpenUriCommand(string uri)
        {
            return uri != null;
        }

        #endregion

        #region SelectCommand

        private RelayCommand _selectCommand;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand(ExecuteSelectCommand));
            }
        }

        private void ExecuteSelectCommand()
        {
            pip.PnrImportType = Common.Enums.EnumPnrImportType.GenericPnrImport;
        }
        #endregion 
        #region SelectCommand0

        private RelayCommand _selectCommand0;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand SelectCommand0
        {
            get
            {
                return _selectCommand0 ?? (_selectCommand0 = new RelayCommand(ExecuteSelectCommand0));
            }
        }

        private void ExecuteSelectCommand0()
        {
            pip.PnrImportType = Common.Enums.EnumPnrImportType.PnrContentImport;
        }
        #endregion 
        #region SelectCommand1

        private RelayCommand _selectCommand1;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand SelectCommand1
        {
            get
            {
                return _selectCommand1 ?? (_selectCommand1 = new RelayCommand(ExecuteSelectCommand1));
            }
        }

        private void ExecuteSelectCommand1()
        {
            pip.PnrImportType = Common.Enums.EnumPnrImportType.CHDPnrImport; 
        }
        #endregion 
        #region SelectCommand2

        private RelayCommand _selectCommand2;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand SelectCommand2
        {
            get
            {
                return _selectCommand2 ?? (_selectCommand2 = new RelayCommand(ExecuteSelectCommand2));
            }
        }

        private void ExecuteSelectCommand2()
        {
            pip.PnrImportType = Common.Enums.EnumPnrImportType.UpSeatChangePnrImport; 
        }
        #endregion 
        #endregion
        public void OnLoaded()
        {
            Policys.Clear();
        }
    }
}
