using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class BehaviorStatModel
    {
        public string BusinessmanCode { get; set; }

        public string BusinessmanName { get; set; }

        public string BusinessmanType { get; set; }

        public string ContactName { get; set; } 

        public string OperatorName { get; set; }

        public string StartDateTime { get; set; }

        public string EndDateTime { get; set; }

        public int page { get; set; }

        public int rows { get; set; }

        public string sort { get; set; }

        public string order { get; set; }
    }
}