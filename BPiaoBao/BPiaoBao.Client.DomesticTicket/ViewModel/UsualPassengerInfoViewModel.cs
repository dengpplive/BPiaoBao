using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 常旅客编辑信息视图模型
    /// </summary>
    public class UsualPassengerInfoViewModel : BaseVM
    {

        #region 构造函数
        public UsualPassengerInfoViewModel(FrePasserDto fpd = null)
        {
            Initialize();
            if (fpd == null) return;
            Passenger = fpd;
            #region 属性字段赋值
            Name = fpd.Name;
            CertificateNo = fpd.CertificateNo;
            AirCardNo = fpd.AirCardNo;
            Mobile = fpd.Mobile;
            Remark = fpd.Remark;
            switch (fpd.SexType)
            {
                case "男":
                    SelectedSexType = EnumSexType.Male;
                    break;
                case "女":
                    SelectedSexType = EnumSexType.Female;
                    break;
                case "未知":
                    SelectedSexType = EnumSexType.UnKnown;
                    break;
            }
            switch (fpd.PasserType)
            {
                case "成人":
                    AgeType = AgeType.Adult;
                    break;
                case "儿童":
                    AgeType = AgeType.Child;
                    break;
                case "婴儿":
                    AgeType = AgeType.Baby;
                    break;
            }
            Birthday = fpd.Birth;
            switch (fpd.CertificateType)
            {
                case "身份证":
                    IdType = IDType.NormalId;
                    if (fpd.CertificateNo.Length == 18)
                    {
                        DateTime? birth = Common.ExtHelper.GetBirthdayDateFromString(CertificateNo);
                        if (birth != null) Birthday = birth;
                    }
                    break;
                case "护照":
                    IdType = IDType.Passport;
                    break;
                case "军官证":
                    IdType = IDType.MilitaryID;
                    break;
                case "出生日期":
                    IdType = IDType.BirthDate;
                    Birthday = Convert.ToDateTime(CertificateNo);
                    break;
                case "其它有效证件":
                    IdType = IDType.Other;
                    break;
            }
            #endregion
        }
        #endregion
        /// <summary>
        /// 乘客类型集合
        /// </summary>
        private List<KeyValuePair<dynamic, string>> _cIdTypeItems;
        #region 初始化数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            _cIdTypeItems = ProjectHelper.Utils.EnumHelper.GetEnumKeyValuesPassger(typeof(IDType));
            IdTypeItems = CollectionViewSource.GetDefaultView(_cIdTypeItems.FindAll(p => p.Key != IDType.BirthDate));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Male, EnumHelper.GetDescription(EnumSexType.Male)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.Female, EnumHelper.GetDescription(EnumSexType.Female)));
            _allInsextypes.Add(new KeyValuePair<EnumSexType?, string>(EnumSexType.UnKnown, EnumHelper.GetDescription(EnumSexType.UnKnown)));
            ResetDate();
        }

        #endregion

        #region 公开属性
        #region Passenger

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string PassengerPropertyName = "Passenger";

        private FrePasserDto _passenger;

        /// <summary>
        /// 常旅客实体
        /// </summary>
        public FrePasserDto Passenger
        {
            get { return _passenger; }

            set
            {
                if (_passenger == value) return;

                RaisePropertyChanging(PassengerPropertyName);
                _passenger = value;
                RaisePropertyChanged(PassengerPropertyName);
            }
        }

        #endregion
        #region 年龄类型

        private const string CAgeTypeName = "AgeType";
        private AgeType _ageType = AgeType.Adult;
        /// <summary>
        /// 年龄类型
        /// </summary>
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
                        IdTypeItems = CollectionViewSource.GetDefaultView(_cIdTypeItems.FindAll(p => p.Key != IDType.BirthDate));
                        IdType = IDType.NormalId;
                        DateIsEnable = true;//DateIsEnable = false;
                        break;
                    case AgeType.Child:
                        IdTypeItems = CollectionViewSource.GetDefaultView(_cIdTypeItems.FindAll(p => p.Key == IDType.BirthDate || p.Key == IDType.NormalId));
                        IdType = IDType.NormalId;
                        DateIsEnable = true;
                        break;
                    case AgeType.Baby:
                        IdTypeItems = CollectionViewSource.GetDefaultView(_cIdTypeItems.FindAll(p => p.Key == IDType.BirthDate));
                        IdType = IDType.BirthDate;
                        DateIsEnable = true;
                        break;
                }
                RaisePropertyChanged(CAgeTypeName);
                ResetDate();
            }
        }
        #endregion
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
        #region 证件类型

        private const string CIdTypeName = "IDType";

        private IDType _idType = IDType.NormalId;

        /// <summary>
        /// 证件类型
        /// </summary>
        public IDType IdType
        {
            get { return _idType; }

            set
            {
                if (_idType == value) return;
                RaisePropertyChanging(CIdTypeName);
                _idType = value;
                CertificateNoIsEnable = IdType != IDType.BirthDate;
                if (IdType == IDType.NormalId && CertificateNo.Length == 18)
                {
                    var birth = Common.ExtHelper.GetBirthdayDateFromString(CertificateNo);
                    if ( birth != null) Birthday = birth;
                        
                }
                RaisePropertyChanged(CIdTypeName);
            }
        }
        #endregion
        #region CertificateNoIsEnable

        /// <summary>
        /// The <see cref="CertificateNoIsEnable" /> property's name.
        /// </summary>
        private const string CertificateNoIsEnablePropertyName = "CertificateNoIsEnable";

        private bool _certificateNoIsEnable = true;

        /// <summary>
        /// 证件类型是否可编辑
        /// </summary>
        public bool CertificateNoIsEnable
        {
            get { return _certificateNoIsEnable; }

            set
            {
                if (_certificateNoIsEnable == value) return;
                RaisePropertyChanging(CertificateNoIsEnablePropertyName);
                _certificateNoIsEnable = value;
                RaisePropertyChanged(CertificateNoIsEnablePropertyName);
            }
        }

        #endregion
        #region DateIsEnable

        /// <summary>
        /// The <see cref="DateIsEnable" /> property's name.
        /// </summary>
        private const string DateIsEnablePropertyName = "DateIsEnable";

        private bool _dateIsEnable = true;

        /// <summary>
        /// 日期是否显示
        /// </summary>
        public bool DateIsEnable
        {
            get { return _dateIsEnable; }

            set
            {
                if (_dateIsEnable == value) return;
                RaisePropertyChanging(DateIsEnablePropertyName);
                _dateIsEnable = value;
                RaisePropertyChanged(DateIsEnablePropertyName);
            }
        }

        #endregion
        #region IsDone

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

        /// <summary>
        /// 是否添加完成
        /// </summary>
        public bool IsDone
        {
            get { return _isDone; }

            set
            {
                if (_isDone == value) return;

                RaisePropertyChanging(IsDonePropertyName);
                _isDone = value;
                RaisePropertyChanged(IsDonePropertyName);
            }
        }

        #endregion
        #region Name

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string NamePropertyName = "Name";

        private string _name = "";

        /// <summary>
        /// 姓名
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
        #region CertificateNo

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string CertificateNoPropertyName = "CertificateNo";

        private string _certificateNo = "";

        /// <summary>
        /// 证件号码
        /// </summary>
        public string CertificateNo
        {
            get { return _certificateNo; }

            set
            {
                if (_certificateNo == value) return;

                RaisePropertyChanging(CertificateNoPropertyName);
                _certificateNo = value;
                if (IdType == IDType.NormalId && CertificateNo.Length == 18)
                {
                    //截取字符串获取生日
                    var birth = Common.ExtHelper.GetBirthdayDateFromString(CertificateNo);
                    if (birth != null) Birthday = birth;
                }
                else
                    ResetDate();
                RaisePropertyChanged(CertificateNoPropertyName);
            }
        }

        #endregion
        #region Mobile

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string MobilePropertyName = "Mobile";

        private string _mobile = "";

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
        #region AirCardNo

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string AirCardNoPropertyName = "AirCardNo";

        private string _airCardNo = "";

        /// <summary>
        /// 航空公司卡
        /// </summary>
        public string AirCardNo
        {
            get { return _airCardNo; }

            set
            {
                if (_airCardNo == value) return;

                RaisePropertyChanging(AirCardNoPropertyName);
                _airCardNo = value;
                RaisePropertyChanged(AirCardNoPropertyName);
            }
        }

        #endregion
        #region Remark

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string RemarkPropertyName = "Remark";

        private string _remark = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }

            set
            {
                if (_remark == value) return;

                RaisePropertyChanging(RemarkPropertyName);
                _remark = value;
                RaisePropertyChanged(RemarkPropertyName);
            }
        }

        #endregion
        #region Birthday

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
        #region AllSexTypes

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string AllSexTypesPropertyName = "AllSexTypes";

        private ObservableCollection<KeyValuePair<EnumSexType?, String>> _allInsextypes = new ObservableCollection<KeyValuePair<EnumSexType?, string>>();

        /// <summary>
        /// 所有性别
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumSexType?, String>> AllInsextypes
        {
            get { return _allInsextypes; }

            set
            {
                if (_allInsextypes == value) return;

                RaisePropertyChanging(AllSexTypesPropertyName);
                _allInsextypes = value;
                RaisePropertyChanged(AllSexTypesPropertyName);
            }
        }

        #endregion
        #region SelectedSexType

        /// <summary>
        /// The <see cref="SelectedSexType" /> property's name.
        /// </summary>
        private const string SelectedSexTypePropertyName = "SelectedSexType";

        private EnumSexType? _selectedSexType = EnumSexType.Male;

        /// <summary>
        /// 选中的性别
        /// </summary>
        public EnumSexType? SelectedSexType
        {
            get { return _selectedSexType; }

            set
            {
                if (_selectedSexType == value) return;

                RaisePropertyChanging(SelectedSexTypePropertyName);
                _selectedSexType = value;
                RaisePropertyChanged(SelectedSexTypePropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令
        #region SaveCommand

        private RelayCommand _saveCommand;

        /// <summary>
        /// 执行保存命令
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }
        /// <summary>
        /// 验证input
        /// </summary>
        private bool Check()
        {
            try
            {
                ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(Name, "乘客姓名");

                if (AgeType == AgeType.Adult)
                {
                    ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(CertificateNo, "证件号");
                    if (IdType == IDType.NormalId)
                    {   //身份证18位
                        if (CertificateNo.Trim().Length != 18)
                        {
                            throw new Exception(string.Format("{0}证件号只能{1}位", CertificateNo, 18));
                        }
                    }
                    ProjectHelper.Utils.Guard.CheckMobilePhoneNum(Mobile);
                }
                else
                {
                    if (AgeType == AgeType.Child && IdType == IDType.NormalId)
                    {
                        ProjectHelper.Utils.Guard.CheckIsNullOrEmpty(CertificateNo, "证件号");
                        if (IdType == IDType.NormalId)
                        {   //身份证18位
                            if (CertificateNo.Trim().Length != 18)
                            {
                                throw new Exception(string.Format("{0}证件号只能{1}位", CertificateNo, 18));
                            }
                        }
                    }
                }

                if (Birthday < DisplayDateStart || Birthday > DisplayDateEnd)
                {
                    throw new Exception(string.Format("请核对出生日期范围{0:yyyy-MM-dd}", Birthday));
                }
                if (IdType != IDType.NormalId || string.IsNullOrWhiteSpace(CertificateNo) || CertificateNo.Trim().Length != 18) return true;
                //生日转换截取字符串
                var birth = Common.ExtHelper.GetBirthdayDateFromString(CertificateNo);
                if (birth != null && (birth < DisplayDateStart || birth > DisplayDateEnd)) throw new Exception(string.Format("请核对出生日期范围{0:yyyy-MM-dd}", Birthday));

                return true;
            }
            catch (Exception ex)
            {
                UIManager.ShowErr(ex);
                return false;
            }
        }
        private void ExecuteSaveCommand()
        {

            if (!Check()) return;
            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IFrePasserService>(service =>
            {
                if (Passenger == null)
                {
                    var model = new FrePasserDto
                    {
                        Name = Name,
                        AirCardNo = AirCardNo,
                        Mobile = Mobile,
                        Remark = Remark,
                        PasserType = EnumHelper.GetDescription(AgeType),
                        CertificateType = EnumHelper.GetDescription(IdType),
                        SexType = SelectedSexType.HasValue ? EnumHelper.GetDescription(SelectedSexType.Value) : "",
                        Birth = Birthday
                    };
                    switch (AgeType)
                    {
                        case AgeType.All:
                            break;
                        case AgeType.Adult:
                            model.CertificateNo = CertificateNo;
                            break;
                        case AgeType.Child:
                            if (IdType == IDType.BirthDate && Birthday != null)
                                model.CertificateNo = Birthday.Value.ToString("yyyy-MM-dd");
                            else
                                model.CertificateNo = CertificateNo;
                            break;
                        case AgeType.Baby:
                            model.CertificateNo = Birthday != null ? Birthday.Value.ToString("yyyy-MM-dd") : CertificateNo;
                            break;
                    }
                    var flag = service.Exists(model.Name, model.CertificateNo);
                    if (flag)
                    {
                        UIManager.ShowMessage("当前常旅客信息已经存在");
                    }
                    else
                    {
                        service.SaveFrePasser(model);
                        IsDone = true;
                    }
                }
                else
                {

                    Passenger.Name = Name;
                    switch (AgeType)
                    {
                        case AgeType.All:
                            break;
                        case AgeType.Adult:
                            Passenger.CertificateNo = CertificateNo;
                            break;
                        case AgeType.Child:
                            if (IdType == IDType.BirthDate && Birthday != null)
                                Passenger.CertificateNo = Birthday.Value.ToString("yyyy-MM-dd");
                            else
                                Passenger.CertificateNo = CertificateNo;
                            break;
                        case AgeType.Baby:
                            Passenger.CertificateNo = Birthday != null ? Birthday.Value.ToString("yyyy-MM-dd") : CertificateNo;
                            break;
                    }
                    Passenger.AirCardNo = AirCardNo;
                    Passenger.Mobile = Mobile;
                    Passenger.Remark = Remark;
                    Passenger.PasserType = EnumHelper.GetDescription(AgeType);
                    Passenger.CertificateType = EnumHelper.GetDescription(IdType);
                    Passenger.SexType = SelectedSexType.HasValue ? EnumHelper.GetDescription(SelectedSexType.Value) : "";
                    Passenger.Birth = Birthday;
                    service.UpdateFrePasser(Passenger);
                    IsDone = true;
                }

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });

        }

        #endregion

        #endregion

        /// <summary>
        /// 初始化出生日期
        /// </summary>
        private void ResetDate()
        {
            switch (AgeType)
            {
                case AgeType.Adult:
                    DisplayDateStart = DateTime.Now.Date.AddYears(-100);
                    DisplayDateEnd = DateTime.Now.Date.AddYears(-12).AddDays(1);
                    if (Passenger == null) Birthday = DisplayDateEnd;
                    break;
                case AgeType.Child:
                    DisplayDateStart = DateTime.Now.Date.AddYears(-12).AddDays(1);
                    DisplayDateEnd = DateTime.Now.Date.AddYears(-2);
                    if (Passenger == null) Birthday = DisplayDateStart;
                    break;
                case AgeType.Baby:
                    DisplayDateStart = DateTime.Now.Date.AddYears(-2).AddDays(1);
                    DisplayDateEnd = DateTime.Now.Date;
                    if (Passenger == null) Birthday = DisplayDateStart;
                    break;
            }
        }

    }
}
