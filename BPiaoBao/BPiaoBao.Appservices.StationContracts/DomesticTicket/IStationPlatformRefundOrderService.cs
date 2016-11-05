using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.DomesticTicket
{
     [ServiceContract]
  public  interface IStationPlatformRefundOrderService
    {
         [OperationContract]
         DataPack<ResponsePlatformRefundOrder> GetList(EnumPlatformRefundType? RefundType, string orderId, int startIndex, int count);

         [OperationContract]
         void ConfirmRefund(int Id,decimal amount, DateTime refundTime);
         
    }
}
