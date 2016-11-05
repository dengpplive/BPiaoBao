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

namespace BPiaoBao.Client.UserControl
{
    /// <summary>
    ///
    /// </summary>
    public class ExtTabItem : TabItem
    {
        static ExtTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtTabItem), new FrameworkPropertyMetadata(typeof(ExtTabItem)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn_close = this.Template.FindName("PART_Close", this) as Button;
            if (btn_close != null)
                btn_close.Click += btn_close_Click;
        }

        void btn_close_Click(object sender, RoutedEventArgs e)
        {
            TabControl tabControl = this.Parent as TabControl;
            if (tabControl != null)
                tabControl.Items.Remove(this);
        }


        public bool IsClose
        {
            get { return (bool)GetValue(IsCloseProperty); }
            set { SetValue(IsCloseProperty, value); }
        }
        public static readonly DependencyProperty IsCloseProperty =
            DependencyProperty.Register("IsClose", typeof(bool), typeof(ExtTabItem), new PropertyMetadata(true));


    }
}
