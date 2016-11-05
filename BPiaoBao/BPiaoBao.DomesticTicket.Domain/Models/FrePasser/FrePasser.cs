using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.Domain.Models.FrePasser
{
    /// <summary>
    /// 常旅客信息
    /// </summary>
    public class FrePasser : EntityBase, IAggregationRoot
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 旅客姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 旅客类型
        /// </summary>
        public  string PasserType { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CertificateNo { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public string SexType { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 航空公司卡号
        /// </summary>
        public string AirCardNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 采购商Code
        /// </summary>
        public string BusinessmanCode { get; set; }

        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }
    }
}
