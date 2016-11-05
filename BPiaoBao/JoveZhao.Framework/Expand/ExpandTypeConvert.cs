using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JoveZhao.Framework.Expand
{
    public static class ExpandTypeConvert
    {
        public static int ToInt(this string str, int defaultValue = 0)
        {
            int.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static decimal ToDecimal(this string str, decimal defaultValue = 0)
        {
            decimal.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static short ToShort(this string str, short defaultValue = 0)
        {
            short.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static bool ToBool(this string str, bool defaultValue = false)
        {
            if (str == "1") defaultValue = true;
            if (str == "0") defaultValue = false;
            bool.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static byte ToByte(this string str, byte defaultValue = 0)
        {
            byte.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static char ToChar(this string str, char defaultValue = '0')
        {
            char.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static float ToFloat(this string str, float defaultValue = 0)
        {
            float.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static long ToLong(this string str, long defaultValue = 0)
        {
            long.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static sbyte ToSbyte(this string str, sbyte defaultValue = 0)
        {
            sbyte.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static uint ToUint(this string str, uint defaultValue = 0)
        {
            uint.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static ulong ToUlong(this string str, ulong defaultValue = 0)
        {
            ulong.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static ushort ToUshort(this string str, ushort defaultValue = 0)
        {
            ushort.TryParse(str, out defaultValue);
            return defaultValue;
        }
        
        public static DateTime ToDateTime(this string str, DateTime defaultValue)
        {

            DateTime.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static Guid ToGuid(this string str, Guid defaultValue = default(Guid))
        {
            Guid.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static T ToEnum<T>(this string str, T defaultValue = default(T)) where T : struct
        {
            Enum.TryParse<T>(str, out defaultValue);
            return defaultValue;
        }

        public static T JsonToObject<T>(this string str, T defaultValue = default(T))
        {
            try
            {
                defaultValue = JsonConvert.DeserializeObject<T>(str);
            }
            catch { }
            return defaultValue;
        }
        public static string ToJson(this object obj, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            string r = string.Empty;
            try
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = dateTimeFormat;
                r = JsonConvert.SerializeObject(obj,timeFormat);
            }
            catch { }
            return r;
        }
       

        public static object ToObject(this byte[] bytes)
        {
            MemoryStream streamMemory = new MemoryStream(bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(streamMemory);
        }
        public static T ToObject<T>(this byte[] bytes) where T : class
        {
            MemoryStream streamMemory = new MemoryStream(bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(streamMemory) as T;
            
         
        }
        public static string ToStr(this byte[] bytes) {
            return System.Text.Encoding.Default.GetString(bytes);
        }
        public static int ToInt(this byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
        public static bool ToBoolean(this byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes, 0);
        }
        public static char ToChar(this byte[] bytes)
        {
            return BitConverter.ToChar(bytes, 0);
        }
        public static double ToDouble(this byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }
        public static short ToInt16(this byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }
        public static long ToInt64(this byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }
        public static float ToSingle(this byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);
        }
        public static ushort ToUInt16(this byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static uint ToUInt32(this byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static ulong ToUInt64(this byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes, 0);
        }
        public static DateTime ToDateTime(this byte[] bytes, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var str = bytes.ToStr();
            return str.ToDateTime(default(DateTime));
        }


       
        public static byte[] GetBytes(this string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }
        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this bool value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this char value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this uint value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this ulong value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this ushort value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this DateTime value, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            return value.ToString(dateTimeFormat).GetBytes();
        }
        public static byte[] ToBytes<T>(this T obj) where T : class
        {
            MemoryStream streamMemory = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(streamMemory, obj);
            return streamMemory.GetBuffer();
        }


        public static T GetCustomAttribute<T>(this Type type) where T:Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }


        public static string ToEnumDesc(this Enum source)
        {
            return EnumItemManager.GetDesc(source);
        }
    }
}
