using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting
{
    [ServiceContract]
    public interface IStationNoticeService
    {
        /// <summary>
        /// 发布公告
        /// </summary>
        /// <param name="requestNotice"></param>
        [OperationContract]
        void AddNotice(RequestNotice requestNotice);
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="noticeDto"></param>
        [OperationContract]
        void ModifyNotice(NoticeDto noticeDto);
        /// <summary>
        /// 获取公告列表
        /// </summary>
        /// <param name="title"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<NoticeDto> FindNotice(string title, string code, bool? state, DateTime? startTime, DateTime? endTime, int currentPageIndex, int pageSize);
        /// <summary>
        /// 根据ID获取公告信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        NoticeDto FindNoticeInfoById(int Id);
        /// <summary>
        /// 公告启用禁用
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void NoticeEnableOrDisable(int Id);
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void DeleteNotice(int Id);
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void DeleteNoticeAttachMent(int NoticeId,int AttachMentId);
    }
}
