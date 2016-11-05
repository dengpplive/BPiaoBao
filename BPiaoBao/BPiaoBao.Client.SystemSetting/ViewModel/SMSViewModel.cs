using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

namespace BPiaoBao.Client.SystemSetting.ViewModel
{
    public class SMSViewModel : BaseVM
    {
        public SMSViewModel()
        {
            EndTime = DateTime.Now;
            StartTime = EndTime.Value.AddMonths(-1);

            if (IsInDesignMode)
                return;

            Initialize();
            Messenger.Default.Register<bool>(this, "SMSRefresh", p =>
            {
                if (p)
                {
                    Initialize();
                }
            });
        }
        private bool _result = true;
        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (!_result) return;
            _result = false;
            IsBuyDetail = true;
            CommunicateManager.Invoke<IBusinessmanService>(p =>
            {
                var tuple = p.GetSystemInfo();
                if (tuple == null) return;
                BuySmsPrice = tuple.Item3;
                RemainCount = tuple.Item1;
                SendCount = tuple.Item2;
            }, UIManager.ShowErr);

            SendDetailViewModel = SendDetailViewModel.CreateInstance();
            BuyDetailViewModel = BuyDetailViewModel.CreateInstance();
            GiveDetailViewModel = GiveDetailViewModel.CreateInstance();
            _result = true;
        }

        /// <summary>
        /// 购买短信价格
        /// </summary>
        private const string BuySmsPricePropertyName = "BuySmsPrice";
        private decimal _buySmsPrice;
        public decimal BuySmsPrice
        {
            get { return _buySmsPrice; }
            set
            {
                if (_buySmsPrice == value) return;
                RaisePropertyChanging(BuySmsPricePropertyName);
                _buySmsPrice = value;
                RaisePropertyChanged(BuySmsPricePropertyName);
            }
        }
        /// <summary>
        /// 短信剩余条数
        /// </summary>
        private const string RemainCountPropertyName = "RemainCount";
        private int _remainCount;
        public int RemainCount
        {
            get { return _remainCount; }
            set
            {
                if (_remainCount == value) return;
                RaisePropertyChanging(RemainCountPropertyName);
                _remainCount = value;
                RaisePropertyChanged(RemainCountPropertyName);
            }
        }
        /// <summary>
        /// 已发送条数
        /// </summary>
        private const string SendCountPropertyName = "SendCount";
        private int _sendCount;
        public int SendCount
        {
            get { return _sendCount; }
            set
            {
                if (_sendCount == value) return;
                RaisePropertyChanging(SendCountPropertyName);
                _sendCount = value;
                RaisePropertyChanged(SendCountPropertyName);
            }
        }

