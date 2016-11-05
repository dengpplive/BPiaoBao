using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using System.Drawing.Printing;
using Brushes = System.Windows.Media.Brushes;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// PrintTravel.xaml 的交互逻辑
    /// </summary>
    public partial class PrintTravelWindow : Window
    {
         
        public PrintTravelWindow()
        {
            InitializeComponent();

        }

    }
}
