namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411251120 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Businessman", "BusinessTel", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Businessman", "BusinessTel");
        }
    }
}
