using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    /// <summary>
    /// 修改支付密码视图模型
    /// </summary>
    public class ChangePayPasswordViewModel : ChangePasswordViewModel
    {
        #region 公开属性

        #region IsSending

        /// <summary>
        /// The <see cref="IsSending" /> property's name.
        /// </summary>
        private const string IsSendingPropertyName = "IsSending";

        private bool _isSending;

        /// <summary>
        /// 是否正在发送短信
        /// </summary>
        public bool IsSending
        {
            get { return _isSending; }

            set
            {
                if (_isSending == value) return;

                RaisePropertyChanging(IsSendingPropertyName);
                _isSending = value;
                RaisePropertyChanged(IsSendingPropertyName);
            }
        }

        #endregion

        #region ValidateCode

        /// <summary>
        /// The <see cref="ValidateCode" /> property's name.
        /// </summary>
        private const string ValidateCodePropertyName = "ValidateCode";

        private string _validateCode;

        /// <summary>
        /// 短息验证码
        /// </summary>
        public string ValidateCode
        {
            get { return _validateCode; }

            set
            {
                if (_validateCode == value) return;

                RaisePropertyChanging(ValidateCodePropertyName);
                _validateCode = value;
                RaisePropertyChanged(ValidateCodePropertyName);
            }
        }

        #endregion

        #endregion

        #region 属性验证

        /// <summary>
        /// 获取具有给定名称的属性的错误信息。
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case ValidateCodePropertyName:
                        if (ValidateCode == null)
                            return "";
                        if (String.IsNullOrWhiteSpace(ValidateCode))
                            return "请输入短信验证码";
                        break;
                    case OldPasswordPropertyName:
                        return null;
                }
                return base[columnName];
            }
        }

        #endregion

        #region 公开命令

        #region GetValidateCodeCommand

        private RelayCommand _getValidateCodeCommand;

        /// <summary>
        /// 获取短信验证码命令
        /// </summary>
        public RelayCommand GetValidateCodeCommand
        {
            get
            {
                return _getValidateCodeCommand ?? (_getValidateCodeCommand = new RelayCommand(ExecuteGetValidateCodeCommand, CanExecuteGetValidateCodeCommand));
            }
        }

        private void ExecuteGetValidateCodeCommand()
        {
            IsSending = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var temp = service.GetValidateCode();
                var test = temp;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsSending = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteGetValidateCodeCommand()
        {
            return !_isSending;
        }

        #endregion

        #region 修改密码

        protected override void ExecuteSaveCommand()
        {
            IsBusy = true;
            ErrorMessage = null;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.SetPayPassword(NewPassword, ValidateCode);
                UIManager.ShowMessage("修改支付密码成功");
                IsChanged = true;
            }, ex =>
            {
                ErrorMessage = ex.Message;
            });

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        protected override bool CanExecuteSaveCommand()
        {
            var can = String.IsNullOrEmpty(ValidateNewPwd()) && String.IsNullOrEmpty(ValidateComparisonPassword());
            if (!can)
                return false;

            return !IsBusy && !String.IsNullOrWhiteSpace(ValidateCode);
        }

        #endregion

        #endregion


    }
}
