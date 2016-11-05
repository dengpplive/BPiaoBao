using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 协调视图模型
    /// </summary>
    public class CoordinationViewModel : BaseVM
    {
        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinationViewModel"/> class.
        /// </summary>
        public CoordinationViewModel()
        {

        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (SourceOrder == null) return;

            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    //Coordination = service.GetCoordinationDto(SourceOrder.OrderId);
                    //RefreshData(result);
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region 公开属性

        #region Coordination

        /// <summary>
        /// The <see cref="Coordination" /> property's name.
        /// </summary>
        public const string CoordinationPropertyName = "Coordination";

        private CoordinationDto coordinationDto = null;

        /// <summary>
        /// 订单协调信息
        /// </summary>
        public CoordinationDto Coordination
        {
            get { return coordinationDto; }

            set
            {
                if (coordinationDto == value) return;

                RaisePropertyChanging(CoordinationPropertyName);
                coordinationDto = value;
                RaisePropertyChanged(CoordinationPropertyName);
            }
        }

        #endregion

        #region CoordinationLogs

        ///// <summary>
        ///// The <see cref="CoordinationLogs" /> property's name.
        ///// </summary>
        //public const string CoordinationLogsPropertyName = "CoordinationLogs";

        //private ObservableCollection<CoordinationLogDto> coordinationLogs = new ObservableCollection<CoordinationLogDto>();

        ///// <summary>
        ///// 协调列表
        ///// </summary>
        //public ObservableCollection<CoordinationLogDto> CoordinationLogs
        //{
        //    get { return coordinationLogs; }

        //    set
        //    {
        //        if (coordinationLogs == value) return;

        //        RaisePropertyChanging(CoordinationLogsPropertyName);
        //        coordinationLogs = value;
        //        RaisePropertyChanged(CoordinationLogsPropertyName);
        //    }
        //}

        #endregion

        #region IsBusy

        ///// <summary>
        ///// The <see cref="IsBusy" /> property's name.
        ///// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        ///// <summary>
        ///// 是否在忙
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

        #region InputText

        /// <summary>
        /// The <see cref="InputText" /> property's name.
        /// </summary>
        public const string InputTextPropertyName = "InputText";

        private string inputText = null;

        /// <summary>
        /// 输入内容
        /// </summary>
        public string InputText
        {
            get { return inputText; }

            set
            {
                if (inputText == value) return;

                RaisePropertyChanging(InputTextPropertyName);
                inputText = value;
                RaisePropertyChanged(InputTextPropertyName);
            }
        }

        #endregion

        #region SourceOrder

        /// <summary>
        /// The <see cref="SourceOrder" /> property's name.
        /// </summary>
        public const string SourceOrderPropertyName = "SourceOrder";

        private OrderDto sourceOrder = null;

        /// <summary>
        /// 输入订单数据
        /// </summary>
        public OrderDto SourceOrder
        {
            get { return sourceOrder; }

            set
            {
                if (sourceOrder == value) return;

                RaisePropertyChanging(SourceOrderPropertyName);
                sourceOrder = value;
                RaisePropertyChanged(SourceOrderPropertyName);
            }
        }

        #endregion

        #endregion

        #region 公开命令

        #region AddCoordinationCommand

        private RelayCommand addCoordination;

        /// <summary>
        /// Gets the AddCoordinationCommand.
        /// </summary>
        public RelayCommand AddCoordinationCommand
        {
            get
            {
                return addCoordination ?? (addCoordination = new RelayCommand(ExecuteAddCoordinationCommand, CanExecuteAddCoordinationCommand));
            }
        }

        private void ExecuteAddCoordinationCommand()
        {
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    //service.AddCoordinationDto(SourceOrder.OrderId, inputText);
                    //添加成功刷新数据
                    // Coordination = service.GetCoordinationDto(SourceOrder.OrderId);
                    //RefreshData(service.GetCoordinationDto(SourceOrder.OrderId));

                    InputText = null;
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteAddCoordinationCommand()
        {
            return SourceOrder != null && !String.IsNullOrEmpty(inputText);
        }

        #endregion

        #endregion


        //private void RefreshData(CoordinationDto result)
        //{
        //    CoordinationLogs.Clear();
        //    if (result != null)
        //    {
        //        foreach (var item in result.CoordinationLogs)
        //        {
        //            DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
        //            {
        //                CoordinationLogs.Add(item);
        //            }));
        //        }
        //    }
        //}


        internal void LoadOrderInfo(string orderId)
        {
            IsBusy = true;
            Action action = () =>
            {
                CommunicateManager.Invoke<IOrderService>(service =>
                {
                    var temp = service.FindAll(orderId, null, null, null, null, null, 0, 1);
                    if (temp != null && temp.TotalCount > 0)
                        DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                        {
                            SourceOrder = temp.List[0];
                        }));
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith((task) =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }
    }
}
