using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model.Enumeration;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.TPOS.ViewModel
{
    /// <summary>
    /// POS机管理视图模型
    /// </summary>
    public class POSManagerViewModel : PageBaseViewModel
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="POSManagerViewModel"/> class.
        /// </summary>
        public POSManagerViewModel()
        {
            this.CurrentPageIndex = 1;
            PosStatus.Add(GetPosStatusData(UIExt.Model.Enumeration.PosStatus.All));
            PosStatus.Add(GetPosStatusData(UIExt.Model.Enumeration.PosStatus.Assigned));
            PosStatus.Add(GetPosStatusData(UIExt.Model.Enumeration.PosStatus.Unassigned));
            this.Initialize();
        }

        private Tuple<PosStatus, string> GetPosStatusData(PosStatus posStatus)
        {
            return new Tuple<PosStatus, string>(posStatus, EnumHelper.GetDescription(posStatus));
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;

            Task t = new Task(() =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    var temp = service.GetAccountStat();
                    if (temp != null)
                        this.AccountStat = temp;

                    if (CanExecuteQueryCommand())
                        ExecuteQueryCommand();

                }, UIManager.ShowErr);
            });
            t.Start();
            t.ContinueWith((task) =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    IsBusy = false;
                }));
            });
        }

        private bool? GetAssign(PosStatus SelectedPosStatus)
        {
            if (SelectedPosStatus == UIExt.Model.Enumeration.PosStatus.All)
                return null;

            return SelectedPosStatus == UIExt.Model.Enumeration.PosStatus.Assigned;
        }

        #endregion

        #region 公开属性

        #region PosStatus

        /// <summary>
        /// The <see cref="PosStatus" /> property's name.
        /// </summary>
        public const string PosStatusPropertyName = "PosStatus";

        private ObservableCollection<Tuple<PosStatus, string>> posStatus = new ObservableCollection<Tuple<PosStatus, string>>();

        /// <summary>
        /// 订单状态
        /// </summary>
        public ObservableCollection<Tuple<PosStatus, string>> PosStatus
        {
            get { return posStatus; }

            set
            {
                if (posStatus == value) return;

                RaisePropertyChanging(PosStatusPropertyName);
                posStatus = value;
                RaisePropertyChanged(PosStatusPropertyName);
            }
        }

        #endregion

        #region SelectedPosStatus

        /// <summary>
        /// The <see cref="SelectedPosStatus" /> property's name.
        /// </summary>
        public const string SelectedPosStatusPropertyName = "SelectedPosStatus";

        private PosStatus selectedPosStatus = UIExt.Model.Enumeration.PosStatus.All;

        /// <summary>
        /// 选中的状态
        /// </summary>
        public PosStatus SelectedPosStatus
        {
            get { return selectedPosStatus; }

            set
            {
                if (selectedPosStatus == value) return;

                RaisePropertyChanging(SelectedPosStatusPropertyName);
                selectedPosStatus = value;
                RaisePropertyChanged(SelectedPosStatusPropertyName);
            }
        }

        #endregion

        #region InputPos

        /// <summary>
        /// The <see cref="InputPos" /> property's name.
        /// </summary>
        public const string InputPosPropertyName = "InputPos";

        private string inputPos = null;

        /// <summary>
        /// 输入的Pos号
        /// </summary>
        public string InputPos
        {
            get { return inputPos; }

            set
            {
                if (inputPos == value) return;

                RaisePropertyChanging(InputPosPropertyName);
                inputPos = value;
                RaisePropertyChanged(InputPosPropertyName);
            }
        }

        #endregion

        #region InputMerchantName

        /// <summary>
        /// The <see cref="InputMerchantName" /> property's name.
        /// </summary>
        public const string InputMerchantNamePropertyName = "InputMerchantName";

        private string inputMerchantName = null;

        /// <summary>
        /// 输入的商户名
        /// </summary>
        public string InputMerchantName
        {
            get { return inputMerchantName; }

            set
            {
                if (inputMerchantName == value) return;

                RaisePropertyChanging(InputMerchantNamePropertyName);
                inputMerchantName = value;
                RaisePropertyChanged(InputMerchantNamePropertyName);
            }
        }

        #endregion

        #region PosList

        /// <summary>
        /// The <see cref="PosList" /> property's name.
        /// </summary>
        public const string PosListPropertyName = "PosList";

        private ObservableCollection<PosInfoDataObject> posList = new ObservableCollection<PosInfoDataObject>();

        /// <summary>
        /// pos机列表
        /// </summary>
        public ObservableCollection<PosInfoDataObject> PosList
        {
            get { return posList; }

            set
            {
                if (posList == value) return;

                RaisePropertyChanging(PosListPropertyName);
                posList = value;
                RaisePropertyChanged(PosListPropertyName);
            }
        }

        #endregion

        #region AccountStatDataObject
        public const string AccountStatPropertyName = "AccountStat";
        private AccountStatDataObject accountStat = new AccountStatDataObject();

        public AccountStatDataObject AccountStat
        {
            get { return accountStat; }
            set
            {
                if (accountStat == value) return;
                RaisePropertyChanging(AccountStatPropertyName);
                accountStat = value;
                RaisePropertyChanged(AccountStatPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        private bool isLoadingList;

        #region QueryCommand
        protected override void ExecuteQueryCommand()
        {
            isLoadingList = IsBusy = true;
            DispatcherHelper.UIDispatcher.Invoke(new Action(PosList.Clear));
            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    //获取Pos列表
                    var temp = service.GetPosList(InputPos, inputMerchantName, GetAssign(SelectedPosStatus), (CurrentPageIndex - 1) * PageSize, PageSize);
                    TotalCount = temp.TotalCount;
                    if (temp.List != null)
                        foreach (var item in temp.List)
                            DispatcherHelper.UIDispatcher.Invoke(new Action<PosInfoDataObject>(PosList.Add), item);

                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { isLoadingList = IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        protected override bool CanExecuteQueryCommand()
        {
            return !isLoadingList;
        }

        #endregion

        #region RecoveryCommand

        private RelayCommand<PosInfoDataObject> _recoveryCommand;
        /// <summary>
        /// Gets the RecoveryCommand.
        /// </summary>
        public RelayCommand<PosInfoDataObject> RecoveryCommand
        {
            get
            {
                return _recoveryCommand ?? (_recoveryCommand = new RelayCommand<PosInfoDataObject>(ExecuteRecoveryCommand));
            }
        }

        private void ExecuteRecoveryCommand(PosInfoDataObject posInfo)
        {
            bool? isRecovery = MessageBoxExt.Show("提示", "待该POS机从商户处完成收回才可以\r\n进行回收，是否确认并回收?", MessageImageType.Info, MessageBoxButtonType.OKCancel);
            if (isRecovery.HasValue && isRecovery.Value)
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    service.RetrievePos(posInfo.CompanyID, posInfo.PosNo);
                    this.Initialize();
                }, UIManager.ShowErr);
            }
        }

        #endregion

        #region DistributionLogCommand

        private RelayCommand<string> _distributionLogViewModel;

        /// <summary>
        /// Gets the DistributionLogCommand.
        /// </summary>
        public RelayCommand<string> DistributionLogCommand
        {
            get
            {
                return _distributionLogViewModel ?? (_distributionLogViewModel = new RelayCommand<string>(ExecuteDistributionLogCommand));
            }
        }

        private void ExecuteDistributionLogCommand(string posNo)
        {
            if (string.IsNullOrEmpty(posNo))
            {
                MessageBoxExt.Show("提示", "获取POS编号失败！", MessageImageType.Info);
                return;
            }
            LocalUIManager.DistributionLog(posNo);
        }

        #endregion

        #endregion
    }
}
