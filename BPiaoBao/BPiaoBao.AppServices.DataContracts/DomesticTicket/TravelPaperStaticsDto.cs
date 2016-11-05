using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 行程单管理的统计数据
    /// </summary>
    public class TravelPaperStaticsDto
    {
        public TravelPaperStaticsDto()
        {
            this.ItemStaticsList = new List<TravelPaperItem>();
            this.Total = new TravelPaperItem();
        }

        public List<TravelPaperItem> ItemStaticsList
        {
            get;
            set;
        }

        public TravelPaperItem Total
        {
            get;
            set;
        }
    }


    public class TravelPaperItem
    {
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
        /// 发放行程单总数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }
        /// <summary>
        /// 已使用数目
        /// </summary>
        public int TotalUse
        {
            get;
            set;
        }
        /// <summary>
        /// 未使用数目
        /// </summary>
        public int TotalNoUse
        {
            get;
            set;
        }
        /// <summary>
        /// 空白回收数目
        /// </summary>
        public int TotalBlankRecovery
        {
            get;
            set;
        }
        /// <summary>
        /// 已作废数目
        /// </summary>
        public int TotalVoid
        {
            get;
            set;
        }
        /// <summary>
        /// 可使用
        /// </summary>
        public int TotalValidateUse
        {
            get;
            set;
        }
    }

}
