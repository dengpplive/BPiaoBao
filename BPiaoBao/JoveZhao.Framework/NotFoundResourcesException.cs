using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework
{
    public class NotFoundResourcesException:CustomException
    {
        public NotFoundResourcesException(string message) : base(500, message) { }      
    }
}
