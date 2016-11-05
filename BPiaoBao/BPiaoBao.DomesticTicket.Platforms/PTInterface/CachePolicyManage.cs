using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models;
using BPiaoBao.DomesticTicket.Domain.Services;
using PnrAnalysis;

namespace BPiaoBao.DomesticTicket.Platforms.PTInterface
{
   
    /// <summary>
    /// 缓存接口政策
    /// </summary>
    public class CachePolicyManage
    {
        string platformCode = string.Empty;
        int cacheDay = 10;
        bool IsGetAll = true;
        DataSet policyData;
        PnrAnalysis.PnrModel pnrMode = null;
        PnrAnalysis.PatModel patMode = null;
        public CachePolicyManage(EnumPlatform platform, DataSet dsPolicy, PnrData pnrData)
        {
            //缓存政策几天过期
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["CachePolicyDay"], out cacheDay);
            //true缓存所有接口政策 false缓存最高的一条接口政策
            IsGetAll = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["IsGetAll"]);
            pnrMode = pnrData.PnrMode;
            patMode = pnrData.PatMode;
            this.policyData = dsPolicy;
            this.platformCode = platform.ToString();
        }

        public void StartCachePolicy(object o)
        {
            StringBuilder sbAddSQL = new StringBuilder();
            StringBuilder sbLog = new StringBuilder();
            try
            {
                GetFlightBasicData manage = new GetFlightBasicData();
                //获取政策
                List<PolicyCache> policyList = GetPolicyList(this.policyData);
                //过滤政策点数为负数或者0的政策
                policyList = policyList.Where(p => p.Point > 0).ToList();
                if (policyList.Count > 0)
                {
                    int patchNo = 0;
                    sbLog.AppendFormat("IsGetAll={0}\r\n", IsGetAll);
                    //获取所有政策
                    if (IsGetAll)
                    {
                        //批量添加
                        manage.TableValuedToDB(policyList, patchNo, cacheDay);
                    }
                    else
                    {
                        #region 取一条最优的
                        PolicyCache Pc = policyList.OrderByDescending(p => p.Point).FirstOrDefault();
                        if (Pc != null)
                        {
                            sbAddSQL.Append("INSERT INTO [dbo].[PolicyCache]([_id],[PolicyId],[PlatformCode],[CarrierCode],[CabinSeatCode],[FromCityCode],[MidCityCode],[ToCityCode],[SuitableFlightNo],[ExceptedFlightNo],[SuitableWeek],[CheckinTime_FromTime],[CheckinTime_EndTime],[IssueTime_FromTime],[IssueTime_EndTime],[ServiceTime_WeekendTime_FromTime],[ServiceTime_WeekendTime_EndTime],[ServiceTime_WeekTime_FromTime],[ServiceTime_WeekTime_EndTime],[TFGTime_WeekendTime_FromTime],[TFGTime_WeekendTime_EndTime],[TFGTime_WeekTime_FromTime],[TFGTime_WeekTime_EndTime],[Remark],[TravelType],[PolicyType],[Point],[CacheDate],[CacheExpiresDate],[PatchNo])VALUES(");
                            sbAddSQL.AppendFormat("'{0}',", Pc._id.ToString());
                            sbAddSQL.AppendFormat("'{0}',", Pc.PolicyId.ToString());
                            sbAddSQL.AppendFormat("'{0}',", Pc.PlatformCode.ToString());
                            sbAddSQL.AppendFormat("'{0}',", Pc.CarrierCode.ToString());
                            sbAddSQL.AppendFormat("'{0}',", string.Join("/", Pc.CabinSeatCode));
                            sbAddSQL.AppendFormat("'{0}',", Pc.FromCityCode);
                            sbAddSQL.AppendFormat("'{0}',", Pc.MidCityCode);
                            sbAddSQL.AppendFormat("'{0}',", Pc.ToCityCode);
                            sbAddSQL.AppendFormat("'{0}',", string.Join("/", Pc.SuitableFlightNo));
                            sbAddSQL.AppendFormat("'{0}',", string.Join("/", Pc.ExceptedFlightNo));
                            string strSuitableWeek = "";
                            if (Pc.SuitableWeek != null && Pc.SuitableWeek.Length > 0)
                            {
                                List<string> ll = new List<string>();
                                for (int i = 0; i < Pc.SuitableWeek.Length; i++)
                                {
                                    ll.Add(((int)Pc.SuitableWeek[i]).ToString());
                                }
                                strSuitableWeek = string.Join("/", ll.ToArray());
                            }
                            sbAddSQL.AppendFormat("'{0}',", strSuitableWeek);
                            sbAddSQL.AppendFormat("'{0}',", Pc.CheckinTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.CheckinTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.IssueTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.IssueTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.ServiceTime.WeekendTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.ServiceTime.WeekendTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.ServiceTime.WeekTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.ServiceTime.WeekTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.TFGTime.WeekendTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.TFGTime.WeekendTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.TFGTime.WeekTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.TFGTime.WeekTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.Remark);
                            sbAddSQL.AppendFormat("'{0}',", ((int)Pc.TravelType).ToString());
                            sbAddSQL.AppendFormat("'{0}',", Pc.PolicyType.ToString());
                            sbAddSQL.AppendFormat(" {0},", Pc.Point);
                            sbAddSQL.AppendFormat("'{0}',", Pc.CacheDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat("'{0}',", Pc.CacheExpiresDate.AddDays(cacheDay).ToString("yyyy-MM-dd HH:mm:ss"));
                            sbAddSQL.AppendFormat(" {0} ", patchNo);
                            sbAddSQL.Append(") \r\n");
                            string sqlExist = string.Format("select count(*) from PolicyCache where PolicyId='{0}' ", Pc.PolicyId);
                            object obj = manage.ExecuteScalar(sqlExist);
                            bool IsExist = false;
                            if (obj != null)
                            {
                                int cc = int.Parse(obj.ToString());
                                if (cc > 0)
                                {
                                    IsExist = true;
                                }
                            }
                            if (!IsExist)
                            {
                                manage.ExecuteSQL(sbAddSQL.ToString());
                            }
                        }
                        #endregion
                    }
                    //删除重复的
                    manage.ExecRepeatSQL();
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
                if (sbAddSQL.ToString() != "")
                {
                    sbLog.Append("SQL:" + sbAddSQL.ToString() + "\r\n\r\n");
                }
            }
            finally
            {
                if (sbAddSQL.ToString() != "")
                {
                    PTLog.LogWrite(sbLog.ToString(), string.Format(@"PT\StartCachePolicy{0}_Err", this.platformCode));
                }
            }
        }


        private List<PolicyCache> GetPolicyList(DataSet dsPolicy)
        {
            List<PolicyCache> policyList = new List<PolicyCache>();
            string platformCode = this.platformCode.Replace("_", "");
            if (dsPolicy != null && dsPolicy.Tables.Count > 0)
            {
                if (dsPolicy.Tables.Contains(platformCode)
                    && dsPolicy.Tables.Contains("Policy")
                    && dsPolicy.Tables[platformCode].Rows.Count > 0)
                {
                    DataRow dr_Price = dsPolicy.Tables[platformCode].Rows[0];
                    if (dr_Price["Status"].ToString() == "T")
                    {
                        DataRowCollection drs = dsPolicy.Tables[0].Rows;
                        foreach (DataRow dr in drs)
                        {
                            try
                            {
                                PolicyCache pc = new PolicyCache();
                                pc.PlatformCode = platformCode;
                                pc.CacheDate = System.DateTime.Now;
                                pc.CacheExpiresDate = pc.CacheDate.AddDays(cacheDay);
                                #region 处理每一条数据
                                switch (platformCode)
                                {
                                    case "517":
                                        {
                                            pc = _517DrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    case "8000YI":
                                        {
                                            if (int.Parse(dr["A7"] != DBNull.Value ? dr["A7"].ToString() : "1") > 2)
                                            {
                                                continue;
                                            }
                                            if ((dr["A9"] != DBNull.Value ? dr["A9"].ToString() : "").ToUpper() == "ALL")
                                            {
                                                continue;
                                            }
                                            pc = _8000YIDrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    case "PiaoMeng":
                                        {
                                            pc = _PiaoMengDrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    case "Today":
                                        {
                                            pc = _TodayDrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    case "BaiTuo":
                                        {
                                            pc = _BaiTuoDrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    case "YeeXing":
                                        {
                                            pc = _YeeXingDrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    case "51Book":
                                        {
                                            pc = _51BookDrToPolicyCache(pc, dr);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                                //添加
                                policyList.Add(pc);
                            }
                            catch (Exception ex)
                            {
                                PTLog.LogWrite("GetPolicyList::" + ex.Message, string.Format(@"PT\GetPolicyList_{0}_Err", platformCode));
                            }
                        }//EndFor
                    }
                }
            }
            return policyList;
        }
        private PolicyCache _517DrToPolicyCache(PolicyCache pc, DataRow dr)
        {
            pc.PolicyId = dr["PolicyID"] != DBNull.Value ? dr["PolicyID"].ToString() : "";
            pc.CarrierCode = dr["CarryCode"] != DBNull.Value ? dr["CarryCode"].ToString() : "";
            decimal strPoint = 0m;
            decimal.TryParse((dr["Policy"] != DBNull.Value ? dr["Policy"].ToString() : "0"), out strPoint);
            pc.Point = strPoint;
            pc.CabinSeatCode = (dr["Space"] != DBNull.Value ? dr["Space"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            pc.FromCityCode = dr["FromCity"] != DBNull.Value ? dr["FromCity"].ToString() : "";
            pc.MidCityCode = "";
            pc.ToCityCode = dr["ToCity"] != DBNull.Value ? dr["ToCity"].ToString() : "";
            string strPolicyType = (dr["PolicyType"] != DBNull.Value ? dr["PolicyType"].ToString() : "B2B");
            pc.PolicyType = (strPolicyType == "1") ? PolicyType.BSP : PolicyType.B2B;
            pc.Remark = dr["Remark"] != DBNull.Value ? dr["Remark"].ToString() : "";

            string strTravelType = dr["TravelType"] != DBNull.Value ? dr["TravelType"].ToString() : "单程";
            //单程 单程/往返  往返  中转
            pc.TravelType = strTravelType == "单程" ? TravelType.Oneway : (strTravelType == "往返" ? TravelType.Twoway : (strTravelType == "中转" ? TravelType.Connway : TravelType.OneTwoway));
            string strFlightType = dr["FlightType"] != DBNull.Value ? dr["FlightType"].ToString() : "";
            string[] strFlight = (dr["Flight"] != DBNull.Value ? dr["Flight"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (strFlightType == "1")
            {
                pc.SuitableFlightNo = strFlight;
            }
            else if (strFlightType == "2")
            {
                pc.ExceptedFlightNo = strFlight;
            }
            else
            {
                pc.SuitableFlightNo = new string[] { };
            }
            //班期限制
            string[] strDayOfWeek = (dr["ScheduleConstraints"] != DBNull.Value ? dr["ScheduleConstraints"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries); ;
            List<DayOfWeek> daylist = new List<DayOfWeek>();
            foreach (string item in strDayOfWeek)
            {
                switch (item)
                {
                    case "1":
                        daylist.Add(DayOfWeek.Monday);
                        break;
                    case "2":
                        daylist.Add(DayOfWeek.Tuesday);
                        break;
                    case "3":
                        daylist.Add(DayOfWeek.Wednesday);
                        break;
                    case "4":
                        daylist.Add(DayOfWeek.Thursday);
                        break;
                    case "5":
                        daylist.Add(DayOfWeek.Friday);
                        break;
                    case "6":
                        daylist.Add(DayOfWeek.Saturday);
                        break;
                    case "7":
                        daylist.Add(DayOfWeek.Sunday);
                        break;
                    default: break;
                }
            }
            pc.SuitableWeek = daylist.ToArray();

            //时间
            DateTime FromTime = System.DateTime.Now;
            DateTime EndTime = System.DateTime.Now;
            if (dr["EffectDate"] != DBNull.Value)
            {
                FromTime = DateTime.Parse(dr["EffectDate"].ToString());
            }
            if (dr["ExpirationDate"] != DBNull.Value)
            {
                EndTime = DateTime.Parse(dr["ExpirationDate"].ToString());
            }
            pc.CheckinTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            pc.IssueTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;
            if (dr["GYOnlineTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["GYOnlineTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    WeekEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            DateTime WeekendStartTime = System.DateTime.Now;
            DateTime WeekendEndTime = System.DateTime.Now;
            if (dr["GYOutlineTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["GYOutlineTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    WeekendStartTime = DateTime.Parse(WeekendStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    WeekendEndTime = DateTime.Parse(WeekendEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            pc.ServiceTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = WeekStartTime,
                    EndTime = WeekEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = WeekendStartTime,
                    EndTime = WeekendEndTime
                }
            };
            DateTime VoidWeekTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekTimeEndTime = System.DateTime.Now;
            if (dr["GYFPTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["GYFPTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    VoidWeekTimeStartTime = DateTime.Parse(VoidWeekTimeStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    VoidWeekTimeEndTime = DateTime.Parse(VoidWeekTimeEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }

            DateTime VoidWeekendStartTime = System.DateTime.Now;
            DateTime VoidWeekendEndTime = System.DateTime.Now;
            if (dr["GYFPTimeNew"] != DBNull.Value)
            {
                string[] strArrTime = dr["GYFPTimeNew"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    VoidWeekendStartTime = DateTime.Parse(VoidWeekendStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    VoidWeekendEndTime = DateTime.Parse(VoidWeekendEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }

            pc.TFGTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = VoidWeekendStartTime,
                    EndTime = VoidWeekendEndTime
                }
            };

            return pc;
        }
        private PolicyCache _8000YIDrToPolicyCache(PolicyCache pc, DataRow dr)
        {
            pc.PolicyId = dr["A1"] != DBNull.Value ? dr["A1"].ToString() : "";
            pc.FromCityCode = (dr["A2"] != DBNull.Value ? dr["A2"].ToString() : "").ToUpper();//.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            pc.ToCityCode = (dr["A3"] != DBNull.Value ? dr["A3"].ToString() : "").ToUpper();//.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            pc.CarrierCode = (dr["A4"] != DBNull.Value ? dr["A4"].ToString() : "").ToUpper();
            pc.SuitableFlightNo = (dr["A5"] != DBNull.Value ? dr["A5"].ToString() : "").ToUpper().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            pc.ExceptedFlightNo = (dr["A6"] != DBNull.Value ? dr["A6"].ToString() : "").ToUpper().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            int i_TravelType = int.Parse(dr["A7"] != DBNull.Value ? dr["A7"].ToString() : "1");
            //if (i_TravelType > 2)
            //{
            //    continue;
            //}
            pc.TravelType = (TravelType)i_TravelType;
            decimal point = decimal.Parse(dr["A8"] != DBNull.Value ? dr["A8"].ToString() : "0");
            pc.Point = point;
            pc.CabinSeatCode = (dr["A9"] != DBNull.Value ? dr["A9"].ToString() : "").ToUpper().Replace("#", "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            DateTime FromTime = System.DateTime.Now;
            DateTime EndTime = System.DateTime.Now;
            if (dr["A10"] != DBNull.Value)
            {
                FromTime = DateTime.Parse(dr["A10"].ToString());
            }
            if (dr["A11"] != DBNull.Value)
            {
                EndTime = DateTime.Parse(dr["A11"].ToString());
            }
            pc.CheckinTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            pc.IssueTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;
            if (dr["A12"] != DBNull.Value)
            {
                string[] strArrTime = dr["A12"].ToString().Split('|');
                if (strArrTime.Length == 2)
                {
                    WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    WeekEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            DateTime WeekendStartTime = System.DateTime.Now;
            DateTime WeekendEndTime = System.DateTime.Now;
            if (dr["A13"] != DBNull.Value)
            {
                string[] strArrTime = dr["A13"].ToString().Split('|');
                if (strArrTime.Length == 2)
                {
                    WeekendStartTime = DateTime.Parse(WeekendStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    WeekendEndTime = DateTime.Parse(WeekendEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            pc.ServiceTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = WeekStartTime,
                    EndTime = WeekEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = WeekendStartTime,
                    EndTime = WeekendEndTime
                }
            };
            string strPolicyType = (dr["A16"] != DBNull.Value ? dr["A16"].ToString() : "B2B");
            pc.PolicyType = (strPolicyType == "B2B") ? PolicyType.B2B : PolicyType.BSP;
            //是否包含换编码
            pc.Remark = dr["A17"] != DBNull.Value ? dr["A17"].ToString() : "";


            DateTime VoidWeekTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekTimeEndTime = System.DateTime.Now;
            if (dr["A19"] != DBNull.Value)
            {
                string[] strArrTime = dr["A19"].ToString().Split('|');
                if (strArrTime.Length == 2)
                {
                    VoidWeekTimeStartTime = DateTime.Parse(VoidWeekTimeStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    VoidWeekTimeEndTime = DateTime.Parse(VoidWeekTimeEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }

            DateTime VoidWeekendStartTime = System.DateTime.Now;
            DateTime VoidWeekendEndTime = System.DateTime.Now;
            if (dr["A20"] != DBNull.Value)
            {
                string[] strArrTime = dr["A20"].ToString().Split('|');
                if (strArrTime.Length == 2)
                {
                    VoidWeekendStartTime = DateTime.Parse(VoidWeekendStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    VoidWeekendEndTime = DateTime.Parse(VoidWeekendEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }

            pc.TFGTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = VoidWeekendStartTime,
                    EndTime = VoidWeekendEndTime
                }
            };
            string[] strDayOfWeek = (dr["A21"] != DBNull.Value ? dr["A21"].ToString() : "").ToUpper().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            List<DayOfWeek> daylist = new List<DayOfWeek>();
            foreach (string item in strDayOfWeek)
            {
                switch (item)
                {
                    case "1":
                        daylist.Add(DayOfWeek.Monday);
                        break;
                    case "2":
                        daylist.Add(DayOfWeek.Tuesday);
                        break;
                    case "3":
                        daylist.Add(DayOfWeek.Wednesday);
                        break;
                    case "4":
                        daylist.Add(DayOfWeek.Thursday);
                        break;
                    case "5":
                        daylist.Add(DayOfWeek.Friday);
                        break;
                    case "6":
                        daylist.Add(DayOfWeek.Saturday);
                        break;
                    case "0":
                        daylist.Add(DayOfWeek.Sunday);
                        break;
                    default: break;
                }
            }
            pc.SuitableWeek = daylist.ToArray();
            pc.CacheDate = System.DateTime.Now;
            return pc;
        }
        private PolicyCache _51BookDrToPolicyCache(PolicyCache pc, DataRow dr)
        {
            pc.PolicyId = dr["Id"] != DBNull.Value ? dr["Id"].ToString() : "";
            pc.CarrierCode = dr["airlineCode"] != DBNull.Value ? dr["airlineCode"].ToString() : "";
            decimal strPoint = 0m;
            decimal.TryParse(dr["Commission"] != DBNull.Value ? dr["Commission"].ToString() : "0", out strPoint);
            pc.Point = strPoint;
            pc.CabinSeatCode = (dr["seatClass"] != DBNull.Value ? dr["seatClass"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            //航线
            string[] strFlightCourse = (dr["flightCourse"] != DBNull.Value ? dr["flightCourse"].ToString() : "").Split('-');
            if (strFlightCourse != null && strFlightCourse.Length == 2)
            {
                pc.FromCityCode = strFlightCourse[0];
                pc.MidCityCode = "";
                pc.ToCityCode = strFlightCourse[1];
            }
            string strPolicyType = (dr["policyType"] != DBNull.Value ? dr["policyType"].ToString() : "B2B");
            pc.PolicyType = (strPolicyType == "B2B") ? PolicyType.B2B : PolicyType.BSP;
            pc.Remark = dr["comment"] != DBNull.Value ? dr["comment"].ToString() : "";
            //单程 单程/往返  往返  中转
            string strTravelType = dr["routeType"] != DBNull.Value ? dr["routeType"].ToString() : "";
            pc.TravelType = strTravelType == "OW" ? TravelType.Oneway : (strTravelType == "RT" ? TravelType.Twoway : TravelType.Connway);
            //用","隔开
            string[] strflightNoIncluding = (dr["flightNoIncluding"] != DBNull.Value ? dr["flightNoIncluding"].ToString() : "").Split(new string[] { "/", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
            pc.SuitableFlightNo = strflightNoIncluding;
            string[] strflightNoExclude = (dr["flightNoExclude"] != DBNull.Value ? dr["flightNoExclude"].ToString() : "").Split(new string[] { "/", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);
            pc.ExceptedFlightNo = strflightNoExclude;
            //适用班期
            string[] strDayOfWeek = (dr["flightCycle"] != DBNull.Value ? dr["flightCycle"].ToString() : "").Split(new string[] { "/", ",", "，" }, StringSplitOptions.RemoveEmptyEntries); ;
            List<DayOfWeek> daylist = new List<DayOfWeek>();
            foreach (string item in strDayOfWeek)
            {
                switch (item)
                {
                    case "1":
                        daylist.Add(DayOfWeek.Monday);
                        break;
                    case "2":
                        daylist.Add(DayOfWeek.Tuesday);
                        break;
                    case "3":
                        daylist.Add(DayOfWeek.Wednesday);
                        break;
                    case "4":
                        daylist.Add(DayOfWeek.Thursday);
                        break;
                    case "5":
                        daylist.Add(DayOfWeek.Friday);
                        break;
                    case "6":
                        daylist.Add(DayOfWeek.Saturday);
                        break;
                    case "0":
                        daylist.Add(DayOfWeek.Sunday);
                        break;
                    default: break;
                }
            }
            pc.SuitableWeek = daylist.ToArray();

            //时间
            DateTime FromTime = System.DateTime.Now;
            DateTime EndTime = System.DateTime.Now;
            if (dr["startDate"] != DBNull.Value)
            {
                FromTime = DateTime.Parse(dr["startDate"].ToString());
            }
            if (dr["expiredDate"] != DBNull.Value)
            {
                EndTime = DateTime.Parse(dr["expiredDate"].ToString());
            }
            pc.CheckinTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };

            FromTime = System.DateTime.Now;
            EndTime = System.DateTime.Now;
            if (dr["printTicketStartDate"] != DBNull.Value)
            {
                FromTime = DateTime.Parse(dr["printTicketStartDate"].ToString());
            }
            if (dr["printTicketExpiredDate"] != DBNull.Value)
            {
                EndTime = DateTime.Parse(dr["printTicketExpiredDate"].ToString());
            }
            pc.IssueTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;
            if (dr["workTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["workTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    WeekEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            DateTime WeekendStartTime = System.DateTime.Now;
            DateTime WeekendEndTime = System.DateTime.Now;
            if (dr["workTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["workTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    WeekendStartTime = DateTime.Parse(WeekendStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    WeekendEndTime = DateTime.Parse(WeekendEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            pc.ServiceTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = WeekStartTime,
                    EndTime = WeekEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = WeekendStartTime,
                    EndTime = WeekendEndTime
                }
            };
            DateTime VoidWeekTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekTimeEndTime = System.DateTime.Now;
            if (dr["chooseOutWorkTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["chooseOutWorkTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    VoidWeekTimeStartTime = DateTime.Parse(VoidWeekTimeStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    VoidWeekTimeEndTime = DateTime.Parse(VoidWeekTimeEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }
            DateTime VoidWeekendStartTime = System.DateTime.Now;
            DateTime VoidWeekendEndTime = System.DateTime.Now;
            if (dr["chooseOutWorkTime"] != DBNull.Value)
            {
                string[] strArrTime = dr["chooseOutWorkTime"].ToString().Split('-');
                if (strArrTime.Length == 2)
                {
                    VoidWeekendStartTime = DateTime.Parse(VoidWeekendStartTime.ToString("yyyy-MM-dd") + " " + strArrTime[0] + ":00");
                    VoidWeekendEndTime = DateTime.Parse(VoidWeekendEndTime.ToString("yyyy-MM-dd") + " " + strArrTime[1] + ":00");
                }
            }

            pc.TFGTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = VoidWeekendStartTime,
                    EndTime = VoidWeekendEndTime
                }
            };
            return pc;
        }
        private PolicyCache _TodayDrToPolicyCache(PolicyCache pc, DataRow dr)
        {

            pc.PolicyId = dr["PolicyId"] != DBNull.Value ? dr["PolicyId"].ToString() : "";
            pc.CarrierCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].AirCode : "";

            decimal strPoint = 0m;
            decimal.TryParse(dr["Discounts"] != DBNull.Value ? dr["Discounts"].ToString() : "0", out strPoint);
            pc.Point = strPoint;

            pc.CabinSeatCode = (dr["Cabin"] != DBNull.Value ? dr["Cabin"].ToString() : "").Split(new string[] { "/", "|" }, StringSplitOptions.RemoveEmptyEntries);
            pc.FromCityCode = string.Join("/", (dr["ScityE"] != DBNull.Value ? dr["ScityE"].ToString() : "").Split(new string[] { "/", "|" }, StringSplitOptions.RemoveEmptyEntries));
            pc.MidCityCode = "";
            pc.ToCityCode = string.Join("/", (dr["EcityE"] != DBNull.Value ? dr["EcityE"].ToString() : "").Split(new string[] { "/", "|" }, StringSplitOptions.RemoveEmptyEntries));

            pc.PolicyType = string.Compare(dr["RateType"].ToString().Trim(), "B2P", true) == 0 ? PolicyType.BSP : PolicyType.B2B;
            pc.Remark = dr["Remark"] != DBNull.Value ? dr["Remark"].ToString() : "";

            string strTravelType = pnrMode != null ? pnrMode.TravelType.ToString() : "1";
            //单程 单程/往返  往返  中转
            pc.TravelType = strTravelType == "1" ? TravelType.Oneway : (strTravelType == "2" ? TravelType.Twoway : TravelType.Connway);
            //时间
            DateTime FromTime = System.DateTime.Now;
            DateTime EndTime = System.DateTime.Now;
            if (dr["Sdate"] != DBNull.Value)
            {
                FromTime = DateTime.Parse(dr["Sdate"].ToString());
            }
            if (dr["Edate"] != DBNull.Value)
            {
                EndTime = DateTime.Parse(dr["Edate"].ToString());
            }
            pc.CheckinTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            pc.IssueTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;
            if (dr["WorkTimeBegin"] != DBNull.Value)
            {
                string strStartTime = dr["WorkTimeBegin"].ToString();
                if (strStartTime.Contains(":"))
                {
                    WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                }
            }
            if (dr["WorkTimeEnd"] != DBNull.Value)
            {
                string strEndTime = dr["WorkTimeEnd"].ToString();
                if (strEndTime.Contains(":"))
                {
                    WeekEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strEndTime + ":00");
                }
            }
            DateTime WeekendStartTime = WeekStartTime;
            DateTime WeekendEndTime = WeekEndTime;

            pc.ServiceTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = WeekStartTime,
                    EndTime = WeekEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = WeekendStartTime,
                    EndTime = WeekendEndTime
                }
            };
            DateTime VoidWeekTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekTimeEndTime = System.DateTime.Now;
            if (dr["RefundTimeBegin"] != DBNull.Value)
            {
                string strStartTime = dr["RefundTimeBegin"].ToString();
                if (strStartTime.Contains(":"))
                {
                    VoidWeekTimeStartTime = DateTime.Parse(VoidWeekTimeStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                }
            }
            if (dr["RefundTimeEnd"] != DBNull.Value)
            {
                string strEndTime = dr["RefundTimeEnd"].ToString();
                if (strEndTime.Contains(":"))
                {
                    VoidWeekTimeEndTime = DateTime.Parse(VoidWeekTimeEndTime.ToString("yyyy-MM-dd") + " " + strEndTime + ":00");
                }
            }
            DateTime VoidWeekendStartTime = VoidWeekTimeStartTime;
            DateTime VoidWeekendEndTime = VoidWeekTimeEndTime;
            pc.TFGTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = VoidWeekendStartTime,
                    EndTime = VoidWeekendEndTime
                }
            };

            return pc;
        }
        private PolicyCache _PiaoMengDrToPolicyCache(PolicyCache pc, DataRow dr)
        {
            pc.PolicyId = dr["id"] != DBNull.Value ? dr["id"].ToString() : "";
            pc.CarrierCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].AirCode : "";
            decimal strPoint = 0m;
            decimal.TryParse(dr["rate"] != DBNull.Value ? dr["rate"].ToString() : "0", out strPoint);
            pc.Point = strPoint;
            pc.CabinSeatCode = (dr["applyclass"] != DBNull.Value ? dr["applyclass"].ToString() : "").Split(new string[] { "/", ",", "，" }, StringSplitOptions.RemoveEmptyEntries);

            pc.FromCityCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].FromCode : "";
            pc.MidCityCode = "";
            pc.ToCityCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].ToCode : "";

            string strPolicyType = dr["policytype"] != DBNull.Value ? (dr["policytype"].ToString().ToUpper().Contains("B2P") ? "1" : "2") : "2";
            pc.PolicyType = (strPolicyType == "1") ? PolicyType.BSP : PolicyType.B2B;
            pc.Remark = dr["note"] != DBNull.Value ? dr["note"].ToString() : "";

            string strTravelType = pnrMode != null ? pnrMode.TravelType.ToString() : "1";
            //单程 单程/往返  往返  中转
            pc.TravelType = strTravelType == "1" ? TravelType.Oneway : (strTravelType == "2" ? TravelType.Twoway : TravelType.Connway);

            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;
            if (dr["worktime"] != DBNull.Value && dr["worktime"].ToString().Split('-').Length == 2)
            {
                string strStartTime = dr["worktime"].ToString().Split('-')[0];
                string strEndTime = dr["worktime"].ToString().Split('-')[1];
                if (!strStartTime.Contains(":"))
                {
                    strStartTime = strStartTime.Insert(2, ":");
                }
                if (!strEndTime.Contains(":"))
                {
                    strEndTime = strEndTime.Insert(2, ":");
                }
                WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                WeekEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strEndTime + ":00");
            }
            DateTime WeekendStartTime = WeekStartTime;
            DateTime WeekendEndTime = WeekEndTime;
            pc.ServiceTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = WeekStartTime,
                    EndTime = WeekEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = WeekendStartTime,
                    EndTime = WeekendEndTime
                }
            };
            DateTime VoidWeekTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekTimeEndTime = System.DateTime.Now;
            if (dr["RefundWorkTimeTo"] != DBNull.Value && dr["RefundWorkTimeTo"].ToString().Length > 2)
            {
                string strRefound = dr["RefundWorkTimeTo"].ToString();
                strRefound = strRefound.Insert(2, ":");
                VoidWeekTimeStartTime = WeekStartTime;
                VoidWeekTimeEndTime = DateTime.Parse(VoidWeekTimeEndTime.ToString("yyyy-MM-dd") + " " + strRefound + ":00");
            }
            pc.TFGTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                }
            };
            return pc;
        }
        private PolicyCache _BaiTuoDrToPolicyCache(PolicyCache pc, DataRow dr)
        {
            pc.PolicyId = dr["Id"] != DBNull.Value ? dr["Id"].ToString() : "";
            pc.CarrierCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].AirCode : "";
            decimal strPoint = 0m;
            decimal.TryParse(dr["Rate"] != DBNull.Value ? dr["Rate"].ToString() : "0", out strPoint);
            pc.Point = strPoint * 100;
            List<string> cabinList = new List<string>();
            if (pnrMode != null && pnrMode._LegList.Count > 0)
            {
                foreach (PnrAnalysis.Model.LegInfo leg in pnrMode._LegList)
                {
                    cabinList.Add(leg.Seat);
                }
            }
            pc.CabinSeatCode = cabinList.ToArray();
            pc.FromCityCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].FromCode : "";
            pc.MidCityCode = "";
            pc.ToCityCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].ToCode : "";
            string strPolicyType = (dr["PolicyType"] != DBNull.Value ? dr["PolicyType"].ToString() : "B2B");
            pc.PolicyType = (strPolicyType == "1") ? PolicyType.B2B : PolicyType.BSP;
            pc.Remark = dr["Remark"] != DBNull.Value ? dr["Remark"].ToString() : "";

            string strTravelType = pnrMode != null ? pnrMode.TravelType.ToString() : "1";
            //单程 单程/往返  往返  中转
            pc.TravelType = strTravelType == "1" ? TravelType.Oneway : (strTravelType == "2" ? TravelType.Twoway : TravelType.Connway);

            //时间
            DateTime FromTime = System.DateTime.Now;
            DateTime EndTime = System.DateTime.Now;
            if (dr["Effdate"] != DBNull.Value)
            {
                FromTime = DateTime.Parse(dr["Effdate"].ToString());
            }
            if (dr["Expdate"] != DBNull.Value)
            {
                EndTime = DateTime.Parse(dr["Expdate"].ToString());
            }
            pc.CheckinTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            pc.IssueTime = new TimePeriod()
            {
                FromTime = FromTime,
                EndTime = EndTime
            };
            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;

            if (pnrMode._LegList.Count > 0)
            {
                string[] strTimeArr = null;
                string strStartTime = "00:00", strEndTime = "00:00";
                int Index = 0;
                DayOfWeek dayOfWeek = DateTime.Parse(pnrMode._LegList[0].FlyDate1).DayOfWeek;
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        Index = 0;
                        break;
                    case DayOfWeek.Tuesday:
                        Index = 1;
                        break;
                    case DayOfWeek.Wednesday:
                        Index = 2;
                        break;
                    case DayOfWeek.Thursday:
                        Index = 3;
                        break;
                    case DayOfWeek.Friday:
                        Index = 4;
                        break;
                    case DayOfWeek.Saturday:
                        Index = 5;
                        break;
                    case DayOfWeek.Sunday:
                        Index = 6;
                        break;
                    default:
                        break;
                }
                if (dr["ProviderWorkTime"].ToString().Split(',').Length == 7 && Index > -1 && Index < 7)
                {
                    strTimeArr = dr["ProviderWorkTime"].ToString().Split(',');
                    if (strTimeArr[Index].Split('-').Length == 2)
                    {
                        strStartTime = strTimeArr[Index].Split('-')[0];
                        strEndTime = strTimeArr[Index].Split('-')[1];
                        WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                        WeekEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strEndTime + ":00");
                    }
                }

                DateTime WeekendStartTime = WeekStartTime;
                DateTime WeekendEndTime = WeekEndTime;
                pc.ServiceTime = new WorkTime()
                {
                    WeekTime = new TimePeriod()
                    {
                        FromTime = WeekStartTime,
                        EndTime = WeekEndTime
                    },
                    WeekendTime = new TimePeriod()
                    {
                        FromTime = WeekendStartTime,
                        EndTime = WeekendEndTime
                    }
                };
                DateTime VoidWeekTimeStartTime = System.DateTime.Now;
                DateTime VoidWeekTimeEndTime = System.DateTime.Now;

                if (dr["VoidWorkTime"] != DBNull.Value)
                {
                    string[] strArrTime = dr["VoidWorkTime"].ToString().Split(',');
                    if (strTimeArr[Index].Split('-').Length == 2)
                    {
                        strStartTime = strTimeArr[Index].Split('-')[0];
                        strEndTime = strTimeArr[Index].Split('-')[1];
                        VoidWeekTimeStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                        VoidWeekTimeEndTime = DateTime.Parse(WeekEndTime.ToString("yyyy-MM-dd") + " " + strEndTime + ":00");
                    }
                }
                DateTime VoidWeekendStartTime = VoidWeekTimeStartTime;
                DateTime VoidWeekendEndTime = VoidWeekTimeEndTime;
                pc.TFGTime = new WorkTime()
                {
                    WeekTime = new TimePeriod()
                    {
                        FromTime = VoidWeekTimeStartTime,
                        EndTime = VoidWeekTimeEndTime
                    },
                    WeekendTime = new TimePeriod()
                    {
                        FromTime = VoidWeekendStartTime,
                        EndTime = VoidWeekendEndTime
                    }
                };
            }
            return pc;
        }
        private PolicyCache _YeeXingDrToPolicyCache(PolicyCache pc, DataRow dr)
        {
            pc.PolicyId = dr["plcid"] != DBNull.Value ? dr["plcid"].ToString() : "";
            pc.CarrierCode = dr["airComp"] != DBNull.Value ? dr["airComp"].ToString() : "";
            decimal strPoint = 0m;
            decimal.TryParse(dr["disc"] != DBNull.Value ? dr["disc"].ToString() : "0", out strPoint);
            pc.Point = strPoint;
            List<string> cabinList = new List<string>();
            if (pnrMode != null && pnrMode._LegList.Count > 0)
            {
                foreach (PnrAnalysis.Model.LegInfo leg in pnrMode._LegList)
                {
                    cabinList.Add(leg.Seat);
                }
            }
            pc.CabinSeatCode = cabinList.ToArray();

            pc.FromCityCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].FromCode : "";
            pc.MidCityCode = "";
            pc.ToCityCode = pnrMode != null && pnrMode._LegList.Count > 0 ? pnrMode._LegList[0].ToCode : "";

            string strPolicyType = (dr["tickType"] != DBNull.Value ? dr["tickType"].ToString() : "1");
            pc.PolicyType = (strPolicyType == "1") ? PolicyType.B2B : PolicyType.BSP;
            pc.Remark = dr["memo"] != DBNull.Value ? dr["memo"].ToString() : "";

            string strTravelType = pnrMode != null ? pnrMode.TravelType.ToString() : "1";
            //单程 单程/往返  往返  中转
            pc.TravelType = strTravelType == "1" ? TravelType.Oneway : (strTravelType == "2" ? TravelType.Twoway : TravelType.Connway);

            //时间
            DayOfWeek dayOfWeek = DateTime.Parse(pnrMode._LegList[0].FlyDate1).DayOfWeek;
            string strStartTime = "00:00";
            string strEndTime = "00:00";
            DateTime WeekStartTime = System.DateTime.Now;
            DateTime WeekEndTime = System.DateTime.Now;
            DateTime WeekendStartTime = System.DateTime.Now;
            DateTime WeekendEndTime = System.DateTime.Now;

            DateTime VoidWeekTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekTimeEndTime = System.DateTime.Now;
            DateTime VoidWeekendTimeStartTime = System.DateTime.Now;
            DateTime VoidWeekendTimeEndTime = System.DateTime.Now;
            if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
            {
                //周一到周五
                if (dr["workTime"].ToString().Split('-').Length == 2)
                {
                    strStartTime = dr["workTime"].ToString().Split('-')[0];
                    strEndTime = dr["workTime"].ToString().Split('-')[1];
                }
                WeekStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                WeekEndTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                if (dr["workReturnTime"].ToString().Split('-').Length == 2)
                {
                    strStartTime = dr["workReturnTime"].ToString().Split('-')[0];
                    strEndTime = dr["workReturnTime"].ToString().Split('-')[1];
                }
                VoidWeekTimeStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                VoidWeekTimeEndTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
            }
            else
            {
                //周末
                if (dr["restWorkTime"].ToString().Split('-').Length == 2)
                {
                    strStartTime = dr["restWorkTime"].ToString().Split('-')[0];
                    strEndTime = dr["restWorkTime"].ToString().Split('-')[1];
                }
                WeekendStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                WeekendEndTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                if (dr["restReturnTime"].ToString().Split('-').Length == 2)
                {
                    strStartTime = dr["restReturnTime"].ToString().Split('-')[0];
                    strEndTime = dr["restReturnTime"].ToString().Split('-')[1];
                }
                VoidWeekendTimeStartTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
                VoidWeekendTimeEndTime = DateTime.Parse(WeekStartTime.ToString("yyyy-MM-dd") + " " + strStartTime + ":00");
            }
            pc.ServiceTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = WeekStartTime,
                    EndTime = WeekEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = WeekendStartTime,
                    EndTime = WeekendEndTime
                }
            };
            pc.TFGTime = new WorkTime()
            {
                WeekTime = new TimePeriod()
                {
                    FromTime = VoidWeekTimeStartTime,
                    EndTime = VoidWeekTimeEndTime
                },
                WeekendTime = new TimePeriod()
                {
                    FromTime = VoidWeekendTimeStartTime,
                    EndTime = VoidWeekendTimeEndTime
                }
            };
            return pc;
        }
    }
}
