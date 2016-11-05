using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class RareResponse
    {
        /// <summary>
        /// 是否包含生僻字 true包含 false不包含 
        /// </summary>
        public bool IsRare { get; set; }

        /// <summary>
        /// IsRare=true时 包含的哪些生僻字
        /// </summary>
        public string RareFontString { get; set; }
    }
}
