using BPiaoBao.Client.DomesticTicket.ViewModel;
using BPiaoBao.Client.Module;
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

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// ReportAnalysisControl.xaml 的交互逻辑
    /// </summary>
    [Part("100132")]
    public partial class ReportAnalysisControl : UserControl, IPart
    {
        private ReportAnalysisViewModel vm;

        public ReportAnalysisControl()
        {
            InitializeComponent();
            vm = DataContext as ReportAnalysisViewModel;
            Loaded += ReportAnalysisControl_Loaded;
        }

        void ReportAnalysisControl_Loaded(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetContent()
        {
            return this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckDays();
        }


        public string Title
        {
            get { return "报表分析"; }
        }
    }
}
