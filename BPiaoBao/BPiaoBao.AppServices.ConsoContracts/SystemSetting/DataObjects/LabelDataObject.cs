using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class LabelDataObject
    {
        /// <summary>
        /// 运营上标签
        /// </summary>
        public string CarrierLabel { get; set; }
        /// <summary>
        /// 采购商标签
        /// </summary>
        public string BuyerLabel { get; set; }
    }
    public class SetLabel
    {
        /// <summary>
        /// 采购商Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 采购商标签
        /// </summary>
        public string BuyerLabel { get; set; }
    }
}
