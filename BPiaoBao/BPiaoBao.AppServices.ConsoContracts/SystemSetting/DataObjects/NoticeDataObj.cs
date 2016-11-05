using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class NoticeDataObj
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 商户Code
        /// </summary>
        public string Code { get; set; }
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
        /// 发布人
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { get; set; }
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
        public List<NoticeAttachmentDataDto> NoticeAttachments { get; set; }
    }
}
