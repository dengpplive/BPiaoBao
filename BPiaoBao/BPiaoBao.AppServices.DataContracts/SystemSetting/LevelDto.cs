using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.SystemSetting
{
    public class LevelDto
    {
        public int ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 欠款手续费费率
        /// </summary>
        public decimal HandRate { get; set; }
        /// <summary>
        /// 滞纳金费率
        /// </summary>
        public decimal OverdueRate { get; set; }
        /// <summary>
        /// 用户积分
        /// </summary>
        public int Integral { get; set; }
        /// <summary>
        /// 信用额度
        /// </summary>
        public decimal CreditQuota { get; set; }
        /// <summary>
        /// 临时额度
        /// </summary>
        public decimal TempQuota { get; set; }
        /// <summary>
        /// 排序编号
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
    }
}
