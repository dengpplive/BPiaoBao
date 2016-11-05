using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class PlatformCfgService : IStationPlatformCfgService
    {

        public PlatformCfgService()
        {

        }

        public PlatformCfgDto GetPlatformConfigurationSection()
        {
            var model = PlatformSection.GetInstances();

            var dto = new PlatformCfgDto();
            dto.Platforms = new List<Platform>();
            foreach (PlatFormElement plat in model.Platforms)
            {

                var pf = new Platform();
                pf.Areas = new AreaBigs();
                pf.Code = plat.Code;
                pf.Name = plat.Name;
                pf.IssueSpeed = plat.IssueSpeed;
                pf.IsClosed = plat.IsClosed;
                pf.ShowCount = plat.ShowCount;
                pf.b2bClose = plat.b2bClose;
                pf.bspClose = plat.bspClose;
                pf.PaidIsTest = plat.paidIsTest;
                pf.Areas.DefaultCity = plat.Areas.DefaultCity;
                pf.Areas.Areas = new List<Area>();
                foreach (AreaElement area in plat.Areas)
                {
                    var a = new Area();
                    a.Parameters = new List<Parameter>();
                    a.City = area.City;
                    foreach (ParameterElement ps in area.Parameters)
                    {
                        var per = new Parameter();
                        per.Description = ps.Description;
                        per.Name = ps.Name;
                        per.Value = ps.Value;
                        a.Parameters.Add(per);

                    }

                    pf.Areas.Areas.Add(a);
                }
                dto.Platforms.Add(pf);
            }

            return dto;

        }

        public List<Parameter> GetParameters(string platName, string areaName)
        {
            var list = new List<Parameter>();
            var model = PlatformSection.GetInstances();
            foreach (PlatFormElement m in model.Platforms)
            {
                if (!m.Name.ToLower().Equals(platName.ToLower())) continue;
                foreach (AreaElement area in m.Areas)
                {
                    if (string.Equals(area.City, areaName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (ParameterElement ps in area.Parameters)
                        {
                            var per = new Parameter();
                            per.Description = ps.Description;
                            per.Name = ps.Name;
                            per.Value = ps.Value;
                            list.Add(per);

                        }

                    }
                }
            }
            return list;
        }


        public List<string> GetAreaList(string platName)
        {
            var model = PlatformSection.GetInstances();
            return (from PlatFormElement m in model.Platforms where string.Equals(m.Name, platName, StringComparison.CurrentCultureIgnoreCase) from AreaElement a in m.Areas select a.City).ToList();
        }

        public void SavePlatFormInfo(Platform platform)
        {
            var old = PlatformSection.GetInstances();
            foreach (PlatFormElement m in old.Platforms)
            {

                if (string.Equals(m.Name, platform.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    m.Code = platform.Code;
                    m.IsClosed = platform.IsClosed;
                    m.ShowCount = platform.ShowCount;
                    m.paidIsTest = platform.PaidIsTest;
                    m.IssueSpeed = platform.IssueSpeed;
                    m.bspClose = platform.bspClose;
                    m.b2bClose = platform.b2bClose;
                }
            }
            PlatformSection.Save();
            InitSystemSwitch.InitPlatSystem();
        }
        public void SaveParatersConfig(string platName, Area areaDto)
        {
            var old = PlatformSection.GetInstances();
            foreach (PlatFormElement m in old.Platforms)
            {
                if (m.Name.ToLower().Equals(platName.ToLower()))
                {
                    foreach (AreaElement area in m.Areas)
                    {
                        if (area.City.ToLower().Equals(areaDto.City.ToLower()))
                        {
                            foreach (var dp in areaDto.Parameters)
                            {
                                area.Parameters[dp.Name].Value = dp.Value;
                            }
                        }
                    }
                }
            }

            PlatformSection.Save();
            InitSystemSwitch.InitPlatSystem();
        }
    }
}
