using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.DomesticTicket
{
    /// <summary>
    /// 内存信息读取-航空公司、平台接口
    /// </summary>
    [ServiceContract]
    public interface IMemoryService
    {
        /// <summary>
        /// 航空公司-列表读取
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PagedList<BPiaoBao.Common.AirSystem> GetMemoryAirList(int page, int rows);
        /// <summary>
        /// 平台接口管理-列表读取
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PagedList<BPiaoBao.Common.PlatSystem> GetMemoryPlatList(int page, int rows);
        /// <summary>
        /// 初始化系统全局开关
        /// </summary>
        [OperationContract]
        void InitSystemSwitchInfo();
        /// <summary>
        /// 航空公司新增
        /// </summary>
        /// <param name="vm"></param>
        [OperationContract]
        string AriLineSave(AirLine vm);
        /// <summary>
        /// 删除航空公司
        /// </summary>
        /// <param name="CarrayCode"></param>
        [OperationContract]
        void DeleteAriLine(int Id);
        /// <summary>
        /// 修改航空公司
        /// </summary>
        /// <param name="vm"></param>
        [OperationContract]
        string ModiflyAriLine(AirLine vm);
        /// <summary>
        /// 获取航空公司信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        AirLine GetAirLineInfo(int Id);
        /// <summary>
        /// 查询航空公司列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<AirLine> GetAriLineList(AirLine model, int page, int rows);
        /// <summary>
        /// 修改航变设置信息
        /// </summary>
        /// <param name="MD"></param>
        [OperationContract]
        void SetAirChangTimeOutInfo(string QTStartTime, string QTEndTime, string TimeOut, bool? IsOpen);
        /// <summary>
        /// 获取设置航变时间间隔
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        QTSetting GetAirChangeTimeOutInfo();
    }
}
