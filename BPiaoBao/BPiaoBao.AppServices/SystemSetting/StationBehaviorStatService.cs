using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.AppServices.StationContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class BehaviorStatService : IStationBehaviorStatService
    {
        public StationContracts.StationMap.PagedList<StationContracts.SystemSetting.SystemMap.ResponseBehaviorStat> FindBehaviorList(StationContracts.SystemSetting.SystemMap.QueryBehaviorStatQuery query)
        {
            var result = behaviorStatRepository.FindAll(p=>p.BusinessmanType == "Buyer");

            if (!string.IsNullOrEmpty(query.BusinessmanCode))
            {
                result = result.Where(p => p.BusinessmanCode.ToLower().Equals(query.BusinessmanCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(query.CarrierCode))
            {
                result = result.Where(p => p.CarrierCode.ToLower().Equals(query.CarrierCode.ToLower()));
            }
            if (!string.IsNullOrEmpty(query.ContactName))
            {
                result = result.Where(p => p.ContactName.Equals(query.ContactName));
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

            var res = BehaviorToResponseBehaviorStats(result);
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
            res = res.Skip((query.PageIndex - 1) * query.PageSize).Take(query.PageSize).ToList();
            var pagelist = new PagedList<ResponseBehaviorStat>();
            pagelist.Total = totalCount;
            pagelist.Rows = res;
            return pagelist;
        }
        /// <summary>
        /// 行转列函数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<ResponseBehaviorStat> BehaviorToResponseBehaviorStats(IQueryable<BehaviorStat> list)
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
