using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class PaySettingViewModel : BaseVM
    {
        #region 构造函数
        /// <summary>
        /// Initializes a new instance of the <see cref="PaySettingViewModel"/> class.
        /// </summary>
        public PaySettingViewModel()
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
            if (CanExecuteQueryBindCommand())
                ExecuteQueryBindCommand();
        }

        #endregion

        #region 公开属性
        #region IsBinded

        /// <summary>
        /// The <see cref="IsBinded" /> property's name.
        /// </summary>
        private const string IsBindedPropertyName = "IsBinded";

        private bool _isBinded;

        /// <summary>
        /// 是否已经绑定
        /// </summary>
        public bool IsBinded
        {
            get { return _isBinded; }

            set
            {
                if (_isBinded == value) return;

                RaisePropertyChanging(IsBindedPropertyName);
                _isBinded = value;
                RaisePropertyChanged(IsBindedPropertyName);
            }
        }

        #endregion
        #region TenPayEmail

        /// <summary>
        /// The <see cref="TenPayEmail" /> property's name.
        /// </summary>
        private const string TenPayEmailPropertyName = "TenPayEmail";

        private string _tenPayEmail;

        /// <summary>
        /// 财付通邮箱
        /// </summary>
        public string TenPayEmail
        {
            get { return _tenPayEmail; }

            set
            {
                if (_tenPayEmail == value) return;

                RaisePropertyChanging(TenPayEmailPropertyName);
                _tenPayEmail = value;
                RaisePropertyChanged(TenPayEmailPropertyName);
            }
        }

        #endregion
        #region AliPayEmail

        /// <summary>
        /// The <see cref="AliPayEmail" /> property's name.
        /// </summary>
        private const string AliPayEmailPropertyName = "AliPayEmail";

        private string _aliPayEmail;

        /// <summary>
        /// 支付宝邮箱
        /// </summary>
        public string AliPayEmail
        {
            get { return _aliPayEmail; }

            set
            {
                if (_aliPayEmail == value) return;

                RaisePropertyChanging(AliPayEmailPropertyName);
                _aliPayEmail = value;
                RaisePropertyChanged(AliPayEmailPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令
        #region QueryQueryBindCommand

        private RelayCommand _queryBindCommand;

        /// <summary>
        /// 查询是否已经绑定
        /// </summary>
        public RelayCommand QueryBindCommand
        {
            get
            {
                return _queryBindCommand ?? (_queryBindCommand = new RelayCommand(ExecuteQueryBindCommand, CanExecuteQueryBindCommand));
            }
        }

        private void ExecuteQueryBindCommand()
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                var info = p.GetBindAccount();
                AliPayEmail = info.AlipayAccount;
                IsBinded = !string.IsNullOrEmpty(AliPayEmail);
                TenPayEmail = info.TenpayAccount;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteQueryBindCommand()
        {
            return !IsBusy;
        }

        #endregion
        #region GetAliPaySignCommand

        private RelayCommand _getAliPaySignCommand;

        /// <summary>
        /// 绑定签约打开网页链接
        /// </summary>
        public RelayCommand GetAliPaySignCommand
        {
            get
            {
                return _getAliPaySignCommand ?? (_getAliPaySignCommand = new RelayCommand(ExecuteGetAliPaySignCommand, CanExecuteGetAliPaySignCommand));
            }
        }

        private void ExecuteGetAliPaySignCommand()
        {
            if (string.IsNullOrEmpty(AliPayEmail.Trim()))
            {
                UIManager.ShowMessage("请输入支付宝账户");
                return;
            }

            //var r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            //if (!r.IsMatch(AliPayEmail))
            //{
            //    UIManager.ShowMessage("请输入格式正确的邮箱地址");
            //    return;
            //}
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                var url = p.GetAlipaySign(AliPayEmail);
                UIManager.OpenDefaultBrower(url);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
            LocalUIManager.ShowConfirmPwd(AliPayEmail, IsBinded ? 1 : 0, p => Initialize()); 
        }

        private bool CanExecuteGetAliPaySignCommand()
        {
            return !IsBusy;
        }

        #endregion
        #region ShowConfirmPwdCommand

        private RelayCommand _showConfirmPwdCommand;

        /// <summary>
        /// 临时额度申请
        /// </summary>
        public RelayCommand ShowConfirmPwdCommand
        {
            get
            {
                return _showConfirmPwdCommand ?? (_showConfirmPwdCommand = new RelayCommand(ExecuteShowConfirmPwdCommand, CanExecuteShowConfirmPwdCommand));
            }
        }

        private void ExecuteShowConfirmPwdCommand()
        {
            if (string.IsNullOrEmpty(AliPayEmail.Trim()))
            {
                UIManager.ShowMessage("请输入支付宝账户");
                return;
            }
            //var r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            //if (!r.IsMatch(AliPayEmail))
            //{
            //    UIManager.ShowMessage("请输入格式正确的邮箱地址");
            //    return;
            //}
            LocalUIManager.ShowConfirmPwd(AliPayEmail, IsBinded ? 1 : 0, p => Initialize());
        }

        private bool CanExecuteShowConfirmPwdCommand()
        {
            return true;
        }

        #endregion
        #endregion
    }
}
