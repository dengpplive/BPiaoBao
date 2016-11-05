using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 乘客证件号修改参数
    /// </summary>
    public class SsrUpdateRequest
    {
        private string _Pnr = string.Empty;
        /// <summary>
        /// 编码
        /// </summary>
        public string Pnr
        {
            get { return _Pnr; }
            set { _Pnr = value; }
        }
        private string _CarryCode = string.Empty;
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarryCode
        {
            get { return _CarryCode; }
            set { _CarryCode = value; }
        }

        private string _Office = string.Empty;
        /// <summary>
        /// Office
        /// </summary>
        public string Office
        {
            get { return _Office; }
            set { _Office = value; }
        }
        private string _BusinessmanCode = string.Empty;
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode
        {
            get { return _BusinessmanCode; }
            set { _BusinessmanCode = value; }
        }
        private Dictionary<string, string> _DicSsrData = new Dictionary<string, string>();
        /// <summary>
        ///  修改的乘机证件号 乘客姓名-证件号
        /// </summary>
        public Dictionary<string, string> DicSsrData
        {
            get { return _DicSsrData; }
            set { _DicSsrData = value; }
        }
    }
    /// <summary>
    /// 乘客证件号修改响应结果
    /// </summary>
    public class SsrUpdateResponse
    {
        private bool _Status = false;
        /// <summary>
        /// 修改状态 true成功 false失败
        /// </summary>
        public bool Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        private string _Remark = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        private string _Recvdata = string.Empty;
        /// <summary>
        /// 指令返回数据
        /// </summary>
        public string Recvdata
        {
            get { return _Recvdata; }
            set { _Recvdata = value; }
        }
    }
}
