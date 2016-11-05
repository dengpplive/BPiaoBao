using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{
    public class InsuranceDomainService
    {
        private IInsuranceOrderRepository _iInsuranceOrderRepository;
        private IInsuranceConfigRepository _iInsuranceConfigRepository;
        private IInsuranceDepositLogRepository _iInsuranceDepositLogRepository;
        private IInsurancePurchaseByBussinessmanRepository _iInsurancePurchaseByBussinessmanRepository;
        private IOrderRepository _iOrderRepository;
        private IPaymentClientProxy _iPayMentClientProxy;
        private IBusinessmanRepository _businessmanRepository;
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());

        public InsuranceDomainService(IInsuranceOrderRepository iInsuranceOrderRepository, IInsuranceConfigRepository iInsuranceConfigRepository, IInsuranceDepositLogRepository iInsuranceDepositLogRepository, IOrderRepository iOrderRepository, IPaymentClientProxy iPayMentClientProxy, IBusinessmanRepository businessmanRepository, IInsurancePurchaseByBussinessmanRepository iInsurancePurchaseByBussinessmanRepository)
        {
            this._iInsuranceOrderRepository = iInsuranceOrderRepository;
            this._iInsuranceConfigRepository = iInsuranceConfigRepository;
            this._iInsuranceDepositLogRepository = iInsuranceDepositLogRepository;
            this._iOrderRepository = iOrderRepository;
            this._iPayMentClientProxy = iPayMentClientProxy;
            this._businessmanRepository = businessmanRepository;
            this._iInsurancePurchaseByBussinessmanRepository = iInsurancePurchaseByBussinessmanRepository;
        }


        /// <summary>
        /// 保存保险
        /// </summary>
        /// <param name="order">机票订单</param>
        /// <param name="currentUser">出保险商户</param>
        /// <param name="isAlone">是否单独购买保险而未伴随机票订单一起</param>
        /// <param name="hasTicket">是否有与之对应的机票</param>
        public void SaveInsurance(InsuranceOrder order, CurrentUserInfo currentUser, bool isAlone = false, bool hasTicket = true)
        {

            //验证机票订单号是否存在
            Order ticketOrder = null;
            if (hasTicket)
            {
                ticketOrder = this._iOrderRepository.FindAll(o => o.OrderId == order.OrderId).FirstOrDefault();
                if (ticketOrder == null)
                {
                    throw new CustomException(110001, "订单号" + order.OrderId + "不存在。");
                }
            }

            //验证保单数量是否足够
            var insuranceCfg = this._iInsuranceConfigRepository.FindAll(i => i.BusinessmanCode == currentUser.Code).FirstOrDefault();
            if (insuranceCfg == null
                || !insuranceCfg.IsOpen)
            {
                throw new CustomException(110002, "尚未开启航意险。");
            }
            if (insuranceCfg.LeaveCount < order.InsuranceRecords.Sum(i => i.Count))
            {
                throw new CustomException(110003, "航意险仅剩余" + insuranceCfg.LeaveCount + "份。");
            }

            //数据准备
            var carrier = this._businessmanRepository.FindAll(b => b.Code == currentUser.CarrierCode).FirstOrDefault();
            string insuranceCompany = (from InsuranceElement ctrl in InsuranceSection.GetInsuranceConfigurationSection().CtrlInsuranceCollection
                                       where ctrl.IsCurrent
                                       select ctrl.Value).FirstOrDefault();//保险公司

            //删除之前该机票下的保险
            if (!isAlone && hasTicket)
            {
                var oldrecord = this._iInsuranceOrderRepository.FindAll(m => m.OrderId == order.OrderId);
                foreach (InsuranceOrder item in oldrecord)
                {
                    item.InsuranceRecords.Clear();
                    this.unitOfWorkRepository.PersistDeletionOf(item);
                }
            }

            //修改机票订单信息
            if (hasTicket)
            {
                foreach (var passger in ticketOrder.Passengers)
                {
                    if (order.InsuranceRecords.Count(r => r.PassengerId == passger.Id) > 0)
                    {
                        passger.BuyInsuranceCount = order.InsuranceRecords.Where(r => r.PassengerId == passger.Id).Sum(r => r.Count);
                        passger.BuyInsurancePrice = insuranceCfg.SinglePrice;
                    }
                }

                this.unitOfWorkRepository.PersistUpdateOf(ticketOrder);
            }
            //补全保险系统信息
            order.BuyTime = DateTime.Now;
            foreach (var record in order.InsuranceRecords)
            {
                record.BussinessmanCode = currentUser.Code;//分销商号
                record.BussinessmanName = currentUser.BusinessmanName;//分销商名称
                record.CarrierCode = carrier.Code;//运营商号
                record.CarrierName = carrier.Name;//运营商名称
                record.InsuranceCompany = insuranceCompany;//保险公司
                record.InsurancePrice = insuranceCfg.SinglePrice;//保险单价
                record.SerialNum = record.GetSerialNum();//获取流水号
                record.InsuranceStatus = EnumInsuranceStatus.NoInsurance;
                record.PolicyAmount = 400000;
            }
            //补全保险订单中机票订单相关信息
            if (hasTicket)
            {

                foreach (var record in order.InsuranceRecords)
                {
                    Passenger passenger = ticketOrder.Passengers.Where(p => p.Id == record.PassengerId).FirstOrDefault();
                    if (passenger == null)
                    {
                        throw new CustomException(110004, "机票订单中不存在指定的乘客。");
                    }
                    SkyWay skyway = ticketOrder.SkyWays.Where(s => s.Id == record.SkyWayId).FirstOrDefault();
                    if (skyway == null)
                    {
                        throw new CustomException(110005, "机票订单中不存在指定的航线。");
                    }
                    string fromCityName = string.Empty;
                    string toCityName = string.Empty;
                    PnrAnalysis.PnrResource pnrResource = new PnrAnalysis.PnrResource();
                    var cityInfo = pnrResource.GetCityInfo(skyway.FromCityCode);
                    if (cityInfo != null)
                        fromCityName = cityInfo.city.Name;
                    cityInfo = pnrResource.GetCityInfo(skyway.ToCityCode);
                    if (cityInfo != null)
                        toCityName = cityInfo.city.Name;

                    //record.InsuranceLimitEndTime = skyway.ToDateTime.AddDays(7);//保险生效结束时间
                    //record.InsuranceLimitStartTime = skyway.StartDateTime;//保险生效开始时间
                    record.InsuranceLimitEndTime = skyway.StartDateTime.Date.AddDays(1).AddSeconds(-1);//保险生效结束时间
                    record.InsuranceLimitStartTime = skyway.StartDateTime.Date;//保险生效开始时间
                    record.InsuredName = passenger.PassengerName;//被投保人姓名
                    record.IdType = passenger.IdType;//证件类型
                    record.CardNo = passenger.CardNo;//证件号
                    record.SexType = passenger.SexType;//性别
                    record.Mobile = passenger.Mobile;//手机号
                    record.FlightNumber = skyway.CarrayCode + skyway.FlightNumber;//航班号
                    record.PNR = ticketOrder.PnrCode;//PNR
                    record.ToCityName = fromCityName;//到达城市名称
                    record.StartDateTime = skyway.StartDateTime;
                    record.ToDateTime = skyway.ToDateTime;
                    record.FromCityCode = skyway.FromCityCode;
                    record.FromCityName = fromCityName;
                    record.ToCityCode = skyway.ToCityCode;
                    record.ToCityName = toCityName;
                    record.InsureType = EnumInsureMethod.Auto;
                    record.PassengerType = passenger.PassengerType;
                    record.Birth = passenger.Birth;
                }
            }
            //验证保险订单信息是否合法
            order.CheckData();

            //如果是单独购买，则立刻获得保单号
            if (isAlone)
            {
                foreach (InsuranceRecord insuranceRecord in order.InsuranceRecords)
                {
                    //获得保单号
                    string insuranceNo = this.GetInsuranceNo(insuranceRecord);
                    if (!string.IsNullOrEmpty(insuranceNo))
                    {
                        insuranceRecord.InsuranceNo = insuranceNo;
                        insuranceRecord.InsuranceStatus = EnumInsuranceStatus.GotInsurance;
                    }
                    else
                    {
                        insuranceRecord.InsuranceStatus = EnumInsuranceStatus.Manual;
                    }
                }
            }
            this.unitOfWorkRepository.PersistCreationOf(order);

            //修改保险配置信息
            insuranceCfg.LeaveCount -= order.InsuranceRecords.Sum(i => i.Count);
            this.unitOfWorkRepository.PersistUpdateOf(insuranceCfg);

        }

        /// <summary>
        /// 获得保险配置
        /// </summary>
        /// <param name="userCode">商户号</param>
        /// <returns></returns>
        public InsuranceConfig QueryInsuranceConfig(string userCode)
        {

            var model = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == userCode).FirstOrDefault();
            return model;

        }

        /// <summary>
        /// 支付成功之后，获取保单号
        /// </summary>
        /// <param name="order"></param>
        public void GetInsuranceNos(Order order)
        {
            var insuranceOrder =
                this._iInsuranceOrderRepository.FindAll(m => m.OrderId == order.OrderId).FirstOrDefault();
            if (insuranceOrder == null) return;
            foreach (var insurance in insuranceOrder.InsuranceRecords)
            {
                //如果保单未获得保单号
                if (insurance.InsuranceStatus == EnumInsuranceStatus.NoInsurance)
                {
                    //获得保单号
                    string insuranceNo = this.GetInsuranceNo(insurance);
                    if (string.IsNullOrEmpty(insuranceNo))
                    {
                        insurance.InsuranceStatus = EnumInsuranceStatus.Manual;
                        continue;
                    }
                    else
                    {
                        insurance.InsuranceNo = insuranceNo;
                        insurance.InsuranceStatus = EnumInsuranceStatus.GotInsurance;
                    }
                }
            }
            this.unitOfWorkRepository.PersistUpdateOf(insuranceOrder);
        }

        /// <summary>
        /// 退回保险数量
        /// </summary>
        /// <param name="order"></param>
        public void ReturnInsurance(Order order)
        {
            try
            {
                int insuranceCount = 0;
                insuranceCount = order.Passengers.Sum(p => p.BuyInsuranceCount);
                //修改运营商保险数量
                var curBuyermodel =
                        this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == order.BusinessmanCode).FirstOrDefault();
                curBuyermodel.LeaveCount += Convert.ToInt32(insuranceCount);
                this.unitOfWorkRepository.PersistUpdateOf(curBuyermodel);

                //修改保单状态
                var insuranceOrders = this._iInsuranceOrderRepository.FindAll(i => i.OrderId == order.OrderId);
                foreach (var insuranceOrder in insuranceOrders)
                {
                    foreach (var record in insuranceOrder.InsuranceRecords)
                    {
                        record.InsuranceStatus = EnumInsuranceStatus.Canceled;
                    }
                    this.unitOfWorkRepository.PersistUpdateOf(insuranceOrder);
                }

            }
            catch { }
        }

        public void DeleteInsurance(string ticketOrderId, CurrentUserInfo currentUser)
        {
            var insuranceCfg = this._iInsuranceConfigRepository.FindAll(i => i.BusinessmanCode == currentUser.Code).FirstOrDefault();
            if (insuranceCfg == null) return;
            var oldrecords = this._iInsuranceOrderRepository.FindAll(m => m.OrderId == ticketOrderId);
            if (oldrecords == null) return;
            //修改保险配置信息
            foreach (var record in oldrecords)
            {
                if (record.InsuranceRecords == null)
                {
                    continue;
                }
                insuranceCfg.LeaveCount += record.InsuranceRecords.Sum(i => i.Count);
            }
            this.unitOfWorkRepository.PersistUpdateOf(insuranceCfg);
            //删除保险订单

            foreach (InsuranceOrder item in oldrecords)
            {
                if (item.InsuranceRecords != null)
                {
                    item.InsuranceRecords.Clear();
                }
                this.unitOfWorkRepository.PersistDeletionOf(item);
            }
        }

        /// <summary>
        /// 运营商批发保险查询
        /// </summary>
        /// <param name="BusinessmanCode"></param>
        /// <param name="BuyStartTime"></param>
        /// <param name="BuyEndTime"></param>
        /// <returns></returns>
        public IQueryable<InsuranceDepositLog> QueryInsuranceDepositLog(string BusinessmanCode, string payNo, string tradeId, DateTime? BuyStartTime, DateTime? BuyEndTime, EnumInsuranceDepositType? insuranceDepositType)
        {
            var query = this._iInsuranceDepositLogRepository.FindAll();

            if (!string.IsNullOrWhiteSpace(BusinessmanCode))
            {
                query = query.Where(p => p.BusinessmanCode == BusinessmanCode);
            }
            if (BuyStartTime != null)
            {
                query = query.Where(p => p.BuyTime >= BuyStartTime.Value);
            }
            if (BuyEndTime != null)
            {
                var date = BuyEndTime.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.BuyTime <= date);
            }
            if (insuranceDepositType != null)
            {
                query = query.Where(p => p.RecordType == insuranceDepositType);
            }
            if (!string.IsNullOrWhiteSpace(payNo))
            {
                query = query.Where(p => p.PayNo == payNo);
            }
            if (!string.IsNullOrWhiteSpace(tradeId))
            {
                query = query.Where(p => p.OutTradeNo == tradeId);
            }

            return query;
        }

        /// <summary>
        /// 查询分销商购买保险记录
        /// </summary>
        /// <param name="buyStartTime">购买时间起始</param>
        /// <param name="buyEndTime">购买时间结束</param>
        /// <param name="recordType">记录类型</param>
        /// <param name="tradeNo">交易号</param>
        /// <param name="payNo">流水号</param>
        /// <param name="businessmanCode">分销商号</param>
        /// <param name="carrierCode">运营商号</param>
        /// <returns></returns>
        public IQueryable<InsurancePurchaseByBussinessman> QueryInsurancePurchaseByBussinessman(
            DateTime? buyStartTime = null,
            DateTime? buyEndTime = null,
            EnumInsurancePurchaseType? recordType = null,
            string tradeNo = null,
            string payNo = null,
            string businessmanCode = null,
            string carrierCode = null,
            EnumInsurancePayStatus? payStatus = null
            )
        {
            var query = this._iInsurancePurchaseByBussinessmanRepository.FindAll();

            if (buyStartTime != null)
            {
                query = query.Where(q => q.BuyTime >= buyStartTime);
            }
            if (buyEndTime != null)
            {
                buyEndTime = buyEndTime.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(q => q.BuyTime <= buyEndTime);
            }
            if (recordType != null)
            {
                query = query.Where(q => q.RecordType == recordType.Value);
            }
            if (!string.IsNullOrEmpty(tradeNo))
            {
                query = query.Where(q => q.OutTradeNo == tradeNo);
            }
            if (!string.IsNullOrEmpty(payNo))
            {
                query = query.Where(q => q.PayNo == payNo);
            }
            if (!string.IsNullOrEmpty(businessmanCode))
            {
                query = query.Where(q => q.BusinessmanCode == businessmanCode);
            }
            if (!string.IsNullOrEmpty(carrierCode))
            {
                query = query.Where(q => q.CarrierCode == carrierCode);
            }
            if (payStatus != null)
            {
                query = query.Where(q => q.BuyState == payStatus);
            }

            return query;
        }

        /// <summary>
        /// 分销商购买保险
        /// </summary>
        /// <param name="buyCount"></param>
        /// <param name="pwd"></param>
        /// <param name="businessmanCode"></param>
        /// <param name="businessmanName"></param>
        /// <param name="carrierCode"></param>
        /// <param name="payUser"></param>
        public void PurchaseInsuranceFromCarrier(int buyCount, string pwd, EnumPayMethod payMethod, string businessmanCode, string businessmanName, string carrierCode, string operatorCode, string operatorName, CurrentUserInfo payUser, string remark = "")
        {
            var carrier = this._businessmanRepository.FindAll(p => p.Code == carrierCode && p is Carrier).OfType<Carrier>().FirstOrDefault();
            //获取当前运营商保险配置
            var carrierCfg = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == carrierCode).FirstOrDefault();
            //获取分销商保险配置
            var buyerCfg = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == businessmanCode).FirstOrDefault();
            if (carrierCfg == null || !carrierCfg.IsOpen)
            {
                throw new CustomException(11114, "运营商保险功能未开启");
            }
            if (buyCount > carrierCfg.LeaveCount)
            {
                string type = payUser == null ? "赠送" : "购买";
                throw new CustomException(11115, "最多仅可" + type + carrierCfg.LeaveCount + "份保险。");
            }
            //单价
            decimal singlePrice = payUser == null ? 0 : carrierCfg.SinglePrice;

            //运营商购买记录
            var logBuiler = AggregationFactory.CreateBuiler<InsurancePurchaseByBussinessmanBuilder>();
            var log = logBuiler.CreateInsurancePurchaseByBussinessman();
            log.PayNo = log.GetPayNo();
            log.BeforeLeaveCount = buyerCfg == null ? 0 : buyerCfg.LeaveCount; //购买前剩余
            log.AfterLeaveCount = log.BeforeLeaveCount + buyCount; //购买后剩余
            log.DepositCount = buyCount; //购买数量
            log.TotalPrice = Math.Round((singlePrice * buyCount), 2); //总价
            log.SinglePrice = singlePrice; //单价
            log.BusinessmanCode = businessmanCode; //商户号
            log.BusinessmanName = businessmanName; //商户名称
            log.PayWay = payMethod; //支付类别
            log.RecordType = payUser == null ? EnumInsurancePurchaseType.Offer : EnumInsurancePurchaseType.Normal;//记录类别
            log.CarrierCode = carrierCode;
            log.CarrierName = carrier.Name;
            log.OperatorAccount = operatorCode;
            log.OperatorName = operatorName;
            log.Remark = remark;
            log.PayFee = Math.Round(log.TotalPrice * SystemConsoSwitch.Rate, 2);

            #region 支付

            DataBill databill = new DataBill();
            CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
            string moneyDispatch = "";
            decimal infMoney = databill.Round(log.TotalPrice * carrier.Rate, 2);
            //支出配置,分销商，支出保险全额
            //moneyDispatch += currentUser.CashbagCode + "^" + insuranceOrder.PayMoney + "^" + EnumOperationType.Insurance.ToEnumDesc();
            //收入配置
            moneyDispatch += "|" + carrier.CashbagCode + "^" + log.TotalPrice + "^" + EnumOperationType.InsuranceReceivables.ToEnumDesc();
            //保险手续费支出
            moneyDispatch += "|" + carrier.CashbagCode + "^" + (-infMoney) + "^" + EnumOperationType.InsuranceServer.ToEnumDesc();
            //手续费收入配置
            moneyDispatch += "|" + setting.CashbagCode + "^" + infMoney + "^" + EnumOperationType.InsuranceServer.ToEnumDesc();

            var payResult = string.Empty;

            if (payUser != null)
            {
                if (_iPayMentClientProxy == null)
                {
                    _iPayMentClientProxy = new CashbagPaymentClientProxy();
                }
                switch (log.PayWay)
                {
                    //现金账户
                    case EnumPayMethod.Account:
                        //    payResult = _iPayMentClientProxy.PaymentByCashAccount(payUser.CashbagCode, payUser.CashbagKey,
                        //log.PayNo, "购买保险", log.TotalPrice, pwd, moneyDispatch);
                        var resultc = _iPayMentClientProxy.PaymentByCashAccount(payUser.CashbagCode, payUser.CashbagKey, log.PayNo, "购买保险", log.TotalPrice, pwd, moneyDispatch);
                        if (resultc.Item1)
                        {
                            //该订单已经被在线支付时写入日志
                            Logger.WriteLog(LogType.INFO, "订单已经支付，交易号为" + resultc.Item2 + "支付方式为" + resultc.Item3);
                            throw new CustomException(0001, "订单已经支付，交易号为" + resultc.Item2 + "支付方式为" + resultc.Item3);
                        }
                        payResult = resultc.Item2;
                        break;
                    //信用账户
                    case EnumPayMethod.Credit:
                        var result = _iPayMentClientProxy.PaymentByCreditAccount(payUser.CashbagCode, payUser.CashbagKey, log.PayNo, "购买保险", log.TotalPrice, pwd, moneyDispatch);
                        if (result.Item1)
                        {
                            //该订单已经被在线支付时写入日志
                            Logger.WriteLog(LogType.INFO, "订单已经支付，交易号为" + result.Item2 + "支付方式为" + result.Item3);
                            throw new CustomException(0001, "订单已经支付，交易号为" + result.Item2 + "支付方式为" + result.Item3);
                        }
                        payResult = result.Item2;
                        break;
                }
            }
            #endregion

            //如果存在交易号，即支付成功
            if (!string.IsNullOrEmpty(payResult))
            {
                log.OutTradeNo = payResult; //设置交易号
                log.BuyState = EnumInsurancePayStatus.OK; //设置交易状态
                //设置运营商剩余数量
                carrierCfg.LeaveCount -= buyCount;
                this.unitOfWorkRepository.PersistUpdateOf(carrierCfg);
                //设置分销商剩余数量
                if (buyerCfg != null)
                {
                    buyerCfg.LeaveCount = log.AfterLeaveCount;
                    this.unitOfWorkRepository.PersistUpdateOf(buyerCfg);
                }
                else
                {
                    var builder = AggregationFactory.CreateBuiler<InsuranceConfigBuilder>();
                    var m = builder.CreateInsuranceConfig();
                    m.BusinessmanCode = businessmanCode;
                    m.BusinessmanName = businessmanName;
                    m.IsOpen = true;
                    m.SinglePrice = carrierCfg.SinglePrice;
                    m.LeaveCount = buyCount;
                    m.ConfigType = EnumInsuranceConfigType.Buyer;
                    this.unitOfWorkRepository.PersistCreationOf(m);
                }
            }
            else if (payUser == null)
            {
                log.BuyState = EnumInsurancePayStatus.Offer;
                //设置运营商剩余数量
                carrierCfg.LeaveCount -= buyCount;
                this.unitOfWorkRepository.PersistUpdateOf(carrierCfg);
                //设置分销商剩余数量
                if (buyerCfg != null)
                {
                    buyerCfg.LeaveCount = log.AfterLeaveCount;
                    this.unitOfWorkRepository.PersistUpdateOf(buyerCfg);
                }
                else
                {
                    var builder = AggregationFactory.CreateBuiler<InsuranceConfigBuilder>();
                    var m = builder.CreateInsuranceConfig();
                    m.BusinessmanCode = businessmanCode;
                    m.BusinessmanName = businessmanName;
                    m.IsOpen = true;
                    m.SinglePrice = carrierCfg.SinglePrice;
                    m.LeaveCount = buyCount;
                    m.ConfigType = EnumInsuranceConfigType.Buyer;
                    this.unitOfWorkRepository.PersistCreationOf(m);
                }
            }
            //否则交易失败
            else
            {
                log.BuyState = EnumInsurancePayStatus.NoPay;
            }

            log.BuyTime = DateTime.Now;
            this.unitOfWorkRepository.PersistCreationOf(log);


        }

        /// <summary>
        /// 运营商批发保险
        /// </summary>
        /// <param name="buyCount">购买保险的数量</param>
        /// <param name="pwd"></param>
        /// <param name="carrierCode"></param>
        /// <param name="carrierName"></param>
        /// <param name="payUser"></param>
        public void BuyInsuranceByCashByCarrier(int buyCount, string pwd, string carrierCode, string carrierName, string operatorCode, string operatorName, CurrentUserInfo payUser, string remark = "")
        {
            //获取保险配置
            var cfg = InsuranceSection.GetInsuranceConfigurationSection();
            if (!cfg.CtrlInsuranceCollection.IsEnabled)
            {
                throw new CustomException(11112, "保险功能未开启");
            }
            //最大能够购买的数量
            var canBuyMaxCount =
                    (from InsuranceElement ctrl in cfg.CtrlInsuranceCollection
                     select ctrl).Where(m => m.IsCurrent).FirstOrDefault().LeaveCount;
            if (buyCount > canBuyMaxCount)
            {
                string type = payUser == null ? "赠送" : "购买";
                throw new CustomException(11113, "最多仅可" + type + canBuyMaxCount + "份保险。");
            }

            //获取当前运营商保险信息
            var model = this._iInsuranceConfigRepository.FindAll(p => p.BusinessmanCode == carrierCode).FirstOrDefault();

            //获取单价
            decimal singlePrice = payUser == null ? 0 : cfg.CtrlInsuranceCollection.SinglePrice;

            //运营商购买记录
            var logBuiler = AggregationFactory.CreateBuiler<InsuranceDepositLogBuilder>();
            var log = logBuiler.CreateInsuranceDepositLog();
            log.PayNo = log.GetPayNo();
            log.BeforeLeaveCount = model == null ? 0 : model.LeaveCount; //购买前剩余
            log.AfterLeaveCount = log.BeforeLeaveCount + buyCount; //购买后剩余
            log.DepositCount = buyCount; //购买数量
            log.TotalPrice = Math.Round((singlePrice * buyCount), 2); //总价
            log.SinglePrice = singlePrice; //单价
            log.BusinessmanCode = carrierCode; //商户号
            log.BusinessmanName = carrierName; //商户名称
            log.PayWay = EnumPayMethod.Account; //支付类别
            log.Remark = remark;
            log.RecordType = payUser == null ? EnumInsuranceDepositType.Offer : EnumInsuranceDepositType.Normal;//记录类别
            log.OperatorAccount = operatorCode;
            log.OperatorName = operatorName;
            log.PayFee = Math.Round(SystemConsoSwitch.Rate * log.TotalPrice, 2);
            if (_iPayMentClientProxy == null)
            {
                _iPayMentClientProxy = new CashbagPaymentClientProxy();
            }
            var payResult = string.Empty;
            if (payUser != null)
            {
                //现金账户
                //payResult = _iPayMentClientProxy.PaymentByCashAccount(payUser.CashbagCode, payUser.CashbagKey,
                //    log.PayNo, "购买保险", log.TotalPrice, pwd);
                var resultc = _iPayMentClientProxy.PaymentByCashAccount(payUser.CashbagCode, payUser.CashbagKey, log.PayNo, "购买保险", log.TotalPrice, pwd);
                if (resultc.Item1)
                {
                    //该订单已经被在线支付时写入日志
                    Logger.WriteLog(LogType.INFO, "订单已经支付，交易号为" + resultc.Item2 + "支付方式为" + resultc.Item3);
                    throw new CustomException(0001, "订单已经支付，交易号为" + resultc.Item2 + "支付方式为" + resultc.Item3);
                }
                payResult = resultc.Item2;
            }
            //如果存在交易号，即支付成功
            if (!string.IsNullOrEmpty(payResult))
            {
                log.OutTradeNo = payResult; //设置交易号
                log.BuyState = EnumInsurancePayStatus.OK; //设置交易状态
                //设置剩余数量
                foreach (var m in (from InsuranceElement ctrl in cfg.CtrlInsuranceCollection
                                   select ctrl).Where(m => m.IsCurrent))
                {
                    m.LeaveCount -= buyCount;
                    break;
                }
                //保存配置信息
                InsuranceSection.Save();
                //保存当前分销商商户保险配置
                if (model != null)
                {
                    model.LeaveCount = log.AfterLeaveCount;
                    this.unitOfWorkRepository.PersistUpdateOf(model);
                }
                else
                {
                    var builder = AggregationFactory.CreateBuiler<InsuranceConfigBuilder>();
                    var m = builder.CreateInsuranceConfig();
                    m.BusinessmanCode = carrierCode;
                    m.BusinessmanName = carrierName;
                    m.IsOpen = true;
                    m.SinglePrice = singlePrice;
                    m.LeaveCount = buyCount;
                    m.ConfigType = EnumInsuranceConfigType.Carrier;
                    this.unitOfWorkRepository.PersistCreationOf(m);
                }

            }
            else if (payUser == null)
            {
                log.BuyState = EnumInsurancePayStatus.Offer;
                //设置剩余数量
                foreach (var m in (from InsuranceElement ctrl in cfg.CtrlInsuranceCollection
                                   select ctrl).Where(m => m.IsCurrent))
                {
                    m.LeaveCount -= buyCount;
                    break;
                }
                //保存配置信息
                InsuranceSection.Save();
                //保存当前分销商商户保险配置
                if (model != null)
                {
                    model.LeaveCount = log.AfterLeaveCount;
                    this.unitOfWorkRepository.PersistUpdateOf(model);
                }
                else
                {
                    var builder = AggregationFactory.CreateBuiler<InsuranceConfigBuilder>();
                    var m = builder.CreateInsuranceConfig();
                    m.BusinessmanCode = carrierCode;
                    m.BusinessmanName = carrierName;
                    m.IsOpen = true;
                    m.SinglePrice = singlePrice;
                    m.LeaveCount = buyCount;
                    m.ConfigType = EnumInsuranceConfigType.Carrier;
                    this.unitOfWorkRepository.PersistCreationOf(m);
                }

            }
            //否则交易失败
            else
            {
                log.BuyState = EnumInsurancePayStatus.NoPay;
            }

            log.BuyTime = DateTime.Now;
            this.unitOfWorkRepository.PersistCreationOf(log);
        }

        /// <summary>
        /// 获得指定id的保险记录保单号
        /// </summary>
        /// <param name="insuranceOrderId"></param>
        /// <param name="recordId"></param>
        public void GetInsuranceNo(int insuranceOrderId, int recordId)
        {
            //获得保险订单
            var insruanceOrders = this._iInsuranceOrderRepository.FindAll(r => r.Id == insuranceOrderId);
            if (insruanceOrders == null)
            {
                throw new CustomException(111113, "不存在请求的记录。");
            }
            //获得保险记录
            InsuranceRecord record = null;
            InsuranceOrder insuranceOrder = null;
            foreach (InsuranceOrder order in insruanceOrders)
            {
                foreach (InsuranceRecord r in order.InsuranceRecords)
                {
                    if (r.Id == recordId)
                    {
                        insuranceOrder = order;
                        break;
                    }
                }
                if (insuranceOrder != null) break;
            }
            if (insuranceOrder == null)
            {
                throw new CustomException(111114, "不存在指定的保险记录。");
            }
            else
            {
                foreach (InsuranceRecord r in insuranceOrder.InsuranceRecords)
                {
                    if (r.Id == recordId)
                    {
                        //只有手动出单状态的保单可以进行此操作
                        if (r.InsuranceStatus != EnumInsuranceStatus.Manual)
                        {
                            throw new CustomException(111115, "只有" + EnumInsuranceStatus.Manual.ToEnumDesc() + "状态的保险记录可以手动出单。");
                        }
                        string insuranceNo = this.GetInsuranceNo(r);
                        //如果获得保单号成功，则填写保单号，改变保单状态
                        if (!string.IsNullOrEmpty(insuranceNo))
                        {
                            r.InsuranceNo = insuranceNo;
                            r.InsuranceStatus = EnumInsuranceStatus.GotInsurance;
                        }

                        break;
                    }
                }
            }
        }

        #region 私有方法
        private string GetInsuranceNo(InsuranceRecord insurance)
        {
            if (!insurance.InsuranceLimitStartTime.HasValue || !insurance.InsuranceLimitEndTime.HasValue)
            {
                throw new CustomException(10002, "保险期限时间不能够为空");
            }
            string insuranceNo = "";
            try
            {

                //DateTime? birth = null;
                string cardno = insurance.CardNo;
                if (insurance.IdType == EnumIDType.BirthDate)
                {
                    //birth = Convert.ToDateTime(cardno);
                    cardno = "";
                }
                //如果返回-1，则为获取失败
                insuranceNo = InsurancePlatformDomainService.Buy_Insurance(
                    insurance.SerialNum,
                    insurance.InsuranceLimitStartTime.Value,
                    insurance.InsuranceLimitEndTime.Value,
                    insurance.InsuredName,
                    insurance.IdType,
                    cardno,
                    insurance.SexType,
                    insurance.Birth,
                    insurance.Mobile,
                    insurance.Count,
                    insurance.FlightNumber,
                    insurance.StartDateTime,
                    insurance.ToCityName
                    );
                if (insuranceNo == "-1")
                {
                    new CommLog().WriteLog("GetInsuranceNo", "保单号获取失败，编号" + insurance.Id);
                    insuranceNo = "";
                }
            }
            catch (Exception ex)
            {
                insuranceNo = "";
                new CommLog().WriteLog("GetInsuranceNo", "保单号获取失败，编号" + insurance.Id + "\n" + ex.StackTrace + ex.Message);
            }
            //如果出现异常，则直接将异常写入日志
            return insuranceNo;
        }

        #endregion



    }
}
