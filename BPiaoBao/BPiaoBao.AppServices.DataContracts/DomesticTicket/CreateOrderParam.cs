using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{

    /// <summary>
    /// 生成订单需要的参数
    /// </summary>
    public class CreateOrderParam
    {
        public CreateOrderParam()
        {
            this.SkyWayDtos = new List<SkyWayDto>();
            this.PassengerDtos = new List<PassengerDto>();
        }
        /// <summary>
        /// 预定的乘客信息
        /// </summary>
        public List<PassengerDto> PassengerDtos { get; set; }
        /// <summary>
        /// 预定的航班信息
        /// </summary>
        public List<SkyWayDto> SkyWayDtos { get; set; }

        public PnrData pnrData { get; set; }

    }
}
