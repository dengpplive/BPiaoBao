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
    /// AfterSaleOrderControl.xaml 的交互逻辑
    /// </summary>
    [Part(Main.AfterSaleOrderCode)]
    public partial class AfterSaleOrderControl : UserControl,IPart
    {
        public AfterSaleOrderControl()
        {
            InitializeComponent();
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


        public string Title
        {
            get { return "售后订单"; }
        }
    }
}
