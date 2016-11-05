using JoveZhao.Framework;
using System;
using System.Threading.Tasks;

namespace BPiaoBao.Client.UIExt.Helper
{
    /// <summary>
    /// 异步执行帮助类
    /// </summary>
    public class AsynHelper
    {

        /// <summary>
        /// 异步执行函数
        /// </summary>
        /// <param name="invokeAction">异步执行的函数.</param>
        /// <param name="exCall">执行时抛错的回调.</param>
        public static Task Invoke(Action invokeAction, Action<Exception> exCall)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    invokeAction();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(LogType.WARN, ex.Message, ex);
                    if (exCall != null)
                        exCall(ex);
                }
            });
        }
    }
}
