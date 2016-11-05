using BPiaoBao.AppServices.DataContracts.SystemSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface ILevelService
    {
       
        [OperationContract]
        LevelDto Find(int id);
        [OperationContract]
        List<LevelDto> GetLevels();
    }
}
