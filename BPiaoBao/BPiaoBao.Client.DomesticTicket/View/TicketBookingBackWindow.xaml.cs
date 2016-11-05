using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BPiaoBao.Client.DomesticTicket.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using JoveZhao.Framework;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// TicketBookingBackWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TicketBookingBackWindow
    {
        public TicketBookingBackWindow()
        {
            InitializeComponent();
            // 在此点之下插入创建对象所需的代码。
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-Hans");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-Hans");
            Messenger.Default.Register<bool>(this, TicketBookingBackViewModel.c_CloseTicketBookingBackWindow,
                p =>
                {
                    if (!p) return;
                    try
                    {
                        Messenger.Default.Unregister<bool>(this, TicketBookingBackViewModel.c_CloseTicketBookingBackWindow);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(LogType.ERROR, "",ex);
                    }
                });

            KeyDown += TicketBookingBackWindow_KeyDown;
            

           
        }

        void TicketBookingBackWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F7:
                    if (dg.Columns[8].Visibility == Visibility.Visible)
                    {
                        dg.Columns[8].Visibility = Visibility.Collapsed;
                        LocalUIManager.DefaultShowhiddenColumn = false;
                    
                    }
                    else
                    {
                        dg.Columns[8].Visibility = Visibility.Visible;
                        LocalUIManager.DefaultShowhiddenColumn = true;
                   
                    }
                    break;
                case Key.F6:
                    Sort("DefaultSite.SeatPrice",
                        dg.ColumnFromDisplayIndex(5).SortDirection == ListSortDirection.Ascending
                            ? ListSortDirection.Descending
                            : ListSortDirection.Ascending);
                    break;
            }
        }

        /// <summary>
        /// 模拟点击列头
        /// </summary>
        /// <param name="c">列名</param>
        /// <param name="d">方向</param>
        private void Sort(string c, ListSortDirection d)
        {
            var v = CollectionViewSource.GetDefaultView(dg.ItemsSource);
            v.SortDescriptions.Clear();
            v.SortDescriptions.Add(new SortDescription(c, d));
            v.Refresh(); 
            dg.ColumnFromDisplayIndex(5).SortDirection = d;
            dg.Focus();
        }

        public TicketBookingBackWindow(TicketBookingBackViewModel vm)
        {
             InitializeComponent();
            DataContext = vm; 
            switch (vm.IsShowCommissionColumn)
            {
                case Visibility.Visible:
                    dg.Columns[8].Visibility = Visibility.Visible;
                    break;
                case Visibility.Collapsed:
                    dg.Columns[8].Visibility = Visibility.Collapsed;
                    break;
            }
            Sort("DefaultSite.SeatPrice", ListSortDirection.Ascending);
        }
    }
}
