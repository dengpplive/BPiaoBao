using System.Globalization;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using ProjectHelper.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 打印页面视图界面
    /// </summary>
    public class PrintTravelViewModel : BaseVM
    {
        public OrderInfoViewModel OrderInfoViewModel { get; set; }

        public AfterSaleInfoViewModel AfterSaleInfoViewModel { get; set; }

        #region 私有属性

        private string printCfgSection = "print_config_section";
        private IniHelper _iniHelper;
        private string _iniFilePath = Directory.GetCurrentDirectory() + "\\config\\printcfg.ini";
        private PrintDocument _printDocument;

        private BitmapEncoder _bitmapEncoder = null;

        private MemoryStream _memoryStream = new MemoryStream();

        private Bitmap _bt = null;
        /// <summary>
        /// 水平方向移动多少
        /// </summary>
        private int _x;//水平方向移动多少
        /// <summary>
        /// 垂直方向移动多少
        /// </summary>
        private int _y;//垂直方向移动多少

        #endregion

        #region 公共属性
        public OrderDetailDto RspOrder { get; set; }
        public PassengerDto Passenger { get; set; }
        /// <summary>
        /// 订单类型（正常订单：0；售后订单：1或者其他）
        /// </summary>
        public int RFlag { get; set; }
        public ResponseChangeOrder RsAferSaleOrder { get; set; }
        public ResponseAfterSalePassenger RsAfterSalePassenger { get; set; }

        private int _marginLeft = 76;
        private int _marginTop = 63;
        private int _marginRight = 77;
        private int _marginButtom = 35;
        public int MarginLeft
        {
            get { return _marginLeft; }
            set
            {
                _marginLeft = value;
                SetMargin();
            }
        }
        public int MarginTop
        {
            get { return _marginTop; }
            set
            {
                _marginTop = value;
                SetMargin();
            }
        }
        public int MarginRight
        {
            get { return _marginRight; }
            set
            {
                _marginRight = value;
                SetMargin();
            }
        }

        public int MarginButtom
        {
            get { return _marginButtom; }
            set
            {
                _marginButtom = value;
                SetMargin();
            }
        }

        /// <summary>
        /// 设置打印位置
        /// </summary>
        private void SetMargin()
        {
            Margin = string.Format("{0},{1},{2},{3}", MarginLeft, MarginTop, MarginRight, MarginButtom);

        }
        #endregion

        #region 公共方法

        public void Init()
        {
            InitData();
            _iniHelper = new IniHelper(_iniFilePath);
            var x = _iniHelper.ReadIniInfo(printCfgSection, "_x");
            var y = _iniHelper.ReadIniInfo(printCfgSection, "_y");
            if (!string.IsNullOrEmpty(x))
            {
                _x = Convert.ToInt32(x);
                MarginLeft += _x;
                MarginRight -= _x;

            }
            if (string.IsNullOrEmpty(y)) return;
            _y = Convert.ToInt32(y);
            MarginTop += _y;
            MarginButtom -= _y;
        }
        #endregion

        #region 私有方法及事件



        private void InitData()
        {
            if (RFlag == 0)
            {
                if (RspOrder == null || Passenger == null)
                {
                    UIManager.ShowErr(new Exception("无效订单，请重试 "));

                }
                else
                {
                    InitPrint();
                    FlightInfo = InitModel();
                    var list = new List<Note>
                    {
                        new Note {Text = "", Value = "0"},
                        new Note {Text = "不得签转", Value = "1"},
                        new Note {Text = "不得签转更改", Value = "2"},
                        new Note {Text = "不得签转,改退收费", Value = "3"},
                        new Note {Text = "不得签转变更退票", Value = "4"},
                        new Note {Text = "不得签转不得改期不得退票", Value = "5"},
                        new Note {Text = "不得签转,仅限原出票处退票", Value = "6"}
                    };
                    FlightInfo.NoteList = list;
                    var list2 = new List<Note>
                    {
                        new Note {Text = "0.00", Value = "1"},
                        new Note {Text = "5.00", Value = "2"},
                        new Note {Text = "10.00", Value = "3"},
                        new Note {Text = "15.00", Value = "4"},
                        new Note {Text = "20.00", Value = "5"},
                        new Note {Text = "30.00", Value = "6"},
                        new Note {Text = "40.00", Value = "7"}
                    };
                    FlightInfo.InsuranList = list2;
                    FlightInfo.SelectedNote = list[3];
                    FlightInfo.SelectedInsuranNote = list2[0];

                    switch (Passenger.PassengerTripStatus)
                    {
                        case EnumPassengerTripStatus.NoCreate:
                            SelectedTravelPaper = null;
                            IsCreateTravelPaper = true;
                            IsVoidTravelPaper = false;
                            GetTravelNumberList();
                            break;
                        case EnumPassengerTripStatus.HasCreate:
                            SelectedTravelPaper = GetTravelPaperDtoByTravelNumber(Passenger.TravelNumber);
                            if (SelectedTravelPaper != null)
                            {
                                //Logger.WriteLog(LogType.DEBUG, "---------HasCreate1111");
                                AgentCode = SelectedTravelPaper.UseOffice + SelectedTravelPaper.IataCode;
                                IssuedBy = SelectedTravelPaper.TicketCompanyName;
                            }
                            IsCreateTravelPaper = false;
                            IsVoidTravelPaper = true;
                            break;
                        case EnumPassengerTripStatus.HasVoid:
                            SelectedTravelPaper = null;
                            IsCreateTravelPaper = true;
                            IsVoidTravelPaper = false;
                            GetTravelNumberList();
                            break;
                    }
                    //Logger.WriteLog(LogType.DEBUG, "Passenger-->" + Newtonsoft.Json.JsonConvert.SerializeObject(Passenger)); 
                    //Logger.WriteLog(LogType.DEBUG, "Passenger.PassengerTripStatus-->" + Passenger.PassengerTripStatus); 
                    //Logger.WriteLog(LogType.DEBUG, "Passenger.TravelNumber-->" + Passenger.TravelNumber);
                    //Logger.WriteLog(LogType.DEBUG, "IsCreateTravelPaper-->" + IsCreateTravelPaper);
                    Logger.WriteLog(LogType.DEBUG, "IsVoidTravelPaper-->" + IsVoidTravelPaper);
                    //Logger.WriteLog(LogType.DEBUG, "SelectedTravelPaper-->" + Newtonsoft.Json.JsonConvert.SerializeObject(SelectedTravelPaper));
                    System.Diagnostics.Debug.Write("IsVoidTravelPaper-->" + IsVoidTravelPaper);

                }
            }
            else//售后订单详情信息
            {
                if (RsAferSaleOrder == null || RsAfterSalePassenger == null)
                {
                    UIManager.ShowErr(new Exception("无效订单，请重试 "));

                }
                else
                {
                    InitPrint();
                    FlightInfo = InitModel();
                    var list = new List<Note>
                    {
                        new Note {Text = "", Value = "0"},
                        new Note {Text = "不得签转", Value = "1"},
                        new Note {Text = "不得签转更改", Value = "2"},
                        new Note {Text = "不得签转,改退收费", Value = "3"},
                        new Note {Text = "不得签转变更退票", Value = "4"},
                        new Note {Text = "不得签转不得改期不得退票", Value = "5"},
                        new Note {Text = "不得签转,仅限原出票处退票", Value = "6"}
                    };
                    FlightInfo.NoteList = list;
                    var list2 = new List<Note>
                    {
                        new Note {Text = "0.00", Value = "1"},
                        new Note {Text = "5.00", Value = "2"},
                        new Note {Text = "10.00", Value = "3"},
                        new Note {Text = "15.00", Value = "4"},
                        new Note {Text = "20.00", Value = "5"},
                        new Note {Text = "30.00", Value = "6"},
                        new Note {Text = "40.00", Value = "7"}
                    };
                    FlightInfo.InsuranList = list2;
                    FlightInfo.SelectedNote = list[3];
                    FlightInfo.SelectedInsuranNote = list2[0];

                    switch (RsAfterSalePassenger.PassengerTripStatus)
                    {
                        case EnumPassengerTripStatus.NoCreate:
                            IsCreateTravelPaper = true;
                            IsVoidTravelPaper = false;
                            SelectedTravelPaper = null;
                            GetTravelNumberList();
                            break;
                        case EnumPassengerTripStatus.HasCreate:
                            IsCreateTravelPaper = false;
                            IsVoidTravelPaper = true;
                            SelectedTravelPaper = GetTravelPaperDtoByTravelNumber(RsAfterSalePassenger.AfterSaleTravelNum);
                            if (SelectedTravelPaper != null)
                            {
                                AgentCode = SelectedTravelPaper.UseOffice + SelectedTravelPaper.IataCode;
                                IssuedBy = SelectedTravelPaper.TicketCompanyName;
                            }
                            break;
                        case EnumPassengerTripStatus.HasVoid:
                            IsCreateTravelPaper = true;
                            IsVoidTravelPaper = false;
                            SelectedTravelPaper = null;
                            GetTravelNumberList();
                            break;
                    }
                }
            }
        }

        #region 根据行程单号查询行程单详情

        /// <summary>
        /// 根据行程单号查询行程单详情
        /// </summary>
        /// <param name="travelNumber"></param>
        /// <returns></returns>
        private TravelPaperDto GetTravelPaperDtoByTravelNumber(string travelNumber)
        {
            TravelPaperDto result = null;
            CommunicateManager.Invoke<ITravelPaperService>(p =>
            {
                result = p.QueryTripNumberDetail(travelNumber);

            }, UIManager.ShowErr);
            return result;
        }

        #endregion

        #region 查询可用行程单号

        /// <summary>
        /// 查询可用行程单号
        /// </summary>
        private void GetTravelNumberList()
        {
            Action action = () => CommunicateManager.Invoke<ITravelPaperService>(p =>
            {
                var result = p.FindUseTravelPaperDto(LoginInfo.Code);
                TravelPaperList = result.List;
                //TravelPaperList.Insert(0, new TravelPaperDto() { TripNumber = "--请选择行程单号--" }); 
            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () =>
                {
                    IsBusy = false;

                };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        private void InitPrint()
        {

            _printDocument = new PrintDocument
            {
                DefaultPageSettings = { PaperSize = new PaperSize("xcd_print", 933, 402) }
            };
            _printDocument.BeginPrint += _printDocument_BeginPrint;
            _printDocument.PrintPage += _printDocument_PrintPage;
            _printDocument.EndPrint += _printDocument_EndPrint;


        }


        private FlightInfo InitModel()
        {
            var format = "-".ToArray();
            var m = new FlightInfo();
            if (RFlag == 0)
            {
                var order = RspOrder.SkyWays;
                if (order.Count == 2)
                {
                    m.CountFlight = 2;
                    m.FromCityName = new[] { order[0].FromTerminal.TrimEx(format) + order[0].FromCity, order[1].FromTerminal.TrimEx(format) + order[1].FromCity };
                    m.FlightNo = new[] { order[0].CarrayCode + order[0].FlightNumber, order[1].CarrayCode + order[1].FlightNumber };
                    m.CarrayName = new[] { order[0].CarrayShortName, order[1].CarrayShortName };
                    m.SeatCode = new[] { order[0].Seat, order[1].Seat };
                    m.TakeDate = new[] { order[0].StartDateTime.ToString("yyyy-M-d"), order[1].StartDateTime.ToString("yyyy-M-d") };
                    var t1 = order[0].StartDateTime.ToString("yyyy-M-d HH:mm");
                    var t2 = order[1].StartDateTime.ToString("yyyy-M-d HH:mm");
                    var time1 = t1.Split(' ')[1];
                    var time2 = t2.Split(' ')[1];
                    m.TakeTime = new[] { time1, time2 };
                    m.Allow = new[] { "20K", "20K" };
                    m.ToCityName = order[1].ToTerminal.TrimEx(format) + order[1].ToCity;
                    //最终到达时间 
                    var toTime = order[1].ToDateTime.ToString("yyyy-M-d HH:mm");
                    m.ToTime = toTime.Split(' ')[1];
                    /////////////
                    if (order[0].Seat.ToLower().Contains("f"))
                    {
                        m.Allow[0] = "40K";

                    }
                    if (order[1].Seat.ToLower().Contains("f"))
                    {
                        m.Allow[1] = "40K";

                    }
                    if (order[0].Seat.ToLower().Contains("c"))
                    {
                        m.Allow[0] = "30K";

                    }
                    if (order[1].Seat.ToLower().Contains("c"))
                    {
                        m.Allow[1] = "30K";

                    }
                }
                else
                {
                    m.CountFlight = 1;
                    m.FromCityName = new[] { order[0].FromTerminal + order[0].FromCity, "" };
                    m.FlightNo = new[] { order[0].CarrayCode + order[0].FlightNumber, "" };
                    m.CarrayName = new[] { order[0].CarrayShortName, "" };
                    m.SeatCode = new[] { order[0].Seat, "" };
                    m.TakeDate = new[] { order[0].StartDateTime.ToString("yyyy-M-d"), "" };
                    var t1 = order[0].StartDateTime.ToString("yyyy-M-d HH:mm");
                    var time1 = t1.Split(' ')[1];
                    m.TakeTime = new[] { time1, "" };
                    m.Allow = new[] { "20K", "" };
                    m.ToCityName = order[0].ToTerminal.TrimEx(format) + order[0].ToCity;
                    var toTime = order[0].ToDateTime.ToString("yyyy-M-d HH:mm");
                    // m.ToTime = toTime.Split(' ')[1];
                    m.TakeTime[1] = toTime.Split(' ')[1];
                    m.FromCityName[1] = m.ToCityName;
                    m.ToCityName = "";
                    ///////////////// 
                    if (order[0].Seat.ToLower().Contains("f"))
                    {
                        m.Allow[0] = "40K";
                    }
                    else if (order[0].Seat.ToLower().Contains("c"))
                    {
                        m.Allow[0] = "30K";
                    }
                    
                }

                m.AgentCode = "";//"CTU18608024332";
                m.ETicketNo = Passenger.TicketNumber;
                m.ID_NO = Passenger.CardNo;
                //m.Insuran = "￥" + (Passenger.BuyInsuranceCount * Passenger.BuyInsurancePrice).ToString("F2");//保险
                m.IssuedBy = "成都华龙航空服务有限公司"; //RspOrder.BusinessmanName;
                m.IssuedDate = DateTime.Now.ToString("yyyy-M-d");
                m.Pnr = RspOrder.PnrCode;
                // m.Note = SelectedNote == null ? "不得签转变更退票" : SelectedNote.Text;
                m.OtherPrice = "0.00";
                m.PassengerName = Passenger.PassengerName;

                m.RQFee = "YQ" + Passenger.RQFee.ToString("F2");
                m.SeatPrice = "CNY" + Passenger.SeatPrice.ToString("F2");
                m.TaxFee = "CN" + Passenger.TaxFee.ToString("F2");
                //m.TotalPrice = "CNY " + RspOrder.OrderMoney.ToString("F2");
                m.TotalPrice = "CNY" + (Passenger.RQFee + Passenger.SeatPrice + Passenger.TaxFee).ToString("F2");
                m.CK = "";//验证码   
            }
            else//售后详情订单
            {
                var order = RsAferSaleOrder.SkyWay; var oldorder = RsAferSaleOrder.SkyWays; //有部分信息是原航段的信息
                if (oldorder.Count == 2)
                {
                    m.CountFlight = 2;
                    switch (order.Count)
                    {
                        case 1:
                            if (order[0].Id == oldorder[0].SkyWayId)
                            {
                                m.FromCityName = new[] { oldorder[0].FromTerminal.TrimEx(format) + oldorder[0].FromCity, oldorder[1].FromTerminal.TrimEx(format) + oldorder[1].FromCity };
                                m.FlightNo = new[] { oldorder[0].CarrayCode + order[0].NewFlightNumber, oldorder[1].CarrayCode + oldorder[1].FlightNumber };
                                m.CarrayName = new[] { oldorder[0].CarrayShortName, oldorder[0].CarrayShortName };
                                m.SeatCode = new[] { oldorder[0].Seat, order[0].NewSeat };
                                m.TakeDate = new[] { oldorder[0].StartDateTime.ToString("yyyy-M-d"), order[0].NewStartDateTime.ToString("yyyy-M-d") };
                                var t1 = oldorder[0].StartDateTime.ToString("yyyy-M-d HH:mm");
                                var t2 = order[0].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                                var time1 = t1.Split(' ')[1];
                                var time2 = t2.Split(' ')[1];
                                m.TakeTime = new[] { time1, time2 };
                                m.Allow = new[] { "20K", "20K" };
                                m.ToCityName = oldorder[0].ToTerminal.TrimEx(format) + oldorder[0].ToCity;
                                //最终到达时间 
                                var toTime = oldorder[0].ToDateTime.ToString("yyyy-M-d HH:mm");
                                m.ToTime = toTime.Split(' ')[1];
                            }
                            else
                            {
                                m.FromCityName = new[] { oldorder[0].FromTerminal.TrimEx(format) + oldorder[0].FromCity, oldorder[1].FromTerminal.TrimEx(format) + oldorder[1].FromCity };
                                m.FlightNo = new[] { oldorder[0].CarrayCode + order[0].NewFlightNumber, oldorder[1].CarrayCode + oldorder[1].FlightNumber };
                                m.CarrayName = new[] { oldorder[1].CarrayShortName, oldorder[1].CarrayShortName };
                                m.SeatCode = new[] { order[0].NewSeat, oldorder[1].Seat };
                                m.TakeDate = new[] { order[0].NewStartDateTime.ToString("yyyy-M-d"), oldorder[1].StartDateTime.ToString("yyyy-M-d") };
                                var t1 = order[0].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                                var t2 = oldorder[1].StartDateTime.ToString("yyyy-M-d HH:mm");
                                var time1 = t1.Split(' ')[1];
                                var time2 = t2.Split(' ')[1];
                                m.TakeTime = new[] { time1, time2 };
                                m.Allow = new[] { "20K", "20K" };
                                m.ToCityName = oldorder[1].ToTerminal.TrimEx(format) + oldorder[1].ToCity;
                                //最终到达时间 
                                var toTime = oldorder[1].ToDateTime.ToString("yyyy-M-d HH:mm");
                                m.ToTime = toTime.Split(' ')[1];
                            }
                            break;
                        case 2:
                            {
                                m.FromCityName = new[] { oldorder[0].FromTerminal.TrimEx(format) + oldorder[0].FromCity, oldorder[1].FromTerminal.TrimEx(format) + oldorder[1].FromCity };
                                m.FlightNo = new[] { oldorder[0].CarrayCode + order[0].NewFlightNumber, oldorder[1].CarrayCode + order[1].NewFlightNumber };
                                m.CarrayName = new[] { oldorder[0].CarrayShortName, oldorder[1].CarrayShortName };
                                m.SeatCode = new[] { order[0].NewSeat, order[1].NewSeat };
                                m.TakeDate = new[] { order[0].NewStartDateTime.ToString("yyyy-M-d"), order[1].NewStartDateTime.ToString("yyyy-M-d") };
                                var t1 = order[0].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                                var t2 = order[1].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                                var time1 = t1.Split(' ')[1];
                                var time2 = t2.Split(' ')[1];
                                m.TakeTime = new[] { time1, time2 };
                                m.Allow = new[] { "20K", "20K" };
                                m.ToCityName = oldorder[1].ToTerminal.TrimEx(format) + oldorder[1].ToCity;
                                //最终到达时间 
                                var toTime = order[1].NewToDateTime.ToString("yyyy-M-d HH:mm");
                                m.ToTime = toTime.Split(' ')[1];
                            }
                            break;
                    }
                    //m.CountFlight = 2;
                    //m.FromCityName = new string[] { oldorder[0].FromTerminal.TrimEx(format) + oldorder[0].FromCity, oldorder[1].FromTerminal.TrimEx(format) + oldorder[1].FromCity };
                    //m.FlightNo = new string[] { oldorder[0].CarrayCode + order[0].NewFlightNumber, oldorder[1].CarrayCode + order[1].NewFlightNumber };
                    //m.CarrayName = new string[] { oldorder[0].CarrayShortName, oldorder[1].CarrayShortName };
                    //m.SeatCode = new string[] { order[0].NewSeat, order[1].NewSeat };
                    //m.TakeDate = new string[] { order[0].NewStartDateTime.ToString("yyyy-M-d"), order[1].NewStartDateTime.ToString("yyyy-M-d") };
                    //var t1 = order[0].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                    //var t2 = order[1].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                    //var time1 = t1.Split(' ')[1];
                    //var time2 = t2.Split(' ')[1];
                    //m.TakeTime = new string[] { time1, time2 };
                    //m.Allow = new string[] { "20K", "20K" };
                    //m.ToCityName = oldorder[1].ToTerminal.TrimEx(format) + oldorder[1].ToCity;
                    ////最终到达时间 
                    //var toTime = order[1].NewToDateTime.ToString("yyyy-M-d HH:mm");
                    //m.ToTime = toTime.Split(' ')[1];
                }
                else
                {
                    m.CountFlight = 1;
                    m.FromCityName = new[] { oldorder[0].FromTerminal + oldorder[0].FromCity, "" };
                    m.FlightNo = new[] { oldorder[0].CarrayCode + order[0].NewFlightNumber, "" };
                    m.CarrayName = new[] { oldorder[0].CarrayShortName, "" };
                    m.SeatCode = new[] { order[0].NewSeat, "" };
                    m.TakeDate = new[] { order[0].NewStartDateTime.ToString("yyyy-M-d"), "" };
                    var t1 = order[0].NewStartDateTime.ToString("yyyy-M-d HH:mm");
                    var time1 = t1.Split(' ')[1];
                    m.TakeTime = new[] { time1, "" };
                    m.Allow = new[] { "20K", "" };
                    m.ToCityName = oldorder[0].ToTerminal.TrimEx(format) + oldorder[0].ToCity;
                    var toTime = order[0].NewToDateTime.ToString("yyyy-M-d HH:mm");
                    // m.ToTime = toTime.Split(' ')[1];
                    m.TakeTime[1] = toTime.Split(' ')[1];
                    m.FromCityName[1] = m.ToCityName;
                    m.ToCityName = "";
                }

                m.AgentCode = "";//"CTU18608024332";
                m.ETicketNo = RsAfterSalePassenger.AfterSaleTravelTicketNum;
                m.ID_NO = RsAfterSalePassenger.CardNo;
                //m.Insuran = "xxx";//保险
                m.IssuedBy = "成都华龙航空服务有限公司"; //RspOrder.BusinessmanName;
                m.IssuedDate = DateTime.Now.ToString("yyyy-M-d");
                m.Pnr = RsAferSaleOrder.PNR;
                // m.Note = SelectedNote == null ? "不得签转变更退票" : SelectedNote.Text;
                m.OtherPrice = "0.00";
                m.PassengerName = RsAfterSalePassenger.PassengerName;

                m.RQFee = "YQ" + RsAfterSalePassenger.RQFee.ToString("F2");
                m.SeatPrice = "CNY" + RsAfterSalePassenger.SeatPrice.ToString("F2");
                m.TaxFee = "CN" + RsAfterSalePassenger.ABFee.ToString("F2");
                //m.TotalPrice = "CNY " + RspOrder.OrderMoney.ToString("F2");
                m.TotalPrice = "CNY" + (RsAfterSalePassenger.RQFee + RsAfterSalePassenger.SeatPrice + RsAfterSalePassenger.ABFee).ToString("F2");
                m.CK = "";//验证码   
            }
            return m;

        }
        void _printDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            //打印前准备
        }
        void _printDocument_EndPrint(object sender, PrintEventArgs e)
        {
            //打印后

        }

        void _printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            //打印中...  
            Drawing(FlightInfo, e);
        }

        /// <summary>
        /// 打印动作
        /// </summary>
        /// <param name="info"></param>
        /// <param name="e"></param>
        private void Drawing(FlightInfo info, PrintPageEventArgs e)
        {
            if (info == null)
            {
                return;
            }
            var font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Regular);

            e.Graphics.DrawString(info.PassengerName, font, Brushes.Black, 20 + _x, 80 + _y);
            e.Graphics.DrawString(info.ID_NO, font, Brushes.Black, 180 + _x, 80 + _y);
            e.Graphics.DrawString(info.SelectedNote.Text, font, Brushes.Black, 440 + _x, 80 + _y);

            e.Graphics.DrawString(info.Pnr, font, Brushes.Black, 20 + _x, 105 + _y);

            e.Graphics.DrawString(info.FromCityName[0], font, Brushes.Black, 30 + _x, 145 + _y);
            e.Graphics.DrawString(info.CarrayName[0], font, Brushes.Black, 140 + _x, 145 + _y);
            e.Graphics.DrawString(info.FlightNo[0], font, Brushes.Black, 183 + _x, 145 + _y);
            e.Graphics.DrawString(info.SeatCode[0], font, Brushes.Black, 250 + _x, 145 + _y);
            e.Graphics.DrawString(info.TakeDate[0], font, Brushes.Black, 280 + _x, 145 + _y);
            e.Graphics.DrawString(info.TakeTime[0], font, Brushes.Black, 390 + _x, 145 + _y);
            e.Graphics.DrawString(info.SeatCode[0], font, Brushes.Black, 480 + _x, 145 + _y);
            e.Graphics.DrawString(info.Allow[0], font, Brushes.Black, 700 + _x, 150 + _y);
            e.Graphics.DrawString(info.FromCityName[1], font, Brushes.Black, 30 + _x, 180 + _y);

            if (info.CountFlight == 1)
            {


                e.Graphics.DrawString("VOID", font, Brushes.Black, 183 + _x, 183 + _y);
                e.Graphics.DrawString(info.TakeTime[1], font, Brushes.Black, 390 + _x, 180 + _y);
                e.Graphics.DrawString("VOID", font, Brushes.Black, 30 + _x, 215 + _y);

            }
            else if (info.CountFlight == 2)
            {

                e.Graphics.DrawString(info.CarrayName[1], font, Brushes.Black, 140 + _x, 180 + _y);
                e.Graphics.DrawString(info.FlightNo[1], font, Brushes.Black, 183 + _x, 180 + _y);
                e.Graphics.DrawString(info.SeatCode[1], font, Brushes.Black, 250 + _x, 180 + _y);
                e.Graphics.DrawString(info.TakeDate[1], font, Brushes.Black, 280 + _x, 180 + _y);
                e.Graphics.DrawString(info.TakeTime[1], font, Brushes.Black, 390 + _x, 180 + _y);
                e.Graphics.DrawString(info.SeatCode[1], font, Brushes.Black, 480 + _x, 180 + _y);
                e.Graphics.DrawString(info.Allow[1], font, Brushes.Black, 700 + _x, 180 + _y);

                e.Graphics.DrawString(info.ToCityName, font, Brushes.Black, 30 + _x, 215 + _y);
                e.Graphics.DrawString("VOID", font, Brushes.Black, 183 + _x, 215 + _y);
                e.Graphics.DrawString(info.ToTime, font, Brushes.Black, 390 + _x, 215 + _y);
                e.Graphics.DrawString("VOID", font, Brushes.Black, 30 + _x, 245 + _y);
            }

            e.Graphics.DrawString(info.SeatPrice, font, Brushes.Black, 138 + _x, 278 + _y);
            e.Graphics.DrawString(info.TaxFee, font, Brushes.Black, 250 + _x, 278 + _y);
            e.Graphics.DrawString(info.RQFee, font, Brushes.Black, 360 + _x, 278 + _y);
            e.Graphics.DrawString(info.OtherPrice, font, Brushes.Black, 460 + _x, 278 + _y);
            e.Graphics.DrawString(info.TotalPrice, font, Brushes.Black, 580 + _x, 278 + _y);

            e.Graphics.DrawString(info.ETicketNo, font, Brushes.Black, 45 + _x, 308 + _y);
            e.Graphics.DrawString(info.CK, font, Brushes.Black, 240 + _x, 308 + _y);
            e.Graphics.DrawString(info.TipInfo, font, Brushes.Black, 380 + _x, 308 + _y);
            e.Graphics.DrawString("￥" + info.SelectedInsuranNote.Text, font, Brushes.Black, 670 + _x, 308 + _y);


            if (info.AgentCode.Length > 6)
            {
                var temp1 = info.AgentCode.ToUpper().Substring(0, 6);
                var temp2 = info.AgentCode.ToUpper().Substring(6);
                var font1 = new Font(FontFamily.GenericMonospace, 10, FontStyle.Bold);
                e.Graphics.DrawString(temp1, font1, Brushes.Black, 55 + _x, 335 + _y);
                e.Graphics.DrawString(temp2, font1, Brushes.Black, 45 + _x, 345 + _y);

            }
            else
            {
                e.Graphics.DrawString(info.AgentCode.ToUpper(), font, Brushes.Black, 45 + _x, 340 + _y);

            }


            e.Graphics.DrawString(info.IssuedBy, font, Brushes.Black, 250 + _x, 340 + _y);
            e.Graphics.DrawString(info.IssuedDate, font, Brushes.Black, 630 + _x, 340 + _y);



        }
        #endregion

        #region 依赖属性

        #region IsCreated

        /// <summary>
        /// The <see cref="IsCreated" /> property's name.
        /// </summary>
        private const string IsCreatedPropertyName = "IsCreated";

        private bool _isCreated;

        /// <summary>
        /// 是否已经创建
        /// </summary>
        public bool IsCreated
        {
            get { return _isCreated; }

            set
            {
                if (_isCreated == value) return;

                RaisePropertyChanging(IsCreatedPropertyName);
                _isCreated = value;
                RaisePropertyChanged(IsCreatedPropertyName);
            }
        }

        #endregion

        #region MarginProtertyName

        private const string MarginProtertyName = "Margin";
        private string _margin = "76,63,77,35";

        public string Margin
        {
            get { return _margin; }
            set
            {
                if (_margin == value)
                {
                    return;
                }
                RaisePropertyChanging(MarginProtertyName);
                _margin = value;
                RaisePropertyChanged(MarginProtertyName);

            }
        }
        #endregion

        #region SelectedTravelPaperProtertyName

        private const string SelectedTravelPaperProtertyName = "SelectedTravelPaper";
        private TravelPaperDto _selectedTravelPaper;

        public TravelPaperDto SelectedTravelPaper
        {
            get { return _selectedTravelPaper; }
            set
            {
                if (_selectedTravelPaper == value)
                {
                    return;
                }
                RaisePropertyChanging(SelectedTravelPaperProtertyName);
                _selectedTravelPaper = value;
                RaisePropertyChanged(SelectedTravelPaperProtertyName);

                if (_selectedTravelPaper == null)
                {
                    AgentCode = "";
                    IssuedBy = "";
                    Ck = "";
                    TripNumber = "";
                }
                else
                {
                    TripNumber = _selectedTravelPaper.TripNumber;
                    AgentCode = _selectedTravelPaper.UseOffice + _selectedTravelPaper.IataCode;
                    IssuedBy = _selectedTravelPaper.TicketCompanyName;
                    if (_selectedTravelPaper.TripNumber != null && _selectedTravelPaper.TripNumber.Length > 4)
                    {
                        var start = _selectedTravelPaper.TripNumber.Length - 4;
                        Ck = _selectedTravelPaper.TripNumber.Substring(start);
                    }
                    System.Diagnostics.Debug.Write("_selectedTravelPaper->TripNumper:" + _selectedTravelPaper.TripNumber);
                    System.Diagnostics.Debug.Write("TripNumper:" + TripNumber);
                }

            }
        }
        #endregion

        #region TravelPaperListProtertyName

        private const string TravelPaperListProtertyName = "TravelPaperList";
        private List<TravelPaperDto> _travelPaperList;

        public List<TravelPaperDto> TravelPaperList
        {
            get { return _travelPaperList; }
            set
            {
                if (_travelPaperList == value)
                {
                    return;
                }
                RaisePropertyChanging(TravelPaperListProtertyName);
                _travelPaperList = value;
                RaisePropertyChanged(TravelPaperListProtertyName);
            }
        }
        #endregion

        #region FlightInfoProtertyName

        private const string FlightInfoProtertyName = "FlightInfo";
        private FlightInfo _flightInfo;

        public FlightInfo FlightInfo
        {
            get { return _flightInfo; }
            set
            {
                if (_flightInfo == value)
                {
                    return;
                }
                RaisePropertyChanging(FlightInfoProtertyName);
                _flightInfo = value;
                RaisePropertyChanged(FlightInfoProtertyName);
            }
        }
        #endregion

        #region AgentCodeProtertyName

        private const string AgentCodeProtertyName = "AgentCode";
        private string _agentCode;

        public string AgentCode
        {
            get { return _agentCode; }
            set
            {
                if (_agentCode == value)
                {
                    return;
                }
                RaisePropertyChanging(AgentCodeProtertyName);
                _agentCode = value;
                RaisePropertyChanged(AgentCodeProtertyName);
                FlightInfo.AgentCode = value;
            }
        }
        #endregion

        #region IssuedByProtertyName

        private const string IssuedByProtertyName = "IssuedBy";
        private string _issuedBy;

        public string IssuedBy
        {
            get { return _issuedBy; }
            set
            {
                if (_issuedBy == value)
                {
                    return;
                }
                RaisePropertyChanging(IssuedByProtertyName);
                _issuedBy = value;
                RaisePropertyChanged(IssuedByProtertyName);
                FlightInfo.IssuedBy = value;
            }
        }
        #endregion

        #region CKProtertyName

        private const string CkProtertyName = "CK";
        private string _cK;
        public string Ck
        {
            get { return _cK; }
            set
            {
                if (_cK == value)
                {
                    return;
                }
                RaisePropertyChanging(CkProtertyName);
                _cK = value;
                RaisePropertyChanged(CkProtertyName);
                FlightInfo.CK = value;
            }
        }
        #endregion

        #region IsCreateTravelPaperProtertyName

        private const string IsCreateTravelPaperProtertyName = "IsCreateTravelPaper";
        private bool _isCreateTravelPaper;

        public bool IsCreateTravelPaper
        {
            get { return _isCreateTravelPaper; }
            set
            {
                if (_isCreateTravelPaper == value)
                {
                    return;
                }
                RaisePropertyChanging(IsCreateTravelPaperProtertyName);
                _isCreateTravelPaper = value;
                RaisePropertyChanged(IsCreateTravelPaperProtertyName);
            }
        }
        #endregion

        #region IsVoidTravelPaperProtertyName

        private const string IsVoidTravelPaperProtertyName = "IsVoidTravelPaper";
        private bool _isVoidTravelPaper;

        public bool IsVoidTravelPaper
        {
            get { return _isVoidTravelPaper; }
            set
            {
                if (_isVoidTravelPaper == value)
                {
                    return;
                }
                RaisePropertyChanging(IsVoidTravelPaperProtertyName);
                _isVoidTravelPaper = value;
                RaisePropertyChanged(IsVoidTravelPaperProtertyName);
            }
        }
        #endregion


        #region TripNumberProtertyName

        private const string TripNumberProtertyName = "TripNumber";
        private string _tripNumber;

        public string TripNumber
        {
            get { return _tripNumber; }
            set
            {
                if (_tripNumber == value)
                {
                    return;
                }
                RaisePropertyChanging(TripNumberProtertyName);
                _tripNumber = value;
                RaisePropertyChanged(TripNumberProtertyName);
            }
        }
        #endregion

        #endregion

        #region 公开命令

        #region PrintTravelCommand

        private RelayCommand _printTravelCommand;

        public RelayCommand PrintTravelCommand
        {
            get
            {
                return _printTravelCommand ??
                       (new RelayCommand(ExceutePrintTravelCommand, CanExceutePrintTravelCommand));
            }
        }

        private void ExceutePrintTravelCommand()
        {
            //var scope = new ManagementScope(@"\root\cimv2");
            //scope.Connect(); 
            //var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            //int co=searcher.Get().Count;
            //System.Diagnostics.Debug.Write("printer Count:"+co);
            //if (co == 0)
            //{
            //    UIManager.ShowMessageDialog("未连接任何打印机");
            //}

            if (_printDocument == null) return;
            try
            {
                if (_printDocument.PrinterSettings.IsValid)
                {
                    _printDocument.Print();

                }
                else
                {
                    UIManager.ShowMessage("未找到任何打印机");
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                throw new CustomException(100002, ex.Message);
            }
        }

        private bool CanExceutePrintTravelCommand()
        {
            return true;
        }
        #endregion

        #region SaveOrExpireCommand

        private RelayCommand<string> _saveOrExpireCommand;

        public RelayCommand<string> SaveOrExpireCommand
        {
            get
            {
                return _saveOrExpireCommand ??
                       (new RelayCommand<string>(ExceuteSaveOrExpireCommand, CanExceuteSaveOrExpireCommand));
            }
        }

        private void ExceuteSaveOrExpireCommand(string flag)
        {
            if (SelectedTravelPaper == null)
            {
                UIManager.ShowMessage("请选择行程单号");
                return;
            }
            switch (flag)
            {
                case "1":
                    {
                        //创建
                        IsBusy = true;
                        Action action = () => CommunicateManager.Invoke<ITravelPaperService>(service =>
                        {
                            var req = new TravelAppRequrst { Flag = RFlag, TripNumber = SelectedTravelPaper.TripNumber };
                            if (RFlag == 0)
                            {
                                req.TicketNumber = Passenger.TicketNumber;
                            }
                            else
                            {
                                req.TicketNumber = string.IsNullOrEmpty(RsAfterSalePassenger.AfterSaleTravelTicketNum) ? RsAfterSalePassenger.TicketNumber : RsAfterSalePassenger.AfterSaleTravelTicketNum;
                            }
                            req.OrderId = RFlag == 0 ? RspOrder.OrderId : RsAferSaleOrder.Id.ToString(CultureInfo.InvariantCulture);
                            req.PassengerId = RFlag == 0 ? Passenger.Id : RsAfterSalePassenger.AfterPassengerID;
                            req.CreateOffice = SelectedTravelPaper.UseOffice;
                            var response = service.CreateTrip(req);
                            if (response.IsSuc)
                            {
                                UIManager.ShowMessage("创建行程单号成功");
                                IsCreateTravelPaper = false;
                                IsVoidTravelPaper = true;
                                // IsCreated = true;
                                if (OrderInfoViewModel != null)
                                {
                                    OrderInfoViewModel.Initialize();
                                }
                                if (AfterSaleInfoViewModel != null)
                                {
                                    AfterSaleInfoViewModel.Initialize();
                                }
                            }
                            else
                            {
                                UIManager.ShowMessage(response.ShowMsg);
                            }
                        }, UIManager.ShowErr);
                        Task.Factory.StartNew(action).ContinueWith(task =>
                        {
                            Action setBusyAction = () => { IsBusy = false; };
                            DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                        });
                    }
                    break;
                //case "0":
                //{
                //    //作废
                //    IsBusy = true;
                //    Action action = () => CommunicateManager.Invoke<ITravelPaperService>(service =>
                //    {
                //        var req = new TravelAppRequrst {Flag = RFlag, TripNumber = SelectedTravelPaper.TripNumber};
                //        if (RFlag == 0)
                //        {
                //            req.TicketNumber = Passenger.TicketNumber;
                //        }
                //        else
                //        {
                //            req.TicketNumber = string.IsNullOrEmpty(RsAfterSalePassenger.AfterSaleTravelTicketNum) ? RsAfterSalePassenger.TicketNumber : RsAfterSalePassenger.AfterSaleTravelTicketNum;
                //        }
                //        req.CreateOffice = SelectedTravelPaper.UseOffice;
                //        req.OrderId = RFlag == 0 ? RspOrder.OrderId : RsAferSaleOrder.Id.ToString(CultureInfo.InvariantCulture);
                //        req.PassengerId = RFlag == 0 ? Passenger.Id : RsAfterSalePassenger.AfterPassengerID;
                //        var response = service.VoidTrip(req);
                //        if (response.IsSuc)
                //        {
                //            UIManager.ShowMessage("作废行程单号成功");
                //            IsCreateTravelPaper = true;
                //            IsVoidTravelPaper = false;
                //            SelectedTravelPaper = null;
                //            GetTravelNumberList();
                //            if (OrderInfoViewModel != null)
                //            {
                //                OrderInfoViewModel.Initialize();
                //            }
                //            if (AfterSaleInfoViewModel != null)
                //            {
                //                AfterSaleInfoViewModel.Initialize();
                //            }

                //        }
                //        else
                //        {
                //            UIManager.ShowMessage(response.ShowMsg);
                //        }
                //    }, UIManager.ShowErr);
                //    Task.Factory.StartNew(action).ContinueWith(task =>
                //    {
                //        Action setBusyAction = () =>
                //        {
                //            IsBusy = false;

                //        };
                //        DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                //    });
                //}
                //    break;
            }
        }

        private bool CanExceuteSaveOrExpireCommand(string flag)
        {
            return SelectedTravelPaper != null && flag != "" && !IsBusy;
        }

        #endregion

        #region SaveOrExpireCommand2

        private RelayCommand<string> _saveOrExpireCommand2;

        public RelayCommand<string> SaveOrExpireCommand2
        {
            get
            {
                return _saveOrExpireCommand2 ??
                       (new RelayCommand<string>(ExceuteSaveOrExpireCommand2, CanExceuteSaveOrExpireCommand2));
            }
        }

        private void ExceuteSaveOrExpireCommand2(string flag)
        {
            if (SelectedTravelPaper == null)
            {
                UIManager.ShowMessage("请选择行程单号");
                return;
            }
            switch (flag)
            {
                case "0":
                    {
                        //作废
                        IsBusy = true;
                        Action action = () => CommunicateManager.Invoke<ITravelPaperService>(service =>
                        {
                            var req = new TravelAppRequrst { Flag = RFlag, TripNumber = SelectedTravelPaper.TripNumber };
                            if (RFlag == 0)
                            {
                                req.TicketNumber = Passenger.TicketNumber;
                            }
                            else
                            {
                                req.TicketNumber = string.IsNullOrEmpty(RsAfterSalePassenger.AfterSaleTravelTicketNum) ? RsAfterSalePassenger.TicketNumber : RsAfterSalePassenger.AfterSaleTravelTicketNum;
                            }
                            req.CreateOffice = SelectedTravelPaper.UseOffice;
                            req.OrderId = RFlag == 0 ? RspOrder.OrderId : RsAferSaleOrder.Id.ToString(CultureInfo.InvariantCulture);
                            req.PassengerId = RFlag == 0 ? Passenger.Id : RsAfterSalePassenger.AfterPassengerID;
                            var response = service.VoidTrip(req);
                            if (response.IsSuc)
                            {
                                UIManager.ShowMessage("作废行程单号成功");
                                IsCreateTravelPaper = true;
                                IsVoidTravelPaper = false;
                                SelectedTravelPaper = null;
                                GetTravelNumberList();
                                if (OrderInfoViewModel != null)
                                {
                                    OrderInfoViewModel.Initialize();
                                }
                                if (AfterSaleInfoViewModel != null)
                                {
                                    AfterSaleInfoViewModel.Initialize();
                                }

                            }
                            else
                            {
                                UIManager.ShowMessage(response.ShowMsg);
                            }
                        }, UIManager.ShowErr);
                        Task.Factory.StartNew(action).ContinueWith(task =>
                        {
                            Action setBusyAction = () =>
                            {
                                IsBusy = false;

                            };
                            DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
                        });
                    }
                    break;
            }
        }

        private bool CanExceuteSaveOrExpireCommand2(string flag)
        {
            return SelectedTravelPaper != null && flag != "" && !IsBusy;
        }

        #endregion

        #region SetPrintLocationCommand

        private RelayCommand<string> _setPrintLocationCommand;

        public RelayCommand<string> SetPrintLocationCommand
        {
            get
            {
                return _setPrintLocationCommand ??
                       (new RelayCommand<string>(ExceuteSetPrintLocationCommand, CanExceuteSetPrintLocationCommand));
            }
        }

        private void ExceuteSetPrintLocationCommand(string flag)
        {
            switch (flag)
            {
                case "up":
                    _y--;
                    MarginTop += -1;
                    MarginButtom += 1;
                    break;

                case "down":
                    _y++;
                    MarginButtom += -1;
                    MarginTop += 1;
                    break;

                case "left":
                    _x--;
                    MarginLeft += -1;
                    MarginRight += 1;
                    break;

                case "right":
                    _x++;
                    MarginRight += -1;
                    MarginLeft += 1;
                    break;
            }
            _iniHelper.WriteInfo(printCfgSection, "_x", _x.ToString(CultureInfo.InvariantCulture));
            _iniHelper.WriteInfo(printCfgSection, "_y", _y.ToString(CultureInfo.InvariantCulture));

        }

        private bool CanExceuteSetPrintLocationCommand(string flag)
        {
            return true;
        }

        #endregion

        #endregion


    }

    public class Note
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    #region 类FlightInfo
    public class FlightInfo
    {
        public List<Note> NoteList { get; set; }
        public List<Note> InsuranList { get; set; }
        public int CountFlight { get; set; }
        public string[] Allow { get; set; }
        public string PassengerName { get; set; }
        public string ID_NO { get; set; }
        public Note SelectedNote { get; set; }
        public string Pnr { get; set; }
        public string[] FromCityName { get; set; }
        public string[] CarrayName { get; set; }
        public string[] FlightNo { get; set; }
        public string[] SeatCode { get; set; }
        public string[] TakeDate { get; set; }
        public string[] TakeTime { get; set; }
        public string ToCityName { get; set; }
        public string ToTime { get; set; }
        public string SeatPrice { get; set; }
        public string TaxFee { get; set; }
        public string RQFee { get; set; }
        public string OtherPrice { get; set; }
        public string TotalPrice { get; set; }

        public string ETicketNo { get; set; }
        public Note SelectedInsuranNote { get; set; }
        public string Insuran { get; set; }

        public string AgentCode { get; set; }
        public string IssuedBy { get; set; }
        public string IssuedDate { get; set; }

        public string CK { get; set; }


        public string TipInfo { get; set; }
    }
    #endregion
}
