using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 行程单数据
    /// </summary>
    public class TravelAppRequrst
    {
        private string m_strTicketNumber = string.Empty;
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            get
            {
                return m_strTicketNumber;
            }
            set
            {
                m_strTicketNumber = value;
            }
        }

        private string m_strTripNumber = string.Empty;
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TripNumber
        {
            get
            {
                return m_strTripNumber;
            }
            set
            {
                m_strTripNumber = value;
            }
        }
        private string m_CreateOffice = string.Empty;
        /// <summary>
        /// 创建或者作废行程单的Office
        /// </summary>
        public string CreateOffice
        {
            get
            {
                return m_CreateOffice;
            }
            set
            {
                m_CreateOffice = value;
            }
        }
        private string m_OrderId = string.Empty;
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId
        {
            get { return m_OrderId; }
            set { m_OrderId = value; }
        }
        private int m_PassengerId = -1;
        /// <summary>
        /// 操作的乘客ID
        /// </summary>
        public int PassengerId
        {
            get { return m_PassengerId; }
            set { m_PassengerId = value; }
        }
        /// <summary>
        /// 订单类型（正常订单：0，售后订单：1或其他）
        /// </summary>
        public int Flag { get; set; }
    }

    /// <summary>
    /// 行程单返回数据
    /// </summary>
    public class TravelAppResponse
    {
        private string m_ShowMsg = string.Empty;
        /// <summary>
        /// 创建作废显示信息
        /// </summary>
        public string ShowMsg
        {
            get
            {
                return m_ShowMsg;
            }
            set
            {
                m_ShowMsg = value;
            }
        }

        private bool m_IsSuc = false;
        /// <summary>
        /// 创建或者作废是否成功
        /// </summary>
        public bool IsSuc
        {
            get
            {
                return m_IsSuc;
            }
            set
            {
                m_IsSuc = value;
            }
        }


        private string m_strTicketNumber = string.Empty;
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            get
            {
                return m_strTicketNumber;
            }
            set
            {
                m_strTicketNumber = value;
            }
        }

        private string m_strTripNumber = string.Empty;
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TripNumber
        {
            get
            {
                return m_strTripNumber;
            }
            set
            {
                m_strTripNumber = value;
            }
        }

        private string m_strPnrAnalysisTripNumber = string.Empty;
        /// <summary>
        /// 票号中解析出来的行程单号 包括创建或者作废
        /// </summary>
        public string PnrAnalysisTripNumber
        {
            get
            {
                return m_strPnrAnalysisTripNumber;
            }
            set
            {
                m_strPnrAnalysisTripNumber = value;
            }
        }
        private string m_CreateOffice = string.Empty;
        /// <summary>
        /// 创建或者作废行程单的Office
        /// </summary>
        public string CreateOffice
        {
            get
            {
                return m_CreateOffice;
            }
            set
            {
                m_CreateOffice = value;
            }
        }       
    }
}
