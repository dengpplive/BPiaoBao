using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
    public class InsuranceOrder:EntityBase,IAggregationRoot
    {
        /// <summary>
        /// 记录id，整型自增，主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 机票订单
        /// </summary>
        public string OrderId { get; set; }

        ///// <summary>
        ///// 支付交易号，支付后返回的交易序列号
        ///// </summary>
        //public string TradeId { get; set; }

        ///// <summary>
        ///// 支付订单号
        ///// </summary>
        //public string PayNo { get; set; }

        /// <summary>
        /// 购买时间 
        /// </summary>
        public DateTime? BuyTime { get; set; }

        ///// <summary>
        ///// 保单状态
        ///// </summary>
        //public EnumInsuranceStatus EnumInsuranceStatus { get; set; }

        /// <summary>
        /// 保单记录
        /// </summary>
        public virtual IList<InsuranceRecord> InsuranceRecords { get; set; }

        public decimal PayMoney 
        { 
            get 
            {
                return this.InsuranceRecords.Sum(m => m.InsurancePrice);
            } 
        }

        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }

        public void CheckData()
        {
            if (InsuranceRecords == null || InsuranceRecords.Count == 0)
            {
                throw new CustomException(111112, "保险订单中并未包含任何一条保险记录。");
            }
            foreach (var record in InsuranceRecords)
            {
                record.CheckData();
            }
            
        }

        ///// <summary>
        ///// 获取保险记录支付用订单号
        ///// </summary>
        ///// <returns></returns>
        //public string GetPayNo()
        //{
        //    //每次生成随机数的时候都使用机密随机数生成器来生成种子，
        //    //这样即使在很短的时间内也可以保证生成的随机数不同
        //    var rand = new Random(GetRandomSeed());
        //    var exNum = rand.Next(1000, 9999);
        //    return string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), exNum);
        //}

        ///// <summary>
        ///// 得到随机数种子
        ///// </summary>
        ///// <returns></returns>
        //private static int GetRandomSeed()
        //{
        //    byte[] bytes = new byte[4];
        //    System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        //    rng.GetBytes(bytes);
        //    return BitConverter.ToInt32(bytes, 0);
        //}
    }
}
