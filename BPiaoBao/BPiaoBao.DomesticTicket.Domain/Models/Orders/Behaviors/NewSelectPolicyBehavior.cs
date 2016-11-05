using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using System;
﻿using BPiaoBao.DomesticTicket.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
using StructureMap;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using JoveZhao.Framework.Expand;
using System.Diagnostics;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using JoveZhao.Framework;
using BPiaoBao.Common;
using PnrAnalysis;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using System.Threading;
using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 选择政策操作
    /// </summary>
    [Behavior("NewSelectPolicy")]
    public class NewSelectPolicyBehavior : BaseOrderBehavior
    {
        string OldOutOrderId = string.Empty;
        public override object Execute()
        {
            //记录时间
            StringBuilder sbLog = new StringBuilder();
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                string platformCode = getParame("platformCode").ToString();
                string policyId = getParame("policyId").ToString();
                string operatorName = getParame("operatorName").ToString();
                string source = getParame("source").ToString();
                PolicyDto policyUI = source != "back" ? (getParame("policy") as PolicyDto) : null;

                decimal TotlePaidPirce = 0m;
                OldOutOrderId = order.OutOrderId;
                string innerPlatformCode = "系统";
                DataBill databill = new DataBill();
                DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
                UserRelation userRealtion = domesticService.GetUserRealtion(order.BusinessmanCode);
                bool IspolicyIsNull = false;

                if (policyUI != null)
                {
                    //赋值
                    order.Policy = PolicyDtoPolicy(policyUI, order.Policy, source);
                }
                else
                {
                    #region 原获取政策
                    PolicyParam policyParam = new PolicyParam();
                    policyParam.code = order.BusinessmanCode;
                    policyParam.PnrContent = order.PnrContent;
                    policyParam.OrderId = order.OrderId;
                    policyParam.OrderType = order.OrderType;
                    policyParam.OrderSource = order.OrderSource;
                    policyParam.IsChangePnrTicket = order.IsChangePnrTicket;
                    policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
                    Passenger pasData = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                    if (pasData != null)
                    {
                        policyParam.defFare = pasData.SeatPrice.ToString();
                        policyParam.defTAX = pasData.ABFee.ToString();
                        policyParam.defRQFare = pasData.RQFee.ToString();
                    }
                    if (order.Policy == null)
                    {
                        IspolicyIsNull = true;
                        order.Policy = new Policy();
                    }
                    Policy localPolicy = null;
                    PlatformPolicy policy = null;
                    PlatformOrder platformOrder = null;
                    PolicyService policyService = ObjectFactory.GetInstance<PolicyService>();
                    watch.Stop();
                    sbLog.AppendFormat("初始话变量时间:{0}\r\n", watch.Elapsed.ToString());
                    watch.Restart();

                    if (platformCode != innerPlatformCode)
                    {
                        PnrData pnrData = PnrHelper.GetPnrData(order.PnrContent);
                        policy = PlatformFactory.GetPlatformByCode(platformCode).GetPoliciesByPnrContent(order.PnrContent, order.IsLowPrice, pnrData).Find((p) => p.Id == policyId);
                        watch.Stop();
                        sbLog.AppendFormat("0.调用方法【GetPlatformByCode】用时:{0}\r\n", watch.Elapsed.ToString());
                        watch.Restart();

                        if (policy == null)
                        {
                            localPolicy = GetLocalPolicy(policyParam, policyId, innerPlatformCode, userRealtion, IspolicyIsNull, policyService, localPolicy);
                            watch.Stop();
                            sbLog.AppendFormat("1.调用方法【GetLocalPolicy】用时:{0}\r\n", watch.Elapsed.ToString());
                            watch.Restart();
                            if (localPolicy != null)
                            {
                                order.Policy = localPolicy;
                            }
                            else
                            {
                                throw new OrderCommException("政策发生变动,请重新获取政策！！！");
                            }
                        }
                        else
                        {
                            SetInterface(platformCode, operatorName, source, ref TotlePaidPirce, policy, userRealtion, ref platformOrder, pnrData);
                            watch.Stop();
                            sbLog.AppendFormat("2.调用方法【SetInterface】用时:{0}\r\n", watch.Elapsed.ToString());
                            watch.Restart();
                        }
                    }
                    else
                    {
                        localPolicy = GetLocalPolicy(policyParam, policyId, innerPlatformCode, userRealtion, IspolicyIsNull, policyService, localPolicy);
                        watch.Stop();
                        sbLog.AppendFormat("3.调用方法【GetLocalPolicy】用时:{0}\r\n", watch.Elapsed.ToString());
                        watch.Restart();
                        if (localPolicy == null)
                        {
                            PnrData pnrData = PnrHelper.GetPnrData(order.PnrContent);
                            localPolicy = policyService.GetInterfacePolicy(policyParam, "", userRealtion, pnrData).Find((p) => p.PolicyId == policyId);
                            watch.Stop();
                            sbLog.AppendFormat("4.调用方法【GetInterfacePolicy】用时:{0}\r\n", watch.Elapsed.ToString());
                            watch.Restart();
                            if (localPolicy != null)
                            {
                                policy.AreaCity = localPolicy.AreaCity;
                                policy.Id = localPolicy.PolicyId;
                                policy.PolicyPoint = localPolicy.PolicyPoint;
                                if (source != "back")
                                {
                                    policy.PolicyPoint = localPolicy.PaidPoint;
                                }
                                policy.ReturnMoney = localPolicy.ReturnMoney;
                                policy.IsLow = localPolicy.IsLow;
                                policy.SeatPrice = localPolicy.SeatPrice;
                                policy.ABFee = localPolicy.ABFee;
                                policy.RQFee = localPolicy.RQFee;
                                policy.Remark = localPolicy.Remark;
                                policy.IsChangePNRCP = localPolicy.IsChangePNRCP;
                                policy.IsSp = localPolicy.IsSp;
                                policy.PolicyType = localPolicy.PolicyType;
                                policy.WorkTime = localPolicy.WorkTime;
                                policy.ReturnTicketTime = localPolicy.ReturnTicketTime;
                                policy.AnnulTicketTime = localPolicy.AnnulTicketTime;
                                policy.CPOffice = localPolicy.CPOffice;
                                policy.IssueSpeed = localPolicy.IssueSpeed;
                                TotlePaidPirce = platformOrder.TotlePaidPirce;

                                SetInterface(platformCode, operatorName, source, ref TotlePaidPirce, policy, userRealtion, ref platformOrder, pnrData);
                                watch.Stop();
                                sbLog.AppendFormat("5.调用方法【GetInterfacePolicy】用时:{0}\r\n", watch.Elapsed.ToString());
                                watch.Restart();
                            }
                            else
                            {
                                if (localPolicy == null)
                                    throw new OrderCommException("政策发生变动,请重新获取政策！");
                            }
                        }
                        else
                        {
                            //赋值
                            order.Policy = localPolicy;
                        }
                    }
                    #endregion
                }


                decimal _OrderMoney = 0m;
                if (source != "back")
                {
                    //扣点组类型
                    DeductionType deductionType = order.Policy.PolicySourceType == EnumPolicySourceType.Local ? DeductionType.Local : (order.Policy.PolicySourceType == EnumPolicySourceType.Share ? DeductionType.Share : DeductionType.Interface);
                    if (policyUI != null)
                    {
                        order.Policy.PolicyPoint = order.Policy.PaidPoint;
                    }
                    PlatformDeductionParam pfDp = new PlatformDeductionParam();
                    foreach (SkyWay leg in order.SkyWays)
                    {
                        pfDp.FlyLineList.Add(new FlyLine()
                        {
                            CarrayCode = leg.CarrayCode,
                            FromCityCode = leg.FromCityCode,
                            ToCityCode = leg.ToCityCode
                        });
                    }
                    //匹配扣点规则               
                    domesticService.MatchDeductionRole(order.Policy, pfDp, order.SkyWays[0].CarrayCode, userRealtion.deductionGroup, userRealtion, deductionType);
                    watch.Stop();
                    sbLog.AppendFormat("6.调用方法【MatchDeductionRole】用时:{0}\r\n", watch.Elapsed.ToString());
                    watch.Restart();

                    //佣金
                    order.Policy.Commission = databill.GetCommission(order.Policy.PolicyPoint, order.Policy.SeatPrice, order.Policy.ReturnMoney);
                    //单人支付金额 根据选择的政策 设置价格
                    for (int i = 0; i < order.Passengers.Count; i++)
                    {
                        Passenger p = order.Passengers[i];
                        if (p.PassengerType != EnumPassengerType.Baby)
                        {
                            p.SeatPrice = order.Policy.SeatPrice;
                            p.ABFee = order.Policy.ABFee;
                            p.RQFee = order.Policy.RQFee;
                            p.PayMoney = databill.GetPayPrice(order.Policy.SeatPrice, order.Policy.ABFee, order.Policy.RQFee, order.Policy.PolicyPoint, order.Policy.ReturnMoney);
                        }
                        else
                        {
                            p.PayMoney = databill.GetPayPrice(p.SeatPrice, p.ABFee, p.RQFee, 0, 0);
                        }
                    }
                    _OrderMoney = order.Passengers.Sum(p => p.PayMoney);
                    order.OrderCommissionTotalMoney = order.Passengers.Sum(p => p.PassengerType != EnumPassengerType.Baby ? databill.GetCommission(order.Policy.PolicyPoint, order.Policy.SeatPrice, order.Policy.ReturnMoney) : 0);
                }

                #region 支付信息
                if (order.OrderPay == null)
                    order.OrderPay = new OrderPay();

                order.OrderPay.OrderId = order.OrderId;
                order.OrderPay.PaidMoney = TotlePaidPirce;
                if (source != "back")
                {
                    order.OrderPay.PayMoney = _OrderMoney;
                    order.OrderMoney = _OrderMoney;
                    order.OrderPay.PayStatus = EnumPayStatus.NoPay;
                }
                order.OrderPay.PaidStatus = EnumPaidStatus.NoPaid;
                order.OrderPay.TradePoundage = 0m;
                order.OrderPay.SystemFee = 0m;
                order.CpOffice = order.Policy.CPOffice;
                #endregion


                #region 根据政策扣点明细计算支付分润
                if (source != "back")
                {
                    domesticService.CreateBillDetails(order, userRealtion);
                    watch.Stop();
                    sbLog.AppendFormat("7.调用方法【CreateBillDetails】用时:{0}\r\n", watch.Elapsed.ToString());
                }
                #endregion
                order.WriteLog(new OrderLog()
                {
                    OperationContent = string.Format("{0}选择政策,政策:{1},编号:{2},订单号:{3},出票速度:{4}", source, order.Policy.PolicyPoint, order.Policy.PolicyId, order.OrderId, order.Policy.IssueSpeed),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName,
                    IsShowLog = false
                });

                order.WriteLog(new OrderLog()
                {
                    OperationContent = string.Format("选择政策,订单号:{0},出票速度:{1}", order.OrderId, order.Policy.IssueSpeed),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName,
                    IsShowLog = true
                });

                if (source == "back")
                {
                    //order.ChangeStatus(EnumOrderStatus.WaitAndPaid);
                    order.ChangeStatus(EnumOrderStatus.PayWaitCreatePlatformOrder);
                }
                else
                {
                    order.ChangeStatus(EnumOrderStatus.NewOrder);
                }
            }
            finally
            {
                if (sbLog.ToString() != "")
                {
                    new CommLog().WriteLog("NewSelectPolicyBehavior", sbLog.ToString());
                }
            }
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                order.Policy.Code = string.Empty;
                order.Policy.Name = string.Empty;
                order.Policy.CashbagCode = string.Empty;
            }
            return null;
        }

        private void SetInterface(string platformCode, string operatorName, string source, ref decimal TotlePaidPirce, PlatformPolicy policy, UserRelation userRealtion, ref PlatformOrder platformOrder, PnrData pnrData)
        {
            #region 接口政策信息

            order.Policy.AreaCity = policy.AreaCity;
            order.Policy.PolicyId = policy.Id;
            order.Policy.PlatformCode = platformCode;
            order.Policy.OriginalPolicyPoint = policy.PolicyPoint;
            order.Policy.DownPoint = 0m;
            order.Policy.PaidPoint = policy.PolicyPoint;
            if (source != "back")
            {
                order.Policy.PolicyPoint = order.Policy.PaidPoint;
            }
            order.Policy.ReturnMoney = policy.ReturnMoney;
            order.Policy.IsLow = policy.IsLow;
            order.Policy.SeatPrice = policy.SeatPrice;
            order.Policy.ABFee = policy.ABFee;
            order.Policy.RQFee = policy.RQFee;

            order.Policy.Remark = policy.Remark;
            order.Policy.IsChangePNRCP = policy.IsChangePNRCP;
            order.Policy.IsSp = policy.IsSp;
            order.Policy.PolicyType = policy.PolicyType;
            order.Policy.WorkTime = policy.WorkTime;
            order.Policy.ReturnTicketTime = policy.ReturnTicketTime;
            order.Policy.AnnulTicketTime = policy.AnnulTicketTime;
            order.Policy.CPOffice = policy.CPOffice;
            order.Policy.OrderId = order.OrderId;
            order.Policy.PolicySourceType = EnumPolicySourceType.Interface;
            order.Policy.CarryCode = order.SkyWays[0].CarrayCode;
            order.Policy.IssueSpeed = policy.IssueSpeed;
            order.Policy.TodayGYCode = policy.TodayGYCode;
            #endregion

        }

        private Policy GetLocalPolicy(PolicyParam policyParam, string policyId, string innerPlatformCode, UserRelation userRealtion, bool IspolicyIsNull, PolicyService policyService, Policy localPolicy)
        {
            PnrData pnrData = PnrHelper.GetPnrData(order.PnrContent);
            if (IspolicyIsNull)
            {
                //全部查询匹配一下
                localPolicy = policyService.GetLocalPolicy(userRealtion, innerPlatformCode, policyParam, pnrData).Find((p) => p.PolicyId == policyId);
                if (localPolicy == null)
                {
                    localPolicy = policyService.GetSharePolicy(userRealtion, innerPlatformCode, policyParam, pnrData).Find((p) => p.PolicyId == policyId);
                }
                if (localPolicy == null && policyId.StartsWith(userRealtion.carrier.Code + "_"))
                {
                    //本地 默认
                    localPolicy = policyService.GetDefaultPolicy(userRealtion, innerPlatformCode, policyParam, pnrData, policyId).FirstOrDefault();
                }
            }
            else
            {
                //非接口重新获取政策               
                //本地
                localPolicy = policyService.GetLocalPolicy(userRealtion, innerPlatformCode, policyParam, pnrData).Find((p) => p.PolicyId == policyId);
                if (localPolicy == null)
                {
                    //共享 异地
                    localPolicy = policyService.GetSharePolicy(userRealtion, innerPlatformCode, policyParam, pnrData).Find((p) => p.PolicyId == policyId);
                }
                if (localPolicy == null)
                {
                    if (policyId.StartsWith(userRealtion.carrier.Code + "_"))
                    {
                        //本地 默认
                        localPolicy = policyService.GetDefaultPolicy(userRealtion, innerPlatformCode, policyParam, pnrData, policyId).FirstOrDefault();
                    }
                }
            }
            return localPolicy;
        }




        private Policy PolicyDtoPolicy(PolicyDto policyDto, Policy policy, string source)
        {
            Policy rePolicy = null;
            if (policy == null)
            {
                rePolicy = new Policy();
                rePolicy.PaidPoint = policyDto.PaidPoint;
                rePolicy.OriginalPolicyPoint = policyDto.OriginalPolicyPoint;
            }
            else
            {
                rePolicy = policy;
                if (source != "back")
                {
                    rePolicy.PaidPoint = policyDto.PaidPoint;
                    rePolicy.OriginalPolicyPoint = policyDto.OriginalPolicyPoint;
                }
            }
            rePolicy.PolicyId = policyDto.Id;
            rePolicy.Commission = policyDto.Commission;
            rePolicy.AreaCity = policyDto.AreaCity;
            rePolicy.PlatformCode = policyDto.PlatformCode;
            rePolicy.PolicyPoint = policyDto.Point;
            rePolicy.DownPoint = policyDto.DownPoint;



            rePolicy.IsChangePNRCP = policyDto.IsChangePNRCP;
            rePolicy.EnumIssueTicketWay = (EnumIssueTicketWay)Enum.Parse(typeof(EnumIssueTicketWay), policyDto.IssueTicketWay);
            rePolicy.IsSp = policyDto.IsSp;
            rePolicy.PolicyType = policyDto.PolicyType;
            string[] strTime = policyDto.WorkTime.Split('-');
            if (strTime != null && strTime.Length == 2)
            {
                rePolicy.WorkTime = new StartAndEndTime()
                {
                    StartTime = strTime[0],
                    EndTime = strTime[1]
                };
            }
            strTime = policyDto.AnnulTicketTime.Split('-');
            if (strTime != null && strTime.Length == 2)
            {
                rePolicy.AnnulTicketTime = new StartAndEndTime()
                {
                    StartTime = strTime[0],
                    EndTime = strTime[1]
                };
            }
            strTime = policyDto.ReturnTicketTime.Split('-');
            if (strTime != null && strTime.Length == 2)
            {
                rePolicy.ReturnTicketTime = new StartAndEndTime()
                {
                    StartTime = strTime[0],
                    EndTime = strTime[1]
                };
            }
            rePolicy.CPOffice = policyDto.CPOffice;
            rePolicy.IssueSpeed = policyDto.IssueSpeed;
            rePolicy.Remark = policyDto.Remark;
            rePolicy.PolicySourceType = policyDto.PolicySourceType == "本地" ? EnumPolicySourceType.Local : policyDto.PolicySourceType == "接口" ? EnumPolicySourceType.Interface : EnumPolicySourceType.Share;

            //(EnumPolicySourceType)Enum.Parse(typeof(EnumPolicySourceType), policyDto.PolicySourceType);
            rePolicy.CarryCode = policyDto.CarryCode;
            rePolicy.CarrierCode = policyDto.CarrierCode;
            rePolicy.PolicyOwnUserRole = (EnumPolicyOwnUserRole)Enum.Parse(typeof(EnumPolicyOwnUserRole), policyDto.PolicyOwnUserRole);
            rePolicy.Code = policyDto.Code;
            rePolicy.Name = policyDto.Name;
            rePolicy.CashbagCode = policyDto.CashbagCode;
            rePolicy.Rate = policyDto.Rate;
            rePolicy.IsLow = policyDto.IsLow;
            rePolicy.SeatPrice = policyDto.SeatPrice;
            rePolicy.ABFee = policyDto.ABFee;
            rePolicy.RQFee = policyDto.RQFee;
            rePolicy.PolicySpecialType = policyDto.PolicySpecialType;
            rePolicy.SpecialPriceOrDiscount = policyDto.SpecialPriceOrDiscount;
            rePolicy.TodayGYCode = policyDto.TodayGYCode;
            return rePolicy;
        }
    }
}
