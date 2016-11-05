using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class UnitTest5
    {
        [TestMethod]
        public void TestMethod1()
        {
            var user = new User(){Age=21,Name = "段伟",Sex="男"};
            var str=XmlSerializerHelper.Serializer(user);
            Console.WriteLine("User序列化后：\n{0}",str);
        }
        [TestMethod]
        public void TestMethod2()
        {
            var xml = "<?xml version='1.0' encoding='GBK'?><User><Name>段伟</Name><Sex>男</Sex><Age>21</Age></User>";
            var user = XmlSerializerHelper.Deserialize<User>(xml);
            Console.WriteLine("User反序列化后：\n{0}", JsonConvert.SerializeObject(user));
        }

           [TestMethod]
        public void TestMethod3()
           {
               var list = new List<User>();
               list.Add(new User() { Age = 21, Name = "段伟1", Sex = "男" });
               list.Add(new User() { Age = 22, Name = "段伟2", Sex = "男" });
               list.Add(new User() { Age = 23, Name = "段伟3", Sex = "男" });
               var str = XmlSerializerHelper.Serializer(list);
               Console.WriteLine("List序列化后：\n{0}", str);
        }

        [TestMethod]
        public void TestMethod4()
        {
            var xml = "";
            var list = XmlSerializerHelper.Deserialize<User>(xml);
            Console.WriteLine("List反序列化后：\n{0}", JsonConvert.SerializeObject(list));
        }

        [TestMethod]
        public void TestMethod5()
        {
            //var _zk=new _ZKInsurancePlatform();
            //var start = DateTime.Now.AddDays(5);
            //var result = InsurancePlatformDomainService.Buy_Insurance("XYJR"+GetPayNo(),start, start.AddDays(7),
            //    "段伟", EnumIDType.NormalId, "513902198509164878", EnumSexType.Male, null, "13880227460", 1);
            //var result = InsurancePlatformDomainService.Buy_Insurance("XYJR201408081221211464562", Convert.ToDateTime("2014-08-13 07:05:00"), Convert.ToDateTime("2014-08-20 09:40:00"),
            //    "汤立军", EnumIDType.NormalId, "430219197210081017", EnumSexType.Male, Convert.ToDateTime("1972-10-08"), "", 1, "3U8931", Convert.ToDateTime("2014-08-13 07:05:00"), "温州");
            //Console.WriteLine(result);
        }

        public void TestMethod6()
        {
            //var _zk=new _ZKInsurancePlatform();
            var start = DateTime.Now.AddDays(5);
            var result = InsurancePlatformDomainService.Buy_Insurance("001819030863388", start, start.AddDays(7),
                "段琳", EnumIDType.BirthDate, "", EnumSexType.Male, DateTime.Now.AddYears(-1), "13880226666");
            Console.WriteLine(result);
        }

        [TestMethod]
        public void TestMethod7()
        {

            var result = InsurancePlatformDomainService.Refund_Insurance("001864682410388", "XYJR201408061014519647625");
            Console.WriteLine(result);
        }


        /// <summary>
        /// 获取保险记录支付用订单号
        /// </summary>
        /// <returns></returns>
        private string GetPayNo()
        {
            //每次生成随机数的时候都使用机密随机数生成器来生成种子，
            //这样即使在很短的时间内也可以保证生成的随机数不同
            var rand = new Random(GetRandomSeed());
            var exNum = rand.Next(1000, 9999);
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), exNum);
        }

        /// <summary>
        /// 得到随机数种子
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

    }


    [XmlRoot("User")]
    public class User
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Sex")]
        public string Sex { get; set; }

        [XmlElement("Age")]
        public int Age { get; set; }
    }
}
