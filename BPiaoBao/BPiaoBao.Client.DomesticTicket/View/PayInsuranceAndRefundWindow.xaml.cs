using BPiaoBao.Client.DomesticTicket.ViewModel;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Input;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// PayInsuranceAndRefundWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PayInsuranceAndRefundWindow
    {
        public PayInsuranceAndRefundWindow()
        {
            InitializeComponent();
            KeyDown += PayInsuranceAndRefundWindow_KeyDown;
        }
        void PayInsuranceAndRefundWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F7) return;
            var vm = DataContext as PolicyDetailViewModel;
            if (vm == null) return;
            if (vm.IsShowCommissinInfo)
            {
                vm.IsShowCommissinInfo = false;
                vm.IsShowTicketPrice = true;
            }
            else
            {
                vm.IsShowCommissinInfo = true;
                vm.IsShowTicketPrice = false;

            }
            if (vm.SumPriceCommand.CanExecute(""))
                vm.SumPriceCommand.Execute("");
            LocalUIManager.DefaultShowhiddenColumn = vm.IsShowCommissinInfo;
            Messenger.Default.Send(LocalUIManager.DefaultShowhiddenColumn ? Visibility.Visible : Visibility.Collapsed, "PolicyDetailWindow_To_PnrControl_Msg");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as PolicyDetailViewModel;
            if (vm == null)
                return;
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                UIManager.ShowMessage("请输入支付密码");
                return;
            }
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
        private void QuikPayButton_Click(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as PolicyDetailViewModel;
            if (vm == null)
                return;
            if (string.IsNullOrEmpty(aplipaypasswordBox.Password))
            {
                UIManager.ShowMessage("请输入支付密码");
                return;
            }
            if (ApliPayRadioButton.IsChecked != null && ApliPayRadioButton.IsChecked.Value)
            {
                if (vm.PayOrderByQuikPayCommand.CanExecute(aplipaypasswordBox.Password))
                    vm.PayOrderByQuikPayCommand.Execute(aplipaypasswordBox.Password);
            }
            //else
            //{
            //    //预留财付通绑定账户操作
            //}
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
