namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407161616 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InsuranceRecord", "Passenger_Id", "dbo.Passenger");
            DropForeignKey("dbo.InsuranceRecord", "SkyWay_Id", "dbo.SkyWay");
            DropIndex("dbo.InsuranceRecord", new[] { "Passenger_Id" });
            DropIndex("dbo.InsuranceRecord", new[] { "SkyWay_Id" });
            AddColumn("dbo.InsuranceRecord", "PassengerId", c => c.Int());
            AddColumn("dbo.InsuranceRecord", "SkyWayId", c => c.Int());
            AddColumn("dbo.InsuranceRecord", "InsuredName", c => c.String());
            AddColumn("dbo.InsuranceRecord", "IdType", c => c.Int(nullable: false));
            AddColumn("dbo.InsuranceRecord", "SexType", c => c.Int(nullable: false));
            AddColumn("dbo.InsuranceRecord", "CardNo", c => c.String());
            AddColumn("dbo.InsuranceRecord", "Mobile", c => c.String());
            AddColumn("dbo.InsuranceRecord", "StartDateTime", c => c.DateTime());
            AddColumn("dbo.InsuranceRecord", "ToDateTime", c => c.DateTime());
            AddColumn("dbo.InsuranceRecord", "FlightNumber", c => c.String());
            AddColumn("dbo.InsuranceRecord", "PNR", c => c.String());
            AddColumn("dbo.InsuranceRecord", "FromCityName", c => c.String());
            AddColumn("dbo.InsuranceRecord", "FromCityCode", c => c.String());
            AddColumn("dbo.InsuranceRecord", "ToCityName", c => c.String());
            AddColumn("dbo.InsuranceRecord", "ToCityCode", c => c.String());
            DropColumn("dbo.InsuranceRecord", "Passenger_Id");
            DropColumn("dbo.InsuranceRecord", "SkyWay_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InsuranceRecord", "SkyWay_Id", c => c.Int());
            AddColumn("dbo.InsuranceRecord", "Passenger_Id", c => c.Int());
            DropColumn("dbo.InsuranceRecord", "ToCityCode");
            DropColumn("dbo.InsuranceRecord", "ToCityName");
            DropColumn("dbo.InsuranceRecord", "FromCityCode");
            DropColumn("dbo.InsuranceRecord", "FromCityName");
            DropColumn("dbo.InsuranceRecord", "PNR");
            DropColumn("dbo.InsuranceRecord", "FlightNumber");
            DropColumn("dbo.InsuranceRecord", "ToDateTime");
            DropColumn("dbo.InsuranceRecord", "StartDateTime");
            DropColumn("dbo.InsuranceRecord", "Mobile");
            DropColumn("dbo.InsuranceRecord", "CardNo");
            DropColumn("dbo.InsuranceRecord", "SexType");
            DropColumn("dbo.InsuranceRecord", "IdType");
            DropColumn("dbo.InsuranceRecord", "InsuredName");
            DropColumn("dbo.InsuranceRecord", "SkyWayId");
            DropColumn("dbo.InsuranceRecord", "PassengerId");
            CreateIndex("dbo.InsuranceRecord", "SkyWay_Id");
            CreateIndex("dbo.InsuranceRecord", "Passenger_Id");
            AddForeignKey("dbo.InsuranceRecord", "SkyWay_Id", "dbo.SkyWay", "Id");
            AddForeignKey("dbo.InsuranceRecord", "Passenger_Id", "dbo.Passenger", "Id");
        }
    }
}
