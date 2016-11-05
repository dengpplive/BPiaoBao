using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
   public  class AirChangeCoordionDto
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
    }
   public class AirChangTimeOut
   {
       /// <summary>
       /// QT开始时间
       /// </summary>
       public string QTStartTime { get; set; }
       /// <summary>
       /// QT结束时间
       /// </summary>
       public string QTEndTime { get; set; }
       /// <summary>
       /// 间隔时间
       /// </summary>
       public string TimeOut { get; set; }
       /// <summary>
       /// 运营商编号
       /// </summary>
       public string CarrierCode { get; set; }
       /// <summary>
       /// 任务是否启动
       /// </summary>
       public bool IsOpen { get; set; }
   }

   public class QTInfo
   {
       public string OrderID { get; set; }
       public string Code { get; set; }
       public string PassengerName { get; set; }
       /// <summary>
       /// 是否是系统PNR TRUE：是
       /// </summary>
       public bool CanPNR { get; set; }
       /// <summary>
       /// 商户名称
       /// </summary>
       public string BusinessmanName { get; set; }

   }
   public class ResponseAirQtInfo
   {
       /// <summary>
       /// QT时间
       /// </summary>
       public DateTime QTDate { get; set; }
       /// <summary>
       /// 发送QT返回结果
       /// </summary>
       public string QTResult { get; set; }
   }
   public class ResponeAirPnrInfo
   {
       public int Id { get; set; }
       /// <summary>
       /// 是否是系统PNR TRUE：是
       /// </summary>
       public bool CanPNR { get; set; }
       /// <summary>
       /// RT信息
       /// </summary>
       public string RTContent { get; set; }
       /// <summary>
       /// QN信息
       /// </summary>
       public string QNContent { get; set; }
       /// <summary>
       /// 操作记录
       /// </summary>
       public List<AirChangeCoordionDto> AirChangeCoordion { get; set; }
   }

   public class ResponseOperateDetail
   {
       /// <summary>
       /// 是否是系统PNR TRUE：是
       /// </summary>
       public bool CanPNR { get; set; }
       /// <summary>
       /// 操作记录
       /// </summary>
       public List<AirChangeCoordionDto> AirChangeCoordion { get; set; }
   }
}
