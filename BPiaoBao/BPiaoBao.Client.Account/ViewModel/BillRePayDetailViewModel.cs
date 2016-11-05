using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model.Enumeration;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 账单还款明细视图模型
    /// </summary>
    public class BillRePayDetailViewModel : PageBaseViewModel
    {
        private bool _isExporting;

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="BillRePayDetailViewModel"/> class.
        /// </summary>
        public BillRePayDetailViewModel()
        {
            //StartTime = null;
            //EndTime = null;
            _allStatus.Add(new KeyValuePair<BillStatus?, string>(BillStatus.All, EnumHelper.GetDescription(BillStatus.All)));
            _allStatus.Add(new KeyValuePair<BillStatus?, string>(BillStatus.Paid, EnumHelper.GetDescription(BillStatus.Paid)));
            _allStatus.Add(new KeyValuePair<BillStatus?, string>(BillStatus.Unpaid, EnumHelper.GetDescription(BillStatus.Unpaid)));

            if (IsInDesignMode)
                return;
            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (IsInDesignMode)
                return;

            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性

        #region SelectedStatus

        /// <summary>
        /// The <see cref="SelectedStatus" /> property's name.
        /// </summary>
        private const string SelectedStatusPropertyName = "SelectedStatus";

        private BillStatus? _selectedStatus = BillStatus.All;

        /// <summary>
        /// 选中状态
        /// </summary>
        public BillStatus? SelectedStatus
        {
            get { return _selectedStatus; }

            set
            {
                if (_selectedStatus == value) return;

                RaisePropertyChanging(SelectedStatusPropertyName);
                _selectedStatus = value;
                RaisePropertyChanged(SelectedStatusPropertyName);
            }
        }

        #endregion

        #region AllStatus

        /// <summary>
        /// The <see cref="AllStatus" /> property's name.
        /// </summary>
        private const string AllStatusPropertyName = "AllStatus";

        private ObservableCollection<KeyValuePair<BillStatus?, string>> _allStatus = new ObservableCollection<KeyValuePair<BillStatus?, string>>();

        /// <summary>
        /// 所有状态
        /// </summary>
        public ObservableCollection<KeyValuePair<BillStatus?, string>> AllStatus
        {
            get { return _allStatus; }

            set
            {
                if (_allStatus == value) return;

                RaisePropertyChanging(AllStatusPropertyName);
                _allStatus = value;
                RaisePropertyChanged(AllStatusPropertyName);
            }
        }

        #endregion

        #region RepayStartDate

        /// <summary>
        /// The <see cref="RepayStartDate" /> property's name.
        /// </summary>
        private const string RepayStartDatePropertyName = "RepayStartDate";

        private DateTime? _repayStartDate;

        /// <summary>
        /// 还款开始时间
        /// </summary>
        public DateTime? RepayStartDate
        {
            get { return _repayStartDate; }

            set
            {
                if (_repayStartDate == value) return;

                RaisePropertyChanging(RepayStartDatePropertyName);
                _repayStartDate = value;
                RaisePropertyChanged(RepayStartDatePropertyName);
            }
        }

        #endregion

        #region RepayEndDate

        /// <summary>
        /// The <see cref="RepayEndDate" /> property's name.
        /// </summary>
        private const string RepayEndDatePropertyName = "RepayEndDate";

        private DateTime? _repayEndDate;

        /// <summary>
        /// 还款结束时间
        /// </summary>
        public DateTime? RepayEndDate
        {
            get { return _repayEndDate; }

            set
            {
                if (_repayEndDate == value) return;

                RaisePropertyChanging(RepayEndDatePropertyName);
                _repayEndDate = value;
                RaisePropertyChanged(RepayEndDatePropertyName);
            }
        }

        #endregion

        #region PayNo

        /// <summary>
        /// The <see cref="PayNo" /> property's name.
        /// </summary>
        private const string PayNoPropertyName = "PayNo";

        private string _payNo;

        /// <summary>
        /// 流水号
        /// </summary>
        public string PayNo
        {
            get { return _payNo; }

            set
            {
                if (_payNo == value) return;

                RaisePropertyChanging(PayNoPropertyName);
                _payNo = value;
                RaisePropertyChanged(PayNoPropertyName);
            }
        }

        #endregion

        #region OutTradeNo

        /// <summary>
        /// The <see cref="PayNo" /> property's name.
        /// </summary>
        private const string OutTradeNoPropertyName = "OutTradeNo";

        private string _outTradeNo;

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo
        {
            get { return _outTradeNo; }

            set
            {
                if (_outTradeNo == value) return;

                RaisePropertyChanging(OutTradeNoPropertyName);
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNoPropertyName);
            }
        }

        #endregion

        #region RePayDetailList

        /// <summary>
        /// The <see cref="RePayDetailList" /> property's name.
        /// </summary>
        private const string RePayDetailListPropertyName = "RePayDetailList";

        private ObservableCollection<RePayDetailListDto> _rePayDetailList = new ObservableCollection<RePayDetailListDto>();

        /// <summary>
        /// 还款明细
        /// </summary>
        public ObservableCollection<RePayDetailListDto> RePayDetailList
        {
            get { return _rePayDetailList; }

            set
            {
                if (_rePayDetailList == value) return;

                RaisePropertyChanging(RePayDetailListPropertyName);
                _rePayDetailList = value;
                RaisePropertyChanged(RePayDetailListPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region 查询

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            RePayDetailList.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var status = SelectedStatus == BillStatus.All ? null : (SelectedStatus == BillStatus.Paid ? "1" : "0");

                var result = service.GetRePayDetail(StartTime, EndTime, RepayStartDate, RepayEndDate, PayNo, status,
                    (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    TotalCount = result.TotalCount;
                }));

                if (result.List != null)
                {
                    foreach (var item in result.List)
                        DispatcherHelper.UIDispatcher.Invoke(new Action<RePayDetailListDto>(RePayDetailList.Add), item);
                }
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region ExportCommand

        private RelayCommand _exportCommand;

        /// <summary>
        /// 导出文件
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand(ExecuteExportCommand, CanExecuteExportCommand));
            }
        }

        private void ExecuteExportCommand()
        {
            var dt = new DataTable("还款明细");
            var headArray = new[]
            {
                new KeyValuePair<string,Type>("还款时间",typeof(DateTime)),
                new KeyValuePair<string,Type>("交易号",typeof(string)),
                new KeyValuePair<string,Type>("还款金额",typeof(decimal)),
                new KeyValuePair<string,Type>("还款方式",typeof(string)),
                new KeyValuePair<string,Type>("销账金额",typeof(decimal)),
                new KeyValuePair<string,Type>("账单日期",typeof(DateTime)),                
                new KeyValuePair<string,Type>("账单状态",typeof(string)),
                new KeyValuePair<string,Type>("备注",typeof(string))                
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }

            var dlg = new SaveFileDialog
            {
                FileName = "还款明细",
                DefaultExt = ".xls",
                Filter = "Excel documents (.xls)|*.xls"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            _isExporting = IsBusy = true;
            var exportAction = new Action(() =>
            {
                try
                {
                    var listDtos = GetList();
                    if (listDtos != null)
                    {
                        foreach (var item in listDtos)
                        {
                            dt.Rows.Add(
                                item.CreateDate,
                                item.OutTradeNo,
                                item.Amount,
                                item.RepayType,
                                item.TotalAmount,
                                item.BillDate,
                                item.Status,
                                item.Notes
                                );
                        }
                    }

                    var filename = dlg.FileName;
                    ExcelHelper.RenderToExcel(dt, filename);
                    UIManager.ShowMessage("导出成功");
                }
                catch (Exception ex)
                {
                    UIManager.ShowErr(ex);
                }
            });

            Task.Factory.StartNew(exportAction).ContinueWith(task =>
            {
                _isExporting = IsBusy = false;
            });
        }

        private IEnumerable<RePayDetailListDto> GetList()
        {
            List<RePayDetailListDto> result = null;
            CommunicateManager.Invoke<IAccountService>(service =>
                {
                    var status = SelectedStatus == BillStatus.All ? null : (SelectedStatus == BillStatus.Paid ? "1" : "0");

                    var temp = service.GetRePayDetail(StartTime, EndTime, RepayStartDate, RepayEndDate, PayNo, status,
                        0, 1000,OutTradeNo);

                    result = temp.List;
                });

            return result;
        }

        private bool CanExecuteExportCommand()
        {
            return !_isExporting;
        }

        #endregion

        #endregion
    }
}
