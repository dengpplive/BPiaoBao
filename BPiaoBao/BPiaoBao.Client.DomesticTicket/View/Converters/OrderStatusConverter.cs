using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using System.Windows;

namespace BPiaoBao.Client.DomesticTicket.View.Converters
{
    public class OrderStatusConverter : IValueConverter
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
             if (value is int?)
            {
                int? model = value as int?;
                if (!model.HasValue)
                    return null;

                string para = null;
                if (parameter != null)
                    para = parameter.ToString();

                switch (model.Value)
                {
                    // 生成订单等待选择政策
                    case 1:
                        if (para == "all")
                            return "生成订单等待选择政策";
                        else if (para == null)
                            return "新订单";
                        else
                            return "等待选择政策";
                    /// 新订单，等待支付
                    case 2:
                        if (para == "all")
                            return "新订单，等待支付";
                        else if (para == null)
                            return "新订单";
                        else
                            return "等待支付";
                    /// 支付成功 等待出票
                    case 3:
                        if (para == "all")
                            return "支付成功 等待出票";
                        else if (para == null)
                            return "支付成功";
                        else
                            return "等待出票";
                    /// 订单取消
                    case 4:
                        if (para == null || para == "all")
                            return "订单取消";
                        else
                            return "";
                    /// 已经出票，订单完成
                    case 5:
                        if (para == "all")
                            return "已经出票，订单完成";
                        else if (para == null)
                            return "已经出票";
                        else
                            return "订单完成";
                    /// 拒绝出票，等待退款
                    case 6:
                        if (para == "all")
                            return "暂停出票";
                        if (para == null)
                            return "暂停出票";
                        else
                            return "暂停出票";
                    /// 拒绝出票，等待退款(退款)
                    case 7:
                        if (para == "all")
                            return "拒绝出票，订单完成";
                        else if (para == null)
                            return "拒绝出票";
                        else
                            return "订单完成";
                    /// 订单失效
                    case 8: if (para == null || para == "all")
                            return "订单失效";
                        else
                            return "";
                    /// 平台拒绝出票，等待退款(平台退款)
                    case 9:
                        if (para == null || para == "all")
                            return "拒绝出票，退款中";
                        else
                            return "";
                    ///线下婴儿申请,等待审核
                    case 13:
                        if (para == "all")
                            return "线下婴儿申请，等待审核";
                        if (para == null)
                            return "线下婴儿申请";
                        else
                            return "等待审核";
                    ///线下婴儿申请,拒绝审核
                    case 14:
                        if (para == "all")
                            return "拒绝审核，订单完成";
                        if (para == null)
                            return "拒绝审核";
                        else
                            return "订单完成";
                    ///新订单，支付中
                    case 16:
                        if (para == "all")
                            return "新订单，支付中";
                        if (para == null)
                            return "新订单";
                        else
                            return "支付中";
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 行程单状态转换
    /// </summary>
    public class ShowTripStatusConverter : IValueConverter
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
            if (value == null)
            {
                return "";
            }

            return EnumHelper.GetDescription((EnumPassengerTripStatus)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 机票状态转换
    /// </summary>
    public class ShowTickStatusVisibilityConverter : IValueConverter
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
            if (value == null && parameter == null)
            {
                return Visibility.Collapsed;
            }
            //if (value.ToString().Contains("改") && parameter.ToString().Equals("改"))
            //    return Visibility.Visible;
            //else if ((value.ToString().Contains("退") || value.ToString().Contains("废") || value.ToString().Contains("出")) && parameter.ToString().Equals("other"))
            //    return Visibility.Visible;
            //else if (value.ToString().Contains("出") && parameter.ToString().Equals("出"))
            //    return Visibility.Visible;
            //else if ((value.ToString().Contains("退") || value.ToString().Contains("废") || value.ToString().Contains("改")) && parameter.ToString().Equals("other2"))
            //    return Visibility.Visible;
            //else
            //    return Visibility.Collapsed;
            if ((value.ToString().Contains("出")) && parameter.ToString().Equals("after"))
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 订单金额转换
    /// </summary>
    public class ShowTicketMoneyConverter : IValueConverter
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
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            if (value.ToString().Contains("退") || value.ToString().Contains("废"))
            {

                return Visibility.Visible;
            }
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 支付查询按钮显隐转换
    /// </summary>
    public class ShowBtnPayStatusVisableConverter : IValueConverter
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
            if (value == null) return false;
            return !string.IsNullOrWhiteSpace(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 重选政策按钮显隐转换
    /// </summary>
    public class ShowBtnConfirmVisableConverter : IValueConverter
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
            if (value == null) return false;
            return (EnumOrderSource)value == EnumOrderSource.LineOrder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
