namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411121135 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OperationLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FunctionName = c.String(),
                        FunctionDescription = c.String(),
                        RequestParams = c.String(),
                        ModuleFullName = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        OperatorAcount = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OperationLog");
        }
    }
}
