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
/*
* CLR版本: 4.0.30319.34014
* 文件名：SearchCombox 
* 类名：SearchCombox
* 用户名：duanwei
* 创建日期：2014/5/6 15:10:52
* 描述：
*/
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BPiaoBao.Client.UIExt.CommonControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:MvvmLight1.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:MvvmLight1.Controls;assembly=MvvmLight1.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:SearchCombox/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_DisabledVisual", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    public class PassengerPicker : Control
    {
        private const string c_Empty = "中文/英文";
        Popup popup;
        Button btn;
        TextBox textBox;
        Grid root;
        Grid disabledVisual;
        PassengerControl passengerControl;
        private int _passIndex = 0;
        /// <summary>
        /// 临时变量
        /// </summary>
        private string cacheText = string.Empty;

        static PassengerPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PassengerPicker), new FrameworkPropertyMetadata(typeof(PassengerPicker)));
        }

        public PassengerPicker()
        {
            MouseLeave += CarrayPicker_MouseLeave;

            KeyboardNavigation.SetIsTabStop(this, false);
            _passIndex = 0;
        }


        void CarrayPicker_MouseLeave(object sender, MouseEventArgs e)
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

            passengerControl = new PassengerControl();
            popup.Child = passengerControl;
            popup.StaysOpen = false;

            textBox.PreviewMouseLeftButtonDown += textBox_PreviewMouseLeftButtonDown;
            textBox.GotFocus += textBox_GotFocus;
            textBox.TextChanged += textBox_TextChanged;
            textBox.PreviewKeyDown += textBox_PreviewKeyDown;
            LostFocus += textBox_LostFocus;
            passengerControl.SelectedChanged += carrayControl_SelectedChanged;


            btn.Click += btn_Click;

            base.OnApplyTemplate();
        }


        void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            passengerControl.SetKeyDown(e);
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
            if (passengerControl.IsMouseOver)
                return;

            popup.StaysOpen = false;
            IsOpen = false;
            SelectedPassenger = passengerControl.CurrentPassenger;

            if (SelectedPassenger != null)
                SetText(SelectedPassenger.Name);
        }

        void SetFocused()
        {
            textBox.GotFocus -= textBox_GotFocus;
            textBox.Focus();
            textBox.GotFocus += textBox_GotFocus;
            textBox.SelectAll();
        }

        void carrayControl_SelectedChanged(object sender, EventArgs e)
        {
            popup.StaysOpen = false;
            IsOpen = false;

            SelectedPassenger = passengerControl.CurrentPassenger;
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
            passengerControl.Search(textBox.Text);
            popup.StaysOpen = false;
            IsOpen = true;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }

        private void Set(PasserModel passenger)
        {

            //if (passenger == null)
            //    SetText(null);
            //else
            //{
            //    SetText(passenger.Name);
            //    if (SelectedCommand != null)
            //    {
            //        var command = this.SelectedCommand as RelayCommand<PasserModel>;
            //        if (command != null)
            //        {
            //            var model=SelectedCommandParameter as PasserModel;
            //            if (model != null)
            //            {
            //                model.Index = passenger.Index;
            //                ((ICommand)SelectedCommand).Execute(model);
            //            }

            //        }
            //    }
            //}
            if (passenger == null)
            {
                SetText(textBox == null ? "" : textBox.Text);

            }
            else
            {
                SetText(passenger.Name);
                if (passenger.Index > 0)
                {
                    _passIndex = passenger.Index;
                }
               
            }
            if (SelectedCommand != null)
            {
                var command = this.SelectedCommand as RelayCommand<PasserModel>;
                if (command != null)
                {
                    var model = SelectedCommandParameter as PasserModel;
                    if (model != null)
                    {
                        model.Index = _passIndex;
                        ((ICommand) SelectedCommand).Execute(model);
                    }
                    else
                    {
                        model = new PasserModel() { Name = textBox == null ? "" : textBox.Text, Index = _passIndex };
                        ((ICommand)SelectedCommand).Execute(model);
                    }

                }
            }

        }

        #endregion


        #region 公开属性

        #region SelectedPassenger

        /// <summary>
        /// The <see cref="SelectedPassenger" /> dependency property's name.
        /// </summary>
        public const string SelectedPassengerPropertyName = "SelectedPassenger";

        /// <summary>
        /// Gets or sets the value of the <see cref="SelectedPassenger" />
        /// property. This is a dependency property.
        /// </summary>
        public PasserModel SelectedPassenger
        {
            get
            {
                return (PasserModel)GetValue(SelectedPassengerProperty);
            }
            set
            {
                SetValue(SelectedPassengerProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedPassenger" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedPassengerProperty = DependencyProperty.Register(
            SelectedPassengerPropertyName,
            typeof(PasserModel),
            typeof(PassengerPicker),
            new PropertyMetadata(null, new PropertyChangedCallback((sender, e) =>
            {
                PassengerPicker control = sender as PassengerPicker;
                control.Set(e.NewValue as PasserModel);
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
            typeof(PassengerPicker),
            new PropertyMetadata(false));

        #endregion

        #endregion


        #region 命令

        public static readonly DependencyProperty SelectedCommandProperty =
            DependencyProperty.Register("SelectedCommand", typeof(ICommand), typeof(PassengerPicker), new PropertyMetadata(null, null));
        public ICommand SelectedCommand
        {
            set { SetValue(SelectedCommandProperty, value); }
            get { return (ICommand)GetValue(SelectedCommandProperty); }
        }

        public static readonly DependencyProperty SelectedCommandParameterProperty = DependencyProperty.Register("SelectedCommandParameter",
                                                                                                     typeof(object),
                                                                                                     typeof(PassengerPicker),
                                                                                                     new PropertyMetadata((object)null));

        public object SelectedCommandParameter
        {
            get
            {
                return (object)GetValue(SelectedCommandParameterProperty);
            }
            set
            {
                SetValue(SelectedCommandParameterProperty, value);
            }
        }


        #endregion
    }
}
