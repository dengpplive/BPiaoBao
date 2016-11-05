using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace BPiaoBao.Common
{
    public static class XmlHelper
    {
        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            settings.Encoding = encoding;
            settings.IndentChars = "    ";

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                XmlSerializeInternal(file, o, encoding);
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding);
        }



    }

    public class AutoIssueTicketSet
    {
        public void CreateXDocument(AutoIssueTicketViewModel vm)
        {
            //string path = System.AppDomain.CurrentDomain.BaseDirectory + vm.Code + ".xml";
            //XDocument xdoc = new XDocument();
            //var root = new XElement("Root");
            //vm.IssueTickets.ForEach(p => root.Add(new XElement("CarrayList", new XElement("Carray", new XAttribute("CarrayCode", p.CarrayCode), new XAttribute("Account", p.Account), new XAttribute("Pwd", p.Pwd)))));
            //root.Add(new XElement("Reconnection", new XAttribute("Count", vm.ReconnectionCount)));
            //root.Add(new XElement("Alipay", new XAttribute("Account", vm.Alipay)));
            //xdoc.Add(root);
            ////if(System.IO.File.Exists(path))
            //   // System.IO.File
            //xdoc.Save(System.AppDomain.CurrentDomain.BaseDirectory + "/" + vm.Code + ".xml");
        }
        public AutoIssueTicketViewModel GetInfoByCode(string code)
        {
            // string path = System.AppDomain.CurrentDomain.BaseDirectory + code + ".xml";
            AutoIssueTicketViewModel vm = new AutoIssueTicketViewModel();
            //if (System.IO.File.Exists(path))
            //{
            //    XElement root = XElement.Load(path);
            //    root.Element("CarrayList").Elements("Carray").Select(p => new IssueTicketModel
            //    {
            //        CarrayCode = p.Attribute("CarrayCode").Value,
            //        Account = p.Attribute("Account").Value,
            //        Pwd = p.Attribute("Pwd").Value
            //    }).ToList().ForEach(p =>
            //    {
            //        vm.IssueTickets.Add(p);
            //    });
            //    vm.ReconnectionCount = root.Element("Reconnection").Attribute("Count").Value;
            //    vm.Alipay.Account = root.Element("Alipay").Attribute("Account").Value;
            //}
            //else
            //{
            ExtHelper.GetCarryInfos().ForEach(p => vm.IssueTickets.Add(new IssueTicketModel { CarrayCode = p.AirCode, CarrayName = p.Carry.AirShortName, Account = string.Empty, Pwd = string.Empty, ContactName = string.Empty, Phone = string.Empty }));

            //}
            return vm;
        }
    }
    public class AutoIssueTicketViewModel
    {
        public AutoIssueTicketViewModel()
        {
            if (IssueTickets == null)
                IssueTickets = new List<IssueTicketModel>();
            if (string.IsNullOrEmpty(ReconnectionCount))
            {
                ReconnectionCount = "3";
            }
        }
        /// <summary>
        /// 运营商或者供应商商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<IssueTicketModel> IssueTickets { get; set; }
        /// <summary>
        /// 重连次数
        /// </summary>
        public string ReconnectionCount { get; set; }
        /// <summary>
        /// 支付宝帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 自动出票设置【B2B】
        /// </summary>
        public bool B2B { get; set; }
        /// <summary>
        /// 自动出票设置【BSP】
        /// </summary>
        public bool BSP { get; set; }
    }

    public class IssueTicketModel
    {
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 航空公司名称
        /// </summary>
        public string CarrayName { get; set; }
        /// <summary>
        /// 帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
    }
}
