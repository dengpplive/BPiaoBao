using GalaSoft.MvvmLight;
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

namespace BPiaoBao.Client.Account.View
{
    /// <summary>
    /// ProductBuyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProductBuyWindow : Window
    {
        public ProductBuyWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, "CloseProductBuy", p =>
            {
                if (p)
                {
                    Messenger.Default.Unregister<bool>(this, "CloseProductBuy");
                    Messenger.Default.Send<bool>(true, "HomeRefresh");
                    this.Close();
                }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            var vm = DataContext as ViewModelBase;
            if (vm != null)
                vm.Cleanup();
            base.OnClosed(e);
        }
    }
}
