using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface INoticeService
    {
        /// <summary>
        /// 获取公告列表
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<NoticeDto> FindNoticeList(EnumNoticeType? noticeType, string title, DateTime? startTime, DateTime? endTime, int currentPageIndex, int pageSize);
        /// <summary>
        /// 获取公告详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        NoticeDto FindNoticeById(int Id);


        /// <summary>
        /// 获得登录时弹出通知公告
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        void GetLoginPopNotice();

    }
}
