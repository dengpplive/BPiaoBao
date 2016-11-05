using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
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
    public partial class MyMessageService:BaseService,IMyMessageService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        IMyMessageRepository myMsgRepository;
        CurrentUserInfo currentUser;
        public MyMessageService(IMyMessageRepository myMsgRepository)
        {
            currentUser = AuthManager.GetCurrentUser();
            this.myMsgRepository = myMsgRepository;
        }
        public DataContracts.DataPack<DataContracts.SystemSetting.MyMessageDto> FindMyMsgList(int currentPageIndex, int pageSize)
        {
            if (currentUser == null)
            {
                Logger.WriteLog(LogType.ERROR, "获取当前用户出错");
            }
            var query = myMsgRepository.FindAll(f => f.Code == currentUser.Code);
            var list = query.OrderByDescending(o => o.CreateTime).Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new DataPack<MyMessageDto>() {
                TotalCount = query.Count(),
                List = AutoMapper.Mapper.Map<List<MyMessage>, List<MyMessageDto>>(list)
            };
        }

        public DataContracts.SystemSetting.MyMessageDto FindMyMsgById(int Id)
        {
            var mymsg = myMsgRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            return AutoMapper.Mapper.Map<MyMessage, MyMessageDto>(mymsg);
        }


        public void ModifyMsgState(int Id)
        {
            var mymsg = myMsgRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            mymsg.State = true;
            unitOfWorkRepository.PersistUpdateOf(mymsg);
            unitOfWork.Commit();
        }


        public int GetUnReadMsgCount()
        {
            var query = myMsgRepository.FindAll(f => f.Code == currentUser.Code && f.State==false);
            return query.ToList().Count();
        }
    }
}
