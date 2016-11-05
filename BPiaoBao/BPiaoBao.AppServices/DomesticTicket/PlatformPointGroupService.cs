using System.Data.Entity;
using System.Globalization;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper.QueryableExtensions;
using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using ExpandQueryable = JoveZhao.Framework.Expand.ExpandQueryable;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class PlatformPointGroupService : IStationPlatformPointGroupService
    {
        private readonly IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        private readonly IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        private readonly IUnitOfWork systemUnitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        private readonly IUnitOfWorkRepository systemUnitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        private readonly IPlatformPointGroupRepository platformPointGroupRepository;
        private readonly IPlatformPointGroupRuleRepository platformPointGroupRuleRepository;
        private readonly IBusinessmanRepository businessmanRepository;
        public PlatformPointGroupService(IPlatformPointGroupRepository platformPointGroupRepository, IPlatformPointGroupRuleRepository platformPointGroupRuleRepository, IBusinessmanRepository businessmanRepository)
        {
            this.platformPointGroupRepository = platformPointGroupRepository;
            this.platformPointGroupRuleRepository = platformPointGroupRuleRepository;
            this.businessmanRepository = businessmanRepository;
        }

        public void AddPointGroup(PlatformPointGroupDataObject dataObject)
        {
            var count = platformPointGroupRepository.FindAll(p => p.GroupName == dataObject.GroupName).Count();
            if (count > 0)
            {
                throw new CustomException(505, "扣点组名称重复");
            }
            PlatformPointGroup pointGroup = AutoMapper.Mapper.Map<PlatformPointGroupDataObject, PlatformPointGroup>(dataObject);
            pointGroup.CreateDate = DateTime.Now;
            pointGroup.OperatorAccount = AuthManager.GetCurrentUser().OperatorAccount;
            this.unitOfWorkRepository.PersistCreationOf(pointGroup);
            this.unitOfWork.Commit();
            if (!string.IsNullOrWhiteSpace(pointGroup.Code))
            {
                var codeArray = pointGroup.Code.Split(',');
                var list = this.businessmanRepository.FindAll(p => codeArray.Contains(p.Code)).OfType<Carrier>().ToList();
                foreach (var item in list)
                {
                    item.PointGroupID = pointGroup.ID;
                    systemUnitOfWorkRepository.PersistUpdateOf(item);
                }
                systemUnitOfWork.Commit();
            }
        }

        public void UpdatePointGroup(PlatformPointGroupDataObject dataObject)
        {
            dataObject.CreateDate = DateTime.Now;
            dataObject.OperatorAccount = AuthManager.GetCurrentUser().OperatorAccount;
            PlatformPointGroup pointGroup = this.platformPointGroupRepository.FindAll(p => p.ID.Equals(dataObject.ID)).FirstOrDefault();
            if (pointGroup == null)
                throw new CustomException(505, "修改组不存在");
            AutoMapper.Mapper.Map<PlatformPointGroupDataObject, PlatformPointGroup>(dataObject, pointGroup);
            this.unitOfWorkRepository.PersistUpdateOf(pointGroup);
            this.unitOfWork.Commit();
            if (!string.IsNullOrWhiteSpace(pointGroup.Code))
            {
                var codeArray = pointGroup.Code.Split(',');
                var list = this.businessmanRepository.FindAll().OfType<Carrier>().Where(p => codeArray.Contains(p.Code) || p.PointGroupID == pointGroup.ID).ToList();
                foreach (var item in list)
                {
                    if (codeArray.Contains(item.Code))
                        item.PointGroupID = pointGroup.ID;
                    else
                        item.PointGroupID = null;
                    systemUnitOfWorkRepository.PersistUpdateOf(item);
                }
                systemUnitOfWork.Commit();
            }
        }

        public void DeletePointGroup(Guid[] guids)
        {
            this.platformPointGroupRepository.FindAll(p => guids.Contains(p.ID)).ToList().ForEach(x =>
            {
                this.platformPointGroupRuleRepository.FindAll(q => q.PlatformPointGroupID.HasValue && q.PlatformPointGroupID.Value == x.ID).ToList().ForEach(
                    y =>
                    {
                        y.DetailRules.Clear();
                        this.unitOfWorkRepository.PersistDeletionOf(y);

                    });
                //删除组下面的规则
                this.unitOfWorkRepository.PersistDeletionOf(x);
            });
            this.unitOfWork.Commit();


            //删除关联的组用户信息
            var list = this.businessmanRepository.FindAll().OfType<Carrier>().ToList();
            foreach (var item in from item in list from guid in guids where item.PointGroupID.HasValue && item.PointGroupID.Value.ToString().Contains(guid.ToString()) select item)
            {
                item.PointGroupID = null;
                systemUnitOfWorkRepository.PersistUpdateOf(item);
        }
            systemUnitOfWork.Commit();
        }

        public List<PlatformPointGroupDataObject> GetPointGroup(string code)
        {
            var query = this.platformPointGroupRepository.FindAllNoTracking();
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(p => p.Code.Contains(code));
            return query.Project().To<PlatformPointGroupDataObject>().ToList();
        }
        public void AddPointRule(PlatformPointGroupRuleDataObject dataObject)
        {
            if (DateTime.Compare(dataObject.StartDate, dataObject.EndDate) >= 0)
            {
                throw new CustomException(505, "开始日期大于等于结束日期");
            }
            CheckValidate(dataObject.DetailRules);
            PlatformPointGroupRule pointGroupRule = AutoMapper.Mapper.Map<PlatformPointGroupRuleDataObject, PlatformPointGroupRule>(dataObject);
            if (!this.platformPointGroupRuleRepository.ValidatePointGroupRule(pointGroupRule))
                throw new CustomException(505, "不能重复添加");
            this.unitOfWorkRepository.PersistCreationOf(pointGroupRule);
            this.unitOfWork.Commit();
        }

        public void UpdatePointRule(PlatformPointGroupRuleDataObject dataObject)
        {
            if (DateTime.Compare(dataObject.StartDate, dataObject.EndDate) >= 0)
            {
                throw new CustomException(505, "开始日期大于等于结束日期");
            }
            CheckValidate(dataObject.DetailRules);
            PlatformPointGroupRule pointGroupRule = this.platformPointGroupRuleRepository.FindAll(p => p.ID.Equals
                (dataObject.ID)).FirstOrDefault();
            if (pointGroupRule == null)
                throw new CustomException(505, "修改扣点规则不存在!");
            pointGroupRule.DetailRules.Clear();
            AutoMapper.Mapper.Map<PlatformPointGroupRuleDataObject, PlatformPointGroupRule>(dataObject, pointGroupRule);
            this.unitOfWorkRepository.PersistUpdateOf(pointGroupRule);
            this.unitOfWork.Commit();
        }

        public void DeletePointRule(int[] ids)
        {
            this.platformPointGroupRuleRepository.FindAll(p => ids.Contains(p.ID)).ToList().ForEach(y =>
            {
                y.DetailRules.Clear();
                this.unitOfWorkRepository.PersistDeletionOf(y);
            });
            this.unitOfWork.Commit();
        }

        public List<PlatformPointGroupRuleDataObject> GetPlatformPointGroupRuleDataObject(Guid? guid)
        {
            var query = this.platformPointGroupRuleRepository.FindAllNoTracking();
            if (guid.HasValue)
                query = query.Where(p => p.PlatformPointGroupID == guid.Value);
            return AutoMapper.Mapper.Map<List<PlatformPointGroupRule>, List<PlatformPointGroupRuleDataObject>>(query.ToList());
        }


        public PlatformPointGroupDataObject FindPointGroupByID(Guid guid)
        {
            return this.platformPointGroupRepository.FindAllNoTracking(p => p.ID.Equals(guid)).Project().To<PlatformPointGroupDataObject>().FirstOrDefault();
        }

        public PlatformPointGroupRuleDataObject FindPointRuleByID(int id)
        {
            return this.platformPointGroupRuleRepository.FindAllNoTracking(p => p.ID == id).Project().To<PlatformPointGroupRuleDataObject>().FirstOrDefault();
        }


        public List<CodePoint> GetCodePoint(Guid? guid)
        {
            var list = new List<CodePoint>();
            var carrierList = this.businessmanRepository.FindAllNoTracking().OfType<Carrier>().Select(p => new
            {
                Code = p.Code,
                Name = p.Name,
                PointGroupID = p.PointGroupID
            }).ToList();

            var groupList = this.platformPointGroupRepository.FindAllNoTracking().Select(p => new { ID = p.ID, GroupName = p.GroupName }).ToList();

            foreach (var p in carrierList)
            {
                var codePoint = new CodePoint
                {
                    Code = p.Code,
                    Name = p.Name,
                    IsSelected = false
                };
                if (p.PointGroupID.HasValue)
                {
                    var model = groupList.FirstOrDefault(m => m.ID == p.PointGroupID.Value);
                    if (model != null)
                    {
                        codePoint.GroupName = model.GroupName;
                    }
                    if (guid.HasValue && p.PointGroupID.Value == guid.Value)
                    {
                        codePoint.IsSelected = true;
                }
                    else
                    {
                        //////////////////跳过已经属于扣点组的用户
                        continue;
                    }
                }
                list.Add(codePoint);
            };
            return list;
        }


        public List<BResponse> GetPlatform(int? id)
        {
            List<BResponse> list = new List<BResponse>();
            list.AddRange(ObjectFactory.GetAllInstances<IPlatform>().Select(p => new BResponse
            {
                Code = p.Code,
                Name = p.Code,
                IsSelected = false
            }).ToList());

            list.AddRange(this.businessmanRepository.FindAllNoTracking(p => (p is Carrier) || (p is Supplier)).Select(p => new BResponse
              {
                  Code = p.Code,
                  Name = p.Name,
                  IsSelected = false
              }).ToList());

            if (id.HasValue)
            {
                var model = this.platformPointGroupRuleRepository.FindAllNoTracking(p => p.ID.Equals(id.Value)).FirstOrDefault();
                if (model != null && !string.IsNullOrWhiteSpace(model.IssueTicketCode))
                {
                    var array = model.IssueTicketCode.Split(',');
                    foreach (var item in array)
                    {
                        foreach (var localItem in list)
                        {
                            if (item == localItem.Code)
                                localItem.IsSelected = true;
                        }
                    }
                }
            }
            return list;
        }


        public List<PointGroup> GetAllPointGroup()
        {
            List<PointGroup> list = new List<PointGroup>();
            list.AddRange(this.platformPointGroupRepository.FindAllNoTracking().Select(p => new PointGroup
            {
                PointGroupID = p.ID,
                PointGroupName = p.GroupName
            }).ToList());
            return list;
        }


        #region 扣点规则验证

        /// <summary>
        /// 检查规则数组中的区间输入是否正确
        /// </summary>
        /// <param name="detailRules"></param>
        private void CheckValidate(List<PointGroupDetailRuleDataObject> detailRules)
        {
            if (detailRules == null || detailRules.Count == 0) return;
            var isValidate = true;
            if (detailRules.Count == 1)
            {
                GetTwoBetweenNumbers(detailRules[0].StartPoint, detailRules[0].EndPoint);
                CheckPointRule(detailRules[0]);
            }
            else
            {
                for (var i = 0; i < detailRules.Count - 1; i++)
                {
                    var rx = detailRules[i];
                    CheckPointRule(rx);
                    var first = GetTwoBetweenNumbers(rx.StartPoint, rx.EndPoint);
                    for (var j = i + 1; j < detailRules.Count; j++)
                    {
                        var ry = detailRules[j];
                        CheckPointRule(ry);
                        var second = GetTwoBetweenNumbers(ry.StartPoint, ry.EndPoint);
                        foreach (var d1 in first)
                        {
                            if (second.Any(d2 => d1 == d2))
                            {
                                isValidate = false;
                            }
                            if (!isValidate)
                            {
                                break;
                            }
                        }
                        if (!isValidate)
                        {
                            break;
                        }
                    }
                    if (!isValidate)
                    {
                        break;
                    }

                }
            }


            if (!isValidate)
            {
                throw new CustomException(505, "规则区间输入错误，区间之间不能有交集");
            }
        }

        /// <summary>
        /// 检测规则输入是否合法
        /// </summary>
        /// <param name="rule"></param>
        private void CheckPointRule(PointGroupDetailRuleDataObject rule)
        {
            if (rule.StartPoint < 0)
            {
                throw new CustomException(505, "规则区间开始值出现负数");
            }
            if (rule.StartPoint >= rule.EndPoint)
            {
                throw new CustomException(505, "规则开始值大于结束值");
            }
            if (rule.StartPoint < 0 || rule.StartPoint > 100)
            {
                throw new CustomException(505, "规则数值必须在0-100之间的数字");
            }
            if (rule.EndPoint < 0 || rule.EndPoint > 100)
            {
                throw new CustomException(505, "规则数值必须在0-100之间的数字");
            }
            if (rule.Point <= 0 || rule.Point >= 100)
            {
                throw new CustomException(505, "点数设置错误");
            }
            var temp = rule.Point.ToString(CultureInfo.InvariantCulture);
            if (temp.Contains(".") && (temp.Split('.')[1].Length > 1))
            {
                throw new CustomException(505, "点数设置请保留小数点1位");

            }
        }
        /// <summary>
        /// 获取两个字之间的数字，默认取小数点后两位有效果
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private List<decimal> GetTwoBetweenNumbers(decimal start, decimal end)
        {
            var tempS = start.ToString(CultureInfo.InvariantCulture);
            var tempE = end.ToString(CultureInfo.InvariantCulture);
            if (tempS.Contains(".") && (tempS.Split('.')[1].Length > 1))
            {
                throw new CustomException(505, "区间设置请保留小数点1位");

            }
            if (tempE.Contains(".") && (tempE.Split('.')[1].Length > 1))
            {
                throw new CustomException(505, "区间设置请保留小数点1位");

            }
            var s = start * 10;
            var e = end * 10;

            var array = new List<decimal> { s };
            for (var i = s + 1M; i < e; i++)
            {
                array.Add(i);
            }
            array.Add(e);
            return array;
        }

        #endregion
    }
}
