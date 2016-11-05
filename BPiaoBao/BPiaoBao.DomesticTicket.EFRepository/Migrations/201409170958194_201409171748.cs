namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201409171748 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LocalPolicy", "PolicyType", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LocalPolicy", "PolicyType", c => c.String(nullable: false, maxLength: 128));
        }
    }
}
