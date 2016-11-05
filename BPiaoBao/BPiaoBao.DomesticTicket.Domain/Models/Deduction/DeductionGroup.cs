using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Deduction
{

    public class DeductionGroup : EntityBase, IAggregationRoot
    {
        public int ID { get; set; }
        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 扣点组名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 扣点组规则
        /// </summary>
        public virtual ICollection<DeductionRule> DeductionRules { get; set; }

        protected override string GetIdentity()
        {
            return ID.ToString();
        }
        public void CheckRule()
        {
            if (string.IsNullOrEmpty(Name))
                throw new CustomException(500, "扣点组名称不能为空");
            else if (string.IsNullOrEmpty(Description))
                throw new CustomException(500, "请输入扣点组描述");
            //实体验证
            this.DeductionRules.ForEach(p =>
            {
                if (p.StartTime == null || p.StartTime == default(DateTime))
                    throw new CustomException(500, "有效开始日期不能为空!");
                else if (p.EndTime == null || p.EndTime == default(DateTime))
                    throw new CustomException(500, "有效结束日期不能为空!");
            });
            var list = this.DeductionRules.GroupBy(p => new { p.CarrCode, p.DeductionType }).Select(x => new { deductionRule = x.Key, Count = x.Count() }).Where(y => y.Count > 1);
            //有承运人和类型相同 比对时间范围
            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    List<DeductionRule> result = new List<DeductionRule>();
                    this.DeductionRules.Where(p => p.CarrCode == item.deductionRule.CarrCode && p.DeductionType == item.deductionRule.DeductionType).ToList().ForEach(x =>
                    {
                        foreach (var resultItem in result)
                        {
                            if ((x.StartTime >= resultItem.StartTime && x.StartTime <= resultItem.EndTime) || (x.EndTime >= resultItem.StartTime && x.EndTime <= resultItem.EndTime) || (resultItem.StartTime >= x.StartTime && resultItem.StartTime <= x.EndTime) || (resultItem.EndTime >= x.StartTime && resultItem.EndTime <= x.EndTime))
                            {
                                throw new CustomException(500, string.Format("{0}-{1}时间有重复", x.CarrCode, x.DeductionType.ToEnumDesc()));
                            }
                        }
                        result.Add(x);
                    });
                }
            }
        }
    }
    public class DeductionRule : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public DeductionType DeductionType { get; set; }
        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 有效截至日期
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 调整明细集合
        /// </summary>
        public virtual ICollection<AdjustDetail> AdjustDetails { get; set; }

        protected override string GetIdentity()
        {
            return ID.ToString();
        }
        public void CheckDetail()
        {
            this.AdjustDetails.ForEach(p =>
            {
                if (p.EndPoint<=0)
                    throw new CustomException(500, string.Format("{0}截至点数不能小于等于0!", this.CarrCode));
                if (p.Point<=0)
                    throw new CustomException(500, string.Format("{0}点数不能不能小于等于0!", this.CarrCode));
            });
            List<AdjustDetail> list = new List<AdjustDetail>();
            foreach (var item in AdjustDetails)
            {
                foreach (var resultItem in list)
                {

                    if ((resultItem.StartPoint >= item.StartPoint && resultItem.StartPoint <= item.EndPoint) ||
                        (resultItem.EndPoint >= item.StartPoint && resultItem.EndPoint <= item.EndPoint) ||
                        (item.StartPoint >= resultItem.StartPoint && item.StartPoint <= resultItem.EndPoint) ||
                        (item.EndPoint >= resultItem.StartPoint && item.EndPoint <= resultItem.EndPoint))
                    {
                        throw new CustomException(500, string.Format("{0}-{1}调整明细有重复", this.CarrCode, this.DeductionType.ToEnumDesc()));
                    }
                }
                list.Add(item);
            }
        }
    }
    /// <summary>
    /// 调整详细
    /// </summary>
    public class AdjustDetail : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 点数范围：开始点数
        /// </summary>
        public decimal StartPoint { get; set; }

        /// <summary>
        /// 点数范围：截至点数
        /// </summary>
        public decimal EndPoint { get; set; }

        /// <summary>
        /// 调整类型
        /// </summary>
        public AdjustType AdjustType { get; set; }

        /// <summary>
        /// 点数
        /// </summary>
        public decimal Point { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }

}
