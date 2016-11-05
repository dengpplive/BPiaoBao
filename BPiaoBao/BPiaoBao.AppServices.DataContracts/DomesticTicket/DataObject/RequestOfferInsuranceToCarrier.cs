using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class RequestOfferInsuranceToCarrier
    {
        public string CarrierCode { get; set; }

        public int OfferCount { get; set; }

        public string Remark { get; set; }
    }
}
