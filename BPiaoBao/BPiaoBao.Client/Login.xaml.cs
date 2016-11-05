using System.Diagnostics;
using BPiaoBao.Client.UIExt.Communicate;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BPiaoBao.Client
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            KeyDown += Login_KeyDown;
            //Messenger.Default.Register<bool>(this, p =>
            //{
            //    if (p && !string.IsNullOrEmpty(LoginInfo.Token))
            //    {
            //        this.Close();
            //        Messenger.Default.Unregister<bool>(this);
            //    }
            //});
        }

        void Login_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    PerformClick(btnLogin);
                    break;
                case Key.Escape:
                    Process.GetCurrentProcess().Kill();
                    break;
            }
        }

        //反射模拟 点击事件。。
        private void PerformClick(ButtonBase button)
        {
            var method = button.GetType().GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(button, null);
            }
        }

        private void Border_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
