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

namespace BPiaoBao.Client.UIExt
{
    public enum MessageBoxButtonType
    {
        OK,
        OKCancel
    }
    public enum MessageImageType
    {
        Error,
        Warning,
        Question,
        Success,
        Info
    }

    /// <summary>
    /// PopupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupWindow : Window
    {
        public PopupWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupWindow"/> class.
        /// </summary>
        /// <param name="content">消息内容.</param>
        /// <param name="messageImageType">图片类型.</param>
        /// <param name="messageBoxButtonType">按钮类型.</param>
        public PopupWindow(string content, MessageImageType messageImageType, MessageBoxButtonType messageBoxButtonType)
            : this()
        {
            Message = content;
            MessageImageType = messageImageType;
            MessageBoxButtonType = messageBoxButtonType;

            switch (MessageImageType)
            {
                case MessageImageType.Error:
                    ImageSource = new Uri("pack://application:,,,/BPiaoBao.Client.UIExt;component/Image/error.png");
                    break;
                case MessageImageType.Warning:
                    ImageSource = new Uri("pack://application:,,,/BPiaoBao.Client.UIExt;component/Image/warning.png");
                    break;
                case MessageImageType.Question:
                    ImageSource = new Uri("pack://application:,,,/BPiaoBao.Client.UIExt;component/Image/question.png");
                    break;
                case MessageImageType.Success:
                    ImageSource = new Uri("pack://application:,,,/BPiaoBao.Client.UIExt;component/Image/success.png");
                    break;
                case MessageImageType.Info:
                    ImageSource = new Uri("pack://application:,,,/BPiaoBao.Client.UIExt;component/Image/info.png");
                    break;
                default:
                    ImageSource = new Uri("pack://application:,,,/BPiaoBao.Client.UIExt;component/Image/info.png");
                    break;
            }

            switch (MessageBoxButtonType)
            {
                case UIExt.MessageBoxButtonType.OK:
                    BtnCancel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case UIExt.MessageBoxButtonType.OKCancel:
                    BtnCancel.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
        }

        #region Message

        /// <summary>
        /// The <see cref="Message" /> dependency property's name.
        /// </summary>
        public const string MessagePropertyName = "Message";

        /// <summary>
        /// Gets or sets the value of the <see cref="Message" />
        /// property. This is a dependency property.
        /// </summary>
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Message" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            MessagePropertyName,
            typeof(string),
            typeof(PopupWindow),
            new PropertyMetadata(null));

        #endregion

        #region MessageBoxButtonType

        /// <summary>
        /// The <see cref="MessageBoxButtonType" /> dependency property's name.
        /// </summary>
        public const string MessageBoxButtonTypePropertyName = "MessageBoxButtonType";

        /// <summary>
        /// Gets or sets the value of the <see cref="MessageBoxButtonType" />
        /// property. This is a dependency property.
        /// </summary>
        public MessageBoxButtonType MessageBoxButtonType
        {
            get
            {
                return (MessageBoxButtonType)GetValue(MessageBoxButtonTypeProperty);
            }
            set
            {
                SetValue(MessageBoxButtonTypeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MessageBoxButtonType" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageBoxButtonTypeProperty = DependencyProperty.Register(
            MessageBoxButtonTypePropertyName,
            typeof(MessageBoxButtonType),
            typeof(PopupWindow),
            new PropertyMetadata(MessageBoxButtonType.OK));

        #endregion

        #region MessageImageType

        /// <summary>
        /// The <see cref="MessageImageType" /> dependency property's name.
        /// </summary>
        public const string MessageImageTypePropertyName = "MessageImageType";

        /// <summary>
        /// Gets or sets the value of the <see cref="MessageImageType" />
        /// property. This is a dependency property.
        /// </summary>
        public MessageImageType MessageImageType
        {
            get
            {
                return (MessageImageType)GetValue(MessageImageTypeProperty);
            }
            set
            {
                SetValue(MessageImageTypeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MessageImageType" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageImageTypeProperty = DependencyProperty.Register(
            MessageImageTypePropertyName,
            typeof(MessageImageType),
            typeof(PopupWindow),
            new PropertyMetadata(MessageImageType.Info));

        #endregion

        #region ImageSource

        /// <summary>
        /// The <see cref="ImageSource" /> dependency property's name.
        /// </summary>
        public const string ImageSourcePropertyName = "ImageSource";

        /// <summary>
        /// Gets or sets the value of the <see cref="ImageSource" />
        /// property. This is a dependency property.
        /// </summary>
        public Uri ImageSource
        {
            get
            {
                return (Uri)GetValue(ImageSourceProperty);
            }
            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ImageSource" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            ImageSourcePropertyName,
            typeof(Uri),
            typeof(PopupWindow),
            new PropertyMetadata(null));

        #endregion

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Btn_OKCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
