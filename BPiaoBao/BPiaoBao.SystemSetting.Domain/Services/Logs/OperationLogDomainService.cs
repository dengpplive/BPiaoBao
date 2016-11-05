/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.SystemSetting.Domain.Services.Logs
* 文 件 名：OperationLogDomainService.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:50:27       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using BPiaoBao.SystemSetting.Domain.Models.Logs;
using JoveZhao.Framework.DDD;
using StructureMap;

namespace BPiaoBao.SystemSetting.Domain.Services.Logs
{
    public class OperationLogDomainService : BaseDomainService
    {

        private IOperationLogRepository m_operationLogRepository;
        public OperationLogDomainService(IOperationLogRepository operationLogRepository)
        {
            this.m_operationLogRepository = operationLogRepository;
        }

        public void AddLog(OperationLog log)
        {
            this._unitOfWorkRepository.PersistCreationOf(log);
            _unitOfWork.Commit();
        }

        public void AddBatch(List<OperationLog> logs)
        {
            foreach (var log in logs)
            {
                this._unitOfWorkRepository.PersistCreationOf(log);
            }

            _unitOfWork.Commit();
        }

        public IQueryable<OperationLog> Query(DateTime? starTime, DateTime? endTime, string key)
        {
            var query = this.m_operationLogRepository.FindAll();
            if (starTime.HasValue)
            {
                query = query.Where(p => p.CreateTime >= starTime.Value.Date);
            }
            if (endTime.HasValue)
            {
                var time = endTime.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.CreateTime <= time);
            }
            if (!string.IsNullOrEmpty(key))
            {
                query =
                    query.Where(
                        p =>
                            p.FunctionDescription.Contains(key) || p.FunctionName.Contains(key) ||
                            p.ModuleFullName.Contains(key) || p.OperatorAcount.Contains(key) || p.BusinessCode.Contains(key) || p.BusinessName.Contains(key));
            } 
            return query;
        }
    }
}
