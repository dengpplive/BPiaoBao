using BPiaoBao.AppServices.Contracts;
using BPiaoBao.Client.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.DESCrypt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BPiaoBao.Client.ViewModel
{
    /// <summary>
    /// 登录界面视图模型
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel()
        {
            //检查是否记住密码
            LocalData cacheData = null;
            try
            {
                cacheData = IsolatedStorageHelper.Get<LocalData>();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.WARN, "读取本地配置错误", ex);
            }

            if (cacheData == null) return;
            Num = cacheData.Num;
            LoginName = cacheData.LoginName;
            LoginPwd = cacheData.LoginPwd;
            AutoLogin = cacheData.AutoLogin;
            RememberPassword = cacheData.RememberPassword;

            if (AutoLogin)
            {
                ExecuteLoginCommand();
            }
        }

        #endregion

        #region 公开属性

        #region Num

        /// <summary>
        /// The <see cref="Num" /> property's name.
        /// </summary>
        private const string NumPropertyName = "Num";

        private string _num;

        /// <summary>
        /// 用户号
        /// </summary>
        [Required(ErrorMessage = @"用户号必填")]
        public string Num
        {
            get { return _num; }

            set
            {
                if (_num == value) return;

                RaisePropertyChanging(NumPropertyName);
                _num = value;
                RaisePropertyChanged(NumPropertyName);

                if (_loginCommand != null)
                    _loginCommand.RaiseCanExecuteChanged();

                ValidationHelper.Instance.ValidateProperty(NumPropertyName, this, value);
            }
        }

        #endregion

        #region LoginName

        /// <summary>
        /// The <see cref="LoginName" /> property's name.
        /// </summary>
        private const string LoginNamePropertyName = "LoginName";

        private string _loginName;

        /// <summary>
        /// 登录账号
        /// </summary>       
        [Required(ErrorMessage = @"登陆名必填")]
        public string LoginName
        {
            get { return _loginName; }

            set
            {
                if (_loginName == value) return;

                RaisePropertyChanging(LoginNamePropertyName);
                _loginName = value;
                RaisePropertyChanged(LoginNamePropertyName);

                if (_loginCommand != null)
                    _loginCommand.RaiseCanExecuteChanged();

                ValidationHelper.Instance.ValidateProperty(LoginNamePropertyName, this, value);
            }
        }

        #endregion

        #region LoginPwd

        /// <summary>
        /// The <see cref="LoginPwd" /> property's name.
        /// </summary>
        private const string LoginPwdPropertyName = "LoginPwd";

        private string _loginPwd;

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required(ErrorMessage = @"登录密码必填")]
        public string LoginPwd
        {
            get { return _loginPwd; }

            set
            {
                if (_loginPwd == value) return;

                RaisePropertyChanging(LoginPwdPropertyName);
                _loginPwd = value;
                RaisePropertyChanged(LoginPwdPropertyName);

                if (_loginCommand != null)
                    _loginCommand.RaiseCanExecuteChanged();

                ValidationHelper.Instance.ValidateProperty(LoginPwdPropertyName, this, value);
            }
        }

        #endregion

        #region RememberPassword

        /// <summary>
        /// The <see cref="RememberPassword" /> property's name.
        /// </summary>
        private const string RememberPasswordPropertyName = "RememberPassword";

        private bool _rememberPassword;

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool RememberPassword
        {
            get { return _rememberPassword; }

            set
            {
                if (_rememberPassword == value) return;

                RaisePropertyChanging(RememberPasswordPropertyName);
                _rememberPassword = value;
                RaisePropertyChanged(RememberPasswordPropertyName);
            }
        }

        #endregion

        #region AutoLogin

        /// <summary>
        /// The <see cref="AutoLogin" /> property's name.
        /// </summary>
        private const string AutoLoginPropertyName = "AutoLogin";

        private bool _autoLogin;

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public bool AutoLogin
        {
            get { return _autoLogin; }

            set
            {
                if (_autoLogin == value) return;

                //自动登录，自然会记住密码
                if (value)
                    RememberPassword = true;

                RaisePropertyChanging(AutoLoginPropertyName);
                _autoLogin = value;
                RaisePropertyChanged(AutoLoginPropertyName);
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
        /// 是否正在忙
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (IsInDesignMode)
                    return true;
                return _isBusy;
            }

            set
            {
                if (_isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_loginCommand != null)
                    _loginCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Message

        /// <summary>
        /// The <see cref="Message" /> property's name.
        /// </summary>
        private const string MessagePropertyName = "Message";

        private string _message;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message
        {
            get
            {
                if (IsInDesignMode)
                    return "系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误系统错误";
                return _message;
            }

            set
            {
                if (_message == value) return;

                RaisePropertyChanging(MessagePropertyName);
                _message = value;
                RaisePropertyChanged(MessagePropertyName);
            }
        }

        #endregion

        #region IsLogined

        /// <summary>
        /// The <see cref="IsLogined" /> property's name.
        /// </summary>
        private const string IsLoginedPropertyName = "IsLogined";

        private bool _isLogined;

        /// <summary>
        /// 是否已经登录
        /// </summary>
        public bool IsLogined
        {
            get { return _isLogined; }

            set
            {
                if (_isLogined == value) return;

                RaisePropertyChanging(IsLoginedPropertyName);
                _isLogined = value;
                RaisePropertyChanged(IsLoginedPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region LoginCommand

        private RelayCommand _loginCommand;

        /// <summary>
        /// 登录命令.
        /// </summary>
        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand));
            }
        }

        private void ExecuteLoginCommand()
        {
            Message = null;
            IsBusy = true;

            Action act = () =>
            {
                if (RememberPassword || AutoLogin)
                    IsolatedStorageHelper.Save(LocalData.Transfer(this));
                else if (!RememberPassword)
                    IsolatedStorageHelper.Delete<LocalData>();

                CommunicateManager.Invoke<ILoginService>(p =>
                {
                    var token = p.Login(Num, LoginName,LoginPwd);
                    LoginInfo.Token = token;
                    LoginInfo.Code = Num.Trim();
                    LoginInfo.Account = LoginName.Trim();
                    LoginInfo.Guid = Guid.NewGuid();
                    LoginInfo.IsLogined = true;
                    //登录成功
                    IsLogined = true;

                }, ShowErr);
            };

            Task.Factory.StartNew(act).ContinueWith(task =>
            {
                CommunicateManager.StartPushMessage();
                InvokeSetBusy(false);

            });

        }

        private bool CanExecuteLoginCommand()
        {
            var can = ValidationHelper.Instance.ValidateModel(this) && !IsBusy;
            return can;
        }

        #endregion

        #endregion

        #region 私有方法

        private void InvokeSetBusy(bool busy)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                IsBusy = busy;
            }));
        }

        private void ShowErr(Exception ex)
        {
            Message = ex.Message;
        }

        #endregion
    }
}