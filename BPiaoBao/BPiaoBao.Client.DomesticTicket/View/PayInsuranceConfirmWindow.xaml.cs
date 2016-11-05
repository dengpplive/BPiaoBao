using BPiaoBao.Client.DomesticTicket.ViewModel;
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
    /// PayInsuranceConfirmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PayInsuranceConfirmWindow : Window
    {
        public PayInsuranceConfirmWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as PolicyDetailViewModel;
            if (vm == null || vm.Order == null)
                return;
            if (vm.ConfirmInsuranceCommand.CanExecute(""))
                vm.ConfirmInsuranceCommand.Execute("");
        }
    }
}
