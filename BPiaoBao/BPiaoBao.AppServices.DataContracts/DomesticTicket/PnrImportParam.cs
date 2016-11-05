using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// PNR导入参数信息
    /// </summary>
    public class PnrImportParam
    {
        public PnrImportParam()
        {
            this.PnrImportType = EnumPnrImportType.PnrContentImport;
        }
        /// <summary>
        /// 导入Pnr或者Pnr内容
        /// </summary>
        public string PnrAndPnrContent { get; set; }
        /// <summary>
        /// 关联订单号 （成人订单号或者升舱关联的原订单号）
        /// </summary>
        public string OldOrderId { get; set; }

        /// <summary>
        /// 是否换编码出票
        /// </summary>
        public bool IsChangePnrTicket { get; set; }
        /// <summary>
        /// 多个价格(高低价格) true低价格(默认) false高价格
        /// </summary>
        public bool IsLowPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 导入方式 
        /// </summary>
        public EnumPnrImportType PnrImportType { get; set; }

    }
}
