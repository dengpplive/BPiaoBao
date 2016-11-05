using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.DomesticTicket.View.Converters
{
    /// <summary>
    /// 结算金额转换器
    /// </summary>
    public class SettlementAmountConverter : IValueConverter
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
            if (value is OrderDto)
            {
                var model = value as OrderDto;
                if (model.Passengers == null) return null;

                var totalTaxFee = model.Passengers.Sum(m => m.TaxFee);//所有机建费
                var totalRQFee = model.Passengers.Sum(m => m.RQFee);//所有燃油费
                var totalSeatPrice = model.Passengers.Sum(m => m.SeatPrice);//所有舱位价
                var commission = model.Policy.Commission;//佣金
                var passengerCount = model.Passengers.Count;

                string result = String.Format("=（舱位价{0}+机建费{1}+燃油费{2}-佣金{3}）* {4}",
                    totalSeatPrice.ToString("f2"), totalTaxFee.ToString("f2"),
                    totalRQFee.ToString("f2"), commission.ToString("f2"), passengerCount.ToString());

                return result;

            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
