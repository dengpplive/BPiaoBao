namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407241528 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InsuranceRecord", "PolicyAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InsuranceRecord", "PolicyAmount");
        }
    }
}
