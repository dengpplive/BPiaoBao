/***********************************************************************   
* Copyright(c)    
* CLR 版本: 4.0.30319.34014   
* 命名空间: BPiaoBao.AppServices
* 文 件 名：QueueLogsManager.cs   
* 创 建 人：duanwei   
* 创建日期：2014/11/12 13:56:29       
* 备注描述：           
************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;
using BPiaoBao.AppServices.Contracts.SystemSetting;
using BPiaoBao.AppServices.Contracts.SystemSetting.DataObject;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.Common;
using StructureMap;

namespace BPiaoBao.AppServices
{
    /// <summary>
    /// 队列日志管理器
    /// </summary>
    public class QueueLogsManager
    {
        private static QueueExt<OperationLogDto> _queueExt;
        /// <summary>
        /// 初始参数
        /// </summary>
        public static void Init()
        {
            _queueExt = new QueueExt<OperationLogDto>(50);
            _queueExt.OnQueueEventHandler += (s, e) => ObjectFactory.GetInstance<OperationLogService>().AddBatch(e.Obj);
            Console.WriteLine("QueueLogsManager操作日志队列管理器初始完成...");
        }
        /// <summary>
        /// 日志入队列
        /// </summary>
        /// <param name="log"></param>
        public static void Enqueue(OperationLogDto log)
        {

            _queueExt.Enqueue(log);
        }


    }
}
