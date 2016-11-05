using BPiaoBao.Client.Module;
using BPiaoBao.Client.TPOS.ViewModel;
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

namespace BPiaoBao.Client.TPOS.View
{
    /// <summary>
    /// TransactionSummary.xaml 的交互逻辑
    /// </summary>
    [Part(Main.TransactionSummaryCode)]
    public partial class TransactionSummary : UserControl, IPart
    {
        private TransactionSummaryViewModel vm;

        public TransactionSummary()
        {
            InitializeComponent();
            vm = DataContext as TransactionSummaryViewModel;
            Loaded += TransactionSummary_Loaded;
        }

        void TransactionSummary_Loaded(object sender, RoutedEventArgs e)
        {
            CheckDays();
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

        /// <summary>
        /// 获取标题
        /// </summary>
        public string Title
        {
            get { return Main.TransactionSummaryName; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckDays();
        }

        private void CheckDays()
        {
            var temp = new DateTime(vm.SelectedYear, vm.SelectedMonth, 1);
            var endTime = temp.AddMonths(1).AddDays(-1);

            column29.Visibility = column30.Visibility = column31.Visibility = System.Windows.Visibility.Collapsed;

            if (endTime.Day > 28)
                column29.Visibility = System.Windows.Visibility.Visible;
            if (endTime.Day > 29)
                column30.Visibility = System.Windows.Visibility.Visible;
            if (endTime.Day > 30)
                column31.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
