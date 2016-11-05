using System;
using GalaSoft.MvvmLight;
using BPiaoBao.Client.UIExt;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    public class BuySingleInsuranceModel : ObservableObject
    {
        #region 航班日期

        private const string FlightStartDatePropertyName = "FlightStartDate";
        private DateTime _flightStartDate = DateTime.Now.Date;

        /// <summary>
        /// 航班日期
        /// </summary>
        public DateTime FlightStartDate
        {
            get { return _flightStartDate; }
            set
            {
                if (_flightStartDate == value) return;
                RaisePropertyChanging(FlightStartDatePropertyName);
                _flightStartDate = value;
                RaisePropertyChanged(FlightStartDatePropertyName);
            }
        }
        #endregion

        #region  航班号

        private const string FlightNumberPropertyName = "FlightNumber";
        private string _flightNumber;

        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber
        {
            get { return _flightNumber; }
            set
            {
                if (_flightNumber == value) return;
                RaisePropertyChanging(FlightNumberPropertyName);
                _flightNumber = value;
                RaisePropertyChanged(FlightNumberPropertyName);
            }
        }
        #endregion

        #region    PNR 

        private const string PnrPropertyName = "PNR";
        private string _pnr;

        /// <summary>
        /// PNR
        /// </summary>
        public string PNR
        {
            get { return _pnr; }
            set
            {
                if (_pnr == value) return;
                RaisePropertyChanging(PnrPropertyName);
                _pnr = value;
                RaisePropertyChanged(PnrPropertyName);
            }
        }
        #endregion

        #region 被投保人姓名

        private const string NamePropertyName = "Name";
        private string _name;

        /// <summary>
        /// 被投保人姓名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                RaisePropertyChanging(NamePropertyName);
                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }

        #endregion

        #region  被投保人性别

        private const string GenderProprtyName = "Gender";
        private bool _gender;
        /// <summary>
        /// 被投保人性别
        /// </summary>
        public bool Gender
        {
            get { return _gender; }
            set
            {
                if (_gender == value) return;
                RaisePropertyChanging(GenderProprtyName);
                _gender = value;
                RaisePropertyChanged(GenderProprtyName);
            }
        }

        #endregion

        #region   被投保人类型(是否成人类型)

        private const string IsAdultTypeProprtyName = "IsAdultType";
        private bool _isAdultType;
        /// <summary>
        ///  被投保人类型(是否成人类型)
        /// </summary>
        public bool IsAdultType
        {
            get { return _isAdultType; }
            set
            {
                if (_isAdultType == value) return;
                RaisePropertyChanging(IsAdultTypeProprtyName);
                _isAdultType = value;
                IsIdType = _isAdultType;
                ResetDate();
                RaisePropertyChanged(IsAdultTypeProprtyName);
            }
        }

        #endregion

        #region   被投保人类型(是否儿童类型)

        private const string IsChildTypeProprtyName = "IsChildType";
        private bool _isChildType;
        /// <summary>
        ///  被投保人类型(是否儿童类型)
        /// </summary>
        public bool IsChildType
        {
            get { return _isChildType; }
            set
            {
                if (_isChildType == value) return;
                RaisePropertyChanging(IsChildTypeProprtyName);
                _isChildType = value;
                IsOtherType = _isChildType && !IsOtherType || IsOtherType;
                ResetDate();
                RaisePropertyChanged(IsChildTypeProprtyName);
            }
        }

        #endregion

        #region   被投保人类型(是否婴儿类型)

        private const string IsBabyTypeProprtyName = "IsBabyType";
        private bool _isBabyType;
        /// <summary>
        ///  被投保人类型(是否婴儿类型)
        /// </summary>
        public bool IsBabyType
        {
            get { return _isBabyType; }
            set
            {
                if (_isBabyType == value) return;
                RaisePropertyChanging(IsBabyTypeProprtyName);
                _isBabyType = value;
                IsOtherType = _isBabyType && !IsOtherType || IsOtherType;
                ResetDate();
                RaisePropertyChanged(IsBabyTypeProprtyName);
            }
        }

        #endregion

        #region  被投保人证件类型(是否身份证类型)

        private const string IsIdTypeProprtyName = "IsIdType";
        private bool _isIdType;
        /// <summary>
        /// 被投保人证件类型(是否身份证类型)
        /// </summary>
        public bool IsIdType
        {
            get { return _isIdType; }
            set
            {
                if (_isIdType == value) return;
                RaisePropertyChanging(IsIdTypeProprtyName);
                _isIdType = value;
                RaisePropertyChanged(IsIdTypeProprtyName);
            }
        }
         
        #endregion

        #region  被投保人证件类型(是否军人证类型)

        private const string IsMilitaryIdTypeProprtyName = "IsMilitaryIdType";
        private bool _isMilitaryIdType;
        /// <summary>
        /// 被投保人证件类型(是否军人证类型)
        /// </summary>
        public bool IsMilitaryIdType
        {
            get { return _isMilitaryIdType; }
            set
            {
                if (_isMilitaryIdType == value) return;
                RaisePropertyChanging(IsMilitaryIdTypeProprtyName);
                _isMilitaryIdType = value;
                RaisePropertyChanged(IsMilitaryIdTypeProprtyName);
            }
        }

        #endregion

        #region  被投保人证件类型(是否护照类型)

        private const string IsPassportIdTypeProprtyName = "IsPassportIdType";
        private bool _isPassportIdType;
        /// <summary>
        /// 被投保人证件类型(是否护照类型)
        /// </summary>
        public bool IsPassportIdType
        {
            get { return _isPassportIdType; }
            set
            {
                if (_isPassportIdType == value) return;
                RaisePropertyChanging(IsPassportIdTypeProprtyName);
                _isPassportIdType = value;
                RaisePropertyChanged(IsPassportIdTypeProprtyName);
            }
        }

        #endregion 

        #region  被投保人证件类型(是否其它类型)

        private const string IsOtherTypeProprtyName = "IsOtherType";
        private bool _isOtherType;
        /// <summary>
        /// 被投保人证件类型(是否其它类型)
        /// </summary>
        public bool IsOtherType
        {
            get { return _isOtherType; }
            set
            {
                if (_isOtherType == value) return;
                RaisePropertyChanging(IsOtherTypeProprtyName);
                _isOtherType = value;
                RaisePropertyChanged(IsOtherTypeProprtyName);
            }
        }

        #endregion

        #region  被投保人证件号码

        private const string IdNoProprtyName = "IdNo";
        private string _idNo;
        /// <summary>
        /// 被投保人证件号码
        /// </summary>
        public string IdNo
        {
            get { return _idNo; }
            set
            {
                if (_idNo == value) return;
                RaisePropertyChanging(IdNoProprtyName);
                _idNo = value;
                if (IsIdType && IdNo.Trim().Length == 18)
                {
                    //生日转换截取字符串
                    var birth = Common.ExtHelper.GetBirthdayDateFromString(IdNo);
                    if (birth != null) BirthDay = birth;
                }
                else
                    ResetDate();
                RaisePropertyChanged(IdNoProprtyName);
            }
        }
        #endregion

        #region  被投保人出生日期

        private const string BirthDayPropertyName = "BirthDay";
        private DateTime? _birthDay;

        /// <summary>
        /// 被投保人出生日期
        /// </summary>
        public DateTime? BirthDay
        {
            get { return _birthDay; }
            set
            {
                if (_birthDay == value) return;
                RaisePropertyChanging(BirthDayPropertyName);
                _birthDay = value;
                RaisePropertyChanged(BirthDayPropertyName);
            }
        }
        #endregion

        #region  手机号码

        private const string MobilePropertyName = "Mobile";
        private string _mobile;
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile
        {
            get { return _mobile; }
            set
            {
                if (_mobile == value) return;
                RaisePropertyChanging(MobilePropertyName);
                _mobile = value;
                RaisePropertyChanged(MobilePropertyName);
            }
        }

        #endregion

        #region  保险份数

        private const string InsuranceCountPropertyName = "InsuranceCount";
        private int _insuranceCount;
        /// <summary>
        /// 保险份数
        /// </summary>
        public int InsuranceCount
        {
            get { return _insuranceCount; }
            set
            {
                if (_insuranceCount == value) return;
                RaisePropertyChanging(InsuranceCountPropertyName);
                _insuranceCount = value;
                RaisePropertyChanged(InsuranceCountPropertyName);
            }
        }
        #endregion

        #region  保额

        private const string SumInsuredPropertyName = "SumInsured";
        private decimal _sumInsured;
        /// <summary>
        /// 保额
        /// </summary>
        public decimal SumInsured
        {
            get { return _sumInsured; }
            set
            {
                if (_sumInsured == value) return;
                RaisePropertyChanging(SumInsuredPropertyName);
                _sumInsured = value;
                RaisePropertyChanged(SumInsuredPropertyName);
            }
        }
        #endregion

        #region  到达城市名称

        private const string ToCityNameProprtyName = "ToCityName";
        private string _toCityName;
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCityName
        {
            get { return _toCityName; }
            set
            {
                if (_toCityName == value) return;
                RaisePropertyChanging(ToCityNameProprtyName);
                _toCityName = value;
                RaisePropertyChanged(ToCityNameProprtyName);
            }
        }
        #endregion

        #region DisplayDateEnd

        private const string CDisplayDateEndPropertyName = "DisplayDateEnd";

        private DateTime _displayDateEnd;

        /// <summary>
        /// DisplayDateEnd
        /// </summary>        
        public DateTime DisplayDateEnd
        {
            get { return _displayDateEnd; }

            set
            {
                if (_displayDateEnd == value) return;
                RaisePropertyChanging(CDisplayDateEndPropertyName);
                _displayDateEnd = value;
                RaisePropertyChanged(CDisplayDateEndPropertyName);
            }
        }
        #endregion

        #region DisplayDateStart

        private const string CDisplayDateStartPropertyName = "DisplayDateStart";

        private DateTime _displayDateStart;
        //private DateTime _startDateTime;

        /// <summary>
        /// DisplayDateStart
        /// </summary>        
        public DateTime DisplayDateStart
        {
            get { return _displayDateStart; }

            set
            {
                if (_displayDateStart == value) return;
                RaisePropertyChanging(CDisplayDateStartPropertyName);
                _displayDateStart = value;
                RaisePropertyChanged(CDisplayDateStartPropertyName);
            }
        }
        #endregion

        /// <summary>
        /// 初始化出生日期
        /// </summary>
        private void ResetDate()
        {
            if (IsAdultType)
            {
                DisplayDateStart = FlightStartDate.Date.AddYears(-100);
                DisplayDateEnd = FlightStartDate.Date.AddYears(-12).AddDays(1);
                if (BirthDay != null && BirthDay > DisplayDateStart && BirthDay <= DisplayDateEnd) return;
                BirthDay = DisplayDateEnd;
            }
            else if (IsChildType)
            {
                DisplayDateStart = FlightStartDate.Date.AddYears(-12).AddDays(1);
                DisplayDateEnd = FlightStartDate.Date.AddYears(-2);
                if (BirthDay != null && BirthDay > DisplayDateStart && BirthDay <= DisplayDateEnd) return;
                BirthDay = DisplayDateStart;
            }
            else if (IsBabyType)
            {
                DisplayDateStart = FlightStartDate.Date.AddYears(-2).AddDays(1);
                DisplayDateEnd = FlightStartDate.Date;
                if (BirthDay != null && BirthDay > DisplayDateStart && BirthDay <= DisplayDateEnd) return;
                BirthDay = DisplayDateStart;
            }
        }
        /// <summary>
        /// 验证input
        /// </summary>
        public bool Check()
        {
            try
            {
                ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(ToCityName, "到达城市");
                ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(FlightNumber, "航班号");
                ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(Name, "乘客姓名");
                if (IsAdultType)
                {
                    if (IsIdType)
                    {
                        ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(IdNo, "证件号");
                        //身份证18位
                        if (IdNo.Trim().Length != 18)
                        {
                            throw new Exception(string.Format("{0}证件号只能{1}位", IdNo, 18));
                        }
                    }
                    //ProjectHelper.Utils.Guard.CheckMobilePhoneNum(Mobile);
                }
                else
                {
                    if (IsChildType && IsIdType)
                    {
                        ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(IdNo, "证件号");
                        if (IsIdType)
                        {   //身份证18位
                            if (IdNo.Trim().Length != 18)
                            {
                                throw new Exception(string.Format("{0}证件号只能{1}位", IdNo, 18));
                            }
                        }
                    }
                }

                if (BirthDay != null && !(BirthDay < DisplayDateStart) && !(BirthDay > DisplayDateEnd))
                {
                    if (!IsIdType || string.IsNullOrWhiteSpace(IdNo) || IdNo.Trim().Length != 18) return true;
                    //生日转换截取字符串
                    var birth = Common.ExtHelper.GetBirthdayDateFromString(IdNo);
                    if (birth != null && (birth < DisplayDateStart || birth > DisplayDateEnd))
                        throw new Exception(string.Format("请核对出生日期范围{0:yyyy-MM-dd}", BirthDay));
                }
                else
                {
                    throw new Exception(string.Format("请核对出生日期范围{0:yyyy-MM-dd}", BirthDay));
                }
                return true;
            }
            catch (Exception ex)
            {
                UIManager.ShowErr(ex);
                return false;
            }
        }
    }
}
