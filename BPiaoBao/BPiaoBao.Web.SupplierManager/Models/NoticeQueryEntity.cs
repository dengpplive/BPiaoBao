using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class NoticeQueryEntity
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public bool? State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
    }
}