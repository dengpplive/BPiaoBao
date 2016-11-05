using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JoveZhao.Framework
{
    public static class Regexes
    {
        public static Regex Num = new Regex("^[0-9]*$");
        public static Regex TwoDecimals = new Regex("^[0-9]+(.[0-9]{2})?$");
        public static Regex ChineseChar = new Regex("^[\u4e00-\u9fa5]{0,}$");
        public static Regex Email = new Regex("^\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$");
        public static Regex Url = new Regex("^http://([\\w-]+\\.)+[\\w-]+(/[\\w-./?%&=]*)?$");
        public static Regex Tel = new Regex("^(\\(\\d{3,4}-)|\\d{3.4}-)?\\d{7,8}$");
    }
}
