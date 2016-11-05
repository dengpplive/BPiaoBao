using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class InsuranceManageViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see /> class.
        /// </summary>
        public InsuranceManageViewModel()
        {
            #region 行程单状态
            _allTravelStatus.Add(new KeyValuePair<EnumInsurancePurchaseType?, string>(null, "请选择"));
            _allTravelStatus.Add(new KeyValuePair<EnumInsurancePurchaseType?, string>(EnumInsurancePurchaseType.Normal, EnumHelper.GetDescription(EnumInsurancePurchaseType.Normal)));
            _allTravelStatus.Add(new KeyValuePair<EnumInsurancePurchaseType?, string>(EnumInsurancePurchaseType.Offer, EnumHelper.GetDescription(EnumInsurancePurchaseType.Offer)));
            #endregion
            //CreateTime = DateTime.Now.AddDays(-15).Date;
            //EndCreateTime = DateTime.Now.Date;
            Initialize();

            Messenger.Default.Register<bool>(this, "buyinsurancemessage", p =>
            {
                if (!p) return;
                Initialize();
            });
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

        #region SelectedTravelStatus

        /// <summary>
        /// The <see cref="SelectedTravelStatus" /> property's name.
        /// </summary>
        private const string SelectedTravelStatusPropertyName = "SelectedTravelStatus";

        private EnumInsurancePurchaseType? _selectedTravelStatus;

        /// <summary>
        /// 选中的行程单状态
        /// </summary>
        public EnumInsurancePurchaseType? SelectedTravelStatus
        {
            get { return _selectedTravelStatus; }

            set
            {
                if (_selectedTravelStatus == value) return;

                RaisePropertyChanging(SelectedTravelStatusPropertyName);
                _selectedTravelStatus = value;
                RaisePropertyChanged(SelectedTravelStatusPropertyName);
            }
        }

        #endregion

        #region AllTravelStatus

        /// <summary>
        /// The <see cref="AllTravelStatus" /> property's name.
        /// </summary>
        private const string AllTravelStatusPropertyName = "AllTravelStatus";

        private ObservableCollection<KeyValuePair<EnumInsurancePurchaseType?, String>> _allTravelStatus = new ObservableCollection<KeyValuePair<EnumInsurancePurchaseType?, string>>();

        /// <summary>
        /// 所有行程单状态
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumInsurancePurchaseType?, String>> AllTravelStatus
        {
            get { return _allTravelStatus; }

            set
            {
                if (_allTravelStatus == value) return;

                RaisePropertyChanging(AllTravelStatusPropertyName);
                _allTravelStatus = value;
                RaisePropertyChanged(AllTravelStatusPropertyName);
            }
        }

        #endregion

        #region LeaveCount
        /// <summary>
        /// 保险剩余份数
        /// The <see cref="LeaveCount" /> property's name.
        /// </summary>
        private const string LeaveCountPropertyName = "LeaveCount";

        private int _leaveCount;

        public int LeaveCount
        {
            get { return _leaveCount; }

            set
            {
                if (_leaveCount == value) return;

                RaisePropertyChanging(LeaveCountPropertyName);
                _leaveCount = value;
                RaisePropertyChanged(LeaveCountPropertyName);
            }
        }

        #endregion

        #region StartCreateTime

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string StartCreateTimePropertyName = "StartCreateTime";

        private DateTime? _startCreateTime;

        /// <summary>
        /// 创建时开始间 
        /// </summary>
        public DateTime? CreateTime
        {
            get {
                return _startCreateTime;
            }

            set
            {
                if (_startCreateTime == value) return;

                RaisePropertyChanging(StartCreateTimePropertyName);
                _startCreateTime = value;
                RaisePropertyChanged(StartCreateTimePropertyName);
            }
        }

        #endregion

        #region EndCreateTime

        /// <summary>
        /// The <see cref="EndCreateTime" /> property's name.
        /// </summary>
        private const string EndCreateTimePropertyName = "EndCreateTime";

        private DateTime? _endCreateTime;

        /// <summary>
        /// 创建时开始间 
        /// </summary>
        public DateTime? EndCreateTime
        {
            get {
                return _endCreateTime;
            }

            set
            {
                if (_endCreateTime == value) return;

                RaisePropertyChanging(EndCreateTimePropertyName);
                _endCreateTime = value;
                RaisePropertyChanged(EndCreateTimePropertyName);
            }
        }

        #endregion

        #region InsuranceDepositLogs

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string InsuranceDepositLogsPropertyName = "InsuranceDepositLogs";

        private List<ResponseInsurancePurchaseByBussinessman> _insuranceDepositLogs = new List<ResponseInsurancePurchaseByBussinessman>();

        /// <summary>
        /// 显示的购买记录
        /// </summary>
        public List<ResponseInsurancePurchaseByBussinessman> InsuranceDepositLogs
        {
            get { return _insuranceDepositLogs; }

            set
            {
                if (_insuranceDepositLogs == value) return;

                RaisePropertyChanging(InsuranceDepositLogsPropertyName);
                _insuranceDepositLogs = value;
                RaisePropertyChanged(InsuranceDepositLogsPropertyName);
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
        /// 是否正在忙碌
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

        #region 翻页

        #region PageSize

        /// <summary>
        /// The <see cref="PageSize" /> property's name.
        /// </summary>
        private const string PageSizePropertyName = "PageSize";

        private int _pageSize = 20;

        /// <summary>
        /// 翻页
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }

            set
            {
                if (_pageSize == value) return;

                RaisePropertyChanging(PageSizePropertyName);
                _pageSize = value;
                RaisePropertyChanged(PageSizePropertyName);
            }
        }

        #endregion

        #region CurrentPageIndex

        /// <summary>
        /// The <see cref="CurrentPageIndex" /> property's name.
        /// </summary>
        private const string CurrentPageIndexPropertyName = "CurrentPageIndex";

        private int _currentPageIndex = 1;

        /// <summary>
        /// 当前索引页
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }

            set
            {
                if (_currentPageIndex == value) return;

                RaisePropertyChanging(CurrentPageIndexPropertyName);
                _currentPageIndex = value;
                RaisePropertyChanged(CurrentPageIndexPropertyName);
            }
        }

        #endregion

        #region TotalCount

        /// <summary>
        /// The <see cref="TotalCount" /> property's name.
        /// </summary>
        private const string TotalCountPropertyName = "TotalCount";

        private int _totalCount;

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }

            set
            {
                if (_totalCount == value) return;

                RaisePropertyChanging(TotalCountPropertyName);
                _totalCount = value;
                RaisePropertyChanged(TotalCountPropertyName);
            }
        }

        #endregion

        #endregion

        #region OutTradeNo

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string OutTradeNoPropertyName = "OutTradeNo";

        private string _outTradeNo;

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo
        {
            get { return _outTradeNo; }

            set
            {
                if (_outTradeNo == value) return;

                RaisePropertyChanging(OutTradeNoPropertyName);
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNoPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region QueryCommand

        private RelayCommand _queryCommand;

        /// <summary>
        /// 查询命令
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return _queryCommand ?? (_queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        protected virtual void ExecuteQueryCommand()
        {
            if (CreateTime != null && EndCreateTime != null && CreateTime.Value.CompareTo(EndCreateTime.Value) > 0)
            {
                UIManager.ShowMessage("购买日期选择开始日期大于结束日期");
                return;
            }
            //查询实体赋值
            var rqp = new RequestQueryInsurancePurchaseByBussinessman
            {
                BuyEndTime = EndCreateTime,
                BuyStartTime = CreateTime,
                RequestFrom = 0,
                TradeNo = OutTradeNo
            };
            if (SelectedTravelStatus.HasValue) rqp.RecordType = SelectedTravelStatus.Value;
            IsBusy = true;
            //执行查询购买记录
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                #region 保险相关设置
                var re = service.GetCurentInsuranceCfgInfo(true);
                if (re.IsOpenCurrenCarrierInsurance && re.IsOpenUnexpectedInsurance)
                    LeaveCount = re.LeaveCount;
                #endregion
                var data = service.QueryInsurancePurchaseByBussinessman(rqp, CurrentPageIndex, PageSize);
                if (data.List == null)
                    return;
                TotalCount = data.TotalCount;
                InsuranceDepositLogs = data.List;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        protected virtual bool CanExecuteQueryCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region OpenCommand

        private RelayCommand _openCommand;

        /// <summary>
        /// 打开保险条款页面命令
        /// </summary>
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(ExecuteOpenCommand, CanExecuteCommand));
            }
        }

        private void ExecuteOpenCommand()
        {
            UIManager.ShowWeb("买票宝保险条款", "http://www.51cbc.cn/CreditInstruction.html");
        }

        private bool CanExecuteCommand()
        {
            return true;
        }

        #endregion

        #region InsuranceCommand

        private RelayCommand _insuranceCommand;

        /// <summary>
        /// 打开投保命令
        /// </summary>
        public RelayCommand InsuranceCommand
        {
            get
            {
                return _insuranceCommand ?? (_insuranceCommand = new RelayCommand(ExecuteInsuranceCommand, CanExecuteInsuranceCommand));
            }
        }

        private void ExecuteInsuranceCommand()
        {
            LocalUIManager.ShowBuySingleInsuranceWindow();
        }

        private bool CanExecuteInsuranceCommand()
        {
            return !isBusy;
        }

        #endregion

        #region BuyCommand

        private RelayCommand _buyCommand;

        /// <summary>
        /// 打开购买命令
        /// </summary>
        public RelayCommand BuyCommand
        {
            get
            {
                return _buyCommand ?? (_buyCommand = new RelayCommand(ExecuteBuyCommand, CanExecuteBuyCommand));
            }
        }

        private void ExecuteBuyCommand()
        {
            LocalUIManager.ShowBuyInsurance(dialogResult =>
            {
                if (dialogResult == null || !dialogResult.Value) return;
                ExecuteQueryCommand();
            });
        }

        private bool CanExecuteBuyCommand()
        {
            return !isBusy;
        }

        #endregion

        #endregion
    }
}
