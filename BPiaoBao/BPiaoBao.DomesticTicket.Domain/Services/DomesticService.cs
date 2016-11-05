using System.ServiceModel.Dispatcher;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using System.Threading.Tasks;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using PnrAnalysis.Model;
using System.IO;
using System.Threading;
using PnrAnalysis;
using JoveZhao.Framework.Expand;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
using JoveZhao.Framework;
using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using System.Linq.Expressions;
namespace BPiaoBao.DomesticTicket.Domain.Services
{
    public class DomesticService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IOrderRepository orderRepository;
        IBusinessmanRepository businessmanRepository;
        IDeductionRepository deductionRepository;
        ILocalPolicyRepository localPolicyRepository;
        private IInsuranceOrderRepository insuranceOrderRepository;
        DataBill databill = new DataBill();
        FormatPNR format = new FormatPNR();
        public DomesticService(IOrderRepository orderRepository, ILocalPolicyRepository localPolicyRepository, IBusinessmanRepository businessmanRepository, IDeductionRepository deductionRepository, IInsuranceOrderRepository insuranceOrderRepository)
        {
            this.orderRepository = orderRepository;
            this.businessmanRepository = businessmanRepository;
            this.deductionRepository = deductionRepository;
            this.localPolicyRepository = localPolicyRepository;
            this.insuranceOrderRepository = insuranceOrderRepository;
        }

        /*
         * 引入商户的扣点规则，平台的政策，配置文件的出票速度取数据
         * 
         */

        /// <summary>
        /// 根据商户号及Pnr内容导出平台的政策列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pnrContext"></param>
        /// <returns></returns>
        public IList<Policy> GetPolicyWithDownPoint(PolicyParam policyParam)
        {
            //获取本地政策          
            List<Policy> localPolicyList = new List<Policy>();
            //获取共享政策
            List<Policy> sharePolicyList = new List<Policy>();
            //获取接口政策
            List<Policy> platformPolicyList = new List<Policy>();
            //默认政策
            List<Policy> defaultPolicyList = new List<Policy>();
            try
            {
                string pnrContent = policyParam.PnrContent; //order.PnrContent;
                //需要的用户
                UserRelation userRealtion = GetUserRealtion(policyParam.code);
                DeductionGroup deductionGroup = userRealtion.deductionGroup;
                PnrData pnrData = policyParam.pnrData == null ? PnrHelper.GetPnrData(pnrContent) : policyParam.pnrData;
                string carrayCode = (pnrData.PnrMode != null && pnrData.PnrMode._LegList.Count > 0) ? pnrData.PnrMode._LegList[0].AirCode : "";
                int OrderType = policyParam.IsDestine ? policyParam.OrderType : ((pnrData.PnrMode != null && pnrData.PnrMode._PasType == "1") ? 0 : 1);
                policyParam.OrderType = OrderType;
                PlatformDeductionParam pfDp = new PlatformDeductionParam();
                foreach (LegInfo leg in pnrData.PnrMode._LegList)
                {
                    pfDp.FlyLineList.Add(new FlyLine()
                    {
                        CarrayCode = leg.AirCode,
                        FromCityCode = leg.FromCode,
                        ToCityCode = leg.ToCode
                    });
                }
                string platformCode = "系统";
                //获取政策类     
                PolicyService policyService = new PolicyService(this.localPolicyRepository, this.businessmanRepository);
                //成人订单
                if (OrderType == 0)
                {
                    if (policyParam.OrderSource == EnumOrderSource.UpSeatChangePnrImport)
                    {
                        //升舱换开 获取该分销的上级运营商的政策
                        Parallel.Invoke(
                               () =>
                               {
                                   //本地上级运营商的政策                                
                                   localPolicyList = policyService.GetLocalParentPolicy(userRealtion, platformCode, policyParam, pnrData);
                               },
                               () =>
                               {
                                   //默认政策                                  
                                   defaultPolicyList = policyService.GetDefaultPolicy(userRealtion, platformCode, policyParam, pnrData);
                               }
                           );
                    }
                    else
                    {

                        //同时执行获取政策
                        Parallel.Invoke(
                                () =>
                                {
                                    localPolicyList = policyService.GetLocalPolicy(userRealtion, platformCode, policyParam, pnrData);
                                }
                            ,
                            () => sharePolicyList = policyService.GetSharePolicy(userRealtion, platformCode, policyParam, pnrData),
                            () =>
                            {
                                //特价的排除掉接口政策
                                bool policySpecialType = (policyParam.PolicySpecialType != EnumPolicySpecialType.Normal) ? true : false;
                                if (!policySpecialType)
                                {
                                    System.Diagnostics.Stopwatch swch = new System.Diagnostics.Stopwatch();
                                    swch.Start();
                                    platformPolicyList = policyService.GetInterfacePolicy(policyParam, "", userRealtion, pnrData);
                                    swch.Stop();
                                    Logger.WriteLog(LogType.INFO, string.Format("接口政策总耗时:{0}", swch.ElapsedMilliseconds));
                                }
                            },
                            () =>
                            {
                                defaultPolicyList = policyService.GetDefaultPolicy(userRealtion, platformCode, policyParam, pnrData);
                            }
                            );
                    }
                    //同时匹配扣点规则
                    //Parallel.Invoke(
                    //       () =>
                    //       {
                    localPolicyList.ForEach(p =>
                    {
                        MatchDeductionRole(p, pfDp, carrayCode, deductionGroup, userRealtion, DeductionType.Local);
                    });
                    //},
                    //() =>
                    //{
                    sharePolicyList.ForEach(p =>
                    {
                        MatchDeductionRole(p, pfDp, carrayCode, deductionGroup, userRealtion, DeductionType.Share);
                    });
                    //},
                    //() =>
                    //{                    
                    platformPolicyList.ForEach(p =>
                    {
                        MatchDeductionRole(p, pfDp, carrayCode, deductionGroup, userRealtion, DeductionType.Interface);
                    });
                    //    }
                    //);
                }
                else
                {
                    //获取儿童的默认政策
                    defaultPolicyList = policyService.GetDefaultPolicy(userRealtion, platformCode, policyParam, pnrData);
                }
                //}
            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("GetPolicyWithDownPoint", "订单号：" + policyParam.OrderId + " 异常信息:" + ex.Message + "\r\n");
            }
            List<Policy> TemplistPolicy = new List<Policy>();
            TemplistPolicy.AddRange(localPolicyList.ToArray());
            TemplistPolicy.AddRange(sharePolicyList.ToArray());
            TemplistPolicy.AddRange(platformPolicyList.ToArray());
            //如果是白屏预定并且选择的是特价 过滤非特价的政策
            if (policyParam.IsDestine)
            {
                if (policyParam.PolicySpecialType != EnumPolicySpecialType.Normal)
                {
                    TemplistPolicy = TemplistPolicy.Where(p => p.PolicySpecialType != EnumPolicySpecialType.Normal).ToList();
                    if (TemplistPolicy.Count == 0)
                    {
                        //添加默认政策
                        TemplistPolicy.AddRange(defaultPolicyList.ToArray());
                    }
                }
                else
                {
                    TemplistPolicy = TemplistPolicy.Where(p => p.PolicySpecialType == EnumPolicySpecialType.Normal).ToList();
                    //添加默认政策
                    TemplistPolicy.AddRange(defaultPolicyList.ToArray());
                }
            }
            else
            {
                //添加默认政策
                TemplistPolicy.AddRange(defaultPolicyList.ToArray());
            }
            //匹配后的政策结果
            List<Policy> listPolicy = new List<Policy>();
            TemplistPolicy.ForEach(p =>
            {
                AirSystem airSystem = SystemConsoSwitch.AirSystems.Where(p1 => p1.AirCode.ToUpper() == p.CarryCode.ToUpper()).FirstOrDefault();
                if (airSystem != null)
                {
                    //B2B政策禁用 过滤
                    //BSP政策禁用 过滤
                    if ((p.PolicyType == "B2B" && !airSystem.IsB2B)
                        || (p.PolicyType == "BSP" && !airSystem.IsBSP))
                        return;
                }
                //排除政策点数为0的异地政策
                if ((p.PolicySourceType != EnumPolicySourceType.Local && p.OriginalPolicyPoint > 0)
                    || p.PolicySourceType == EnumPolicySourceType.Local
                    )
                    listPolicy.Add(p);
            });
            //排序
            return listPolicy.OrderByDescending(p => p.PolicyPoint).ToList<Policy>();
        }

