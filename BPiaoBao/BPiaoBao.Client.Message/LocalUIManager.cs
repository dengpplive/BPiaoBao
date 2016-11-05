using System;
using System.Windows;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.Message.View;
using BPiaoBao.Client.Message.ViewModel;

namespace BPiaoBao.Client.Message
{
    internal class LocalUIManager
    {
        /// <summary>
        /// 显示消息详情窗口
        /// </summary>
        /// <param name="model"></param>
        /// <param name="action"></param>
        internal static void ShowMessageDetailsDialog(MyMessageDto model,Action action=null)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var win = new MessageDetailsWindow { Owner = Application.Current.MainWindow };
                var vm = new MessageDetailsViewModel(model);
                win.DataContext = vm;
                win.ShowDialog();
                if (action == null) return;
                action();
            }));
        }
    }
}
