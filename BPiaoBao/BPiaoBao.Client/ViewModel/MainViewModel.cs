using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.Model;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Utils;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Client.ViewModel
{
    /// <summary>
    /// 主窗体视图模型
    /// </summary>
    public class MainViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
                return;
            try
            {
                Modules = new List<PluginAttribute>();
                PluginManager.Register();
                PluginManager.Modules.ForEach(p => Modules.Add(p));
                if (Modules.Count > 0)
                {
                    CurrentPlugin = Modules[0];
                }
                GetVersion();
                Initialize();
                //注册
                var action = new Action(PluginManager.RegisterClientCommand);
                Task.Factory.StartNew(action).ContinueWith(task => CommunicateManager.Invoke<INoticeService>(p => p.GetLoginPopNotice(), UIManager.ShowErr));
                //全局tipcount赋值处理
                GlobalData.ClientMainVm = this;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.Message, ex);
                UIManager.ShowErr(ex);
            }
        }



        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            GetNoticeList();
            GetUserInfoAsyn();
            var homeControl = PluginManager.FindItem(CurrentPlugin.Code, CurrentPlugin.HomeCode);
            if (homeControl as System.Windows.Controls.UserControl == null) return;
            var vm = (homeControl as System.Windows.Controls.UserControl).DataContext as BaseVM;
            if (vm != null)
                vm.Initialize();//刷新当前模块首页数据
        }

        private void GetVersion()
        {
            var action = new Action(() => DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (dir == null) return;
                var streamReader = new StreamReader(Path.Combine(dir, "AutoUpdate\\Version.txt"));
                var localVersion = streamReader.ReadToEnd();
                streamReader.Close();
                Version = localVersion.Replace("Version=", "") + "Beta版";
            })));
            Task.Factory.StartNew(action);
        }

        private void GetUserInfoAsyn()
        {
            //读取用户信息
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                CurrentUserInfoDto = service.GetCurrentUserInfo();
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion

        #region 公开属性

        /// <summary>
        /// 插件模块
        /// </summary>
        public List<PluginAttribute> Modules { get; set; }

        /// <summary>
        /// 当前模块名
        /// </summary>
        private PluginAttribute _currentPlugin;
        public PluginAttribute CurrentPlugin
        {
            get { return _currentPlugin; }
            set
            {
                if (_currentPlugin == value) return;
                _currentPlugin = value;
                RaisePropertyChanged("CurrentPlugin");
                RaisePropertyChanged("CurrentModule");

                PluginService.Run(value.Code, value.HomeCode, true);
            }
        }

        /// <summary>
        /// 模块菜单
        /// </summary>
        public IPlugin CurrentModule
        {
            get
            {
                return PluginManager.GetPlugin(CurrentPlugin.Code);
            }
        }

        #region Version

        /// <summary>
        /// The <see cref="Version" /> property's name.
        /// </summary>
        private const string VersionPropertyName = "Version";

        private string _version;

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get { return _version; }

            set
            {
                if (_version == value) return;

                RaisePropertyChanging(VersionPropertyName);
                _version = value;
                RaisePropertyChanged(VersionPropertyName);
            }
        }

        #endregion

        #region CurrentUserInfoDto

        /// <summary>
        /// The <see cref="CurrentUserInfoDto" /> property's name.
        /// </summary>
        private const string CurrentUserInfoDtoPropertyName = "CurrentUserInfoDto";

        private CurrentUserInfoDto _currentUserInfoDto;

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public CurrentUserInfoDto CurrentUserInfoDto
        {
            get { return _currentUserInfoDto; }

            set
            {
                if (_currentUserInfoDto == value) return;

                RaisePropertyChanging(CurrentUserInfoDtoPropertyName);
                _currentUserInfoDto = value;
                RaisePropertyChanged(CurrentUserInfoDtoPropertyName);
            }
        }

        #endregion

        #region NoticeList

        private const string NoticeListPropertyName = "NoticeList";
        private List<LocalNoticeDto> _noticeList;

        public List<LocalNoticeDto> NoticeList
        {
            get { return _noticeList; }
            set
            {
                if (_noticeList == value) return;
                RaisePropertyChanging(NoticeListPropertyName);
                _noticeList = value;
                RaisePropertyChanged(NoticeListPropertyName);
            }
        }

        #endregion



        #endregion

        #region 公开命令

        #region SwitchViewCommand

        private RelayCommand<string> _switchViewCommand;

        /// <summary>
        /// 切换页面命令
        /// </summary>
        public RelayCommand<string> SwitchViewCommand
        {
            get
            {
                return _switchViewCommand ?? (_switchViewCommand = new RelayCommand<string>(ExecuteSwitchViewCommand, CanExecuteSwitchViewCommand));
            }
        }

        private void ExecuteSwitchViewCommand(string code)
        {
            switch (code)
            {
                //现金帐户
                case "cash":
                    UIManager.InvokeCommand(EnumPushCommands.Cash);
                    break;
                //信用帐户
                case "credit":
                    UIManager.InvokeCommand(EnumPushCommands.Credit);
                    break;
                //理财
                case "finance":
                    UIManager.InvokeCommand(EnumPushCommands.Finance);
                    break;
                //积分
                case "points":
                    UIManager.InvokeCommand(EnumPushCommands.Points);
                    break;
                //设置
                case "setting":
                    UIManager.InvokeCommand(EnumPushCommands.SystemSetting);
                    break;
                ////充值
                //case "recharge":
                //    PluginService.Run(BPiaoBao.Client.Account.Main.ProjectCode, BPiaoBao.Client.Account.Main.DepositCode);
                //    break;
                ////结算
                //case "withdraw":
                //    PluginService.Run(BPiaoBao.Client.Account.Main.ProjectCode, BPiaoBao.Client.Account.Main.WithdrawDepositCode);
                //    break;
                ////转账
                //case "transfer":
                //    PluginService.Run(BPiaoBao.Client.Account.Main.ProjectCode, BPiaoBao.Client.Account.Main.TransferCode);
                //    break;
                ////所有理财
                //case "allfinance":
                //    PluginService.Run(BPiaoBao.Client.Account.Main.ProjectCode, BPiaoBao.Client.Account.Main.AllFinanceCode);
                //    break;
                ////还款
                //case "repayment":
                //    PluginService.Run(BPiaoBao.Client.Account.Main.ProjectCode, BPiaoBao.Client.Account.Main.RepaymentCode);
                //    break;
                ////订单管理
                //case "order":
                //    PluginService.Run(BPiaoBao.Client.DomesticTicket.Main.ProjectCode, BPiaoBao.Client.DomesticTicket.Main.OrderCode);
                //    break;
            }
        }

        private bool CanExecuteSwitchViewCommand(string code)
        {
            return true;
        }

        #endregion

        #region LogoutCommand

        private RelayCommand _logoutCommand;

        /// <summary>
        /// 注销登录
        /// </summary>
        public RelayCommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new RelayCommand(ExecuteLogoutCommand, CanExecuteLogoutCommand));
            }
        }

        private void ExecuteLogoutCommand()
        {
            var dialogResult = UIManager.ShowMessageDialog("确认注销登录？");
            if (dialogResult == null || !dialogResult.Value) return;
            //修改登录配置
            var cacheData = IsolatedStorageHelper.Get<LocalData>();
            if (cacheData != null)
            {
                cacheData.LoginPwd = null;
                cacheData.AutoLogin = cacheData.RememberPassword = false;
                IsolatedStorageHelper.Save(cacheData);
            }
            UIManager.Restart();
        }

        private bool CanExecuteLogoutCommand()
        {
            return true;
        }

        #endregion

        #region MoreCommand

        private RelayCommand _moreCommand;

        public RelayCommand MoreCommand
        {
            get
            {
                return _moreCommand ?? (new RelayCommand(ExecuteMoreCommand, CanExecuteMoreCommand));

            }
        }

        private void ExecuteMoreCommand()
        {
            UIManager.InvokeCommand(EnumPushCommands.NoticePage);
        }

        private bool CanExecuteMoreCommand()
        {
            return true;
        }

        #endregion

        #region OpenNoticeCommand

        private RelayCommand<NoticeDto> _openNoticeCommand;

        public RelayCommand<NoticeDto> OpenNoticeCommand
        {
            get
            {
                return _openNoticeCommand ?? (new RelayCommand<NoticeDto>(ExecuteOpenNoticeCommand, CanExecuteOpenNoticeCommand));

            }
        }

        private void ExecuteOpenNoticeCommand(NoticeDto model)
        {
            UIManager.InvokeCommand(EnumPushCommands.NoticePage);


        }

        private bool CanExecuteOpenNoticeCommand(NoticeDto model)
        {
            return model != null;
        }
        #endregion

        #region OpenCustomProvideCommand
        private RelayCommand _openCustomProvideCommand;
        public RelayCommand OpenCustomProvideCommand
        {
            get
            {
                return _openCustomProvideCommand ?? (new RelayCommand(ExecuteOpenCustomProvideCommand));
            }
        }

        private void ExecuteOpenCustomProvideCommand()
        {
            UIManager.OpenDefaultBrower("http://kegui.jptonghang.com/dm/Prescribe/guiding.html?Airways=");
        }

        #endregion

        #region OpenAppPageCommand
        private RelayCommand _openAppPageCommand;
        public RelayCommand OpenAppPageCommand
        {
            get
            {
                return _openAppPageCommand ?? (new RelayCommand(ExecuteOpenAppPageCommand));
            }
        }

        private void ExecuteOpenAppPageCommand()
        {
            UIManager.OpenDefaultBrower("http://www.51cbc.cn/down/app_index.html");
        }

        #endregion

        #region UpdateCommand 软件升级
        private RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (new RelayCommand(ExecuteUpdateCommand));
            }
        }

        private void ExecuteUpdateCommand()
        {
            //GetVersion();
            //UIManager.ShowMessage("当前版本号：" + Version);
            //var autoUpdatePath = Path.Combine(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar)), "AutoUpdate/AutoUpdate.exe");
            //var currentAppName = Assembly.GetEntryAssembly().Location.Substring(Assembly.GetEntryAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(".exe", "");
            //var url = ConfigurationManager.AppSettings["UpdateUrl"];
            //if (System.Diagnostics.Process.GetProcessesByName("AutoUpdate").Length != 0) return;
            //var pro = new System.Diagnostics.Process();
            //if (!File.Exists(autoUpdatePath)) return;
            //pro.StartInfo.FileName = autoUpdatePath;
            //pro.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", currentAppName, url);
            //pro.Start();
        }

        #endregion

        #endregion

        #region 私有函数
        /// <summary>
        /// 得到通知信息
        /// </summary>
        private void GetNoticeList()
        {
            Action act = () => CommunicateManager.Invoke<INoticeService>(p =>
            {
                var data = p.FindNoticeList(EnumNoticeType.Roll, "", null, null, 1, 20);
                if (data.TotalCount <= 0) return;
                var list = data.List.Select(m => new LocalNoticeDto { Title = m.Title, ShowTitle = CutString(m.Title, 9) }).ToList();
                NoticeList = list;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(act).ContinueWith(task => CommunicateManager.Invoke<IMyMessageService>(service =>
            {
                TipCount = service.GetUnReadMsgCount();
            }, UIManager.ShowErr));
        }


        private string CutString(string str, int max)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (str.Length > max)
            {
                return str.Substring(0, max) + "...";
            }
            return str;
        }

        #endregion
    }

}
