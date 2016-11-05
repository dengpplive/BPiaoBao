using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
   public class AuthInfoStroageFactory
    {
       public static IAuthInfoStroage CreateAuthInfoStroage()
       {
           return new MemAuthInfoStroage();
       }
    }
}
