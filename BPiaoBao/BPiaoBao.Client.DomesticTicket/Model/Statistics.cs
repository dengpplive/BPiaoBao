using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 报表分析数据模型
    /// </summary>
    public class Statistics : ObservableObject
    {
        #region 构造函数

        public Statistics()
        {

        }

        #endregion

        #region 公开属性

        #region TotalTradeTotalMoney

        /// <summary>
        /// The <see cref="TotalTradeTotalMoney" /> property's name.
        /// </summary>
        public const string TotalTradeTotalMoneyPropertyName = "TotalTradeTotalMoney";

        private decimal totalTradeTotalMoney = 0;

        /// <summary>
        /// 总交易额
        /// </summary>
        public decimal TotalTradeTotalMoney
        {
            get { return totalTradeTotalMoney; }

            set
            {
                if (totalTradeTotalMoney == value) return;

                RaisePropertyChanging(TotalTradeTotalMoneyPropertyName);
                totalTradeTotalMoney = value;
                RaisePropertyChanged(TotalTradeTotalMoneyPropertyName);
            }
        }

        #endregion

        #region TotalCommissionTotalMoney

        /// <summary>
        /// The <see cref="TotalCommissionTotalMoney" /> property's name.
        /// </summary>
        public const string TotalCommissionTotalMoneyPropertyName = "TotalCommissionTotalMoney";

        private decimal totalCommissionTotalMoney = 0;

        /// <summary>
        /// 总佣金
        /// </summary>
        public decimal TotalCommissionTotalMoney
        {
            get { return totalCommissionTotalMoney; }

            set
            {
                if (totalCommissionTotalMoney == value) return;

                RaisePropertyChanging(TotalCommissionTotalMoneyPropertyName);
                totalCommissionTotalMoney = value;
                RaisePropertyChanged(TotalCommissionTotalMoneyPropertyName);
            }
        }

        #endregion

        #region TotalIssueTicketCount

        /// <summary>
        /// The <see cref="TotalIssueTicketCount" /> property's name.
        /// </summary>
        public const string TotalIssueTicketCountPropertyName = "TotalIssueTicketCount";

        private decimal totalIssueTicketCount = 0;

        /// <summary>
        /// 总出票量
        /// </summary>
        public decimal TotalIssueTicketCount
        {
            get { return totalIssueTicketCount; }

            set
            {
                if (totalIssueTicketCount == value) return;

                RaisePropertyChanging(TotalIssueTicketCountPropertyName);
                totalIssueTicketCount = value;
                RaisePropertyChanged(TotalIssueTicketCountPropertyName);
            }
        }

        #endregion

        #endregion


        #region Items

        /// <summary>
        /// The <see cref="Items" /> property's name.
        /// </summary>
        public const string ItemsPropertyName = "Items";

        private ObservableCollection<StatisticsItem> items = new ObservableCollection<StatisticsItem>();

        /// <summary>
        /// 统计数据
        /// </summary>
        public ObservableCollection<StatisticsItem> Items
        {
            get { return items; }

            set
            {
                if (items == value) return;

                RaisePropertyChanging(ItemsPropertyName);
                items = value;
                RaisePropertyChanged(ItemsPropertyName);
            }
        }

        #endregion

        internal static Statistics Transfer(List<AppServices.DataContracts.DomesticTicket.DataStatisticsDto> list)
        {
            Statistics result = new Statistics();

            foreach (var item in list)
            {
                //生成一个员工一天的数据
                var employeeItems = GeneralItem(item.DataStatisticsList, item);
                result.Items.Add(employeeItems);
            }

            //foreach (var item in list)
            //{
            //    item.OperatorAccount = "tes";

            //    //生成一个员工的3行数据
            //    var employeeItems = GeneralItem(item.DataStatisticsList, item);
            //    temp.Add(employeeItems);
            //}

            if (result.Items != null && result.Items.Count > 0)
            {
                //每日汇总3行           
                StatisticsItem total = GeneralDayTotalItem(result.Items.ToList());
                result.TotalCommissionTotalMoney = total.Data[31].CommissionTotalMoney;
                result.TotalIssueTicketCount = total.Data[31].IssueTicketCount;
                result.TotalTradeTotalMoney = total.Data[31].TradeTotalMoney;
                result.Items.Add(total);
            }

            return result;
        }

        //生成每日总汇
        private static StatisticsItem GeneralDayTotalItem(List<StatisticsItem> temp)
        {
            StatisticsItem result = new StatisticsItem();
            for (int i = 0; i < result.Data.Length; i++)
            {
                result.Data[i] = new OneDayStatisticsItem();
                result.Data[i].CommissionTotalMoney = temp.Sum(m => m.Data[i] == null ? 0 : m.Data[i].CommissionTotalMoney);
                result.Data[i].IssueTicketCount = temp.Sum(m => m.Data[i] == null ? 0 : m.Data[i].IssueTicketCount);
                result.Data[i].TradeTotalMoney = temp.Sum(m => m.Data[i] == null ? 0 : m.Data[i].TradeTotalMoney);
                //空数据恢复为空，界面上不显示0
                if (result.Data[i].CommissionTotalMoney == 0 &&
                    result.Data[i].IssueTicketCount == 0 &&
                    result.Data[i].TradeTotalMoney == 0)
                    result.Data[i] = null;
            }

            result.EmployeeName = "每日总汇";
            return result;
        }

        private static StatisticsItem GeneralItem(List<BPiaoBao.AppServices.DataContracts.DomesticTicket.DataStatisticsDto.DataStatistics> data, DataStatisticsDto statisticsDto)
        {
            var result = new StatisticsItem();
            result.EmployeeName = statisticsDto.OperatorAccount;

            foreach (var item in data)
            {
                result.Data[item.Day - 1] = (OneDayStatisticsItem)item;
            }
            //每月汇总
            result.Data[31] = new OneDayStatisticsItem()
            {
                CommissionTotalMoney = statisticsDto.TotalCommission,
                IssueTicketCount = statisticsDto.TotalIssueTicket,
                TradeTotalMoney = statisticsDto.TotalTradeMoney
            };

            return result;
        }
    }

    /// <summary>
    /// 表示一行数据
    /// </summary>
    public class StatisticsItem : ObservableObject
    {
        #region TypeName

        /// <summary>
        /// The <see cref="TypeName" /> property's name.
        /// </summary>
        public const string TypeNamePropertyName = "TypeName";

        private string typeName = null;

        /// <summary>
        /// 数据类型(出票量，交易额，佣金)
        /// </summary>
        public string TypeName
        {
            get { return typeName; }

            set
            {
                if (typeName == value) return;

                RaisePropertyChanging(TypeNamePropertyName);
                typeName = value;
                RaisePropertyChanged(TypeNamePropertyName);
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// The <see cref="Data" /> property's name.
        /// </summary>
        public const string DataPropertyName = "Data";

        private OneDayStatisticsItem[] data = new OneDayStatisticsItem[32];

        /// <summary>
        /// 每日数据,总共31日,第32个是当月汇总
        /// </summary>
        public OneDayStatisticsItem[] Data
        {
            get { return data; }

            set
            {
                if (data == value) return;

                RaisePropertyChanging(DataPropertyName);
                data = value;
                RaisePropertyChanged(DataPropertyName);
            }
        }

        #endregion

        #region EmployeeName

        /// <summary>
        /// The <see cref="EmployeeName" /> property's name.
        /// </summary>
        public const string EmployeeNamePropertyName = "EmployeeName";

        private string employeeName = null;

        /// <summary>
        /// 员工名称
        /// </summary>
        public string EmployeeName
        {
            get { return employeeName; }

            set
            {
                if (employeeName == value) return;

                RaisePropertyChanging(EmployeeNamePropertyName);
                employeeName = value;
                RaisePropertyChanged(EmployeeNamePropertyName);
            }
        }

        #endregion
    }

    /// <summary>
    /// 一个员工一天的数据
    /// </summary>
    public class OneDayStatisticsItem : ObservableObject
    {
        #region TradeTotalMoney

        /// <summary>
        /// The <see cref="TradeTotalMoney" /> property's name.
        /// </summary>
        public const string TradeTotalMoneyPropertyName = "TradeTotalMoney";

        private decimal tradeTotalMoney = 0;

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TradeTotalMoney
        {
            get { return tradeTotalMoney; }

            set
            {
                if (tradeTotalMoney == value) return;

                RaisePropertyChanging(TradeTotalMoneyPropertyName);
                tradeTotalMoney = value;
                RaisePropertyChanged(TradeTotalMoneyPropertyName);
            }
        }

        #endregion

        #region CommissionTotalMoney

        /// <summary>
        /// The <see cref="CommissionTotalMoney" /> property's name.
        /// </summary>
        public const string CommissionTotalMoneyPropertyName = "CommissionTotalMoney";

        private decimal commissionTotalMoney = 0;

        /// <summary>
        /// 佣金
        /// </summary>
        public decimal CommissionTotalMoney
        {
            get { return commissionTotalMoney; }

            set
            {
                if (commissionTotalMoney == value) return;

                RaisePropertyChanging(CommissionTotalMoneyPropertyName);
                commissionTotalMoney = value;
                RaisePropertyChanged(CommissionTotalMoneyPropertyName);
            }
        }

        #endregion

        #region IssueTicketCount

        /// <summary>
        /// The <see cref="IssueTicketCount" /> property's name.
        /// </summary>
        public const string IssueTicketCountPropertyName = "IssueTicketCount";

        private decimal issueTicketCount = 0;

        /// <summary>
        /// 出票量
        /// </summary>
        public decimal IssueTicketCount
        {
            get { return issueTicketCount; }

            set
            {
                if (issueTicketCount == value) return;

                RaisePropertyChanging(IssueTicketCountPropertyName);
                issueTicketCount = value;
                RaisePropertyChanged(IssueTicketCountPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 几号
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Called when [day statistics item].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static explicit operator OneDayStatisticsItem(DataStatisticsDto.DataStatistics source)
        {
            OneDayStatisticsItem result = new OneDayStatisticsItem();
            result.CommissionTotalMoney = source.CommissionTotalMoney;
            result.Day = source.Day;
            result.IssueTicketCount = source.IssueTicketCount;
            result.TradeTotalMoney = source.TradeTotalMoney;
            return result;
        }
    }
}
