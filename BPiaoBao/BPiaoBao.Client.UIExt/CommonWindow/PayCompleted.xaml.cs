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
    /// PayCompleted.xaml 的交互逻辑
    /// </summary>
    public partial class PayCompleted : Window
    {
        public PayCompleted()
        {
            InitializeComponent();
        }


        public bool? IsOK
        {
            get { return (bool?)GetValue(IsOKProperty); }
            set { SetValue(IsOKProperty, value); }
        }
        public static readonly DependencyProperty IsOKProperty =
            DependencyProperty.Register("IsOK", typeof(bool?), typeof(PayCompleted), new PropertyMetadata(null));

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


        
    }
}
