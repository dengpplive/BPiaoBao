using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
    public interface IPlugin
    {
        /// <summary>
        /// 获取模块菜单
        /// </summary>
        IEnumerable<MainMenuItem> Parts { get; }
    }
}
