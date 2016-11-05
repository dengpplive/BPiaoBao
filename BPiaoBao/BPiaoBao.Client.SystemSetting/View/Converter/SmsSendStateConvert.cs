using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.SystemSetting.View.Converter
{
    /// <summary>
    /// 短信发送状态
    /// </summary>
    public class SmsSendStateConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result="成功";
            if (!System.Convert.ToBoolean(value))
                result = "失败";
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
