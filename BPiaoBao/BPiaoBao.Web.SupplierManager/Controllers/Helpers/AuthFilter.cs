using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers.Helpers
{
    public class AuthFilterAttribute : AuthorizeAttribute
    {
        public AuthFilterAttribute(string code)
        {
            this.Code = code;
        }
        public string Code { get; private set; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (Code != "10")
                throw new Exception("没有权限");
        }
    }
}