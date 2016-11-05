using System;
using System.Collections.Generic;

namespace BPiaoBao.Client.Module
{
    public class MainMenuItem
    {


        /// <summary>
        /// 代码
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 显示图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public IEnumerable<MainMenuItem> Children { get; set; }

        ///// <summary>
        ///// 是否显示tipcount属性
        ///// </summary>
        //public bool IsShowTip { get; set; }
    }
}
