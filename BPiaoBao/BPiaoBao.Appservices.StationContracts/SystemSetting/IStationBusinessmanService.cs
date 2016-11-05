using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common.Enums;
using System.IO;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting
{

    [ServiceContract]
    public interface IStationBusinessmanService
    {
        /// <summary>
        /// 添加运营商
        /// </summary>
        [OperationContract]
        void AddBussinessmen(RequestCarrier carrier);
        /// <summary>
        /// 根据Code获取运营商信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseDetailCarrier GetCarrierByCode(string code);
        /// <summary>
        /// 信息变更
        /// </summary>
        /// <param name="businessmanDataObject"></param>
        [OperationContract]
        void ModifyCarrier(RequestCarrier carrier);
        /// <summary>
        /// 商户号是否存在
        /// </summary>
        /// <param name="code">商户号</param>
        /// <returns>存在True</returns>
        [OperationContract]
        bool IsExistCarrier(string code);
        /// <summary>
        /// 查询商户列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="cashbagCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ResponseListCarrierOrSupplier> FindCarrierOrSupplier(string name, string code, string cashbagCode, DateTime? startTime, DateTime? endTime, int startIndex, int count, int type);
        /// <summary>
        /// 查找供应商
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        StationContracts.SystemSetting.SystemMap.ResponseDetailSupplier GetSupplierByCode(string code);
        /// <summary>
        /// 重置商户管理员密码
        /// </summary>
        /// <param name="code">商户号</param>
        [OperationContract]
        void ResetAdminPassword(string code);
        /// <summary>
        /// 启用或者禁用商户[启用->禁用,禁用->启用]
        /// </summary>
        /// <param name="code">商户号</param>
        [OperationContract]
        void EnableAndDisable(string code);
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="messageList">需要发送消息列表</param>
        /// <param name="contentTemplate">内容模版</param>
        [OperationContract]
        void SendMessage(string jsonStr, string contentTemplate);
        [OperationContract]
        BusinessmanDataObject GetBusinessmanByCode(string code);
        /// <summary>
        /// 获取商户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseBusinessman GetBusinessmanByCashBagCode(string code);
        /// <summary>
        /// 获取采购商列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="carriercode"></param>
        /// <param name="cashbagCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseListBuyer> FindBuyerList(string code, string carriercode, string cashbagCode, string tel, DateTime? startTime, DateTime? endTime, int startIndex, int count);
        /// <summary>
        /// 发送一般信息
        /// </summary>
        [OperationContract]
        void SendNormalMsg(string content, bool isRepeatSend = false);

        /// <summary>
        /// 短信汇总报表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="businessName"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseSMSSum> GetSMSSum(string code, string businessName, DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 短信使用汇总报表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseSMSSendSum> GetSMSSendSum(DateTime? startTime, DateTime? endTime);

        /// <summary>
        /// 短信销售汇总报表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseSMSSaleSum> GetSMSSaleSum(DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// OPEN票扫描列表
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseOPENScan> GetOpenScanList(int startIndex, int count);
        /// <summary>
        /// 添加OPEN票扫描信息
        /// </summary>
        /// <param name="OpenScan"></param>
        [OperationContract]
        void AddOpenScanInfo(RequestOPENScan OpenScan);
        /// <summary>
        /// 获取运营PID信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        PIDDataObject GetCarrierPid(string code);
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [OperationContract]
        Tuple<string, byte[]> DownloadOpenFileByName(string fileName);

        /// <summary>
        /// 获取控台客服信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CustomerDto GetStationCustomerInfo();
        /// <summary>
        /// 设置控台客服信息
        /// </summary>
        /// <param name="customerInfo"></param>
        [OperationContract]
        void SetCustomerInfo(CustomerDto customerInfo);
        /// <summary>
        /// 查询供应商列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="cashbagCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<SupplierDataObj> FindSupplier(string code, string carriercode, DateTime? startTime, DateTime? endTime, int startIndex, int count);
        /// <summary>
        /// 根据Code获取供应商信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        SupplierDataObj FindSupplierInfoByCode(string code);
        /// <summary>
        /// 修改供应开关
        /// </summary>
        /// <param name="businessmanDataObject"></param>
        [OperationContract]
        void ModifySupplierSwitch(SupplierDataObj supplierDto);
        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="supplier"></param>
        [OperationContract]
        void ModifySupplier(StationContracts.SystemSetting.SystemMap.STRequestSupplier supplier);
        ///// <summary>
        ///// 添加供应商
        ///// </summary>
        ///// <param name="supplier"></param>
        //[OperationContract]
        //void AddSupplier(BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.STRequestSupplier supplier);

        //查询控台分销分组
        IList<StationBuyerGroupDto> SearchStationBuyerGroups();
        //设置控台分销分组
        void SetBuyerToStationBuyerGroup(SetBuyerToStationBuyerGroupRequest request);
        //删除控台分销分组
        void DeleteStationBuyreGroup(string groupId);
        //编辑控台分销分组
        void SetStationBuyerGroupInfo(SetStationBuyerGroupInfoRequest dto);
        //新增控台分销分组
        void AddStationBuyerGroup(AddStationBuyerGroupRequest request);
    }
}
