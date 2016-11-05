using System.Timers;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UserControl;
using BPiaoBao.Client.View;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BPiaoBao.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 左边菜单是否展开
        /// </summary>
        private bool isLeftExpend = true;
        private string CurrentModuleCode
        {
            get
            {
                return (string)MenuTitle.Tag;
            }
        }
        //private Point mousePoint = new Point();
        //private readonly int cornerWidth = 1;
        //private readonly int customBorderThickness = 1;
        public MainWindow()
        {
            InitializeComponent();
            //SourceInitialized += MainWindow_SourceInitialized;
            //StateChanged += MainWindow_StateChanged;
            //titleGrid.MouseDown += titleGrid_MouseDown;
            //TV.SelectedItemChanged += TV_SelectedItemChanged;
            //TV.MouseUp += TV_Selected_1;
            var binding = new CommandBinding(NavigationCommands.Refresh, ExecutedRefresh);
            CommandBindings.Add(binding);
            var binding1 = new CommandBinding(NavigationCommands.Zoom, ExecuteFullScreenCommand);
            CommandBindings.Add(binding1); 
            MessageObserver.CreateInstance.DoAction += CreateInstance_DoAction;
            Loaded += MainWindow_Loaded;
            //默认启动项
            //PluginService.Run("1", "10011");
            this.KeyDown += MainWindow_KeyDown;
            this.RightImg.MouseEnter += Overlay_MouseEnter;
            this.RightImg.MouseLeave += Overlay_MouseLeave;
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F11:
                    var currentControl = TabContent.SelectedContent as Control;
                    if (currentControl == null)
                        return;
                    var item = TabContent.SelectedValue as ExtTabItem;
                    RemoveParent(currentControl);
                    var fullScreen = new FullScreenWindow(currentControl, item) { Owner = this };
                    fullScreen.ShowDialog();
                    break;
                case Key.F9:
                    this.BtnLeft_OnClick(this,null);
                    break;
            }
        }

        private void ExecuteFullScreenCommand(object sender, RoutedEventArgs e)
        {
            var currentControl = TabContent.SelectedContent as Control;
            if (currentControl == null)
                return;
            var item = TabContent.SelectedValue as ExtTabItem;
            RemoveParent(currentControl);
            var fullScreen = new FullScreenWindow(currentControl, item) { Owner = this };
            fullScreen.ShowDialog();
        }



        //void titleGrid_MouseDown(MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount == 2)
        //    {
        //        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        //    }
        //}

        void ItemMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mainMenuItem = ((TreeViewItem)sender).DataContext as MainMenuItem;
            if (mainMenuItem == null)
                return;

            if (mainMenuItem.Children != null && mainMenuItem.Children.Any())//包含子节点
                return;
            PluginService.Run(CurrentModuleCode, mainMenuItem.MenuCode);
            e.Handled = true;
        }

        private void ExecutedRefresh(object sender, ExecutedRoutedEventArgs e)
        {
            var currentControl = TabContent.SelectedContent as Control;
            if (currentControl == null)
                return;

            var vm = currentControl.DataContext as BaseVM;
            if (vm != null)
                vm.Initialize();
        }
        void CreateInstance_DoAction(object sender, PluginEventArgs args)
        {
            var tabItem = TabContent.Items.OfType<UserControl.ExtTabItem>().SingleOrDefault(x => (string)x.Tag == args.PluginCode);
            if (tabItem == null)
            {
                tabItem = new UserControl.ExtTabItem();
                TabContent.Items.Add(tabItem);
                tabItem.Tag = args.PluginCode;
                tabItem.Header = args.Header;
                RemoveParent(args.UserControl as Control);
                tabItem.Content = args.UserControl;
            }
            if (!args.IsMain)
            {
                tabItem.Header = args.Header;
                RemoveParent(args.UserControl as Control);

                tabItem.Content = args.UserControl;
            }
            tabItem.IsSelected = true;
        }

        private void RemoveParent(Control control)
        {
            if (control == null) return;

            if (control.Parent != null)
            {
                control.Parent.SetValue(ContentPresenter.ContentProperty, null);
                //Logger.WriteLog(LogType.INFO, "父容器不为空", null);
            }
        }

        //private void MainWindow_StateChanged(object sender, EventArgs e)
        //{
        //    if (WindowState == WindowState.Maximized)
        //    {
        //        // BorderThickness = new System.Windows.Thickness(0);
        //        WindowContent.BorderThickness = new System.Windows.Thickness(0);
        //    }
        //    else
        //    {
        //        // BorderThickness = new System.Windows.Thickness(customBorderThickness);
        //        WindowContent.BorderThickness = new System.Windows.Thickness(customBorderThickness);
        //    }
        //}

        //最大化遮挡任务栏问题
        //private void MainWindow_SourceInitialized(object sender, EventArgs e)
        //{
        //    HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
        //    if (source == null)
        //        // Should never be null
        //        throw new Exception("Cannot get HwndSource instance.");

        //    source.AddHook(new HwndSourceHook(WndProc));
        //}
        //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{

        //    switch (msg)
        //    {
        //        case Win32.WM_GETMINMAXINFO: // WM_GETMINMAXINFO message
        //            WmGetMinMaxInfo(hwnd, lParam);
        //            handled = true;
        //            break;
        //        case Win32.WM_NCHITTEST: // WM_NCHITTEST message
        //            return WmNCHitTest(lParam, ref handled);
        //    }

        //    return IntPtr.Zero;
        //}
        //private IntPtr WmNCHitTest(IntPtr lParam, ref bool handled)
        //{
        //    // Update cursor point
        //    // The low-order word specifies the x-coordinate of the cursor.
        //    // #define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))
        //    mousePoint.X = (int)(short)(lParam.ToInt32() & 0xFFFF);
        //    // The high-order word specifies the y-coordinate of the cursor.
        //    // #define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))
        //    mousePoint.Y = (int)(short)(lParam.ToInt32() >> 16);

        //    // Do hit test
        //    handled = true;
        //    if (Math.Abs(mousePoint.Y - Top) <= cornerWidth
        //        && Math.Abs(mousePoint.X - Left) <= cornerWidth)
        //    { // Top-Left
        //        return new IntPtr((int)Win32.HitTest.HTTOPLEFT);
        //    }
        //    else if (Math.Abs(ActualHeight + Top - mousePoint.Y) <= cornerWidth
        //        && Math.Abs(mousePoint.X - Left) <= cornerWidth)
        //    { // Bottom-Left
        //        return new IntPtr((int)Win32.HitTest.HTBOTTOMLEFT);
        //    }
        //    else if (Math.Abs(mousePoint.Y - Top) <= cornerWidth
        //        && Math.Abs(ActualWidth + Left - mousePoint.X) <= cornerWidth)
        //    { // Top-Right
        //        return new IntPtr((int)Win32.HitTest.HTTOPRIGHT);
        //    }
        //    else if (Math.Abs(ActualWidth + Left - mousePoint.X) <= cornerWidth
        //        && Math.Abs(ActualHeight + Top - mousePoint.Y) <= cornerWidth)
        //    { // Bottom-Right
        //        return new IntPtr((int)Win32.HitTest.HTBOTTOMRIGHT);
        //    }
        //    else if (Math.Abs(mousePoint.X - Left) <= customBorderThickness)
        //    { // Left
        //        return new IntPtr((int)Win32.HitTest.HTLEFT);
        //    }
        //    else if (Math.Abs(ActualWidth + Left - mousePoint.X) <= customBorderThickness)
        //    { // Right
        //        return new IntPtr((int)Win32.HitTest.HTRIGHT);
        //    }
        //    else if (Math.Abs(mousePoint.Y - Top) <= customBorderThickness)
        //    { // Top
        //        return new IntPtr((int)Win32.HitTest.HTTOP);
        //    }
        //    else if (Math.Abs(ActualHeight + Top - mousePoint.Y) <= customBorderThickness)
        //    { // Bottom
        //        return new IntPtr((int)Win32.HitTest.HTBOTTOM);
        //    }
        //    else
        //    {
        //        handled = false;
        //        return IntPtr.Zero;
        //    }
        //}
        //private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        //{
        //    // MINMAXINFO structure
        //    Win32.MINMAXINFO minMaxInfo = (Win32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Win32.MINMAXINFO));

        //    // Get the handle of the nearest monitor to the window
        //    WindowInteropHelper interopHelper = new WindowInteropHelper(this);
        //    IntPtr hMonitor = Win32.MonitorFromWindow(interopHelper.Handle, Win32.MONITOR_DEFAULTTONEAREST);

        //    // Get monitor information
        //    Win32.MONITORINFOEX monitorInfo = new Win32.MONITORINFOEX();
        //    monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
        //    Win32.GetMonitorInfo(new HandleRef(this, hMonitor), monitorInfo);

        //    // Get HwndSource
        //    HwndSource source = HwndSource.FromHwnd(interopHelper.Handle);
        //    if (source == null)
        //        // Should never be null
        //        throw new Exception("Cannot get HwndSource instance.");
        //    if (source.CompositionTarget == null)
        //        // Should never be null
        //        throw new Exception("Cannot get HwndTarget instance.");

        //    // Get working area rectangle
        //    Win32.RECT workingArea = monitorInfo.rcWork;

        //    // Get transformation matrix
        //    Matrix matrix = source.CompositionTarget.TransformFromDevice;

        //    // Convert working area rectangle to DPI-independent values
        //    Point convertedSize =
        //        matrix.Transform(new Point(
        //                workingArea.Right - workingArea.Left,
        //                workingArea.Bottom - workingArea.Top
        //                ));

        //    // Set the maximized size of the window
        //    minMaxInfo.ptMaxSize.x = (int)convertedSize.X;
        //    minMaxInfo.ptMaxSize.y = (int)convertedSize.Y;

        //    // Set the position of the maximized window
        //    minMaxInfo.ptMaxPosition.x = 0;
        //    minMaxInfo.ptMaxPosition.y = 0;

        //    Marshal.StructureToPtr(minMaxInfo, lParam, true);
        //}

        private void MinWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void MaxWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Service_Click(object sender, RoutedEventArgs e)
        {
            var window = new ServiceCenterWindow { Owner = Application.Current.MainWindow };
            window.ShowDialog();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //private void MouseDragMove(object sender, MouseButtonEventArgs e)
        //{
        //    DragMove();
        //}

        //private void TV_Selected_1(object sender, MouseButtonEventArgs e)
        //{
        //    // var m = e.Source;
        //    MainMenuItem mainMenuItem = ((TreeView)sender).SelectedItem as MainMenuItem;
        //    if (mainMenuItem == null)
        //        return;
        //    PluginService.Run(currentModuleCode, mainMenuItem.MenuCode);
        //    e.Handled = true;
        //}


        //void TV_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    // var m = e.Source;
        //    var mainMenuItem = ((TreeView)sender).SelectedItem as MainMenuItem;
        //    if (mainMenuItem == null)
        //        return;
        //    PluginService.Run(CurrentModuleCode, mainMenuItem.MenuCode);
        //    e.Handled = true;
        //}

        private Timer _timer;
        int _listViewIndex;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _timer = new Timer { Interval = 2000 };
            _timer.Elapsed += (s, ee) => Dispatcher.Invoke(new Action(() =>
            {
                if (listView.Items.Count < 1)
                {
                    return;
                }
                if (_listViewIndex >= listView.Items.Count - 1)
                {
                    _listViewIndex = 0;
                }
                var item = listView.Items[_listViewIndex];
                //var da = new DoubleAnimation(1.0d, new Duration(TimeSpan.FromMilliseconds(2000)));
                //BeginAnimation(OpacityProperty, da);
                listView.ScrollIntoView(item);

                _listViewIndex++;
            }));

            _timer.Start();
        }
        private void listView_MouseEnter(object sender, MouseEventArgs e)
        {
            _timer.Stop();
        }

        private void listView_MouseLeave(object sender, MouseEventArgs e)
        {
            _timer.Start();
        }

        private void Animation(double to, double duration)
        { 
            var doubleAnimation = new DoubleAnimation(to, new Duration(TimeSpan.FromMilliseconds(duration)));
            var transform = new TranslateTransform(Overlay.RenderTransform.Value.OffsetX, Overlay.RenderTransform.Value.OffsetY);
            Overlay.RenderTransform = transform;
             ((TranslateTransform)Overlay.RenderTransform).BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
        }


        void Overlay_MouseLeave(object sender, MouseEventArgs e)
        {
            Animation(35, 200);
        }

        void Overlay_MouseEnter(object sender, MouseEventArgs e)
        {
            Animation(-6, 200);
        }

        private void BtnLeft_OnClick(object sender, RoutedEventArgs e)
        {
            if (isLeftExpend)
            {
                this.BorderLeft.Width = 25;
                isLeftExpend = false;
                var img=new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/BPiaoBao.Client;component/Images/layout_arrows2.png"));
                btnLeft.Content = img;
                btnLeft.ToolTip = "展开菜单";
                LeftGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.BorderLeft.Width = 183;
                isLeftExpend = true;
                var img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/BPiaoBao.Client;component/Images/layout_arrows1.png"));
                btnLeft.Content = img;
                btnLeft.ToolTip = "隐藏菜单";
                LeftGrid.Visibility = Visibility.Visible;
            }
        }
    }

}
