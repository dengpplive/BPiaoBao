namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407291623 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Passenger", "CPMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Passenger", "CPMoney");
        }
    }
}
