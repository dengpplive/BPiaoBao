using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface IBehaviorStatService
    {
        

        [OperationContract]
        DataPack<ResponseBehaviorStat> Query(RequestQueryBehaviorStatQuery query);

    }
}
