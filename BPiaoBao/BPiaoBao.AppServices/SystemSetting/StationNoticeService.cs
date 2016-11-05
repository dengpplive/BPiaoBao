using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.AppServices.StationContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.Notice;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class NoticeService : IStationNoticeService
    {

        public void AddNotice(RequestNotice requestNotice)
        {
            var currentuser = AuthManager.GetCurrentUser();
            var model = AutoMapper.Mapper.Map<RequestNotice, Notice>(requestNotice);
            model.CreateName = currentuser.OperatorName;
            model.CreateTime = DateTime.Now;
            model.Code = "";
            model.State = true;
            model.EffectiveEndTime = model.EffectiveEndTime.AddHours(23).AddMinutes(59).AddSeconds(59);
            this.unitOfWorkRepository.PersistCreationOf(model);
            this.unitOfWork.Commit();
            try
            {
                if (string.IsNullOrEmpty(model.NoticeShowType))
                    return;
                List<string> list = new List<string>();
                if (model.NoticeShowType.Contains("1"))
                    list.AddRange(this.businessmanRepository.FindAllNoTracking().OfType<Carrier>().Select(x => x.Code).ToList());
                if (model.NoticeShowType.Contains("2"))
                    list.AddRange(this.businessmanRepository.FindAllNoTracking().OfType<Supplier>().Select(x => x.Code).ToList());
                if (list.Count > 0)
                    WebMessageManager.GetInstance().Send(EnumMessageCommand.Aannouncement, list.ToArray(), model.Title);
                if (model.NoticeShowType.Contains("0"))
                    MessagePushManager.SendAll(EnumPushCommands.EnforcePopNoticeWindow, model.Title, false, new object[] { model.ID });
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "公告异常", e);

            }

        }

        public void ModifyNotice(NoticeDto noticeDto)
        {
            var model = this.noticeRepository.FindAll(p => p.ID == noticeDto.ID).SingleOrDefault();
            if (model == null)
                throw new CustomException(500, "公告不存在");
            model.NoticeShowType = noticeDto.NoticeShowType;
            model.NoticeAttachments.Clear();
            noticeDto.NoticeAttachments.Select(p => new NoticeAttachment
            {
                Url = p.Url,
                Name = p.Name,
            }).ToList().ForEach(x => model.NoticeAttachments.Add(x));
            model.NoticeType = noticeDto.NoticeType;
            model.Title = noticeDto.Title;
            model.Contents = noticeDto.Contents;
            model.EffectiveStartTime = noticeDto.EffectiveStartTime;
            model.EffectiveEndTime = noticeDto.EffectiveEndTime.AddHours(23).AddMinutes(59).AddSeconds(59);
            model.State = noticeDto.State;
            this.unitOfWorkRepository.PersistUpdateOf(model);
            this.unitOfWork.Commit();
        }

        public PagedList<NoticeDto> FindNotice(string title, string code, bool? state, DateTime? startTime, DateTime? endTime, int currentPageIndex, int pageSize)
        {
            var query = this.noticeRepository.FindAll();
            if (!string.IsNullOrEmpty(title))
                query = query.Where(p => p.Title.Contains(title));
            if (!string.IsNullOrEmpty(code))
                query = query.Where(p => p.Code == code);
            if (state.HasValue)
                query = query.Where(p => p.State == state);
            if (startTime.HasValue)
                query = query.Where(p => p.CreateTime > startTime);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateTime < endTime);

            var list = query.OrderByDescending(o => o.CreateTime).Skip(currentPageIndex).Take(pageSize).ToList();
            return new PagedList<NoticeDto>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<Notice>, List<NoticeDto>>(list)
            };
        }

        public NoticeDto FindNoticeInfoById(int Id)
        {
            var mnotice = noticeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            return AutoMapper.Mapper.Map<Notice, NoticeDto>(mnotice);

        }

        public void NoticeEnableOrDisable(int Id)
        {
            var model = noticeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作公告不存在!");
            model.State = !model.State;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }

        public void DeleteNotice(int Id)
        {
            var model = this.noticeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作公告不存在");
            unitOfWorkRepository.PersistDeletionOf(model);
            unitOfWork.Commit();
        }


        public void DeleteNoticeAttachMent(int NoticeId, int AttachMentId)
        {
            var model = this.noticeRepository.FindAll(p => p.ID == NoticeId).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "操作公告不存在");
            var mattach = model.NoticeAttachments.SingleOrDefault(p => p.Id == AttachMentId);
            model.NoticeAttachments.Remove(mattach);
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }
    }
}
