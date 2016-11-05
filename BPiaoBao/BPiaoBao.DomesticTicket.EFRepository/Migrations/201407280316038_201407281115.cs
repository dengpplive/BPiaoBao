namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407281115 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Order", "NewPnrCode", c => c.String());
            AddColumn("dbo.LocalPolicy", "WeeKWorkTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "WeeKWorkTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "WeekReturnTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "WeekReturnTicketTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "WeekAnnulTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "WeekAnnulTicketTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WeekWorkTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WeekWorkTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WeekReturnTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WeekReturnTicketTime_EndTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WeekAnnulTicketTime_StartTime", c => c.String());
            AddColumn("dbo.LocalPolicy", "Carrier_WeekAnnulTicketTime_EndTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocalPolicy", "Carrier_WeekAnnulTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WeekAnnulTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WeekReturnTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WeekReturnTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WeekWorkTime_EndTime");
            DropColumn("dbo.LocalPolicy", "Carrier_WeekWorkTime_StartTime");
            DropColumn("dbo.LocalPolicy", "WeekAnnulTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "WeekAnnulTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "WeekReturnTicketTime_EndTime");
            DropColumn("dbo.LocalPolicy", "WeekReturnTicketTime_StartTime");
            DropColumn("dbo.LocalPolicy", "WeeKWorkTime_EndTime");
            DropColumn("dbo.LocalPolicy", "WeeKWorkTime_StartTime");
            DropColumn("dbo.Order", "NewPnrCode");
        }
    }
}
