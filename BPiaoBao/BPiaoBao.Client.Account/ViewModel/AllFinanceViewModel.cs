using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 所有理财产品视图模型
    /// </summary>
    public class AllFinanceViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AllFinanceViewModel"/> class.
        /// </summary>
        public AllFinanceViewModel()
        {
            if (IsInDesignMode)
                return;

            Initialize();
        }

        #endregion

        #region 公开属性

        #region AllProducts

        ///// <summary>
        ///// The <see cref="AllProducts" /> property's name.
        ///// </summary>
        //public const string AllProductsPropertyName = "AllProducts";

        //private ObservableCollection<FinancialProductDto> allProducts = new ObservableCollection<FinancialProductDto>();

        ///// <summary>
        ///// 所有理财产品
        ///// </summary>
        //public ObservableCollection<FinancialProductDto> AllProducts
        //{
        //    get { return allProducts; }

        //    set
        //    {
        //        if (allProducts == value) return;

        //        RaisePropertyChanging(AllProductsPropertyName);
        //        allProducts = value;
        //        RaisePropertyChanged(AllProductsPropertyName);
        //    }
        //}

        #endregion

        #region SoldOutFinancialProducts

        /// <summary>
        /// The <see cref="SoldOutFinancialProducts" /> property's name.
        /// </summary>
        private const string SoldOutFinancialProductsPropertyName = "SoldOutFinancialProducts";

        private IEnumerable<FinancialProductDto> _soldOutFinancialProducts;

        /// <summary>
        /// 下架的理财产品
        /// </summary>
        public IEnumerable<FinancialProductDto> SoldOutFinancialProducts
        {
            get { return _soldOutFinancialProducts; }

            set
            {
                if (_soldOutFinancialProducts == value) return;

                RaisePropertyChanging(SoldOutFinancialProductsPropertyName);
                _soldOutFinancialProducts = value;
                RaisePropertyChanged(SoldOutFinancialProductsPropertyName);
            }
        }

        #endregion

        #region OnSaleFinancialProducts

        /// <summary>
        /// The <see cref="OnSaleFinancialProducts" /> property's name.
        /// </summary>
        private const string OnSaleFinancialProductsPropertyName = "OnSaleFinancialProducts";

        private IEnumerable<FinancialProductDto> _onSaleFinancialProducts;

        /// <summary>
        /// 出售中的理财产品
        /// </summary>
        public IEnumerable<FinancialProductDto> OnSaleFinancialProducts
        {
            get { return _onSaleFinancialProducts; }

            set
            {
                if (_onSaleFinancialProducts == value) return;

                RaisePropertyChanging(OnSaleFinancialProductsPropertyName);
                _onSaleFinancialProducts = value;
                RaisePropertyChanged(OnSaleFinancialProductsPropertyName);
            }
        }

        #endregion

        #endregion

        private bool _isWait;
        public bool IsWait
        {
            get { return _isWait; }
            set
            {
                if (_isWait == value) return;
                _isWait = value;
                RaisePropertyChanged("IsWait");
            }
        }

        public ICommand BuyCommand
        {
            get
            {
                return new RelayCommand<string>(p =>
                {
                    var model = OnSaleFinancialProducts.SingleOrDefault(x => x.ProductId == p);
                    if (model == null)
                    {
                        UIManager.ShowMessage("获取产品信息失败，请稍后再试!");
                        return;
                    }
                    LocalUIManager.OpenProductBuyWindow(new ProductBuyViewModel(model));
                });
            }
        }

        #region OpenProductsInfoCommand

        private RelayCommand<FinancialProductDto> _openProductsInfoCommand;

        /// <summary>
        /// 理财产品详情信息
        /// </summary>
        public RelayCommand<FinancialProductDto> OpenProductsInfoCommand
        {
            get
            {
                return _openProductsInfoCommand ?? (_openProductsInfoCommand = new RelayCommand<FinancialProductDto>(ExecuteOpenProductsInfoCommand, CanExecuteOpenProductsInfoCommand));
            }
        }

        private void ExecuteOpenProductsInfoCommand(FinancialProductDto info)
        {
            LocalUIManager.ShowFinancialProductInfo(info.ProductId);
        }

        private bool CanExecuteOpenProductsInfoCommand(FinancialProductDto info)
        {
            return info != null;
        }

        #endregion

        #region QueryCommand

        private RelayCommand _queryCommand;

        /// <summary>
        /// Gets the QueryCommand.
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return _queryCommand ?? (_queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        private void ExecuteQueryCommand()
        {
            IsWait = true;
            OnSaleFinancialProducts = null;
            SoldOutFinancialProducts = null;

            Action action = () => CommunicateManager.Invoke<IFinancialService>(p =>
            {
                var result = p.GetShelfProducts("5");
                var enumerable = result as IList<FinancialProductDto> ?? result.ToList();
                if (result != null)
                {
                    foreach (var item in enumerable)
                    {
                        item.MaxAmount = 0;
                        item.CurrentAmount = 0;
                    }
                }
                SoldOutFinancialProducts = enumerable;

                var temp = p.GetActiveProduct(null);
                OnSaleFinancialProducts = temp;

                //DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                //{
                //    OnSaleFinancialProducts = temp;
                //}));


            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsWait = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteQueryCommand()
        {
            return !_isWait;
        }

        #endregion
    }
}
