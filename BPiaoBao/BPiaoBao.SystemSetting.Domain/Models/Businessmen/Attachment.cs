using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    public class Attachment : EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        protected override string GetIdentity()
        {
            return Id.ToString();
        }
    }
}
