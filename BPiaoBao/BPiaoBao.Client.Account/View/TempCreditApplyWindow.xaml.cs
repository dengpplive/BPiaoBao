using System.Windows;
using BPiaoBao.Client.Account.ViewModel;
using BPiaoBao.Client.UIExt;

namespace BPiaoBao.Client.Account.View
{
    /// <summary>
    /// TempCreditApplyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TempCreditApplyWindow : Window
    {
        public TempCreditApplyWindow()
        {
            InitializeComponent();
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                UIManager.ShowMessage("请输入支付密码");
                return;
            }
            //password不能绑定，手动触发
            var vm = DataContext as TempCreditApplyViewModel;
            if (vm == null)
                return;
                if (vm.ApplyTemporaryLimitCommand.CanExecute(PasswordBox.Password))
                    vm.ApplyTemporaryLimitCommand.Execute(PasswordBox.Password);
        }
    }
}
