using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.AriChang
{
    public class AirChange : EntityBase, IAggregationRoot
    {
        public int Id { get; set; }
        /// <summary>
        /// QT时间
        /// </summary>
        public DateTime QTDate { get; set; }
        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// QT条数
        /// </summary>
        public int QTCount { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// PNR编码
        /// </summary>
        public string PNR { get; set; }
        /// <summary>
        /// 是否是系统PNR TRUE：是
        /// </summary>
        public bool CanPNR { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 发送QT返回结果
        /// </summary>
        public string QTResult { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string CTCT { get; set; }
        /// <summary>
        /// 处理状态 true：完成 false：未完成
        /// </summary>
        public bool ProcessStatus { get; set; }
        /// <summary>
        /// 运营商姓名
        /// </summary>
        public string CarrayName { get; set; }
        /// <summary>
        /// 乘机人多个以|分隔
        /// </summary>
        public string PassengerName { get; set; }
        public string OfficeNum { get; set; }

        /// <summary>
        /// 通知方式
        /// </summary>
        public EnumAriChangNotifications NotifyWay { get; set; }
        /// <summary>
        /// QN内容
        /// </summary>
        public string QNContent { get; set; }

        public virtual ICollection<AirChangeCoordion> AriChangeCoordion { get; set; }

        protected override string GetIdentity()
        {
            return Id.ToString();
        }

    }
    public class AirChangeCoordion : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 通知方式
        /// </summary>
        public EnumAriChangNotifications NotifyWay { get; set; }
        /// <summary>
        /// 处理状态 true：完成 false：未完成
        /// </summary>
        public bool ProcessStatus { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string OpertorName { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }

  


}
