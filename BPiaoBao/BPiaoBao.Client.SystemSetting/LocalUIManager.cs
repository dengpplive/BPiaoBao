using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.SystemSetting.View;
using BPiaoBao.Client.SystemSetting.ViewModel;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows;

namespace BPiaoBao.Client.SystemSetting
{
    internal class LocalUIManager
    {
        internal static void ShowOperatorDialog(OperatorDto operatorDto, Action<bool?> call)
        {
            //DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            //    {
            var addOperator = new AddOperator();

            var vm = operatorDto != null ? new AddOperatorViewModel(operatorDto) : new AddOperatorViewModel();

            addOperator.DataContext = vm;

            addOperator.Owner = Application.Current.MainWindow;
            var dialogResult = addOperator.ShowDialog();
            if (call != null)
                call(dialogResult);
            addOperator.DataContext = null;
            //}));
        }
        /// <summary>
        /// 启动浏览器
        /// </summary>
        /// <param name="url">显示URL</param>
        internal static void OpenBrowser(string url)
        {
            var proc = new System.Diagnostics.Process { StartInfo = { FileName = url } };
            proc.Start();
            var result = MessageBoxExt.ShowPay();
            if (result.HasValue && result.Value)
                Messenger.Default.Send(true, "CloseSMSPay");
        }

        /// <summary>
        /// 打开修改密码界面
        /// </summary>
        internal static void ShowChangePassword()
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new ChangePasswordWindow { Owner = Application.Current.MainWindow };
                window.ShowDialog();
            }));
        }

        /// <summary>
        /// 打开修改支付密码界面
        /// </summary>
        internal static void ShowChangePayPassword()
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new ChangePayPasswordWindow { Owner = Application.Current.MainWindow };
                window.ShowDialog();
            }));
        }


        /// <summary>
        /// 显示公告详情窗口
        /// </summary>
        /// <param name="model"></param>
        internal static void ShowBulletionDetailsDialog(NoticeDto model)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var win = new BulletinDetailsWindow { Owner = Application.Current.MainWindow };
                var vm = new BulletinDetailsViewModel();
                vm.InitData(model);
                win.DataContext = vm;
                win.ShowDialog();


            }));
        }

        /// <summary>
        /// 打开公告详情页面
        /// </summary>
        /// <param name="id"></param>
        internal static void ShowBulletionDetailsDialog(int id)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var win = new BulletinDetailsWindow { Owner = Application.Current.MainWindow };
                var vm = new BulletinDetailsViewModel();
                vm.InitData(id);
                win.DataContext = vm;
                win.ShowDialog();
            }));
        }


        /// <summary>
        /// 打开登录时的公告详情页面
        /// </summary>
        /// <param name="ids"></param>
        internal static void ShowLoginPopBulletionDetailsDialog(int[] ids)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var win = new BulletinDetailsPopWindow { Owner = Application.Current.MainWindow };
                var vm = new BulletinDetailsPopViewModel();
                vm.InitData(ids);
                win.DataContext = vm;
                win.ShowDialog();
            }));
        }
        /// <summary>
        /// 确认绑定与解绑
        /// </summary>
        internal static void ShowConfirmPwd(string email, int flag = 0, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var vm = new ConfirmPwdViewModel(email, flag);
                var window = new ConfirmPwdWindow() { Title = flag == 0 ? "确认绑定" : "确认解绑", Owner = Application.Current.MainWindow, DataContext = vm };
                var result = window.ShowDialog();
                if (call != null && vm.IsDone)
                    call(result);
            }));
        }
    }
}
