namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Attachment",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 200),
            //            Url = c.String(maxLength: 200),
            //            Businessman_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Businessman", t => t.Businessman_Code)
            //    .Index(t => t.Businessman_Code);
            
            //CreateTable(
            //    "dbo.BehaviorStat",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            BusinessmanCode = c.String(nullable: false, maxLength: 20),
            //            BusinessmanName = c.String(nullable: false, maxLength: 50),
            //            BusinessmanType = c.String(nullable: false, maxLength: 20),
            //            CarrierCode = c.String(maxLength: 20),
            //            BehaviorOperate = c.Int(nullable: false),
            //            OpDateTime = c.DateTime(nullable: false),
            //            ContactName = c.String(),
            //            OperatorName = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.BuyDetail",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 20),
            //            PayNo = c.String(maxLength: 50),
            //            OutPayNo = c.String(maxLength: 50),
            //            BuyTime = c.DateTime(nullable: false),
            //            PayTime = c.DateTime(),
            //            Count = c.Int(nullable: false),
            //            PayAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PayWay = c.Int(nullable: false),
            //            BuyState = c.Int(nullable: false),
            //            Businessman_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.Businessman", t => t.Businessman_Code)
            //    .Index(t => t.Businessman_Code);
            
            //CreateTable(
            //    "dbo.GiveDetail",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            GiveName = c.String(maxLength: 100),
            //            GiveCount = c.Int(nullable: false),
            //            GiveTime = c.DateTime(nullable: false),
            //            Remark = c.String(maxLength: 2000),
            //            Businessman_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.Businessman", t => t.Businessman_Code)
            //    .Index(t => t.Businessman_Code);
            
            //CreateTable(
            //    "dbo.Operator",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Account = c.String(nullable: false, maxLength: 20),
            //            Password = c.String(nullable: false, maxLength: 50),
            //            Realname = c.String(maxLength: 20),
            //            Phone = c.String(maxLength: 20),
            //            CreateDate = c.DateTime(nullable: false),
            //            OperatorState = c.Int(nullable: false),
            //            IsAdmin = c.Boolean(nullable: false),
            //            RoleID = c.Int(),
            //            Businessman_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Role", t => t.RoleID)
            //    .ForeignKey("dbo.Businessman", t => t.Businessman_Code)
            //    .Index(t => t.RoleID)
            //    .Index(t => t.Businessman_Code);
            
            //CreateTable(
            //    "dbo.Role",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Code = c.String(),
            //            RoleName = c.String(maxLength: 50),
            //            AuthNodes = c.String(),
            //            Description = c.String(maxLength: 2000),
            //            CreateTime = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.SendDetail",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 20),
            //            SendTime = c.DateTime(nullable: false),
            //            Content = c.String(maxLength: 900),
            //            ReceiveNum = c.String(maxLength: 20),
            //            ReceiveName = c.String(maxLength: 20),
            //            SendState = c.Boolean(nullable: false),
            //            SendCount = c.Int(nullable: false),
            //            Businessman_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.Businessman", t => t.Businessman_Code)
            //    .Index(t => t.Businessman_Code);
            
            //CreateTable(
            //    "dbo.SMS",
            //    c => new
            //        {
            //            BusinessmanCode = c.String(nullable: false, maxLength: 20),
            //            RemainCount = c.Int(nullable: false),
            //            SendCount = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.BusinessmanCode)
            //    .ForeignKey("dbo.Businessman", t => t.BusinessmanCode)
            //    .Index(t => t.BusinessmanCode);
            
            //CreateTable(
            //    "dbo.Businessman",
            //    c => new
            //        {
            //            Code = c.String(nullable: false, maxLength: 20),
            //            Name = c.String(nullable: false, maxLength: 50),
            //            CreateTime = c.DateTime(nullable: false),
            //            Contact = c.String(maxLength: 200),
            //            Tel = c.String(maxLength: 20),
            //            Address = c.String(maxLength: 200),
            //            ContactWay_Province = c.String(),
            //            ContactWay_City = c.String(),
            //            ContactName = c.String(),
            //            Phone = c.String(),
            //            IsEnable = c.Boolean(nullable: false),
            //            CashbagKey = c.String(maxLength: 50),
            //            CashbagCode = c.String(maxLength: 20),
            //            DefaultPolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ChildrenPolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            IssueSpeed = c.Int(nullable: false),
            //            BuyerLable = c.String(maxLength: 200),
            //            DeductionGroupID = c.Int(),
            //            BuyerCarrierCode = c.String(),
            //            CarrierLable = c.String(maxLength: 200),
            //            Rate = c.Decimal(precision: 18, scale: 3),
            //            NormalWork_WeekDay = c.String(),
            //            NormalWork_WorkOnLineTime = c.String(),
            //            NormalWork_WorkUnLineTime = c.String(),
            //            NormalWork_ServiceOnLineTime = c.String(),
            //            NormalWork_ServiceUnLineTime = c.String(),
            //            RestWork_WeekDay = c.String(),
            //            RestWork_WorkOnLineTime = c.String(),
            //            RestWork_WorkUnLineTime = c.String(),
            //            RestWork_ServiceOnLineTime = c.String(),
            //            RestWork_ServiceUnLineTime = c.String(),
            //            SupplierCarrierCode = c.String(),
            //            SupRate = c.Decimal(precision: 18, scale: 2),
            //            SupNormalWork_WeekDay = c.String(),
            //            SupNormalWork_WorkOnLineTime = c.String(),
            //            SupNormalWork_WorkUnLineTime = c.String(),
            //            SupNormalWork_ServiceOnLineTime = c.String(),
            //            SupNormalWork_ServiceUnLineTime = c.String(),
            //            SupRestWork_WeekDay = c.String(),
            //            SupRestWork_WorkOnLineTime = c.String(),
            //            SupRestWork_WorkUnLineTime = c.String(),
            //            SupRestWork_ServiceOnLineTime = c.String(),
            //            SupRestWork_ServiceUnLineTime = c.String(),
            //            Type = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Code);
            
            //CreateTable(
            //    "dbo.CarrierSetting",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            CarrierCode = c.String(maxLength: 20),
            //            CarrayCode = c.String(maxLength: 10),
            //            YDOffice = c.String(maxLength: 10),
            //            CPOffice = c.String(maxLength: 10),
            //            PrintNo = c.String(maxLength: 20),
            //            Supplier_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.Businessman", t => t.CarrierCode)
            //    .ForeignKey("dbo.Businessman", t => t.Supplier_Code)
            //    .Index(t => t.CarrierCode)
            //    .Index(t => t.Supplier_Code);
            
            //CreateTable(
            //    "dbo.PID",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            IP = c.String(),
            //            Port = c.Int(nullable: false),
            //            Office = c.String(),
            //            Carrier_Code = c.String(maxLength: 20),
            //            Supplier_Code = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.Businessman", t => t.Carrier_Code)
            //    .ForeignKey("dbo.Businessman", t => t.Supplier_Code)
            //    .Index(t => t.Carrier_Code)
            //    .Index(t => t.Supplier_Code);
            
            //CreateTable(
            //    "dbo.LoginLog",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Code = c.String(maxLength: 20),
            //            Account = c.String(maxLength: 50),
            //            LoginIP = c.String(maxLength: 100),
            //            LoginDate = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            
            
            //CreateTable(
            //    "dbo.SMSTemplate",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Code = c.String(),
            //            TemplateName = c.String(maxLength: 50),
            //            TemplateContents = c.String(maxLength: 1000),
            //            State = c.Boolean(nullable: false),
            //            CreateName = c.String(maxLength: 50),
            //            CreateTime = c.DateTime(nullable: false),
            //            LastOperTime = c.DateTime(nullable: false),
            //            IsSystemTemplate = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            //CreateTable(
            //    "dbo.SMSChargeSet",
            //    c => new
            //    {
            //        ID = c.Int(nullable: false, identity: true),
            //        Code = c.String(),
            //        Price = c.Decimal(nullable: false, precision: 18, scale: 3),
            //        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //        Count = c.Int(nullable: false),
            //        State = c.Boolean(nullable: false),
            //        CreateTime = c.DateTime(nullable: false),
            //    })
            //    .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PID", "Supplier_Code", "dbo.Businessman");
            DropForeignKey("dbo.CarrierSetting", "Supplier_Code", "dbo.Businessman");
            DropForeignKey("dbo.PID", "Carrier_Code", "dbo.Businessman");
            DropForeignKey("dbo.CarrierSetting", "CarrierCode", "dbo.Businessman");
            DropForeignKey("dbo.SMS", "BusinessmanCode", "dbo.Businessman");
            DropForeignKey("dbo.SendDetail", "Businessman_Code", "dbo.Businessman");
            DropForeignKey("dbo.Operator", "Businessman_Code", "dbo.Businessman");
            DropForeignKey("dbo.Operator", "RoleID", "dbo.Role");
            DropForeignKey("dbo.GiveDetail", "Businessman_Code", "dbo.Businessman");
            DropForeignKey("dbo.BuyDetail", "Businessman_Code", "dbo.Businessman");
            DropForeignKey("dbo.Attachment", "Businessman_Code", "dbo.Businessman");
            DropIndex("dbo.PID", new[] { "Supplier_Code" });
            DropIndex("dbo.CarrierSetting", new[] { "Supplier_Code" });
            DropIndex("dbo.PID", new[] { "Carrier_Code" });
            DropIndex("dbo.CarrierSetting", new[] { "CarrierCode" });
            DropIndex("dbo.SMS", new[] { "BusinessmanCode" });
            DropIndex("dbo.SendDetail", new[] { "Businessman_Code" });
            DropIndex("dbo.Operator", new[] { "Businessman_Code" });
            DropIndex("dbo.Operator", new[] { "RoleID" });
            DropIndex("dbo.GiveDetail", new[] { "Businessman_Code" });
            DropIndex("dbo.BuyDetail", new[] { "Businessman_Code" });
            DropIndex("dbo.Attachment", new[] { "Businessman_Code" });
            DropTable("dbo.SMSTemplate");
            DropTable("dbo.SMSChargeSet");
            DropTable("dbo.LoginLog");
            DropTable("dbo.PID");
            DropTable("dbo.CarrierSetting");
            DropTable("dbo.Businessman");
            DropTable("dbo.SMS");
            DropTable("dbo.SendDetail");
            DropTable("dbo.Role");
            DropTable("dbo.Operator");
            DropTable("dbo.GiveDetail");
            DropTable("dbo.BuyDetail");
            DropTable("dbo.BehaviorStat");
            DropTable("dbo.Attachment");
        }
    }
}
