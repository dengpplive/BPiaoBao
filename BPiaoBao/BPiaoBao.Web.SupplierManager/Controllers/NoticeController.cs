using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Web.SupplierManager.Controllers.Helpers;
using BPiaoBao.Web.SupplierManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BPiaoBao.Web.SupplierManager.Controllers
{
    public class NoticeController : BaseController
    {
        //
        // GET: /Notice/

        public ActionResult NoticeIndex()
        {
            return View();
        }
        public ActionResult NoticeList()
        {
            return View();
        }
         /// <summary>
        /// 公告详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult NoticeDetail(int Id)
        {
            NoticeDataObj notice = null;
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
            {
                notice = p.FindConsoNoticeById(Id);
            });
            return View(notice);
        }
        /// <summary>
        /// 获取自己发布公告列表
        /// </summary>
        /// <param name="queryCond"></param>
        /// <returns></returns>
        public JsonResult GetNoticeList(NoticeQueryEntity queryCond)
        {
            PagedList<NoticeDataObj> list = null;
            if (queryCond.EndTime.HasValue)
            {
                queryCond.EndTime = queryCond.EndTime.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
            {
                list = p.FindNotice(queryCond.Title, queryCond.State, queryCond.StartTime, queryCond.EndTime, (queryCond.page - 1) * queryCond.rows, queryCond.rows); 
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            });     
        }
        /// <summary>
        /// 获取控台发布公告列表
        /// </summary>
        /// <param name="queryCond"></param>
        /// <returns></returns>
        public JsonResult GetIndustryNoticeList(NoticeQueryEntity queryCond)
        {
            PagedList<NoticeDataObj> list = null;
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
            {
                list = p.FindIndustyNotice(queryCond.Title,(queryCond.page - 1) * queryCond.rows, queryCond.rows);
            });
            return Json(new
            {
                total = list.Total,
                rows = list.Rows
            });
        }
       
        /// <summary>
        /// 发布公告
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public JsonResult AddNotice(RequestNoticeDto notice)
        {
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
          {
              notice.NoticeShowType = "0,2";
              notice.NoticeType = notice.NoticeType == null ? "" : notice.NoticeType;
              notice.NoticeAttachments = notice.NoticeAttachments == null ? new List<NoticeAttachmentDataDto>() : notice.NoticeAttachments;
              p.AddNotice(notice);
          });
            return Json(true);
        }
        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetNoticeinfoById(int Id)
        {
            NoticeDataObj notice = null;
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
            {
                notice = p.FindConsoNoticeById(Id);
            });
            return Json(notice);
        }
        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetNoticeAttachMentinfoById(int Id)
        {
            List<NoticeAttachmentDataDto> notice = null;
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
            {
                notice = p.FindConsoNoticeById(Id).NoticeAttachments;
            });
            return Json(notice);
        }
        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="notice"></param>
        /// <returns></returns>
        public JsonResult ModifyNotice(NoticeDataObj notice)
        {
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
            {
                notice.NoticeShowType = "0";
                notice.NoticeType = notice.NoticeType == null ? "" : notice.NoticeType;
                notice.NoticeAttachments = notice.NoticeAttachments == null ? new List<NoticeAttachmentDataDto>() : notice.NoticeAttachments;
                p.ModifyNotice(notice);
            });
            return Json(true);
        }
        /// <summary>
        /// 禁用启用
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult NoticeEnableOrDisable(int Id)
        {
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
           {
               p.NoticeEnableOrDisable(Id);
           });
            return Json(true);
        }
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult DeleteNotice(int Id)
        {
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
           {
               p.DeleteNotice(Id);
           });
            return Json(true);
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="NoticeId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult DeleteNoticeAttachMent(int NoticeId, int Id)
        {
            CommunicateManager.Invoke<IConsoNoticeService>(p =>
           {
               p.DeleteAttachMent(NoticeId, Id);
           });
            return Json(true);
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="Filedata"></param>
        /// <returns></returns>
        public JsonResult FileUpload(HttpPostedFileBase Filedata)
        {
            if (Filedata != null)
            {
                try
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileUpload");
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    string fileName = Path.GetFileName(Filedata.FileName);
                    string fileExtension = Path.GetExtension(fileName);
                    string saveName = Guid.NewGuid().ToString() + fileExtension;//保存文件名称
                    Filedata.SaveAs(Path.Combine(filePath, saveName));
                    return Json(new { Success = true, Url = Url.Content("http://" + HttpContext.Request.Url.Host + ":" + HttpContext.Request.Url.Port + "/FileUpload/" + saveName) });
                }
                catch (Exception e)
                {
                    return Json(new { Success = false, Message = e.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { Success = false, Message = "请选择要上传的文件!" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult FileDown(string fileName,string filePath)
        {
            //string fileName = "aaa.txt";//客户端保存的文件名
            //filePath = Server.MapPath("");//路径

            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment;  filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
            return Json("");
        }

    }
}
