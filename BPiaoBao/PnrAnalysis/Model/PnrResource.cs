using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;

namespace PnrAnalysis
{
    public class PnrResource
    {
        private CityDictionary m_CityDictionary = null;

        public CityDictionary CityDictionary
        {
            get
            {
                return m_CityDictionary;
            }
            set
            {
                m_CityDictionary = value;
            }
        }
        private CarrayDictionary m_CarrayDictionary = null;

        public CarrayDictionary CarrayDictionary
        {
            get
            {
                return m_CarrayDictionary;
            }
            set
            {
                m_CarrayDictionary = value;
            }
        }
        public PnrResource()
        {
            try
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                //string[] strArr = executingAssembly.GetManifestResourceNames();
                using (Stream stream = executingAssembly.GetManifestResourceStream("PnrAnalysis.DLL.city.resources"))
                {
                    byte[] bytes = new byte[stream.Length];
                    int len = stream.Read(bytes, 0, bytes.Length);
                    if (len > 0)
                    {
                        this.CityDictionary = DeserializeObject(bytes) as CityDictionary;
                    }
                }
                using (Stream stream = executingAssembly.GetManifestResourceStream("PnrAnalysis.DLL.aircode.resources"))
                {
                    byte[] bytes = new byte[stream.Length];
                    int len = stream.Read(bytes, 0, bytes.Length);
                    if (len > 0)
                    {
                        this.CarrayDictionary = DeserializeObject(bytes) as CarrayDictionary;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("资源数据未获取到,获取资源文件不存在");
            }
        }
        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public CityInfo GetCityInfo(string Name)
        {
            CityInfo cityInfo = null;
            if (m_CityDictionary != null && !string.IsNullOrEmpty(Name.ToLower().Trim()))
            {
                cityInfo = m_CityDictionary.CityList.Find(p => p.city.Name.ToUpper().Trim() == Name.ToUpper().Trim()
                    || p.key == Name.ToUpper().Trim()
                    || p.city.PinYin.Trim().ToLower() == Name.ToLower().Trim()
                    || p.city.JPPinyin.Trim().ToLower() == Name.ToLower().Trim()
                    );
                //if (cityInfo == null)
                //{
                //    throw new Exception("资源数据中为获取到" + Name + "信息！");
                //}
            }
            return cityInfo;
        }

        /// <summary>
        /// 获取航空公司信息
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public CarryInfo GetAirInfo(string airCode)
        {
            CarryInfo carryInfo = null;
            if (CarrayDictionary != null && !string.IsNullOrEmpty(airCode))
            {
                carryInfo = CarrayDictionary.CarrayList.Find(p => p.Carry.AirName.ToUpper().Trim() == airCode.ToUpper().Trim()
                    || p.AirCode == airCode.ToUpper().Trim()
                    || p.Carry.AirShortName == airCode.ToUpper().Trim()
                    );
                //if (carryInfo == null)
                //{
                //    throw new Exception("资源数据中为获取到" + airCode + "信息！");
                //}
            }
            return carryInfo;
        }

        /// <summary>
        /// 写入二进制文件
        /// </summary>
        /// <param name="filepath"></param>
        private static void WriteBinaryFiles(string filepath, byte[] b)
        {
            FileStream filesstream = new FileStream(filepath, FileMode.Create);
            BinaryWriter objBinaryWriter = new BinaryWriter(filesstream);
            objBinaryWriter.Write(b);
            objBinaryWriter.Close();
            filesstream.Close();
        }
        /// <summary>
        /// 把对象序列化并返回相应的字节
        /// </summary>
        /// <param name="pObj">需要序列化的对象</param>
        /// <returns>byte[]</returns>
        private static byte[] SerializeObject(object pObj)
        {
            if (pObj == null)
                return null;
            System.IO.MemoryStream _memory = new System.IO.MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(_memory, pObj);
            _memory.Position = 0;
            byte[] read = new byte[_memory.Length];
            _memory.Read(read, 0, read.Length);
            _memory.Close();
            return read;
        }
        /// <summary>
        /// 把字节反序列化成相应的对象
        /// </summary>
        /// <param name="pBytes">字节流</param>
        /// <returns>object</returns>
        private static object DeserializeObject(byte[] pBytes)
        {
            object _newOjb = null;
            if (pBytes == null)
                return _newOjb;
            System.IO.MemoryStream _memory = new System.IO.MemoryStream(pBytes);
            _memory.Position = 0;
            BinaryFormatter formatter = new BinaryFormatter();
            _newOjb = formatter.Deserialize(_memory);
            _memory.Close();
            return _newOjb;
        }

    }
    [Serializable]
    public class CityDictionary
    {
        public CityDictionary()
        {
            this.CityList = new List<CityInfo>();
        }
        /// <summary>
        /// 城市信息集合
        /// </summary>
        public List<CityInfo> CityList
        {
            get;
            set;
        }
    }


    [Serializable]
    public class CityInfo
    {
        /// <summary>
        /// 三字码
        /// </summary>
        public string key
        {
            get;
            set;
        }
        /// <summary>
        /// 对应城市信息
        /// </summary>
        public City city
        {
            get;
            set;
        }
    }

    [Serializable]
    public class City
    {
        /// <summary>
        /// 城市名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 城市三字码
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 城市简拼
        /// </summary>
        public string JPPinyin
        {
            get;
            set;
        }
        /// <summary>
        /// 城市全拼
        /// </summary>
        public string PinYin
        {
            get;
            set;
        }
        /// <summary>
        /// 城市机场名称
        /// </summary>
        public string AirPortName
        {
            get;
            set;
        }
    }
    //-----------------------------------------------
    [Serializable]
    public class CarrayDictionary
    {
        public CarrayDictionary()
        {
            this.CarrayList = new List<CarryInfo>();
        }
        /// <summary>
        /// 航空公司信息
        /// </summary>
        public List<CarryInfo> CarrayList
        {
            get;
            set;
        }
    }

    [Serializable]
    public class CarryInfo
    {
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string AirCode
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司信息
        /// </summary>
        public CarryModel Carry
        {
            get;
            set;
        }
    }
    [Serializable]
    public class CarryModel
    {
        /// <summary>
        /// 航空公司名称
        /// </summary>
        public string AirName
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string AirShortName
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string AirCode
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司结算码
        /// </summary>
        public string AirSettle
        {
            get;
            set;
        }
    }

}
