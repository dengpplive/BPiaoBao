using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class PosAssignLogDataObject
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operater { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }
    }
}
