using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket.CityMapper
{
    public class FromCityResolver : ValueResolver<BPiaoBao.DomesticTicket.Domain.Models.Orders.SkyWay, string>
    {
        protected override string ResolveCore(BPiaoBao.DomesticTicket.Domain.Models.Orders.SkyWay source)
        {
            string city = string.Empty;
            PnrAnalysis.PnrResource pnrResource = new PnrAnalysis.PnrResource();
            var cityInfo = pnrResource.GetCityInfo(source.FromCityCode);
            if (cityInfo != null)
                city = cityInfo.city.Name;
            return city;
        }
    }
    public class ToCityResolver : ValueResolver<BPiaoBao.DomesticTicket.Domain.Models.Orders.SkyWay, string>
    {
        protected override string ResolveCore(BPiaoBao.DomesticTicket.Domain.Models.Orders.SkyWay source)
        {
            string city = string.Empty;
            PnrAnalysis.PnrResource pnrResource = new PnrAnalysis.PnrResource();
            var cityInfo = pnrResource.GetCityInfo(source.ToCityCode);
            if (cityInfo != null)
                city = cityInfo.city.Name;
            return city;
        }
    }


}
