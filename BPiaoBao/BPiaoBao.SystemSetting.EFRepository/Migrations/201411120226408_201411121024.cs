namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411121024 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CarrierCode = c.String(maxLength: 20),
                        CustomPhone = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QQInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QQ = c.String(maxLength: 15),
                        Description = c.String(maxLength: 50),
                        CustomerInfo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerInfo", t => t.CustomerInfo_Id)
                .Index(t => t.CustomerInfo_Id);
            
            CreateTable(
                "dbo.PhoneInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Phone = c.String(maxLength: 15),
                        Description = c.String(maxLength: 50),
                        CustomerInfo_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerInfo", t => t.CustomerInfo_Id)
                .Index(t => t.CustomerInfo_Id);
            
            AddColumn("dbo.Businessman", "RemoteRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Businessman", "SupRemoteRate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Businessman", "LocalPolicySwitch", c => c.Boolean());
            AddColumn("dbo.Businessman", "InterfacePolicySwitch", c => c.Boolean());
            AddColumn("dbo.Businessman", "ForeignRemotePolicySwich", c => c.Boolean());
            AddColumn("dbo.Businessman", "BuyerRemotoPolicySwich", c => c.Boolean());
            AddColumn("dbo.Businessman", "ShowLocalCSCSwich", c => c.Boolean());
            AddColumn("dbo.Businessman", "SupLocalPolicySwitch", c => c.Boolean());
            AddColumn("dbo.Businessman", "SupRemotePolicySwitch", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhoneInfo", "CustomerInfo_Id", "dbo.CustomerInfo");
            DropForeignKey("dbo.QQInfo", "CustomerInfo_Id", "dbo.CustomerInfo");
            DropIndex("dbo.PhoneInfo", new[] { "CustomerInfo_Id" });
            DropIndex("dbo.QQInfo", new[] { "CustomerInfo_Id" });
            DropColumn("dbo.Businessman", "RemotePolicySwitch");
            DropColumn("dbo.Businessman", "LocalPolicySwitch1");
            DropColumn("dbo.Businessman", "SupRemoteRate");
            DropColumn("dbo.Businessman", "ShowLocalCSCSwich");
            DropColumn("dbo.Businessman", "BuyerRemotoPolicySwich");
            DropColumn("dbo.Businessman", "ForeignRemotePolicySwich");
            DropColumn("dbo.Businessman", "InterfacePolicySwitch");
            DropColumn("dbo.Businessman", "LocalPolicySwitch");
            DropColumn("dbo.Businessman", "RemoteRate");
            DropTable("dbo.PhoneInfo");
            DropTable("dbo.QQInfo");
            DropTable("dbo.CustomerInfo");
        }
    }
}
