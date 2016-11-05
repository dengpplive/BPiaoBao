using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    /// <summary>
    /// 个人中心视图模型
    /// </summary>
    public class UserSettingViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettingViewModel"/> class.
        /// </summary>
        public UserSettingViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            //HasAdminRight = LoginInfo.Account == "admin";
            HasAdminRight = LoginInfo.IsAdmin;
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                CurrentUserInfo = service.GetCurrentUserInfo();
                var list = service.GetAllOperators(null, LoginInfo.Account.ToLower(), null);
                OperatorDto = list.SingleOrDefault(p => String.Equals(p.Account, LoginInfo.Account, StringComparison.CurrentCultureIgnoreCase));
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion

        #region 公开属性

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


        #region   OperatorDto
        /// <summary>
        /// The <see cref="OperatorDto" /> property's name.
        /// </summary>
        private const string OperatorDtoPropertyName = "OperatorDto";

        private OperatorDto _operatorDto;

        /// <summary>
        /// 当前操作员信息
        /// </summary>
        public OperatorDto OperatorDto
        {
            get { return _operatorDto; }

            set
            {
                if (_operatorDto == value) return;

                RaisePropertyChanging(OperatorDtoPropertyName);
                _operatorDto = value;
                RaisePropertyChanged(OperatorDtoPropertyName);
            }
        }
        #endregion


        #region CurrentUserInfo

        /// <summary>
        /// The <see cref="CurrentUserInfo" /> property's name.
        /// </summary>
        private const string CurrentUserInfoPropertyName = "CurrentUserInfo";

        private CurrentUserInfoDto _currentUserInfo;

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public CurrentUserInfoDto CurrentUserInfo
        {
            get { return _currentUserInfo; }

            set
            {
                if (_currentUserInfo == value) return;

                RaisePropertyChanging(CurrentUserInfoPropertyName);
                _currentUserInfo = value;
                RaisePropertyChanged(CurrentUserInfoPropertyName);
            }
        }

        #endregion

        #region HasAdminRight

        /// <summary>
        /// The <see cref="HasAdminRight" /> property's name.
        /// </summary>
        private const string HasAdminRightPropertyName = "HasAdminRight";

        private bool _hasAdmin;

        /// <summary>
        /// 是否拥有管理员权限
        /// </summary>
        public bool HasAdminRight
        {
            get { return _hasAdmin; }

            set
            {
                if (_hasAdmin == value) return;

                RaisePropertyChanging(HasAdminRightPropertyName);
                _hasAdmin = value;
                RaisePropertyChanged(HasAdminRightPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region ChangePasswordCommand

        private RelayCommand _changePasswordCommand;

        /// <summary>
        /// 修改密码命令
        /// </summary>
        public RelayCommand ChangePasswordCommand
        {
            get
            {
                return _changePasswordCommand ?? (_changePasswordCommand = new RelayCommand(ExecuteChangePasswordCommand, CanExecuteChangePasswordCommand));
            }
        }

        private void ExecuteChangePasswordCommand()
        {
            LocalUIManager.ShowChangePassword();
        }

        private bool CanExecuteChangePasswordCommand()
        {
            return true;
        }

        #endregion

        #region ChangePayPasswordCommand

        private RelayCommand _changePayPasswordCommand;

        /// <summary>
        /// 修改支付密码
        /// </summary>
        public RelayCommand ChangePayPasswordCommand
        {
            get
            {
                return _changePayPasswordCommand ?? (_changePayPasswordCommand = new RelayCommand(ExecuteChangePayPasswordCommand, CanExecuteChangePayPasswordCommand));
            }
        }

        private void ExecuteChangePayPasswordCommand()
        {
            LocalUIManager.ShowChangePayPassword();
        }

        private bool CanExecuteChangePayPasswordCommand()
        {
            return _hasAdmin;
        }

        #endregion

        #endregion
    }
}
