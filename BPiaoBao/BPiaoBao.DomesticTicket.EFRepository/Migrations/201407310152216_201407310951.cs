namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407310951 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Policy", "CarrierCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Policy", "CarrierCode");
        }
    }
}
