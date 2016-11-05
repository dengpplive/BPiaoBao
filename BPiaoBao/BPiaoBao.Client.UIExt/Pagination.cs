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
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Client.UIExt
{
    public class Pagination : Control
    {
        public Pagination()
        {
            SetUpCommands();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //不判断是否为0，blend设计器会报错
            if (PageSize != 0)
                TotalPage = (int)Math.Ceiling((double)TotalCount / PageSize);

            TextBox tb = this.Template.FindName("PART_Tb", this) as TextBox;
            if (tb != null)
            {
                tb.KeyDown += tb_KeyDown;

            }

        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                int pageIndex = 1;
                try
                {
                    pageIndex = int.Parse(tb.Text);
                    if (pageIndex < 1)
                        pageIndex = 1;
                    else if (pageIndex > TotalPage)
                        pageIndex = TotalPage;
                }
                catch 
                {
                    pageIndex = this.TotalPage;
                }
                this.CurrentPageIndex = pageIndex;
               // tb.Text = this.CurrentPageIndex.ToString();
            }
        }

        /// <summary>
        /// 命令集
        /// </summary>
        private void SetUpCommands()
        {
            CommandBinding firstPage = new CommandBinding(NavigationCommands.FirstPage, FirstPageCommand_Executed, FirstPageCommand_CanExecute);
            CommandBinding previousPage = new CommandBinding(NavigationCommands.PreviousPage, PreviousPageCommand_Executed, PreviousPageCommand_CanExecute);
            CommandBinding nextPage = new CommandBinding(NavigationCommands.NextPage, NextPageCommand_Executed, NextPageCommand_CanExecute);
            CommandBinding lastPage = new CommandBinding(NavigationCommands.LastPage, LastPageCommand_Executed, LastPageCommand_CanExecute);
            //CommandBinding goToPage = new CommandBinding(NavigationCommands.GoToPage, GoToPageCommand_Executed);
            //NavigationCommands.GoToPage.InputGestures.Add(new KeyGesture(Key.Enter));

            //this.CommandBindings.Add(goToPage);
            this.CommandBindings.Add(firstPage);
            this.CommandBindings.Add(previousPage);
            this.CommandBindings.Add(nextPage);
            this.CommandBindings.Add(lastPage);
        }
        //private void GoToPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        //{
        //    this.CurrentPageIndex = int.Parse(TxtPageIndex);
        //}
        #region 最后一页
        private void LastPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CurrentPageIndex < this.TotalPage;
        }
        private void LastPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CurrentPageIndex = this.TotalPage;
        }
        #endregion

        #region 下一页
        private void NextPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CurrentPageIndex < this.TotalPage;
        }

        private void NextPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CurrentPageIndex++;
        }
        #endregion

        #region 上一页
        private void PreviousPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CurrentPageIndex > 1;
        }

        private void PreviousPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CurrentPageIndex--;
        }
        #endregion

        #region 第一页
        private void FirstPageCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CurrentPageIndex > 1;
        }
        private void FirstPageCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.CurrentPageIndex = 1;
        }
        #endregion

        static Pagination()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Pagination), new FrameworkPropertyMetadata(typeof(Pagination)));

        }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(Pagination));


        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPageIndex
        {
            get { return (int)GetValue(CurrentPageIndexProperty); }
            set { SetValue(CurrentPageIndexProperty, value); }
        }
        public static readonly DependencyProperty CurrentPageIndexProperty =
            DependencyProperty.Register("CurrentPageIndex", typeof(int), typeof(Pagination), new FrameworkPropertyMetadata(1, OnPageIndexChanged));

        private static void OnPageIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Pagination pagination = sender as Pagination;
            var newValue = (int)e.NewValue;
            RoutedEventArgs args = new RoutedEventArgs(Pagination.PageChangeEvent, newValue);
            pagination.RaiseEvent(args);

        }
        /// <summary>
        /// 页码发生改变事件
        /// </summary>
        public static readonly RoutedEvent PageChangeEvent = EventManager.RegisterRoutedEvent("PageChanged", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(Pagination));
        public event RoutedEventHandler PageChanged
        {
            add { AddHandler(PageChangeEvent, value); }
            remove { RemoveHandler(PageChangeEvent, value); }
        }


        /// <summary>
        /// 总条数
        /// </summary>
        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(Pagination),new FrameworkPropertyMetadata(0,OnTotalChanged));

        private static void OnTotalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Pagination pagination = d as Pagination;
            if (pagination.PageSize != 0)
                pagination.TotalPage = (int)Math.Ceiling((double)pagination.TotalCount / pagination.PageSize);
        }


        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            private set { SetValue(TotalPageProperty, value); }
        }
        public static readonly DependencyProperty TotalPageProperty =
            DependencyProperty.Register("TotalPage", typeof(int), typeof(Pagination));


    }
    public class PageIndexConvert : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value;
        }
    }
    public class PositivePriceRule : ValidationRule
    {
        private int min;
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        private int max;
        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                int pageIndex = value.ToString().ToInt();
                if (pageIndex >= min && pageIndex <= max)
                {
                    return new ValidationResult(true, null);
                }
                return new ValidationResult(false, string.Format("当前页必须在{0}到{1}之间", min, max)); 
            }
            catch
            {
                return new ValidationResult(false, string.Format("请输入一个{0}到{1}的有效数值", min, max));
            }
        }
        
    }
}