        /// <summary>
        /// 运营商对下级的扣点 和平台对供应和运营的扣点
        /// </summary>       
        public decimal MatchDeductionRole(Policy policy, PlatformDeductionParam pfDp, string carrayCode, DeductionGroup deductionGroup, UserRelation userMode, DeductionType deductionType)
        {
            //多次选择 清空重新计算 清除之前的政策扣点明细 
            if (policy != null && policy.DeductionDetails != null && policy.DeductionDetails.Count > 0)
                policy.DeductionDetails.Clear();
            bool isDefaultPolicy = (policy != null &&
                (policy.PolicyId.StartsWith(userMode.carrier.Code + "_") || policy.PolicyId.StartsWith(PnrHelper.DefAccount + "_"))) ? true : false;

            #region 平台扣点
            PlatformDeduction pfDDeduction = null;
            DeductionType DeductionType = DeductionType.Share;
            //合作者的信息
            CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
            //只扣非本地非默认政策
            if (!isDefaultPolicy && policy.PolicySourceType != EnumPolicySourceType.Local)
            {
                pfDDeduction = GetPlatformDownPoint(policy, userMode, pfDp);
                if (pfDDeduction != null)
                {
                    //原始政策
                    decimal OldPoint = policy.OriginalPolicyPoint;
                    if (pfDDeduction.IsMax)
                    {
                        policy.DownPoint = 0m;
                        //#region 开启的协调
                        //decimal maxPoint = pfDDeduction.MaxPoint.HasValue ? pfDDeduction.MaxPoint.Value : 0m;
                        //if (maxPoint <= 0) maxPoint = 0m;
                        //policy.IsCoordination = pfDDeduction.IsMax;
                        //policy.MaxPoint = maxPoint;
                        //policy.PaidPoint = maxPoint;
                        ////平台扣点
                        //policy.DownPoint = (policy.OriginalPolicyPoint - policy.PaidPoint) > 0 ? (policy.OriginalPolicyPoint - policy.PaidPoint) : 0m;
                        //#endregion
                    }
                    else
                    {
                        #region 扣点规则
                        decimal depoint = Math.Abs(pfDDeduction.Point);
                        //扣点
                        if (pfDDeduction.AdjustType == AdjustType.Lrish)
                        {
                            decimal d = OldPoint - depoint;
                            if (d <= 0)
                            {
                                d = 0m;
                            }
                            policy.PaidPoint = d;
                        }
                        //留点
                        else if (pfDDeduction.AdjustType == AdjustType.Leave)
                        {
                            decimal kd = OldPoint - depoint;
                            //原始政策高于设置的留点数 就留 低于不留
                            if (kd > 0)
                            {
                                policy.PaidPoint = depoint;
                            }
                        }
                        //补点
                        else if (pfDDeduction.AdjustType == AdjustType.Compensation)
                        {
                            policy.PaidPoint = OldPoint + depoint;
                        }
                        //平台扣点数
                        policy.DownPoint = policy.OriginalPolicyPoint - policy.PaidPoint;
                        #endregion
                    }
                    //添加扣点明细
                    if (policy.DownPoint != 0)
                    {
                        if (policy.PolicySourceType == EnumPolicySourceType.Local)
                        {
                            DeductionType = DeductionType.Local;
                        }
                        else if (policy.PolicySourceType == EnumPolicySourceType.Interface)
                        {
                            DeductionType = DeductionType.Interface;
                        }
                        else if (policy.PolicySourceType == EnumPolicySourceType.Share)
                        {
                            DeductionType = DeductionType.Share;
                        }
                        policy.DeductionDetails.Add(new DeductionDetail()
                        {
                            UnCode = policy.Code,
                            UnName = policy.Name,
                            Code = setting.CashbagCode,
                            Name = "系统",
                            Point = policy.DownPoint,
                            AdjustType = pfDDeduction.AdjustType,
                            DeductionType = DeductionType,
                            DeductionSource = DeductionSource.PlatformDeduction
                        });
                        //最终政策保持和供应运营一致
                        policy.PolicyPoint = policy.PaidPoint;
                    }
                }
            }
            #endregion

            #region 运营扣点
            //有扣点规则
            if (deductionGroup != null
                && deductionGroup.DeductionRules != null
                && deductionGroup.DeductionRules.Count > 0
                //排除默认政策扣点
                && !isDefaultPolicy)
            {
                //筛选可用规则 时间 承运人 类型
                var result = deductionGroup.DeductionRules.Where(p1 => p1.StartTime <= System.DateTime.Now && System.DateTime.Now <= p1.EndTime);
                result = result.Where(p1 => p1.CarrCode.ToUpper() == carrayCode.ToUpper());
                result = result.Where(p1 => p1.DeductionType == deductionType);//本地 接口 共享
                List<DeductionRule> deductionRuleList = result.ToList();
                //具体的承运人没有设置再看有没有设置ALL的
                if (deductionRuleList == null || deductionRuleList.Count == 0)
                {
                    var result1 = deductionGroup.DeductionRules.Where(p1 => p1.StartTime <= System.DateTime.Now && System.DateTime.Now <= p1.EndTime);
                    result1 = result1.Where(p1 => p1.CarrCode.ToUpper() == "" || p1.CarrCode.ToUpper() == "ALL");
                    result1 = result1.Where(p1 => p1.DeductionType == deductionType);//本地 接口 共享
                    deductionRuleList = result1.ToList();
                }

                if (deductionRuleList != null && deductionRuleList.Count > 0 && userMode.carrier != null)
                {
                    //取一条规则
                    DeductionRule p2 = deductionRuleList[0];
                    //循环规则
                    //deductionRuleList.ForEach(p2 =>
                    //{
                    if (p2.AdjustDetails != null && p2.AdjustDetails.Count > 0)
                    {
                        //运营和供应的政策
                        decimal handPoint = policy.PaidPoint;
                        //找到设置范围内的 扣留补的点数
                        AdjustDetail adjustDetail = p2.AdjustDetails.Where(p3 => p3.StartPoint <= handPoint && handPoint <= p3.EndPoint && p3.Point != 0m).FirstOrDefault();
                        if (adjustDetail != null)
                        {
                            if (adjustDetail.AdjustType == AdjustType.Lrish)//扣点
                            {
                                handPoint = handPoint - Math.Abs(adjustDetail.Point);
                                if (handPoint >= 0)
                                {
                                    policy.DeductionDetails.Add(new DeductionDetail()
                                    {
                                        UnCode = userMode.buyer.Code,
                                        UnName = userMode.buyer.Name,
                                        Code = userMode.carrier.Code,
                                        Name = userMode.carrier.Name,
                                        Point = adjustDetail.Point,
                                        AdjustType = adjustDetail.AdjustType,
                                        DeductionType = deductionType
                                    });
                                }
                                else
                                {
                                    handPoint = 0m;
                                }
                            }
                            else if (adjustDetail.AdjustType == AdjustType.Leave)//留点
                            {
                                decimal kd = handPoint - Math.Abs(adjustDetail.Point);
                                if (kd > 0)//大于0才扣
                                {
                                    policy.DeductionDetails.Add(new DeductionDetail()
                                    {
                                        UnCode = userMode.buyer.Code,
                                        UnName = userMode.buyer.Name,
                                        Code = userMode.carrier.Code,
                                        Name = userMode.carrier.Name,
                                        Point = adjustDetail.Point,
                                        AdjustType = adjustDetail.AdjustType,
                                        DeductionType = deductionType
                                    });
                                    handPoint = Math.Abs(adjustDetail.Point);
                                }
                                //else
                                //{
                                //    //小于设置的点数  是否需要补点 需要补点时 handPoint = Math.Abs(adjustDetail.Point);
                                //    handPoint = handPoint;
                                //}
                            }
                            else if (adjustDetail.AdjustType == AdjustType.Compensation)//补点
                            {
                                handPoint = handPoint + Math.Abs(adjustDetail.Point);
                                policy.DeductionDetails.Add(new DeductionDetail()
                                {
                                    UnCode = userMode.buyer.Code,
                                    UnName = userMode.buyer.Name,
                                    Code = userMode.carrier.Code,
                                    Name = userMode.carrier.Name,
                                    AdjustType = adjustDetail.AdjustType,
                                    Point = Math.Abs(adjustDetail.Point),//补点
                                    DeductionType = deductionType
                                });
                            }
                            //最终政策点数
                            policy.PolicyPoint = handPoint;
                        }
                    }
                    //});//EndFor
                }
            }
            #endregion

            #region 平台协调
            if (pfDDeduction != null && pfDDeduction.IsMax)
            {
                decimal maxPoint = pfDDeduction.MaxPoint.HasValue ? pfDDeduction.MaxPoint.Value : 0m;
                if (maxPoint <= 0) maxPoint = 0m;
                //最总政策高于协调点数 进行协调
                if (policy.PolicyPoint > maxPoint)
                {
                    policy.IsCoordination = pfDDeduction.IsMax;
                    policy.MaxPoint = maxPoint;
                    //平台扣点
                    policy.DownPoint = policy.PolicyPoint - maxPoint;
                    policy.PolicyPoint = maxPoint;
                    policy.DeductionDetails.Add(new DeductionDetail()
                    {
                        UnCode = policy.Code,
                        UnName = policy.Name,
                        Code = setting.CashbagCode,
                        Name = "系统",
                        Point = policy.DownPoint,
                        AdjustType = pfDDeduction.AdjustType,
                        DeductionType = DeductionType,
                        DeductionSource = DeductionSource.PlatformDeduction
                    });
                }
            }
            #endregion
            return policy.PolicyPoint;
        }

