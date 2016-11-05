namespace BPiaoBao.SystemSetting.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201410151507 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BuyDetail", "PayFee", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BuyDetail", "PayFee");
        }
    }
}
