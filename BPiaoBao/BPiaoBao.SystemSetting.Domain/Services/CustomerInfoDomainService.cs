using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Services
{
    public class CustomerInfoDomainService : BaseDomainService
    {
        private const int _errorCode = 200001;
        private const string _stationRecordCode = "station";

        private readonly ICustomerInfoRepository _customerInfoRepository;
        private readonly IBusinessmanRepository _businessmanRepository;

        #region ctors

        //构造函数
        public CustomerInfoDomainService(ICustomerInfoRepository customerInfoRepository, IBusinessmanRepository businessmanRepository)
        {
            _customerInfoRepository = customerInfoRepository;
            _businessmanRepository = businessmanRepository;
        }

        #endregion

        #region private methods

        //根据运营商Code获取运营商信息
        private Carrier GetCarrierInfo(string carrierCode)
        {
            Carrier carrierInfo = null;
            carrierInfo = _businessmanRepository.FindAll(q => q.Code == carrierCode).OfType<Carrier>().FirstOrDefault();
            if (carrierInfo == null)
            {
                throw new CustomException(_errorCode, "运营商编号" + carrierCode + "不存在。");
            }
            return carrierInfo;
        }
        //根据运营商Code获取服务中心信息
        private CustomerInfo GetCarrierCustomerInfo(string carrierCode)
        {
            return _customerInfoRepository.FindAll(q => q.CarrierCode == carrierCode).FirstOrDefault();
        }
        //获取控台服务中心信息
        private CustomerInfo GetStationCustomerInfo()
        {
            return GetCarrierCustomerInfo(_stationRecordCode);
        }

        #endregion

        //设置运营商客服中心内容
        public void SetCarrierCustomerInfo(CustomerInfo info)
        {
            #region 数据验证
            //CarrierCode必须存在
            if (string.IsNullOrWhiteSpace(info.CarrierCode))
            {
                throw new CustomException(_errorCode, "运营商编号不可为空。");
            }
            if (info.CarrierCode != _stationRecordCode
                && _businessmanRepository.FindAll(q => q.Code == info.CarrierCode).OfType<Carrier>().Count() == 0)
            {
                throw new CustomException(_errorCode, "不存在运营商编号" + info.CarrierCode);
            }
            //qq必须合法
            //string qqExpression = StringExpend.QQExpression;
            if (info.AdvisoryQQ != null)
            {
                foreach (var data in info.AdvisoryQQ)
                {
                    if (!data.QQ.IsMatch(StringExpend.QQPattern))
                    {
                        throw new CustomException(_errorCode, data.QQ + "不是合法的QQ。");
                    }
                }
            }

            //电话必须合法
            if (info.HotlinePhone != null)
            {
                foreach (var data in info.HotlinePhone)
                {
                    if(!data.Phone.IsMatch(StringExpend.PhonePattern))
                    {
                        throw new CustomException(_errorCode,data.Phone+"不是合法的电话号码");
                    }
                }
            }

            #endregion

            //删除原有数据
            var orgData = _customerInfoRepository.FindAll(q => q.CarrierCode == info.CarrierCode);
            if (orgData != null)
            {
                foreach (var data in orgData)
                {
                    data.AdvisoryQQ.Clear();
                    data.HotlinePhone.Clear();
                    _unitOfWorkRepository.PersistDeletionOf(data);
                }
            }

            //添加新的数据
            _unitOfWorkRepository.PersistCreationOf(info);

            _unitOfWork.Commit();
        }
        /// <summary>
        /// 获取客服中心内容
        /// </summary>
        /// <param name="useSet">是否使用配置规则读取客服中心内容</param>
        /// <param name="carrierCode">运营商Code，在不使用运营商配置规则时，该字段为null则读取控台客服中心信息,如果使用配置规则，则该字段不可为空</param>
        /// <returns></returns>
        public CustomerInfo GetCustomerInfo(bool useSet, string carrierCode)
        {
            Carrier carrierInfo = null;
            //如果使用配置规则
            if (useSet)
            {
                //获得运营上信息
                if (string.IsNullOrWhiteSpace(carrierCode))
                {
                    throw new CustomException(_errorCode, "运营商编号不可为空。");
                }
                carrierInfo = this.GetCarrierInfo(carrierCode);
                //根据配置信息读取信息
                if (carrierInfo.ShowLocalCSCSwich)
                {
                    return this.GetCarrierCustomerInfo(carrierCode);
                }
                else
                {
                    return this.GetStationCustomerInfo();
                }
            }
            //如果不是用配置规则
            else
            {
                //如果传入了运营商Code
                if (!string.IsNullOrWhiteSpace(carrierCode))
                {
                    return this.GetCarrierCustomerInfo(carrierCode);
                }
                //如果未传入运营商Code
                else
                {
                    return this.GetStationCustomerInfo();
                }
            }
        }
        //设置控台客服中心内容
        public void SetStationCustomerInfo(CustomerInfo info)
        {
            info.CarrierCode = _stationRecordCode;
            SetCarrierCustomerInfo(info);
        }
    }
}
