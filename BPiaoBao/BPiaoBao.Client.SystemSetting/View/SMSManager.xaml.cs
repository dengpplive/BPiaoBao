using BPiaoBao.Client.Module;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPiaoBao.Client.SystemSetting.View
{
    /// <summary>
    /// SMSManager.xaml 的交互逻辑
    /// </summary>
    [Part(Main.SmsCode)]
    public partial class SMSManager : UserControl, IPart
    {
        public SMSManager()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, p =>
            {
                if (p)
                {
                    SMSPay smsPay = new SMSPay();
                    smsPay.Owner = Application.Current.MainWindow;
                    smsPay.ShowDialog();
                }
            });
        }

        public object GetContent()
        {
            return this;
        }



        public string Title
        {
            get { return "短信管理"; }

        }
    }
}
