using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework.Expand;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 机票总表视图模型
    /// </summary>
    public class TicketTableViewModel : OrderManagerViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="TicketTableViewModel"/> class.
        /// </summary>
        public TicketTableViewModel()
        { 
            //EndCreateTime = DateTime.Now;
            //CreateTime = EndCreateTime.Value.Date;
            PageSize = 50;
            SelectedOrderStatus = EnumClientOrderStatus.IssueAndCompleted;//暂时写死
            //机票状态
            _allTicketStatus.Add(new KeyValuePair<EnumTicketStatus?, string>(null, "请选择"));
            _allTicketStatus.Add(new KeyValuePair<EnumTicketStatus?, string>(EnumTicketStatus.IssueTicket, EnumTicketStatus.IssueTicket.ToEnumDesc()));
            _allTicketStatus.Add(new KeyValuePair<EnumTicketStatus?, string>(EnumTicketStatus.Refound, EnumTicketStatus.Refound.ToEnumDesc()));
            _allTicketStatus.Add(new KeyValuePair<EnumTicketStatus?, string>(EnumTicketStatus.Annul, EnumTicketStatus.Annul.ToEnumDesc()));
            _allTicketStatus.Add(new KeyValuePair<EnumTicketStatus?, string>(EnumTicketStatus.Change, EnumTicketStatus.Change.ToEnumDesc()));
            Initialize();

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性

        #region CreateTime

        /// <summary>
        /// The <see cref="CreateTime" /> property's name.
        /// </summary>
        public const string CreateTimePropertyName = "CreateTime";

        private DateTime? createTime = null;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get { return createTime; }

            set
            {
                if (createTime == value && value != null) return;
                else if (value == null && EndCreateTime != null) value = EndCreateTime.Value.AddMonths(-1);
                RaisePropertyChanging(CreateTimePropertyName);
                createTime = value;
                RaisePropertyChanged(CreateTimePropertyName);
            }
        }

        #endregion

        #region EndCreateTime

        /// <summary>
        /// The <see cref="EndCreateTime" /> property's name.
        /// </summary>
        public const string EndCreateTimePropertyName = "EndCreateTime";

        private DateTime? endCreateTime = null;

        /// <summary>
        /// 创建结束时间
        /// </summary>
        public new DateTime? EndCreateTime
        {
            get { return endCreateTime; }

            set
            {
                if (endCreateTime == value) return;
                if (value == null) value = DateTime.Now.Date;

                value = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day);
                //结尾时间修改为当天结尾
                value = value.Value.Add(TimeSpan.FromDays(1)).Add(TimeSpan.FromMilliseconds(-1));

                RaisePropertyChanging(EndCreateTimePropertyName);
                endCreateTime = value;
                RaisePropertyChanged(EndCreateTimePropertyName);
            }
        }

        #endregion

        #region Tickets

        /// <summary>
        /// The <see cref="Tickets" /> property's name.
        /// </summary>
        public const string TicketsPropertyName = "Tickets";

        private List<ReponseTicketSum> _tickets = new List<ReponseTicketSum>();

        /// <summary>
        /// 机票总表
        /// </summary>
        public List<ReponseTicketSum> Tickets
        {
            get { return _tickets; }

            set
            {
                if (_tickets == value) return;

                RaisePropertyChanging(TicketsPropertyName);
                _tickets = value;
                RaisePropertyChanged(TicketsPropertyName);
            }
        }

        #endregion

        #region OrderId

        /// <summary>
        /// The <see cref="OrderId" /> property's name.
        /// </summary>
        public const string OrderIDPropertyName = "OrderId";

        private string orderId = null;

        /// <summary>
        /// 原订单编号
        /// </summary>
        public string OrderId
        {
            get { return orderId; }

            set
            {
                if (orderId == value) return;

                RaisePropertyChanging(OrderIDPropertyName);
                orderId = value;
                RaisePropertyChanged(OrderIDPropertyName);
            }
        }

        #endregion

        #region CurrentOrderID

        /// <summary>
        /// The <see cref="OrderId" /> property's name.
        /// </summary>
        public const string CurrentOrderIDPropertyName = "CurrentOrderID";

        private string currentOrderID = null;

        /// <summary>
        /// 售后订单编号
        /// </summary>
        public string CurrentOrderID
        {
            get { return currentOrderID; }

            set
            {
                if (currentOrderID == value) return;

                RaisePropertyChanging(CurrentOrderIDPropertyName);
                currentOrderID = value;
                RaisePropertyChanged(CurrentOrderIDPropertyName);
            }
        }

        #endregion

        #region TransactionNumber

        /// <summary>
        /// The <see cref="PayNo" /> property's name.
        /// </summary>
        public const string TransactionNumberPropertyName = "TransactionNumber";

        private string transactionNumber = null;

        /// <summary>
        /// 交易号
        /// </summary>
        public string TransactionNumber
        {
            get { return transactionNumber; }

            set
            {
                if (transactionNumber == value) return;

                RaisePropertyChanging(TransactionNumberPropertyName);
                transactionNumber = value;
                RaisePropertyChanged(TransactionNumberPropertyName);
            }
        }

        #endregion

        #region TicketNum

        /// <summary>
        /// The <see cref="PayNo" /> property's name.
        /// </summary>
        public const string TicketNumPropertyName = "TicketNum";

        private string ticketNum = null;

        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNum
        {
            get { return ticketNum; }

            set
            {
                if (ticketNum == value) return;

                RaisePropertyChanging(TicketNumPropertyName);
                ticketNum = value;
                RaisePropertyChanged(TicketNumPropertyName);
            }
        }

        #endregion

        #region PNRCode

        /// <summary>
        /// The <see cref="PNRCode" /> property's name.
        /// </summary>
        public const string PNRCodePropertyName = "PNRCode";

        private string _PNRCode;

        /// <summary>
        /// PNR编号
        /// </summary>
        public string PNRCode
        {
            get { return _PNRCode; }

            set
            {
                if (_PNRCode == value) return;

                RaisePropertyChanging(PNRCodePropertyName);
                _PNRCode = value;
                RaisePropertyChanged(PNRCodePropertyName);
            }
        }

        #endregion

        #region SelectedTicketStatus

        /// <summary>
        /// The <see cref="SelectedTicketStatus" /> property's name.
        /// </summary>
        private const string SelectedOrderStatusPropertyName = "SelectedTicketStatus";

        private EnumTicketStatus? _ticketStatus;

        /// <summary>
        /// 选中的订单状态
        /// </summary>
        public EnumTicketStatus? SelectedTicketStatus
        {
            get { return _ticketStatus; }

            set
            {
                if (_ticketStatus == value) return;

                RaisePropertyChanging(SelectedOrderStatusPropertyName);
                _ticketStatus = value;
                RaisePropertyChanged(SelectedOrderStatusPropertyName);
            }
        }

        #endregion

        #region AllTicketStatus

        /// <summary>
        /// The <see cref="AllTicketStatus" /> property's name.
        /// </summary>
        private const string AllTicketStatusPropertyName = "AllTicketStatus";

        private ObservableCollection<KeyValuePair<EnumTicketStatus?, String>> _allTicketStatus = new ObservableCollection<KeyValuePair<EnumTicketStatus?, string>>();

        /// <summary>
        /// 所有订单状态
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumTicketStatus?, String>> AllTicketStatus
        {
            get { return _allTicketStatus; }

            set
            {
                if (_allTicketStatus == value) return;

                RaisePropertyChanging(AllTicketStatusPropertyName);
                _allTicketStatus = value;
                RaisePropertyChanged(AllTicketStatusPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            if (CreateTime > EndCreateTime)
            {
                UIManager.ShowMessage("选择日期时开始日期大于结束日期");
                return;
            }
            if (EndCreateTime - CreateTime > TimeSpan.FromDays(32))
            {
                UIManager.ShowMessage("查询日期不能超过31天");
                return;
            };
            IsBusy = true;
            Tickets.Clear();
            Action action = () => CommunicateManager.Invoke<IOrderService>(service =>
            {
                var data = service.FindTicketSum(OrderId, CurrentOrderID, PNRCode, TicketNum, PassengerName, createTime, endCreateTime, (CurrentPageIndex - 1) * PageSize, PageSize, TransactionNumber,SelectedTicketStatus);
                if (data.List == null)
                    return;
                TotalCount = data.TotalCount;
                Tickets = data.List;
                //foreach (var item in data.List)
                //{
                //    DispatcherHelper.UIDispatcher.Invoke(new Action<ReponseTicketSum>(Tickets.Add), item);
                //}
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        protected override bool CanExecuteQueryCommand()
        {
            return base.CanExecuteQueryCommand();
        }

        #region ExportCommand

        private RelayCommand exportCommand;

        /// <summary>
        /// 导出文件
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return exportCommand ?? (exportCommand = new RelayCommand(ExecuteExportCommand, CanExecuteExportCommand));
            }
        }

        bool isAbort = false;

        private void ExecuteExportCommand()
        {
            DataTable dt = new DataTable("机票总表");
            KeyValuePair<string, Type>[] headArray = new KeyValuePair<string, Type>[]
            {
                new KeyValuePair<string,Type>("票号",typeof(string)),
                new KeyValuePair<string,Type>("PNR",typeof(string)),
                new KeyValuePair<string,Type>("舱位价",typeof(decimal)),
                new KeyValuePair<string,Type>("机建费",typeof(decimal)),
                new KeyValuePair<string,Type>("燃油费",typeof(decimal)),
                new KeyValuePair<string,Type>("返点",typeof(decimal)),
                new KeyValuePair<string,Type>("佣金",typeof(decimal)),
                new KeyValuePair<string,Type>("交易时间",typeof(DateTime)),
                new KeyValuePair<string,Type>("订单金额",typeof(string)),
                new KeyValuePair<string,Type>("支付方式",typeof(string)),
                new KeyValuePair<string,Type>("机票状态",typeof(string)),
                new KeyValuePair<string,Type>("起飞时间",typeof(string)),
                new KeyValuePair<string,Type>("航班号",typeof(string)),
                new KeyValuePair<string,Type>("航程",typeof(string)),
                new KeyValuePair<string,Type>("航位",typeof(string)),
                new KeyValuePair<string,Type>("乘客姓名",typeof(string)),
                //new KeyValuePair<string,Type>("行程单号",typeof(string)),
                new KeyValuePair<string,Type>("交易号",typeof(string)),
                new KeyValuePair<string,Type>("原订单号",typeof(string)),
                new KeyValuePair<string,Type>("售后订单号",typeof(string))
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "机票总表";
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel documents (.xls)|*.xls";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    int? orderStatus = GetOrderState();

                    isAbort = false;
                    ExportProgressViewModel vm = new ExportProgressViewModel(AbortExport);

                    var action = new Action(() =>
                    {
                        int totalCount = GetTotalCount(OrderId, PNRCode, null, PassengerName, createTime, endCreateTime);
                        int pageCount = (int)Math.Ceiling((double)totalCount / PageSize);
                        vm.Maximum = pageCount;

                        for (int i = 0; i < pageCount; i++)
                        {
                            if (isAbort)
                                break;

                            vm.Message = "下载数据" + i;
                            vm.Value = i;
                            DataPack<ReponseTicketSum> data = GetTicketInfo(OrderId, PNRCode, TicketNum, PassengerName, createTime, endCreateTime, (i) * PageSize, PageSize,SelectedTicketStatus);
                            AddData(dt, data.List);
                        }

                        vm.Message = "导出Excel";
                        ExcelHelper.RenderToExcel(dt, filename);
                        vm.Value++;
                        vm.IsClose = true;
                        if (isAbort)
                            UIManager.ShowMessage("导出终止");
                        else
                            UIManager.ShowMessage("导出成功");
                    });
                    Task.Factory.StartNew(action);

                    UIManager.ShowExportProgress(vm);
                }
                catch (Exception ex)
                {
                    UIManager.ShowErr(ex);
                }
            }
        }

        private int? GetOrderState()
        {
            int? orderStatus = null;
            if (SelectedOrderStatus.HasValue)
                orderStatus = (int)SelectedOrderStatus.Value;

            return orderStatus;
        }

        private void AddData(DataTable dt, List<ReponseTicketSum> list)
        {
            if (list != null)
            {
                foreach (var item in list)
                {
                    Console.WriteLine("PayDate" + item.CreateDate);
                    Console.WriteLine("IssueTicket" + item.CreateDate);
                    dt.Rows.Add(
                        item.TicketNum,
                        item.PNR,
                        item.SeatPrice,
                        item.ABFee,
                        item.RQFee,
                        item.PolicyPoint,
                        item.Commission,
                        item.CreateDate,
                        (item.TicketState.Contains("退") || item.TicketState.Contains("废")) ? "-" + item.OrderMoney.ToString("f2") : item.OrderMoney.ToString("f2"),
                        item.PayMethod,
                        item.TicketState,
                        item.StartTime,
                        item.FlightNum,
                        item.Voyage,
                        item.Seat,
                        //item.Discount
                        item.PassengerName,
                        //item.Taxation
                        item.PayNumber,
                        item.OrderID,
                        item.CurrentOrderID
                        );
                }
            }
        }

        public void AbortExport()
        {
            isAbort = true;
        }

        private bool CanExecuteExportCommand()
        {
            return true;
        }

        #endregion

        #endregion

        DataPack<ReponseTicketSum> GetTicketInfo(string orderId, string pnr, string ticketNumber, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, int startIndex, int count, EnumTicketStatus? selectedTicketStatus = null)
        {
            DataPack<ReponseTicketSum> result = null;
            CommunicateManager.Invoke<IOrderService>(service =>
            {
                result = service.FindTicketSum(OrderId, CurrentOrderID, PNRCode, TicketNum, PassengerName, createTime, endCreateTime, startIndex, PageSize, TransactionNumber,selectedTicketStatus);
            }, (ex) =>
            {
                result = null;
            });

            return result;
        }

        int GetTotalCount(string orderId, string pnr, string ticketNumber, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, EnumTicketStatus? selectedTicketStatus = null)
        {
            int result = 0;
            CommunicateManager.Invoke<IOrderService>(service =>
            {
                var temp = service.FindTicketSum(OrderId, CurrentOrderID, PNRCode, TicketNum, PassengerName, createTime, endCreateTime, 0, 1, TransactionNumber,selectedTicketStatus);
                result = temp.TotalCount;
            }, (ex) =>
            {

            });

            return result;
        }
    }
}
