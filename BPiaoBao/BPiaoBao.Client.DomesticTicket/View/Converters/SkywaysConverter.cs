using System.Globalization;
using System.Windows;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework.Expand;
using NPOI.SS.Formula.Functions;

namespace BPiaoBao.Client.DomesticTicket.View.Converters
{
    /// <summary>
    /// 把航班列表转换为字符串
    /// </summary>
    public class SkywaysConverter : IValueConverter
    {
        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定源生成的值。</param>
        /// <param name="targetType">绑定目标属性的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>
        /// 转换后的值。如果该方法返回 null，则使用有效的 null 值。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IList<SkyWayDto>)
            {
                var list = value as IList<SkyWayDto>;
                StringBuilder sb = new StringBuilder();
                foreach (var item in list)
                {
                    if (parameter == null)
                        sb.AppendFormat("{0} {1}-{2} {3},{4},", item.FlightNumber, item.FromCityCode, item.ToCityCode, item.StartDateTime.ToString("yyyy-MM-dd HH:mm"), item.Seat);
                    else
                        sb.AppendFormat("{0}-{1},", item.FromCity, item.ToCity);
                }

                if (sb.Length > 0)
                    sb = sb.Remove(sb.Length - 1, 1);
                var result = sb.ToString();
                return result;
            }
            if (value is IList<ResponseSkyWay>)
            {

                var list = value as IList<ResponseSkyWay>;
                StringBuilder sb = new StringBuilder();
                foreach (var item in list)
                {
                    if (parameter == null)
                        sb.AppendFormat("{0} {1}-{2} {3},{4},", item.FlightNumber, item.FromCityCode, item.ToCityCode, item.StartDateTime.ToString("yyyy-MM-dd HH:mm"), item.Seat);
                    else
                        sb.AppendFormat("{0}-{1},", item.FromCity, item.ToCity);
                }

                if (sb.Length > 0)
                    sb = sb.Remove(sb.Length - 1, 1);
                var result = sb.ToString();
                return result;
            }

