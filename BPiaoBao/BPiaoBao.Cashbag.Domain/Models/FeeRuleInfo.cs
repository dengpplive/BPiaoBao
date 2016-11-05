using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    public class FeeRuleInfo
    {
        ///// <summary>
        ///// 当日到账费率
        ///// </summary>
        //public decimal todayFee { get; set; }
        ///// <summary>
        ///// 当日到账最低费用
        ///// </summary>
        //public decimal todayMin { get; set; }
        ///// <summary>
        ///// 当日到账最高费用（可为空，为空不限制）
        ///// </summary>
        //public decimal? todayMax { get; set; }
        ///// <summary>
        ///// 次日到账费率
        ///// </summary>
        //public decimal tomorrowFee { get; set; }
        ///// <summary>
        ///// 次日到账最低费用
        ///// </summary>
        //public decimal tomorrowMin { get; set; }
        ///// <summary>
        ///// 次日到账最高费用（可为空，为空不限制）
        ///// </summary>
        //public decimal? tomorrowMax { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// 组名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 默认组所属用户类型，手动添加的都为空
        /// </summary>
        public int? CustomerType { get; set; }
        /// <summary>
        /// 是否默认，默认的不能删除
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 是否节假日
        /// </summary>
        public bool IsHoliday { get; set; }
        #region 当日设置

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool TodayEnable { get; set; }
        /// <summary>
        /// 最迟提交时间，如：15:30:00
        /// </summary>
        public string TodayLast { get; set; }
        /// <summary>
        /// 手续费模式
        /// </summary>
        public int TodayWithdrawRateType { get; set; }
        /// <summary>
        /// 单笔手续费
        /// </summary>
        public decimal TodayEachFeeAmount { get; set; }
        /// <summary>
        /// 单笔手续费费率
        /// </summary>
        public decimal TodayEachRate { get; set; }
        /// <summary>
        /// 单笔最低手续费
        /// </summary>
        public decimal TodayEachFeeAmountMin { get; set; }
        /// <summary>
        /// 单笔最高手续费，为空则不限制
        /// </summary>
        public decimal? TodayEachFeeAmountMax { get; set; }
        /// <summary>
        /// 每日限额，为空则不限制
        /// </summary>
        public decimal? TodayDayAmount { get; set; }
        /// <summary>
        /// 每笔限额，为空则不限制
        /// </summary>
        public decimal? TodayEachAmount { get; set; }

        #endregion

        #region 次日设置

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool MorrowEnable { get; set; }
        /// <summary>
        /// 最迟提交时间，如：15:30:00
        /// </summary>
        public string MorrowLast { get; set; }
        /// <summary>
        /// 手续费模式
        /// </summary>
        public int MorrowWithdrawRateType { get; set; }
        /// <summary>
        /// 单笔手续费
        /// </summary>
        public decimal MorrowEachFeeAmount { get; set; }
        /// <summary>
        /// 单笔手续费费率
        /// </summary>
        public decimal MorrowEachRate { get; set; }
        /// <summary>
        /// 单笔最低手续费
        /// </summary>
        public decimal MorrowEachFeeAmountMin { get; set; }
        /// <summary>
        /// 单笔最高手续费，为空则不限制
        /// </summary>
        public decimal? MorrowEachFeeAmountMax { get; set; }
        /// <summary>
        /// 每日限额，为空则不限制
        /// </summary>
        public decimal? MorrowDayAmount { get; set; }
        /// <summary>
        /// 每笔限额，为空则不限制
        /// </summary>
        public decimal? MorrowEachAmount { get; set; }

        #endregion

    }
}
