using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Client.DomesticTicket.Model;
using BPiaoBao.Client.UIExt;
using BPiaoBao.Client.UIExt.Helper;
using JoveZhao.Framework;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class InsuranceDetailViewModel : BaseVM
    {
        public void Init(ResponseInsurance model)
        {
            if (model == null)
            {
                throw new CustomException(10001, "传入保单异常");
            }
            InitData(model);
        }

        /// <summary>
        /// 转换成本地数据
        /// </summary>
        /// <param name="model"></param>
        private void InitData(ResponseInsurance model)
        {
            InsuranceDetailModel = new InsuranceDetailModel
            {
                InsuranceNo = model.InsuranceNo,
                OrderId = model.OrderId,
                InsuranceCount = 1,
                SumInsured = model.PolicyAmount,
                BuyInsuranceStateText = EnumHelper.GetDescription(model.EnumInsuranceStatus),
                BuyInsuranceState = model.EnumInsuranceStatus,
                BuyInsuranceType = EnumHelper.GetDescription(model.InsureType),
                Name = model.PassengerName,
                Mobile = model.Mobile,
                Gender = model.SexType == Common.Enums.EnumSexType.Male,
                BirthDay = model.Birth
            };
            switch (model.IdType)
            {
                case Common.Enums.EnumIDType.NormalId:
                    InsuranceDetailModel.IsIdType = true;
                    break;
                case Common.Enums.EnumIDType.Passport:
                    InsuranceDetailModel.IsPassportIdType = true;
                    break;
                case Common.Enums.EnumIDType.MilitaryId:
                    InsuranceDetailModel.IsMilitaryIdType = true;
                    break;
                case Common.Enums.EnumIDType.BirthDate:
                    break;
                case Common.Enums.EnumIDType.Other:
                    InsuranceDetailModel.IsOtherType = true;
                    break;
            }
            switch (model.PassengerType)
            {
                case Common.Enums.EnumPassengerType.Adult:
                    InsuranceDetailModel.IsAdultType = true;
                    break;
                case Common.Enums.EnumPassengerType.Child:
                    InsuranceDetailModel.IsChildType = true;
                    break;
                case Common.Enums.EnumPassengerType.Baby:
                    InsuranceDetailModel.IsBabyType = true;
                    break;
            }
            InsuranceDetailModel.IdNo = model.CardNo;
            InsuranceDetailModel.CreateTime = model.BuyTime;
            InsuranceDetailModel.InsuranceStartTime = model.InsuranceLimitStartTime;
            InsuranceDetailModel.InsuracneEndTime = model.InsuranceLimitEndTime;
            InsuranceDetailModel.FlightNumber = model.FlightNumber;
            if (model.StartDateTime.HasValue) InsuranceDetailModel.FlightStartDate = model.StartDateTime.Value;
            InsuranceDetailModel.PNR = model.PNR;
            InsuranceDetailModel.ToCityName = model.ToCity;

        }

        #region InsuranceDetailModel

        private const string InsuranceDetailModelPropertyName = "InsuranceDetailModel";
        private InsuranceDetailModel _insuranceDetailModel;

        public InsuranceDetailModel InsuranceDetailModel
        {
            get { return _insuranceDetailModel; }
            set
            {
                if (_insuranceDetailModel == value) return;
                RaisePropertyChanging(InsuranceDetailModelPropertyName);
                _insuranceDetailModel = value;
                RaisePropertyChanged(InsuranceDetailModelPropertyName);
            }
        }

        #endregion

    }
}
