using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using JoveZhao.Framework.DDD;
using BPiaoBao.Common;
using StructureMap;
using JoveZhao.Framework;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class ConsoLocalPolicyService : IConsoLocalPolicyService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());

        ILocalPolicyRepository localPolicyRepository;
        IBusinessmanRepository businessmanRepository;
        public ConsoLocalPolicyService(ILocalPolicyRepository localPolicyRepository, IBusinessmanRepository _businessmanRepository)
        {
            this.localPolicyRepository = localPolicyRepository;
            this.businessmanRepository = _businessmanRepository;
        }
        public void AddLocalPolicy(RequestNormalPolicy policy)
        {
            var currentUser = AuthManager.GetCurrentUser();
            var builder = AggregationFactory.CreateBuiler<LocalPolicyBuilder>();
            var localPolicy = builder.CreateLocalNormalPolicy(Mapper.Map<RequestNormalPolicy, LocalNormalPolicy>(policy));
            localPolicy.CreateMan = currentUser.OperatorName;
            localPolicy.RoleType = currentUser.Type;
            localPolicy.Code = currentUser.Code;
            if (currentUser.Type == "Supplier")
            {
                localPolicy.CarrierCode = currentUser.CarrierCode;
                var bm = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.CarrierCode).OfType<Carrier>().FirstOrDefault();
                if (bm != null)
                {
                    localPolicy.CarrierWeek = bm.RestWork.WeekDay;
                    localPolicy.Carrier_AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.WorkOnLineTime,
                        EndTime = bm.NormalWork.WorkUnLineTime
                    };
                    localPolicy.Carrier_WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.WorkOnLineTime,
                        EndTime = bm.RestWork.WorkUnLineTime
                    };
                }
                var supbm = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.Code).OfType<Supplier>().FirstOrDefault();
                if (supbm != null)
                {

                    localPolicy.SupplierWeek = supbm.SupRestWork.WeekDay;
                    localPolicy.AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupNormalWork.ServiceOnLineTime,
                        EndTime = supbm.SupNormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupNormalWork.ServiceOnLineTime,
                        EndTime = supbm.SupNormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupNormalWork.WorkOnLineTime,
                        EndTime = supbm.SupNormalWork.WorkUnLineTime
                    };
                    localPolicy.WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupRestWork.ServiceOnLineTime,
                        EndTime = supbm.SupRestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupRestWork.ServiceOnLineTime,
                        EndTime = supbm.SupRestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.WeeKWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupRestWork.WorkOnLineTime,
                        EndTime = supbm.SupRestWork.WorkUnLineTime
                    };
                }
            }
            else
            {
                localPolicy.CarrierCode = currentUser.Code;
                var bm = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.Code).OfType<Carrier>().FirstOrDefault();
                if (bm != null)
                {
                    localPolicy.CarrierWeek = bm.RestWork.WeekDay;
                    localPolicy.Carrier_AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.WorkOnLineTime,
                        EndTime = bm.NormalWork.WorkUnLineTime
                    };
                    localPolicy.Carrier_WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.WorkOnLineTime,
                        EndTime = bm.RestWork.WorkUnLineTime
                    };
                }
            }
            unitOfWorkRepository.PersistCreationOf(localPolicy);
            unitOfWork.Commit();
        }


        public void PartUpdateLocalPolicy(RequestPartPolicy policy)
        {
            if (policy == null)
                throw new CustomException(400, "输入数据不能为空");
            var localPolicy = this.localPolicyRepository.FindAll(p => p.ID == policy.ID).OfType<LocalNormalPolicy>().FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(500, "查找修改政策不存在");
            string oldStr = localPolicy.ToString();
            localPolicy.LocalPoint = policy.LocalPoint;
            localPolicy.Different = policy.Different;
            localPolicy.TravelType = policy.TravelType;
            localPolicy.LocalPolicyType = policy.LocalPolicyType;
            localPolicy.Seats = policy.Seats;
            localPolicy.Low = policy.Low;
            localPolicy.ChangeCode = policy.ChangeCode;
            localPolicy.PassengeDate.StartTime = policy.PassengeDate.StartTime;
            localPolicy.PassengeDate.EndTime = policy.PassengeDate.EndTime;
            localPolicy.IssueDate.StartTime = policy.IssueDate.StartTime;
            localPolicy.IssueDate.EndTime = policy.IssueDate.EndTime;

            unitOfWorkRepository.PersistUpdateOf(localPolicy);
            unitOfWork.Commit();
            string newStr = localPolicy.ToString();
            try
            {
                Logger.WriteLog(LogType.INFO, oldStr + "__" + newStr);
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "写入错误", e);
            }
        }


        public PagedList<ResponsePolicy> FindPolicyByPager(SearchPolicy search, int page, int rows)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var query = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code).OfType<LocalNormalPolicy>();
            if (search == null)
                search = new SearchPolicy();
            if (!string.IsNullOrEmpty(search.PolicyType) && !string.IsNullOrEmpty(search.PolicyType.Trim()))
                query = query.Where(p => p.LocalPolicyType == search.PolicyType.Trim());
            if (search.LocalStartPoint.HasValue)
                query = query.Where(p => p.LocalPoint >= search.LocalStartPoint.Value);
            if (search.LocalEndPoint.HasValue)
                query = query.Where(p => p.LocalPoint <= search.LocalEndPoint.Value);
            if (search.PStartTime.HasValue)
                query = query.Where(p => p.PassengeDate.StartTime >= search.PStartTime.Value);
            if (search.PEndTime.HasValue)
            {
                search.PEndTime = search.PEndTime.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.PassengeDate.EndTime <= search.PEndTime.Value);
            }
            if (search.Review.HasValue)
                query = query.Where(p => p.Review == search.Review.Value);
            if (!string.IsNullOrEmpty(search.FromCityCode) && !string.IsNullOrEmpty(search.FromCityCode.Trim()))
                query = query.Where(p => p.FromCityCodes.Contains(search.FromCityCode.Trim()));
            if (!string.IsNullOrEmpty(search.ToCityCode) && !string.IsNullOrEmpty(search.ToCityCode.Trim()))
                query = query.Where(p => p.ToCityCodes.Contains(search.ToCityCode.Trim()));
            if (search.TravelType.HasValue)
                query = query.Where(p => p.TravelType == search.TravelType.Value);
            if (search.ReleaseType.HasValue)
                query = query.Where(p => p.ReleaseType == search.ReleaseType.Value);
            if (search.IssueTicketWay.HasValue)
                query = query.Where(p => p.IssueTicketWay == search.IssueTicketWay.Value);
            if (!string.IsNullOrEmpty(search.CarrayCode))
                query = query.Where(p => p.CarrayCode.Equals(search.CarrayCode));
            return new PagedList<ResponsePolicy>()
            {
                Total = query.Count(),
                Rows = query.OrderByDescending(p => p.CreateDate).Skip((page - 1) * rows).Take(rows).Project().To<ResponsePolicy>().ToList()
            };

        }

        public void BatchReview(Guid[] ids)
        {
            var code = AuthManager.GetCurrentUser().Code;
            this.localPolicyRepository.FindAll(p => p.Code == code && ids.Contains(p.ID)).ToList().ForEach(p =>
            {
                p.Review = true;
                unitOfWorkRepository.PersistUpdateOf(p);
            });
            unitOfWork.Commit();
        }


        public void BatchDelete(Guid[] ids)
        {
            var code = AuthManager.GetCurrentUser().Code;
            this.localPolicyRepository.FindAll(p => p.Code == code && ids.Contains(p.ID)).ToList().ForEach(p =>
            {
                unitOfWorkRepository.PersistDeletionOf(p);
            });
            unitOfWork.Commit();
        }


        public void BatchCancelHangUp(Guid[] ids)
        {
            var code = AuthManager.GetCurrentUser().Code;
            this.localPolicyRepository.FindAll(p => p.Code == code && ids.Contains(p.ID)).ToList().ForEach(p =>
            {
                p.HangUp = true;
                unitOfWorkRepository.PersistUpdateOf(p);
            });
            unitOfWork.Commit();
        }


        public ResponseFullPolicy Find(Guid id)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code && p.ID == id).FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在!");
            return Mapper.Map<LocalPolicy, ResponseFullPolicy>(localPolicy);
        }

        /// <summary>
        /// 特价政策详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseOperPolicy FindSpecialPolicyInfo(Guid id)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code && p.ID == id).OfType<SpecialPolicy>().FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在!");
            return Mapper.Map<SpecialPolicy, ResponseOperPolicy>(localPolicy);
        }


        /// <summary>
        /// 查询单条政策-政策对比
        /// </summary>
        /// <param name="polyid">政策Id</param>
        /// <param name="value">无效值</param>
        /// <returns></returns>
        public ResponseLocalPolicy Find(Guid polyid, int value = 0)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code && p.ID == polyid).FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在!");
            return Mapper.Map<LocalPolicy, ResponseLocalPolicy>(localPolicy);
        }
        /// <summary>
        /// 特价政策修改信息查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseOperPolicy EditSpeciaFind(Guid id)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code && p.ID == id).OfType<SpecialPolicy>().FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在!");
            return Mapper.Map<SpecialPolicy, ResponseOperPolicy>(localPolicy); 
        }

        public RequestNormalPolicy EditFind(Guid id)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code && p.ID == id).OfType<LocalNormalPolicy>().Project().To<RequestNormalPolicy>().FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在!");
            return localPolicy;
        }


        public RequestPartPolicy EditPartFind(Guid id)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code && p.ID == id).Project().To<RequestPartPolicy>().FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在或被删除!");
            return localPolicy;
        }


        public CabinData GetBaseCabinData(string CarryCode)
        {
            CabinData cabin = new CabinData();
            if (!string.IsNullOrEmpty(CarryCode))
            {
                cabin = new QueryFlightService().GetBaseCabinUsePolicy(CarryCode);
                if (cabin == null || cabin.CabinList.Count == 0)
                    throw new CustomException(400, "基础舱位不存在!");
            }
            return cabin;
        }


        public void ImportPolicy(List<RequestNormalPolicy> list)
        {

            var currentUser = AuthManager.GetCurrentUser();
            string _carrierCode = string.Empty;
            Carrier carrier = null;
            Supplier supplier = null;
            if (currentUser.Type == "Supplier")
            {
                _carrierCode = currentUser.CarrierCode;
                supplier = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.Code).OfType<Supplier>().FirstOrDefault();
            }
            else
                _carrierCode = currentUser.Code;

            carrier = this.businessmanRepository.FindAllNoTracking(p => p.Code == _carrierCode).OfType<Carrier>().FirstOrDefault();
            List<LocalNormalPolicy> policyList = Mapper.Map<List<RequestNormalPolicy>, List<LocalNormalPolicy>>(list);
            policyList.ForEach(p =>
            {
                p.CreateDate = DateTime.Now;
                p.CreateMan = currentUser.OperatorName;
                p.Code = currentUser.Code;
                p.Review = false;
                p.HangUp = false;
                p.RoleType = currentUser.Type;
                p.CarrierCode = _carrierCode;
                if (carrier != null)
                {
                    p.CarrierWeek = carrier.RestWork.WeekDay;
                    p.Carrier_AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                        EndTime = carrier.NormalWork.ServiceUnLineTime
                    };
                    p.Carrier_ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                        EndTime = carrier.NormalWork.ServiceUnLineTime
                    }; ;
                    p.Carrier_WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.NormalWork.WorkOnLineTime,
                        EndTime = carrier.NormalWork.WorkUnLineTime
                    };
                    
                    p.Carrier_WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.RestWork.ServiceOnLineTime,
                        EndTime = carrier.RestWork.ServiceUnLineTime
                    }; ;
                    p.Carrier_WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.RestWork.ServiceOnLineTime,
                        EndTime = carrier.RestWork.ServiceUnLineTime
                    };
                    p.Carrier_WeekWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.RestWork.WorkOnLineTime,
                        EndTime = carrier.RestWork.WorkUnLineTime
                    };
                }
                if (currentUser.Type == "Supplier")
                {
                    if (supplier != null)
                    {
                        p.SupplierWeek = supplier.SupRestWork.WeekDay;
                        p.AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupNormalWork.ServiceOnLineTime,
                            EndTime = supplier.SupNormalWork.ServiceUnLineTime
                        }; ;
                        p.ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupNormalWork.ServiceOnLineTime,
                            EndTime = supplier.SupNormalWork.ServiceUnLineTime
                        }; ;
                        p.WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupNormalWork.WorkOnLineTime,
                            EndTime = supplier.SupNormalWork.WorkUnLineTime
                        };
                        
                        p.WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupRestWork.ServiceOnLineTime,
                            EndTime = supplier.SupRestWork.ServiceUnLineTime
                        }; ;
                        p.WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupRestWork.ServiceOnLineTime,
                            EndTime = supplier.SupRestWork.ServiceUnLineTime
                        }; ;
                        p.WeeKWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupRestWork.WorkOnLineTime,
                            EndTime = supplier.SupRestWork.WorkUnLineTime
                        };
                    }
                }
                this.unitOfWorkRepository.PersistCreationOf(p);
            });
            this.unitOfWork.Commit();
        }
        public void ImportPolicy(List<RequestSpecialPolicy> list)
        {

            var currentUser = AuthManager.GetCurrentUser();
            string _carrierCode = string.Empty;
            Carrier carrier = null;
            Supplier supplier = null;
            if (currentUser.Type == "Supplier")
            {
                _carrierCode = currentUser.CarrierCode;
                supplier = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.Code).OfType<Supplier>().FirstOrDefault();
            }
            else
                _carrierCode = currentUser.Code;

            carrier = this.businessmanRepository.FindAllNoTracking(p => p.Code == _carrierCode).OfType<Carrier>().FirstOrDefault();
            List<SpecialPolicy> policyList = Mapper.Map<List<RequestSpecialPolicy>, List<SpecialPolicy>>(list);
            policyList.ForEach(p =>
            {
                p.CreateDate = DateTime.Now;
                p.CreateMan = currentUser.OperatorName;
                p.Code = currentUser.Code;
                p.Review = false;
                p.HangUp = false;
                p.RoleType = currentUser.Type;
                p.CarrierCode = _carrierCode;
                if (carrier != null)
                {
                    p.CarrierWeek = carrier.RestWork.WeekDay;
                    p.Carrier_AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                        EndTime = carrier.NormalWork.ServiceUnLineTime
                    };
                    p.Carrier_ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                        EndTime = carrier.NormalWork.ServiceUnLineTime
                    }; ;
                    p.Carrier_WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.NormalWork.WorkOnLineTime,
                        EndTime = carrier.NormalWork.WorkUnLineTime
                    };

                    p.Carrier_WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.RestWork.ServiceOnLineTime,
                        EndTime = carrier.RestWork.ServiceUnLineTime
                    }; ;
                    p.Carrier_WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.RestWork.ServiceOnLineTime,
                        EndTime = carrier.RestWork.ServiceUnLineTime
                    };
                    p.Carrier_WeekWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = carrier.RestWork.WorkOnLineTime,
                        EndTime = carrier.RestWork.WorkUnLineTime
                    };
                }
                if (currentUser.Type == "Supplier")
                {
                    if (supplier != null)
                    {
                        p.SupplierWeek = supplier.SupRestWork.WeekDay;
                        p.AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupNormalWork.ServiceOnLineTime,
                            EndTime = supplier.SupNormalWork.ServiceUnLineTime
                        }; ;
                        p.ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupNormalWork.ServiceOnLineTime,
                            EndTime = supplier.SupNormalWork.ServiceUnLineTime
                        }; ;
                        p.WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupNormalWork.WorkOnLineTime,
                            EndTime = supplier.SupNormalWork.WorkUnLineTime
                        };

                        p.WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupRestWork.ServiceOnLineTime,
                            EndTime = supplier.SupRestWork.ServiceUnLineTime
                        }; ;
                        p.WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupRestWork.ServiceOnLineTime,
                            EndTime = supplier.SupRestWork.ServiceUnLineTime
                        }; ;
                        p.WeeKWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                        {
                            StartTime = supplier.SupRestWork.WorkOnLineTime,
                            EndTime = supplier.SupRestWork.WorkUnLineTime
                        };
                    }
                }
                this.unitOfWorkRepository.PersistCreationOf(p);
            });
            this.unitOfWork.Commit();
        }

        public void BatchHangUp(Guid[] ids)
        {
            var code = AuthManager.GetCurrentUser().Code;
            this.localPolicyRepository.FindAll(p => p.Code == code && ids.Contains(p.ID)).ToList().ForEach(p =>
            {
                p.HangUp = false;
                unitOfWorkRepository.PersistUpdateOf(p);
            });
            unitOfWork.Commit();
        }


        public void UpdateLocalPolicy(RequestNormalPolicy policy)
        {
            var currentUser = AuthManager.GetCurrentUser();
            var localPolicy = this.localPolicyRepository.FindAll(p => p.ID == policy.ID && p.Code == currentUser.Code).OfType<LocalNormalPolicy>().FirstOrDefault();

            if (localPolicy == null)
                throw new CustomException(500, "政策不存在或被删除");
            string oldStr = localPolicy.ToString();

            Mapper.Map<RequestNormalPolicy, LocalNormalPolicy>(policy, localPolicy);
            unitOfWorkRepository.PersistUpdateOf(localPolicy);
            unitOfWork.Commit();
            try
            {
                Logger.WriteLog(LogType.INFO, oldStr + "__" + localPolicy.ToString());
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "写入错误", e);
            }
        }


        public PagedList<ResponsePolicy> FindAllPolicyByPager(string Code, string Id, int page, int rows)
        {
            var query = this.localPolicyRepository.FindAll();

            if (!string.IsNullOrEmpty(Code) && !string.IsNullOrEmpty(Code.Trim()))
                query = query.Where(p => p.Code == Code.Trim());
            if (!string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Id.Trim()))
            {
                Guid gid = default(Guid);
                Guid.TryParse(Id.Trim(), out gid);
                query = query.Where(p => p.ID == gid);
            }
            var localList = query.OrderByDescending(p => p.CreateDate).Skip((page - 1) * rows).Take(rows).ToList();
            var speciaList = localList.OfType<SpecialPolicy>().ToList();
            var rowList = Mapper.Map<List<LocalPolicy>, List<ResponsePolicy>>(localList);
            rowList.ForEach(x=>
            {
                for(int i = 0;i<speciaList.Count();i++)
                {
                    if (speciaList[i].ID == x.ID)
                    {
                        x.PolicySpecialType = (BPiaoBao.Common.Enums.EnumPolicySpecialType)1;
                        break;
                    }
                }
            });
            return new PagedList<ResponsePolicy>()
            {
                Total = query.Count(),
                Rows = rowList
            };
        }
                                                                                                

        public ResponseFullPolicy FindPolicyInfoById(Guid id)
        {
            var localPolicy = this.localPolicyRepository.FindAllNoTracking(p => p.ID == id).FirstOrDefault();
            if (localPolicy == null)
                throw new CustomException(400, "查找的政策不存在!");
            return Mapper.Map<LocalPolicy, ResponseFullPolicy>(localPolicy);
        }

        /// <summary>
        /// 政策对比
        /// </summary>
        /// <param name="carrayCode"></param>
        /// <param name="fromCityCode"></param>
        /// <param name="toCityCode"></param>
        /// <param name="localPolicyType"></param>
        /// <param name="startDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="seat"></param>
        /// <returns></returns>
        public PagedList<ResponseLocalNormalPolicy> PolicyContrast(string carrayCode, string fromCityCode, string toCityCode, string localPolicyType, DateTime? startDate, DateTime? EndDate, string seat, int page, int rows)
        {
            var query = this.localPolicyRepository.FindAllNoTracking(p => p.HangUp != true && p.Review != false).OfType<LocalNormalPolicy>();
            if (!string.IsNullOrEmpty(carrayCode))
                query = query.Where(p => p.CarrayCode == carrayCode);
            if (!string.IsNullOrEmpty(fromCityCode))
                query = query.Where(p => p.FromCityCodes.Contains(fromCityCode));
            if (!string.IsNullOrEmpty(toCityCode))
                query = query.Where(p => p.ToCityCodes.Contains(toCityCode));
            if (!string.IsNullOrEmpty(localPolicyType))
                query = query.Where(p => p.LocalPolicyType == localPolicyType);
            if(startDate.HasValue)
                query = query.Where(p => p.IssueDate.StartTime >= startDate.Value);
            if (EndDate.HasValue)
                query = query.Where(p => p.IssueDate.EndTime >= EndDate.Value);
            if (!string.IsNullOrEmpty(seat))
                query = query.Where(p => p.Seats.Contains(seat));
            var list = query.OrderByDescending(p => p.LocalPoint).Skip((page - 1) * rows).Take(rows).ToList();
            return new PagedList<ResponseLocalNormalPolicy>()
            {
                Total = query.Count(),
                Rows = Mapper.Map<List<LocalNormalPolicy>, List<ResponseLocalNormalPolicy>>(list)
            };
        }

        public void AdjustPoint(Guid policyId, decimal newPoint)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var Info = this.localPolicyRepository.FindAll(p => p.Code == code && p.ID == policyId).FirstOrDefault();
            if (Info == null)
                throw new CustomException(500,"此政策不能修改");
            Logger.WriteLog(LogType.INFO, string.Format("policyId:{0},修改前点数:{1},修改后点数:{2};", policyId.ToString(), Info.LocalPoint, newPoint));
            Info.LocalPoint = newPoint;
            unitOfWorkRepository.PersistUpdateOf(Info);
            this.unitOfWork.Commit();
          
        }

        /// <summary>
        /// 查询特价政策
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public PagedList<ResponseSpecialPolicy> FindSpeciaPolicyByPager(SearchSpecialPolicy search, int page, int rows)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var query = this.localPolicyRepository.FindAllNoTracking(p => p.Code == code).OfType<SpecialPolicy>();
            if (search == null)
                search = new SearchSpecialPolicy();
            if (!string.IsNullOrEmpty(search.LocalPolicyType) )
                query = query.Where(p => p.LocalPolicyType == search.LocalPolicyType);
            if (!string.IsNullOrEmpty(search.FromCityCodes) && !string.IsNullOrEmpty(search.FromCityCodes.Trim()))
                query = query.Where(p => p.FromCityCodes.Contains(search.FromCityCodes.Trim()));
            if (!string.IsNullOrEmpty(search.ToCityCodes) && !string.IsNullOrEmpty(search.ToCityCodes.Trim()))
                query = query.Where(p => p.ToCityCodes.Contains(search.ToCityCodes.Trim()));
            if (search.PassengeDateStart.HasValue)
                query = query.Where(p => p.PassengeDate.StartTime >= search.PassengeDateStart.Value);
            if (search.PassengeDateEnd.HasValue)
            {
                search.PassengeDateEnd = search.PassengeDateEnd.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.PassengeDate.EndTime <= search.PassengeDateEnd.Value);
            }
            if (search.IssueDateStart.HasValue)
                query = query.Where(p => p.IssueDate.StartTime >= search.IssueDateStart.Value);
            if (search.IssueDateEnd.HasValue)
            {
                search.IssueDateEnd = search.IssueDateEnd.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(p => p.IssueDate.EndTime <= search.IssueDateEnd.Value);
            }
            if (search.TravelType.HasValue)
                query = query.Where(p => p.TravelType == search.TravelType.Value);
            if (search.HangUp.HasValue)
                query = query.Where(p => p.HangUp == search.HangUp.Value);
            if (search.IssueTicketWay.HasValue)
                query = query.Where(p => p.IssueTicketWay == search.IssueTicketWay.Value);
            if (!string.IsNullOrEmpty(search.CarrayCode))
                query = query.Where(p => p.CarrayCode.Equals(search.CarrayCode));
            var list = query.OrderByDescending(p => p.CreateDate).Skip((page - 1) * rows).Take(rows).ToList();
            return new PagedList<ResponseSpecialPolicy>()
            {
                Total = query.Count(),
                Rows = Mapper.Map<List<SpecialPolicy>, List<ResponseSpecialPolicy>>(list)
            };
        }
        /// <summary>
        /// 修改特价政策
        /// </summary>
        /// <param name="policy"></param>
        public void UpdateSpeaiaPolicy(RequestSpecialPolicy policy)
        {
            var currentUser = AuthManager.GetCurrentUser();
            var localPolicy = this.localPolicyRepository.FindAll(p => p.ID == policy.ID && p.Code == currentUser.Code).FirstOrDefault();

            if (localPolicy == null)
                throw new CustomException(500, "政策不存在或被删除");
            string oldStr = localPolicy.ToString();

            Mapper.Map<RequestSpecialPolicy, LocalPolicy>(policy, localPolicy);
            unitOfWorkRepository.PersistUpdateOf(localPolicy);
            unitOfWork.Commit();
            try
            {
                Logger.WriteLog(LogType.INFO, oldStr + "__" + localPolicy.ToString());
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "写入错误", e);
            }
        }
        /// <summary>
        /// 新增特价政策
        /// </summary>
        /// <param name="policy"></param>
        public void AddSpeaiaPolicy(RequestSpecialPolicy policy)
        {
            var currentUser = AuthManager.GetCurrentUser();
            var builder = AggregationFactory.CreateBuiler<LocalPolicyBuilder>();
            SpecialPolicy localPolicy = builder.CreateSpecialPolicy(Mapper.Map<RequestSpecialPolicy, SpecialPolicy>(policy));
            localPolicy.CreateMan = currentUser.OperatorName;
            localPolicy.RoleType = currentUser.Type;
            localPolicy.Code = currentUser.Code;
            if (currentUser.Type == "Supplier")
            {
                localPolicy.CarrierCode = currentUser.CarrierCode;
                var bm = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.CarrierCode).OfType<Carrier>().FirstOrDefault();
                if (bm != null)
                {
                    localPolicy.CarrierWeek = bm.RestWork.WeekDay;
                    localPolicy.Carrier_AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.WorkOnLineTime,
                        EndTime = bm.NormalWork.WorkUnLineTime
                    };
                    localPolicy.Carrier_WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.WorkOnLineTime,
                        EndTime = bm.RestWork.WorkUnLineTime
                    };
                }
                var supbm = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.Code).OfType<Supplier>().FirstOrDefault();
                if (supbm != null)
                {

                    localPolicy.SupplierWeek = supbm.SupRestWork.WeekDay;
                    localPolicy.AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupNormalWork.ServiceOnLineTime,
                        EndTime = supbm.SupNormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupNormalWork.ServiceOnLineTime,
                        EndTime = supbm.SupNormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupNormalWork.WorkOnLineTime,
                        EndTime = supbm.SupNormalWork.WorkUnLineTime
                    };
                    localPolicy.WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupRestWork.ServiceOnLineTime,
                        EndTime = supbm.SupRestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupRestWork.ServiceOnLineTime,
                        EndTime = supbm.SupRestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.WeeKWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = supbm.SupRestWork.WorkOnLineTime,
                        EndTime = supbm.SupRestWork.WorkUnLineTime
                    };
                }
            }
            else
            {
                localPolicy.CarrierCode = currentUser.Code;
                var bm = this.businessmanRepository.FindAllNoTracking(p => p.Code == currentUser.Code).OfType<Carrier>().FirstOrDefault();
                if (bm != null)
                {
                    localPolicy.CarrierWeek = bm.RestWork.WeekDay;
                    localPolicy.Carrier_AnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_ReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.ServiceOnLineTime,
                        EndTime = bm.NormalWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.NormalWork.WorkOnLineTime,
                        EndTime = bm.NormalWork.WorkUnLineTime
                    };
                    localPolicy.Carrier_WeekAnnulTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekReturnTicketTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.ServiceOnLineTime,
                        EndTime = bm.RestWork.ServiceUnLineTime
                    }; ;
                    localPolicy.Carrier_WeekWorkTime = new BPiaoBao.DomesticTicket.Platform.Plugin.StartAndEndTime
                    {
                        StartTime = bm.RestWork.WorkOnLineTime,
                        EndTime = bm.RestWork.WorkUnLineTime
                    };
                }
            }
            unitOfWorkRepository.PersistCreationOf(localPolicy);
            unitOfWork.Commit();
        }
    }
}
