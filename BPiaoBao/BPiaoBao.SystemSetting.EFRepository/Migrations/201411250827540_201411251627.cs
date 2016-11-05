namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411251627 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StationBuyGroup",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        GroupName = c.String(),
                        Description = c.String(),
                        Color = c.String(),
                        LastOperatorUser = c.String(),
                        LastOperatTime = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Businessman", "StationBuyGroupID", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Businessman", "StationBuyGroupID");
            DropTable("dbo.StationBuyGroup");
        }
    }
}
