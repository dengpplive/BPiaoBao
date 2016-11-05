using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using PnrAnalysis;
namespace BPiaoBao.Web.SupplierManager.Controllers
{
    /// <summary>
    /// 黑屏查看
    /// </summary>
    public class BlackQueryController : Controller
    {
        public ActionResult BlackSendData()
        {
            List<CarrierDataObject> CarrierList = null;
            var msg = new RspMessageModel();
            CommunicateManager.Invoke<IConsoBusinessmanService>(service =>
            {
                CarrierList = service.GetAllCarrier();
                msg.Success = 1;
                msg.Message = "成功";
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            Dictionary<string, string> dicList = new Dictionary<string, string>();
            foreach (CarrierDataObject item in CarrierList)
            {
                List<string> pids = new List<string>();
                List<string> Office = new List<string>();
                item.Pids.ForEach(p =>
                {
                    pids.Add(p.IP + ":" + p.Port);
                    Office.Add(p.Office);
                });
                dicList.Add(item.Code + "-" + item.Name, string.Join("^", pids.ToArray()) + "#" + string.Join("|", Office.ToArray()));
            }
            return View(dicList);
        }
        //
        // GET: /BlackQuery/
        /// <summary>
        /// 发送指令数据 暂时 比较简单 用着
        /// </summary>
        /// <param name="TripNumber"></param>
        /// <returns></returns>
        public JsonResult GetBlackData(string Cmd, string IP, string Port, string Office)
        {
            var msg = new RspMessageModel();
            PidParam pidParam = new PidParam();
            pidParam.ServerIP = IP;
            pidParam.ServerPort = Port;
            pidParam.Office = Office;
            pidParam.Cmd = Cmd;
            CommunicateManager.Invoke<IPidService>(service =>
            {
                msg.Message = service.SendPid(pidParam);
                msg.Success = 1;
            }, (p =>
            {
                msg.Success = 0;
                msg.Message = p.Message;
            }));
            return Json(new { msg }, JsonRequestBehavior.AllowGet);
        }
    }
}
