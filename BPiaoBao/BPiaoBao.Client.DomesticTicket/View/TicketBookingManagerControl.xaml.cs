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
using BPiaoBao.Client.Module;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// TicketBookingManageControl.xaml 的交互逻辑
    /// 机票预订管理导入页面
    /// </summary>
    [Part(Main.TicketBookingCode)]
    public partial class TicketBookingManagerControl : UserControl, IPart
    {
        public TicketBookingManagerControl()
        {
            InitializeComponent();
           
        }
         

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public object GetContent()
        {
            return this;
        }

        public string Title
        {
            get { return Main.TicketBookingName; }
        }
    }
}
