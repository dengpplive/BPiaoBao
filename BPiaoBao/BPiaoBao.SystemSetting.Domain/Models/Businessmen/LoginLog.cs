using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    /// <summary>
    /// 登录日志
    /// </summary>
    public class LoginLog : EntityBase, IAggregationRoot
    {
        public int ID { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 登录帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public string LoginIP { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginDate { get; set; }
        
        protected override string GetIdentity()
        {
            return this.ID.ToString();
        }
    }
}
