using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.UIExt.Converter
{
    /// <summary>
    /// 把乘客列表转换为一个字符串
    /// </summary>
    public class PassengersConverter : IValueConverter
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
            if (value is IList<PassengerDto> || value is IList<ResponseAfterSalePassenger>)
            {
                if (value is IList<PassengerDto>)
                {
                    var list = value as IList<PassengerDto>;
                    var result = GetSB(list.Select(m => m.PassengerName));

                    return result.ToString();
                }
                else if (value is IList<ResponseAfterSalePassenger>)
                {
                    var list = value as IList<ResponseAfterSalePassenger>;
                    var result = GetSB(list.Select(m => m.PassengerName));

                    return result.ToString();
                }
            }

            return null;
        }

        private StringBuilder GetSB(IEnumerable<string> enumerable)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in enumerable)
            {
                sb.AppendFormat("{0},", item);
            }

            if (sb.Length > 0)
                sb = sb.Remove(sb.Length - 1, 1);

            return sb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
