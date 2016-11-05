using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace BPiaoBao.Client.SystemSetting.Validation
{
    public class BuyCountValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int v;
            if (!int.TryParse((string)value, out v) || v <= 0)
            {
                return new ValidationResult(false, string.Format("请输入正确的购买条数"));
            }

            return ValidationResult.ValidResult;
        }
    }
}
