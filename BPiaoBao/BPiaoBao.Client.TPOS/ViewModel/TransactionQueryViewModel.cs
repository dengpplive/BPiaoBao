using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Converter;
using BPiaoBao.Client.UIExt.Helper;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.TPOS.ViewModel
{
    /// <summary>
    /// 交易查询视图模型
    /// </summary>
    public class TransactionQueryViewModel : PageBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionQueryViewModel"/> class.
        /// </summary>
        public TransactionQueryViewModel()
        {
            this.CurrentPageIndex = 1;
            this.Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #region 公开属性

        #region POSNo
        /// <summary>
        /// The <see cref="POSNo" /> property's name.
        /// </summary>
        public const string POSNoPropertyName = "POSNo";
        private string _posNo = string.Empty;
        /// <summary>
        /// pos终端号
        /// </summary>
        public string POSNo
        {
            get { return _posNo; }
            set
            {
                if (_posNo == value) return;
                RaisePropertyChanging(POSNoPropertyName);
                _posNo = value;
                RaisePropertyChanged(POSNoPropertyName);
            }
        }

        #endregion

        #region TradeDetailList

        /// <summary>
        /// The <see cref="TradeDetailList" /> property's name.
        /// </summary>
        public const string TradeDetailListPropertyName = "TradeDetailList";

        private ObservableCollection<TradeDetailDataObject> _tradeDetailList = new ObservableCollection<TradeDetailDataObject>();

        /// <summary>
        /// desc
        /// </summary>
        public ObservableCollection<TradeDetailDataObject> TradeDetailList
        {
            get { return _tradeDetailList; }

            set
            {
                if (_tradeDetailList == value) return;

                RaisePropertyChanging(TradeDetailListPropertyName);
                _tradeDetailList = value;
                RaisePropertyChanged(TradeDetailListPropertyName);
            }
        }

        #endregion

        #endregion

        #region 命令

        private bool isLoadingList = false;
        protected override bool CanExecuteQueryCommand()
        {
            return !isLoadingList;
        }

        protected override void ExecuteQueryCommand()
        {
            isLoadingList = IsBusy = true;
            this.TradeDetailList.Clear();

            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    var temp = service.GetTradeDetail(this.StartTime, this.EndTime, this.POSNo, (this.CurrentPageIndex - 1) * PageSize, PageSize);
                    if (temp != null)
                    {
                        this.TotalCount = temp.TotalCount;
                        if (temp.List != null && temp.List.Count != 0)
                            temp.List.ForEach(p => DispatcherHelper.UIDispatcher.Invoke(new Action<TradeDetailDataObject>(TradeDetailList.Add), p));
                    }
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith(p =>
            {
                Action setAction = () => { isLoadingList = IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #region ExportCommand

        private RelayCommand _exportCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand(ExecuteExportCommand));
            }
        }

        private void ExecuteExportCommand()
        {
            DataTable dt = new DataTable("TPOS交易明细");
            List<KeyValuePair<string, Type>> headArray = new List<KeyValuePair<string, Type>>
            {
                 new KeyValuePair<string,Type>("POS终端号",typeof(string)),
                 new KeyValuePair<string,Type>("商户名",typeof(string)),
                 new KeyValuePair<string,Type>("交易时间",typeof(DateTime)),
                 new KeyValuePair<string,Type>("批次号",typeof(string)),
                 new KeyValuePair<string,Type>("刷卡卡号",typeof(string)),
                 new KeyValuePair<string,Type>("类型",typeof(string)),
                 new KeyValuePair<string,Type>("交易金额",typeof(decimal)),
                 new KeyValuePair<string,Type>("收款金额",typeof(decimal)),
                 new KeyValuePair<string,Type>("POS费率",typeof(double)),
                 new KeyValuePair<string,Type>("POS收入",typeof(decimal))
            };
            headArray.ForEach(p => dt.Columns.Add(p.Key, p.Value));

            BankCardConverter bankCardConverter = new BankCardConverter();

            CommunicateManager.Invoke<ITPosService>(service =>
            {
                var temp = service.GetTradeDetail(this.StartTime, this.EndTime, this.POSNo, 0, 1000);
                if (temp != null)
                {
                    if (temp.List != null && temp.List.Count != 0)
                        temp.List.ForEach(p =>
                        {
                            dt.Rows.Add(
                                p.PosNo,
                                p.BusinessmanName,
                                p.TradeTime,
                                p.BatchNo,
                                bankCardConverter.Convert(p.TradeCardNo, null, null, null),
                                p.TradeCardType,
                                p.TradeMoney,
                                p.ReceivMoney,
                                p.PosRate,
                                p.PosGain);
                        });
                }
            }, UIManager.ShowErr);






            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "TPOS交易明细";
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel documents (.xls)|*.xls";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filename = dlg.FileName;
                    ExcelHelper.RenderToExcel(dt, filename);
                    UIManager.ShowMessage("导出成功");
                }
                catch (Exception ex)
                {
                    UIManager.ShowErr(ex);
                }
            }
        }

        #endregion

        #endregion
    }
}
