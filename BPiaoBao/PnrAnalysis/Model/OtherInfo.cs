using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    [Serializable]
    public class OtherInfo
    {
        private string _CT = string.Empty;
        /// <summary>
        /// CT
        /// </summary>
        public string CT
        {
            get { return _CT; }
            set { _CT = value; }
        }
        private string _CTCT = string.Empty;
        /// <summary>
        /// CTCT
        /// </summary>
        public string CTCT
        {
            get { return _CTCT; }
            set { _CTCT = value; }
        }

        private List<string> _CTCTList = new List<string>();
        /// <summary>
        /// CTCT电话列表
        /// </summary>
        public List<string> CTCTList
        {
            get { return _CTCTList; }
            set { _CTCTList = value; }
        }

        private bool _IsTL = false;
        /// <summary>
        /// 编码中是否含有出票时限
        /// </summary>
        public bool IsTL
        {
            get { return _IsTL; }
            set { _IsTL = value; }
        }
        private string _strTL = string.Empty;
        /// <summary>
        /// 出票时限字符串显示 如：TL/0000/13AUG/CTU324
        /// </summary>
        public string StrTL
        {
            get
            {
                return _strTL;
            }
            set
            {
                _strTL = value;
            }
        }
        private Tktl _TKTL = null;
        /// <summary>
        /// 出票时限
        /// </summary>
        public Tktl TKTL
        {
            get
            {
                return _TKTL;
            }
            set
            {
                _TKTL = value;
            }
        }
    }
}
