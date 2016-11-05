using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class ResponseRole
    {
        public int ID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    public class RequestRole
    {
        public int ID { get; set; }
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
    }
}
