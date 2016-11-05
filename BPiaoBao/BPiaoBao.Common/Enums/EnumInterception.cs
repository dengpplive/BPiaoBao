/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.Common.Enums
* 文 件 名：EnumInterception.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/11 17:37:53       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 日志枚举拦截类型
    /// </summary>
    public enum EnumInterception
    {
        None,
        LogOperation,
        LogException,
    }
    /// <summary>
    /// 日志枚举忽略拦截类型
    /// </summary>
    public enum EnumIgnoreInterception
    {
        None,
        Password
    }
}
