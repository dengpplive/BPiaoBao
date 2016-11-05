using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    public class BusinessmanBuilder : IAggregationBuilder
    {
        
        /// <summary>
        /// 运营商创建初始化
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        public Carrier CreateCarrier(Carrier carrier = null)
        {
            if (carrier == null)
                carrier = new Carrier();
            if (carrier.Attachments == null)
                carrier.Attachments = new List<Attachment>();
            carrier.Operators = new List<Operator>()
            {
                new Operator{
                Account="admin",
                CreateDate=DateTime.Now,
                IsAdmin=true,
                OperatorState=EnumOperatorState.Normal,
                Password="123456".Md5(),
                Phone=carrier.ContactWay.Tel,
                Realname=carrier.ContactWay.Contact
                }
            };
            carrier.SMS = new SMS.SMS();
            carrier.IsEnable = true;
            carrier.CreateTime = DateTime.Now;
            carrier.NormalWork = new WorkBusinessman
            {
                WeekDay = "1,2,3,4,5",
                WorkOnLineTime = "09:00",
                WorkUnLineTime = "18:00",
                ServiceOnLineTime = "09:00",
                ServiceUnLineTime = "18:00"
            };
            carrier.RestWork = new WorkBusinessman
            {
                WeekDay = "6,7",
                WorkOnLineTime = "09:00",
                WorkUnLineTime = "18:00",
                ServiceOnLineTime = "09:00",
                ServiceUnLineTime = "18:00"
            };
            return carrier;
        }
        /// <summary>
        /// 采购商初始化
        /// </summary>
        /// <param name="buyer"></param>
        /// <returns></returns>
        public Buyer CreateBuyer(Buyer buyer = null)
        {
            if (buyer == null)
                buyer = new Buyer();
            if (buyer.Attachments == null)
                buyer.Attachments = new List<Attachment>();
            buyer.Operators = new List<Operator>()
            {
                new Operator{
                Account="admin",
                CreateDate=DateTime.Now,
                IsAdmin=true,
                OperatorState=EnumOperatorState.Normal,
                Password="123456".Md5(),
                Phone=buyer.ContactWay.Tel,
                Realname=buyer.ContactWay.Contact
                }
            };
            buyer.SMS = new SMS.SMS();
            buyer.IsEnable = true;
            buyer.CreateTime = DateTime.Now;
            return buyer;
        }
        /// <summary>
        /// 供应商初始化
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public Supplier CreateSupplier(Supplier supplier = null)
        {
            if (supplier == null)
                supplier = new Supplier();
            if (supplier.Attachments == null)
                supplier.Attachments = new List<Attachment>();
            supplier.Operators = new List<Operator>()
            {
                new Operator{
                Account="admin",
                CreateDate=DateTime.Now,
                IsAdmin=true,
                OperatorState=EnumOperatorState.Normal,
                Password="123456".Md5(),
                Phone=supplier.ContactWay.Tel,
                Realname=supplier.ContactWay.Contact
                }
            };
            supplier.SMS = new SMS.SMS();
            supplier.IsEnable = true;
            supplier.CreateTime = DateTime.Now;
            supplier.SupNormalWork = new WorkBusinessman
            {
                WeekDay = "1,2,3,4,5",
                WorkOnLineTime = "09:00",
                WorkUnLineTime = "18:00",
                ServiceOnLineTime = "09:00",
                ServiceUnLineTime = "18:00"
            };
            supplier.SupRestWork = new WorkBusinessman
            {
                WeekDay = "6,7",
                WorkOnLineTime = "09:00",
                WorkUnLineTime = "18:00",
                ServiceOnLineTime = "09:00",
                ServiceUnLineTime = "18:00"
            };
            return supplier;
        }
    }
}
