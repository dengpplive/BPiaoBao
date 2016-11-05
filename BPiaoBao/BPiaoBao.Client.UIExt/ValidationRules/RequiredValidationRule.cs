using System;
using System.Windows.Controls;

namespace BPiaoBao.Client.UIExt.ValidationRules
{
    /// <summary>
    /// 
    /// </summary>
    public class RequiredValidationRule : ValidationRule
    {
        /// <summary>
        /// 验证属性的描述性文字
        /// </summary>
        /// <value>
        /// The property description.
        /// </value>
        public string PropertyDescription { get; set; }

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
            if (value == null || String.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(false, PropertyDescription);

            return new ValidationResult(true, null);
        }
    }
}
