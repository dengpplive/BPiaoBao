namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410281616 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OPENScan",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Operator = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        OPENCount = c.Int(nullable: false),
                        ScanCount = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        OfficeNum = c.String(),
                        IP = c.String(),
                        Port = c.Int(nullable: false),
                        TemplateName = c.String(),
                        TemplateUrl = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OPENScan");
        }
    }
}
