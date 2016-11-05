using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace BPiaoBao.Client.ValueConverter
{
    public class ChangeWindowState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                WindowState window = (WindowState)value;
                if (window == WindowState.Maximized)
                {
                    return @"Images/Tab2_01.png";
                }
                else
                {
                    return @"Images/Tab2_03.png";
                }
            }
            return @"Images/Tab2_01.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
