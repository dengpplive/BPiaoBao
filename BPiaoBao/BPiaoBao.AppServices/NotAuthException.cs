using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;

namespace BPiaoBao.AppServices
{
  public  class NotAuthException : CustomException
    {
      public NotAuthException(string message):base(403,message)
      {

      }
    }
}
