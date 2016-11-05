using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var list = MenuHelper.GetMenu();
            CurrentUserInfo currentUser = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                currentUser = p.GetCurrentUser();
            });
            if (currentUser == null)
                currentUser = new CurrentUserInfo();
            int Share = 0;
            if (currentUser.Type == "Supplier")
                Share = 2;
            else
                Share = 1;
            Tuple<bool, string> value = null;
            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                value = p.GetMentList(currentUser.OperatorAccount) ?? new Tuple<bool, string>(false, "");
            });
            //第一个元素代表TopModule 第二元素代表LeftModule
            Tuple<List<int>, List<int>> mentList = new Tuple<List<int>, List<int>>(new List<int>(), new List<int>());

            var array =  value.Item2 ==null? new List<string>():value.Item2.Split(',').Where(p => p.Length == 6).ToList();
            array.ForEach(p =>
            {
                int sencond = int.Parse(p.Substring(0, 2));
                int four = int.Parse(p.Substring(0, 4));
                if (!mentList.Item1.Contains(sencond))
                    mentList.Item1.Add(sencond);
                if (!mentList.Item2.Contains(four))
                    mentList.Item2.Add(four);
            });
            var model = new
                  {
                      topmodule = list.Where(p => (p.Share == 0 || p.Share == Share) && (value.Item1 || mentList.Item1.Contains(p.Code))).Select(p => new
                      {
                          Code = p.Code,
                          Name = p.Name,
                          Icon = p.Icon
                      }),
                      leftmenu = list.Where(y => (y.Share == 0 || y.Share == Share) && (value.Item1 || mentList.Item1.Contains(y.Code))).Select(p => new
                      {
                          Code = p.Code,
                          Name = p.Name,
                          Menus = p.Menus.Where(z => (z.Share == 0 || z.Share == Share) && (value.Item1 || mentList.Item2.Contains(z.Code))).Select(x => new
                          {
                              Code = x.Code,
                              Url = x.Url,
                              Name = x.Name,
                              Menus = x.ItemMenus.Where(pp => (pp.Share == 0 || pp.Share == Share) && (value.Item1 || array.Contains(pp.Code.ToString()))).Select(cc => new
                              {
                                  Code = cc.Code,
                                  Url = cc.Url,
                                  Name = cc.Name
                              })
                          })
                      }),
                      selected = list.FirstOrDefault(m => (m.Share == 0 || m.Share == Share) && (value.Item1 || mentList.Item1.Contains(m.Code))).Code,
                      Businessman = new
                      {
                          BusinessmanName = currentUser.BusinessmanName,
                          OperatorName = currentUser.OperatorName,
                          Code = currentUser.BusinessmanCode
                      }
                  };
                return View(model);
        }
        [HttpPost]
        public JsonResult ChangePwd(string oldPwd, string newPwd)
        {

            CommunicateManager.Invoke<IConsoBusinessmanService>(p =>
            {
                p.ModifyPassword(newPwd, oldPwd);
            });
            return Json(null, JsonRequestBehavior.DenyGet);
        }
        public ActionResult Exit()
        {
            CommunicateManager.Invoke<IConsoLoginService>(p =>
            {
                p.Exist(Request.Cookies["auth"].Values["token"]);
            });
            return RedirectToAction("Login", "Account");//
        }
    }
}
