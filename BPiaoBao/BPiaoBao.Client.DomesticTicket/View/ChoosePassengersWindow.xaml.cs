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
    /// ChoosePassengersWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChoosePassengersWindow : Window
    {
        public ChoosePassengersWindow()
        {
            InitializeComponent();
            
        }

        public ChoosePassengersWindow(int flag)
        {
            InitializeComponent();
            switch (flag)
            {
                case 0:
                    dg.Columns[1].Visibility = Visibility.Visible;
                    break;
                case 1:
                    dg.Columns[1].Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }
    }
}
