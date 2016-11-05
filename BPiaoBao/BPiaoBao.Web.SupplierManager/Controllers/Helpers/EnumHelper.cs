using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BPiaoBao.Web.SupplierManager.Controllers.Helpers
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
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

        /// <summary>
        /// 获取枚举项的Attribute
        /// </summary>
        /// <typeparam name="T">自定义的Attribute</typeparam>
        /// <param name="source">枚举</param>
        /// <returns>返回枚举,否则返回null</returns>
        public static T GetCustomAttribute<T>(Enum source) where T : Attribute
        {
            Type sourceType = source.GetType();
            string sourceName = Enum.GetName(sourceType, source);
            FieldInfo field = sourceType.GetField(sourceName);
            object[] attributes = field.GetCustomAttributes(typeof(T), false);
            foreach (object attribute in attributes)
            {
                if (attribute is T)
                    return (T)attribute;
            }
            return null;
        }

        /// <summary>
        ///获取DescriptionAttribute描述
        /// </summary>
        /// <param name="source">枚举</param>
        /// <returns>有description标记，返回标记描述，否则返回null</returns>
        public static string GetDescription(Enum source)
        {
            var attr = GetCustomAttribute<System.ComponentModel.DescriptionAttribute>(source);
            if (attr == null)
                return null;
            return attr.Description;
        }


        #region 获取枚举成员的名称
        /// <summary>
        /// 获取枚举成员的名称
        /// </summary>
        /// <typeparam name="T">枚举名,比如Enum1</typeparam>
        /// <param name="member">枚举成员实例或成员值,
        /// 范例:Enum1枚举有两个成员A=0,B=1,则传入Enum1.A或0,获取成员名称"A"</param>
        public static string GetMemberName<T>(object member)
        {
            //转成基础类型的成员值 
            object memberValue = (T) member;

            //获取枚举成员的名称
            return Enum.GetName(typeof(T), memberValue);
        }


     
        #endregion


        #region 通过字符串获取枚举成员实例
        /// <summary>
        /// 通过字符串获取枚举成员实例
        /// </summary>
        /// <typeparam name="T">枚举名,比如Enum1</typeparam>
        /// <param name="member">枚举成员的常量名或常量值,
        /// 范例:Enum1枚举有两个成员A=0,B=1,则传入"A"或"0"获取 Enum1.A 枚举类型</param>
        public static T GetInstance<T>(string member)
        {
            return  (T)(Enum.Parse(typeof(T), member, true));
        }
        #endregion
        
    }


}
