using System;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 临时额度申请ViewModel
    /// </summary>
    public class TempCreditApplyViewModel : BaseVM
    {
        #region 构造函数
        /// <summary>
        /// Initializes a new instance of the <see cref="TempCreditApplyViewModel"/> class.
        /// </summary>
        public TempCreditApplyViewModel(string day,string number)
        {
            if (IsInDesignMode)
                return;
            Day = day;
            Number = number;
            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryApplyTemporaryLimitCommand())
                ExecuteQueryApplyTemporaryLimitCommand();
        }

        #endregion

        #region 公开属性
        #region TempCreditAmount

        /// <summary>
        /// The <see cref="TempCreditAmount" /> property's name.
        /// </summary>
        private const string TempCreditAmountPropertyName = "TempCreditAmount";

        private decimal _tempCreditAmount;

        /// <summary>
        /// 可申请的临时额度
        /// </summary>
        public decimal TempCreditAmount
        {
            get { return _tempCreditAmount; }

            set
            {
                if (_tempCreditAmount == value) return;

                RaisePropertyChanging(TempCreditAmountPropertyName);
                _tempCreditAmount = value;
                RaisePropertyChanged(TempCreditAmountPropertyName);
            }
        }

        #endregion
        #region ApplyTempCreditAmount

        /// <summary>
        /// The <see cref="ApplyTempCreditAmount" /> property's name.
        /// </summary>
        private const string ApplyTempCreditAmountPropertyName = "ApplyTempCreditAmount";

        private decimal _applyTempCreditAmountPropertyName;

        /// <summary>
        /// 申请的临时额度
        /// </summary>
        public decimal ApplyTempCreditAmount
        {
            get { return _applyTempCreditAmountPropertyName; }

            set
            {
                if (_applyTempCreditAmountPropertyName == value) return;
                RaisePropertyChanging(ApplyTempCreditAmountPropertyName);
                _applyTempCreditAmountPropertyName = value;
                RaisePropertyChanged(ApplyTempCreditAmountPropertyName);
            }
        }

        #endregion
        #region IsDone

        /// <summary>
        /// The <see cref="IsDone" /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

        /// <summary>
        /// 是否已经申请
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
        #region IsValidate

        /// <summary>
        /// The <see cref="IsValidate" /> property's name.
        /// </summary>
        private const string IsValidatePropertyName = "IsValidate";

        private bool _isValidate = true;

        /// <summary>
        /// 是否满足条件
        /// </summary>
        public bool IsValidate
        {
            get { return _isValidate; }

            set
            {
                if (_isValidate == value) return;

                RaisePropertyChanging(IsValidatePropertyName);
                _isValidate = value;
                RaisePropertyChanged(IsValidatePropertyName);
            }
        }

        #endregion
        #region Day

        /// <summary>
        /// The <see cref="Day" /> property's name.
        /// </summary>
        private const string DayPropertyName = "Day";

        private string _day;

        /// <summary>
        /// 逾期范围
        /// </summary>
        public string Day
        {
            get { return _day; }

            set
            {
                if (_day == value) return;

                RaisePropertyChanging(DayPropertyName);
                _day = value;
                RaisePropertyChanged(DayPropertyName);
            }
        }

        #endregion
        #region Number

        /// <summary>
        /// The <see cref="Number" /> property's name.
        /// </summary>
        private const string NumberPropertyName = "Number";

        private string _number;

        /// <summary>
        /// 申请次数
        /// </summary>
        public string Number
        {
            get { return _number; }

            set
            {
                if (_number == value) return;

                RaisePropertyChanging(NumberPropertyName);
                _number = value;
                RaisePropertyChanged(NumberPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令
        #region QueryApplyTemporaryLimitCommand

        private RelayCommand _queryApplyTemporaryLimitCommand;

        /// <summary>
        /// 查询可用临时额度
        /// </summary>
        public RelayCommand QueryApplyTemporaryLimitCommand
        {
            get
            {
                return _queryApplyTemporaryLimitCommand ?? (_queryApplyTemporaryLimitCommand = new RelayCommand(ExecuteQueryApplyTemporaryLimitCommand, CanExecuteQueryApplyTemporaryLimitCommand));
            }
        }

        private void ExecuteQueryApplyTemporaryLimitCommand()
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFundService>(p =>
            {
                TempCreditAmount = p.GetTempCreditAmount();
            }, e =>
            {
                UIManager.ShowErr(e);
                IsValidate = false;
            });

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteQueryApplyTemporaryLimitCommand()
        {
            return !IsBusy;
        }

        #endregion
        #region ApplyTemporaryLimitCommand

        private RelayCommand<string> _applyTemporaryLimitCommand;

        /// <summary>
        /// 临时额度申请
        /// </summary>
        public RelayCommand<string> ApplyTemporaryLimitCommand
        {
            get
            {
                return _applyTemporaryLimitCommand ?? (_applyTemporaryLimitCommand = new RelayCommand<string>(ExecuteApplyTemporaryLimitCommand, CanExecuteApplyTemporaryLimitCommand));
            }
        }

        private void ExecuteApplyTemporaryLimitCommand(string pwd)
        {
            if (ApplyTempCreditAmount == 0)
            {
                UIManager.ShowMessage("申请临时额度不能为零");
                return;
            }
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFundService>(p =>
            {
                p.TempCreditApplication(pwd, ApplyTempCreditAmount);
                UIManager.ShowMessage("申请临时额度成功");
                IsDone = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteApplyTemporaryLimitCommand(string pwd)
        {
            return !IsBusy && !string.IsNullOrEmpty(pwd) && !IsDone;
        }

        #endregion
        #endregion
    }
}
