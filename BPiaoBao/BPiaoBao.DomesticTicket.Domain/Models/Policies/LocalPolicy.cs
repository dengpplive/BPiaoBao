using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Policies
{
    public abstract class LocalPolicy : EntityBase, IAggregationRoot
    {
        public LocalPolicy()
        {
            this.WorkTime = new StartAndEndTime();
            this.ReturnTicketTime = new StartAndEndTime();
            this.AnnulTicketTime = new StartAndEndTime();
            this.WeeKWorkTime = new StartAndEndTime();
            this.WeekReturnTicketTime = new StartAndEndTime();
            this.WeekAnnulTicketTime = new StartAndEndTime();
            this.Carrier_WorkTime = new StartAndEndTime();
            this.Carrier_ReturnTicketTime = new StartAndEndTime();
            this.Carrier_WeekAnnulTicketTime = new StartAndEndTime();
            this.Carrier_WeekWorkTime = new StartAndEndTime();
            this.Carrier_WeekReturnTicketTime = new StartAndEndTime();
            this.Carrier_WeekAnnulTicketTime = new StartAndEndTime();
        }
        public Guid ID { get; set; }
        /// <summary>
        /// 商户Code【运营，供应】
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 角色类型
        /// </summary>
        public string RoleType { get; set; }
        /// <summary>
        /// 发布类型
        /// </summary>
        public EnumReleaseType ReleaseType { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCodes { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCodes { get; set; }
        /// <summary>
        /// 本地返点
        /// </summary>
        public decimal LocalPoint { get; set; }
        /// <summary>
        /// 异地返点
        /// </summary>
        public decimal Different { get; set; }
        /// <summary>
        /// 行程类型
        /// </summary>
        public EnumTravelType TravelType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string LocalPolicyType { get; set; }
        /// <summary>
        /// 低开
        /// </summary>
        public bool Low { get; set; }
        /// <summary>
        /// 换编码
        /// </summary>
        public bool ChangeCode { get; set; }
        /// <summary>
        /// OFFICE号
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 班期限制
        /// </summary>
        public string WeekLimit { get; set; }
        /// <summary>
        /// 共享航班
        /// </summary>
        public bool Share { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seats { get; set; }
        /// <summary>
        /// 乘机日期
        /// </summary>
        public DateLimit PassengeDate { get; set; }
        /// <summary>
        /// 出票日期范围
        /// </summary>
        public DateLimit IssueDate { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay IssueTicketWay { get; set; }
        /// <summary>
        /// 是否适用
        /// </summary>
        public EnumApply Apply { get; set; }
        /// <summary>
        /// 政策使用航班
        /// </summary>
        public string ApplyFlights { get; set; }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public bool Review { get; set; }
        /// <summary>
        /// 挂起状态
        /// </summary>
        public bool HangUp { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateMan { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 运营商Code【如果是供应商政策】
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 供应周末
        /// </summary>
        public string SupplierWeek { get; set; }
        /// <summary>
        /// 运营周末
        /// </summary>
        public string CarrierWeek { get; set; }
        /// <summary>
        /// 工作时间
        /// </summary>
        public StartAndEndTime WorkTime
        {
            get;
            set;
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public StartAndEndTime ReturnTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 废票时间
        /// </summary>
        public StartAndEndTime AnnulTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 工作时间
        /// </summary>
        public StartAndEndTime WeeKWorkTime
        {
            get;
            set;
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public StartAndEndTime WeekReturnTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 废票时间
        /// </summary>
        public StartAndEndTime WeekAnnulTicketTime
        {
            get;
            set;
        }

        /// <summary>
        /// 工作时间
        /// </summary>
        public StartAndEndTime Carrier_WorkTime
        {
            get;
            set;
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public StartAndEndTime Carrier_ReturnTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 废票时间
        /// </summary>
        public StartAndEndTime Carrier_AnnulTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 工作时间
        /// </summary>
        public StartAndEndTime Carrier_WeekWorkTime
        {
            get;
            set;
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public StartAndEndTime Carrier_WeekReturnTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 废票时间
        /// </summary>
        public StartAndEndTime Carrier_WeekAnnulTicketTime
        {
            get;
            set;
        }

        protected override string GetIdentity()
        {
            return ID.ToString();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (var item in this.GetType().GetProperties())
            {
                str.AppendFormat("{0}:{1}", item.Name, item.GetValue(this, null));
            }

            return str.ToString();
        }
    }
    /// <summary>
    /// 本地政策
    /// </summary>
    public class LocalNormalPolicy : LocalPolicy
    {

    }
    /// <summary>
    /// 日期值对象
    /// </summary>
    public class DateLimit : ValueObjectBase
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 截至日期
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