        /// <summary>
        /// 生成账单明细 收款为正数 付款为负数  
        /// </summary>
        /// <param name="order"></param>
        public void CreateBillDetails(Order order, UserRelation userRealtion)
        {
            //本地采购商
            Buyer buyer = userRealtion.buyer;
            //本地运营商
            Carrier carrier = userRealtion.carrier;
            Policy policy = order.Policy;
            //多次选择 清空重新计算 
            order.OrderPay.PayBillDetails.Clear();
            //合作者的信息
            CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
            //乘客人数 非婴儿
            int pasCount = order.Passengers.Sum(p => p.PassengerType != EnumPassengerType.Baby ? 1 : 0);
            //婴儿人数
            int INFPasCount = order.Passengers.Sum(p => p.PassengerType == EnumPassengerType.Baby ? 1 : 0);
            //舱位价
            decimal seatPrice = policy.SeatPrice;

            #region 读取运营或者供应上级运营商的信息
            //保存出票供应的code
            string tempGyCode = policy.Code;
            decimal tempGyRate = policy.Rate;
            decimal tempRemoteRate = 0m;
            string tempGyName = policy.Name;
            string tempGyCashbagCode = policy.CashbagCode;
            string tempGyIssueSpeed = policy.IssueSpeed;
            if (policy.PolicySourceType != EnumPolicySourceType.Interface)
            {
                string CarrierCode = policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier ? policy.Code.Trim() : policy.CarrierCode;
                Carrier _carrier = this.businessmanRepository.FindAll(p => p.Code.Trim() == CarrierCode).FirstOrDefault() as Carrier;
                if (_carrier != null)
                {
                    policy.Rate = _carrier.Rate;
                    tempRemoteRate = _carrier.RemoteRate;
                    policy.Code = _carrier.Code;
                    policy.Name = _carrier.Name;
                    policy.CashbagCode = _carrier.CashbagCode;
                    policy.IssueSpeed = _carrier.IssueSpeed.ToString();
                }
            }

            #endregion

            #region 付款
            //----------------采购商付款----------------------------------------
            //付款方          
            for (int i = 0; i < order.Passengers.Count; i++)
            {
                Passenger p = order.Passengers[i];
                p.PayMoney = p.PassengerType != EnumPassengerType.Baby ?
                     databill.GetPayPrice(policy.SeatPrice, policy.ABFee, policy.RQFee, policy.PolicyPoint, policy.ReturnMoney) :
                     databill.GetPayPrice(p.SeatPrice, p.ABFee, p.RQFee, 0, 0);
            }
            //支付总金额
            decimal _OrderMoney = order.Passengers.Sum(p => p.PayMoney);
            order.RefundedTradeMoney = _OrderMoney;
            order.RefundedServiceMoney = Math.Round(_OrderMoney * SystemConsoSwitch.Rate, 2);
            if (_OrderMoney != 0)
            {
                order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                {
                    Code = buyer.Code,
                    Name = buyer.Name,
                    CashbagCode = buyer.CashbagCode,
                    Money = _OrderMoney,
                    Point = 0,
                    OpType = EnumOperationType.PayMoney,
                    Remark = "付款"
                });
            }
            #endregion 付款

