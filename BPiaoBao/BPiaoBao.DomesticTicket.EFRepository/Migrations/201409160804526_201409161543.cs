namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201409161543 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketCarrier", "CarrierCode", c => c.String());
            AddColumn("dbo.TicketSupplier", "IssueTicketCode", c => c.String());
            AddColumn("dbo.TicketBuyer", "Code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketBuyer", "Code");
            DropColumn("dbo.TicketSupplier", "IssueTicketCode");
            DropColumn("dbo.TicketCarrier", "CarrierCode");
        }
    }
}
