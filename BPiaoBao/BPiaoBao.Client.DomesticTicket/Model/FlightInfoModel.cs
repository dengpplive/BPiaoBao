using System;
using System.Collections.Generic;
using System.Linq;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 航班信息
    /// </summary>

    public class FlightInfoModel : ObservableObject
    {
        /// <summary>
        /// 航程类型
        /// </summary>
        public FlightTypeEnum FlightType { get; set; }

        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 出发城市名称
        /// </summary>
        public string FromCity { get; set; }
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCity { get; set; }

        /// <summary>
        /// 出发城市机场名称
        /// </summary>
        public string FromAirPortrName { get; set; }
        /// <summary>
        /// 到达城市机场名称
        /// </summary>
        public string ToAirPortrName { get; set; }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string FromTerminal { get; set; }
        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string ToTerminal
        {
            get;
            set;
        }

        public const string StartDateTimeProtertyName = "StartDateTime";
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        /// <summary>
        /// 航空公司编码
        /// </summary>
        public string CarrayCode { get; set; }
        public string CarrayShortName { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber
        {
            get;
            set;
        }
        private decimal _taxFee;
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal TaxFee
        {
            get { return _taxFee; }
            set
            {
                if (_taxFee == value) return;
                RaisePropertyChanging("TaxFee");
                _taxFee = value;
                RaisePropertyChanged("TaxFee");
            }
        }
        private decimal _rqFee;
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee
        {
            get { return _rqFee; }
            set
            {
                if (_rqFee == value) return;
                RaisePropertyChanging("RQFee");
                _rqFee = value;
                RaisePropertyChanged("RQFee");
            }
        }

        private Site _defaultSite;

        /// <summary>
        /// 默认舱位信息
        /// </summary>
        public Site DefaultSite
        {
            get { return _defaultSite; }
            set
            {
                if (_defaultSite == value) return;
                RaisePropertyChanging("DefaultSite");
                _defaultSite = value;
                RaisePropertyChanged("DefaultSite");
            }
        }

        /// <summary>
        /// 所有舱位信息
        /// </summary>
        //public List<Site> SiteList { get; set; }
        private List<Site> _sites;

        public List<Site> SiteList
        {
            get { return _sites; }
            set
            {
                if (_sites == value) return;
                RaisePropertyChanging("SiteList");
                _sites = value;
                RaisePropertyChanged("SiteList");

            }
        }

        /// <summary>
        /// 机型
        /// </summary>
        public string Model { get; set; }


        /// <summary>
        /// 自定义号
        /// </summary>
        public int CusNo { get; set; }

        /// <summary>
        /// 是否经停
        /// </summary>
        public bool IsStop
        {
            get;
            set;
        }
        /// <summary>
        /// 经停显示字段
        /// </summary>
        public string StopText
        {
            get;
            set;
        }

        /// <summary>
        /// 是否共享
        /// </summary>
        public bool IsShareFlight
        {
            get;
            set;
        }

        private LegStopModel _legStopModel;

        /// <summary>
        /// 经停详情
        /// </summary>
        public LegStopModel LegStopModel
        {
            get { return _legStopModel; }
            set
            {
                if (_legStopModel == value) return;
                RaisePropertyChanging("LegStopModel");
                _legStopModel = value;
                RaisePropertyChanged("LegStopModel");
            }

        }

        /// <summary>
        /// 是否含有特价舱位
        /// </summary>
        public bool IsHaveSpecial
        {
            get { return SiteList.Any(p => p.PolicySpecialType != EnumPolicySpecialType.Normal); }
        }

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

        private bool _isSiteListChanged;
        /// <summary>
        /// 是否更改数据类型
        /// </summary>
        public bool IsSiteListChanged
        {
            get { return _isSiteListChanged; }
            set
            {
                RaisePropertyChanging("IsSiteListChanged");
                _isSiteListChanged = value;
                RaisePropertyChanged("IsSiteListChanged");
            }
        }

        private bool _isOpened;
        /// <summary>
        /// 是否展开下拉框详情
        /// </summary>
        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                RaisePropertyChanging("IsOpened");
                _isOpened = value;
                RaisePropertyChanged("IsOpened");
            }
        }
    }

    /// <summary>
    /// 舱位信息
    /// </summary>
    public class Site : ObservableObject
    {
        /// <summary>
        /// 自定义号与父实体CusNo相等 
        /// </summary>
        public int CusNo { get; set; }

        /// <summary>
        /// 舱位
        /// </summary>
        public string SeatCode { get; set; }

        /// <summary>
        /// 剩余座位数
        /// </summary>
        public string SeatCount { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }

        private decimal _seatPrice;

        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice
        {
            get { return _seatPrice; }
            set
            {
                if (_seatPrice == value) return;
                RaisePropertyChanging("SeatPrice");
                _seatPrice = value;
                RaisePropertyChanged("SeatPrice");
            }
        }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal TicketPrice { get; set; }
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal PolicyPoint { get; set; }

        private decimal _commission;
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission
        {
            get
            {
                return _commission = Math.Floor(PolicyPoint / 100 * SeatPrice);
            }
            set
            {
                if (_commission == value) return;
                RaisePropertyChanging("Commission");
                _commission = value;
                RaisePropertyChanged("Commission");
            }
        }


        private decimal _taxFee;
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal TaxFee
        {
            get { return _taxFee; }
            set
            {
                if (_taxFee == value) return;
                RaisePropertyChanging("TaxFee");
                _taxFee = value;
                RaisePropertyChanged("TaxFee");
            }
        }

        private decimal _rqFee;
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee
        {
            get { return _rqFee; }
            set
            {
                if (_rqFee == value) return;
                RaisePropertyChanging("RQFee");
                _rqFee = value;
                RaisePropertyChanged("RQFee");
            }
        }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }

        /// <summary>
        /// 特价Y舱的舱位价
        /// </summary>
        public decimal SpecialYPrice { get; set; }

        private bool _isGotSpecial;
        /// <summary>
        /// 是否执行获取特价
        /// </summary>
        public bool IsGotSpecial
        {
            get { return _isGotSpecial; }
            set
            {
                if (_isGotSpecial == value) return;
                RaisePropertyChanging("IsGotSpecial");
                _isGotSpecial = value;
                RaisePropertyChanged("IsGotSpecial");
            }
        }
        /// <summary>
        /// 原始舱位信息
        /// </summary>
        public SeatResponse IbeSeatResponse { get; set; }
        /// <summary>
        /// 特价描述
        /// </summary>
        public string PolicySpecialTypeDesc
        {
            get { return PolicySpecialType.ToEnumDesc(); }
        }
        private bool _isReceivedSpecial;
        /// <summary>
        /// 是否成功获取特价
        /// </summary>
        public bool IsReceivedSpecial
        {
            get { return _isReceivedSpecial; }
            set
            {
                if (_isReceivedSpecial == value) return;
                RaisePropertyChanging("IsReceivedSpecial");
                _isReceivedSpecial = value;
                RaisePropertyChanged("IsReceivedSpecial");
            }
        }
        /// <summary>
        /// 航程类型
        /// </summary>
        public FlightTypeEnum FlightType { get; set; }
    }
}
