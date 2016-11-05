namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411251630 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "StationBuyGroupID", c => c.String(maxLength: 40));
            AddColumn("dbo.AfterSaleOrder", "StationBuyGroupID", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AfterSaleOrder", "StationBuyGroupID");
            DropColumn("dbo.Order", "StationBuyGroupID");
        }
    }
}
