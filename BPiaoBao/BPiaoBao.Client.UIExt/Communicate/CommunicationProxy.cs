
using BPiaoBao.AppServices.Contracts.Cashbag;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using JoveZhao.Framework;

namespace BPiaoBao.Client.UIExt.Communicate
{
    /// <summary>
    /// 客户端通信代理对象（抽象需要重复调用的通信代码）
    /// </summary>
    public class CommunicationProxy
    {
        /// <summary>
        /// 获取当前用户授信开通权限
        /// </summary>
        /// <param name="isThrowEx">异常是否抛错</param>
        /// <returns></returns>
        public static bool GetCreditOpenStatus(bool isThrowEx = false)
        {
            bool result = false;
            CommunicateManager.Invoke<IAccountService>(service =>
            {
                var temp = service.GetAccountInfo();
                if (temp != null && temp.CreditInfo != null)
                    result = temp.CreditInfo.Status;
            }, ex =>
            {
                Logger.WriteLog(LogType.WARN, ex.Message, ex);
                if (isThrowEx)
                    throw ex;
            });
            return result;
        }

        /// <summary>
        /// 获取客服信息
        /// </summary>
        /// <param name="throwEx">if set to <c>true</c> [throw ex].</param>
        /// <returns></returns>
        public static CustomerDto GetCustomerService(bool throwEx = false)
        {
            CustomerDto result = null;
            CommunicateManager.Invoke<IBusinessmanService>(service =>
            {
                result = service.GetCustomerInfo();

            }, ex =>
            {
                Logger.WriteLog(LogType.WARN, ex.Message, ex);
                if (throwEx)
                    throw ex;
            });

            return result;
        }
    }
}
