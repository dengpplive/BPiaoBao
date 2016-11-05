/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices.SystemSetting
* 文 件 名：OperationLogService.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/12 10:01:17       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.SystemSetting.Domain.Models.Logs;
using BPiaoBao.SystemSetting.Domain.Services.Logs;
using StructureMap;

namespace BPiaoBao.AppServices.SystemSetting
{
    public class OperationLogService : IOperationLogService
    {
        private readonly OperationLogDomainService _operationLogDomainService;
        public OperationLogService()
        {
            _operationLogDomainService = ObjectFactory.GetInstance<OperationLogDomainService>();
        }
        public DataPack<OperationLogDto> Query(RequestQueryOperationLog request, int currentPageIndex, int pageSize)
        {
            var data=new DataPack<OperationLogDto>();
            var query = _operationLogDomainService.Query(request.StartTime, request.EndTime, request.Key);
            var total = query.Count();
            var list = query.OrderByDescending(p => p.CreateTime).Skip((currentPageIndex - 1)*pageSize).Take(pageSize).ToList();
            data.List= Mapper.Map<List<OperationLog>, List<OperationLogDto>>(list);
            data.TotalCount = total;
            return data;
        }

        public void Add(OperationLogDto log)
        {
            var m = Mapper.Map<OperationLogDto, OperationLog>(log);
            _operationLogDomainService.AddLog(m);
        }

        public void AddBatch(List<OperationLogDto> logs)
        {
            var list= Mapper.Map<List<OperationLogDto>, List<OperationLog>>(logs);
            _operationLogDomainService.AddBatch(list);
        }
    }

    
}
