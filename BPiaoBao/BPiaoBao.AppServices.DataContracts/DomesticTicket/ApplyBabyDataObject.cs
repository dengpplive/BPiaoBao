using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 线下婴儿申请
    /// </summary>
    public class ApplyBabyDataObject
    {
        /// <summary>
        /// 关联订单号
        /// </summary>
        public string RelationOrderId { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 申请婴儿列表
        /// </summary>
        public List<BabyDataObject> BabyList { get; set; }

    }
    public class BabyDataObject
    {
        /// <summary>
        /// 婴儿姓名
        /// </summary>
        public string BabyName { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime BornDate { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public EnumSexType SexType { get; set; }
    }

}
