namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407161016 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AfterSaleOrder", "CompletedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AfterSaleOrder", "CompletedTime");
        }
    }
}
