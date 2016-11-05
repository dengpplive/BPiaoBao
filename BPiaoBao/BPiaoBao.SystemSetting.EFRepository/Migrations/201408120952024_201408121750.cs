namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201408121750 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DefaultPolicy", "IssueTicketWay", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DefaultPolicy", "IssueTicketWay");
        }
    }
}
