using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 理财产品详情
    /// </summary>
    public class FinanceInfoWindowViewModel : BaseVM
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsBusy = true;

            var action = new Action(() => CommunicateManager.Invoke<IFinancialService>(service =>
            {
                FinancialProduct = service.GetSingleProductInfo(Id);
            }, ex =>
            {
                UIManager.ShowErr(ex);
                Logger.WriteLog(LogType.WARN, "获取理财信息失败", ex);
            }));

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    IsBusy = false;
                }));
            });
        }

        #region 公开属性

        #region Id

        /// <summary>
        /// The <see cref="Id" /> property's name.
        /// </summary>
        private const string IdPropertyName = "Id";

        private string _id;

        /// <summary>
        /// 产品id
        /// </summary>
        public string Id
        {
            get { return _id; }

            set
            {
                if (_id == value) return;

                RaisePropertyChanging(IdPropertyName);
                _id = value;
                RaisePropertyChanged(IdPropertyName);
            }
        }

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否在繁忙
        ///// </summary>
        //public bool IsBusy
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

        #region FinancialProduct

        /// <summary>
        /// The <see cref="FinancialProduct" /> property's name.
        /// </summary>
        private const string FinancialProductPropertyName = "FinancialProduct";

        private FinancialProductDto _financialProduct;

        /// <summary>
        /// 产品信息
        /// </summary>
        public FinancialProductDto FinancialProduct
        {
            get { return _financialProduct; }

            set
            {
                if (_financialProduct == value) return;

                RaisePropertyChanging(FinancialProductPropertyName);
                _financialProduct = value;
                RaisePropertyChanged(FinancialProductPropertyName);
            }
        }

        #endregion

        #endregion
    }
}
