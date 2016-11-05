using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BPiaoBao.Common
{
    /// <summary>
    /// xml序列化帮助类
    /// </summary>
    public class XmlSerializerHelper
    {
        /// <summary>
        /// xml序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serializer<T>(T obj)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var ser = new XmlSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {

                var writer = new XmlTextWriter(ms, Encoding.GetEncoding("GBK"));
                writer.Formatting = Formatting.Indented;
                ser.Serialize(writer, obj, ns);
                writer.Close();
                var xml = Encoding.Default.GetString(ms.ToArray());
                xml = xml.Replace("gb2312", "GBK");
                return xml;
            }

        }

        /// <summary>
        ///  xml反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlStr"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlStr)
        {

            if (string.IsNullOrEmpty(xmlStr)) return default (T);
            var xml = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.GetEncoding("GBK").GetBytes(xmlStr)))
            { 
                var obj = xml.Deserialize(ms);
                if (obj == null)
                {
                    return default(T);
                }
                return (T)obj;

            }
        }
    }
}
