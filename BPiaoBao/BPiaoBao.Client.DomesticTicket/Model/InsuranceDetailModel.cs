using System;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    public class InsuranceDetailModel : BuySingleInsuranceModel
    {
        /// <summary>
        /// 保单号
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 投保状态
        /// </summary>
        public EnumInsuranceStatus BuyInsuranceState { get; set; }

        /// <summary>
        /// 投保状态文字
        /// </summary>
        public string BuyInsuranceStateText { get; set; }

        /// <summary>
        /// 投保方式（自动投保/手动投保）
        /// </summary>
        public string BuyInsuranceType { get; set; }

        /// <summary>
        /// 投保时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 保险有限期（开始时间）
        /// </summary>
        public DateTime InsuranceStartTime { get; set; }

        /// <summary>
        /// 保险有限期(结束时间)
        /// </summary>
        public DateTime InsuracneEndTime { get; set; }
        
        /// <summary>
        /// 投保人性别显示字段
        /// </summary>
        public string GenderText { get { return Gender ? "男" : "女"; } }

        /// <summary>
        /// 投保人类型显示字段
        /// </summary>
        public string PersonTypeText
        {
            get
            {
                if (IsAdultType&&!IsChildType&&!IsBabyType)
                {
                    return "成人";
                }
                if (IsChildType&&!IsAdultType&&!IsBabyType)
                {
                    return "儿童";
                }
                if (IsBabyType&&!IsAdultType&&!IsChildType)
                {
                    return "婴儿";
                }
                return "";
            }
        }

        /// <summary>
        /// 证件类型显示字段
        /// </summary>
        public string IdTypeText
        {
            get
            {
                if (IsIdType && !IsMilitaryIdType && !IsPassportIdType && !IsOtherType)
                {
                    return "身份证";
                }
                if (!IsIdType && IsMilitaryIdType && !IsPassportIdType && !IsOtherType)
                {
                    return "军人证";
                }
                if (!IsIdType && !IsMilitaryIdType && IsPassportIdType && !IsOtherType)
                {
                    return "护照";
                }
                if (!IsIdType && !IsMilitaryIdType && !IsPassportIdType && IsOtherType)
                {
                    return "其它";
                }
                
                return "";
            }
        }


    }
}
