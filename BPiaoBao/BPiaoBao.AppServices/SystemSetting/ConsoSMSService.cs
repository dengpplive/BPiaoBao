using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework.SMS;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using BPiaoBao.Common.Enums;
using System.Text.RegularExpressions;
using BPiaoBao.Common;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class BusinessmanService : IConsoSMSService
    {
        #region 短信模板
        public void AddSmsTemplate(SmsTemplateDataObject smstemplate)
        {
            if (smstemplate == null)
                throw new CustomException(500, "输入信息不完整");
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            if (currentCode != "")
            {
                var cmodel = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
                if (cmodel == null)
                    throw new CustomException(404, "获取信息失败，请重新登录");
            }
            var template = smsTemplateRepository.FindAll(p => p.Code == currentCode && p.TemplateName == smstemplate.TemplateName).FirstOrDefault();
            if (template != null)
                throw new CustomException(500, "模板名重复，不能添加");
            var model = AutoMapper.Mapper.Map<SmsTemplateDataObject, SMSTemplate>(smstemplate);
            model.Code = currentCode;
            model.CreateTime = DateTime.Now;
            model.CreateName = AuthManager.GetCurrentUser().OperatorAccount;
            model.LastOperTime = DateTime.Now;
            unitOfWorkRepository.PersistCreationOf(model);
            unitOfWork.Commit();
        }

        public PagedList<ResponseSmsTemplate> GetSmsTemplateList(int currentPageIndex, int pageSize)
        {
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            var list = smsTemplateRepository.FindAll(x => x.Code == currentCode || x.IsSystemTemplate == true).ToList();
            List<ResponseSmsTemplate> listsmstemplate = new List<ResponseSmsTemplate>();
            list.OrderByDescending(x => x.CreateTime).ForEach(t =>
            {
                listsmstemplate.Add(new ResponseSmsTemplate()
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
                });
            });
            return new PagedList<ResponseSmsTemplate>
            {
                Total = listsmstemplate.Count(),
                Rows = listsmstemplate.AsQueryable().Skip(currentPageIndex).Take(pageSize).ToList()
            };
        }


        public void EditSmsTemplate(SmsTemplateDataObject smstemplate)
        {
            var model = this.smsTemplateRepository.FindAll(p => p.ID == smstemplate.ID).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "操作模板不存在");
            model.TemplateName = smstemplate.TemplateName;
            model.TemplateContents = smstemplate.TemplateContents;
            model.SkyWayType = smstemplate.SkyWayType;
            model.TemplateType = smstemplate.TemplateType;
            model.LastOperTime = DateTime.Now;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public void DeleteSmsTemplate(int Id)
        {
            var model = this.smsTemplateRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作模板不存在");
            unitOfWorkRepository.PersistDeletionOf(model);
            unitOfWork.Commit();
        }


        public void SmsTemplateEnableOrDisable(int Id)
        {
            var model = smsTemplateRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作模板不存在!");
            model.State = !model.State;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public ResponseSmsTemplate GetSmsTemplatebyId(int Id)
        {
            var model = this.smsTemplateRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "操作模板不存在");
            return AutoMapper.Mapper.Map<SMSTemplate, ResponseSmsTemplate>(model);
        }
        #endregion

        #region 短信记录
        public PagedList<SendDetailDataObj> GetSendRecordByPage(string name, string tel, int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime, string businessName = "")
        {
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            var query = businessmanRepository.FindAll();
            if (currentCode.Length != 0)
                query = query.Where(x => x.Code == currentCode);
            if (!string.IsNullOrEmpty(businessName))
                query = query.Where(x => x.Name.Contains(businessName.Trim()));
            if (!string.IsNullOrEmpty(tel))
                query = query.Where(x => x.SendDetails.Where(t => t.ReceiveNum.Contains(tel.Trim())).Count() > 0);
            if (startTime.HasValue)
                query = query.Where(x => x.SendDetails.Where(t => t.SendTime >= startTime.Value).Count() > 0);
            if (endTime.HasValue)
                query = query.Where(x => x.SendDetails.Where(t => t.SendTime < endTime.Value).Count() > 0);


            List<SendDetailDataObj> listSend = new List<SendDetailDataObj>();

            query.Select(s => new { s.SendDetails, s.Name }).ToList().ForEach(x =>
            {
                var querysend = x.SendDetails.AsQueryable();
                if (!string.IsNullOrEmpty(name))
                    querysend = querysend.Where(xx => xx.Name.Contains(name));
                if (!string.IsNullOrEmpty(tel))
                    querysend = querysend.Where(xx => xx.ReceiveNum.Contains(tel.Trim()));
                if (startTime.HasValue)
                    querysend = querysend.Where(xx => xx.SendTime >= startTime);
                if (endTime.HasValue)
                    querysend = querysend.Where(xx => xx.SendTime <= endTime);
                querysend.ToList().ForEach(t =>
                    {
                        listSend.Add(new SendDetailDataObj()
                        {
                            ID = t.ID,
                            BusinessName = x.Name,
                            Name = t.Name,
                            ReceiveName = t.ReceiveName,
                            ReceiveNum = t.ReceiveNum,
                            Content = t.Content,
                            SendCount = t.SendCount,
                            SendState = t.SendState,
                            SendTime = t.SendTime
                        });
                    });

            });
            //List<SendDetailDataObj> list = query.Project().To<SendDetailDataObj>().ToList();
            return new PagedList<SendDetailDataObj>
            {
                Total = listSend.Count(),
                Rows = listSend.OrderByDescending(x => x.SendTime).Skip(currentPageIndex).Take(pageSize).ToList()
            };
        }

        public PagedList<BuyDetailDataObj> GetBuyRecordByPage(string name, int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime, string businessName = "")
        {
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            var query = businessmanRepository.FindAll();
            if (currentCode.Length != 0)
                query = query.Where(x => x.Code == currentCode);
            if (!string.IsNullOrEmpty(businessName))
                query = query.Where(x => x.Name.Contains(businessName.Trim()));
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.BuyDetails.Where(t => t.Name == name).Count() > 0);
            if (startTime.HasValue)
                query = query.Where(x => x.BuyDetails.Where(t => t.BuyTime >= startTime.Value).Count() > 0);
            if (endTime.HasValue)
                query = query.Where(x => x.BuyDetails.Where(t => t.BuyTime < endTime.Value).Count() > 0);

            List<BuyDetailDataObj> listbuy = new List<BuyDetailDataObj>();
            query.Select(s => new { s.Name, s.BuyDetails }).ToList().ForEach(x =>
            {
                var querybuy = x.BuyDetails.AsQueryable();
                if (!string.IsNullOrEmpty(name))
                    querybuy = querybuy.Where(xx => xx.Name.Contains(name));
                if (startTime.HasValue)
                    querybuy = querybuy.Where(xx => xx.BuyTime >= startTime);
                if (endTime.HasValue)
                    querybuy = querybuy.Where(xx => xx.BuyTime <= endTime);
                querybuy.ToList().ForEach(t =>
                {
                    listbuy.Add(new BuyDetailDataObj()
                    {
                        ID = t.ID,
                        BusinessName = x.Name,
                        Name = t.Name,
                        BuyState = t.BuyState,
                        BuyTime = t.BuyTime,
                        Count = t.Count,
                        OutTradeNo = t.OutPayNo,
                        PayAmount = t.PayAmount,
                        PayNo = t.PayNo,
                        PayTime = t.PayTime,
                        PayWay = t.PayWay
                    });
                });
            });
            //List<BuyDetailDataObj> list = query.Project().To<BuyDetailDataObj>().ToList();
            return new PagedList<BuyDetailDataObj>
            {
                Total = listbuy.Count(),
                Rows = listbuy.OrderByDescending(x => x.BuyTime).Skip(currentPageIndex).Take(pageSize).ToList()
            };
        }
        #endregion

        #region 短信赠送
        public PagedList<GiveDetailDataObj> GetGiveDetailByPage(string giveName, int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime)
        {
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            var query = businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            var querygive = query.GiveDetails.AsQueryable();
            if (!string.IsNullOrEmpty(giveName))
                querygive = querygive.Where(x => x.GiveName.Contains(giveName));
            if (startTime.HasValue)
                querygive = querygive.Where(x => x.GiveTime >= startTime);
            if (endTime.HasValue)
                querygive = querygive.Where(x => x.GiveTime <= endTime);
            List<GiveDetailDataObj> list = new List<GiveDetailDataObj>();
            if (query != null)
            {
                list = querygive.Select(x => new GiveDetailDataObj
                {
                    GiveCode = x.GiveCode,
                    GiveCount = x.GiveCount,
                    GiveName = x.GiveName,
                    GiveTime = x.GiveTime,
                    Remark = x.Remark
                }).ToList();
            }
            return new PagedList<GiveDetailDataObj>
            {
                Total = list.Count(),
                Rows = list.OrderByDescending(x => x.GiveTime).Skip(currentPageIndex).Take(pageSize).ToList()
            };
        }


        public void AddSmsGive(GiveDetailDataObj givedetail)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var bm = this.businessmanRepository.FindAll(p => p.Code == currentCode && p is Carrier).FirstOrDefault();
            if (bm.SMS.RemainCount < givedetail.GiveCount)
                throw new CustomException(500, "当前短信剩余条数不足!");

            bm.SMS.RemainCount -= givedetail.GiveCount;//运营减
            var model = AutoMapper.Mapper.Map<GiveDetailDataObj, GiveDetail>(givedetail);
            model.GiveTime = DateTime.Now;
            bm.GiveDetails.Add(model);

            var bmreceive = this.businessmanRepository.FindAll(p => p.Code == givedetail.GiveCode && p is Buyer).FirstOrDefault();
            if (bmreceive == null)
                throw new CustomException(400, "赠送人不存在!");
            bmreceive.SMS.RemainCount += givedetail.GiveCount;//采购加
            this.unitOfWorkRepository.PersistUpdateOf(bmreceive);
            this.unitOfWorkRepository.PersistUpdateOf(bm);
            this.unitOfWork.Commit();
        }

        public string GetSmsRemainCount()
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var bm = this.businessmanRepository.FindAll(p => p.Code == currentCode && p is Carrier).FirstOrDefault();
            return bm.SMS.RemainCount.ToString();
        }
        #endregion

        #region 短信费用设置
        public PagedList<SMSChargeSetDataObj> GetSmsChargeSetByPage(int currentPageIndex, int pageSize)
        {
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            var query = this.smsChargeRepository.FindAll(p => p.Code == currentCode);
            List<SMSChargeSetDataObj> list = query.Project().To<SMSChargeSetDataObj>().ToList();
            //list = query.FirstOrDefault().Select(x => new SMSChargeSetDataObj { 
            //    ID = x.ID,
            //    Price = x.Price,
            //    TotalPrice = x.TotalPrice,
            //    State = x.State,
            //    Count = x.Count,
            //    CreateTime = x.CreateTime
            //}).ToList();
            return new PagedList<SMSChargeSetDataObj>()
            {
                Total = list.Count(),
                Rows = list.OrderByDescending(x => x.CreateTime).Skip(currentPageIndex).Take(pageSize).ToList()
            };
        }

        public void AddSmsChargeSet(SMSChargeSetDataObj smschargeset)
        {
            if (smschargeset == null)
                throw new CustomException(500, "输入信息不完整");
            string currentCode = AuthManager.GetCurrentUser().Code == null ? "" : AuthManager.GetCurrentUser().Code;
            var model = AutoMapper.Mapper.Map<SMSChargeSetDataObj, SMSChargeSet>(smschargeset);
            model.Code = currentCode;
            model.CreateTime = DateTime.Now;
            model.State = true;
            this.unitOfWorkRepository.PersistCreationOf(model);
            this.unitOfWork.Commit();
        }

        public void DeleteSmsChargeSet(int Id)
        {
            var model = this.smsChargeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作列不存在");
            unitOfWorkRepository.PersistDeletionOf(model);
            unitOfWork.Commit();
        }

        public void EditSmsChargeSet(SMSChargeSetDataObj smschargeset)
        {
            var model = this.smsChargeRepository.FindAll(p => p.ID == smschargeset.ID).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "操作列不存在");
            model.Price = smschargeset.Price;
            model.TotalPrice = smschargeset.Price * smschargeset.Count;
            model.Count = smschargeset.Count;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }

        public SMSChargeSetDataObj GetSmsChargeSetById(int Id)
        {
            var model = this.smsChargeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "操作列不存在");
            return AutoMapper.Mapper.Map<SMSChargeSet, SMSChargeSetDataObj>(model);
        }
        public void SmsChargeSetEnableOrDisable(int Id)
        {
            var model = smsChargeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作列不存在!");
            model.State = !model.State;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }
        #endregion

        #region 短信购买
        public List<SMSChargeSetDataObj> GetBuySmsChargeSetByPage()
        {
            var query = this.smsChargeRepository.FindAll(p => p.Code == "");
            List<SMSChargeSetDataObj> list = query.Project().To<SMSChargeSetDataObj>().ToList();
            return list;
        }


        public bool BuySmsByCashAccount(int ChargeSetId, string pwd)
        {
            bool rs = false;
            var currentUser = AuthManager.GetCurrentUser();
            var bm = this.businessmanRepository.FindAll(p => p.Code == currentUser.Code && p is Carrier).FirstOrDefault();
            var mchargeset = this.smsChargeRepository.FindAll(p => p.ID == ChargeSetId).FirstOrDefault();
            var payNo = bm.GetPayNo();
            BuyDetail smsbuy = new BuyDetail
            {
                Count = mchargeset.Count,
                PayAmount = mchargeset.TotalPrice,
                PayWay = EnumPayMethod.Account,
                PayNo = payNo,
                BuyTime = DateTime.Now,
                Name = currentUser.OperatorName,
                PayFee = Math.Round(mchargeset.TotalPrice * SystemConsoSwitch.Rate, 2)
            };
            //string outpaynum = payMentClientProxy.PaymentByCashAccount(currentUser.CashbagCode, currentUser.CashbagKey, payNo, "短信购买", mchargeset.TotalPrice, pwd);
            var reslut = payMentClientProxy.PaymentByCashAccount(currentUser.CashbagCode, currentUser.CashbagKey, payNo, "短信购买", mchargeset.TotalPrice, pwd);
            string outpaynum = reslut.Item2;
            if (!string.IsNullOrEmpty(outpaynum))
            {
                bm.SMS.RemainCount += mchargeset.Count;
                smsbuy.BuyState = EnumPayStatus.OK;
                smsbuy.OutPayNo = outpaynum;
                rs = true;
            }
            smsbuy.PayTime = DateTime.Now;
            bm.BuyDetails.Add(smsbuy);
            this.unitOfWorkRepository.PersistUpdateOf(bm);
            this.unitOfWork.Commit();
            return rs;
        }
        #endregion


        public string SmsSend(SendDetailDataObj SendDetailobj)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var businessman = businessmanRepository.FindAll(x => x.Code == code).SingleOrDefault();
            var currentUser = AuthManager.GetCurrentUser();
            if (currentUser == null)
                throw new CustomException(404, "获取信息失败，请稍后再试!");
            if (string.IsNullOrEmpty(SendDetailobj.Content))
                throw new CustomException(400, "发送短信内容不能为空!");
            if (SendDetailobj.Content.Length > 300)
                throw new CustomException(400, "短信内容只能在300字数内！");

            string[] phones = SendDetailobj.ReceiveNum.ToString().Replace("\r\n", "").Trim().Replace('，', ',').Split(new char[] { ',' });
            if (phones.Length > 0)
            {
                for (int i = 0; i < phones.Length; i++)
                {
                    if (!Regex.IsMatch(phones[i], "(^1[358]\\d{9}$)"))
                    {
                        throw new CustomException(400, "手机号格式有误:" + phones[i]);
                    }
                    else if (phones.Length > 100)
                    {
                        throw new CustomException(400, "手机号码最多为20个!");
                    }
                }
            }
            int sendCount = Convert.ToInt32(Math.Ceiling((double)SendDetailobj.Content.Length / 64));
            if (sendCount * phones.Length > businessman.SMS.RemainCount)
                throw new CustomException(400, "你的短信余额不足，请充值购买！");
            int countok = 0, counterr = 0, sendokcount = 0;
            for (int i = 0; i < phones.Length; i++)
            {
                var model = AutoMapper.Mapper.Map<SendDetailDataObj, SendDetail>(SendDetailobj);
                model.SendTime = DateTime.Now;
                model.Name = currentUser.OperatorName;
                model.SendCount = sendCount;
                model.ReceiveNum = phones[i].ToString();
                int sendrs = new SMSDefaultService().SendSms(phones[i], SendDetailobj.Content);
                if (sendrs > 0)
                {
                    model.SendState = true;
                    countok++;
                    sendokcount += sendCount;
                }
                else
                {
                    counterr++;
                }
                businessman.SendDetails.Add(model);
            }
            businessman.SMS.RemainCount -= sendokcount;
            businessman.SMS.SendCount += sendokcount;
            unitOfWorkRepository.PersistUpdateOf(businessman);
            unitOfWork.Commit();
            return "成功发送:" + countok + "条(计费" + sendokcount + "条),发送失败:" + counterr + "条";
        }
    }
}
