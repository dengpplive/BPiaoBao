using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Client.Module;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Client.UIExt.Model.Enumeration;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BPiaoBao.Client.Account.ViewModel
{
    /// <summary>
    /// 信用账户 视图模型
    /// </summary>
    public class CreditViewModel : BillsViewModel
    {
        private bool _isExporting;
        #region 成员变量



        #endregion

        #region 构造函数

        /// <summary>
        /// 页面呈现后触发
        /// </summary>
        protected override void ExecutePageLoadCommand()
        {

        }

        //public CreditViewModel()
        //{
        //    if (IsInDesignMode)
        //        return;

        //}

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            StartTime = null;
            EndTime = null;
            PageSize = 10;

            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
            Messenger.Default.Send(true, "HomeRefresh");

            IsBusy = true;

            //刷新账户信息
            Action action = () =>
            {
                ViewModelLocator.Home.RefreshAccountInfo();
                ViewModelLocator.Home.RefreshRepay();
                HomeViewModel = ViewModelLocator.Home;

                CommunicateManager.Invoke<IBusinessmanService>(service =>
                {
                    CurrentUserInfoDto = service.GetCurrentUserInfo();
                    Debug.WriteLine("");
                }, UIManager.ShowErr);
                //获取申请临时额度相关返回条件值
                CommunicateManager.Invoke<IFundService>(service =>
                {
                    var info = service.GetTempCreditSetting();
                    Day = info.Day.ToString(CultureInfo.InvariantCulture); Number = info.Number.ToString(CultureInfo.InvariantCulture);
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                IsBusy = false;
            });
        }

        #endregion

        #region 公开属性

        #region HomeViewModel

        private HomeViewModel _homeViewModel;

        /// <summary>
        /// HomeViewModel
        /// </summary>        
        [DisplayName(@"HomeViewModel")]
        public HomeViewModel HomeViewModel
        {
            get { return _homeViewModel; }

            set
            {
                if (_homeViewModel == value) return;
                const string cHomeViewModelPropertyName = "HomeViewModel";
                RaisePropertyChanging(cHomeViewModelPropertyName);
                _homeViewModel = value;
                RaisePropertyChanged(cHomeViewModelPropertyName);
            }
        }
        #endregion

        #region CurrentUserInfoDto

        /// <summary>
        /// The <see cref="CurrentUserInfoDto" /> property's name.
        /// </summary>
        private const string CurrentUserInfoDtoPropertyName = "CurrentUserInfoDto";

        private CurrentUserInfoDto _currentUserInfoDto;

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public CurrentUserInfoDto CurrentUserInfoDto
        {
            get { return _currentUserInfoDto; }

            set
            {
                if (_currentUserInfoDto == value) return;

                RaisePropertyChanging(CurrentUserInfoDtoPropertyName);
                _currentUserInfoDto = value;
                RaisePropertyChanged(CurrentUserInfoDtoPropertyName);
            }
        }

        #endregion

        #region HasNotCredit

        /// <summary>
        /// The <see cref="HasNotCredit" /> property's name.
        /// </summary>
        private const string HasNotCreditPropertyName = "HasNotCredit";

        private bool _hasNotCredit;

        /// <summary>
        /// 没有信用权限
        /// </summary>
        public bool HasNotCredit
        {
            get { return _hasNotCredit; }

            set
            {
                if (_hasNotCredit == value) return;

                RaisePropertyChanging(HasNotCreditPropertyName);
                _hasNotCredit = value;
                RaisePropertyChanged(HasNotCreditPropertyName);
            }
        }

        #endregion

        #region Day

        /// <summary>
        /// The <see cref="Day" /> property's name.
        /// </summary>
        private const string DayPropertyName = "Day";

        private string _day;

        /// <summary>
        /// 逾期范围
        /// </summary>
        public string Day
        {
            get { return _day; }

            set
            {
                if (_day == value) return;

                RaisePropertyChanging(DayPropertyName);
                _day = value;
                RaisePropertyChanged(DayPropertyName);
            }
        }

        #endregion

        #region Number

        /// <summary>
        /// The <see cref="Number" /> property's name.
        /// </summary>
        private const string NumberPropertyName = "Number";

        private string _number;

        /// <summary>
        /// 申请次数
        /// </summary>
        public string Number
        {
            get { return _number; }

            set
            {
                if (_number == value) return;

                RaisePropertyChanging(NumberPropertyName);
                _number = value;
                RaisePropertyChanged(NumberPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令


        #region SwitchToBillsCommand

        private RelayCommand _switchToBillsCommand;

        /// <summary>
        /// 切换到账单详细页
        /// </summary>
        public RelayCommand SwitchToBillsCommand
        {
            get
            {
                return _switchToBillsCommand ?? (_switchToBillsCommand = new RelayCommand(ExecuteSwitchToBillsCommand, CanExecuteSwitchToBillsCommand));
            }
        }

        private void ExecuteSwitchToBillsCommand()
        {
            if (this.FullWidowExt != null && this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.BillsCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.BillsCode);
            }

        }

        private bool CanExecuteSwitchToBillsCommand()
        {
            return true;
        }

        #endregion

        #region SwitchToRepaymentCommand

        private RelayCommand _switchToRepaymentCommand;

        /// <summary>
        /// 切换到还款页面
        /// </summary>
        public RelayCommand SwitchToRepaymentCommand
        {
            get
            {
                return _switchToRepaymentCommand ?? (_switchToRepaymentCommand = new RelayCommand(ExecuteSwitchToRepaymentCommand, CanExecuteSwitchToRepaymentCommand));
            }
        }

        private void ExecuteSwitchToRepaymentCommand()
        {

            if (this.FullWidowExt != null && this.FullWidowExt.IsFullScreen)
            {
                var uc = PluginManager.FindItem(Main.ProjectCode, Main.RepaymentCode);
                if (uc != null)
                {
                    var control = uc.GetContent() as Control;
                    this.FullWidowExt.SetContent(control);
                }
            }
            else
            {

                PluginService.Run(Main.ProjectCode, Main.RepaymentCode);
            }

        }

        private bool CanExecuteSwitchToRepaymentCommand()
        {
            return true;
        }

        #endregion

        #region OpenAgreementCommand

        private RelayCommand _openAgreementCommand;

        /// <summary>
        /// 打开协议命令
        /// </summary>
        public RelayCommand OpenAgreementCommand
        {
            get
            {
                return _openAgreementCommand ?? (_openAgreementCommand = new RelayCommand(ExecuteOpenAgreementCommand, CanExecuteOpenAgreementCommand));
            }
        }

        private void ExecuteOpenAgreementCommand()
        {
            //string runDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //UIManager.ShowWeb("购买协议", String.Format("{0}/FinanceAgreement.html", runDir));

            UIManager.ShowWeb("信用账户使用规则说明", "http://www.51cbc.cn/CreditInstruction.html");


            //pack://siteoforigin:,,,/FinanceAgreement.html
        }

        private bool CanExecuteOpenAgreementCommand()
        {
            return true;
        }

        #endregion


        //  ExportCommand
        private RelayCommand _exportCreditCommand;

        /// <summary>
        /// Gets the ExportCommand.
        /// </summary>
        public RelayCommand ExportCreditCommand
        {
            get
            {
                return _exportCreditCommand ?? (_exportCreditCommand = new RelayCommand(ExecuteExportCreditCommand, CanExecuteExportCreditCommand));
            }
        }

        private IEnumerable<BillListDto> GetList()
        {
            List<BillListDto> result = null;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var status = SelectedStatus == BillStatus.All ? null : (SelectedStatus == BillStatus.Paid ? "1" : "0");
                var temp = service.GetBill(StartTime, EndTime, status, 0, 1000);
                result = temp.List;
            });
            return result;
        }
        private void ExecuteExportCreditCommand()
        {
            var dt = new DataTable("信用账户-历史账单");
            var headArray = new[]
            {
                new KeyValuePair<string,Type>("账单日期",typeof(DateTime)),
                new KeyValuePair<string,Type>("账单金额",typeof(decimal)),
                new KeyValuePair<string,Type>("消费金额",typeof(decimal)),
                new KeyValuePair<string,Type>("利息",typeof(decimal)),
                new KeyValuePair<string,Type>("滞纳金",typeof(decimal)),
                new KeyValuePair<string,Type>("已还金额",typeof(decimal)),
                new KeyValuePair<string,Type>("未还金额",typeof(decimal)),
                new KeyValuePair<string,Type>("账单状态",typeof(string))
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }
            var dlg = new SaveFileDialog
            {
                FileName = "信用账户-历史账单",
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
                                item.CreateDate,
                                item.BillAmount,
                                item.Amount,
                                item.FeeAmount,
                                item.LateAmount,
                                item.RepayAmount,
                                item.ShouldRepayAmount,
                                item.Status
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
        private bool CanExecuteExportCreditCommand()
        {
            return !_isExporting;
        }

        #region ShowApplyTemporaryLimitCommand

        private RelayCommand _showApplyTemporaryLimitCommand;

        /// <summary>
        /// 打开临时额度申请
        /// </summary>
        public RelayCommand ShowApplyTemporaryLimitCommand
        {
            get
            {
                return _showApplyTemporaryLimitCommand ?? (_showApplyTemporaryLimitCommand = new RelayCommand(ExecuteShowApplyTemporaryLimitCommand, CanExecuteShowApplyTemporaryLimitCommand));
            }
        }

        private void ExecuteShowApplyTemporaryLimitCommand()
        {
            LocalUIManager.ShowApplyTemporaryLimit(Day, Number, p => Initialize());
        }

        private bool CanExecuteShowApplyTemporaryLimitCommand()
        {
            return true;
        }

        #endregion
        #endregion


    }
}
