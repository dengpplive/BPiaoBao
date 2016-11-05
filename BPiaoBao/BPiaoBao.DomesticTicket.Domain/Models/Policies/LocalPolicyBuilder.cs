using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Policies
{
    public class LocalPolicyBuilder : IAggregationBuilder
    {
        public LocalNormalPolicy CreateLocalNormalPolicy(LocalNormalPolicy localNormalPolicy=null)
        {
            if (localNormalPolicy == null)
                localNormalPolicy = new LocalNormalPolicy();
            if (localNormalPolicy.PassengeDate == null)
                localNormalPolicy.PassengeDate = new DateLimit();
            if (localNormalPolicy.IssueDate == null)
                localNormalPolicy.IssueDate = new DateLimit();
            if (localNormalPolicy.Carrier_AnnulTicketTime == null)
                localNormalPolicy.Carrier_AnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.Carrier_ReturnTicketTime == null)
                localNormalPolicy.Carrier_ReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.Carrier_WeekAnnulTicketTime == null)
                localNormalPolicy.Carrier_WeekAnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.Carrier_WeekReturnTicketTime == null)
                localNormalPolicy.Carrier_WeekReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.Carrier_WeekWorkTime == null)
                localNormalPolicy.Carrier_WeekWorkTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.Carrier_WorkTime == null)
                localNormalPolicy.Carrier_WorkTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.AnnulTicketTime == null)
                localNormalPolicy.AnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.ReturnTicketTime == null)
                localNormalPolicy.ReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.WorkTime == null)
                localNormalPolicy.WorkTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.WeekAnnulTicketTime == null)
                localNormalPolicy.WeekAnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.WeekReturnTicketTime == null)
                localNormalPolicy.WeekReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (localNormalPolicy.WeeKWorkTime == null)
                localNormalPolicy.WeeKWorkTime = new Platform.Plugin.StartAndEndTime();
            localNormalPolicy.CreateDate = DateTime.Now;
            return localNormalPolicy;
        }
        public SpecialPolicy CreateSpecialPolicy(SpecialPolicy specialPolicy = null)
        {
            if (specialPolicy == null)
                specialPolicy = new SpecialPolicy();
            if (specialPolicy.PassengeDate == null)
                specialPolicy.PassengeDate = new DateLimit();
            if (specialPolicy.IssueDate == null)
                specialPolicy.IssueDate = new DateLimit();
            if (specialPolicy.Carrier_AnnulTicketTime == null)
                specialPolicy.Carrier_AnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.Carrier_ReturnTicketTime == null)
                specialPolicy.Carrier_ReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.Carrier_WeekAnnulTicketTime == null)
                specialPolicy.Carrier_WeekAnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.Carrier_WeekReturnTicketTime == null)
                specialPolicy.Carrier_WeekReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.Carrier_WeekWorkTime == null)
                specialPolicy.Carrier_WeekWorkTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.Carrier_WorkTime == null)
                specialPolicy.Carrier_WorkTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.AnnulTicketTime == null)
                specialPolicy.AnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.ReturnTicketTime == null)
                specialPolicy.ReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.WorkTime == null)
                specialPolicy.WorkTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.WeekAnnulTicketTime == null)
                specialPolicy.WeekAnnulTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.WeekReturnTicketTime == null)
                specialPolicy.WeekReturnTicketTime = new Platform.Plugin.StartAndEndTime();
            if (specialPolicy.WeeKWorkTime == null)
                specialPolicy.WeeKWorkTime = new Platform.Plugin.StartAndEndTime();
            specialPolicy.CreateDate = DateTime.Now;
            return specialPolicy;
        }
       
    }
}
