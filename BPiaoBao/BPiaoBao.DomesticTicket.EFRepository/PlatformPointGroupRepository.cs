using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class PlatformPointGroupRepository : BaseRepository<PlatformPointGroup>, IPlatformPointGroupRepository
    {
        public PlatformPointGroupRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
    }
    public class PlatformPointGroupRuleRepository : BaseRepository<PlatformPointGroupRule>, IPlatformPointGroupRuleRepository
    {
        public PlatformPointGroupRuleRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
        //验证是否可以添加或者修改
        public bool ValidatePointGroupRule(PlatformPointGroupRule pointGroupRule)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select count(*) from PlatformPointGroupRule where ");
            sb.AppendFormat("AirCode='{0}' ", pointGroupRule.AirCode);
            sb.AppendFormat(" and StartDate<='{0}' and EndDate>='{0}'", pointGroupRule.StartDate);
            sb.AppendFormat(" and AdjustType={0}", (int)pointGroupRule.AdjustType);
            sb.AppendFormat(" and FromCityCodes like '%{0}%'", pointGroupRule.FromCityCodes.Replace(',', '%'));
            sb.AppendFormat(" and ToCityCodes like '%{0}%'", pointGroupRule.ToCityCodes.Replace(',', '%'));
            sb.AppendFormat(" and IssueTicketCode like '%{0}%'", pointGroupRule.IssueTicketCode.Replace(',', '%'));
            var count = this.DbContext.Database.SqlQuery<int>(sb.ToString()).FirstOrDefault();
            return count == 0;
        }
    }
}
