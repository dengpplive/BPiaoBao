using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{

    public class PidSetting
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Office { get; set; }
        public string buyerCode { get; set; }
        public string CarrierCode { get; set; }
        public string SupplierCode { get; set; }
    }

    public class PidParam
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 请求IP
        /// </summary>
        public string RequestIP { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string ServerIP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string ServerPort { get; set; }
        /// <summary>
        /// 使用的Office
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 发送指令
        /// </summary>
        public string Cmd { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public string RecvData { get; set; }
        /// <summary>
        /// 是否需要PN
        /// </summary>
        public bool IsPn = false;
        /// <summary>
        /// 是否需要返回RT所有内容
        /// </summary>
        public bool IsAllResult = false;

        /// <summary>
        /// 是否使用扩展发送指令的方法
        /// </summary>
        public bool IsUseExtend = false;
        /// <summary>
        /// 扩展数据
        /// </summary>
        public string ExtendData = string.Empty;

        /// <summary>
        /// 是否处理返回结果 如变大写 ^替换为\r
        /// </summary>
        public bool IsHandResult = true;
        /// <summary>
        /// 记录指令日志目录
        /// </summary>
        public string LogDir = string.Empty;
    }
}
