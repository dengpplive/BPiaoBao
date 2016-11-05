using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.AppServices.StationContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using StructureMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class BusinessmanService : IStationBusinessmanService
    {
        IAccountClientProxy accountClientProxy = ObjectFactory.GetInstance<IAccountClientProxy>();

       
        public void AddBussinessmen(BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.RequestCarrier carrier)
        {
            if (carrier == null)
                throw new CustomException(501, "输入信息不完整");
            var curentUser = AuthManager.GetCurrentUser();
            var isexist = this.businessmanRepository.FindAll(p => p.Code == carrier.Code).Count() > 0;
            if (isexist)
                throw new CustomException(500, string.Format("{0}已存在", carrier.Code));
            var builder = AggregationFactory.CreateBuiler<BusinessmanBuilder>();
            var carrierModel = builder.CreateCarrier(AutoMapper.Mapper.Map<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.RequestCarrier, Carrier>(carrier));

            carrierModel.CheckRule();
            var cashbagModel = accountClientProxy.AddCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
              {
                  ClientAccount = carrier.Code,
                  Contact = carrier.ContactWay.Contact,
                  CpyName = carrier.Name,
                  Moblie = carrier.ContactWay.Tel,
                  Province = carrier.ContactWay.Province,
                  City = carrier.ContactWay.City,
                  Address = carrier.ContactWay.Address
              });
            try
            {
                carrierModel.CashbagCode = cashbagModel.PayAccount;
                carrierModel.CashbagKey = cashbagModel.Token;
                this.unitOfWorkRepository.PersistCreationOf(carrierModel);
                this.unitOfWork.Commit();
            }
            catch (Exception e)
            {
                if (cashbagModel != null)
                    accountClientProxy.DeleteCashBagBusinessman(curentUser.CashbagCode, cashbagModel.PayAccount, curentUser.CashbagKey);
                Logger.WriteLog(LogType.ERROR, "添加商户发生异常", e);
                throw new CustomException(500, "添加商户发生异常");
            }
        }


        //public void AddSupplier(BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.STRequestSupplier supplier)
        //{
        //    if (supplier == null)
        //        throw new CustomException(501, "输入信息不完整");
        //    var curentUser = AuthManager.GetCurrentUser();
        //    var isexist = this.businessmanRepository.FindAll(p => p.Code == supplier.Code).Count() > 0;
        //    if (isexist)
        //        throw new CustomException(500, string.Format("{0}已存在", supplier.Code));
        //    var builder = AggregationFactory.CreateBuiler<BusinessmanBuilder>();
        //    var carrierModel = builder.CreateSupplier(AutoMapper.Mapper.Map<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.STRequestSupplier, Supplier>(supplier));

        //    carrierModel.CheckRule();
        //    var cashbagModel = accountClientProxy.AddCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
        //    {
        //        ClientAccount = supplier.Code,
        //        Contact = supplier.ContactWay.Contact,
        //        CpyName = supplier.Name,
        //        Moblie = supplier.ContactWay.Tel,
        //        Province = supplier.ContactWay.Province,
        //        City = supplier.ContactWay.City,
        //        Address = supplier.ContactWay.Address
        //    });
        //    try
        //    {
        //        carrierModel.CashbagCode = cashbagModel.PayAccount;
        //        carrierModel.CashbagKey = cashbagModel.Token;
        //        this.unitOfWorkRepository.PersistCreationOf(carrierModel);
        //        this.unitOfWork.Commit();
        //    }
        //    catch (Exception e)
        //    {
        //        if (cashbagModel != null)
        //            accountClientProxy.DeleteCashBagBusinessman(curentUser.CashbagCode, cashbagModel.PayAccount, curentUser.CashbagKey);
        //        Logger.WriteLog(LogType.ERROR, "添加商户发生异常", e);
        //        throw new CustomException(500, "添加商户发生异常");
        //    }
        //}


        public bool IsExistCarrier(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new CustomException(500, "商户号不能为空");
            return this.businessmanRepository.FindAll(x => x.Code == code).Count() > 0;
        }

        public PagedList<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ResponseListCarrierOrSupplier> FindCarrierOrSupplier(string name, string code, string cashbagCode, DateTime? startTime, DateTime? endTime, int startIndex, int count,int type)
        {
           var query  = this.businessmanRepository.FindAll();
            if (type == 0)
               query = query.Where(x=>(x is Carrier));
            if( type ==1)
                query = query.Where(x => (x is Supplier));
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(name.Trim()))
                query = query.Where(p => p.Name.Contains(name.Trim()));
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
                query = query.Where(p => p.Code == code.Trim());
            if (!string.IsNullOrEmpty(cashbagCode) && !string.IsNullOrEmpty(cashbagCode.Trim()))
                query = query.Where(p => p.CashbagCode == cashbagCode.Trim());
            if (startTime.HasValue)
                query = query.Where(p => p.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateTime <= endTime.Value);
            var list = query.OrderByDescending(p => p.CreateTime).Skip(startIndex).Take(count).Select(p => new BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ResponseListCarrierOrSupplier
            {
                Code = p.Code,
                Address = p.ContactWay.Address,
                CashbagCode = p.CashbagCode,
                CashbagKey = p.CashbagKey,
                Contact = p.ContactWay.Contact,
                CreateTime = p.CreateTime,
                IsEnable = p.IsEnable,
                Name = p.Name,
                Tel = p.ContactWay.Tel
            }).ToList();
            return new PagedList<StationContracts.SystemSetting.SystemMap.ResponseListCarrierOrSupplier>
            {
                Total = query.Count(),
                Rows = list
            };
        }

        public ResponseDetailCarrier GetCarrierByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new CustomException(500, "商户号不能空");
            var carrierModel = this.businessmanRepository.FindAll(p => p.Code == code).OfType<Carrier>().FirstOrDefault();

            if (carrierModel != null)
            {
                ResponseDetailCarrier response = AutoMapper.Mapper.Map<Carrier, ResponseDetailCarrier>(carrierModel);

                var user = AuthManager.GetCurrentUser();

                var info = _customerInfoDomainService.GetCustomerInfo(false, code);

                #region 对象转换

                if (info != null)
                {
                    response.CustomPhone = info.CustomPhone;
                    if (info.AdvisoryQQ != null)
                    {
                        foreach (var data in info.AdvisoryQQ)
                        {
                            response.AdvisoryQQ += data.Description + "QQ:" + data.QQ + "、";
                        }
                    }
                    if (info.HotlinePhone != null)
                    {
                        foreach (var data in info.HotlinePhone)
                        {
                            response.HotlinePhone += data.Description + "电话:" + data.Phone + "、";
                        }
                    }
                }
                #endregion
                return response;
            }
            throw new CustomException(500, "不是运营类型");
        }

        public StationContracts.SystemSetting.SystemMap.ResponseDetailSupplier GetSupplierByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new CustomException(500, "商户号不能空");
            var carrierModel = this.businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();

            if (carrierModel is Supplier)
            {
                var response = AutoMapper.Mapper.Map<Supplier, BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ResponseDetailSupplier>(carrierModel as Supplier);
                return response;
            }
            throw new CustomException(500, "不是供应商类型");
        }

        public void ModifyCarrier(StationContracts.SystemSetting.SystemMap.RequestCarrier carrier)
        {
            if (carrier == null)
                throw new CustomException(501, "输入信息不完整");
            var carrierModel = this.businessmanRepository.FindAll(p => p.Code == carrier.Code).Select(p => p as Carrier).FirstOrDefault();
            if (carrierModel == null)
                throw new CustomException(500, "查找商户不存在");
            var curentUser = AuthManager.GetCurrentUser();
            accountClientProxy.UpdateCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
            {
                ClientAccount = carrier.Code,
                CpyName = carrier.Name,
                Contact = carrier.ContactWay.Contact,
                Moblie = carrier.ContactWay.Tel,
                Province = carrier.ContactWay.Province,
                City = carrier.ContactWay.City,
                Address = carrier.ContactWay.Address,
                PayAccount = carrierModel.CashbagCode
            });
            carrierModel.ContactWay = AutoMapper.Mapper.Map<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ContactWayDataObject, ContactWay>(carrier.ContactWay);
            carrierModel.Pids.Clear();
            if (carrier.Pids != null && carrier.Pids.Count > 0)
            {
                carrier.Pids.ForEach(p =>
                {
                    carrierModel.Pids.Add(AutoMapper.Mapper.Map<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.PIDDataObject, PID>(p));
                });
            }
            carrierModel.Rate = carrier.Rate;
            carrierModel.RemoteRate = carrier.RemoteRate;
            carrierModel.LocalPolicySwitch = carrier.LocalPolicySwitch;
            carrierModel.InterfacePolicySwitch = carrier.InterfacePolicySwitch;
            carrierModel.ShowLocalCSCSwich = carrier.ShowLocalCSCSwich;
            carrierModel.ForeignRemotePolicySwich = carrier.ForeignRemotePolicySwich;
            carrierModel.BuyerRemotoPolicySwich = carrier.BuyerRemotoPolicySwich;
            unitOfWorkRepository.PersistUpdateOf(carrierModel);
            unitOfWork.Commit();
        }
        public void ModifySupplier(StationContracts.SystemSetting.SystemMap.STRequestSupplier supplier)
        {
            if (supplier == null)
                throw new CustomException(501, "输入信息不完整");
            var carrierModel = this.businessmanRepository.FindAll(p => p.Code == supplier.Code).Select(p => p as Supplier).FirstOrDefault();
            if (carrierModel == null)
                throw new CustomException(500, "查找商户不存在");
            var curentUser = AuthManager.GetCurrentUser();
            accountClientProxy.UpdateCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
            {
                ClientAccount = supplier.Code,
                CpyName = supplier.Name,
                Contact = supplier.ContactWay.Contact,
                Moblie = supplier.ContactWay.Tel,
                Province = supplier.ContactWay.Province,
                City = supplier.ContactWay.City,
                Address = supplier.ContactWay.Address,
                PayAccount = carrierModel.CashbagCode
            });
            carrierModel.ContactWay = AutoMapper.Mapper.Map<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ContactWayDataObject, ContactWay>(supplier.ContactWay);
            carrierModel.SupPids.Clear();
            if (supplier.Pids != null && supplier.Pids.Count > 0)
            {
                supplier.Pids.ForEach(p =>
                {
                    carrierModel.SupPids.Add(AutoMapper.Mapper.Map<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.PIDDataObject, PID>(p));
                });
            }
            carrierModel.SupRate = supplier.SupRate;
            carrierModel.SupRemoteRate = supplier.SupRemoteRate;
            carrierModel.SupLocalPolicySwitch = supplier.SupLocalPolicySwitch;
            carrierModel.SupRemotePolicySwitch = supplier.SupRemotePolicySwitch;
            unitOfWorkRepository.PersistUpdateOf(carrierModel);
            unitOfWork.Commit();
        }

        public void ResetAdminPassword(string code)
        {
            var Model = businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (Model == null)
                throw new CustomException(404, "操作的商户号不存在!");
            var op = Model.Operators.Where(p => p.IsAdmin == true).FirstOrDefault();
            if (op == null)
                throw new CustomException(404, "未找到该商户号的管理员!");
            op.Password = "123456".Md5();
            unitOfWorkRepository.PersistUpdateOf(Model);
            unitOfWork.Commit();
        }

        public void EnableAndDisable(string code)
        {
            var carrierModel = businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (carrierModel == null)
                throw new CustomException(404, "操作的商户号不存在!");
            carrierModel.IsEnable = !carrierModel.IsEnable;
            unitOfWorkRepository.PersistUpdateOf(carrierModel);
            unitOfWork.Commit();
        }

        public void SendMessage(string jsonStr, string contentTemplate)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonStr))
                    return;
                MatchCollection matchs = Regex.Matches(contentTemplate, @"(\[(?<val>.*?)\])+");
                List<string> properties = new List<string>();
                foreach (Match item in matchs)
                {
                    properties.Add(item.Groups["val"].Value);
                }


                Newtonsoft.Json.Linq.JArray objList = Newtonsoft.Json.Linq.JArray.Parse(jsonStr);
                foreach (var item in objList)
                {
                    string contentMessage = contentTemplate;
                    foreach (string propertyName in properties)
                    {
                        contentMessage = contentMessage.Replace(string.Format("[{0}]", propertyName), item[propertyName].ToString());
                    }
                    MessagePushManager.SendMsgByBuyerCodes(new string[] { item["Code"].ToString() }, (EnumPushCommands)Convert.ToInt32(item["command"]), contentMessage, true);
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "发送消息异常", e);
            }


        }

        public ResponseBusinessman GetBusinessmanByCashBagCode(string code)
        {
            var model = this.businessmanRepository.FindAll(p => p.CashbagCode == code).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "查找商户不存在");
            ResponseBusinessman rb = new ResponseBusinessman();
            rb.Code = model.Code;
            rb.ContactName = model.ContactName;
            rb.Name = model.Name;
            rb.Phone = model.Phone;
            if (model.ContactWay != null)
                rb.ContactWay = new ContactWayDataObject
                {
                    Address = model.ContactWay.Address,
                    Contact = model.ContactWay.Contact,
                    Tel = model.ContactWay.Tel
                };
            return rb;
        }

        public PagedList<ResponseListBuyer> FindBuyerList(string code, string carriercode, string cashbagCode, string tel, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var pagedlist = new PagedList<ResponseListBuyer>();
            var query = this.businessmanRepository.FindAll(p => p is Buyer).Select(p => p as Buyer);
            if (!string.IsNullOrEmpty(carriercode) && !string.IsNullOrEmpty(carriercode.Trim()))
                query = query.Where(x => x.CarrierCode == carriercode.Trim());
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
                query = query.Where(x => x.Code == code.Trim());
            if (!string.IsNullOrEmpty(cashbagCode) && !string.IsNullOrEmpty(cashbagCode.Trim()))
                query = query.Where(x => x.CashbagCode == cashbagCode.Trim());
            if (!string.IsNullOrWhiteSpace(tel))
                query = query.Where(x => x.ContactWay.Tel == tel.Trim());
            if (startTime.HasValue)
                query = query.Where(x => x.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(x => x.CreateTime <= endTime.Value);
            pagedlist.Total = query.Count();
            query = query.OrderByDescending(x => x.CreateTime).Skip(startIndex).Take(count);
            pagedlist.Rows = query.ToList()
                                 .Select(x => new ResponseListBuyer
                                 {
                                     Address = x.ContactWay.Address,
                                     Code = x.Code,
                                     CarrierCode = x.CarrierCode,
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
                                     Phone = x.Phone,
                                     Plane = x.Plane
                                 })
                                 .ToList();
            return pagedlist;
        }


        #region 短信汇总报表
        public PagedList<ResponseSMSSum> GetSMSSum(string code, string businessName, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            var query = this.businessmanRepository.FindAll();
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(p => p.Code == code);
            if (!string.IsNullOrWhiteSpace(businessName))
                query = query.Where(p => p.Name == businessName);
            if (startTime.HasValue)
                query = query.Where(p => p.BuyDetails.Where(w => w.BuyTime > startTime).Count() > 0 && p.SendDetails.Where(s => s.SendTime > startTime).Count() > 0);
            if (endTime.HasValue)
                query = query.Where(p => p.BuyDetails.Where(w => w.BuyTime <= endTime).Count() > 0 && p.SendDetails.Where(s => s.SendTime <= endTime).Count() > 0);

            List<ResponseSMSSum> list = new List<ResponseSMSSum>();
            query.Select(p => new { p.SendDetails, p.BuyDetails, p.Name, p.Code, p.SMS.RemainCount }).ToList().AsParallel().ForEach(x =>
            {
                var querysend = x.SendDetails.AsQueryable().Where(p => p.SendState == true);
                var querybuy = x.BuyDetails.AsQueryable().Where(p => p.BuyState == EnumPayStatus.OK);
                if (startTime.HasValue)
                {
                    querysend = querysend.Where(p => p.SendTime > startTime);
                    querybuy = querybuy.Where(p => p.BuyTime > startTime);
                }
                if (endTime.HasValue)
                {
                    querysend = querysend.Where(p => p.SendTime <= endTime);
                    querybuy = querybuy.Where(p => p.BuyTime <= endTime);
                }
                list.Add(new ResponseSMSSum()
                {
                    Code = x.Code,
                    BusinessName = x.Name,
                    BuyCount = querybuy.Sum(p => p.Count),
                    BuyMoney = querybuy.Sum(p => p.PayAmount),
                    BuyTimes = querybuy.Count(),
                    UseCount = querysend.Sum(p => p.SendCount),
                    RemainCount = x.RemainCount
                });
            });
            list.Add(new ResponseSMSSum()
            {
                Code = "",
                BusinessName = "合计:",
                BuyCount = list.Sum(p => p.BuyCount),
                BuyMoney = list.Sum(p => p.BuyMoney),
                BuyTimes = list.Sum(p => p.BuyTimes),
                UseCount = list.Sum(p => p.UseCount),
                RemainCount = list.Sum(p => p.RemainCount)
            });
            return new PagedList<ResponseSMSSum>()
            {
                Total = list.Count(),
                Rows = list.OrderByDescending(p => p.BuyCount).Skip(startIndex).Take(count).ToList()
            };
        }

        public List<ResponseSMSSendSum> GetSMSSendSum(DateTime? startTime, DateTime? endTime)
        {
            var query = this.businessmanRepository.FindAll();

            if (startTime.HasValue)
                query = query.Where(p => p.SendDetails.Where(s => s.SendTime > startTime).Count() > 0);
            if (endTime.HasValue)
                query = query.Where(p => p.SendDetails.Where(s => s.SendTime <= endTime).Count() > 0);

            List<ResponseSMSSendSum> list = new List<ResponseSMSSendSum>();
            query.Select(p => new { p.SendDetails }).ToList().AsParallel().ForEach(x =>
            {
                var querysend = x.SendDetails.AsQueryable().Where(p => p.SendState == true);
                if (startTime.HasValue)
                    querysend = querysend.Where(p => p.SendTime > startTime);
                if (endTime.HasValue)
                    querysend = querysend.Where(p => p.SendTime <= endTime);

                foreach (var item in querysend.ToLookup(q => q.SendTime.ToString("yyyy-MM-dd")).ToList())
                {
                    list.Add(new ResponseSMSSendSum()
                    {
                        Count = item.Sum(ss => ss.SendCount),
                        DateTime = item.Key
                    });
                }
            });
            List<ResponseSMSSendSum> listsum = new List<ResponseSMSSendSum>();
            foreach (var item in list.OrderByDescending(o => o.DateTime).ToLookup(g => g.DateTime))
            {
                listsum.Add(new ResponseSMSSendSum()
                {
                    Count = item.Sum(ss => ss.Count),
                    DateTime = item.Key
                });
            }

            return listsum;
        }

        public List<ResponseSMSSaleSum> GetSMSSaleSum(DateTime? startTime, DateTime? endTime)
        {
            var query = this.businessmanRepository.FindAll();

            if (startTime.HasValue)
                query = query.Where(p => p.BuyDetails.Where(s => s.BuyTime > startTime).Count() > 0);
            if (endTime.HasValue)
                query = query.Where(p => p.BuyDetails.Where(s => s.BuyTime <= endTime).Count() > 0);

            List<ResponseSMSSaleSum> list = new List<ResponseSMSSaleSum>();
            query.Select(p => new { p.BuyDetails }).ToList().AsParallel().ForEach(x =>
            {
                var querysend = x.BuyDetails.AsQueryable().Where(p => p.BuyState == EnumPayStatus.OK);
                if (startTime.HasValue)
                    querysend = querysend.Where(p => p.BuyTime > startTime);
                if (endTime.HasValue)
                    querysend = querysend.Where(p => p.BuyTime <= endTime);

                foreach (var item in querysend.ToLookup(q => q.BuyTime.ToString("yyyy-MM-dd")).ToList().AsParallel())
                {
                    list.Add(new ResponseSMSSaleSum()
                    {
                        DateTime = item.Key,
                        AccountMoney = item.Where(w => w.PayWay == EnumPayMethod.Account).Sum(s => s.PayAmount),
                        AccountPoundage = item.Where(w => w.PayWay == EnumPayMethod.Account).Sum(s => s.PayFee),
                        CreditMoney = item.Where(w => w.PayWay == EnumPayMethod.Credit).Sum(s => s.PayAmount),
                        CreditPoundage = item.Where(w => w.PayWay == EnumPayMethod.Credit).Sum(s => s.PayFee),
                        TenPayMoney = item.Where(w => w.PayWay == EnumPayMethod.Bank || w.PayWay == EnumPayMethod.TenPay).Sum(s => s.PayAmount),
                        TenPayPoundage = item.Where(w => w.PayWay == EnumPayMethod.Bank || w.PayWay == EnumPayMethod.TenPay).Sum(s => s.PayFee),
                        AliPayMoney = item.Where(w => w.PayWay == EnumPayMethod.AliPay).Sum(s => s.PayAmount),
                        AliPayPoundage = item.Where(w => w.PayWay == EnumPayMethod.AliPay).Sum(s => s.PayFee),
                        TotalMoney = item.Sum(s => s.PayAmount),
                        TotalPoundage = item.Sum(s => s.PayFee)

                    });
                }
            });
            List<ResponseSMSSaleSum> listsum = new List<ResponseSMSSaleSum>();
            foreach (var item in list.OrderByDescending(o => o.DateTime).ToLookup(g => g.DateTime))
            {
                listsum.Add(new ResponseSMSSaleSum()
                {
                    DateTime = item.Key,
                    AccountMoney = item.Sum(s => s.AccountMoney),
                    AccountPoundage = item.Sum(s => s.AccountPoundage),
                    CreditMoney = item.Sum(s => s.CreditMoney),
                    CreditPoundage = item.Sum(s => s.CreditPoundage),
                    AliPayMoney = item.Sum(s => s.AliPayMoney),
                    AliPayPoundage = item.Sum(s => s.AliPayPoundage),
                    TenPayMoney = item.Sum(s => s.TenPayMoney),
                    TenPayPoundage = item.Sum(s => s.TenPayPoundage),
                    TotalMoney = item.Sum(s => s.TotalMoney),
                    TotalPoundage = item.Sum(s => s.TotalPoundage)
                });
            }
            listsum.Add(new ResponseSMSSaleSum()
            {
                DateTime = "合计:",
                AccountMoney = listsum.Sum(s => s.AccountMoney),
                AccountPoundage = listsum.Sum(s => s.AccountPoundage),
                CreditMoney = listsum.Sum(s => s.CreditMoney),
                CreditPoundage = listsum.Sum(s => s.CreditPoundage),
                AliPayMoney = listsum.Sum(s => s.AliPayMoney),
                AliPayPoundage = listsum.Sum(s => s.AliPayPoundage),
                TenPayMoney = listsum.Sum(s => s.TenPayMoney),
                TenPayPoundage = listsum.Sum(s => s.TenPayPoundage),
                TotalMoney = listsum.Sum(s => s.TotalMoney),
                TotalPoundage = listsum.Sum(s => s.TotalPoundage)
            });
            return listsum;
        }
        #endregion

        #region OPEN票扫描信息
        public PagedList<ResponseOPENScan> GetOpenScanList(int startIndex, int count)
        {
            var currentuser = AuthManager.GetCurrentUser();
            var query = openScanRepository.FindAllNoTracking().Where(p => p.Operator == currentuser.OperatorAccount);
            var list = query.OrderByDescending(o => o.CreateTime).Skip((startIndex - 1) * count).Take(count).ToList();
            return new PagedList<ResponseOPENScan>()
            {
                Total = list.Count(),
                Rows = AutoMapper.Mapper.Map<List<OPENScan>, List<ResponseOPENScan>
                >(list)
            };
        }

        public void AddOpenScanInfo(RequestOPENScan OpenScan)
        {
            var model = AutoMapper.Mapper.Map<RequestOPENScan, OPENScan>(OpenScan);
            model.CreateTime = DateTime.Now;
            model.Operator = AuthManager.GetCurrentUser().OperatorAccount;
            model.State = EnumOPEN.NoScan;
            this.unitOfWorkRepository.PersistCreationOf(model);
            this.unitOfWork.Commit();
        }

        public PIDDataObject GetCarrierPid(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new CustomException(500, "商户号不能空");
            var carrierModel = this.businessmanRepository.FindAll(p => p.Code == code && p is Carrier).FirstOrDefault();
            return AutoMapper.Mapper.Map<PID, PIDDataObject>((carrierModel as Carrier).Pids.FirstOrDefault());
        }
        #endregion


        public Tuple<string, byte[]> DownloadOpenFileByName(string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Export", fileName);
            if (!File.Exists(path))
                return Tuple.Create<string, byte[]>("文件不存在", null);
            StreamReader sr = new StreamReader(path);
            Stream stream = sr.BaseStream;
            byte[] resultStream = new byte[stream.Length];
            stream.Read(resultStream, 0, resultStream.Length);
            stream.Seek(0, SeekOrigin.Begin);
            sr.Close();
            return Tuple.Create<string, byte[]>(string.Empty, resultStream);
        }

        [ExtOperationInterceptor("控台修改客服中心")]
        public void SetCustomerInfo(CustomerDto customerInfo)
        {
            if (customerInfo == null)
            {
                throw new CustomException(30001, "客服中心信息不可为空。");
            }
            //if (!customerInfo.CustomPhone.IsMatch(StringExpend.PhonePattern))
            //{
            //    throw new CustomException(30001, customerInfo.HotlinePhone + "不是合法的电话。");
            //}
            //if (customerInfo.AdvisoryQQ.Count(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value)) > 0)
            //    throw new CustomException(30001, "QQ及QQ描述均必须填写。");
            //if (customerInfo.HotlinePhone.Count(c => string.IsNullOrWhiteSpace(c.Key) || string.IsNullOrWhiteSpace(c.Value)) > 0)
            //    throw new CustomException(30001, "电话及电话描述均必须填写。");

            CustomerInfo info = new CustomerInfo();
            info.AdvisoryQQ = new List<QQInfo>();
            info.HotlinePhone = new List<PhoneInfo>();

            info.CustomPhone = customerInfo.CustomPhone;
            if (customerInfo.AdvisoryQQ != null)
            {
                foreach (var data in customerInfo.AdvisoryQQ)
                {
                    info.AdvisoryQQ.Add(new QQInfo { Description = data.Key, QQ = data.Value });
                }
            }
            if (customerInfo.HotlinePhone != null)
            {
                foreach (var data in customerInfo.HotlinePhone)
                {
                    info.HotlinePhone.Add(new PhoneInfo { Description = data.Key, Phone = data.Value });
                }
            }


            _customerInfoDomainService.SetStationCustomerInfo(info);
        }

        [ExtOperationInterceptor("控台获取客服中心")]
        public CustomerDto GetStationCustomerInfo()
        {
            var user = AuthManager.GetCurrentUser();

            var info = _customerInfoDomainService.GetCustomerInfo(false, null);

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


        public PagedList<SupplierDataObj> FindSupplier(string code, string carriercode, DateTime? startTime, DateTime? endTime, int startIndex, int count)
        {
            //var query = this.businessmanRepository.FindAll(p => (p is Supplier));
            //if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
            //    query = query.Where(p => p.Code == code.Trim());
            //if (!string.IsNullOrEmpty(carriercode) && !string.IsNullOrEmpty(carriercode.Trim()))
            //    query = query.Where(p => p.CashbagCode == carriercode.Trim());
            //if (startTime.HasValue)
            //    query = query.Where(p => p.CreateTime >= startTime.Value);
            //if (endTime.HasValue)
            //    query = query.Where(p => p.CreateTime <= endTime.Value);
            //var list = query.OrderByDescending(p => p.CreateTime).Skip(startIndex).Take(count).Select(p => new BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ResponseListCarrier
            //{
            //    Code = p.Code,
            //    Rate = p.ra
            //    Address = p.ContactWay.Address,
            //    CashbagCode = p.CashbagCode,
            //    CashbagKey = p.CashbagKey,
            //    Contact = p.ContactWay.Contact,
            //    CreateTime = p.CreateTime,
            //    IsEnable = p.IsEnable,
            //    Name = p.Name,
            //    Tel = p.ContactWay.Tel
            //}).ToList();
            //return new PagedList<StationContracts.SystemSetting.SystemMap.ResponseListCarrier>
            //{
            //    Total = query.Count(),
            //    Rows = list
            //};
            throw new NotImplementedException();
        }

        public void ModifySupplierSwitch(SupplierDataObj supplierDto)
        {
            throw new NotImplementedException();
        }


        public SupplierDataObj FindSupplierInfoByCode(string code)
        {
            throw new NotImplementedException();
        }


        public IList<StationBuyerGroupDto> SearchStationBuyerGroups()
        {
            var datas = _stationBuyGroupDomainService.QueryStationBuyGroups().ToList();
            IList<StationBuyerGroupDto> records = new List<StationBuyerGroupDto>();
            foreach (var data in datas)
            {
                StationBuyerGroupDto record = new StationBuyerGroupDto();
                record.ID = data.ID;
                record.GroupName = data.GroupName;
                record.Description = data.Description;
                record.Color = data.Color;
                record.LastOperatorUser = data.LastOperatorUser;
                record.LastOperatTime = data.LastOperatTime;

                records.Add(record);
            }

            return records;
        }

        public void SetBuyerToStationBuyerGroup(SetBuyerToStationBuyerGroupRequest request)
        {
            _stationBuyGroupDomainService.SetBuyerToGroup(request.BuyerCode, request.GroupID);
        }

        public void DeleteStationBuyreGroup(string groupId)
        {
            _stationBuyGroupDomainService.DeleteStationBuyGroup(groupId);
        }

        public void SetStationBuyerGroupInfo(SetStationBuyerGroupInfoRequest dto)
        {
            StationBuyGroup group = new StationBuyGroup();
            group.ID = dto.ID;
            group.GroupName = dto.GroupName;
            group.Description = dto.Description;
            group.Color = dto.Color;

            _stationBuyGroupDomainService.UpdateStationBuyGroup(group, AuthManager.GetCurrentUser().OperatorAccount);

            //_stationBuyGroupDomainService.UpdateStationBuyGroup()
        }

        public void AddStationBuyerGroup(AddStationBuyerGroupRequest request)
        {
            StationBuyGroup group = new StationBuyGroup();
            group.GroupName = request.GroupName;
            group.Description = request.Description;
            group.Color = request.Color;

            _stationBuyGroupDomainService.AddStationBuyGroup(group, AuthManager.GetCurrentUser().OperatorAccount);
        }
    }
}
