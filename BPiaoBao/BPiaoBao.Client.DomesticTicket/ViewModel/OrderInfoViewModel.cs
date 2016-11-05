using System.Globalization;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 订单详情视图模型
    /// </summary>
    public class OrderInfoViewModel : BaseVM
    {



        #region 初始化数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    OrderDetail = service.GetClientOrderDetail(OrderId);  
                }, UIManager.ShowErr);
                //获取保险相关
                CommunicateManager.Invoke<IInsuranceService>(service =>
                {
                    var re = service.GetCurentInsuranceCfgInfo(true);
                    IsOpenCurrenCarrierInsurance = re.IsOpenCurrenCarrierInsurance && re.IsOpenUnexpectedInsurance;
                }, UIManager.ShowErr); 
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region 公开属性
        /// <summary>
        /// 是否显示极速退服务费列
        /// </summary>
        //public int IsShowRefundColum = 0;

        #region IsShowOldOrderID

        /// <summary>
        /// The <see cref="IsShowOldOrderId" /> property's name.
        /// </summary>
        private const string IsShowOldOrderIdPropertyName = "IsShowOldOrderID";

        private bool _isShowOldOrderId;

        /// <summary>
        /// 是否显示成人订单号
        /// </summary>
        public bool IsShowOldOrderId
        {
            get { return _isShowOldOrderId; }

            set
            {
                if (_isShowOldOrderId == value) return;

                RaisePropertyChanging(IsShowOldOrderIdPropertyName);
                _isShowOldOrderId = value;
                RaisePropertyChanged(IsShowOldOrderIdPropertyName);
            }
        }

        #endregion

        #region OrderId
        public string OrderId { get; set; }
        #endregion

        #region OrderDetail

        /// <summary>
        /// The <see cref="OrderDetail" /> property's name.
        /// </summary>
        private const string OrderDetailPropertyName = "OrderDetail";

        private OrderDetailDto _orderDetail;

        /// <summary>
        /// 详情对象
        /// </summary>
        /// <value>
        /// The order detail.
        /// </value>
        public OrderDetailDto OrderDetail
        {
            get { return _orderDetail; }

            set
            {
                if (_orderDetail == value) return;

                RaisePropertyChanging(OrderDetailPropertyName);
                _orderDetail = value;
                if(!string.IsNullOrEmpty(OrderDetail.Policy.Remark)) OrderDetail.Policy.Remark = OrderDetail.Policy.Remark.Replace("\r", "").Replace("\n", ""); //字符串处理
                IsShowOldOrderId = _orderDetail.OrderType == "1" || _orderDetail.OrderType == "2";//关联成人订单号隐藏(儿童和线下婴儿订单时显示)    
                RaisePropertyChanged(OrderDetailPropertyName);
            }
        }

        #endregion

        #region IsOpenCurrenCarrierInsurance

        /// <summary>
        /// The <see cref="IsOpenCurrenCarrierInsurance" /> property's name.
        /// </summary>
        private const string IsOpenCurrenCarrierInsurancePropertyName = "IsOpenCurrenCarrierInsurance";

        private bool _isOpenCurrenCarrierInsurance;

        /// <summary>
        /// 是否显示保险相关
        /// </summary>
        public bool IsOpenCurrenCarrierInsurance
        {
            get { return _isOpenCurrenCarrierInsurance; }

            set
            {
                if (_isOpenCurrenCarrierInsurance == value) return;

                RaisePropertyChanging(IsOpenCurrenCarrierInsurancePropertyName);
                _isOpenCurrenCarrierInsurance = value;
                //购买保险按钮隐藏
                switch (OrderDetail.OrderStatus)
                {
                    //case 1: //创建平台订单失败
                    //case 5: //订单已取消
                    //case 6: //已经出票，订单完成
                    //case 7: //拒绝出票，等待退款
                    //case 8: //拒绝出票，等待平台退款  
                    //case 9: //拒绝出票，订单完成
                    //case 10://订单已失效
                    //case 11://自动复合票号失败
                    //case 12://拒绝出票,退款中
                    case 5: // 已经出票，订单完成
                        if ( OrderDetail.PayInfo.PayStatus == Common.Enums.EnumPayStatus.NoPay || OrderDetail.Passengers.Count(p => p.IsBuyInsurance) == OrderDetail.Passengers.Count)//购买保险乘机人数和订单状态是否已经支付判断
                            IsOpenCurrenCarrierInsurance = false;
                        var dto = OrderDetail.SkyWays.FirstOrDefault();
                        if (dto != null && DateTime.Now > dto.StartDateTime.AddHours(-2))//起飞前2小时才能二次购买保险
                            IsOpenCurrenCarrierInsurance = false;
                        break;
                    //case 1: // 生成订单等待选择政策
                    //case 2: // 新订单，等待支付
                    //case 3: // 支付成功 等待出票
                    //case 4: // 订单取消
                    //case 6: // 拒绝出票，等待退款
                    //case 7: // 拒绝出票，等待退款(退款)
                    //case 8: // 平台拒绝出票，等待退款(平台退款)
                    //case 9: // 拒绝出票,退款中
                    default:
                        IsOpenCurrenCarrierInsurance = false;
                        break;
                }
                RaisePropertyChanged(IsOpenCurrenCarrierInsurancePropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region OpenOrderInfoCommand

        private RelayCommand<string> _openTravelCommand;

        /// <summary>
        /// 打开订单详情页面命令
        /// </summary>
        public RelayCommand<string> OpenTravelCommand
        {
            get
            {
                return _openTravelCommand ?? (_openTravelCommand = new RelayCommand<string>(ExecuteOpenTravelCommand));
            }
        }

        private void ExecuteOpenTravelCommand(string id)
        {
            var passenger = OrderDetail.Passengers.FirstOrDefault(p => p.Id.ToString(CultureInfo.InvariantCulture) == id);

            //LocalUIManager.ShowPrintTravel(OrderDetail,  passenger);
            //LocalUIManager.ShowPrintTravel(OrderDetail,  passenger,null,null,0,this,null,dialogResult =>
            //{
            //    if (dialogResult == null || !dialogResult.Value) return;
            //    Initialize();
            //});
            LocalUIManager.ShowPrintTravel(OrderDetail, passenger, null, null, 0, this, null, dialogResult => Initialize());
        }

        #endregion

        #region OpenPassengersCommand 购买航意险弹出窗口

        private RelayCommand _openPassengersCommand;

        /// <summary>
        /// 打开选择乘客信息窗口命令
        /// </summary>
        public RelayCommand OpenPassengersCommand
        {
            get
            {
                return _openPassengersCommand ?? (_openPassengersCommand = new RelayCommand(ExecuteOpenPassengersCommand));
            }
        }

        private void ExecuteOpenPassengersCommand()
        {
            if (String.IsNullOrEmpty(OrderId)) return;
            if (OrderDetail == null) return;
            //LocalUIManager.ShowPolicy(OrderId, null);
            //LocalUIManager.ShowPolicy(OrderId, (dialogResult) =>
            //{
            //    if (dialogResult != null && dialogResult.Value)
            //        Initialize();
            //});
            LocalUIManager.ShowSecondPayInsurance(OrderId, dialogResult =>
            {
                if (dialogResult != null && dialogResult.Value)
                    Initialize();
            });
        }

        #endregion

        #endregion
    }
}
