using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class CountInsuranceViewModel : ViewModelBase
    {

        private ChoosePassengersViewModel _cpvm;
        #region 构造函数
        public CountInsuranceViewModel(ChoosePassengersViewModel cpvm)
        {
            _cpvm = cpvm;  
        }
        #endregion
        #region Count 保险份数

        /// <summary>
        /// The <see cref="Count" /> property's name.
        /// </summary>
        private const string CountPropertyName = "Count";

        private int _count = 1;//默认

        /// <summary>
        /// 保险份数
        /// </summary>
        public int Count
        {
            get { return _count; }

            set
            {
                if (_count == value) return;

                RaisePropertyChanging(CountPropertyName);
                _count = value;
                RaisePropertyChanged(CountPropertyName);
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
        /// 执行
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
            if (_cpvm == null) return;
            if (Count <= 0)
                UIManager.ShowMessage("保险份数必须大于零");
            else
            {
                _cpvm.CountInsurance = Count;
                IsDone = true;
            }
        }

        private bool CanExecuteDoneCommandCommand()
        {
            return !IsBusy;
        }
        #endregion
    }
}
