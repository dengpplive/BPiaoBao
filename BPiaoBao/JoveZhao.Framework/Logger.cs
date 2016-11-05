using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework
{
    public enum LogType
    {
        /// <summary>
        /// 致命错误
        /// 记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。
        /// </summary>
        FATAL,
        /// <summary>
        /// 一般错误
        /// 记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        ERROR,
        /// <summary>
        /// 警告
        /// 记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。
        /// </summary>
        WARN,
        /// <summary>
        /// 一般信息
        /// 记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        INFO,
        /// <summary>
        /// 调试信息
        /// 记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。
        /// </summary>
        DEBUG
    }
    public class Logger
    {
        static Logger()
        {
            
            if (System.Web.HttpContext.Current == null)
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("config/log4net.config"));
            else
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(System.Web.HttpContext.Current.Server.MapPath("~/config/log4net.config")));
        }
        public static void WriteLog(LogType type, string message, Exception ex = null)
        {
            ILog logger;
            switch (type)
            {
                case LogType.DEBUG:
                    logger = LogManager.GetLogger("logger-debug");
                    logger.Debug(message, ex);
                    break;
                case LogType.ERROR:
                    logger = LogManager.GetLogger("logger-error");
                    logger.Error(message, ex);
                    break;
                case LogType.FATAL:
                    logger = LogManager.GetLogger("logger-fatal");
                    logger.Fatal(message, ex);
                    break;
                case LogType.INFO:
                    logger = LogManager.GetLogger("logger-info");
                    logger.Info(message, ex);
                    break;
                case LogType.WARN:
                    logger = LogManager.GetLogger("logger-warn");
                    logger.Warn(message, ex);
                    break;
            }

        }
    }
}
