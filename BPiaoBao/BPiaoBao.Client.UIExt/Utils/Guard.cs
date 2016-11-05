using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using JoveZhao.Framework.Expand;

namespace ProjectHelper.Utils
{
    public static class Guard
    {
        public static void CheckIsNullOrEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value.Trim()))
            {
                throw new Exception(string.Format("{0} 不能为空", name));
            }
           
        }

        public static void CheckMobilePhoneNum(string num, string msg = null)
        {
            if (string.IsNullOrEmpty(num) || string.IsNullOrEmpty(num.Trim()))
            {
                throw new Exception("手机号码不能为空");
            }

            if (!Regex.IsMatch(num.Trim(), @"^1[1-9]\d{9}$"))
            {
                //throw new Exception("手机号码不正确");
                throw new Exception(string.Format("{0}手机号码不正确：{1}", msg, num));
            }
        }

        public static void CheckRepeatEx<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> func, string msg = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException("集合不能为空");
            }

            source.ForEach(p =>
            {
                if (source.Count(p1 => p == (p1 as dynamic)) > 1)
                {
                    throw new Exception(string.Format("{0}有重复", msg));
                }
            });
        }
    }
}
