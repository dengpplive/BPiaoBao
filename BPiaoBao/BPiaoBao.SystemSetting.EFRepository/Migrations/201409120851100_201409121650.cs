namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201409121650 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SMSTemplate", "TemplateType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SMSTemplate", "TemplateType");
        }
    }
}
