namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410241107 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AirChange",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QTDate = c.DateTime(nullable: false),
                        QTCount = c.Int(nullable: false),
                        BusinessmanName = c.String(maxLength: 100),
                        Code = c.String(maxLength: 20),
                        PNR = c.String(maxLength: 10),
                        CanPNR = c.Boolean(nullable: false),
                        OrderId = c.String(maxLength: 20),
                        CTCT = c.String(maxLength: 50),
                        PassengerName = c.String(maxLength: 200),
                        OfficeNum = c.String(maxLength: 20),
                        NotifyWay = c.Int(nullable: false),
                        QNContent = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AirChangeCoordion",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NotifyWay = c.Int(nullable: false),
                        ProcessStatus = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        OpertorName = c.String(),
                        AirChange_Id = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AirChange", t => t.AirChange_Id)
                .Index(t => t.AirChange_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AirChangeCoordion", "AirChange_Id", "dbo.AirChange");
            DropIndex("dbo.AirChangeCoordion", new[] { "AirChange_Id" });
            DropTable("dbo.AirChangeCoordion");
            DropTable("dbo.AirChange");
        }
    }
}
