using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket
{
    /// <summary>
    /// 扣点组服务
    /// </summary>
    [ServiceContract]
    public interface IConsoDeductionGroupService
    {
        /// <summary>
        /// 添加扣点组
        /// </summary>
        /// <param name="deduction"></param>
        [OperationContract]
        void AddDeductionGroup(RequestDeduction deduction);
        /// <summary>
        /// 更新扣点组
        /// </summary>
        /// <param name="deduction"></param>
        [OperationContract]
        void EditDeductionGroup(RequestDeduction deduction);
        /// <summary>
        /// 查询扣点组
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseDeduction> FindDeductionByPager(string name, int currentPage, int pageSize);
        /// <summary>
        /// 查询信息根据ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        RequestDeduction GetDeductionByID(int id);
        /// <summary>
        /// 删除扣点组
        /// </summary>
        /// <param name="id"></param>
        [OperationContract]
        void DeleteDeduction(int id);
    }
}
