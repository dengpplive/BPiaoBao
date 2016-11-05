using BPiaoBao.Client.DomesticTicket.ViewModel;
using BPiaoBao.Client.UIExt;
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
    /// OrderInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OrderInfoWindow : Window
    {
        public OrderInfoWindow()
        {
            InitializeComponent();
        }

        public OrderInfoWindow(int flag)
        {
            InitializeComponent();
            switch (flag)
            {
                case 0:
                    dg.Columns[7].Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    dg.Columns[7].Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
