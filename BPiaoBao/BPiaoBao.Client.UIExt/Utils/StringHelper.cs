using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectHelper.Utils
{
    public static class StringHelper
    {
        public static readonly char[] s_EmptyStr = "\r\n ".ToCharArray();
        public static readonly char[] s_NewLineStr = "\r\n".ToCharArray();

        public static string TrimEx(this string text, char[] format = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            return text.Trim(format ?? s_EmptyStr);
        }
        public static string FirstParagraphEx(this string text)
        {
            return text.TrimEx().Split(s_NewLineStr, StringSplitOptions.RemoveEmptyEntries).First();
        }

        public static string UpperFirstCharEx(this string text)
        {
            var v = text.TrimEx();
            const int index = 1;
            v = text.Substring(0, index).ToUpper() + v.Substring(index);
            return v;
        }
    }
}
