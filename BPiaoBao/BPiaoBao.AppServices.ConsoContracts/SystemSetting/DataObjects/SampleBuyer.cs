using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class SampleBuyer
    {
        /// <summary>
        /// 商户Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 扣点组ID
        /// </summary>
        public int? DeductionGroupID { get; set; }

    }
    public class SampleListBuyer
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 扣点组ID
        /// </summary>
        public int? DeductionGroupID { get; set; }
        /// <summary>
        /// 扣点组名称
        /// </summary>
        public string DeductionGroupName { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 商户创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}
