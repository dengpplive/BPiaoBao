using System.Security.Permissions;
using System.Windows.Forms.VisualStyles;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using JoveZhao.Framework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 常旅客管理视图模型
    /// </summary>
    public class UsualPassengersManagerViewModel : BaseVM
    {
        #region 构造函数
        public UsualPassengersManagerViewModel()
        {
            Initialize();
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
        #region Passengers

        /// <summary>
        /// The <see cref="Passengers" /> property's name.
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
                if (_ageType != value)
                {
                    RaisePropertyChanging(CAgeTypeName);
                    _ageType = value;
                    RaisePropertyChanged(CAgeTypeName);
                }
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
                if (_idType != value)
                {
                    RaisePropertyChanging(CIdTypeName);
                    _idType = value;
                    RaisePropertyChanged(CIdTypeName);
                }
            }
        }
        #endregion
        #region SelectedPassenger

        /// <summary>
        /// The <see cref="SelectedPassenger" /> property's name.
        /// </summary>
        private const string SelectedPassengerPropertyName = "SelectedPassenger";

        private PassengerModel _selectedPassenger;

        /// <summary>
        /// 选中的常旅客
        /// </summary>
        public PassengerModel SelectedPassenger
        {
            get { return _selectedPassenger; }

            set
            {
                if (_selectedPassenger == value) return;

                RaisePropertyChanging(SelectedPassengerPropertyName);
                _selectedPassenger = value;
                RaisePropertyChanged(SelectedPassengerPropertyName);
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
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFrePasserService>(service =>
            {
                var data = service.QueryFrePassers(QueryFrePasser, CurrentPageIndex, PageSize);
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
            return !IsBusy;
        }

        #endregion
        #region OpenCommand

        private RelayCommand _openCommand;

        /// <summary>
        /// 打开新增页面命令
        /// </summary>
        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new RelayCommand(ExecuteOpenCommand, CanExecuteOpenCommand));
            }
        }

        private void ExecuteOpenCommand()
        {
            LocalUIManager.ShowUsualPassengerInfo(resut =>
            {
                if (resut != null && resut.Value)
                {
                    //添加完成刷新界面
                    ExecuteQueryCommand();
                }
            });
        }
        private bool CanExecuteOpenCommand()
        {
            return !IsBusy;
        }
        #endregion
        #region DelCommand

        private RelayCommand<FrePasserDto> _delCommand;

        /// <summary>
        /// 执行删除命令
        /// </summary>
        public RelayCommand<FrePasserDto> DelCommand
        {
            get
            {
                return _delCommand ?? (_delCommand = new RelayCommand<FrePasserDto>(ExecuteDelCommand, CanExecuteDelCommand));
            }
        }

        private void ExecuteDelCommand(FrePasserDto passenger)
        {
            var dialogResult = UIManager.ShowMessageDialog("确定要删除常旅客信息？");
            if (dialogResult == null || !dialogResult.Value)
                return;

            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IFrePasserService>(service =>
            {
                service.DeleteFrePasser(passenger.Id);
                DispatcherHelper.UIDispatcher.Invoke(new Func<FrePasserDto, bool>(Passengers.Remove), passenger);
                Passengers.Remove(passenger);
                ExecuteQueryCommand();
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setAction);
            });
        }
        private bool CanExecuteDelCommand(FrePasserDto passenger)
        {
            return passenger != null;
        }
        #endregion
        #region EditCommand

        private RelayCommand<FrePasserDto> _editCommand;

        /// <summary>
        /// 打开编辑页面命令
        /// </summary>
        public RelayCommand<FrePasserDto> EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new RelayCommand<FrePasserDto>(ExecuteEditCommand));
            }
        }

        private void ExecuteEditCommand(FrePasserDto fpd)
        {
            LocalUIManager.ShowUsualPassengerInfo(result =>
            {
                if (result == null || !result.Value) return;
                //编辑完成刷新界面
                ExecuteQueryCommand();
            }, fpd);
        }

        #endregion
        #region ImportCommand 导入

        private RelayCommand _importCommand;

        /// <summary>
        /// 导入命令
        /// </summary>
        public RelayCommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new RelayCommand(ExecuteImportCommand, CanExecuteImportCommand));
            }
        }

        private void ExecuteImportCommand()
        {
            Search();
        }

        public void Search()
        {
            var dtos = new List<FrePasserDto>();
            var ofd = new OpenFileDialog();
            ofd.ShowDialog();
            if (string.IsNullOrEmpty(ofd.FileName)) { return; }
            DataTable dt = null;
            try
            {
                dt = ExcelHelper.RenderToTableByNOPI(ofd.FileName);
            }
            catch (Exception ex)
            {

                UIManager.ShowErr(ex);
            }
            try
            {
                if (dt == null) return;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        var fpd = new FrePasserDto();
                        //Check null
                        if (string.IsNullOrEmpty(item["姓名"].ToString().Trim()))
                            continue;
                        fpd.Name = item["姓名"].ToString().Trim();
                        fpd.Mobile = item["手机号"].ToString().Trim();
                        if (dt.Columns.Contains("航空公司卡号")) fpd.AirCardNo = item["航空公司卡号"].ToString().Trim();
                        if (dt.Columns.Contains("备注")) fpd.Remark = item["备注"].ToString().Trim();
                        if (dt.Columns.Contains("性别") && !string.IsNullOrEmpty(item["性别"].ToString().Trim())) fpd.SexType = item["性别"].ToString().Trim();
                        if (!string.IsNullOrEmpty(item["证件类型"].ToString().Trim())) fpd.CertificateType = item["证件类型"].ToString().Trim();
                        if (!string.IsNullOrEmpty(item["乘客类型"].ToString().Trim())) fpd.PasserType = item["乘客类型"].ToString().Trim();
                        fpd.CertificateNo = item["证件号"].ToString().Trim();
                        fpd.CertificateNo = string.IsNullOrEmpty(item["证件号"].ToString().Trim()) && !string.IsNullOrEmpty(item["出生日期"].ToString().Trim()) ? Convert.ToDateTime(item["出生日期"]).ToString("yyyy-MM-dd").Trim() : fpd.CertificateNo;
                        if (!string.IsNullOrEmpty(item["出生日期"].ToString().Trim()))
                            fpd.Birth = Convert.ToDateTime(item["出生日期"].ToString().Trim());
                        else if (!string.IsNullOrEmpty(fpd.CertificateNo) && !string.IsNullOrEmpty(item["证件类型"].ToString().Trim()) && item["证件类型"].ToString().Trim() == EnumHelper.GetDescription(EnumIDType.BirthDate))
                            fpd.Birth = Convert.ToDateTime(item["证件号"].ToString().Trim());
                        else if (!string.IsNullOrEmpty(fpd.CertificateNo) && !string.IsNullOrEmpty(item["证件类型"].ToString().Trim()) && item["证件类型"].ToString().Trim() == EnumHelper.GetDescription(EnumIDType.NormalId))
                        {
                            //生日转换截取字符串
                            var birth = Common.ExtHelper.GetBirthdayDateFromString(fpd.CertificateNo);
                            if (birth != null) fpd.Birth = birth;
                        }
                        //Add List
                        dtos.Add(fpd);
                    }
                }
            }
            catch (Exception ex)
            {
                UIManager.ShowErr(ex);
                return;
            }
            Action action = () =>
            {
                IsBusy = true;


                CommunicateManager.Invoke<IFrePasserService>(service =>
                {

                    if (dtos.Count == 0)
                    {
                        UIManager.ShowMessage("没有可导入数据");
                        return;
                    }
                    service.Import(dtos);
                    //导入成功
                    UIManager.ShowMessage("导入成功");
                    ExecuteQueryCommand();
                }, UIManager.ShowErr);
            };

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteImportCommand()
        {
            return !IsBusy;
        }

        #endregion
        #region ExportCommand 导出
        private bool _isExporting;
        private RelayCommand _exportCommand;

        /// <summary>
        /// 导出文件
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return _exportCommand ?? (_exportCommand = new RelayCommand(ExecuteExportCommand, CanExecuteExportCommand));
            }
        }

        private void ExecuteExportCommand()
        {
            var dt = new DataTable("常旅客明细");
            var headArray = new[]
            {
                new KeyValuePair<string,Type>("姓名",typeof(string)),
                new KeyValuePair<string,Type>("性别",typeof(string)),
                new KeyValuePair<string,Type>("乘客类型",typeof(string)),
                new KeyValuePair<string,Type>("证件类型",typeof(string)),
                new KeyValuePair<string,Type>("证件号",typeof(string)),
                new KeyValuePair<string,Type>("手机号",typeof(string)),
                new KeyValuePair<string,Type>("出生日期",typeof(string)),
                new KeyValuePair<string,Type>("航空公司卡号",typeof(string)),
                new KeyValuePair<string,Type>("备注",typeof(string))                
            };

            foreach (var item in headArray)
            {
                dt.Columns.Add(item.Key, item.Value);
            }

            var dlg = new SaveFileDialog
            {
                FileName = "常旅客明细",
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
                    var detailList = GetPassengersList();

                    if (detailList != null)
                    {
                        foreach (var item in detailList)
                        {
                            dt.Rows.Add(
                                item.Name,
                                item.SexType,
                                item.PasserType,
                                item.CertificateType,
                                item.CertificateNo,
                                item.Mobile,
                                item.Birth.HasValue ? item.Birth.Value.ToString("yyyy/MM/dd") : "",
                                item.AirCardNo,
                                item.Remark
                                );
                        }
                    }
                    string filename = dlg.FileName;
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

        private IEnumerable<FrePasserDto> GetPassengersList()
        {
            QueryFrePasser.CertificateType = IdType == IDType.All ? "" : EnumHelper.GetDescription(IdType);
            QueryFrePasser.PasserType = AgeType == AgeType.All ? "" : EnumHelper.GetDescription(AgeType);
            List<FrePasserDto> result = null;
            CommunicateManager.Invoke<IFrePasserService>(service =>
            {
                result = service.Export(QueryFrePasser);
            });

            return result;
        }

        private bool CanExecuteExportCommand()
        {
            return !_isExporting && !IsBusy;
        }

        #endregion
        #region ExportModelCommand 导出模板

        private RelayCommand _exportModelCommand;

        /// <summary>
        /// 导出文件模板
        /// </summary>
        public RelayCommand ExportModelCommand
        {
            get
            {
                return _exportModelCommand ?? (_exportModelCommand = new RelayCommand(ExecuteExportModelCommand, CanExecuteExportModelCommand));
            }
        }

        private void ExecuteExportModelCommand()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "All Files | *.xls",
                FileName = "常旅客信息模板.xls",
                DefaultExt = ".xls"
            };
            if (saveDialog.ShowDialog() == true)
            {
                //// Open the output stream      
                //Stream deststream = new FileStream();
                //deststream = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "config/常旅客信息模板.xls");
                //deststream = saveDialog.OpenFile();
                //// Initiate asynchronous download    
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "config/常旅客信息模板.xls", saveDialog.FileName, true);
            }
        }

        private bool CanExecuteExportModelCommand()
        {
            return !IsBusy;
        }

        #endregion
        #endregion
    }
}
