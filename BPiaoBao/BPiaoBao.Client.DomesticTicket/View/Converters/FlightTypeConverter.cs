using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt.Helper;

namespace BPiaoBao.Client.DomesticTicket.View.Converters
{
   public class FlightTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "单程";
            }
            if (((FlightTypeEnum)value) == FlightTypeEnum.SingleWay)
            {
                return "单程";
            }
            else if (((FlightTypeEnum)value) == FlightTypeEnum.DoubleWay)
            {
                return "往返";
            }
            else if (((FlightTypeEnum)value) == FlightTypeEnum.ConnWay)
            {
                return "联程";
            }
            else
            {
                return "单程";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


   public class FlightTypeBoolConverter : IValueConverter
   {
       public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       {
           if (value == null)
           {
               return false;
           }
           var v = EnumHelper.GetInstance<FlightTypeEnum>(value.ToString());
           if (((int)v).ToString().Contains(parameter.ToString()))
           {
               return true;
           }
           return false;
       }

       public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
       {
           throw new NotImplementedException();
       }
   }
}
