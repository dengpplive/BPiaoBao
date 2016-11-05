using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// BSP自动出票参数
    /// </summary>
    /// 
    [Serializable]
    public class BSPParam
    {
        private ParamObject _Param = new ParamObject();
        /// <summary>
        /// 发送指令参数设置 IP 端口  Office即可
        /// </summary>
        public ParamObject Param
        {
            get { return _Param; }
            set { _Param = value; }
        }

        private string _Pnr = string.Empty;
        /// <summary>
        /// 出票的编码
        /// </summary>
        public string Pnr
        {
            get { return _Pnr; }
            set { _Pnr = value; }
        }
        private decimal _CpPrice = 0m;
        /// <summary>
        /// 出票的舱位价
        /// </summary>
        public decimal CpPrice
        {
            get { return _CpPrice; }
            set { _CpPrice = value; }
        }
        private string _PrintNo = string.Empty;
        /// <summary>
        /// 出票设置的打票机号
        /// </summary>
        public string PrintNo
        {
            get { return _PrintNo; }
            set { _PrintNo = value; }
        }
        private string _CarrayCode = string.Empty;
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode
        {
            get { return _CarrayCode; }
            set { _CarrayCode = value; }
        }

        private bool _IssueINFTicekt = false;
        /// <summary>
        /// 是否出婴儿票
        /// </summary>
        public bool IssueINFTicekt
        {
            get { return _IssueINFTicekt; }
            set { _IssueINFTicekt = value; }
        }
        private int _TryCount = 0;
        /// <summary>
        /// 尝试次数
        /// </summary>
        public int TryCount
        {
            get { return _TryCount; }
            set { _TryCount = value; }
        }
    }
    [Serializable]
    public class BSPResponse
    {
        private string _Msg = string.Empty;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg
        {
            get { return _Msg; }
            set { _Msg = value; }
        }
        private Dictionary<string, string> _BspResult = new Dictionary<string, string>();
        /// <summary>
        /// 票号结果
        /// </summary>
        public Dictionary<string, string> BspResult
        {
            get { return _BspResult; }
            set { _BspResult = value; }
        }
    }
}
