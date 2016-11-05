using BPiaoBao.Client.DomesticTicket.ViewModel;
using GalaSoft.MvvmLight.Messaging;
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
using System.Windows.Shapes;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// PayInsuranceSecondWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PayInsuranceSecondWindow : Window
    {
        public PayInsuranceSecondWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as PolicyDetailViewModel;
            if (vm == null || vm.Order == null)
                return;
            //if (radioBtnCashbag.IsChecked != null && radioBtnCashbag.IsChecked.Value)
            //{
                if (vm.CofirmBuyInsuranceCommand.CanExecute(""))
                    vm.CofirmBuyInsuranceCommand.Execute("");
                //if (vm.PayOrderByCashbagAccountCommand.CanExecute(passwordBox.Password))
                //    vm.PayOrderByCashbagAccountCommand.Execute(passwordBox.Password);
            //}
            //else if (radioBtnCredit.IsChecked != null && radioBtnCredit.IsChecked.Value)
            //{
            //    if (vm.PayOrderByCreditAccountCommand.CanExecute(passwordBox.Password))
            //        vm.PayOrderByCreditAccountCommand.Execute(passwordBox.Password);
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = this.DataContext as PolicyDetailViewModel;

            if (vm != null && vm.IsPaid)
            {
                Messenger.Default.Send<bool>(true, "close_choose_policy_window");
            }

        }
    }
}
