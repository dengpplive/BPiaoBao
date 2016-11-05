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
    /// BuyInsuranceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BuyInsuranceWindow : Window
    {
        public BuyInsuranceWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as BuyInsuranceViewModel;
            if (vm == null)
                return;
            if (radioBtnCashbag.IsChecked != null && radioBtnCashbag.IsChecked.Value)
            {
                if (vm.PayOrderByCashbagAccountCommand.CanExecute(passwordBox.Password))
                    vm.PayOrderByCashbagAccountCommand.Execute(passwordBox.Password);
            }
            //else if (radioBtnCredit.IsChecked != null && radioBtnCredit.IsChecked.Value)
            //{
            //    if (vm.PayOrderByCreditAccountCommand.CanExecute(passwordBox.Password))
            //        vm.PayOrderByCreditAccountCommand.Execute(passwordBox.Password);
            //}
        }
    }
}
