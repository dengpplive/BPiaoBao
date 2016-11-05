using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Module
{
    public delegate void DoActionEventHander(object sender, PluginEventArgs args);
   public class MessageObserver
    {
       private MessageObserver()
       {
       }
       private static object lockIn = new object();
       private static MessageObserver createInstance;
       public static MessageObserver CreateInstance
       {
           get {
               if (createInstance == null)
               {
                   lock (lockIn)
                   {
                       if (createInstance == null)
                           createInstance = new MessageObserver();
                   }
               }
               return createInstance;
           }
       }
       public event DoActionEventHander DoAction;
       public void OnAction(PluginEventArgs args)
       {
           if (DoAction != null)
               DoAction(this, args);
       }
    }
}
