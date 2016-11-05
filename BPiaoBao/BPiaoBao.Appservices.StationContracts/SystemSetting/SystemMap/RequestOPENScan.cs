using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class RequestOPENScan
    {
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
