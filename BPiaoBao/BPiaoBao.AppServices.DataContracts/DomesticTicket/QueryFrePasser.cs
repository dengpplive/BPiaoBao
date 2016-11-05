using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 常旅客查询条件
    /// </summary>
   public class QueryFrePasser
    {
        /// <summary>
        /// 旅客姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 旅客类型
        /// </summary>
        public string PasserType { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CertificateNo { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 航空公司卡号
        /// </summary>
        public string AirCardNo { get; set; }
        
    }
}
