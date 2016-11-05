namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201411121614 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Policy", "IsCoordination", c => c.Boolean(nullable: false));
            AddColumn("dbo.Policy", "MaxPoint", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DeductionDetail", "DeductionSource", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeductionDetail", "DeductionSource");
            DropColumn("dbo.Policy", "MaxPoint");
            DropColumn("dbo.Policy", "IsCoordination");
        }
    }
}
