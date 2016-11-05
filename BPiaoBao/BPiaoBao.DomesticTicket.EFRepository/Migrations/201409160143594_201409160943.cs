namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201409160943 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ticket",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrderID = c.String(maxLength: 20),
                        CurrentOrderID = c.String(),
                        TicketNum = c.String(),
                        TicketState = c.String(maxLength: 10),
                        StartTime = c.String(),
                        FlightNum = c.String(maxLength: 50),
                        Voyage = c.String(maxLength: 50),
                        Seat = c.String(maxLength: 10),
                        PassengerName = c.String(maxLength: 50),
                        PNR = c.String(maxLength: 10),
                        BigCode = c.String(),
                        SeatPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ABFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RQFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CarryCode = c.String(),
                        PMFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RetirementPoundage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TicketConso",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Code = c.String(maxLength: 20),
                        PolicyFrom = c.String(maxLength: 20),
                        IssueTicketCode = c.String(maxLength: 20),
                        CarrierCode = c.String(maxLength: 20),
                        PaidMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.TicketCarrier",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        Code = c.String(maxLength: 20),
                        PolicyFrom = c.String(maxLength: 20),
                        IssueTicketCode = c.String(maxLength: 20),
                        PolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Point = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.TicketSupplier",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        PolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.ID)
                .Index(t => t.ID);
            
            CreateTable(
                "dbo.TicketBuyer",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        PayMethod = c.String(),
                        PolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CommissionMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Ticket", t => t.ID)
                .Index(t => t.ID);

            AlterColumn("dbo.LocalPolicy", "SpecialType", c => c.Int());
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketBuyer", "ID", "dbo.Ticket");
            DropForeignKey("dbo.TicketSupplier", "ID", "dbo.Ticket");
            DropForeignKey("dbo.TicketCarrier", "ID", "dbo.Ticket");
            DropForeignKey("dbo.TicketConso", "ID", "dbo.Ticket");
            DropIndex("dbo.TicketBuyer", new[] { "ID" });
            DropIndex("dbo.TicketSupplier", new[] { "ID" });
            DropIndex("dbo.TicketCarrier", new[] { "ID" });
            DropIndex("dbo.TicketConso", new[] { "ID" });
            DropTable("dbo.TicketBuyer");
            DropTable("dbo.TicketSupplier");
            DropTable("dbo.TicketCarrier");
            DropTable("dbo.TicketConso");
            DropTable("dbo.Ticket");
        }
    }
}
