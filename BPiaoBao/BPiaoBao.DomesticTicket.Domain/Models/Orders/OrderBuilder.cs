using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.DDD;
using PnrAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using BPiaoBao.DomesticTicket.Domain.Services;
using PnrAnalysis.Model;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class OrderBuilder : IAggregationBuilder
    {
        //PNR资源
        PnrResource pnrResource = new PnrResource();
        GetFlightBasicData baseQuery = new GetFlightBasicData();
        private string GetOrderId(string prxFix)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long l = BitConverter.ToInt64(buffer, 0);
            return prxFix + l.ToString();
        }
        /// <summary>
        /// 处理:航站楼 
        /// </summary>
        ///  <param name="city">城市</param>
        /// <param name="airName">二字码</param>
        ///  <param name="Ttime">航站楼</param>
        /// <returns></returns>
        private string getHZL(string city, string airName, string Ttime)
        {
            string msg = "";
            try
            {
                #region MyRegion

                if (!string.IsNullOrEmpty(city) && city.Contains("CKG"))
                {
                    if (!string.IsNullOrEmpty(airName))
                    {
                        airName = airName.ToUpper();

                        if (airName.Contains("CA") || airName.Contains("ZH") || airName.Contains("SC") ||
                            airName.Contains("TV") || airName.Contains("HU") || airName.Contains("PN") ||
                            airName.Contains("MF") || airName.Contains("JD") || airName.Contains("8L") ||
                            airName.Contains("KY") || airName.Contains("HO") || airName.Contains("JR") ||
                             airName.Contains("GS") || airName.Contains("GJ") || airName.Contains("G5") ||
                             airName.Contains("BK") || airName.Contains("NS"))
                        {
                            //T2B:
                            //CA（国航）、ZH（深航）、SC（山东航）、TV（西藏航）、HU（海航）、PN（西部航）、MF（厦门航）、JD（首都航）、8L（祥鹏航）、KY（昆明航）、
                            //HO（吉祥航）、JR（幸福航）、GS（天津航）、GJ（长龙航）、G5（华夏航）、BK（奥凯航）、NS（河北航）
                            msg = "T2B";
                        }
                        else if (airName.Contains("3U") || airName.Contains("EU") || airName.Contains("CZ") ||
                            airName.Contains("MU") || airName.Contains("FM") || airName.Contains("KN"))
                        {
                            //T2A：
                            //3U（四川航）、EU（成都航）、CZ（南方航）、MU（东方航）、FM（上航）、KN（联合航）
                            msg = "T2A";
                        }
                    }
                }
                else
                {
                    msg = Ttime;
                }

                #endregion
            }
            catch (Exception)
            {

            }
            return msg;
        }
        public bool CompareSky(List<SkyWay> adultSkyList, DestineSkyWayRequest[] SkyWay, string OldOrderId)
        {
            List<SkyWay> chdSkyList = new List<SkyWay>();
            if (SkyWay != null && SkyWay.Length > 0)
            {
                foreach (DestineSkyWayRequest item in SkyWay)
                {
                    chdSkyList.Add(new SkyWay()
                    {
                        CarrayCode = item.CarrayCode,
                        FlightNumber = item.FlightNumber,
                        StartDateTime = item.StartDate,
                        ToDateTime = item.EndDate,
                        FromCityCode = item.FromCityCode,
                        ToCityCode = item.ToCityCode,
                        Seat = item.Seat
                    });
                }
                //排序
                chdSkyList = chdSkyList.OrderBy(p => p.StartDateTime).ToList();
            }
            return CompareSky(adultSkyList, chdSkyList, OldOrderId);
        }
        /// <summary>
        /// 比较航段不一致  true 不一致 false 一致
        /// </summary>      
        /// <returns></returns>
        public bool CompareSky(List<SkyWay> adultSkyList, List<SkyWay> chdSkyList, string OldOrderId, string source = "")
        {
            if (adultSkyList == null || adultSkyList.Count == 0)
            {
                if (string.IsNullOrEmpty(source))
                    throw new OrderCommException("关联的成人订单号(" + OldOrderId + ")航段数据不全，请检查！");
                else
                    throw new OrderCommException("关联的订单号(" + OldOrderId + ")航段数据不全，请检查！");
            }
            if (chdSkyList == null || chdSkyList.Count == 0)
            {
                if (string.IsNullOrEmpty(source))
                    throw new OrderCommException("儿童航段信息缺失,请检查！");
                else
                    throw new OrderCommException("航段信息缺失,请检查！");
            }
            if (chdSkyList.Count > adultSkyList.Count)
            {
                if (string.IsNullOrEmpty(source))
                    throw new OrderCommException("儿童航段信息超出关联的成人航段信息,不能导入！");
                else
                    throw new OrderCommException("航段信息超出关联的航段信息,不能导入！");
            }

            bool skyIsNoSame = false;
            //单程 true相同  false不相同  
            bool IsOneSame = false;
            //循环成人航段
            for (int i = 0; i < adultSkyList.Count; i++)
            {
                if (chdSkyList.Count == 1)
                {
                    bool IsPass = string.IsNullOrEmpty(source) ?
                        (chdSkyList[0].FromCityCode.ToUpper() == adultSkyList[i].FromCityCode.ToUpper()
                        && chdSkyList[0].ToCityCode.ToUpper() == adultSkyList[i].ToCityCode.ToUpper()
                        && chdSkyList[0].CarrayCode.ToUpper() == adultSkyList[i].CarrayCode.ToUpper()
                        && chdSkyList[0].FlightNumber.ToUpper() == adultSkyList[i].FlightNumber.ToUpper()
                        && chdSkyList[0].StartDateTime == adultSkyList[i].StartDateTime
                        && chdSkyList[0].ToDateTime == adultSkyList[i].ToDateTime)
                        : (
                        chdSkyList[0].FromCityCode.ToUpper() == adultSkyList[i].FromCityCode.ToUpper()
                        && chdSkyList[0].ToCityCode.ToUpper() == adultSkyList[i].ToCityCode.ToUpper()
                        && chdSkyList[0].CarrayCode.ToUpper() == adultSkyList[i].CarrayCode.ToUpper()
                        );
                    //比较单程  单程中只要有一程匹配即可
                    if (IsPass)
                    {
                        IsOneSame = true;
                        break;
                    }
                }
                else
                {
                    if (chdSkyList.Count <= (i + 1))
                    {
                        bool IsPass = string.IsNullOrEmpty(source) ?
                            (!(chdSkyList[i].FromCityCode.ToUpper() == adultSkyList[i].FromCityCode.ToUpper()
                           && chdSkyList[i].ToCityCode.ToUpper() == adultSkyList[i].ToCityCode.ToUpper()
                           && chdSkyList[i].CarrayCode.ToUpper() == adultSkyList[i].CarrayCode.ToUpper()
                           && chdSkyList[i].FlightNumber.ToUpper() == adultSkyList[i].FlightNumber.ToUpper()
                           && chdSkyList[i].StartDateTime == adultSkyList[i].StartDateTime
                           && chdSkyList[i].ToDateTime == adultSkyList[i].ToDateTime
                           )) : (!(chdSkyList[i].FromCityCode.ToUpper() == adultSkyList[i].FromCityCode.ToUpper()
                           && chdSkyList[i].ToCityCode.ToUpper() == adultSkyList[i].ToCityCode.ToUpper()
                           && chdSkyList[i].CarrayCode.ToUpper() == adultSkyList[i].CarrayCode.ToUpper()
                           ));
                        //比较多程                          
                        if (IsPass)
                        {
                            skyIsNoSame = true;
                            break;
                        }
                    }
                }
            }//endfor
            if ((chdSkyList.Count == 1))
            {
                if (!IsOneSame)
                {
                    skyIsNoSame = true;
                }
                else
                {
                    skyIsNoSame = false;
                }
            }
            return skyIsNoSame;
        }

        public Order CreateOrderByPnrContent(Order order, string pnrContent, EnumPnrImportType PnrImportType, CreateOrderParam OrderParam, string businessmanCode, string CarrierCode, string businessmanName, string account, bool IsChangePnrTicket)
        {
            #region 验证PNR
            if (pnrContent.ToUpper().Contains("NO PNR"))
                pnrContent = pnrContent.ToUpper().Replace("NO PNR", "");
            FormatPNR format = new FormatPNR();
            if (format.GetPnrContentByteLength(pnrContent) > 2000)
                throw new PnrAnalysisFailException("RT和PAT内容长度超出范围[0-2000]个字符,请去掉多余的空格，再导入！");
            if (pnrContent.Trim().Contains("授权"))
                throw new PnrAnalysisFailException(pnrContent);
            if (pnrContent.Trim().Contains("数据包不完整"))
                throw new PnrAnalysisFailException(pnrContent);


            string Msg = "";
            PnrModel pnrMode = null;
            PatModel patMode = null;
            PatModel infPatMode = null;
            string Pnr = string.Empty;
            PnrAnalysis.Model.SplitPnrCon splitPnrCon = null;
            //PNR内容导入
            if (OrderParam == null)
            {
                splitPnrCon = format.GetSplitPnrCon(pnrContent);
                string RTCon = splitPnrCon.RTCon;
                string PatCon = splitPnrCon.AdultPATCon != string.Empty ? splitPnrCon.AdultPATCon : splitPnrCon.ChdPATCon;

                if (string.IsNullOrEmpty(PatCon) || string.IsNullOrEmpty(RTCon) || !PatCon.Contains("PAT:A"))
                    throw new PnrAnalysisFailException("RT和PAT内容不能为空！");
                //去重复和备注
                string pnrRemark = string.Empty;
                RTCon = format.DelRepeatRTCon(RTCon, ref pnrRemark);
                if (!string.IsNullOrEmpty(pnrRemark))
                {
                    PatCon = PatCon.Replace(pnrRemark, "");
                }
                pnrContent = RTCon + "\r\n" + PatCon;
                Pnr = format.GetPNR(RTCon, out Msg);
                if (string.IsNullOrEmpty(Pnr) || !format.IsPnr(Pnr))
                    throw new PnrAnalysisFailException("PNR内容提取编码失败,请检查编码内容！");

                pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                    throw new PnrAnalysisFailException("PNR内容解析航段信息失败,请检查编码内容");

                //成人或者儿童PAT
                patMode = format.GetPATInfo(PatCon, out Msg);

                if (PnrImportType == EnumPnrImportType.PnrContentImport)
                {
                    if (pnrMode._PasType == "2")
                    {
                        throw new PnrAnalysisFailException("该编码【" + Pnr + "】为儿童编码，请选择“儿童编码”通道导入!");
                    }
                }
                else if (PnrImportType == EnumPnrImportType.GenericPnrImport)
                {
                    if (pnrMode._PasType == "2")
                    {
                        throw new PnrAnalysisFailException("该编码【" + Pnr + "】为儿童编码，请选择“儿童编码”通道导入!");
                    }
                }
            }
            else
            {
                PnrData pnrData = OrderParam.pnrData;
                if (pnrData == null)
                {
                    if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                        throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：预定内容有误！");
                    else
                        throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：PNR内容解析失败！");
                }
                //订座失败，请重新预定！失败原因：预定内容有误！
                //导入失败，请重新导入！失败原因：[PnrData]PNR内容解析失败,PnrData Fail！

                pnrMode = pnrData.PnrMode;
                patMode = pnrData.PatMode;
                infPatMode = pnrData.InfPatMode;
                Pnr = order.OrderType == 0 ? pnrData.AdultPnr : pnrData.ChdPnr;

                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                        throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：预定航段内容有误！");
                    else
                        throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：解析编码航段内容有误！");
                }

                //订座失败，请重新预定！失败原因：预定航段内容有误！
                //导入失败，请重新导入！失败原因：解析编码航段内容有误！
            }

            if (!pnrMode.PnrConIsOver)
            {
                if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                    throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：预定内容有误！");
                else
                    throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：编码内容信息有误，RT编码内容需以Office号结尾,请检查编码内容！");
            }

            //订座失败，请重新预定！失败原因：预定内容有误！
            //导入失败，请重新导入！失败原因：编码内容信息有误，RT编码内容需以Office号结尾,请检查编码内容！

            if (pnrMode.PnrStatus.Trim() == "")
            {
                if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                    throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：解析出编码状态有误！");
                else
                    throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：解析出编码状态有误，请检查编码内容！");
            }
            //订座失败，请重新预定！失败原因：解析出编码状态有误！
            //导入失败，请重新导入！失败原因：解析出编码状态有误，请检查编码内容


            if (pnrMode.PnrStatus.Contains("XX")
                || pnrMode.PnrStatus.Contains("NO")
                || pnrMode.PnrStatus.Contains("HL")
                || pnrMode.PnrStatus.Contains("HN")
                )
            {
                if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                    throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：编码状态有误，状态为：" + pnrMode.PnrStatus + "！");
                else
                    throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：编码状态有误，状态为XX,NO,HL,HN项不能导入！");

                //订座失败，请重新预定！失败原因：编码状态有误，状态为：“+pnrMode.PnrStatus+”！
                //导入失败，请重新导入！失败原因：编码状态有误，状态为XX,NO,HL,HN项不能导入！

            }
            else if (!pnrMode.PassengerNameIsCorrent && pnrMode.ErrorPassengerNameList.Count > 0)//乘机人名字检查
            {
                if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                    throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：乘机人名字（" + string.Join(",", pnrMode.ErrorPassengerNameList.ToArray()) + "）有误,请检查乘客姓名，生僻字用拼音代替！");
                else
                    throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：乘机人名字（" + string.Join(",", pnrMode.ErrorPassengerNameList.ToArray()) + "）有误,请检查乘客姓名，生僻字用拼音代替！");
            }
            //订座失败，请重新预定！失败原因：乘机人名字（" + string.Join(",", pnrMode.ErrorPassengerNameList.ToArray()) + "）有误,请检查乘客姓名，生僻字用拼音代替！");
            //导入失败，请重新导入！失败原因：乘机人名字（" + string.Join(",", pnrMode.ErrorPassengerNameList.ToArray()) + "）有误,请检查乘客姓名，生僻字用拼音代替！");

            else if (!pnrMode.PnrConHasFirstNum)//乘客姓名序号是否已1开始
            {
                if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                    throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：PNR内容中乘客姓名序号项不规范,请检查编码内容！");
                else
                    throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：PNR内容中乘客姓名序号项不规范,请检查编码内容！");
            }
            //订座失败，请重新预定！失败原因：PNR内容中乘客姓名序号项不规范,请检查编码内容！
            //导入失败，请重新导入！失败原因：PNR内容中乘客姓名序号项不规范,请检查编码内容！


            //else if (string.IsNullOrEmpty(pnrMode._BigPnr))
            //    throw new PnrAnalysisFailException("编码[" + Pnr + "]PNR内容中没有大编码,请补齐大编码后导入！");
            //else if (pnrMode.HasExistNoSsr)
            //    throw new PnrAnalysisFailException("编码内容中乘客证件号不全，请补全证件号！");
            //else if (pnrMode.SsrIsRepeat)
            //    throw new PnrAnalysisFailException("编码内容中乘客证件号重复，请检查PNR内容证件号！");
            else if (pnrMode.TravelType > 3)
            {
                if (PnrImportType == EnumPnrImportType.WhiteScreenDestine)
                    throw new PnrAnalysisFailException("订座失败，请重新预定！失败原因：系统暂不支持缺口程和多程航段！");
                else
                    throw new PnrAnalysisFailException("导入失败，请重新导入！失败原因：系统暂不支持缺口程和多程航段！");
            }
            //订座失败，请重新预定！失败原因：系统暂不支持缺口程和多程航段！
            //导入失败，请重新导入！失败原因：系统暂不支持缺口程和多程航段！


            //JD导入 验证
            if (pnrMode._CarryCode.ToUpper().Contains("JD"))
            {
                bool JDIsPass = true;
                //编码中成人 儿童 婴儿 个数
                int adultNum = 0, childNum = 0, YNum = 0;
                foreach (PassengerInfo item in pnrMode._PassengerList)
                {
                    if (item.PassengerType == "1")
                        adultNum++;
                    if (item.PassengerType == "2")
                        childNum++;
                    if (item.PassengerType == "3")
                        YNum++;
                }
                bool b1 = adultNum == 1 && childNum == 0 && YNum == 0;
                bool b2 = adultNum == 0 && childNum == 1 && YNum == 0;
                bool b3 = adultNum == 1 && childNum == 1 && YNum == 0;
                bool b4 = adultNum == 1 && childNum == 1 && YNum == 1;
                bool b5 = adultNum == 1 && childNum == 0 && YNum == 1;
                if (!(b1 || b2 || b3 || b4 || b5))
                {
                    JDIsPass = false;
                }
                if (!JDIsPass)
                    throw new PnrAnalysisFailException("JD航空编码【" + Pnr + "】中只能有一个成人,一个儿童,一个婴儿，请手动处理编码内容!");
            }
            decimal TicketPrice = 0m, INFTicketPrice = 0m;
            decimal SeatPrice = 0m, TaxFare = 0m, RQFare = 0m;
            decimal INFSeatPrice = 0m, INFTaxFare = 0m, INFRQFare = 0m;


            if (patMode == null || patMode.UninuePatList.Count == 0)
                throw new PnrAnalysisFailException("PAT解析价格失败");
            if (patMode.IsOverMaxPrice)
                throw new PnrAnalysisFailException("PAT支付价格超出范围!");
            //默认取低价       
            PatInfo pat = order.IsLowPrice ? patMode.UninuePatList[0] : patMode.UninuePatList[patMode.UninuePatList.Count - 1];
            decimal.TryParse(pat.Price, out TicketPrice);
            decimal.TryParse(pat.Fare, out SeatPrice);
            decimal.TryParse(pat.TAX, out TaxFare);
            decimal.TryParse(pat.RQFare, out RQFare);
            if (SeatPrice <= 0)
                throw new PnrAnalysisFailException("舱位价不能小于等于0");
            if (TicketPrice <= 0)
                throw new PnrAnalysisFailException("PAT总价不能小于等于0");

            //PNR内容导入
            if (OrderParam == null)
            {
                if (pnrMode.HasINF && !string.IsNullOrEmpty(splitPnrCon.INFPATCon.Trim())) //如果有婴儿
                {
                    if (string.IsNullOrEmpty(splitPnrCon.INFPATCon) || !splitPnrCon.INFPATCon.Contains("PAT:A*IN"))
                        throw new PnrAnalysisFailException("编码中含有婴儿，为获取到婴儿的PAT价格信息！");

                    infPatMode = format.GetPATInfo(splitPnrCon.INFPATCon, out Msg);
                    if (infPatMode == null || infPatMode.UninuePatList.Count == 0)
                        throw new PnrAnalysisFailException("婴儿PAT解析价格失败");
                    //默认取低价
                    PatInfo patInf = order.IsLowPrice ? infPatMode.UninuePatList[0] : infPatMode.UninuePatList[infPatMode.UninuePatList.Count - 1];
                    decimal.TryParse(patInf.Price, out INFTicketPrice);
                    decimal.TryParse(patInf.Fare, out INFSeatPrice);
                    decimal.TryParse(patInf.TAX, out INFTaxFare);
                    decimal.TryParse(patInf.RQFare, out INFRQFare);
                    if (INFTicketPrice <= 0)
                        throw new PnrAnalysisFailException("婴儿舱位价不能小于等于0");
                    if (INFTicketPrice <= 0)
                        throw new PnrAnalysisFailException("婴儿PAT总价不能小于等于0");

                    pnrContent = pnrContent + "\r\n" + splitPnrCon.INFPATCon;
                }
            }
            else
            {
                if (pnrMode != null && pnrMode.HasINF && infPatMode != null) //如果有婴儿
                {
                    if (infPatMode == null || infPatMode.UninuePatList.Count == 0)
                        throw new PnrAnalysisFailException("婴儿PAT解析价格失败");
                    //默认取低价
                    PatInfo patInf = order.IsLowPrice ? infPatMode.UninuePatList[0] : infPatMode.UninuePatList[infPatMode.UninuePatList.Count - 1];
                    decimal.TryParse(patInf.Price, out INFTicketPrice);
                    decimal.TryParse(patInf.Fare, out INFSeatPrice);
                    decimal.TryParse(patInf.TAX, out INFTaxFare);
                    decimal.TryParse(patInf.RQFare, out INFRQFare);
                    if (INFTicketPrice <= 0)
                        throw new PnrAnalysisFailException("婴儿舱位价不能小于等于0");
                    if (INFTicketPrice <= 0)
                        throw new PnrAnalysisFailException("婴儿PAT总价不能小于等于0");
                }
            }

            if (TicketPrice <= 0)
                throw new PnrAnalysisFailException("票面总价不能小于等于0");

            #endregion

            //构造数据
            double FlyAdvanceTime = 1;// SettingSection.GetInstances().Cashbag.FlyAdvanceTime;            
            double.TryParse(System.Configuration.ConfigurationManager.AppSettings["FlyAdvanceTime"].ToString(), out FlyAdvanceTime);
            order.PnrCode = Pnr;
            order.BigCode = pnrMode._BigPnr;
            order.OrderType = pnrMode._PasType == "1" ? 0 : 1;
            order.PnrType = pnrMode._PnrType == "1" ? 0 : 1;
            order.Remark = "";
            order.TicketPrice = TicketPrice;
            order.INFTicketPrice = INFTicketPrice;
            order.BusinessmanCode = businessmanCode;
            order.BusinessmanName = businessmanName;
            order.CarrierCode = CarrierCode;
            order.OperatorAccount = account;
            order.CreateTime = DateTime.Now;
            order.PnrContent = pnrContent;
            order.HasAfterSale = false;
            order.YdOffice = pnrMode._Office;
            decimal defaultOrderMoney = 0m;
            bool HaveBabyFlag = false;
            List<SkyWay> SkyWays = new List<SkyWay>();
            List<Passenger> Passengers = new List<Passenger>();
            if (OrderParam == null)
            {
                DateTime tempStartTime = System.DateTime.Now;
                string Seat = string.Empty;
                //内容导入
                foreach (var leg in pnrMode._LegList)
                {
                    Seat = leg.Seat;
                    //如果有子舱位就用子舱位
                    if (!string.IsNullOrEmpty(leg.ChildSeat) && leg.ChildSeat.Trim().Length == 2)
                    {
                        Seat = leg.ChildSeat.Trim();
                    }
                    SkyWays.Add(new SkyWay()
                    {
                        FromCityCode = leg.FromCode,
                        ToCityCode = leg.ToCode,
                        FlightNumber = leg.FlightNum,
                        StartDateTime = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00"),
                        ToDateTime = DateTime.Parse(leg.FlyDateE + " " + leg.FlyEndTime.Insert(2, ":") + ":00"),
                        CarrayCode = leg.AirCode,
                        Seat = Seat,
                        FromTerminal = getHZL(leg.FromCode, leg.AirCode, leg.FromCityT1),
                        ToTerminal = getHZL(leg.ToCode, leg.AirCode, leg.ToCityT2),
                        Discount = baseQuery.GetZK(leg.FromCode, leg.ToCode, leg.AirCode, leg.Seat, SeatPrice)
                    });
                }
                tempStartTime = SkyWays[0].StartDateTime.AddHours(-FlyAdvanceTime);
                if (DateTime.Compare(tempStartTime, System.DateTime.Now) <= 0)
                {
                    //起飞时间小于预定时间  已起飞 已失效 不能导入
                    throw new PnrAnalysisFailException("航班起飞前" + FlyAdvanceTime + "小时内不能导入编码内容！");
                }
                foreach (var pas in pnrMode._PassengerList)
                {
                    if (pas.PassengerType == "3")
                    {
                        Passengers.Add(new Passenger()
                        {
                            PassengerName = pas.PassengerName,
                            CardNo = pas.SsrCardID,
                            SeatPrice = INFSeatPrice,
                            ABFee = INFTaxFare,
                            RQFee = INFRQFare,
                            PassengerType = EnumPassengerType.Baby,
                            TicketStatus = EnumTicketStatus.Unknown,
                            Mobile = pas.PassengerTel,
                        });
                        defaultOrderMoney += INFSeatPrice + INFTaxFare + INFRQFare;
                        HaveBabyFlag = true;
                    }
                    else
                    {
                        Passengers.Add(new Passenger()
                        {
                            PassengerName = pas.PassengerName,
                            CardNo = pas.SsrCardID,
                            SeatPrice = SeatPrice,
                            ABFee = TaxFare,
                            RQFee = RQFare,
                            PassengerType = (EnumPassengerType)pas.PassengerType.ToInt(),
                            TicketStatus = EnumTicketStatus.Unknown,
                            Mobile = pas.PassengerTel
                        });
                        defaultOrderMoney += SeatPrice + TaxFare + RQFare;
                    }
                }
            }
            else
            {
                //是否儿童订单
                bool IsChdOrder = false;
                foreach (PassengerDto pas in OrderParam.PassengerDtos)
                {
                    if (pas.PassengerType == EnumPassengerType.Baby)
                    {
                        Passengers.Add(new Passenger()
                        {
                            PassengerName = pas.PassengerName,
                            CardNo = pas.CardNo,
                            SeatPrice = INFSeatPrice,
                            ABFee = INFTaxFare,
                            RQFee = INFRQFare,
                            PassengerType = EnumPassengerType.Baby,
                            TicketStatus = EnumTicketStatus.Unknown,
                            Mobile = pas.Mobile,
                            IdType = pas.IdType,
                            SexType = pas.SexType,
                            Birth = pas.Birth
                        });
                        defaultOrderMoney += INFSeatPrice + INFTaxFare + INFRQFare;
                        HaveBabyFlag = true;
                    }
                    else
                    {
                        IsChdOrder = pas.PassengerType == EnumPassengerType.Child ? true : false;
                        Passengers.Add(new Passenger()
                        {
                            PassengerName = pas.PassengerName,
                            CardNo = pas.CardNo,
                            SeatPrice = SeatPrice,
                            ABFee = TaxFare,
                            RQFee = RQFare,
                            PassengerType = pas.PassengerType,
                            TicketStatus = EnumTicketStatus.Unknown,
                            Mobile = pas.Mobile,
                            IdType = pas.IdType,
                            SexType = pas.SexType,
                            Birth = pas.Birth
                        });
                        defaultOrderMoney += SeatPrice + TaxFare + RQFare;
                    }
                }

                List<LegInfo> legInfo = pnrMode._LegList;
                //白屏预定                
                int i = 0;
                foreach (SkyWayDto leg in OrderParam.SkyWayDtos)
                {
                    string strSeat = leg.Seat;
                    //处理儿童的舱位问题
                    if (IsChdOrder)
                    {
                        if (!"FCY".Contains(leg.Seat.ToUpper()))
                        {
                            if (legInfo[0].Seat.Trim().ToUpper() != leg.Seat.Trim().ToUpper())
                            {
                                strSeat = "Y";
                            }
                        }
                    }
                    else
                    {
                        //如果有子舱位就用子舱位
                        if (legInfo.Count > i && !string.IsNullOrEmpty(legInfo[i].ChildSeat) && legInfo[i].ChildSeat.Trim().Length == 2)
                        {
                            strSeat = legInfo[i].ChildSeat.Trim();
                        }
                    }
                    SkyWays.Add(new SkyWay()
                    {
                        FromCityCode = leg.FromCityCode,
                        ToCityCode = leg.ToCityCode,
                        FlightNumber = leg.FlightNumber,
                        StartDateTime = leg.StartDateTime,
                        ToDateTime = leg.ToDateTime,
                        CarrayCode = leg.CarrayCode,
                        Seat = strSeat,
                        FromTerminal = leg.FromTerminal,
                        ToTerminal = leg.ToTerminal,
                        Discount = leg.Discount,
                        FlightModel = leg.FlightModel
                    });
                    i++;
                }
            }
            order.SkyWays = SkyWays;
            order.Passengers = Passengers;
            order.OrderMoney = defaultOrderMoney;
            order.HaveBabyFlag = HaveBabyFlag;
            //是否换编码出票
            order.IsChangePnrTicket = IsChangePnrTicket;
            //order.Policy = new Policy();
            order.OrderPay = new OrderPay()
            {
                PaidStatus = EnumPaidStatus.NoPaid,
                PayStatus = EnumPayStatus.NoPay,
                OrderId = order.OrderId
            };
            order.ChangeStatus(EnumOrderStatus.WaitChoosePolicy);
            //订单日志            
            order.WriteLog(new OrderLog()
            {
                OperationDatetime = System.DateTime.Now,
                OperationPerson = account,
                Remark = "",
                OperationContent = "生成订单",
                IsShowLog = true
            });
            return order;
        }
    }
}
