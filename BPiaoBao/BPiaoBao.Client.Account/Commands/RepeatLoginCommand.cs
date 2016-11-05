using System;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.Account.Commands
{
    /// <summary>
    /// 下线提示
    /// </summary>
    //[Export(typeof(IClientCommand))]
    //[ExportMetadata("CommandKey", EnumPushCommands.RepeatLogin)]
    [ClientCommand("RepeatLoginCommand", EnumPushCommands.RepeatLogin)]
    public class RepeatLoginCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            LoginInfo.IsLogined = false;
            if (para == null || para.Length < 1)
                return;
            var action = new Action(() =>
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    UIManager.Restart();
                }));
            });

            UIManager.ShowRemindWindow(
                title: "下线提示",
                text: (string)para[0],
                action: action);
        }
    }
}
