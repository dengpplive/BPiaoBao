using BPiaoBao.AppServices.DataContracts.Cashbag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.UIExt.Converter
{
    /// <summary>
    /// 理财产品可用额度计算
    /// </summary>
    public class FinancialProductAvailableAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is FinancialProductDto))
                return value;

            var model = value as FinancialProductDto;

            var result = String.Format("{0} - {1}", model.LimitAmount, model.MaxAmount - model.CurrentAmount);

            if (model.MaxAmount - model.CurrentAmount <= 0)
                return 0;

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
