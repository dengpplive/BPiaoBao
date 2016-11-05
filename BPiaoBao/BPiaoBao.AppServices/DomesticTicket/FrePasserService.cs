using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.FrePasser;
using JoveZhao.Framework.DDD;
using StructureMap;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public class FrePasserService : BaseService, IFrePasserService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IFrePasserRepository _frePasserRepository;
        private string _code = AuthManager.GetCurrentUser().Code;
        public FrePasserService(IFrePasserRepository frePasserRepository)
        {
            this._frePasserRepository = frePasserRepository;
        }
        public void SaveFrePasser(FrePasserDto passer)
        {
            var builder = AggregationFactory.CreateBuiler<FrePasserBuilder>();
            var frePasser = builder.CreateFrePasser();
            frePasser.PasserType = passer.PasserType;
            frePasser.Name = passer.Name.Trim();
            frePasser.CertificateType = passer.CertificateType;
            frePasser.CertificateNo = passer.CertificateNo;
            frePasser.Mobile = passer.Mobile;
            frePasser.AirCardNo = passer.AirCardNo;
            frePasser.Remark = passer.Remark;
            frePasser.Birth = passer.Birth;
            frePasser.SexType = passer.SexType;
            frePasser.BusinessmanCode = _code;
            unitOfWorkRepository.PersistCreationOf(frePasser);
            unitOfWork.Commit();

        }
        [ExtOperationInterceptor("更新常旅客信息")]
        public void UpdateFrePasser(FrePasserDto passer)
        {

            var frePasser = this._frePasserRepository.FindAll(p => p.Id == passer.Id).FirstOrDefault();
            frePasser.PasserType = passer.PasserType;
            frePasser.Name = passer.Name;
            frePasser.CertificateType = passer.CertificateType;
            frePasser.CertificateNo = passer.CertificateNo;
            frePasser.Mobile = passer.Mobile;
            frePasser.AirCardNo = passer.AirCardNo;
            frePasser.Remark = passer.Remark;
            frePasser.Birth = passer.Birth;
            frePasser.SexType = passer.SexType;
            frePasser.BusinessmanCode = _code;
            unitOfWorkRepository.PersistUpdateOf(frePasser);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("删除常旅客信息")]
        public void DeleteFrePasser(int id)
        {
            var passer = this._frePasserRepository.FindAll(p => p.Id == id).FirstOrDefault();
            unitOfWorkRepository.PersistDeletionOf(passer);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("查询常旅客信息列表")]
        public DataPack<FrePasserDto> QueryFrePassers(QueryFrePasser queryDto, int pageIndex, int pageSize)
        {
            pageIndex = (pageIndex - 1) * pageSize;
            var query = this._frePasserRepository.FindAll(p => p.BusinessmanCode == _code);
            if (queryDto != null)
            {
                if (!string.IsNullOrEmpty(queryDto.Name))
                {
                    query = query.Where(p => p.Name.Trim().Equals(queryDto.Name.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.AirCardNo))
                {
                    query = query.Where(p => p.AirCardNo.Trim().Equals(queryDto.AirCardNo.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.CertificateNo))
                {
                    query = query.Where(p => p.CertificateNo.Trim().Equals(queryDto.CertificateNo.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.CertificateType))
                {
                    query = query.Where(p => p.CertificateType.Trim().Equals(queryDto.CertificateType.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.PasserType))
                {
                    //手机端接口传递参数
                    switch (queryDto.PasserType)
                    {
                        case "1":
                            query = query.Where(p => p.PasserType.Trim().Equals("成人", StringComparison.OrdinalIgnoreCase));
                            break;
                        default:
                            query = query.Where(p => p.PasserType.Trim().Equals(queryDto.PasserType.Trim(), StringComparison.OrdinalIgnoreCase));
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(queryDto.Mobile))
                {
                    query = query.Where(p => p.Mobile.Trim().Equals(queryDto.Mobile.Trim(), StringComparison.OrdinalIgnoreCase));
                }
            }

            query = query.OrderByDescending(p => p.Id);
            //var list = query.Skip(pageIndex).Take(pageSize).ToList();
            List<FrePasser> list = new List<FrePasser>();
            if (pageSize > 0)
                list = query.Skip(pageIndex).Take(pageSize).ToList();
            else
                list = query.ToList();

            var dataPack = new DataPack<FrePasserDto>
            {
                TotalCount = query.Count(),
                List = AutoMapper.Mapper.Map<List<FrePasser>, List<FrePasserDto>>(list)
            };
            return dataPack;
        }
        [ExtOperationInterceptor("判断常旅客信息是否存在")]
        public bool Exists(string name, string certificateNo)
        {
            var query = this._frePasserRepository.FindAll(p => p.BusinessmanCode == _code);
            query = query.Where(p => p.Name == name && p.CertificateNo == certificateNo);
            return query.Any();
        }
        [ExtOperationInterceptor("根据名称查询常旅客")]
        public List<FrePasserDto> Query(string name)
        {
            var query = this._frePasserRepository.FindAll(p => p.BusinessmanCode == _code);
            query = from pass in query where pass.Name.Contains(name) select pass;
            var list = AutoMapper.Mapper.Map<List<FrePasser>, List<FrePasserDto>>(query.ToList());
            return list;
        }

        [ExtOperationInterceptor("导入常旅客信息")]
        public void Import(List<FrePasserDto> list)
        {
            foreach (var m in list)
            {
                DeleteByName(m.Name.Trim(), m.CertificateNo.Trim());
                SaveFrePasser(m);
            }
        }
        [ExtOperationInterceptor("导出常旅客信息")]
        public List<FrePasserDto> Export(QueryFrePasser queryDto)
        {
            var query = this._frePasserRepository.FindAll(p => p.BusinessmanCode == _code);
            if (queryDto != null)
            {
                if (!string.IsNullOrEmpty(queryDto.Name))
                {
                    query = query.Where(p => p.Name.Trim().Equals(queryDto.Name.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.AirCardNo))
                {
                    query = query.Where(p => p.AirCardNo.Trim().Equals(queryDto.AirCardNo.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.CertificateNo))
                {
                    query = query.Where(p => p.CertificateNo.Trim().Equals(queryDto.CertificateNo.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.CertificateType))
                {
                    query = query.Where(p => p.CertificateType.Trim().Equals(queryDto.CertificateType.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.PasserType))
                {
                    query = query.Where(p => p.PasserType.Trim().Equals(queryDto.PasserType.Trim(), StringComparison.OrdinalIgnoreCase));
                }
                if (!string.IsNullOrEmpty(queryDto.Mobile))
                {
                    query = query.Where(p => p.Mobile.Trim().Equals(queryDto.Mobile.Trim(), StringComparison.OrdinalIgnoreCase));
                }
            }

            query = query.OrderByDescending(p => p.Id);

            var list = AutoMapper.Mapper.Map<List<FrePasser>, List<FrePasserDto>>(query.ToList());

            return list;
        }
        private void DeleteByName(string name, string cardid)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(cardid))
            {
                return;
            }
            var model = this._frePasserRepository.FindAll(p => p.BusinessmanCode == _code && p.Name == name && p.CertificateNo == cardid).FirstOrDefault();
            if (model != null)
            {
                DeleteFrePasser(model.Id);
            }
        }
    }
}
