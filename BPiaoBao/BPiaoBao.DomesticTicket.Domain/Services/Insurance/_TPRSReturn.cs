using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
     [XmlRoot("RETURN")]
    public  class _TPRSReturn
    {
         [XmlElement("ACKSTS")]
         public string Acksts { get; set; }

         [XmlElement("STSDESC")]
         public string StsDesc { get; set; }


         [XmlElement("BUSI")]
         public Busi Busi { get; set; }


    }

    [XmlRoot("BUSI")]
    public class Busi
    {
        /// <summary>
        /// 交易科目(1-投保2-撤保3-查询)
        /// </summary>
        [XmlElement("SUBJECT")]
        public string Subject { get; set; }

        /// <summary>
        /// 中介方流水号
        /// </summary>
        [XmlElement("TRANS")]
        public string Trans { get; set; }

        /// <summary>
        /// 返回代码(0000—成功)
        /// </summary>
        [XmlElement("REJECT_CODE")]
        public string RejectCode { get; set; }

        [XmlElement("REJECT_DESC")]
        public string RejectDesc { get; set; }

        [XmlElement("MAIN")]
        public Main Main { get; set; }
        
    }
    [XmlRoot("MAIN")]
    public class Main
    {

        /// <summary>
        ///投保单号
        /// </summary>
        [XmlElement("TBDH")]
        public string Tbdh { get; set; }

        /// <summary>
        /// 保单号
        /// </summary>
        [XmlElement("BDH")]
        public string Bdh { get; set; }

        /// <summary>
        /// 保单状态(0-无1-有效2-终止)
        /// </summary>
        [XmlElement("BDZT")]
        public string Bdzt { get; set; }


        [XmlElement("CHECK_TIME")]
        public string CheckTime { get; set; }
         
    }
}
