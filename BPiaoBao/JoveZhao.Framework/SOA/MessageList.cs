using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.SOA
{
    public static class MessageExpend
    {
        public static void SetState(this List<MessageItem> source, MessageState state)
        {
            foreach (var s in source)
            {
                s.State = state;
            }
        }
    }
    public class MessageManager
    {
        private List<MessageItem> lst = new List<MessageItem>();

        public void AddMessage(MessageItem item)
        {
            lst.Add(item);
        }
        public List<MessageItem> GetByKey(string key)
        {
            var data = lst.Where(p => p.Key == key && p.State == MessageState.NewMessage).ToList();
            return data;
        }
        public void RemoveOld()
        {
            lst.RemoveAll(p => p.State == MessageState.Readed);
        }
    }

    public enum MessageState
    {
        NewMessage,
        Reading,
        Readed
    }
    public class MessageItem
    {
        public IMessage Message { get; set; }
        public string Key { get; set; }
        public MessageState State { get; set; }
    }

    public interface IMessage { }
    public interface IData
    {
    }
}
