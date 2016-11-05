using System.Text.RegularExpressions;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt.CommonWindow;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Microsoft.Win32;

namespace BPiaoBao.Client.UIExt
{
    public interface IClientCommandData
    {
        EnumPushCommands CommandKey { get; }
    } 
    /// <summary>
    /// 各种界面管理类
    /// </summary>
    public class UIManager
    {
       

        /// <summary>
        /// Initializes the <see cref="UIManager"/> class.
        /// </summary>
        static UIManager()
        {
          
        }

        public static void Initlize()
        {
            //什么都不用做
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowErr(Exception ex)
        {
            try
            {
                string message = null;
                if (ex != null)
                    message = ex.Message;

                if (ex != null && ex.InnerException != null)
                    message += " " + ex.InnerException.Message;

                Logger.WriteLog(LogType.ERROR, message, ex);

                if (Application.Current == null)
                {
                    if (message == null)
                        message = "未发现错误信息";

                    //出现严重问题时，可能会出现该Application.Current 为null
                    Logger.WriteLog(LogType.ERROR, "Application.Current==null");
                    NeedRestart(ex);
                    return;
                }

                if (Application.Current.Dispatcher == null)
                {
                    Logger.WriteLog(LogType.ERROR, "Application.Current.Dispatcher==null");
                    return;
                }

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBoxExt.Show("提示", message, MessageImageType.Info);
                    NeedRestart(ex);
                }));
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
        }

        //检查是否需要重启
        private static void NeedRestart(Exception ex)
        {
            try
            {
                if (!(ex is FaultException)) return;
                var temp = ex as FaultException;
                if (temp.Code.Name != "403") return;
                Restart();
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
        }

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="message"></param>
        public static void ShowMessage(string message)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() => MessageBoxExt.Show("提示", message, MessageImageType.Info)));
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
        }

        /// <summary>
        /// 显示确认框
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool? ShowMessageDialog(string message)
        {
            bool? result = null;
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    result = MessageBoxExt.Show("提示", message, MessageImageType.Warning, MessageBoxButtonType.OKCancel);
                }));
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
            return result;
        }

        /// <summary>
        /// 显示支付成功界面
        /// </summary>
        public static bool? ShowPayWindow()
        {
            bool? result = false;
            try
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var pay = new PayCompleted {Owner = Application.Current.MainWindow};
                    pay.ShowDialog();
                    result = pay.IsOK;
                }));
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
            return result;
        }

        //多线程可能导致多次重启
        private static volatile bool _isRestaring;
        private static object syncobject = new object();
        public static void Restart()
        {
            lock (syncobject)
            {
                if (_isRestaring)
                    return;

                _isRestaring = true;
            }

            //重启
            if (Application.ResourceAssembly == null)
                Logger.WriteLog(LogType.ERROR, "Application.ResourceAssembly==null");
            else
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();

            //isRestaring = false;
        }

        /// <summary>
        /// 显示web窗体
        /// </summary>
        /// <param name="title"></param>
        /// <param name="source"></param>
        public static void ShowWeb(string title, string source)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new WebWindow
                {
                    Owner = Application.Current.MainWindow,
                    Title = title,
                    UriSource = new Uri(source, UriKind.RelativeOrAbsolute)
                };
                window.ShowDialog();

            }));
        }

        /// <summary>
        /// 执行客户端命令
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="param">The parameter.</param>
        public static void InvokeCommand(EnumPushCommands command, params object[] param)
        {
            //if (importer.ClientCommands == null)
            //    return;

            //foreach (var item in importer.ClientCommands.Where(item => item.Metadata.CommandKey == command))
            //    {
            //        item.Value.Invoke(param);
            //        return;
            //    }

            //UIManager.ShowMessage("没有找到目标模块");

            var items = PluginManager.ClientCommandModules;
            if (items == null) return;
            foreach (var item in items.Where(item => (EnumPushCommands) item.Value.Command == command))
            {
                item.Key.Invoke(param);
                return;
            }

            //UIManager.ShowMessage("没有找到目标模块");

        }

        /// <summary>
        /// 显示提醒窗口
        /// </summary>
        /// <param name="title">窗口标题.</param>
        /// <param name="text">窗口内容</param>   
        /// <param name="action">按钮结果.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void ShowRemindWindow(string title, string text, Action action)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var window = new RemindWindow(title, text, action) { Owner = Application.Current.MainWindow };

                    window.Show();
                }));
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
        }


        /// <summary>
        /// 显示一般信息提醒窗口
        /// </summary>
        /// <param name="title">窗口标题.</param>
        /// <param name="text">窗口内容</param>   
        /// <param name="action">按钮结果.</param>
        /// <param name="isClickContentClose">是否点击内容关闭</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void ShowNormalMsgWindow(string title, string text, Action action,bool isClickContentClose = false)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var window = new RemindWindow(title, text, action,isClickContentClose) { Owner = Application.Current.MainWindow };

                    window.Show();
                }));
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
        }

        public static void ShowExportProgress(ExportProgressViewModel vm)
        {
            try
            {
                var action = new Action(() =>
                {
                    var window = new ExportProgressWindow { Owner = Application.Current.MainWindow, DataContext = vm };

                    window.ShowDialog();
                });

                if (Application.Current.Dispatcher.CheckAccess())
                    action();
                else
                    Application.Current.Dispatcher.Invoke(action);
            }
            catch (Exception exx)
            {
                Logger.WriteLog(LogType.ERROR, exx.Message, exx);
            }
        }

        /// <summary>
        /// 打开默认浏览器
        /// </summary>
        /// <param name="url"></param>
        public static void OpenDefaultBrower(string url)
        {
            try
            {
                Logger.WriteLog(LogType.DEBUG, "通过注册表打开默认浏览器");
                var key = Registry.ClassesRoot.OpenSubKey(@"http\shell\open\command\");
                if (key == null) return;
                var s = key.GetValue("").ToString();
                var reg = new Regex("\"([^\"]+)\"");
                var matchs = reg.Matches(s);
                if (matchs.Count <= 0) return;
                var filename = matchs[0].Groups[1].Value;
                System.Diagnostics.Process.Start(filename, url);
            }
            catch (Exception e1)
            {

                Logger.WriteLog(LogType.ERROR, "通过注册表打开默认浏览器失败。原因：" + e1.Message);
                try
                {
                    System.Diagnostics.Process.Start("iexplore.exe", url);

                }
                catch (Exception e2)
                {
                    Logger.WriteLog(LogType.ERROR, "直接打开默认浏览器出现异常。原因：" + e2.Message);
                    throw new CustomException(10032, "请设置IE为默认浏览器");
                }
            }
            //ShowWeb("", url);
        }

        /// <summary>
        /// 显示取消订单窗口
        /// </summary>
        /// <param name="isShowCancelPnrInfo"></param>
        /// <param name="isChecked"></param>
        /// <returns></returns>
        public static bool? ShowCancelOrderWindow(bool isShowCancelPnrInfo, out bool isChecked)
        {
            bool? result = false;
            try
            {
                bool? ischk = false;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    var can = new CancelOrderConfirmWindow
                    {
                        Owner = Application.Current.MainWindow,
                        IsShowCancelPnrInfo = isShowCancelPnrInfo
                    };

                    can.ShowDialog();
                    result = can.IsOK;
                    ischk = can.IsChecked;
                }));
                isChecked = Convert.ToBoolean(ischk);
                return result;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR,ex.Message,ex);
                isChecked = false;
                return result;
            }
        }
    }
}
