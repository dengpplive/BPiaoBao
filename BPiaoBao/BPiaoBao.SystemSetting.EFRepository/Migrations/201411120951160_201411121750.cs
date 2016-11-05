namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411121750 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OperationLog", "BusinessCode", c => c.String());
            AddColumn("dbo.OperationLog", "BusinessName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OperationLog", "BusinessName");
            DropColumn("dbo.OperationLog", "BusinessCode");
        }
    }
}
