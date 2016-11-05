using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BPiaoBao.Client.UIExt
{
    public class BaseVM : ViewModelBase
    {
        #region InitializeCommand

        private RelayCommand initializeCommand;

        /// <summary>
        /// 初始化数据命令
        /// </summary>
        public RelayCommand InitializeCommand
        {
            get
            {
                return initializeCommand ?? (initializeCommand = new RelayCommand(Initialize, CanInitialize));
            }
        }

        /// <summary>
        /// 能否初始化
        /// </summary>
        /// <returns></returns>
        public virtual bool CanInitialize()
        {
            return !IsBusy;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public virtual void Initialize()
        {

        }

        #endregion

        #region PageLoadCommand

        private RelayCommand pageLoadCommand;

        /// <summary>
        /// 页面呈现后触发的命令
        /// </summary>
        public RelayCommand PageLoadCommand
        {
            get
            {
                return pageLoadCommand ?? (pageLoadCommand = new RelayCommand(ExecutePageLoadCommand));
            }
        }

        /// <summary>
        /// 页面呈现后触发
        /// </summary>
        protected virtual void ExecutePageLoadCommand()
        {

        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        public const string IsBusyPropertyName = "IsBusy";

        protected bool isBusy = false;

        /// <summary>
        /// 是否在繁忙
        /// </summary>
        public virtual bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);
            }
        }

        #endregion


        #region FullWidowExt

        private FullWidowExt _fullWidowExt;

        public FullWidowExt FullWidowExt
        {
            get { return _fullWidowExt; }
            set { _fullWidowExt = value; }
        }
        #endregion

        /// <summary>
        /// 右上角提示数目
        /// </summary>
        public int TipCount
        {
            get { return _tipCount; }
            set
            {
                RaisePropertyChanging("TipCount");
                _tipCount = value;
                RaisePropertyChanged("TipCount");
            }

        }
        private int _tipCount = 0;
    }
}
