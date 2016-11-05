using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
  public static  class TokenBuilderFactory
    {
      public static ITokenBuilder CreateTokenBuilder()
      {
          return new DefaultTokenBuilder();
      }
    }
}
