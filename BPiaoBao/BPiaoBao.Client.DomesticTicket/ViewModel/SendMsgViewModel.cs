using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 发送短信视图模型
    /// </summary>
    public class SendMsgViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="SendMsgViewModel"/> class.
        /// </summary>
        public SendMsgViewModel()
        {
            IsLoading = true;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                //1:剩余条数
                //2:发送条数
                //3:短信价格
                var result = service.GetSystemInfo();
                MessageCount = result.Item1;

                //SmsTemplates = service.GetAllSmsTemplate();
                //SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("出票模板"));

                _listSms = service.GetAllSmsTemplate();

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region 公开属性

        #region Order

        /// <summary>
        /// The <see cref="Order " /> property's name.
        /// </summary>
        private const string OrderPropertyName = "Order ";

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
                //模板乘机人相关筛选
                if (_order.Passengers == null) return;
                foreach (var item in _order.Passengers)
                {
                    Passenger.Add(new PassengerModel
                    {
                        Id = item.Id,
                        IDNumer = item.CardNo,
                        IsChecked = true,
                        Name = item.PassengerName,
                        PhoneNum = item.Mobile
                    });
                }
                //乘机人添加到临时缓存
                Passenger.ForEach(_passengerCahce.Add);
                FilterPassengers();
                //根据订单航程筛选模板内容
                if (_order.SkyWays != null && _order.SkyWays.Count == 1)
                {
                    SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.IssueTicket).ToList();
                    SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("出票模板"));
                }
                else if (_order.SkyWays != null && _order.SkyWays.Count == 2)
                {
                    //返程联程
                    var fromCity1 = _order.SkyWays[0].FromCity;
                    var fromCity2 = _order.SkyWays[1].ToCity;
                    var toCity1 = _order.SkyWays[0].FromCity;
                    var toCity2 = _order.SkyWays[1].ToCity;
                    if (fromCity1 == toCity2 && toCity1 == fromCity2)
                    {
                        SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.Return && p.TemplateType == EnumSmsTemplateType.IssueTicket).ToList();
                        SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("出票往返模板"));
                    }
                    else
                    {
                        SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.Connecting && p.TemplateType == EnumSmsTemplateType.IssueTicket).ToList();
                        SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("出票中转模板"));
                    }
                }
                GeneralMessage();
                RaisePropertyChanged(OrderPropertyName);
            }
        }

        #endregion

        #region Passenger

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

        #region Message

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        private const string MessagePropertyName = "Message";

        private string _message;

        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return _message; }

            set
            {
                if (_message == value) return;

                RaisePropertyChanging(MessagePropertyName);
                _message = value;
                RaisePropertyChanged(MessagePropertyName);
            }
        }

        #endregion

        #region MessageCount

        /// <summary>
        /// The <see cref="MessageCount" /> property's name.
        /// </summary>
        private const string MessageCountPropertyName = "MessageCount";

        private int _messageCount;

        /// <summary>
        /// 剩余短信数量
        /// </summary>
        public int MessageCount
        {
            get { return _messageCount; }

            set
            {
                if (_messageCount == value) return;

                RaisePropertyChanging(MessageCountPropertyName);
                _messageCount = value;
                RaisePropertyChanged(MessageCountPropertyName);
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

        #region IsSending

        /// <summary>
        /// The <see cref="IsSending" /> property's name.
        /// </summary>
        private const string IsSendingPropertyName = "IsSending";

        private bool _isSending;

        /// <summary>
        /// 是否正在发送短信
        /// </summary>
        public bool IsSending
        {
            get { return _isSending; }

            set
            {
                if (_isSending == value) return;

                RaisePropertyChanging(IsSendingPropertyName);
                _isSending = value;
                RaisePropertyChanged(IsSendingPropertyName);

                if (_sendMessageCommand != null)
                    _sendMessageCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region SendMessage

        /// <summary>
        /// The <see cref="SendMessage" /> property's name.
        /// </summary>
        private const string SendMessagePropertyName = "SendMessage";

        private string _sendMessage;

        /// <summary>
        /// 发送结果
        /// </summary>
        public string SendMessage
        {
            get { return _sendMessage; }

            set
            {
                if (_sendMessage == value) return;

                RaisePropertyChanging(SendMessagePropertyName);
                _sendMessage = value;
                RaisePropertyChanged(SendMessagePropertyName);
            }
        }

        #endregion

        #region SelectedSmsTemplate

        /// <summary>
        /// The <see cref="SelectedSmsTemplate" /> property's name.
        /// </summary>
        private const string SelectedSmsTemplatePropertyName = "SelectedSmsTemplate";

        private SMSTemplateDto _selectedSmsTemplate;

        /// <summary>
        /// 选中的短信模板
        /// </summary>
        public SMSTemplateDto SelectedSmsTemplate
        {
            get { return _selectedSmsTemplate; }

            set
            {
                if (_selectedSmsTemplate == value) return;
                RaisePropertyChanging(SelectedSmsTemplatePropertyName);
                _selectedSmsTemplate = value;
                GeneralMessage();
                RaisePropertyChanged(SelectedSmsTemplatePropertyName);
            }
        }

        #endregion

        #region SmsTemplates

        /// <summary>
        /// The <see cref="SelectedSmsTemplate" /> property's name.
        /// </summary>
        private const string SmsTemplatesPropertyName = "SmsTemplates";

        private List<SMSTemplateDto> _smsTemplates;

        /// <summary>
        /// 短信模板列表
        /// </summary>
        public List<SMSTemplateDto> SmsTemplates
        {
            get { return _smsTemplates; }

            set
            {
                if (_smsTemplates == value) return;

                RaisePropertyChanging(SmsTemplatesPropertyName);
                _smsTemplates = value;
                RaisePropertyChanged(SmsTemplatesPropertyName);
            }
        }

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

                //模板乘机人相关筛选
                if (_afterSaleInfo.Passenger == null) return;
                foreach (var item in _afterSaleInfo.Passenger)
                {
                    Passenger.Add(new PassengerModel
                    {
                        Id = item.Id,
                        IDNumer = item.CardNo,
                        IsChecked = true,
                        Name = item.PassengerName,
                        PhoneNum = item.Mobile
                    });
                }
                //根据订单航程筛选模板内容
                if (_afterSaleInfo.SkyWays != null && _afterSaleInfo.SkyWays.Count == 1)
                {
                    switch (_afterSaleInfo.AfterSaleType)
                    {
                        case "退票":
                            SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.RefoundTicket).ToList();
                            SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("退票模板"));
                            break;
                        case "废票":
                            SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.AnnulTicket).ToList();
                            SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("废票模板"));
                            break;
                        case "改签":
                            SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.ChangeTicket).ToList();
                            SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("改签模板"));
                            break;
                    }
                }
                else if (_afterSaleInfo.SkyWays != null && _afterSaleInfo.SkyWays.Count == 2)
                {
                    //返程联程
                    var fromCity1 = _afterSaleInfo.SkyWays[0].FromCity;
                    var fromCity2 = _afterSaleInfo.SkyWays[1].ToCity;
                    var toCity1 = _afterSaleInfo.SkyWays[0].FromCity;
                    var toCity2 = _afterSaleInfo.SkyWays[1].ToCity;
                    if (fromCity1 == toCity2 && toCity1 == fromCity2)
                    {
                        switch (_afterSaleInfo.AfterSaleType)
                        {
                            case "退票":
                                SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.RefoundTicket).ToList();
                                SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("退票模板"));
                                break;
                            case "废票":
                                SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.AnnulTicket).ToList();
                                SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("废票模板"));
                                break;
                            case "改签":
                                SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.Return && p.TemplateType == EnumSmsTemplateType.ChangeTicket).ToList();
                                SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("改签(往返)模板"));
                                break;
                        }
                    }
                    else
                    {
                        switch (_afterSaleInfo.AfterSaleType)
                        {
                            case "退票":
                                SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.RefoundTicket).ToList();
                                SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("退票模板"));
                                break;
                            case "废票":
                                SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.OneWay && p.TemplateType == EnumSmsTemplateType.AnnulTicket).ToList();
                                SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("废票模板"));
                                break;
                            case "改签":
                                SmsTemplates = _listSms.Where(p => p.SkyWayType == EnumSkyWayType.Connecting && p.TemplateType == EnumSmsTemplateType.ChangeTicket).ToList();
                                SelectedSmsTemplate = SmsTemplates.FirstOrDefault(p => p.TemplateName.Equals("改签(中转)模板"));
                                break;
                        }
                    }
                }
                GeneralMessage();
                RaisePropertyChanged(AfterSaleInfoPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令

        #region SendMessageCommand

        private RelayCommand _sendMessageCommand;

        /// <summary>
        /// 发送短信命令
        /// </summary>
        public RelayCommand SendMessageCommand
        {
            get
            {
                return _sendMessageCommand ?? (_sendMessageCommand = new RelayCommand(ExecuteSendMessageCommand, CanExecuteSendMessageCommand));
            }
        }

        private void ExecuteSendMessageCommand()
        {
            SendMessage = null;
            IsSending = true;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                var existSelected = Passenger.FirstOrDefault(m => m.IsChecked);
                if (existSelected == null)
                {
                    UIManager.ShowMessage("请至少选中一个联系人");
                    return;
                }

                var isOk = false;
                foreach (var item in Passenger.Where(item => item.IsChecked))
                {
                    if (!ValidationHelper.Instance.IsMobilePhoneNum(item.PhoneNum))
                    {
                        UIManager.ShowMessage(String.Format("{0} 不是有效手机号码", item.PhoneNum));
                        continue;
                    }

                    service.SendSms(item.Name, item.PhoneNum, Message);
                    isOk = true;
                }

                if (!isOk) return;
                SendMessage = "发送成功";
                //执行短信条数更新
                DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IBusinessmanService>(service2 =>
                {
                    //1:剩余条数
                    //2:发送条数
                    //3:短信价格
                    var result = service2.GetSystemInfo();
                    MessageCount = result.Item1;
                }, UIManager.ShowErr)));
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsSending = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteSendMessageCommand()
        {
            bool can = Passenger != null && Passenger.FirstOrDefault(m => m.IsChecked) != null && !_isSending;
            return can;
        }

        #endregion

        #endregion

        #region 私有方法

        /// <summary>
        /// 过滤筛选乘机人信息
        /// </summary>
        private void FilterPassengers()
        {
            var passengers0 = new List<ResponseAfterSalePassenger>();
            if (_afterSaleOrders.Any())
            {
                _afterSaleOrders.ForEach(p => p.Passenger.ForEach(x =>
                {
                    if (!passengers0.Contains(x) && p.AfterSaleType != "其它修改") 
                        passengers0.Add(x);
                }));
            }
            if (passengers0.Any())
            {
                _passengerCahce.ForEach(x =>
                {
                    if (passengers0.Any(p => p.Id == x.Id && p.Status != EnumTfgPassengerStatus.RepelProcess))
                        Passenger.Remove(x);
                });
            }
        }
        /// <summary>
        /// 生成短信内容
        /// </summary>
        private void GeneralMessage()
        {
            if (SelectedSmsTemplate == null) return;
            if (Order != null)
            {
                //筛选乘机人信息处理
                var reulstpassengers = new List<PassengerDto>();
                Passenger.ForEach(p => reulstpassengers.Add(Order.Passengers.FirstOrDefault(x => x.Id == p.Id)));
                var passengerInfo = GetPassengerInfo(reulstpassengers).ToString();// new PassengersConverter().Convert(Order.Passengers, null, null, null) + "";
                string toCity = null;
                string flightTime;
                string arriveTime;
                string flightNumber;
                string flightCarrayCode;
                string carrayShortName;
                if (Order.SkyWays != null && Order.SkyWays.Count == 1)
                {
                    //单程
                    toCity = String.Format("{0}-{1}", Order.SkyWays[0].FromCity + Order.SkyWays[0].FromTerminal, Order.SkyWays[0].ToCity + Order.SkyWays[0].ToTerminal);
                    flightTime = Order.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                    //arriveTime = Order.SkyWays[0].ToDateTime.ToString("yyyy-MM-dd HH:mm");
                    arriveTime = Order.SkyWays[0].ToDateTime.ToString("HH:mm");
                    flightNumber = Order.SkyWays[0].FlightNumber;
                    flightCarrayCode = Order.SkyWays[0].CarrayCode;
                    carrayShortName = Order.SkyWays[0].CarrayShortName;
                    // Message = String.Format(@"尊敬的{0}，您预订的{1}{2}{3}({4}) ，于{5}起飞，{6}到达，已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!",
                    //passengerInfo, carrayShortName, flightCarrayCode, flightNumber, toCity, flightTime, arriveTime);
                    //////////////////////////////////////// 
                    var templateStr = SelectedSmsTemplate.TemplateContents;
                    templateStr = templateStr.Replace("[乘客名称]", passengerInfo);
                    templateStr = templateStr.Replace("[航空公司航班号]", carrayShortName + flightCarrayCode + flightNumber);
                    templateStr = templateStr.Replace("[出发城市]", Order.SkyWays[0].FromCity);
                    templateStr = templateStr.Replace("[出发航站楼]", Order.SkyWays[0].FromTerminal);
                    templateStr = templateStr.Replace("[到达城市]", Order.SkyWays[0].ToCity);
                    templateStr = templateStr.Replace("[到达航站楼]", Order.SkyWays[0].ToTerminal);
                    templateStr = templateStr.Replace("[起飞时间]", flightTime);
                    templateStr = templateStr.Replace("[到达时间]", arriveTime);
                    Message = templateStr;

                }
                else if (Order.SkyWays != null && Order.SkyWays.Count == 2)
                {
                    //返程联程
                    var fromCity1 = Order.SkyWays[0].FromCity;
                    var fromCity2 = Order.SkyWays[1].ToCity;
                    var toCity1 = Order.SkyWays[0].FromCity;
                    var toCity2 = Order.SkyWays[1].ToCity;
                    carrayShortName = Order.SkyWays[0].CarrayShortName;
                    flightCarrayCode = Order.SkyWays[0].CarrayCode;
                    flightNumber = Order.SkyWays[0].FlightNumber;
                    toCity = String.Format("{0}-{1}", Order.SkyWays[0].FromCity + Order.SkyWays[0].FromTerminal, Order.SkyWays[0].ToCity + Order.SkyWays[0].ToTerminal);
                    flightTime = Order.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                    arriveTime = Order.SkyWays[0].ToDateTime.ToString("HH:mm");
                    //////////////
                    var carrayShortNameBack = Order.SkyWays[1].CarrayShortName;
                    var flightCarrayCodeBack = Order.SkyWays[1].CarrayCode;
                    var flightNumberNameBack = Order.SkyWays[1].FlightNumber;
                    var toCityBack = String.Format("{0}-{1}", Order.SkyWays[1].FromCity + Order.SkyWays[1].FromTerminal, Order.SkyWays[1].ToCity + Order.SkyWays[1].ToTerminal);
                    var flightTimeBack = Order.SkyWays[1].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                    var arriveTimeBack = Order.SkyWays[1].ToDateTime.ToString("HH:mm");
                    if (fromCity1 == toCity2 && toCity1 == fromCity2)
                    {
                        //往返
                        //Message =
                        //    String.Format(
                        //        @"尊敬的{0}，您预订的{1}{2}{3}({4}) ，于{5}起飞，{6}到达，（回程）{7}{8}{9}({10})，于{11}起飞，{12}到达，已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!",
                        //        passengerInfo, carrayShortName, flightCarrayCode, flightNumber, toCity, flightTime,
                        //        arriveTime, carrayShortNameBack, flightCarrayCodeBack, flightNumberNameBack, toCityBack,
                        //        flightTimeBack, arriveTimeBack);
                        var templateStr = SelectedSmsTemplate.TemplateContents;
                        templateStr = templateStr.Replace("[乘客名称]", passengerInfo);
                        templateStr = templateStr.Replace("[航空公司航班号]", carrayShortName + flightCarrayCode + flightNumber);
                        templateStr = templateStr.Replace("[出发城市]", Order.SkyWays[0].FromCity);
                        templateStr = templateStr.Replace("[出发航站楼]", Order.SkyWays[0].FromTerminal);
                        templateStr = templateStr.Replace("[到达城市]", Order.SkyWays[0].ToCity);
                        templateStr = templateStr.Replace("[到达航站楼]", Order.SkyWays[0].ToTerminal);
                        templateStr = templateStr.Replace("[起飞时间]", flightTime);
                        templateStr = templateStr.Replace("[到达时间]", arriveTime);

                        templateStr = templateStr.Replace("[回程航空公司航班号]", carrayShortNameBack + flightCarrayCodeBack + flightNumberNameBack);
                        templateStr = templateStr.Replace("[回程出发城市]", Order.SkyWays[1].FromCity);
                        templateStr = templateStr.Replace("[回程出发航站楼]", Order.SkyWays[1].FromTerminal);
                        templateStr = templateStr.Replace("[回程到达城市]", Order.SkyWays[1].ToCity);
                        templateStr = templateStr.Replace("[回程到达航站楼]", Order.SkyWays[1].ToTerminal);
                        templateStr = templateStr.Replace("[回程起飞时间]", flightTimeBack);
                        templateStr = templateStr.Replace("[回程到达时间]", arriveTimeBack);
                        Message = templateStr;


                    }
                    else
                    {
                        //中转
                        //Message =
                        //  String.Format(
                        //      @"尊敬的{0}，您预订的{1}{2}{3}({4}) ，于{5}起飞，{6}到达，（中转）{7}{8}{9}({10})，于{11}起飞，{12}到达，已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!",
                        //      passengerInfo, carrayShortName, flightCarrayCode, flightNumber, toCity, flightTime,
                        //      arriveTime, carrayShortNameBack, flightCarrayCodeBack, flightNumberNameBack, toCityBack,
                        //      flightTimeBack, arriveTimeBack);
                        var templateStr = SelectedSmsTemplate.TemplateContents;
                        templateStr = templateStr.Replace("[乘客名称]", passengerInfo);
                        templateStr = templateStr.Replace("[航空公司航班号]", carrayShortName + flightCarrayCode + flightNumber);
                        templateStr = templateStr.Replace("[出发城市]", Order.SkyWays[0].FromCity);
                        templateStr = templateStr.Replace("[出发航站楼]", Order.SkyWays[0].FromTerminal);
                        templateStr = templateStr.Replace("[到达城市]", Order.SkyWays[0].ToCity);
                        templateStr = templateStr.Replace("[到达航站楼]", Order.SkyWays[0].ToTerminal);
                        templateStr = templateStr.Replace("[起飞时间]", flightTime);
                        templateStr = templateStr.Replace("[到达时间]", arriveTime);

                        templateStr = templateStr.Replace("[中转航空公司航班号]", carrayShortNameBack + flightCarrayCodeBack + flightNumberNameBack);
                        templateStr = templateStr.Replace("[中转出发城市]", Order.SkyWays[1].FromCity);
                        templateStr = templateStr.Replace("[中转出发航站楼]", Order.SkyWays[1].FromTerminal);
                        templateStr = templateStr.Replace("[中转到达城市]", Order.SkyWays[1].ToCity);
                        templateStr = templateStr.Replace("[中转到达航站楼]", Order.SkyWays[1].ToTerminal);
                        templateStr = templateStr.Replace("[回程起飞时间]", flightTimeBack);
                        templateStr = templateStr.Replace("[回程到达时间]", arriveTimeBack);
                        Message = templateStr;
                    }
                }
            }
            if (AfterSaleInfo != null)
            {
                //筛选乘机人信息处理
                var passengerInfo = GetAfterSalePassengerInfo(AfterSaleInfo.Passenger).ToString();// new PassengersConverter().Convert(AfterSaleInfo.Passengers, null, null, null) + "";
                string toCity = null;
                string flightTime;
                string arriveTime;
                string flightNumber;
                string flightCarrayCode;
                string carrayShortName;
                if (AfterSaleInfo.SkyWays != null && AfterSaleInfo.SkyWays.Count == 1)
                {
                    //单程
                    toCity = String.Format("{0}-{1}",
                    AfterSaleInfo.SkyWays[0].FromCity + AfterSaleInfo.SkyWays[0].FromTerminal,
                    AfterSaleInfo.SkyWays[0].ToCity + AfterSaleInfo.SkyWays[0].ToTerminal);
                    if (AfterSaleInfo.AfterSaleType == "改签")
                    {
                        var changeOrder = AfterSaleInfo as ResponseChangeOrder;
                        if (changeOrder == null) return;
                        var skyway = changeOrder.SkyWay;
                        flightTime = skyway[0].NewStartDateTime.ToString("yyyy-MM-dd HH:mm");
                        arriveTime = skyway[0].NewToDateTime.ToString("HH:mm");
                        flightNumber = skyway[0].NewFlightNumber;
                    }
                    else
                    {
                        flightTime = AfterSaleInfo.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                        arriveTime = AfterSaleInfo.SkyWays[0].ToDateTime.ToString("HH:mm");
                        flightNumber = AfterSaleInfo.SkyWays[0].FlightNumber;
                    }
                    flightCarrayCode = AfterSaleInfo.SkyWays[0].CarrayCode;
                    carrayShortName = AfterSaleInfo.SkyWays[0].CarrayShortName;
                    // Message = String.Format(@"尊敬的{0}，您预订的{1}{2}{3}({4}) ，于{5}起飞，{6}到达，已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!",
                    //passengerInfo, carrayShortName, flightCarrayCode, flightNumber, toCity, flightTime, arriveTime);
                    //////////////////////////////////////// 
                    var templateStr = SelectedSmsTemplate.TemplateContents;
                    templateStr = templateStr.Replace("[乘客名称]", passengerInfo);
                    templateStr = templateStr.Replace("[航空公司航班号]", carrayShortName + flightCarrayCode + flightNumber);
                    templateStr = templateStr.Replace("[出发城市]", AfterSaleInfo.SkyWays[0].FromCity);
                    templateStr = templateStr.Replace("[出发航站楼]", AfterSaleInfo.SkyWays[0].FromTerminal);
                    templateStr = templateStr.Replace("[到达城市]", AfterSaleInfo.SkyWays[0].ToCity);
                    templateStr = templateStr.Replace("[到达航站楼]", AfterSaleInfo.SkyWays[0].ToTerminal);
                    templateStr = templateStr.Replace("[起飞时间]", flightTime);
                    templateStr = templateStr.Replace("[到达时间]", arriveTime);
                    Message = templateStr;
                }
                else if (AfterSaleInfo.SkyWays != null && AfterSaleInfo.SkyWays.Count == 2)
                {
                    //返程联程
                    var fromCity1 = AfterSaleInfo.SkyWays[0].FromCity;
                    var fromCity2 = AfterSaleInfo.SkyWays[1].ToCity;
                    var toCity1 = AfterSaleInfo.SkyWays[0].FromCity;
                    var toCity2 = AfterSaleInfo.SkyWays[1].ToCity;
                    carrayShortName = AfterSaleInfo.SkyWays[0].CarrayShortName;
                    flightCarrayCode = AfterSaleInfo.SkyWays[0].CarrayCode;
                    toCity = String.Format("{0}-{1}", AfterSaleInfo.SkyWays[0].FromCity + AfterSaleInfo.SkyWays[0].FromTerminal, AfterSaleInfo.SkyWays[0].ToCity + AfterSaleInfo.SkyWays[0].ToTerminal);
                    if (AfterSaleInfo.AfterSaleType == "改签")
                    {
                        var changeOrder = AfterSaleInfo as ResponseChangeOrder;
                        if (changeOrder == null) return;
                        var skyway = changeOrder.SkyWay;
                        flightNumber = skyway[0].NewFlightNumber;
                        flightTime = skyway[0].NewStartDateTime.ToString("yyyy-MM-dd HH:mm");
                        arriveTime = skyway[0].NewToDateTime.ToString("HH:mm");
                    }
                    else
                    {
                        flightNumber = AfterSaleInfo.SkyWays[0].FlightNumber;
                        flightTime = AfterSaleInfo.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                        arriveTime = AfterSaleInfo.SkyWays[0].ToDateTime.ToString("HH:mm"); 
                    }
                    //////////////
                    var carrayShortNameBack = AfterSaleInfo.SkyWays[1].CarrayShortName;
                    var flightCarrayCodeBack = AfterSaleInfo.SkyWays[1].CarrayCode;
                    var flightNumberNameBack = AfterSaleInfo.SkyWays[1].FlightNumber;
                    var toCityBack = String.Format("{0}-{1}",
                        AfterSaleInfo.SkyWays[1].FromCity + AfterSaleInfo.SkyWays[1].FromTerminal,
                        AfterSaleInfo.SkyWays[1].ToCity + AfterSaleInfo.SkyWays[1].ToTerminal);
                    var flightTimeBack = AfterSaleInfo.SkyWays[1].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                    var arriveTimeBack = AfterSaleInfo.SkyWays[1].ToDateTime.ToString("HH:mm");
                    if (fromCity1 == toCity2 && toCity1 == fromCity2)
                    {
                        //往返
                        //Message =
                        //    String.Format(
                        //        @"尊敬的{0}，您预订的{1}{2}{3}({4}) ，于{5}起飞，{6}到达，（回程）{7}{8}{9}({10})，于{11}起飞，{12}到达，已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!",
                        //        passengerInfo, carrayShortName, flightCarrayCode, flightNumber, toCity, flightTime,
                        //        arriveTime, carrayShortNameBack, flightCarrayCodeBack, flightNumberNameBack, toCityBack,
                        //        flightTimeBack, arriveTimeBack);
                        var templateStr = SelectedSmsTemplate.TemplateContents;
                        templateStr = templateStr.Replace("[乘客名称]", passengerInfo);
                        templateStr = templateStr.Replace("[航空公司航班号]", carrayShortName + flightCarrayCode + flightNumber);
                        templateStr = templateStr.Replace("[出发城市]", AfterSaleInfo.SkyWays[0].FromCity);
                        templateStr = templateStr.Replace("[出发航站楼]", AfterSaleInfo.SkyWays[0].FromTerminal);
                        templateStr = templateStr.Replace("[到达城市]", AfterSaleInfo.SkyWays[0].ToCity);
                        templateStr = templateStr.Replace("[到达航站楼]", AfterSaleInfo.SkyWays[0].ToTerminal);
                        templateStr = templateStr.Replace("[起飞时间]", flightTime);
                        templateStr = templateStr.Replace("[到达时间]", arriveTime);

                        templateStr = templateStr.Replace("[回程航空公司航班号]", carrayShortNameBack + flightCarrayCodeBack + flightNumberNameBack);
                        templateStr = templateStr.Replace("[回程出发城市]", AfterSaleInfo.SkyWays[1].FromCity);
                        templateStr = templateStr.Replace("[回程出发航站楼]", AfterSaleInfo.SkyWays[1].FromTerminal);
                        templateStr = templateStr.Replace("[回程到达城市]", AfterSaleInfo.SkyWays[1].ToCity);
                        templateStr = templateStr.Replace("[回程到达航站楼]", AfterSaleInfo.SkyWays[1].ToTerminal);
                        templateStr = templateStr.Replace("[回程起飞时间]", flightTimeBack);
                        templateStr = templateStr.Replace("[回程到达时间]", arriveTimeBack);
                        Message = templateStr;
                    }
                    else
                    {
                        //中转
                        //Message =
                        //  String.Format(
                        //      @"尊敬的{0}，您预订的{1}{2}{3}({4}) ，于{5}起飞，{6}到达，（中转）{7}{8}{9}({10})，于{11}起飞，{12}到达，已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!",
                        //      passengerInfo, carrayShortName, flightCarrayCode, flightNumber, toCity, flightTime,
                        //      arriveTime, carrayShortNameBack, flightCarrayCodeBack, flightNumberNameBack, toCityBack,
                        //      flightTimeBack, arriveTimeBack);
                        var templateStr = SelectedSmsTemplate.TemplateContents;
                        templateStr = templateStr.Replace("[乘客名称]", passengerInfo);
                        templateStr = templateStr.Replace("[航空公司航班号]", carrayShortName + flightCarrayCode + flightNumber);
                        templateStr = templateStr.Replace("[出发城市]", AfterSaleInfo.SkyWays[0].FromCity);
                        templateStr = templateStr.Replace("[出发航站楼]", AfterSaleInfo.SkyWays[0].FromTerminal);
                        templateStr = templateStr.Replace("[到达城市]", AfterSaleInfo.SkyWays[0].ToCity);
                        templateStr = templateStr.Replace("[到达航站楼]", AfterSaleInfo.SkyWays[0].ToTerminal);
                        templateStr = templateStr.Replace("[起飞时间]", flightTime);
                        templateStr = templateStr.Replace("[到达时间]", arriveTime);

                        templateStr = templateStr.Replace("[中转航空公司航班号]", carrayShortNameBack + flightCarrayCodeBack + flightNumberNameBack);
                        templateStr = templateStr.Replace("[中转出发城市]", AfterSaleInfo.SkyWays[1].FromCity);
                        templateStr = templateStr.Replace("[中转出发航站楼]", AfterSaleInfo.SkyWays[1].FromTerminal);
                        templateStr = templateStr.Replace("[中转到达城市]", AfterSaleInfo.SkyWays[1].ToCity);
                        templateStr = templateStr.Replace("[中转到达航站楼]", AfterSaleInfo.SkyWays[1].ToTerminal);
                        templateStr = templateStr.Replace("[回程起飞时间]", flightTimeBack);
                        templateStr = templateStr.Replace("[回程到达时间]", arriveTimeBack);
                        Message = templateStr;
                    }
                }
            }
        }
        /// <summary>
        /// 获取订单乘机人字符串
        /// </summary>
        /// <param name="passenger"></param>
        /// <returns></returns>
        private StringBuilder GetPassengerInfo(IEnumerable<PassengerDto> passenger)
        {
            var sb=new StringBuilder();
            foreach (var m in passenger)
            {
                sb.AppendFormat("{0}({1})、", m.PassengerName, m.CardNo);
            }
            if (sb.Length > 0)
            {
              sb = sb.Remove(sb.Length - 1, 1);
            }
            return sb;
        }
        /// <summary>
        /// 获取售后订单乘机人字符串
        /// </summary>
        /// <param name="passenger"></param>
        /// <returns></returns>
        private StringBuilder GetAfterSalePassengerInfo(IEnumerable<ResponseAfterSalePassenger> passenger)
        {
            var sb = new StringBuilder();
            foreach (var m in passenger)
            {
                sb.AppendFormat("{0}({1})、", m.PassengerName, m.CardNo);
            }
            if (sb.Length > 0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
            }
            return sb;
        }

        #endregion

        /// <summary>
        /// 模板缓存信息
        /// </summary>
        private List<SMSTemplateDto> _listSms = new List<SMSTemplateDto>(); 
        /// <summary>
        /// 售后订单记录
        /// </summary>
        private List<ResponseAfterSaleOrder> _afterSaleOrders = new List<ResponseAfterSaleOrder>();
        /// <summary>
        /// 乘机人缓存信息
        /// </summary>
        private readonly ObservableCollection<PassengerModel> _passengerCahce = new ObservableCollection<PassengerModel>();
        /// <summary>
        /// Load订单相关信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="afterorderId"></param>
        internal void LoadOrderInfo(string orderId,int afterorderId = 0)
        {
            //当售后订单号为空时
            if (afterorderId == 0)
            {
                IsBusy = true;
                Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var aftertemp = service.GetAfterSaleOrderById(orderId);
                    _afterSaleOrders = aftertemp.ToList();
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
            //售后订单号不为空时
            else 
            {
                IsBusy = true;
                Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var aftertemp = service.GetAfterSaleOrderDetail(orderId,afterorderId);
                    if (aftertemp != null)
                        DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                        {
                            AfterSaleInfo = aftertemp;
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
}
