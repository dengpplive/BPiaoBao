namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407250958 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Businessman", "Plane", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Businessman", "Plane");
        }
    }
}
