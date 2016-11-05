using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class ResponseOPENScan
    {
        /// <summary>
        /// 操作账号
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// OPEN票数量
        /// </summary>
        public int OPENCount { get; set; }
        /// <summary>
        /// 扫描数据
        /// </summary>
        public int ScanCount { get; set; }
        /// <summary>
        /// 扫描状态
        /// </summary>
        public EnumOPEN State { get; set; }
        /// <summary>
        /// office号
        /// </summary>
        public string OfficeNum { get; set; }
        /// <summary>
        /// 扫描IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 扫描端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 模板名
        /// </summary>
        public string TemplateName { get; set; }
        /// <summary>
        /// 模板路径
        /// </summary>
        public string TemplateUrl { get; set; }

    }
}
