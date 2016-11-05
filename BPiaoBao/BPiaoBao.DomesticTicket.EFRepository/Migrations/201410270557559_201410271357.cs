namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410271357 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AirChange", "QTResult", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AirChange", "QTResult");
        }
    }
}
