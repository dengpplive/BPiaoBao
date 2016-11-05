using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.SOA
{
    [Serializable]
    public class ResponseMessage
    {
        public string Cmd { get; set; }
        public IData Data { get; set; }
    }
}
