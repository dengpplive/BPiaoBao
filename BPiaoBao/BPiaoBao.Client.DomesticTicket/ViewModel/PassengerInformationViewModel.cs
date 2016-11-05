using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using BPiaoBao.Client.DomesticTicket.Model;
using ProjectHelper.Utils;
using BPiaoBao.Client.UIExt.Model;
using System.Windows.Data;
using BPiaoBao.Common.Enums;
using System.Windows;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class PassengerInformationViewModel : ViewModelBase
    {
        private const int CIdLength = 18;

        /// <summary>
        /// 常旅客行数索引
        /// </summary>
        public int PasserIndex;

        #region 模糊查询后常旅客实体

        private const string PasseName = "Passer";

        private PasserModel _passer;

        /// <summary>
        /// 模糊查询后常旅客实体
        /// </summary>

        public PasserModel Passer
        {
            get { return _passer; }

            set
            {
                if (_passer == value) return;
                RaisePropertyChanging(PasseName);
                _passer = value;
                RaisePropertyChanged(PasseName);
                ResetDate();
            }
        }
        #endregion
         
        #region 姓名

        private const string CNameName = "Name";

        private string _name;

        /// <summary>
        /// 姓名
        /// </summary>

        [DisplayName(@"姓名")]
        public string Name
        {
            get { return _name; }

            set
            {
                if (_name == value) return;
                RaisePropertyChanging(CNameName);
                _name = value;
                RaisePropertyChanged(CNameName);
            }
        }
        #endregion

        #region 性别

        private const string CEnumSexTypeName = "SexType";

        private EnumSexType _sexType = EnumSexType.Male;

        /// <summary>
        /// 性别
        /// </summary>

        [DisplayName(@"性别")]
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


        private Visibility _isShowTextBox;
        public Visibility IsShowTextBox
        {

            get { return _isShowTextBox; }
            set
            {
                RaisePropertyChanging("IsShowTextBox");
                _isShowTextBox = value;
                RaisePropertyChanged("IsShowTextBox");
            }
        }

        private Visibility _isShowDatePicker;
        public Visibility IsShowDatePicker
        {

            get { return _isShowDatePicker; }
            set
            {
                RaisePropertyChanging("IsShowDatePicker");
                _isShowDatePicker = value;
                RaisePropertyChanged("IsShowDatePicker");
            }
        }



        #region 证件类型集合

        private const string CIdTypeItemsName = "IDTypeItems";
        private ICollectionView _idTypeItems;

        /// <summary>
        /// 证件类型集合
        /// </summary>
        public ICollectionView IdTypeItems
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

        #region 年龄类型

        private const string CAgeTypeName = "AgeType";

        private AgeType _ageType = AgeType.Adult;

        /// <summary>
        /// 年龄类型
        /// </summary>

        [DisplayName(@"年龄类型")]
        public AgeType AgeType
        {
            get { return _ageType; }

            set
            {
                if (_ageType == value) return;
                RaisePropertyChanging(CAgeTypeName);
                _ageType = value;
                switch (AgeType)
                {
                    case AgeType.All:
                        break;
                    case AgeType.Adult:
                        IdTypeItems = CollectionViewSource.GetDefaultView(EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key != EnumIDType.BirthDate));
                        IdType = EnumIDType.NormalId;

                        break;
                    case AgeType.Child:
                        IdTypeItems = CollectionViewSource.GetDefaultView(EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key == EnumIDType.BirthDate || p.Key == EnumIDType.NormalId || p.Key == EnumIDType.Passport));
                        IdType = EnumIDType.NormalId;
                        break;
                    case AgeType.Baby:
                        IdTypeItems = CollectionViewSource.GetDefaultView(EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key == EnumIDType.BirthDate));
                        IdType = EnumIDType.BirthDate;
                        break;
                }
                RaisePropertyChanged(CAgeTypeName);
                ResetDate();
            }
        }

        #endregion

        #region 证件号

        private const string CIdName = "ID";

        private string _id;

        /// <summary>
        /// 身份标示
        /// </summary>

        [DisplayName(@"证件号")]
        public string Id
        {
            get { return _id; }

            set
            {
                    RaisePropertyChanging(CIdName);
                    _id = value;
                    if (IdType == EnumIDType.NormalId && Id != null && Id.Length == 18)
                    {
                    //截取字符串生日转换
                    var birth = Common.ExtHelper.GetBirthdayDateFromString(Id);
                    if (birth != null) Birthday = birth;
                    }
                    else
                        ResetDate();
                    RaisePropertyChanged(CIdName);
                }
            }
        #endregion

        #region 证件类型

        private const string CIdTypeName = "IDType";

        private EnumIDType _idType;

        /// <summary>
        /// 证件类型
        /// </summary>

        [DisplayName(@"证件类型")]
        public EnumIDType IdType
        {
            get { return _idType; }

            set
            {

                RaisePropertyChanging(CIdTypeName);
                _idType = value;
                switch (_idType)
                {
                    case EnumIDType.NormalId:
                        IsShowTextBox = Visibility.Visible;
                        IsShowDatePicker = Visibility.Hidden;
                        break;
                    case EnumIDType.BirthDate:
                        IsShowTextBox = Visibility.Hidden;
                        IsShowDatePicker = Visibility.Visible;
                        break;
                    default:
                        IsShowTextBox = Visibility.Visible;
                        IsShowDatePicker = Visibility.Hidden;
                        break;
                }
                RaisePropertyChanged(CIdTypeName);

            }
        }
        #endregion

        #region 生日

        private const string CBirthdayPropertyName = "Birthday";

        private DateTime? _birthday;

        /// <summary>
        /// 生日
        /// </summary>        
        [DisplayName(@"生日")]
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

        #region 电话

        private const string CTelephoneName = "Telephone";

        private string _telephone;

        /// <summary>
        /// 电话
        /// </summary>

        [DisplayName(@"电话")]
        public string Telephone
        {
            get { return _telephone; }

            set
            {
                if (_telephone == value) return;
                RaisePropertyChanging(CTelephoneName);
                _telephone = value;
                RaisePropertyChanged(CTelephoneName);
            }
        }
        #endregion

        #region 航空公司卡

        private const string CBusinessCardName = "BusinessCard";

        private string _businessCard;

        /// <summary>
        /// 航空公司卡
        /// </summary>

        [DisplayName(@"航空公司卡")]
        public string BusinessCard
        {
            get { return _businessCard; }

            set
            {
                if (_businessCard == value) return;
                RaisePropertyChanging(CBusinessCardName);
                _businessCard = value;
                RaisePropertyChanged(CBusinessCardName);
            }
        }
        #endregion

        #region DisplayDateEnd

        private const string CDisplayDateEndPropertyName = "DisplayDateEnd";

        private DateTime _displayDateEnd;

        /// <summary>
        /// DisplayDateEnd
        /// </summary>        
        [DisplayName(@"DisplayDateEnd")]
        public DateTime DisplayDateEnd
        {
            get { return _displayDateEnd; }

            set
            {
                if (_displayDateEnd != value)
                {
                    RaisePropertyChanging(CDisplayDateEndPropertyName);
                    _displayDateEnd = value;
                    RaisePropertyChanged(CDisplayDateEndPropertyName);
                }
            }
        }
        #endregion

        #region DisplayDateStart

        private const string CDisplayDateStartPropertyName = "DisplayDateStart";

        private DateTime _displayDateStart;
        private DateTime _startDateTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startDateTime"></param>
        public PassengerInformationViewModel(DateTime startDateTime)
        {
            _startDateTime = startDateTime.Date;
            IdTypeItems = CollectionViewSource.GetDefaultView(EnumHelper.GetEnumKeyValuesPassger(typeof(EnumIDType)).FindAll(p => p.Key != EnumIDType.BirthDate));
        }

        /// <summary>
        /// DisplayDateStart
        /// </summary>        
        [DisplayName(@"DisplayDateStart")]
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

        private void ResetDate()
        {
            switch (AgeType)
            {
                case AgeType.Adult:
                    DisplayDateStart = _startDateTime.Date.AddYears(-100);
                    DisplayDateEnd = _startDateTime.Date.AddYears(-12).AddDays(1);
                    if (IdType == EnumIDType.NormalId && Id != null && !string.IsNullOrWhiteSpace(Id)) return;
                    if (Birthday != null && Name != null && Id != null && !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Name)) return;
                    Birthday = DisplayDateEnd;
                    break;
                case AgeType.Child:
                    DisplayDateStart = _startDateTime.Date.AddYears(-12).AddDays(1);
                    DisplayDateEnd = _startDateTime.Date.AddYears(-2);
                    if (Id != null && Name != null && !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Name) && Birthday != null) return;
                    Birthday = DisplayDateStart;
                    break;
                case AgeType.Baby:
                    DisplayDateStart = _startDateTime.Date.AddYears(-2).AddDays(1);
                    DisplayDateEnd = _startDateTime.Date;
                    if (Id != null && Name != null && !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Name) && Birthday != null) return;
                    Birthday = DisplayDateStart;
                    break;
            }
        }

        public void Check()
        {
            Guard.CheckIsNullOrEmpty(Passer.Name, "乘客姓名");

            if (AgeType == AgeType.Adult)
            {
                Guard.CheckIsNullOrEmpty(Id, "证件号");
                if (IdType != EnumIDType.NormalId) return; //身份证18位
                if (Id.Trim().Length != CIdLength)
                {
                    throw new Exception(string.Format("乘机人{0}的证件号{1}只能{2}位", Name, Id, CIdLength));
                }
                //截取字符串生日转换
                var birth = Common.ExtHelper.GetBirthdayDateFromString(Id);
                if (birth != null && (birth < DisplayDateStart || birth > DisplayDateEnd)) throw new Exception(string.Format("请核对乘机人{0}的出生日期范围{1:yyyy-MM-dd}", Name, Birthday));

                #region 其它证件类型判断
                //Guard.CheckMobilePhoneNum(_Telephone);
                //else if (IDType == Model.IDType.Passport)
                //{   //因私护照8位 因公护照7位 不包括英文字母
                //    if (ID.Replace("[a-z|A-Z]", "").Trim().Length != 7 && ID.Replace("[a-z|A-Z]", "").Trim().Length != 8)
                //    {
                //        throw new Exception(string.Format("{0}证件号只能{1}位或{2}", ID, 7,8));
                //    }
                //}
                //else if (IDType == Model.IDType.MilitaryID)
                //{   //军官证7位 不包括汉字英文字母
                //    if (ID.Replace("[a-z|A-Z]", "").Trim().Length != 7)
                //    {
                //        throw new Exception(string.Format("{0}证件号只能{1}位", ID, 7));
                //    }
                //}
                #endregion
            }
            else
            {
                switch (IdType)
                {
                    case EnumIDType.Passport:
                    case EnumIDType.NormalId:
                        Guard.CheckIsNullOrEmpty(Id, "证件号");
                        if (IdType == EnumIDType.NormalId)
                        {   //身份证18位
                            if (Id.Trim().Length != CIdLength)
                            {
                                throw new Exception(string.Format("乘机人{0}的证件号{1}只能{2}位", Name, Id, CIdLength));
                            }
                            //截取字符串生日转换
                            var birth = Common.ExtHelper.GetBirthdayDateFromString(Id);
                            if (birth != null && (birth < DisplayDateStart || birth > DisplayDateEnd)) throw new Exception(string.Format("请核对乘机人{0}的出生日期范围{1:yyyy-MM-dd}", Name ,Birthday));
                        }
                        break;
                    case EnumIDType.BirthDate:
                        if (Birthday == null || Birthday < DisplayDateStart || Birthday > DisplayDateEnd)
                        {
                            throw new Exception(string.Format("请核对乘机人{0}的出生日期范围{1:yyyy-MM-dd}", Name, Birthday));
                        }
                        break;
                }
            }
        }

    }
}
