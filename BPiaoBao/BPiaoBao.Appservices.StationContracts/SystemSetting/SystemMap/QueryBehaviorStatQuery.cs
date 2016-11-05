using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class QueryBehaviorStatQuery
    {
        public string CarrierCode { get; set; }

        public string BusinessmanCode { get; set; }

        public string BusinessType { get; set; }

        public string ContactName { get; set; }

        public string OperatorName { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
        public string Sort { get; set; }

        public string Order { get; set; }
    }
}
