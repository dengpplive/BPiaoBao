namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201408251757 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalPolicy", "SupplierWeek", c => c.String());
            AddColumn("dbo.LocalPolicy", "CarrierWeek", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalPolicy", "CarrierWeek");
            DropColumn("dbo.LocalPolicy", "SupplierWeek");
        }
    }
}
