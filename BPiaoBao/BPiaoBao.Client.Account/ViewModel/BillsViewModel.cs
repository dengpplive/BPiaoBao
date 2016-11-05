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
    /// 账单视图模型
    /// </summary>
    public class BillsViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="BillsViewModel"/> class.
        /// </summary>
        public BillsViewModel()
        {
            if (IsInDesignMode)
                return;

            _allStatus.Add(new KeyValuePair<BillStatus?, string>(BillStatus.All, EnumHelper.GetDescription(BillStatus.All)));
            _allStatus.Add(new KeyValuePair<BillStatus?, string>(BillStatus.Paid, EnumHelper.GetDescription(BillStatus.Paid)));
            _allStatus.Add(new KeyValuePair<BillStatus?, string>(BillStatus.Unpaid, EnumHelper.GetDescription(BillStatus.Unpaid)));
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

        #region BalanceDetails

        /// <summary>
        /// The <see cref="Bills" /> property's name.
        /// </summary>
        private const string BillsPropertyName = "Bills";

        private ObservableCollection<BillListDto> _bills = new ObservableCollection<BillListDto>();

        /// <summary>
        /// 最近账单
        /// </summary>
        public ObservableCollection<BillListDto> Bills
        {
            get { return _bills; }

            set
            {
                if (_bills == value) return;

                RaisePropertyChanging(BillsPropertyName);
                _bills = value;
                RaisePropertyChanged(BillsPropertyName);
            }
        }

        #endregion

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

        #region IsLoadingList

        /// <summary>
        /// The <see cref="IsLoadingList" /> property's name.
        /// </summary>
        private const string IsLoadingListPropertyName = "IsLoadingList";

        private bool _isLoadingList;

        /// <summary>
        /// 是否正在加载列表
        /// </summary>
        public bool IsLoadingList
        {
            get { return _isLoadingList; }

            private set
            {
                if (_isLoadingList == value) return;

                RaisePropertyChanging(IsLoadingListPropertyName);
                _isLoadingList = value;
                RaisePropertyChanged(IsLoadingListPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region 查询

        /// <summary>
        /// 检查是否可以执行命令
        /// </summary>
        /// <returns></returns>
        protected override bool CanExecuteQueryCommand()
        {
            return !_isLoadingList;
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            IsBusy = true;
            IsLoadingList = true;
            Bills.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                string status = SelectedStatus == BillStatus.All ? null : (SelectedStatus == BillStatus.Paid ? "2" : "0");
                var result = service.GetBill(StartTime, EndTime, status, (CurrentPageIndex - 1) * PageSize, PageSize);
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    TotalCount = result.TotalCount;
                }));

                if (result.List == null) return;
                foreach (var item in result.List)
                    DispatcherHelper.UIDispatcher.Invoke(new Action<BillListDto>(Bills.Add), item);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; IsLoadingList = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region BillDetailCommand

        private RelayCommand<BillListDto> _billDetailCommand;

        /// <summary>
        /// 打开消费明细命令
        /// </summary>
        public RelayCommand<BillListDto> BillDetailCommand
        {
            get
            {
                return _billDetailCommand ?? (_billDetailCommand = new RelayCommand<BillListDto>(ExecuteBillDetailCommand, CanExecuteBillDetailCommand));
            }
        }

        private void ExecuteBillDetailCommand(BillListDto bill)
        {
            ViewModelLocator.BillDetail.StartTime = bill.CreateDate;
            ViewModelLocator.BillDetail.EndTime = bill.CreateDate;
            ViewModelLocator.BillDetail.Initialize();
            LocalUIManager.ShowBillDetail();
            //PluginService.Run(Main.ProjectCode, Main.BillDetailCode);
        }

        private bool CanExecuteBillDetailCommand(BillListDto bill)
        {
            return true;
        }

        #endregion

        #region BillRePayDetailCommand

        private RelayCommand<BillListDto> _billRePayDetailCommand;

        /// <summary>
        /// 打开还款明细命令
        /// </summary>
        public RelayCommand<BillListDto> BillRePayDetailCommand
        {
            get
            {
                return _billRePayDetailCommand ?? (_billRePayDetailCommand = new RelayCommand<BillListDto>(ExecuteBillRePayDetailCommand, CanExecuteBillRePayDetailCommand));
            }
        }

        private void ExecuteBillRePayDetailCommand(BillListDto bill)
        {
            ViewModelLocator.BillRePayDetail.StartTime = bill.CreateDate;
            ViewModelLocator.BillRePayDetail.EndTime = bill.CreateDate;
            ViewModelLocator.BillRePayDetail.Initialize();
            LocalUIManager.ShowBillRePayDetail();
            //PluginService.Run(Main.ProjectCode, Main.BillRePayDetailCode);
        }

        private bool CanExecuteBillRePayDetailCommand(BillListDto bill)
        {
            return true;
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
            var dt = new DataTable("账单明细");
            var headArray = new[]
            {
                new KeyValuePair<string,Type>("账单日期",typeof(DateTime)),
                new KeyValuePair<string,Type>("消费金额",typeof(decimal)),
                new KeyValuePair<string,Type>("利息",typeof(decimal)),
                new KeyValuePair<string,Type>("滞纳金",typeof(decimal)),
                new KeyValuePair<string,Type>("已还金额",typeof(decimal)),
                new KeyValuePair<string,Type>("应还金额",typeof(decimal)),
                new KeyValuePair<string,Type>("账单状态",typeof(string))
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }

            if (_bills != null)
            {
                foreach (var item in _bills)
                {
                    dt.Rows.Add(
                        item.CreateDate,
                        item.Amount,
                        item.FeeAmount,
                        item.LateAmount,
                        item.RepayAmount,
                        item.ShouldRepayAmount,
                        item.Status
                        );
                }
            }

            var dlg = new SaveFileDialog
            {
                FileName = "账单明细",
                DefaultExt = ".xls",
                Filter = "Excel documents (.xls)|*.xls"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            try
            {
                var filename = dlg.FileName;
                ExcelHelper.RenderToExcel(dt, filename);
                UIManager.ShowMessage("导出成功");
            }
            catch (Exception ex)
            {
                UIManager.ShowErr(ex);
            }
        }

        private bool CanExecuteExportCommand()
        {
            return true;
        }

        #endregion

        #endregion
    }
}
