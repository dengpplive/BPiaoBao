namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411070954 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MyMessage", "QnContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MyMessage", "QnContent");
        }
    }
}
