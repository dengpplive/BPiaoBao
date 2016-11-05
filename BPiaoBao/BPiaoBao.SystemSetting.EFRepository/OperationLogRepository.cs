/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.SystemSetting.EFRepository
* 文 件 名：OperationLogRepository.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:48:03       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.SystemSetting.Domain.Models.Logs;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;

namespace BPiaoBao.SystemSetting.EFRepository
{
    public class OperationLogRepository : BaseRepository<OperationLog>, IOperationLogRepository
    {
        public OperationLogRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {
        }
    }
}
