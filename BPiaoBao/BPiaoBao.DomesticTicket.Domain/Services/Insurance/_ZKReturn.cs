using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    [XmlRoot("Return")]
    public class _ZKReturn
    {
        [XmlElement("Table")]
        public Table Table { get; set; }

        [XmlElement("MESSAGE")]
        public Message Message { get; set; }
    }


    [XmlRoot("Table")]
    public class Table
    {
        [XmlElement("PolicyNo")]
        public string PolicyNo { get; set; }

        [XmlElement("LocalPolicyNo")]
        public string LocalPolicyNo { get; set; }
    }

    [XmlRoot("MESSAGE")]
    public class Message
    {
        [XmlElement("VALUE")]
        public string Value { get; set; }

        [XmlElement("TIME")]
        public string Time { get; set; }
    }
}
