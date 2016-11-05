using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    [Serializable]
    public class AVHModel
    {
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string fromCityCode = string.Empty;
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string toCityCode = string.Empty;
        /// <summary>
        /// 起飞日期
        /// </summary>
        public string strDate = string.Empty;
        /// <summary>
        /// avh结果列表
        /// </summary>
        public List<AVH> avhList = new List<AVH>();
    }


    [Serializable]
    public class AVH
    {
        private string _SerialNum = string.Empty;
        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNum
        {
            get { return _SerialNum; }
            set { _SerialNum = value; }
        }

        private string _AirCodeAndFlight = string.Empty;
        /// <summary>
        /// 承运人航班号 如：MU5848
        /// </summary>
        public string AirCodeAndFlight
        {
            get { return _AirCodeAndFlight; }
            set { _AirCodeAndFlight = value; }
        }

        private string _ModifyIdentity = string.Empty;
        /// <summary>
        /// 连接协议级别如 “DS#”  "AS#"
        /// </summary>
        public string ModifyIdentity
        {
            get { return _ModifyIdentity; }
            set { _ModifyIdentity = value; }
        }

        private List<SeatModel> _SeatList = new List<SeatModel>();

        /// <summary>
        /// 舱位列表
        /// </summary>
        public List<SeatModel> SeatList
        {
            get { return _SeatList; }
            set { _SeatList = value; }
        }

        private string _fromCityCode = string.Empty;
        /// <summary>
        /// 出发城市 CTU        
        /// </summary>
        public string FromCityCode
        {
            get { return _fromCityCode; }
            set { _fromCityCode = value; }
        }
        private string _toCityCode = string.Empty;
        /// <summary>
        /// 到达城市 PEK
        /// </summary>
        public string ToCityCode
        {
            get { return _toCityCode; }
            set { _toCityCode = value; }
        }
        private string _CityDoubleCode = string.Empty;
        /// <summary>
        /// 城市对 CTUPEK
        /// </summary>
        public string CityDoubleCode
        {
            get { return _CityDoubleCode; }
            set { _CityDoubleCode = value; }
        }

        private string _StartDate = string.Empty;
        /// <summary>
        /// 起飞日期
        /// </summary>
        public string StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }
        private string _EndDate = string.Empty;
        /// <summary>
        /// 到达日期
        /// </summary>
        private string _StartTime = string.Empty;
        /// <summary>
        /// 起飞时间
        /// </summary>
        public string StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        private string _EndTime = string.Empty;
        /// <summary>
        /// 到达时间
        /// </summary>
        public string EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }

        private string _FlyModel = string.Empty;
        /// <summary>
        /// 机型 737
        /// </summary>
        public string FlyModel
        {
            get { return _FlyModel; }
            set { _FlyModel = value; }
        }
        private string _FlyTime = string.Empty;
        /// <summary>
        /// 飞行时间 1:30
        /// </summary>
        public string FlyTime
        {
            get { return _FlyTime; }
            set { _FlyTime = value; }
        }

        private string _FromDeparture = string.Empty;
        /// <summary>
        /// 起飞航站楼 如T1
        /// </summary>
        public string FromDeparture
        {
            get { return _FromDeparture; }
            set { _FromDeparture = value; }
        }

        private string _ToDeparture = string.Empty;
        /// <summary>
        /// 到达航站楼 如T2
        /// </summary>
        public string ToDeparture
        {
            get { return _ToDeparture; }
            set { _ToDeparture = value; }
        }

        private bool _IsStop = false;

        /// <summary>
        /// 是否经停 true经停 false不经停(即直达)
        /// </summary>
        public bool IsStop
        {
            get { return _IsStop; }
            set { _IsStop = value; }
        }
        private bool _IsMeals = false;
        /// <summary>
        /// 是否餐食 true是 false不是
        /// </summary>
        public bool IsMeals
        {
            get { return _IsMeals; }
            set { _IsMeals = value; }
        }

        private string _strMeals = string.Empty;
        /// <summary>
        /// 餐饮符号 如 S M L R V
        /// </summary>
        public string StrMeals
        {
            get { return _strMeals; }
            set { _strMeals = value; }
        }

        private string _Identity = "E";
        /// <summary>
        /// 电子客票标识
        /// </summary>
        public string Identity
        {
            get { return _Identity; }
            set { _Identity = value; }
        }
    }
    [Serializable]
    public class SeatModel
    {
        /// <summary>
        /// 普通舱位 如A Y  F
        /// </summary>
        public string SeatChar = string.Empty;
        /// <summary>
        /// 子舱位 M1
        /// </summary>
        public string ChildSeat = string.Empty;

        /// <summary>
        /// 剩余舱位个数
        /// </summary>
        public string SeatNum = "0";

        /// <summary>
        /// 舱位名称 ：如经济舱  公务舱 商务舱
        /// </summary>
        public string SeatName = string.Empty;
        /// <summary>
        /// 舱位级别 暂无
        /// </summary>
        public string SeatLevel = string.Empty;
        /// <summary>
        /// 舱位类型
        /// </summary>
        public string SeatType = string.Empty;


    }
}

