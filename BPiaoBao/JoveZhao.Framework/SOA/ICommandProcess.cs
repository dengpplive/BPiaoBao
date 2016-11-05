using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.SOA
{
   public interface ICommandProcess
    {
        string Cmd { get; }
        ResponseMessage Execute(RequestMessage request);
    }
}
