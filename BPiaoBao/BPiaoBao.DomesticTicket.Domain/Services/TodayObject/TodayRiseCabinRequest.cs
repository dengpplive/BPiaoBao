using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Services.TodayObject
{    
    public class TodayRiseCabinRequest
    {
        /// <summary>
        /// 订单编号（接口）
        /// </summary>
        public string _OrderNo { get; set; }
        /// <summary>
        /// 需要升舱的乘宠信息,多名乘宠应以|进行分隔(张三|李四)
        /// </summary>
        public string _PsgName { get; set; }
        /// <summary>
        /// 需要升舱的 Pnr 编码
        /// </summary>
        public string _PNR { get; set; }
        /// <summary>
        /// 申请升舱的飞机的起飞日期
        /// </summary>
        public string _DepartDate { get; set; }
        /// <summary>
        /// 申请升舱的飞机的起飞时间
        /// </summary>
        public string _DepartTime { get; set; }
        /// <summary>
        /// 升舱的航班号
        /// </summary>
        public string _FlightNo { get; set; }
        /// <summary>
        /// 升舱的舱位
        /// </summary>
        public string _Cabin { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string _Remark { get; set; }
        /// <summary>
        /// 供应商 ID(加密
        /// </summary>
        public string _ProviderID { get; set; }
        /// <summary>
        /// 1:   前台     0：  后台
        /// </summary>
        public string _IsFront { get; set; }
        /// <summary>
        /// 服务商 ID
        /// </summary>
        public string _SystemID { get; set; }
    }
}
