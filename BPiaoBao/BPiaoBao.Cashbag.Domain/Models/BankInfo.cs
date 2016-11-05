using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 银行信息列表
    /// </summary>
    public class BankInfo
    {

        /// <summary>
        /// 银行Code
        /// </summary>
        public string Code { get; set; }


        /// <summary>
        /// 银行名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行图标远程地址
        /// </summary>
        public string ImageUri { get; set; }
    }
}
