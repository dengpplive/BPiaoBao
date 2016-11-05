using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using GalaSoft.MvvmLight;
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

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 报表分析视图模型
    /// </summary>
    public class ReportAnalysisViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportAnalysisViewModel"/> class.
        /// </summary>
        public ReportAnalysisViewModel()
        {
            int thisYear = DateTime.Now.Year;
            //显示最近10年
            for (int i = 0; i < 10; i++)
            {
                Year.Add(thisYear - i);
            }
            for (int i = 0; i < 12; i++)
            {
                Month.Add(i + 1);
            }

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

        #region Year

        /// <summary>
        /// The <see cref="Year" /> property's name.
        /// </summary>
        public const string YearPropertyName = "Year";

        private ObservableCollection<int> years = new ObservableCollection<int>();

        /// <summary>
        /// 年
        /// </summary>
        public ObservableCollection<int> Year
        {
            get { return years; }

            set
            {
                if (years == value) return;

                RaisePropertyChanging(YearPropertyName);
                years = value;
                RaisePropertyChanged(YearPropertyName);
            }
        }

        #endregion

        #region Month

        /// <summary>
        /// The <see cref="Month" /> property's name.
        /// </summary>
        public const string MonthPropertyName = "Month";

        private ObservableCollection<int> month = new ObservableCollection<int>();

        /// <summary>
        /// 月
        /// </summary>
        public ObservableCollection<int> Month
        {
            get { return month; }

            set
            {
                if (month == value) return;

                RaisePropertyChanging(MonthPropertyName);
                month = value;
                RaisePropertyChanged(MonthPropertyName);
            }
        }

        #endregion

        #region SelectedYear

        /// <summary>
        /// The <see cref="SelectedYear" /> property's name.
        /// </summary>
        public const string SelectedYearPropertyName = "SelectedYear";

        private int selectedYear = DateTime.Today.Year;

        /// <summary>
        /// 选中的年份
        /// </summary>
        public int SelectedYear
        {
            get { return selectedYear; }

            set
            {
                if (selectedYear == value) return;

                RaisePropertyChanging(SelectedYearPropertyName);
                selectedYear = value;

                RaisePropertyChanged(SelectedYearPropertyName);
            }
        }

        #endregion

        #region SelectedMonth

        /// <summary>
        /// The <see cref="SelectedMonth" /> property's name.
        /// </summary>
        public const string SelectedMonthPropertyName = "SelectedMonth";

        private int selectedMonth = DateTime.Now.Month;

        /// <summary>
        /// 选中的年份
        /// </summary>
        public int SelectedMonth
        {
            get { return selectedMonth; }

            set
            {
                if (selectedMonth == value) return;

                RaisePropertyChanging(SelectedMonthPropertyName);
                selectedMonth = value;

                RaisePropertyChanged(SelectedMonthPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否忙碌
        ///// </summary>
        //public bool IsBusy
        //{
        //    get { return isBusy; }

        //    set
        //    {
        //        if (isBusy == value) return;

        //        RaisePropertyChanging(IsBusyPropertyName);
        //        isBusy = value;
        //        RaisePropertyChanged(IsBusyPropertyName);
        //    }
        //}

        #endregion

        #region DataStatistics

        /// <summary>
        /// The <see cref="DataStatistics" /> property's name.
        /// </summary>
        public const string DataStatisticsPropertyName = "DataStatistics";

        private Statistics dataStatistics = null;

        /// <summary>
        /// 统计数据
        /// </summary>
        public Statistics DataStatistics
        {
            get { return dataStatistics; }

            set
            {
                if (dataStatistics == value) return;

                RaisePropertyChanging(DataStatisticsPropertyName);
                dataStatistics = value;
                RaisePropertyChanged(DataStatisticsPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region QueryCommand

        private RelayCommand queryCommand;

        /// <summary>
        /// 查询数据
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return queryCommand ?? (queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        private void ExecuteQueryCommand()
        {
            IsBusy = true;

            Action action = () =>
            {
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    DataPack<DataStatisticsDto> result = service.QueryByCpTime(new DateTime(selectedYear, selectedMonth, 1));
                    DataStatistics = Statistics.Transfer(result.List);
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteQueryCommand()
        {
            return true;
        }

        #endregion

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

        private void ExecuteExportCommand()
        {
            DataTable dt = new DataTable("报表分析");
            List<KeyValuePair<string, Type>> headList = new List<KeyValuePair<string, Type>>();

            headList.Add(new KeyValuePair<string, Type>("员工名", typeof(string)));
            headList.Add(new KeyValuePair<string, Type>("类别", typeof(string)));

            for (int i = 1; i <= 31; i++)
            {
                headList.Add(new KeyValuePair<string, Type>(i.ToString(), typeof(string)));
            }
            headList.Add(new KeyValuePair<string, Type>("总量 ", typeof(string)));

            foreach (var item in headList)
            {
                dt.Columns.Add(item.Key, item.Value);
            }

            if (dataStatistics != null && dataStatistics.Items != null)
            {
                foreach (var item in dataStatistics.Items)
                {
                    List<OneDayStatisticsItem> temp = new List<OneDayStatisticsItem>();
                    for (int i = 0; i <= 31; i++)
                    {
                        if (item.Data[i] == null)
                            temp.Add(null);
                        else
                            temp.Add(item.Data[i]);
                    }
                    var row = dt.NewRow();
                    List<string> tempList = new List<string>();
                    tempList.Add(item.EmployeeName);
                    tempList.Add("佣金");
                    tempList.AddRange(temp.Select(m => m == null ? null : m.CommissionTotalMoney.ToString()));
                    row.ItemArray = tempList.ToArray();
                    dt.Rows.Add(row);

                    row = dt.NewRow();
                    tempList = new List<string>();
                    tempList.Add(item.EmployeeName);
                    tempList.Add("出票量");
                    tempList.AddRange(temp.Select(m => m == null ? null : m.IssueTicketCount.ToString()));
                    row.ItemArray = tempList.ToArray();
                    dt.Rows.Add(row);

                    row = dt.NewRow();
                    tempList = new List<string>();
                    tempList.Add(item.EmployeeName);
                    tempList.Add("交易额");
                    tempList.AddRange(temp.Select(m => m == null ? null : m.TradeTotalMoney.ToString()));
                    row.ItemArray = tempList.ToArray();
                    dt.Rows.Add(row);
                }
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "报表分析";
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

        private bool CanExecuteExportCommand()
        {
            return true;
        }

        #endregion

        #endregion
    }
}
