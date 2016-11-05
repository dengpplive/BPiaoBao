namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410281616 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FrePasser", "Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FrePasser", "Name", c => c.String(maxLength: 20));
        }
    }
}
