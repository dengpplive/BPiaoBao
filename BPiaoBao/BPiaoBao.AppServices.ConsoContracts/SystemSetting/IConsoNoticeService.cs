using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting
{
    [ServiceContract]
    public interface IConsoNoticeService
    {
        /// <summary>
        /// 发布公告
        /// </summary>
        /// <param name="requestNotice"></param>
        [OperationContract]
        void AddNotice(RequestNoticeDto requestNoticeDto);
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="requestNoticeDto"></param>
        [OperationContract]
        void ModifyNotice(NoticeDataObj noticedataobj);
        /// <summary>
        /// 获取自己发布的公告
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
        PagedList<NoticeDataObj> FindNotice(string title, bool? state, DateTime? startTime, DateTime? endTime, int currentPageIndex, int pageSize);
        /// <summary>
        /// 根据ID获取公告信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        NoticeDataObj FindConsoNoticeById(int Id);
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
        void DeleteAttachMent(int NoticeId, int AttachMentId);
        /// <summary>
        /// 获取平台发布的运营公告
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<NoticeDataObj> FindIndustyNotice(string title,int currentPageIndex, int pageSize);
    }
}
