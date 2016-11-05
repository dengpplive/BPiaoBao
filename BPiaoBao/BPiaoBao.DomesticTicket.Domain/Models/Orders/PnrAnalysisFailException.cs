using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class PnrAnalysisFailException : CustomException
    {
        public PnrAnalysisFailException(string message) : base(600, message) { }
    }
}
