using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.EFRepository.ContextStorage;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class InsuranceOrderRepository:BaseRepository<InsuranceOrder>,IInsuranceOrderRepository
    {
        public InsuranceOrderRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {
            //this.DbContext.
        }

        public IEnumerable<InsuranceSearchRecord> getAllSearchItem()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("select ");
            sql.AppendLine("ir.InsuranceOrder_Id as InsuranceOrderId--保险订单id");
            sql.AppendLine(",ir.Id as Id--保险记录主键id");
            sql.AppendLine(",io.OrderId as OrderId--机票订单号");
            //sql.AppendLine(",io.TradeId as TradeId--支付交易号");
            sql.AppendLine(",ir.SerialNum as SerialNum--流水号");
            sql.AppendLine(",ir.InsuranceStatus as EnumInsuranceStatus--保单状态");
            sql.AppendLine(",io.BuyTime as BuyTime--下单时间");
            sql.AppendLine(",ir.InsuranceNo as InsuranceNo--保险单号");
            sql.AppendLine(",ir.InsurancePrice as InsurancePrice--保险单价");
            sql.AppendLine(",ir.InsuranceLimitStartTime as InsuranceLimitStartTime--保险生效开始时间");
            sql.AppendLine(",ir.InsuranceLimitEndTime as InsuranceLimitEndTime--保险生效结束时间");
            sql.AppendLine(",ir.InsuranceCompany as InsuranceCompany--保险公司");
            sql.AppendLine(",ir.CarrierCode as CarrierCode--运营商Code");
            sql.AppendLine(",ir.CarrierName as CarrierName--运营商名称");
            sql.AppendLine(",ir.BussinessmanCode as BussinessmanCode--分销商Code");
            sql.AppendLine(",ir.BussinessmanName as BussinessmanName--分销商名称");
            sql.AppendLine(",ir.InsuredName as PassengerName--乘客名称");
            sql.AppendLine(",ir.Mobile as Mobile--手机");
            sql.AppendLine(",ir.CardNo as CardNo--证件号");
            sql.AppendLine(",ir.FromCityCode as FromCityCode--出发城市Code");
            sql.AppendLine(",ir.FromCityName as FromCity--出发城市名称");
            sql.AppendLine(",ir.ToCityCode as ToCityCode--到达城市Code");
            sql.AppendLine(",ir.ToCityName as ToCity--到达城市名称");
            sql.AppendLine(",ir.StartDateTime as StartDateTime--起飞时间");
            sql.AppendLine(",ir.ToDateTime as ToDateTime--到达时间");
            sql.AppendLine(",ir.IdType as IdType--证件类型");
            sql.AppendLine(",ir.FlightNumber as FlightNumber--航班号");
            sql.AppendLine(",ir.PNR as PNR--PNR");
            sql.AppendLine(",ir.SexType as SexType--性别类型");
            sql.AppendLine(",ir.Birth as Birth--生日");
            sql.AppendLine(",ir.PassengerType as PassengerType--乘客类型");
            sql.AppendLine(",ir.PolicyAmount as PolicyAmount--保额");
            sql.AppendLine(",ir.InsureType as InsureType--投保方式");
            sql.AppendLine("from InsuranceRecord as ir");
            sql.AppendLine("left join InsuranceOrder as io on ir.InsuranceOrder_Id=io.Id");
            sql.AppendLine("where ir.InsuranceOrder_Id is not null");

            return this.DbContext.Database.SqlQuery<InsuranceSearchRecord>(sql.ToString(), new object[] { });
        }
    }
}
