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

namespace BPiaoBao.Client.UIExt.CommonWindow
{
    /// <summary>
    /// CancelOrderConfirmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CancelOrderConfirmWindow : Window
    {
        public CancelOrderConfirmWindow()
        {
            InitializeComponent();
        }

        //public static readonly DependencyProperty IsCheckedProperty =
        //    DependencyProperty.Register("IsChecked", typeof (bool?), typeof (CancelOrderConfirmWindow),
        //        new PropertyMetadata(
        //            new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
        //            {
        //                var c = sender as CancelOrderConfirmWindow;
        //                if (c != null)
        //                {
        //                    c.IsChecked = e.NewValue as bool?;
        //                }

        //            }))));

        public bool? IsChecked
        {
            get { return this.ChkBox.IsChecked; }
            set { this.ChkBox.IsChecked = value; }
        }

        public bool? IsOK
        {
            get { return (bool?)GetValue(IsOKProperty); }
            set { SetValue(IsOKProperty, value); }
        }
        public static readonly DependencyProperty IsOKProperty =
            DependencyProperty.Register("IsOK", typeof(bool?), typeof(CancelOrderConfirmWindow), new PropertyMetadata(null));

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            this.IsOK = true;
            this.Close();
        }

        private void Btn_Error_Click(object sender, RoutedEventArgs e)
        {
            this.IsOK = false;
            this.Close();
        }

        public bool IsShowCancelPnrInfo
        {
            set { this.SpCanStackPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
            get { return this.SpCanStackPanel.Visibility==Visibility.Visible; }
        }

    }
}
