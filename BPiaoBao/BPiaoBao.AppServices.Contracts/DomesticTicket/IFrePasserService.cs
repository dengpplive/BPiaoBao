using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;

namespace BPiaoBao.AppServices.Contracts.DomesticTicket
{
    /// <summary>
    /// 常旅客服务
    /// </summary>
    [ServiceContract]
    public interface IFrePasserService
    {
        /// <summary>
        /// 添加常旅客 
        /// </summary>
        /// <param name="passer"></param>
        [OperationContract]
        void SaveFrePasser(FrePasserDto passer);

        /// <summary>
        /// 修改常旅客
        /// </summary>
        /// <param name="passer"></param>
        [OperationContract]
        void UpdateFrePasser(FrePasserDto passer);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        [OperationContract]
        void DeleteFrePasser(int id);

        /// <summary>
        /// 查询分页显示常旅客列表
        /// </summary>
        /// <param name="queryDto"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<FrePasserDto> QueryFrePassers(QueryFrePasser queryDto, int pageIndex, int pageSize);

        /// <summary>
        /// 根据身份证查询常旅客是否存在
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="certificateNo">身份证号</param>
        /// <returns></returns>
        [OperationContract]
        bool Exists(string name, string certificateNo);


        /// <summary>
        /// 根据输入姓名模糊查找
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [OperationContract]
        List<FrePasserDto> Query(string name);

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="list"></param>

        [OperationContract]
        void Import(List<FrePasserDto> list);

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns></returns>
        [OperationContract]
        List<FrePasserDto> Export(QueryFrePasser queryDto);


    }
}
