using BPiaoBao.AppServices.ConsoContracts.DomesticTicket;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class DeductionGroupService : IConsoDeductionGroupService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IDeductionRepository deductionRepository;
        public DeductionGroupService(IDeductionRepository deductionRepository)
        {
            this.deductionRepository = deductionRepository;
        }

        public void AddDeductionGroup(ConsoContracts.DomesticTicket.DataObjects.RequestDeduction deduction)
        {
            
            if (deduction == null)
                throw new CustomException(500, "数据信息获取失败!");
            var model = AutoMapper.Mapper.Map<BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects.RequestDeduction, DeductionGroup>(deduction);
            model.CarrierCode = AuthManager.GetCurrentUser().Code;
            model.CheckRule();
            model.DeductionRules.ToList().ForEach(p => p.CheckDetail());
            unitOfWorkRepository.PersistCreationOf(model);
            unitOfWork.Commit();
        }

        public void EditDeductionGroup(ConsoContracts.DomesticTicket.DataObjects.RequestDeduction deduction)
        {
            if (deduction == null)
                throw new CustomException(500, "数据信息获取失败!");
            var dataModel = this.deductionRepository.FindAll(p => p.ID == deduction.ID).FirstOrDefault();
            if (dataModel == null)
                throw new CustomException(500, "更新扣点组不存在");
            dataModel.Name = deduction.Name;
            dataModel.Description = deduction.Description;
            dataModel.DeductionRules.Clear();
            if (deduction.DeductionRules != null)
                deduction.DeductionRules.ForEach(p => dataModel.DeductionRules.Add(AutoMapper.Mapper.Map<BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects.RequestDeductionRule, DeductionRule>(p)));
            dataModel.CheckRule();
            dataModel.DeductionRules.ToList().ForEach(p => p.CheckDetail());
            unitOfWorkRepository.PersistUpdateOf(dataModel);
            unitOfWork.Commit();
        }

        public ConsoContracts.SystemSetting.DataObjects.PagedList<ConsoContracts.DomesticTicket.DataObjects.ResponseDeduction> FindDeductionByPager(string name, int currentPage, int pageSize)
        {
            string currentCode = AuthManager.GetCurrentUser().Code;
            var query = this.deductionRepository.FindAll(p => p.CarrierCode == currentCode);
            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name == name);
            return new ConsoContracts.SystemSetting.DataObjects.PagedList<ConsoContracts.DomesticTicket.DataObjects.ResponseDeduction>
            {
                Total = query.Count(),
                Rows = query.Select(p => new BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects.ResponseDeduction
                {
                    ID = p.ID,
                    Name = p.Name,
                    Description = p.Description
                }).OrderByDescending(p => p.ID).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
            };
        }

        public ConsoContracts.DomesticTicket.DataObjects.RequestDeduction GetDeductionByID(int id)
        {
            var model = this.deductionRepository.FindAll(p => p.ID == id).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "不存在或已被删除");
            return AutoMapper.Mapper.Map<DeductionGroup, ConsoContracts.DomesticTicket.DataObjects.RequestDeduction>(model);
        }

        public void DeleteDeduction(int id)
        {
            var model = this.deductionRepository.FindAll(p => p.ID == id).FirstOrDefault();
            if (model == null)
                throw new CustomException(400, "不存在或已被删除");
            model.DeductionRules.Clear();
            unitOfWorkRepository.PersistDeletionOf(model);
            unitOfWork.Commit();
        }
    }
}
