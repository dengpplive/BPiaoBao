using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/*
* CLR版本: 4.0.30319.34014
* 文件名：CarrayControl
* 命名空间：MvvmLight1.Controls
* 类名：CarrayControl
* 用户名：duanwei
* 创建日期：2014/5/6 15:18:42
* 描述：
*/
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BPiaoBao.Client.UIExt.CommonControl
{
    /// <summary>
    /// PassengerControl.xaml 的交互逻辑
    /// </summary>
    public partial class PassengerControl : UserControl
    {
        private bool isSearchMode = false;//是否是搜索模式
        public PassengerControl()
        {
            InitializeComponent(); 
            InputBindings.Add(new KeyBinding(EnterCommand, new KeyGesture(Key.Enter)));

            Loaded += CitysControl_Loaded;

            if (ViewModelBase.IsInDesignModeStatic)
                IsBusy = true;

            searchDataGrid.PreviewMouseDown += searchDataGrid_PreviewMouseDown;
            searchDataGrid.PreviewKeyDown += searchDataGrid_PreviewKeyDown;
            //NormalDataGrid.PreviewMouseDown += searchDataGrid_PreviewMouseDown;
            //NormalDataGrid.PreviewKeyDown += searchDataGrid_PreviewKeyDown;
            IsVisibleChanged += CarrayControl_IsVisibleChanged;
        }
        void CarrayControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false && isSearchMode)
            {
                SetCurrent(searchDataGrid.SelectedItem as PasserModel);
            }
        }
        void searchDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            SetKeyDown(e);
        }
        public void SetKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                SetCurrent(searchDataGrid.SelectedItem as PasserModel);
            }
            else if (e.Key == Key.Down)
            {
                var temp = searchDataGrid.SelectedIndex + 1;
                if (temp >= searchDataGrid.Items.Count - 1)
                    temp = searchDataGrid.Items.Count - 1;

                searchDataGrid.SelectedIndex = temp;
            }
            else if (e.Key == Key.Up)
            {
                var temp = searchDataGrid.SelectedIndex - 1;
                if (temp < 0)
                    temp = 0;
                searchDataGrid.SelectedIndex = temp;
            }

            if (searchDataGrid.SelectedItem != null)
                searchDataGrid.ScrollIntoView(searchDataGrid.SelectedItem);
        }

        private void SetCurrent(PasserModel passenger)
        {


            CurrentPassenger = passenger;
            searchDataGrid.SelectedValue = CurrentPassenger;

            if (SelectedChanged != null)
                SelectedChanged(this, new EventArgs());
        }

        void searchDataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var temp = e.OriginalSource as FrameworkElement;
            if (temp == null)
                return;

            var tempModel = temp.DataContext as PasserModel;
            SetCurrent(tempModel);
            e.Handled = true;
        }

        void CitysControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (CarrayData != null || IsBusy)
            //    return;

            //IsBusy = true;

            //var loadAction = new Action(() =>
            //{
            //    var tempData = new PassengerData();
            //    tempData.PassengerList = Agent.GetCarryInfos();

            //    Dispatcher.Invoke(new Action(() =>
            //    {
            //        CarrayData = tempData;
            //    }));
            //});

            //AsynHelper.Invoke(loadAction, (ex) =>
            //{
                
            //}).ContinueWith((task) =>
            //{
            //    InvokeSetBusy(false);
            //});
        }

        private void InvokeSetBusy(bool busy)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                IsBusy = busy;
            }));
        }

        #region 公开属性

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> dependency property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsBusy" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return (bool)GetValue(IsBusyProperty);
            }
            set
            {
                SetValue(IsBusyProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsBusy" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            IsBusyPropertyName,
            typeof(bool),
            typeof(PassengerControl),
            new PropertyMetadata(false));

        #endregion

        #region PassengerData

        /// <summary>
        /// The <see cref="PassengerData" /> dependency property's name.
        /// </summary>
        public const string PassengerDataPropertyName = "PassengerData";

        /// <summary>
        /// Gets or sets the value of the <see cref="PassengerData" />
        /// property. This is a dependency property.
        /// </summary>
        public PassengerData PassengerData
        {
            get
            {
                return (PassengerData)GetValue(PassengerDataProperty);
            }
            set
            {
                SetValue(PassengerDataProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PassengerDataProperty" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PassengerDataProperty = DependencyProperty.Register(
            PassengerDataPropertyName,
            typeof(PassengerData),
            typeof(PassengerControl),
            new PropertyMetadata(null));

        #endregion

        #region CurrentPassenger

        /// <summary>
        /// The <see cref="CurrentPassenger" /> dependency property's name.
        /// </summary>
        public const string CurrentPassengerPropertyName = "CurrentPassenger";

        /// <summary>
        /// Gets or sets the value of the <see cref="CurrentPassenger" />
        /// property. This is a dependency property.
        /// </summary>
        public PasserModel CurrentPassenger
        {
            get
            {
                return (PasserModel)GetValue(CurrentPassengerProperty);
            }
            set
            {
                SetValue(CurrentPassengerProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CurrentPassengerProperty" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPassengerProperty = DependencyProperty.Register(
            CurrentPassengerPropertyName,
            typeof(PasserModel),
            typeof(PassengerControl),
            new PropertyMetadata(null));

        #endregion

        #endregion


        #region 事件

        public event EventHandler SelectedChanged; 

        #endregion


        #region 搜索 
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="txt">The text.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal void Search(string txt)
        {
            if (String.IsNullOrEmpty(txt))
            {
                VisualStateManager.GoToElementState(this.Content as FrameworkElement, "ChooseState", true);
                isSearchMode = false;
                SetCurrent(null);
                return;
            }
            else
            {
                VisualStateManager.GoToElementState(this.Content as FrameworkElement, "SearchState", true);
                isSearchMode = true;
            }

            IsBusy = true;
            Action action = new Action(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (PassengerData == null)
                    {
                        PassengerData=new PassengerData();
                    }
                    PassengerData.SearchResult = Agent.SearchPassenger(txt);
                   // searchDataGrid.SelectedIndex = 0;
                   // searchDataGrid.Focus();
                }));
            });

            AsynHelper.Invoke(action, (ex) =>
            {
               
            }).ContinueWith((task) =>
            {
                InvokeSetBusy(false);
            });
        }
        #endregion

        #region 命令

        #region SelectPassengerCommand

        private RelayCommand<PasserModel> selectPassengerCommand;

        /// <summary>
        /// 选择航空公司命令.
        /// </summary>
        public RelayCommand<PasserModel> SelectPassengerCommand
        {
            get
            {
                return selectPassengerCommand ?? (selectPassengerCommand = new RelayCommand<PasserModel>(ExecuteSelectPassengerCommand, CanExecuteSelectPassengerCommand));
            }
        }

        private void ExecuteSelectPassengerCommand(PasserModel passer)
        {
            CurrentPassenger = passer;
            if (SelectedChanged != null)
                SelectedChanged(this, new EventArgs());
        }

        private bool CanExecuteSelectPassengerCommand(PasserModel passer)
        {
            return passer != null;
        }

        #endregion

        #region EnterCommand

        private RelayCommand enterCommand;

        /// <summary>
        /// Gets the EnterCommand.
        /// </summary>
        public RelayCommand EnterCommand
        {
            get
            {
                return enterCommand ?? (enterCommand = new RelayCommand(ExecuteEnterCommand, CanExecuteEnterCommand));
            }
        }

        private void ExecuteEnterCommand()
        {
            CurrentPassenger = searchDataGrid.SelectedItem as PasserModel;
        }

        private bool CanExecuteEnterCommand()
        {
            return true;
        }

        #endregion

        #endregion


    }

    /// <summary>
    /// 数据，新写这个类，主要是方便blend模拟数据
    /// </summary>
    public class PassengerData : ObservableObject
    {
        #region PassengerList

        /// <summary>
        /// The <see cref="PassengerList" /> property's name.
        /// </summary>
        public const string PassengerListPropertyName = "PassengerList";

        private List<PasserModel> _passengerList = new List<PasserModel>();

        /// <summary>
        /// 列表
        /// </summary>
        public List<PasserModel> PassengerList
        {
            get { return _passengerList; }

            set
            {
                if (_passengerList == value) return;

                RaisePropertyChanging(PassengerListPropertyName);
                _passengerList = value;
                RaisePropertyChanged(PassengerListPropertyName);
            }
        }

        #endregion

        #region SearchResult

        /// <summary>
        /// The <see cref="SearchResult" /> property's name.
        /// </summary>
        public const string SearchResultPropertyName = "SearchResult";

        private List<PasserModel> _searchResult = new List<PasserModel>();

        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<PasserModel> SearchResult
        {
            get { return _searchResult; }

            set
            {
                if (_searchResult == value) return;

                RaisePropertyChanging(SearchResultPropertyName);
                _searchResult = value;
                RaisePropertyChanged(SearchResultPropertyName);
            }
        }

        #endregion
    }
}
