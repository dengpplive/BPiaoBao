using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace BPiaoBao.Client.UIExt.ValidationRules
{
    /// <summary>
    /// 至少一个
    /// </summary>
    public class AtLeastOneValidationRule : ValidationRule
    {

        private static List<Tuple<string, string, string>> list = new List<Tuple<string, string, string>>();

        /// <summary>
        /// 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 验证属性的描述性文字
        /// </summary>
        public string PropertyDescription { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var exist = list.FirstOrDefault(m => m.Item1 == Key && m.Item2 == GroupName);
            if (exist != null)
                list.Remove(exist);

            if (value != null && !String.IsNullOrWhiteSpace(value.ToString()))
            {
                list.Add(new Tuple<string, string, string>(Key, GroupName, value.ToString()));
            }

            var otherData = list.FirstOrDefault(m => m.Item2 == GroupName && !String.IsNullOrEmpty(m.Item3));
            if (otherData == null)
                return new ValidationResult(false, PropertyDescription);

            return new ValidationResult(true, null);
        }
    }
}
