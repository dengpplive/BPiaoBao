using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework.DDD;
using System.Linq;
using StructureMap;
using JoveZhao.Framework.Expand;
using BPiaoBao.AppServices.Contracts;
using System.Collections.Generic;
using JoveZhao.Framework;
using BPiaoBao.Common;
using System;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.StationContracts.SystemSetting;
using BPiaoBao.Common.Enums;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using PnrAnalysis;
using BPiaoBao.SystemSetting.Domain.Services;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class BusinessmanService : BaseService, IBusinessmanService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        IUnitOfWork _ticketUnitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository _ticketUnitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());

        IBusinessmanRepository businessmanRepository;
        IPaymentClientProxy payMentClientProxy;
        IRoleRepository roleRepository;
        ISmsTemplateRepository smsTemplateRepository;
        ISMSChargeSetRepository smsChargeRepository;
        IOrderRepository orderRepository;
        IAfterSaleOrderRepository afterSaleOrderRepository;
        IOPENScanRepository openScanRepository;
        CustomerInfoDomainService _customerInfoDomainService;
        StationBuyGroupDomainService _stationBuyGroupDomainService;

        public BusinessmanService(IBusinessmanRepository businessmanRepository, IPaymentClientProxy payMentClientProxy, IRoleRepository roleRepository, ISmsTemplateRepository smsTemplateRepository, ISMSChargeSetRepository smsChargeRepository, IOrderRepository orderRepository, IAfterSaleOrderRepository afterSaleOrderRepository, IOPENScanRepository openScanRepository, CustomerInfoDomainService customerInfoDomainService,StationBuyGroupDomainService stationBuyGroupDomainService)
        {
            this.businessmanRepository = businessmanRepository;
            this.roleRepository = roleRepository;
            this.payMentClientProxy = payMentClientProxy;
            this.smsTemplateRepository = smsTemplateRepository;
            this.smsChargeRepository = smsChargeRepository;
            this.orderRepository = orderRepository;
            this.afterSaleOrderRepository = afterSaleOrderRepository;
            this.openScanRepository = openScanRepository;
            this._customerInfoDomainService = customerInfoDomainService;
            this._stationBuyGroupDomainService = stationBuyGroupDomainService;
        }
        [ExtOperationInterceptor("得到所有员工")]
        public List<OperatorDto> GetAllOperators(string realName, string account, EnumOperatorState? operatorState)
        {

            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();

            var query = businessman.GetOperatorsBySearch(realName, account, operatorState);
            return query.ToList().Select(p => new OperatorDto
            {
                Account = p.Account,
                OperatorState = p.OperatorState,
                Phone = p.Phone,
                Realname = p.Realname,
                CreateDate = p.CreateDate,
                Id = p.Id,
                Tel = p.Tel
            }).ToList();
        }
        [ExtOperationInterceptor("修改员工状态")]
        public void ModifyOperatorState(string account)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            businessman.CanExecute(account);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("删除员工")]
        public void DeleteOperator(string account)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            businessman.RemoveOperator(account);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("添加员工")]
        public void AddOperator(OperatorDto operatorDto)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            Operator op = new Operator();
            op.Account = operatorDto.Account;
            op.OperatorState = EnumOperatorState.Normal;
            op.Password = operatorDto.Password;
            op.Phone = operatorDto.Phone;
            op.Realname = operatorDto.Realname;
            op.Tel = operatorDto.Tel;
            businessman.NewOperator(op);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();

        }
        [ExtOperationInterceptor("更新员工")]
        public void UpdateOperator(OperatorDto operatorDto)
        {
            if (operatorDto == null)
                throw new CustomException(500, "Operator is null");
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).FirstOrDefault();
            if (businessman == null)
                throw new CustomException(404, "商户不存在");
            var oModel = businessman.Operators.Where(p => p.Id == operatorDto.Id).FirstOrDefault();
            if (oModel == null)
                throw new CustomException(404, "修改操作员不存在");
            oModel.Realname = operatorDto.Realname;
            oModel.Phone = operatorDto.Phone;
            oModel.Tel = operatorDto.Tel;
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
        }

        [ExtOperationInterceptor("修改员工密码")]
        public void ChangePassword(string account, string oldPassword, string newPassword)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            businessman.ChangePassword(account, oldPassword, newPassword);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
            Logger.WriteLog(LogType.DEBUG, string.Format("{0}修改采购商[{1}]密码", AuthManager.GetCurrentUser().Code, AuthManager.GetCurrentUser().Code));
        }
        [ExtOperationInterceptor("发送短信")]
        public void SendSms(string receiveName, string receiveNum, string content)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
            if (currentUser == null)
                throw new CustomException(404, "获取信息失败，请稍后再试!");
            if (string.IsNullOrEmpty(content))
                throw new CustomException(400, "发送短信内容不能为空!");
            if (content.Length > 300)
                throw new CustomException(400, "短信内容只能在300字数内！");
            int sendCount = Convert.ToInt32(Math.Ceiling((double)content.Length / 64));
            if (sendCount > businessman.SMS.RemainCount)
                throw new CustomException(400, "你的短信不足，请充值购买！");
            businessman.SendMessage(currentUser.OperatorName, receiveName, receiveNum, content);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("发送短信列表")]
        public DataPack<SendDetailDto> SendRecordByPage(int currentPageIndex, int? pageSize, DateTime? startTime, DateTime? endTime)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            Tuple<int, IList<SendDetail>> result = businessman.GetSendRecordByPage(currentPageIndex, pageSize, startTime, endTime);
            var dataPack = new DataPack<SendDetailDto>();
            dataPack.TotalCount = result.Item1;
            dataPack.List = result.Item2.Select(x =>
                new SendDetailDto
                {
                    ID = x.ID,
                    Content = x.Content,
                    Name = x.Name,
                    ReceiveNum = x.ReceiveNum,
                    ReceiveName = x.ReceiveName,
                    SendState = x.SendState,
                    SendTime = x.SendTime,
                    SendCount = x.SendCount
                }).ToList();
            return dataPack;
        }
        [ExtOperationInterceptor("购买短信记录")]
        public DataPack<BuyDetailDto> BuyRecordByPage(int currentPageIndex, int? pageSize, DateTime? startTime, DateTime? endTime, string outTradeNo = null)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            Tuple<int, IList<BuyDetail>> result = businessman.GetBuyRecordByPage(currentPageIndex, pageSize, startTime, endTime, outTradeNo);
            var dataPack = new DataPack<BuyDetailDto>();
            dataPack.TotalCount = result.Item1;
            dataPack.List = result.Item2.Select(x => new BuyDetailDto
            {
                ID = x.ID,
                BuyState = x.BuyState,
                BuyTime = x.BuyTime,
                Count = x.Count,
                Name = x.Name,
                PayAmount = x.PayAmount,
                PayNo = x.PayNo,
                PayTime = x.PayTime,
                PayWay = x.PayWay,
                OutTradeNo = x.OutPayNo
            }).ToList();
            return dataPack;
        }
        public bool IsExistBussinessmen(string code)
        {
            return businessmanRepository.FindAll(x => x.Code == code).Count() > 0;
        }

        public void AddBussinessmen(BusinessmanDataObject businessmanDataObject)
        {
            //var businessmanBuilder = AggregationFactory.CreateBuiler<BusinessmanBuilder>();
            //Businessman bm = businessmanBuilder.CreateBusinessman();
            //bm.Code = businessmanDataObject.Code;
            //bm.Name = businessmanDataObject.Name;
            //bm.CashbagCode = businessmanDataObject.CashbagCode;
            //bm.CashbagKey = businessmanDataObject.CashbagKey;
            //bm.ContactName = businessmanDataObject.ContactName;
            //bm.Phone = businessmanDataObject.Phone;
            //bm.ContactWay = new ContactWay
            //{
            //    Address = businessmanDataObject.Address,
            //    Contact = businessmanDataObject.Contact,
            //    Tel = businessmanDataObject.Tel
            //};
            //bm.NewOperator(new Operator
            //{
            //    Account = "admin",
            //    OperatorState = EnumOperatorState.Normal,
            //    Password = "123456",
            //    Realname = businessmanDataObject.Contact,
            //    Phone = businessmanDataObject.Tel
            //});
            //bm.CreateTime = DateTime.Now;
            //if (businessmanDataObject.Attachments != null)
            //{
            //    bm.Attachments = businessmanDataObject.Attachments
            //                                          .Select(p => new Attachment { Name = p.Name, Url = p.Url })
            //                                          .ToList();
            //}
            //unitOfWorkRepository.PersistCreationOf(bm);
            //unitOfWork.Commit();
        }

        public void ModifyBussinessmen(BusinessmanDataObject businessmanDataObject)
        {
            //var bm = businessmanRepository.FindAll(x => x.Code == businessmanDataObject.Code).SingleOrDefault();
            //if (bm == null)
            //    throw new CustomException(404, "操作的商户不存在!");
            //bm.Name = businessmanDataObject.Name;
            //bm.ContactWay.Address = businessmanDataObject.Address;
            //bm.ContactWay.Contact = businessmanDataObject.Contact;
            //bm.ContactWay.Tel = businessmanDataObject.Tel;
            //bm.ContactName = businessmanDataObject.ContactName;
            //bm.Phone = businessmanDataObject.Phone;
            //var attachments = businessmanDataObject.Attachments;
            //if (attachments == null || attachments.Count() == 0)
            //    bm.Attachments.Clear();
            //else
            //    attachments.ForEach(x =>
            //    {
            //        var model = bm.Attachments.Where(p => p.Name == x.Name).SingleOrDefault();
            //        if (model == null)
            //            bm.Attachments.Add(new Attachment
            //            {
            //                Name = x.Name,
            //                Url = x.Url
            //            });
            //        else
            //            model.Url = x.Url;
            //    });
            //unitOfWorkRepository.PersistUpdateOf(bm);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("购买短信【账户支付】")]
        public void BuySmsByAccount(int count, decimal smsPrice, int payAccountWay, string payPassword)
        {
            var currentUser = AuthManager.GetCurrentUser();
            string code = currentUser.Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();

            string InPayNo = businessman.GetPayNo();
            //  decimal smsPrice = SettingSection.GetInstances().Sms.SmsPrice;
            decimal payAmount = Math.Round((count * smsPrice), 2);
            string payResult = string.Empty;
            BuyDetail buyDetail = new BuyDetail
            {
                BuyTime = DateTime.Now,
                Count = count,
                Name = currentUser.OperatorName,
                PayNo = InPayNo,
                PayAmount = payAmount,
                PayFee = Math.Round(payAmount * SystemConsoSwitch.Rate, 2)
            };
            if (payAccountWay == 0)
            {
                buyDetail.PayWay = EnumPayMethod.Account;
                //payResult = payMentClientProxy.PaymentByCashAccount(currentUser.CashbagCode, currentUser.CashbagKey, InPayNo, "购买短信", payAmount, payPassword);
                var result = payMentClientProxy.PaymentByCashAccount(currentUser.CashbagCode, currentUser.CashbagKey, InPayNo, "购买短信", payAmount, payPassword);
                payResult = result.Item2;
            }
            else if (payAccountWay == 1)
            {
                buyDetail.PayWay = EnumPayMethod.Credit;
                //payResult = payMentClientProxy.PaymentByCreditAccount(currentUser.CashbagCode, currentUser.CashbagKey, InPayNo, "购买短信", payAmount, payPassword);
                var result =payMentClientProxy.PaymentByCreditAccount(currentUser.CashbagCode, currentUser.CashbagKey, InPayNo, "购买短信", payAmount, payPassword);
                payResult = result.Item2;
            }
            if (!string.IsNullOrEmpty(payResult))
            {
                buyDetail.OutPayNo = payResult;
                buyDetail.BuyState = EnumPayStatus.OK;
                businessman.SMS.Buy(count);
            }
            else
            {
                buyDetail.BuyState = EnumPayStatus.NoPay;
            }
            buyDetail.PayTime = DateTime.Now;
            businessman.BuySms(buyDetail);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("短信购买【银行卡支付】")]
        public string BuySmsByBank(int count, decimal smsPrice, string bankCode)
        {
            CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
            string code = currentUser.Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();

            var smsNotify = SettingSection.GetInstances().Payment.SmsNotify;

            string partnerKey = SettingSection.GetInstances().Cashbag.PartnerKey;
            //decimal smsPrice = SettingSection.GetInstances().Sms.SmsPrice;
            decimal payAmount = Math.Round((count * smsPrice), 2);
            string payNo = businessman.BuySms(currentUser.OperatorName, count, EnumPayMethod.Bank, payAmount);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
            return payMentClientProxy.PaymentByBank(currentUser.CashbagCode, currentUser.CashbagKey, payNo, "购买短信", payAmount, bankCode, smsNotify, businessman.Code);

        }
        [ExtOperationInterceptor("短信购买【平台支付】")]
        public string BuySmsByPlatform(int count, decimal smsPrice, string platformCode)
        {
            CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
            string code = currentUser.Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            var smsNotify = SettingSection.GetInstances().Payment.SmsNotify;

            string partnerKey = SettingSection.GetInstances().Cashbag.PartnerKey;
            // decimal smsPrice = SettingSection.GetInstances().Sms.SmsPrice;
            decimal payAmount = Math.Round((count * smsPrice), 2);
            string payNo = businessman.BuySms(currentUser.OperatorName, count, EnumPayMethod.Platform, payAmount);
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
            return payMentClientProxy.PaymentByPlatform(currentUser.CashbagCode, currentUser.CashbagKey, payNo, "购买短信", payAmount, platformCode, smsNotify, businessman.Code);
        }
        [ExtOperationInterceptor("获取商户基本信息和系统信息")]
        public Tuple<int, int, decimal> GetSystemInfo()
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            decimal smsPrice = SettingSection.GetInstances().Sms.SmsPrice;
            return Tuple.Create<int, int, decimal>(businessman.SMS.RemainCount, businessman.SMS.SendCount, smsPrice);
        }
        [ExtOperationInterceptor("账号是否重复")]
        public bool IsExistAccount(string account)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            return businessman.IsExistAccount(account);
        }
        [ExtOperationInterceptor("获取钱袋子余额信息")]
        public Tuple<decimal, decimal> GetRecieveAndCreditMoney()
        {
            CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
            if (currentUser == null)
                throw new CustomException(404, "获取信息失败，请重新登录");
            FundInfo fundInfo = payMentClientProxy.GetRecieveAndCreditMoney(currentUser.CashbagCode, currentUser.CashbagKey);
            if (fundInfo == null)
                throw new CustomException(400, "获取余额信息失败！");
            return Tuple.Create<decimal, decimal>(fundInfo.RecieveAmount, fundInfo.CreditMoney);
        }

        /// <summary>
        /// 获取采购商信息
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("获取采购商信息")]
        public CurrentUserInfoDto GetCurrentUserInfo()
        {
            CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
            var bm = businessmanRepository.FindAll(x => x.Code == currentUser.Code && x is Buyer).Select(p => p as Buyer).SingleOrDefault();
            var current = new CurrentUserInfoDto()
            {
                BusinessmanCode = currentUser.Code,
                BusinessmanName = currentUser.BusinessmanName,
                OperatorAccount = currentUser.OperatorAccount,
                OperatorName = currentUser.OperatorName,
                OperatorPhone = currentUser.OperatorPhone,
                Contact = bm.ContactWay.Contact,
                Tel = bm.ContactWay.Tel,
                CreateTime = bm.CreateTime,
                ContactName = bm.ContactName,
                Phone = bm.Phone
            };
            return current;
        }


        [ExtOperationInterceptor("获取商户名称")]
        public string GetBusinessmanName(string code)
        {
            var model = businessmanRepository.FindAll(p => p.Code == code).SingleOrDefault();
            if (model == null)
                throw new CustomException(404, "当前商户不能存在");
            return model.Name;
        }
        /// <summary>
        /// 采购商
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="cashbagCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("查找采购商")]
        public DataPack<BusinessmanDto> FindBusinessmen(string name, string code, string cashbagCode, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var dataPack = new DataPack<BusinessmanDto>();
            var query = this.businessmanRepository.FindAll(p => p is Buyer).Select(p => p as Buyer);
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(name.Trim()))
                query = query.Where(x => x.Name.Contains(name.Trim()));
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
                query = query.Where(x => x.Code == code.Trim());
            if (!string.IsNullOrEmpty(cashbagCode) && !string.IsNullOrEmpty(cashbagCode.Trim()))
                query = query.Where(x => x.CashbagCode == cashbagCode.Trim());
            if (startTime.HasValue)
                query = query.Where(x => x.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(x => x.CreateTime <= endTime.Value.AddDays(1));
            dataPack.TotalCount = query.Count();
            query = query.OrderByDescending(x => x.CreateTime).Skip(startIndex).Take(count);
            dataPack.List = query.ToList()
                                 .Select(x => new BusinessmanDto
                                 {
                                     Address = x.ContactWay.Address,
                                     Code = x.Code,
                                     Contact = x.ContactWay.Contact,
                                     CreateTime = x.CreateTime,
                                     Name = x.Name,
                                     RemainCount = x.SMS.RemainCount,
                                     SendCount = x.SMS.SendCount,
                                     Tel = x.ContactWay.Tel,
                                     CashbagCode = x.CashbagCode,
                                     CashbagKey = x.CashbagKey,
                                     IsEnable = x.IsEnable,
                                     ContactName = x.ContactName,
                                     Phone = x.Phone
                                 })
                                 .ToList();
            return dataPack;


        }

        /// <summary>
        /// 采购商
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("获得采购商信息")]
        public BusinessmanDataObject GetBusinessmanByCode(string code)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(code.Trim()))
                throw new CustomException(400, "未接收到商户号!");
            var model = businessmanRepository.FindAll(p => p.Code == code.Trim() && p is Buyer).Select(p => p as Buyer).SingleOrDefault();
            if (model == null)
                throw new CustomException(404, "获取商户信息不存在");
            var businessmanDto = new BusinessmanDataObject()
            {
                Address = model.ContactWay.Address,
                Attachments = model.Attachments.Select(p => new AttachmentDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Url = p.Url
                }).ToList(),
                CashbagCode = model.CashbagCode,
                IsEnable = model.IsEnable,
                CashbagKey = model.CashbagKey,
                Code = model.Code,
                Contact = model.ContactWay.Contact,
                CreateTime = model.CreateTime,
                Name = model.Name,
                Tel = model.ContactWay.Tel,
                ContactName = model.ContactName,
                Phone = model.Phone,
                Plane = model.Plane,
                BusinessTel  =model.ContactWay.BusinessTel
            };

            return businessmanDto;
        }

        public override void Dispose()
        {
            unitOfWork.Dispose();
        }
        [ExtOperationInterceptor("获取客服信息")]
        public CustomerDto GetCustomerInfo()
        {

            //var model = CustomerSection.GetInstances();
            //CustomerDto customerDto = new CustomerDto()
            //{
            //    AdvisoryQQ = model.AdvisoryQQs.OfType<KeyAndValueElement>().Select(p => new KeyAndValueDto() { Key = p.Key, Value = p.Value }).ToList(),
            //    HotlinePhone = model.HotlinePhones.OfType<KeyAndValueElement>().Select(p => new KeyAndValueDto() { Key = p.Key, Value = p.Value }).ToList(),
            //    //LinkMethod = model.LinkMethods.OfType<KeyAndValueElement>().Select(p => new KeyAndValueDto() { Key = p.Key, Value = p.Value }).ToList(),
            //    CustomPhone = model.CustomPhone
            //};
            var user = AuthManager.GetCurrentUser();

            var info = _customerInfoDomainService.GetCustomerInfo(true, user.CarrierCode);


            #region 对象转换

            CustomerDto customerDto = new CustomerDto();
            customerDto.AdvisoryQQ = new List<KeyAndValueDto>();
            customerDto.HotlinePhone = new List<KeyAndValueDto>();
            if (info != null)
            {
                customerDto.CustomPhone = info.CustomPhone;
                if (info.AdvisoryQQ != null)
                {
                    foreach (var data in info.AdvisoryQQ)
                    {
                        customerDto.AdvisoryQQ.Add(new KeyAndValueDto { Key = data.Description, Value = data.QQ });
                    }
                }
                if (info.HotlinePhone != null)
                {
                    foreach (var data in info.HotlinePhone)
                    {
                        customerDto.HotlinePhone.Add(new KeyAndValueDto { Key = data.Description, Value = data.Phone });
                    }
                }
            }
            #endregion

            return customerDto;
        }

        [ExtOperationInterceptor("获取服务时间")]
        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }




        [ExtOperationInterceptor("重置员工密码")]
        public void ResetPassword(string account)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取商户信息失败，请重新登录");
            var currentOp = model.Operators.Where(p => p.Account == account).FirstOrDefault();
            if (currentOp == null)
                throw new CustomException(400, "员工不存在");
            currentOp.Password = "123456".Md5();
            this.unitOfWorkRepository.PersistUpdateOf(model);
            this.unitOfWork.Commit();
        }

        [ExtOperationInterceptor("获取短信模板")]
        public List<SMSTemplateDto> GetAllSmsTemplate()
        {
            var code = AuthManager.GetCurrentUser().CarrierCode;
            var query = smsTemplateRepository.FindAll(x => x.State == true && (x.Code == code || x.IsSystemTemplate == true));
            List<SMSTemplateDto> listsmstemplate = new List<SMSTemplateDto>();
            listsmstemplate = query.ToList().OrderByDescending(x => x.CreateTime).Select(t => new SMSTemplateDto
            {
                Code = t.Code,
                CreateName = t.CreateName,
                CreateTime = t.CreateTime,
                ID = t.ID,
                TemplateContents = t.TemplateContents,
                IsSystemTemplate = t.IsSystemTemplate,
                State = t.State,
                LastOperTime = t.LastOperTime,
                TemplateName = t.TemplateName,
                SkyWayType = t.SkyWayType,
                TemplateType = t.TemplateType
            }).ToList();
            return listsmstemplate;
        }
        [ExtOperationInterceptor("获取短信费用设置")]
        public List<SMSChargeSetDto> GetAllChargeSet()
        {
            var query = this.smsChargeRepository.FindAll(p => p.State == true);//只有平台可设置
            List<SMSChargeSetDto> list = new List<SMSChargeSetDto>();
            list = query.ToList().OrderByDescending(p => p.CreateTime).Select(x => new SMSChargeSetDto
            {
                ID = x.ID,
                Price = x.Price,
                TotalPrice = x.TotalPrice,
                State = x.State,
                Count = x.Count,
                CreateTime = x.CreateTime
            }).ToList();
            return list;
        }

        public DataPack<GiveDetailDto> GetSmsGiveDetail(int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime)
        {
            var currentuser = AuthManager.GetCurrentUser();
            var model = businessmanRepository.FindAll(p => p.Code == currentuser.CarrierCode).FirstOrDefault();
            var query = model.GiveDetails.Where(p => p.GiveCode == currentuser.Code).AsQueryable();
            if (startTime.HasValue)
                query = query.Where(xx => xx.GiveTime >= startTime);
            if (endTime.HasValue)
                query = query.Where(xx => xx.GiveTime <= endTime);

            var list = query.OrderByDescending(o => o.GiveTime).Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();
            var dataPack = new DataPack<GiveDetailDto>();
            dataPack.TotalCount = query.Count();
            dataPack.List = AutoMapper.Mapper.Map<List<GiveDetail>, List<GiveDetailDto>>(list); ;
            return dataPack;
        }

        public void SendSmsForPhone(string OrderId, string Content, int[] PassengerId)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            CurrentUserInfo currentUser = AuthManager.GetCurrentUser();
            if (currentUser == null)
                throw new CustomException(404, "获取信息失败，请稍后再试!");
            if (string.IsNullOrEmpty(Content))
                throw new CustomException(400, "发送短信内容不能为空!");


            var mOrder = orderRepository.FindAll(x => x.OrderId == OrderId).FirstOrDefault();
            ChangeOrder mafterOrder = null;
            if (mOrder.HasAfterSale == true)
            {
                mafterOrder = afterSaleOrderRepository.FindAll(p => p.Order.OrderId == mOrder.OrderId && p is ChangeOrder).Select(p => p as ChangeOrder).FirstOrDefault();
            }
            PnrResource pnrResource = new PnrResource();
            string fromCity = null;
            string toCity = null;
            string flightTime = null;
            string arriveTime = null;
            string flightNumber = null;
            string flightCarrayCode = null;
            if (mOrder.SkyWays != null && mOrder.SkyWays.Count == 1)
            {
                //单程
                fromCity = pnrResource.GetCityInfo(mOrder.SkyWays[0].FromCityCode).city.Name;
                toCity = pnrResource.GetCityInfo(mOrder.SkyWays[0].ToCityCode).city.Name;
                flightTime = mOrder.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                arriveTime = mOrder.SkyWays[0].ToDateTime.ToString("HH:mm");
                flightNumber = mOrder.SkyWays[0].FlightNumber;
                flightCarrayCode = mOrder.SkyWays[0].CarrayCode;

                Content = Content.Replace("[航空公司航班号]", flightCarrayCode + flightNumber);
                Content = Content.Replace("[出发城市]", fromCity);
                Content = Content.Replace("[出发航站楼]", mOrder.SkyWays[0].FromTerminal);
                Content = Content.Replace("[到达城市]", toCity);
                Content = Content.Replace("[到达航站楼]", mOrder.SkyWays[0].ToTerminal);
                if (mafterOrder != null)
                {
                    flightTime = DateTime.Parse(mafterOrder.SkyWay[0].FlyDate.ToString()).ToString("yyyy-MM-dd HH:mm");
                }
                Content = Content.Replace("[起飞时间]", flightTime);
                Content = Content.Replace("[到达时间]", arriveTime);
            }
            else if (mOrder.SkyWays != null && mOrder.SkyWays.Count == 2)
            {
                //返程联程
                var fromCity1 = pnrResource.GetCityInfo(mOrder.SkyWays[0].FromCityCode).city.Name;
                var fromCity2 = pnrResource.GetCityInfo(mOrder.SkyWays[1].FromCityCode).city.Name;
                var toCity1 = pnrResource.GetCityInfo(mOrder.SkyWays[0].ToCityCode).city.Name;
                var toCity2 = pnrResource.GetCityInfo(mOrder.SkyWays[1].ToCityCode).city.Name;
                flightCarrayCode = mOrder.SkyWays[0].CarrayCode;
                flightNumber = mOrder.SkyWays[0].FlightNumber;
                flightTime = mOrder.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                arriveTime = mOrder.SkyWays[0].ToDateTime.ToString("HH:mm");
                var flightCarrayCodeBack = mOrder.SkyWays[1].CarrayCode;
                var flightNumberNameBack = mOrder.SkyWays[1].FlightNumber;

                var flightTimeBack = mOrder.SkyWays[1].StartDateTime.ToString("yyyy-MM-dd HH:mm");
                var arriveTimeBack = mOrder.SkyWays[1].ToDateTime.ToString("HH:mm");
                if (fromCity1 == toCity2 && toCity1 == fromCity2)
                {
                    //往返

                    Content = Content.Replace("[航空公司航班号]", flightCarrayCode + flightNumber);
                    Content = Content.Replace("[出发城市]", fromCity1);
                    Content = Content.Replace("[出发航站楼]", mOrder.SkyWays[0].FromTerminal);
                    Content = Content.Replace("[到达城市]", toCity1);
                    Content = Content.Replace("[到达航站楼]", mOrder.SkyWays[0].ToTerminal);
                    Content = Content.Replace("[起飞时间]", flightTime);
                    Content = Content.Replace("[到达时间]", arriveTime);

                    Content = Content.Replace("[回程航空公司航班号]", flightCarrayCodeBack + flightNumberNameBack);
                    Content = Content.Replace("[回程出发城市]", fromCity2);
                    Content = Content.Replace("[回程出发航站楼]", mOrder.SkyWays[1].FromTerminal);
                    Content = Content.Replace("[回程到达城市]", toCity2);
                    Content = Content.Replace("[回程到达航站楼]", mOrder.SkyWays[1].ToTerminal);
                    Content = Content.Replace("[回程起飞时间]", flightTimeBack);
                    Content = Content.Replace("[回程到达时间]", arriveTimeBack);
                }
                else
                {
                    //中转
                    Content = Content.Replace("[航空公司航班号]", flightCarrayCode + flightNumber);
                    Content = Content.Replace("[出发城市]", fromCity1);
                    Content = Content.Replace("[出发航站楼]", mOrder.SkyWays[0].FromTerminal);
                    Content = Content.Replace("[到达城市]", toCity1);
                    Content = Content.Replace("[到达航站楼]", mOrder.SkyWays[0].ToTerminal);
                    Content = Content.Replace("[起飞时间]", flightTime);
                    Content = Content.Replace("[到达时间]", arriveTime);

                    Content = Content.Replace("[中转航空公司航班号]", flightCarrayCodeBack + flightNumberNameBack);
                    Content = Content.Replace("[中转出发城市]", fromCity2);
                    Content = Content.Replace("[中转出发航站楼]", mOrder.SkyWays[1].FromTerminal);
                    Content = Content.Replace("[中转到达城市]", toCity2);
                    Content = Content.Replace("[中转到达航站楼]", mOrder.SkyWays[1].ToTerminal);
                    Content = Content.Replace("[回程起飞时间]", flightTimeBack);
                    Content = Content.Replace("[回程到达时间]", arriveTimeBack);
                }
            }


            for (int i = 0; i < mOrder.Passengers.Where(p => PassengerId.Contains(p.Id)).Count(); i++)
            {
                Content = Content.Replace("[乘客名称]", mOrder.Passengers[i].PassengerName);
                Content = Content.Replace("[证件号]", mOrder.Passengers[i].CardNo);
                if (Content.Length > 300)
                    throw new CustomException(400, "短信内容只能在300字数内！");
                int sendCount = Convert.ToInt32(Math.Ceiling((double)Content.Length / 64));
                if (sendCount > businessman.SMS.RemainCount)
                    throw new CustomException(400, "你的短信不足，请充值购买！");
                businessman.SendMessage(currentUser.OperatorName, mOrder.Passengers[i].PassengerName, mOrder.Passengers[i].Mobile, Content);
            }

            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
        }

        [ExtOperationInterceptor("获取短信模板by行程类型）")]
        public List<SMSTemplateDto> GetAllSmsTemplateForPhone(int SkyWayType)
        {
            var code = AuthManager.GetCurrentUser().CarrierCode;
            var query = smsTemplateRepository.FindAll(x => x.State == true && (x.Code == code || x.IsSystemTemplate == true) && (int)x.SkyWayType == SkyWayType);
            List<SMSTemplateDto> listsmstemplate = new List<SMSTemplateDto>();
            listsmstemplate = query.ToList().OrderByDescending(x => x.CreateTime).Select(t => new SMSTemplateDto
            {
                Code = t.Code,
                CreateName = t.CreateName,
                CreateTime = t.CreateTime,
                ID = t.ID,
                TemplateContents = t.TemplateContents,
                IsSystemTemplate = t.IsSystemTemplate,
                State = t.State,
                LastOperTime = t.LastOperTime,
                TemplateName = t.TemplateName
            }).ToList();
            return listsmstemplate;
        }

        /// <summary>
        /// 发送一般信息
        /// </summary>
        /// <param name="content">信息内容</param>
        /// <param name="isRepeatSend">是否重复发送</param>
        [ExtOperationInterceptor("发送一般信息）")]
        public void SendNormalMsg(string content, bool isRepeatSend = false)
        {
            MessagePushManager.SendAll(EnumPushCommands.NormalMsg, content, isRepeatSend);

        }
    }
}
