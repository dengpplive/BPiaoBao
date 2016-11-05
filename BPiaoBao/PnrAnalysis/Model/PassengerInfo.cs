using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 乘机人信息
    /// </summary>
    /// 
    [Serializable]
    public class PassengerInfo
    {
        private string _SerialNum = string.Empty;
        /// <summary>
        /// 乘客序号
        /// </summary>
        public string SerialNum
        {
            get { return _SerialNum; }
            set { _SerialNum = value; }
        }

        private string _YinToAdltNum = string.Empty;
        /// <summary>
        ///婴儿对应成人序号
        /// </summary>
        public string YinToAdltNum
        {
            get { return _YinToAdltNum; }
            set { _YinToAdltNum = value; }
        }
        private string _YinToINFTNum = string.Empty;
        /// <summary>
        /// 婴儿INFT项序号
        /// </summary>
        public string YinToINFTNum
        {
            get { return _YinToINFTNum; }
            set { _YinToINFTNum = value; }
        }

        private string _YinToINFTName = string.Empty;
        /// <summary>
        /// INFT项婴儿姓名 为英文
        /// </summary>
        public string YinToINFTName
        {
            get { return _YinToINFTName; }
            set { _YinToINFTName = value; }
        }

        private string _YinToLegNum = string.Empty;
        /// <summary>
        /// 婴儿对应航段序号
        /// </summary>
        public string YinToLegNum
        {
            get { return _YinToLegNum; }
            set { _YinToLegNum = value; }
        }

        private string _YinerBirthday = string.Empty;
        /// <summary>
        ///婴儿出生日期如：30JUL12
        /// </summary>
        public string YinerBirthday
        {
            get { return _YinerBirthday; }
            set { _YinerBirthday = value; }
        }

        private string _YinerBirthdayDate = string.Empty;
        /// <summary>
        ///婴儿出生日期如：2012-07-30
        /// </summary>
        public string YinerBirthdayDate
        {
            get { return _YinerBirthdayDate; }
            set { _YinerBirthdayDate = value; }
        }


        private string _PassengerName = string.Empty;
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName
        {
            get { return _PassengerName; }
            set { _PassengerName = value; }
        }

        private string _PassengerNameEnsh = string.Empty;
        /// <summary>
        /// 乘客姓名 英文全拼大写
        /// </summary>
        public string PassengerNameEnsh
        {
            get { return _PassengerNameEnsh; }
            set { _PassengerNameEnsh = value; }
        }

        private string _PassengerType = "0";
        /// <summary>
        /// 成人1 儿童2 婴儿3
        /// </summary>
        public string PassengerType
        {
            get { return _PassengerType; }
            set { _PassengerType = value; }
        }

        private string _SsrCardID = string.Empty;
        /// <summary>
        /// 乘机人证件号
        /// </summary>
        public string SsrCardID
        {
            get { return _SsrCardID; }
            set { _SsrCardID = value; }
        }

        private string _ssrCardIDSerialNum = string.Empty;
        /// <summary>
        /// 乘机人证件号所在PNR序号 多个用","隔开 如：1,2,3
        /// </summary>
        public string SsrCardIDSerialNum
        {
            get { return _ssrCardIDSerialNum; }
            set { _ssrCardIDSerialNum = value; }
        }

        private string _TicketNum = string.Empty;
        /// <summary>
        /// 乘机人票号
        /// </summary>
        public string TicketNum
        {
            get { return _TicketNum; }
            set { _TicketNum = value; }
        }

        private bool _PassengerNameIsCorrent = true;
        /// <summary>
        /// 乘机人名字是否正确 true正确 false不正确 
        /// </summary>
        public bool PassengerNameIsCorrent
        {
            get
            {
                return _PassengerNameIsCorrent;
            }
            set
            {
                _PassengerNameIsCorrent = value;
            }
        }
        //CTCM
        private string _PassengerTel = string.Empty;
        /// <summary>
        /// 乘客联系方式
        /// </summary>
        public string PassengerTel
        {
            get
            {
                return _PassengerTel;
            }
            set
            {
                _PassengerTel = value;
            }
        }

    }
}
