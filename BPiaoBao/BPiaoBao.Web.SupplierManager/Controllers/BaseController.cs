using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JoveZhao.Framework.Expand;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using JoveZhao.Framework;

namespace BPiaoBao.Web.SupplierManager.Controllers
{

    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Response.Cookies["token"] == null)
            {
                Response.Redirect("/Account/Login");
            }
            else
            {
                base.OnAuthorization(filterContext);
            }
        }


        public ActionResult Error()
        {
            return View();
        }
        public string GetJson(object data)
        {
            return data.ToJson();
        }
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new NewtonJsonResult()
            {
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 500;
                filterContext.ExceptionHandled = true;
                string errorMsg = filterContext.Exception.Message;
                filterContext.Result = Json(new { ErrorCode = filterContext.Exception.Source, ErrorMsg = errorMsg }, filterContext.Exception.Message);

            }
            else
            {
                filterContext.ExceptionHandled = true;
                var errorView =View("Error", (object)filterContext.Exception.Message);
                Logger.WriteLog(LogType.ERROR, filterContext.Exception.Message, filterContext.Exception);
                filterContext.Result = errorView;
            }
            base.OnException(filterContext);
        }

    }
    public class NewtonJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("不允许的请求");
            }
            HttpResponseBase response = context.HttpContext.Response;
            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "text/html";
            }
            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }
            if (this.Data != null)
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                response.Write(JsonConvert.SerializeObject(this.Data, Newtonsoft.Json.Formatting.Indented, timeFormat));
            }
        }
    }
}
