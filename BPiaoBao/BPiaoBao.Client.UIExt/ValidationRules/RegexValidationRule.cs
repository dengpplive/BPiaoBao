using System;
using System.Windows.Controls;

namespace BPiaoBao.Client.UIExt.ValidationRules
{
    class RegexValidationRule : ValidationRule
    {
        /// <summary>
        /// 当在派生类中重写时，对值执行验证检查。
        /// </summary>
        /// <param name="value">要检查的来自绑定目标的值。</param>
        /// <param name="cultureInfo">要在此规则中使用的区域性。</param>
        /// <returns>
        /// 一个 <see cref="T:System.Windows.Controls.ValidationResult" /> 对象。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
