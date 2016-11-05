namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410291801 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketConso", "PaidMethod", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketConso", "PaidMethod");
        }
    }
}
