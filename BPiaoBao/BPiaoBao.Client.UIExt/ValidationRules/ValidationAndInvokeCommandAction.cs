using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BPiaoBao.Client.UIExt.ValidationRules
{
    /// <summary>
    /// 执行数据验证，验证通过执行命令
    /// </summary>
    public class ValidationAndInvokeCommandAction : TargetedTriggerAction<UIElement>
    {
        private string commandName;
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ValidationAndInvokeCommandAction), null);
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ValidationAndInvokeCommandAction), null);

        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                ICommand command = this.ResolveCommand();

                if (!IsDataValid())
                    return;

                if ((command != null) && command.CanExecute(this.CommandParameter))
                {
                    command.Execute(this.CommandParameter);
                }
            }
        }

        //检查数据有效性
        private bool IsDataValid()
        {
            var dependencyObject = Target as DependencyObject;
            if (dependencyObject == null)
                return true;

            var result = IsValid(dependencyObject);
            return result;
        }
        bool IsValid(DependencyObject node)
        {
            if (node != null)
            {
                TextBox textBox = node as TextBox;
                //目标是文本框，并且文本框没有输入值。第一次没输入值,没触发绑定是不会触发数据检查的
                if (textBox != null /*&& String.IsNullOrEmpty(textBox.Text)*/)
                {
                    var temp = textBox.GetValue(AlwaysTriggerProperty);
                    if (temp != null && temp is bool && (bool)temp)
                    {
                        textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    }
                }

                bool isValid = !Validation.GetHasError(node);
                if (!isValid)
                {
                    //输入控件获取焦点
                    if (node is IInputElement) Keyboard.Focus((IInputElement)node);
                    return false;
                }
            }

            foreach (object subnode in LogicalTreeHelper.GetChildren(node))
            {
                if (subnode is DependencyObject)
                {
                    if (IsValid((DependencyObject)subnode) == false) return false;
                }
            }

            return true;
        }

        private ICommand ResolveCommand()
        {
            ICommand command = null;
            if (this.Command != null)
            {
                return this.Command;
            }
            if (base.AssociatedObject != null)
            {
                foreach (PropertyInfo info in base.AssociatedObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (typeof(ICommand).IsAssignableFrom(info.PropertyType) && string.Equals(info.Name, this.CommandName, StringComparison.Ordinal))
                    {
                        command = (ICommand)info.GetValue(base.AssociatedObject, null);
                    }
                }
            }
            return command;
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }

        public string CommandName
        {
            get
            {
                base.ReadPreamble();
                return this.commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
                    base.WritePreamble();
                    this.commandName = value;
                    base.WritePostscript();
                }
            }
        }

        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }

        #region AlwaysTrigger

        /// <summary>
        /// The AlwaysTrigger attached property's name.
        /// </summary>
        public const string AlwaysTriggerPropertyName = "AlwaysTrigger";

        /// <summary>
        /// Gets the value of the AlwaysTrigger attached property 
        /// for a given dependency object.
        /// </summary>
        /// <param name="obj">The object for which the property value
        /// is read.</param>
        /// <returns>The value of the AlwaysTrigger property of the specified object.</returns>
        public static bool GetAlwaysTrigger(DependencyObject obj)
        {
            return (bool)obj.GetValue(AlwaysTriggerProperty);
        }

        /// <summary>
        /// Sets the value of the AlwaysTrigger attached property
        /// for a given dependency object. 
        /// </summary>
        /// <param name="obj">The object to which the property value
        /// is written.</param>
        /// <param name="value">Sets the AlwaysTrigger value of the specified object.</param>
        public static void SetAlwaysTrigger(DependencyObject obj, bool value)
        {
            obj.SetValue(AlwaysTriggerProperty, value);
        }

        /// <summary>
        /// Identifies the AlwaysTrigger attached property.
        /// </summary>
        public static readonly DependencyProperty AlwaysTriggerProperty = DependencyProperty.RegisterAttached(
            AlwaysTriggerPropertyName,
            typeof(bool),
            typeof(ValidationAndInvokeCommandAction),
            new PropertyMetadata(false));

        #endregion
    }
}
