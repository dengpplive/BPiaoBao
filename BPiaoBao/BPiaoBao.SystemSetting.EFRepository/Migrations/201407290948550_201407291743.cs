namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407291743 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Operator", "Tel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Operator", "Tel");
        }
    }
}
