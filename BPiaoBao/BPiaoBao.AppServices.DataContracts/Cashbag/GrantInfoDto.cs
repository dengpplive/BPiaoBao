using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    public class GrantInfoDto
    {
        /// <summary>
        /// 认证信息数组
        /// </summary>
        public List<GrantArrayDto> GrantArray { get; set; }
        /// <summary>
        /// 状态(-2表示暂无申请认证信息,-1表示审核未通过,0-1表示审核中,大于1表示审核通过)
        /// </summary>
        public int Applystatus { get; set; }
        /// <summary>
        /// 审核消息
        /// </summary>
        public string message { get; set; }
    }
}
