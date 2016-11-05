using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class ResponseBusinessmanInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 商户地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkMan { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string LinkTel { get; set; }
        /// <summary>
        /// 联系手机
        /// </summary>
        public string LinkPhone { get; set; }
        /// <summary>
        /// Pos总数
        /// </summary>
        public int TotalPosCount { get; set; }
        /// <summary>
        /// Pos费率
        /// </summary>
        public decimal PosRate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 银行卡Id
        /// </summary>
        public string BankId { get; set; }
        /// <summary>
        /// 持卡人
        /// </summary>
        public string Cardholder { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 支行
        /// </summary>
        public string Subbranch { get; set; }
    }
}
