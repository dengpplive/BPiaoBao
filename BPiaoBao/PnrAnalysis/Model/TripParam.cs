using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 行程单操作参数
    /// </summary>
    /// 
    [Serializable]
    public class TripParam
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
        private string m_Office = string.Empty;
        /// <summary>
        /// 创建行程单的Office
        /// </summary>
        public string Office
        {
            get
            {
                return m_Office;
            }
            set
            {
                m_Office = value;
            }
        }
        private bool _CreateDetrPnrAnalysis = false;
        /// <summary>
        /// 创建行程单解析行程单号是否成功
        /// </summary>
        public bool CreateDetrPnrAnalysis
        {
            get
            {
                return _CreateDetrPnrAnalysis;
            }
            set
            {
                _CreateDetrPnrAnalysis = value;
            }
        }

        private string m_Msg = string.Empty;
        /// <summary>
        /// 创建行程单错误信息
        /// </summary>
        public string Msg
        {
            get
            {
                return m_Msg;
            }
            set
            {
                m_Msg = value;
            }
        }

        private int m_insType = 0;
        /// <summary>
        /// 0创建行程单 1作废行程单
        /// </summary>
        public int InsType
        {
            get
            {
                return m_insType;
            }
            set
            {
                m_insType = value;
            }
        }

        private bool m_InsIsEndOffice = false;
        /// <summary>
        /// 发送指令后面是否跟随Office默认不接
        /// </summary>
        public bool InsIsEndOffice
        {
            get
            {
                return m_InsIsEndOffice;
            }
            set
            {
                m_InsIsEndOffice = value;
            }
        }


        private int m_UseIns = 0;
        /// <summary>
        /// 使用指令方式 0原指令(prinv) 1新指令(pass) 2原指令(prinv)新指令(pass)一起使用
        /// </summary>
        public int UseIns
        {
            get
            {
                return m_UseIns;
            }
            set
            {
                m_UseIns = value;
            }
        }

        private List<DetrInfo> m_DetrList = new List<DetrInfo>();
        /// <summary>
        /// 票号和行程单号信息
        /// </summary>
        public List<DetrInfo> DetrList
        {
            get
            {
                return m_DetrList;
            }
            set
            {
                m_DetrList = value;
            }
        }

        private int m_InsSource = 0;
        /// <summary>
        /// 指令来源 0网站 1行程单软件客户端
        /// </summary>
        public int InsSource
        {
            get
            {
                return m_InsSource;
            }
            set
            {
                m_InsSource = value;
            }
        }
    }
}
