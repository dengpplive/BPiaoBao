namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407151112 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GiveDetail",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GiveCode = c.String(maxLength: 20),
                        GiveName = c.String(maxLength: 100),
                        GiveCount = c.Int(nullable: false),
                        GiveTime = c.DateTime(nullable: false),
                        Remark = c.String(maxLength: 2000),
                        Businessman_Code = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Businessman", t => t.Businessman_Code)
                .Index(t => t.Businessman_Code);
            
            CreateTable(
                "dbo.DefaultPolicy",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CarrayCode = c.String(maxLength: 10),
                        DefaultPolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChildrenPolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssueTicketType = c.String(),
                        Office = c.String(),
                        Carrier_Code = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Businessman", t => t.Carrier_Code)
                .Index(t => t.Carrier_Code);
            
            CreateTable(
                "dbo.NoticeAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        Url = c.String(maxLength: 500),
                        Notice_ID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Notice", t => t.Notice_ID)
                .Index(t => t.Notice_ID);
            
            CreateTable(
                "dbo.Notice",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Title = c.String(maxLength: 200),
                        Contents = c.String(),
                        State = c.Boolean(nullable: false),
                        NoticeType = c.String(),
                        NoticeShowType = c.String(),
                        CreateName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        EffectiveStartTime = c.DateTime(nullable: false),
                        EffectiveEndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SMSChargeSet",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 3),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Count = c.Int(nullable: false),
                        State = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SMSTemplate",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        TemplateName = c.String(maxLength: 50),
                        TemplateContents = c.String(maxLength: 1000),
                        State = c.Boolean(nullable: false),
                        CreateName = c.String(maxLength: 50),
                        CreateTime = c.DateTime(nullable: false),
                        LastOperTime = c.DateTime(nullable: false),
                        IsSystemTemplate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Businessman", "IssueSpeed", c => c.Int(nullable: false));
            AddColumn("dbo.Businessman", "SupRate", c => c.Decimal(precision: 18, scale: 3));
            AddColumn("dbo.Businessman", "SupNormalWork_WeekDay", c => c.String());
            AddColumn("dbo.Businessman", "SupNormalWork_WorkOnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupNormalWork_WorkUnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupNormalWork_ServiceOnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupNormalWork_ServiceUnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupRestWork_WeekDay", c => c.String());
            AddColumn("dbo.Businessman", "SupRestWork_WorkOnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupRestWork_WorkUnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupRestWork_ServiceOnLineTime", c => c.String());
            AddColumn("dbo.Businessman", "SupRestWork_ServiceUnLineTime", c => c.String());
            AddColumn("dbo.CarrierSetting", "Supplier_Code", c => c.String(maxLength: 20));
            AddColumn("dbo.PID", "Supplier_Code", c => c.String(maxLength: 20));
            CreateIndex("dbo.CarrierSetting", "Supplier_Code");
            CreateIndex("dbo.PID", "Supplier_Code");
            AddForeignKey("dbo.CarrierSetting", "Supplier_Code", "dbo.Businessman", "Code");
            AddForeignKey("dbo.PID", "Supplier_Code", "dbo.Businessman", "Code");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NoticeAttachment", "Notice_ID", "dbo.Notice");
            DropForeignKey("dbo.PID", "Supplier_Code", "dbo.Businessman");
            DropForeignKey("dbo.CarrierSetting", "Supplier_Code", "dbo.Businessman");
            DropForeignKey("dbo.DefaultPolicy", "Carrier_Code", "dbo.Businessman");
            DropForeignKey("dbo.GiveDetail", "Businessman_Code", "dbo.Businessman");
            DropIndex("dbo.NoticeAttachment", new[] { "Notice_ID" });
            DropIndex("dbo.PID", new[] { "Supplier_Code" });
            DropIndex("dbo.CarrierSetting", new[] { "Supplier_Code" });
            DropIndex("dbo.DefaultPolicy", new[] { "Carrier_Code" });
            DropIndex("dbo.GiveDetail", new[] { "Businessman_Code" });
            DropColumn("dbo.PID", "Supplier_Code");
            DropColumn("dbo.CarrierSetting", "Supplier_Code");
            DropColumn("dbo.Businessman", "SupRestWork_ServiceUnLineTime");
            DropColumn("dbo.Businessman", "SupRestWork_ServiceOnLineTime");
            DropColumn("dbo.Businessman", "SupRestWork_WorkUnLineTime");
            DropColumn("dbo.Businessman", "SupRestWork_WorkOnLineTime");
            DropColumn("dbo.Businessman", "SupRestWork_WeekDay");
            DropColumn("dbo.Businessman", "SupNormalWork_ServiceUnLineTime");
            DropColumn("dbo.Businessman", "SupNormalWork_ServiceOnLineTime");
            DropColumn("dbo.Businessman", "SupNormalWork_WorkUnLineTime");
            DropColumn("dbo.Businessman", "SupNormalWork_WorkOnLineTime");
            DropColumn("dbo.Businessman", "SupNormalWork_WeekDay");
            DropColumn("dbo.Businessman", "SupRate");
            DropColumn("dbo.Businessman", "IssueSpeed");
            DropTable("dbo.SMSTemplate");
            DropTable("dbo.SMSChargeSet");
            DropTable("dbo.Notice");
            DropTable("dbo.NoticeAttachment");
            DropTable("dbo.DefaultPolicy");
            DropTable("dbo.GiveDetail");
        }
    }
}
