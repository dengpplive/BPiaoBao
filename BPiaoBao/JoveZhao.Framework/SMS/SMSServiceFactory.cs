using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.SMS
{
   public class SMSServiceFactory
    {
       static SMSServiceFactory()
       {
           var smsConfig = JZFSection.GetInstances().SMS;
           ObjectFactory.Configure(x => {
               x.For(typeof(ISMSService)).Use(smsConfig.Type);
           });
       }
      
      public static ISMSService GetEmailService()
       {
           return ObjectFactory.GetInstance<ISMSService>();
       }
    }
}
