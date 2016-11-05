using System.Globalization;
using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 转账视图模型
    /// </summary>
    public class TransferViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsLoading = true;

            Action action = () =>
            {
                //支付成功
                var homeVm = ViewModelLocator.Home;
                //刷新数据
                homeVm.RefreshAccountInfo();
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsLoading = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region 公开属性

        #region IsLoading

        /// <summary>
        /// The <see cref="IsLoading" /> property's name.
        /// </summary>
        private const string IsLoadingPropertyName = "IsLoading";

        private bool _isLoading;

        /// <summary>
        /// 是否在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (_isLoading == value) return;

                RaisePropertyChanging(IsLoadingPropertyName);
                _isLoading = value;
                RaisePropertyChanged(IsLoadingPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否在繁忙
        ///// </summary>
        //public bool IsBusy
        //{
        //    get { return isBusy; }

        //    set
        //    {
        //        if (isBusy == value) return;

        //        RaisePropertyChanging(IsBusyPropertyName);
        //        isBusy = value;
        //        RaisePropertyChanged(IsBusyPropertyName);
        //    }
        //}

        #endregion

        #region PayeeName

        /// <summary>
        /// The <see cref="PayeeName" /> property's name.
        /// </summary>
        private const string PayeeNamePropertyName = "PayeeName";

        private string _payeeName;

        /// <summary>
        /// 收款人姓名
        /// </summary>
        [Required(ErrorMessage = @"收款人姓名必填")]
        public string PayeeName
        {
            get { return _payeeName; }

            set
            {
                if (_payeeName == value) return;

                RaisePropertyChanging(PayeeNamePropertyName);
                _payeeName = value;
                RaisePropertyChanged(PayeeNamePropertyName);

                ValidationHelper.Instance.ValidateProperty(PayeeNamePropertyName, this, value);

                Action action = () => CommunicateManager.Invoke<IBusinessmanService>(service =>
                {
                    var result = service.GetBusinessmanName(value);
                    if (result == null) return;
                    ValidationResult = result;
                    IsPayeeNameValid = true;
                }, ex =>
                {
                    ValidationResult = "商户号不存在";
                    IsPayeeNameValid = false;
                });

                IsValidatingName = true;
                ValidationResult = null;
                Task.Factory.StartNew(action).ContinueWith(task =>
                {
                    IsValidatingName = false;
                });
            }
        }

        #endregion

        #region ValidationResult

        /// <summary>
        /// The <see cref="ValidationResult" /> property's name.
        /// </summary>
        private const string ValidationResultPropertyName = "ValidationResult";

        private string _validationResult;

        /// <summary>
        /// 检查结果
        /// </summary>
        public string ValidationResult
        {
            get { return _validationResult; }

            set
            {
                if (_validationResult == value) return;

                RaisePropertyChanging(ValidationResultPropertyName);
                _validationResult = value;
                RaisePropertyChanged(ValidationResultPropertyName);
            }
        }

        #endregion

        #region IsPayeeNameValid

        /// <summary>
        /// The <see cref="IsPayeeNameValid" /> property's name.
        /// </summary>
        private const string IsPayeeNameValidPropertyName = "IsPayeeNameValid";

        private bool _isPayeeNameValid;

        /// <summary>
        /// 是否付款人合法
        /// </summary>
        public bool IsPayeeNameValid
        {
            get { return _isPayeeNameValid; }

            set
            {
                if (_isPayeeNameValid == value) return;

                RaisePropertyChanging(IsPayeeNameValidPropertyName);
                _isPayeeNameValid = value;
                RaisePropertyChanged(IsPayeeNameValidPropertyName);
            }
        }

        #endregion

        #region IsValidatingName

        /// <summary>
        /// The <see cref="IsValidatingName" /> property's name.
        /// </summary>
        private const string IsValidatingNamePropertyName = "IsValidatingName";

        private bool _isValidatingName;

        /// <summary>
        /// 是否正在验证名称
        /// </summary>
        public bool IsValidatingName
        {
            get { return _isValidatingName; }

            set
            {
                if (_isValidatingName == value) return;

                RaisePropertyChanging(IsValidatingNamePropertyName);
                _isValidatingName = value;
                RaisePropertyChanged(IsValidatingNamePropertyName);
            }
        }

        #endregion

        #region Password

        /// <summary>
        /// The <see cref="Password" /> property's name.
        /// </summary>
        private const string PasswordPropertyName = "Password";

        private string _password;

        /// <summary>
        /// 支付密码
        /// </summary>
        public string Password
        {
            get { return _password; }

            set
            {
                if (_password == value) return;

                RaisePropertyChanging(PasswordPropertyName);
                _password = value;
                RaisePropertyChanged(PasswordPropertyName);
            }
        }

        #endregion

        #region Money

        /// <summary>
        /// The <see cref="Money" /> property's name.
        /// </summary>
        private const string MoneyPropertyName = "Money";

        private decimal _money;

        /// <summary>
        /// 转账金额
        /// </summary>
        public decimal Money
        {
            get { return _money; }

            set
            {
                if (_money == value) return;

                RaisePropertyChanging(MoneyPropertyName);
                //money = value;
                var temp = decimal.Parse(value.ToString("F2"));
                _money = temp;
                RaisePropertyChanged(MoneyPropertyName);
            }
        }

        #endregion

        #region Message

        ///// <summary>
        ///// The <see cref="Message" /> property's name.
        ///// </summary>
        //public const string MessagePropertyName = "Message";

        //private string message = null;

        ///// <summary>
        ///// 消息
        ///// </summary>
        //public string Message
        //{
        //    get { return message; }

        //    set
        //    {
        //        if (message == value) return;

        //        RaisePropertyChanging(MessagePropertyName);
        //        message = value;
        //        RaisePropertyChanged(MessagePropertyName);
        //    }
        //}

        #endregion

        #region Notes

        /// <summary>
        /// The <see cref="Notes" /> property's name.
        /// </summary>
        private const string NotesyPropertyName = "Notes";

        private string _notes = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string Notes
        {
            get { return _notes; }

            set
            {
                if (_notes == value) return;

                RaisePropertyChanging(NotesyPropertyName); 
                _notes = value;
                RaisePropertyChanged(NotesyPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region SwtichToLogViewCommand

        private RelayCommand _switchToLogViewCommand;

        /// <summary>
        /// 切换到最近日志
        /// </summary>
        public RelayCommand SwtichToLogViewCommand
        {
            get
            {
                return _switchToLogViewCommand ?? (_switchToLogViewCommand = new RelayCommand(ExecuteSwtichToLogViewCommand));
            }
        }

        private void ExecuteSwtichToLogViewCommand()
        {
          if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.TransferLogCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {
                PluginService.Run(Main.ProjectCode, Main.TransferLogCode);
            }  
          
            
        }

        #endregion

        #region TransferCommand

        private RelayCommand _transferCommand;

        /// <summary>
        /// 转账命令
        /// </summary>
        public RelayCommand TransferCommand
        {
            get
            {
                return _transferCommand ?? (_transferCommand = new RelayCommand(ExecuteTransferCommand, CanExecuteTransferCommand));
            }
        }

        private void ExecuteTransferCommand()
        {
            //Message = null;
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IFundService>(service =>
            {
                service.InnerTransfer(_payeeName, _money.ToString(CultureInfo.InvariantCulture), _password,_notes);
                //清除数据
                CleanTxt();
                //Message = "转账成功";// String.Format("转账成功 金额：【￥{0}】，时间：{1}", money, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                UIManager.ShowMessage("转账成功");
                //支付成功
                var homeVm = ViewModelLocator.Home;
                //刷新还款数据
                homeVm.RefreshAccountInfo();

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteTransferCommand()
        {
            var can = !String.IsNullOrWhiteSpace(_payeeName) && !String.IsNullOrWhiteSpace(_password) && _money > 0 && IsPayeeNameValid && !IsValidatingName;
            return can;
        }

        #endregion

        /// <summary>
        /// 页面加载后触发
        /// </summary>
        protected override void ExecutePageLoadCommand()
        {
            if (!IsBusy)
                CleanTxt();

            if (!IsValidatingName)
                ValidationResult = null;
        }

        private void CleanTxt()
        {
            try
            {
                PayeeName = Password = null;
            }
            catch (ValidationException)
            {
                //PayeeName 为null时，会触发
            }
            Money = 0;
            Password = null;
            Notes = null;
            //Message = null;
        }

        #endregion
    }
}
