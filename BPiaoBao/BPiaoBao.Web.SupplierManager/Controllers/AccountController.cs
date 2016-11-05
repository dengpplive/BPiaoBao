using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.Common.DESCrypt;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class AccountController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                string errorMsg= filterContext.Exception.Message;
                filterContext.Result = Json(new { ErrorCode = filterContext.Exception.Source, ErrorMsg = errorMsg }, filterContext.Exception.Message);
            }
            else
            {
                filterContext.ExceptionHandled = true;
                var errorView = View("Error", (object)filterContext.Exception.Message);

                filterContext.Result = errorView;
            }
            base.OnException(filterContext);
        }
        public ActionResult Login()
        {
            LoginModel mode = new LoginModel();
            if (Request.Cookies["User"] != null)
            {
                mode.Code = Request.Cookies["User"].Values["code"];
                mode.UserName = Request.Cookies["User"].Values["account"];
                mode.Password = Request.Cookies["User"].Values["password"];
                mode.RememberMe = Request.Cookies["User"].Values["remember"].ToBool();
            };
            return View(mode);
        }
        [HttpPost]
        public JsonResult Login(LoginModel login)
        {
            CommunicateManager.Invoke<IConsoLoginService>(p =>
            {
                string token = p.ConsoLogin(login.Code, login.UserName,login.Password, Request.Url.Host);
                var cookie = new HttpCookie("auth");
                cookie.Values["token"] = token;
                cookie.Expires = DateTime.Now.AddMonths(1);
               
                Response.Cookies.Add(cookie);
                if (login.RememberMe)
                {
                    var cookieUser = new HttpCookie("User");
                    cookieUser.Expires = DateTime.Now.AddMonths(1);
                    cookieUser.Values["code"] = login.Code;
                    cookieUser.Values["password"] = login.Password;
                    cookieUser.Values["account"] = login.UserName.Trim();
                    cookieUser.Values["remember"] = login.RememberMe.ToString();

                    Response.Cookies.Add(cookieUser);
                }
                else
                {
                     var cookieUser = new HttpCookie("User");
                    cookieUser.Expires = DateTime.Now.AddDays(-1);


                    Response.Cookies.Add(cookieUser);
                }

            });
            return Json(new { Url = Url.Action("Index", "Home") }, JsonRequestBehavior.DenyGet);
        }
        public ActionResult Exit()
        {
            CommunicateManager.Invoke<IConsoLoginService>(p =>
            {
                string token = User.Identity.Name;
                p.Exist(token);
            });
            return RedirectToAction("Login", "Account");
        }

    }
}
