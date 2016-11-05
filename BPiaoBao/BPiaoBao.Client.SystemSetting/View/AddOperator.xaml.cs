using BPiaoBao.Client.Module;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPiaoBao.Client.SystemSetting.View
{
    /// <summary>
    /// AddOperator.xaml 的交互逻辑
    /// </summary>
    public partial class AddOperator : Window
    {
        public AddOperator()
        {
            InitializeComponent();
            //Messenger.Default.Register<bool>(this, "CloseAddOperator", p =>
            //{
            //    //注销
            //    Messenger.Default.Unregister<bool>(this, "CloseAddOperator");
            //    //发送消息 刷新列表
            //    if (p)
            //    {
            //        Messenger.Default.Send<bool>(true, "RefreshOperator");
            //    }
            //    this.Close();
            //});
        }
    }
}
