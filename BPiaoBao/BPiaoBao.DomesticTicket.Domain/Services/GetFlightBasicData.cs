using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models;
namespace BPiaoBao.DomesticTicket.Domain.Services
{
    public class GetFlightBasicData
    {
        string strConnectionString = string.Empty;
        public GetFlightBasicData()
        {
            strConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FlightQuery"].ConnectionString;
        }
        #region 测试数据
        /// <summary>
        /// 返回datatable
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public DataTable GetDataSet(string safeSql)
        {
            DataSet ds = new DataSet();
            try
            {

                SqlCommand cmd = new SqlCommand(safeSql, new SqlConnection(strConnectionString));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetDataSet]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetDataSet]", e);
            }
            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }
        public int ExecuteSQL(string safeSql)
        {
            int result = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(strConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(safeSql, connection))
                    {
                        cmd.CommandTimeout = 120;
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        result = cmd.ExecuteNonQuery();
                    }
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[ExecuteSQL]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[ExecuteSQL]", e);
            }
            return result;
        }

        public object ExecuteScalar(string safeSql)
        {
            object result = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(strConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(safeSql, connection))
                    {
                        cmd.CommandTimeout = 120;
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        result = cmd.ExecuteScalar();
                    }
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[ExecuteScalar]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[ExecuteScalar]", e);
            }
            return result;
        }


        public int ExecuteSQLList(List<string> safeSql)
        {
            int result = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(strConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 120;
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    foreach (string sql in safeSql)
                    {
                        cmd.CommandText = sql;
                        result = cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[ExecuteSQLList]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[ExecuteSQLList]", e);
            }
            return result;
        }
        public List<CabinRow> GetCabinSeatListPolicy(string sqlWhere)
        {
            string sql = "select CarrierCode,SeatList from dbo.CabinSeat";
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql += " where " + sqlWhere;
            }
            List<CabinRow> reList = new List<CabinRow>();
            try
            {
                using (SqlConnection conn = new SqlConnection(strConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    string strCarrayCode = string.Empty;
                    while (rdr.Read())
                    {
                        CabinRow cabinRow = new CabinRow();
                        strCarrayCode = rdr["CarrierCode"] != null ? rdr["CarrierCode"].ToString() : "";
                        string strSeat = rdr["SeatList"] != null ? rdr["SeatList"].ToString() : "";
                        string[] strArray = strSeat.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (strArray.Length > 0)
                        {
                            List<Seat> listSpace = new List<Seat>();
                            foreach (string item in strArray)
                            {
                                if (item.Split('-').Length == 2)
                                {
                                    reList.Add(new CabinRow()
                                    {
                                        CarrayCode = strCarrayCode,
                                        Seat = item.Split('-')[0],
                                        Rebate = decimal.Parse(item.Split('-')[1])
                                    });
                                }
                            }
                        }
                    }
                    rdr.Close();
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetCabinSeatListPolicy]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetCabinSeatListPolicy]", e);
            }
            return reList;
        }


        public List<CabinSeat> GetCabinSeatList(string sqlWhere)
        {
            string sql = "select * from dbo.CabinSeat";
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql += " where " + sqlWhere;
            }
            List<CabinSeat> reList = new List<CabinSeat>();
            try
            {
                using (SqlConnection conn = new SqlConnection(strConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        CabinSeat cabinSeat = new CabinSeat();
                        cabinSeat._id = rdr["_id"] != null ? rdr["_id"].ToString() : "";
                        cabinSeat.CarrierCode = rdr["CarrierCode"] != null ? rdr["CarrierCode"].ToString() : "";
                        cabinSeat.Airline = new Airline()
                        {
                            FromCode = rdr["FromCode"] != null ? rdr["FromCode"].ToString() : "",
                            ToCode = rdr["ToCode"] != null ? rdr["ToCode"].ToString() : "",
                            Mileage = int.Parse(rdr["Mileage"] != null ? rdr["Mileage"].ToString() : "0"),
                            SeatPrice = decimal.Parse(rdr["SeatPrice"] != null ? rdr["SeatPrice"].ToString() : "0")
                        };
                        cabinSeat.Fuel = new Fuel()
                        {
                            AdultFuelFee = decimal.Parse(rdr["AdultFuelFee"] != null ? rdr["AdultFuelFee"].ToString() : "0"),
                            ChildFuelFee = decimal.Parse(rdr["ChildFuelFee"] != null ? rdr["ChildFuelFee"].ToString() : "0")
                        };
                        string strSeat = rdr["SeatList"] != null ? rdr["SeatList"].ToString() : "";
                        string[] strArray = strSeat.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (strArray.Length > 0)
                        {
                            List<Seat> listSpace = new List<Seat>();
                            foreach (string item in strArray)
                            {
                                if (item.Split('-').Length == 2)
                                {
                                    listSpace.Add(new Seat()
                                    {
                                        Code = item.Split('-')[0],
                                        Rebate = decimal.Parse(item.Split('-')[1])
                                    });
                                }
                            }
                            cabinSeat.SeatList = listSpace;
                        }
                        else
                        {
                            cabinSeat.SeatList = new List<Seat>();
                        }
                        reList.Add(cabinSeat);
                    }
                    rdr.Close();
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetCabinSeatList]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetCabinSeatList]", e);
            }
            return reList;
        }
        public bool UpdateCarrier(string Code, string YDOffice, string CPOffice, string PrintNo)
        {
            string sql = string.Format("update dbo.AirCarrier set YDOffice='{0}',CPOffice='{1}',PrintNo='{2}' where Code='{3}' ",
                YDOffice, CPOffice, PrintNo, Code
                );
            int execCount = ExecuteSQL(sql);
            return (execCount > 0 ? true : false);
        }
        public List<AirCarrier> GetAirCarrier(string sqlWhere)
        {
            List<AirCarrier> listAirCarrier = new List<AirCarrier>();
            string sql = "select * from dbo.AirCarrier";
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql += " where " + sqlWhere;
            }
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        AirCarrier ac = new AirCarrier();
                        //ac._id = rdr["_id"] != null ? rdr["_id"].ToString() : "";
                        ac.AirName = rdr["AirName"] != null ? rdr["AirName"].ToString() : "";
                        ac.Code = rdr["Code"] != null ? rdr["Code"].ToString() : "";
                        ac.SettleCode = rdr["SettleCode"] != null ? rdr["SettleCode"].ToString() : "";
                        ac.ShortName = rdr["ShortName"] != null ? rdr["ShortName"].ToString() : "";
                        ac.YDOffice = rdr["YDOffice"] != null ? rdr["YDOffice"].ToString() : "";
                        ac.CPOffice = rdr["CPOffice"] != null ? rdr["CPOffice"].ToString() : "";
                        ac.ShortName = rdr["PrintNo"] != null ? rdr["PrintNo"].ToString() : "";
                        listAirCarrier.Add(ac);
                    }
                    rdr.Close();
                }
                catch (SqlException ex)
                {
                    JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetAirCarrier]", ex);
                }
                catch (Exception e)
                {
                    JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetAirCarrier]", e);
                }
            }
            return listAirCarrier;
        }

        public bool ExistAirplainType(string code)
        {
            int count = -1;
            string sql = string.Format("select count(*) from dbo.AirplainType where Code='{0}'", code);
            int.TryParse(ExecuteScalar(sql).ToString(), out count);
            return count > 0 ? true : false;
        }

        public List<AirplainType> GetAirplainType()
        {
            List<AirplainType> listAirplainType = new List<AirplainType>();
            string sql = "select * from dbo.AirplainType";
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        AirplainType atype = new AirplainType();
                        //atype._id = rdr["_id"] != null ? rdr["_id"].ToString() : "";
                        atype.Code = rdr["Code"] != null ? rdr["Code"].ToString() : "";
                        atype.TaxFee = decimal.Parse(rdr["TaxFee"] != null ? rdr["TaxFee"].ToString() : "0");
                        listAirplainType.Add(atype);
                    }
                    rdr.Close();
                }
                catch (SqlException ex)
                {
                    JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetAirplainType]", ex);
                }
                catch (Exception e)
                {
                    JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetAirplainType]", e);
                }
            }
            return listAirplainType;
        }

        public List<CityCode> GetCityCode()
        {
            string sql = "select * from dbo.CityCode";
            List<CityCode> listCityCode = new List<CityCode>();
            using (SqlConnection conn = new SqlConnection(strConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        CityCode cityCode = new CityCode();
                        //cityCode._id = rdr["_id"] != null ? rdr["_id"].ToString() : "";
                        cityCode.Code = rdr["Code"] != null ? rdr["Code"].ToString() : "";
                        cityCode.Name = rdr["Name"] != null ? rdr["Name"].ToString() : "";
                        cityCode.SimplePinyin = rdr["SimplePinyin"] != null ? rdr["SimplePinyin"].ToString() : "";
                        cityCode.WholePinyin = rdr["WholePinyin"] != null ? rdr["WholePinyin"].ToString() : "";
                        cityCode.Terminal = rdr["Terminal"] != null ? rdr["Terminal"].ToString() : "";
                        listCityCode.Add(cityCode);
                    }
                    rdr.Close();
                }
                catch (SqlException ex)
                {
                    JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetCityCode]", ex);
                }
                catch (Exception e)
                {
                    JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetCityCode]", e);
                }
            }
            return listCityCode;
        }

        public List<BaseCabin> GetBaseCabin(string sqlWhere)
        {
            string sql = "select * from dbo.BaseCabin";
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql += " where " + sqlWhere;
            }
            List<BaseCabin> reList = new List<BaseCabin>();
            try
            {
                using (SqlConnection conn = new SqlConnection(strConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        BaseCabin baseCabin = new BaseCabin();
                        //baseCabin._id = rdr["_id"] != null ? rdr["_id"].ToString() : "";
                        baseCabin.CarrierCode = rdr["CarrierCode"] != null ? rdr["CarrierCode"].ToString() : "";
                        baseCabin.Code = rdr["Code"] != null ? rdr["Code"].ToString() : "";
                        decimal _Rebate = 0m;
                        decimal.TryParse((rdr["Rebate"] != null ? rdr["Rebate"].ToString() : "0"), out _Rebate);
                        baseCabin.Rebate = _Rebate;
                        reList.Add(baseCabin);
                    }
                    rdr.Close();
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetBaseCabin]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetBaseCabin]", e);
            }
            return reList;

        }

        public List<PolicyCache> GetPolicyCache(string sqlWhere)
        {
            List<PolicyCache> reList = new List<PolicyCache>();
            string sql = "select * from PolicyCache ";
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql += " where " + sqlWhere;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(strConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    while (rdr.Read())
                    {
                        PolicyCache pc = new PolicyCache();
                        pc._id = rdr["_id"] != DBNull.Value ? rdr["_id"].ToString() : "";
                        pc.PolicyId = rdr["PolicyId"] != DBNull.Value ? rdr["PolicyId"].ToString() : "";
                        pc.CarrierCode = rdr["CarrierCode"] != DBNull.Value ? rdr["CarrierCode"].ToString() : "";
                        pc.CabinSeatCode = (rdr["CabinSeatCode"] != DBNull.Value ? rdr["CabinSeatCode"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        pc.FromCityCode = rdr["FromCityCode"] != DBNull.Value ? rdr["FromCityCode"].ToString() : "";
                        pc.MidCityCode = rdr["MidCityCode"] != DBNull.Value ? rdr["MidCityCode"].ToString() : "";
                        pc.ToCityCode = rdr["ToCityCode"] != DBNull.Value ? rdr["ToCityCode"].ToString() : "";
                        pc.SuitableFlightNo = (rdr["SuitableFlightNo"] != DBNull.Value ? rdr["SuitableFlightNo"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        pc.ExceptedFlightNo = (rdr["ExceptedFlightNo"] != DBNull.Value ? rdr["ExceptedFlightNo"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] strSuitableWeek = (rdr["SuitableWeek"] != DBNull.Value ? rdr["SuitableWeek"].ToString() : "").Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        List<DayOfWeek> daylist = new List<DayOfWeek>();
                        foreach (string item in strSuitableWeek)
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

                        DateTime CheckinTime_FromTime = System.DateTime.Now;
                        DateTime CheckinTime_EndTime = System.DateTime.Now;
                        DateTime IssueTime_FromTime = System.DateTime.Now;
                        DateTime IssueTime_EndTime = System.DateTime.Now;
                        DateTime ServiceTime_WeekendTime_FromTime = System.DateTime.Now;
                        DateTime ServiceTime_WeekendTime_EndTime = System.DateTime.Now;
                        DateTime ServiceTime_WeekTime_FromTime = System.DateTime.Now;
                        DateTime ServiceTime_WeekTime_EndTime = System.DateTime.Now;
                        DateTime TFGTime_WeekendTime_FromTime = System.DateTime.Now;
                        DateTime TFGTime_WeekendTime_EndTime = System.DateTime.Now;
                        DateTime TFGTime_WeekTime_FromTime = System.DateTime.Now;
                        DateTime TFGTime_WeekTime_EndTime = System.DateTime.Now;

                        if (rdr["CheckinTime_FromTime"] != DBNull.Value)
                        {
                            CheckinTime_FromTime = DateTime.Parse(rdr["CheckinTime_FromTime"].ToString());
                        }
                        if (rdr["CheckinTime_EndTime"] != DBNull.Value)
                        {
                            CheckinTime_EndTime = DateTime.Parse(rdr["CheckinTime_EndTime"].ToString());
                        }
                        if (rdr["IssueTime_FromTime"] != DBNull.Value)
                        {
                            IssueTime_FromTime = DateTime.Parse(rdr["IssueTime_FromTime"].ToString());
                        }
                        if (rdr["IssueTime_EndTime"] != DBNull.Value)
                        {
                            IssueTime_EndTime = DateTime.Parse(rdr["IssueTime_EndTime"].ToString());
                        }

                        pc.CheckinTime = new TimePeriod()
                        {
                            FromTime = CheckinTime_FromTime,
                            EndTime = CheckinTime_EndTime
                        };
                        pc.IssueTime = new TimePeriod()
                        {
                            FromTime = IssueTime_FromTime,
                            EndTime = IssueTime_EndTime
                        };
                        if (rdr["ServiceTime_WeekendTime_FromTime"] != DBNull.Value)
                        {
                            ServiceTime_WeekendTime_FromTime = DateTime.Parse(rdr["ServiceTime_WeekendTime_FromTime"].ToString());
                        }
                        if (rdr["ServiceTime_WeekendTime_EndTime"] != DBNull.Value)
                        {
                            ServiceTime_WeekendTime_EndTime = DateTime.Parse(rdr["ServiceTime_WeekendTime_EndTime"].ToString());
                        }

                        if (rdr["ServiceTime_WeekTime_FromTime"] != DBNull.Value)
                        {
                            ServiceTime_WeekTime_FromTime = DateTime.Parse(rdr["ServiceTime_WeekTime_FromTime"].ToString());
                        }
                        if (rdr["ServiceTime_WeekTime_EndTime"] != DBNull.Value)
                        {
                            ServiceTime_WeekTime_EndTime = DateTime.Parse(rdr["ServiceTime_WeekTime_EndTime"].ToString());
                        }
                        pc.ServiceTime = new WorkTime()
                        {
                            WeekTime = new TimePeriod()
                            {
                                FromTime = ServiceTime_WeekTime_FromTime,
                                EndTime = ServiceTime_WeekTime_EndTime
                            },
                            WeekendTime = new TimePeriod()
                            {
                                FromTime = ServiceTime_WeekendTime_FromTime,
                                EndTime = ServiceTime_WeekendTime_EndTime
                            }
                        };
                        if (rdr["TFGTime_WeekendTime_FromTime"] != DBNull.Value)
                        {
                            TFGTime_WeekendTime_FromTime = DateTime.Parse(rdr["TFGTime_WeekendTime_FromTime"].ToString());
                        }
                        if (rdr["TFGTime_WeekendTime_EndTime"] != DBNull.Value)
                        {
                            TFGTime_WeekendTime_EndTime = DateTime.Parse(rdr["TFGTime_WeekendTime_EndTime"].ToString());
                        }
                        if (rdr["TFGTime_WeekTime_FromTime"] != DBNull.Value)
                        {
                            TFGTime_WeekTime_FromTime = DateTime.Parse(rdr["TFGTime_WeekTime_FromTime"].ToString());
                        }
                        if (rdr["TFGTime_WeekTime_EndTime"] != DBNull.Value)
                        {
                            TFGTime_WeekTime_EndTime = DateTime.Parse(rdr["TFGTime_WeekTime_EndTime"].ToString());
                        }
                        pc.TFGTime = new WorkTime()
                        {
                            WeekTime = new TimePeriod()
                            {
                                FromTime = TFGTime_WeekTime_FromTime,
                                EndTime = TFGTime_WeekTime_EndTime
                            },
                            WeekendTime = new TimePeriod()
                            {
                                FromTime = TFGTime_WeekendTime_FromTime,
                                EndTime = TFGTime_WeekendTime_EndTime
                            }
                        };
                        pc.Remark = rdr["Remark"] != DBNull.Value ? rdr["Remark"].ToString() : "";
                        string strTravelType = rdr["TravelType"] != DBNull.Value ? rdr["TravelType"].ToString() : "1";
                        pc.TravelType = strTravelType == "1" ? TravelType.Oneway : (strTravelType == "2" ? TravelType.Twoway : TravelType.Connway);
                        string strPolicyType = rdr["PolicyType"] != DBNull.Value ? rdr["PolicyType"].ToString() : "B2B";
                        pc.PolicyType = (strPolicyType == "B2B") ? PolicyType.B2B : PolicyType.BSP;
                        decimal _point = 0m;
                        decimal.TryParse((rdr["Point"] != DBNull.Value ? rdr["Point"].ToString() : "0"), out _point);
                        pc.Point = _point;
                        pc.OldPoint = _point;
                        pc.PlatformCode = rdr["PlatformCode"] != DBNull.Value ? rdr["PlatformCode"].ToString() : "";
                        pc.CacheDate = rdr["CacheDate"] != DBNull.Value ? DateTime.Parse(rdr["CacheDate"].ToString()) : System.DateTime.Now;
                        pc.CacheExpiresDate = rdr["CacheExpiresDate"] != DBNull.Value ? DateTime.Parse(rdr["CacheExpiresDate"].ToString()) : System.DateTime.Now.AddMonths(1);
                        pc.PolicySourceType = EnumPolicySourceType.Interface;
                        reList.Add(pc);
                    }
                    rdr.Close();
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetPolicyCache]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetPolicyCache]", e);
            }
            return reList;
        }

        public bool AddPolicyCache(List<PolicyCache> PolicyList)
        {
            bool Issucess = false;
            StringBuilder sbAdd = new StringBuilder();
            try
            {
                foreach (PolicyCache Pc in PolicyList)
                {
                    sbAdd.Append("INSERT INTO [dbo].[PolicyCache]([_id],[PolicyId],[PlatformCode],[CarrierCode],[CabinSeatCode],[FromCityCode],[MidCityCode],[ToCityCode],[SuitableFlightNo],[ExceptedFlightNo],[SuitableWeek],[CheckinTime_FromTime],[CheckinTime_EndTime],[IssueTime_FromTime],[IssueTime_EndTime],[ServiceTime_WeekendTime_FromTime],[ServiceTime_WeekendTime_EndTime],[ServiceTime_WeekTime_FromTime],[ServiceTime_WeekTime_EndTime],[TFGTime_WeekendTime_FromTime],[TFGTime_WeekendTime_EndTime],[TFGTime_WeekTime_FromTime],[TFGTime_WeekTime_EndTime],[Remark],[TravelType],[PolicyType],[Point],[CacheDate])VALUES(");
                    sbAdd.AppendFormat("'{0}',", Pc._id.ToString());
                    sbAdd.AppendFormat("'{0}',", Pc.PolicyId.ToString());
                    sbAdd.AppendFormat("'{0}',", Pc.PlatformCode.ToString());
                    sbAdd.AppendFormat("'{0}',", Pc.CarrierCode.ToString());
                    sbAdd.AppendFormat("'{0}',", string.Join("/", Pc.CabinSeatCode));
                    sbAdd.AppendFormat("'{0}',", Pc.FromCityCode);
                    sbAdd.AppendFormat("'{0}',", Pc.MidCityCode);
                    sbAdd.AppendFormat("'{0}',", Pc.ToCityCode);
                    sbAdd.AppendFormat("'{0}',", string.Join("/", Pc.SuitableFlightNo));
                    sbAdd.AppendFormat("'{0}',", string.Join("/", Pc.ExceptedFlightNo));
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
                    sbAdd.AppendFormat("'{0}',", strSuitableWeek);
                    sbAdd.AppendFormat("'{0}',", Pc.CheckinTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.CheckinTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.IssueTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.IssueTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.ServiceTime.WeekendTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.ServiceTime.WeekendTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.ServiceTime.WeekTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.ServiceTime.WeekTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.TFGTime.WeekendTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.TFGTime.WeekendTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.TFGTime.WeekTime.FromTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.TFGTime.WeekTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.AppendFormat("'{0}',", Pc.Remark);
                    sbAdd.AppendFormat("'{0}',", ((int)Pc.TravelType).ToString());
                    sbAdd.AppendFormat("'{0}',", Pc.PolicyType.ToString());
                    sbAdd.AppendFormat("{0},", Pc.Point);
                    sbAdd.AppendFormat("'{0}'", Pc.CacheDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbAdd.Append(") \r\n");
                }
                PnrAnalysis.LogText.LogWrite(sbAdd.ToString(), "AddPolicyCache");
                Issucess = true;
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[AddPolicyCache]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[AddPolicyCache]", e);
            }
            return Issucess;
        }



        /// <summary>
        /// 获取折扣
        /// </summary>
        /// <returns></returns>
        public decimal GetZK(string fromcode, string tocode, string carrayCode, string seat, decimal seatPrice)
        {
            decimal zk = 0m;
            try
            {
                string sqlWhere = string.Format("CarrierCode='{0}' and FromCode='{1}' and ToCode='{2}' and SeatList like '%Y-100%'", carrayCode, fromcode, tocode);
                List<CabinSeat> list = GetCabinSeatList(sqlWhere);
                if (list != null && list.Count > 0)
                {
                    Seat m_seat = list[0].SeatList.Where(p1 => p1.Code.ToUpper() == seat.ToUpper()).FirstOrDefault();
                    if (m_seat != null)
                    {
                        zk = m_seat.Rebate;
                    }
                }
                if (zk == 0)
                {
                    string sql = string.Format("select FareFee from dbo.Bd_Air_Fares where FromCityCode='{0}' and ToCityCode='{1}' and (CarryCode=''or CarryCode='{2}')", fromcode, tocode, carrayCode);
                    object objFare = ExecuteScalar(sql);
                    decimal YFare = 0m;
                    if (objFare != null && decimal.TryParse(objFare.ToString(), out YFare) && YFare > 0)
                    {
                        zk = GetZk(seatPrice, YFare);
                    }
                }
            }
            catch (SqlException ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetZK]", ex);
            }
            catch (Exception e)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "航班数据库查询[GetZK]", e);
            }
            return zk;
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





        #endregion

        #region 缓存接口政策
        private DataTable GetTable(List<PolicyCache> policyList, int patchNo, int cacheDay)
        {
            DataTable dt = new DataTable();
            if (policyList != null && policyList.Count > 0)
            {
                PolicyCache policy = policyList[0];
                List<DataColumn> listCol = new List<DataColumn>();
                listCol.Add(new DataColumn("_id", typeof(string)));
                listCol.Add(new DataColumn("PolicyId", typeof(string)));
                listCol.Add(new DataColumn("PlatformCode", typeof(string)));
                listCol.Add(new DataColumn("CarrierCode", typeof(string)));
                listCol.Add(new DataColumn("CabinSeatCode", typeof(string)));
                listCol.Add(new DataColumn("FromCityCode", typeof(string)));
                listCol.Add(new DataColumn("MidCityCode", typeof(string)));
                listCol.Add(new DataColumn("ToCityCode", typeof(string)));
                listCol.Add(new DataColumn("SuitableFlightNo", typeof(string)));
                listCol.Add(new DataColumn("ExceptedFlightNo", typeof(string)));
                listCol.Add(new DataColumn("SuitableWeek", typeof(string)));
                listCol.Add(new DataColumn("CheckinTime_FromTime", typeof(DateTime)));
                listCol.Add(new DataColumn("CheckinTime_EndTime", typeof(DateTime)));
                listCol.Add(new DataColumn("IssueTime_FromTime", typeof(DateTime)));
                listCol.Add(new DataColumn("IssueTime_EndTime", typeof(DateTime)));
                listCol.Add(new DataColumn("ServiceTime_WeekendTime_FromTime", typeof(DateTime)));
                listCol.Add(new DataColumn("ServiceTime_WeekendTime_EndTime", typeof(DateTime)));
                listCol.Add(new DataColumn("ServiceTime_WeekTime_FromTime", typeof(DateTime)));
                listCol.Add(new DataColumn("ServiceTime_WeekTime_EndTime", typeof(DateTime)));
                listCol.Add(new DataColumn("TFGTime_WeekendTime_FromTime", typeof(DateTime)));
                listCol.Add(new DataColumn("TFGTime_WeekendTime_EndTime", typeof(DateTime)));
                listCol.Add(new DataColumn("TFGTime_WeekTime_FromTime", typeof(DateTime)));
                listCol.Add(new DataColumn("TFGTime_WeekTime_EndTime", typeof(DateTime)));
                listCol.Add(new DataColumn("Remark", typeof(string)));
                listCol.Add(new DataColumn("TravelType", typeof(string)));
                listCol.Add(new DataColumn("PolicyType", typeof(string)));
                listCol.Add(new DataColumn("Point", typeof(decimal)));
                listCol.Add(new DataColumn("CacheDate", typeof(DateTime)));
                listCol.Add(new DataColumn("CacheExpiresDate", typeof(DateTime)));
                listCol.Add(new DataColumn("PatchNo", typeof(int)));
                dt.Columns.AddRange(listCol.ToArray());
                dt.TableName = policy.GetType().Name;
                DataRow dr = null;
                foreach (PolicyCache item in policyList)
                {
                    dr = dt.NewRow();
                    //赋值
                    dr["_id"] = item._id;
                    dr["PolicyId"] = item.PolicyId;
                    dr["PlatformCode"] = item.PlatformCode;
                    dr["CarrierCode"] = item.CarrierCode;
                    dr["CabinSeatCode"] = string.Join("/", item.CabinSeatCode);
                    dr["FromCityCode"] = item.FromCityCode;
                    dr["MidCityCode"] = item.MidCityCode;
                    dr["ToCityCode"] = item.ToCityCode;
                    dr["SuitableFlightNo"] = string.Join("/", item.SuitableFlightNo);
                    dr["ExceptedFlightNo"] = string.Join("/", item.ExceptedFlightNo);
                    string strSuitableWeek = "";
                    if (item.SuitableWeek != null && item.SuitableWeek.Length > 0)
                    {
                        List<string> ll = new List<string>();
                        for (int i = 0; i < item.SuitableWeek.Length; i++)
                        {
                            ll.Add(((int)item.SuitableWeek[i]).ToString());
                        }
                        strSuitableWeek = string.Join("/", ll.ToArray());
                    }
                    dr["SuitableWeek"] = strSuitableWeek;
                    dr["CheckinTime_FromTime"] = item.CheckinTime.FromTime;
                    dr["CheckinTime_EndTime"] = item.CheckinTime.EndTime;
                    dr["IssueTime_FromTime"] = item.IssueTime.FromTime;
                    dr["IssueTime_EndTime"] = item.IssueTime.EndTime;
                    dr["ServiceTime_WeekendTime_FromTime"] = item.ServiceTime.WeekendTime.FromTime;
                    dr["ServiceTime_WeekendTime_EndTime"] = item.ServiceTime.WeekendTime.EndTime;
                    dr["ServiceTime_WeekTime_FromTime"] = item.ServiceTime.WeekTime.FromTime;
                    dr["ServiceTime_WeekTime_EndTime"] = item.ServiceTime.WeekTime.EndTime;
                    dr["TFGTime_WeekendTime_FromTime"] = item.TFGTime.WeekendTime.FromTime;
                    dr["TFGTime_WeekendTime_EndTime"] = item.TFGTime.WeekendTime.EndTime;
                    dr["TFGTime_WeekTime_FromTime"] = item.TFGTime.WeekTime.FromTime;
                    dr["TFGTime_WeekTime_EndTime"] = item.TFGTime.WeekTime.EndTime;
                    dr["Remark"] = item.Remark;
                    dr["TravelType"] = (int)item.TravelType;
                    dr["PolicyType"] = item.PolicyType;
                    dr["Point"] = item.Point;
                    dr["CacheDate"] = item.CacheDate;
                    dr["CacheExpiresDate"] = System.DateTime.Now.AddDays(cacheDay);
                    dr["PatchNo"] = patchNo;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public void TableValuedToDB(List<PolicyCache> policyList, int patchNo, int cacheDay)
        {
            patchNo = 0;
            DataTable dt = GetTable(policyList, patchNo, cacheDay);
            SqlConnection sqlConn = new SqlConnection(strConnectionString);
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("INSERT INTO [dbo].[PolicyCache]([_id],[PolicyId],[PlatformCode],[CarrierCode],[CabinSeatCode],[FromCityCode],[MidCityCode],[ToCityCode],[SuitableFlightNo],[ExceptedFlightNo],[SuitableWeek],[CheckinTime_FromTime],[CheckinTime_EndTime],[IssueTime_FromTime],[IssueTime_EndTime],[ServiceTime_WeekendTime_FromTime],[ServiceTime_WeekendTime_EndTime],[ServiceTime_WeekTime_FromTime],[ServiceTime_WeekTime_EndTime],[TFGTime_WeekendTime_FromTime],[TFGTime_WeekendTime_EndTime],[TFGTime_WeekTime_FromTime],[TFGTime_WeekTime_EndTime],[Remark],[TravelType],[PolicyType],[Point],[CacheDate],[CacheExpiresDate],[PatchNo]) ");
            sbSQL.Append(" select  t.[_id],t.[PolicyId],t.[PlatformCode],t.[CarrierCode],t.[CabinSeatCode],t.[FromCityCode],t.[MidCityCode],t.[ToCityCode],t.[SuitableFlightNo],t.[ExceptedFlightNo],t.[SuitableWeek],t.[CheckinTime_FromTime],t.[CheckinTime_EndTime],t.[IssueTime_FromTime],t.[IssueTime_EndTime],t.[ServiceTime_WeekendTime_FromTime],t.[ServiceTime_WeekendTime_EndTime],t.[ServiceTime_WeekTime_FromTime],t.[ServiceTime_WeekTime_EndTime],t.[TFGTime_WeekendTime_FromTime],t.[TFGTime_WeekendTime_EndTime],t.[TFGTime_WeekTime_FromTime],t.[TFGTime_WeekTime_EndTime],t.[Remark],t.[TravelType],t.[PolicyType],t.[Point],t.[CacheDate],t.[CacheExpiresDate],t.[PatchNo] from @Tvp AS t");
            SqlCommand cmd = new SqlCommand(sbSQL.ToString(), sqlConn);
            SqlParameter catParam = cmd.Parameters.AddWithValue("@Tvp", dt);
            catParam.SqlDbType = SqlDbType.Structured;
            //表值参数的名字叫BulkUdt
            catParam.TypeName = "dbo.BulkUdt";
            try
            {
                if (dt != null && dt.Rows.Count != 0)
                {
                    sqlConn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();
                }
            }
        }
        /// <summary>
        /// 删除重复的政策或者过期的政策
        /// </summary>
        public void ExecRepeatSQL()
        {
            try
            {
                //去掉重复或者过期的政策的
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("delete dbo.PolicyCache  where CacheExpiresDate is not null and CacheExpiresDate <getdate()");
                //删除过期的
                int exec = ExecuteSQL(sbSql.ToString());
                //删除重复的
                sbSql.Append(" delete  from  dbo.PolicyCache where PolicyId in( ");
                sbSql.Append(" select PolicyId from dbo.PolicyCache   group by PolicyId having COUNT(PolicyId)>1 ");
                sbSql.Append(" ) and _id not in(select MAX(_id) from dbo.PolicyCache   group by PolicyId having COUNT(PolicyId)>1) ");
                //sbSql.Append(" or (CacheExpiresDate is not null and CacheExpiresDate <getdate())");
                exec = ExecuteSQL(sbSql.ToString());
                if (exec < 0)
                {
                    exec = ExecuteSQL(sbSql.ToString());
                }
            }
            catch (Exception ex)
            {
                JoveZhao.Framework.Logger.WriteLog(JoveZhao.Framework.LogType.ERROR, "删除重复的政策或者过期的政策[ExecRepeatSQL]", ex);
            }
        }
        #endregion
    }
}

