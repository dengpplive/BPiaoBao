using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class PIDInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Office号
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 运营Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 供应Code
        /// </summary>
        public string SupplierCode { get; set; }
    }
}
