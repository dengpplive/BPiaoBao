/***********************************************************************   
* Copyright(c) 
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.SystemSetting.Domain.Models.Logs
* 文 件 名：IOperationLogRepository.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:47:35       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.SystemSetting.Domain.Models.Logs
{
    public interface IOperationLogRepository:IRepository<OperationLog>
    {
    }
}
