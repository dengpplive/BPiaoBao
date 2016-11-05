using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 预订编码返回信息实体
    /// </summary>
    [Serializable]
    public class YDInfo
    {
        private string _Pnr = string.Empty;
        /// <summary>
        /// 预订信息中解析出来的编码
        /// </summary>
        public string Pnr
        {
            get
            {
                return _Pnr;
            }
            set
            {
                _Pnr = value;
            }
        }

        private string _YDPnrStatus = string.Empty;
        /// <summary>
        /// 预订编码返回来的 PNR状态信息 DK|HK
        /// </summary>
        public string YDPnrStatus
        {
            get
            {
                return _YDPnrStatus;
            }
            set
            {
                _YDPnrStatus = value;
            }
        }

        private List<LegInfo> _YDLegInfo = new List<LegInfo>();
        /// <summary>
        /// 预订返回来的航段信息
        /// </summary>
        public List<LegInfo> YDLegInfo
        {
            get
            {
                return _YDLegInfo;
            }
            set
            {
                _YDLegInfo = value;
            }
        }
    }
}
