using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class QTResponse
    {
        /// <summary>
        /// 发送QT时间
        /// </summary>
        public DateTime QTDate { get; set; }
        /// <summary>
        /// 发送QT返回结果
        /// </summary>
        public string QTResult { get; set; }

        /// <summary>
        /// SC航变数目
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 发送QN次数
        /// </summary>
        public int QnCount { get; set; }
        /// <summary>
        /// Office
        /// </summary>
        public string Office
        {
            get;
            set;
        }
        /// <summary>
        /// 配置方商户Code
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        private List<QnItem> _QnList = new List<QnItem>();
        /// <summary>
        /// 发送QN返回信息集合
        /// </summary>
        public List<QnItem> QnList
        {
            get
            {
                return _QnList;
            }
            set
            {
                _QnList = value;
            }
        }
    }

    public class QnItem
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Pnr { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string CTCT { get; set; }
        /// <summary>
        /// QN返回结果
        /// </summary>
        public string QnResult { get; set; }
    }
}
