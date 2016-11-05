using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.StationMap
{
    public class AirLine
    {
        public int? Id { get; set; }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 航空公司名称
        /// </summary>
        public string CarrayName { get; set; }
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string CarrayAbbreviation { get; set; }
        /// <summary>
        /// BSP状态:True:开启;False:关闭
        /// </summary>
        public bool? BSPStatus { get; set; }
        /// <summary>
        /// B2B状态:True:开启;False:关闭
        /// </summary>
        public bool? B2BStatus { get; set; }
        /// <summary>
        /// 航空公司类型
        /// </summary>
        public string AriLineType { get; set; }
        /// <summary>
        /// 航空公司状态:True:开启;False:关闭
        /// </summary>
        public bool? AriLineStatus { get; set; }
    }
}
