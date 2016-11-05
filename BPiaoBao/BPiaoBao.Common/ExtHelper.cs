using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PnrAnalysis;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.Common
{
    public static class ExtHelper
    {
        private static PnrAnalysis.PnrResource pnrResource = new PnrAnalysis.PnrResource();
        /// <summary>
        /// 获取城市名称
        /// </summary>
        /// <param name="code">城市简码</param>
        /// <returns></returns>
        public static string GetCityNameByCode(string code)
        {
            var model = pnrResource.GetCityInfo(code);
            return model != null ? model.city.Name : string.Empty;
        }
        /// <summary>
        /// 获取航空公司简称
        /// </summary>
        /// <param name="code">航空公司三字码</param>
        /// <returns></returns>
        public static string GetAirNameByCode(string code)
        {
            var model = pnrResource.GetAirInfo(code);
            return model != null ? model.Carry.AirShortName : string.Empty;
        }
        /// <summary>
        /// 格式话时间
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static string FormatTime(string time1, string time2)
        {
            return PnrAnalysis.FormatPNR.GetIntersectionTimeSlot(time1, time2);
        }

        /// <summary>
        /// 获取所有城市
        /// </summary>
        /// <returns></returns>
        public static List<CityInfo> GetAllCitys()
        {
            return pnrResource.CityDictionary.CityList;
        }

        /// <summary>
        /// 得到所有航空公司列表
        /// </summary>
        /// <returns></returns>
        public static List<CarryInfo> GetCarryInfos()
        {
            return pnrResource.CarrayDictionary.CarrayList;
        }

        /// <summary>
        /// 根据城市得到城市Code
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCityCodeByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            var list = pnrResource.CityDictionary.CityList;
            var model = list.FirstOrDefault(p => p.city.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            return model == null ? "" : model.city.Code;

        }
        /// <summary>
        /// 截取身份证字符串得到可为空的时间类型出生日期
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DateTime? GetBirthdayDateFromString(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id.Trim().Length != 18) return null;
            var cardId = id;
            var year = cardId.Substring(6, 4);
            var month = cardId.Substring(10, 2);
            var date = cardId.Substring(12, 2);
            var result = year + "-" + month + "-" + date;
            DateTime birth;   
            if (DateTime.TryParse(result, out birth)) return birth;
            return null;
        }
        public static EnumPayMethod GetPayMethod(string payway)
        {
            switch (payway)
            {
                case "Recive":
                    return EnumPayMethod.Account;
                case "Credit":
                    return EnumPayMethod.Credit;
                case "AliPay":
                    return EnumPayMethod.AliPay;
                case "TenPay":
                    return EnumPayMethod.TenPay;
                case "InternetBank":
                    return EnumPayMethod.Bank;
                default:
                    return EnumPayMethod.TenPay;
            }
        }
    }
   
}
