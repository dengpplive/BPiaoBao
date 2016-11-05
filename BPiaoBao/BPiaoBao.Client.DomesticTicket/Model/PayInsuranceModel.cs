using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    public class PayInsuranceModel : ObservableObject
    {
        #region 乘客信息ID

        private const string PassengerIdPropertyName = "PassengerId";
        private int _passengerId;

        /// <summary>
        /// 乘客信息ID
        /// </summary>
        public int PassengerId
        {
            get { return _passengerId; }
            set
            {
                if (_passengerId == value) return;
                RaisePropertyChanging(PassengerIdPropertyName);
                _passengerId = value;
                RaisePropertyChanged(PassengerIdPropertyName);
            }
        }
        #endregion

        #region 乘客信息Name

        private const string PassengerNamePropertyName = "PassengerName";
        private string _passengerName;

        /// <summary>
        /// 乘客信息ID
        /// </summary>
        public string PassengerName
        {
            get { return _passengerName; }
            set
            {
                if (_passengerName == value) return;
                RaisePropertyChanging(PassengerNamePropertyName);
                _passengerName = value;
                RaisePropertyChanged(PassengerNamePropertyName);
            }
        }
        #endregion

        #region 航程list

        private const string PayInsuranceSkyWayModelsPropertyName = "PayInsuranceSkyWayModels";
        private List<PayInsuranceSkyWayModel> _payInsuranceSkyWayModels;

        /// <summary>
        ///  航程list
        /// </summary>
        public List<PayInsuranceSkyWayModel> PayInsuranceSkyWayModels
        {
            get { return _payInsuranceSkyWayModels; }
            set
            {
                if (_payInsuranceSkyWayModels == value) return;
                RaisePropertyChanging(PayInsuranceSkyWayModelsPropertyName);
                _payInsuranceSkyWayModels = value;
                RaisePropertyChanged(PayInsuranceSkyWayModelsPropertyName);
            }
        }
        #endregion

        #region 类型
        private const string CEnumPassengerTypeName = "PassengerType";

        private EnumPassengerType _passengerType = EnumPassengerType.Adult;

        /// <summary>
        /// 乘客类型
        /// </summary>
        public EnumPassengerType PassengerType
        {
            get { return _passengerType; }

            set
            {
                RaisePropertyChanging(CEnumPassengerTypeName);
                _passengerType = value;
                switch (_passengerType)
                {
                    case EnumPassengerType.Adult:
                        IDTypeItems = CollectionViewSource.GetDefaultView(ProjectHelper.Utils.EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key != EnumIDType.BirthDate));
                        IDType = EnumIDType.NormalId;
                        break;
                    case EnumPassengerType.Child:
                        IDTypeItems = CollectionViewSource.GetDefaultView(ProjectHelper.Utils.EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key == EnumIDType.BirthDate || p.Key == EnumIDType.NormalId || p.Key == EnumIDType.Passport));
                        IDType = EnumIDType.NormalId;
                        break;
                    case EnumPassengerType.Baby:
                        IDTypeItems = CollectionViewSource.GetDefaultView(ProjectHelper.Utils.EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key == EnumIDType.BirthDate));
                        IDType = EnumIDType.BirthDate;
                        break;
                }
                RaisePropertyChanged(CEnumPassengerTypeName);
            }
        }
        #endregion

        #region 性别
        private const string CEnumSexTypeName = "SexType";

        private EnumSexType _sexType = EnumSexType.Male;

        /// <summary>
        /// 性别
        /// </summary>
        public EnumSexType SexType
        {
            get { return _sexType; }

            set
            {
                if (_sexType == value) return;
                RaisePropertyChanging(CEnumSexTypeName);
                _sexType = value;
                RaisePropertyChanged(CEnumSexTypeName);
            }
        }
        #endregion

        #region 生日
        private const string CBirthdayPropertyName = "Birthday";

        private DateTime? _birthday;

        /// <summary>
        /// 生日
        /// </summary>        
        public DateTime? Birthday
        {
            get { return _birthday; }

            set
            {
                if (_birthday == value) return;
                RaisePropertyChanging(CBirthdayPropertyName);
                _birthday = value;
                RaisePropertyChanged(CBirthdayPropertyName);
            }
        }
        #endregion

        #region 证件号
        private const string CIdName = "ID";

        private string _id;

        /// <summary>
        /// 证件号
        /// </summary>
        public string ID
        {
            get { return _id; }

            set
            {
                if (_id == value) return;
                RaisePropertyChanging(CIdName);
                _id = value;
                if (IDType == EnumIDType.NormalId && ID != null && ID.Length == 18)
                {
                    //生日转换截取字符串
                    var birth = Common.ExtHelper.GetBirthdayDateFromString(ID);
                    if (birth != null) Birthday = birth;
                }
                RaisePropertyChanged(CIdName);
            }
        }
        #endregion

        #region 手机号
        private const string CTelName = "ID";

        private string _tel;

        /// <summary>
        /// 手机号
        /// </summary>
        public string Tel
        {
            get { return _tel; }

            set
            {
                if (_tel == value) return;
                RaisePropertyChanging(CTelName);
                _tel = value;
                RaisePropertyChanged(CTelName);
            }
        }
        #endregion

        #region 证件类型集合

        private const string CIdTypeItemsName = "IDTypeItems";
        private ICollectionView _idTypeItems;

        /// <summary>
        /// 证件类型集合
        /// </summary>
        public ICollectionView IDTypeItems
        {
            get { return _idTypeItems; }

            set
            {
                if (_idTypeItems == value) return;
                RaisePropertyChanging(CIdTypeItemsName);
                _idTypeItems = value;
                RaisePropertyChanged(CIdTypeItemsName);
            }
        }
        #endregion

        #region 证件类型
        private const string CIdTypeName = "IDType";

        private EnumIDType _idType = EnumIDType.NormalId;

        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumIDType IDType
        {
            get { return _idType; }

            set
            {
                if (_idType == value) return;
                RaisePropertyChanging(CIdTypeName);
                _idType = value;
                RaisePropertyChanged(CIdTypeName);
            }
        }
        #endregion
    }


    public class PayInsuranceSkyWayModel : ObservableObject
    {
        #region 航程ID

        private const string SkyWayIdPropertyName = "SkyWayId";
        private int _skyWayId;

        /// <summary>
        /// 航程ID
        /// </summary>
        public int SkyWayId
        {
            get { return _skyWayId; }
            set
            {
                if (_skyWayId == value) return;
                RaisePropertyChanging(SkyWayIdPropertyName);
                _skyWayId = value;
                RaisePropertyChanged(SkyWayIdPropertyName);
            }
        }
        #endregion

        #region 保险份数

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


    }
}
