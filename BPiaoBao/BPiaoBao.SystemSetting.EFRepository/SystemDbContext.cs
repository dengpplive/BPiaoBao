using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using JoveZhao.Framework;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System.Data.Entity.ModelConfiguration.Conventions;
using BPiaoBao.SystemSetting.EFRepository.BusinessmenMap;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using BPiaoBao.SystemSetting.Domain.Models.Notice;
using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using BPiaoBao.SystemSetting.EFRepository.CustomerInfosMap;
using BPiaoBao.SystemSetting.EFRepository.LogMap;
using BPiaoBao.SystemSetting.Domain.Models.Logs;


namespace PiaoBao.BTicket.EFRepository
{
    public class SystemDbContext : DbContext
    {
        static SystemDbContext()
        {
            Database.SetInitializer<SystemDbContext>(null);
        }

        public SystemDbContext() : base("name=system") {
            
        }

        public DbSet<Operator> Operators { get; set; }
        public DbSet<Businessman> Businessmen { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<SMS> Sms { get; set; }
        public DbSet<BuyDetail> BuyDetails { get; set; }
        public DbSet<SendDetail> SendDetails { get; set; }
        public DbSet<LoginLog> LoginLog { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<BehaviorStat> BehaviorStats { get; set; }
        public DbSet<CarrierSetting> CarrierSettings { get; set; }
        public DbSet<SMSTemplate> SMSTemplates { get; set; }
        public DbSet<GiveDetail> GiveDetails { get; set; }
        public DbSet<SMSChargeSet> SMSChargeSets { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<NoticeAttachment> NoticeAttachments { get; set; }
        public DbSet<MyMessage> Messages { get; set; }
        public DbSet<OPENScan> OpenScan { get; set; }

        public DbSet<CustomerInfo> CustomerInfo { get; set; }
        public DbSet<PhoneInfo> PhoneInfo { get; set; }
        public DbSet<QQInfo> QQInfo { get; set; }
        public DbSet<OperationLog> OperationLog {get;set;}
        public DbSet<StationBuyGroup> StationBuyGroup { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.ComplexType<ContactWay>();
            modelBuilder.ComplexType<WorkBusinessman>();
            modelBuilder.Configurations.Add(new BusinessmanMap());
            modelBuilder.Configurations.Add(new OperatorMap());
            modelBuilder.Configurations.Add(new AttachmentMap());
            modelBuilder.Configurations.Add(new SMSMap());
            modelBuilder.Configurations.Add(new BuyDetailMap());
            modelBuilder.Configurations.Add(new SendDetailMap());
            modelBuilder.Configurations.Add(new LoginLogMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new BuyerMap());
            modelBuilder.Configurations.Add(new CarrierMap());
            modelBuilder.Configurations.Add(new SupplierMap());
            modelBuilder.Configurations.Add(new BehaviorStatMap());
            modelBuilder.Configurations.Add(new CarrierSettingMap());
            modelBuilder.Configurations.Add(new SMSTemplateMap());
            modelBuilder.Configurations.Add(new GiveDetailMap());
            modelBuilder.Configurations.Add(new SMSChargeSetMap());
            modelBuilder.Configurations.Add(new NoticeMap());
            modelBuilder.Configurations.Add(new NoticeAttachmentMap());
            modelBuilder.Configurations.Add(new DefaultPolicyMap());
            modelBuilder.Configurations.Add(new MyMessageMap());
            modelBuilder.Configurations.Add(new CustomerInfoMap());
            modelBuilder.Configurations.Add(new PhoneInfoMap());
            modelBuilder.Configurations.Add(new QQInfoMap());
            modelBuilder.Configurations.Add(new OperationLogMap());
            base.OnModelCreating(modelBuilder);
        }

       
    }
}
