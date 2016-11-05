using BPiaoBao.Client.Module;
using System.Collections.Generic;

namespace BPiaoBao.Client.Message
{
    [Plugin(ProjectCode, "我的消息", "pack://application:,,,/BPiaoBao.Client.Message;component/Image/message_60.png", 5, MessageCode, true)]
    public class Main  : IPlugin
    {
        public const string ProjectCode = "7000";
        public const string MessageCode = "7001";

        public IEnumerable<MainMenuItem> Parts
        {
            get
            {
                return new List<MainMenuItem>
                {
                    new MainMenuItem
                    {
                        Name = "我的消息",
                        MenuCode = MessageCode,
                        Icon="pack://application:,,,/BPiaoBao.Client.Message;component/Image/message_30.png",     
                        //IsShowTip = true
                    }
                };
            }
        }
    }
}
