using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.Client.UIExt.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BPiaoBao.Client.TPOS.ViewModel
{
    /// <summary>
    /// 添加 修改 商户视图模型
    /// </summary>
    public class MerchantAddEditViewModel : BaseVM, IDataErrorInfo
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="MerchantAddEditViewModel"/> class.
        /// </summary>
        public MerchantAddEditViewModel()
        {
            LoadProvinces();
        }

        private void LoadProvinces()
        {
            Provinces.Clear();

            IsLoadingCity = true;
            var action = new Action(() =>
            {
                var tempBanks = BankData.GetBanks(); 

                if (tempBanks != null)
                    foreach (var item in tempBanks)
                        DispatcherHelper.UIDispatcher.Invoke(new Action<BankInfo>(Banks.Add), item);
                //if (!isUpdate)
                SelectBank();

                var temp = CityData.GetAllState();
                if (temp != null)
                {
                    foreach (var item in temp)
                        DispatcherHelper.UIDispatcher.Invoke(new Action<CityModel>(Provinces.Add), item);
                }
                SelectProvince();
            });

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    IsLoadingCity = false;
                }));
            });
        }

        private void SelectBank()
        {
            if (inputModel.Bank == null || String.IsNullOrEmpty(InputModel.Bank.BankName))
            {
                if (Banks.Count > 0)
                    SelectedBank = Banks[0];
                return;
            }

            var exist = Banks.FirstOrDefault(m => m.Name == inputModel.Bank.BankName);
            SelectedBank = exist;
        }

        private void SelectProvince()
        {
            if (String.IsNullOrEmpty(InputModel.Bank.Address.Province))
            {
                if (Provinces.Count > 0)
                    SelectedProvince = Provinces[0];
                return;
            }

            var exist = Provinces.FirstOrDefault(m => m.State == InputModel.Bank.Address.Province);
            SelectedProvince = exist;
        }

        private void SelectCity()
        {
            if (String.IsNullOrEmpty(InputModel.Bank.Address.City))
            {
                if (Citys.Count > 0)
                    SelectedCity = Citys[0];
                return;
            }

            var exist = Citys.FirstOrDefault(m => m.City == InputModel.Bank.Address.City);
            if (exist == null && Citys.Count > 0)
                SelectedCity = Citys[0];
            else
                SelectedCity = exist;
        }

        #endregion

        #region 公开属性

        #region IsUpdate

        /// <summary>
        /// The <see cref="IsUpdate" /> property's name.
        /// </summary>
        public const string IsUpdatePropertyName = "IsUpdate";

        private bool isUpdate = false;

        /// <summary>
        /// 当前是编辑数据 或者是添加数据
        /// </summary>
        public bool IsUpdate
        {
            get { return isUpdate; }

            set
            {
                if (isUpdate == value) return;

                RaisePropertyChanging(IsUpdatePropertyName);
                isUpdate = value;
                RaisePropertyChanged(IsUpdatePropertyName);
            }
        }

        #endregion

        #region InputModel

        /// <summary>
        /// The <see cref="InputModel" /> property's name.
        /// </summary>
        public const string InputModelPropertyName = "InputModel";

        private RequestBusinessmanInfo inputModel = new RequestBusinessmanInfo() { Bank = new RequestBusinessmanInfo.RequestBankInfo() { Address = new RequestBusinessmanInfo.RequestBankAddress() } };

        /// <summary>
        /// 输入的数据
        /// </summary>
        public RequestBusinessmanInfo InputModel
        {
            get { return inputModel; }

            set
            {
                if (inputModel == value) return;

                RaisePropertyChanging(InputModelPropertyName);
                inputModel = value;
                RaisePropertyChanged(InputModelPropertyName);
            }
        }

        #endregion

        #region Provinces

        /// <summary>
        /// The <see cref="Provinces" /> property's name.
        /// </summary>
        public const string ProvincesPropertyName = "Provinces";

        private ObservableCollection<CityModel> provinces = new ObservableCollection<CityModel>();

        /// <summary>
        /// 所有省份
        /// </summary>
        public ObservableCollection<CityModel> Provinces
        {
            get { return provinces; }

            set
            {
                if (provinces == value) return;

                RaisePropertyChanging(ProvincesPropertyName);
                provinces = value;
                RaisePropertyChanged(ProvincesPropertyName);
            }
        }

        #endregion

        #region Citys

        /// <summary>
        /// The <see cref="Citys" /> property's name.
        /// </summary>
        public const string CitysPropertyName = "Citys";

        private ObservableCollection<CityModel> citys = new ObservableCollection<CityModel>();

        /// <summary>
        /// 所有城市
        /// </summary>
        public ObservableCollection<CityModel> Citys
        {
            get { return citys; }

            set
            {
                if (citys == value) return;

                RaisePropertyChanging(CitysPropertyName);
                citys = value;
                RaisePropertyChanged(CitysPropertyName);
            }
        }

        #endregion

        #region IsLoadingCity

        /// <summary>
        /// The <see cref="IsLoadingCity" /> property's name.
        /// </summary>
        public const string IsLoadingCityPropertyName = "IsLoadingCity";

        private bool isLoadingCity = false;

        /// <summary>
        /// 是否在加载城市
        /// </summary>
        public bool IsLoadingCity
        {
            get { return isLoadingCity; }

            set
            {
                if (isLoadingCity == value) return;

                RaisePropertyChanging(IsLoadingCityPropertyName);
                isLoadingCity = value;
                RaisePropertyChanged(IsLoadingCityPropertyName);
            }
        }

        #endregion

        #region SelectedProvince

        /// <summary>
        /// The <see cref="SelectedProvince" /> property's name.
        /// </summary>
        public const string SelectedProvincePropertyName = "SelectedProvince";

        private CityModel selectedProvince = null;

        /// <summary>
        /// 选中的省份
        /// </summary>
        public CityModel SelectedProvince
        {
            get { return selectedProvince; }

            set
            {
                if (selectedProvince == value) return;

                RaisePropertyChanging(SelectedProvincePropertyName);
                selectedProvince = value;
                RaisePropertyChanged(SelectedProvincePropertyName);

                LoadCity();
            }
        }

        private void LoadCity()
        {
            IsLoadingCity = true;
            DispatcherHelper.UIDispatcher.Invoke(new Action(Citys.Clear));

            var action = new Action(() =>
            {
                var temp = CityData.GetCity(SelectedProvince.City);
                if (temp != null)
                {
                    foreach (var item in temp)
                        DispatcherHelper.UIDispatcher.Invoke(new Action<CityModel>(Citys.Add), item);
                }
                SelectCity();
            });

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    IsLoadingCity = false;
                }));
            });
        }

        #endregion

        #region SelectedCity

        /// <summary>
        /// The <see cref="SelectedCity" /> property's name.
        /// </summary>
        public const string SelectedCityPropertyName = "SelectedCity";

        private CityModel selectedCity = null;

        /// <summary>
        /// 选中的城市
        /// </summary>
        public CityModel SelectedCity
        {
            get { return selectedCity; }

            set
            {
                if (selectedCity == value) return;

                RaisePropertyChanging(SelectedCityPropertyName);
                selectedCity = value;
                RaisePropertyChanged(SelectedCityPropertyName);
            }
        }

        #endregion

        #region Banks

        /// <summary>
        /// The <see cref="Banks" /> property's name.
        /// </summary>
        public const string BanksPropertyName = "Banks";

        private ObservableCollection<BankInfo> banks = new ObservableCollection<BankInfo>();

        /// <summary>
        /// 银行列表
        /// </summary>
        public ObservableCollection<BankInfo> Banks
        {
            get { return banks; }

            set
            {
                if (banks == value) return;

                RaisePropertyChanging(BanksPropertyName);
                banks = value;
                RaisePropertyChanged(BanksPropertyName);
            }
        }

        #endregion

        #region SelectedBank

        /// <summary>
        /// The <see cref="SelectedBank" /> property's name.
        /// </summary>
        public const string SelectedBankPropertyName = "SelectedBank";

        private BankInfo selectedBank = null;

        /// <summary>
        /// 选中的银行卡
        /// </summary>
        public BankInfo SelectedBank
        {
            get { return selectedBank; }

            set
            {
                if (selectedBank == value) return;

                RaisePropertyChanging(SelectedBankPropertyName);
                selectedBank = value;
                RaisePropertyChanged(SelectedBankPropertyName);
            }
        }

        #endregion

        #region IsOk

        /// <summary>
        /// The <see cref="IsOk" /> property's name.
        /// </summary>
        public const string IsOkPropertyName = "IsOk";

        private bool isOk = false;

        /// <summary>
        /// 操作是否完成
        /// </summary>
        public bool IsOk
        {
            get { return isOk; }

            set
            {
                if (isOk == value) return;

                RaisePropertyChanging(IsOkPropertyName);
                isOk = value;
                RaisePropertyChanged(IsOkPropertyName);
            }
        }

        #endregion

        #endregion

        #region IDataErrorInfo

        /// <summary>
        /// 获取指示对象何处出错的错误信息。
        /// </summary>
        /// <returns>指示对象何处出错的错误信息。默认值为空字符串 ("")。</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 获取具有给定名称的属性的错误信息。
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string this[string columnName]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region 公开命令

        #region SaveCommand

        private RelayCommand saveCommand;

        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            var test = InputModel;
            if (SelectedProvince != null)
            {
                InputModel.Bank.Address.Province = SelectedProvince.State;
            }
            if (SelectedCity != null)
            {
                InputModel.Bank.Address.City = SelectedCity.City;
            }
            if (SelectedBank != null)
            {
                InputModel.Bank.BankName = SelectedBank.Name;
            }

            InputModel.PosRate = decimal.Parse(InputModel.PosRate.ToString("f2")) / 100;

            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    if (isUpdate)
                    {
                        service.UpdateBusinessman(inputModel);
                        UIManager.ShowMessage("修改成功");
                    }
                    else
                    {
                        service.AddBusinessman(inputModel);
                        UIManager.ShowMessage("添加成功");
                    }

                    IsOk = true;
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteSaveCommand()
        {
            return !IsBusy;
        }

        #endregion

        #endregion

        #region 公开方法

        /// <summary>
        /// Loads the information.
        /// </summary>
        /// <param name="id">The identifier.</param>
        internal void LoadInfo(string id)
        {
            IsUpdate = true;
            IsBusy = true;
            Action action = () =>
            {
                var temp = GetBusinessmanInfo(id);
                temp.PosRate = temp.PosRate * 100;
                InputModel = temp;
                SelectBank();
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        public RequestBusinessmanInfo GetBusinessmanInfo(string id, bool throwEx = false)
        {
            RequestBusinessmanInfo result = null;
            CommunicateManager.Invoke<ITPosService>(service =>
            {
                result = Transfer(service.GetBusinessmanInfo(id));
                result.Id = id;
            }, UIManager.ShowErr);

            return result;
        }

        private RequestBusinessmanInfo Transfer(ResponseBusinessmanInfo info)
        {
            RequestBusinessmanInfo result = new RequestBusinessmanInfo();
            result.Id = info.Id;
            result.Address = info.Address;
            result.Bank = new RequestBusinessmanInfo.RequestBankInfo()
            {
                Address = new RequestBusinessmanInfo.RequestBankAddress()
                {
                    City = info.City,
                    Province = info.Province,
                    Subbranch = info.Subbranch
                },
                BankName = info.BankName,
                Cardholder = info.Cardholder,
                CardNo = info.CardNo,
                BankId = info.BankId
            };
            result.BusinessmanName = info.BusinessmanName;
            result.LinkMan = info.LinkMan;
            result.LinkPhone = info.LinkPhone;
            result.LinkTel = info.LinkTel;
            result.PosRate = info.PosRate;
            return result;
        }

        #endregion

    }
}
