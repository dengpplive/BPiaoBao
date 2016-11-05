using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 证件号信息
    /// </summary>
    /// 
    [Serializable]
    public class SSRInfo
    {
        private string _SerialNum = string.Empty;
        /// <summary>
        /// 证件号序号
        /// </summary>
        public string SerialNum
        {
            get { return _SerialNum; }
            set { _SerialNum = value; }
        }
        private string _PasNum = string.Empty;
        /// <summary>
        /// 证件号对应乘客序号
        /// </summary>
        public string PasNum
        {
            get { return _PasNum; }
            set { _PasNum = value; }
        }
        private string _CardID = string.Empty;
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardID
        {
            get { return _CardID; }
            set { _CardID = value; }
        }

        private string _PassType = string.Empty;

        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public string PassType
        {
            get { return _PassType; }
            set { _PassType = value; }
        }
        private string _PasName = string.Empty;
        /// <summary>
        /// 该证件号所属乘客姓名
        /// </summary>
        public string PasName
        {
            get { return _PasName; }
            set { _PasName = value; }
        }
    }
}
