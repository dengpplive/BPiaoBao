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

namespace BPiaoBao.Client.SystemSetting.View
{
    /// <summary>
    /// BulletinControl.xaml 的交互逻辑
    /// </summary>
    [Part(Main.BulletinCode)]
    public partial class BulletinControl : UserControl,IPart
    {
        public BulletinControl()
        {
            InitializeComponent();
        }

        public object GetContent()
        {
            return this;
        }

        public string Title
        {
            get { return "公告管理"; }
        }
    }
}
