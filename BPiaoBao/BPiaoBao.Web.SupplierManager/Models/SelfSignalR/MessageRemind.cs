using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using BPiaoBao.AppServices.Contracts.ServerMessages;
using System.Threading.Tasks;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.Web.SupplierManager.Models.SelfSignalR
{
    public class MessageRemind : Hub
    {
        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }
        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }
        /// <summary>
        /// 发送单个组
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <param name="roomName"></param>
        public void SendGroupMessage(EnumMessageCommand command,string title, string message,string roomName)
        {
            Clients.Group(roomName).Remind(command,title,message);
        }
        /// <summary>
        /// 发送给指定商户组
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <param name="roomList"></param>
        public void SendGroupList(EnumMessageCommand command,string title, string message, List<string> roomList)
        {
            Clients.Groups(roomList).Remind(command,title,message);
        }
        /// <summary>
        /// 发送给所有连接用户
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public void SendAllMessage(EnumMessageCommand command,string title, string message)
        {
            Clients.All.Remind(command,title, message);
        }
    }
}