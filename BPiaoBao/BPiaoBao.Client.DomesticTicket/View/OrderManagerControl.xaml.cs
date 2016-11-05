using BPiaoBao.Client.Module;
using System.Windows;
using System.Windows.Input;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// 订单管理界面
    /// </summary>
    [Part(Main.OrderQueryCode)]
    public partial class OrderManagerControl : IPart
    {
        public OrderManagerControl()
        {
            InitializeComponent();
            KeyDown += OrderManagerControl_KeyDown;
            if (LocalUIManager.DefaultShowhiddenColumn)
            {
                dg.Columns[6].Visibility = Visibility.Visible;
                dg.Columns[7].Visibility = Visibility.Visible;
                dg.Columns[8].Visibility = Visibility.Collapsed;
            }
            else
            {
                dg.Columns[6].Visibility = Visibility.Collapsed;
                dg.Columns[7].Visibility = Visibility.Collapsed;
                dg.Columns[8].Visibility = Visibility.Visible;
            }
        }

        void OrderManagerControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F7) return;
            if (dg.Columns[6].Visibility == Visibility.Visible)
            {
                dg.Columns[6].Visibility = Visibility.Collapsed;
                dg.Columns[7].Visibility = Visibility.Collapsed;
                dg.Columns[8].Visibility = Visibility.Visible;
                LocalUIManager.DefaultShowhiddenColumn = false;
            }
            else
            {
                dg.Columns[6].Visibility = Visibility.Visible;
                dg.Columns[7].Visibility = Visibility.Visible;
                dg.Columns[8].Visibility = Visibility.Collapsed;
                LocalUIManager.DefaultShowhiddenColumn = true;
            }
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
            get { return "订单查询"; }
        }
    }
}
