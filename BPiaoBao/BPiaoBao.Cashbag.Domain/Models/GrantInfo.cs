using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    public class GrantInfo
    {
        /// <summary>
        /// 认证信息数组
        /// </summary>
        public List<GrantArray> GrantArray { get; set; }
        /// <summary>
        /// 状态(小于0表示审核未通过,0-1表示审核中,大于1表示审核通过)
        /// </summary>
        public int Applystatus { get; set; }
        /// <summary>
        /// 审核消息
        /// </summary>
        public string message { get; set; }
    }
}
