using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.SystemSetting
{
    public class SendDetailDto
    {
        public int ID { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 接收号码
        /// </summary>
        public string ReceiveNum { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string ReceiveName { get; set; }
        /// <summary>
        /// 发送状态
        /// </summary>
        public bool SendState { get; set; }
        /// <summary>
        /// 发送条数
        /// </summary>
        public int SendCount { get; set; }
    }
}
