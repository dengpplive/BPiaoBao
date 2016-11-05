using System.ComponentModel;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class FlightInformationItemViewModel : ViewModelBase
    {
        private FlightInfoModel _fi;

        public FlightInfoModel FlightInfoModel
        {
            get { return _fi; }
        }

        #region CarrayShortName

        protected const string c_CarrayShortPropertyNameName = "CarrayShortName";

        protected string _CarrayShortName = null;

        /// <summary>
        /// CarrayShortName
        /// </summary>
        [DisplayName("航空公司")]
        public string CarrayShortName
        {
            get { return _CarrayShortName; }

            set
            {
                if (_CarrayShortName != value)
                {
                    RaisePropertyChanging(c_CarrayShortPropertyNameName);
                    _CarrayShortName = value;
                    RaisePropertyChanged(c_CarrayShortPropertyNameName);
                }
            }
        }

        #endregion

        #region CarrayCode
        protected const string c_CarrayCodePropertyName = "CarrayCode";

        protected string _CarrayCode;

        /// <summary>
        /// CarrayCode
        /// </summary>        
        [DisplayName("CarrayCode")]
        public string CarrayCode
        {
            get { return _CarrayCode; }

            set
            {
                if (_CarrayCode != value)
                {
                    RaisePropertyChanging(c_CarrayCodePropertyName);
                    _CarrayCode = value;
                    RaisePropertyChanged(c_CarrayCodePropertyName);
                }
            }
        }
        #endregion

        #region FlightNumber

        protected const string c_FlightNumberPropertyName = "FlightNumber";

        protected string _FlightNumber = null;

        /// <summary>
        /// FlightNumber
        /// </summary>
        [DisplayName("航空类型")]
        public string FlightNumber
        {
            get { return _FlightNumber; }

            set
            {
                if (_FlightNumber != value)
                {
                    RaisePropertyChanging(c_FlightNumberPropertyName);
                    _FlightNumber = value;
                    RaisePropertyChanged(c_FlightNumberPropertyName);
                }
            }
        }

        #endregion

        #region Model

        protected const string c_ModelPropertyName = "Model";

        protected string _Model = null;

        /// <summary>
        /// Model
        /// </summary>
        public string Model
        {
            get { return _Model; }

            set
            {
                if (_Model != value)
                {
                    RaisePropertyChanging(c_ModelPropertyName);
                    _Model = value;
                    RaisePropertyChanged(c_ModelPropertyName);
                }
            }
        }

        #endregion

        #region People

        protected const string c_PeoplePropertyName = "People";

        protected string _People = null;

        /// <summary>
        /// People
        /// </summary>
        public string People
        {
            get { return _People; }

            set
            {
                if (_People != value)
                {
                    RaisePropertyChanging(c_PeoplePropertyName);
                    _People = value;
                    RaisePropertyChanged(c_PeoplePropertyName);
                }
            }
        }

        #endregion

        #region TaxFee

        protected const string c_TaxFeePropertyName = "TaxFee";

        protected decimal _TaxFee;

        /// <summary>
        /// TaxFee
        /// </summary>
        public decimal TaxFee
        {
            get { return _TaxFee; }

            set
            {
                if (_TaxFee != value)
                {
                    RaisePropertyChanging(c_TaxFeePropertyName);
                    _TaxFee = value;
                    RaisePropertyChanged(c_TaxFeePropertyName);
                }
            }
        }

        #endregion

        #region RQFee

        protected const string c_RQFeePropertyName = "RQFee";

        protected decimal _RQFee;

        /// <summary>
        /// RQFee
        /// </summary>
        public decimal RQFee
        {
            get { return _RQFee; }

            set
            {
                if (_RQFee != value)
                {
                    RaisePropertyChanging(c_RQFeePropertyName);
                    _RQFee = value;
                    RaisePropertyChanged(c_RQFeePropertyName);
                }
            }
        }

        #endregion

        #region SeatCode
        protected const string c_SeatCodePropertyName = "SeatCode";

        protected string _SeatCode;

        /// <summary>
        /// SeatCode
        /// </summary>
        public string SeatCode
        {
            get { return _SeatCode; }

            set
            {
                if (_SeatCode != value)
                {
                    RaisePropertyChanging(c_SeatCodePropertyName);
                    _SeatCode = value;
                    RaisePropertyChanged(c_SeatCodePropertyName);
                }
            }
        }
        #endregion

        #region Discount
        protected const string c_DiscountPropertyName = "Discount";

        protected decimal _Discount;

        /// <summary>
        /// Discount
        /// </summary>
        public decimal Discount
        {
            get { return _Discount; }

            set
            {
                if (_Discount != value)
                {
                    RaisePropertyChanging(c_DiscountPropertyName);
                    _Discount = value;
                    RaisePropertyChanged(c_DiscountPropertyName);
                }
            }
        }
        #endregion

        #region SeatCount
        protected const string c_SeatCountPropertyName = "SeatCount";

        protected string _SeatCount;

        /// <summary>
        /// SeatCount
        /// </summary>
        public string SeatCount
        {
            get { return _SeatCount; }

            set
            {
                if (_SeatCount != value)
                {
                    RaisePropertyChanging(c_SeatCountPropertyName);
                    _SeatCount = value;
                    RaisePropertyChanged(c_SeatCountPropertyName);
                }
            }
        }
        #endregion

        #region SeatPrice
        protected const string c_SeatPricePropertyName = "SeatPrice";

        protected decimal _SeatPrice;

        /// <summary>
        /// SeatPrice
        /// </summary>
        public decimal SeatPrice
        {
            get { return _SeatPrice; }

            set
            {
                if (_SeatPrice != value)
                {
                    RaisePropertyChanging(c_SeatPricePropertyName);
                    _SeatPrice = value;
                    RaisePropertyChanged(c_SeatPricePropertyName);
                }
            }
        }
        #endregion

        #region TotalPrice
        protected const string c_TotalPricePropertyName = "TotalPrice";

        /// <summary>
        /// TotalPrice
        /// </summary>
        public decimal TotalPrice
        {
            get { return TaxFee + RQFee + SeatPrice; }
        }
        #endregion

        #region FromTerminal
        protected const string c_FromTerminalPropertyName = "FromTerminal";

        protected string _FromTerminal;

        /// <summary>
        /// FromTerminal
        /// </summary>        
        [DisplayName("FromTerminal")]
        public string FromTerminal
        {
            get { return _FromTerminal; }

            set
            {
                if (_FromTerminal != value)
                {
                    RaisePropertyChanging(c_FromTerminalPropertyName);
                    _FromTerminal = value;
                    RaisePropertyChanged(c_FromTerminalPropertyName);
                }
            }
        }
        #endregion

        #region StartDateTime
        protected const string c_StartDateTimePropertyName = "StartDateTime";

        protected DateTime _StartDateTime;

        /// <summary>
        /// StartDateTime
        /// </summary>        
        [DisplayName("StartDateTime")]
        public DateTime StartDateTime
        {
            get { return _StartDateTime; }

            set
            {
                if (_StartDateTime != value)
                {
                    RaisePropertyChanging(c_StartDateTimePropertyName);
                    _StartDateTime = value;
                    RaisePropertyChanged(c_StartDateTimePropertyName);
                }
            }
        }
        #endregion

        #region FromAirPortrName
        protected const string c_FromAirPortrNamePropertyName = "FromAirPortrName";

        protected string _FromAirPortrName;

        /// <summary>
        /// FromAirPortrName
        /// </summary>        
        [DisplayName("FromAirPortrName")]
        public string FromAirPortrName
        {
            get { return _FromAirPortrName; }

            set
            {
                if (_FromAirPortrName != value)
                {
                    RaisePropertyChanging(c_FromAirPortrNamePropertyName);
                    _FromAirPortrName = value;
                    RaisePropertyChanged(c_FromAirPortrNamePropertyName);
                }
            }
        }
        #endregion

        #region ToTerminal
        protected const string c_ToTerminalPropertyName = "ToTerminal";

        protected string _ToTerminal;

        /// <summary>
        /// ToTerminal
        /// </summary>        
        [DisplayName("ToTerminal")]
        public string ToTerminal
        {
            get { return _ToTerminal; }

            set
            {
                if (_ToTerminal != value)
                {
                    RaisePropertyChanging(c_ToTerminalPropertyName);
                    _ToTerminal = value;
                    RaisePropertyChanged(c_ToTerminalPropertyName);
                }
            }
        }
        #endregion

        #region ToDateTime
        protected const string c_ToDateTimePropertyName = "ToDateTime";

        protected DateTime _ToDateTime;

        /// <summary>
        /// ToDateTime
        /// </summary>        
        [DisplayName("ToDateTime")]
        public DateTime ToDateTime
        {
            get { return _ToDateTime; }

            set
            {
                if (_ToDateTime != value)
                {
                    RaisePropertyChanging(c_ToDateTimePropertyName);
                    _ToDateTime = value;
                    RaisePropertyChanged(c_ToDateTimePropertyName);
                }
            }
        }
        #endregion

        #region ToAirPortrName
        protected const string c_ToAirPortrNamePropertyName = "ToAirPortrName";

        protected string _ToAirPortrName;

        /// <summary>
        /// ToAirPortrName
        /// </summary>        
        [DisplayName("ToAirPortrName")]
        public string ToAirPortrName
        {
            get { return _ToAirPortrName; }

            set
            {
                if (_ToAirPortrName != value)
                {
                    RaisePropertyChanging(c_ToAirPortrNamePropertyName);
                    _ToAirPortrName = value;
                    RaisePropertyChanged(c_ToAirPortrNamePropertyName);
                }
            }
        }
        #endregion

        #region 政策点数
        protected decimal _PolicyPoint;

        /// <summary>
        /// 政策点数
        /// </summary>        
        [DisplayName("政策点数")]
        public decimal PolicyPoint
        {
            get { return _PolicyPoint; }

            set
            {
                if (_PolicyPoint != value)
                {
                    const string c_PolicyPointPropertyName = "PolicyPoint";
                    RaisePropertyChanging(c_PolicyPointPropertyName);
                    _PolicyPoint = value;
                    RaisePropertyChanged(c_PolicyPointPropertyName);
                }
            }
        }
        #endregion

        #region 特殊政策类型
        private const string PolicySpecialTypePortrNamePropertyName = "PolicySpecialType";
        private EnumPolicySpecialType _policySpecialType;
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType
        {
            get { return _policySpecialType; }
            set
            {
                if (_policySpecialType == value) return;
                RaisePropertyChanging(PolicySpecialTypePortrNamePropertyName);
                _policySpecialType = value;
                RaisePropertyChanged(PolicySpecialTypePortrNamePropertyName);
            }
        } 
        #endregion

        #region YPrice
        protected const string c_YPricePropertyName = "YPrice";

        protected decimal _YPrice;

        /// <summary>
        /// SeatPrice
        /// </summary>
        public decimal YPrice
        {
            get { return _YPrice; }

            set
            {
                if (_YPrice != value)
                {
                    RaisePropertyChanging(c_YPricePropertyName);
                    _YPrice = value;
                    RaisePropertyChanged(c_YPricePropertyName);
                }
            }
        }
        #endregion

        /// <summary>
        /// Ibe机建费
        /// </summary>
        public decimal IbeTaxFee
        {
            get;
            set;
        }

        /// <summary>
        /// Ibe燃油费
        /// </summary>
        public decimal IbeRQFee
        {
            get;
            set;
        }
        /// <summary>
        /// Ibe舱位价
        /// </summary>
        public decimal IbeSeatPrice
        {
            get;
            set;
        }

        public FlightInformationItemViewModel(FlightInfoModel fi)
        {
            this._fi = fi;

            CarrayShortName = fi.CarrayShortName;
            CarrayCode = fi.CarrayCode;
            FlightNumber = fi.FlightNumber;
            Model = fi.Model;

            FromTerminal = fi.FromTerminal;
            StartDateTime = fi.StartDateTime;
            FromAirPortrName = fi.FromAirPortrName;

            ToTerminal = fi.ToTerminal;
            ToDateTime = fi.ToDateTime;
            ToAirPortrName = fi.ToAirPortrName;

            People = fi.CarrayShortName;

            TaxFee = fi.TaxFee;
            RQFee = fi.RQFee;
            YPrice = fi.DefaultSite.SpecialYPrice;
            SeatCode = fi.DefaultSite.SeatCode;
            SeatCount = fi.DefaultSite.SeatCount;
            Discount = fi.DefaultSite.Discount;
            SeatPrice = fi.DefaultSite.SeatPrice;
            PolicyPoint = fi.DefaultSite.PolicyPoint;
            PolicySpecialType = fi.DefaultSite.PolicySpecialType;
            IbeRQFee = fi.IbeRQFee;
            IbeTaxFee = fi.IbeTaxFee;
            IbeSeatPrice = fi.DefaultSite.IbeSeatResponse.IbeSeatPrice;
            OnTotalPriceChanged();
        }

        public FlightInformationItemViewModel()
        {
        }

        public void OnTotalPriceChanged()
        {
            RaisePropertyChanged(c_TotalPricePropertyName);
        }

        public string ToolTip
        {
            get { return ToString(); }

        }

        public override string ToString()
        {
            var str = string.Format(
@"航空公司编码：{0}
航空公司简称：{1}
折扣：{2}
航班号：{3}
出发城市机场：{4}
出发航站楼：{5}
机型：{6}
航空公司：{7}
政策点数：{8}
燃油费：{9}
舱位：{10}
剩余座位数：{11}
舱位价：{12}
起飞时间：{13:yyyy-MM-dd HH:mm:ss}
机建费：{14}
到达城市机场名称：{15}
到达时间：{16:yyyy-MM-dd HH:mm:ss}
到达航站楼：{17}"
, _CarrayCode
, _CarrayShortName
, _Discount
, _FlightNumber
, _FromAirPortrName
, _FromTerminal
, _Model
, _People
, _PolicyPoint
, _RQFee
, _SeatCode
, _SeatCount
, _SeatPrice
, _StartDateTime
, _TaxFee
, _ToAirPortrName
, _ToDateTime
, _ToTerminal)
            ;
            return str;
        }
    }

}