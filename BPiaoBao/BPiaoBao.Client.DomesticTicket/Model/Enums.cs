using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    public enum AgeType
    {
        [Description("全部")]
        All = 0,
        [Description("成人")]//>12岁
        Adult = 1,
        [Description("儿童")]//2-12岁
        Child = 2,
        [Description("婴儿")]//<2岁
        Baby = 3
    }
    public enum IDType
    {
        [Description("全部")]
        All,
        [Description("身份证")]
        NormalId,
        [Description("护照")]
        Passport,
        [Description("军官证")]
        MilitaryID,
        [Description("出生日期")]
        BirthDate,
        [Description("其它有效证件")]
        Other
    }
}
