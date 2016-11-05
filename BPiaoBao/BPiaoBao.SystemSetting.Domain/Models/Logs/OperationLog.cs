/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.SystemSetting.Domain.Models.Logs
* 文 件 名：OperationLog.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 14:18:45       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.SystemSetting.Domain.Models.Logs
{
    public class OperationLog : EntityBase, IAggregationRoot
    {
        public int Id { get; set; }
        public string FunctionName { get; set; }
        public string FunctionDescription { get; set; } 
        public string RequestParams { get; set; }
        public string ModuleFullName { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 操作员帐号
        /// </summary>
        public string OperatorAcount { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessCode { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessName { get; set; }

        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }
    }
}
