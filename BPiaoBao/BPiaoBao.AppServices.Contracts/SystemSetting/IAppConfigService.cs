using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts.SystemSetting;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface IAppConfigService
    {
        [OperationContract]
        PlatformConfigurationSectionDto GetPlatformConfiguration();

        [OperationContract]
        void SavePlatformConfiguration(PlatformConfigurationSectionDto obj);

    }
}
