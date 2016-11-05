using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TravelGrantRecordDto
    {
        public int ID
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商户号
        /// </summary>
        public string BusinessmanCode
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商户名
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
        /// 分配行程单号段范围
        /// </summary>
        public string TripScope
        {
            set;
            get;
        }
        /// <summary>
        /// Office
        /// </summary>
        public string Office
        {
            set;
            get;
        }
        /// <summary>
        /// 分配行程单张数
        /// </summary>
        public int TripCount
        {
            set;
            get;
        }
        /// <summary>
        /// 分配行程单备注
        /// </summary>
        public string TripRemark
        {
            set;
            get;
        }
        /// <summary>
        /// 分配时间
        /// </summary>
        public DateTime GrantTime
        {
            get;
            set;
        }
    }
}
