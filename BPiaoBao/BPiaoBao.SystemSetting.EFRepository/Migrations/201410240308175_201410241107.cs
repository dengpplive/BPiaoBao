namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410241107 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MyMessage",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 20),
                        State = c.Boolean(nullable: false),
                        Title = c.String(maxLength: 200),
                        Content = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MyMessage");
        }
    }
}
