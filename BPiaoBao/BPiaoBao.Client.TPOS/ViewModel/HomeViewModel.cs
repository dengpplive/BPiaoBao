using BPiaoBao.AppServices.Contracts.TPos;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
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
    /// TPOS主页视图模型
    /// </summary>
    public class HomeViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        public HomeViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<ITPosService>(service =>
                {
                    //获取帐户信息
                    AccountStatData = service.GetAccountStat();

                    //获取客服电话
                    var tempService = CommunicationProxy.GetCustomerService();
                    var serviceMode = tempService.HotlinePhone.FirstOrDefault(m => m.Key != null && m.Key.ToLower().Contains("pos"));
                    if (serviceMode != null)
                        ServiceHotline = serviceMode.Value;

                    //获取统计数据
                    int totalDays = 7;//统计的天数
                    var tempServerData = service.GainStat(DateTime.Today.AddDays(-totalDays), DateTime.Now);
                    if (tempServerData == null)
                        return;

                    Collection<TradeStatDataObject> tempCollection = new Collection<TradeStatDataObject>();
                    for (int i = 0; i < totalDays; i++)
                    {
                        var tempDate = DateTime.Today.AddDays(-totalDays + i + 1);
                        var serverModel = tempServerData.FirstOrDefault(m => m.Date.Year == tempDate.Year && m.Date.DayOfYear == tempDate.DayOfYear);

                        var displayModel = new TradeStatDataObject();
                        displayModel.Date = tempDate;
                        if (serverModel != null)
                        {
                            displayModel.TradeGain = serverModel.TradeGain;
                            displayModel.TradeMoney = serverModel.TradeMoney;
                            displayModel.TradeTimes = serverModel.TradeTimes;
                        }

                        tempCollection.Add(displayModel);
                    }
                    TradeStatData = tempCollection;

                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        #endregion

        #region 公开属性

        #region AccountStatData

        /// <summary>
        /// The <see cref="AccountStatData" /> property's name.
        /// </summary>
        public const string AccountStatDataPropertyName = "AccountStatData";

        private AccountStatDataObject accountStatData = null;

        /// <summary>
        /// 帐户统计信息
        /// </summary>
        public AccountStatDataObject AccountStatData
        {
            get { return accountStatData; }

            set
            {
                if (accountStatData == value) return;

                RaisePropertyChanging(AccountStatDataPropertyName);
                accountStatData = value;
                RaisePropertyChanged(AccountStatDataPropertyName);
            }
        }

        #endregion

        #region TradeStatData

        /// <summary>
        /// The <see cref="TradeStatData" /> property's name.
        /// </summary>
        public const string TradeStatDataPropertyName = "TradeStatData";

        private Collection<TradeStatDataObject> tradeStatData = null;

        /// <summary>
        /// 收益统计
        /// </summary>
        public Collection<TradeStatDataObject> TradeStatData
        {
            get { return tradeStatData; }

            set
            {
                if (tradeStatData == value) return;

                RaisePropertyChanging(TradeStatDataPropertyName);
                tradeStatData = value;
                RaisePropertyChanged(TradeStatDataPropertyName);
            }
        }

        #endregion

        #region ServiceHotline

        /// <summary>
        /// The <see cref="ServiceHotline" /> property's name.
        /// </summary>
        public const string ServiceHotlinePropertyName = "ServiceHotline";

        private string serviceHotline = null;

        /// <summary>
        /// 服务热线
        /// </summary>
        public string ServiceHotline
        {
            get { return serviceHotline; }

            set
            {
                if (serviceHotline == value) return;

                RaisePropertyChanging(ServiceHotlinePropertyName);
                serviceHotline = value;
                RaisePropertyChanged(ServiceHotlinePropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region SwitchViewCommand

        private RelayCommand<string> switchViewCommand;

        /// <summary>
        /// 切换命令窗口
        /// </summary>
        public RelayCommand<string> SwitchViewCommand
        {
            get
            {
                return switchViewCommand ?? (switchViewCommand = new RelayCommand<string>(ExecuteSwitchViewCommand, CanExecuteSwitchViewCommand));
            }
        }

        private void ExecuteSwitchViewCommand(string code)
        {
            switch (code)
            {
                case "pos":
                    PluginService.Run(Main.ProjectCode, Main.POSManagerCode);
                    break;
                case "merchant":
                    PluginService.Run(Main.ProjectCode, Main.MerchantManagerCode);
                    break;
                case "transactionQuery":
                    PluginService.Run(Main.ProjectCode, Main.TransactionQueryCode);
                    break;
            }
        }

        private bool CanExecuteSwitchViewCommand(string code)
        {
            return true;
        }

        #endregion

        #region OpenIntroduceUriCommand

        private RelayCommand openIntroduceUriCommand;

        /// <summary>
        /// 打开介绍链接
        /// </summary>
        public RelayCommand OpenIntroduceUriCommand
        {
            get
            {
                return openIntroduceUriCommand ?? (openIntroduceUriCommand = new RelayCommand(ExecuteOpenIntroduceUriCommand, CanExecuteOpenIntroduceUriCommand));
            }
        }

        private void ExecuteOpenIntroduceUriCommand()
        {
            UIManager.ShowWeb("TOPS业务介绍", "http://www.51cbc.com/faq/tpos.html");
        }

        private bool CanExecuteOpenIntroduceUriCommand()
        {
            return true;
        }

        #endregion

        #endregion
    }
}
