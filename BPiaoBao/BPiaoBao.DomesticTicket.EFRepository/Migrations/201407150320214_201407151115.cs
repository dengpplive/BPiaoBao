namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _201407151115 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InsuranceConfig",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveCount = c.Int(nullable: false),
                        SinglePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsOpen = c.Boolean(nullable: false),
                        BusinessmanCode = c.String(maxLength: 20),
                        BusinessmanName = c.String(maxLength: 50),
                        ConfigType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.InsuranceDepositLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PayNo = c.String(maxLength: 50),
                        OutTradeNo = c.String(maxLength: 50),
                        BeforeLeaveCount = c.Int(nullable: false),
                        AfterLeaveCount = c.Int(nullable: false),
                        DepositCount = c.Int(nullable: false),
                        BuyTime = c.DateTime(nullable: false),
                        SinglePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BusinessmanCode = c.String(maxLength: 20),
                        BusinessmanName = c.String(maxLength: 50),
                        PayWay = c.Int(nullable: false),
                        BuyState = c.Int(nullable: false),
                        RecordType = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.InsuranceOrder",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.String(maxLength: 20),
                        TradeId = c.String(maxLength: 50),
                        PayNo = c.String(maxLength: 50),
                        BuyTime = c.DateTime(),
                        EnumInsuranceStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.InsuranceRecord",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerialNum = c.String(),
                        InsuranceNo = c.String(maxLength: 50),
                        InsurancePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InsuranceLimitStartTime = c.DateTime(),
                        InsuranceLimitEndTime = c.DateTime(),
                        InsuranceCompany = c.String(maxLength: 100),
                        Count = c.Int(nullable: false),
                        CarrierCode = c.String(maxLength: 20),
                        CarrierName = c.String(maxLength: 50),
                        BussinessmanCode = c.String(maxLength: 20),
                        BussinessmanName = c.String(maxLength: 50),
                        Passenger_Id = c.Int(),
                        SkyWay_Id = c.Int(),
                        InsuranceOrder_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Passenger", t => t.Passenger_Id)
                .ForeignKey("dbo.SkyWay", t => t.SkyWay_Id)
                .ForeignKey("dbo.InsuranceOrder", t => t.InsuranceOrder_Id)
                .Index(t => t.Passenger_Id)
                .Index(t => t.SkyWay_Id)
                .Index(t => t.InsuranceOrder_Id);

            CreateTable(
                "dbo.InsurancePurchaseByBussinessman",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PayNo = c.String(maxLength: 50),
                        OutTradeNo = c.String(maxLength: 50),
                        BeforeLeaveCount = c.Int(nullable: false),
                        AfterLeaveCount = c.Int(nullable: false),
                        DepositCount = c.Int(nullable: false),
                        BuyTime = c.DateTime(nullable: false),
                        SinglePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BusinessmanCode = c.String(maxLength: 20),
                        BusinessmanName = c.String(maxLength: 50),
                        CarrierCode = c.String(maxLength: 20),
                        CarrierName = c.String(maxLength: 50),
                        PayWay = c.Int(nullable: false),
                        BuyState = c.Int(nullable: false),
                        RecordType = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.LocalPolicy",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        Code = c.String(maxLength: 20),
                        RoleType = c.String(),
                        ReleaseType = c.Int(nullable: false),
                        FromCityCodes = c.String(),
                        ToCityCodes = c.String(),
                        LocalPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Different = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TravelType = c.Int(nullable: false),
                        LocalPolicyType = c.String(),
                        Low = c.Boolean(nullable: false),
                        ChangeCode = c.Boolean(nullable: false),
                        Office = c.String(),
                        WeekLimit = c.String(),
                        Share = c.Boolean(nullable: false),
                        CarrayCode = c.String(),
                        Seats = c.String(),
                        PassengeDate_StartTime = c.DateTime(nullable: false),
                        PassengeDate_EndTime = c.DateTime(nullable: false),
                        IssueDate_StartTime = c.DateTime(nullable: false),
                        IssueDate_EndTime = c.DateTime(nullable: false),
                        IssueTicketWay = c.Int(nullable: false),
                        Apply = c.Int(nullable: false),
                        ApplyFlights = c.String(),
                        Remark = c.String(),
                        Review = c.Boolean(nullable: false),
                        HangUp = c.Boolean(nullable: false),
                        CreateMan = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        CarrierCode = c.String(),
                    })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.TicketDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TicketSum_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TicketSum", t => t.TicketSum_ID)
                .Index(t => t.TicketSum_ID);

            AddColumn("dbo.Order", "OldOrderId", c => c.String());
            AddColumn("dbo.Order", "OrderSource", c => c.Int(nullable: false));
            AddColumn("dbo.Order", "HaveBabyFlag", c => c.Boolean(nullable: false));
            AddColumn("dbo.Order", "IsChangePnrTicket", c => c.Boolean(nullable: false));
            AddColumn("dbo.Order", "AssocChdCount", c => c.Int(nullable: false));
            AddColumn("dbo.Order", "AutoEtdzCallCount", c => c.Int());
            AddColumn("dbo.Order", "B2bFailLastCallMethod", c => c.String());
            AddColumn("dbo.PayBillDetail", "InfMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Passenger", "IdType", c => c.Int(nullable: false));
            AddColumn("dbo.Passenger", "SexType", c => c.Int(nullable: false));
            AddColumn("dbo.Passenger", "BuyInsuranceCount", c => c.Int(nullable: false));
            AddColumn("dbo.Passenger", "BuyInsurancePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Passenger", "IsInsuranceRefund", c => c.Boolean(nullable: false));
            AddColumn("dbo.Passenger", "InsuranceRefunrPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Policy", "EnumIssueTicketWay", c => c.Int(nullable: false));
            AddColumn("dbo.Policy", "PolicyOwnUserRole", c => c.Int(nullable: false));
            AddColumn("dbo.Policy", "IsLow", c => c.Boolean(nullable: false));
            AddColumn("dbo.Policy", "SeatPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Policy", "ABFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Policy", "RQFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AfterSalePassenger", "PassengerTripStatus", c => c.Int(nullable: false));
            AddColumn("dbo.AfterSalePassenger", "AfterSaleTravelNum", c => c.String());
            AddColumn("dbo.AfterSalePassenger", "AfterSaleTravelTicketNum", c => c.String());
            AddColumn("dbo.FrePasser", "SexType", c => c.String());
            AddColumn("dbo.TicketSum", "IssueTicketCode", c => c.String());
            AddColumn("dbo.TicketSum", "RetirementPoundage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TravelPaper", "PassengerId", c => c.Int(nullable: false));
            AddColumn("dbo.TravelPaper", "OrderId", c => c.String());
            AlterColumn("dbo.Policy", "Rate", c => c.Decimal(nullable: false, precision: 18, scale: 3));
            DropColumn("dbo.TicketSum", "SupplierCode");
            DropColumn("dbo.TicketSum", "CarrierMoney");
            DropColumn("dbo.TicketSum", "ParentMoney");
        }

        public override void Down()
        {
            AddColumn("dbo.TicketSum", "ParentMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TicketSum", "CarrierMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TicketSum", "SupplierCode", c => c.String());
            DropForeignKey("dbo.TicketDetail", "TicketSum_ID", "dbo.TicketSum");
            DropForeignKey("dbo.InsuranceRecord", "InsuranceOrder_Id", "dbo.InsuranceOrder");
            DropForeignKey("dbo.InsuranceRecord", "SkyWay_Id", "dbo.SkyWay");
            DropForeignKey("dbo.InsuranceRecord", "Passenger_Id", "dbo.Passenger");
            DropIndex("dbo.TicketDetail", new[] { "TicketSum_ID" });
            DropIndex("dbo.InsuranceRecord", new[] { "InsuranceOrder_Id" });
            DropIndex("dbo.InsuranceRecord", new[] { "SkyWay_Id" });
            DropIndex("dbo.InsuranceRecord", new[] { "Passenger_Id" });
            AlterColumn("dbo.Policy", "Rate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.TravelPaper", "OrderId");
            DropColumn("dbo.TravelPaper", "PassengerId");
            DropColumn("dbo.TicketSum", "RetirementPoundage");
            DropColumn("dbo.TicketSum", "IssueTicketCode");
            DropColumn("dbo.FrePasser", "SexType");
            DropColumn("dbo.AfterSalePassenger", "AfterSaleTravelTicketNum");
            DropColumn("dbo.AfterSalePassenger", "AfterSaleTravelNum");
            DropColumn("dbo.AfterSalePassenger", "PassengerTripStatus");
            DropColumn("dbo.Policy", "RQFee");
            DropColumn("dbo.Policy", "ABFee");
            DropColumn("dbo.Policy", "SeatPrice");
            DropColumn("dbo.Policy", "IsLow");
            DropColumn("dbo.Policy", "PolicyOwnUserRole");
            DropColumn("dbo.Policy", "EnumIssueTicketWay");
            DropColumn("dbo.Passenger", "InsuranceRefunrPrice");
            DropColumn("dbo.Passenger", "IsInsuranceRefund");
            DropColumn("dbo.Passenger", "BuyInsurancePrice");
            DropColumn("dbo.Passenger", "BuyInsuranceCount");
            DropColumn("dbo.Passenger", "SexType");
            DropColumn("dbo.Passenger", "IdType");
            DropColumn("dbo.PayBillDetail", "InfMoney");
            DropColumn("dbo.Order", "B2bFailLastCallMethod");
            DropColumn("dbo.Order", "AutoEtdzCallCount");
            DropColumn("dbo.Order", "AssocChdCount");
            DropColumn("dbo.Order", "IsChangePnrTicket");
            DropColumn("dbo.Order", "HaveBabyFlag");
            DropColumn("dbo.Order", "OrderSource");
            DropColumn("dbo.Order", "OldOrderId");
            DropTable("dbo.TicketDetail");
            DropTable("dbo.LocalPolicy");
            DropTable("dbo.InsurancePurchaseByBussinessman");
            DropTable("dbo.InsuranceRecord");
            DropTable("dbo.InsuranceOrder");
            DropTable("dbo.InsuranceDepositLog");
            DropTable("dbo.InsuranceConfig");
        }
    }
}
