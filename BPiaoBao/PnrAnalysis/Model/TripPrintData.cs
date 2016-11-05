using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 行程单打印数据实体
    /// </summary>
    /// 
    [Serializable]
    public class TripPrintData
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string m_strOrderId = string.Empty;
        /// <summary>
        /// 乘客Id
        /// </summary>
        public string m_strPassengerId = string.Empty;

        /// <summary>
        /// 1单程 2往返 3联程 4缺口程 5.多程
        /// </summary>
        public string m_strTravelType = "1";

        /// <summary>
        /// 行程单号 10位数字
        /// </summary>
        public string m_strTravelNumber = string.Empty;
        /// <summary>
        /// 乘机人姓名
        /// </summary>
        public string m_strPassengerName = string.Empty;
        /// <summary>
        /// 乘机人证件号
        /// </summary>
        public string m_strPassengerCardId = string.Empty;
        /// <summary>
        /// 签注
        /// </summary>
        public string m_strEndorsements = "不得签转变更退票";
        /// <summary>
        /// 出票时限显示字符串 如：TL/0000/13AUG/CTU324
        /// </summary>
        public string m_strTL = string.Empty;
        /// <summary>
        /// 小编码
        /// </summary>
        public string m_strPnrB = string.Empty;

        /// <summary>
        /// 舱位价
        /// </summary>
        public string m_strSpaceFare = string.Empty;
        /// <summary>
        /// 基建
        /// </summary>
        public string m_strABFare = string.Empty;
        /// <summary>
        /// 燃油
        /// </summary>
        public string m_strFuelFare = string.Empty;
        /// <summary>
        /// 其他费用
        /// </summary>
        public string m_strOtherFare = string.Empty;
        /// <summary>
        /// 总共票价
        /// </summary>
        public string m_strTotalFare = string.Empty;



        /// <summary>
        /// 票号
        /// </summary>
        public string m_strTicketNumber = string.Empty;
        /// <summary>
        /// 验证码
        /// </summary>
        public string m_strCheckNum = string.Empty;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string m_strPromptMsg = string.Empty;
        /// <summary>
        /// 保险
        /// </summary>
        public string m_strInsuranceFare = string.Empty;

        /// <summary>
        /// 代理人Office
        /// </summary>
        public string m_strAgentOffice = string.Empty;
        /// <summary>
        /// 航协号
        /// </summary>
        public string m_strIataCode = string.Empty;
        /// <summary>
        /// 填开单位中文
        /// </summary>
        public string m_strCNTKTConjunction = string.Empty;
        /// <summary>
        /// 填开单位英文
        /// </summary>
        public string m_strENTKTConjunction = string.Empty;
        /// <summary>
        /// 填开日期 当天
        /// </summary>
        public string m_strIssuedDate = string.Empty;


        /*-------------1-------------------------------------*/
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string m_strAirName1 = string.Empty;
        /// <summary>
        /// 出发城市 
        /// </summary>
        public string m_strFCityName1 = string.Empty;
        /// <summary>
        /// 到达城市
        /// </summary>
        public string m_strTCityName1 = string.Empty;
        /// <summary>
        /// 航站楼 T1--
        /// </summary>
        public string m_strTerminal1 = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string m_strAirCode1 = string.Empty;
        /// <summary>
        /// 航班号 1475
        /// </summary>
        public string m_strFlightNum1 = string.Empty;
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string m_strSpace1 = string.Empty;
        /// <summary>
        /// 起飞日期 2013-07-20
        /// </summary>
        public string m_strFlyDate1 = string.Empty;
        /// <summary>
        /// 起飞时间 08:00
        /// </summary>
        public string m_strFlyStartTime1 = string.Empty;
        /// <summary>
        /// 到达时间 08:00
        /// </summary>
        public string m_strFlyEndTime1 = string.Empty;
        /// <summary>
        /// 客票级别/客票类型 Y100
        /// </summary>
        public string m_strTicketBasis1 = string.Empty;
        /// <summary>
        /// 客票开始生效期 2013-01-24
        /// </summary>
        public string m_strTicketValidBefore1 = string.Empty;
        /// <summary>
        /// 客票结束日期 2013-01-24
        /// </summary>
        public string m_strTicketValidAfter1 = string.Empty;
        /// <summary>
        /// 行李包重量 20K
        /// </summary>
        public string m_strAllowPacket1 = string.Empty;
        /*-------------2-------------------------------------*/
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string m_strAirName2 = string.Empty;
        /// <summary>
        /// 出发城市
        /// </summary>
        public string m_strFCityName2 = string.Empty;
        /// <summary>
        /// 到达城市
        /// </summary>
        public string m_strTCityName2 = string.Empty;
        /// <summary>
        /// 航站楼 T1--
        /// </summary>
        public string m_strTerminal2 = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string m_strAirCode2 = string.Empty;
        /// <summary>
        /// 航班号 1475
        /// </summary>
        public string m_strFlightNum2 = string.Empty;
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string m_strSpace2 = string.Empty;
        /// <summary>
        /// 起飞日期 2013-07-20
        /// </summary>
        public string m_strFlyDate2 = string.Empty;
        /// <summary>
        /// 起飞时间 08:00
        /// </summary>
        public string m_strFlyStartTime2 = string.Empty;
        /// <summary>
        /// 到达时间 08:00
        /// </summary>
        public string m_strFlyEndTime2 = string.Empty;
        /// <summary>
        /// 客票级别/客票类型 Y100
        /// </summary>
        public string m_strTicketBasis2 = string.Empty;
        /// <summary>
        /// 客票开始生效期 2013-01-24
        /// </summary>
        public string m_strTicketValidBefore2 = string.Empty;
        /// <summary>
        /// 客票结束日期 2013-01-24
        /// </summary>
        public string m_strTicketValidAfter2 = string.Empty;
        /// <summary>
        /// 行李包重量 20K
        /// </summary>
        public string m_strAllowPacket2 = string.Empty;

        /*-------------3-------------------------------------*/
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string m_strAirName3 = string.Empty;
        /// <summary>
        /// 出发城市
        /// </summary>
        public string m_strFCityName3 = string.Empty;
        /// <summary>
        /// 到达城市
        /// </summary>
        public string m_strTCityName3 = string.Empty;
        /// <summary>
        /// 航站楼 T1--
        /// </summary>
        public string m_strTerminal3 = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string m_strAirCode3 = string.Empty;
        /// <summary>
        /// 航班号 1475
        /// </summary>
        public string m_strFlightNum3 = string.Empty;
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string m_strSpace3 = string.Empty;
        /// <summary>
        /// 起飞日期 2013-07-20
        /// </summary>
        public string m_strFlyDate3 = string.Empty;
        /// <summary>
        /// 起飞时间 08:00
        /// </summary>
        public string m_strFlyStartTime3 = string.Empty;
        /// <summary>
        /// 到达时间 08:00
        /// </summary>
        public string m_strFlyEndTime3 = string.Empty;
        /// <summary>
        /// 客票级别/客票类型 Y100
        /// </summary>
        public string m_strTicketBasis3 = string.Empty;
        /// <summary>
        /// 客票开始生效期 2013-01-24
        /// </summary>
        public string m_strTicketValidBefore3 = string.Empty;
        /// <summary>
        /// 客票结束日期 2013-01-24
        /// </summary>
        public string m_strTicketValidAfter3 = string.Empty;
        /// <summary>
        /// 行李包重量 20K
        /// </summary>
        public string m_strAllowPacket3 = string.Empty;

        /*-------------4-------------------------------------*/
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string m_strAirName4 = string.Empty;
        /// <summary>
        /// 出发城市
        /// </summary>
        public string m_strFCityName4 = string.Empty;
        /// <summary>
        /// 到达城市
        /// </summary>
        public string m_strTCityName4 = string.Empty;
        /// <summary>
        /// 航站楼 T1--
        /// </summary>
        public string m_strTerminal4 = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string m_strAirCode4 = string.Empty;
        /// <summary>
        /// 航班号 1475
        /// </summary>
        public string m_strFlightNum4 = string.Empty;
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string m_strSpace4 = string.Empty;
        /// <summary>
        /// 起飞日期 2013-07-20
        /// </summary>
        public string m_strFlyDate4 = string.Empty;
        /// <summary>
        /// 起飞时间 08:00
        /// </summary>
        public string m_strFlyStartTime4 = string.Empty;
        /// <summary>
        /// 到达时间 08:00
        /// </summary>
        public string m_strFlyEndTime4 = string.Empty;
        /// <summary>
        /// 客票级别/客票类型 Y100
        /// </summary>
        public string m_strTicketBasis4 = string.Empty;
        /// <summary>
        /// 客票开始生效期 2013-01-24
        /// </summary>
        public string m_strTicketValidBefore4 = string.Empty;
        /// <summary>
        /// 客票结束日期 2013-01-24
        /// </summary>
        public string m_strTicketValidAfter4 = string.Empty;
        /// <summary>
        /// 行李包重量 20K
        /// </summary>
        public string m_strAllowPacket4 = string.Empty;

        /*-------------5-------------------------------------*/
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string m_strAirName5 = string.Empty;
        /// <summary>
        /// 出发城市
        /// </summary>
        public string m_strFCityName5 = string.Empty;
        /// <summary>
        /// 到达城市
        /// </summary>
        public string m_strTCityName5 = string.Empty;
        /// <summary>
        /// 航站楼 T1--
        /// </summary>
        public string m_strTerminal5 = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string m_strAirCode5 = string.Empty;
        /// <summary>
        /// 航班号 1475
        /// </summary>
        public string m_strFlightNum5 = string.Empty;
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string m_strSpace5 = string.Empty;
        /// <summary>
        /// 起飞日期 2013-07-20
        /// </summary>
        public string m_strFlyDate5 = string.Empty;
        /// <summary>
        /// 起飞时间 08:00
        /// </summary>
        public string m_strFlyStartTime5 = string.Empty;
        /// <summary>
        /// 到达时间 08:00
        /// </summary>
        public string m_strFlyEndTime5 = string.Empty;
        /// <summary>
        /// 客票级别/客票类型 Y100
        /// </summary>
        public string m_strTicketBasis5 = string.Empty;
        /// <summary>
        /// 客票开始生效期 2013-01-24
        /// </summary>
        public string m_strTicketValidBefore5 = string.Empty;
        /// <summary>
        /// 客票结束日期 2013-01-24
        /// </summary>
        public string m_strTicketValidAfter5 = string.Empty;
        /// <summary>
        /// 行李包重量 20K
        /// </summary>
        public string m_strAllowPacket5 = string.Empty;

    }

}

