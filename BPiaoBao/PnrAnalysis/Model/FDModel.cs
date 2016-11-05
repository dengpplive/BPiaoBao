﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    [Serializable]
    public class FDModel
    {
        public string fromCode = string.Empty;
        public string toCode = string.Empty;
        public string strDate = string.Empty;
        public string fdDate = string.Empty;
        public string fdMileage = string.Empty;
        /// <summary>
        /// 总数据条数
        /// </summary>
        public string TotalCount = "0";
        /// <summary>
        /// 所有FD数据
        /// </summary>
        public List<FdItemData> FdDataList = new List<FdItemData>();
        /// <summary>
        /// Y舱数据
        /// </summary>
        public List<FdItemData> FdYDataList = new List<FdItemData>();
    }
    [Serializable]
    public class FdItemData
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string num = string.Empty;
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string Carry = string.Empty;
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat = string.Empty;

        /// <summary>
        /// 折扣 保留2位小数
        /// </summary>
        public string DiscountRate = string.Empty;

        /// <summary>
        /// 价格
        /// </summary>
        public string Fare1 = string.Empty;
        /// <summary>
        /// 两个舱位价格
        /// </summary>
        public string Fare2 = string.Empty;
        /// <summary>
        /// 舱位1
        /// </summary>
        public string seat1 = string.Empty;
        /// <summary>
        /// 舱位2
        /// </summary>
        public string seat2 = string.Empty;
        /// <summary>
        /// 日期
        /// </summary>
        public string date = string.Empty;
        /// <summary>
        /// 其他
        /// </summary>
        public string Orther = string.Empty;
    }
}

