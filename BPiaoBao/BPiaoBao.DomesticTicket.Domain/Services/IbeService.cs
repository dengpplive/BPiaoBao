using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BPiaoBao.DomesticTicket.Domain.Models;
using StructureMap;
namespace BPiaoBao.DomesticTicket.Domain.Services
{
    public class IbeService
    {
        //public List<CabinSeat> MemData = new List<CabinSeat>();
        GetFlightBasicData test = new GetFlightBasicData();
        DataBill dataBill = new DataBill();
        // MangoHelper mgHelper = new MangoHelper();
        CommLog log = new CommLog();
        bool IsOpenShareflght = false;
        decimal DefaultPolicy = 0m;
        decimal DefaultReturnMoney = 0m;
        int useProxyQuery = 0;
        QueryParam QueryParam;
        public IbeService(QueryParam queryParam)
        {
            //用于测试 缓存数据
            // MemData = test.GetCabinSeatList();
            //默认政策 没有匹配到政策的舱位用于默认政策
            string strDefaultPolicy = System.Configuration.ConfigurationManager.AppSettings["DefaultPolicy"];
            string strDefaultReturnMoney = System.Configuration.ConfigurationManager.AppSettings["DefaultReturnMoney"];
            string strUseProxyQuery = System.Configuration.ConfigurationManager.AppSettings["useProxyQuery"];
            decimal.TryParse(strDefaultPolicy, out DefaultPolicy);
            decimal.TryParse(strDefaultReturnMoney, out DefaultReturnMoney);
            int.TryParse(strUseProxyQuery, out useProxyQuery);
            IsOpenShareflght = queryParam.IsShare;
            this.QueryParam = queryParam;
        }
        /// <summary>
        /// 默认政策数据
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        public IbeSeat SetDefaultData(IbeSeat seat)
        {
            try
            {
                seat.PolicyId = Guid.NewGuid().ToString();
                seat.Policy = DefaultPolicy;
                seat.ReturnMoney = DefaultReturnMoney;
                seat.PlatformCode = "默认";
                seat.PolicyRMK = string.Empty;
            }
            catch (Exception ex)
            {
                //记录日志
                log.WriteLog("SetDefaultData", "异常信息:" + ex.Message + "\r\n");
            }
            return seat;
        }
        /// <summary>
        /// 获取IBE数据
        /// </summary>
        /// <param name="FromCode"></param>
        /// <param name="ToCode"></param>
        /// <param name="FlyDate"></param>
        /// <returns></returns>
        public AVHData GetAvh(string FromCode, string ToCode, DateTime FlyDate, string carrayCode)
        {
            AVHData ibeData = new AVHData();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("参数:\r\nFromCode=" + FromCode + "\r\n ToCode=" + ToCode + "\r\n FlyDate=" + FlyDate.ToString("yyyy-MM-dd HH:mm:ss") + (carrayCode != null ? carrayCode : ""));
            try
            {
                ibeData.IbeData = new List<IBERow>();
                ibeData.QueryParam = new QueryParam()
                {
                    FromCode = FromCode,
                    ToCode = ToCode,
                    FlyDate = FlyDate.ToString("yyy-MM-dd"),
                    FlyTime = "00:00:00"
                };
                string strData = string.Empty;
                if (useProxyQuery == 0)
                {
                    IBEService.WebService1SoapClient ibe = new IBEService.WebService1SoapClient();
                    strData = ibe.getIBEAVData(ibeData.QueryParam.FromCode, ibeData.QueryParam.ToCode, ibeData.QueryParam.FlyDate, ibeData.QueryParam.FlyTime);
                }
                else if (useProxyQuery == 1)
                {
                    FlightService flightService = ObjectFactory.GetInstance<FlightService>();
                    strData = flightService.GetAV(this.QueryParam.Code, ibeData.QueryParam.FromCode, ibeData.QueryParam.ToCode, carrayCode, ibeData.QueryParam.FlyDate, "0000");
                }
                ibeData.AVHString = strData;
                if (!strData.Contains("NoRoutingException"))
                {
                    string[] IbeArray = strData.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                    if (IbeArray != null && IbeArray.Length > 0)
                    {
                        string[] strRow = null;
                        foreach (string row in IbeArray)
                        {
                            strRow = row.ToUpper().Split(',');
                            if (strRow.Length >= 15)
                            {
                                IBERow ibeRow = new IBERow();
                                try
                                {
                                    //检测日期格式是否正确
                                    DateTime.Parse(strRow[0]);
                                }
                                catch
                                {
                                    strRow[0] = ibeData.QueryParam.FlyDate;
                                }
                                ibeRow.FlyDate = strRow[0];
                                ibeRow.StartTime = strRow[1];
                                ibeRow.EndTime = strRow[2];
                                ibeRow.CarrierCode = strRow[3].Trim(new char[] { '*' });
                                //过滤航空公司
                                if (!string.IsNullOrEmpty(carrayCode))
                                {
                                    if (ibeRow.CarrierCode != carrayCode.ToUpper())
                                    {
                                        continue;
                                    }
                                }
                                ibeRow.FlightNo = strRow[4].Trim(new char[] { '*' });
                                if (!string.IsNullOrEmpty(strRow[5]) && strRow[5].Split('*')[0].Trim() != "")
                                {
                                    string strSeat = strRow[5].Split('*')[0].Trim();
                                    string pattern = "((?<Seat>[0-9A-Z])(?<SeatSymbol>[0-9A-Z]))";
                                    MatchCollection mchs = Regex.Matches(strSeat, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                    ibeRow.IBESeat = new List<IbeSeat>();
                                    foreach (Match item in mchs)
                                    {
                                        if (item.Success)
                                        {
                                            IbeSeat ibeseat = new IbeSeat();
                                            ibeseat.Seat = item.Groups["Seat"].Value.Trim();
                                            ibeseat.SeatSymbol = item.Groups["SeatSymbol"].Value.Trim();
                                            if (ibeseat.SeatSymbol == "A")
                                            {
                                                ibeseat.SeatCount = ">9";
                                            }
                                            else if (char.IsNumber(ibeseat.SeatSymbol[0]))
                                            {
                                                ibeseat.SeatCount = ibeseat.SeatSymbol;
                                            }
                                            else
                                            {
                                                ibeseat.SeatCount = "0";
                                            }
                                            //过滤没有位置的舱位
                                            int mSeatCount = 0;
                                            int.TryParse(ibeseat.SeatCount.Replace(">", ""), out mSeatCount);
                                            if (mSeatCount > 0)
                                            {
                                                ibeseat = SetDefaultData(ibeseat);
                                                //去重
                                                if (!ibeRow.IBESeat.Exists(p => p.Seat == ibeseat.Seat))
                                                    ibeRow.IBESeat.Add(ibeseat);
                                            }
                                        }
                                    }
                                }
                                ibeRow.FromCode = strRow[6];
                                ibeRow.ToCode = strRow[7];
                                ibeRow.AirModel = strRow[8];
                                ibeRow.IsStop = strRow[9].Trim() == "1" ? true : false;
                                ibeRow.IsMeals = strRow[10].Trim() == "1" ? true : false;
                                ibeRow.IsElectronicTicket = strRow[11].Trim() == "1" ? true : false;
                                ibeRow.IsShareFlight = (strRow[12].Trim().ToLower() == "1" || strRow[12].Trim().ToLower() == "true") ? true : false;
                                //处理子舱位
                                ibeRow.ChildSeat = strRow[13].Trim();
                                if (!string.IsNullOrEmpty(ibeRow.ChildSeat))
                                {
                                    string pattern = "((?<Seat>[0-9A-Z]1)(?<SeatSymbol>[0-9A-Z]))";
                                    MatchCollection mchs = Regex.Matches(ibeRow.ChildSeat, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                                    if (ibeRow.IBESeat == null) ibeRow.IBESeat = new List<IbeSeat>();
                                    foreach (Match item in mchs)
                                    {
                                        if (item.Success)
                                        {
                                            IbeSeat ibeseat = new IbeSeat();
                                            ibeseat.Seat = item.Groups["Seat"].Value.Trim();
                                            ibeseat.SeatSymbol = item.Groups["SeatSymbol"].Value.Trim();
                                            if (ibeseat.SeatSymbol == "A")
                                            {
                                                ibeseat.SeatCount = ">9";
                                            }
                                            else if (char.IsNumber(ibeseat.SeatSymbol[0]))
                                            {
                                                ibeseat.SeatCount = ibeseat.SeatSymbol;
                                            }
                                            else
                                            {
                                                ibeseat.SeatCount = "0";
                                            }
                                            //过滤没有位置的舱位
                                            int mSeatCount = 0;
                                            int.TryParse(ibeseat.SeatCount.Replace(">", ""), out mSeatCount);
                                            if (mSeatCount > 0)
                                            {
                                                ibeseat = SetDefaultData(ibeseat);
                                                //去重
                                                if (!ibeRow.IBESeat.Exists(p => p.Seat == ibeseat.Seat))
                                                    ibeRow.IBESeat.Add(ibeseat);
                                            }
                                        }
                                    }
                                }
                                string strFromCityT1 = strRow[14].Trim();
                                if (strFromCityT1.Length == 2)
                                {
                                    strFromCityT1 = strFromCityT1.Substring(0, 1);
                                }
                                else if (strFromCityT1.Length == 3)
                                {
                                    if (strFromCityT1.StartsWith("D"))
                                    {
                                        strFromCityT1 = strFromCityT1.Substring(0, 1);
                                    }
                                    else
                                    {
                                        strFromCityT1 = strFromCityT1.Substring(0, 2);
                                    }
                                }
                                else
                                {
                                    if ((strFromCityT1.Length >= 2))
                                    {
                                        strFromCityT1 = strFromCityT1.Substring(0, 2);
                                    }
                                }
                                ibeRow.FromCityT1 = getHZL(ibeRow.FromCode, ibeRow.CarrierCode, strFromCityT1);

                                string strToCityT1 = strRow[14].Trim();
                                if (strToCityT1.Length == 2)
                                {
                                    strToCityT1 = strToCityT1.Substring(1, 1);
                                }
                                else if (strToCityT1.Length == 3)
                                {
                                    if (strToCityT1.StartsWith("D"))
                                    {
                                        strToCityT1 = strToCityT1.Substring(1, 2);
                                    }
                                    else
                                    {
                                        strToCityT1 = strToCityT1.Substring(2, 1);
                                    }
                                }
                                else
                                {
                                    if ((strToCityT1.Length >= 4))
                                    {
                                        strToCityT1 = strToCityT1.Substring(2, 2);
                                    }
                                }
                                ibeRow.ToCityT1 = getHZL(ibeRow.ToCode, ibeRow.CarrierCode, strToCityT1);

                                if (ibeRow.IsShareFlight)
                                {
                                    if (IsOpenShareflght)//过滤共享航班
                                    {
                                        //过去完后添加集合
                                        ibeData.IbeData.Add(ibeRow);
                                    }
                                }
                                else
                                {
                                    //添加集合
                                    ibeData.IbeData.Add(ibeRow);
                                }
                            }
                        }
                    }
                }
                else
                {
                    sbLog.Append("IBE异常信息:" + strData + "\r\n");
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
                //记录日志
                log.WriteLog("GetAvh", sbLog.ToString());
            }
            return ibeData;
        }

        /// <summary>
        /// 获取FD数据 包括数据的补全
        /// </summary>
        /// <param name="FromCode"></param>
        /// <param name="ToCode"></param>
        /// <param name="FlyDate"></param>
        /// <returns></returns>
        public FDData GetFD(string FromCode, string ToCode, DateTime FlyDate, string carrayCode)
        {
            FDData fd = new FDData();
            List<string> execSQL = new List<string>();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("参数:\r\nFromCode=" + FromCode + "\r\n ToCode=" + ToCode + "\r\n FlyDate=" + FlyDate.ToString("yyyy-MM-dd HH:mm:ss") + (carrayCode != null ? carrayCode : ""));
            try
            {
                fd.QueryParam = new QueryParam()
                {
                    FromCode = FromCode,
                    ToCode = ToCode,
                    FlyDate = FlyDate.ToString("yyy-MM-dd"),
                    FlyTime = "00:00:00"
                };
                fd.FdRow = new List<FdRow>();

                //所有的baseCabin
                List<BaseCabin> allBaseCabin = test.GetBaseCabin("");

                IBEService.WebService1SoapClient ibe = new IBEService.WebService1SoapClient();
                DataSet dsIBE = ibe.getIBEFDData(fd.QueryParam.FromCode, fd.QueryParam.ToCode, fd.QueryParam.FlyDate, fd.QueryParam.FlyTime);
                #region 数据条数
                int FltShopAVJourneyCount = dsIBE.Tables["FltShopAVJourney"].Rows.Count;
                int FltShopAVOptCount = dsIBE.Tables["FltShopAVOpt"].Rows.Count;
                int CabinAllCount = dsIBE.Tables["CabinAll"].Rows.Count;
                int TaxAllCount = dsIBE.Tables["TaxAll"].Rows.Count;
                int FltShopAVFltCount = dsIBE.Tables["FltShopAVFlt"].Rows.Count;
                int FltShopPSCount = dsIBE.Tables["FltShopPS"].Rows.Count;
                int RouteAllCount = dsIBE.Tables["RouteAll"].Rows.Count;
                int FltShopNFareCount = dsIBE.Tables["FltShopNFare"].Rows.Count;
                int FltShopPFareCount = dsIBE.Tables["FltShopPFare"].Rows.Count;
                int FltShopRuleCount = dsIBE.Tables["FltShopRule"].Rows.Count;

                List<string> AirCodeList = new List<string>();
                List<AirCarrier> m_AirCideList = test.GetAirCarrier("");
                //..从数据库中读取航空公司                        
                m_AirCideList.ForEach(p =>
                 {
                     if (!AirCodeList.Contains(p.Code))
                     {
                         AirCodeList.Add(p.Code);
                     }
                 });
                //从FD中读取航空公司记入列表中
                if (FltShopAVFltCount > 0)
                {
                    for (int i = 0; i < FltShopAVFltCount; i++)
                    {
                        string tempC = dsIBE.Tables["FltShopAVFlt"].Rows[i]["A1"].ToString().ToUpper();
                        if (!AirCodeList.Contains(tempC))
                        {
                            AirCodeList.Add(tempC);
                        }
                    }
                }
                #endregion


                //航线
                // var result = // mgHelper.All<CabinSeat>().Where(p => p.Airline.FromCode == FromCode && p.Airline.ToCode == ToCode);
                List<CabinSeat> CabinSeatList = test.GetCabinSeatList(string.Format(" FromCode='{0}' and  ToCode='{1}' ", FromCode, ToCode));// result.ToList();
                #region 获取燃油
                decimal ranyou = 0m;
                for (int i = 0; i < TaxAllCount; i++)
                {
                    if (dsIBE.Tables["TaxAll"].Rows[i]["A1"].ToString().ToUpper() == "YQ")
                    {
                        ranyou = decimal.Parse((dsIBE.Tables["TaxAll"].Rows[i]["A2"] == DBNull.Value || dsIBE.Tables["TaxAll"].Rows[i]["A2"].ToString() == "") ? "0" : dsIBE.Tables["TaxAll"].Rows[i]["A2"].ToString());
                        if (ranyou > 0)
                        {
                            break;
                        }
                    }
                }
                #endregion
                #region 获取舱价格
                //获取这条行线里程有没有不为0的
                CabinSeat cabinSeat = CabinSeatList.Where(p => p.Airline.Mileage != 0).FirstOrDefault();
                Airline airline = cabinSeat != null ? cabinSeat.Airline : null;
                //各个航空公司的这条航线的Y舱数据
                Dictionary<string, FdRow> dicY = new Dictionary<string, FdRow>();
                for (int i = 0; i < FltShopPFareCount; i++)
                {
                    FdRow fdr = new FdRow();
                    fdr.Seat = dsIBE.Tables["FltShopPFare"].Rows[i]["A2"].ToString().ToUpper();
                    fdr.CarrierCode = dsIBE.Tables["FltShopPFare"].Rows[i]["A1"].ToString().ToUpper();
                    //过滤航空公司
                    if (!string.IsNullOrEmpty(carrayCode))
                    {
                        if (fdr.CarrierCode != carrayCode.ToUpper())
                        {
                            continue;
                        }
                    }
                    //过滤完后添加到集合
                    fd.FdRow.Add(fdr);
                    //获取舱位价格
                    if (fdr.Seat.Trim().Length == 1 || fdr.Seat.Trim().Length == 2)
                    {
                        decimal _SeatPrice = 0m;
                        if (decimal.TryParse(dsIBE.Tables["FltShopPFare"].Rows[i]["A3"].ToString(), out _SeatPrice))
                        {
                            fdr.SeatPrice = _SeatPrice;
                        }
                    }
                    fdr.Mileage = (airline == null || airline.Mileage == 0) ? (int)fdr.SeatPrice : airline.Mileage;
                    if (ranyou <= 0)
                    {
                        if (cabinSeat != null)
                        {
                            fdr.ADultFuleFee = cabinSeat.Fuel.AdultFuelFee;
                            fdr.ChildFuleFee = cabinSeat.Fuel.ChildFuelFee;
                        }
                    }
                    else
                    {
                        fdr.ADultFuleFee = ranyou;
                        fdr.ChildFuleFee = ranyou;
                        //List<CabinSeat> FuelList = CabinSeatList.Where(p => p.CarrierCode == fdr.CarrierCode).ToList();
                        //List<string> idsList = new List<string>();
                        //for (int j = 0; j < FuelList.Count; j++)
                        //{
                        //    idsList.Add("'" + FuelList[j]._id + "'");
                        //}
                        //if (idsList.Count > 0)
                        //{
                        //    execSQL.Add(string.Format("update CabinSeat set AdultFuelFee={0},ChildFuelFee={1} where _id in({2}) ", ranyou, ranyou, string.Join(",", idsList.ToArray())));
                        //}
                    }
                    if (fdr.Seat == "Y")
                    {
                        if (!dicY.ContainsKey(fdr.CarrierCode))
                        {
                            dicY.Add(fdr.CarrierCode, fdr);
                        }
                    }
                }
                fd.YFdRow = dicY;
                //循环承运人
                foreach (string item in AirCodeList)
                {
                    if (!dicY.ContainsKey(item))
                    {
                        //没有Y舱数据信息的航空公司 需要补全
                        CabinSeat lineResult = CabinSeatList.Where(p => p.CarrierCode == item).FirstOrDefault();
                        FdRow fdr = new FdRow()
                            {
                                ADultFuleFee = ranyou == 0 && lineResult != null ? lineResult.Fuel.AdultFuelFee : ranyou,
                                ChildFuleFee = ranyou == 0 && lineResult != null ? lineResult.Fuel.ChildFuelFee : ranyou,
                                CarrierCode = item,
                                Mileage = lineResult != null ? lineResult.Airline.Mileage : 0,
                                Rebate = 100,
                                SeatPrice = lineResult != null ? lineResult.Airline.SeatPrice : 0,
                                TaxFee = 0m,
                                Seat = "Y"
                            };
                        dicY.Add(item, fdr);
                        fd.FdRow.Add(fdr);
                    }
                    else
                    {
                        //更新Y舱价格和里程数据    
                        //List<CabinSeat> YMList = CabinSeatList.Where(p => p.CarrierCode == item).ToList();
                        //List<string> idslIst = new List<string>();
                        //for (int i = 0; i < YMList.Count; i++)
                        //{
                        //    idslIst.Add("'" + YMList[i]._id + "'");
                        //}
                        //if (idslIst.Count > 0)
                        //{
                        //    execSQL.Add(string.Format("update CabinSeat set SeatPrice={0},Mileage={1} where _id in({2}) ", dicY[item].SeatPrice, dicY[item].Mileage, string.Join(",", idslIst.ToArray())));
                        //}
                    }
                }
                //检查漏掉的舱位
                foreach (string aircode in AirCodeList)
                {
                    if (dicY.ContainsKey(aircode))
                    {
                        CabinSeat lineResult = CabinSeatList.Where(p => p.CarrierCode == aircode).FirstOrDefault();
                        if (lineResult != null)
                        {
                            lineResult.SeatList.ForEach(p =>
                            {
                                //不存在就添加
                                FdRow frw = fd.FdRow.Where(p1 => p1.CarrierCode == lineResult.CarrierCode && p1.Seat == p.Code).FirstOrDefault();
                                if (frw == null)
                                {
                                    fd.FdRow.Add(new FdRow()
                                    {
                                        ADultFuleFee = lineResult.Fuel.AdultFuelFee,
                                        ChildFuleFee = lineResult.Fuel.ChildFuelFee,
                                        CarrierCode = aircode,
                                        Mileage = lineResult != null ? lineResult.Airline.Mileage : 0,
                                        SeatPrice = 0,
                                        Seat = p.Code
                                    });
                                }
                            });
                        }
                    }
                }

                //IBE没有获取到价格的 用基本数据计算
                //List<FdRow> nfdr = fd.FdRow.Where(p => p.SeatPrice == 0m).ToList();
                //for (int i = 0; i < nfdr.Count; i++)
                //{
                //    FdRow fdr = nfdr[i];
                //    if (fdr.SeatPrice == 0 && dicY.ContainsKey(fdr.CarrierCode))
                //    {
                //        var seatResult = CabinSeatList.Where(p => p.CarrierCode == fdr.CarrierCode).FirstOrDefault();
                //        Seat seat = seatResult != null ? seatResult.SeatList.Where(p => p.Code == fdr.Seat).FirstOrDefault() : null;
                //        if (seat != null)
                //        {
                //            fdr.Rebate = seat.Rebate;
                //            fdr.SeatPrice = dataBill.CeilTen((seat.Rebate / 100) * dicY[fdr.CarrierCode].SeatPrice);
                //        }
                //        else
                //        {
                //            //去基础表查询
                //            List<BaseCabin> baseCabinList = allBaseCabin.Where(m => m.CarrierCode == fdr.CarrierCode && m.Code == fdr.Seat).ToList();
                //            if (baseCabinList != null && baseCabinList.Count > 0)
                //            {
                //                BaseCabin baseCabin = baseCabinList[0];
                //                fdr.Rebate = baseCabin.Rebate;
                //                fdr.SeatPrice = dataBill.CeilTen((baseCabin.Rebate / 100) * dicY[fdr.CarrierCode].SeatPrice);
                //            }
                //        }
                //    }
                //}

                #endregion

                #region 计算更新折扣
                //fd.FdRow没有的舱位就没有办法了
                Dictionary<string, List<Seat>> dicSeat = new Dictionary<string, List<Seat>>();
                for (int i = 0; i < fd.FdRow.Count; i++)
                {
                    if (dicY.ContainsKey(fd.FdRow[i].CarrierCode))
                    {
                        if (dicY[fd.FdRow[i].CarrierCode].SeatPrice != 0)
                        {
                            //计算折扣
                            fd.FdRow[i].Rebate = GetZk(fd.FdRow[i].SeatPrice, dicY[fd.FdRow[i].CarrierCode].SeatPrice);
                        }
                        if (!dicSeat.ContainsKey(fd.FdRow[i].CarrierCode))
                        {
                            List<Seat> ffdList = new List<Seat>();
                            ffdList.Add(new Seat()
                            {
                                Rebate = fd.FdRow[i].Rebate,
                                Code = fd.FdRow[i].Seat
                            });
                            dicSeat.Add(fd.FdRow[i].CarrierCode, ffdList);
                        }
                        else
                        {
                            dicSeat[fd.FdRow[i].CarrierCode].Add(new Seat()
                            {
                                Rebate = fd.FdRow[i].Rebate,
                                Code = fd.FdRow[i].Seat
                            });
                        }
                    }
                }

                if (dicSeat.Count > 0)
                {
                    foreach (KeyValuePair<string, List<Seat>> item in dicSeat)
                    {
                        List<CabinSeat> List = CabinSeatList.Where(p => p.CarrierCode == item.Key && (p.SeatList == null || p.SeatList.Count == 0)).ToList();
                        for (int i = 0; i < List.Count; i++)
                        {
                            //更新舱位                                             
                            if (item.Value != null && item.Value.Count > 0)
                            {
                                List[i].SeatList = item.Value;
                            }
                            else
                            {
                                List<Seat> sbSeat = new List<Seat>();
                                List<BaseCabin> baseCabinList = allBaseCabin.Where(m => m.CarrierCode == item.Key).ToList();
                                foreach (BaseCabin baseItem in baseCabinList)
                                {
                                    sbSeat.Add(new Seat()
                                    {
                                        Code = baseItem.Code,
                                        Rebate = baseItem.Rebate
                                    });
                                }
                                List[i].SeatList = sbSeat;
                            }
                            StringBuilder sbSeatData = new StringBuilder();
                            foreach (Seat seat in List[i].SeatList)
                            {
                                sbSeatData.Append(seat.ToString() + "|");
                            }
                            //更新舱位                            
                            //execSQL.Add(string.Format("update dbo.CabinSeat set SeatList='{0}' where _id='{1}' and (SeatList='' or SeatList is null)  ", sbSeatData, List[i]._id));
                        }
                    }
                }
                #endregion

                //System.Threading.ThreadPool.QueueUserWorkItem(p =>
                //{
                //    if (execSQL.Count > 0)
                //    {
                //        test.ExecuteSQLList(execSQL);
                //    }
                //});
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
                //记录日志
                log.WriteLog("GetFD", sbLog.ToString());
            }
            return fd;
        }
        /// <summary>
        /// 计算折扣
        /// </summary>
        /// <param name="SeatPrice"></param>
        /// <param name="YSeatPrice"></param>
        /// <returns></returns>
        public decimal GetZk(decimal SeatPrice, decimal YSeatPrice)
        {
            decimal _zk = 0m;
            if (YSeatPrice != 0)
            {
                _zk = Math.Round((SeatPrice / YSeatPrice) + 0.0000001M, 4) * 100;
            }
            return _zk;
        }

        /// <summary>
        /// 处理:航站楼 
        /// </summary>
        ///  <param name="city">城市</param>
        /// <param name="airName">二字码</param>
        ///  <param name="Ttime">航站楼</param>
        /// <returns></returns>
        public string getHZL(string city, string airName, string Ttime)
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
                            airName.Contains("MU") || airName.Contains("FM") || airName.Contains("KN") || airName.Contains("YI"))
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
    }
}
