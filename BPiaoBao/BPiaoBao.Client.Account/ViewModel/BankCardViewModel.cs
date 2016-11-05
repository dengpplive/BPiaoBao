using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 银行卡视图模型
    /// </summary>
    public class BankCardViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="BankCardViewModel"/> class.
        /// </summary>
        public BankCardViewModel()
        {
            if (IsInDesignMode)
                return;

            IsShowError = true;

            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性

        #region BankCards

        /// <summary>
        /// The <see cref="BankCards" /> property's name.
        /// </summary>
        private const string BankCardsPropertyName = "BankCards";

        private ObservableCollection<BankCardDto> _bankCards = new ObservableCollection<BankCardDto>();

        /// <summary>
        /// 银行卡列表
        /// </summary>
        public ObservableCollection<BankCardDto> BankCards
        {
            get { return _bankCards; }

            set
            {
                if (_bankCards == value) return;

                RaisePropertyChanging(BankCardsPropertyName);
                _bankCards = value;
                RaisePropertyChanged(BankCardsPropertyName);
            }
        }

        #endregion

        #region DefaultBankCard

        /// <summary>
        /// The <see cref="DefaultBankCard" /> property's name.
        /// </summary>
        private const string DefaultBankCardPropertyName = "DefaultBankCard";

        private BankCardDto _defaultBankCard;

        /// <summary>
        /// 默认银行卡
        /// </summary>
        public BankCardDto DefaultBankCard
        {
            get { return _defaultBankCard; }

            set
            {
                if (_defaultBankCard == value) return;

                RaisePropertyChanging(DefaultBankCardPropertyName);
                _defaultBankCard = value;
                RaisePropertyChanged(DefaultBankCardPropertyName);
            }
        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否在繁忙状态
        /// </summary>
        public new bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_queryCommand != null)
                    _queryCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        /// <summary>
        /// 获取错误时，是否显示消息框
        /// </summary>
        private bool IsShowError { get; set; }

        #endregion

        #region 公开命令

        #region QueryCommand

        private RelayCommand _queryCommand;

        /// <summary>
        /// 查询银行卡命令
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return _queryCommand ?? (_queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        private void ExecuteQueryCommand()
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(BankCards.Clear));
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var temp = service.GetBank();
                DefaultBankCard = temp.FirstOrDefault(m => m.IsDefault);

                if (temp == null) return;
                foreach (var item in temp)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<BankCardDto>(BankCards.Add), item);
                }
            }, ex =>
            {
                if (IsShowError)
                    UIManager.ShowErr(ex);
                else
                    Logger.WriteLog(LogType.INFO, "获取银行卡信息失败", ex);
            });

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private void SetData(BankCardDto exist, BankCardDto item)
        {
            exist.BankBranch = item.BankBranch;
            exist.BankId = item.BankId;
            exist.CardNo = item.CardNo;
            exist.City = item.City;
            exist.IsDefault = item.IsDefault;
            exist.Name = item.Name;
            exist.Owner = item.Owner;
            exist.Province = item.Province;
        }

        private bool CanExecuteQueryCommand()
        {
            return !isBusy;
        }

        #endregion

        #region ModifyBankCardCommand

        private RelayCommand<BankCardDto> _modifyBankCardCommand;

        public RelayCommand<BankCardDto> ModifyBankCardCommand
        {
            get
            {
                return _modifyBankCardCommand ??
                       (_modifyBankCardCommand = new RelayCommand<BankCardDto>(ExcuteModifyBankCardCommand, CanExcuteModifyBankCardCommand));
            }
        }

        private void ExcuteModifyBankCardCommand(BankCardDto bankCard)
        {
            LocalUIManager.ModifyBank(bankCard,resut =>
            {
                if (resut == null || !resut.Value) return;
                //添加完成刷新界面
                ExecuteQueryCommand();
            });
        }


        private bool CanExcuteModifyBankCardCommand(BankCardDto bankCard)
        {
            return bankCard != null;
        }
        #endregion

        #region DeleteBankCardCommand

        private RelayCommand<BankCardDto> _deleteBankCardCommand;

        /// <summary>
        /// 删除机票命令
        /// </summary>
        public RelayCommand<BankCardDto> DeleteBankCardCommand
        {
            get
            {
                return _deleteBankCardCommand ?? (_deleteBankCardCommand = new RelayCommand<BankCardDto>(ExecuteDeleteBankCardCommand, CanExecuteDeleteBankCardCommand));
            }
        }

        private void ExecuteDeleteBankCardCommand(BankCardDto bankCard)
        {
            var dialogResult = UIManager.ShowMessageDialog("确定要删除银行卡？");
            if (dialogResult == null || !dialogResult.Value)
                return;

            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.RemoveBank(bankCard.BankId);
                DispatcherHelper.UIDispatcher.Invoke(new Func<BankCardDto, bool>(_bankCards.Remove), bankCard);
                _bankCards.Remove(bankCard);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteDeleteBankCardCommand(BankCardDto bankCard)
        {
            return bankCard != null;
        }

        #endregion

        #region SetDefaultCardCommand

        private RelayCommand<BankCardDto> _setDefaultCardCommand;

        /// <summary>
        /// 设置默认卡号
        /// </summary>
        public RelayCommand<BankCardDto> SetDefaultCardCommand
        {
            get
            {
                return _setDefaultCardCommand ?? (_setDefaultCardCommand = new RelayCommand<BankCardDto>(ExecuteSetDefaultCardCommand, CanExecuteSetDefaultCardCommand));
            }
        }

        private void ExecuteSetDefaultCardCommand(BankCardDto card)
        {
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                service.SetDefaultBank(card.BankId);
                //刷新界面
                ExecuteQueryCommand();
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        private bool CanExecuteSetDefaultCardCommand(BankCardDto card)
        {
            return card != null;
        }

        #endregion

        #region AddBankCardCommand

        private RelayCommand _addBankCardCommand;

        /// <summary>
        /// 新增银行卡
        /// </summary>
        public RelayCommand AddBankCardCommand
        {
            get
            {
                return _addBankCardCommand ?? (_addBankCardCommand = new RelayCommand(ExecuteAddBankCardCommand, CanExecuteAddBankCardCommand));
            }
        }

        private void ExecuteAddBankCardCommand()
        {
            LocalUIManager.AddBank(resut =>
            {
                if (resut != null && resut.Value)
                {
                    //添加完成刷新界面
                    ExecuteQueryCommand();
                }
            });
        }

        private bool CanExecuteAddBankCardCommand()
        {
            return true;
        }

        #endregion

        #endregion

        /// <summary>
        /// 刷新数据
        /// </summary>
        internal void RefreshBankCards()
        {
            IsShowError = false;
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
            IsShowError = true;
        }
    }
}
