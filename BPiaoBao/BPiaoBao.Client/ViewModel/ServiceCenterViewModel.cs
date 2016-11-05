using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BPiaoBao.Client.ViewModel
{
    /// <summary>
    /// 客服中心
    /// </summary>
    public class ServiceCenterViewModel : ViewModelBase
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCenterViewModel"/> class.
        /// </summary>
        public ServiceCenterViewModel()
        {
            if (IsInDesignMode)
                return;

            IsBusy = true;
            Action action = () =>
            {
                Customer = CommunicationProxy.GetCustomerService();
                if (Customer == null)
                {
                    UIManager.ShowMessage("获取信息失败");
                    return;
                }

                if (Customer.AdvisoryQQ == null)
                    return;

                Parallel.For(0, Customer.AdvisoryQQ.Count, delegate(int i)
                {
                    var model = new Usher()
                    {
                        Name = Customer.AdvisoryQQ[i].Key,
                        QQ = Customer.AdvisoryQQ[i].Value
                    };

                    var uri = GetImageUri(Customer.AdvisoryQQ[i].Value);
                    model.QQStateImage = uri;

                    DispatcherHelper.UIDispatcher.Invoke(new Action(() => Ushers.Add(model)));
                });
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        //获取实际qq状态图片，请求后腾讯会跳转一个地址
        [MethodImpl(MethodImplOptions.Synchronized)]
        private string GetImageUri(string qq)
        {
            try
            {
                var request = WebRequest.Create(String.Format("http://wpa.qq.com/pa?p=2:{0}:52", qq));
                var response = request.GetResponse();
                request.Abort();
                var result = response.ResponseUri.AbsoluteUri;
                return result;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.WARN, string.Format("获取客服qq[{0}]在线状态图片异常", qq), ex);
                return null;
            }
        }

        #endregion

        #region 公开属性

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy;

        /// <summary>
        /// 是否在忙
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (_isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                _isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        #endregion

        #region Customer

        /// <summary>
        /// The <see cref="Customer" /> property's name.
        /// </summary>
        public const string CustomerPropertyName = "Customer";

        private CustomerDto _customer;

        /// <summary>
        /// 客服信息
        /// </summary>
        public CustomerDto Customer
        {
            get { return _customer; }

            set
            {
                if (_customer == value) return;

                RaisePropertyChanging(CustomerPropertyName);
                _customer = value;
                RaisePropertyChanged(CustomerPropertyName);
            }
        }

        #endregion

        #region Ushers

        /// <summary>
        /// The <see cref="Ushers" /> property's name.
        /// </summary>
        public const string UshersPropertyName = "Ushers";

        private ObservableCollection<Usher> _ushers = new ObservableCollection<Usher>();

        /// <summary>
        /// 客服人员
        /// </summary>
        public ObservableCollection<Usher> Ushers
        {
            get { return _ushers; }

            set
            {
                if (_ushers == value) return;

                RaisePropertyChanging(UshersPropertyName);
                _ushers = value;
                RaisePropertyChanged(UshersPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region OpenQQCommand

        private RelayCommand<string> _openQqCommand;

        /// <summary>
        /// 打开qq命令
        /// </summary>
        public RelayCommand<string> OpenQQCommand
        {
            get
            {
                return _openQqCommand ?? (_openQqCommand = new RelayCommand<string>(ExecuteOpenQQCommand, CanExecuteOpenQQCommand));
            }
        }

        private void ExecuteOpenQQCommand(string qq)
        {
            var url = string.Format("http://wpa.qq.com/msgrd?v=3&uin={0}&site=qq&menu=yes", qq);
            UIManager.OpenDefaultBrower(url);
        }

        private bool CanExecuteOpenQQCommand(string qq)
        {
            return true;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 表示一个客服
    /// </summary>
    public class Usher
    {
        public string Name { get; set; }

        public string QQStateImage { get; set; }

        public string QQ { get; set; }
    }
}
