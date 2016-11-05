namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201407290959 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OrderLog", "OperationContent", c => c.String());
            AlterColumn("dbo.OrderLog", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OrderLog", "Remark", c => c.String(maxLength: 200));
            AlterColumn("dbo.OrderLog", "OperationContent", c => c.String(maxLength: 500));
        }
    }
}
