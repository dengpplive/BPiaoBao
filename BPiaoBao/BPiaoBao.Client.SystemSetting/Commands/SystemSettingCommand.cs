using System.Threading.Tasks;
using System.Windows;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
using BPiaoBao.Client.UIExt.Utils;
using BPiaoBao.Client.UIExt.Communicate;

namespace BPiaoBao.Client.SystemSetting.Commands
{

    /// <summary>
    /// 
    /// </summary>
    // [Export(typeof(IClientCommand))]
    //[ExportMetadata("CommandKey", EnumPushCommands.SystemSetting)]
    [ClientCommand("SystemSettingCommand", EnumPushCommands.SystemSetting)]
    public class SystemSettingCommand : IClientCommand
    {
        public void Invoke(params object[] para)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                PluginService.Run(Main.ProjectCode, Main.UserSettingCode);
            }));
        }
    }



    /// <summary>
    /// 
    /// </summary>
    //[Export(typeof(IClientCommand))]
    // [ExportMetadata("CommandKey", EnumPushCommands.NoticePage)]
    [ClientCommand("SystemSettingNoticeCommand", EnumPushCommands.NoticePage)]
    public class SystemSettingNoticeCommand : IClientCommand
    {

        public void Invoke(params object[] para)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                PluginService.Run(Main.ProjectCode, Main.BulletinCode);
            }));
        }
    }



    /// <summary>
    /// 
    /// </summary>
    //[Export(typeof(IClientCommand))]
    // [ExportMetadata("CommandKey", EnumPushCommands.LoginPopNoticeWindow)]
    [ClientCommand("SystemSettingLoginPopNoticeCommand", EnumPushCommands.LoginPopNoticeWindow)]
    public class SystemSettingLoginPopNoticeCommand : IClientCommand
    {

        public void Invoke(params object[] para)
        {
            if (!LoginInfo.IsLogined) return;
            if (para == null || para.Length < 1)
                return;

            Application.Current.Dispatcher.Invoke(new Action(() =>
                  {

                      try
                      {
                          var obj = para[1];
                          var objs = (object[])obj;
                          var idStr = Convert.ToString(objs[0]);
                          Logger.WriteLog(LogType.DEBUG, "登录后接收的通知ID字符：" + idStr);
                          if (idStr.Trim().Length <= 0) return;
                          var temp = idStr.Split(',');
                          var list = temp.Select(s => Convert.ToInt32(s)).ToList();
                          LocalUIManager.ShowLoginPopBulletionDetailsDialog(list.ToArray());
                      }
                      catch (Exception e)
                      {
                          Logger.WriteLog(LogType.ERROR, "登录后接收的通知ID字符异常。原因：" + e.Message);
                      }

                  }));

        }
    }

    /// <summary>
    /// 
    /// </summary>
    //[Export(typeof(IClientCommand))]
    //[ExportMetadata("CommandKey", EnumPushCommands.EnforcePopNoticeWindow)]
    [ClientCommand("SystemSettingEnforcePopNoticeCommand", EnumPushCommands.EnforcePopNoticeWindow)]
    public class SystemSettingEnforcePopNoticeCommand : IClientCommand
    {

        public void Invoke(params object[] para)
        {
            if (para == null || para.Length < 1)
                return;
            var id = (int)((object[])para[1])[0];
            var action = new Action(() => DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                LocalUIManager.ShowBulletionDetailsDialog(id);
                PluginService.Run(Main.ProjectCode, Main.BulletinCode);
            })));
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => UIManager.ShowRemindWindow(
                title: "最新公告",
                text: (string)para[0],
                action: action)));
        }
    }


    /// <summary>
    /// 一般信息命令
    /// </summary>
    //[Export(typeof(IClientCommand))]
    //[ExportMetadata("CommandKey", EnumPushCommands.NormalMsg)]
    [ClientCommand("NormalMsgCommand", EnumPushCommands.NormalMsg)]
    public class NormalMsgCommand : IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="para"></param>
        public void Invoke(params object[] para)
        {
            if (para == null || para.Length < 1)
                return; 
          

            UIManager.ShowNormalMsgWindow(
                title: EnumPushCommands.NormalMsg.ToEnumDesc(),
                text: (string)para[0],
                action: null);

        }
    }
}
