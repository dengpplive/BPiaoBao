using BPiaoBao.Client.UIExt.CommonWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BPiaoBao.Client.UIExt
{
    public sealed class MessageBoxExt
    {
        /// <summary>
        /// 显示支付成功界面
        /// </summary>
        /// <returns></returns>
        public static bool? ShowPay()
        {
            PayCompleted pay = new PayCompleted();
            pay.Owner = Application.Current.MainWindow;
            pay.ShowDialog();
            return pay.IsOK;
        }
        /// <summary>
        /// 对话框
        /// </summary>
        /// <param name="title">显示对话框标题</param>
        /// <param name="content">消息内容</param>
        /// <param name="messageImageType">显示图片</param>
        /// <param name="messageBoxButtonType">按钮方式</param>
        /// <returns></returns>
        public static bool? Show(string title, string content, MessageImageType messageImageType, MessageBoxButtonType messageBoxButtonType = MessageBoxButtonType.OK)
        {
            bool? result = null;
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //   {
            PopupWindow pop = new PopupWindow(content, messageImageType, messageBoxButtonType);
            pop.Title = title;
            if (Application.Current.MainWindow != pop)
                pop.Owner = Application.Current.MainWindow;
            pop.ShowDialog();
            result = pop.DialogResult;
            //}));
            return result;
        }
        ///// <summary>
        ///// 弹出窗口【默认大小600x400】
        ///// </summary>
        ///// <param name="title">标题</param>
        ///// <param name="content">内容</param>
        //public static void PopupShow(string title, object content)
        //{
        //    PopupShow(title, content, 600, 400);
        //}
        ///// <summary>
        ///// 弹出窗口
        ///// </summary>
        ///// <param name="title">标题</param>
        ///// <param name="content">内容</param>
        ///// <param name="width">宽度</param>
        ///// <param name="height">高度</param>
        //public static void PopupShow(string title, object content, double width, double height)
        //{
        //    PopupShow(title, content, width, height, string.Empty);
        //}
        ///// <summary>
        ///// 弹出窗口
        ///// </summary>
        ///// <param name="title">标题</param>
        ///// <param name="content">内容</param>
        ///// <param name="width">宽度</param>
        ///// <param name="height">高度</param>
        ///// <param name="icon">左上角图片显示路径【Uri路径】</param>
        //public static void PopupShow(string title, object content, double width, double height, string icon)
        //{
        //    CustomWindow customWindow = new CustomWindow();
        //    customWindow.AllowsTransparency = true;
        //    customWindow.WindowStyle = WindowStyle.None;
        //    customWindow.ResizeMode = ResizeMode.NoResize;
        //    customWindow.Background = new SolidColorBrush(Colors.Transparent);
        //    customWindow.Width = width;
        //    customWindow.Height = height;
        //    customWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        //    customWindow.Title = title;
        //    if (!string.IsNullOrEmpty(icon))
        //    {
        //        customWindow.Icon = new BitmapImage(new Uri(icon));
        //    }
        //    customWindow.Owner = Application.Current.MainWindow;
        //    customWindow.Content = content;
        //    customWindow.ShowDialog();
        //}
    }
}
