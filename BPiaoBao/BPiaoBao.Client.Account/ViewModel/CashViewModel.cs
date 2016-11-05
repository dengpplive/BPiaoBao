using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 现金账户 视图模型
    /// </summary>
    public class CashViewModel : PageBaseViewModel
    {
        private bool _isExporting;
        #region 成员变量

        private bool _isQueryList;

        #endregion

        #region 构造函数

        /// <summary>
        /// Initializes a new instance of the <see cref="CashViewModel" /> class.
        /// </summary>
        public CashViewModel()
        {
            if (IsInDesignMode)
                return;

            //if (CanExecuteQueryCommand())
                Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            IsShowTransfer = UIExt.Communicate.LoginInfo.IsAdmin;
            IsBusy = true;
            //刷新账户信息
            Action action = () => ViewModelLocator.Home.RefreshAccountInfo();

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                IsBusy = false;
            });

            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #endregion

        #region 公开属性

        #region BalanceDetails

        /// <summary>
        /// The <see cref="BalanceDetails" /> property's name.
        /// </summary>
        private const string BalanceDetailsPropertyName = "BalanceDetails";

        private ObservableCollection<BalanceDetailDto> _balanceDetail = new ObservableCollection<BalanceDetailDto>();

        /// <summary>
        /// 现金账户明细
        /// </summary>
        public ObservableCollection<BalanceDetailDto> BalanceDetails
        {
            get { return _balanceDetail; }

            set
            {
                if (_balanceDetail == value) return;

                RaisePropertyChanging(BalanceDetailsPropertyName);
                _balanceDetail = value;
                RaisePropertyChanged(BalanceDetailsPropertyName);
            }
        }

        #endregion

        #region OutTradeNo

        /// <summary>
        /// The <see cref="OutTradeNo" /> property's name.
        /// </summary>
        private const string OutTradeNoPropertyName = "OutTradeNo";

        private string _outTradeNo;

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo
        {
            get { return _outTradeNo; }

            set
            {
                if (_outTradeNo == value) return;

                RaisePropertyChanging(OutTradeNoPropertyName);
                _outTradeNo = value;
                RaisePropertyChanged(OutTradeNoPropertyName);
            }
        }

        #endregion

        #region IsShowTransfer

        /// <summary>
        /// The <see cref="IsShowTransfer" /> property's name.
        /// </summary>
        private const string IsShowTransferPropertyName = "IsShowTransfer";

        private bool _isShowTransfer;

        /// <summary>
        /// 显示隐藏转账按钮
        /// </summary>
        public bool IsShowTransfer
        {
            get { return _isShowTransfer; }

            set
            {
                if (_isShowTransfer == value) return;

                RaisePropertyChanging(IsShowTransferPropertyName);
                _isShowTransfer = value;
                RaisePropertyChanged(IsShowTransferPropertyName);
            }
        }

        #endregion

        /// <summary>
        /// 显示隐藏转账按钮
        /// </summary>
        //public bool IsShowTransfer { get { return BPiaoBao.Client.UIExt.Communicate.LoginInfo.IsAdmin; } }       

        #endregion

        #region 公开命令

        #region 查询

        /// <summary>
        /// 检查是否可以执行命令
        /// </summary>
        /// <returns></returns>
        protected override bool CanExecuteQueryCommand()
        {
            return !_isQueryList;
        }

        /// <summary>
        /// 执行查询命令
        /// </summary>
        protected override void ExecuteQueryCommand()
        {
            //if (StartTime == null)
            //{
            //    UIManager.ShowMessage("请选择开始时间");
            //    return;
            //}
            //if (EndTime == null)
            //{
            //    UIManager.ShowMessage("请选择结束时间");
            //    return;
            //}

            //TimeSpan ts = EndTime.Value - StartTime.Value;
            //if (ts.Days > 92)
            //{
            //    UIManager.ShowMessage("导出的日期间隔最大为3个月时间");
            //    return;
            //}

            _isQueryList = IsBusy = true;
            BalanceDetails.Clear();

            Action action = () => CommunicateManager.Invoke<IAccountService>(service =>
            {
                var result = service.GetReadyAccountDetails(StartTime, EndTime, OutTradeNo, null, (CurrentPageIndex - 1) * PageSize, PageSize);
                DispatcherHelper.UIDispatcher.Invoke(new Action(() =>
                {
                    TotalCount = result.TotalCount;
                }));

                if (result.List == null) return;
                foreach (var item in result.List)
                    DispatcherHelper.UIDispatcher.Invoke(new Action<BalanceDetailDto>(BalanceDetails.Add), item);
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { _isQueryList = IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region SwitchToDepositCommand

        private RelayCommand _switchToDepositCommand;

        /// <summary>
        /// 切换到充值记录
        /// </summary>
        public RelayCommand SwitchToDepositCommand
        {
            get
            {
                return _switchToDepositCommand ?? (_switchToDepositCommand = new RelayCommand(ExecuteSwitchToDepositCommand, CanExecuteSwitchToDepositCommand));
            }
        }

        private void ExecuteSwitchToDepositCommand()
        {
           if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.DepositCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.DepositCode);
            }
        }

        private bool CanExecuteSwitchToDepositCommand()
        {
            return true;
        }

        #endregion

        #region SwitchToWithdrawDepositCommand

        private RelayCommand _switchToWithdrawDepositCommand;

        /// <summary>
        /// 切换到结算界面
        /// </summary>
        public RelayCommand SwitchToWithdrawDepositCommand
        {
            get
            {
                return _switchToWithdrawDepositCommand ?? (_switchToWithdrawDepositCommand = new RelayCommand(ExecuteSwitchToWithdrawDepositCommand, CanExecuteSwitchToWithdrawDepositCommand));
            }
        }

        private void ExecuteSwitchToWithdrawDepositCommand()
        {
            if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.WithdrawDepositCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.WithdrawDepositCode); 
            }
        }

        private bool CanExecuteSwitchToWithdrawDepositCommand()
        {
            return true;
        }

        #endregion

        #region SwitchToWithTransferCommand

        private RelayCommand _switchToWithTransferCommand;

        /// <summary>
        /// 切换到转账界面
        /// </summary>
        public RelayCommand SwitchToWithTransferCommand
        {
            get
            {
                return _switchToWithTransferCommand ?? (_switchToWithTransferCommand = new RelayCommand(ExecuteSwitchToWithTransferCommand, CanExecuteSwitchToWithTransferCommand));
            }
        }

        private void ExecuteSwitchToWithTransferCommand()
        {
          if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.TransferCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.TransferCode);
            } 
        }

        private bool CanExecuteSwitchToWithTransferCommand()
        {
            return true;
        }

        #endregion

        #region SwitchViewCommand

        private RelayCommand<string> _switchViewCommand;

        /// <summary>
        /// Gets the SwitchViewCommand.
        /// </summary>
        public RelayCommand<string> SwitchViewCommand
        {
            get
            {
                return _switchViewCommand ?? (_switchViewCommand = new RelayCommand<string>(ExecuteSwitchViewCommand, CanExecuteSwitchViewCommand));
            }
        }

        private void ExecuteSwitchViewCommand(string viewCode)
        {
            switch (viewCode)
            {
                case "AllFinance":
                  if (this.FullWidowExt!=null&&this.FullWidowExt.IsFullScreen)
                    {
                        var uc = PluginManager.FindItem(Main.ProjectCode, Main.AllFinanceCode);
                        if (uc != null)
                        {
                            var control = uc.GetContent() as Control;
                            this.FullWidowExt.SetContent(control);
                        }
                    }
                    else
                    {

                        PluginService.Run(Main.ProjectCode, Main.AllFinanceCode);
                    } 
                    
                    break;
            }
        }

        private bool CanExecuteSwitchViewCommand(string viewCode)
        {
            return true;
        }

        #endregion

        //  ExportCommand
        private RelayCommand _exportCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand(ExecuteExportCommand, CanExecuteExportCommand));
            }
        }
        private IEnumerable<BalanceDetailDto> GetList()
        {
            List<BalanceDetailDto> result = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var temp = service.GetReadyAccountDetails(StartTime, EndTime, OutTradeNo, null, 0, 65535);
                result = temp.List;
            });
            return result;
        }
        private void ExecuteExportCommand()
        {
            //if (StartTime == null)
            //{
            //    UIManager.ShowMessage("请选择开始时间");
            //    return;
            //}
            //if (EndTime == null)
            //{
            //    UIManager.ShowMessage("请选择结束时间");
            //    return;
            //}

            //var ts = EndTime.Value - StartTime.Value;
            //if (ts.Days > 92)
            //{
            //    UIManager.ShowMessage("导出的日期间隔最大为3个月时间");
            //    return;
            //}

            var dt = new DataTable("现金账户-收支明细");
            var headArray = new[]
            {
                new KeyValuePair<string,Type>("日期",typeof(string)),
                new KeyValuePair<string,Type>("收支(元)",typeof(decimal)),
                new KeyValuePair<string,Type>("交易类型",typeof(string)),
                new KeyValuePair<string,Type>("账户余额(元)",typeof(decimal)),  
                new KeyValuePair<string,Type>("交易号",typeof(string)),
                new KeyValuePair<string,Type>("备注",typeof(string))
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }
            var dlg = new SaveFileDialog
            {
                FileName = "现金账户-收支明细",
                DefaultExt = ".xls",
                Filter = "Excel documents (.xls)|*.xls"
            };

            var result = dlg.ShowDialog();
            if (result != true) return;
            _isExporting = IsBusy = true;
            var exportAction = new Action(() =>
            {
                try
                {
                    var balanceDetailList = GetList();
                    if (balanceDetailList != null)
                    {
                        foreach (var item in balanceDetailList)
                        {
                            dt.Rows.Add(
                                //item.SerialNum,
                                item.CreateAmount.ToString("yyyy-MM-dd HH:mm:ss"),
                                item.Amount,
                                item.OperationType,
                                item.LeaveAmount,
                                item.OutTradeNo,
                                item.Remark
                                );
                        }
                    }

                    var filename = dlg.FileName;
                    ExcelHelper.RenderToExcel(dt, filename);
                    UIManager.ShowMessage("导出成功");
                }
                catch (Exception ex)
                {
                    UIManager.ShowErr(ex);
                }
            });

            Task.Factory.StartNew(exportAction).ContinueWith(task =>
            {
                _isExporting = IsBusy = false;
            });
        }
        private bool CanExecuteExportCommand()
        {
            return !_isExporting;
        }
        #endregion
    }
}
