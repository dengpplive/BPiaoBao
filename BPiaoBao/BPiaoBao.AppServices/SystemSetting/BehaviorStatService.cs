using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using StructureMap;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class BehaviorStatService : BaseService, IBehaviorStatService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        IBehaviorStatRepository behaviorStatRepository;
        static IBusinessmanRepository businessmanRepository;
        public BehaviorStatService(IBehaviorStatRepository behaviorStatRepository, IBusinessmanRepository businessmanRepository)
        {
            this.behaviorStatRepository = behaviorStatRepository;
            BehaviorStatService.businessmanRepository = businessmanRepository;
        }

        /// <summary>
        ///  保存当前用户操作行为(登录专用)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="enumBeOperate"></param>

        public static void SaveLoginBehaviorStat(string token, EnumBehaviorOperate enumBeOperate)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    Logger.WriteLog(LogType.ERROR, "传入Token为空");
                }
                var auth = new AuthService(businessmanRepository);
                var user = auth.GetCurrentUserByToken(token);
                if (user == null)
                {
                    Logger.WriteLog(LogType.WARN, string.Format("获取用户登录信息失败[token:{0}]", token));
                }
                else
                {
                    SaveBehaviorStat(user.Code, user.BusinessmanName, user.Type, user.CarrierCode, user.ContactName, user.OperatorName, enumBeOperate);
                  
                }

            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.WARN, "保存用户操作行为异常[" + enumBeOperate + "]", e);

            }
        }



        /// <summary>
        /// 保存当前用户操作行为（对外公开）
        /// </summary>
        /// <param name="enumBeOperate">操作类型</param>
        public static void SaveBehaviorStat(EnumBehaviorOperate enumBeOperate)
        {
            try
            {
                var user = AuthManager.GetCurrentUser();
                SaveBehaviorStat(user.Code, user.BusinessmanName, user.Type, user.CarrierCode, user.ContactName, user.OperatorName, enumBeOperate);
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "保存用户操作行为异常[" + enumBeOperate + "]", e);

            }
        }
        public static void SaveBehaviorStat(string code, EnumBehaviorOperate enumBeOperate)
        {
            try
            {

                var model = businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
                if (model == null)
                    return;
                var businessName = string.Empty;
                var type = string.Empty;
                var carrierCode = string.Empty;
                var contactName = string.Empty;//业务员
                var operatorName = string.Empty;
                if (model is Carrier)
                {
                    carrierCode = code;
                    type = "Carrier";
                }
                else if (model is Buyer)
                {
                    carrierCode = (model as Buyer).CarrierCode;
                    type = "Buyer";

                }
                else if (model is Supplier)
                {
                    type = "Supplier";
                }
                businessName = model.Name;
                contactName = model.ContactName;
                operatorName = model.Operators != null ? model.Operators.FirstOrDefault().Realname : "";
                SaveBehaviorStat(code, businessName, type, carrierCode, contactName, operatorName, enumBeOperate);
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "保存用户操作行为异常[" + enumBeOperate + "]", e);
                
            }
        }
        /// <summary>
        /// 保存用户操作行为
        /// </summary>
        /// <param name="businessmanCode">商户号</param>
        /// <param name="businessmanName">商户名称</param>
        /// <param name="businessmanType">商户类别</param>
        /// <param name="carrierCode">运营商Code</param>
        /// <param name="contactName">所属业务员</param>
        /// <param name="OperatorName">操作员</param>
        /// <param name="enumBeOperate">操作类型</param>
        private static void SaveBehaviorStat(string businessmanCode, string businessmanName, string businessmanType, string carrierCode, string contactName, string OperatorName, EnumBehaviorOperate enumBeOperate)
        {
            //try
            //{
            //    var service = ObjectFactory.GetInstance<BehaviorStatService>();
            //    service.Save(businessmanCode, businessmanName, businessmanType, carrierCode, contactName, OperatorName, enumBeOperate);
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            BehaviorStat star = new BehaviorStat();
            star.BusinessmanCode = businessmanCode;
            star.BusinessmanName = businessmanName;
            star.BusinessmanType = businessmanType;
            star.CarrierCode = carrierCode;
            star.ContactName = contactName;
            star.OperatorName = OperatorName;
            star.BehaviorOperate = (int)enumBeOperate;
            star.OpDateTime = DateTime.Now;
            UserBehaviorManage.list.Add(star);
        }

        /// <summary>
        /// 保存用户操作行为
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="businessmanName"></param>
        /// <param name="businessmanType"></param>
        /// <param name="carrierCode"></param>
        /// <param name="contactName"></param>
        /// <param name="OperatorName"></param>
        /// <param name="enumBeOperate"></param>
        private void Save(string businessmanCode, string businessmanName, string businessmanType, string carrierCode, string contactName, string OperatorName, EnumBehaviorOperate enumBeOperate)
        {
            try
            {
                var builder = AggregationFactory.CreateBuiler<BehaviorStatBuilder>();
                var behaviorStat = builder.CreateBehaviorStat();
                behaviorStat.BusinessmanCode = businessmanCode;
                behaviorStat.BusinessmanName = businessmanName;
                behaviorStat.BusinessmanType = businessmanType;
                behaviorStat.CarrierCode = carrierCode;
                behaviorStat.ContactName = contactName;
                behaviorStat.OperatorName = OperatorName;
                behaviorStat.OpDateTime = DateTime.Now;
                behaviorStat.BehaviorOperate = (int)enumBeOperate;
                unitOfWorkRepository.PersistCreationOf(behaviorStat);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// 查询用户操作行为
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataPack<ResponseBehaviorStat> Query(RequestQueryBehaviorStatQuery query)
        {
            string carriercode = AuthManager.GetCurrentUser().Code;
            var contactName = AuthManager.GetCurrentUser().ContactName;
            var isAdmin = AuthManager.GetCurrentUser().IsAdmin;
            int pSize = query.PageSize ?? 10;
            int count = ((query.PageIndex == default(int) ? 1 : query.PageIndex) - 1) * pSize;

            var result = behaviorStatRepository.FindAll();

            if (!string.IsNullOrEmpty(carriercode))
            {
                result = result.Where(p => p.CarrierCode.ToLower().Equals(carriercode.ToLower()));
            }
            if (!string.IsNullOrEmpty(query.BusinessmanCode))
            {
                result = result.Where(p => p.BusinessmanCode.ToLower().Equals(query.BusinessmanCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(query.BusinessmanName))
            {
                result = result.Where(p => p.BusinessmanName.ToLower().Contains(query.BusinessmanName.ToLower()));
            }
            if (!string.IsNullOrEmpty(query.BusinessmanType))
            {
                result = result.Where(p => p.BusinessmanType.ToLower().Equals(query.BusinessmanType.ToLower()));
            }
            if (!isAdmin && !string.IsNullOrEmpty(contactName))
            {
                result = result.Where(p => p.ContactName.ToLower().Equals(contactName.ToLower()));
            }
            if (query.StartDateTime != null)
            {
                var date = query.StartDateTime.Value.Date;
                result = result.Where(p => p.OpDateTime >= date);
            }
            if (query.EndDateTime != null)
            {
                var date = query.EndDateTime.Value.Date.AddDays(1).AddSeconds(-1);
                result = result.Where(p => p.OpDateTime <= date);
            }

            var res = ProcessBehaviorStatsToResponseBehaviorStats(result);
            if (string.IsNullOrWhiteSpace(query.Sort) || string.IsNullOrWhiteSpace(query.Order))
            {
                res = res.OrderByDescending(p => p.OutTicketCount).ToList();
            }
            else
            {
                if (query.Order == "desc")
                {
                    switch (query.Sort)
                    {
                        case "LoginCount":
                            res = res.OrderByDescending(p => p.LoginCount).ToList();
                            break;
                        case "QueryCount":
                            res = res.OrderByDescending(p => p.QueryCount).ToList();
                            break;
                        case "ImportCount":
                            res = res.OrderByDescending(p => p.ImportCount).ToList();
                            break;
                        case "OutTicketCount":
                            res = res.OrderByDescending(p => p.OutTicketCount).ToList();
                            break;
                        case "BackTicketCount":
                            res = res.OrderByDescending(p => p.BackTicketCount).ToList();
                            break;
                        case "AbolishTicketCount":
                            res = res.OrderByDescending(p => p.AbolishTicketCount).ToList();
                            break;
                        case "AccessCount":
                            res = res.OrderByDescending(p => p.AccessCount).ToList();
                            break;
                        case "FinancingCount":
                            res = res.OrderByDescending(p => p.FinancingCount).ToList();
                            break;
                        case "UseCount":
                            res = res.OrderByDescending(p => p.UseCount).ToList();
                            break;
                        default:
                            res = res.OrderByDescending(p => p.OutTicketCount).ToList();
                            break;
                            
                    }
                }
                else
                {
                    switch (query.Sort)
                    {
                        case "LoginCount":
                            res = res.OrderBy(p => p.LoginCount).ToList();
                            break;
                        case "QueryCount":
                            res = res.OrderBy(p => p.QueryCount).ToList();
                            break;
                        case "ImportCount":
                            res = res.OrderBy(p => p.ImportCount).ToList();
                            break;
                        case "OutTicketCount":
                            res = res.OrderBy(p => p.OutTicketCount).ToList();
                            break;
                        case "BackTicketCount":
                            res = res.OrderBy(p => p.BackTicketCount).ToList();
                            break;
                        case "AbolishTicketCount":
                            res = res.OrderBy(p => p.AbolishTicketCount).ToList();
                            break;
                        case "AccessCount":
                            res = res.OrderBy(p => p.AccessCount).ToList();
                            break;
                        case "FinancingCount":
                            res = res.OrderBy(p => p.FinancingCount).ToList();
                            break;
                        case "UseCount":
                            res = res.OrderBy(p => p.UseCount).ToList();
                            break;
                        default:
                            res = res.OrderBy(p => p.OutTicketCount).ToList();
                            break;
                    }
                }
            }

            var totalCount = res.Count();
            res = res.Skip(count).Take(pSize).ToList();
            var dataPack = new DataPack<ResponseBehaviorStat>();
            dataPack.TotalCount = totalCount;
            dataPack.List = res;
            return dataPack;

        }


        /// <summary>
        /// 行转列函数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<ResponseBehaviorStat> ProcessBehaviorStatsToResponseBehaviorStats(IQueryable<BehaviorStat> list)
        {
            var respList = new List<ResponseBehaviorStat>();
            var result = from m in list
                         group m by new
                         {
                             m.BusinessmanCode,
                             m.BusinessmanName,
                             m.BusinessmanType,
                             m.CarrierCode,
                             //m.OpDateTime,
                             m.BehaviorOperate,
                             m.ContactName,
                             m.OperatorName
                         }
                             into g
                             //  where g.Key.OpDateTime >= startTime && g.Key.OpDateTime <= endTime
                             select
                             new
                             {
                                 Obj = g.Key,
                                 Count = g.Count()
                             };
            foreach (var m in result)
            {
                var model = m.Obj;
                if (model.ContactName == null)
                {
                    continue;
                }
                var rsp = respList.FirstOrDefault(rspm =>
                  rspm.BusinessmanCode.ToLower().Equals(model.BusinessmanCode.ToLower())
                      //  && rspm.OpDateTime.Date.Equals(model.OpDateTime.Date)
                   && rspm.ContactName.Trim().Equals(model.ContactName.Trim(), StringComparison.OrdinalIgnoreCase)
                  );
                if (rsp == null)
                {
                    rsp = new ResponseBehaviorStat
                    {
                        BusinessmanCode = model.BusinessmanCode,
                        BusinessmanName = model.BusinessmanName,
                        BusinessmanType = model.BusinessmanType,
                        CarrierCode = model.CarrierCode,
                        //OpDateTime = model.OpDateTime.Date,
                        ContactName = model.ContactName,
                        OperatorName = model.OperatorName
                    };

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.LoginCount)
                    {
                        rsp.LoginCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.QueryCount)
                    {
                        rsp.QueryCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.ImportCount)
                    {
                        rsp.ImportCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.OutTicketCount)
                    {
                        rsp.OutTicketCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.BackTicketCount)
                    {
                        rsp.BackTicketCount += m.Count;
                    }

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.AbolishTicketCount)
                    {
                        rsp.AbolishTicketCount += m.Count;
                    }

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.AccessCount)
                    {
                        rsp.AccessCount += m.Count;
                    }

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.FinancingCount)
                    {
                        rsp.FinancingCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.UseCount)
                    {
                        rsp.UseCount += m.Count;
                    }

                    respList.Add(rsp);
                }
                else
                {
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.LoginCount)
                    {
                        rsp.LoginCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.QueryCount)
                    {
                        rsp.QueryCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.ImportCount)
                    {
                        rsp.ImportCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.OutTicketCount)
                    {
                        rsp.OutTicketCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.BackTicketCount)
                    {
                        rsp.BackTicketCount += m.Count;
                    }

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.AbolishTicketCount)
                    {
                        rsp.AbolishTicketCount += m.Count;
                    }

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.AccessCount)
                    {
                        rsp.AccessCount += m.Count;
                    }

                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.FinancingCount)
                    {
                        rsp.FinancingCount += m.Count;
                    }
                    if (model.BehaviorOperate == (int)EnumBehaviorOperate.UseCount)
                    {
                        rsp.UseCount += m.Count;
                    }
                }
            }

            return respList;
        }
    }
}
