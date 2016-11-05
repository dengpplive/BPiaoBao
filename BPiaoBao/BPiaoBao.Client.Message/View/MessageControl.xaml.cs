using System.Windows.Controls;
using BPiaoBao.Client.Module;

namespace BPiaoBao.Client.Message.View
{
    /// <summary>
    /// MessageControl.xaml 的交互逻辑
    /// </summary>
    [Part(Main.MessageCode)] 
    public partial class MessageControl : UserControl, IPart
    {
        public MessageControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        public object GetContent()
        {
            return this;
        }

        /// <summary>
        /// 获取标题
        /// </summary>
        public string Title
        {
            get { return "我的消息"; }
        }
    }
}
