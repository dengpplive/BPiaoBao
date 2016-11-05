using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class LoginModel
    {
        public string Code { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}