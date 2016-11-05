using System;

namespace BPiaoBao.Client.Module
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        public string Code { get;private set; }
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public int Sequence { get; private set; }
        public string HomeCode { get; private set; }
        /// <summary>
        /// 是否显示tipcount属性
        /// </summary>
        public bool IsShowTip { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        /// <param name="sequence"></param>
        /// <param name="homeCode"></param>
        /// <param name="isShowTip"></param>
        public PluginAttribute(string code, string name, string icon, int sequence, string homeCode,bool isShowTip=false)
        {
            this.Code = code;
            this.Name = name;
            this.Icon = icon;
            this.Sequence = sequence;
            this.HomeCode = homeCode;
            this.IsShowTip = isShowTip;
        }               
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PartAttribute : Attribute
    {
        public string Code { get;private set; }
        public PartAttribute(string code)
        {
            this.Code = code;
        }
    }
}
