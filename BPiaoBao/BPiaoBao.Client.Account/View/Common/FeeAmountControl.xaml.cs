using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BPiaoBao.Client.Account.View
{
    /// <summary>
    /// FeeAmountControl.xaml 的交互逻辑
    /// </summary>
    public partial class FeeAmountControl : Grid
    {
        public FeeAmountControl()
        {
            InitializeComponent();
            Loaded += FeeAmountControl_Loaded;
        }

        void FeeAmountControl_Loaded(object sender, RoutedEventArgs e)
        {
            IsBusy = true;

            dataGrid.ItemsSource = null;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                var temp = service.GetFeeRule();
                //var tempList = new List<object>
                //{
                //    new
                //    {
                //        Time = "次日",
                //        Fee = temp.tomorrowFee,
                //        Min = GetFeeStr(temp.tomorrowMin),
                //        Max = GetFeeStr(temp.tomorrowMax)
                //    },
                //    new
                //    {
                //        Time = "当日",
                //        Fee = temp.todayFee,
                //        Min = GetFeeStr(temp.todayMin),
                //        Max = GetFeeStr(temp.todayMax)
                //    }
                //};
                var tempList = new List<object>();
                if (temp.TodayEnable)
                {
                    tempList.Add(new
                    {
                        Time = "当天",
                        Type = temp.TodayWithdrawRateType == 1,  //type为True时显示Rate比例
                        Rate = temp.TodayEachRate,
                        FeeAmount = GetFeeStr(temp.TodayEachFeeAmount),
                        FeeAmountMin = GetFeeStr(temp.TodayEachFeeAmountMin),
                        FeeAmountMax = GetFeeStr(temp.TodayEachFeeAmountMax),
                        EachAmount = GetFeeStr(temp.TodayEachAmount),
                        DayAmount = GetDayAmountStr(temp.TodayDayAmount)
                    });
                }
                if (temp.MorrowEnable)
                {
                    tempList.Add(new
                    {
                        Time = "次日",
                        Type = temp.MorrowWithdrawRateType == 1,  //type为True时显示Rate比例
                        Rate = temp.MorrowEachRate,
                        FeeAmount = GetFeeStr(temp.MorrowEachFeeAmount),
                        FeeAmountMin = GetFeeStr(temp.MorrowEachFeeAmountMin),
                        FeeAmountMax = GetFeeStr(temp.MorrowEachFeeAmountMax),
                        EachAmount = GetFeeStr(temp.MorrowEachAmount),
                        DayAmount = GetDayAmountStr(temp.MorrowDayAmount)
                    });
                }
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    dataGrid.ItemsSource = tempList;
                }));
            }, ex => Logger.WriteLog(LogType.WARN, "获取手续费规则失败", ex));

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private object GetFeeStr(decimal? fee)
        {
            if (fee == null)
                return "------";

            return string.Format("{0}元/笔", fee);
        }
        private object GetDayAmountStr(decimal? admount)
        {
            if (admount == null)
                return "------";

            return string.Format("{0}元/天", admount);
        }

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> dependency property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsBusy" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return (bool)GetValue(IsBusyProperty);
            }
            set
            {
                SetValue(IsBusyProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsBusy" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            IsBusyPropertyName,
            typeof(bool),
            typeof(FeeAmountControl),
            new PropertyMetadata(false));

        #endregion

        #region Data

        ///// <summary>
        ///// The <see cref="Data" /> dependency property's name.
        ///// </summary>
        //public const string DataPropertyName = "Data";

        ///// <summary>
        ///// Gets or sets the value of the <see cref="Data" />
        ///// property. This is a dependency property.
        ///// </summary>
        //public FeeRuleInfoDto Data
        //{
        //    get
        //    {
        //        return (FeeRuleInfoDto)GetValue(DataProperty);
        //    }
        //    set
        //    {
        //        SetValue(DataProperty, value);
        //    }
        //}

        ///// <summary>
        ///// Identifies the <see cref="Data" /> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
        //    DataPropertyName,
        //    typeof(FeeRuleInfoDto),
        //    typeof(FeeAmountControl),
        //    new PropertyMetadata(null));

        #endregion

    }
}
