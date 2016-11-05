using System;
using System.IO;
using System.Xml.Serialization;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public class SerializeHelper
    {
        /// <summary>
        /// 把对象序列化为xml
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ObjectToXml(Object obj)
        {
            if (obj == null) return null;

            string xml;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                xml = writer.GetStringBuilder().ToString();
            }

            return xml;
        }

        /// <summary>
        /// 把xml序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T XmlToObject<T>(string xml)
        {
            if (String.IsNullOrWhiteSpace(xml))
                return default(T);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StringReader(xml))
            {
                var result = (T)serializer.Deserialize(reader);

                return result;
            }
        }
    }
}
