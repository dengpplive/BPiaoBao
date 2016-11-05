using System.Windows.Controls;
using BPiaoBao.Client.Module;

namespace BPiaoBao.Client.SystemSetting.View
{
    /// <summary>
    /// PaySettingControl.xaml 的交互逻辑
    /// </summary>
    [Part(Main.PaySettingCode)]
    public partial class PaySettingControl : UserControl, IPart
    {
        public PaySettingControl()
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
            get { return "支付设置"; }
        }
    }
}
