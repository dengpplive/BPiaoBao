using BPiaoBao.AppServices.StationContracts.StationMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.DomesticTicket
{
    /// <summary>
    /// 运营扣点管理
    /// </summary>
    [ServiceContract]
    public interface IStationPlatformPointGroupService
    {
        /// <summary>
        /// 添加扣点组
        /// </summary>
        /// <param name="dataObject"></param>
        [OperationContract]
        void AddPointGroup(PlatformPointGroupDataObject dataObject);
        /// <summary>
        /// 更新扣点组
        /// </summary>
        /// <param name="dataObject"></param>
        [OperationContract]
        void UpdatePointGroup(PlatformPointGroupDataObject dataObject);
        /// <summary>
        /// 删除扣点组
        /// </summary>
        /// <param name="guids"></param>
        [OperationContract]
        void DeletePointGroup(Guid[] guids);
        /// <summary>
        /// 获取扣点组
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        PlatformPointGroupDataObject FindPointGroupByID(Guid guid);
        /// <summary>
        /// 扣点组列表查询
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<PlatformPointGroupDataObject> GetPointGroup(string code);
        /// <summary>
        /// 添加扣点规则
        /// </summary>
        [OperationContract]
        void AddPointRule(PlatformPointGroupRuleDataObject dataObject);
        /// <summary>
        /// 更新扣点规则
        /// </summary>
        /// <param name="dataObject"></param>
        [OperationContract]
        void UpdatePointRule(PlatformPointGroupRuleDataObject dataObject);
        /// <summary>
        /// 删除扣点规则
        /// </summary>
        /// <param name="ids"></param>
        [OperationContract]
        void DeletePointRule(int[] ids);
        /// <summary>
        /// 获取扣点规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        PlatformPointGroupRuleDataObject FindPointRuleByID(int id);
        /// <summary>
        /// 获取扣点组规则
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<PlatformPointGroupRuleDataObject> GetPlatformPointGroupRuleDataObject(Guid? guid);
        /// <summary>
        /// 扣点组商户
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        List<CodePoint> GetCodePoint(Guid? guid);
        /// <summary>
        /// 获取运营平台
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        List<BResponse> GetPlatform(int? id);
        /// <summary>
        /// 获取所有扣点组
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<PointGroup> GetAllPointGroup();
    }
}
