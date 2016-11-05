using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.SOA
{
   public class HeartProcess : ICommandProcess //心跳处理器返回消息列表给客户端
    {
       
        public ResponseMessage Execute(RequestMessage request)
        {
            return new ResponseMessage() { Data = request.Data };
        }


        public string Cmd
        {
            get { return "heart"; }
        }
    }
}