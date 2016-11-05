using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.DomesticTicket.View.Converters
{
    /// <summary>
    /// 单人支付金额
    /// </summary>
    public class SinglePriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is PolicyDto)
            {
                PolicyDto model = (PolicyDto)value;
                //return model.TicketPrice - model.Commission;
                return model.PayMoney;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
