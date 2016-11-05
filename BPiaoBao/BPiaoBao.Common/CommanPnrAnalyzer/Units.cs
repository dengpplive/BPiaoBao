using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.CommanPnrAnalyzer
{
    public class Units
    {
        public static DateTime GetDateTime(string date, string time)
        {
            DateTime r = default(System.DateTime);
            string[] ws = { "SU", "MO", "TU", "WE", "TH", "FR", "SA" };
            string[] ms = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            //SA13SEP
            string w = date.Substring(0, 2);
            string d = date.Substring(2, 2);
            string m = date.Substring(4, 3);
            string h = time.Substring(0, 2);
            string mm = time.Substring(2, 2);
            int sy = DateTime.Now.Year - 2;
            while (sy < DateTime.Now.Year + 2)
            {
                r = new DateTime(sy, ms.GetIndexOf(m) + 1, d.ToInt(), h.ToInt(), mm.ToInt(), 0);
                if ((int)r.DayOfWeek == ws.GetIndexOf(w))
                    return r;
                sy++;
            }
            if (time.Contains("+"))
            {
                time = time.Substring(0, time.IndexOf("+"));
                r = r.AddDays(1);
            }
            throw new Exception("解析日期失败");
        }

        public static string GetFTerminal(string term)
        {
            if (string.IsNullOrEmpty(term))
                return "";
            if (term.First() == '-')
                return "-";
            return term.Substring(0, 2);
        }
        public static string GetTTerminal(string term)
        {
            if (string.IsNullOrEmpty(term))
                return "";
            if (term.Last() == '-')
                return "-";

            return term.Substring(term.Length - 2, 2);
        }
        public static decimal GetPrice(string price)
        {
            return price.Replace("CNY", "").ToDecimal();
        }
    }


    public static class Extent
    {
        public static int GetIndexOf(this string[] sl, string s)
        {
            for (var i = 0; i < sl.Length; i++)
            {
                if (string.Compare(sl[i], s, true) == 0) return i;
            }
            throw new Exception("没有从索引中解析到当前字符串");
        }
        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }
        public static decimal ToDecimal(this string s, decimal dft = 0m)
        {
            decimal.TryParse(s, out dft);
            return dft;
        }
    }
}
