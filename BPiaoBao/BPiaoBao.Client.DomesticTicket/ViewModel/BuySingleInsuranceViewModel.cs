using System;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using GalaSoft.MvvmLight.Threading;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public sealed class BuySingleInsuranceViewModel : BaseVM
    {

        #region 构造函数

        public BuySingleInsuranceViewModel()
        {
            if (IsInDesignMode) return;
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                #region 保险相关设置
                var re = service.GetCurentInsuranceCfgInfo();
                var leaveCount = service.GetCurentInsuranceCfgInfo(true).LeaveCount;
                if (re.IsOpenCurrenCarrierInsurance && re.IsOpenUnexpectedInsurance && leaveCount > 0)
                {
                    BuySingleInsuranceModel = new BuySingleInsuranceModel
                    {
                        FlightStartDate = DateTime.Now.Date,
                        BirthDay = DateTime.Now.AddYears(-16),
                        InsuranceCount = 1,
                        SumInsured = 400000,
                        IsAdultType = true,
                        Gender = true,
                        IsIdType = true,
                    };
                }
                else
                {
                    UIManager.ShowMessage("剩余保险数量不足");
                    IsDone = true;
                }
                #endregion
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        #endregion

        #region 公开属性
        #region IsDone

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

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

        #region BuySingleInsuranceModel

        private const string BuySingleInsuranceModelPropertyName = "BuySingleInsuranceModel";
        private BuySingleInsuranceModel _buySingleInsuranceModel;

        public BuySingleInsuranceModel BuySingleInsuranceModel
        {
            get { return _buySingleInsuranceModel; }
            set
            {
                if (_buySingleInsuranceModel == value) return;
                RaisePropertyChanging(BuySingleInsuranceModelPropertyName);
                _buySingleInsuranceModel = value;
                RaisePropertyChanged(BuySingleInsuranceModelPropertyName);
            }
        }

        #endregion
        #endregion

        #region 公开命令
        #region BuySingleInsuranceCommand

        private RelayCommand _buySingleInsuranceCommand;

        public RelayCommand BuySingleInsuranceCommand
        {
            get
            {
                return _buySingleInsuranceCommand ?? (_buySingleInsuranceCommand = new RelayCommand(ExecuteBuySingleInsuranceCommand, CanExecuteBuySingleInsuranceCommand));
            }
        }


        public void ExecuteBuySingleInsuranceCommand()
        {
            if (BuySingleInsuranceModel == null) return;
            if (!BuySingleInsuranceModel.Check()) return;
            if (BuySingleInsuranceModel.BirthDay == null) return;
            var rbim = new RequestBuyInsuranceManually
            {
                CardNo =
                    string.IsNullOrWhiteSpace(BuySingleInsuranceModel.IdNo) &&
                    (BuySingleInsuranceModel.IsBabyType || BuySingleInsuranceModel.IsChildType)
                        ? BuySingleInsuranceModel.BirthDay.Value.ToString("yyyy-MM-dd")
                        : BuySingleInsuranceModel.IdNo,
                Count = BuySingleInsuranceModel.InsuranceCount,
                FlightNumber = BuySingleInsuranceModel.FlightNumber
            };
            if (BuySingleInsuranceModel.IsIdType)
                rbim.IdType = EnumIDType.NormalId;
            else if (BuySingleInsuranceModel.IsMilitaryIdType)
                rbim.IdType = EnumIDType.MilitaryId;
            else if (BuySingleInsuranceModel.IsPassportIdType)
                rbim.IdType = EnumIDType.Passport;
            else if (BuySingleInsuranceModel.IsOtherType)
                rbim.IdType = EnumIDType.Other;
            if (BuySingleInsuranceModel.IsAdultType)
                rbim.PassengerType = EnumPassengerType.Adult;
            else if (BuySingleInsuranceModel.IsChildType)
                rbim.PassengerType = EnumPassengerType.Child;
            else if (BuySingleInsuranceModel.IsBabyType)
                rbim.PassengerType = EnumPassengerType.Baby;
            rbim.InsuranceLimitEndTime = BuySingleInsuranceModel.FlightStartDate.Date.AddDays(1).AddSeconds(-1);
            rbim.InsuranceLimitStartTime = BuySingleInsuranceModel.FlightStartDate.Date;
            rbim.StartDateTime = BuySingleInsuranceModel.FlightStartDate;
            rbim.InsuredName = BuySingleInsuranceModel.Name;
            rbim.Mobile = BuySingleInsuranceModel.Mobile;
            rbim.PNR = BuySingleInsuranceModel.PNR;
            rbim.SexType = BuySingleInsuranceModel.Gender ? EnumSexType.Male : EnumSexType.Female;
            rbim.Birth = BuySingleInsuranceModel.BirthDay;
            rbim.ToCityName = BuySingleInsuranceModel.ToCityName;
            IsBusy = true;
            Action action = () => CommunicateManager.Invoke<IInsuranceService>(service =>
            {
                service.BuyInsuranceManually(rbim);
                UIManager.ShowMessage("投保成功！");
                IsDone = true;
            }, UIManager.ShowErr);

            Task.Factory.StartNew(action).ContinueWith(task =>
            {
                Action setBusyAction = () => { IsBusy = false; };
                DispatcherHelper.UIDispatcher.Invoke(setBusyAction);
            });
        }

        private bool CanExecuteBuySingleInsuranceCommand()
        {
            return !IsBusy;
        }
        #endregion

        #endregion
    }
}
