using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
   public class PluginEventArgs:EventArgs
    {
       public string PluginCode { get; private set; }
       public string PartCode { get; private set; }
        public string Header { get; private set; }
        public object UserControl { get; private set; }
        public bool IsMain { get; private set; }

        public PluginEventArgs(string header, object userControl, string pluginCode, string partCode,bool isMain=false)
        {
            this.Header = header;
            this.UserControl = userControl;
            this.PartCode = partCode;
            this.PluginCode = pluginCode;
            this.IsMain = isMain;
        }
    }
}
