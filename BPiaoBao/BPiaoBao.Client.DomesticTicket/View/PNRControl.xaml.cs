using BPiaoBao.Client.DomesticTicket.ViewModel;
using BPiaoBao.Client.Module;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// PNR内容导入界面
    /// </summary>
    [Part(Main.PnrControlCode)]
    public partial class PNRControl : UserControl, IPart
    {
        public PNRControl()
        {
            InitializeComponent();
            KeyDown += PNRControl_KeyDown;
            cbSH.IsChecked = LocalUIManager.DefaultShowhiddenColumn;
            Messenger.Default.Register<Visibility>(this, TicketBookingQueryViewModel.ToPnrViewMessageCommission, ShowHidenColumn);
            Messenger.Default.Register<Visibility>(this, ChoosePolicyViewModel.ChoosePolicyToPnrViewMessage_Commission,ShowHidenColumn); 
            Messenger.Default.Register<Visibility>(this, "PolicyDetailWindow_To_PnrControl_Msg", ShowHidenColumn);
        }

        private void ShowHidenColumn(Visibility p)
        {
            switch (p)
            {
                case Visibility.Visible:
                    dg.Columns[0].Visibility = Visibility.Visible;
                    cbSH.IsChecked = true;
                    break;
                case Visibility.Collapsed:
                    dg.Columns[0].Visibility = Visibility.Collapsed;
                    cbSH.IsChecked = false;
                    break;
            }
        }

        void PNRControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F7) return;
            if (dg.Columns[0].Visibility == Visibility.Visible)
            {
                dg.Columns[0].Visibility = Visibility.Collapsed;
                cbSH.IsChecked = false;
                LocalUIManager.DefaultShowhiddenColumn = false;
            }
            else
            {
                dg.Columns[0].Visibility = Visibility.Visible;
                cbSH.IsChecked = true;
                LocalUIManager.DefaultShowhiddenColumn = true;
            }
#if DEBUGx
            if (e.Key == Key.F2)
            {
                var dlg = new TicketBookingWindow();
                dlg.DataContext = new TicketBookingViewModel(new FlightInfoModel[]
                {
                    new FlightInfoModel()
                    {
                        DefaultSite = new Site(),
                    },
                });
                dlg.ShowDialog();
            }
#endif
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetContent()
        {
            return this;
        }


        public string Title
        {
            get { return "PNR内容导入"; }
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var main = DataContext as PNRViewModel;
            if (main == null) return;
            dg.Columns[0].Visibility = Visibility.Visible;
            main.IsShowCommissionColumn = Visibility.Visible;
            cbSH.Content = "隐";
            Messenger.Default.Send(main.IsShowCommissionColumn, PNRViewModel.ToTicketBookingQueryViewMessageCommission);
            Messenger.Default.Send(main.IsShowCommissionColumn, PNRViewModel.ToChoosePolicyViewMessageCommission);
            LocalUIManager.DefaultShowhiddenColumn = true;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var main = DataContext as PNRViewModel;
            if (main == null) return;
            dg.Columns[0].Visibility = Visibility.Collapsed;
            main.IsShowCommissionColumn = Visibility.Collapsed;
            cbSH.Content = "显";
            Messenger.Default.Send(main.IsShowCommissionColumn, PNRViewModel.ToTicketBookingQueryViewMessageCommission);
            Messenger.Default.Send(main.IsShowCommissionColumn, PNRViewModel.ToChoosePolicyViewMessageCommission);
            LocalUIManager.DefaultShowhiddenColumn = false;
        }
    }
}
