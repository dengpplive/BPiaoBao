using System.Windows.Controls;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 理财产品
    /// </summary>
    public class FinanceViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// 
        /// </summary>
        public FinanceViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
        }

        #endregion

        #region HomeVM

        /// <summary>
        /// The <see cref="HomeVm" /> property's name.
        /// </summary>
        private const string HomeVmPropertyName = "HomeVM";

        private HomeViewModel _homeVm;

        /// <summary>
        /// 主页视图模型
        /// </summary>
        public HomeViewModel HomeVm
        {
            get { return _homeVm; }

            set
            {
                if (_homeVm == value) return;

                RaisePropertyChanging(HomeVmPropertyName);
                _homeVm = value;
                RaisePropertyChanged(HomeVmPropertyName);
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;
            Action action = () =>
            {
                ViewModelLocator.Home.RefreshAccountInfo();
                HomeVm = ViewModelLocator.Home;
            };
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setIsWaitAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setIsWaitAction);
            });
        }

        #endregion

        #region 命令

        #region SwitchToAllFinanceCommand

        private RelayCommand _switchToAllFinanceCommand;

        /// <summary>
        /// 切换到所有理财产品
        /// </summary>
        public RelayCommand SwitchToAllFinanceCommand
        {
            get
            {
                return _switchToAllFinanceCommand ?? (_switchToAllFinanceCommand = new RelayCommand(ExecuteSwitchToAllFinanceCommand, CanExecuteSwitchToAllFinanceCommand));
            }
        }

        private void ExecuteSwitchToAllFinanceCommand()
        {
          if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.AllFinanceCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.AllFinanceCode);
            }  
        }

        private bool CanExecuteSwitchToAllFinanceCommand()
        {
            return true;
        }

        #endregion

        #region SwitchToFinanceLogViewCommand

        private RelayCommand _switchToFinanceLogViewCommand;

        /// <summary>
        /// 切换到理财日志命令
        /// </summary>
        public RelayCommand SwitchToFinanceLogViewCommand
        {
            get
            {
                return _switchToFinanceLogViewCommand ?? (_switchToFinanceLogViewCommand = new RelayCommand(ExecuteSwitchToFinanceLogViewCommand, CanExecuteSwitchToFinanceLogViewCommand));
            }
        }

        private void ExecuteSwitchToFinanceLogViewCommand()
        {
          if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.FinanceLogCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.FinanceLogCode);
            }  
            
        }

        private bool CanExecuteSwitchToFinanceLogViewCommand()
        {
            return true;
        }

        #endregion

        #region OpenInfoCommand

        private RelayCommand _openInfoCommand;

        /// <summary>
        /// Gets the OpenInfoCommand.
        /// </summary>
        public RelayCommand OpenInfoCommand
        {
            get
            {
                return _openInfoCommand ?? (_openInfoCommand = new RelayCommand(ExecuteOpenInfoCommand, CanExecuteOpenInfoCommand));
            }
        }

        private void ExecuteOpenInfoCommand()
        {
            UIManager.ShowWeb("如何玩转理财", "http://www.51cbc.cn/playlicai.html");
        }

        private bool CanExecuteOpenInfoCommand()
        {
            return true;
        }

        #endregion

        #endregion

        #region 方法


        #endregion

    }
}
