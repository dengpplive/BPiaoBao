using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 票号信息
    /// </summary>
    [Serializable]
    public class TicketInfo
    {
        private string _StrGetTciketDate = string.Empty;
        /// <summary>
        /// 提取票号时间 17JUL14
        /// </summary>
        public string StrGetTciketDate
        {
            get { return _StrGetTciketDate; }
            set { _StrGetTciketDate = value; }
        }
        private string _GetTciketDate = string.Empty;
        /// <summary>
        /// 提取票号时间 2014-07-14
        /// </summary>
        public string GetTciketDate
        {
            get { return _GetTciketDate; }
            set { _GetTciketDate = value; }
        }

        private string _TicketNumber = string.Empty;
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            get { return _TicketNumber; }
            set { _TicketNumber = value; }
        }
        private string _FollowTicketNumber = string.Empty;
        /// <summary>
        /// 后续票号
        /// </summary>
        public string FollowTicketNumber
        {
            get { return _FollowTicketNumber; }
            set { _FollowTicketNumber = value; }
        }
        private string _TicketStatus = string.Empty;
        /// <summary>
        /// 票号状态
        /// </summary>
        public string TicketStatus
        {
            get { return _TicketStatus; }
            set { _TicketStatus = value; }
        }

        private string _PrintStatus = string.Empty;
        /// <summary>
        /// 打印行程单状态
        /// </summary>
        public string PrintStatus
        {
            get { return _PrintStatus; }
            set { _PrintStatus = value; }
        }
        private string _IssueOffice = string.Empty;
        /// <summary>
        /// 出票Office
        /// </summary>
        public string IssueOffice
        {
            get { return _IssueOffice; }
            set { _IssueOffice = value; }
        }

        private string _IssueDate = string.Empty;
        /// <summary>
        /// 出票日期
        /// </summary>
        public string IssueDate
        {
            get { return _IssueDate; }
            set { _IssueDate = value; }
        }
        private string _IssueAddress = string.Empty;
        /// <summary>
        /// 出票地点
        /// </summary>
        public string IssueAddress
        {
            get { return _IssueAddress; }
            set { _IssueAddress = value; }
        }
        private string _ORG_DST = string.Empty;
        /// <summary>
        /// 出发到达地
        /// </summary>
        public string ORG_DST
        {
            get { return _ORG_DST; }
            set { _ORG_DST = value; }
        }

        private string _IssueAirLine = string.Empty;
        /// <summary>
        /// 出票航空公司
        /// </summary>
        public string IssueAirLine
        {
            get { return _IssueAirLine; }
            set { _IssueAirLine = value; }
        }
        private string _AirCode = string.Empty;
        /// <summary>
        /// 出票航空公司二字码
        /// </summary>
        public string AirCode
        {
            get { return _AirCode; }
            set { _AirCode = value; }
        }
        private string _SaleInfo = string.Empty;
        /// <summary>
        /// 售票处信息 出票公司|Office|操作员
        /// </summary>
        public string SaleInfo
        {
            get { return _SaleInfo; }
            set { _SaleInfo = value; }
        }

        private string _PassengerName = string.Empty;
        /// <summary>
        /// 旅客姓名
        /// </summary>
        public string PassengerName
        {
            get { return _PassengerName; }
            set { _PassengerName = value; }
        }
        private string _SsrCardID = string.Empty;
        /// <summary>
        /// 旅客证件号
        /// </summary>
        public string SsrCardID
        {
            get { return _SsrCardID; }
            set { _SsrCardID = value; }
        }
        private string _Remark = string.Empty;
        /// <summary>
        /// 签注信息
        /// </summary>
        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }
        private string _UseLimitRemark = string.Empty;
        /// <summary>
        /// 使用限制备注
        /// </summary>
        public string UseLimitRemark
        {
            get { return _UseLimitRemark; }
            set { _UseLimitRemark = value; }
        }

        private string _PayAir = string.Empty;
        /// <summary>
        /// 支付航空公司
        /// </summary>
        public string PayAir
        {
            get { return _PayAir; }
            set { _PayAir = value; }
        }


        private string _Fare = "0";
        /// <summary>
        /// 舱位价
        /// </summary>
        public string Fare
        {
            get { return _Fare; }
            set { _Fare = value; }
        }
        private string _TAX = "0";
        /// <summary>
        /// 机建费
        /// </summary>
        public string TAX
        {
            get { return _TAX; }
            set { _TAX = value; }
        }
        private string _RQFare = "0";
        /// <summary>
        /// 燃油费
        /// </summary>
        public string RQFare
        {
            get { return _RQFare; }
            set { _RQFare = value; }
        }
        private string _PayTotalMoney = "0";
        /// <summary>
        /// 支付总价
        /// </summary>
        public string PayTotalMoney
        {
            get { return _PayTotalMoney; }
            set { _PayTotalMoney = value; }
        }
        private string _CRS_Pnr = string.Empty;
        /// <summary>
        /// B系统小编码
        /// </summary>
        public string CRS_Pnr
        {
            get { return _CRS_Pnr; }
            set { _CRS_Pnr = value; }
        }
        private string _ICS_Pnr = string.Empty;
        /// <summary>
        /// C系统大编码
        /// </summary>
        public string ICS_Pnr
        {
            get { return _ICS_Pnr; }
            set { _ICS_Pnr = value; }
        }
        private List<LegInfo> _TLegInfo = new List<LegInfo>();
        /// <summary>
        /// 票号航段信息
        /// </summary>
        public List<LegInfo> TLegInfo
        {
            get { return _TLegInfo; }
            set { _TLegInfo = value; }
        }
    }
}
