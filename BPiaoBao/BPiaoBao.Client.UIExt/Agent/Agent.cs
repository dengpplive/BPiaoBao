using System.Collections.Generic;
using System.Linq;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.Client.UIExt.Model;
using BPiaoBao.Common;
using JoveZhao.Framework;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.Client.UIExt
{
    /// <summary>
    //静态数据代理类 
    /// </summary>
    public class Agent
    {


        /// <summary>
        /// 得到所有航空公司列表
        /// </summary>
        /// <returns></returns>
        public static List<CarrayModel> GetCarryInfos()
        {
            var temp = ExtHelper.GetCarryInfos();
            var list = temp.Select(args => new CarrayModel
            {
                AirCode = args.Carry.AirCode,
                AirName = args.Carry.AirCode + "-" + args.Carry.AirShortName,
                AirSettle = args.Carry.AirSettle,
                AirShortName = args.Carry.AirShortName,
            }).ToList();

            return list;
        }
        /// <summary>
        /// 得到常旅客信息
        /// </summary>
        /// <returns></returns>
        public static List<PasserModel> SearchPassenger(string name)
        {

            var list = new List<PasserModel>();
            CommunicateManager.Invoke<IFrePasserService>(service =>
            {
                var data = service.Query(name);
                list = data.Select(args => new PasserModel
                {
                    AirCardNo=args.AirCardNo,
                    CertificateNo=args.CertificateNo,
                    CertificateType=args.CertificateType,
                    Mobile=args.Mobile,
                    Name=args.Name,         
                    PasserType=args.PasserType,
                    SexType = GetSexType(args.SexType),
                    Birth = args.Birth,
                   
                }).ToList();

            }, ex => Logger.WriteLog(LogType.INFO, "获取常旅客信息失败", ex));
             

            return list;
        }

        public static EnumSexType GetSexType(string sextype)
        {
            switch (sextype)
            {
                case "男":
                    return EnumSexType.Male;
                case "女":
                    return EnumSexType.Female;
                default:
                    return EnumSexType.UnKnown;
            }
        }

        /// <summary>
        /// 获取所有城市
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public static List<CityNewModel> GetCitys()
        {
            var temp = ExtHelper.GetAllCitys();
            var source = temp.Select(args => new CityNewModel
            {
                Info = new CityInfoModel
                {
                    AirPortName = args.city.AirPortName,
                    Code = args.city.Code,
                    JPPinyin = args.city.JPPinyin,
                    Name = args.city.Name,
                    PinYin = args.city.PinYin
                },
                Key = args.key
            });
            return source.ToList();
        }


        /// <summary>
        /// 搜索城市
        /// </summary>
        /// <param name="key">The text.</param>
        /// <returns></returns>
        public static List<CityNewModel> SearchCity(string key)
        {
            if (key == null)
                key = "";
            string strInputData = key.Trim();
            var cityList = GetCitys();
            if (!string.IsNullOrEmpty(strInputData))
            {
                cityList = cityList.FindAll(
                        p => p.Info.Name.ToUpper().Trim().StartsWith(strInputData.ToUpper().Trim())
                        || p.Info.Code.ToUpper().StartsWith(strInputData.ToUpper().Trim())
                        || p.Info.PinYin.ToUpper().Trim().Contains(strInputData.ToUpper().Trim())
                        || p.Info.JPPinyin.ToUpper().Trim().Contains(strInputData.ToUpper().Trim())
                        || p.Info.AirPortName.ToUpper().Trim().StartsWith(strInputData.ToUpper().Trim())
                        );
            }

            return cityList;
        }


        
    }
}
