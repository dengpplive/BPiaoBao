using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
* CLR版本: 4.0.30319.34014
* 文件名：ShowBoolTextConverter
* 命名空间：BPiaoBao.Client.Account.View.Converters
* 类名：ShowBoolTextConverter
* 用户名：duanwei
* 创建日期：2014/5/8 11:41:15
* 描述：
*/
using System.Windows;
using System.Windows.Data;

namespace BPiaoBao.Client.Account.View.Converters
{
    public class ShowBoolTextConverter : IValueConverter
    {
        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定源生成的值。</param>
        /// <param name="targetType">绑定目标属性的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>
        /// 转换后的值。如果该方法返回 null，则使用有效的 null 值。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "否";
            }
            if (((bool)value))
            {
                return "是";
            }
            return "否";
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定目标生成的值。</param>
        /// <param name="targetType">要转换到的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>
        /// 转换后的值。如果该方法返回 null，则使用有效的 null 值。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

     
}
