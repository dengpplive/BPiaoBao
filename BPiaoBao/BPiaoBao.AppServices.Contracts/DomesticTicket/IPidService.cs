using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using PnrAnalysis.Model;

namespace BPiaoBao.AppServices.Contracts.DomesticTicket
{
    [ServiceContract]
    public interface IPidService
    {
        /// <summary>
        /// 发送指令
        /// </summary>
        /// <param name="pidParam"></param>
        /// <returns></returns>
        [OperationContract]
        string SendPid(PidParam pidParam);
        /// <summary>
        /// 获取编码或者票号信息
        /// </summary>
        /// <param name="BusinessmanCode">商户号</param>
        /// <param name="PnrAndTicketNum">编码或者票号</param>
        /// <param name="YdOffice">预定的Office</param>
        /// <param name="isINF">编码中是否有婴儿</param>
        /// <returns></returns>
        [OperationContract]
        string GetPnrAndTickeNumInfo(string businessmanCode, string pnrAndTicketNum, string ydOffice, bool isINF = false, bool isPAT = true);

        /// <summary>
        ///获取指令信息
        /// </summary>
        /// <param name="BusinessmanCode"></param>
        /// <param name="PnrAndTicketNum"></param>
        /// <param name="YdOffice"></param>
        /// <returns></returns>
        [OperationContract]
        string SendCmd(string businessmanCode, string cmd, string Office, string ExtendData = "", bool isRT = false, bool isUseExtend = false);

        /// <summary>
        /// 获取原始配置流量计数
        /// </summary>
        /// <param name="carrierCode">运营商户号</param>
        /// <param name="findConfigType">查找配置的方式  0.使用指定的Office均衡发送指令 1.使用指令的配置名发送指令 2.指定特定的用户发送</param>
        /// <param name="startDate">开始日期[可选]</param>
        /// <param name="endDate">结束日期[可选]</param>
        /// <param name="office">Office</param>
        /// <param name="configName">配置名[可选]</param>
        /// <returns></returns>
        [OperationContract]
        UserFlow GetFlow(string carrierCode, int findConfigType, DateTime? startDate, DateTime? endDate, string office, string configName = "");

        /// <summary>
        /// 自动授权
        /// </summary>
        /// <param name="businessmanCode">采购商户号</param>
        /// <param name="authOffice">授权的Office</param>
        /// <param name="office">预定的Office</param>
        /// <param name="pnr">编码</param>
        /// <returns></returns>
        [OperationContract]
        bool AuthToOffice(string businessmanCode, string authOffice, string office, string pnr);

        /// <summary>
        /// 取消编码
        /// </summary>
        /// <param name="businessmanCode">采购商户号</param>
        /// <param name="office">预定Office</param>
        /// <param name="pnr">编码</param>
        /// <returns></returns>
        [OperationContract]
        bool CancelPnr(string businessmanCode, string office, string pnr);

        /// <summary>
        /// 分离编码
        /// </summary>
        /// <param name="splitPnrParam">分离编码信息</param>
        /// <returns></returns>
        [OperationContract]
        ResposeSplitPnrInfo SplitPnr(RequestSplitPnrInfo splitPnrParam);

        /// <summary>
        /// 票号的挂起解挂
        /// </summary>
        /// <param name="ticketSuppendRequest"></param>
        /// <returns></returns>
        [OperationContract]
        TicketSuppendResponse TicketNumberLock(TicketSuppendRequest ticketSuppendRequest);

        /// <summary>
        /// 修改乘客证件号 只是在myPb预定的可以修改
        /// </summary>
        /// <param name="ssrUpdateRequest"></param>
        /// <returns></returns>
        [OperationContract]
        SsrUpdateResponse UpdateSsr(SsrUpdateRequest ssrUpdateRequest);

        /// <summary>
        /// 儿童编码备注成人编码信息
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="chdPnr"></param>
        /// <param name="adultPnr"></param>
        /// <param name="CarryCode"></param>
        /// <param name="Office"></param>
        [OperationContract]
        void ChdRemarkAdultPnr(string businessmanCode, string chdPnr, string adultPnr, string CarryCode, string Office);

        /// <summary>
        /// 获取运营商的Office信息
        /// </summary>
        /// <param name="carrierCode"></param>
        /// <returns></returns>
        [OperationContract]
        List<string> GetOffice(string carrierCode);

        /// <summary>
        /// 该编码能否取消
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="office"></param>
        /// <param name="pnr"></param>
        /// <returns></returns>
        [OperationContract]
        bool CanCancel(string businessmanCode, string office, string pnr);

        /// <summary>
        /// 配置查航班
        /// </summary>
        /// <param name="code">商户code必填</param>
        /// <param name="fromCode">出发城市三字码必填</param>
        /// <param name="toCode">到达城市三字码必填</param>
        /// <param name="airCode">航空公司二字码</param>
        /// <param name="flyDate">起飞日期必填</param>
        /// <param name="flyTime">起飞时间必填（0000）</param>
        /// <returns></returns>
        [OperationContract]
        string GetAV(string code, string fromCode, string toCode, string airCode, string flyDate, string flyTime);
        /// <summary>
        /// 成人编码中补入或者添加婴儿
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="adultPnr"></param>
        /// <param name="infName"></param>
        /// <param name="birthDay"></param>
        /// <param name="carryCode"></param>
        /// <param name="dblCityCode"></param>
        /// <param name="flightCode"></param>
        /// <param name="seat"></param>
        /// <param name="flyStartDate"></param>
        /// <param name="adultNum"></param>
        /// <param name="skyNum"></param>
        /// <returns></returns>

        [OperationContract]
        bool AddINF(string businessmanCode, string adultPnr, string infName, string birthDay, string carryCode, string dblCityCode, string flightCode, string seat, string flyStartDate, string adultNum, string skyNum);

        /// <summary>
        /// 编码获取PAT数据运价信息
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="Pnr"></param>
        /// <param name="passengerType"></param>
        /// <returns></returns>
        [OperationContract]
        PnrAnalysis.PatModel sendPat(string businessmanCode, string Pnr, EnumPassengerType passengerType);

        /// <summary>
        /// 根据航段信息获取运价
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="flightNo"></param>
        /// <param name="seat"></param>
        /// <param name="flyDate"></param>
        /// <param name="fromCode"></param>
        /// <param name="toCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<PnrAnalysis.PatInfo> sendSSPat(string businessmanCode, string airCode, string flightNo, string seat, string flyDate, string fromCode, string toCode, string startTime, string endTime);

        /// <summary>
        /// 获取所有绑定了配置的商户
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<PIDInfo> GetPids();
        /// <summary>
        /// 发送QT
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        QTResponse SendQT(PIDInfo pid);

        /// <summary>
        /// Open票扫描 阻塞方法
        /// </summary>
        /// <param name="ip">配置的IP</param>
        /// <param name="port">配置的端口</param>
        /// <param name="office">配置的Office</param>
        /// <param name="tkList">要扫面的票号列表</param>
        /// <param name="action">回调(每个票号的指令状况)</param>
        /// <returns></returns>
        [OperationContract]
        OpenTicketResponse ScanOpenTicket(string ip, string port, string office, List<string> tkList, Action<TicketItem> action = null);
    }
}
