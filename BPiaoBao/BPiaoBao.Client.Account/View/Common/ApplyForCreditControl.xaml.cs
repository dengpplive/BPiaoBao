using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BPiaoBao.Client.Account.View
{
    /// <summary>
    /// ApplyForCreditControl.xaml 的交互逻辑
    /// </summary>
    public partial class ApplyForCreditControl : Xceed.Wpf.Toolkit.BusyIndicator
    {
        public ApplyForCreditControl()
        {
            InitializeComponent();
            Loaded += ApplyForCreditControl_Loaded;

            Check();
        }

        public void Check()
        {
            IsBusy = true;

            //刷新账户信息
            Action action = () =>
            {
                bool isOpen = false;
                try
                {
                    isOpen = CommunicationProxy.GetCreditOpenStatus(true);
                }
                catch (Exception ex)
                {
                    UIManager.ShowErr(ex);
                }
                if (isOpen)
                {
                    var setVisible = new Action(() => { Visibility = System.Windows.Visibility.Collapsed; });
                    DispatcherHelper.UIDispatcher.BeginInvoke(setVisible);
                }
                else
                {
                    var setVisible = new Action(() => { Visibility = System.Windows.Visibility.Visible; });
                    DispatcherHelper.UIDispatcher.BeginInvoke(setVisible);
                }
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                var setBusy = new Action(() => { IsBusy = false; });
                DispatcherHelper.UIDispatcher.Invoke(setBusy);
            });
        }

        public void Switch()
        {
            var vm = this.DataContext as BaseVM;
            if (vm != null && vm.FullWidowExt != null && vm.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.ApplyingForCreditCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    vm.FullWidowExt.SetContent(control);
                }
            }
            else
            {
                //切换到信用申请
                BPiaoBao.Client.Module.PluginService.Run(Main.ProjectCode, Main.ApplyingForCreditCode);
            }
         
        }

        void ApplyForCreditControl_Loaded(object sender, RoutedEventArgs e)
        {
            Check();
        }
    }
}
