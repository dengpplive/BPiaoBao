using System.Globalization;
using BPiaoBao.Client.UIExt.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BPiaoBao.Client.UIExt.Model;

namespace BPiaoBao.Client.UIExt.CommonControl
{
    /// <summary>
    /// WeekControl.xaml 的交互逻辑
    /// </summary>
    public partial class WeekControl : UserControl
    {

        public event EventHandler<WeekRoutedEventArgs> RbChecked;
        public event EventHandler<WeekRoutedEventArgs> RbUnChecked;

        /// <summary>
        /// 显示星期Tab数
        /// </summary>
        public WeekControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示星期Tab数
        /// </summary>
        private int _showTabCount = 11;

        /// <summary>
        /// 显示星期Tab数
        /// </summary>
        public int ShowTabCount
        {
            get { return this._showTabCount; }
            set { this._showTabCount = value; }
        }


        public int MidShowTabCount
        {
            get { return this._showTabCount / 2; }
        }

        /// <summary>
        /// 初始按钮
        /// </summary> 
        /// <param name="isInit">是否是第一次初始</param>
        /// <param name="date">传入日期</param> 
        public void CreateRadioTime(bool isInit, DateTime date)
        {
            stWeekPanel.Children.Clear();

            var offset = 0;
            if (isInit)
            {
                var count = ((int)(date - DateTime.Today).TotalDays);
                if (count < ShowTabCount)
                {
                    offset = count;
                }
                else
                {
                    offset = MidShowTabCount;
                }
            }

            for (var i = 1; i <= ShowTabCount; i++)
            {
                var datetime = date.AddDays(i - 1 - offset);

                var showStr = DateTimeHelper.GetWeekByDate(datetime) + "(" + DateTimeHelper.GetMonthDay(datetime) + ")";

                var rb = new RadioButton();
                rb.Content = showStr;
                rb.Style = Resources["RadioButtonWeekStyle"] as Style;

                if (isInit && date.Date == datetime.Date)
                {
                    rb.IsChecked = true;
                }
                if (!isInit && i == MidShowTabCount + 1)
                {
                    rb.IsChecked = true;
                }

                rb.IsEnabled = datetime.Date >= DateTime.Today;
                if (!rb.IsEnabled)
                {
                    ToolTipService.SetShowOnDisabled(rb, true);
                }
                rb.Tag = i + "|" + datetime.ToString(CultureInfo.InvariantCulture);

                rb.Checked += rb_Checked;
                rb.Unchecked += rb_Unchecked;
                stWeekPanel.Children.Add(rb);
            }

            //stWeekPanel.Children.Clear();
            //var now = DateTime.Now.Date;
            //var dt = /*isInit ? DateTime.Now : */date;
            //for (var i = 1; i <= ShowTabCount; i++)
            //{
            //    var offset = isInit ? (ShowTabCount / 2) : 0;
            //    var datetime = dt.AddDays(i - 1 - offset);
            //    var showStr = DateTimeHelper.GetWeekByDate(datetime) + "(" + DateTimeHelper.GetMonthDay(datetime) + ")";
            //    var rb = new RadioButton();
            //    rb.Content = showStr;
            //    rb.HorizontalAlignment = HorizontalAlignment.Center;
            //    rb.VerticalAlignment = VerticalAlignment.Top;
            //    rb.Style = Resources["RadioButtonWeekStyle"] as Style;

            //    if (isInit && date.Date == datetime.Date)
            //    {
            //        rb.IsChecked = true;
            //    }
            //    if (!isInit && i == (ShowTabCount / 2) + 1)
            //    {
            //        rb.IsChecked = true;
            //    }

            //    if (!rb.IsEnabled)
            //    {
            //        ToolTipService.SetShowOnDisabled(rb, true);
            //    }
            //    rb.IsEnabled = datetime.Date >= now;

            //    rb.Tag = i + "|" + datetime.ToString(CultureInfo.InvariantCulture);

            //    rb.Checked += rb_Checked;
            //    rb.Unchecked += rb_Unchecked;
            //    stWeekPanel.Children.Add(rb);
            //}

        }


        /// <summary>
        /// 未选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void rb_Unchecked(object sender, RoutedEventArgs e)
        {
            if (RbUnChecked != null)
            {
                var rb = (RadioButton)sender;
                var tag = rb.Tag.ToString().Split('|');
                var index = int.Parse(tag[0]);
                var datetime = Convert.ToDateTime(tag[1]);
                var re = new WeekRoutedEventArgs();
                re.DateTime = datetime;
                re.Index = index;
                RbUnChecked(sender, re);
            }
        }


        /// <summary>
        /// 选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void rb_Checked(object sender, RoutedEventArgs e)
        {
            var rb = (RadioButton)sender;
            var tag = rb.Tag.ToString().Split('|');
            var index = int.Parse(tag[0]);
            var datetime = Convert.ToDateTime(tag[1]);
            SelectedDateTime = datetime;
            if (index == ShowTabCount)
            {
                datetime = datetime.AddDays(-MidShowTabCount);

                CreateRadioTime(false, datetime);
            }
            else if (index == 1)
            {
                if (datetime.Date != DateTime.Now.Date)
                {
                    datetime = datetime.AddDays(-MidShowTabCount);
                    CreateRadioTime(false, datetime);
                }
            }
            if (RbChecked != null)
            {
                var re = new WeekRoutedEventArgs();
                re.DateTime = datetime;
                re.Index = index;

                RbChecked(sender, re);
            }

        }

        #region TakeDateTime

        /// <summary>
        /// The <see cref="TakeDateTime" /> dependency property's name.
        /// </summary>
        public const string TakeDateTimePropertyName = "TakeDateTime";

        /// <summary>
        /// Gets or sets the value of the <see cref="TakeDateTime" />
        /// property. This is a dependency property.
        /// </summary>
        public string TakeDateTime
        {
            get
            {
                return (string)GetValue(TakeDateTimeProperty);
            }
            set
            {
                SetValue(TakeDateTimeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="TakeDateTime" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TakeDateTimeProperty = DependencyProperty.Register(
            TakeDateTimePropertyName,
            typeof(string),
            typeof(WeekControl),
            new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
            {
                var control = sender as WeekControl;
                if (control == null) return;
                var dt = Convert.ToDateTime(e.NewValue);
                control.CreateRadioTime(true, dt);
            })));

        #endregion


        #region SelectedDateTime

        /// <summary>
        /// The <see cref="SelectedDateTime" /> dependency property's name.
        /// </summary>
        public const string SelectedDateTimePropertyName = "SelectedDateTime";

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedDateTime" />
        /// property. This is a dependency property.
        /// </summary>
        public DateTime SelectedDateTime
        {
            get
            {
                return (DateTime)GetValue(SelectedDateTimeProperty);
            }
            set
            {
                SetValue(SelectedDateTimeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedDateTime" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register(
            SelectedDateTimePropertyName,
            typeof(DateTime),
            typeof(WeekControl),
            new PropertyMetadata(null));

        #endregion

    }


    /// <summary>
    /// 事件路由参数
    /// </summary>
    public class WeekRoutedEventArgs : RoutedEventArgs
    {
        public int Index { get; set; }
        public DateTime DateTime { get; set; }
    }
}
