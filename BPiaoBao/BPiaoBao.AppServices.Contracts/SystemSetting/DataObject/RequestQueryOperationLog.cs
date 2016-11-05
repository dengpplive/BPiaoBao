/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices.Contracts.SystemSetting.DataObject
* 文 件 名：RequestQueryOperationLog.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 18:06:35       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting.DataObject
{
    public class RequestQueryOperationLog
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Key { get; set; }
         
    }
}
