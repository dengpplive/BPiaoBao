namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407251158 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "CPMoney", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "CPMoney");
        }
    }
}
