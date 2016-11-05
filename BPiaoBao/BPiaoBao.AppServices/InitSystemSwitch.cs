using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices
{
    /// <summary>
    /// 初始化系统开关
    /// </summary>
    public class InitSystemSwitch
    {
        /// <summary>
        /// 初始化所有
        /// </summary>
        public static void Init()
        {
            InitAirSystem();
            InitPlatSystem();
            InitAirChangeTimeOut();
            SystemConsoSwitch.Rate = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["MoneyRate"]);
        }
        /// <summary>
        /// 初始化航空信息
        /// </summary>
        public static void InitAirSystem()
        {
            try
            {
                string _path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "AirLineManage.xml");
                if (SystemConsoSwitch.AirSystems == null)
                    SystemConsoSwitch.AirSystems = new List<AirSystem>();
                SystemConsoSwitch.AirSystems.Clear();
                if (File.Exists(_path))
                {
                    List<AirLine> list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<AirLine>>(_path, Encoding.Default);
                    if (list != null && list.Count > 0)
                        list.ForEach(p =>
                        {
                            SystemConsoSwitch.AirSystems.Add(new AirSystem
                            {
                                AirCode = p.CarrayCode,
                                IsB2B = p.B2BStatus.Value,
                                IsBSP = p.BSPStatus.Value,
                                IsQuery = p.AriLineStatus.Value
                            });
                        });
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "初始化航空公司信息失败", e);
            }
        }

        /// <summary>
        /// 初始化航变设置信息
        /// </summary>
        public static void InitAirChangeTimeOut()
        {
            try
            {
                string _path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "AirChange.xml");
                if (!File.Exists(_path))
                {
                    List<QTSetting> list = new List<QTSetting>();
                    BPiaoBao.Common.XmlHelper.XmlSerializeToFile(list, _path, Encoding.Default);
                }
                else
                {
                    List<QTSetting> list = new List<QTSetting>();
                    list = BPiaoBao.Common.XmlHelper.XmlDeserializeFromFile<List<QTSetting>>(_path, Encoding.Default);
                    list.ForEach(x =>
                    {              
                        SystemConsoSwitch.QTSettingList.Add(x);
                    });
                }

            }
            catch (Exception e)
            {
                //Logger.WriteLog(LogType.ERROR, "初始化航变设置信息失败", e);
            }
        }
        /// <summary>
        /// 初始化平台接口信息
        /// </summary>
        public static void InitPlatSystem()
        {
            try
            {
                string _path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "platform.config");
                if (SystemConsoSwitch.PlatSystems == null)
                    SystemConsoSwitch.PlatSystems = new List<PlatSystem>();
                SystemConsoSwitch.PlatSystems.Clear();
                if (File.Exists(_path))
                {
                    var platFormService = StructureMap.ObjectFactory.GetInstance<PlatformCfgService>();
                    platFormService.GetPlatformConfigurationSection().Platforms.ForEach(p =>
                    {
                        PlatSystem platSystem = new PlatSystem();
                        platSystem.B2B = p.b2bClose;
                        platSystem.BSP = p.bspClose;
                        platSystem.GetPolicyCount = p.ShowCount;
                        platSystem.IssueTicketSpeed = p.IssueSpeed;
                        platSystem.PlatfromCode = p.Name;
                        platSystem.State = !p.IsClosed;
                        if (platSystem.SystemBigArea == null)
                            platSystem.SystemBigArea = new SystemBigArea();
                        platSystem.SystemBigArea.DefaultCity = p.Areas.DefaultCity;
                        if (platSystem.SystemBigArea.SystemAreas == null)
                            platSystem.SystemBigArea.SystemAreas = new List<SystemArea>();
                        p.Areas.Areas.ForEach(m =>
                        {
                            SystemArea systemArea = new SystemArea();
                            systemArea.City = m.City;
                            if (systemArea.Parameters == null)
                                systemArea.Parameters = new List<AreaParameter>();
                            systemArea.Parameters.AddRange(m.Parameters.Select(n =>
                                new AreaParameter { Name = n.Name, Value = n.Value })
                                );
                            platSystem.SystemBigArea.SystemAreas.Add(systemArea);
                        });
                        SystemConsoSwitch.PlatSystems.Add(platSystem);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "初始化平台接口信息失败", e);
            }
        }
    }
}
