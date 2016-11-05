using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common
{
    /// <summary>
    /// 系统全局开关
    /// </summary>
    public static class SystemConsoSwitch
    {       
        /// <summary>
        /// 航空公司开关
        /// </summary>
        public static List<AirSystem> AirSystems { get; set; }
        /// <summary>
        /// 平台开关设置
        /// </summary>
        public static List<PlatSystem> PlatSystems { get; set; }
        /// <summary>
        /// 钱袋子手续费费率
        /// </summary>
        public static decimal Rate { get; set; }
        /// <summary>
        /// 所有商户的QT设置
        /// </summary>
        public static List<QTSetting> QTSettingList = new List<QTSetting>();
        
    }
    /// <summary>
    /// QT设置
    /// </summary>
    public class QTSetting
    {
        /// <summary>
        /// 提供配置的商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// QT间隔时间
        /// </summary>
        public string Timeout { get; set; }
        /// <summary>
        /// QT开始时间
        /// </summary>
        public string QTStartTime { get; set; }
        /// <summary>
        /// QT结束时间
        /// </summary>
        public string QTEndTime { get; set; }
        /// <summary>
        /// 任务是否启动
        /// </summary>
        public bool IsOpen { get; set; }
    }
    /// <summary>
    /// 航空公司开关设置
    /// </summary>
    public class AirSystem
    {
        public AirSystem()
        {
            this.IsQuery = true;
            this.IsB2B = true;
            this.IsBSP = true;
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string AirCode { get; set; }
        /// <summary>
        /// 是否开启查询
        /// </summary>
        public bool IsQuery { get; set; }
        /// <summary>
        /// B2B政策 true开 false禁用
        /// </summary>
        public bool IsB2B { get; set; }
        /// <summary>
        /// BSP政策
        /// </summary>
        public bool IsBSP { get; set; }

    }
    /// <summary>
    /// 平台接口开关设置
    /// </summary>
    public class PlatSystem
    {
        public PlatSystem()
        {
            this.State = true;
            this.GetPolicyCount = 5;
        }
        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatfromCode { get; set; }
        /// <summary>
        /// 获取政策数量
        /// </summary>
        public int GetPolicyCount { get; set; }
        /// <summary>
        /// 出票速度
        /// </summary>
        public string IssueTicketSpeed { get; set; }
        /// <summary>
        /// 接口状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 关闭B2B航空公司
        /// </summary>
        public string B2B { get; set; }
        /// <summary>
        /// 关闭BSP航空公司
        /// </summary>
        public string BSP { get; set; }
        /// <summary>
        /// 区域参数
        /// </summary>
        public SystemBigArea SystemBigArea { get; set; }
    }
    /// <summary>
    /// 区域大范围
    /// </summary>
    public class SystemBigArea
    {
        public SystemBigArea()
        {
            this.SystemAreas = new List<SystemArea>();
        }
        public string DefaultCity { get; set; }
        public List<SystemArea> SystemAreas { get; set; }
    }
    /// <summary>
    /// 具体区域
    /// </summary>
    public class SystemArea
    {
        public SystemArea()
        {
            this.Parameters = new List<AreaParameter>();
        }
        public string City { get; set; }
        public List<AreaParameter> Parameters { get; set; }
    }
    /// <summary>
    /// 参数设置
    /// </summary>
    public class AreaParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
