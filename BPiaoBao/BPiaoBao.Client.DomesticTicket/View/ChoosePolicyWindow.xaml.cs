using System;
using System.Windows;
using System.Windows.Input;
using BPiaoBao.Client.DomesticTicket.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using JoveZhao.Framework;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// ChoosePolicyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChoosePolicyWindow
    {
        public ChoosePolicyWindow()
        {
            InitializeComponent();
            KeyDown += ChoosePolicyWindow_KeyDown;
            Messenger.Default.Register<bool>(this, "close_choose_policy_window", p =>
            {
                if (!p) return;
                try
                {
                    Messenger.Default.Unregister<bool>(this, "close_choose_policy_window");
                    Close();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(LogType.ERROR, "",ex);
                }
            });
            dg.Columns[0].Visibility = LocalUIManager.DefaultShowhiddenColumn ? Visibility.Visible : Visibility.Collapsed;

        }

        void ChoosePolicyWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F7) return;
            if (dg.Columns[0].Visibility == Visibility.Visible)
            {
                dg.Columns[0].Visibility = Visibility.Collapsed;
                dg.Columns[5].Visibility = Visibility.Collapsed;
                LocalUIManager.DefaultShowhiddenColumn = false;
            }
            else
            {
                dg.Columns[0].Visibility = Visibility.Visible;
                dg.Columns[5].Visibility = Visibility.Visible;
                LocalUIManager.DefaultShowhiddenColumn = true;
            }

            if (Messenger.Default != null)
            {
                Messenger.Default.Send(dg.Columns[0].Visibility,
                    ChoosePolicyViewModel.ChoosePolicyToTicketBookingQueryViewMessage_Commission);
                Messenger.Default.Send(dg.Columns[0].Visibility,
                    ChoosePolicyViewModel.ChoosePolicyToPnrViewMessage_Commission);
            }
            var main = DataContext as ChoosePolicyViewModel;
            if (main == null) return;
            main.IsShowCommissionColumn = dg.Columns[0].Visibility;
        }


    }
}
