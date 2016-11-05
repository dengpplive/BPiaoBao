namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411141744 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Businessman", "RemoteRate", c => c.Decimal(precision: 18, scale: 3));
            AlterColumn("dbo.Businessman", "SupRemoteRate", c => c.Decimal(precision: 18, scale: 3));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Businessman", "SupRemoteRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Businessman", "RemoteRate", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
