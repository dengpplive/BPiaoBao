using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.StationMap
{
    /// <summary>
    /// 订单修改信息
    /// </summary>
    public class OrderDataObject
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public string PlatForm { get; set; }
        /// <summary>
        /// 接口单号
        /// </summary>
        public string InterfaceOrderId { get; set; }
        /// <summary>
        /// 平台政策
        /// </summary>
        public decimal PlatPolicy { get; set; }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 退改上班时间
        /// </summary>
        public string ReturnOnTime { get; set; }
        /// <summary>
        /// 退改下班时间
        /// </summary>
        public string ReturnUnTime { get; set; }
        /// <summary>
        /// 废票上班时间
        /// </summary>
        public string AnnulOnTime { get; set; }
        /// <summary>
        /// 废票下班时间
        /// </summary>
        public string AnnulUnTime { get; set; }
    }
    public class PassengerDataObject
    {
        /// <summary>
        /// 乘机人ID
        /// </summary>
        public int PassengerId { get; set; }
        /// <summary>
        /// 乘机人姓名
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber { get; set; }
    }
    public class AfterPassengerDataObject
    {
        /// <summary>
        /// 售后乘机人
        /// </summary>
        public int AfterPassengerId { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public string AfterPassengerName { get; set; }
        /// <summary>
        /// 售后乘机人票号
        /// </summary>
        public string AfterSaleTravelTicketNum { get; set; }
    }

    #region 起飞日期监控
    /// <summary>
    /// 
    /// </summary>
    public class ResponeTempSum
    {
        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime IssueDate { get; set; }
        /// <summary>
        /// T+0 当天
        /// </summary>
        public int T0 { get; set; }
        /// <summary>
        /// T+1 第二天
        /// </summary>
        public int T1 { get; set; }
        /// <summary>
        /// T+2 第三天
        /// </summary>
        public int T2 { get; set; }
        /// <summary>
        /// T+3 第四天
        /// </summary>
        public int T3 { get; set; }
        /// <summary>
        /// T+4 第五天
        /// </summary>
        public int T4 { get; set; }
        /// <summary>
        /// T+5 第六天
        /// </summary>
        public int T5 { get; set; }
        /// <summary>
        /// T+6 第七天
        /// </summary>
        public int T6 { get; set; }
        /// <summary>
        /// T+7  第八天
        /// </summary>
        public int T7 { get; set; }
        /// <summary>
        /// >T+7  表示起飞时间大于第八天的所有
        /// </summary>
        public int T8 { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }
    }
    public class TempTSum
    {
        public TempTSum()
        {
            if (this.List == null)
                this.List = new List<DateTime>();
        }
        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime IssueDate { get; set; }
        /// <summary>
        /// 起飞时间订单数集合
        /// </summary>
        public List<DateTime> List { get; set; }
    }
    public class TempSum
    {
        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime IssueDate { get; set; }
        /// <summary>
        /// 起飞日期
        /// </summary>
        public DateTime FlyDate { get; set; }
    } 
    #endregion

    public class TempAllTicketSum
    {
        public TempAllTicketSum()
        {
            if (this.Hour == null)
                this.Hour = new List<int>();
        }
        public DateTime CreateDate { get; set; }
        public List<int> Hour { get; set; }
    }

    public class ResponseAllTicketSum
    {
        public DateTime CreateDate { get; set; }
        public int total { get; set; }
        public int Zero { get; set; }
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
        public int four { get; set; }
        public int five { get; set; }
        public int six { get; set; }
        public int seven { get; set; }
        public int eight { get; set; }
        public int nine { get; set; }
        public int ten { get; set; }
        public int eleven { get; set; }
        public int twelve { get; set; }
        public int thirteen { get; set; }
        public int fourteen { get; set; }
        public int fifteen { get; set; }
        public int sixteen { get; set; }
        public int seventeen { get; set; }
        public int eighteen { get; set; }
        public int ninteen { get; set; }
        public int twenty { get; set; }
        public int twenty_one { get; set; }
        public int twenty_two { get; set; }
        public int twenty_three { get; set; }
    }
}
