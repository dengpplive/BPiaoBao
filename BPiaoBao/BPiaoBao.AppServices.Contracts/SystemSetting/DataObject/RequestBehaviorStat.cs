using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.SystemSetting
{
    public class RequestBehaviorStat
    {
        public int Id { get; set; }

        public string BusinessmanCode { get; set; }

        public string BusinessmanName { get; set; }

        public string BusinessmanType { get; set; }

        public EnumBehaviorOperate BehaviorOperate { get; set; }
         
        public DateTime OpDateTime { get; set; }

    }

    public class RequestQueryBehaviorStatQuery
    {
        public string BusinessmanCode { get; set; }

        public string BusinessmanName { get; set; }

        public string BusinessmanType { get; set; }

        public string ContactName { get; set; }

        public string OperatorName { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public int PageIndex { get; set; } 

        public int? PageSize { get; set; }

        public string Sort { get; set; }

        public string Order { get; set; }


    }

    public enum EnumBehaviorOperate
    {
        [Description("登录次数")]
        LoginCount = 1,

        [Description("查询次数")]
        QueryCount,

        [Description("导入次数")]
        ImportCount,

        [Description("出票量")]
        OutTicketCount,

        [Description("退票量")]
        BackTicketCount,

        [Description("废票量")]
        AbolishTicketCount,

        [Description("访问次数")]
        AccessCount,

        [Description("理财笔数")]
        FinancingCount,

        [Description("使用次数")]
        UseCount
    }

}
