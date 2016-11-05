namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411191041 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Policy", "TodayGYCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Policy", "TodayGYCode");
        }
    }
}
