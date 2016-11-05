using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Platform.Plugin;

namespace BPiaoBao.AppServices.StationContracts.DomesticTicket
{
    /// <summary>
    /// 平台配置服务
    /// </summary>
    [ServiceContract]
    public interface IStationPlatformCfgService
    {


        /// <summary>
        /// 得到平台配置信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PlatformCfgDto GetPlatformConfigurationSection();

        /// <summary>
        /// 根据平台名，区域名获取参数列表
        /// </summary>
        /// <param name="platName"></param>
        /// <param name="areaName"></param>
        /// <returns></returns>
        [OperationContract]
        List<Parameter> GetParameters(string platName, string areaName);

        /// <summary>
        /// 根据平台名称获取区域列表
        /// </summary>
        /// <param name="platName"></param>
        /// <returns></returns>
        [OperationContract]
        List<string> GetAreaList(string platName);

        /// <summary>
        /// 保存平台配置
        /// </summary>
        /// <param name="section"></param>
        [OperationContract]
        void SavePlatFormInfo(Platform platform);


        /// <summary>
        /// 保存平台配置参数
        /// </summary>
        /// <param name="platName"></param>
        /// <param name="areaDto"></param>
        [OperationContract]
        void SaveParatersConfig(string platName, Area areaDto);

    }
}
