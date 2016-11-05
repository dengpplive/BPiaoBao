namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411141609 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Businessman", "PointGroupID", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Businessman", "PointGroupID");
        }
    }
}
