using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.Domain.Models.TravelPaper
{
    /// <summary>
    /// 行程单发放记录
    /// </summary>
    public class TravelGrantRecord : EntityBase, IAggregationRoot
    {
        public TravelGrantRecord()
        {
            this.GrantTime = DateTime.Parse("1900-01-01");
        }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
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
