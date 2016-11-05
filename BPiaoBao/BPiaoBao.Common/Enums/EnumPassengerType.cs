using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.Common.Enums
{
   
    public enum EnumPassengerType
    {
        
        /// <summary>
        /// 成人
        /// </summary>
        [Description("成人")]
        Adult = 1,
        /// <summary>
        /// 儿童
        /// </summary>
        [Description("儿童")]
        Child = 2,
        /// <summary>
        /// 婴儿
        /// </summary>
        [Description("婴儿")]
        Baby = 3
    }
}
