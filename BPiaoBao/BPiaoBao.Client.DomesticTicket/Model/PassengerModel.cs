using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 乘客信息
    /// </summary>
    public class PassengerModel : ObservableObject
    {
        #region 公开属性

        public int Id { get; set; }

        #region IsChecked

        /// <summary>
        /// The <see cref="IsChecked" /> property's name.
        /// </summary>
        private const string IsCheckedPropertyName = "IsChecked";

        private bool _isChecked;

        /// <summary>
        /// 是否已选中
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }

            set
            {
                if (_isChecked == value) return;

                RaisePropertyChanging(IsCheckedPropertyName);
                _isChecked = value;
                RaisePropertyChanged(IsCheckedPropertyName);
            }
        }

        #endregion

        #region Name

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        private const string NamePropertyName = "Name";

        private string _name;

        /// <summary>
        /// 乘客姓名
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

        #region PhoneNum

        /// <summary>
        /// The <see cref="PhoneNum" /> property's name.
        /// </summary>
        private const string PhoneNumPropertyName = "PhoneNum";

        private string _phoneNum;

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNum
        {
            get { return _phoneNum; }

            set
            {
                if (_phoneNum == value) return;

                RaisePropertyChanging(PhoneNumPropertyName);
                _phoneNum = value;
                RaisePropertyChanged(PhoneNumPropertyName);
            }
        }

        #endregion

        #region IDNumer

        /// <summary>
        /// The <see cref="IDNumer" /> property's name.
        /// </summary>
        private const string IdNumerPropertyName = "IDNumer";

        private string _idNumber;

        /// <summary>
        /// 证件号
        /// </summary>
        public string IDNumer
        {
            get { return _idNumber; }

            set
            {
                if (_idNumber == value) return;

                RaisePropertyChanging(IdNumerPropertyName);
                _idNumber = value;
                RaisePropertyChanged(IdNumerPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 是否够买了保险急速退
        /// </summary>
        public bool IsInsuranceRefund { get; set; }
        /// <summary>
        /// 行程单状态
        /// </summary>
        public EnumPassengerTripStatus PassengerTripStatus
        {
            get;
            set;
        }
        #endregion
    }
}
