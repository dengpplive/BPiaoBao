using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.DomesticTicket.View;
using BPiaoBao.Client.DomesticTicket.ViewModel;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows;
using JoveZhao.Framework;

namespace BPiaoBao.Client.DomesticTicket
{
    /// <summary>
    /// 当前模块界面管理
    /// </summary>
    internal class LocalUIManager
    {
        public static bool DefaultShowhiddenColumn = true;//全局变量显示隐列
        private static TicketBookingBackWindow _backwindow;

        /// <summary>
        /// 显示政策页
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="call"></param>
        internal static void ShowPolicy(string orderId, Action<bool?> call)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new PolicyDetailWindow {Owner = Application.Current.MainWindow};
                var vm = new PolicyDetailViewModel();
                vm.LoadOrderInfo(orderId);

                window.DataContext = vm;
                var dialogResult = window.ShowDialog();
                if (call != null)
                    call(dialogResult);

                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示订单详情窗口
        /// </summary>
        /// <param name="orderId">订单对象</param>
        /// <param name="call"></param>
        /// <param name="flag"></param>
        internal static void ShowOrderInfo(string orderId, Action<bool?> call,int flag = 0)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var vm = new OrderInfoViewModel {OrderId = orderId};
                vm.Initialize();
                var window = new OrderInfoWindow(flag) {Owner = Application.Current.MainWindow, DataContext = vm};
                var result = window.ShowDialog();
                if (call != null)
                    call(result); 
                window.DataContext = null;
 
            }));
        }
        internal static void ShowPrintTravel(OrderDetailDto order, PassengerDto passenger, ResponseChangeOrder rasorder = null, ResponseAfterSalePassenger raspassenger = null, int flag = 0,OrderInfoViewModel orderInfoViewModel=null,AfterSaleInfoViewModel afterSaleInfoViewModel=null, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new PrintTravelWindow {Owner = Application.Current.MainWindow};
                var vm = new PrintTravelViewModel
                {
                    RspOrder = order,
                    Passenger = passenger,
                    RsAferSaleOrder = rasorder,
                    RsAfterSalePassenger = raspassenger,
                    RFlag = flag,
                    OrderInfoViewModel = orderInfoViewModel,
                    AfterSaleInfoViewModel = afterSaleInfoViewModel
                };
                vm.Init();
                window.DataContext = vm;
                var result = window.ShowDialog();
                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示发送短信窗口
        /// </summary>
        /// <param name="orderId">订单对象.</param>
        /// <param name="afterorderid">售后订单对象</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void ShowSendMsg(string orderId,int afterorderid = 0)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new SendMsgWindow {Owner = Application.Current.MainWindow};
                var vm = new SendMsgViewModel();
                vm.LoadOrderInfo(orderId,afterorderid);
                window.DataContext = vm;

                window.ShowDialog();
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示政策列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="call"></param>
        internal static void ShowPolicyList(PolicyPack model, Action call)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
               
                var vm = new ChoosePolicyViewModel {OrderId = model.OrderId};
                if (model.OrderId == null)
                    vm.OrderId = model.ChdOrderId;
                else if (model.OrderId != null && model.ChdOrderId != null)
                    vm.OrderId = model.OrderId;
                //var temp = new System.Collections.ObjectModel.ObservableCollection<PolicyDto>();
                //if (model.PolicyList != null)
                //    foreach (var item in model.PolicyList)
                //        temp.Add(item);
                if (model.PolicyList != null)
                    vm.Policys = model.PolicyList;
                var window = new ChoosePolicyWindow {Owner = Application.Current.MainWindow, DataContext = vm};
                window.ShowDialog();
                if (call != null)
                    call();

                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示退改签面板
        /// </summary>
        /// <param name="order"></param>
        /// <param name="call"></param>
        internal static void ShowReissue(ResponseOrder order, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new ReissueWindow {Owner = Application.Current.MainWindow};
                var vm = new ReissueViewModel();
                vm.LoadOrderInfo(order.OrderId);

                window.DataContext = vm;
                var dialogResult = window.ShowDialog();
                if (call != null)
                    call(dialogResult);

                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示售后窗体
        /// </summary>
        /// <param name="orderId"></param>
        internal static void ShowAfterSale(string orderId)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new AfterSaleWindow();
                var vm = new AfterSaleViewModel();

                window.DataContext = vm;
                vm.LoadOrderInfo(orderId);

                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();

                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示售后详情
        /// </summary>
        /// <param name="orderId">The sale order.</param>
        /// <param name="afterId"></param>
        internal static void ShowAfterSaleInfo(string orderId, int afterId)
        {
            var infoWindow = new AfterSaleInfoWindow {Owner = Application.Current.MainWindow};
            var vm = new AfterSaleInfoViewModel(orderId, afterId);
            infoWindow.DataContext = vm;
            vm.Initialize();

            infoWindow.ShowDialog();

            infoWindow.DataContext = null;
        }

        /// <summary>
        /// 显示协调信息
        /// </summary>
        /// <param name="orderId"></param>
        internal static void ShowCoordination(string orderId)
        {
            var window = new CoordinationWindow {Owner = Application.Current.MainWindow};
            var vm = new CoordinationViewModel();
            vm.LoadOrderInfo(orderId);

            window.DataContext = vm;

            vm.Initialize();
            window.ShowDialog();

            window.DataContext = null;
        }

        /// <summary>
        /// 显示改签支付窗体
        /// </summary>
        /// <param name="order"></param>
        /// <param name="call"></param>
        internal static void ShowAfterSalePay(ResponseAfterSaleOrder order, Action<bool?> call)
        {
            var window = new AfterSalePayWindow {Owner = Application.Current.MainWindow};

            var vm = new AfterSalePayViewModel {SourceOrder = order};
            vm.Initialize();

            window.DataContext = vm;
            var result = window.ShowDialog();
            if (call != null)
                call(result);

            window.DataContext = null;
        }

        /// <summary>
        /// 显示机票预订窗口
        /// </summary>
        /// <param name="flights"></param>
        /// <param name="call"></param>
        /// <param name="closeWindowMsg"></param>
        internal static void ShowTicketBooking(FlightInfoModel[] flights,Site site , Action<bool?> call, string closeWindowMsg = null)
        {
            var window = new TicketBookingWindow {Owner = Application.Current.MainWindow};

            var vm = new TicketBookingViewModel(flights,site);
            window.DataContext = vm;
            var result = window.ShowDialog();
            if (call != null)
                call(result);

            window.DataContext = null;
            if (_backwindow != null)
            {
                _backwindow.Close();
            }
        }

        /// <summary>
        /// 显示机票预订往返/联程窗口
        /// </summary>
        /// <param name="flightInfoModel"></param>
        /// <param name="flightInfoModels"></param>
        /// <param name="isShowCommissionColumn"></param>
        /// <param name="call"></param>
        internal static void ShowTicketBookingBack(FlightInfoModel flightInfoModel, FlightInfoModel[] flightInfoModels,Visibility isShowCommissionColumn, Action<bool?> call)
        {
           

            var vm = new TicketBookingBackViewModel
            {
                FlightInfoModel = flightInfoModel,
                FlightInfoModels = flightInfoModels,
                IsShowCommissionColumn = isShowCommissionColumn
            };
            _backwindow = new TicketBookingBackWindow(vm) {Owner = Application.Current.MainWindow};
            var result = _backwindow.ShowDialog();
            if (call != null)
                call(result);

            _backwindow.DataContext = null;
            _backwindow = null;

        }


        /// <summary>
        /// 启动默认浏览器
        /// </summary>
        /// <param name="url"></param>
        internal static void OpenDefaultBrowser(string url)
        {
            UIManager.OpenDefaultBrower(url);
        }


        /// <summary>
        /// 显示常用乘客信息窗体
        /// </summary>
        /// <param name="tvm"></param>
        /// <param name="call"></param>
        internal static void ShowUsualPassengers(TicketBookingViewModel tvm,Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new UsualPassengersWindow();
                var vm = new UsualPassengersViewModel(tvm);
                window.Owner = Application.Current.MainWindow;
                window.DataContext = vm;
                var result = window.ShowDialog();

                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示常用乘客信息编辑窗体
        /// </summary>
        /// <param name="fpd"></param>
        /// <param name="call"></param>
        internal static void ShowUsualPassengerInfo(Action<bool?> call = null,FrePasserDto fpd = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new UsualPassengerInfoWindow();
                var vm = new UsualPassengerInfoViewModel(fpd);
                window.Owner = Application.Current.MainWindow;
                window.DataContext = vm;
                var result = window.ShowDialog();

                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示关联成人订单号
        /// </summary>
        /// <param name="tvm"></param>
        internal static void ShowRelationOrderNoWindow(TicketBookingViewModel tvm)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new RelationOrderNoWindow();
                var vm = new RelationOrderNoViewModel(tvm);
                window.Owner = Application.Current.MainWindow;
                window.DataContext = vm;
                window.ShowDialog();
                //if (call != null)
                //    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示选择购买航意险乘客信息窗体
        /// </summary>
        /// <param name="tvm"></param>
        /// <param name="call"></param>
        internal static void ShowPassengers(PolicyDetailViewModel tvm, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new ChoosePassengersWindow(tvm.Flag);
                var vm = new ChoosePassengersViewModel(tvm);
                window.Owner = Application.Current.MainWindow;
                window.DataContext = vm;
                var result = window.ShowDialog();

                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 选择保险份数窗体
        /// </summary>
        /// <param name="cpvm"></param>
        /// <param name="call"></param>
        internal static void ChooseInsuranceCount(ChoosePassengersViewModel cpvm, Action<bool?> call = null)
        {
            if (cpvm == null) return;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new CountInsuranceWindow();
                var vm = new CountInsuranceViewModel(cpvm);
                window.Owner = Application.Current.MainWindow;
                window.DataContext = vm;
                var result = window.ShowDialog();

                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 支付订单、保险、极速退窗体
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="call"></param>
        internal static void ShowPayInsuranceAndRefund(string orderId, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new PayInsuranceAndRefundWindow();
                var vm = new PolicyDetailViewModel();
                window.Owner = Application.Current.MainWindow;
                vm.LoadOrderInfo(orderId);
                window.DataContext = vm;
                var result = window.ShowDialog();

                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 二次支付保险窗体
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="call"></param>
        internal static void ShowSecondPayInsurance(string orderId, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new PayInsuranceSecondWindow();
                var vm = new PolicyDetailViewModel();
                window.Owner = Application.Current.MainWindow;
                vm.LoadOrderInfo(orderId);
                window.DataContext = vm;
                var result = window.ShowDialog();

                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 打开投保窗口
        /// </summary>
        /// <param name="call"></param>
        internal static void ShowBuySingleInsuranceWindow(Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var win = new BuySingleInsuranceWindow {Owner = Application.Current.MainWindow};
                var result = win.ShowDialog();
                if (call != null)
                {
                    call(result);
                }
            }));
        }

        /// <summary>
        /// 打开保险详情窗口
        /// </summary>
        /// <param name="model">保单实体</param>
        /// <param name="call"></param>
        internal static void ShowInsuranceDetailWindow(ResponseInsurance model, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var win = new InsuranceDetailWindow {Owner = Application.Current.MainWindow};
                var vm = (InsuranceDetailViewModel) win.DataContext;
                vm.Init(model);
                var result = win.ShowDialog();
                if (call != null)
                {
                    call(result);
                }
            }));
        }
        /// <summary>
        /// 保险管理购买保险窗体
        /// </summary>
        /// <param name="call"></param>
        internal static void ShowBuyInsurance(Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new BuyInsuranceWindow();
                var vm = new BuyInsuranceViewModel();
                window.Owner = Application.Current.MainWindow;
                window.DataContext = vm;
                var result = window.ShowDialog();
                if (call != null)
                    call(result);
                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 打开确认投保窗体
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="call"></param>
        internal static void ShowConfirmPayInsurance(PolicyDetailViewModel vm, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var win = new PayInsuranceConfirmWindow {Owner = Application.Current.MainWindow, DataContext = vm};
                vm.IsDone = false;
                var result = win.ShowDialog();
                if (call != null)
                    call(result);
                win.DataContext = null;
            }));
        }

        /// <summary>
        /// 显示关联成人订单列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="call"></param>
        internal static string ShowAduldtsOrderIdList(Action call)
        {
            var oid = string.Empty;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var vm = new ChooseOrderIdViewModel();
                var window = new ChooseOrderIdWindow { Owner = Application.Current.MainWindow, DataContext = vm };
                window.ShowDialog();
               oid= vm.OrderId;
                if (call != null)
                    call();
                window.DataContext = null;
            }));
            return oid;
        }
        
    }
}
