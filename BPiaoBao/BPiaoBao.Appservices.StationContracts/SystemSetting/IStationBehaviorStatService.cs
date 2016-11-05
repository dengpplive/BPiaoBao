using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting
{
    [ServiceContract]
    public interface IStationBehaviorStatService
    {
        [OperationContract]
        PagedList<ResponseBehaviorStat> FindBehaviorList(QueryBehaviorStatQuery query);
    }
}
