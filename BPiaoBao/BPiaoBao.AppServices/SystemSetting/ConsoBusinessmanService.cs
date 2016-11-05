using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using System.IO;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class BusinessmanService : IConsoBusinessmanService
    {

        public void OpenBuyer(RequestBuyer buyer)
        {
            if (buyer == null)
                throw new CustomException(500, "输入信息丢失");
            var isexist = this.businessmanRepository.FindAll(p => p.Code == buyer.Code).Count() > 0;
            if (isexist)
                throw new CustomException(500, string.Format("{0}已存在", buyer.Code));
            var curentUser = AuthManager.GetCurrentUser();
            var businessmanBuilder = AggregationFactory.CreateBuiler<BusinessmanBuilder>();
            buyer.CarrierCode = curentUser.Code;
            Buyer by = businessmanBuilder.CreateBuyer(AutoMapper.Mapper.Map<RequestBuyer, Buyer>(buyer));
            by.CheckRule();
            var cashbagModel = accountClientProxy.AddCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
            {
                ClientAccount = buyer.Code,
                Contact = buyer.ContactWay.Contact,
                CpyName = buyer.Name,
                Moblie = buyer.ContactWay.Tel,
                Province = buyer.ContactWay.Province,
                City = buyer.ContactWay.City,
                Address = buyer.ContactWay.Address
            });
            try
            {
                by.CashbagCode = cashbagModel.PayAccount;
                by.CashbagKey = cashbagModel.Token;
                unitOfWorkRepository.PersistCreationOf(by);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "添加商户发生异常", e);
                accountClientProxy.DeleteCashBagBusinessman(curentUser.CashbagCode, cashbagModel.PayAccount, curentUser.CashbagKey);
                throw new CustomException(500, "添加商户发生异常");
            }

        }

        public ResponseDetailBuyer GetBuyerInfo(string code)
        {
            var model = this.businessmanRepository.FindAllNoTracking(p => p.Code == code).OfType<Buyer>().FirstOrDefault();
            return AutoMapper.Mapper.Map<Buyer, ResponseDetailBuyer>(model);
        }


        public void EditBuyer(RequestBuyer buyer)
        {
            if (buyer == null)
                throw new CustomException(500, "输入信息丢失");
            var currentBuyer = this.businessmanRepository.FindAll(p => p.Code == buyer.Code).OfType<Buyer>().FirstOrDefault();
            if (currentBuyer == null)
                throw new CustomException(404, "修改商户不存在!");
            currentBuyer.ContactWay = AutoMapper.Mapper.Map<ContactWayDataObject, ContactWay>(buyer.ContactWay);
            currentBuyer.ContactName = buyer.ContactName;
            currentBuyer.Phone = buyer.Phone;
            currentBuyer.Plane = buyer.Plane;
            var curentUser = AuthManager.GetCurrentUser();
            accountClientProxy.UpdateCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
            {
                ClientAccount = buyer.Code,
                CpyName=buyer.Name,
                Contact = buyer.ContactWay.Contact,
                Moblie = buyer.ContactWay.Tel,
                Address = buyer.ContactWay.Address,
                PayAccount = currentBuyer.CashbagCode,
                Province = buyer.ContactWay.Province,
                City = buyer.ContactWay.City
            });
            unitOfWorkRepository.PersistUpdateOf(currentBuyer);
            unitOfWork.Commit();
        }


        public PagedList<ResponseBuyer> GetBusinessmanBuyerByCode(string code, string businessmanName, DateTime? startTime, DateTime? endTime, int pageIndex = 1, int pageSize = 15)
        {
            if (endTime.HasValue)
                endTime = endTime.Value.AddDays(1);
            var currentCode = AuthManager.GetCurrentUser().Code;
            var query = this.businessmanRepository.FindAll(p => p is Buyer).Select(p => p as Buyer).Where(p => p.CarrierCode == currentCode);
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
                query = query.Where(p => p.Code == code.Trim());
            if (!string.IsNullOrEmpty(businessmanName) && !string.IsNullOrEmpty(businessmanName.Trim()))
                query = query.Where(p => p.Name.Contains(businessmanName.Trim()));
            if (startTime.HasValue)
                query = query.Where(p => p.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateTime <= endTime.Value);
            List<ResponseBuyer> buyerList = new List<ResponseBuyer>();
            return new PagedList<ResponseBuyer>
            {
                Total = query.Count(),
                Rows = query.OrderBy(p => p.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).Project().To<ResponseBuyer>().ToList()
            };
        }




        public RequestBuyer GetEditBuyerInfo(string code)
        {
            var model = this.businessmanRepository.FindAll(p => p.Code == code && (p is Buyer)).Select(p => p as Buyer).FirstOrDefault();
            return AutoMapper.Mapper.Map<Buyer, RequestBuyer>(model);
        }


        public List<ResponseOperator> GetOperators(string account,string realName,string phone,string status,int? roleId)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var cmodel = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            if (cmodel == null)
                throw new CustomException(404, "获取信息失败，请重新登录");
            var list = cmodel.Operators.AsQueryable();
            if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(account.Trim()))
                list = list.Where(p => p.Account.Contains(account.Trim()));
            if (!string.IsNullOrEmpty(realName) && !string.IsNullOrEmpty(realName.Trim()))
                list = list.Where(p => p.Realname.Contains(realName.Trim()));
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(phone.Trim()))
                list = list.Where(p => p.Phone.Contains(phone.Trim()));
            if (!string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(status.Trim()))
            {
                var state = EnumCommonHelper.GetInstance<EnumOperatorState>(status);
                list = list.Where(p => p.OperatorState == state);
            }
            if (roleId.HasValue)
            {
                list = list.Where(p => p.RoleID == roleId.Value);
            }
              
            
            return AutoMapper.Mapper.Map<List<Operator>, List<ResponseOperator>>(list.ToList());
        }

        public void AddConsoOperator(RequestOperator rp)
        {
            if (rp == null)
                throw new CustomException(500, "输入信息不完整");
            string currentCode = AuthManager.GetCurrentUser().Code;
            var cmodel = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            if (cmodel == null)
                throw new CustomException(404, "获取信息失败，请重新登录");
            var op = cmodel.Operators.Where(p => p.Account == rp.Account).FirstOrDefault();
            if (op != null)
                throw new CustomException(500, "员工帐号重复，不能添加");
            var model = AutoMapper.Mapper.Map<RequestOperator, Operator>(rp);
            model.Password = "123456".Md5();
            model.CreateDate = DateTime.Now;
            cmodel.Operators.Add(model);
            unitOfWorkRepository.PersistUpdateOf(cmodel);
            unitOfWork.Commit();
        }

        /// <summary>
        /// 重置员工密码
        /// </summary>
        public void ResetPassWord(int Id)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "获取登录信息失败，请重新登录");
            var op = model.Operators.AsQueryable().Where(p => p.Id == Id).FirstOrDefault();
            op.Password = "123456".Md5();
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public List<ResponseRole> GetRole(string roleName)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var query = roleRepository.FindAll(p => p.Code == currentCode);
            if (!string.IsNullOrEmpty(roleName))
                query = query.Where(p => p.RoleName == roleName);
            return query.Select(p => new ResponseRole
            {
                ID = p.ID,
                RoleName = p.RoleName,
                Description = p.Description,
                CreateTime = p.CreateTime
            }).ToList();
        }

        public void AddRole(RequestRole role)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var model = this.roleRepository.FindAll(p => p.Code == currentCode && p.RoleName == role.RoleName).FirstOrDefault();
            if (model != null)
                throw new CustomException(500, "当前角色已经存在");
            var currentRole = AutoMapper.Mapper.Map<RequestRole, Role>(role);
            currentRole.Code = currentCode;
            currentRole.CreateTime = DateTime.Now;
            unitOfWorkRepository.PersistCreationOf(currentRole);
            unitOfWork.Commit();
        }




        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="Id"></param>
        public bool DeleteRole(int Id)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var cmodel = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            var operatorModel = cmodel.Operators.Where(p => p.RoleID.Equals(Id)).FirstOrDefault();
            if (operatorModel != null)
                return false;//若此角色已被分配则无法删除
            var role = this.roleRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            unitOfWorkRepository.PersistDeletionOf(role);
            unitOfWork.Commit();
            return true;
        }


        public RequestRole GetRoleInfo(int id)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var model = this.roleRepository.FindAll(p => p.Code == code && p.ID == id).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "角色不存在");
            return AutoMapper.Mapper.Map<Role, RequestRole>(model);
        }


        public void EditRole(RequestRole role)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var model = this.roleRepository.FindAll(p => p.Code == code && p.ID == role.ID).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "角色不存在");
            model.Description = role.Description;
            model.AuthNodes = role.AuthNodes;
            model.RoleName = role.RoleName;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public RequestOperator GetOperatorInfo(int id)
        {
            string code = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "获取登录信息失败，请重新登录");
            var op = model.Operators.AsQueryable().Where(p => p.Id == id).FirstOrDefault();
            return AutoMapper.Mapper.Map<Operator, RequestOperator>(op);
        }


        public void EditConsoOperator(RequestOperator rp)
        {
            if (rp == null)
                throw new CustomException(500, "输入信息不完整");
            string currentCode = AuthManager.GetCurrentUser().Code;
            var cmodel = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            if (cmodel == null)
                throw new CustomException(404, "获取信息失败，请重新登录");
            var opModel = cmodel.Operators.AsQueryable().Where(p => p.Id == rp.Id).FirstOrDefault();
            if (opModel == null)
                throw new CustomException(404, "员工不存在，或被删除");
            opModel.Realname = rp.Realname;
            opModel.Phone = rp.Phone;
            opModel.RoleID = rp.RoleID;
            unitOfWorkRepository.PersistUpdateOf(cmodel);
            unitOfWork.Commit();
        }


        public void SetEnableStatus(int id, int status)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取信息失败，请重新登录");
            var currentOperator = model.Operators.AsQueryable().Where(p => p.Id == id).FirstOrDefault();
            if (currentOperator == null)
                throw new CustomException(500, "员工不存在，或者已被删除!");
            currentOperator.OperatorState = (EnumOperatorState)status;
            this.unitOfWorkRepository.PersistUpdateOf(model);
            this.unitOfWork.Commit();
        }


        public void BusinessmanSetEnable(string code, bool isenable)
        {
            var model = this.businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "查找商户不存在");
            model.IsEnable = isenable;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public CarrierDataObject GetCarrierDetail()
        {
            string code = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == code && (p is Carrier || p is Supplier)).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取登录信息失败，请重新登录");
            if (model is Carrier)
                return AutoMapper.Mapper.Map<Carrier, CarrierDataObject>((model as Carrier));
            return AutoMapper.Mapper.Map<Supplier, CarrierDataObject>((model as Supplier));
        }
        public List<CarrierDataObject> GetAllCarrier()
        {
            var list = new List<CarrierDataObject>();
            List<Carrier> model = null;
            var code = AuthManager.GetCurrentUser() == null ? null : AuthManager.GetCurrentUser().Code;
            model = code == null ? this.businessmanRepository.FindAll(p => p is Carrier).Select(p => p as Carrier).ToList() : this.businessmanRepository.FindAll(p => p.Code == code && p is Carrier).Select(p => p as Carrier).ToList();
            list = AutoMapper.Mapper.Map<List<Carrier>, List<CarrierDataObject>>(model);
            return list;
        }


        public void SubmitCarrier(CarrierDataObject carrier)
        {
            if (carrier == null)
                throw new CustomException(500, "获取信息失败");
            string code = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == code && (p is Carrier || p is Supplier)).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取登录信息失败，请重新登录");
            model.ContactWay = AutoMapper.Mapper.Map<ContactWayDataObject, ContactWay>(carrier.ContactWay);

            var localPolicyRepository = ObjectFactory.GetInstance<ILocalPolicyRepository>();

            if (model is Carrier)
            {
                (model as Carrier).NormalWork = AutoMapper.Mapper.Map<WorkBusinessmanDataObject, WorkBusinessman>(carrier.NormalWork);
                (model as Carrier).RestWork = AutoMapper.Mapper.Map<WorkBusinessmanDataObject, WorkBusinessman>(carrier.RestWork);

                localPolicyRepository.FindAll(p => p.Code == code || p.CarrierCode == code).ToList().ForEach(p =>
                {
                    p.CarrierWeek = carrier.RestWork.WeekDay;
                    p.Carrier_AnnulTicketTime.StartTime = carrier.NormalWork.ServiceOnLineTime;
                    p.Carrier_AnnulTicketTime.EndTime = carrier.NormalWork.ServiceUnLineTime;
                    p.Carrier_ReturnTicketTime.StartTime = carrier.NormalWork.ServiceOnLineTime;
                    p.Carrier_ReturnTicketTime.EndTime = carrier.NormalWork.ServiceUnLineTime;
                    p.Carrier_WorkTime.StartTime = carrier.NormalWork.WorkOnLineTime;
                    p.Carrier_WorkTime.EndTime = carrier.NormalWork.WorkUnLineTime;
                    p.Carrier_WeekAnnulTicketTime.StartTime = carrier.RestWork.ServiceOnLineTime;
                    p.Carrier_WeekAnnulTicketTime.EndTime = carrier.RestWork.ServiceUnLineTime;
                    p.Carrier_WeekReturnTicketTime.StartTime = carrier.RestWork.ServiceOnLineTime;
                    p.Carrier_WeekReturnTicketTime.EndTime = carrier.RestWork.ServiceUnLineTime;
                    p.Carrier_WeekWorkTime.StartTime = carrier.RestWork.WorkOnLineTime;
                    p.Carrier_WeekWorkTime.EndTime = carrier.RestWork.WorkUnLineTime;
                    _ticketUnitOfWorkRepository.PersistUpdateOf(p);
                });
                (model as Carrier).Pids.Clear();
                if (carrier.Pids != null && carrier.Pids.Count > 0)
                {
                    AutoMapper.Mapper.Map<List<PIDDataObject>, List<PID>>(carrier.Pids).ForEach(p =>
                    {
                        (model as Carrier).Pids.Add(p);
                    });
                }
                (model as Carrier).CarrierSettings.Clear();
                if (carrier.CarrierSettings != null && carrier.CarrierSettings.Count > 0)
                {
                    AutoMapper.Mapper.Map<List<CarrierSettingDataObject>, List<CarrierSetting>>(carrier.CarrierSettings).ForEach(p =>
                    {
                        (model as Carrier).CarrierSettings.Add(p);
                    });
                }

                (model as Carrier).Label = carrier.Label;
            }
            else
            {
                (model as Supplier).SupNormalWork = AutoMapper.Mapper.Map<WorkBusinessmanDataObject, WorkBusinessman>(carrier.NormalWork);
                (model as Supplier).SupRestWork = AutoMapper.Mapper.Map<WorkBusinessmanDataObject, WorkBusinessman>(carrier.RestWork);

                localPolicyRepository.FindAll(p => p.Code == code).ToList().ForEach(p =>
                {
                    p.SupplierWeek = carrier.RestWork.WeekDay;
                    p.AnnulTicketTime.StartTime = carrier.NormalWork.ServiceOnLineTime;
                    p.AnnulTicketTime.EndTime = carrier.NormalWork.ServiceUnLineTime;
                    p.ReturnTicketTime.StartTime = carrier.NormalWork.ServiceOnLineTime;
                    p.ReturnTicketTime.EndTime = carrier.NormalWork.ServiceUnLineTime;
                    p.WorkTime.StartTime = carrier.NormalWork.WorkOnLineTime;
                    p.WorkTime.EndTime = carrier.NormalWork.WorkUnLineTime;
                    p.WeekAnnulTicketTime.StartTime = carrier.RestWork.ServiceOnLineTime;
                    p.WeekAnnulTicketTime.EndTime = carrier.RestWork.ServiceUnLineTime;
                    p.WeekReturnTicketTime.StartTime = carrier.RestWork.ServiceOnLineTime;
                    p.WeekReturnTicketTime.EndTime = carrier.RestWork.ServiceUnLineTime;
                    p.WeeKWorkTime.StartTime = carrier.RestWork.WorkOnLineTime;
                    p.WeeKWorkTime.EndTime = carrier.RestWork.WorkUnLineTime;
                    _ticketUnitOfWorkRepository.PersistUpdateOf(p);
                });
                (model as Supplier).SupPids.Clear();
                if (carrier.Pids != null && carrier.Pids.Count > 0)
                {
                    AutoMapper.Mapper.Map<List<PIDDataObject>, List<PID>>(carrier.Pids).ForEach(p =>
                    {
                        (model as Supplier).SupPids.Add(p);
                    });
                }
                (model as Supplier).CarrierSettings.Clear();
                if (carrier.CarrierSettings != null && carrier.CarrierSettings.Count > 0)
                {
                    AutoMapper.Mapper.Map<List<CarrierSettingDataObject>, List<CarrierSetting>>(carrier.CarrierSettings).ForEach(p =>
                    {
                        (model as Supplier).CarrierSettings.Add(p);
                    });
                }
            }
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            _ticketUnitOfWork.Commit();
        }


        public void TransferAccount(RechargeDataObject dataObject)
        {
        }


        public void SetBuyerLabel(string code, string label)
        {
            var model = this.businessmanRepository.FindAll(p => p.Code == code && p is Buyer).Select(p => p as Buyer).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, string.Format("{0}采购商不存在", code));
            model.Label = label;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public List<SampleListBuyer> GetDistributionBuyer()
        {
            string code = AuthManager.GetCurrentUser().Code;
            var list = this.businessmanRepository.FindAll(p => p is Buyer).Select(p => p as Buyer).Where(p => p.CarrierCode == code).Select(p => new
             {
                 Code = p.Code,
                 Name = p.Name,
                 DeductionGroupID = p.DeductionGroupID,
                 CreateTime = p.CreateTime,
                 Label = p.Label
             }).ToList();
            var darray = list.Select(x => x.DeductionGroupID);
            BPiaoBao.DomesticTicket.Domain.Models.Deduction.IDeductionRepository _deductionRepository = ObjectFactory.GetInstance<BPiaoBao.DomesticTicket.Domain.Models.Deduction.IDeductionRepository>();
            var dList = _deductionRepository.FindAll(p => darray.Contains(p.ID)).Select(p => new
            {
                ID = p.ID,
                Name = p.Name
            }).ToList();
            List<SampleListBuyer> listBuyer = new List<SampleListBuyer>();
            list.ForEach(p =>
            {
                SampleListBuyer sb = new SampleListBuyer
                {
                    Code = p.Code,
                    Name = p.Name,
                    DeductionGroupID = p.DeductionGroupID,
                    CreateTime = p.CreateTime.ToString("yyyy-MM-dd"),
                    Label = p.Label
                };
                foreach (var item in dList)
                {
                    if (p.DeductionGroupID == item.ID)
                    {
                        sb.DeductionGroupName = item.Name;
                        break;
                    }
                }
                listBuyer.Add(sb);
            });
            return listBuyer;
        }

        public void DistributionBuyer(List<SampleBuyer> list)
        {
            if (list == null)
                return;
            string code = AuthManager.GetCurrentUser().Code;
            var array = list.Select(x => x.Code).ToArray();
            var result = this.businessmanRepository.FindAll(p => p is Buyer).Select(p => p as Buyer).Where(p => p.CarrierCode == code && array.Contains(p.Code)).ToList();
            foreach (var item in result)
            {
                foreach (var sb in list)
                {
                    if (sb.Code == item.Code && sb.DeductionGroupID != item.DeductionGroupID)
                    {
                        item.DeductionGroupID = sb.DeductionGroupID;
                        this.unitOfWorkRepository.PersistUpdateOf(item);
                        break;
                    }
                }
            }
            this.unitOfWork.Commit();
        }

        public void ModifyPassword(string newPwd, string oldPwd)
        {
            if (string.IsNullOrEmpty(newPwd))
                throw new CustomException(400, "新密码不能为空");
            var currentUser = AuthManager.GetCurrentUser();
            var model = this.businessmanRepository.FindAll(p => p.Code == currentUser.Code).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取信息失败，请重新登录");
            var op = model.Operators.Where(p => p.Account.ToUpper() == currentUser.OperatorAccount.ToUpper()).FirstOrDefault();
            if (op == null)
                throw new CustomException(400, "获取信息失败，请重新登录");
            if (op.Password != oldPwd.Md5())
                throw new CustomException(500, "原密码输入错误");
            op.Password = newPwd.Md5();
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            Logger.WriteLog(LogType.DEBUG, string.Format("{0}修改卖票宝[{1}]密码", AuthManager.GetCurrentUser().Code, AuthManager.GetCurrentUser().Code));
        }


        public string GetLabel()
        {
            string carrierCode = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAll(p => p.Code == carrierCode && p is Carrier).Select(p => p as Carrier).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取信息失败，请重新登录");
            return model.Label;
        }

        public void SetLabel(SetLabel setLabel)
        {
            if (setLabel == null)
                throw new CustomException(400, "设置信息不能为空");
            var model = this.businessmanRepository.FindAll(p => p.Code == setLabel.Code && p is Buyer).Select(p => p as Buyer).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, string.Format("{0}商户号不存在", setLabel.Code));
            model.Label = setLabel.BuyerLabel;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }

        public CurrentUserInfo GetCurrentUser()
        {
            var currentUserInfo = AuthManager.GetCurrentUser();
            CurrentUserInfo currentUser = new CurrentUserInfo();
            currentUser.BusinessmanName = currentUserInfo.BusinessmanName;
            currentUser.OperatorName = currentUserInfo.OperatorName;
            currentUser.BusinessmanCode = currentUserInfo.Code;
            currentUser.OperatorAccount = currentUserInfo.OperatorAccount;
            currentUser.Type = currentUserInfo.Type;
            return currentUser;
        }


        public List<string> GetSelectedDistribution(int deductionId)
        {
            var list = this.businessmanRepository.FindAll(p => p is Buyer).Select(p => p as Buyer).Where(p => p.DeductionGroupID == deductionId).Select(x => x.Name).ToList();
            return list;
        }

        public void AddSupplier(RequestSupplier requestSupplier)
        {
            if (requestSupplier == null)
                throw new CustomException(500, "输入信息丢失");
            var isexist = this.businessmanRepository.FindAll(p => p.Code == requestSupplier.Code).Count() > 0;
            if (isexist)
                throw new CustomException(500, string.Format("{0}已存在", requestSupplier.Code));
            var curentUser = AuthManager.GetCurrentUser();
            var businessmanBuilder = AggregationFactory.CreateBuiler<BusinessmanBuilder>();
            Supplier supplier = businessmanBuilder.CreateSupplier(Mapper.Map<RequestSupplier, Supplier>(requestSupplier));
            supplier.CarrierCode = curentUser.Code;
            supplier.CheckRule();
            var cashbagModel = accountClientProxy.AddCompany(curentUser.CashbagCode, curentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
            {
                ClientAccount = requestSupplier.Code,
                Contact = requestSupplier.ContactWay.Contact,
                CpyName = requestSupplier.Name,
                Moblie = requestSupplier.ContactWay.Tel,
                Province = requestSupplier.ContactWay.Province,
                City = requestSupplier.ContactWay.City,
                Address = requestSupplier.ContactWay.Address
            });
            try
            {
                supplier.CashbagCode = cashbagModel.PayAccount;
                supplier.CashbagKey = cashbagModel.Token;
                supplier.SupLocalPolicySwitch = false;
                supplier.SupRemotePolicySwitch = false;
                unitOfWorkRepository.PersistCreationOf(supplier);
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "添加商户发生异常", e);
                accountClientProxy.DeleteCashBagBusinessman(curentUser.CashbagCode, cashbagModel.PayAccount, curentUser.CashbagKey);
                throw new CustomException(500, "添加商户发生异常");
            }
        }

        public void UpdateSupplier(RequestSupplier requestSupplier)
        {
            if (requestSupplier == null)
                throw new CustomException(500, "输入信息丢失");
            var currentUser = AuthManager.GetCurrentUser();
            var model = this.businessmanRepository.FindAll(p => p.Code == requestSupplier.Code).OfType<Supplier>().Where(p => p.CarrierCode == currentUser.Code).FirstOrDefault();
            Mapper.Map<RequestSupplier, Supplier>(requestSupplier, model);
            accountClientProxy.UpdateCompany(currentUser.CashbagCode, currentUser.CashbagKey, new BPiaoBao.Cashbag.Domain.Models.CashCompanyInfo
            {
                ClientAccount = model.Code,
                Contact = requestSupplier.ContactWay.Contact,
                CpyName=requestSupplier.Name,
                Moblie = requestSupplier.ContactWay.Tel,
                Address = requestSupplier.ContactWay.Address,
                Province = requestSupplier.ContactWay.Province,
                City = requestSupplier.ContactWay.City,
                PayAccount = model.CashbagCode
            });
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }

        public List<ResponseSupplier> FindSupplier(string code, string businessmanName, DateTime? startTime, DateTime? endTime)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var query = this.businessmanRepository.FindAllNoTracking().OfType<Supplier>().Where(p => p.CarrierCode == currentCode);
            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(p => p.Code == code.Trim());
            if (!string.IsNullOrWhiteSpace(businessmanName))
                query = query.Where(p => p.Name.Contains(businessmanName.Trim()));
            if (startTime.HasValue)
                query = query.Where(p => p.CreateTime >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateTime <= endTime.Value);
            return query.OrderByDescending(p => p.CreateTime).Project().To<ResponseSupplier>().ToList();
        }

        public SupplierDataObject FindSupplierByCode(string code)
        {
            var model = this.businessmanRepository.FindAllNoTracking().OfType<Supplier>().Where(p => p.Code == code).Project().To<SupplierDataObject>().FirstOrDefault();
            if (model == null)
                throw new CustomException(400, string.Format("{0}不存在或被删除!", code));
            return model;
        }


        public RequestSupplier EditFind(string code)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var model = this.businessmanRepository.FindAllNoTracking(p => p.Code == code).OfType<Supplier>().Where(p => p.CarrierCode == currentCode).Project().To<RequestSupplier>().FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "获取供应商信息失败!");
            if (model.ContactWay == null)
                model.ContactWay = new ContactWayDataObject();
            if (model.CarrierSettings == null)
                model.CarrierSettings = new List<CarrierSettingDataObject>();
            if (model.Pids == null)
                model.Pids = new List<PIDDataObject>();
            return model;
        }
        public void ResetBuyerAdminPwd(string code)
        {
            var buyer = this.businessmanRepository.FindAll(p => p.Code == code).OfType<Buyer>().FirstOrDefault();
            if (buyer == null)
                throw new CustomException(404, "查找所属商户不存在");
            var operatorModel = buyer.Operators.Where(p => string.Equals(p.Account, "admin", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (operatorModel == null)
                throw new CustomException(404, "未查询到管理员帐号");
            operatorModel.Password = "123456".Md5();
            unitOfWorkRepository.PersistUpdateOf(buyer);
            unitOfWork.Commit();
        }


        public void AutoIssueTicketSave(Common.AutoIssueTicketViewModel vm)
        {
            if (vm == null)
                throw new CustomException(500, "设置信息失败!");
            vm.Code = AuthManager.GetCurrentUser().Code;
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "AutoIssueTicket\\" + vm.Code + ".xml";
            BPiaoBao.Common.XmlHelper.XmlSerializeToFile(vm, path, Encoding.Default);
        }

        public Common.AutoIssueTicketViewModel GetAutoIssueTicket()
        {
            var currentCode = AuthManager.GetCurrentUser().Code;
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "AutoIssueTicket\\" + currentCode + ".xml";
            if (File.Exists(path))
                return Common.XmlHelper.XmlDeserializeFromFile<BPiaoBao.Common.AutoIssueTicketViewModel>(path, Encoding.Default);
            BPiaoBao.Common.AutoIssueTicketViewModel vm = new Common.AutoIssueTicketViewModel();
            BPiaoBao.Common.ExtHelper.GetCarryInfos().ForEach(p => vm.IssueTickets.Add(new BPiaoBao.Common.IssueTicketModel { CarrayCode = p.AirCode, CarrayName = p.Carry.AirShortName, Account = string.Empty, Pwd = string.Empty, ContactName = string.Empty, Phone = string.Empty }));
            return vm;
        }
        public void SetDefaultPolicy(List<DefaultPolicyDataObject> defaultPolicys)
        {
            var code = AuthManager.GetCurrentUser().Code;
            var carrier = this.businessmanRepository.FindAll(p => p.Code == code).OfType<Carrier>().FirstOrDefault();
            if (carrier == null)
                throw new CustomException(400, "获取商户信息失败!");
            carrier.DefaultPolicys.Clear();
            if (defaultPolicys != null && defaultPolicys.Count > 0)
                Mapper.Map<List<DefaultPolicyDataObject>, List<DefaultPolicy>>(defaultPolicys).Each(p =>
                {
                    carrier.DefaultPolicys.Add(p);
                });
            this.unitOfWorkRepository.PersistUpdateOf(carrier);
            this.unitOfWork.Commit();
        }

        public List<DefaultPolicyDataObject> GetDefaultPolicy()
        {
            var code = AuthManager.GetCurrentUser().Code;
            var carrier = this.businessmanRepository.FindAllNoTracking(p => p.Code == code).OfType<Carrier>().FirstOrDefault();
            if (carrier == null)
                return new List<DefaultPolicyDataObject>();
            return Mapper.Map<List<DefaultPolicy>, List<DefaultPolicyDataObject>>(carrier.DefaultPolicys.ToList());
        }


        public BusinessmanOperator GetBusinessmanOperator(string code, string name)
        {
            BusinessmanOperator businessmanOperator = new BusinessmanOperator();
            var bm = this.businessmanRepository.FindAllNoTracking(p => p.Code == code).FirstOrDefault();
            if (bm != null)
            {
                businessmanOperator.Code = bm.Code;
                businessmanOperator.Name = bm.Name;
                businessmanOperator.ContactWay = new ContactWayDataObject
                {
                    Contact = bm.ContactWay.Contact,
                    Phone = bm.ContactWay.Tel
                };
                businessmanOperator.Operator = bm.Operators.Where(p => p.Realname == name).Select(x => new OperatorDataObject
                {
                    Realname = x.Realname,
                    Phone = x.Phone,
                    Tel = x.Tel
                }).FirstOrDefault();
            }
            return businessmanOperator;
        }

        /// <summary>
        /// 返回权限点数
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        public Tuple<bool, string> GetMentList(string Account)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var cmodel = this.businessmanRepository.FindAll(p => p.Code == currentCode).FirstOrDefault();
            var operatorModel = cmodel.Operators.Where(p => p.Account.Equals(Account)).FirstOrDefault();
            if (operatorModel == null)
                throw new CustomException(400, "当前账号不存在");
            if (operatorModel.Role != null)
                return Tuple.Create<bool, string>(operatorModel.IsAdmin, operatorModel.Role.AuthNodes);
            else
                return Tuple.Create<bool, string>(operatorModel.IsAdmin, null);

        }

        [ExtOperationInterceptor("卖票宝获取客服中心")]
        public CustomerDto GetConsoCustomerInfo()
        {
            var user = AuthManager.GetCurrentUser();

            var info = _customerInfoDomainService.GetCustomerInfo(false, user.Code);

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
        [ExtOperationInterceptor("卖票宝修改客服中心")]
        public void SetConsoCustomerInfo(CustomerDto customerInfo)
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

            info.CarrierCode = AuthManager.GetCurrentUser().Code;
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


            _customerInfoDomainService.SetCarrierCustomerInfo(info);
        }

    }
}
