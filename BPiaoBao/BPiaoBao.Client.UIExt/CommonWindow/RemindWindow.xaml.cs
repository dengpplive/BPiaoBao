using BPiaoBao.Client.UIExt.Helper.HTMLConverter;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace BPiaoBao.Client.UIExt.CommonWindow
{
    /// <summary>
    /// RemindWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RemindWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private Action action;


        public RemindWindow()
        {
            ShowInTaskbar = false;
            InitializeComponent();
            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = SystemParameters.WorkArea.Height;
            Topmost = true;

            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
        }

        public RemindWindow(string title, string html, Action action,bool isClickContentClose=true)
            : this()
        {
            this.Title = title;
            this.text = html;
            this.action = action;

            StringBuilder sb = new StringBuilder();
            //html = @"<html><head><meta http-equiv='content-type' content='textml;charset=UTF-8'></head><body scroll='no'><div style='margin:0px;padding:0px;font-family:Microsoft YaHei; background:none;'><strong><span style='color:#111111;font-size:14;font-family:SimHei;'>29推送测试用户，您目前信用账户欠款如下：</span><br/><br/><span style='color:#111111;font-size:12;'>账户欠款总额：￥888元， 最低还款额：￥888元，请在今日15:00点前还清“最低还款额”，否则您的账户将被冻结！</span></div></body><ml>";
            //sb.Append("<html><head><meta http-equiv='content-type' content='text/html;charset=UTF-8'></head>");
            //sb.Append(@"<body scroll='no'>");
            sb.Append(html);
            //sb.Append(@"</body></html>");
            var temp = sb.ToString();
            var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, true);

            using (System.IO.StringReader sr = new System.IO.StringReader(xaml))
            using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(sr))
                richTextBox.Document = (FlowDocument)System.Windows.Markup.XamlReader.Load(xr);

            if (isClickContentClose)
            {
                richTextBox.PreviewMouseDown += richTextBox_PreviewMouseDown;

                this.Closed += RemindWindow_Closed;
            }
            //webBrowser.LoadCompleted += webBrowser_LoadCompleted;
            //webBrowser.NavigateToString(temp);
        }

        void RemindWindow_Closed(object sender, EventArgs e)
        {

        }

        void richTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (action != null) action();
            Close();
        }

        //void webBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    mshtml.HTMLDocumentEvents2_Event doc = ((mshtml.HTMLDocumentEvents2_Event)webBrowser.Document);
        //    doc.onclick -= doc_onclick;
        //    doc.onclick += doc_onclick;
        //    doc.oncontextmenu += doc_oncontextmenu;
        //    doc.onmousedown += doc_onmousedown;
        //    doc.onmousemove += doc_onmousemove;
        //    System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());
        //}


        //bool doc_onclick(mshtml.IHTMLEventObj pEvtObj)
        //{
        //    if (action != null)
        //        action();
        //    Close();

        //    return false;
        //}

        void timer_Tick(object sender, EventArgs e)
        {
            var temp = Top - 10;
            if (temp <= SystemParameters.WorkArea.Height - Height)
            {
                temp = SystemParameters.WorkArea.Height - Height;
                timer.Tick -= timer_Tick;
                timer.Stop();
            }
            Top = temp;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
