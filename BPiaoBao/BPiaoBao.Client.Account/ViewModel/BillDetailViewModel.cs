using System.Globalization;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
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
    /// 账单明细视图模型
    /// </summary>
    public class BillDetailViewModel : PageBaseViewModel
    {
        private bool _isExporting;

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="BillDetailViewModel"/> class.
        /// </summary>
        public BillDetailViewModel()
        {
            StartTime = null;
            EndTime = null;

            //Initialize();
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

        #region BillDetailList

        /// <summary>
        /// The <see cref="BillDetailList" /> property's name.
        /// </summary>
        private const string BillDetailListPropertyName = "BillDetailList";

        private ObservableCollection<BillDetailListDto> _billDetailList = new ObservableCollection<BillDetailListDto>();

        /// <summary>
        /// 账单明细
        /// </summary>
        public ObservableCollection<BillDetailListDto> BillDetailList
        {
            get { return _billDetailList; }

            set
            {
                if (_billDetailList == value) return;

                RaisePropertyChanging(BillDetailListPropertyName);
                _billDetailList = value;
                RaisePropertyChanged(BillDetailListPropertyName);
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

        #region AmountMin

        /// <summary>
        /// The <see cref="AmountMin" /> property's name.
        /// </summary>
        private const string AmountMinPropertyName = "AmountMin";

        private decimal? _amountMin;

        /// <summary>
        /// 最低消费金额
        /// </summary>
        public decimal? AmountMin
        {
            get { return _amountMin; }

            set
            {
                if (_amountMin == value) return;

                RaisePropertyChanging(AmountMinPropertyName);
                _amountMin = value;
                RaisePropertyChanged(AmountMinPropertyName);
            }
        }

        #endregion

        #region AmountMax

        /// <summary>
        /// The <see cref="AmountMax" /> property's name.
        /// </summary>
        private const string AmountMaxPropertyName = "AmountMax";

        private decimal? _amountMax;

        /// <summary>
        /// 最高消费金额
        /// </summary>
        public decimal? AmountMax
        {
            get { return _amountMax; }

            set
            {
                if (_amountMax == value) return;

                RaisePropertyChanging(AmountMaxPropertyName);
                _amountMax = value;
                RaisePropertyChanged(AmountMaxPropertyName);
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
            BillDetailList.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var result = service.GetBillDetail(StartTime, EndTime, PayNo, (_amountMin ?? 0).ToString(CultureInfo.InvariantCulture),
                    (AmountMax ?? decimal.MaxValue).ToString(CultureInfo.InvariantCulture),
                    (CurrentPageIndex - 1) * PageSize, PageSize,OutTradeNo);
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    TotalCount = result.TotalCount;
                }));

                if (result.List == null) return;
                foreach (var item in result.List)
                    DispatcherHelper.UIDispatcher.Invoke(new Action<BillDetailListDto>(BillDetailList.Add), item);
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
            var dt = new DataTable("消费明细");
            var headArray = new[]
            {
                new KeyValuePair<string,Type>("交易时间",typeof(DateTime)),
                new KeyValuePair<string,Type>("交易号",typeof(string)),
                new KeyValuePair<string,Type>("订单号",typeof(string)),
                new KeyValuePair<string,Type>("消费金额",typeof(decimal)),
                new KeyValuePair<string,Type>("账单日期",typeof(DateTime)),
                new KeyValuePair<string,Type>("产品名称",typeof(string))                
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }

            var dlg = new SaveFileDialog
            {
                FileName = "消费明细",
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
                    var billDetailList = GetBillList();

                    if (billDetailList != null)
                    {
                        foreach (var item in billDetailList)
                        {
                            dt.Rows.Add(
                                item.CreateDate,
                                item.OutTradeNo,
                                item.OutOrderNo,
                                item.Amount,
                                item.BillDate,
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

        private IEnumerable<BillDetailListDto> GetBillList()
        {
            List<BillDetailListDto> result = null;
            CommunicateManager.Invoke<IAccountService>(service =>
               {
                   var temp = service.GetBillDetail(StartTime, EndTime, PayNo, (_amountMin ?? 0).ToString(CultureInfo.InvariantCulture),
                      (AmountMax ?? decimal.MaxValue).ToString(CultureInfo.InvariantCulture),
                      0, 1000/*最多获取1000条？*/,OutTradeNo);

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
