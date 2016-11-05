using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework;

namespace BPiaoBao.Client.SystemSetting.View.Converter
{
    /// <summary>
    /// 支付方式
    /// </summary>
    public class PayWayConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EnumPayMethod payWay = (EnumPayMethod)value;
            return payWay.ToEnumDesc();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
