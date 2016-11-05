/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices.Contracts.SystemSetting.DataObject
* 文 件 名：ResponseOperationLog.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 18:04:44       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting.DataObject
{
    public class OperationLogDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 函数名称
        /// </summary>
        public string FunctionName { get; set; }
        /// <summary>
        /// 函数描述
        /// </summary>
        public string FunctionDescription { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestParams { get; set; }
        /// <summary>
        /// 模块全名称
        /// </summary>
        public string ModuleFullName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
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

    }
}
