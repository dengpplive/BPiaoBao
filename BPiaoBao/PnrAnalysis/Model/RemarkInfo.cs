using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 编码备注儿童或者婴儿的指令信息
    /// </summary>
    /// 
    [Serializable]
    public class RemarkInfo
    {
        private string _PassengerType = "0";
        /// <summary>
        /// 成人1 儿童2 婴儿3
        /// </summary>
        public string PassengerType
        {
            get { return _PassengerType; }
            set { _PassengerType = value; }
        }

        private int _RemarkType = 0;
        /// <summary>
        /// 备注类型 0.成人编码备注婴儿 1.儿童编码备注成人信息
        /// </summary>
        public int RemarkType
        {
            get { return _RemarkType; }
            set { _RemarkType = value; }
        }

        private string _RemarkCmd = string.Empty;
        /// <summary>
        /// 备注指令
        /// </summary>
        public string RemarkCmd
        {
            get { return _RemarkCmd; }
            set { _RemarkCmd = value; }
        }
        private string _RemarkRecvData = string.Empty;
        /// <summary>
        /// 卑备注返回信息
        /// </summary>
        public string RemarkRecvData
        {
            get { return _RemarkRecvData; }
            set { _RemarkRecvData = value; }
        }
    }
}
