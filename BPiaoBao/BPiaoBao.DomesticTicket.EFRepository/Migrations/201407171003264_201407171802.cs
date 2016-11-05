namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407171802 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InsuranceDepositLog", "OperatorAccount", c => c.String());
            AddColumn("dbo.InsuranceDepositLog", "OperatorName", c => c.String());
            AddColumn("dbo.InsuranceRecord", "InsuranceStatus", c => c.Int(nullable: false));
            AddColumn("dbo.InsurancePurchaseByBussinessman", "OperatorAccount", c => c.String());
            AddColumn("dbo.InsurancePurchaseByBussinessman", "OperatorName", c => c.String());
            DropColumn("dbo.InsuranceOrder", "TradeId");
            DropColumn("dbo.InsuranceOrder", "PayNo");
            DropColumn("dbo.InsuranceOrder", "EnumInsuranceStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InsuranceOrder", "EnumInsuranceStatus", c => c.Int(nullable: false));
            AddColumn("dbo.InsuranceOrder", "PayNo", c => c.String(maxLength: 50));
            AddColumn("dbo.InsuranceOrder", "TradeId", c => c.String(maxLength: 50));
            DropColumn("dbo.InsurancePurchaseByBussinessman", "OperatorName");
            DropColumn("dbo.InsurancePurchaseByBussinessman", "OperatorAccount");
            DropColumn("dbo.InsuranceRecord", "InsuranceStatus");
            DropColumn("dbo.InsuranceDepositLog", "OperatorName");
            DropColumn("dbo.InsuranceDepositLog", "OperatorAccount");
        }
    }
}
