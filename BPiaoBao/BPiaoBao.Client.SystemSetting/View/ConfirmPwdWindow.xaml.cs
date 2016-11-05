using System.Windows;
using BPiaoBao.Client.SystemSetting.ViewModel;
using BPiaoBao.Client.UIExt;

namespace BPiaoBao.Client.SystemSetting.View
{
    /// <summary>
    /// 确认支付设置绑定与解绑ConfirmPwdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmPwdWindow : Window
    {
        public ConfirmPwdWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 确认绑定（解绑）按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AplipaypasswordBox.Password))
            {
                UIManager.ShowMessage("请输入支付密码");
                return;
            }
            //password不能绑定，手动触发
            var vm = DataContext as ConfirmPwdViewModel;
            if (vm == null)
                return;
            switch (vm.Flag)
            {
                case 0://绑定
                    if (vm.AlipayBindCommand.CanExecute(AplipaypasswordBox.Password))
                        vm.AlipayBindCommand.Execute(AplipaypasswordBox.Password);
                    break;
                case 1://解绑
                    if (vm.AlipayUnBindCommand.CanExecute(AplipaypasswordBox.Password))
                        vm.AlipayUnBindCommand.Execute(AplipaypasswordBox.Password);
                    break;
            }
        }
    }
}
