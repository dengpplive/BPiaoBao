using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt.Model
{
    /// <summary>
    /// 常旅客列表
    /// </summary>
    public class PasserModel
    {
        /// <summary>
        /// 是否新建
        /// </summary>
        public bool isCusCreate { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 旅客姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 旅客类型
        /// </summary>
        public string PasserType { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CertificateNo { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 航空公司卡号
        /// </summary>
        public string AirCardNo { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public EnumSexType SexType { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birth { get; set; }
    }
}
