namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201409171248 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Policy", "PolicySpecialType", c => c.Int(nullable: false));
            AddColumn("dbo.Policy", "SpecialPriceOrDiscount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.TicketConso", "Paymethod", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TicketConso", "Paymethod");
            DropColumn("dbo.Policy", "SpecialPriceOrDiscount");
            DropColumn("dbo.Policy", "PolicySpecialType");
        }
    }
}
