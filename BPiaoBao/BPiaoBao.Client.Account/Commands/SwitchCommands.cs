using BPiaoBao.Client.Module;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Account.Commands
{

    //[Export(typeof(IClientCommand))]
    //[ExportMetadata("CommandKey", EnumPushCommands.Cash)]
    [ClientCommand("CashCommand", EnumPushCommands.Cash)]
    public class CashCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            Main.Initlize();
            PluginService.Run(Main.ProjectCode, Main.CashCode);
        }
    }

    // [Export(typeof(IClientCommand))]
    // [ExportMetadata("CommandKey", EnumPushCommands.Credit)]
    [ClientCommand("CreditCommand", EnumPushCommands.Credit)]
    public class CreditCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            Main.Initlize();
            PluginService.Run(Main.ProjectCode, Main.CreditCode);
        }
    }

    // [Export(typeof(IClientCommand))]
    //  [ExportMetadata("CommandKey", EnumPushCommands.Finance)]
    [ClientCommand("FinanceCommand", EnumPushCommands.Finance)]
    public class FinanceCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            Main.Initlize();
            PluginService.Run(Main.ProjectCode, Main.FinanceCode);
        }
    }

    //   [Export(typeof(IClientCommand))]
    // [ExportMetadata("CommandKey", EnumPushCommands.Points)]
    [ClientCommand("PointsCommand", EnumPushCommands.Points)]
    public class PointsCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            Main.Initlize();
            PluginService.Run(Main.ProjectCode, Main.PointCode);
        }
    }
}
