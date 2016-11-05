using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.HttpServers;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
namespace BPiaoBao.AppServices.Hosting.Notify
{
    [HttpCode("Platform")]
    public class PlatformHttpHandler : IHttpHandler
    {
        public void Process(HttpRequest request, HttpResponse response)
        {

            var path = request.Url.AbsolutePath;
            var ps = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string areaCity = "";
            if (ps.Length > 1)
                areaCity = ps[1];
            NameValueCollection nv = new NameValueCollection();
            nv.Add(request.Form);
            nv.Add(request.QueryString);
            nv.Add("areaCity", areaCity);
            if (request.QueryString.Count == 1)
                nv.Add("json", request.QueryString[0]);


            NotifyObserver notify = new NotifyObserver();
            notify.OnCancelIssue += notify_OnCancelIssue;
            notify.OnIssue += notify_OnIssue;
            notify.OnPaid += notify_OnPaid;
            notify.OnRefund += notify_OnRefund;
            notify.OnRefundTicket += notify_OnRefundTicket;
            var result = notify.Process(nv);
            response.WriteLine(result);

        }
        //退废通知
        private void notify_OnRefundTicket(object sender, RefundTicketArgs args)
        {
            //IStationOrderService server = ObjectFactory.GetInstance<OrderService>();
            //server.PlatformCancelIssueNotify(args.OutOrderId, args.Ramark);
        }
        //取消出票通知
        void notify_OnCancelIssue(object sender, CancelIssueEventArgs args)
        {
            IStationOrderService server = ObjectFactory.GetInstance<OrderService>();
            server.PlatformCancelIssueNotify(args.OutOrderId, args.Ramark);
        }
        //平台退款通知处理
        void notify_OnRefund(object sender, PlatformRefundEventArgs args)
        {
            IStationOrderService server = ObjectFactory.GetInstance<OrderService>();
            server.PlatformRefundNotify(args.OutOrderId, args.RefundMoney, args.RefundRemark);
        }
        //代付通知处理
        void notify_OnPaid(object sender, PaidEventArgs args)
        {
            IStationOrderService server = ObjectFactory.GetInstance<OrderService>();
            server.UpdateOrderByPayNotify(args.OutOrderId, args.PlatformCode, args.SerialNumber, args.PaidMeony);
        }
        //出票通知处理
        void notify_OnIssue(object sender, IssueEventArgs args)
        {
            IStationOrderService server = ObjectFactory.GetInstance<OrderService>();
            IList<PassengerTicketDto> PassengerTicketDtoList = new List<PassengerTicketDto>();
            foreach (string passengerName in args.TicketInfo.AllKeys)
            {
                PassengerTicketDtoList.Add(new PassengerTicketDto()
                {
                    Name = passengerName,
                    TicketNumber = args.TicketInfo[passengerName]
                });
            }
            server.IssueTicket(args.OutOrderId, PassengerTicketDtoList, "");
        }

    }
}
