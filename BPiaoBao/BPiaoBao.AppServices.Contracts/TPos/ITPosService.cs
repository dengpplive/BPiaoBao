using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.TPos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.TPos
{
    [ServiceContract]
    public interface ITPosService
    {
        /// <summary>
        /// 添加Pos商户
        /// </summary>
        /// <param name="businessmanInfo"></param>
        [OperationContract]
        void AddBusinessman(RequestBusinessmanInfo businessmanInfo);
        /// <summary>
        /// 修改Pos商户
        /// </summary>
        /// <param name="businessmanInfo"></param>
        [OperationContract]
        void UpdateBusinessman(RequestBusinessmanInfo businessmanInfo);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Id">商户ID</param>
        [OperationContract]
        void DeleteBusinessman(string Id); 
        /// <summary>
        /// 取Pos商户信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseBusinessmanInfo GetBusinessmanInfo(string Id);
        /// <summary>
        /// 分配Pos机
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="posNoList"></param>
        [OperationContract]
        void AssignPos(string Id, string[] posNoList);
        /// <summary>
        /// 取Pos商户列表
        /// </summary>
        /// <param name="businessmanName"></param>
        /// <param name="posNo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseBusinessmanInfo> GetPosBusinessman(string businessmanName, string posNo, int startIndex, int count);
        /// <summary>
        /// 取Pos列表
        /// </summary>
        /// <param name="posNo"></param>
        /// <param name="businessmanName"></param>
        /// <param name="isAssign"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<PosInfoDataObject> GetPosList(string posNo, string businessmanName, bool? isAssign, int startIndex, int count);
        /// <summary>
        /// 取Pos分配日志
        /// </summary>
        /// <param name="posNo"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<PosAssignLogDataObject> GetPosAssignLogs(string posNo);
        /// <summary>
        /// 账户统计
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [OperationContract]
        AccountStatDataObject GetAccountStat();
        /// <summary>
        /// 收益统计
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<TradeStatDataObject> GainStat(DateTime startTime, DateTime endTime);
        /// <summary>
        /// 交易明细
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="posNo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<TradeDetailDataObject> GetTradeDetail(DateTime? startTime, DateTime? endTime, string posNo, int startIndex, int count);
        /// <summary>
        /// Pos商户报表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        BusinessmanReportDataObject GetBusinessmanReport(DateTime startTime, DateTime endTime);
        /// <summary>
        /// 回收Pos机
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="Id">商户ID</param>
        /// <param name="PosNo"></param>
        /// <param name="OperationUser"></param>
        [OperationContract]
        void RetrievePos(string Id, string PosNo);
    }
}
