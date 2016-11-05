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

namespace BPiaoBao.Client.UIExt.CommonControl
{
    /// <summary>
    /// NoticeBrowerControl.xaml 的交互逻辑
    /// </summary>
    public partial class NoticeBrowerControl : UserControl
    {
        public NoticeBrowerControl()
        {
            InitializeComponent();
          
        }

        public const string HtmlStringPropertyName = "HtmlString";

        public static readonly DependencyProperty HtmlStringProperty =
            DependencyProperty.Register(HtmlStringPropertyName, typeof(string), typeof(NoticeBrowerControl),
                new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
                {
                    var c = (NoticeBrowerControl) sender;
                    if (c != null)
                    {
                        var html = (string) e.NewValue;
                        if (!string.IsNullOrEmpty(html))
                        {
                            c.WebBrowser.NavigateToString(html); 
                        }
                    }

                })));


        public string HtmlString
        {
            get { return (string)GetValue(HtmlStringProperty); }
            set { SetValue(HtmlStringProperty, value); }
        }



    }
}
