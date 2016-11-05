namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201408280923 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "IsLowPrice", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "IsLowPrice");
        }
    }
}
