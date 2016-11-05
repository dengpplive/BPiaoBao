using System.Collections.Specialized;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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

namespace BPiaoBao.Client.UIExt.CommonControl
{
    /// <summary>
    /// CitysControl.xaml 的交互逻辑
    /// </summary>
    public partial class CitysControl : UserControl
    {
        private bool isSearchMode = false;//是否是搜索模式
        public CitysControl()
        {

            InitializeComponent();
            InputBindings.Add(new KeyBinding(EnterCommand, new KeyGesture(Key.Enter)));

            Loaded += CitysControl_Loaded;

            if (ViewModelBase.IsInDesignModeStatic)
                IsBusy = true;

            searchDataGrid.PreviewMouseDown += searchDataGrid_PreviewMouseDown;
            searchDataGrid.PreviewKeyDown += searchDataGrid_PreviewKeyDown;
            tabControl.PreviewMouseDown += tabControl_PreviewMouseDown;
            IsVisibleChanged += CitysControl_IsVisibleChanged;
        }

        void CitysControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false && isSearchMode)
            {
                SetCurrent(searchDataGrid.SelectedItem as CityNewModel);
            }
        }

        void tabControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var temp = e.OriginalSource as FrameworkElement;
            if (temp == null)
                return;

            tabControl.SelectedItem = temp.DataContext;
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
                SetCurrent(searchDataGrid.SelectedItem as CityNewModel);
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

        private void SetCurrent(CityNewModel cityModel)
        {


            CurrentCity = cityModel;
            searchDataGrid.SelectedValue = CurrentCity;

            if (SelectedChanged != null)
                SelectedChanged(this, new EventArgs());
        }

        void searchDataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var temp = e.OriginalSource as FrameworkElement;
            if (temp == null)
                return;

            var tempModel = temp.DataContext as CityNewModel;
            SetCurrent(tempModel);
            e.Handled = true;
        }

        void CitysControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CityData != null || IsBusy)
                return;

            IsBusy = true;

            var loadAction = new Action(() =>
            {
                var tempData = new CityData();
                List<CityCategory> tempCategory = new List<CityCategory>();
                tempCategory.Add(GetCategoryHotCity("热门"));
                tempCategory.Add(GetCategoryEx("ABC"));
                tempCategory.Add(GetCategoryEx("DEFG"));
                tempCategory.Add(GetCategoryEx("HI"));
                tempCategory.Add(GetCategoryEx("JK"));
                tempCategory.Add(GetCategoryEx("LM"));
                tempCategory.Add(GetCategoryEx("NOPQR"));
                tempCategory.Add(GetCategoryEx("ST"));
                tempCategory.Add(GetCategoryEx("UVWX"));
                tempCategory.Add(GetCategoryEx("YZ"));
                tempData.CityCategory = tempCategory;

                Dispatcher.Invoke(new Action(() =>
                {
                    CityData = tempData;
                }));
            });

            AsynHelper.Invoke(loadAction, (ex) =>
            {
                Logger.WriteLog(LogType.ERROR, ex.Message, ex);
            }).ContinueWith((task) =>
            {
                InvokeSetBusy(false);
            });
        }

        private void InvokeSetBusy(bool busy)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                IsBusy = busy;
            }));
        }

        static CitysControl()
        {
            s_ListHotCity = new StringCollection()
            {
                "PEK",
                "NAY",
                "PVG",
                "SHA",
                "CAN",
                "SZX",
                "CTU",
                "HGH",
                "WUH",
                "XIY",
                "CKG",
                "TAO",
                "CSX",
                "NKG",
                "XMN",
                "KMG",
                "DLC",
                "TSN",
                "CGO",
                "SYX",
                "TNA",
                "FOC",
            };

        }

        private CityCategory GetCategoryHotCity(string indexs)
        {
            CityCategory result = new CityCategory();
            result.Indexs = indexs;

            var dicAllCitys = Agent.GetCitys().ToLookup(p => p.Key, p => p);

            foreach (var item in s_ListHotCity)
            {
                if (dicAllCitys.Contains(item))
                {
                    result.SubItems.AddRange(dicAllCitys[item]);
                }
            }

            return result;
        }

        private CityCategory GetCategoryEx(string indexs)
        {
            char[] arr = indexs.ToArray();

            CityCategory result = new CityCategory();
            result.Indexs = indexs;
            foreach (char item in arr)
            {
                result.SubItems.AddRange(GetGroup(item));
            }
            return result;
        }
        private List<CityNewModel> GetGroup(char key)
        {
            var cities = new List<CityNewModel>();

            var allCitys = Agent.GetCitys();
            var temp = allCitys.Where(m => m.Info.PinYin.ToLower().StartsWith(key.ToString().ToLower()));
            if (temp != null)
                foreach (var item in temp)
                {
                    cities.Add(new CityNewModel()
                    {
                        Key = item.Key,
                        Info = new CityInfoModel()
                        {
                            AirPortName = item.Info.AirPortName,
                            Code = item.Info.Code,
                            JPPinyin = item.Info.JPPinyin,
                            Name = item.Info.Name,
                            PinYin = item.Info.PinYin
                        }
                    });
                }

            return cities;
        }

        //private CityCategory GetCategory(string indexs)
        //{
        //    char[] arr = indexs.ToArray();

        //    CityCategory result = new CityCategory();
        //    result.Groups = new List<CityGroup>();
        //    result.Indexs = indexs;
        //    foreach (char item in arr)
        //    {
        //        result.Groups.Add(GetGroup(item));
        //    }
        //    return result;
        //}

        //private CityGroup GetGroup(char key)
        //{

        //    var result = new CityGroup();
        //    result.Index = key.ToString();
        //    result.Cities = new List<CityNewModel>();

        //    var allCitys = Agent.GetCitys();
        //    var temp = allCitys.Where(m => m.Info.PinYin.ToLower().StartsWith(key.ToString().ToLower()));
        //    if (temp != null)
        //        foreach (var item in temp)
        //        {
        //            result.Cities.Add(new CityNewModel()
        //            {
        //                Key = item.Key,
        //                Info = new CityInfoModel()
        //                {
        //                    AirPortName = item.Info.AirPortName,
        //                    Code = item.Info.Code,
        //                    JPPinyin = item.Info.JPPinyin,
        //                    Name = item.Info.Name,
        //                    PinYin = item.Info.PinYin
        //                }
        //            });
        //        }

        //    return result;
        //}

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
            typeof(CitysControl),
            new PropertyMetadata(false));

        #endregion

        #region CityData

        /// <summary>
        /// The <see cref="CityData" /> dependency property's name.
        /// </summary>
        public const string CityDataPropertyName = "CityData";

        /// <summary>
        /// Gets or sets the value of the <see cref="CityData" />
        /// property. This is a dependency property.
        /// </summary>
        public CityData CityData
        {
            get
            {
                return (CityData)GetValue(CityDataProperty);
            }
            set
            {
                SetValue(CityDataProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CityData" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CityDataProperty = DependencyProperty.Register(
            CityDataPropertyName,
            typeof(CityData),
            typeof(CitysControl),
            new PropertyMetadata(null));

        #endregion

        #region CurrentCity

        /// <summary>
        /// The <see cref="CurrentCity" /> dependency property's name.
        /// </summary>
        public const string CurrentCityPropertyName = "CurrentCity";

        /// <summary>
        /// Gets or sets the value of the <see cref="CurrentCity" />
        /// property. This is a dependency property.
        /// </summary>
        public CityNewModel CurrentCity
        {
            get
            {
                return (CityNewModel)GetValue(CurrentCityProperty);
            }
            set
            {
                SetValue(CurrentCityProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CurrentCity" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentCityProperty = DependencyProperty.Register(
            CurrentCityPropertyName,
            typeof(CityNewModel),
            typeof(CitysControl),
            new PropertyMetadata(null));

        #endregion

        #endregion

        #region 命令

        #region SelectCityCommand

        private RelayCommand<CityNewModel> selectCityCommand;

        /// <summary>
        /// 选择城市命令.
        /// </summary>
        public RelayCommand<CityNewModel> SelectCityCommand
        {
            get
            {
                return selectCityCommand ?? (selectCityCommand = new RelayCommand<CityNewModel>(ExecuteSelectCity, CanExecuteSelectCity));
            }
        }

        private void ExecuteSelectCity(CityNewModel city)
        {
            CurrentCity = city;
            if (SelectedChanged != null)
                SelectedChanged(this, new EventArgs());
        }

        private bool CanExecuteSelectCity(CityNewModel city)
        {
            return city != null;
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
            CurrentCity = searchDataGrid.SelectedItem as CityNewModel;
        }

        private bool CanExecuteEnterCommand()
        {
            return true;
        }

        #endregion

        #endregion

        #region 事件

        public event EventHandler SelectedChanged;
        private static StringCollection s_ListHotCity;

        #endregion



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
                    CityData.SearchResult = Agent.SearchCity(txt);
                    searchDataGrid.SelectedIndex = 0;
                    searchDataGrid.Focus();
                }));
            });

            AsynHelper.Invoke(action, (ex) =>
            {
                Logger.WriteLog(LogType.ERROR, ex.Message, ex);
            }).ContinueWith((task) =>
            {
                InvokeSetBusy(false);
            });
        }
    }

    /// <summary>
    /// 城市数据，新写这个类，主要是方便blend模拟数据
    /// </summary>
    public class CityData : ObservableObject
    {
        #region CityCategory

        /// <summary>
        /// The <see cref="CityCategory" /> property's name.
        /// </summary>
        public const string CityCategoryPropertyName = "CityCategory";

        private List<CityCategory> cityCategory = new List<CityCategory>();

        /// <summary>
        /// 城市列表
        /// </summary>
        public List<CityCategory> CityCategory
        {
            get { return cityCategory; }

            set
            {
                if (cityCategory == value) return;

                RaisePropertyChanging(CityCategoryPropertyName);
                cityCategory = value;
                RaisePropertyChanged(CityCategoryPropertyName);
            }
        }

        #endregion

        #region SearchResult

        /// <summary>
        /// The <see cref="SearchResult" /> property's name.
        /// </summary>
        public const string SearchResultPropertyName = "SearchResult";

        private List<CityNewModel> searchResult = new List<CityNewModel>();

        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<CityNewModel> SearchResult
        {
            get { return searchResult; }

            set
            {
                if (searchResult == value) return;

                RaisePropertyChanging(SearchResultPropertyName);
                searchResult = value;
                RaisePropertyChanged(SearchResultPropertyName);
            }
        }

        #endregion
    }

    /// <summary>
    /// 城市分类，包括一组城市列表
    /// </summary>
    public class CityCategory
    {
        //public List<CityGroup> Groups { get; set; }

        private List<CityNewModel> _SubItems = new List<CityNewModel>();

        public List<CityNewModel> SubItems
        {
            get { return _SubItems; }
            set { _SubItems = value; }
        }

        public string Indexs { get; set; }
    }

    /// <summary>
    /// 城市分组，包括一个字母的城市
    /// </summary>
    public class CityGroup
    {
        /// <summary>
        /// 字母索引
        /// </summary>      
        public string Index { get; set; }

        /// <summary>
        /// 城市列表
        /// </summary>    
        public List<CityNewModel> Cities { get; set; }
    }
}
