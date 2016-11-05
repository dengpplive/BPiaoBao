using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common.Enums;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class InsuranceViewModel : BaseVM
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InsuranceViewModel()
        {
            _allInsurancesStatus.Add(new KeyValuePair<EnumInsuranceStatus?, string>(null, "请选择"));
            _allInsurancesStatus.Add(new KeyValuePair<EnumInsuranceStatus?, string>(EnumInsuranceStatus.NoInsurance, EnumHelper.GetDescription(EnumInsuranceStatus.NoInsurance)));
            _allInsurancesStatus.Add(new KeyValuePair<EnumInsuranceStatus?, string>(EnumInsuranceStatus.GotInsurance, EnumHelper.GetDescription(EnumInsuranceStatus.GotInsurance)));
            _allInsurancesStatus.Add(new KeyValuePair<EnumInsuranceStatus?, string>(EnumInsuranceStatus.Canceled, EnumHelper.GetDescription(EnumInsuranceStatus.Canceled)));

            Initialize();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public override void Initialize()
        {
            if (CanExecuteQueryCommand())
                ExecuteQueryCommand();
        }

        #region 公开属性

        #region RequestQueryInsurance

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string RequestQueryInsurancePropertyName = "RequestQueryInsurance";

        private QueryInsuranceModel _queryInsurance = new QueryInsuranceModel();

        /// <summary>
        /// 查询实体
        /// </summary>
        public QueryInsuranceModel QueryInsurance
        {
            get { return _queryInsurance; }

            set
            {
                if (_queryInsurance == value) return;

                RaisePropertyChanging(RequestQueryInsurancePropertyName);
                _queryInsurance = value;
                RaisePropertyChanged(RequestQueryInsurancePropertyName);
            }
        }

        #endregion

        #region AllInsurancesStatus

        /// <summary>
        /// The <see cref="AllInsurancesStatus" /> property's name.
        /// </summary>
        private const string AllInsurancesStatusPropertyName = "AllInsurancesStatus";

        private ObservableCollection<KeyValuePair<EnumInsuranceStatus?, String>> _allInsurancesStatus = new ObservableCollection<KeyValuePair<EnumInsuranceStatus?, string>>();

        /// <summary>
        /// 所有保单状态
        /// </summary>
        public ObservableCollection<KeyValuePair<EnumInsuranceStatus?, String>> AllInsurancesStatus
        {
            get { return _allInsurancesStatus; }

            set
            {
                if (_allInsurancesStatus == value) return;

                RaisePropertyChanging(AllInsurancesStatusPropertyName);
                _allInsurancesStatus = value;
                RaisePropertyChanged(AllInsurancesStatusPropertyName);
            }
        }

        #endregion

        #region Insurances

        /// <summary>
        /// The <see cref="Insurances" /> property's name.
        /// </summary>
        private const string InsurancesPropertyName = "Insurances";

        private ObservableCollection<ResponseInsurance> _insurances = new ObservableCollection<ResponseInsurance>();

        /// <summary>
        /// 显示的保单
        /// </summary>
        public ObservableCollection<ResponseInsurance> Insurances
        {
            get { return _insurances; }

            set
            {
                if (_insurances == value) return;

                RaisePropertyChanging(InsurancesPropertyName);
                _insurances = value;
                RaisePropertyChanged(InsurancesPropertyName);
            }
        }

        #endregion

        #region IsBusy

        /// <summary>
        /// The <see cref="IsBusy" /> property's name.
        /// </summary>
        //public const string IsBusyPropertyName = "IsBusy";

        //private bool isBusy = false;

        /// <summary>
        /// 是否正在忙碌
        /// </summary>
        public new bool IsBusy
        {
            get { return isBusy; }

            set
            {
                if (isBusy == value) return;

                RaisePropertyChanging(IsBusyPropertyName);
                isBusy = value;
                RaisePropertyChanged(IsBusyPropertyName);

                if (_queryCommand != null)
                    _queryCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

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
            #region 日期判断处理
            if (QueryInsurance.BuyStartTime != null && QueryInsurance.BuyEndTime != null)
            {
                if (QueryInsurance.BuyStartTime.Value.CompareTo(QueryInsurance.BuyEndTime.Value) > 0)
                {
                    UIManager.ShowMessage("航班日期选择开始日期大于结束日期");
                    return;
                }
            }
            if (QueryInsurance.InsuranceLimitStartTime != null && QueryInsurance.InsuranceLimitEndTime != null)
            {
                if (QueryInsurance.InsuranceLimitStartTime.Value.CompareTo(QueryInsurance.InsuranceLimitEndTime.Value) > 0)
                {
                    UIManager.ShowMessage("保险有限期日期选择开始日期大于结束日期");
                    return;
                }
            }
            if (QueryInsurance.FlyStartTime != null && QueryInsurance.FlyEndTime != null)
            {
                if (QueryInsurance.FlyStartTime.Value.CompareTo(QueryInsurance.FlyEndTime.Value) > 0)
                {
                    UIManager.ShowMessage("航班日期选择开始日期大于结束日期");
                    return;
                }
            } 
            #endregion
            //保单接口查询实体
            var reQueryInsurance = new RequestQueryInsurance
            {
                IsClientCall = true,
                BuyEndTime = QueryInsurance.BuyEndTime,
                BuyStartTime = QueryInsurance.BuyStartTime,
                EnumInsuranceStatus = QueryInsurance.EnumInsuranceStatus.HasValue ? QueryInsurance.EnumInsuranceStatus : null,
                FlyEndTime = QueryInsurance.FlyEndTime,
                FlyStartTime = QueryInsurance.FlyStartTime,
                InsuranceLimitEndTime = QueryInsurance.InsuranceLimitEndTime,
                InsuranceLimitStartTime = QueryInsurance.InsuranceLimitStartTime,
                InsuranceNo = QueryInsurance.InsuranceNo,
                Mobile = QueryInsurance.Mobile,
                OrderId = QueryInsurance.OrderId,
                PassengerName = QueryInsurance.PassengerName,
                CardNo = QueryInsurance.IdNo
            };
            IsBusy = true;
            Insurances.Clear();
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                var data = service.QueryInsurance(reQueryInsurance, CurrentPageIndex, PageSize);
                if (data.List == null)
                    return;
                TotalCount = data.TotalCount;

                foreach (var item in data.List)
                {
                    DispatcherHelper.UIDispatcher.Invoke(new Action<ResponseInsurance>(Insurances.Add), item);
                }
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        protected virtual bool CanExecuteQueryCommand()
        {
            return !isBusy;
        }

        #endregion

        #region ClearCommand 清空

        private RelayCommand _clearCommand;

        /// <summary>
        /// 导入命令
        /// </summary>
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteClearCommand));
            }
        }

        private void ExecuteClearCommand()
        {
            QueryInsurance.BuyEndTime = null;
            QueryInsurance.BuyStartTime = null;
            QueryInsurance.EnumInsuranceStatus = null;
            QueryInsurance.FlightNumber = string.Empty;
            QueryInsurance.FlyEndTime = null;
            QueryInsurance.FlyStartTime = null;
            QueryInsurance.IdNo = string.Empty;
            QueryInsurance.InsuranceLimitEndTime = null;
            QueryInsurance.InsuranceLimitStartTime = null;
            QueryInsurance.InsuranceNo = string.Empty;
            QueryInsurance.Mobile = string.Empty;
            QueryInsurance.OrderId = string.Empty;
            QueryInsurance.PassengerName = string.Empty;
            QueryInsurance.PNR = string.Empty;
        }

        private bool CanExecuteClearCommand()
        {
            return !IsBusy;
        }

        #endregion

        #region ExportCommand 导出

        private bool IsExporting = false;
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
            //DataTable dt = new DataTable("常旅客明细");
            //KeyValuePair<string, Type>[] headArray = new KeyValuePair<string, Type>[]
            //{
            //    new KeyValuePair<string,Type>("保单号",typeof(string)),
            //    new KeyValuePair<string,Type>("订单号",typeof(string)),
            //    new KeyValuePair<string,Type>("投保时间",typeof(string)),
            //    new KeyValuePair<string,Type>("被保人姓名",typeof(string)),
            //    new KeyValuePair<string,Type>("证件号",typeof(string)),
            //    new KeyValuePair<string,Type>("手机号",typeof(string)),

            //    new KeyValuePair<string,Type>("航空公司卡号",typeof(string)),
            //    new KeyValuePair<string,Type>("备注",typeof(string))                
            //};

            //foreach (var item in headArray)
            //{
            //    dt.Columns.Add(item.Key, item.Value);
            //}

            //SaveFileDialog dlg = new SaveFileDialog();
            //dlg.FileName = "常旅客明细";
            //dlg.DefaultExt = ".xls";
            //dlg.Filter = "Excel documents (.xls)|*.xls";

            //var result = dlg.ShowDialog();
            //if (result == true)
            //{
            //    isExporting = IsBusy = true;
            //    var exportAction = new Action(() =>
            //    {
            //        try
            //        {
            //            List<FrePasserDto> DetailList = GetPassengersList();

            //            if (DetailList != null)
            //            {
            //                foreach (var item in DetailList)
            //                {
            //                    dt.Rows.Add(
            //                        item.Name,
            //                        item.PasserType,
            //                        item.CertificateType,
            //                        item.CertificateNo,//item.PasserType != EnumHelper.GetDescription(AgeType.Baby) ? item.CertificateNo : "",
            //                        item.Mobile,
            //                        item.PasserType == EnumHelper.GetDescription(AgeType.Baby) ? Convert.ToDateTime(item.CertificateNo).ToString("yyyy-MM-dd") : "",
            //                        item.AirCardNo,
            //                        item.Remark
            //                        );
            //                }
            //            }
            //            string filename = dlg.FileName;
            //            ExcelHelper.RenderToExcel(dt, filename);
            //            UIManager.ShowMessage("导出成功");
            //        }
            //        catch (Exception ex)
            //        {
            //            UIManager.ShowErr(ex);
            //        }
            //    });

            //    Task.Factory.StartNew(exportAction).ContinueWith((task) =>
            //    {
            //        isExporting = IsBusy = false;
            //    });
            //}
        }

        //private List<ResponseInsurance> GetList()
        //{
        //    List<ResponseInsurance> result = null;
        //    CommunicateManager.Invoke<IInsuranceService>(service =>
        //    {
        //        //result = service.Export(QueryInsurance);
        //    });

        //    return result;
        //}

        private bool CanExecuteExportCommand()
        {
            return !IsExporting && !IsBusy;
        }

        #endregion

        #region DetailCommand

        private RelayCommand<ResponseInsurance> _detailCommand;

        /// <summary>
        /// 打开详情命令
        /// </summary>
        public RelayCommand<ResponseInsurance> DetailCommand
        {
            get
            {
                return _detailCommand ?? (_detailCommand = new RelayCommand<ResponseInsurance>(ExecuteDetailCommand, CanExecuteDetailCommand));
            }
        }

        private void ExecuteDetailCommand(ResponseInsurance model)
        {
            LocalUIManager.ShowInsuranceDetailWindow(model);
        }

        private bool CanExecuteDetailCommand(ResponseInsurance model)
        {
            return !isBusy;
        }

        #endregion

        #endregion
    }


}
