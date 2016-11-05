using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using PnrAnalysis;
using StructureMap;
using InsuranceConfig = BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject.InsuranceConfigData;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class InsuranceService : BaseService, IInsuranceService, IConsoInsuranceService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        private IInsuranceOrderRepository _iInsuranceOrderRepository;
        private IInsuranceConfigRepository _iInsuranceConfigRepository;
        private IInsurancePurchaseByBussinessmanRepository _iInsurancePurchaseByBussinessmanRepository;
        private IInsuranceDepositLogRepository _iInsuranceDepositLogRepository;
        private IBusinessmanRepository _iBusinessmanRepository;
        private IOrderRepository _iOrderRepository;
        private IPaymentClientProxy _iPayMentClientProxy;
        private string _code = AuthManager.GetCurrentUser().Code;
        private string _bussinessName = AuthManager.GetCurrentUser().BusinessmanName;
        private string _carrierCode = AuthManager.GetCurrentUser().CarrierCode;
        public InsuranceService(IInsuranceOrderRepository iInsuranceOrderRepository, IInsuranceConfigRepository iInsuranceConfigRepository, IInsurancePurchaseByBussinessmanRepository _iInsurancePurchaseByBussinessmanRepository, IInsuranceDepositLogRepository _iInsuranceDepositLogRepository, IBusinessmanRepository iBusinessmanRepository, IOrderRepository iOrderRepository, IPaymentClientProxy iPayMentClientProxy)
        {
            this._iInsuranceOrderRepository = iInsuranceOrderRepository;
            this._iInsuranceConfigRepository = iInsuranceConfigRepository;
            this._iBusinessmanRepository = iBusinessmanRepository;
            this._iOrderRepository = iOrderRepository;
            this._iPayMentClientProxy = iPayMentClientProxy;
            this._iInsurancePurchaseByBussinessmanRepository = _iInsurancePurchaseByBussinessmanRepository;
            this._iInsuranceDepositLogRepository = _iInsuranceDepositLogRepository;
        }


        /// <summary>
        /// 获取保险配置
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("获取保险配置")]
        public CtrlInsuranceDto GetCtrlInsurance()
        {
            var model = InsuranceSection.GetInsuranceConfigurationSection();
            if (model == null)
            {
                return null;
            }

            var m = new CtrlInsuranceDto { IsEnabled = model.CtrlInsuranceCollection.IsEnabled, SinglePrice = model.CtrlInsuranceCollection.SinglePrice, CtrlInsurance = new List<CtrlInsuranceConfig>() };
            foreach (var cfg in from InsuranceElement ctrl in model.CtrlInsuranceCollection
                                select new CtrlInsuranceConfig
                                {
                                    LeaveCount = ctrl.LeaveCount,
                                    IsCurrent = ctrl.IsCurrent,
                                    Value = ctrl.Value
                                })
            {
                m.CtrlInsurance.Add(cfg);
            }
            return m;
        }

        /// <summary>
        /// 获得急速退配置
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("获得急速退配置")]
        public CtrlInsuranceRefundDto GetCtrlInsuranceRefund()
        {
            var model = InsuranceSection.GetInsuranceConfigurationSection();
            if (model == null)
            {
                return null;
            }
            var m = new CtrlInsuranceRefundDto();
            m.IsEnabled = model.InsuranceRefund.IsEnabled;
            m.SinglePrice = model.InsuranceRefund.SinglePrice;
            return m;
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="dto"></param>
        [ExtOperationInterceptor("保存配置信息")]
        public void SaveCtrlConfig(CtrlInsuranceDto dto)
        {
            var model = InsuranceSection.GetInsuranceConfigurationSection();
            if (model == null || model.CtrlInsuranceCollection == null) return;
            model.CtrlInsuranceCollection.IsEnabled = dto.IsEnabled;
            model.CtrlInsuranceCollection.SinglePrice = dto.SinglePrice;
            foreach (InsuranceElement ctrl in model.CtrlInsuranceCollection)
            {
                foreach (var m in dto.CtrlInsurance.Where(m => ctrl.Value.Trim().Equals(m.Value, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ctrl.IsCurrent = m.IsCurrent;
                    ctrl.LeaveCount = m.LeaveCount;
                }
            }
            InsuranceSection.Save();
        }

        /// <summary>
        /// 保存控台急速退配置信息
        /// </summary>
        /// <param name="dto"></param>
        [ExtOperationInterceptor("保存控台急速退配置信息")]
        public void SaveCtrlRefundConfig(CtrlInsuranceRefundDto dto)
        {
            var model = InsuranceSection.GetInsuranceConfigurationSection();
            if (model == null || model.InsuranceRefund == null) return;
            model.InsuranceRefund.IsEnabled = dto.IsEnabled;
            model.InsuranceRefund.SinglePrice = dto.SinglePrice;
            InsuranceSection.Save();
        }

        /// <summary>
        /// 查询保险记录
        /// </summary>
        /// <param name="qInsurance"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询保险记录")]
        public DataPack<ResponseInsurance> QueryInsurance(RequestQueryInsurance qInsurance, int pageIndex, int pageSize = 20)
        {
            pageIndex = (pageIndex - 1) * pageSize;
            var query = this._iInsuranceOrderRepository.getAllSearchItem();//this._iInsuranceOrderRepository.FindAll();
            if (qInsurance != null)
            {
                //是否是控台调用
                if (qInsurance.IsCtrlStationCall)
                {
                    //如果是控台调用，则筛选指定的运营商
                    if (!string.IsNullOrWhiteSpace(qInsurance.Code))
                    {
                        query = query.Where(p => p.CarrierCode == qInsurance.Code);
                    }
                }
                //是否是分销商
                else if (qInsurance.IsClientCall)
                {
                    query = query.Where(p => p.BussinessmanCode == _code);
                }
                //否则，仅显示当前运营商
                else
                {
                    query = query.Where(p => p.CarrierCode == _code);
                }

                //保单号
                if (!string.IsNullOrWhiteSpace(qInsurance.InsuranceNo))
                {
                    query = query.Where(p => p.InsuranceNo == qInsurance.InsuranceNo);
                }
                //机票订单号
                if (!string.IsNullOrWhiteSpace(qInsurance.OrderId))
                {
                    query = query.Where(p => p.OrderId == qInsurance.OrderId);
                }
                //乘客姓名
                if (!string.IsNullOrWhiteSpace(qInsurance.PassengerName))
                {
                    query = query.Where(p => p.PassengerName == qInsurance.PassengerName);
                }
                //电话
                if (!string.IsNullOrWhiteSpace(qInsurance.Mobile))
                {
                    query = query.Where(p => p.Mobile == qInsurance.Mobile);
                }
                //订单状态
                if (qInsurance.EnumInsuranceStatus != null)
                {
                    query = query.Where(p => p.EnumInsuranceStatus == qInsurance.EnumInsuranceStatus);
                }
                //购买开始时间
                if (qInsurance.BuyStartTime != null)
                {
                    query = query.Where(p => p.BuyTime >= qInsurance.BuyStartTime.Value);
                }
                //购买结束时间
                if (qInsurance.BuyEndTime != null)
                {
                    var date = qInsurance.BuyEndTime.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(p => p.BuyTime <= date);
                }
                //起飞开始时间
                if (qInsurance.FlyStartTime != null)
                {
                    query = query.Where(p => p.StartDateTime >= qInsurance.FlyStartTime.Value);
                }
                //起飞结束时间
                if (qInsurance.FlyEndTime != null)
                {
                    var date = qInsurance.FlyEndTime.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(p => p.StartDateTime <= date);
                }
                //航程起点
                if (!string.IsNullOrWhiteSpace(qInsurance.FlightTripFrom))
                {
                    //var cityCode = ExtHelper.GetCityCodeByName(qInsurance.FlightTripFrom);
                    query = query.Where(p => p.FromCity == qInsurance.FlightTripFrom);
                }
                //航程终点
                if (!string.IsNullOrWhiteSpace(qInsurance.FlightTripTo))
                {
                    //var cityCode = ExtHelper.GetCityCodeByName(qInsurance.FlightTripTo);
                    query = query.Where(p => p.ToCity == qInsurance.FlightTripTo);
                }
                //保险生效起始时间
                if (qInsurance.InsuranceLimitStartTime != null)
                {
                    query = query.Where(p => p.InsuranceLimitStartTime >= qInsurance.InsuranceLimitStartTime.Value);
                }
                //保险生效结束时间
                if (qInsurance.InsuranceLimitEndTime != null)
                {
                    var date = qInsurance.InsuranceLimitEndTime.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(p => p.InsuranceLimitEndTime <= date);
                }
                //保险公司
                if (!string.IsNullOrWhiteSpace(qInsurance.InsuranceCompany))
                {
                    query = query.Where(p => p.InsuranceCompany == qInsurance.InsuranceCompany);
                }
                //流水号
                if (!string.IsNullOrWhiteSpace(qInsurance.PayNo))
                {
                    query = query.Where(p => p.SerialNum == qInsurance.PayNo);
                }
                //证件号
                if (!string.IsNullOrWhiteSpace(qInsurance.CardNo))
                {
                    query = query.Where(p => p.CardNo == qInsurance.CardNo);
                }
                //if(!string.IsNullOrWhiteSpace(qInsurance))
            }
            //var x = query.AsStreaming().ToString();
            query = query.OrderByDescending(p => p.BuyTime);
            var total = query.Count();
            query = query.Skip(pageIndex).Take(pageSize);

            List<ResponseInsurance> listinsurance = new List<ResponseInsurance>();
            query.ToList().ForEach(p => listinsurance.Add(AutoMapper.Mapper.Map<InsuranceSearchRecord, ResponseInsurance>(p)));
            var data = new DataPack<ResponseInsurance>
            {
                TotalCount = total,
                List = listinsurance
            };
            return data;

        }

        /// <summary>
        /// 查询运营商保险批发记录
        /// </summary>
        /// <param name="qInsuranceDepositLog"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询运营商保险批发记录")]
        public DataPack<ResponseInsuranceDepositLog> QueryInsuranceDepositLog(RequestQueryInsuranceDepositLog qInsuranceDepositLog, int pageIndex, int pageSize = 20)
        {
            pageIndex = (pageIndex - 1) * pageSize;
            IQueryable<InsuranceDepositLog> query;
            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            //查询参数
            string bussinessCode = null;
            DateTime? buyStartTime = null;
            DateTime? buyEndTime = null;
            EnumInsuranceDepositType? insuranceDepositType = null;
            string payNo = null;
            string tradeId = null;
            //获得查询参数
            if (qInsuranceDepositLog != null)
            {
                if (qInsuranceDepositLog.IsCtrlStationCall)
                {
                    if (!string.IsNullOrWhiteSpace(qInsuranceDepositLog.Code))
                    {
                        bussinessCode = qInsuranceDepositLog.Code;
                    }
                }
                else
                {
                    bussinessCode = _code;
                }
                buyStartTime = qInsuranceDepositLog.BuyStartTime;
                buyEndTime = qInsuranceDepositLog.BuyEndTime;
                insuranceDepositType = qInsuranceDepositLog.InsuranceDepositType;
                payNo = qInsuranceDepositLog.PayNo;
                tradeId = qInsuranceDepositLog.TradeId;
            }
            //查询逻辑
            query = insuranceDomainService.QueryInsuranceDepositLog(bussinessCode, payNo, tradeId, buyStartTime, buyEndTime, insuranceDepositType);
            //排序
            query = query.OrderByDescending(p => p.BuyTime);
            //获取总数
            var total = query.Count();
            //分页
            query = query.Skip(pageIndex).Take(pageSize);
            //数据转换
            var data = new DataPack<ResponseInsuranceDepositLog>
            {
                TotalCount = total,
                List = AutoMapper.Mapper.Map<List<InsuranceDepositLog>, List<ResponseInsuranceDepositLog>>(query.ToList())
            };

            return data;

        }

        /// <summary>
        /// 查询保险配置
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("查询保险配置")]
        public InsuranceConfigData QueryInsuranceConfig()
        {

            var model = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == _code).FirstOrDefault();
            var m = AutoMapper.Mapper.Map<BPiaoBao.DomesticTicket.Domain.Models.Insurance.InsuranceConfig, InsuranceConfigData>(model);
            return m;

        }

        /// <summary>
        /// 保存保险配置
        /// </summary>
        /// <param name="req"></param>
        [ExtOperationInterceptor("保存保险配置")]
        public void SaveInsuranceConfig(InsuranceConfigData req)
        {
            SaveInsuranceConfigNoCommit(req);
            this.unitOfWork.Commit();
        }

        /// <summary>
        /// 保存运营商保险配置（没有提交动作）
        /// </summary>
        /// <param name="req"></param>
        [ExtOperationInterceptor("保存运营商保险配置（没有提交动作）")]
        private void SaveInsuranceConfigNoCommit(InsuranceConfigData req)
        {
            var model = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode.Equals(_code)).FirstOrDefault();
            if (model != null)
            {
                model.IsOpen = req.IsOpen;
                model.SinglePrice = req.SinglePrice;
                model.LeaveCount = req.LeaveCount;
                this.unitOfWorkRepository.PersistUpdateOf(model);
            }
            else
            {
                var builder = AggregationFactory.CreateBuiler<InsuranceConfigBuilder>();
                var m = builder.CreateInsuranceConfig();
                m.BusinessmanCode = _code;
                m.BusinessmanName = _bussinessName;
                m.IsOpen = req.IsOpen;
                m.SinglePrice = req.SinglePrice;
                m.LeaveCount = req.LeaveCount;
                this.unitOfWorkRepository.PersistCreationOf(m);
            }
        }
        [ExtOperationInterceptor("保存保单信息")]
        public void SaveInsurance(RequestInsurance req)
        {
            //request不能为空
            if (req == null)
            {
                throw new CustomException(111113, "传入保单信息为空");
            }

            //构建保险订单数据结构
            var insuranceOrderBuilder = AggregationFactory.CreateBuiler<InsuranceOrderBuilder>();
            var insuranceOrder = insuranceOrderBuilder.CreateInsuranceOrder();
            insuranceOrder.OrderId = req.OrderId;
            insuranceOrder.InsuranceRecords = new List<InsuranceRecord>();
            var order = this._iOrderRepository.FindAll(p => p.OrderId == req.OrderId).FirstOrDefault();

            //填写Passenger信息
            foreach (var passer in order.Passengers)
            {
                foreach (var passengerDto in req.UnexpectedPassenger)
                {
                    if (passengerDto.Id == passer.Id)
                    {
                        passer.PassengerType = passengerDto.PassengerType;
                        passer.SexType = passengerDto.SexType;
                        passer.Birth = passengerDto.Birth;
                        passer.IdType = passengerDto.IdType;
                        passer.CardNo = passengerDto.CardNo;
                        //极速退处理
                        if (passengerDto.IsInsuranceRefund)
                        {
                            passer.IsInsuranceRefund = passengerDto.IsInsuranceRefund;
                            passer.InsuranceRefunrPrice = passengerDto.InsuranceRefunrPrice;
                        }
                    }
                }
            }

            InsuranceRecordBuilder insuranceRecordBuilder = new InsuranceRecordBuilder();
            if (req.BuyInsuranceAllCount > 0)
            {
                foreach (var passer in req.UnexpectedPassenger)
                {
                    foreach (var skyway in passer.SkyWays)
                    {
                        if (skyway.InsuranceCount > 0)
                        {
                            InsuranceRecord record = insuranceRecordBuilder.CreateInsuranceRecord();
                            record.Count = skyway.InsuranceCount;
                            record.SkyWayId = skyway.SkyWayId;
                            record.PassengerId = passer.Id;

                            insuranceOrder.InsuranceRecords.Add(record);
                        }
                    }
                }

                if (insuranceOrder.InsuranceRecords.Count > 0)
                {
                    var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
                    insuranceDomainService.SaveInsurance(insuranceOrder, AuthManager.GetCurrentUser());
                }
            }


            this.unitOfWork.Commit();
        }

        /// <summary>
        /// 运营商，现金购买保险
        /// </summary>
        /// <param name="buyCount"></param>
        /// <param name="pwd"></param>
        [ExtOperationInterceptor("运营商，现金购买保险")]
        public void BuyInsuranceByCashByCarrier(int buyCount, string pwd)
        {
            //获取航意险配置信息
            var cfg = this.GetCtrlInsurance();
            try
            {
                var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
                insuranceDomainService.BuyInsuranceByCashByCarrier(buyCount, pwd, _code, _bussinessName, AuthManager.GetCurrentUser().OperatorAccount, AuthManager.GetCurrentUser().OperatorName, AuthManager.GetCurrentUser());

                this.unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new CustomException(10012, e.Message);
            }
        }

        /// <summary>
        /// 现金账户支付，分销商购买调用
        /// 机票订单id，
        /// </summary>
        /// <param name="buyCount"></param>
        /// <param name="pwd"></param>
        /// <param name="payType">0：现金账户；1：信用卡账户</param>
        [ExtOperationInterceptor("现金账户支付，分销商购买调用")]
        public void BuyInsuranceByCashOrCredit(string OrderId, List<PassengerDto> passengerDtos, int payType, string pwd)
        {
            //构建保险订单数据结构
            var insuranceOrderBuilder = AggregationFactory.CreateBuiler<InsuranceOrderBuilder>();
            var insuranceOrder = insuranceOrderBuilder.CreateInsuranceOrder();
            insuranceOrder.OrderId = OrderId;
            insuranceOrder.InsuranceRecords = new List<InsuranceRecord>();

            var order = this._iOrderRepository.FindAll(p => p.OrderId == OrderId).FirstOrDefault();
            //填写Passenger信息
            foreach (var passer in order.Passengers)
            {
                foreach (var passengerDto in passengerDtos)
                {
                    if (passengerDto.Id == passer.Id)
                    {
                        passer.PassengerType = passengerDto.PassengerType;
                        passer.SexType = passengerDto.SexType;
                        passer.Birth = passengerDto.Birth;
                        passer.IdType = passengerDto.IdType;
                        passer.CardNo = passengerDto.CardNo;
                    }
                }
            }

            //转换请求数据结构
            InsuranceRecordBuilder insuranceRecordBuilder = new InsuranceRecordBuilder();
            foreach (var passer in passengerDtos)
            {
                foreach (var skyway in passer.SkyWays)
                {
                    if (skyway.InsuranceCount > 0)
                    {
                        InsuranceRecord record = insuranceRecordBuilder.CreateInsuranceRecord();
                        record.Count = skyway.InsuranceCount;
                        record.SkyWayId = skyway.SkyWayId;
                        record.PassengerId = passer.Id;
                        insuranceOrder.InsuranceRecords.Add(record);
                    }
                }
            }
            //如果没有需要申请的保险，则终止请求
            if (insuranceOrder.InsuranceRecords.Count == 0)
                return;

            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();

            insuranceDomainService.SaveInsurance(insuranceOrder, AuthManager.GetCurrentUser(), true);

            this.unitOfWork.Commit();
        }
        /// <summary>
        /// 查询当前的保险配置
        /// </summary>
        /// <param name="isBuyer">true则是分销商的，false则是运营商的</param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询当前的保险配置")]
        public ResponseCurrentCarrierInsurance GetCurentInsuranceCfgInfo(bool isBuyer = false)
        {
            var m = new ResponseCurrentCarrierInsurance();
            var unCfg = this.GetCtrlInsurance();
            var reCfg = this.GetCtrlInsuranceRefund();
            m.IsOpenUnexpectedInsurance = unCfg.IsEnabled;//航意险是否启用
            m.IsOpenRefundInsurance = reCfg.IsEnabled;//急速退险是否启用

            if (m.IsOpenUnexpectedInsurance)
            {
                m.InsuranceCompany = unCfg.CtrlInsurance.Where(c => c.IsCurrent).Select(c => c.Value).FirstOrDefault();
            }
            if (m.IsOpenRefundInsurance)
            {
                m.RefundSinglePrice = reCfg.SinglePrice;
            }
            string code = isBuyer ? _code : _carrierCode;
            var model = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == code).FirstOrDefault();
            var ins = AutoMapper.Mapper.Map<BPiaoBao.DomesticTicket.Domain.Models.Insurance.InsuranceConfig, InsuranceConfigData>(model);
            if (ins != null)
            {
                m.IsOpenCurrenCarrierInsurance = ins.IsOpen;//当前保险开关是否打开
                m.LeaveCount = ins.LeaveCount;
                m.UnexpectedSinglePrice = ins.SinglePrice;
            }
            return m;
        }

        /// <summary>
        /// 分销商查询保险购买记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("分销商查询保险购买记录")]
        public DataPack<ResponseInsurancePurchaseByBussinessman> QueryInsurancePurchaseByBussinessman(RequestQueryInsurancePurchaseByBussinessman request, int pageIndex, int pageSize = 20)
        {
            if (request == null)
            {
                throw new CustomException(10004, "请求不可为空。");
            }
            pageIndex = (pageIndex - 1) * pageSize;
            string buyerCode = request.BuyerCode;
            string carrierCode = request.CarrierCode;
            //实例化领域服务
            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            IQueryable<InsurancePurchaseByBussinessman> result;
            switch (request.RequestFrom)
            {
                case 0://买票宝
                    //查询
                    buyerCode = AuthManager.GetCurrentUser().Code;
                    break;
                case 1://卖票宝
                    carrierCode = AuthManager.GetCurrentUser().Code;
                    break;
                case 2://控台
                    break;
                default: throw new CustomException(10001, "非法客户端编号。");
            }
            result = insuranceDomainService.QueryInsurancePurchaseByBussinessman(
                        request.BuyStartTime,
                        request.BuyEndTime,
                        request.RecordType,
                        request.TradeNo,
                        request.PayNo,
                        buyerCode,
                        carrierCode,
                        request.PayStatus);
            //排序
            result = result.OrderByDescending(p => p.BuyTime);
            //获取总数
            var total = result.Count();
            //分页
            result = result.Skip(pageIndex).Take(pageSize);
            //数据转换
            var data = new DataPack<ResponseInsurancePurchaseByBussinessman>
            {
                TotalCount = total,
                List = AutoMapper.Mapper.Map<List<InsurancePurchaseByBussinessman>, List<ResponseInsurancePurchaseByBussinessman>>(result.ToList())
            };

            return data;
        }
        /// <summary>
        /// 从运营商处购买保险
        /// </summary>
        /// <param name="request"></param>
        [ExtOperationInterceptor("从运营商处购买保险")]
        public void PurchaseInsuranceFromCarrier(RequestPurchaseFromCarrier request)
        {
            if (request == null)
            {
                throw new CustomException(10004, "请求不可为空。");
            }
            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            insuranceDomainService.PurchaseInsuranceFromCarrier(request.buyCount, request.pwd, request.payMethod, AuthManager.GetCurrentUser().Code, AuthManager.GetCurrentUser().BusinessmanName, AuthManager.GetCurrentUser().CarrierCode, AuthManager.GetCurrentUser().OperatorAccount, AuthManager.GetCurrentUser().OperatorName, AuthManager.GetCurrentUser());

            this.unitOfWork.Commit();
        }
        /// <summary>
        /// 向分销商赠送保险
        /// </summary>
        /// <param name="request"></param>
        [ExtOperationInterceptor("向分销商赠送保险")]
        public void OfferInsuranceToBuyer(RequestOfferInsuranceToBuyer request)
        {
            if (request == null)
            {
                throw new CustomException(10004, "请求不可为空。");
            }
            var buyer = this._iBusinessmanRepository.FindAll(m => m.Code == request.BuyerCode).FirstOrDefault();
            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            insuranceDomainService.PurchaseInsuranceFromCarrier(
                request.OfferCount,
                "",
                EnumPayMethod.Account,
                request.BuyerCode,
                buyer == null ? "" : buyer.Name,
                AuthManager.GetCurrentUser().Code,
                AuthManager.GetCurrentUser().OperatorAccount,
                AuthManager.GetCurrentUser().OperatorName,
                null,
                request.Remark);

            this.unitOfWork.Commit();
        }
        /// <summary>
        /// 向运营商赠送保险
        /// </summary>
        /// <param name="request"></param>
        [ExtOperationInterceptor("向运营商赠送保险")]
        public void OfferInsuranceToCarrier(RequestOfferInsuranceToCarrier request)
        {
            if (request == null)
            {
                throw new CustomException(10004, "请求不可为空。");
            }
            var carrier = this._iBusinessmanRepository.FindAll(m => m.Code == request.CarrierCode).FirstOrDefault();
            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            insuranceDomainService.BuyInsuranceByCashByCarrier(request.OfferCount, "", request.CarrierCode, carrier.Code, AuthManager.GetCurrentUser().OperatorAccount, AuthManager.GetCurrentUser().OperatorName, null, request.Remark);

            this.unitOfWork.Commit();
        }
        /// <summary>
        /// 手动投保
        /// </summary>
        /// <param name="request"></param>
        [ExtOperationInterceptor("手动投保")]
        public void BuyInsuranceManually(RequestBuyInsuranceManually request)
        {
            if (request == null)
            {
                throw new CustomException(10004, "请求不可为空。");
            }
            //构建保险订单数据结构
            var insuranceOrderBuilder = AggregationFactory.CreateBuiler<InsuranceOrderBuilder>();
            var insuranceOrder = insuranceOrderBuilder.CreateInsuranceOrder();
            insuranceOrder.InsuranceRecords = new List<InsuranceRecord>();
            //转换请求数据结构
            InsuranceRecordBuilder insuranceRecordBuilder = new InsuranceRecordBuilder();
            InsuranceRecord record = insuranceRecordBuilder.CreateInsuranceRecord();

            record.Count = request.Count;
            record.FlightNumber = request.FlightNumber;
            record.IdType = request.IdType;
            if (record.IdType == EnumIDType.BirthDate
                && request.Birth != null)
            {
                record.CardNo = request.Birth.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                record.CardNo = request.CardNo;
            }
            record.InsuranceLimitEndTime = request.InsuranceLimitEndTime;
            record.InsuranceLimitStartTime = request.InsuranceLimitStartTime;
            record.InsuredName = request.InsuredName;
            record.Mobile = request.Mobile;
            record.PNR = request.PNR;
            record.SexType = request.SexType;
            record.ToCityName = request.ToCityName;
            record.InsureType = EnumInsureMethod.Manually;
            record.PassengerType = request.PassengerType;
            record.Birth = request.Birth;
            record.StartDateTime = request.StartDateTime;

            insuranceOrder.InsuranceRecords.Add(record);

            //如果没有需要申请的保险，则终止请求
            if (insuranceOrder.InsuranceRecords.Count == 0)
                return;

            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();

            insuranceDomainService.SaveInsurance(insuranceOrder, AuthManager.GetCurrentUser(), true, false);

            this.unitOfWork.Commit();
            //根据request创建保险记录
        }
        /// <summary>
        /// 删除保险
        /// </summary>
        /// <param name="ticketOrderId"></param>
        [ExtOperationInterceptor("删除保险")]
        public void DeleteInsurance(string ticketOrderId)
        {
            if (string.IsNullOrWhiteSpace(ticketOrderId)) return;

            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            insuranceDomainService.DeleteInsurance(ticketOrderId, AuthManager.GetCurrentUser());
            this.unitOfWork.Commit();
        }
        [ExtOperationInterceptor("获取保险记录")]
        public void GetInsuranceNoManual(string insuranceOrderId, string recordId)
        {
            int recordid = 0;
            if (!Int32.TryParse(recordId, out recordid))
            {
                throw new CustomException(111116, "非法保险记录请求。");
            }

            int insuranceOrderid = 0;
            if (!Int32.TryParse(insuranceOrderId, out insuranceOrderid))
            {
                throw new CustomException(111116, "非法保险记录请求。");
            }
            var insuranceDomainService = ObjectFactory.GetInstance<InsuranceDomainService>();
            insuranceDomainService.GetInsuranceNo(insuranceOrderid, recordid);
            this.unitOfWork.Commit();
        }

        [ExtOperationInterceptor("获取保单使用汇总")]
        public List<ResponseInsuranceUse> GetInsuranceUseSum(DateTime? startTime, DateTime? endTime)
        {
            var query = this._iInsuranceOrderRepository.getAllSearchItem().Where(p => p.EnumInsuranceStatus == EnumInsuranceStatus.GotInsurance);

            if (startTime.HasValue)
                query = query.Where(p => p.BuyTime > startTime);
            if (endTime.HasValue)
                query = query.Where(p => p.BuyTime <= endTime);

            List<ResponseInsuranceUse> list = new List<ResponseInsuranceUse>();

            foreach (var item in query.OrderByDescending(o => o.BuyTime).GroupBy(g => DateTime.Parse(g.BuyTime.ToString()).ToString("yyyy-MM-dd")).ToList())
            {
                list.Add(new ResponseInsuranceUse()
                {
                    DateTime = item.Key,
                    Count = item.Count()
                });
            }
            list.Add(new ResponseInsuranceUse()
            {
                DateTime = "合计:",
                Count = list.Sum(p => p.Count)
            });
            return list;
        }

        /// <summary>
        /// 保单销售汇总(控台)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("保单销售汇总(控台)")]
        public List<ResponseInsuranceSaleSum> GetInsuranceSaleSum(DateTime? startTime, DateTime? endTime)
        {
            var query = this._iInsuranceDepositLogRepository.FindAll(f => f.BuyState != EnumInsurancePayStatus.NoPay);

            if (startTime.HasValue)
                query = query.Where(p => p.BuyTime > startTime);
            if (endTime.HasValue)
                query = query.Where(p => p.BuyTime <= endTime);

            List<ResponseInsuranceSaleSum> list = new List<ResponseInsuranceSaleSum>();

            foreach (var item in query.OrderByDescending(o => o.BuyTime).ToLookup(t => t.BuyTime.ToString("yyyy-MM-dd")).ToList().AsParallel())
            {
                list.Add(new ResponseInsuranceSaleSum()
                {
                    DateTime = item.Key,
                    AccountMoney = item.Where(w => w.PayWay == EnumPayMethod.Account).Sum(s => s.TotalPrice),
                    AccountPoundage = item.Where(w => w.PayWay == EnumPayMethod.Account).Sum(s => s.PayFee),
                    CreditMoney = item.Where(w => w.PayWay == EnumPayMethod.Credit).Sum(s => s.TotalPrice),
                    CreditPoundage = item.Where(w => w.PayWay == EnumPayMethod.Credit).Sum(s => s.PayFee),
                    //PlatformMoney = item.Where(w => w.PayWay == EnumPayMethod.Platform).Sum(s => s.TotalPrice),
                    //PlatPoundage = item.Where(w => w.PayWay == EnumPayMethod.Platform).Sum(s => s.PayFee),
                    //BankMoney = item.Where(w => w.PayWay == EnumPayMethod.Bank).Sum(s => s.TotalPrice),
                    //BankPoundage = item.Where(w => w.PayWay == EnumPayMethod.Bank).Sum(s => s.PayFee),
                    TotalBuyCount = item.Where(w => w.RecordType == EnumInsuranceDepositType.Normal).Sum(s => s.DepositCount),
                    TotalGiveCount = item.Where(w => w.RecordType == EnumInsuranceDepositType.Offer).Sum(s => s.DepositCount),
                    TotalMoney = item.Sum(s => s.TotalPrice),
                    TotalPoundage = item.Sum(s => s.PayFee)
                });
            }
            return list;
        }

        /// <summary>
        /// 保单销售汇总(运营)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("保单销售汇总(运营)")]
        public List<ResponseInsuranceSaleSum> GetCarrierInsuranceSaleSum(DateTime? startTime, DateTime? endTime)
        {
            var query = this._iInsurancePurchaseByBussinessmanRepository.FindAll(f => f.BuyState != EnumInsurancePayStatus.NoPay);

            if (startTime.HasValue)
                query = query.Where(p => p.BuyTime > startTime);
            if (endTime.HasValue)
                query = query.Where(p => p.BuyTime <= endTime);

            List<ResponseInsuranceSaleSum> list = new List<ResponseInsuranceSaleSum>();

            foreach (var item in query.OrderByDescending(o => o.BuyTime).ToLookup(t => t.BuyTime.ToString("yyyy-MM-dd")).ToList().AsParallel())
            {
                list.Add(new ResponseInsuranceSaleSum()
                {
                    DateTime = item.Key,
                    AccountMoney = item.Where(w => w.PayWay == EnumPayMethod.Account).Sum(s => s.TotalPrice),
                    AccountPoundage = item.Where(w => w.PayWay == EnumPayMethod.Account).Sum(s => s.PayFee),
                    CreditMoney = item.Where(w => w.PayWay == EnumPayMethod.Credit).Sum(s => s.TotalPrice),
                    CreditPoundage = item.Where(w => w.PayWay == EnumPayMethod.Credit).Sum(s => s.PayFee),
                    //PlatformMoney = item.Where(w => w.PayWay == EnumPayMethod.Platform).Sum(s => s.TotalPrice),
                    //PlatPoundage = item.Where(w => w.PayWay == EnumPayMethod.Platform).Sum(s => s.PayFee),
                    //BankMoney = item.Where(w => w.PayWay == EnumPayMethod.Bank).Sum(s => s.TotalPrice),
                    //BankPoundage = item.Where(w => w.PayWay == EnumPayMethod.Bank).Sum(s => s.PayFee),
                    TotalBuyCount = item.Where(w => w.RecordType == EnumInsurancePurchaseType.Normal).Sum(s => s.DepositCount),
                    TotalGiveCount = item.Where(w => w.RecordType == EnumInsurancePurchaseType.Offer).Sum(s => s.DepositCount),
                    TotalMoney = item.Sum(s => s.TotalPrice),
                    TotalPoundage = item.Sum(s => s.PayFee)
                });
            }
            return list;
        }
    }
}
