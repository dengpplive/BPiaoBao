using BPiaoBao.AppServices.Contracts.DomesticTicket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JoveZhao.Framework;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using BPiaoBao.AppServices.SystemSetting;
using StructureMap;
using BPiaoBao.Common.Enums;
using PnrAnalysis.Model;
using PnrAnalysis;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class FlightDestineService : BPiaoBao.AppServices.Contracts.DomesticTicket.IFlightDestineService
    {
        CurrentUserInfo currentUser;
        FlightService flightDestineService;
        IPidService PidService;
        public FlightDestineService(IBusinessmanRepository businessmanRepository, IPidService PidService)
        {
            currentUser = AuthManager.GetCurrentUser();
            flightDestineService = new FlightService(businessmanRepository, currentUser);
            this.PidService = PidService;
        }
        //单程
        [ExtOperationInterceptor("单程查询航班")]
        public DataContracts.DomesticTicket.FlightResponse QueryOnewayFlight(string formCityCode, string toCityCode, DateTime takeDate, bool IsShare = false, string carrayCode = null)
        {
            BehaviorStatService.SaveBehaviorStat(DataContracts.SystemSetting.EnumBehaviorOperate.QueryCount);
            DataContracts.DomesticTicket.FlightResponse flightResponse = null;
            try
            {
                if (currentUser == null)
                    throw new CustomException(111, "用户未登录或者掉线，请重新登陆！");
                flightResponse = flightDestineService.QueryOnewayFlight(currentUser.Code, formCityCode, toCityCode, takeDate, IsShare, carrayCode);
            }
            catch (Exception ex)
            {
                throw new CustomException(111, ex.Message);
            }
            return flightResponse;
        }
        //往返程
        [ExtOperationInterceptor("往返查询航班")]
        public DataContracts.DomesticTicket.FlightResponse[] QueryTwowayFlight(string formCityCode, string toCityCode, DateTime takeDate, DateTime backDate, bool IsShare = false, string carrayCode = null)
        {
            BehaviorStatService.SaveBehaviorStat(DataContracts.SystemSetting.EnumBehaviorOperate.QueryCount);
            DataContracts.DomesticTicket.FlightResponse[] FlightResponseArr = null;
            try
            {
                if (currentUser == null)
                    throw new CustomException(111, "用户未登录或者掉线，请重新登陆！");
                FlightResponseArr = flightDestineService.QueryTwowayFlight(currentUser.Code, formCityCode, toCityCode, takeDate, backDate, IsShare, carrayCode);
            }
            catch (Exception ex)
            {
                throw new CustomException(111, ex.Message);
            }
            return FlightResponseArr;
        }


        //联程
        [ExtOperationInterceptor("联程查询航班")]
        public DataContracts.DomesticTicket.FlightResponse[] QueryConnwayFlight(string formCityCode, string midCityCode, string toCityCode, DateTime takeDate, DateTime midDate, bool IsShare = false, string carrayCode = null)
        {
            BehaviorStatService.SaveBehaviorStat(DataContracts.SystemSetting.EnumBehaviorOperate.QueryCount);
            DataContracts.DomesticTicket.FlightResponse[] FlightResponseArr = null;
            try
            {
                if (currentUser == null)
                    throw new CustomException(111, "用户未登录或者掉线，请重新登陆！");
                FlightResponseArr = flightDestineService.QueryConnwayFlight(currentUser.Code, formCityCode, midCityCode, toCityCode, takeDate, midDate, IsShare, carrayCode);
            }
            catch (Exception ex)
            {
                throw new CustomException(111, ex.Message);
            }
            return FlightResponseArr;
        }
        //预订编码 返回PNR内容PAT 内容
        public DestineResponse Destine(DataContracts.DomesticTicket.DestineRequest destine)
        {
            DestineResponse response = new DestineResponse();
            try
            {
                if (currentUser == null)
                    throw new CustomException(111, "用户未登录或者掉线，请重新登陆！");
                response = flightDestineService.Destine(currentUser.Code, destine);
            }
            catch (Exception ex)
            {
                throw new CustomException(111, ex.Message);
            }
            return response;
        }

        /// <summary>
        /// 获取经停信息
        /// </summary>
        /// <param name="businessmanCode">分销商户号</param>
        /// <param name="flightNo">航班号  CA4193</param>
        /// <param name="flyDate">起飞(经停)日期</param>
        /// <returns></returns>
        public LegStop GetLegStop(string businessmanCode, string flightNo, DateTime flyDate)
        {
            //FF:CA4193/30JUL14
            var cmd = string.Format("FF:{0}/{1}", flightNo.Trim(),
                FormatPNR.DateToStr(flyDate.ToString("yyyy-MM-dd"), DataFormat.dayMonthYear));
            var strRecvData = PidService.SendCmd(businessmanCode, cmd, "");
            strRecvData = strRecvData.Replace("^", "\r");
            var result = string.Empty;
            var stopInfo = new FormatPNR().GetStop(strRecvData, out result);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.WriteLog(LogType.ERROR, result);
            };
            return stopInfo;
        }

        public RareResponse HasRare(string PassengerName)
        {
            return flightDestineService.IsRarePidString(PassengerName);
        }

    }
}
