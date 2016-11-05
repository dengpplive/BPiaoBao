using BPiaoBao.Common.Enums;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.SystemSetting.View.Converter
{
    /// <summary>
    /// 员工状态
    /// </summary>
    public class OperatorStateConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value==null)
                return null;
          return  ((EnumOperatorState)value).ToEnumDesc();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
