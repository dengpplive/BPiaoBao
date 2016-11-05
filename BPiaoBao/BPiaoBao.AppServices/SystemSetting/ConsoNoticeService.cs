using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.SystemSetting.Domain.Models.Notice;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class NoticeService : IConsoNoticeService
    {

        public void AddNotice(RequestNoticeDto requestNoticeDto)
        {
            var model = AutoMapper.Mapper.Map<RequestNoticeDto, Notice>(requestNoticeDto);
            model.CreateName = currentUser.OperatorName;
            model.CreateTime = DateTime.Now;
            model.Code = currentUser.Code;
            model.State = true;
            model.EffectiveEndTime = model.EffectiveEndTime.AddHours(23).AddMinutes(59).AddSeconds(59);
            this.unitOfWorkRepository.PersistCreationOf(model);
            this.unitOfWork.Commit();
        }

        public void ModifyNotice(NoticeDataObj noticedataobj)
        {
            var model = this.noticeRepository.FindAll(p => p.ID == noticedataobj.ID).SingleOrDefault();
            if (model == null)
                throw new CustomException(500, "公告不存在");
            model.NoticeShowType = noticedataobj.NoticeShowType;
            model.NoticeAttachments.Clear();
            noticedataobj.NoticeAttachments.Select(p => new NoticeAttachment
            {
                Url = p.Url,
                Name = p.Name,
            }).ToList().ForEach(x => model.NoticeAttachments.Add(x));
            model.NoticeType = noticedataobj.NoticeType;
            model.Title = noticedataobj.Title;
            model.Contents = noticedataobj.Contents;
            model.EffectiveStartTime = noticedataobj.EffectiveStartTime;
            model.EffectiveEndTime = noticedataobj.EffectiveEndTime.AddHours(23).AddMinutes(59).AddSeconds(59); 
            model.State = noticedataobj.State;
            this.unitOfWorkRepository.PersistUpdateOf(model);
            this.unitOfWork.Commit();
        }

        public PagedList<NoticeDataObj> FindNotice(string title, bool? state, DateTime? startTime, DateTime? endTime, int currentPageIndex, int pageSize)
        {

            var query = this.noticeRepository.FindAll(p => p.Code == currentUser.Code);
            if (!string.IsNullOrEmpty(title))
                query = query.Where(p => p.Title.Contains(title));
            if (state.HasValue)
                query = query.Where(p => p.State == state);

            var list = query.OrderByDescending(o => o.CreateTime).Skip(currentPageIndex).Take(pageSize).ToList();
            return new PagedList<NoticeDataObj>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<Notice>, List<NoticeDataObj>>(list)
            };
        }

        public NoticeDataObj FindConsoNoticeById(int Id)
        {
            var mnotice = noticeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            return AutoMapper.Mapper.Map<Notice, NoticeDataObj>(mnotice);
        }
        public void DeleteAttachMent(int NoticeId, int AttachMentId)
        {
            var model = this.noticeRepository.FindAll(p => p.Code == currentUser.Code && p.ID == NoticeId).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作公告不存在");
            var mattach = model.NoticeAttachments.SingleOrDefault(p => p.Id == AttachMentId);
            model.NoticeAttachments.Remove(mattach);
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }


        public PagedList<NoticeDataObj> FindIndustyNotice(string title,int currentPageIndex, int pageSize)
        {
            var query = this.noticeRepository.FindAll(p =>p.State == true);
            if (this.currentUser.Type=="Supplier")
            {
                query = query.Where(p => p.Code == currentUser.CarrierCode || (p.Code == "" && p.NoticeShowType.Contains("2")));
            }
            else
            {
                query = query.Where(p => p.NoticeShowType.Contains("1"));
            }
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(p => p.Title.Contains(title));
            }
            var list = query.OrderByDescending(o => o.CreateTime).Skip(currentPageIndex).Take(pageSize).ToList();
            return new PagedList<NoticeDataObj>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<Notice>, List<NoticeDataObj>>(list)
            };
        }
    }
}
