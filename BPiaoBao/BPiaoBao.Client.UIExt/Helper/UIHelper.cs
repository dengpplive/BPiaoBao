using System.Reflection;
using System.Windows.Controls.Primitives;

namespace BPiaoBao.Client.UIExt.Helper
{
    public class UIHelper
    {
        //反射模拟 点击事件。。
        public static void PerformClick(ButtonBase button)
        {
            var method = button.GetType().GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(button, null);
            }
        }
    }
}
