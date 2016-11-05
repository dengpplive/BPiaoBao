using System.Linq;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class AddOperatorViewModel : BaseVM, IDataErrorInfo
    {
        #region 公有属性

        #region IsAddMode

        /// <summary>
        /// The <see cref="IsAddMode" /> property's name.
        /// </summary>
        private const string IsAddModePropertyName = "IsAddMode";

        private bool _isAddMode = true;

        /// <summary>
        /// 当前是否为添加模式
        /// </summary>
        public bool IsAddMode
        {
            get { return _isAddMode; }

            set
            {
                if (_isAddMode == value) return;

                RaisePropertyChanging(IsAddModePropertyName);
                _isAddMode = value;
                RaisePropertyChanged(IsAddModePropertyName);
            }
        }

        #endregion

        #region Operator

        /// <summary>
        /// The <see cref="Operator" /> property's name.
        /// </summary>
        private const string OperatorPropertyName = "Operator";

        private OperatorDto _operatorDto;

        /// <summary>
        /// 对象
        /// </summary>
        public OperatorDto Operator
        {
            get { return _operatorDto; }

            private set
            {
                if (_operatorDto == value) return;

                RaisePropertyChanging(OperatorPropertyName);
                _operatorDto = value;
                RaisePropertyChanged(OperatorPropertyName);
            }
        }

        #endregion

        #region Title

        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        private const string TitlePropertyName = "添加员工";

        private string _title;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _title; }

            set
            {
                if (_title == value) return;

                RaisePropertyChanging(TitlePropertyName);
                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        #endregion

        public AddOperatorViewModel()
        {
            Operator = new OperatorDto();
        }

        public AddOperatorViewModel(OperatorDto operatorDto)
        {
            InitlizeEdit(operatorDto);
        }

        public string RealName
        {
            get { return Operator.Realname; }
            set
            {
                if (Operator.Realname == value) return;
                Operator.Realname = value;
                RaisePropertyChanged("RealName");
            }
        }
        public string Tel
        {
            get { return Operator.Tel; }
            set
            {
                if (Operator.Tel != value)
                {
                    Operator.Tel = value;
                    RaisePropertyChanged("Tel");
                }
            }
        }

        public string Phone
        {
            get { return Operator.Phone; }
            set
            {
                if (Operator.Phone == value) return;
                Operator.Phone = value;
                RaisePropertyChanged("Phone");
            }
        }

        public string Account
        {
            get { return Operator.Account; }
            set
            {
                if (Operator.Account == value) return;
                Operator.Account = value;
                RaisePropertyChanged("Account");
            }
        }
        public string Password
        {
            get { return Operator.Password; }
            set
            {
                if (Operator.Password == value) return;
                Operator.Password = value;
                RaisePropertyChanged("Password");
            }
        }

        private string _comparisonPassword;
        public string ComparisonPassword
        {
            get { return _comparisonPassword; }
            set
            {
                if (_comparisonPassword == value) return;
                _comparisonPassword = value;
                RaisePropertyChanged("ComparisonPassword");
            }
        }

        #region IsOk

        /// <summary>
        /// The <see cref="IsOk" /> property's name.
        /// </summary>
        private const string IsOkPropertyName = "IsOk";

        private bool _isOk;

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsOk
        {
            get { return _isOk; }

            set
            {
                if (_isOk == value) return;

                RaisePropertyChanging(IsOkPropertyName);
                _isOk = value;
                RaisePropertyChanged(IsOkPropertyName);
            }
        }

        #endregion

        #endregion

        #region 属性验证
        //需要验证的属性
        static readonly string[] ValidateProperties = 
        {   "RealName",
            "Tel",
            "Phone",
            "Account",
            "Password",
            "ComparisonPassword"
        };
        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidateProperties, propertyName) < 0)
                return null;
            string error = null;
            switch (propertyName)
            {
                case "RealName":
                    if (string.IsNullOrEmpty(RealName))
                        error = "*";
                    break;
                case "Tel":
                    if (string.IsNullOrEmpty(Tel))
                        error = "*";
                    break;
                //case "Phone":
                //    if (string.IsNullOrEmpty(Phone))
                //        error = "*";
                //    break;
                case "Account":
                    if (!IsAddMode)
                        return null;
                    error = ValidateAccount();
                    break;
                case "Password":
                    if (!IsAddMode)
                        return null;
                    error = ValidatePassword();
                    break;
                case "ComparisonPassword":
                    if (!IsAddMode)
                        return null;
                    error = ValidateConfirmPassword();
                    break;
            }
            return error;
        }
        string ValidateAccount()
        {
            if (string.IsNullOrEmpty(Account))
                return "*";
            bool isExist = false;
            CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                isExist = p.IsExistAccount(Account);
            }, UIManager.ShowErr);
            return isExist ? string.Empty : "此用户名已存在";
        }
        string ValidatePassword()
        {
            if (string.IsNullOrEmpty(Password))
                return "*";
            return Password.Length < 6 ? "密码长度至少6位" : string.Empty;
        }
        string ValidateConfirmPassword()
        {
            if (string.IsNullOrEmpty(ComparisonPassword))
                return "*";
            return Password != ComparisonPassword ? "密码输入不一致" : string.Empty;
        }
        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get { return GetValidationError(columnName); }
        }
        #endregion

        #region 命令
        private RelayCommand _saveCommand;

        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand));
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return ValidateProperties.All(property => string.IsNullOrEmpty(GetValidationError(property)));
        }

        private void ExecuteSaveCommand()
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                if (!IsAddMode)
                    p.UpdateOperator(Operator);
                else
                    p.AddOperator(Operator);
                //Messenger.Default.Send<bool>(true, "CloseAddOperator");
                IsOk = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                {
                    IsBusy = false;
                }));
            });
        }

        ///// <summary>
        ///// 取消命令
        ///// </summary>
        //public ICommand CancelCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() =>
        //        {
        //            Messenger.Default.Send<bool>(false, "CloseAddOperator");
        //        });
        //    }
        //}

        #endregion

        private void InitlizeEdit(OperatorDto operatorDto)
        {
            operatorDto.Password = "******";
            Operator = operatorDto;
            //Operator.Password = "";
            ComparisonPassword = "******";

            IsAddMode = false;
            Title = "修改员工";
        }
    }
}
