using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace JoveZhao.Framework
{
    
    public class EnumItemManager
    {
        static Hashtable hs = new Hashtable();
        public static List<KeyValuePair<T?, string>> GetItemList<T>() where T:struct
        {
            var result = hs[typeof(T?)] as List<KeyValuePair<T?, string>>;
            if (result == null)
            {
                var fs = typeof(T).GetFields().Select(p => new
                {
                    p,
                    att = p.GetCustomAttributes(false).FirstOrDefault(q => q is DescriptionAttribute) as DescriptionAttribute
                }).Where(p => p.att != null).ToList();


                result = new List<KeyValuePair<T?, string>>();
                
                foreach (var f in fs)
                {
                    T t = (T)Enum.Parse(typeof(T), f.p.GetValue(null).ToString());
                    result.Add(new KeyValuePair<T?, string>(t, f.att.Description));
                }
                hs[typeof(T)] = result;
            }

          
            return result;
        }

        public static string GetDesc(Enum source )
        {
            if (source == null) return string.Empty;
            var l = GetCustomAttribute(source);
            if (l == null) return "";
            return l.Description;
        }

        public static DescriptionAttribute GetCustomAttribute(Enum source)
        {
            if (source == null) return null;
            Type sourceType = source.GetType();
            string sourceName = Enum.GetName(sourceType, source);
            FieldInfo field = sourceType.GetField(sourceName);
            object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            foreach (object attribute in attributes)
            {
                if (attribute is DescriptionAttribute)
                    return attribute as DescriptionAttribute;
            }
            return null;
        }

    }
}
