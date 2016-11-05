using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.Client.TPOS.View;
using BPiaoBao.Client.TPOS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BPiaoBao.Client.TPOS
{
    /// <summary>
    /// 本地界面管理类
    /// </summary>
    public class LocalUIManager
    {

        /// <summary>
        /// 添加商户
        /// </summary>
        /// <param name="call">The action.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void AddMerchant(Action<bool?> call)
        {
            MerchantAddWindow window = new MerchantAddWindow();
            window.Owner = Application.Current.MainWindow;
            var result = window.ShowDialog();

            if (call != null)
                call(result);
        }

        /// <summary>
        /// 修改商户
        /// </summary>
        /// <param name="call">The action.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void EditMerchant(ResponseBusinessmanInfo merchant, Action<bool?> call)
        {
            MerchantEditWindow window = new MerchantEditWindow();
            window.Owner = Application.Current.MainWindow;
            MerchantAddEditViewModel vm = new MerchantAddEditViewModel();
            vm.LoadInfo(merchant.Id);
            window.DataContext = vm;
            var result = window.ShowDialog();

            if (call != null)
                call(result);

            window.DataContext = null;
        }

        internal static void ShowMerchantInfo(ResponseBusinessmanInfo merchant)
        {
            MerchantInfoWindow window = new MerchantInfoWindow();
            window.Owner = Application.Current.MainWindow;
            MerchantAddEditViewModel vm = new MerchantAddEditViewModel();
            vm.LoadInfo(merchant.Id);
            window.DataContext = vm;
            var result = window.ShowDialog();

            window.DataContext = null;
        }

        /// <summary>
        /// 给指定商户分配POS机
        /// </summary>
        /// <param name="merchant">The merchant.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void AssignPos(ResponseBusinessmanInfo merchant, Action<bool?> call)
        {
            AssignPosWindow window = new AssignPosWindow();
            window.Owner = Application.Current.MainWindow;
            AssignPosViewModel vm = new AssignPosViewModel();
            window.DataContext = vm;

            vm.Merchant = merchant;
            vm.Initialize();

            var result = window.ShowDialog();
            if (call != null)
                call(result);

            window.DataContext = null;
        }
        /// <summary>
        /// POS分配日志
        /// </summary>
        /// <param name="posNo">POS编号</param>
        internal static void DistributionLog(string posNo)
        {
            POSLogWindow window = new POSLogWindow();
            POSLogViewModel vm= new POSLogViewModel(posNo);
            window.Owner = Application.Current.MainWindow;
            window.DataContext = vm;
            vm.Initialize();
            window.ShowDialog();

            window.DataContext = null;
        }
    }
}
