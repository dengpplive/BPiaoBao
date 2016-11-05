using BPiaoBao.Client.UIExt.CommonControl;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_DisabledVisual", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    public class CityPicker : Control
    {
        private const string c_Empty = "中文/英文";
        Popup popup;
        Button btn;
        TextBox textBox;
        Grid root;
        Grid disabledVisual;
        CitysControl citysControl;

        /// <summary>
        /// 临时变量
        /// </summary>
        private string cacheText = string.Empty;
        static CityPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CityPicker), new FrameworkPropertyMetadata(typeof(CityPicker)));
        }

        public CityPicker()
        {
            MouseLeave += CityPicker_MouseLeave;

            KeyboardNavigation.SetIsTabStop(this, false);
        }

        void CityPicker_MouseLeave(object sender, MouseEventArgs e)
        {
            popup.StaysOpen = false;
        }

        #region 私有方法

        public override void OnApplyTemplate()
        {
            popup = GetTemplateChild("PART_Popup") as Popup;
            btn = GetTemplateChild("PART_Button") as Button;

            KeyboardNavigation.SetIsTabStop(btn, false);

            textBox = GetTemplateChild("PART_TextBox") as TextBox;
            textBox.Text = cacheText;
            SetEmpty();
            root = GetTemplateChild("PART_Root") as Grid;
            disabledVisual = GetTemplateChild("PART_DisabledVisual") as Grid;

            if (ViewModelBase.IsInDesignModeStatic)
                return;

            citysControl = new CitysControl();
            popup.Child = citysControl;
            popup.StaysOpen = false;

            textBox.PreviewMouseLeftButtonDown += textBox_PreviewMouseLeftButtonDown;
            textBox.GotFocus += textBox_GotFocus;
            textBox.TextChanged += textBox_TextChanged;
            textBox.PreviewKeyDown += textBox_PreviewKeyDown;
            LostFocus += textBox_LostFocus;
            citysControl.SelectedChanged += citysControl_SelectedChanged;
            btn.Click += btn_Click;

            base.OnApplyTemplate();
        }


        void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            citysControl.SetKeyDown(e);
        }

        void textBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    tb.Focus();
                    e.Handled = true;
                }
            }
            if (popup.IsOpen == false)
                OpenPopup();
        }

        private void OpenPopup()
        {
            popup.StaysOpen = true;
            IsOpen = true;
        }
        #region empty
        private void ResetEmpty()
        {
            if (textBox != null)
            {
                if (!string.IsNullOrEmpty(textBox.Text) && c_Empty == textBox.Text)
                {
                    //SetText(string.Empty);
                }
            }
        }
        private void SetEmpty()
        {
            if (textBox != null)
            {
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    //SetText(c_Empty);
                }
            }
        }
        #endregion
        void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ResetEmpty();
            textBox.SelectAll();

            OpenPopup();
        }

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetEmpty();
            if (citysControl.IsMouseOver)
                return;

            popup.StaysOpen = false;
            IsOpen = false;
            SelectedCity = citysControl.CurrentCity;

            if (SelectedCity != null)
                SetText(SelectedCity.Info.Name);
        }

        void SetFocused()
        {
            //textBox.GotFocus -= textBox_GotFocus;
            //textBox.Focus();
            //textBox.GotFocus += textBox_GotFocus;
            textBox.SelectAll();
        }

        void citysControl_SelectedChanged(object sender, EventArgs e)
        {
            popup.StaysOpen = false;
            IsOpen = false;

            SelectedCity = citysControl.CurrentCity;
            SetFocused();
        }

        private void SetText(string text)
        {
            if (textBox == null)
            {
                cacheText = text;
                return;
            }

            textBox.TextChanged -= textBox_TextChanged;
            textBox.Text = text;
            textBox.TextChanged += textBox_TextChanged;
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            citysControl.Search(textBox.Text);
            popup.StaysOpen = false;
            IsOpen = true;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }

        private void Set(CityNewModel cityModel)
        {
            if (cityModel == null)
                SetText(null);
            else
                SetText(cityModel.Info.Name);
        }

        #endregion

        #region 公开属性

        #region SelectedCity

        /// <summary>
        /// The <see cref="SelectedCity" /> dependency property's name.
        /// </summary>
        public const string SelectedCityPropertyName = "SelectedCity";

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedCity" />
        /// property. This is a dependency property.
        /// </summary>
        public CityNewModel SelectedCity
        {
            get
            {
                return (CityNewModel)GetValue(SelectedCityProperty);
            }
            set
            {
                SetValue(SelectedCityProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedCity" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCityProperty = DependencyProperty.Register(
            SelectedCityPropertyName,
            typeof(CityNewModel),
            typeof(CityPicker),
            new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
            {
                CityPicker control = sender as CityPicker;
                control.Set(e.NewValue as CityNewModel);
            })));

        #endregion

        #region IsOpen

        /// <summary>
        /// The <see cref="IsOpen" /> dependency property's name.
        /// </summary>
        public const string IsOpenPropertyName = "IsOpen";

        /// <summary>
        /// Gets or sets the value of the <see cref="IsOpen" />
        /// property. This is a dependency property.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return (bool)GetValue(IsOpenProperty);
            }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsOpen" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            IsOpenPropertyName,
            typeof(bool),
            typeof(CityPicker),
            new PropertyMetadata(false));

        #endregion

        #endregion
    }
}
