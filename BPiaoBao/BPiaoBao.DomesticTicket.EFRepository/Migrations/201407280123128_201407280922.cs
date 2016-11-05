namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407280922 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocalPolicy", "WorkTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "WorkTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "ReturnTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "ReturnTicketTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "AnnulTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "AnnulTicketTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WorkTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WorkTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_ReturnTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_ReturnTicketTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_AnnulTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_AnnulTicketTime_EndTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalPolicy", "Carrier_AnnulTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "Carrier_AnnulTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "Carrier_ReturnTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "Carrier_ReturnTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WorkTime_EndTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WorkTime_StartTime");
            DropColumn("dbo.LocalPolicy", "AnnulTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "AnnulTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "ReturnTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "ReturnTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "WorkTime_EndTime");
            DropColumn("dbo.LocalPolicy", "WorkTime_StartTime");
        }
    }
}