            #region 分润和服务费
            //----------------分润方----------------------------------------
            //单人分润金额
            decimal pasProfit = 0m;
            decimal kdPoint = 0;
            if (policy.DeductionDetails.Count > 0)
            {
                foreach (DeductionDetail dep in policy.DeductionDetails)
                {
                    //运营扣点
                    if (dep.DeductionSource == DeductionSource.CarrierDeduction)
                    {
                        #region 运营扣点
                        decimal money = 0m;
                        if (dep.AdjustType == AdjustType.Lrish)//扣点
                        {
                            pasProfit = databill.Round(dep.Point * seatPrice / 100, 2);
                            money = order.Passengers.Sum(p => p.PassengerType != EnumPassengerType.Baby ? pasProfit : 0);
                            kdPoint = dep.Point;
                        }
                        else if (dep.AdjustType == AdjustType.Compensation)//补点
                        {
                            money = -order.Passengers.Sum(p => p.PassengerType != EnumPassengerType.Baby ? databill.Round(dep.Point * seatPrice / 100, 2) : 0);
                            kdPoint = -dep.Point;
                        }
                        else if (dep.AdjustType == AdjustType.Leave)//留点
                        {
                            decimal kd = policy.PaidPoint - Math.Abs(dep.Point);
                            kd = kd > 0 ? kd : 0;//大于就扣点 小于不处理不补也不扣
                            pasProfit = databill.Round(kd * seatPrice / 100, 2);
                            money = order.Passengers.Sum(p => p.PassengerType != EnumPassengerType.Baby ? pasProfit : 0);
                            kdPoint = kd > 0 ? kd : 0;
                        }
                        if (money != 0)
                        {
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = carrier.Code,
                                CashbagCode = carrier.CashbagCode,
                                Name = carrier.Name,
                                Point = dep.Point,
                                AdjustType = dep.AdjustType,
                                Money = money,
                                OpType = EnumOperationType.Profit,
                                Remark = "分润"
                            });
                        }
                        #endregion
                    }
                    else
                    {
                        #region //平台扣点分润
                        decimal platformProfit = databill.Round(Math.Abs(dep.Point) * seatPrice / 100, 2);
                        decimal money = order.Passengers.Sum(p => p.PassengerType != EnumPassengerType.Baby ? platformProfit : 0);
                        if (dep.AdjustType == AdjustType.Compensation)
                        {
                            money = -money;
                        }
                        order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                        {
                            Code = setting.CashbagCode,
                            Name = "系统",
                            CashbagCode = setting.CashbagCode,
                            Point = dep.Point,
                            Money = money,
                            OpType = EnumOperationType.ParterProfit,
                            Remark = "分润"
                        });
                        #endregion
                    }
                }
            }

            #endregion 分润和服务费

            #region 收款和服务费
            //佣金的小数部分
            decimal commissionSmallPart = databill.GetCommissionPartNumber(policy.PolicyPoint, policy.SeatPrice, 0); ;
            //佣金的小数部分总和
            decimal commissionSmallPartTotal = commissionSmallPart * pasCount;
            //单人运营票款 非婴儿
            decimal oneCpRecvFee = 0m;
            //婴儿单人票款 婴儿没有政策
            decimal oneInfCpRecvFee = 0m;
            //服务费保留小数位数 四舍五入
            int serverPointLength = 2;
            //单人出票金额
            Parallel.ForEach(order.Passengers, p =>
            {
                decimal cpMoney = p.PassengerType != EnumPassengerType.Baby ?
                     databill.GetRecvPrice(policy.SeatPrice, policy.ABFee, policy.RQFee, policy.OriginalPolicyPoint, policy.ReturnMoney) :
                     databill.GetRecvPrice(p.SeatPrice, p.ABFee, p.RQFee, 0, 0);
                p.CPMoney = cpMoney;
            });
            //出票方应收款
            decimal cpRecvFee = order.Passengers.Sum(p => p.CPMoney);
            order.CPMoney = cpRecvFee;
            //接口
            if (policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                #region 接口
                //多人分润服务费总和
                decimal serverFee = Math.Abs(databill.CeilAngle(pasProfit * carrier.Rate) * pasCount);
                if (serverFee != 0)
                {
                    //运营商支付分润的服务费
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = carrier.Code,
                        Name = carrier.Name,
                        CashbagCode = carrier.CashbagCode,
                        Money = -serverFee,
                        OpType = EnumOperationType.CarrierPayProfitServer,
                        Remark = "服务费"
                    });
                    //合作方收取运营分润的服务费
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = serverFee,
                        OpType = EnumOperationType.ParterProfitServer,
                        Remark = "服务费"
                    });
                }

                //合作者收款               
                order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                {
                    Code = setting.CashbagCode,
                    Name = "系统",
                    CashbagCode = setting.CashbagCode,
                    Money = cpRecvFee,
                    OpType = EnumOperationType.Receivables,
                    Remark = "收款"
                });
                //添加保留费用 合作者收取
                order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                {
                    Code = setting.CashbagCode,
                    Name = "系统",
                    CashbagCode = setting.CashbagCode,
                    Money = commissionSmallPartTotal,
                    OpType = EnumOperationType.ParterRetainServer,
                    Remark = "保留"
                });
                #endregion
            }
            else
            {
                //运营票款
                oneCpRecvFee = databill.GetRecvPrice(policy.SeatPrice, policy.ABFee, policy.RQFee, policy.OriginalPolicyPoint, policy.ReturnMoney);
                //婴儿票款
                oneInfCpRecvFee = INFPasCount > 0 ? order.Passengers.Where(p => p.PassengerType == EnumPassengerType.Baby).FirstOrDefault().PayMoney : 0m;
                //本地
                if (policy.PolicySourceType == EnumPolicySourceType.Local)
                {
                    #region 本地
                    //运营商婴儿服务费
                    decimal carrInfServerFee = Math.Abs(databill.NewRound(oneInfCpRecvFee * policy.Rate, serverPointLength) * INFPasCount);
                    //运营商服务费
                    //decimal carrServerFee = databill.NewRound((oneCpRecvFee + pasProfit + commissionSmallPart) * policy.Rate, serverPointLength) * pasCount + carrInfServerFee;
                    decimal carrServerFee = databill.NewRound((oneCpRecvFee + pasProfit) * policy.Rate, serverPointLength) * pasCount + carrInfServerFee;
                    //运营商分润服务费
                    decimal carrProfitServerFee = Math.Abs(databill.CeilAngle(pasProfit * policy.Rate)) * pasCount;
                    //出票方是供应商
                    if (policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Supplier)
                    {
                        #region 供应

                        //获取供应商
                        Supplier supplier = this.businessmanRepository.FindAll(p => p.Code.Trim() == tempGyCode.Trim() && (p is Supplier)).FirstOrDefault() as Supplier;
                        if (supplier != null)
                        {
                            //填补政策数据
                            tempGyRate = supplier.SupRate;
                            tempGyName = supplier.Name;
                            tempGyCashbagCode = supplier.CashbagCode;
                            tempGyIssueSpeed = supplier.IssueSpeed.ToString();
                            //婴儿服务费
                            decimal gyInfFee = Math.Abs(databill.NewRound(oneInfCpRecvFee * supplier.SupRate, serverPointLength)) * INFPasCount;
                            //供应服务费                       
                            decimal gyServerFee = gyInfFee + Math.Abs(databill.NewRound(oneCpRecvFee * supplier.SupRate, serverPointLength)) * pasCount;
                            //供应收款
                            decimal gyRecvFee = oneCpRecvFee * pasCount + oneInfCpRecvFee * INFPasCount;

                            //供应收款                 
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = supplier.Code,
                                Name = supplier.Name,
                                CashbagCode = supplier.CashbagCode,
                                Money = gyRecvFee,
                                OpType = EnumOperationType.Receivables,
                                Remark = "收款"
                            });
                            //供应付款   支付运营商的服务费         
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = supplier.Code,
                                Name = supplier.Name,
                                CashbagCode = supplier.CashbagCode,
                                Money = -gyServerFee,
                                InfMoney = -gyInfFee,
                                OpType = EnumOperationType.IssuePayServer,
                                Remark = "服务费"
                            });
                            //运营商收款 收取供应商的服务费
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = policy.Code,
                                Name = policy.Name,
                                CashbagCode = policy.CashbagCode,
                                Money = gyServerFee,
                                InfMoney = gyInfFee,
                                OpType = EnumOperationType.CarrierRecvServer,
                                Remark = "服务费"
                            });
                        }
                        //}
                        #endregion
                    }
                    //出票方是运营商
                    else if (policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        //运营商收款
                        decimal CarrierRecvFee = oneCpRecvFee * pasCount + oneInfCpRecvFee * INFPasCount;
                        //运营商收款
                        order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                        {
                            Code = policy.Code,
                            Name = policy.Name,
                            CashbagCode = policy.CashbagCode,
                            Money = CarrierRecvFee,
                            OpType = EnumOperationType.Receivables,
                            Remark = "收款"
                        });
                    }
                    //添加保留费用 出票收取
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        //Code = policy.Code,
                        //Name = policy.Name, 
                        //CashbagCode = policy.CashbagCode,
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = commissionSmallPartTotal,
                        OpType = EnumOperationType.ParterRetainServer,
                        Remark = "保留"
                    });
                    //运营商付款 分润服务费付给合作方
                    //order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    //{
                    //    Code = policy.Code,
                    //    Name = policy.Name,
                    //    CashbagCode = policy.CashbagCode,
                    //    Money = -carrProfitServerFee,
                    //    OpType = EnumOperationType.CarrierPayProfitServer,
                    //    Remark = "服务费"
                    //});
                    ////合作方收款 收取运营分润的服务费                   
                    //order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    //{
                    //    Code = setting.CashbagCode,
                    //    Name = "系统",
                    //    CashbagCode = setting.CashbagCode,
                    //    Money = carrProfitServerFee,
                    //    OpType = EnumOperationType.ParterProfitServer,
                    //    Remark = "服务费"
                    //});
                    //运营商付款 支付票价的服务费付给合作方
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = policy.Code,
                        Name = policy.Name,
                        CashbagCode = policy.CashbagCode,
                        Money = -carrServerFee,
                        InfMoney = -carrInfServerFee,
                        OpType = policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier ? EnumOperationType.IssuePayServer : EnumOperationType.CarrierPayServer,
                        Remark = "服务费"
                    });
                    //合作方收款 收取运营票款服务费                          
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = carrServerFee,
                        InfMoney = carrInfServerFee,
                        OpType = EnumOperationType.ParterServer,
                        Remark = "服务费"
                    });
                    #endregion 本地
                }
                //共享
                else if (policy.PolicySourceType == EnumPolicySourceType.Share)
                {
                    #region 共享
                    //异地运营商婴儿服务费
                    decimal ydInfCarrServerFee = Math.Abs(databill.NewRound(oneInfCpRecvFee * tempRemoteRate, serverPointLength)) * INFPasCount;
                    //异地运营商服务费
                    decimal ydCarrServerFee = ydInfCarrServerFee + Math.Abs(databill.NewRound(oneCpRecvFee * tempRemoteRate, serverPointLength)) * pasCount;
                    //本地运营商分润服务费 分润保留到角
                    decimal localCarrProfitServerFee = Math.Abs(databill.CeilAngle(pasProfit * carrier.Rate)) * pasCount;
                    //出票方是供应商
                    if (policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Supplier)
                    {
                        #region 异地供应
                        Supplier supplier = this.businessmanRepository.FindAll(p => p.Code.Trim() == tempGyCode.Trim() && (p is Supplier)).FirstOrDefault() as Supplier;
                        if (supplier != null)
                        {
                            //填补政策数据
                            tempGyRate = supplier.SupRate;
                            tempGyName = supplier.Name;
                            tempGyCashbagCode = supplier.CashbagCode;
                            tempGyIssueSpeed = supplier.IssueSpeed.ToString();

                            //异地供应婴儿服务费
                            decimal ydInfSupplierServerFee = Math.Abs(databill.NewRound(oneInfCpRecvFee * supplier.SupRemoteRate, serverPointLength)) * INFPasCount;
                            //异地供应服务费
                            decimal ydSupplierServerFee = ydInfSupplierServerFee + Math.Abs(databill.NewRound(oneCpRecvFee * supplier.SupRemoteRate, serverPointLength)) * pasCount;
                            //异地供应收款  
                            decimal gyRecvFee = oneCpRecvFee * pasCount + oneInfCpRecvFee * INFPasCount;
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = supplier.Code,
                                Name = supplier.Name,
                                CashbagCode = supplier.CashbagCode,
                                Money = gyRecvFee,
                                OpType = EnumOperationType.Receivables,
                                Remark = "收款"
                            });

                            //异地供应付款 给上级运营商的服务费
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = supplier.Code,
                                Name = supplier.Name,
                                CashbagCode = supplier.CashbagCode,
                                Money = -ydSupplierServerFee,
                                InfMoney = -ydInfSupplierServerFee,
                                OpType = EnumOperationType.IssuePayServer,
                                Remark = "服务费"
                            });
                            //异地运营商收款 收取下级出票供应服务费
                            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                            {
                                Code = policy.Code,
                                Name = policy.Name,
                                CashbagCode = policy.CashbagCode,
                                Money = ydSupplierServerFee,
                                InfMoney = ydInfSupplierServerFee,
                                OpType = EnumOperationType.CarrierRecvServer,
                                Remark = "服务费"
                            });
                        }
                        #endregion
                    }
                    //出票方是运营商
                    else if (policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier)
                    {
                        decimal ydCarrierRecvFee = oneCpRecvFee * pasCount + oneInfCpRecvFee * INFPasCount;
                        //异地运营商收款
                        order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                        {
                            Code = policy.Code,
                            Name = policy.Name,
                            CashbagCode = policy.CashbagCode,
                            Money = ydCarrierRecvFee,
                            OpType = EnumOperationType.Receivables,
                            Remark = "收款"
                        });
                    }

                    //异地运营商付款 给合作者的服务费
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = policy.Code,
                        Name = policy.Name,
                        CashbagCode = policy.CashbagCode,
                        Money = -ydCarrServerFee,
                        InfMoney = -ydInfCarrServerFee,
                        OpType = policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Carrier ? EnumOperationType.IssuePayServer : EnumOperationType.CarrierPayServer,
                        Remark = "服务费"
                    });
                    //合作者收款 收取出票方的出票的服务费
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = ydCarrServerFee,
                        InfMoney = ydInfCarrServerFee,
                        OpType = EnumOperationType.ParterServer,
                        Remark = "服务费"
                    });

                    //本地运营商付款 给合作者的分润服务费
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = carrier.Code,
                        Name = carrier.Name,
                        CashbagCode = carrier.CashbagCode,
                        Money = -localCarrProfitServerFee,
                        OpType = EnumOperationType.CarrierPayProfitServer,
                        Remark = "服务费"
                    });
                    //合作者收款 收取分润方的分润服务费 保留到角
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = localCarrProfitServerFee,
                        OpType = EnumOperationType.ParterProfitServer,
                        Remark = "服务费"
                    });

                    //添加保留费用 合作者收取
                    order.OrderPay.PayBillDetails.Add(new PayBillDetail()
                    {
                        Code = setting.CashbagCode,
                        Name = "系统",
                        CashbagCode = setting.CashbagCode,
                        Money = commissionSmallPartTotal,
                        OpType = EnumOperationType.ParterRetainServer,
                        Remark = "保留"
                    });
                    #endregion 共享
                }
            }
            //还原这几个数据
            if (policy.PolicyOwnUserRole == EnumPolicyOwnUserRole.Supplier)
            {
                policy.Rate = tempGyRate;
                policy.Code = tempGyCode;
                policy.Name = tempGyName;
                policy.CashbagCode = tempGyCashbagCode;
                policy.IssueSpeed = tempGyIssueSpeed;
            }

            #endregion 收款
        }

        /// <summary>
        /// 平台对供应和运营的扣点
        /// </summary>
        /// <returns></returns>
        private PlatformDeduction GetPlatformDownPoint(Policy policy, UserRelation userRealtion, PlatformDeductionParam pfDp)
        {
            PlatformDeduction pfd = null;
            if (userRealtion.carrier.PointGroupID.HasValue)
            {
                IPlatformPointGroupRepository platformPointGroupRepository = ObjectFactory.GetInstance<IPlatformPointGroupRepository>();
                //默认组
                PlatformPointGroup pGroup = platformPointGroupRepository.FindAll(p => p.ID == userRealtion.carrier.PointGroupID.Value).FirstOrDefault();
                if (pGroup == null) return pfd;
                pfd = new PlatformDeduction();
                pfd.PlatformPointGroupID = pGroup.ID;
                pfd.Point = pGroup.DefaultPoint;
                pfd.MaxPoint = pGroup.MaxPoint;
                pfd.IsMax = pGroup.IsMax;
                List<PlatformPointGroupRule> groupRuleList = GetPlatformPointGroupRule(pGroup, pfDp);
                if (groupRuleList != null && groupRuleList.Count > 0)
                {
                    //匹配政策来源
                    var result = groupRuleList.Where(p => p.IssueTicketCode != null && p.IssueTicketCode.Contains(policy.Code) || p.IssueTicketCode.Contains(policy.PlatformCode));
                    int a = 0;
                    //a = result.Count();
                    //再次筛选一次航空公司
                    result = result.Where(p => p.AirCode.ToLower() == "all" || p.AirCode.ToLower() == policy.CarryCode.ToLower());
                    // a = result.Count();
                    PlatformPointGroupRule pdfRule = result.Where(p => p.DetailRules != null && p.DetailRules.Count > 0).FirstOrDefault();
                    if (pdfRule != null
                        && pdfRule.DetailRules != null
                        && pdfRule.DetailRules.Count > 0
                        )
                    {
                        //点数范围
                        PlatformPointGroupDetailRule ruleResult = pdfRule.DetailRules.Where(p => policy.OriginalPolicyPoint >= p.StartPoint && policy.OriginalPolicyPoint <= p.EndPoint && p.Point != 0m).FirstOrDefault();
                        if (ruleResult != null)
                        {
                            pfd.AdjustType = pdfRule.AdjustType;
                            pfd.IsMax = pGroup.IsMax;
                            pfd.MaxPoint = pGroup.MaxPoint;
                            pfd.Point = ruleResult.Point;
                            pfd.PlatformPointGroupID = pdfRule.PlatformPointGroupID;
                            pfd.RuleID = pdfRule.ID;
                            pfd.PolicyId = policy.PolicyId;
                        }
                    }
                }
            }
            return pfd;
        }
        public List<PlatformPointGroupRule> GetPlatformPointGroupRule(PlatformPointGroup pGroup, PlatformDeductionParam pdParam)
        {
            IPlatformPointGroupRuleRepository platformPointGroupRuleRepository = ObjectFactory.GetInstance<IPlatformPointGroupRuleRepository>();
            var result = platformPointGroupRuleRepository.FindAll(p => p.PlatformPointGroupID == pGroup.ID);
            //航空公司
            if (!string.IsNullOrEmpty(pdParam.CarrayCode))
                result = result.Where(p => p.AirCode == pdParam.CarrayCode || p.AirCode.ToLower() == "all");
            //航线
            FlyLine line1 = pdParam.FlyLineList[0];
            //单程或者往返取第一程
            if (pdParam.TravelType == 1 || pdParam.TravelType == 2)
                result = result.Where(p => (p.FromCityCodes.Contains(line1.FromCityCode) || p.FromCityCodes.ToLower() == "all") && (p.ToCityCodes.Contains(line1.ToCityCode) || p.ToCityCodes.ToLower() == "all"));
            else if (pdParam.TravelType == 3)//联程
            {
                FlyLine line2 = pdParam.FlyLineList[1];
                result = result.Where(p => (p.FromCityCodes.Contains(line1.FromCityCode) || p.FromCityCodes.ToLower() == "all") && (p.ToCityCodes.Contains(line2.ToCityCode) || p.ToCityCodes.ToLower() == "all"));
            }
            //生效日期
            DateTime dtNow = System.DateTime.Now;
            result = result.Where(p => p.StartDate <= dtNow && p.EndDate >= dtNow);
            //int a = result.Count();
            return result.ToList();
        }

        public UserRelation GetUserRealtion(string code)
        {
            Carrier carrier = null;
            DeductionGroup deductionGroup = null;
            Buyer buyer = this.businessmanRepository.FindAll(p => p.Code == code && (p is Buyer)).FirstOrDefault() as Buyer;
            var result = this.businessmanRepository.FindAll(p => p.IsEnable && (p is Carrier || p is Supplier)).Select(p => p).ToList();
            List<Carrier> carrierList = result.OfType<Carrier>().ToList();
            List<Supplier> supplierList = result.OfType<Supplier>().ToList();
            if (buyer != null)
            {
                if (carrierList != null && carrierList.Count > 0)
                {
                    carrier = carrierList.Where(p => p.Code == buyer.CarrierCode).FirstOrDefault();
                }
            }
            if (buyer != null && buyer.DeductionGroupID != null)
            {
                //根据用户所在组找到对应的政策规则 扣留补 
                deductionGroup = this.deductionRepository.FindAll(p => p.ID == buyer.DeductionGroupID).FirstOrDefault();
            }
            UserRelation userRelation = new UserRelation();
            userRelation.buyer = buyer;
            userRelation.carrier = carrier;
            userRelation.CarrierList = carrierList;
            userRelation.SupplierList = supplierList;
            userRelation.deductionGroup = deductionGroup;
            return userRelation;
        }



        public PlatformElementCollection GetPlatList()
        {
            return PlatformSection.GetInstances().Platforms;
        }
        /// <summary>
        /// 该编码是否已经支付成功
        /// </summary>
        /// <param name="pnrCode"></param>
        /// <returns></returns>
        public bool PnrIsPay(Order order)
        {
            bool pnrIsPay = false;
            //相同编码
            var query = orderRepository.FindAll(p => p.PnrCode == order.PnrCode);
            query = query.Where(p => p.OrderStatus != EnumOrderStatus.OrderCanceled);
            query = query.Where(p => p.OrderType != 2);
            query = query.Where(p => p.OrderStatus != EnumOrderStatus.Invalid);
            query = query.Where(p => p.OrderStatus != EnumOrderStatus.RepelIssueAndCompleted);
            query = query.Where(p => p.OrderStatus != EnumOrderStatus.PaymentInWaiting);
            query = query.Where(p => (int)p.OrderStatus >= (int)EnumOrderStatus.WaitAndPaid);
            var result = query.ToList();
            string strStartTime = order.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm");
            string strPassengerName = order.Passengers[0].PassengerName;
            if (result.Count > 0)
            {
                result.ForEach(o =>
                {
                    //相同的起飞时间 第一个乘机人的姓名相同
                    if (!pnrIsPay && o.SkyWays[0].StartDateTime.ToString("yyyy-MM-dd HH:mm") == strStartTime && o.Passengers[0].PassengerName == strPassengerName)
                    {
                        pnrIsPay = true;
                    }
                });
            }
            return pnrIsPay;
        }
        /// <summary>
        /// 是否存在该用户的编码是 新订单 等待支付的状态
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="Pnr"></param>
        /// <returns></returns>
        public bool IsExistNewOrder(string businessmanCode, string Pnr)
        {
            var query = orderRepository.FindAll(p => p.BusinessmanCode.Trim() == businessmanCode.Trim());
            query = query.Where(p => p.PnrCode == Pnr);
            query = query.Where(p => p.OrderStatus == EnumOrderStatus.NewOrder);
            return query.Count() > 0 ? true : false;
        }
        /// <summary>
        /// 获取出票信息
        /// </summary>
        /// <param name="PlatformCode"></param>
        /// <param name="areaCity"></param>
        /// <param name="orderId"></param>
        /// <param name="outOrderId"></param>
        /// <param name="pnr"></param>
        /// <returns></returns>
        public Dictionary<string, string> AutoCompositeTicket(string platformCode, string areaCity, string orderId, string outOrderId, string pnr)
        {
            var plateform = ObjectFactory.GetNamedInstance<IPlatform>(platformCode);
            return plateform.AutoCompositeTicket(areaCity, orderId, outOrderId, pnr);
        }
        /// <summary>
        /// 设置除了该PNR的订单号外的其他设置无效 支付的订单排除
        /// </summary>
        /// <param name="orderId"></param>
        public void SetOrderStatusInvalid(string operatorName, string pnrCode, string orderId)
        {
            var query = orderRepository.FindAll(p => p.PnrCode == pnrCode);
            query = query.Where(p => p.OrderId != orderId);
            query = query.Where(p => p.OrderStatus != EnumOrderStatus.CreatePlatformFail);
            query = query.Where(p => ((int)p.OrderStatus) < (int)EnumOrderStatus.WaitAndPaid);
            var result = query.ToList();
            if (result.Count > 0)
            {
                result.ForEach(p =>
                {
                    bool IsUpdate = ((p.OrderPay != null && p.OrderPay.PayStatus != EnumPayStatus.OK) || p.OrderPay == null) ? true : false;
                    if (IsUpdate)
                    {
                        p.ChangeStatus(EnumOrderStatus.Invalid);
                        p.WriteLog(new OrderLog()
                        {
                            OperationPerson = operatorName,
                            OperationDatetime = DateTime.Now,
                            OperationContent = "存在未支付订单(" + orderId + "),该订单设置无效",
                            IsShowLog = true
                        });
                        unitOfWorkRepository.PersistUpdateOf(p);
                    }
                });
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="PlatformCode"></param>
        /// <param name="areaCity"></param>
        /// <param name="orderId"></param>
        /// <param name="outOrderId"></param>
        /// <param name="pnr"></param>
        /// <returns></returns>
        public string GetOrderStatus(string platformCode, string areaCity, string orderId, string outOrderId, string pnr)
        {
            var plateform = ObjectFactory.GetNamedInstance<IPlatform>(platformCode);
            return plateform.GetOrderStatus(areaCity, orderId, outOrderId, pnr);
        }


        /// <summary>
        /// 设置保险单状态 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        public void SetInsuranceStatus(string orderId, EnumInsuranceStatus status)
        {
            //if (insuranceOrderRepository == null)
            //{
            //    insuranceOrderRepository = ObjectFactory.GetInstance<IInsuranceOrderRepository>();

            //}

            //var models = this.insuranceOrderRepository.FindAll(p => p.OrderId == orderId);
            //foreach (var m in models)
            //{

            //    //m.EnumInsuranceStatus = status;
            //    this.unitOfWorkRepository.PersistUpdateOf(m);

            //}
        }


        /// <summary>
        /// 自动出票 B2B/BSP 
        /// </summary>
        public void AutoIssue(string orderId, string remark, Action action)
        {
            ThreadPool.QueueUserWorkItem(it =>
               {
                   Order order = null;
                   IUnitOfWork unitOfWork1 = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
                   IUnitOfWorkRepository unitOfWorkRepository1 = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
                   try
                   {
                       IOrderRepository orderRepository1 = ObjectFactory.GetInstance<IOrderRepository>();
                       order = orderRepository1.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                   }
                   catch (Exception ex)
                   {
                       new CommLog().WriteLog("LoadOrder", "自动出票读取订单号【" + orderId + "】备注:" + remark + "\r\n 错误信息:" + ex.Message + "\r\n");
                   }
                   if (order != null && order.Policy != null
                       && order.Policy.PolicySourceType != EnumPolicySourceType.Interface
                       && order.Policy.EnumIssueTicketWay == EnumIssueTicketWay.Automatic
                       && order.OrderStatus == EnumOrderStatus.WaitIssue
                       )
                   {
                       int resultCount = 0;
                       //备注
                       string strRemark = remark;
                       //日志
                       StringBuilder sbAuto = new StringBuilder();
                       string strFail = string.Empty;
                       //循环延时秒数
                       int LeayTime = 3;
                       try
                       {
                           sbAuto.AppendFormat("订单号:{0} 政策类型:{1}\r\n", order.OrderId, order.Policy.PolicyType);
                           sbAuto.AppendFormat("Remark={0}\r\n", strRemark);
                           AutoIssueTicketViewModel paramSetting = GetB2BParam(order.Policy.Code);
                           AutoTicketService autoTicketService = ObjectFactory.GetInstance<AutoTicketService>();
                           string carryCode = order.SkyWays.Count > 0 ? order.SkyWays[0].CarrayCode : "";
                           //1=单程，2=往返，3=中转联程 4缺口程 5多程
                           int _TravelType = 1;
                           if (order.SkyWays.Count >= 2)
                           {
                               if (order.SkyWays.Count > 2)
                               {
                                   _TravelType = 5;
                               }
                               else
                               {
                                   SkyWay leg0 = order.SkyWays[0];
                                   SkyWay leg1 = order.SkyWays[1];
                                   if (leg0.ToCityCode == leg1.FromCityCode)
                                   {
                                       if (leg0.FromCityCode == leg1.ToCityCode)
                                       {
                                           _TravelType = 2;
                                       }
                                       else
                                       {
                                           _TravelType = 3;
                                       }
                                   }
                                   else
                                   {
                                       _TravelType = 4;
                                   }
                               }
                           }
                           //过滤 CA CZ的 联程和缺口程自动出票
                           bool IsFilter = ("CA|CZ".Contains(carryCode.ToUpper()) && _TravelType > 3);
                           if (!IsFilter)
                           {
                               if (order.Policy.PolicyType == "B2B" && paramSetting.B2B)
                               {
                                   #region B2B自动出票
                                   B2BResponse b2BResponse = new B2BResponse();
                                   AutoEtdzParam autoEtdzParam = new AutoEtdzParam();
                                   IssueTicketModel issueTicketModel = null;
                                   strFail = string.Format("{0}航空公司B2B账号未设置,请设置！", carryCode);
                                   if (paramSetting.IssueTickets == null
                                       || !paramSetting.IssueTickets.Exists(p => p.CarrayCode.ToUpper() == carryCode.ToUpper()))
                                   {
                                       b2BResponse.Remark = strFail;
                                   }
                                   else
                                   {
                                       issueTicketModel = paramSetting.IssueTickets.Where(p => p.CarrayCode.ToUpper() == carryCode.ToUpper()).FirstOrDefault();
                                       if (issueTicketModel == null)
                                       {
                                           b2BResponse.Remark = strFail;
                                       }
                                   }
                                   if (string.IsNullOrEmpty(b2BResponse.Remark) && issueTicketModel != null)
                                   {
                                       #region 设置参数
                                       PnrModel pnrMode = null;
                                       PatModel patMode = null;
                                       GetPat(order, out patMode, out pnrMode);

                                       autoEtdzParam.FlatformOrderId = order.OrderId;
                                       autoEtdzParam.CarryCode = carryCode;
                                       autoEtdzParam.B2BAccount = issueTicketModel.Account;
                                       autoEtdzParam.B2BPwd = issueTicketModel.Pwd;
                                       autoEtdzParam.Pnr = order.PnrCode;
                                       autoEtdzParam.BigPnr = order.BigCode;
                                       //多个价格时指定票面总价筛选政策
                                       bool IsMulPrice = patMode.UninuePatList.Count > 0 ? true : false;
                                       autoEtdzParam.IsMulPrice = IsMulPrice;
                                       //指定价格
                                       autoEtdzParam.IsLimitScope = IsMulPrice;
                                       //代理人在平台放的政策
                                       autoEtdzParam.OldPolicyPoint = order.Policy.OriginalPolicyPoint;
                                       //配置文件获取URL
                                       autoEtdzParam.UrlInfo = GetUrlInfo(autoEtdzParam.UrlInfo);
                                       //支付账号和价格
                                       autoEtdzParam.PayInfo.PayAccount = paramSetting.Account;
                                       autoEtdzParam.PayInfo.SeatTotalPrice = order.Passengers.Sum(p => p.SeatPrice);
                                       autoEtdzParam.PayInfo.TaxTotalPrice = order.Passengers.Sum(p => (p.ABFee + p.RQFee));
                                       //order.Passengers.Sum(p => (p.SeatPrice + p.ABFee + p.RQFee));
                                       autoEtdzParam.PayInfo.PayTotalPrice = order.Passengers.Sum(p => (p.SeatPrice + p.ABFee + p.RQFee));//order.OrderMoney;
                                       //获取扩展的参数
                                       autoEtdzParam.ExParam = GetExParam(paramSetting, autoEtdzParam, carryCode);

                                       Passenger passenger = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                                       autoEtdzParam.PayInfo.OnlyAdultSeatPrice = passenger != null ? passenger.SeatPrice : 0m;
                                       autoEtdzParam.PayInfo.OnlyAdultTaxPrice = passenger != null ? (passenger.ABFee + passenger.RQFee) : 0m;
                                       #endregion

                                       #region 处理
                                       int TryCount = 0;
                                       int MaxReconnectionCount = 0;
                                       int.TryParse(paramSetting.ReconnectionCount, out MaxReconnectionCount);
                                       int AutoEtdzCallCount = order.AutoEtdzCallCount ?? 0;
                                       if (AutoEtdzCallCount <= MaxReconnectionCount)
                                       {
                                           while (TryCount < MaxReconnectionCount)
                                           {
                                               //调用接口
                                               b2BResponse = autoTicketService.B2BAutoEtdz(autoEtdzParam);
                                               if (b2BResponse.Status)
                                               {
                                                   strFail = string.Format("{0}自动出票成功({1}):{2}",
                                                     order.Policy.PolicyType, (TryCount + 1), b2BResponse.Remark);
                                                   order.WriteLog(new OrderLog()
                                                   {
                                                       OperationContent = strFail,
                                                       OperationDatetime = DateTime.Now,
                                                       OperationPerson = "系统",
                                                       IsShowLog = false,
                                                       Remark = strRemark
                                                   });
                                                   break;
                                               }
                                               else
                                               {
                                                   strFail = string.Format("{0}自动出票失败({1}):{2}！",
                                                       order.Policy.PolicyType, (TryCount + 1), b2BResponse.Remark);
                                                   //日志
                                                   order.WriteLog(new OrderLog()
                                                   {
                                                       OperationContent = strFail,
                                                       OperationDatetime = DateTime.Now,
                                                       OperationPerson = "系统",
                                                       IsShowLog = false,
                                                       Remark = strRemark
                                                   });
                                                   sbAuto.Append(strFail + "\r\n XML=" + b2BResponse.RetuenXML + "\r\n");
                                               }
                                               if (!autoEtdzParam.ParamIsPass)
                                               {
                                                   break;
                                               }
                                               TryCount++;
                                               Thread.Sleep(1000 * LeayTime);
                                           }//end While
                                           order.B2bFailLastCallMethod = autoEtdzParam.LastCallMethod;
                                           order.AutoEtdzCallCount = TryCount;
                                           sbAuto.AppendFormat("1. b2BResponse.Status={0}\r\n", b2BResponse.Status);
                                           sbAuto.Append("在调用一次TicketOut\r\n");
                                           b2BResponse = autoTicketService.TicketOut(autoEtdzParam);
                                           //在调用一次同步出票
                                           if (!b2BResponse.Status)
                                           {
                                               Thread.Sleep(50);
                                               b2BResponse = autoTicketService.TicketOut(autoEtdzParam);
                                               sbAuto.AppendFormat("2. 返回b2BResponse.Status={0}\r\n", b2BResponse.Status);
                                           }
                                           if (b2BResponse.Status)
                                           {
                                               #region 回帖票号
                                               List<AutoTicketInfo> b2bList = b2BResponse.TicketNofityInfo.AutoTicketList;
                                               if (b2bList != null && b2bList.Count > 0)
                                               {
                                                   Dictionary<string, string> ticketDict = new Dictionary<string, string>();
                                                   List<string> tklist = new List<string>();
                                                   foreach (AutoTicketInfo item in b2bList)
                                                   {
                                                       if (!ticketDict.ContainsKey(item.PassengerName))
                                                       {
                                                           ticketDict.Add(item.PassengerName, item.TicketNumber);
                                                           tklist.Add(item.PassengerName + "|" + item.TicketNumber);
                                                       }
                                                   }
                                                   try
                                                   {
                                                       var behavior = order.State.GetBehaviorByCode("TicketsIssue");
                                                       behavior.SetParame("ticketDict", ticketDict);
                                                       behavior.SetParame("operatorName", "系统");
                                                       behavior.SetParame("platformCode", "系统");
                                                       behavior.SetParame("opratorSource", "系统");
                                                       behavior.SetParame("remark", order.Policy.PolicyType + "自动出票");
                                                       behavior.Execute();
                                                       resultCount++;
                                                       sbAuto.Append("B2B自动出票回帖票号成功\r\n");
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       sbAuto.Append("B2B自动出票,订单状态错误：" + ex.Message + "\r\n");
                                                       if (tklist.Count > 0)
                                                       {
                                                           strFail = string.Format("B2B自动出票,订单状态错误[{0}],票号信息:{1}", order.OrderStatus.ToEnumDesc(), string.Join(",", tklist.ToArray()));
                                                           //日志
                                                           order.WriteLog(new OrderLog()
                                                           {
                                                               OperationContent = strFail,
                                                               OperationDatetime = DateTime.Now,
                                                               OperationPerson = "系统",
                                                               IsShowLog = false,
                                                               Remark = strRemark
                                                           });
                                                       }
                                                   }
                                               }
                                               else
                                               {
                                                   //等待通知
                                                   sbAuto.Append("出票数据为空【b2bList.Count=" + b2bList.Count + "】，等待通知\r\n");
                                               }
                                               #endregion
                                           }
                                           else
                                           {
                                               //失败
                                               strFail = string.Format("{0}自动出票失败:{1}!!!",
                                                      order.Policy.PolicyType, b2BResponse.Remark);
                                               //日志
                                               order.WriteLog(new OrderLog()
                                               {
                                                   OperationContent = strFail,
                                                   OperationDatetime = DateTime.Now,
                                                   OperationPerson = "系统",
                                                   IsShowLog = false,
                                                   Remark = strRemark
                                               });
                                               sbAuto.Append(strFail + "\r\n");
                                           }
                                       }//比较调用次数
                                       else
                                       {
                                           sbAuto.AppendFormat("(TryCount < MaxReconnectionCount)={0}\r\n", (TryCount < MaxReconnectionCount));
                                       }
                                       #endregion
                                   }
                                   else
                                   {
                                       strFail = string.Format("{0}自动出票失败:{1}！",
                                                       order.Policy.PolicyType, b2BResponse.Remark);
                                       //日志
                                       order.WriteLog(new OrderLog()
                                       {
                                           OperationContent = strFail,
                                           OperationDatetime = DateTime.Now,
                                           OperationPerson = "系统",
                                           IsShowLog = false,
                                           Remark = strRemark
                                       });
                                       sbAuto.Append(strFail + "\r\n");
                                   }
                                   #endregion
                               }
                               else if (order.Policy.PolicyType == "BSP" && paramSetting.BSP)
                               {
                                   #region BSP自动出票
                                   BSPResponse bSPResponse = new BSPResponse();
                                   BSPParam bSPParam = new BSPParam();
                                   bSPParam.IssueINFTicekt = order.OrderType == 2 ? true : false;
                                   #region 筛选条件
                                   var businessman = businessmanRepository.FindAll(p => p.Code == order.Policy.Code).FirstOrDefault();
                                   if (businessman == null)
                                   {
                                       bSPResponse.Msg = string.Format("未找到商户({0})信息，请联系客服！", order.Policy.Code);
                                   }
                                   else
                                   {
                                       if (businessman is Carrier)
                                       {
                                           if ((businessman as Carrier).Pids == null || (businessman as Carrier).Pids.Count == 0)
                                           {
                                               bSPResponse.Msg = "运营商配置信息未设置，请联系运营商设置！";
                                           }
                                       }
                                       else if (businessman is Supplier)
                                       {
                                           if ((businessman as Supplier).SupPids == null || (businessman as Supplier).SupPids.Count == 0)
                                           {
                                               bSPResponse.Msg = "运营商配置信息未设置，请联系运营商设置！";
                                           }
                                       }
                                       if (string.IsNullOrEmpty(bSPResponse.Msg))
                                       {
                                           PID pid = (businessman is Carrier) ? (businessman as Carrier).Pids.FirstOrDefault() : (businessman as Supplier).SupPids.FirstOrDefault();

                                           if (pid == null
                                               || string.IsNullOrEmpty(pid.IP)
                                               || string.IsNullOrEmpty(pid.Port.ToString())
                                                || string.IsNullOrEmpty(pid.Office)
                                               )
                                           {
                                               bSPResponse.Msg = string.Format("商户({0})配置信息未设置，请联系管理员！", order.Policy.Code);
                                           }
                                           else
                                           {
                                               bSPParam.Param.ServerIP = pid.IP;
                                               bSPParam.Param.ServerPort = pid.Port;

                                               if (string.IsNullOrEmpty(carryCode))
                                               {
                                                   bSPResponse.Msg = string.Format("航段信息错误");
                                               }
                                               else
                                               {
                                                   CarrierSetting carrierSetting = (businessman is Carrier) ?
                                                   (businessman as Carrier).CarrierSettings.Where(p => p.CarrayCode.ToUpper() == carryCode.ToUpper()).FirstOrDefault()
                                                   : (businessman as Supplier).CarrierSettings.Where(p => p.CarrayCode.ToUpper() == carryCode.ToUpper()).FirstOrDefault();
                                                   if (carrierSetting == null || string.IsNullOrEmpty(carrierSetting.CPOffice))
                                                   {
                                                       bSPResponse.Msg = string.Format("航空公司[{0}]BSP自动出票的Office没有设置,请在个人中心设置！", carryCode);
                                                   }
                                                   else
                                                   {
                                                       PID PidInfo = (businessman is Carrier) ?
           (businessman as Carrier).Pids.Where(p => p.Office.ToUpper() == carrierSetting.CPOffice.ToUpper()).FirstOrDefault() :
           (businessman as Supplier).SupPids.Where(p => p.Office.ToUpper() == carrierSetting.CPOffice.ToUpper()).FirstOrDefault();
                                                       if (PidInfo == null)
                                                       {
                                                           bSPResponse.Msg = string.Format("航空公司[{0}]BSP自动出票的Office没有设置,请在个人中心设置！", carryCode);
                                                       }
                                                       else
                                                       {
                                                           bSPParam.Param.ServerIP = pid.IP;
                                                           bSPParam.Param.ServerPort = pid.Port;
                                                           bSPParam.Param.Office = carrierSetting.CPOffice;
                                                           bSPParam.CarrayCode = carryCode;
                                                           if (string.IsNullOrEmpty(order.PnrCode))
                                                           {
                                                               bSPResponse.Msg = "编码不能为空！";
                                                           }
                                                           else
                                                           {
                                                               bSPParam.Pnr = order.PnrCode;
                                                               bSPParam.PrintNo = carrierSetting.PrintNo;
                                                               Passenger passenger = null;
                                                               if (bSPParam.IssueINFTicekt)
                                                               {
                                                                   passenger = order.Passengers.Where(p => p.PassengerType == EnumPassengerType.Baby).FirstOrDefault();
                                                               }
                                                               else
                                                               {
                                                                   passenger = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
                                                               }
                                                               if (passenger == null || passenger.SeatPrice == 0)
                                                               {
                                                                   bSPResponse.Msg = "乘客信息舱位价错误！";
                                                               }
                                                               else
                                                               {
                                                                   bSPParam.CpPrice = passenger.SeatPrice;
                                                               }
                                                           }
                                                       }
                                                   }
                                               }
                                           }
                                       }
                                   }
                                   #endregion

                                   bool IsSuc = false;
                                   int TryCount = 0;
                                   int MaxReconnectionCount = 0;
                                   int.TryParse(paramSetting.ReconnectionCount, out MaxReconnectionCount);
                                   while (TryCount < MaxReconnectionCount)
                                   {
                                       //if (string.IsNullOrEmpty(bSPResponse.Msg))
                                       //{
                                       //调用出票接口
                                       bSPParam.TryCount = (TryCount + 1);
                                       bSPResponse = autoTicketService.BSPAutoIssue(bSPParam);
                                       Dictionary<string, string> ticketDict = bSPResponse.BspResult;
                                       if (ticketDict.Count > 0)
                                       {
                                           var behavior = order.State.GetBehaviorByCode("TicketsIssue");
                                           behavior.SetParame("ticketDict", ticketDict);
                                           behavior.SetParame("operatorName", "系统");
                                           behavior.SetParame("platformCode", "系统");
                                           behavior.SetParame("opratorSource", "系统");
                                           behavior.SetParame("remark", order.Policy.PolicyType + "自动出票");
                                           behavior.Execute();
                                           IsSuc = true;
                                           resultCount++;
                                           break;
                                       }
                                       //}
                                       if (!IsSuc)
                                       {
                                           strFail = string.Format("BSP自动出票失败({0})：{1}\r\n", (TryCount + 1), bSPResponse.Msg);
                                           //日志
                                           order.WriteLog(new OrderLog()
                                           {
                                               OperationContent = strFail,
                                               OperationDatetime = DateTime.Now,
                                               OperationPerson = "系统",
                                               IsShowLog = false,
                                               Remark = strRemark
                                           });
                                           sbAuto.Append(strFail);
                                       }
                                       TryCount++;
                                       bSPResponse.Msg = "";
                                       Thread.Sleep(1000 * LeayTime);
                                   }
                                   #endregion
                               }
                               else
                               {
                                   strFail = string.Format("{0}自动出票设置未开启！", order.Policy.PolicyType);
                                   //日志
                                   order.WriteLog(new OrderLog()
                                   {
                                       OperationContent = strFail,
                                       OperationDatetime = DateTime.Now,
                                       OperationPerson = "系统",
                                       IsShowLog = false,
                                       Remark = strRemark
                                   });
                                   sbAuto.Append(strFail + "\r\n");
                               }
                               sbAuto.AppendFormat("提交修改订单:{0}\r\n", order.OrderId);
                               //提交
                               unitOfWorkRepository1.PersistUpdateOf(order);
                               unitOfWork1.Commit();
                               if (resultCount > 0)
                                   action();
                           }
                       }
                       catch (Exception ex)
                       {
                           sbAuto.AppendFormat("异常信息:{0}\r\n", ex.Message);
                       }
                       finally
                       {
                           new CommLog().WriteLog(order.Policy.PolicyType, sbAuto.ToString());
                       }
                   }
               });
        }

        private AutoIssueTicketViewModel GetB2BParam(string code)
        {
            BPiaoBao.Common.AutoIssueTicketViewModel vm = new Common.AutoIssueTicketViewModel();
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "AutoIssueTicket\\" + code + ".xml";
            if (File.Exists(path))
                vm = Common.XmlHelper.XmlDeserializeFromFile<BPiaoBao.Common.AutoIssueTicketViewModel>(path, Encoding.Default);
            else
            {
                BPiaoBao.Common.ExtHelper.GetCarryInfos().ForEach(p => vm.IssueTickets.Add(new BPiaoBao.Common.IssueTicketModel { CarrayCode = p.AirCode, CarrayName = p.Carry.AirShortName, Account = string.Empty, Pwd = string.Empty, ContactName = string.Empty, Phone = string.Empty }));
            }
            return vm;
        }

        /// <summary>
        /// 扩展参数
        /// </summary>
        /// <param name="paramSetting"></param>
        /// <param name="autoEtdzParam"></param>
        /// <param name="carryCode"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetExParam(AutoIssueTicketViewModel paramSetting, AutoEtdzParam autoEtdzParam, string carryCode)
        {
            if (paramSetting != null && paramSetting.IssueTickets.Count > 0)
            {
                carryCode = carryCode.ToUpper().Trim();
                paramSetting.IssueTickets.ForEach(p =>
                {
                    switch (carryCode)
                    {
                        case "HU":
                            if (!string.IsNullOrEmpty(p.Phone))
                                autoEtdzParam.ExParam.Add("hu_linktel", p.Phone);
                            if (!string.IsNullOrEmpty(p.ContactName))
                                autoEtdzParam.ExParam.Add("hu_linkman", p.ContactName);
                            break;
                        case "CZ":
                            if (!string.IsNullOrEmpty(p.Phone))
                                autoEtdzParam.ExParam.Add("cz_linktel", p.Phone);
                            break;
                        case "ZH":
                            if (!string.IsNullOrEmpty(p.Phone))
                            {
                                autoEtdzParam.ExParam.Add("zh_tel", p.Phone);
                                autoEtdzParam.ExParam.Add("zh_mobile", p.Phone);
                            }
                            if (!string.IsNullOrEmpty(p.ContactName))
                                autoEtdzParam.ExParam.Add("zh_name", p.ContactName);
                            break;
                        case "KY":
                            if (!string.IsNullOrEmpty(p.Phone))
                            {
                                autoEtdzParam.ExParam.Add("ky_tel", p.Phone);
                                autoEtdzParam.ExParam.Add("ky_mobile", p.Phone);
                            }
                            if (!string.IsNullOrEmpty(p.ContactName))
                                autoEtdzParam.ExParam.Add("ky_name", p.ContactName);
                            break;
                        default:
                            break;
                    }
                });
            }
            return autoEtdzParam.ExParam;
        }

        private void GetPat(Order order, out PatModel patMode, out PnrModel pnrMode)
        {
            pnrMode = null;
            patMode = null;
            //系统中的价格
            string Msg = string.Empty;
            SplitPnrCon splitPnrCon = format.GetSplitPnrCon(order.PnrContent);
            string RTCon = splitPnrCon.RTCon;
            string PatCon = splitPnrCon.AdultPATCon != string.Empty ? splitPnrCon.AdultPATCon : splitPnrCon.ChdPATCon;
            pnrMode = format.GetPNRInfo(order.PnrCode, RTCon, false, out Msg);
            //成人或者儿童PAT
            patMode = format.GetPATInfo(PatCon, out Msg);
        }

        private UrlInfo GetUrlInfo(UrlInfo urlInfo)
        {
            try
            {
                AutoIssueConfigurationElement AutoIssue = SettingSection.GetInstances().AutoIssue;
                urlInfo.AlipayAutoCPUrl = AutoIssue.AlipayAutoCPUrl;
                urlInfo.AlipayTicketNotifyUrl = AutoIssue.AlipayTicketNotifyUrl;
                urlInfo.AlipayPayNotifyUrl = AutoIssue.AlipayPayNotifyUrl;
            }
            catch (Exception)
            {
            }
            return urlInfo;
        }

        private object _instince = new object();
        /// <summary>
        /// 生成接口订单
        /// </summary>
        /// <param name="orderId"></param>    
        public void CreatePlatformOrderAndPaid(string orderId, string operatorName, string isNotify, bool IsTest = false)
        {
            WaitCallback call = new WaitCallback(it =>
            {
                lock (_instince)
                {
                    IUnitOfWork unitOfWork1 = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
                    IUnitOfWorkRepository unitOfWorkRepository1 = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
                    StringBuilder sbLog = new StringBuilder();
                    try
                    {
                        sbLog.AppendFormat("orderId={0}\r\noperatorName={1}\r\n isNotify={2}\r\n", orderId, operatorName, isNotify);
                        IOrderRepository orderRepository1 = ObjectFactory.GetInstance<IOrderRepository>();
                        var order = orderRepository1.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                        if (order == null)
                            throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
                        if (order.Policy == null)
                            throw new OrderCommException("订单编号(" + orderId + ")没有选择政策！");
                        if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                        {
                            if (order.OrderPay.PayStatus != EnumPayStatus.OK)
                            {
                                throw new OrderCommException("订单编号(" + orderId + ")没有支付！");
                            }
                            else if (order.OrderPay.PayStatus == EnumPayStatus.OK
                                && order.OrderPay.PaidStatus == EnumPaidStatus.OK)
                            {
                                throw new OrderCommException("订单编号(" + orderId + ")已支付,已代付！");
                            }
                            else
                            {
                                //调用生成接口订单
                                var behavior = order.State.GetBehaviorByCode("CreatePlatformOrder");
                                try
                                {
                                    behavior.SetParame("areaCity", order.Policy.AreaCity);
                                    behavior.SetParame("PlatformCode", order.Policy.PlatformCode);
                                    behavior.SetParame("operatorName", operatorName);
                                    behavior.SetParame("PlatformName", GetPlatList()[order.Policy.PlatformCode].Name);
                                    behavior.SetParame("isNotify", isNotify);
                                    behavior.Execute();
                                }
                                catch (Exception ex)
                                {
                                    sbLog.AppendFormat("生成接口订单异常信息:{0}\r\n", ex.Message);
                                }
                                //提交
                                unitOfWorkRepository1.PersistUpdateOf(order);
                                unitOfWork1.Commit();

                                sbLog.Append(order.OrderId + "生成接口订单成功,接口订单号:" + order.OutOrderId + "\r\n");
                                order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                                if (order.OrderStatus == EnumOrderStatus.WaitAndPaid
                                   && !string.IsNullOrEmpty(order.OutOrderId)
                                    && order.OrderPay.PaidStatus != EnumPaidStatus.OK
                                    )
                                {
                                    try
                                    {
                                        //调用代付
                                        behavior = order.State.GetBehaviorByCode("PaidOrder");
                                        behavior.SetParame("areaCity", order.Policy.AreaCity);
                                        behavior.SetParame("PlatformCode", order.Policy.PlatformCode);
                                        behavior.SetParame("operatorName", operatorName);
                                        behavior.SetParame("isNotify", isNotify);
                                        behavior.Execute();
                                    }
                                    catch (Exception ex)
                                    {
                                        sbLog.AppendFormat("代付接口订单异常信息:{0}\r\n", ex.Message);
                                    }
                                    sbLog.Append("订单号:" + order.OrderId + " 接口订单号：" + order.OutOrderId + " 代付成功\r\n");
                                }
                                //提交
                                unitOfWorkRepository1.PersistUpdateOf(order);
                                unitOfWork1.Commit();
                            }
                        }
                        else
                        {
                            sbLog.AppendFormat("订单号（{0}）政策非接口政策！\r\n", orderId);
                        }
                    }
                    catch (Exception ex)
                    {
                        sbLog.AppendFormat("错误信息:{0}\r\n", ex.Message);
                    }
                    finally
                    {
                        new CommLog().WriteLog("CreatePlatformOrderAndPaid", sbLog.ToString());
                    }
                }
            });
            if (IsTest)
            {
                call.Invoke(null);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(call);
            }
        }
    }
}

