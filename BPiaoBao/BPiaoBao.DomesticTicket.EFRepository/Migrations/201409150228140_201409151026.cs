namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201409151026 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalPolicy", "SpecialType", c => c.String());
            AddColumn("dbo.LocalPolicy", "Type", c => c.Int());
            AddColumn("dbo.LocalPolicy", "FixedSeatPirce", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.LocalPolicy", "PolicyType", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalPolicy", "PolicyType");
            DropColumn("dbo.LocalPolicy", "FixedSeatPirce");
            DropColumn("dbo.LocalPolicy", "Type");
            DropColumn("dbo.LocalPolicy", "SpecialType");
        }
    }
}
