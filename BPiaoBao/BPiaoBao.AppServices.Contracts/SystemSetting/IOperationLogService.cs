/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices.Contracts.SystemSetting
* 文 件 名：IOperationLogService.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 18:04:09       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.DataContracts;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface IOperationLogService
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<OperationLogDto> Query(RequestQueryOperationLog request, int currentPageIndex, int pageSize);
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="log"></param> 
        void Add(OperationLogDto log);
        /// <summary>
        /// 批量添加日志 
        /// </summary>
        /// <param name="logs"></param> 
        void AddBatch(List<OperationLogDto> logs);
    }
}
