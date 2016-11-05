using BPiaoBaoTPos.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBaoTPos.Domain.Services
{
    public interface IBusinessmanClientProxy
    {
        /// <summary>
        /// 取Pos商户列表
        /// </summary>
        /// <param name="businessmanName"></param>
        /// <param name="posNo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BusinessmanInfo>, int> GetPosBusinessman(string code, string key,string businessmanName, string posNo, int startIndex, int count);
        /// <summary>
        /// 添加Pos商户
        /// </summary>
        /// <param name="businessmanInfo"></param>
        void AddBusinessman(string code, string key, string OperationUser, BusinessmanInfo businessmanInfo);
        /// <summary>
        /// 删除商户
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="Id"></param>
        void DeleteBusinessman(string code, string key, string OperationUser, string Id); 
        /// <summary>
        /// 修改Pos商户
        /// </summary>
        /// <param name="businessmanInfo"></param>
        void UpdateBusinessman(string code, string key,string OperationUser, BusinessmanInfo businessmanInfo);
        /// <summary>
        /// 取Pos商户信息
        /// </summary>
        /// <param name="Id">公司ID</param>
        /// <returns></returns>
        BusinessmanInfo GetBusinessmanInfo(string code, string key, string Id);
        /// <summary>
        /// 分配Pos机
        /// </summary>
        /// <param name="Id">商户ID</param>
        /// <param name="posNoList"></param>
        void AssignPos(string code, string key,string OperationUser, string Id, string[] posNoList);
        /// <summary>
        /// 回收Pos机
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="Id">商户ID</param>
        /// <param name="PosNo"></param>
        /// <param name="OperationUser"></param>
        void RetrievePos(string code, string key, string Id, string PosNo, string OperationUser);
    }
}
