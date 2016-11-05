using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace BPiaoBao.Client.UIExt
{
    /// <summary>
    /// WebWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WebWindow : Window
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="WebWindow"/> class.
        /// </summary>
        public WebWindow()
        {
            InitializeComponent();
            //this.webBrowser.LoadCompleted += webBrowser_LoadCompleted;
            //this.webBrowser.Navigated += webBrowser_Navigated;
            this.webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
            this.webBrowser.Navigated += webBrowser_Navigated;

        }

        void webBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            //FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            //if (fiComWebBrowser == null) return;

            //object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            //if (objComWebBrowser == null) return;

            //objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });
        }

        void webBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            this.stProcess.Visibility = Visibility.Hidden;
            this.stWebBrower.Visibility = Visibility.Visible;
            this.stWebBrower.Height = this.Height-50;
            this.webBrowser.Height =(int)this.stWebBrower.Height;
        }

        void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });
        }

        void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            //    this.webBrowser.Visibility = Visibility.Visible;
            //    this.stProcess.Visibility = Visibility.Hidden;
            //    this.Background = Brushes.White;


        }




        #endregion

        #region 公开属性

        #region UriSource

        /// <summary>
        /// The <see cref="UriSource" /> dependency property's name.
        /// </summary>
        public const string UriSourcePropertyName = "UriSource";

        /// <summary>
        /// Gets or sets the value of the <see cref="UriSource" />
        /// property. This is a dependency property.
        /// </summary>
        public Uri UriSource
        {
            get
            {
                return (Uri)GetValue(UriSourceProperty);
            }
            set
            {
                SetValue(UriSourceProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="UriSource" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(
            UriSourcePropertyName,
            typeof(Uri),
            typeof(WebWindow),
            new PropertyMetadata(new PropertyChangedCallback((sender, e) =>
            {
                var window = sender as WebWindow;

                if (window == null) return;

                var action = new Action(() =>
                {
                    window.Dispatcher.Invoke(new Action(() =>
                    {
                        window.webBrowser.Navigate(e.NewValue as Uri);

                    }));
                });
                Task.Factory.StartNew(action);


            })));

        #endregion

        #endregion

    }
}
