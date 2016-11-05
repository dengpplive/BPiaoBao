using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
   public class ResponseBusinessMan
    {
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWayDto ContactWay { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
    }
}
