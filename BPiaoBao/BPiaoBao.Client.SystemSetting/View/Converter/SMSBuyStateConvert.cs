using BPiaoBao.Common.Enums;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.SystemSetting.View.Converter
{
    public class SMSBuyStateConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EnumPayStatus smsBuyState = (EnumPayStatus)value;
            return smsBuyState.ToEnumDesc();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
