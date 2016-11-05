using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
    public interface IPart
    {
        /// <summary>
        /// 获取内容
        /// </summary>
        /// <returns></returns>
        object GetContent();

        /// <summary>
        /// 获取标题
        /// </summary>
        string Title { get; }
    }
}
