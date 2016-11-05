using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BPiaoBao.Client.UIExt.CommonControl
{
    /// <summary>
    /// PayControl.xaml 的交互逻辑
    /// </summary>
    public partial class PayControl : UserControl
    {
        public PayControl()
        {
            InitializeComponent();
        }

        #region 公开属性

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> dependency property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        /// <summary>
        /// 是否正在忙
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return (bool)GetValue(IsBusyProperty);
            }
            set
            {
                SetValue(IsBusyProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsBusy" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            IsBusyPropertyName,
            typeof(bool),
            typeof(PayControl),
            new PropertyMetadata(false, new PropertyChangedCallback((sender, e) =>
            {
                PayControl control = (PayControl)sender;
                control.SetBtnEnable(!(bool)e.NewValue);
            })));

        private void SetBtnEnable(bool isEnable)
        {
            payBtn.IsEnabled = payByBankBtn.IsEnabled = payByPlatformBtn.IsEnabled = isEnable;
        }

        #endregion

        #region HeadTemplate

        /// <summary>
        /// The <see cref="HeadTemplate" /> dependency property's name.
        /// </summary>
        public const string HeadTemplatePropertyName = "HeadTemplate";

        /// <summary>
        /// 头模板
        /// </summary>
        public ControlTemplate HeadTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(HeadTemplateProperty);
            }
            set
            {
                SetValue(HeadTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="HeadTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeadTemplateProperty = DependencyProperty.Register(
            HeadTemplatePropertyName,
            typeof(ControlTemplate),
            typeof(PayControl),
            new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
            {
                PayControl control = sender as PayControl;
                control.headContent.Template = e.NewValue as ControlTemplate;
            })));

        #endregion

        #region CashBalance

        /// <summary>
        /// The <see cref="CashBalance" /> dependency property's name.
        /// </summary>
        public const string CashBalancePropertyName = "CashBalance";

        /// <summary>
        /// 现金余额
        /// </summary>
        public decimal CashBalance
        {
            get
            {
                return (decimal)GetValue(CashBalanceProperty);
            }
            set
            {
                SetValue(CashBalanceProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CashBalance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CashBalanceProperty = DependencyProperty.Register(
            CashBalancePropertyName,
            typeof(decimal),
            typeof(PayControl),
            new PropertyMetadata(decimal.Zero));

        #endregion

        #region CreditBalance

        /// <summary>
        /// The <see cref="CreditBalance" /> dependency property's name.
        /// </summary>
        public const string CreditBalancePropertyName = "CreditBalance";

        /// <summary>
        /// 信用余额
        /// </summary>
        public decimal CreditBalance
        {
            get
            {
                return (decimal)GetValue(CreditBalanceProperty);
            }
            set
            {
                SetValue(CreditBalanceProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CreditBalance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CreditBalanceProperty = DependencyProperty.Register(
            CreditBalancePropertyName,
            typeof(decimal),
            typeof(PayControl),
            new PropertyMetadata(decimal.Zero));

        #endregion

        #region SelectedBankCode

        /// <summary>
        /// The <see cref="SelectedBankCode" /> dependency property's name.
        /// </summary>
        public const string SelectedBankCodePropertyName = "SelectedBankCode";

        /// <summary>
        /// 当前选中银行卡
        /// </summary>
        public string SelectedBankCode
        {
            get
            {
                return (string)GetValue(SelectedBankCodeProperty);
            }
            set
            {
                SetValue(SelectedBankCodeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedBankCode" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBankCodeProperty = DependencyProperty.Register(
            SelectedBankCodePropertyName,
            typeof(string),
            typeof(PayControl),
            new PropertyMetadata(null));

        #endregion

        #region SelectedPlatform

        /// <summary>
        /// The <see cref="SelectedPlatform" /> dependency property's name.
        /// </summary>
        public const string SelectedPlatformPropertyName = "SelectedPlatform";

        /// <summary>
        /// 当前选中的平台
        /// </summary>
        public string SelectedPlatform
        {
            get
            {
                return (string)GetValue(SelectedPlatformProperty);
            }
            set
            {
                SetValue(SelectedPlatformProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedPlatform" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedPlatformProperty = DependencyProperty.Register(
            SelectedPlatformPropertyName,
            typeof(string),
            typeof(PayControl),
            new PropertyMetadata(null));

        #endregion

        #endregion

        #region 公开命令

        #region PayByBankCommand

        /// <summary>
        /// The <see cref="PayByBankCommand" /> dependency property's name.
        /// </summary>
        public const string PayByBankCommandPropertyName = "PayByBankCommand";

        /// <summary>
        /// 按银行卡支付
        /// </summary>
        [Category("命令"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand PayByBankCommand
        {
            get
            {
                return (ICommand)GetValue(PayByBankCommandProperty);
            }
            set
            {
                SetValue(PayByBankCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PayByBankCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PayByBankCommandProperty = DependencyProperty.Register(
            PayByBankCommandPropertyName,
            typeof(ICommand),
            typeof(PayControl),
            new PropertyMetadata((ICommand)null,
            new PropertyChangedCallback((sender, e) =>
            {

            })));

        #endregion

        #region PayByPlatformCommand

        /// <summary>
        /// The <see cref="PayByPlatformCommand" /> dependency property's name.
        /// </summary>
        public const string PayByPlatformCommandPropertyName = "PayByPlatformCommand";

        /// <summary>
        /// Gets or sets the value of the <see cref="PayByPlatformCommand" />
        /// property. This is a dependency property.
        /// </summary>
        [Category("命令"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand PayByPlatformCommand
        {
            get
            {
                return (ICommand)GetValue(PayByPlatformCommandProperty);
            }
            set
            {
                SetValue(PayByPlatformCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PayByPlatformCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PayByPlatformCommandProperty = DependencyProperty.Register(
            PayByPlatformCommandPropertyName,
            typeof(ICommand),
            typeof(PayControl),
            new PropertyMetadata(null));

        #endregion

        #region PayByCashbagCommand

        /// <summary>
        /// The <see cref="PayByCashbagCommand" /> dependency property's name.
        /// </summary>
        public const string PayByCashbagCommandPropertyName = "PayByCashbagCommand";

        /// <summary>
        /// Gets or sets the value of the <see cref="PayByCashbagCommand" />
        /// property. This is a dependency property.
        /// </summary>
        [Category("命令"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand PayByCashbagCommand
        {
            get
            {
                return (ICommand)GetValue(PayByCashbagCommandProperty);
            }
            set
            {
                SetValue(PayByCashbagCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PayByCashbagCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PayByCashbagCommandProperty = DependencyProperty.Register(
            PayByCashbagCommandPropertyName,
            typeof(ICommand),
            typeof(PayControl),
            new PropertyMetadata(null));

        #endregion

        #region PayByCreditCommand

        /// <summary>
        /// The <see cref="PayByCreditCommand" /> dependency property's name.
        /// </summary>
        public const string PayByCreditCommandPropertyName = "PayByCreditCommand";

        /// <summary>
        /// Gets or sets the value of the <see cref="PayByCreditCommand" />
        /// property. This is a dependency property.
        /// </summary>
        [Category("命令"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand PayByCreditCommand
        {
            get
            {
                return (ICommand)GetValue(PayByCreditCommandProperty);
            }
            set
            {
                SetValue(PayByCreditCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PayByCreditCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PayByCreditCommandProperty = DependencyProperty.Register(
            PayByCreditCommandPropertyName,
            typeof(ICommand),
            typeof(PayControl),
            new PropertyMetadata(null));

        #endregion

        #region PayByQuikAliPayCommand

        /// <summary>
        /// The <see cref="PayByQuikAliPayCommand" /> dependency property's name.
        /// </summary>
        public const string PayByQuikAliPayCommandPropertyName = "PayByQuikAliPayCommand";

        /// <summary>
        /// Gets or sets the value of the <see cref="PayByQuikAliPayCommand" />
        /// property. This is a dependency property.
        /// </summary>
        [Category("命令"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand PayByQuikAliPayCommand
        {
            get
            {
                return (ICommand)GetValue(PayByQuikAliPayCommandProperty);
            }
            set
            {
                SetValue(PayByQuikAliPayCommandProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PayByQuikAliPayCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PayByQuikAliPayCommandProperty = DependencyProperty.Register(
            PayByQuikAliPayCommandPropertyName,
            typeof(ICommand),
            typeof(PayControl),
            new PropertyMetadata(null));

        #endregion

        #endregion

        #region 控件事件

        private void payBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(passwordBox.Password))
            {
                UIManager.ShowMessage("请输入密码");
                return;
            }

            if (radioBtnCashbag.IsChecked != null && radioBtnCashbag.IsChecked.Value)
            {
                if (PayByCashbagCommand != null && PayByCashbagCommand.CanExecute(passwordBox.Password))
                    PayByCashbagCommand.Execute(passwordBox.Password);
            }
            else if (radioBtnCredit.IsChecked != null && radioBtnCredit.IsChecked.Value)
            {
                if (PayByCreditCommand != null && PayByCreditCommand.CanExecute(passwordBox.Password))
                    PayByCreditCommand.Execute(passwordBox.Password);
            }
        }

        private void payByBankBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedBankCode == null)
            {
                UIManager.ShowMessage("请选择银行卡");
                return;
            }
            if (PayByBankCommand != null && PayByBankCommand.CanExecute(SelectedBankCode))
                PayByBankCommand.Execute(SelectedBankCode);
        }

        private void payByPlatform_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPlatform == null)
            {
                UIManager.ShowMessage("请选择支付平台");
                return;
            }
            if (PayByPlatformCommand != null && PayByPlatformCommand.CanExecute(SelectedPlatform))
                PayByPlatformCommand.Execute(SelectedPlatform);
        }

        #endregion

        private void PayByQuikAliPayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(aplipaypasswordBox.Password))
            {
                UIManager.ShowMessage("请输入密码");
                return;
            }
            if (ApliPayRadioButton.IsChecked != null && ApliPayRadioButton.IsChecked.Value)
            {
                if (PayByQuikAliPayCommand != null && PayByQuikAliPayCommand.CanExecute(aplipaypasswordBox.Password))
                    PayByQuikAliPayCommand.Execute(aplipaypasswordBox.Password);
            }
            //else
            //{
            //    //财付通操作
            //}
        }

    }
}
