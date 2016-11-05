using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Utils;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Client.Message.Command
{
    [ClientCommand("MyMessageCommand", EnumPushCommands.MyMessageTip)]
    public class MyMessageCommand : IClientCommand
    {
        public void Invoke(params object[] para)
        {
            if (para == null || para.Length < 1)
                return;
            var obj = para[1];
            var objs = (object[])obj;
            var id = objs[0];
            var qncontent = objs[1];
            Messenger.Default.Send(true, "mymessage");
            var action = new Action(() => DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                LocalUIManager.ShowMessageDetailsDialog(new MyMessageDto { ID = Convert.ToInt32(id) , QnContent = (string)qncontent }, () => Messenger.Default.Send(true, "mymessage"))
                )));
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() => UIManager.ShowRemindWindow(EnumPushCommands.MyMessageTip.ToEnumDesc(), (string)para[0], action)));
        }
    }
}
