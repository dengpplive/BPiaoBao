namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411121024 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlatformPointGroupRule",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AirCode = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        FromCityCodes = c.String(),
                        ToCityCodes = c.String(),
                        IssueTicketCode = c.String(),
                        AdjustType = c.Int(nullable: false),
                        PlatformPointGroupID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PlatformPointGroup", t => t.PlatformPointGroupID, cascadeDelete: true)
                .Index(t => t.PlatformPointGroupID);
            
            CreateTable(
                "dbo.PlatformPointGroupDetailRule",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StartPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EndPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Point = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlatformPointGroupRule_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PlatformPointGroupRule", t => t.PlatformPointGroupRule_ID)
                .Index(t => t.PlatformPointGroupRule_ID);
            
            CreateTable(
                "dbo.PlatformPointGroup",
                c => new
                    {
                        ID = c.Guid(nullable: false, identity: true),
                        GroupName = c.String(maxLength: 50),
                        DefaultPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsMax = c.Boolean(nullable: false),
                        MaxPoint = c.Decimal(precision: 18, scale: 2),
                        Remark = c.String(maxLength: 200),
                        OperatorAccount = c.String(maxLength: 20),
                        CreateDate = c.DateTime(nullable: false),
                        Code = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Policy", "OriginalPolicyPoint", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AirChange", "CarrayName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlatformPointGroupRule", "PlatformPointGroupID", "dbo.PlatformPointGroup");
            DropForeignKey("dbo.PlatformPointGroupDetailRule", "PlatformPointGroupRule_ID", "dbo.PlatformPointGroupRule");
            DropIndex("dbo.PlatformPointGroupRule", new[] { "PlatformPointGroupID" });
            DropIndex("dbo.PlatformPointGroupDetailRule", new[] { "PlatformPointGroupRule_ID" });
            DropColumn("dbo.AirChange", "CarrayName");
            DropColumn("dbo.Policy", "OriginalPolicyPoint");
            DropTable("dbo.PlatformPointGroup");
            DropTable("dbo.PlatformPointGroupDetailRule");
            DropTable("dbo.PlatformPointGroupRule");
        }
    }
}
