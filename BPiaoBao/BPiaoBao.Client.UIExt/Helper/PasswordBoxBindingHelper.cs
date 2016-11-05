using System.Windows;
using System.Windows.Controls;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// 使password控件支持绑定
    /// </summary>
    public static class PasswordBoxBindingHelper
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc... 
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBindingHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxBindingHelper), new FrameworkPropertyMetadata(false, Attach));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxBindingHelper));

        /// <summary>
        /// Sets the attach.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Gets the attach.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">The value.</param>
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        /// <summary>
        /// Sets the is updating.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        /// <summary>
        /// Gets the is updating.
        /// </summary>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        public static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void OnPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            if (pb == null) return;
            pb.PasswordChanged -= pb_PasswordChanged;
            if (!GetIsUpdating(pb))
            {
                pb.Password = (string)e.NewValue;
            }

            pb.PasswordChanged += pb_PasswordChanged;
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            if (pb == null) return;

            if ((bool)e.OldValue)
            {
                pb.PasswordChanged -= pb_PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += pb_PasswordChanged;
            }
        }

        static void pb_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = sender as PasswordBox;
            if (pb == null) return;
            SetIsUpdating(pb, true);
            SetPassword(pb, pb.Password);
            SetIsUpdating(pb, false);
        }
    }
}
