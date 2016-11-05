using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface IMyMessageService
    {
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataPack<MyMessageDto> FindMyMsgList(int currentPageIndex, int pageSize);
        /// <summary>
        /// 通过ID获取消息详情
        /// </summary>
        /// <param name="Id">主键ID</param>
        /// <returns></returns>
        [OperationContract]
        MyMessageDto FindMyMsgById(int Id);
        /// <summary>
        /// 消息状态修改(变为已读)
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void ModifyMsgState(int Id);
        /// <summary>
        /// 获取未读消息数量
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        int GetUnReadMsgCount();
    }
}
