namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201408111732 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SMSTemplate", "SkyWayType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SMSTemplate", "SkyWayType");
        }
    }
}
