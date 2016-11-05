using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 票号信息
    /// </summary>
    /// 
    [Serializable]
    public class TicketNumInfo
    {
        private string _SerialNum = string.Empty;
        /// <summary>
        /// 票号序号
        /// </summary>
        public string SerialNum
        {
            get { return _SerialNum; }
            set { _SerialNum = value; }
        }
        private string _PasNum = string.Empty;
        /// <summary>
        /// 票号对应乘客序号 如:TN/027-8958584747/P序号
        /// </summary>
        public string PasNum
        {
            get { return _PasNum; }
            set { _PasNum = value; }
        }

        private string _PasIsYinger = string.Empty;
        /// <summary>
        /// 票号是否为婴儿票号 1是
        /// </summary>
        public string PasIsYinger
        {
            get { return _PasIsYinger; }
            set { _PasIsYinger = value; }
        }

        private string _PasName = string.Empty;
        /// <summary>
        /// 乘机人姓名
        /// </summary>
        public string PasName
        {
            get { return _PasName; }
            set { _PasName = value; }
        }

        private string _PassengerNameEnsh = string.Empty;
        /// <summary>
        /// 乘客姓名 英文全拼大写
        /// </summary>
        public string PassengerNameEnsh
        {
            get { return _PassengerNameEnsh; }
            set { _PassengerNameEnsh = value; }
        }

        private string _TicketNum = string.Empty;
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNum
        {
            get { return _TicketNum; }
            set { _TicketNum = value; }
        }       
    }
}
