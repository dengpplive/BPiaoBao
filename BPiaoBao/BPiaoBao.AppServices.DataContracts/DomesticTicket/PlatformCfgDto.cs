using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    
    public class PlatformCfgDto
    {
        public List<Platform> Platforms{ get; set; }

    }

    public class Platform
    {

        public string Code { get; set; }
        public string Name { get; set; }
         
        public int ShowCount { get; set; }
         
        public string IssueSpeed { get; set; }

        public bool IsClosed { get; set; }

        public bool PaidIsTest { get; set; }

        public AreaBigs Areas { get; set; }

        public string b2bClose { get; set; }

        public string bspClose { get; set; }
    }

    public class AreaBigs
    {
        public string DefaultCity { get; set; }
        public List<Area> Areas { get; set; }


    }

    public class Area
    { 
        public string City { get; set; }
        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {

        public string Name { get; set; }

        public string Value { get; set; }
        public string Description { get; set; }
    }
}
