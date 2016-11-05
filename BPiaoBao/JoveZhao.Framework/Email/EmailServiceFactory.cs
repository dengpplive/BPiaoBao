using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Email
{
   public class EmailServiceFactory
    {
       static EmailServiceFactory()
       {
           var emailConfig = JZFSection.GetInstances().Email;
           ObjectFactory.Configure(x => {
               x.For(typeof(IEmailService)).Use(emailConfig.Type);
           });
       }
      
      public static IEmailService GetEmailService()
       {
           return ObjectFactory.GetInstance<IEmailService>();
       }
    }
}
