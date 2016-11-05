using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class ResponseBehaviorStat
    {
        public string BusinessmanCode { get; set; }

        public string BusinessmanName { get; set; }

        public string BusinessmanType { get; set; }

        public string CarrierCode { get; set; }

        public DateTime OpDateTime { get; set; }
        public string ContactName { get; set; }
        public string OperatorName { get; set; }
        public int LoginCount { get; set; }

        public int QueryCount { get; set; }

        public int ImportCount { get; set; }

        public int OutTicketCount { get; set; }

        public int BackTicketCount { get; set; }

        public int AbolishTicketCount { get; set; }

        public int AccessCount { get; set; }

        public int FinancingCount { get; set; }

        public int UseCount { get; set; }
    }
}
