using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProjectHelper.Utils
{
    public static class EnumHelper
    {
        public static List<KeyValuePair<dynamic, string>> GetEnumKeyValues(Type type)
        {
            var buf = Enum.GetNames(type).Select(name =>
            {
                FieldInfo field = type.GetField(name);
                object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    throw new ArgumentException(string.Format("Enum.{0}没有DescriptionAttribute特性", name));
                }
                var f = (DescriptionAttribute)attributes.First();
                return new KeyValuePair<dynamic, string>(Enum.Parse(type, name), f.Description);
            }).ToList();
            return buf;
        }


        public static List<KeyValuePair<dynamic, string>> GetEnumKeyValuesPassger(Type type)
        {

            var result = new List<KeyValuePair<dynamic, string>>();
            var list = Enum.GetNames(type);

            foreach (var name in list)
            {
                FieldInfo field = type.GetField(name);
                object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    throw new ArgumentException(string.Format("Enum.{0}没有DescriptionAttribute特性", name));
                }
                var f = (DescriptionAttribute)attributes.First();
                if (f.Description != "全部")
                {
                    result.Add(new KeyValuePair<dynamic, string>(Enum.Parse(type, name), f.Description));
                }

            }

            return result;

            
        }
    }
}
