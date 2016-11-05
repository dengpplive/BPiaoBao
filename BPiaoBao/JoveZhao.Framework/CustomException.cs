using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework
{
    /// <summary>
    /// 自定义错误信息
    /// </summary>
    [DataContract]
    public class CustomException : Exception
    {
        public int ErrorCode { get; private set; }
        public CustomException( int errorCode,string message)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}
