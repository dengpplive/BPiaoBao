using BPiaoBao.Client.Account;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Account.Commands
{
    /// <summary>
    /// 提醒还款命令
    /// </summary>
    //[Export(typeof(IClientCommand))]
    //[ExportMetadata("CommandKey", EnumPushCommands.RemindRepayment)]
    [ClientCommand("RemindRepaymentCommand", EnumPushCommands.RemindRepayment)]
    public class RemindRepaymentCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            if (para == null || para.Length < 1)
                return;
            Main.Initlize();
            var action = new Action(() =>
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    PluginService.Run(Main.ProjectCode, Main.RepaymentCode);
                }));
            });

            UIManager.ShowRemindWindow(
                title: "还款提醒",
                text: (string)para[0],
                action: action);
        }
    }
}
