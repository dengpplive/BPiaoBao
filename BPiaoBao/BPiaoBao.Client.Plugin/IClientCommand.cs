using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
    /// <summary>
    /// 客户端命令
    /// </summary>
    public interface IClientCommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Invoke(params object[] para);
    }
}
