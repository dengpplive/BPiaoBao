using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.SMS
{
    public class GiveDetail : ValueObjectBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 赠予人商户号
        /// </summary>
        public string GiveCode { get; set; }
        /// <summary>
        /// 赠予人
        /// </summary>
        public string GiveName { get; set; }
        /// <summary>
        /// 赠予条数
        /// </summary>
        public int GiveCount { get; set; }
        /// <summary>
        /// 赠送时间
        /// </summary>
        public DateTime GiveTime { get; set; }
        /// <summary>
        /// 赠送原因
        /// </summary>
        public string Remark { get; set; }
    }
}
