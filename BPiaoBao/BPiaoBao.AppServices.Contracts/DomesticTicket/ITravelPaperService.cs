using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.Contracts.DomesticTicket
{
    [ServiceContract]
    public interface ITravelPaperService
    {
        /// <summary>
        /// 发放行程单
        /// </summary>      
        [OperationContract]
        int AddTravelPaper(string buyerBusinessman,
            string startTripNumber, string endTripNumber, string useOffice, string iataCode,
            string ticketCompanyName, string tripRemark);

        /// <summary>
        /// 查询行程单详情
        /// </summary>     
        /// <returns></returns>
        [OperationContract]
        DataPack<TravelPaperDto> FindTravelPaper(string buyBusinessmanCode, string buyBusinessmanName, string useOffice,
            string startTripNumber, string endTripNumber,
            string startTicketNumber, string endTicketNumber,
            DateTime? startCreateTime, DateTime? endCreateTime,
            DateTime? startVoidTime, DateTime? endVoidTime,
            DateTime? startGrantTime, DateTime? endGrantTime,
            DateTime? startRecoveryTime, DateTime? endRecoveryTime, string PageSource,
            int? tripStatus, int pageIndex, int pageSize, bool isPager = true, int?[] tripStatuss = null,string OrderId = ""
            );
        /// <summary>
        /// 查询行程单发放记录
        /// </summary>       
        /// <returns></returns>
        [OperationContract]
        DataPack<TravelGrantRecordDto> FindTravelRecord(string useBusinessmanCode, string office, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSize, bool isPager = true);

        /// <summary>
        /// 查询行程单统计数据
        /// </summary>      
        /// <returns></returns>
        [OperationContract]
        TravelPaperStaticsDto FindTravelPaperStatistics(string buyBusinessmanCode, string buyBusinessmanName);

        /// <summary>
        /// 回收空白行程单
        /// </summary>      
        [OperationContract]
        int RecoveryBlackTravelPaper(List<int> travelIdList);
        /// <summary>
        /// 回收作废行程单
        /// </summary>      
        [OperationContract]
        int RecoveryVoidTravelPaper(List<int> travelIdList);

        /// <summary>
        /// 发放空白行程单
        /// </summary>      
        [OperationContract]
        int GrantBlankRecoveryTravelPaper(
           string useBusinessman, string useOffice, string iataCode,
           string ticketCompanyName, string sripRemark, List<int> selectIds);

        /// <summary>
        /// 批量修改Office
        /// </summary>       
        [OperationContract]
        int UpdateOffice(string useOffice, List<int> selectIds);

        /// <summary>
        /// 查询行程单号详情
        /// </summary>       
        [OperationContract]
        TravelPaperDto QueryTripNumberDetail(string tripNumber);

        /// <summary>
        /// 查询可用行程单号
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataPack<TravelPaperDto> FindUseTravelPaperDto(string buyBusinessmanCode);

        /// <summary>
        /// 创建行程单
        /// </summary>
        /// <param name="req"></param>
        [OperationContract]
        TravelAppResponse CreateTrip(TravelAppRequrst req);
        /// <summary>
        /// 作作废行程单
        /// </summary>
        /// <param name="req"></param>
        [OperationContract]
        TravelAppResponse VoidTrip(TravelAppRequrst req);

        /// <summary>
        /// 批量修改行程单数据
        /// </summary>      
        /// <returns></returns>
        [OperationContract]
        bool UpdateTripNumberInfo(List<string> tripNumberList, string ticketNumber, EnumTripStatus tripStatus, string useOffice, string iataCode, string ticketCompanyName);
    }
}
