using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class ResponeAirChange
    {
        public int Id { get; set; }
        /// <summary>
        /// QT时间
        /// </summary>
        public DateTime QTDate { get; set; }
        /// <summary>
        /// QT条数
        /// </summary>
        public int QTCount { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// PNR编码
        /// </summary>
        public string PNR { get; set; }
        /// <summary>
        /// 是否是系统PNR TRUE：是
        /// </summary>
        public bool CanPNR { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string CTCT { get; set; }
        /// <summary>
        /// 运营商姓名
        /// </summary>
        public string CarrayName { get; set; }
        /// <summary>
        /// 乘机人多个以|分隔
        /// </summary 
        public string PassengerName { get; set; }
        public string OfficeNum { get; set; }
        public List<AirChangeCoordionDto> AriChangeCoordion { get; set; }
        /// <summary>
        /// 通知方式
        /// </summary>
        public EnumAriChangNotifications NotifyWay { get; set; }
        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }
    }

}
