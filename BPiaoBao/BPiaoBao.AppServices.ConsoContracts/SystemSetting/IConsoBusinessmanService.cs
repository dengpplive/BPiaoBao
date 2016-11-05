using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting
{
    [ServiceContract]
    public interface IConsoBusinessmanService
    {
        /// <summary>
        /// 添加采购商
        /// </summary>
        [OperationContract]
        void OpenBuyer(RequestBuyer buyer);
        /// <summary>
        /// 修改采购商联系信息
        /// </summary>
        /// <param name="buyer"></param>
        [OperationContract]
        void EditBuyer(RequestBuyer buyer);
        /// <summary>
        /// 获取编辑更改信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        RequestBuyer GetEditBuyerInfo(string code);
        /// <summary>
        ///采购商列表
        /// </summary>
        /// <param name="supplierCode">供应上Code</param>
        /// <param name="code">商户Code</param>
        /// <param name="businessmanName">商户名称</param>
        /// <param name="startTime">开户时间段</param>
        /// <param name="endTime">开户时间段</param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseBuyer> GetBusinessmanBuyerByCode(string code, string businessmanName, DateTime? startTime, DateTime? endTime, int pageIndex = 1, int pageSize = 15);
        /// <summary>
        /// 详细信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseDetailBuyer GetBuyerInfo(string code);
        /// <summary>
        /// 商户禁用启用
        /// </summary>
        /// <param name="code"></param>
        /// <param name="status"></param>
        [OperationContract]
        void BusinessmanSetEnable(string code, bool isenable);
        /// <summary>
        /// 获取操作员工
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ResponseOperator> GetOperators(string account,string realName,string phone,string status,int? roleId);
        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="?"></param>
        [OperationContract]
        void AddConsoOperator(RequestOperator rp);
        /// <summary>
        /// 员工信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        RequestOperator GetOperatorInfo(int id);
        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="rp"></param>
        [OperationContract]
        void EditConsoOperator(RequestOperator rp);
        /// <summary>
        /// 重置员工密码
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
         void ResetPassWord(int Id);
        /// <summary>
        /// 获取角色
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ResponseRole> GetRole(string roleName);
        /// <summary>
        /// 添加角色
        /// </summary>
        [OperationContract]
        void AddRole(RequestRole role);
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        RequestRole GetRoleInfo(int id);
        /// <summary>
        /// 修改角色信息
        /// </summary>
        /// <param name="role"></param>
        [OperationContract]
        void EditRole(RequestRole role);
        /// <summary>
        /// 员工启用禁用
        /// </summary>
        /// <param name="id">操作员ID</param>
        /// <param name="status">状态</param>
        [OperationContract]
        void SetEnableStatus(int id, int status);
        /// <summary>
        /// 获取详细【运营商】
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        CarrierDataObject GetCarrierDetail();
        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="carrier"></param>
        [OperationContract]
        void SubmitCarrier(CarrierDataObject carrier);
        /// <summary>
        /// 下级充值，转账
        /// </summary>
        /// <param name="dataObject"></param>
        [OperationContract]
        void TransferAccount(RechargeDataObject dataObject);
        /// <summary>
        /// 给采购商设置标签
        /// </summary>
        /// <param name="code"></param>
        /// <param name="label"></param>
        [OperationContract]
        void SetBuyerLabel(string code, string label);

        /// <summary>
        /// 获取所有运营商 的Pid信息
        /// </summary> 
        /// <returns></returns>
        [OperationContract]
        List<CarrierDataObject> GetAllCarrier();
        /// <summary>
        /// 获取分配商户
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<SampleListBuyer> GetDistributionBuyer();
        /// <summary>
        /// 分配商户
        /// </summary>
        /// <param name="list"></param>
        [OperationContract]
        void DistributionBuyer(List<SampleBuyer> list);
        /// <summary>
        /// 修改当前登录人员密码
        /// </summary>
        /// <param name="newPwd"></param>
        /// <param name="oldPwd"></param>
        [OperationContract]
        void ModifyPassword(string newPwd, string oldPwd);
        /// <summary>
        /// 获取营运商标签
        /// </summary>
        /// <param name="code">采购上Code</param>
        /// <returns></returns>
        [OperationContract]
        string GetLabel();
        /// <summary>
        /// 采购商设置标签
        /// </summary>
        /// <param name="setLabel"></param>
        [OperationContract]
        void SetLabel(SetLabel setLabel);
        /// <summary>
        /// 获取商户信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CurrentUserInfo GetCurrentUser();
        /// <summary>
        /// 根据扣点组ＩＤ获取商户
        /// </summary>
        /// <param name="deductionId"></param>
        /// <returns></returns>
        [OperationContract]
        List<string> GetSelectedDistribution(int deductionId);
        /// <summary>
        /// 默认政策设置
        /// </summary>
        [OperationContract]
        void SetDefaultPolicy(List<DefaultPolicyDataObject> defaultPolicys);
        /// <summary>
        /// 获取默认政策
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<DefaultPolicyDataObject> GetDefaultPolicy();

        /// <summary>
        /// 获取商户名称
        /// </summary>
        /// <param name="code">商户好</param>
        /// <returns></returns>
        [OperationContract]
        string GetBusinessmanName(string code);
        [OperationContract]
        BusinessmanOperator GetBusinessmanOperator(string code, string name);

        #region 供应商
        /// <summary>
        /// 添加供应商
        /// </summary>
        [OperationContract]
        void AddSupplier(RequestSupplier requestSupplier);
        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <param name="supplierDataObject"></param>
        [OperationContract]
        void UpdateSupplier(RequestSupplier requestSupplier);
        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="businessmanName">商户名称</param>
        /// <param name="startTime">创建日期范围</param>
        /// <param name="endTime">创建日期范围</param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseSupplier> FindSupplier(string code, string businessmanName, DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 获取供应商信息
        /// </summary>
        /// <param name="code">供应商商户号</param>
        /// <returns></returns>
        [OperationContract]
        SupplierDataObject FindSupplierByCode(string code);
        /// <summary>
        /// 获取供应商编辑信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        RequestSupplier EditFind(string code);
        #endregion

        /// <summary>
        /// 重置采购商管理员密码
        /// </summary>
        /// <param name="code"></param>
        [OperationContract]
        void ResetBuyerAdminPwd(string code);

        #region 自动出票设置
        [OperationContract]
        void AutoIssueTicketSave(AutoIssueTicketViewModel vm);
        [OperationContract]
        AutoIssueTicketViewModel GetAutoIssueTicket();
        #endregion

        /// <summary>
        /// 返回权限点数
        /// </summary>
        /// <param name="Account"></param>
        /// <returns></returns>
        [OperationContract]
        Tuple<bool, string> GetMentList(string Account);
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        bool DeleteRole(int Id);
        /// <summary>
        /// 获取控台客服信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CustomerDto GetConsoCustomerInfo();
        /// <summary>
        /// 设置控台客服信息
        /// </summary>
        /// <param name="customerInfo"></param>
        [OperationContract]
        void SetConsoCustomerInfo(CustomerDto customerInfo);
    }
}