            return null;
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定目标生成的值。</param>
        /// <param name="targetType">要转换到的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>
        /// 转换后的值。如果该方法返回 null，则使用有效的 null 值。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 支付方式
    /// </summary>
    public class PayWayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            var  payWay = (EnumPayMethod)value;
            return payWay.ToEnumDesc();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    /// <summary>
    /// 乘客转换
    /// </summary>
    public class PassengersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<ResponsePassenger>)
            {
                var sb = new StringBuilder();
                var passers = (IList<ResponsePassenger>)value;
                foreach (var p in passers)
                {
                    sb.AppendFormat("{0},", p.PassengerName);
                }
                if (sb.Length > 0)
                {
                    var str = sb.ToString().Substring(0, sb.Length - 1);
                    return str;
                }
            }
            if (value is ResponseAfterSalePassenger)
            {
                var sb = new StringBuilder();
                var passers = (IList<ResponseAfterSalePassenger>)value;
                foreach (var p in passers)
                {
                    sb.AppendFormat("{0},", p.PassengerName);
                }
                if (sb.Length > 0)
                {
                    var str = sb.ToString().Substring(0, sb.Length - 1);
                    return str;
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 非也
    /// </summary>
    public class NotBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return true;
            if (value is bool)
                return !(bool)value;
            if (value is FlightInfoModel)
            {
                var model = value as FlightInfoModel;
                if (model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial) return true;
                return model.DefaultSite.IsGotSpecial && model.DefaultSite.IsReceivedSpecial;
            }
            if (value is Site)
            {
                var site = value as Site;
                if (site.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial) return true;
                return site.IsGotSpecial && site.IsReceivedSpecial;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 非显
    /// </summary>
    public class NotBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;//正常执行非显
            if(value is bool) return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            if (value is FlightInfoModel)//默认舱位舱位价
            {
                var model = value as FlightInfoModel;
                if (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Visible;
                return (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && model.DefaultSite.IsGotSpecial && model.DefaultSite.IsReceivedSpecial)
                    || (model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.Normal && model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            if (!(value is Site)) return Visibility.Visible;
            var site = value as Site;//更多舱位舱位价
            if (site.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Visible;
            return (site.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && site.IsGotSpecial && site.IsReceivedSpecial)
                || (site.PolicySpecialType != EnumPolicySpecialType.Normal && site.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial)
                ? Visibility.Visible
                : Visibility.Collapsed;
            #region Past Code
            //if (value is FlightInfoModel)//默认舱位舱位价
            //{
            //    var model = value as FlightInfoModel;
            //    if (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Visible;
            //    return (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && model.DefaultSite.IsGotSpecial)
            //        || (model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.Normal && model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial)
            //        ? Visibility.Visible
            //        : Visibility.Collapsed;
            //}
            //if (!(value is Site)) return Visibility.Visible;
            //var site = value as Site;//更多舱位舱位价
            //if (site.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Visible;
            //return (site.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && site.IsGotSpecial)
            //        || (site.PolicySpecialType != EnumPolicySpecialType.Normal && site.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial)
            //    ? Visibility.Visible
            //    : Visibility.Collapsed; 
            #endregion
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 布尔值 特价转换为可见或隐藏
    /// </summary>
    public class SpecialBooleanToVisibilityConverter : IValueConverter
    {
        #region IValueConverter 成员

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定源生成的值。</param>
        /// <param name="targetType">绑定目标属性的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>转换后的值。如果该方法返回 null，则使用有效的 null 值。</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool) return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            if (value is EnumPolicySpecialType) return (EnumPolicySpecialType)value == EnumPolicySpecialType.Normal ? Visibility.Collapsed : Visibility.Visible;//特价标识
            if (parameter != null)
            {
                int param;
                int.TryParse(parameter.ToString(), out param);
                if (param != 0) return Visibility.Collapsed;
                if (value is FlightInfoModel)//无运价按钮
                {
                    var model = value as FlightInfoModel;
                    if (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Collapsed;
                    var res = (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && (model.DefaultSite.IsReceivedSpecial && model.DefaultSite.IsGotSpecial))
                        || (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && !model.DefaultSite.IsGotSpecial)
                        || (model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial && model.DefaultSite.PolicySpecialType != EnumPolicySpecialType.Normal)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                    return res;
                }
                if (!(value is Site)) return Visibility.Collapsed;
                var site = value as Site;//无运价按钮
                if (site.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Collapsed;
                return (site.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && site.IsReceivedSpecial && site.IsGotSpecial)
                    || (site.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && !site.IsGotSpecial)
                    || (site.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial && site.PolicySpecialType != EnumPolicySpecialType.Normal)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            else
            {
                if (value is FlightInfoModel)//默认舱位特价按钮
                {
                    var model = value as FlightInfoModel;
                    if (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Collapsed;
                    var res = (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && !model.DefaultSite.IsGotSpecial)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    return res;
                }
                if (!(value is Site)) return Visibility.Collapsed;
                var site = value as Site;//更多舱位特价按钮
                if (site.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Collapsed;
                return site.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && !site.IsGotSpecial
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            //if (value is FlightInfoModel)//默认舱位特价按钮
            //{
            //    var model = value as FlightInfoModel;
            //    if (model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Collapsed;
            //    var res = model.DefaultSite.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && !model.IsGotSpecial
            //        ? Visibility.Visible
            //        : Visibility.Collapsed;
            //    return res;
            //}
            //if (!(value is Site)) return Visibility.Collapsed;
            //var site = value as Site;//更多舱位特价按钮
            //if (site.PolicySpecialType == EnumPolicySpecialType.Normal) return Visibility.Collapsed;
            //return site.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial && !site.IsGotSpecial
            //    ? Visibility.Visible
            //    : Visibility.Collapsed;
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定目标生成的值。</param>
        /// <param name="targetType">要转换到的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>转换后的值。如果该方法返回 null，则使用有效的 null 值。</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// --Decimal
    /// </summary>
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";
            if (!(value is decimal)) return "--";
            var v = (decimal) value;
            var i = System.Convert.ToInt32(v);
            return i > 0 ? i.ToString(CultureInfo.InvariantCulture) : "--";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //LocalPolicyBooleanToVisibilityConverter
    /// <summary>
    /// 本地政策（本地）字段表示显隐转换
    /// </summary>
    public class LocalPolicyBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定源生成的值。</param>
        /// <param name="targetType">绑定目标属性的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>
        /// 转换后的值。如果该方法返回 null，则使用有效的 null 值。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value.ToString() == "本地")
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        /// <summary>
        /// 转换值。
        /// </summary>
        /// <param name="value">绑定目标生成的值。</param>
        /// <param name="targetType">要转换到的类型。</param>
        /// <param name="parameter">要使用的转换器参数。</param>
        /// <param name="culture">要用在转换器中的区域性。</param>
        /// <returns>
        /// 转换后的值。如果该方法返回 null，则使用有效的 null 值。
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
