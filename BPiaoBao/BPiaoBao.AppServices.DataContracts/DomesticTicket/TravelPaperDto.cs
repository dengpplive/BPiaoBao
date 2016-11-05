using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TravelPaperDto
    {
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商商户号
        /// </summary>
        public string BusinessmanCode
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商商户名
        /// </summary>
        public string BusinessmanName
        {
            get;
            set;
        }
        /// <summary>
        /// 采购商户号
        /// </summary>
        public string UseBusinessmanCode
        {
            get;
            set;
        }
        /// <summary>
        /// 采购商户名
        /// </summary>
        public string UseBusinessmanName
        {
            get;
            set;
        }
        /// <summary>
        /// 乘客ID
        /// </summary>
        public int PassengerId
        {
            get;
            set;
        }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TripNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 行程单状态
        /// </summary>
        public EnumTripStatus TripStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 使用Office终端号
        /// </summary>
        public string UseOffice
        {
            set;
            get;
        }
        /// <summary>
        ///Office终端号对应的航协号
        /// </summary>
        public string IataCode
        {
            set;
            get;
        }
        /// <summary>
        /// Office终端号对应的填开单位(开票公司名称)
        /// </summary>
        public string TicketCompanyName
        {
            set;
            get;
        }
        /// <summary>
        /// 行程单分配时间
        /// </summary>
        public DateTime GrantTime
        {
            set;
            get;
        }
        /// <summary>
        /// 创建打印时间
        /// </summary>
        public DateTime PrintTime
        {
            set;
            get;
        }
        /// <summary>
        /// 作废时间
        /// </summary>
        public DateTime InvalidTime
        {
            set;
            get;
        }

        /// <summary>
        /// 空白回收时间
        /// </summary>
        public DateTime BlankRecoveryTime
        {
            set;
            get;
        }
        /// <summary>
        /// 使用行程单备注
        /// </summary>
        public string UseTripRemark
        {
            set;
            get;
        }
        /// <summary>
        /// 操作日志
        /// </summary>
        public  List<TravelPaperLogDto> TravelPaperLogDtos
        {
            get;
            set;
        }
    }
}
