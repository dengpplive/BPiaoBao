using AutoMapper;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using BPiaoBao.DomesticTicket.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public static class RefundOrderMapper
    {
        public static ResponsePlatformRefundOrder ToResponseRefundOrder(this PlatformRefundOrder order)
        {
            return Mapper.Map<PlatformRefundOrder, ResponsePlatformRefundOrder>(order);
        }
    }
}
