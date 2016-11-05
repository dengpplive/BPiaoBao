using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class RequestSplitPnrInfo
    {
        public RequestSplitPnrInfo()
        {
            this.SplitPasList = new List<SplitPassenger>();
        }
        public string BusinessmanCode { get; set; }
        /// <summary>
        /// 预定该编码的Office
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 进行分离的编码
        /// </summary>
        public string Pnr { get; set; }
        /// <summary>
        /// 分离的乘客信息
        /// </summary>
        public List<SplitPassenger> SplitPasList { get; set; }
    }

    public class ResposeSplitPnrInfo
    {
        /// <summary>
        /// 原编码(分离之前的编码)
        /// </summary>
        public string OldPnr { get; set; }
        /// <summary>
        /// 分离成功后的编码
        /// </summary>
        public string NewPnr { get; set; }
        /// <summary>
        /// 分离编码是否成功
        /// </summary>
        public bool IsSUccess { get; set; }
    }
    public class SplitPassenger
    {
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 分离乘客的票号 可选项
        /// </summary>
        public string TicketNumber { get; set; }
    }
}
