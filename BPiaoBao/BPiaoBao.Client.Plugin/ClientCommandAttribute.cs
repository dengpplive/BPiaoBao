using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
     [AttributeUsage(AttributeTargets.Class)]
    public class ClientCommandAttribute : Attribute
    {
         public string CommandKey { get; set; }

         public object Command { get; set; }

        public ClientCommandAttribute(string commandKey, object command)
        {
            this.CommandKey = commandKey;
            this.Command = command;
        }

    }
}
