using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using PnrAnalysis.Model;

namespace BPiaoBao.AppServices.Contracts.DomesticTicket
{

    /// <summary>
    /// 查询预订
    /// </summary>
    [ServiceContract]
    public interface IFlightDestineService
    {
        /// <summary>
        /// 单程查航班
        /// </summary>
        /// <param name="formCityCode"></param>
        /// <param name="toCityCode"></param>
        /// <param name="takeDate"></param>
        /// <param name="carrayCode"></param>
        /// <returns></returns>
        [OperationContract]
        FlightResponse QueryOnewayFlight(string formCityCode, string toCityCode, DateTime takeDate, bool IsShare = false, string carrayCode = null);
        /// <summary>
        /// 往返查航班
        /// </summary>
        /// <param name="formCityCode"></param>
        /// <param name="toCityCode"></param>
        /// <param name="takeDate"></param>
        /// <param name="backDate"></param>
        /// <param name="carrayCode"></param>
        /// <returns></returns>
        [OperationContract]
        FlightResponse[] QueryTwowayFlight(string formCityCode, string toCityCode, DateTime takeDate, DateTime backDate, bool IsShare = false, string carrayCode = null);
        /// <summary>
        /// 联程查航班
        /// </summary>
        /// <param name="formCityCode"></param>
        /// <param name="midCityCode"></param>
        /// <param name="toCityCode"></param>
        /// <param name="takeDate"></param>
        /// <param name="midDate"></param>
        /// <param name="carrayCode"></param>
        /// <returns></returns>
        [OperationContract]
        FlightResponse[] QueryConnwayFlight(string formCityCode, string midCityCode, string toCityCode, DateTime takeDate, DateTime midDate, bool IsShare = false, string carrayCode = null);
        /// <summary>
        /// 预订编码 返回PNR内容
        /// </summary>
        /// <param name="destine"></param>
        /// <returns></returns>
        [OperationContract]
        DestineResponse Destine(DataContracts.DomesticTicket.DestineRequest destine);

        /// <summary>
        /// 获取经停信息
        /// </summary>
        /// <param name="businessmanCode">分销商户号</param>
        /// <param name="flightNo">航班号</param>
        /// <param name="flyDate">起飞(经停)日期</param>
        /// <returns></returns>
        [OperationContract]
        LegStop GetLegStop(string businessmanCode, string flightNo, DateTime flyDate);

        /// <summary>
        /// 乘客姓名是否含有生僻字,生僻字请用拼音代替
        /// </summary>
        /// <param name="PassengerName">乘客姓名</param>
        /// <returns></returns>
        [OperationContract]
        RareResponse HasRare(string passengerName);
    }
}
