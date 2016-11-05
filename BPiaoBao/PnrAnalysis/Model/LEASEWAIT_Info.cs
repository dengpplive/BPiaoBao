using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis
{
    /// <summary>
    /// 没有取到编码 根据旅客姓名获取编码实体信息
    /// </summary>
    [Serializable]
    public class LEASEWAIT_Info
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

        private string _FlightNum = string.Empty;
        /// <summary>
        /// 航班号 HO1336
        /// </summary>
        public string FlightNum
        {
            get { return _FlightNum; }
            set { _FlightNum = value; }
        }
        private string _GobalDate = string.Empty;
        /// <summary>
        /// 全局日期
        /// </summary>
        public string GobalDate
        {
            get { return _GobalDate; }
            set { _GobalDate = value; }
        }

        private string _CarryCode = string.Empty;
        /// <summary>
        /// 航空公司二字码 CA
        /// </summary>
        public string CarryCode
        {
            get { return _CarryCode; }
            set { _CarryCode = value; }
        }
        private string _FlightNo = string.Empty;
        /// <summary>
        /// 4为航班号 1584
        /// </summary>
        public string FlightNo
        {
            get { return _FlightNo; }
            set { _FlightNo = value; }
        }

        private string _FromCode = string.Empty;
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCode
        {
            get { return _FromCode; }
            set { _FromCode = value; }
        }

        private string _ToCode = string.Empty;
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCode
        {
            get { return _ToCode; }
            set { _ToCode = value; }
        }
        private string _PasPinYing = string.Empty;
        /// <summary>
        /// 乘客姓名拼音 DENGJIYUAN
        /// </summary>
        public string PasPinYing
        {
            get
            {
                return _PasPinYing;
            }
            set
            {
                _PasPinYing = value;
            }
        }
        private string _PasForwardNum = string.Empty;
        /// <summary>
        /// 旅客姓名前的的数字
        /// </summary>
        public string PasForwardNum
        {
            get
            {
                return _PasForwardNum;
            }
            set
            {
                _PasForwardNum = value;
            }
        }

        private string _Pnr = string.Empty;
        /// <summary>
        /// 编码 
        /// </summary>
        public string PNR
        {
            get
            {
                return _Pnr;
            }
            set
            {
                _Pnr = value;
            }
        }

        private string _Seat = string.Empty;
        /// <summary>
        /// 舱位 V
        /// </summary>
        public string Seat
        {
            get
            {
                return _Seat;
            }
            set
            {
                _Seat = value;
            }
        }

        private string _PnrStatus = string.Empty;
        /// <summary>
        /// 编码状态  HK1
        /// </summary>
        public string PnrStatus
        {
            get
            {
                return _PnrStatus;
            }
            set
            {
                _PnrStatus = value;
            }
        }

        private string _PasCount = "0";
        /// <summary>
        /// 乘客人数 3
        /// </summary>
        public string PasCount
        {
            get
            {
                return _PasCount;
            }
            set
            {
                _PasCount = value;
            }
        }

        private string _Office = string.Empty;
        /// <summary>
        /// 解析出来的Office CTU324
        /// </summary>
        public string Office
        {
            get
            {
                return _Office;
            }
            set
            {
                _Office = value;
            }
        }

        private string _dayMonth = string.Empty;
        /// <summary>
        /// 日期 格式: 17JUL
        /// </summary>
        public string dayMonth
        {
            get
            {
                return _dayMonth;
            }
            set
            {
                _dayMonth = value;
            }
        }
    }
}
