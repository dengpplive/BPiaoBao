using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Norm;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// 城市信息
    /// </summary>
    public class CityCode
    {
        public CityCode()
        {
            _id = Guid.NewGuid().ToString();
        }
        public string _id { get; set; }
        /// <summary>
        /// 三字码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 航站楼
        /// </summary>
        public string Terminal { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 简拼
        /// </summary>
        public string SimplePinyin { get; set; }
        /// <summary>
        /// 全拼
        /// </summary>
        public string WholePinyin { get; set; }
    }
}
