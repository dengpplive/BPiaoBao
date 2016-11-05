using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class SetBuyerToStationBuyerGroupRequest
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public string GroupID { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public IList<string> BuyerCode { get; set; }
    }
}
