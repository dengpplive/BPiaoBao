using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.DomesticTicket.ViewModel;
using BPiaoBao.Client.Module;
using GalaSoft.MvvmLight.Messaging;
using NPOI.SS.Formula.Functions;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// 机票预订查询结果页面 的交互逻辑
    /// </summary>
    [Part(Main.TicketBookingQueryResultCode)]
    public partial class TicketBookingQueryControl : IPart
    {
        public TicketBookingQueryControl()
        {
            InitializeComponent();
            KeyDown += TicketBookingQueryControl_KeyDown;
            cbSH.IsChecked = LocalUIManager.DefaultShowhiddenColumn;
            Messenger.Default.Register<Visibility>(this, PNRViewModel.ToTicketBookingQueryViewMessageCommission, ShowHidenColumn);
            Messenger.Default.Register<Visibility>(this, ChoosePolicyViewModel.ChoosePolicyToTicketBookingQueryViewMessage_Commission, ShowHidenColumn);
            Messenger.Default.Register<bool>(this, "ticketbookingquerysort", p =>
            {
                if (p)
                {
                    Sort("DefaultSite.SeatPrice", ListSortDirection.Ascending);
                }
            });
        }
        private void ShowHidenColumn(Visibility p)
        {
            switch (p)
            {
                case Visibility.Visible:
                    dataGrid.Columns[5].Visibility = Visibility.Visible;
                    cbSH.IsChecked = true;
                    break;
                case Visibility.Collapsed:
                    dataGrid.Columns[5].Visibility = Visibility.Collapsed;
                    cbSH.IsChecked = false;
                    break;
            }
        }

        void TicketBookingQueryControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F7:
                    if (dataGrid.Columns[5].Visibility == Visibility.Visible)
                    {
                        dataGrid.Columns[5].Visibility = Visibility.Collapsed;
                        cbSH.IsChecked = false;
                        LocalUIManager.DefaultShowhiddenColumn = false;
                    }
                    else
                    {
                        dataGrid.Columns[5].Visibility = Visibility.Visible;
                        cbSH.IsChecked = true;
                        LocalUIManager.DefaultShowhiddenColumn = true;
                    }
                    break;
                case Key.F6:
                    Sort("DefaultSite.SeatPrice",
                        dataGrid.ColumnFromDisplayIndex(3).SortDirection == ListSortDirection.Ascending
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
            var v = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);
            v.SortDescriptions.Clear();
            v.SortDescriptions.Add(new SortDescription(c, d));
            v.Refresh();
            dataGrid.ColumnFromDisplayIndex(3).SortDirection = d;
            dataGrid.Focus();
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public object GetContent()
        {
            return this;
        }

        public string Title
        {
            get { return Main.TicketBookingQueryResultName; }
        }


        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var main = DataContext as TicketBookingQueryViewModel;
            if (main == null) return;
            dataGrid.Columns[5].Visibility = Visibility.Visible;
            main.IsShowCommissionColumn = Visibility.Visible;
            cbSH.Content = "隐";
            Messenger.Default.Send(main.IsShowCommissionColumn, TicketBookingQueryViewModel.ToPnrViewMessageCommission);
            Messenger.Default.Send(main.IsShowCommissionColumn, TicketBookingQueryViewModel.ToChoosePolicyViewMessageCommission);
            LocalUIManager.DefaultShowhiddenColumn = true;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var main = DataContext as TicketBookingQueryViewModel;
            if (main == null) return;
            dataGrid.Columns[5].Visibility = Visibility.Collapsed;
            main.IsShowCommissionColumn = Visibility.Collapsed;
            cbSH.Content = "显";
            Messenger.Default.Send(main.IsShowCommissionColumn, TicketBookingQueryViewModel.ToPnrViewMessageCommission);
            Messenger.Default.Send(main.IsShowCommissionColumn, TicketBookingQueryViewModel.ToChoosePolicyViewMessageCommission);
            LocalUIManager.DefaultShowhiddenColumn = false;
        }

        private void ButtonShowHide_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            var cell = sender as FrameworkElement;
            if (cell == null) return;
            DataGridRow row = DataGridRow.GetRowContainingElement(cell);
            if (row == null) return;
            foreach (var r in (from object item in dataGrid.Items select dataGrid.ItemContainerGenerator.ContainerFromItem(item)).OfType<DataGridRow>().Where(r => !r.Equals(row)))
            {
                r.DetailsVisibility = Visibility.Collapsed;
                var model = r.Item as FlightInfoModel;
                if (model != null) model.IsOpened = false;
            }
            row.DetailsVisibility = row.DetailsVisibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
            var rowmodel = row.Item as FlightInfoModel;
            if (rowmodel != null) rowmodel.IsOpened = row.DetailsVisibility == Visibility.Visible;
        }
    }
}