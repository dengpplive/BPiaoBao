using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketSuppendRequest
    {
        public string TicketNumber
        {
            get;
            set;
        }
        public string Office
        {
            get;
            set;
        }
        public string BusinessmanCode
        {
            get;
            set;
        }
        public TicketNumberOpType TicketNumberOpType
        {
            get;
            set;
        }
    }

    public class TicketSuppendResponse
    {
        public bool Result
        {
            get;
            set;
        }
        private string _Remark = string.Empty;
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
    }
}
