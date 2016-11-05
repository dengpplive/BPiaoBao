using JoveZhao.Framework;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(BPiaoBao.Web.SupplierManager.Startup))]
namespace BPiaoBao.Web.SupplierManager
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                app.MapSignalR();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString(), ex);
            }
        }
    }
}