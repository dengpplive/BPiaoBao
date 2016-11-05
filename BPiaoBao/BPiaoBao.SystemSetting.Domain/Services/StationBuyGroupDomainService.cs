using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Services
{
    public class StationBuyGroupDomainService : BaseDomainService
    {
        private const int _errorCode = 200002;
        private readonly IStationBuyGroupRepository _stationBuyGroupRepository;
        private readonly IBusinessmanRepository _businessmanRepository;

        #region ctor

        public StationBuyGroupDomainService(IStationBuyGroupRepository stationBuyGroupRepository, IBusinessmanRepository businessmanRepository)
        {
            _stationBuyGroupRepository = stationBuyGroupRepository;
            _businessmanRepository = businessmanRepository;
        }

        #endregion

        public IQueryable<StationBuyGroup> QueryStationBuyGroups()
        {
            return _stationBuyGroupRepository.FindAll().OrderBy(q => q.GroupName);
        }

        public void AddStationBuyGroup(StationBuyGroup group, string userName)
        {
            #region 数据验证

            //组名不可为空
            if (string.IsNullOrWhiteSpace(group.GroupName))
            {
                throw new CustomException(_errorCode, "平台分销商组名不可为空。");
            }
            //组名不可超过100字符
            if (group.GroupName.Length > 100)
            {
                throw new CustomException(_errorCode, "平台分销商组名不能超过100字。");
            }
            //组名是否已存在
            if (_stationBuyGroupRepository.FindAll(q => q.GroupName == group.GroupName).Count() > 0)
            {
                throw new CustomException(_errorCode, "组名" + group.GroupName + "已存在。");
            }
            //描述不可超过200字符
            if (group.Description.Length > 200)
            {
                throw new CustomException(_errorCode, "描述不能超过100字。");
            }
            //颜色不可为空
            if (string.IsNullOrWhiteSpace(group.Color))
            {
                throw new CustomException(_errorCode, "颜色不可为空。");
            }
            //颜色不可超过100字符
            if (group.Color.Length > 100)
            {
                throw new CustomException(_errorCode, "颜色不能超过100字");
            }
            //必须存在操作员
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new CustomException(_errorCode, "操作用户不可为空。");
            }

            #endregion
            //补充最后操作信息
            group.LastOperatorUser = userName;
            group.LastOperatTime = DateTime.Now;

            _stationBuyGroupRepository.Create(group);

            _unitOfWork.Commit();
        }

        public void UpdateStationBuyGroup(StationBuyGroup group, string userName)
        {
            #region 数据验证
            if (string.IsNullOrWhiteSpace(group.ID))
            {
                throw new CustomException(_errorCode, "分组ID不可为空。");
            }
            //组名不可为空
            if (string.IsNullOrWhiteSpace(group.GroupName))
            {
                throw new CustomException(_errorCode, "平台分销商组名不可为空。");
            }
            //组名不可超过100字符
            if (group.GroupName.Length > 100)
            {
                throw new CustomException(_errorCode, "平台分销商组名不能超过100字。");
            }
            //验证组名是否存在
            if (_stationBuyGroupRepository.FindAll(q => q.GroupName == group.GroupName && q.ID != group.ID).Count() > 0)
            {
                throw new CustomException(_errorCode, "组名" + group.GroupName + "已存在。");
            }
            //描述不可超过200字符
            if (group.Description.Length > 200)
            {
                throw new CustomException(_errorCode, "描述不能超过100字。");
            }
            //颜色不可为空
            if (string.IsNullOrWhiteSpace(group.Color))
            {
                throw new CustomException(_errorCode, "颜色不可为空。");
            }
            //颜色不可超过100字符
            if (group.Color.Length > 100)
            {
                throw new CustomException(_errorCode, "颜色不能超过100字");
            }
            //必须存在操作员
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new CustomException(_errorCode, "操作用户不可为空。");
            }

            #endregion

            var oldGroup = _stationBuyGroupRepository.FindAll(q => q.ID == group.ID).FirstOrDefault();
            if (oldGroup == null)
            {
                throw new CustomException(_errorCode, "分组ID不存在。");
            }
            oldGroup.GroupName = group.GroupName;
            oldGroup.Description = group.Description;
            oldGroup.Color = group.Color;
            oldGroup.LastOperatorUser = userName;
            oldGroup.LastOperatTime = DateTime.Now;

            _stationBuyGroupRepository.Update(oldGroup);

            _unitOfWork.Commit();
        }

        public void DeleteStationBuyGroup(string groupID)
        {
            if (string.IsNullOrWhiteSpace(groupID))
            {
                throw new CustomException(_errorCode, "分组ID不可为空。");
            }

            var oldGroup = _stationBuyGroupRepository.FindAll(q => q.ID == groupID).FirstOrDefault();
            if (oldGroup == null)
            {
                throw new CustomException(_errorCode, "分组ID不存在。");
            }

            _stationBuyGroupRepository.Delete(oldGroup);

            _unitOfWork.Commit();
        }

        public void SetBuyerToGroup(IList<string> buyerCodes, string groupID)
        {
            #region 数据验证

            if (string.IsNullOrWhiteSpace(groupID))
            {
                throw new CustomException(_errorCode, "分组ID不可为空。");
            }
            if (_stationBuyGroupRepository.FindAll(q => q.ID == groupID).Count() == 0)
            {
                throw new CustomException(_errorCode, "分组ID不存在。");
            }

            #endregion

            if (buyerCodes != null)
            {
                foreach (string code in buyerCodes)
                {
                    var buyer=_businessmanRepository.FindAll().Cast<Buyer>().Where(q => q.Code == code).FirstOrDefault();

                    if (buyer == null)
                    {
                        throw new CustomException(_errorCode, "商户号" + code + "不存在。");
                    }
                    buyer.StationBuyGroupID = groupID;

                    _businessmanRepository.Update(buyer);
                }
            }

            _unitOfWork.Commit();
        }
    }
}
