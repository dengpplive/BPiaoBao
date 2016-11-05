using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.SOA
{
    class AuthProcess:ICommandProcess
    {
        public string Cmd
        {
            get { return "auth"; }
        }

        public ResponseMessage Execute(RequestMessage request)
        {
            throw new NotImplementedException();
        }
    }
}
