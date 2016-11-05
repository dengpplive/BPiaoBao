using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 添加银行卡视图模型
    /// </summary>
    public class AddBankCardViewModel : BaseVM
    {

        #region 初始修改信息
        /// <summary>
        /// 初始修改信息
        /// </summary>
        /// <param name="dto"></param>
        public void InitModify(BankCardDto dto)
        {
            ModifyBankCardDto = dto;
            BankBranch = dto.BankBranch;
            CardNo = dto.CardNo;
            SelectedBankInfo = Banks.FirstOrDefault(p=>p.Name.Equals(dto.Name));
            SelectedState = States.FirstOrDefault(p=>p.State.Equals(dto.Province));
            Citys = CityData.GetCity(dto.Province);
            SelectedCity = Citys.FirstOrDefault(p => p.City.Equals(dto.City));
            IsAdded = false;

        } 
        #endregion

        #region 构造函数


        /// <summary>
        /// Initializes a new instance of the <see cref="AddBankCardViewModel"/> class.
        /// </summary>
        public AddBankCardViewModel()
        {
            Banks = BankData.GetBanks();
            
            States = CityData.GetAllState();

            if (IsInDesignMode)
                return;

            isBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                CashCompanyInfo = service.GetCompanyInfo();
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion

        #region 公开属性

        #region Banks

        /// <summary>
        /// The <see cref="ModifyBankCardDto" /> property's name.
        /// </summary>
        private const string ModifyBankCardDtoPropertyName = "ModifyBankCardDto";

        private BankCardDto _modifyBankCardDtoName;

        /// <summary>
        /// 要修改的银行卡信息
        /// </summary>
        public BankCardDto ModifyBankCardDto
        {
            get { return _modifyBankCardDtoName; }

            set
            {
                if (_modifyBankCardDtoName == value) return;

                RaisePropertyChanging(ModifyBankCardDtoPropertyName);
                _modifyBankCardDtoName = value;
                RaisePropertyChanged(ModifyBankCardDtoPropertyName);
            }
        }

        #endregion

        #region CashCompanyInfo

        /// <summary>
        /// The <see cref="CashCompanyInfo" /> property's name.
        /// </summary>
        private const string CashCompanyInfoPropertyName = "CashCompanyInfo";

        private CashCompanyInfoDto _cashCompanyInfoDto;

        /// <summary>
        /// 钱袋子信息
        /// </summary>
        public CashCompanyInfoDto CashCompanyInfo
        {
            get { return _cashCompanyInfoDto; }

            set
            {
                if (_cashCompanyInfoDto == value) return;

                RaisePropertyChanging(CashCompanyInfoPropertyName);
                _cashCompanyInfoDto = value;
                RaisePropertyChanged(CashCompanyInfoPropertyName);
            }
        }

        #endregion

        #region CurrentUserInfoDto

        ///// <summary>
        ///// The <see cref="CurrentUserInfoDto" /> property's name.
        ///// </summary>
        //public const string CurrentUserInfoDtoPropertyName = "CurrentUserInfoDto";

        //private CurrentUserInfoDto currentUserInfoDto = null;

        ///// <summary>
        ///// 当前用户信息
        ///// </summary>
        //public CurrentUserInfoDto CurrentUserInfoDto
        //{
        //    get { return currentUserInfoDto; }

        //    set
        //    {
        //        if (currentUserInfoDto == value) return;

        //        RaisePropertyChanging(CurrentUserInfoDtoPropertyName);
        //        currentUserInfoDto = value;
        //        RaisePropertyChanged(CurrentUserInfoDtoPropertyName);
        //    }
        //}

        #endregion

        #region States

        /// <summary>
        /// The <see cref="States" /> property's name.
        /// </summary>
        private const string StatesPropertyName = "States";

        private List<CityModel> _states = new List<CityModel>();

        /// <summary>
        /// 所有省份
        /// </summary>
        public List<CityModel> States
        {
            get { return _states; }

            set
            {
                if (_states == value) return;

                RaisePropertyChanging(StatesPropertyName);
                _states = value;
                RaisePropertyChanged(StatesPropertyName);
            }
        }

        #endregion

        #region SelectedState

        /// <summary>
        /// The <see cref="SelectedState" /> property's name.
        /// </summary>
        private const string SelectedStatePropertyName = "SelectedState";

        private CityModel _selectedState;

        /// <summary>
        /// 选中的省份
        /// </summary>
        public CityModel SelectedState
        {
            get { return _selectedState; }

            set
            {
                if (_selectedState == value) return;

                RaisePropertyChanging(SelectedStatePropertyName);
                _selectedState = value;
                RaisePropertyChanged(SelectedStatePropertyName);

                Task.Factory.StartNew(() =>
                {
                    var temp = CityData.GetCity(value.State);

                    Citys = temp;
                    if (temp != null && temp.Count > 0)
                        SelectedCity = temp[0];
                });
            }
        }

        #endregion

        #region Citys

        /// <summary>
        /// The <see cref="Citys" /> property's name.
        /// </summary>
        private const string CitysPropertyName = "Citys";

        private List<CityModel> _citys = new List<CityModel>();

        /// <summary>
        /// 当前省份下属城市
        /// </summary>
        public List<CityModel> Citys
        {
            get { return _citys; }

            set
            {
                if (_citys == value) return;

                RaisePropertyChanging(CitysPropertyName);
                _citys = value;
                RaisePropertyChanged(CitysPropertyName);
            }
        }

        #endregion

        #region SelectedCity

        /// <summary>
        /// The <see cref="SelectedCity" /> property's name.
        /// </summary>
        private const string SelectedCityPropertyName = "SelectedCity";

        private CityModel _selectedCity;

        /// <summary>
        /// 选中的城市
        /// </summary>
        public CityModel SelectedCity
        {
            get { return _selectedCity; }

            set
            {
                if (value == null)
                    return;
                if (_selectedCity == value) return;

                RaisePropertyChanging(SelectedCityPropertyName);
                _selectedCity = value;
                RaisePropertyChanged(SelectedCityPropertyName);
            }
        }

        #endregion

        #region Banks

        /// <summary>
        /// The <see cref="Banks" /> property's name.
        /// </summary>
        private const string BanksPropertyName = "Banks";

        private List<BankInfo> _banks;

        /// <summary>
        /// 银行卡列表
        /// </summary>
        public List<BankInfo> Banks
        {
            get { return _banks; }

            set
            {
                if (_banks == value) return;

                RaisePropertyChanging(BanksPropertyName);
                _banks = value;
                RaisePropertyChanged(BanksPropertyName);
            }
        }

        #endregion

        #region SelectedBankInfo

        /// <summary>
        /// The <see cref="SelectedBankInfo" /> property's name.
        /// </summary>
        private const string SelectedBankInfoPropertyName = "SelectedBankInfo";

        private BankInfo _selectedBankInfo;

        /// <summary>
        /// 选中的银行卡
        /// </summary>
        public BankInfo SelectedBankInfo
        {
            get { return _selectedBankInfo; }

            set
            {
                if (_selectedBankInfo == value) return;

                RaisePropertyChanging(SelectedBankInfoPropertyName);
                _selectedBankInfo = value;
                RaisePropertyChanged(SelectedBankInfoPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否在忙
        ///// </summary>
        //public bool IsBusy
        //{
        //    get { return isBusy; }

        //    set
        //    {
        //        if (isBusy == value) return;

        //        RaisePropertyChanging(IsBusyPropertyName);
        //        isBusy = value;
        //        RaisePropertyChanged(IsBusyPropertyName);
        //    }
        //}

        #endregion

        #region CardNo

        /// <summary>
        /// The <see cref="CardNo" /> property's name.
        /// </summary>
        private const string CardNoPropertyName = "CardNo";

        private string _cardNo = "";

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNo
        {
            get { return _cardNo; }

            set
            {
                if (_cardNo == value) return;

                RaisePropertyChanging(CardNoPropertyName);
                _cardNo = value;
                RaisePropertyChanged(CardNoPropertyName);
            }
        }

        #endregion

        #region BankBranch

        /// <summary>
        /// The <see cref="BankBranch" /> property's name.
        /// </summary>
        private const string BankBranchPropertyName = "BankBranch";

        private string _bankBranch = "";

        /// <summary>
        /// 开户行
        /// </summary>
        public string BankBranch
        {
            get { return _bankBranch; }

            set
            {
                if (_bankBranch == value) return;

                RaisePropertyChanging(BankBranchPropertyName);
                _bankBranch = value;
                RaisePropertyChanged(BankBranchPropertyName);
            }
        }

        #endregion

        #region IsAdded

        /// <summary>
        /// The <see cref="IsAdded" /> property's name.
        /// </summary>
        private const string IsAddedPropertyName = "IsAdded";

        private bool _isAdded = true;

        /// <summary>
        /// 是否添加完成
        /// </summary>
        public bool IsAdded
        {
            get { return _isAdded; }

            set
            {
                if (_isAdded == value) return;

                RaisePropertyChanging(IsAddedPropertyName);
                _isAdded = value;
                RaisePropertyChanged(IsAddedPropertyName);
            }
        }

        #endregion

        #region IsClosed

        /// <summary>
        /// The <see cref="IsClosed" /> property's name.
        /// </summary>
        private const string IsClosedPropertyName = "IsClosed";

        private bool _isClosed;

        /// <summary>
        /// 是否可以关闭窗口
        /// </summary>
        public bool IsClosed
        {
            get { return _isClosed; }

            set
            {
                if (_isClosed == value) return;

                RaisePropertyChanging(IsClosedPropertyName);
                _isClosed = value;
                RaisePropertyChanged(IsClosedPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region AddCommand

        private RelayCommand _addCommand;

        /// <summary>
        /// 新增银行卡命令
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand));
            }
        }

        private void ExecuteAddCommand()
        {
            if (string.IsNullOrWhiteSpace(_cardNo.Trim()))
            {
                UIManager.ShowErr(new Exception("请输入您的卡号"));
                return;
            }

            if (string.IsNullOrWhiteSpace(_bankBranch.Trim()))
            {
                UIManager.ShowErr(new Exception("请输入开户网点"));
                return;
            }

            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var model = new BankCardDto
                {
                    BankBranch = _bankBranch.Trim(),
                    CardNo = _cardNo.Trim(),
                    City = SelectedCity.City,
                    Name = _selectedBankInfo.Name,
                    Province = SelectedState.State,
                };
                if (_cashCompanyInfoDto != null)
                    model.Owner = _cashCompanyInfoDto.Contact;
                service.AddBank(model);
                IsClosed = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteAddCommand()
        {
            var can = !IsBusy && SelectedCity != null && SelectedState != null && !String.IsNullOrWhiteSpace(_cardNo) &&
                !String.IsNullOrWhiteSpace(_bankBranch);
            return can;
        }

        #endregion

        #region ModifyCommand
        private RelayCommand _modifyCommand;
        /// <summary>
        /// 修改银行卡命令
        /// </summary>
        public RelayCommand ModifyCommand
        {
            get
            {
                return _modifyCommand ?? (_modifyCommand = new RelayCommand(ExecuteModifyCommand, CanExecuteModifyCommand));
            }
        }

        private void ExecuteModifyCommand()
        {
            if (string.IsNullOrWhiteSpace(_cardNo.Trim()))
            {
                UIManager.ShowErr(new Exception("请输入您的卡号"));
                return;
            }

            if (string.IsNullOrWhiteSpace(_bankBranch.Trim()))
            {
                UIManager.ShowErr(new Exception("请输入开户网点"));
                return;
            }

            IsBusy = true;

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var model = new BankCardDto
                {
                    BankId = ModifyBankCardDto.BankId,
                    BankBranch = BankBranch.Trim(),
                    CardNo = CardNo.Trim(),
                    City = SelectedCity.City,
                    Name = SelectedBankInfo.Name,
                    Province = SelectedState.State,
                };
                if (_cashCompanyInfoDto != null)
                    model.Owner = _cashCompanyInfoDto.Contact;
                service.ModifyBank(model);
                IsClosed = true;

            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteModifyCommand()
        {
            var can = !IsBusy && SelectedCity != null && SelectedState != null && !String.IsNullOrWhiteSpace(_cardNo) &&
                  !String.IsNullOrWhiteSpace(_bankBranch);
            return can;
        }


        #endregion

        #endregion
    }
}
