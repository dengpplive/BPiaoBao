using BPiaoBao.AppServices.DataContracts.Cashbag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BPiaoBao.Client.UIExt.Converter
{
    /// <summary>
    /// 可购买理财产品 百分比
    /// </summary>
    public class FinancialProductPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is FinancialProductDto))
                return value;

            var model = value as FinancialProductDto;

            if (model.MaxAmount == 0)
                return "100.00%";
            var result = model.CurrentAmount / model.MaxAmount * 100;
            return string.Format("{0:f2}%", result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