        private DateTime? _startTime;
        [DataType(DataType.DateTime, ErrorMessage = @"格式不正确")]
        public DateTime? StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime == value) return;
                _startTime = value;
                RaisePropertyChanged("StartTime");
            }
        }

        private DateTime? _endTime;
        [DataType(DataType.DateTime, ErrorMessage = @"格式不正确")]
        public DateTime? EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime == value) return;
                _endTime = value;
                RaisePropertyChanged("EndTime");
            }
        }

        private bool _isBuyDetail;
        public bool IsBuyDetail
        {
            get { return _isBuyDetail; }
            set
            {
                if (_isBuyDetail == value) return;
                _isBuyDetail = value;
                RaisePropertyChanged("IsBuyDetail");
                PropertyChanged += SMSViewModel_PropertyChanged;
            }
        }

        private bool _isSendDetail;
        public bool IsSendDetail
        {
            get { return _isSendDetail; }
            set
            {
                if (_isSendDetail == value) return;
                _isSendDetail = value;
                RaisePropertyChanged("IsSendDetail");
                PropertyChanged += SMSViewModel_PropertyChanged;
            }
        }
        private bool _isGiveDetail;
        public bool IsGiveDetail
        {
            get { return _isGiveDetail; }
            set
            {
                if (_isGiveDetail != value)
                {
                    _isGiveDetail = value;
                    RaisePropertyChanged("IsGiveDetail");
                    PropertyChanged += SMSViewModel_PropertyChanged;
                }
            }
        }

        private const string OutTradeNoProtertyName = "OutTradeNo";
        private string _outTradeNo = "";

        public string OutTradeNo
        {
            get { return _outTradeNo.Trim(); }
            set
            {
                if (_outTradeNo == value)
                {
                    return;
                }
                RaisePropertyChanging(OutTradeNoProtertyName);
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNoProtertyName);
            }
        }

        void SMSViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBuyDetail" && IsBuyDetail)
            {
                StartTime = BuyDetailViewModel.StartTime;
                EndTime = BuyDetailViewModel.EndTime;
            }
            else if (e.PropertyName == "IsSendDetail" && IsSendDetail)
            {
                StartTime = SendDetailViewModel.StartTime;
                EndTime = SendDetailViewModel.EndTime;
            }
            else if (e.PropertyName == "IsGiveDetail" && IsGiveDetail)
            {
                StartTime = GiveDetailViewModel.StartTime;
                EndTime = GiveDetailViewModel.EndTime;
            }
        }
        public ICommand QueryCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (IsBuyDetail)
                    {
                        BuyDetailViewModel.OutTradeNo = OutTradeNo;
                        BuyDetailViewModel.StartTime = StartTime;
                        BuyDetailViewModel.EndTime = EndTime;
                        BuyDetailViewModel.CurrentPageIndex = 1;
                        BuyDetailViewModel.Init();
                    }
                    else if(_isSendDetail)
                    {
                        SendDetailViewModel.StartTime = StartTime;
                        SendDetailViewModel.EndTime = EndTime;
                        SendDetailViewModel.CurrentPageIndex = 1;
                        SendDetailViewModel.Init();
                    }
                    else if (_isGiveDetail)
                    {
                        GiveDetailViewModel.StartTime = StartTime;
                        GiveDetailViewModel.EndTime = EndTime;
                        GiveDetailViewModel.CurrentPageIndex = 1;
                        GiveDetailViewModel.Init();
                    }
                });
            }
        }

        #region SendDetailViewModel

        /// <summary>
        /// The <see cref="SendDetailViewModel" /> property's name.
        /// </summary>
        private const string SendDetailViewModelPropertyName = "SendDetailViewModel";

        private SendDetailViewModel _sendDetailViewModel;

        /// <summary>
        /// 发送明细
        /// </summary>
        public SendDetailViewModel SendDetailViewModel
        {
            get { return _sendDetailViewModel; }

            set
            {
                if (_sendDetailViewModel == value) return;

                RaisePropertyChanging(SendDetailViewModelPropertyName);
                _sendDetailViewModel = value;
                RaisePropertyChanged(SendDetailViewModelPropertyName);
            }
        }

        #endregion

        #region BuyDetailViewModel

        /// <summary>
        /// The <see cref="BuyDetailViewModel" /> property's name.
        /// </summary>
        private const string BuyDetailViewModelPropertyName = "BuyDetailViewModel";

        private BuyDetailViewModel _buyDetailViewModel;

        /// <summary>
        /// 购买明细
        /// </summary>
        public BuyDetailViewModel BuyDetailViewModel
        {
            get { return _buyDetailViewModel; }

            set
            {
                if (_buyDetailViewModel == value) return;

                RaisePropertyChanging(BuyDetailViewModelPropertyName);
                _buyDetailViewModel = value;
                RaisePropertyChanged(BuyDetailViewModelPropertyName);
            }
        }

        #endregion

        #region GiveDetailViewModel

        /// <summary>
        /// The <see cref="GiveDetailViewModel" /> property's name.
        /// </summary>
        private const string GiveDetailViewModelPropertyName = "GiveDetailViewModel";

        private GiveDetailViewModel _giveDetailViewModel;

        /// <summary>
        /// 发送明细
        /// </summary>
        public GiveDetailViewModel GiveDetailViewModel
        {
            get { return _giveDetailViewModel; }

            set
            {
                if (_giveDetailViewModel == value) return;

                RaisePropertyChanging(GiveDetailViewModelPropertyName);
                _giveDetailViewModel = value;
                RaisePropertyChanged(GiveDetailViewModelPropertyName);
            }
        }

        #endregion

        public ICommand SmsBuyCommand
        {
            get
            {
                return new RelayCommand(() => Messenger.Default.Send(true));
            }
        }


    }
}
