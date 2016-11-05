using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 扫描票号结果信息
    /// </summary>
    public class OpenTicketResponse
    {
        private List<TicketItem> _OpenTKList = new List<TicketItem>();
        /// <summary>
        /// 票号信息
        /// </summary>
        public List<TicketItem> OpenTKList
        {
            get
            {
                return _OpenTKList;
            }
            set
            {
                _OpenTKList = value;
            }
        }
    }
    /// <summary>
    /// 提取的票号信息
    /// </summary>
    public class TicketItem
    {
        /// <summary>
        /// 票号
        /// </summary>
        public string TKNumber { get; set; }
        /// <summary>
        /// 提取票号结果
        /// </summary>
        public string DetrData { get; set; }
        /// <summary>
        /// 票号状态
        /// </summary>
        public string TKStatus { get; set; }
    }
}
