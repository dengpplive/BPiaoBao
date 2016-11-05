using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Communicate;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 预订机票视图界面
    /// </summary>
    public class TicketBookingBackViewModel : BaseVM
    {
        public const string c_CloseTicketBookingBackWindow = "CloseTicketBookingBackWindow";

        #region 公开命令

        #region BookingCommand

        private RelayCommand<FlightInfoModel> _bookingCommand;

        public RelayCommand<FlightInfoModel> BookingCommand
        {
            get { return _bookingCommand ?? (new RelayCommand<FlightInfoModel>(ExceuteBookingCommand)); }
        }

        private void ExceuteBookingCommand(FlightInfoModel flight)
        {
            var list = new List<FlightInfoModel> { FlightInfoModel, flight };
            LocalUIManager.ShowTicketBooking(list.ToArray(), null, null, c_CloseTicketBookingBackWindow);
        }

        #endregion

        #region BookingSubCommand

        private RelayCommand<Site> _bookingSubCommand;

        public RelayCommand<Site> BookingSubCommand
        {
            get { return _bookingSubCommand ?? (new RelayCommand<Site>(ExecuteBookingSubCommand)); }
        }

        private void ExecuteBookingSubCommand(Site site)
        {
            var list = new List<FlightInfoModel>();
            list.Add(FlightInfoModel);
            foreach (var fm in FlightInfoModels)
            {
                if (fm.CusNo == site.CusNo)
                {
                    fm.DefaultSite = site;
                    list.Add(fm);
                    break;

                }
            }
            LocalUIManager.ShowTicketBooking(list.ToArray(), null, null, c_CloseTicketBookingBackWindow);
        }

        #endregion

        #region QueryStopTextRemarkCommand

        private RelayCommand<FlightInfoModel> _queryStopTextRemarkCommand;

        public RelayCommand<FlightInfoModel> QueryStopTextRemarkCommand
        {
            get
            {
                return _queryStopTextRemarkCommand ?? (new RelayCommand<FlightInfoModel>(ExecuteQueryStopTextRemarkCommand, CanExecuteQueryStopTextRemarkCommand));
            }
        }

        public void ExecuteQueryStopTextRemarkCommand(FlightInfoModel model)
        {
            //查询航班经停止信息

            Action action = () => CommunicateManager.Invoke<IFlightDestineService>(service =>
            {
                var m = service.GetLegStop(LoginInfo.Code, model.CarrayCode + model.FlightNumber, model.StartDateTime);

                var leg = new LegStopModel
                {
                    FlightCodeNumber = m.CarrayCodeFlightNum,
                    StartDate = m.StopDate + " " + m.FromTime,
                    ToDate = m.StopDate + " " + m.ToTime,
                    MiddleDate = m.StopDate + " " + m.MiddleTime1,
                    FromCity = m.FromCity,
                    ToCity = m.ToCity,
                    MiddleCity = m.MiddleCity
                };

                model.LegStopModel = leg;



            }, UIManager.ShowErr);
            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        public bool CanExecuteQueryStopTextRemarkCommand(FlightInfoModel model)
        {
            return model.LegStopModel == null;
        }
        #endregion

        #region GetSpecialFromSiteCommand

        private RelayCommand<Site> _getSpecialFromSiteCommand;

        public RelayCommand<Site> GetSpecialFromSiteCommand
        {
            get { return _getSpecialFromSiteCommand ?? (new RelayCommand<Site>(ExecuteGetSpecialFromSiteCommand, CanExecuteGetSpecialFromSiteCommand)); }
        }

        private void ExecuteGetSpecialFromSiteCommand(Site site)
        {
            var model = FlightInfoModels.ToList().FirstOrDefault(m => m.CusNo == site.CusNo);
            if (model == null) return;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IPidService>(service =>
            {
                var pricelist = service.sendSSPat(LoginInfo.Code, model.CarrayCode, model.FlightNumber, site.SeatCode,
                    model.StartDateTime.ToString("yyyy-MM-dd"), model.FromCityCode, model.ToCityCode, model.StartDateTime.ToString("HH:mm"),
                    model.ToDateTime.ToString("HH:mm"));
                var sitelist = new List<Site>();
                if (!pricelist.Any())
                {
                    if (site == model.DefaultSite)
                    {
                        model.DefaultSite.IsGotSpecial = true;
                        model.DefaultSite.IsReceivedSpecial = false;
                        if (model.IsOpened) model.IsOpened = false;//ui恢复
                        FlightInfoModels = FlightInfoModels;
                    }
                    foreach (var item in model.SiteList)
                    {
                        if (item == site)
                        {
                            item.IsGotSpecial = true;
                            item.IsReceivedSpecial = false;
                        }
                        sitelist.Add(item);
                    }
                    model.SiteList = sitelist;
                    model.IsSiteListChanged = true;
                }
                else
                {
                    var info = pricelist.FirstOrDefault();
                    if (info == null) return;
                    decimal taxFee;
                    decimal rqFee;
                    decimal seatPrice;
                    decimal.TryParse(info.TAX, out taxFee);
                    decimal.TryParse(info.RQFare, out rqFee);
                    decimal.TryParse(info.Fare, out seatPrice);
                    foreach (var item in model.SiteList)
                    {
                        if (item == site)
                        {
                            item.TaxFee = taxFee;
                            item.RQFee = rqFee;
                            item.SeatPrice = seatPrice;
                            item.TicketPrice = taxFee + rqFee + seatPrice;
                            item.Commission = Math.Floor(site.PolicyPoint / 100 * site.SeatPrice);
                            item.Discount = item.SeatPrice > 0 && item.SpecialYPrice > 0 ? Convert.ToInt32((item.SeatPrice / item.SpecialYPrice) * 100) : item.Discount;
                            item.IsGotSpecial = true;
                            item.IsReceivedSpecial = true;
                        }
                        sitelist.Add(item);
                    }
                    model.SiteList = sitelist;
                    model.IsSiteListChanged = true;

                    if (site == model.DefaultSite)
                    {
                        model.TaxFee = model.DefaultSite.TaxFee = taxFee;
                        model.RQFee = model.DefaultSite.RQFee = rqFee;
                        model.DefaultSite.SeatPrice = seatPrice;
                        model.DefaultSite.TicketPrice = taxFee + rqFee + seatPrice;
                        model.DefaultSite.Commission = Math.Floor(model.DefaultSite.PolicyPoint / 100 * model.DefaultSite.SeatPrice);
                        model.DefaultSite.Discount = model.DefaultSite.SeatPrice > 0 && model.DefaultSite.SpecialYPrice > 0 ? Convert.ToInt32((model.DefaultSite.SeatPrice / model.DefaultSite.SpecialYPrice) * 100) : model.DefaultSite.Discount;
                        model.DefaultSite.IsGotSpecial = true;
                        model.DefaultSite.IsReceivedSpecial = true;
                        if (model.IsOpened) model.IsOpened = false;//ui恢复
                        FlightInfoModels = FlightInfoModels;
                    }
                }
            }, UIManager.ShowErr)));
        }
        private bool CanExecuteGetSpecialFromSiteCommand(Site site)
        {
            return !IsBusy && site != null;
        }
        #endregion

        #region GetSpecialFromModelCommand

        private RelayCommand<FlightInfoModel> _getSpecialFromModelCommand;

        public RelayCommand<FlightInfoModel> GetSpecialFromModelCommand
        {
            get { return _getSpecialFromModelCommand ?? (new RelayCommand<FlightInfoModel>(ExecuteGetSpecialFromModelCommand, CanExecuteGetSpecialFromModelCommand)); }
        }

        private void ExecuteGetSpecialFromModelCommand(FlightInfoModel model)
        {
            if (model == null) return;
            DispatcherHelper.UIDispatcher.Invoke(new Action(() => CommunicateManager.Invoke<IPidService>(service =>
            {
                var pricelist = service.sendSSPat(LoginInfo.Code, model.CarrayCode, model.FlightNumber, model.DefaultSite.SeatCode,
                    model.StartDateTime.ToString("yyyy-MM-dd"), model.FromCityCode, model.ToCityCode, model.StartDateTime.ToString("HH:mm"),
                    model.ToDateTime.ToString("HH:mm"));
                if (!pricelist.Any())
                {
                    foreach (var m in model.SiteList.Where(m => m.CusNo == model.DefaultSite.CusNo))
                    {
                        m.IsGotSpecial = true;
                        m.IsReceivedSpecial = false;
                        break;
                    }
                    model.DefaultSite.IsGotSpecial = true;
                    model.DefaultSite.IsReceivedSpecial = false;
                }
                else
                {
                    var info = pricelist.FirstOrDefault();
                    if (info == null) return;
                    decimal taxFee;
                    decimal rqFee;
                    decimal seatPrice;
                    decimal.TryParse(info.TAX, out taxFee);
                    decimal.TryParse(info.RQFare, out rqFee);
                    decimal.TryParse(info.Fare, out seatPrice);
                    foreach (var m in model.SiteList.Where(m => m.CusNo == model.DefaultSite.CusNo))
                    {
                        m.TaxFee = taxFee;
                        m.RQFee = rqFee;
                        m.SeatPrice = seatPrice;
                        m.TicketPrice = taxFee + rqFee + seatPrice;
                        m.Commission = Math.Floor(m.PolicyPoint / 100 * m.SeatPrice);
                        m.Discount = m.SeatPrice > 0 && m.SpecialYPrice > 0 ? Convert.ToInt32((m.SeatPrice / m.SpecialYPrice) * 100) : m.Discount;
                        m.IsGotSpecial = true;
                        m.IsReceivedSpecial = true;
                        break;
                    }
                    model.TaxFee = model.DefaultSite.TaxFee = taxFee;
                    model.RQFee = model.DefaultSite.RQFee = rqFee;
                    model.DefaultSite.SeatPrice = seatPrice;
                    model.DefaultSite.TicketPrice = taxFee + rqFee + seatPrice;
                    model.DefaultSite.Commission = Math.Floor(model.DefaultSite.PolicyPoint / 100 * model.DefaultSite.SeatPrice);
                    model.DefaultSite.Discount = model.DefaultSite.SeatPrice > 0 && model.DefaultSite.SpecialYPrice > 0 ? Convert.ToInt32((model.DefaultSite.SeatPrice / model.DefaultSite.SpecialYPrice) * 100) : model.DefaultSite.Discount;
                    model.DefaultSite.IsGotSpecial = true;
                    model.DefaultSite.IsReceivedSpecial = true;
                }
                FlightInfoModels = FlightInfoModels;
            }, UIManager.ShowErr)));
        }

        private bool CanExecuteGetSpecialFromModelCommand(FlightInfoModel model)
        {
            return !IsBusy && model != null;
        }

        #endregion

        #region LeaveCommand

        private RelayCommand<FlightInfoModel> _leaveCommand;

        /// <summary>
        /// 执行鼠标离开popup命令
        /// </summary>
        public RelayCommand<FlightInfoModel> LeaveCommand
        {
            get
            {
                return _leaveCommand ?? (_leaveCommand = new RelayCommand<FlightInfoModel>(ExecuteLeaveCommand));
            }
        }

        private void ExecuteLeaveCommand(FlightInfoModel model)
        {
            if (model == null) return;
            if (model.IsSiteListChanged) model.IsSiteListChanged = false;
        }
        #endregion
        #endregion


        #region 公开属性

        public const string FlightInfoModelProtertyName = "FlightInfoModel";

        private FlightInfoModel _flightInfoModel;//去程机票

        public FlightInfoModel FlightInfoModel
        {
            get { return _flightInfoModel; }
            set
            {
                if (_flightInfoModel == value)
                {
                    return;
                }
                RaisePropertyChanging(FlightInfoModelProtertyName);
                _flightInfoModel = value;
                RaisePropertyChanged(FlightInfoModelProtertyName);
            }
        }
        public const string FlightInfoModelsProtertyName = "FlightInfoModels";

        private FlightInfoModel[] _flightInfoModels;//返程/联程机票集合

        public FlightInfoModel[] FlightInfoModels
        {
            get { return _flightInfoModels; }
            set
            {
                if (_flightInfoModels == value)
                {
                    return;
                }
                RaisePropertyChanging(FlightInfoModelsProtertyName);
                _flightInfoModels = value;
                RaisePropertyChanged(FlightInfoModelsProtertyName);
            }
        }

        public const string IsShowCommissionColumnProtertyName = "IsShowCommissionColumn";

        private Visibility _isShowCommissionColumn = Visibility.Visible;

        /// <summary>
        /// 是否显示佣金列
        /// </summary>
        public Visibility IsShowCommissionColumn
        {
            get { return _isShowCommissionColumn; }
            set
            {
                if (_isShowCommissionColumn == value)
                {
                    return;
                }
                RaisePropertyChanging(IsShowCommissionColumnProtertyName);
                _isShowCommissionColumn = value;
                RaisePropertyChanged(IsShowCommissionColumnProtertyName);
            }
        }
        #endregion


        #region 私有方法

        #endregion

    }
}
