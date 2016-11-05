using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis
{
    [Serializable]
    public class Tktl
    {
        private string _SerialNum = string.Empty;
        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNum
        {
            get { return _SerialNum; }
            set { _SerialNum = value; }
        }

        private string _TkTime = "";
        /// <summary>
        /// 出票时限时间
        /// </summary>
        public string TkTime
        {
            get
            {
                return _TkTime;
            }
            set
            {
                _TkTime = value;
            }
        }
        private string _TkDate = "";
        /// <summary>
        /// 出票时限日期
        /// </summary>
        public string TkDate
        {
            get
            {
                return _TkDate;
            }
            set
            {
                _TkDate = value;
            }
        }
        private string _TkOffice = "";
        /// <summary>
        /// 出票时限Office
        /// </summary>
        public string TkOffice
        {
            get
            {
                return _TkOffice;
            }
            set
            {
                _TkOffice = value;
            }
        }
    }
}
