using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.Notice;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.SystemSetting
{
    public partial class NoticeService : BaseService, INoticeService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());

        INoticeRepository noticeRepository;
        CurrentUserInfo currentUser;
        IBusinessmanRepository businessmanRepository;
        public NoticeService(INoticeRepository noticeRepository, IBusinessmanRepository businessmanRepository)
        {
            this.noticeRepository = noticeRepository;
            currentUser = AuthManager.GetCurrentUser();
            this.businessmanRepository = businessmanRepository;
        }

        public DataPack<NoticeDto> FindNoticeList(EnumNoticeType? noticeType, string title, DateTime? startTime, DateTime? endTime, int currentPageIndex, int pageSize)
        {
            var user = AuthManager.GetCurrentUser();
            if (user == null)
            {
                //throw  new CustomException(10001,"获取当前用户出错");
                Logger.WriteLog(LogType.ERROR, "获取当前用户出错");

            }
            var code = user == null ? "" : user.CarrierCode;//运营Code

            //if (AuthManager.GetCurrentUser() == null && token != null)
            //{
            //    var auth = ObjectFactory.GetInstance<AuthService>();
            //    var user = auth.GetCurrentUserByToken(token);
            //    code = user.CarrierCode;
            //}
            //else
            //{
            //    code = AuthManager.GetCurrentUser() != null ? AuthManager.GetCurrentUser().CarrierCode : code;//运营Code
            //}
            var query = noticeRepository.FindAll(p => p.State == true && (p.Code == code || (p.Code == "" && p.NoticeShowType.Contains("0"))));
            if (!string.IsNullOrEmpty(title))
                query = query.Where(p => p.Title.Contains(title));
            if (startTime.HasValue)
                query = query.Where(p => p.CreateTime > startTime);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateTime < endTime);
            if (noticeType.HasValue)
            {
                var type = ((int)noticeType).ToString();
                query = query.Where(p => p.NoticeType.Contains(type));
            }
           
            var list = query.OrderByDescending(o => o.CreateTime).Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();
            var datapack = new DataPack<NoticeDto>();
            datapack.TotalCount = query.Count();
            datapack.List = AutoMapper.Mapper.Map<List<Notice>, List<NoticeDto>>(list);
            return datapack;
        }

        public NoticeDto FindNoticeById(int Id)
        {
            var mnotice = noticeRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            return AutoMapper.Mapper.Map<Notice, NoticeDto>(mnotice);
        }

        public void GetLoginPopNotice()
        {
            var user = AuthManager.GetCurrentUser();
            if (user == null)
            {
                //throw  new CustomException(10001,"获取当前用户出错");
                Logger.WriteLog(LogType.ERROR, "获取当前用户出错");

            }
            var code = user == null ? "" : user.Code;
            var account = user == null ? "" : user.OperatorAccount;
            var curDate = DateTime.Now;
            var list = FindNoticeList(EnumNoticeType.LoginEject, "", null, null, 1, 20);
            var models = list.List.Where(p => p.NoticeShowType.Contains(((int)EnumNoticeShowType.Mpb).ToString()) && p.EffectiveStartTime <= curDate && p.EffectiveEndTime >= curDate).ToList();

            var sb = new StringBuilder();
            foreach (var noticeDto in models)
            {
                sb.Append(noticeDto.ID + ",");
            }
            if (sb.Length <= 0) return;
            var s = sb.ToString().Substring(0, sb.Length - 1);
            MessagePushManager.Send(code, account, EnumPushCommands.LoginPopNoticeWindow, "公告", new object[] { s });
        }
    }
}
