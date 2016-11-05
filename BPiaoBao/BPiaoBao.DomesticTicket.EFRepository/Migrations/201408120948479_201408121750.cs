namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201408121750 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "InfNotGetPrice", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Order", "InfNotGetPrice");
        }
    }
}
