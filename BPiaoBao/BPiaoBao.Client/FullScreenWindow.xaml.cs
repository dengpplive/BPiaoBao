using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UserControl;

namespace BPiaoBao.Client
{
    /// <summary>
    /// FullScreenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenWindow : FullWidowExt
    {
      
        public ExtTabItem _extTabItem = null;
        public FullScreenWindow(Control control, ExtTabItem item)
        {
            InitializeComponent();  
            this.Title = item.Header.ToString();
            this._extTabItem = item;
            var vm = control.DataContext as BaseVM;
            if (vm != null)
            {
                vm.FullWidowExt=new FullWidowExt();
                vm.FullWidowExt.IsFullScreen = true;
                vm.FullWidowExt.InitFullWindow(this,control);
            } 
            this.KeyDown += FullScreenWindow_KeyDown;
            this.Closed += FullScreenWindow_Closed;
        }



        void FullScreenWindow_Closed(object sender, EventArgs e)
        {
            var control = this.Content as Control;
            if (control == null || _extTabItem == null) return;
            if (control.Parent != null)
            {
                this.Content = null;
            }
            _extTabItem.Content = control;
            foreach (var ctrl in CacheControlList)
            {
                var vm = ctrl.DataContext as BaseVM;
                if (vm != null)
                {
                   vm.FullWidowExt.SetNormalWindow();
                }
            }
            
            

        }

        private void FullScreenWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                case Key.F11:
                    Close();
                    break;
                case Key.F5:
                    var control = this.Content as Control;
                    if (control != null)
                    {
                        var vm = control.DataContext as BaseVM;
                        if (vm != null)
                        {
                            vm.Initialize();
                        }
                    }

                    break; ;
            }

        }
    }
}
