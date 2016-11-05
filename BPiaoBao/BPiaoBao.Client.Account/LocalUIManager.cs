using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.Account.View;
using BPiaoBao.Client.Account.ViewModel;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Windows;

namespace BPiaoBao.Client.Account
{
    /// <summary>
    /// 当前程序集界面管理
    /// </summary>
    internal class LocalUIManager
    {
        /// <summary>
        /// 新增银行卡
        /// </summary>
        internal static void AddBank(Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var bankWindow = new AddBankCardWindow { Owner = Application.Current.MainWindow };

                var result = bankWindow.ShowDialog();

                if (call != null)
                    call(result);
            }));
        }

        /// <summary>
        /// 修改银行卡
        /// </summary>
        internal static void ModifyBank(BankCardDto dto, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var bankWindow = new AddBankCardWindow { Owner = Application.Current.MainWindow };
                var vm = (AddBankCardViewModel)bankWindow.DataContext;
                vm.InitModify(dto);
                bankWindow.Title = "修改银行卡";
                var result = bankWindow.ShowDialog();
                if (call != null)
                    call(result);
            }));
        }

        /// <summary>
        /// 打开转出界面
        /// </summary>
        internal static void OpenRollOutWindow(ViewModelBase viewModel)
        {
            var confirmRollOutWindow = new ConfirmRollOutWindow
            {
                DataContext = viewModel,
                Owner = Application.Current.MainWindow
            };
            confirmRollOutWindow.ShowDialog();

            confirmRollOutWindow.DataContext = null;
        }

        /// <summary>
        /// 打开产品详情页
        /// </summary>
        /// <param name="viewModel"></param>
        internal static void OpenProductBuyWindow(ViewModelBase viewModel)
        {
            var buyWindow = new ProductBuyWindow { DataContext = viewModel, Owner = Application.Current.MainWindow };
            buyWindow.ShowDialog();

            buyWindow.DataContext = null;
        }

        /// <summary>
        /// 启动浏览器
        /// </summary>
        /// <param name="url">显示URL</param>
        internal static void OpenBrowser(string url)
        {
            UIManager.OpenDefaultBrower(url);
            var result = MessageBoxExt.ShowPay();
            if (result.HasValue && result.Value)
                Messenger.Default.Send(true, "CloseProductBuy");
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
        /// 显示理财产品详情
        /// </summary>
        /// <param name="productId"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void ShowFinancialProductInfo(string productId)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var window = new FinanceInfoWindow { Owner = Application.Current.MainWindow };
                var vm = new FinanceInfoWindowViewModel { Id = productId };
                vm.Initialize();

                window.DataContext = vm;
                window.Show();

                window.DataContext = null;
            }));
        }

        /// <summary>
        /// 打开积分兑换窗体
        /// </summary>
        /// <param name="call"></param>
        public static void ShowPointsExchange(Action<bool> call)
        {
            var window = new PointsExchangeWindow { Owner = Application.Current.MainWindow };
            window.ShowDialog();

            if (call == null) return;
            var vm = window.DataContext as PointsExchangeViewModel;
            call(vm != null && vm.IsExchangeSuccess);
        }

        /// <summary>
        /// 显示充值窗体
        /// </summary>
        /// <param name="call"></param>
        internal static void ShowRecharge(Action call)
        {
            var window = new RechargeWindow { Owner = Application.Current.MainWindow };
            window.ShowDialog();

            if (call != null)
            {
                call();
            }
        }

        /// <summary>
        /// 显示消费记录窗口
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void ShowBillDetail()
        {
            var window = new BillDetailWindow { Owner = Application.Current.MainWindow };
            window.ShowDialog();
        }

        /// <summary>
        /// 显示还款记录窗口
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void ShowBillRePayDetail()
        {
            var window = new BillRePayDetailWindow { Owner = Application.Current.MainWindow };
            window.ShowDialog();
        }

        /// <summary>
        /// 临时额度申请
        /// </summary>
        internal static void ShowApplyTemporaryLimit(string day, string number, Action<bool?> call = null)
        {
            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
            {
                var vm = new TempCreditApplyViewModel(day, number);
                var window = new TempCreditApplyWindow { Owner = Application.Current.MainWindow, DataContext = vm };
                var result = window.ShowDialog();
                if (call != null && vm.IsDone)
                    call(result);
            }));
        }
    }
}
