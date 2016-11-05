using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using System;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    public class QueryInsuranceModel : ObservableObject
    {

        #region  投保状态

        private const string EnumInsuranceStatusPropertyName = "EnumInsuranceStatus";
        private EnumInsuranceStatus? _enumInsuranceStatus;
        /// <summary>
        /// 投保状态
        /// </summary>
        public EnumInsuranceStatus? EnumInsuranceStatus
        {
            get { return _enumInsuranceStatus; }
            set
            {
                if (_enumInsuranceStatus == value) return;
                RaisePropertyChanging(EnumInsuranceStatusPropertyName);
                _enumInsuranceStatus = value;
                RaisePropertyChanged(EnumInsuranceStatusPropertyName);
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

        #region 航班日期

        private const string FlyStartTimePropertyName = "FlyStartTime";
        private DateTime? _flyStartTime;

        /// <summary>
        /// 航班日期
        /// </summary>
        public DateTime? FlyStartTime
        {
            get { return _flyStartTime; }
            set
            {
                if (_flyStartTime == value) return;
                RaisePropertyChanging(FlyStartTimePropertyName);
                _flyStartTime = value;
                RaisePropertyChanged(FlyStartTimePropertyName);
            }
        }

        private const string FlyEndTimePropertyName = "FlyEndTime";
        private DateTime? _flyEndTime;

        /// <summary>
        /// 航班日期
        /// </summary>
        public DateTime? FlyEndTime
        {
            get { return _flyEndTime; }
            set
            {
                if (_flyEndTime == value) return;
                RaisePropertyChanging(FlyEndTimePropertyName);
                _flyEndTime = value;
                RaisePropertyChanged(FlyEndTimePropertyName);
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
                RaisePropertyChanged(IdNoProprtyName);
            }
        }
        #endregion

        #region 保险期限

        private const string InsuranceLimitStartTimePropertyName = "InsuranceLimitStartTime";
        private DateTime? _insuranceLimitStartTime;

        /// <summary>
        /// 保险期限
        /// </summary>
        public DateTime? InsuranceLimitStartTime
        {
            get { return _insuranceLimitStartTime; }
            set
            {
                if (_insuranceLimitStartTime == value) return;
                RaisePropertyChanging(InsuranceLimitStartTimePropertyName);
                _insuranceLimitStartTime = value;
                RaisePropertyChanged(InsuranceLimitStartTimePropertyName);
            }
        }

        private const string InsuranceLimitEndTimePropertyName = "InsuranceLimitEndTime";
        private DateTime? _insuranceLimitEndTime;

        /// <summary>
        /// 保险期限
        /// </summary>
        public DateTime? InsuranceLimitEndTime
        {
            get { return _insuranceLimitEndTime; }
            set
            {
                if (_insuranceLimitEndTime == value) return;
                RaisePropertyChanging(InsuranceLimitEndTimePropertyName);
                _insuranceLimitEndTime = value;
                RaisePropertyChanged(InsuranceLimitEndTimePropertyName);
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

        #region 被投保人姓名

        private const string PassengerNamePropertyName = "PassengerName";
        private string _name;

        /// <summary>
        /// 被投保人姓名
        /// </summary>
        public string PassengerName
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                RaisePropertyChanging(PassengerNamePropertyName);
                _name = value;
                RaisePropertyChanged(PassengerNamePropertyName);
            }
        }

        #endregion

        #region 投保时间

        private const string BuyStartTimePropertyName = "BuyStartTime";
        private DateTime? _buyStartTime;

        /// <summary>
        /// 投保时间
        /// </summary>
        public DateTime? BuyStartTime
        {
            get { return _buyStartTime; }
            set
            {
                if (_buyStartTime == value) return;
                RaisePropertyChanging(BuyStartTimePropertyName);
                _buyStartTime = value;
                RaisePropertyChanged(BuyStartTimePropertyName);
            }
        }

        private const string BuyEndTimePropertyName = "BuyEndTime";
        private DateTime? _buyEndTime;

        /// <summary>
        /// 投保时间
        /// </summary>
        public DateTime? BuyEndTime
        {
            get { return _buyEndTime; }
            set
            {
                if (_buyEndTime == value) return;
                RaisePropertyChanging(BuyEndTimePropertyName);
                _buyEndTime = value;
                RaisePropertyChanged(BuyEndTimePropertyName);
            }
        }
        #endregion

        #region 订单号

        private const string OrderIdPropertyName = "OrderId";
        private string _orderId;
        /// <summary>
        /// 保单号
        /// </summary>
        public string OrderId
        {
            get { return _orderId; }
            set
            {
                if (_orderId == value) return;
                RaisePropertyChanging(OrderIdPropertyName);
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }
        #endregion

        #region  保单号

        private const string InsuranceNoPropertyName = "InsuranceNo";
        private string _insuranceNo;
        /// <summary>
        /// 保单号
        /// </summary>
        public string InsuranceNo
        {
            get { return _insuranceNo; }
            set
            {
                if (_insuranceNo == value) return;
                RaisePropertyChanging(InsuranceNoPropertyName);
                _insuranceNo = value;
                RaisePropertyChanged(InsuranceNoPropertyName);
            }
        }
        #endregion

    }
}
