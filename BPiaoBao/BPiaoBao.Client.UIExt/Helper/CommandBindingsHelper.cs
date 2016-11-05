using System.Windows;
using System.Windows.Input;

namespace BPiaoBao.Client.UIExt.Helper
{
    public static class CommandBindingsHelper
    {
        public static CommandBindingCollection GetCommandBindings(DependencyObject obj)
        {
            return (CommandBindingCollection)obj.GetValue(CommandBindingsProperty);
        }

        public static void SetCommandBindings(DependencyObject obj, CommandBindingCollection value)
        {
            obj.SetValue(CommandBindingsProperty, value);
        }

        public static readonly DependencyProperty CommandBindingsProperty =
            DependencyProperty.RegisterAttached("CommandBindings", typeof(CommandBindingCollection), typeof(CommandBindingsHelper), new PropertyMetadata(null, OnCommandBindingsChanged));

        private static void OnCommandBindingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = sender as UIElement;
            if (element == null) return;

            var bindings = (e.NewValue as CommandBindingCollection);
            if (bindings != null)
            {
                element.CommandBindings.Clear();
                element.CommandBindings.AddRange(bindings);
            }
        }
    }
}
