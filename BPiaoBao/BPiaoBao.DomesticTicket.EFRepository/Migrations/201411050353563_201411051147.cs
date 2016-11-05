namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411051147 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AirChange", "CarrierCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AirChange", "CarrierCode");
        }
    }
}
