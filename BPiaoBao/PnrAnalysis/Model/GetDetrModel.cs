﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    [Serializable]
    public class DetrModel
    {
        /// <summary>
        /// 乘机人姓名
        /// </summary>
        public string PassengerName = string.Empty;
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber = string.Empty;
        /// <summary>
        /// 行程单号
        /// </summary>
        public string SerialNumber = string.Empty;
        /// <summary>
        /// 证件号
        /// </summary>
        public string SsrCard = string.Empty;

        /// <summary>
        /// //验证码
        /// </summary>
        public string CheckVate = string.Empty;
    }
    [Serializable]
    public class DetrInfo
    {
        public string CreateSerialNumber = string.Empty;
        public string VoidSerialNumber = string.Empty;
        public string TicketNum = string.Empty;
        public string PassengerName = string.Empty;
        public string PasCardID = string.Empty;
    }
}
