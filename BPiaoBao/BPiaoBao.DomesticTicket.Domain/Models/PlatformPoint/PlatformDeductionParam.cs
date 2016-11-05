using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint
{
    /// <summary>
    /// 平台扣点参数
    /// </summary>
    public class PlatformDeductionParam
    {
        public PlatformDeductionParam()
        {
        }
        private List<FlyLine> _FlyLineList = new List<FlyLine>();
        public List<FlyLine> FlyLineList { get { return _FlyLineList; } }
        /// <summary>
        /// 1=单程，2=往返，3=中转联程 4缺口程 5多程
        /// </summary>
        public int TravelType
        {
            get
            {
                int _TravelType = 1;
                if (_FlyLineList.Count >= 2)
                {
                    if (_FlyLineList.Count > 2)
                    {
                        _TravelType = 5;
                    }
                    else
                    {
                        FlyLine leg0 = FlyLineList[0];
                        FlyLine leg1 = FlyLineList[1];
                        if (leg0.ToCityCode == leg1.FromCityCode)
                        {
                            if (leg0.FromCityCode == leg1.ToCityCode)
                            {
                                _TravelType = 2;
                            }
                            else
                            {
                                _TravelType = 3;
                            }
                        }
                        else
                        {
                            _TravelType = 4;
                        }
                    }
                }
                return _TravelType;
            }
        }
        /// <summary>
        /// 航空公司
        /// </summary>
        public string CarrayCode
        {
            get
            {
                string CarrayCode = string.Empty;
                if (this._FlyLineList.Count > 0)
                {
                    CarrayCode = this._FlyLineList[0].CarrayCode;
                }
                return CarrayCode;
            }
        }
    }
    public class FlyLine
    {
        public string CarrayCode { get; set; }
        public string FromCityCode { get; set; }
        public string ToCityCode { get; set; }
    }
    /// <summary>
    /// 匹配到的平台扣点结果
    /// </summary>
    public class PlatformDeduction
    {
        public PlatformDeduction()
        {
            this.AdjustType = AdjustType.Lrish;
        }
        /// <summary>
        /// 规则类型
        /// </summary>
        public AdjustType AdjustType { get; set; }
        /// <summary>
        /// 扣点组Id
        /// </summary>
        public Guid? PlatformPointGroupID { get; set; }
        /// <summary>
        /// 规则ID
        /// </summary>
        public int RuleID { get; set; }
        /// <summary>
        /// 扣点
        /// </summary>
        public decimal Point { get; set; }
        /// <summary>
        /// 是否协调【设置最大点数】
        /// </summary>
        public bool IsMax { get; set; }
        /// <summary>
        /// 协调点数
        /// </summary>
        public decimal? MaxPoint { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PolicySource { get; set; }
        /// <summary>
        /// 匹配到的政策ID
        /// </summary>
        public string PolicyId { get; set; }

    }
}
