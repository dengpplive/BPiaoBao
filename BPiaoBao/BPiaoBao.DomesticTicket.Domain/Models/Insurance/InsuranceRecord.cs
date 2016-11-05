using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
    public class InsuranceRecord : EntityBase
    {
        /// <summary>
        /// 主键，自增
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 保险单号
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// 保险单价
        /// </summary>
        public decimal InsurancePrice { get; set; }

        /// <summary>
        /// 保险生效开始时间
        /// </summary>
        public DateTime? InsuranceLimitStartTime { get; set; }

        /// <summary>
        /// 保险生效结束时间
        /// </summary>
        public DateTime? InsuranceLimitEndTime { get; set; }

        /// <summary>
        /// 保险公司
        /// </summary>
        public string InsuranceCompany { get; set; }

        /// <summary>
        /// 保单状态
        /// </summary>
        public EnumInsuranceStatus InsuranceStatus { get; set; }

        /// <summary>
        /// 保险数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 运营商名称
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 分销商Code
        /// </summary>
        public string BussinessmanCode { get; set; }

        /// <summary>
        /// 分销商名称
        /// </summary>
        public string BussinessmanName { get; set; }


        /// <summary>
        /// 乘机人Id
        /// </summary>
        public int? PassengerId { get; set; }

        /// <summary>
        /// 航线Id
        /// </summary>
        public int? SkyWayId { get; set; }

        /// <summary>
        /// 被投保人姓名
        /// </summary>
        public string InsuredName { get; set; }

        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumIDType IdType { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public EnumSexType SexType { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime? ToDateTime { get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }

        /// <summary>
        /// 保额
        /// </summary>
        public decimal PolicyAmount { get; set; }

        public string PNR { get; set; }

        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityName { get; set; }

        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCode { get; set; }

        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityName { get; set; }

        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCode { get; set; }

        /// <summary>
        /// 投保方式
        /// </summary>
        public EnumInsureMethod InsureType { get; set; }

        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }

        public void CheckData()
        {
            //保险序列号不可为空
            if (string.IsNullOrWhiteSpace(this.SerialNum))
            {
                throw new CustomException(1000111, "保险序列号不可为空。");
            }
            //保险生效开始时间不得为空
            if (this.InsuranceLimitStartTime==null)
            {
                throw new CustomException(1000112, "保险生效开始时间不得为空。");
            }
            //保险生效结束时间不得为空
            if (this.InsuranceLimitEndTime == null)
            {
                throw new CustomException(1000113, "保险生效结束时间不得为空。");
            }
            //被投保人姓名不得为空
            if (string.IsNullOrWhiteSpace(this.InsuredName))
            {
                throw new CustomException(1000114, "被投保人姓名不得为空。");
            }
            //证件类型不可为空，不用判断
            //证件号不用判断
            //性别不可为空，不用判断
            //生日不得为空
            if (string.IsNullOrWhiteSpace(this.InsuredName))
            {
                throw new CustomException(1000115, "生日不得为空。");
            }
            ////电话号码不可为空
            //if (string.IsNullOrWhiteSpace(this.Mobile))
            //{
            //    throw new CustomException(1000116, "电话号码不可为空。");
            //}
            //保险数量必须大于零
            if (this.Count <= 0)
            {
                throw new CustomException(1000117, "保险数量必须大于零。");
            }
            //航班号不可为空
            if (string.IsNullOrWhiteSpace(this.FlightNumber))
            {
                throw new CustomException(1000118, "航班号不可为空。");
            }
            //起飞时间不可为空
            if (this.StartDateTime==null)
            {
                throw new CustomException(1000119, "起飞时间不可为空。");
            }
            //到达城市名不可为空
            if (string.IsNullOrWhiteSpace(this.ToCityName))
            {
                throw new CustomException(1000120, "到达城市名不可为空。");
            }
        }

        /// <summary>
        /// 获取保险记录支付用订单号
        /// </summary>
        /// <returns></returns>
        public string GetSerialNum()
        {
            //每次生成随机数的时候都使用机密随机数生成器来生成种子，
            //这样即使在很短的时间内也可以保证生成的随机数不同
            var rand = new Random(GetRandomSeed());
            var exNum = rand.Next(1000, 9999);
            return string.Format("XYJR{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), exNum);
        }

        /// <summary>
        /// 得到随机数种子
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
