using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    public class Role : EntityBase, IAggregationRoot
    {
        public int ID { get; set; }
        /// <summary>
        /// 商户Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 权限点
        /// </summary>
        public string AuthNodes { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 分配员工
        /// </summary>
        public IList<Operator> Operators { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
}
