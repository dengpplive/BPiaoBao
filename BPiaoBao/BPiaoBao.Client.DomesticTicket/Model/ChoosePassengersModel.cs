using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 选择购买保险和极速退乘客信息
    /// </summary>
    public class ChoosePassengersModel : ObservableObject
    {
        #region 公开属性

        public int Id { get; set; }

        #region IsSelected

        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        private const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelected;

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (_isSelected == value) return;

                RaisePropertyChanging(IsSelectedPropertyName);
                _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 姓名
        /// </summary>
        public string PassengerName
        {
            get;
            set;
        }

        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType
        {
            get;
            set;
        }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo
        {
            get;
            set;
        }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }

        #region IsInsuranceRefund
        /// <summary>
        /// 是否够买了急速退
        /// The <see cref="IsInsuranceRefund" /> property's name.
        /// </summary>
        private const string IsInsuranceRefundPropertyName = "IsInsuranceRefund";

        private bool _isInsuranceRefund;

        public bool IsInsuranceRefund
        {
            get { return _isInsuranceRefund; }

            set
            {
                if (_isInsuranceRefund == value) return;

                RaisePropertyChanging(IsInsuranceRefundPropertyName);
                _isInsuranceRefund = value;
                RaisePropertyChanged(IsInsuranceRefundPropertyName);
            }
        }

        #endregion

        #region IsBuyInsurance
        /// <summary>
        /// 是否购买航意险
        /// The <see cref="IsBuyInsurance" /> property's name.
        /// </summary>
        private const string IsBuyInsurancePropertyName = "IsBuyInsurance";

        private bool _isBuyInsurance;

        public bool IsBuyInsurance
        {
            get { return _isBuyInsurance; }

            set
            {
                if (_isBuyInsurance == value) return;

                RaisePropertyChanging(IsBuyInsurancePropertyName);
                _isBuyInsurance = value;
                RaisePropertyChanged(IsBuyInsurancePropertyName);
            }
        }

        #endregion

        #region BuyInsuranceCount
        /// <summary>
        /// 购买航意保险份数
        /// The <see cref="BuyInsuranceCount" /> property's name.
        /// </summary>
        private const string BuyInsuranceCountPropertyName = "BuyInsuranceCount";

        private int _buyInsuranceCount;

        public int BuyInsuranceCount
        {
            get { return _buyInsuranceCount; }

            set
            {
                if (_buyInsuranceCount == value) return;

                RaisePropertyChanging(BuyInsuranceCountPropertyName);
                _buyInsuranceCount = value;
                RaisePropertyChanged(BuyInsuranceCountPropertyName);
            }
        }

        #endregion

        #endregion
    }
}
