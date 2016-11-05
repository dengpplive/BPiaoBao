using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 用户流量信息
    /// </summary>
    public class UserFlow
    {
        /// <summary>
        /// 总流量计数
        /// </summary>
        public int TotalFlow
        {
            get;
            set;
        }
        /// <summary>
        /// 每个配置或者Office的使用流量计数
        /// </summary>
        public List<FlowData> FlowList = new List<FlowData>();
    }

    public class FlowData
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public string ConfigName { get; set; }
        /// <summary>
        /// Office
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 每天的流量
        /// </summary>
        public int SendCount { get; set; }
        /// <summary>
        /// 发送指令的用户名    分销商户号#配置提供商户号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 指令来源IP
        /// </summary>
        public string ClientIP { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public string UseDate { get; set; }
        /// <summary>
        /// 指令来源 web client
        /// </summary>
        public string Source { get; set; }
    }
}
