using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.HttpServers;
using JoveZhao.Framework.Expand;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.Hosting.Notify
{
    [HttpCode("SmsPay")]
    public class SmsNotifyHttpHandler : IHttpHandler
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        IBusinessmanRepository businessmanRepository = ObjectFactory.GetInstance<IBusinessmanRepository>();
        IAfterSaleOrderRepository afterSaleOrderRepository = ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
        public void Process(HttpRequest request, HttpResponse response)
        {

            Logger.WriteLog(LogType.INFO, request.Url.ToString());
            string orderid = request.QueryString["orderId"];
            string outPayNo=request.QueryString["payNo"];
            string remark=request.QueryString["remark"];
            string payway = request.QueryString["payWay"];
            var model = businessmanRepository.FindAll(x => x.Code == remark).FirstOrDefault();
            if (model == null)
                return;
            var buyModel = model.BuyDetails.Where(p => p.PayNo == orderid).FirstOrDefault();
            if (buyModel != null && string.IsNullOrEmpty(buyModel.OutPayNo))
            {
                model.SMSNotify(orderid, outPayNo,ExtHelper.GetPayMethod(payway));
                unitOfWorkRepository.PersistUpdateOf(model);
                unitOfWork.Commit();
            }
            response.WriteLine("success");

            if (string.Equals(remark, "BankOrPlatform", StringComparison.OrdinalIgnoreCase))
            { 
                
            }

        }
        
    }
}
