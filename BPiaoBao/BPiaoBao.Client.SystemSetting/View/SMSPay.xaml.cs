using BPiaoBao.Client.UIExt;
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

namespace BPiaoBao.Client.SystemSetting.View
{
    /// <summary>
    /// SMSPay.xaml 的交互逻辑
    /// </summary>
    public partial class SMSPay : Window
    {
        public SMSPay()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, "CloseSMSPay", p =>
            {
                if (p)
                {
                    Messenger.Default.Unregister<bool>(this, "CloseSMSPay");
                    Messenger.Default.Send<bool>(true, "SMSRefresh");
                    this.Close();
                }
            });
        }
    }
}
