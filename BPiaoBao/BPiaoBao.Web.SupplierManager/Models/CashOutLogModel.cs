using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPiaoBao.AppServices.DataContracts.Cashbag;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class CashOutLogModel : CashOutLogDto
    {
        public string BankCardNoShowStar { get; set; }
    }
}