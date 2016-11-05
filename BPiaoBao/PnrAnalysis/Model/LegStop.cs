using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 经停信息
    /// </summary>
    /// 
    [Serializable]
    public class LegStop
    {
        private string _CarrayCode;
        /// <summary>
        /// 承运人 CA
        /// </summary>
        public string CarrayCode
        {
            get { return _CarrayCode; }
            set { _CarrayCode = value; }
        }
        private string _CarrayName;
        /// <summary>
        /// 承运人名称
        /// </summary>
        public string CarrayName
        {
            get { return _CarrayName; }
            set { _CarrayName = value; }
        }
        private string _AirShortName;
        /// <summary>
        /// 承运人简称
        /// </summary>
        public string AirShortName
        {
            get { return _AirShortName; }
            set { _AirShortName = value; }
        }

        private string _FlightNum;
        /// <summary>
        /// 航班号 1406
        /// </summary>
        public string FlightNum
        {
            get { return _FlightNum; }
            set { _FlightNum = value; }
        }
        private string _CarrayCodeFlightNum;
        /// <summary>
        /// 承运人航班号 CA1406
        /// </summary>
        public string CarrayCodeFlightNum
        {
            get { return _CarrayCodeFlightNum; }
            set { _CarrayCodeFlightNum = value; }
        }

        private string _StrStopDate;
        /// <summary>
        /// 经停日期字符串 12NOV12
        /// </summary>
        public string StrStopDate
        {
            get { return _StrStopDate; }
            set { _StrStopDate = value; }
        }
        private string _StopDate;
        /// <summary>
        /// 经停日期 2012-12-12
        /// </summary>
        public string StopDate
        {
            get { return _StopDate; }
            set { _StopDate = value; }
        }
        private string _Model;
        /// <summary>
        /// 机型 731
        /// </summary>
        public string Model
        {
            get { return _Model; }
            set { _Model = value; }
        }
        private string _FromCityCode;
        /// <summary>
        /// 出发城市三字码 CTU
        /// </summary>
        public string FromCityCode
        {
            get { return _FromCityCode; }
            set { _FromCityCode = value; }
        }
        private string _FromCity;
        /// <summary>
        /// 出发城市名称
        /// </summary>
        public string FromCity
        {
            get { return _FromCity; }
            set { _FromCity = value; }
        }

        private string _MiddleCityCode;
        /// <summary>
        /// 经停城市(即中专城市)三字码 HGH
        /// </summary>
        public string MiddleCityCode
        {
            get { return _MiddleCityCode; }
            set { _MiddleCityCode = value; }
        }
        private string _MiddleCity;
        /// <summary>
        /// 经停城市(即中专城市)
        /// </summary>
        public string MiddleCity
        {
            get { return _MiddleCity; }
            set { _MiddleCity = value; }
        }

        private string _ToCityCode;
        /// <summary>
        /// 到达城市三字码 PEK
        /// </summary>
        public string ToCityCode
        {
            get { return _ToCityCode; }
            set { _ToCityCode = value; }
        }
        private string _ToCity;
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCity
        {
            get { return _ToCity; }
            set { _ToCity = value; }
        }
        private string _FromTime;
        /// <summary>
        /// 出发时间 08:00
        /// </summary>
        public string FromTime
        {
            get { return _FromTime; }
            set { _FromTime = value; }
        }
        private string _MiddleTime1;
        /// <summary>
        /// 中专城市时间1 12:00
        /// </summary>
        public string MiddleTime1
        {
            get { return _MiddleTime1; }
            set { _MiddleTime1 = value; }
        }
        private string _MiddleTime2;
        /// <summary>
        /// 中专城市时间2 13:00
        /// </summary>
        public string MiddleTime2
        {
            get { return _MiddleTime2; }
            set { _MiddleTime2 = value; }
        }
        private string _ToTime;
        /// <summary>
        /// 到达时间 15:00
        /// </summary>
        public string ToTime
        {
            get { return _ToTime; }
            set { _ToTime = value; }
        }
    }
}
