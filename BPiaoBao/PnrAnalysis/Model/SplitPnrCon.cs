using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 从混乱的字符串内容中分离出正确的数据内容
    /// </summary>
    /// 
    [Serializable]
    public class SplitPnrCon
    {
        /// <summary>
        /// 所有内容
        /// </summary>
        public string ALLCon = string.Empty;
        /// <summary>
        /// 单独的RT内容
        /// </summary>
        public string RTCon = string.Empty;
        /// <summary>
        /// 成人PAT内容
        /// </summary>
        public string AdultPATCon = string.Empty;
        /// <summary>
        /// 儿童PAT内容
        /// </summary>
        public string ChdPATCon = string.Empty;
        /// <summary>
        /// 婴儿PAT内容
        /// </summary>
        public string INFPATCon = string.Empty;
    }
}
