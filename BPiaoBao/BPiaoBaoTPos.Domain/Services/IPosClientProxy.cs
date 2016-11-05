using BPiaoBaoTPos.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBaoTPos.Domain.Services
{
    
    public interface IPosClientProxy
    {
        /// <summary>
        /// 取Pos列表
        /// </summary>
        /// <param name="posNo"></param>
        /// <param name="businessmanName"></param>
        /// <param name="isAssign"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
         Tuple<IEnumerable<PosInfo>, int> GetPosList(string code, string key, string posNo, string businessmanName, bool? isAssign, int startIndex, int count);
        /// <summary>
        /// 取Pos分配日志
        /// </summary>
        /// <param name="posNo"></param>
        /// <returns></returns>
         IEnumerable<PosAssignLog> GetPosAssignLogs(string code, string key, string posNo);
    }
}
