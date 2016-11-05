using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 经停显示实体
    /// </summary>
    public class LegStopModel:ObservableObject
    {

        #region 航班号 CA4431
        public string FlightCodeNumberPropertyName = "FlightCodeNumber";
        private string _flightCodeNumber;

        /// <summary>
        /// 航班号 CA4431
        /// </summary>
        public string FlightCodeNumber
        {
            get { return this._flightCodeNumber; }
            set
            {
                if (this._flightCodeNumber == value) return;
                this.RaisePropertyChanging(FlightCodeNumberPropertyName);
                this._flightCodeNumber = value;
                this.RaisePropertyChanged(FlightCodeNumberPropertyName);
            }
        }
        #endregion

        #region 出发日期
        public string StartDatePropertyName = "StartDate";
        private string _startDate;

        /// <summary>
        /// 出发日期
        /// </summary>
        public string StartDate
        {
            get { return this._startDate; }
            set
            {
                if (this._startDate == value) return;
                this.RaisePropertyChanging(StartDatePropertyName);
                this._startDate = value;
                this.RaisePropertyChanged(StartDatePropertyName);
            }
        }
        #endregion

        #region 到达日期
        public string ToDatePropertyName = "ToDate";
        private string _toDate;

        /// <summary>
        /// 到达日期
        /// </summary>
        public string ToDate
        {
            get { return this._toDate; }
            set
            {
                if (this._toDate == value) return;
                this.RaisePropertyChanging(ToDatePropertyName);
                this._toDate = value;
                this.RaisePropertyChanged(ToDatePropertyName);
            }
        }
        #endregion
        #region 中转日期
        public string MiddleDatePropertyName = "MiddleDate";
        private string _middleDate;

        /// <summary>
        /// 中转日期
        /// </summary>
        public string MiddleDate
        {
            get { return this._middleDate; }
            set
            {
                if (this._middleDate == value) return;
                this.RaisePropertyChanging(MiddleDatePropertyName);
                this._middleDate = value;
                this.RaisePropertyChanged(MiddleDatePropertyName);
            }
        }
        #endregion

        #region 出发城市
        public string FromCityPropertyName = "FromCity";
        private string _fromCity;

        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCity
        {
            get { return this._fromCity; }
            set
            {
                if (this._fromCity == value) return;
                this.RaisePropertyChanging(FromCityPropertyName);
                this._fromCity = value;
                this.RaisePropertyChanged(FromCityPropertyName);
            }
        }
        #endregion


        #region 到达城市
        public string ToCityPropertyName = "ToCity";
        private string _toCity;

        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCity
        {
            get { return this._toCity; }
            set
            {
                if (this._toCity == value) return;
                this.RaisePropertyChanging(ToCityPropertyName);
                this._toCity = value;
                this.RaisePropertyChanged(ToCityPropertyName);
            }
        }
        #endregion


        #region 中转城市
        public string MiddleCityPropertyName = "MiddleCity";
        private string _middleCity;

        /// <summary>
        /// 中转城市
        /// </summary>
        public string MiddleCity
        {
            get { return this._middleCity; }
            set
            {
                if (this._middleCity == value) return;
                this.RaisePropertyChanging(MiddleCityPropertyName);
                this._middleCity = value;
                this.RaisePropertyChanged(MiddleCityPropertyName);
            }
        }
        #endregion
    }
}
