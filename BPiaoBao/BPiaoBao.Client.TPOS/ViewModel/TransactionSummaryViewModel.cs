using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.TPOS.Model;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BPiaoBao.Client.TPOS.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class TransactionSummaryViewModel : BaseVM
    {
        #region 构造函数

        public TransactionSummaryViewModel()
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
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性

        #region BusinessmanReportData

        /// <summary>
        /// The <see cref="BusinessmanReportData" /> property's name.
        /// </summary>
        public const string BusinessmanReportDataPropertyName = "BusinessmanReportData";

        private BusinessmanReportDataObject businessmanReportData = null;

        /// <summary>
        /// 报表数据
        /// </summary>
        public BusinessmanReportDataObject BusinessmanReportData
        {
            get { return businessmanReportData; }

            set
            {
                if (businessmanReportData == value) return;

                RaisePropertyChanging(BusinessmanReportDataPropertyName);
                businessmanReportData = value;
                RaisePropertyChanged(BusinessmanReportDataPropertyName);
            }
        }

        #endregion

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

        #region GainData

        /// <summary>
        /// The <see cref="GainData" /> property's name.
        /// </summary>
        public const string GainDataPropertyName = "GainData";

        private ObservableCollection<dynamic> gainData = new ObservableCollection<dynamic>();

        /// <summary>
        /// 数据报表
        /// </summary>
        public ObservableCollection<dynamic> GainData
        {
            get { return gainData; }

            set
            {
                if (gainData == value) return;

                RaisePropertyChanging(GainDataPropertyName);
                gainData = value;
                RaisePropertyChanged(GainDataPropertyName);
            }
        }

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
        /// 查询命令
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
            GainData.Clear();
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    DateTime startTime = new DateTime(selectedYear, selectedMonth, 1);
                    BusinessmanReportData = service.GetBusinessmanReport(startTime, startTime.AddMonths(1).AddDays(-1));
                    DataStatistics = Statistics.Transfer(BusinessmanReportData.TradeList);
                }, UIManager.ShowErr);
            };
            Task.Factory.StartNew(action).ContinueWith(p =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteQueryCommand()
        {
            return true;
        }

        #endregion

        #endregion
    }
    public class DynamicObjectClass : DynamicObject, INotifyPropertyChanged
    {
        #region DynamicObject overrides

        public DynamicObjectClass()
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return members.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            members[binder.Name] = value;
            OnPropertyChanged(binder.Name);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return members.Keys;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            int index = (int)indexes[0];
            try
            {
                result = itemsCollection[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                result = null;
                return false;
            }
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            int index = (int)indexes[0];
            itemsCollection[index] = value;
            OnPropertyChanged(System.Windows.Data.Binding.IndexerName);
            return true;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            if (members.ContainsKey(binder.Name))
            {
                members.Remove(binder.Name);
                return true;
            }
            return false;
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            int index = (int)indexes[0];
            itemsCollection.RemoveAt(index);
            return true;
        }

        #endregion DynamicObject overrides

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Public methods

        public object AddItem(object item)
        {
            itemsCollection.Add(item);
            OnPropertyChanged(Binding.IndexerName);
            return null;
        }

        #endregion Public methods

        #region Private data

        Dictionary<string, object> members = new Dictionary<string, object>();
        ObservableCollection<object> itemsCollection = new ObservableCollection<object>();

        #endregion Private data
    }


    public class SetPropertyBinder : SetMemberBinder
    {
        public SetPropertyBinder(string propertyName)
            : base(propertyName, false) { }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            return new DynamicMetaObject(
              System.Linq.Expressions.Expression.Throw(
                System.Linq.Expressions.Expression.New(
                  typeof(InvalidOperationException).GetConstructor(new Type[] { typeof(string) }),
                  new System.Linq.Expressions.Expression[] { System.Linq.Expressions.Expression.Constant("Error") }),
              this.ReturnType), BindingRestrictions.Empty);
        }
    }
}
