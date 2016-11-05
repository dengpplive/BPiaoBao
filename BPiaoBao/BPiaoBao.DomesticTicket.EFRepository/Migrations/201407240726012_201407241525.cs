namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407241525 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Passenger", "Birth", c => c.DateTime());
            AddColumn("dbo.InsuranceRecord", "PassengerType", c => c.Int(nullable: false));
            AddColumn("dbo.InsuranceRecord", "Birth", c => c.DateTime(nullable: false));
            AddColumn("dbo.InsuranceRecord", "InsureType", c => c.Int(nullable: false));
            AddColumn("dbo.FrePasser", "Birth", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FrePasser", "Birth");
            DropColumn("dbo.InsuranceRecord", "InsureType");
            DropColumn("dbo.InsuranceRecord", "Birth");
            DropColumn("dbo.InsuranceRecord", "PassengerType");
            DropColumn("dbo.Passenger", "Birth");
        }
    }
}
