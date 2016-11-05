/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices.DataContracts.Cashbag
* 文 件 名：QuickPayDto.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/27 10:40:36       
* 备注描述：           
************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    public class QuickPayDto
    {
        public string AlipayAccount { get; set; }

        public string TenpayAccount { get; set; }
    }
}
