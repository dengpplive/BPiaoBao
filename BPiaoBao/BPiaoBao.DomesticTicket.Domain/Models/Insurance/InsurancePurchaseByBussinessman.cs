using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
    public class InsurancePurchaseByBussinessman : EntityBase, IAggregationRoot
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string PayNo { get; set; }

        /// <summary>
        /// 外部交易号
        /// </summary>
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 充值前剩余
        /// </summary>
        public int BeforeLeaveCount { get; set; }


        /// <summary>
        /// 充值后剩余
        /// </summary>
        public int AfterLeaveCount { get; set; }

        /// <summary>
        /// 充值张数
        /// </summary>
        public int DepositCount { get; set; }


        /// <summary>
        /// 充值时间 
        /// </summary>
        public DateTime BuyTime { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal SinglePrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        ///商户Code
        /// </summary>
        public string BusinessmanCode { get; set; }

        /// <summary>
        ///商户名称
        /// </summary>
        public string BusinessmanName { get; set; }

        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 运营商名称
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod PayWay { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public EnumInsurancePayStatus BuyState { get; set; }

        /// <summary>
        /// 记录类别
        /// </summary>
        public EnumInsurancePurchaseType RecordType { get; set; }

        /// <summary>
        /// 操作员账号
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 操作员名称
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 支付手续费
        /// </summary>
        public decimal PayFee { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        protected override string GetIdentity()
        {

            return this.Id.ToString();
        }
        public string GetPayNo()
        {
            return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), new Random().Next(1000, 9999).ToString());
        }
    }
}
