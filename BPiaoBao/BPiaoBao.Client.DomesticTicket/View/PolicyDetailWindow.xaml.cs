using System.Windows;
using System.Windows.Input;
using BPiaoBao.Client.DomesticTicket.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// 政策详情.xaml 的交互逻辑
    /// </summary>
    public partial class PolicyDetailWindow
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PolicyDetailWindow()
        {
            InitializeComponent();
            KeyDown += PolicyDetailWindow_KeyDown;
        }

        void PolicyDetailWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F7) return;
            var vm = DataContext as PolicyDetailViewModel;
            if (vm != null && vm.IsShowCommissinInfo)
            {
                vm.IsShowCommissinInfo = false;
                vm.IsShowTicketPrice = true;
            }
            else
            {
                if (vm != null)
                {
                    vm.IsShowCommissinInfo = true;
                    vm.IsShowTicketPrice = false;
                }
            }
            if (vm != null) LocalUIManager.DefaultShowhiddenColumn = vm.IsShowCommissinInfo;
            Messenger.Default.Send(LocalUIManager.DefaultShowhiddenColumn?Visibility.Visible:Visibility.Collapsed, "PolicyDetailWindow_To_PnrControl_Msg");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as PolicyDetailViewModel;
            if (vm == null)
                return;
            if (radioBtnCashbag.IsChecked != null && radioBtnCashbag.IsChecked.Value)
            {
                if (vm.PayOrderByCashbagAccountCommand.CanExecute(passwordBox.Password))
                    vm.PayOrderByCashbagAccountCommand.Execute(passwordBox.Password);
            }
            else if (radioBtnCredit.IsChecked != null && radioBtnCredit.IsChecked.Value)
            {
                if (vm.PayOrderByCreditAccountCommand.CanExecute(passwordBox.Password))
                    vm.PayOrderByCreditAccountCommand.Execute(passwordBox.Password);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = DataContext as PolicyDetailViewModel;

            if (vm != null && vm.IsPaid)
            {
                Messenger.Default.Send(true, "close_choose_policy_window");
            }
 
        }
    }
}
