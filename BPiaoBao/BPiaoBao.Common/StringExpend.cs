using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BPiaoBao.Common
{
    public static class StringExpend
    {
        /// <summary>
        /// QQ号码正则
        /// </summary>
        public const string QQPattern = @"^\d{5,12}$";
        /// <summary>
        /// 电话号码正则表达式（支持手机号码，3-4位区号，7-8位直播号码，1－4位分机号）
        /// </summary>
        public const string PhonePattern = @"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)";
        /// <summary>
        /// 正则表达式验证
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="regularExpression">正则表达式</param>
        /// <returns>源字符串是否匹配正则表达式</returns>
        public static bool IsMatch(this string str,string pattern)
        {
            Regex reg=new Regex(pattern);
            return reg.IsMatch(str);
        }
    }
}
