using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 选择常旅客视图模型
    /// </summary>
    public class UsualPassengersViewModel:BaseVM
    {

        private List<FrePasserDto> _list = new List<FrePasserDto>();
        private TicketBookingViewModel _ticketBookingViewModel;
        #region 构造函数
        public UsualPassengersViewModel(TicketBookingViewModel ticketBookingVm)
        {
            Initialize();
            _ticketBookingViewModel = ticketBookingVm;    
        } 
        #endregion

        #region 初始化数据

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性
        #region QueryFrePasser

        /// <summary>
        /// The <see cref="QueryFrePasser" /> property's name.
        /// </summary>
        private const string QueryFrePasserPropertyName = "QueryFrePasser";

        private QueryFrePasser _queryFrePasser = new QueryFrePasser();

        /// <summary>
        /// 查询实体
        /// </summary>
        public QueryFrePasser QueryFrePasser
        {
            get { return _queryFrePasser; }

            set
            {
                if (_queryFrePasser == value) return;

                RaisePropertyChanging(QueryFrePasserPropertyName);
                _queryFrePasser = value;
                RaisePropertyChanged(QueryFrePasserPropertyName);
            }
        }

        #endregion 
        #region IsSelected

        /// <summary>
        /// The <see cref="IsSelected" /> property's name.
        /// </summary>
        private const string IsSelectedPropertyName = "IsSelected";

        private bool _isSelected;

        /// <summary>
        /// 是否选择完成
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (_isSelected == value) return;

                RaisePropertyChanging(IsSelectedPropertyName);
                _isSelected = value;
                RaisePropertyChanged(IsSelectedPropertyName);
            }
        }

        #endregion
        #region Passengers

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string PassengersPropertyName = "Passengers";

        private List<FrePasserDto> _passengers;

        /// <summary>
        /// 常旅客列表
        /// </summary>
        public List<FrePasserDto> Passengers
        {
            get { return _passengers; }

            set
            {
                if (_passengers == value) return;

                RaisePropertyChanging(PassengersPropertyName);
                _passengers = value;
                RaisePropertyChanged(PassengersPropertyName);
            }
        }

        #endregion
        #region 年龄类型

        private const string CAgeTypeName = "AgeType";

        private AgeType _ageType = AgeType.All;
        /// <summary>
        /// 年龄类型
        /// </summary>
        public AgeType AgeType
        {
            get { return _ageType; }

            set
            {
                if (_ageType == value) return;
                RaisePropertyChanging(CAgeTypeName);
                _ageType = value;
                RaisePropertyChanged(CAgeTypeName);
            }
        }

        #endregion
        #region 证件类型

        private const string CIdTypeName = "IDType";

        private IDType _idType = IDType.All;

        /// <summary>
        /// 证件类型
        /// </summary>
        public IDType IdType
        {
            get { return _idType; }

            set
            {
                if (_idType == value) return;
                RaisePropertyChanging(CIdTypeName);
                _idType = value;
                RaisePropertyChanged(CIdTypeName);
            }
        }
        #endregion
        /// <summary>
        /// 获取错误时，是否显示消息框
        /// </summary>
        private bool IsShowError { get; set; }
        #region 翻页

        #region PageSize

        /// <summary>
        /// The <see cref="PageSize" /> property's name.
        /// </summary>
        private const string PageSizePropertyName = "PageSize";

        private int _pageSize = 20;

        /// <summary>
        /// 翻页
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }

            set
            {
                if (_pageSize == value) return;

                RaisePropertyChanging(PageSizePropertyName);
                _pageSize = value;
                RaisePropertyChanged(PageSizePropertyName);
            }
        }

        #endregion

        #region CurrentPageIndex

        /// <summary>
        /// The <see cref="CurrentPageIndex" /> property's name.
        /// </summary>
        private const string CurrentPageIndexPropertyName = "CurrentPageIndex";

        private int _currentPageIndex = 1;

        /// <summary>
        /// 当前索引页
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }

            set
            {
                if (_currentPageIndex == value) return;

                RaisePropertyChanging(CurrentPageIndexPropertyName);
                _currentPageIndex = value;
                RaisePropertyChanged(CurrentPageIndexPropertyName);
            }
        }

        #endregion

        #region TotalCount

        /// <summary>
        /// The <see cref="TotalCount" /> property's name.
        /// </summary>
        private const string TotalCountPropertyName = "TotalCount";

        private int _totalCount;

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }

            set
            {
                if (_totalCount == value) return;

                RaisePropertyChanging(TotalCountPropertyName);
                _totalCount = value;
                RaisePropertyChanged(TotalCountPropertyName);
            }
        }

        #endregion

        #endregion
        #endregion

        #region 公开命令
        #region QueryCommand

        private RelayCommand _queryCommand;

        /// <summary>
        /// 查询命令
        /// </summary>
        public RelayCommand QueryCommand
        {
            get
            {
                return _queryCommand ?? (_queryCommand = new RelayCommand(ExecuteQueryCommand, CanExecuteQueryCommand));
            }
        }

        protected virtual void ExecuteQueryCommand()
        {
            QueryFrePasser.CertificateType = IdType == IDType.All ? "" : EnumHelper.GetDescription(IdType);
            QueryFrePasser.PasserType = AgeType == AgeType.All ? "" : EnumHelper.GetDescription(AgeType);
            
            _list.Clear();
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFrePasserService>(service =>
            {
                var data = service.QueryFrePassers(QueryFrePasser,CurrentPageIndex, PageSize);
                if (data == null) return;
                Passengers = data.List;
                TotalCount = data.TotalCount;
            }, ex =>
            {
                if (IsShowError)
                    UIManager.ShowErr(ex);
                else
                    Logger.WriteLog(LogType.INFO, "获取常旅客信息失败", ex);
            });

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }

        protected virtual bool CanExecuteQueryCommand()
        {
            return !isBusy;
        }

        #endregion
        #region SelectCommand

        private RelayCommand<FrePasserDto> _selectCommand;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand<FrePasserDto> SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand<FrePasserDto>(ExecuteSelectCommand));
            }
        }

        private void ExecuteSelectCommand(FrePasserDto model)
        {
            //if (model.PasserType != EnumHelper.GetDescription(AgeType.Adult))
            //    UIManager.ShowMessage("暂不支持儿童、婴儿预定");
            //else
                _list.Add(model);
        }

        #endregion
        #region UnSelectCommand

        private RelayCommand<FrePasserDto> _unselectCommand;

        /// <summary>
        /// 执行取消选择命令
        /// </summary>
        public RelayCommand<FrePasserDto> UnSelectCommand
        {
            get
            {
                return _unselectCommand ?? (_unselectCommand = new RelayCommand<FrePasserDto>(ExecuteUnSelectCommand));
            }
        }

        private void ExecuteUnSelectCommand(FrePasserDto model)
        {
            if(_list.Contains(model))
                _list.Remove(model);
        }

        #endregion
        #region SaveCommand

        private RelayCommand _saveCommand;

        /// <summary>
        /// 执行保存命令
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(ExecuteSaveCommand));
            }
        }

        private void ExecuteSaveCommand()
        {
            
            _ticketBookingViewModel.FrePassers = _list;
            IsSelected = true;
        }

        #endregion
        #endregion
    }
}
