using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interactivity;
//using Microsoft.Expression.Interactivity.Core;

namespace BPiaoBao.Client.UIExt.Behaviors
{
    public class DatePickerDisplayDateStartBehavior : Behavior<DatePicker>
    {
        public DatePickerDisplayDateStartBehavior()
        {
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.DisplayDateStart = DateTime.Now;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

        }

    }
}