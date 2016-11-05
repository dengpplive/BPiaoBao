using System.Windows;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Properties;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Utils;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 选择政策视图模型
    /// </summary>
    public class ChoosePolicyViewModel : BaseVM
    {

        #region 成员变量

        public const string ChoosePolicyToTicketBookingQueryViewMessage_Commission = "ChoosePolicyToTicketBookingQueryViewMessage_Commission";

        public const string ChoosePolicyToPnrViewMessage_Commission = "ChoosePolicyToPnrViewMessage_Commission";

        #endregion

        #region 公开属性

        public string OrderId { get; set; }

        #region Policys

        /// <summary>
        /// The <see cref="Policys" /> property's name.
        /// </summary>
        public const string PolicysPropertyName = "Policys";

        private List<PolicyDto> policys = null;
        /// <summary>
        /// PNR列表
        /// </summary>
        public List<PolicyDto> Policys
        {
            get { return policys; }

            set
            {
                if (policys == value) return;

                RaisePropertyChanging(PolicysPropertyName);
                //policys = value;
                //policys = value.OrderByDescending(t => t.PolicySpecialType != EnumPolicySpecialType.Normal).ToList();
                policys = value.OrderBy(t => t.PayMoney).ToList();
                RaisePropertyChanged(PolicysPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //protected bool isBusy = false;

        ///// <summary>
        ///// 是否正在忙
        ///// </summary>
        //public virtual bool IsBusy
        //{
        //    get { return isBusy; }

        //    set
        //    {
        //        if (isBusy == value) return;

        //        RaisePropertyChanging(IsBusyPropertyName);
        //        isBusy = value;
        //        RaisePropertyChanged(IsBusyPropertyName);
        //    }
        //}

        #endregion

        #region IsShowCommissionColumn
        public const string IsShowCommissionColumnProtertyName = "IsShowCommissionColumn";

        private Visibility _isShowCommissionColumn = Visibility.Visible;

        /// <summary>
        /// 是否显示佣金列
        /// </summary>
        public Visibility IsShowCommissionColumn
        {
            get { return this._isShowCommissionColumn; }
            set
            {
                if (this._isShowCommissionColumn == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowCommissionColumnProtertyName);
                this._isShowCommissionColumn = value;
                RaisePropertyChanged(IsShowCommissionColumnProtertyName);


            }
        }
        #endregion


        #endregion

        #region 公开命令

        #region ChoosePolicyCommand

        private RelayCommand<PolicyDto> choosePolicyCommand;

        /// <summary>
        /// 打开订单详情命令
        /// </summary>
        public RelayCommand<PolicyDto> ChoosePolicyCommand
        {
            get
            {
                return choosePolicyCommand ?? (choosePolicyCommand = new RelayCommand<PolicyDto>(ExecuteChoosePolicyCommand, CanExecuteChoosePolicyCommand));
            }
        }

        private bool CanExecuteChoosePolicyCommand(PolicyDto arg)
        {
            return !IsBusy;
        }

        private void ExecuteChoosePolicyCommand(PolicyDto policy)
        {
            Func<OrderDto> func = () =>
            {
                IsBusy = true;
                OrderDto result = new OrderDto();

                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    result = service.ChoosePolicy(policy, OrderId);
                    if (result.OrderSource == EnumOrderSource.PnrContentImport && !result.IsAuthSuc) UIManager.ShowMessage(result.AuthInfo);
                }, UIManager.ShowErr);

                return result;
            };

            Task.Factory.StartNew<OrderDto>(func).ContinueWith((task) =>
            {
                IsBusy = false;

                var model = task.Result as OrderDto;
                if (String.IsNullOrEmpty(model.OrderId))
                    return;
                //LocalUIManager.ShowPolicy(model.OrderId, null);
                LocalUIManager.ShowPayInsuranceAndRefund(model.OrderId, p => Module.PluginService.Run(Main.ProjectCode, Main.OrderQueryCode));
            });
        }

        #endregion

        #endregion

    }
}
