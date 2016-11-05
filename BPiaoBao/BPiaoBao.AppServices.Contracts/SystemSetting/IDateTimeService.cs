using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface IDateTimeService
    {
        /// <summary>
        /// 得到当前系统日期时间
        /// </summary>
        /// <returns>datetime类型</returns>
        [OperationContract]
        DateTime GetCurrentSystemDateTime();

        /// <summary>
        /// 得到当前系统长日期
        /// </summary>
        /// <returns>string类型(格式:yyyy-MM-dd HH:mm:ss)</returns>
        [OperationContract]
        string GetCurrentSystemDateTimeLongString();

        /// <summary>
        /// 得到当前系统短日期
        /// </summary>
        /// <returns>string类型(格式:yyyy-MM-dd)</returns>
        [OperationContract]
        string GetCurrentSystemDateTimeShortString();

        /// <summary>
        /// 得到当前系统时间
        /// </summary>
        /// <returns>string类型(格式:HH:mm:ss)</returns>
        [OperationContract]
        string GetCurrentSystemTimeString();

        /// <summary>
        /// 得到当前系统时间的Ticks
        /// 表示自 0001 年 1 月 1 日午夜 12:00:00（表示 DateTime.MinValue）
        /// 以来经过的以 100 纳秒为间隔的间隔数.
        /// 每个计时周期表示一百纳秒，即一千万分之一秒
        /// </summary>
        /// <returns></returns>
         [OperationContract]
        long GetCurrentSystemTimeTicks();
    }
}
