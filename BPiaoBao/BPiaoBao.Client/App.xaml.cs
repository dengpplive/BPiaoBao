using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.ViewModel;
using System;
using System.Configuration;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System.Globalization;
using System.Threading;

namespace BPiaoBao.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary> 
    public partial class App
    {

        static App()
        {
            DispatcherHelper.Initialize();
        }

        public App()
        {
            var autoUpdatePath = System.IO.Path.Combine(System.Reflection.Assembly.GetExecutingAssembly().Location.Substring(0, System.Reflection.Assembly.GetExecutingAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar)), "AutoUpdate/AutoUpdate.exe");
            var currentAppName = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(System.Reflection.Assembly.GetEntryAssembly().Location.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1).Replace(".exe", "");
            var url = ConfigurationManager.AppSettings["UpdateUrl"];
            if (System.Diagnostics.Process.GetProcessesByName("AutoUpdate").Length == 0)
            {
                var pro = new System.Diagnostics.Process();
                if (System.IO.File.Exists(autoUpdatePath))
                {
                    pro.StartInfo.FileName = autoUpdatePath;
                    pro.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", currentAppName, url);
                    pro.Start();
                }
            }
            Current.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
            //设置系统默认日期格式
            var cultureInfo = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            cultureInfo.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            cultureInfo.DateTimeFormat.FullDateTimePattern = "yyyy-MM-dd HH:mm:ss";
            Thread.CurrentThread.CurrentCulture = cultureInfo;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            CommunicateManager.StopPushMessage();
            CommunicateManager.Invoke<AppServices.Contracts.ILoginService>(p => p.Exist(LoginInfo.Token), UIManager.ShowErr);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject == null)
                Logger.WriteLog(LogType.ERROR, "发生严重错误，并且e.ExceptionObject == null");
            else
            {
                var ex = (Exception)e.ExceptionObject;
                if (ex != null)
                    Logger.WriteLog(LogType.ERROR, ex.Message, ex);
                else
                    Logger.WriteLog(LogType.ERROR, e.ExceptionObject.ToString());
            }
        }

        static void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var comException = e.Exception as System.Runtime.InteropServices.COMException;

            if (comException != null && comException.ErrorCode == -2147221040)
            {
                Logger.WriteLog(LogType.ERROR, e.Exception.Message, e.Exception);
                UIManager.ShowMessage("复制数据失败");
                e.Handled = true;
                return;
            }

            if (e.Exception != null)
            {
                UIManager.ShowErr(e.Exception);
                Logger.WriteLog(LogType.ERROR, e.Exception.Message, e.Exception);
            }
            else
            {
                UIManager.ShowMessage("发生异常，并且e.Exception==null");
                Logger.WriteLog(LogType.ERROR, "发生异常，并且e.Exception==null", e.Exception);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //var win = new OpenTipWindow();
            // win.Show(); 
            // win.Close();
            //初始化主窗体，增快登录成功后切换速度
            ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;

            var login = new Login();
            var mainWindow = new MainWindow();
            
  
            var result = login.ShowDialog();

            if (result == null || result.Value == false)
            {
                Current.Shutdown();
                return;
            }

            var splashScreen = new SplashScreen("SplashScreen.png");
            splashScreen.Show(true, true);

           

            mainWindow.Show();
            var vm = new MainViewModel();
            mainWindow.DataContext = vm;
            Current.MainWindow = mainWindow; 
        }

    }
}
