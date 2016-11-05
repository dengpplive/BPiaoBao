using System;

namespace BPiaoBao.Client.UIExt.Helper
{
    public class DateTimeHelper
    {


        /// <summary>
        /// 获取月日（格式：04-20）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetMonthDay(DateTime dt)
        {
            var dateStr = dt.ToString("yyyy-MM-dd");
            var temp = dateStr.Split('-');
            var md = temp[1] + "-" + temp[2];
            return md;
        }

        /// <summary>
        /// 根据日期获取星期几
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetWeekByDate(DateTime dt)
        {
            var week = "";
            var wk = dt.DayOfWeek.ToString();
            switch (wk)
            {

                case "Monday":

                    week = "星期一";

                    break;

                case "Tuesday":

                    week = "星期二";

                    break;

                case "Wednesday":

                    week = "星期三";

                    break;

                case "Thursday":

                    week = "星期四";

                    break;

                case "Friday":

                    week = "星期五";

                    break;

                case "Saturday":

                    week = "星期六";

                    break;

                case "Sunday":

                    week = "星期日";

                    break;

            }
            return week;
        }
    }
}
