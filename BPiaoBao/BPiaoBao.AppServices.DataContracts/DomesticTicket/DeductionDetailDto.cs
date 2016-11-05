using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class DeductionDetailDto
    {
        public int ID
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
        /// 点数
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

        public virtual List<DeductionDetailDto> DeductionDetails
        {
            get;
            set;
        }
    }
}
