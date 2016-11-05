using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class RspMessageModel
    {
        /// <summary>
        /// 操作成功与否标志1：成功 0：失败
        /// </summary>
        public int Success { get; set; }

        /// <summary>
        /// 操作信息提示
        /// </summary>
        public string Message { get; set; }
    }
}