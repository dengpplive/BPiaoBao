using System.Linq;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class ChangePasswordViewModel : ViewModelBase, IDataErrorInfo
    {
        #region 属性

        #region OldPassword

        /// <summary>
        /// The <see cref="OldPassword" /> property's name.
        /// </summary>
        protected const string OldPasswordPropertyName = "OldPassword";

        private string _oldPassword;

        /// <summary>
        /// 原始密码
        /// </summary>
        public string OldPassword
        {
            get { return _oldPassword; }

            set
            {
                if (_oldPassword == value) return;

                RaisePropertyChanging(OldPasswordPropertyName);
                _oldPassword = value;
                RaisePropertyChanged(OldPasswordPropertyName);
            }
        }

        #endregion

        #region NewPassword

        /// <summary>
        /// The <see cref="NewPassword" /> property's name.
        /// </summary>
        private const string NewPasswordPropertyName = "NewPassword";

        private string _newPassword;

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword
        {
            get { return _newPassword; }

            set
            {
                if (_newPassword == value) return;

                RaisePropertyChanging(NewPasswordPropertyName);
                _newPassword = value;
                RaisePropertyChanged(NewPasswordPropertyName);
            }
        }

        #endregion

        #region ComparisonPassword

        /// <summary>
        /// The <see cref="ComparisonPassword" /> property's name.
        /// </summary>
        private const string ComparisonPasswordPropertyName = "ComparisonPassword";

        private string _comparisonPassword;

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ComparisonPassword
        {
            get { return _comparisonPassword; }

            set
            {
                if (_comparisonPassword == value) return;

                RaisePropertyChanging(ComparisonPasswordPropertyName);
                _comparisonPassword = value;
                RaisePropertyChanged(ComparisonPasswordPropertyName);
            }
        }

        #endregion

        #region IsChanged

        /// <summary>
        /// The <see cref="IsChanged" /> property's name.
        /// </summary>
        private const string IsChangedPropertyName = "IsChanged";

        private bool _isChanged;

        /// <summary>
        /// 是否已修改
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }

            set
            {
                if (_isChanged == value) return;

                RaisePropertyChanging(IsChangedPropertyName);
                _isChanged = value;
                RaisePropertyChanged(IsChangedPropertyName);
            }
        }

        #endregion

        #region ErrorMessage

        /// <summary>
        /// The <see cref="ErrorMessage" /> property's name.
        /// </summary>
        private const string ErrorMessagePropertyName = "ErrorMessage";

        private string _errorMessage;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return IsInDesignMode ? "原始密码错误" : _errorMessage;
            }

            set
            {
                if (_errorMessage == value) return;

                RaisePropertyChanging(ErrorMessagePropertyName);
                _errorMessage = value;
                RaisePropertyChanged(ErrorMessagePropertyName);
            }
        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        private const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_saveCommand != null)
                    _saveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #endregion

        #region 命令

        #region SaveCommand

        private RelayCommand _saveCommand;

        /// <summary>
        /// Gets the SaveCommand.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand));
            }
        }

        protected virtual void ExecuteSaveCommand()
        {
            IsBusy = true;
            ErrorMessage = null;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                p.ChangePassword(LoginInfo.Account, _oldPassword, _newPassword);
                UIManager.ShowMessage("修改密码成功");
                IsChanged = true;
            }, ex =>
            {
                ErrorMessage = ex.Message;
            });

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        protected virtual bool CanExecuteSaveCommand()
        {
            return ValidationProperties.All(property => string.IsNullOrEmpty(GetValidationError(property)));
        }

        #endregion

        #endregion

        #region 验证属性
        static readonly string[] ValidationProperties = 
        {
            "OldPassword",
            "NewPassword",
            "ComparisonPassword"
        };
        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidationProperties, propertyName) < 0)
                return null;
            var error = string.Empty;
            switch (propertyName)
            {
                case "OldPassword":
                    error = ValidateOldPwd();
                    break;
                case "NewPassword":
                    error = ValidateNewPwd();
                    break;
                case "ComparisonPassword":
                    error = ValidateComparisonPassword();
                    break;
            }
            return error;
        }

        private string ValidateOldPwd()
        {
            return string.IsNullOrEmpty(OldPassword) ? "*" : string.Empty;
        }

        protected string ValidateNewPwd()
        {
            return string.IsNullOrEmpty(NewPassword)  ? "*" : (NewPassword.Length < 6 ? "密码长度至少6位" : string.Empty);
        }

        protected string ValidateComparisonPassword()
        {
            if (string.IsNullOrEmpty(ComparisonPassword)) return "*";
            return NewPassword != ComparisonPassword ? "确认密码不一致" : string.Empty;
        }
        public string Error
        {
            get { return string.Empty; }
        }
        public virtual string this[string columnName]
        {
            get
            {
                return GetValidationError(columnName);
            }
        }
        #endregion

    }
}
