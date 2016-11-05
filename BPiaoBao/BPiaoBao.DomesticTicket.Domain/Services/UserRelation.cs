using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;

namespace BPiaoBao.DomesticTicket.Domain.Services
{
    //用户关系
    public class UserRelation
    {
        public Buyer buyer { get; set; }
        public Carrier carrier { get; set; }
        public DeductionGroup deductionGroup { get; set; }
        public List<Carrier> CarrierList = new List<Carrier>();
        public List<Supplier> SupplierList = new List<Supplier>();
    }
}
