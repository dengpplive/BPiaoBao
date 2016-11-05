using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 匹配到的一条扣点明细
    /// </summary>
    public class DeductionDetail
    {
        public DeductionDetail()
        {
            this.DeductionSource = DeductionSource.CarrierDeduction;
        }
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 被扣点商户code
        /// </summary>
        public string UnCode
        {
            get;
            set;
        }

        /// <summary>
        /// 被扣点商户名
        /// </summary>
        public string UnName
        {
            get;
            set;
        }


        /// <summary>
        /// 扣点商户code
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 扣点商户名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 扣留补的点数 其他为0
        /// </summary>
        public decimal Point
        {
            get;
            set;
        }
        /// <summary>
        /// 扣点类型
        /// </summary>
        public AdjustType AdjustType
        {
            get;
            set;
        }
        /// <summary>
        /// 本地 接口 共享
        /// </summary>
        public DeductionType DeductionType
        {
            get;
            set;
        }
        /// <summary>
        /// 扣点来源
        /// </summary>
        public DeductionSource DeductionSource
        {
            get;
            set;
        }
    }
}
