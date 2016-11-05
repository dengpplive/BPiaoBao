using System.ComponentModel;

namespace BPiaoBao.Common.Enums
{
    public enum EnumTicketStatus
    {
        //出票，退票，改签，
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        Unknown = -1,
        /// <summary>
        /// 出票
        /// </summary>
        [Description("出票")]
        IssueTicket = 0,
        /// <summary>
        /// 退票
        /// </summary>
        [Description("退票中")]
        RefoundTicket = 1,
        [Description("已退票")]
        RefoundTicketed = 2,
        [Description("退票")]
        Refound = 7,
        /// <summary>
        /// 改签
        /// </summary>
        [Description("改签中")]
        ChangeTicket = 3,
        [Description("改签")]
        Change = 8,
        /// <summary>
        /// 废票
        /// </summary>
        [Description("废票中")]
        AnnulTicket = 4,
        [Description("已废票")]
        AnnulTicketed = 5,
        [Description("废票")]
        Annul = 9,
        /// <summary>
        /// 修改s
        /// </summary>
        [Description("其他信息修改")]
        ModifyTicket = 6


    }
}
