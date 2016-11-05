using System;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class ConfirmPwdViewModel : BaseVM
    {
        #region 构造函数
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmPwdViewModel"/> class.
        /// </summary>
        public ConfirmPwdViewModel(string email, int flag = 0)
        {
            if (IsInDesignMode)
                return;
            Email = email;
            Flag = flag;
            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {

        }

        #endregion

        #region 公开属性
        /// <summary>
        /// 解绑与绑定的标识
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string Email { get; set; }
        #region IsDone

        /// <summary>
        /// The <see cref="IsDone" /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

        /// <summary>
        /// 是否已经申请
        /// </summary>
        public bool IsDone
        {
            get { return _isDone; }

            set
            {
                if (_isDone == value) return;

                RaisePropertyChanging(IsDonePropertyName);
                _isDone = value;
                RaisePropertyChanged(IsDonePropertyName);
            }
        }

        #endregion
        #region IsDoing

        /// <summary>
        /// The <see cref="IsDoing" /> property's name.
        /// </summary>
        private const string IsDoingPropertyName = "IsDoing";

        private bool _isDoing;

        /// <summary>
        /// 正在繁忙
        /// </summary>
        public bool IsDoing
        {
            get { return _isDoing; }

            set
            {
                if (_isDoing == value) return;

                RaisePropertyChanging(IsDoingPropertyName);
                _isDoing = value;
                RaisePropertyChanged(IsDoingPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令
        #region AlipayBindCommand

        private RelayCommand<string> _alipayBindCommand;

        /// <summary>
        /// 绑定
        /// </summary>
        public RelayCommand<string> AlipayBindCommand
        {
            get
            {
                return _alipayBindCommand ?? (_alipayBindCommand = new RelayCommand<string>(ExecuteAlipayBindCommand, CanExecuteAlipayBindCommand));
            }
        }

        private void ExecuteAlipayBindCommand(string password)
        {
            IsDoing = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                p.AlipayBind(Email, password);
                UIManager.ShowMessage("绑定成功！");
                IsDone = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsDoing = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteAlipayBindCommand(string password)
        {
            return !IsDoing && !string.IsNullOrEmpty(password);
        }

        #endregion
        #region AlipayUnBindCommand

        private RelayCommand<string> _alipayUnBindCommand;

        /// <summary>
        /// 解绑
        /// </summary>
        public RelayCommand<string> AlipayUnBindCommand
        {
            get
            {
                return _alipayUnBindCommand ?? (_alipayUnBindCommand = new RelayCommand<string>(ExecuteAlipayUnBindCommand, CanExecuteAlipayUnBindCommand));
            }
        }

        private void ExecuteAlipayUnBindCommand(string password)
        {
            IsDoing = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(p =>
            {
                p.AlipayUnBind(password);
                UIManager.ShowMessage("解绑成功！");
                IsDone = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsDoing = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteAlipayUnBindCommand(string password)
        {
            return  !IsDoing && !string.IsNullOrEmpty(password);
        }

        #endregion
        #endregion
    }
}
