/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.SystemSetting.EFRepository.LogMap
* 文 件 名：OperationLogMap.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:45:41       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using BPiaoBao.SystemSetting.Domain.Models.Logs;

namespace BPiaoBao.SystemSetting.EFRepository.LogMap
{
    public class OperationLogMap : EntityTypeConfiguration<OperationLog>
    {
        public OperationLogMap()
        {
            this.HasKey(t => t.Id);
        }
    }
}
