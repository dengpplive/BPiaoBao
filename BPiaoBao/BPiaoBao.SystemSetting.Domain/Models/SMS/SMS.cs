using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.SMS
{
    public class SMS : ValueObjectBase
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode { get; set; }
        /// <summary>
        /// 剩余条数
        /// </summary>
        public int RemainCount { get; set; }
        /// <summary>
        /// 已发送条数
        /// </summary>
        public int SendCount { get; set; }
        public virtual Businessman Businessman { get; set; }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="count"></param>
        public void Send(int count)
        {
            this.RemainCount -= count;
            this.SendCount += count;
        }
        /// <summary>
        /// 购买
        /// </summary>
        /// <param name="count"></param>
        public void Buy(int count)
        {
            this.RemainCount += count;
        }

    }
}
