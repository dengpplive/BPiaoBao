using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD
{
    public class BusinessRule
    {


        public string Property { get; set; }
        public string Rule { get; set; }
        public BusinessRule(string property, string rule)
        {
            this.Property = property;
            this.Rule = rule;
        }
    }
}
