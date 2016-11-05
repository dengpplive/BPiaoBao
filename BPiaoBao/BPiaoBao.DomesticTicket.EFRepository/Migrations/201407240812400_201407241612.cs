namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407241612 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.InsuranceRecord", "Birth", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.InsuranceRecord", "Birth", c => c.DateTime(nullable: false));
        }
    }
}
