using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class RelationOrderNoViewModel : ViewModelBase
    {
        /// <summary>
        /// 关联成人订单号vm
        /// </summary>
        private TicketBookingViewModel _vm;
        #region 构造函数
        public RelationOrderNoViewModel(TicketBookingViewModel ticketBookingVm)
        {
            _vm = ticketBookingVm;
            OrderId = _vm.OldOrderId;
        }
        #endregion
        #region OrderID 关联成人订单号

        /// <summary>
        /// The <see cref="OrderId" /> property's name.
        /// </summary>
        private const string OrderIdPropertyName = "OrderID";

        private string _orderId;

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId
        {
            get { return _orderId; }

            set
            {
                if (_orderId == value) return;

                RaisePropertyChanging(OrderIdPropertyName);
                _orderId = value;
                RaisePropertyChanged(OrderIdPropertyName);
            }
        }

        #endregion
        #region IsDone

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

        /// <summary>
        /// 是否操作完成
        /// </summary>
        public bool IsDone
        {
            get { return _isDone; }

            set
            {
                if (_isDone == value) return;

                RaisePropertyChanging(IsDonePropertyName);
                _isDone = value;
                RaisePropertyChanged(IsDonePropertyName);
            }
        }

        #endregion
        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        private const string IsBusyPropertyName = "IsBusy";

        private bool _isBusy;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public virtual bool IsBusy
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
        #region DoneCommand

        private RelayCommand _doneCommand;

        /// <summary>
        /// 执行关联成人订单操作
        /// </summary>
        public RelayCommand DoneCommand
        {
            get
            {
                return _doneCommand ?? (_doneCommand = new RelayCommand(ExecuteDoneCommandCommand, CanExecuteDoneCommandCommand));
            }
        }

        private void ExecuteDoneCommandCommand()
        {
            if (_vm == null || string.IsNullOrWhiteSpace(OrderId))
            {
                UIManager.ShowMessage("关联成人订单号为空");
                return;
            }
            _vm.OldOrderId = OrderId;
            IsDone = true;
        }
        private bool CanExecuteDoneCommandCommand()
        {
            return !IsBusy;
        }
        #endregion

    }
}
