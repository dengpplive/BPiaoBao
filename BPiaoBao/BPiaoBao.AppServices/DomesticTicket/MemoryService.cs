using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.ScheduleTask;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class MemoryService : IMemoryService
    {

        public PagedList<AirSystem> GetMemoryAirList(int page, int rows)
        {
            var list = SystemConsoSwitch.AirSystems;

            return new PagedList<AirSystem>
            {
                Total = list.Count,
                Rows = list.Skip((page - 1) * rows).Take(100).ToList()
            };
        }

        public PagedList<PlatSystem> GetMemoryPlatList(int page, int rows)
        {
            var list = SystemConsoSwitch.PlatSystems;
            return new PagedList<PlatSystem>
            {
                Total = list.Count,
                Rows = list.Skip((page - 1) * rows).Take(100).ToList()
            };
        }

        public void InitSystemSwitchInfo()
        {
            InitSystemSwitch.Init();
        }


        /// <summary>
        /// 航空公司新增
        /// </summary>
        /// <param name="vm"></param>
        public string AriLineSave(AirLine vm)
        {
            if (vm == null)
                return "设置信息失败";
            List<AirLine> list = null;
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "config\\AirLineManage.xml";
            if (!File.Exists(path))
            {
                vm.Id = 1;
                list = new List<AirLine>();
                list.Add(vm);
                BPiaoBao.Common.XmlHelper.XmlSerializeToFile(list, path, Encoding.Default);
            }
            else
            {
                list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<AirLine>>(path, Encoding.Default);
                if (list.Where(x => x.CarrayCode.Equals(vm.CarrayCode) & x.CarrayName.Equals(vm.CarrayName)).FirstOrDefault() != null)
                    return "此航空公司已存在!";
                vm.Id = list.Max(x => x.Id).HasValue ? list.Max(x => x.Id) + 1 : 1;
                list.Add(vm);
                BPiaoBao.Common.XmlHelper.XmlSerializeToFile(list, path, Encoding.Default);

            }
            InitSystemSwitch.InitAirSystem();
            return "成功";
        }

        /// <summary>
        ///  删除航空公司
        /// </summary>
        /// <param name="CarrayCode"></param>
        public void DeleteAriLine(int Id)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "config\\AirLineManage.xml";
            List<AirLine> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<AirLine>>(path, Encoding.Default);
            list.Remove(list.Where(x => x.Id == Id).FirstOrDefault());
            BPiaoBao.Common.XmlHelper.XmlSerializeToFile(list, path, Encoding.Default);
            InitSystemSwitch.InitAirSystem();

        }

        /// <summary>
        /// 修改航空公司信息
        /// </summary>
        /// <param name="vm"></param>
        public string ModiflyAriLine(AirLine vm)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "config\\AirLineManage.xml";
            List<AirLine> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<AirLine>>(path, Encoding.Default);
            AirLine airLine = list.Where(x => x.CarrayCode.Equals(vm.CarrayCode) & x.CarrayName.Equals(vm.CarrayName) & x.CarrayAbbreviation.Equals(vm.CarrayAbbreviation)).FirstOrDefault();
            if (airLine != null)
            {
                if (airLine.Id != vm.Id)
                    return "此航空公司已存在!";
            }
            list.ForEach(x =>
            {
                if (x.Id == vm.Id)
                {
                    x.CarrayName = vm.CarrayName;
                    x.CarrayCode = vm.CarrayCode;
                    x.CarrayAbbreviation = vm.CarrayAbbreviation;
                    x.B2BStatus = vm.B2BStatus;
                    x.BSPStatus = vm.BSPStatus;
                    x.AriLineStatus = vm.AriLineStatus;
                    x.AriLineType = vm.AriLineType;
                }
            });
            BPiaoBao.Common.XmlHelper.XmlSerializeToFile(list, path, Encoding.Default);
            InitSystemSwitch.InitAirSystem();
            return "成功";
        }

        public AirLine GetAirLineInfo(int Id)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "config\\AirLineManage.xml";
            List<AirLine> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<AirLine>>(path, Encoding.Default);
            return list.Where(x => x.Id == Id).FirstOrDefault();
        }
        /// <summary>
        ///  航空公司查询
        /// </summary>
        /// <returns></returns>
        public PagedList<AirLine> GetAriLineList(AirLine model, int page, int rows)
        {

            string path = System.AppDomain.CurrentDomain.BaseDirectory + "config\\AirLineManage.xml";
            if (!File.Exists(path))
            {
                return new PagedList<AirLine>
                {
                    Total = 0,
                    Rows = new List<AirLine>()
                };
            }
            else
            {
                List<AirLine> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<AirLine>>(path, Encoding.Default);
                if (!string.IsNullOrEmpty(model.CarrayName))
                    list = list.Where(x => x.CarrayName == model.CarrayName).ToList();
                if (!string.IsNullOrEmpty(model.CarrayCode))
                    list = list.Where(x => x.CarrayCode.Equals(model.CarrayCode)).ToList();
                if (!string.IsNullOrEmpty(model.CarrayAbbreviation))
                    list = list.Where(x => x.CarrayAbbreviation.Equals(model.CarrayAbbreviation)).ToList();
                if (model.B2BStatus.HasValue)
                    list = list.Where(x => x.B2BStatus == model.B2BStatus).ToList();
                if (model.BSPStatus.HasValue)
                    list = list.Where(x => x.BSPStatus == model.BSPStatus).ToList();
                if (model.AriLineStatus.HasValue)
                    list = list.Where(x => x.AriLineStatus == model.AriLineStatus).ToList();
                if (!string.IsNullOrEmpty(model.AriLineType))
                    list = list.Where(x => x.AriLineType.Equals(model.AriLineType)).ToList();

                return new PagedList<AirLine>
                {
                    Total = list.Count,
                    Rows = list.OrderByDescending(x => x.Id).Skip((page - 1) * rows).Take(rows).ToList()
                };
            }
        }

        /// <summary>
        /// 修改航变设置信息
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("修改航变设置信息")]
        public void SetAirChangTimeOutInfo(string QTStartTime, string QTEndTime, string TimeOut,bool? IsOpen)
        {
            bool _status ;
            if (!IsOpen.HasValue)
                _status = false;
            else
                _status = IsOpen.Value;
            string _code = AuthManager.GetCurrentUser().Code;
            string _path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "AirChange.xml");
            List<QTSetting> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<QTSetting>>(_path, Encoding.Default);
            if (int.Parse(TimeOut) > 60)
            {
                if (int.Parse(TimeOut) >= 600)
                    TimeOut = "" + int.Parse(TimeOut) / 60 + ":00:00";
                else
                    TimeOut = "0" + int.Parse(TimeOut) / 60 + ":" + int.Parse(TimeOut) % 60 + ":00";
            }
            else
            {
                TimeOut = "00:" + TimeOut + ":00";
            }
            if (list.Where(x => x.Code == _code).FirstOrDefault() == null)
            {
                QTSetting model = new QTSetting()
                { 
                    QTEndTime = QTEndTime,
                    QTStartTime = QTStartTime,
                    Timeout = TimeOut,
                    Code = _code ,
                    IsOpen = _status
                };
                list.Add(model);
                ScheduleTaskServices.RegisterTask(new GetQTInfo(model), new IntervalSchedule(TimeSpan.Parse(TimeOut), DateTime.Now));
            }
            else
            {
                list.ForEach(x =>
                {
                    if (x.Code == _code)
                    {
                        x.QTStartTime = QTStartTime;
                        x.QTEndTime = QTEndTime;
                        x.Timeout = TimeOut;
                        x.Code = AuthManager.GetCurrentUser().Code;
                        x.IsOpen = _status;
                    }
                });
            }
            BPiaoBao.Common.XmlHelper.XmlSerializeToFile(list, _path, Encoding.Default);
            var info = SystemConsoSwitch.QTSettingList.Where(x => x.Code == _code).FirstOrDefault();
            if (info == null)
            {
                info = new QTSetting() { Code = _code, Timeout = TimeOut, QTEndTime = QTEndTime, QTStartTime = QTStartTime, IsOpen = _status };
                SystemConsoSwitch.QTSettingList.Add(info);
            }
            else
            {
                info.QTEndTime = QTEndTime;
                info.QTStartTime = QTStartTime;
                info.Timeout = TimeOut;
                info.IsOpen = _status;
            }
        }

        /// <summary>
        /// 获取设置航变时间间隔
        /// </summary>
        /// <returns></returns>
        public QTSetting GetAirChangeTimeOutInfo()
        {
            string _path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "AirChange.xml");
            List<QTSetting> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<QTSetting>>(_path, Encoding.Default);
            var  info = list.Where(x => x.Code == AuthManager.GetCurrentUser().Code).FirstOrDefault();
            return info;
        }

    }
}
