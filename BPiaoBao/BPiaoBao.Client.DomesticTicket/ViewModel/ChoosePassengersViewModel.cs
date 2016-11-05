using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// 选择购买航意险乘客视图模型
    /// </summary>
    public class ChoosePassengersViewModel : BaseVM
    {
        private PolicyDetailViewModel _vm;
        /// <summary>
        /// 默认保险份数为1份
        /// </summary>
        public int CountInsurance = 1;          
        #region 构造函数
        public ChoosePassengersViewModel(PolicyDetailViewModel vm)
        {
            if (vm == null) return;
            _vm = vm;
            if (vm.Order == null) return;
            switch (vm.Flag)
            {
                case 0://航意险
                    Passengers = vm.InPassers.Select(p => new ChoosePassengersModel
                    {
                        BuyInsuranceCount = p.BuyInsuranceCount,
                        CardNo = p.CardNo,
                        Id = p.Id,
                        IsBuyInsurance = p.IsBuyInsurance,
                        IsInsuranceRefund = p.IsInsuranceRefund,
                        IsSelected = p.IsBuyInsurance,
                        Mobile = p.Mobile,
                        PassengerName = p.PassengerName,
                        PassengerType = p.PassengerType,
                    }).ToList();
                    break;
                case 1://极速退
                    Passengers = vm.RePassers.Select(p => new ChoosePassengersModel
                    {
                        CardNo = p.CardNo,
                        Id = p.Id,
                        IsBuyInsurance = p.IsBuyInsurance,
                        IsInsuranceRefund = p.IsInsuranceRefund,
                        IsSelected = p.IsInsuranceRefund,
                        Mobile = p.Mobile,
                        PassengerName = p.PassengerName,
                        PassengerType = p.PassengerType,
                    }).ToList();
                    break;
            }
        } 
        #endregion
        #region 公开属性
        #region Passengers

        /// <summary>
        /// The <see /> property's name.
        /// </summary>
        private const string PassengersPropertyName = "Passengers";

        private List<ChoosePassengersModel> _passengers;

        /// <summary>
        /// 乘客列表
        /// </summary>
        public List<ChoosePassengersModel> Passengers
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
        #region IsDone

        /// <summary>
        /// The <see cref="IsDone" /> property's name.
        /// </summary>
        private const string IsDonePropertyName = "IsDone";

        private bool _isDone;

        /// <summary>
        /// 是否完成
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
        #endregion
        #region 公开命令
        #region SelectCommand

        private RelayCommand<ChoosePassengersModel> _selectCommand;

        /// <summary>
        /// 执行选择命令
        /// </summary>
        public RelayCommand<ChoosePassengersModel> SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand = new RelayCommand<ChoosePassengersModel>(ExecuteSelectCommand));
            }
        }

        private void ExecuteSelectCommand(ChoosePassengersModel model)
        {

            switch (_vm.Flag)
            {
                case 0:
                    if (model.BuyInsuranceCount > 0)
                    {
                        model.BuyInsuranceCount = 0;
                        model.IsBuyInsurance = false;
                    }
                    else
                    {
                        LocalUIManager.ChooseInsuranceCount(this);
                        model.BuyInsuranceCount = CountInsurance;
                        model.IsBuyInsurance = true;
                    }
                    break;
                case 1:
                    model.IsInsuranceRefund = model.IsInsuranceRefund != true;
                    break;
            }
        }

        #endregion
        #region UnSelectCommand

        private RelayCommand<ChoosePassengersModel> _unselectCommand;

        /// <summary>
        /// 执行取消选择命令
        /// </summary>
        public RelayCommand<ChoosePassengersModel> UnSelectCommand
        {
            get
            {
                return _unselectCommand ?? (_unselectCommand = new RelayCommand<ChoosePassengersModel>(ExecuteUnSelectCommand));
            }
        }

        private void ExecuteUnSelectCommand(ChoosePassengersModel model)
        {
            switch (_vm.Flag)
            {
                case 0:
                    model.BuyInsuranceCount = 0;
                    model.IsBuyInsurance = false;
                    break;
                case 1:
                    model.IsInsuranceRefund = false;
                    break;
            }
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
            switch (_vm.Flag)
            {
                case 0:
                    foreach (var item in _vm.InPassers)
                    {
                        foreach (var temp in Passengers)
                        {
                            if (item.Id != temp.Id) continue;
                            item.IsBuyInsurance = temp.IsBuyInsurance;
                            item.BuyInsuranceCount = temp.BuyInsuranceCount;
                        }
                    }
                    var allcount = _vm.InPassers.Where(p => p.IsBuyInsurance).Sum(p => p.BuyInsuranceCount);
                    if (allcount > _vm.InsuranceLeaveCount)
                    {
                        UIManager.ShowMessage("当前运营商保险份数不足" + allcount + "份");
                        return;
                    }
                    IsDone = true;
                    break;
                case 1:
                    foreach (var item in _vm.RePassers)
                    {
                        foreach (var temp in Passengers)
                        {
                            if (item.Id == temp.Id)
                            {
                                item.IsInsuranceRefund = temp.IsInsuranceRefund;
                            }
                        }
                    }
                    IsDone = true;
                    break;
            }
        }

        #endregion
        #region CancelCommand

        private RelayCommand _cancelCommand;

        /// <summary>
        /// 执行取消命令
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(ExecuteCancelCommand));
            }
        }

        private void ExecuteCancelCommand()
        {

        }

        #endregion
        #endregion
    }
}
