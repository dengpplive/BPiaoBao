using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket
{
    /// <summary>
    /// 政策服务
    /// </summary>
    [ServiceContract]
    public interface IConsoLocalPolicyService
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="policy"></param>
        [OperationContract]
        void AddLocalPolicy(RequestNormalPolicy policy);
        /// <summary>
        /// 政策修改
        /// </summary>
        /// <param name="policy"></param>
        [OperationContract]
        void UpdateLocalPolicy(RequestNormalPolicy policy);
        /// <summary>
        /// 局部修改
        /// </summary>
        /// <param name="policy"></param>
        [OperationContract]
        void PartUpdateLocalPolicy(RequestPartPolicy policy);
        /// <summary>
        /// 政策查看
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponsePolicy> FindPolicyByPager(SearchPolicy search, int page, int rows);
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="ids"></param>
        [OperationContract]
        void BatchReview(Guid[] ids);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        [OperationContract]
        void BatchDelete(Guid[] ids);
        /// <summary>
        /// 批量取消挂起
        /// </summary>
        /// <param name="ids"></param>
        [OperationContract]
        void BatchCancelHangUp(Guid[] ids);
        /// <summary>
        /// 批量挂起
        /// </summary>
        /// <param name="ids"></param>
        [OperationContract]
        void BatchHangUp(Guid[] ids);
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseFullPolicy Find(Guid id);
        /// <summary>
        /// 详情-政策对比
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract(Name = "FindInfo")]
        ResponseLocalPolicy Find(Guid polyid, int value = 0);
        /// <summary>
        /// 编辑查看
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        RequestNormalPolicy EditFind(Guid id);
        /// <summary>
        /// 部分视图修改元数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        RequestPartPolicy EditPartFind(Guid id);
        /// <summary>
        /// 导入政策
        /// </summary>
        /// <param name="list"></param>
        [OperationContract]
        void ImportPolicy(List<RequestNormalPolicy> list);
        /// <summary>
        /// 导入政策
        /// </summary>
        /// <param name="list"></param>
        [OperationContract(Name="Special")]
        void ImportPolicy(List<RequestSpecialPolicy> list);
        /// <summary>
        /// 获取基础舱位数据
        /// </summary>
        /// <param name="CarryCode"></param>
        /// <returns></returns>
        [OperationContract]
        CabinData GetBaseCabinData(string CarryCode);
        /// <summary>
        /// 获取所有政策信息(控台)
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Id"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponsePolicy> FindAllPolicyByPager(string Code, string Id, int page, int rows);
        /// <summary>
        /// 获取政策详情(控台)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseFullPolicy FindPolicyInfoById(Guid id);
        /// <summary>
        /// 政策对比
        /// </summary>
        /// <param name="carrayCode"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseLocalNormalPolicy> PolicyContrast(string carrayCode, string fromCityCode, string toCityCode, string localPolicyType, DateTime? startDate, DateTime? EndDate, string seat, int page, int rows);
        /// <summary>
        /// 政策调整点数
        /// </summary>
        /// <param name="policyId"></param>
        /// <param name="newPoint"></param>
        [OperationContract]
        void AdjustPoint(Guid policyId, decimal newPoint);
        /// <summary>
        /// 查询特价政策
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseSpecialPolicy> FindSpeciaPolicyByPager(SearchSpecialPolicy search, int page, int rows);
        /// <summary>
        /// 特价政策查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseOperPolicy EditSpeciaFind(Guid id);
        /// <summary>
        /// 修改特价政策
        /// </summary>
        /// <param name="policy"></param>
        [OperationContract]
        void UpdateSpeaiaPolicy(RequestSpecialPolicy policy);
        /// <summary>
        /// 新增特价政策
        /// </summary>
        /// <param name="policy"></param>
        [OperationContract]
        void AddSpeaiaPolicy(RequestSpecialPolicy policy);
        /// <summary>
        /// 特价政策详情查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseOperPolicy FindSpecialPolicyInfo(Guid id);
    }
}
