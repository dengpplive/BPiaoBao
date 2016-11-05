using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class RequestNotice
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 公告类型
        /// </summary>
        public string NoticeType { get; set; }
        /// <summary>
        /// 可见类型
        /// </summary>
        public string NoticeShowType { get; set; }
        /// <summary>
        /// 有效期(起始日期)
        /// </summary>
        public DateTime EffectiveStartTime { get; set; }
        /// <summary>
        /// 有效期(结束日期)
        /// </summary>
        public DateTime EffectiveEndTime { get; set; }
        /// <summary>
        /// 公告附件
        /// </summary>
        public List<NoticeAttachmentDto> NoticeAttachments { get; set; }
    }
}
