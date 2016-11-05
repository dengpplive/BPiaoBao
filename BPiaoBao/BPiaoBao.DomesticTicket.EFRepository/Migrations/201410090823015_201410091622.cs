namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410091622 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ticket", "PayNumber", c => c.String());
            AddColumn("dbo.TicketConso", "PayFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TicketConso", "TransactionFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TicketConso", "InCome", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketConso", "InCome");
            DropColumn("dbo.TicketConso", "TransactionFee");
            DropColumn("dbo.TicketConso", "PayFee");
            DropColumn("dbo.Ticket", "PayNumber");
        }
    }
}
