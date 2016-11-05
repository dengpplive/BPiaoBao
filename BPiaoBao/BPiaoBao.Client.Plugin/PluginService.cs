using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
   public class PluginService
    {
       /// <summary>
       /// tab项目打开
       /// </summary>
       /// <param name="projectCode">程序集编号</param>
       /// <param name="partCode">子集菜单编号</param>
       public static void Run(string projectCode, string partCode,bool isMain=false)
       {
           var uc = PluginManager.FindItem(projectCode, partCode);

           MessageObserver.CreateInstance.OnAction(new PluginEventArgs(uc.Title, uc.GetContent(),projectCode,partCode,isMain));
       }
       
    }
}
