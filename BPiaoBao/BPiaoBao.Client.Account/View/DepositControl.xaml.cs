using BPiaoBao.Client.Account.ViewModel;
using BPiaoBao.Client.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BPiaoBao.Client.UIExt;

namespace BPiaoBao.Client.Account.View
{
    /// <summary>
    /// DepositControl.xaml 的交互逻辑
    /// </summary>
    [Part(Main.DepositCode)]
    public partial class DepositControl : UserControl, IPart
    {
        public DepositControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetContent()
        {
            return this;
        }

        public string Title
        {
            get { return "充值"; }
        }

        #region 公开属性

        #region IsShowLogButton

        /// <summary>
        /// The <see cref="IsShowLogButton" /> dependency property's name.
        /// </summary>
        public const string IsShowLogButtonPropertyName = "IsShowLogButton";

        /// <summary>
        /// 是否显示充值按钮
        /// </summary>
        public bool IsShowLogButton
        {
            get
            {
                return (bool)GetValue(IsShowLogButtonProperty);
            }
            set
            {
                SetValue(IsShowLogButtonProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsShowLogButton" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowLogButtonProperty = DependencyProperty.Register(
            IsShowLogButtonPropertyName,
            typeof(bool),
            typeof(DepositControl),
            new PropertyMetadata(true));

        #endregion

        #endregion

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //password不能绑定，手动触发
            var vm = DataContext as DepositViewModel;
            if (vm == null)
                return;
            if (string.IsNullOrEmpty(aplipaypasswordBox.Password))
            {
                UIManager.ShowMessage("请输入支付密码");
                return;
            }
            if (ApliPayRadioButton.IsChecked != null && ApliPayRadioButton.IsChecked.Value)
            {
                if (vm.RechargeByQuikAliPayCommand.CanExecute(aplipaypasswordBox.Password))
                    vm.RechargeByQuikAliPayCommand.Execute(aplipaypasswordBox.Password);
            }
            //else
            //{
            //    //预留财付通绑定账户操作
            //}
        }
    }
}
