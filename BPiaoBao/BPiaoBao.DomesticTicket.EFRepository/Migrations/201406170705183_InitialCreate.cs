namespace BPiaoBao.DomesticTicket.EFRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.AdjustDetail",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            StartPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            EndPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            AdjustType = c.Int(nullable: false),
            //            Point = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            DeductionRule_ID = c.Int(),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.DeductionRule", t => t.DeductionRule_ID)
            //    .Index(t => t.DeductionRule_ID);
            
            //CreateTable(
            //    "dbo.CoordinationLog",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            AddDatetime = c.DateTime(nullable: false),
            //            Content = c.String(maxLength: 500),
            //            OperationPerson = c.String(maxLength: 50),
            //            Type = c.String(),
            //            AfterSaleOrder_Id = c.Int(),
            //            Order_OrderId = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.AfterSaleOrder_Id)
            //    .ForeignKey("dbo.Order", t => t.Order_OrderId)
            //    .Index(t => t.AfterSaleOrder_Id)
            //    .Index(t => t.Order_OrderId);
            
            //CreateTable(
            //    "dbo.OrderLog",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            OperationDatetime = c.DateTime(nullable: false),
            //            OperationContent = c.String(maxLength: 500),
            //            OperationPerson = c.String(maxLength: 20),
            //            Remark = c.String(maxLength: 200),
            //            IsShowLog = c.Boolean(nullable: false),
            //            AfterSaleOrder_Id = c.Int(),
            //            Order_OrderId = c.String(maxLength: 20),
            //            PlatformRefundOrder_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.AfterSaleOrder_Id)
            //    .ForeignKey("dbo.Order", t => t.Order_OrderId)
            //    .ForeignKey("dbo.PlatformRefundOrder", t => t.PlatformRefundOrder_Id)
            //    .Index(t => t.AfterSaleOrder_Id)
            //    .Index(t => t.Order_OrderId)
            //    .Index(t => t.PlatformRefundOrder_Id);
            
            //CreateTable(
            //    "dbo.Order",
            //    c => new
            //        {
            //            OrderId = c.String(nullable: false, maxLength: 20),
            //            TicketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            INFTicketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            OrderType = c.Int(nullable: false),
            //            PnrType = c.Int(nullable: false),
            //            OldOrderId = c.String(),
            //            OrderSource = c.Int(nullable: false),
            //            HaveBabyFlag = c.Boolean(nullable: false),
            //            OrderMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            OrderCommissionTotalMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PnrCode = c.String(nullable: false, maxLength: 10),
            //            BigCode = c.String(nullable: false, maxLength: 10),
            //            PnrContent = c.String(maxLength: 2000),
            //            BusinessmanCode = c.String(maxLength: 20),
            //            BusinessmanName = c.String(maxLength: 20),
            //            OperatorAccount = c.String(maxLength: 20),
            //            LockAccount = c.String(maxLength: 20),
            //            OutOrderId = c.String(maxLength: 50),
            //            OrderStatus = c.Int(nullable: false),
            //            Remark = c.String(maxLength: 200),
            //            CreateTime = c.DateTime(nullable: false),
            //            IssueTicketTime = c.DateTime(),
            //            RefundedTradeMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            RefundedServiceMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            CoordinationStatus = c.Boolean(),
            //            HasAfterSale = c.Boolean(nullable: false),
            //            AfterSaleTotalMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            CarrierCode = c.String(),
            //            PnrSource = c.Int(nullable: false),
            //            YdOffice = c.String(),
            //            CpOffice = c.String(),
            //            IsInsuranceRefund = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.OrderId);
            
            //CreateTable(
            //    "dbo.OrderPay",
            //    c => new
            //        {
            //            OrderId = c.String(nullable: false, maxLength: 20),
            //            PaySerialNumber = c.String(maxLength: 50),
            //            PayMoney = c.Decimal(precision: 18, scale: 2),
            //            TradePoundage = c.Decimal(precision: 18, scale: 2),
            //            SystemFee = c.Decimal(precision: 18, scale: 2),
            //            PayDateTime = c.DateTime(),
            //            PayStatus = c.Int(nullable: false),
            //            PayMethod = c.Int(),
            //            PayMethodCode = c.String(),
            //            PaidSerialNumber = c.String(maxLength: 50),
            //            PaidMoney = c.Decimal(precision: 18, scale: 2),
            //            PaidStatus = c.Int(nullable: false),
            //            PaidDateTime = c.DateTime(),
            //            PaidMethod = c.String(),
            //        })
            //    .PrimaryKey(t => t.OrderId)
            //    .ForeignKey("dbo.Order", t => t.OrderId)
            //    .Index(t => t.OrderId);
            
            //CreateTable(
            //    "dbo.PayBillDetail",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Code = c.String(maxLength: 50),
            //            CashbagCode = c.String(maxLength: 50),
            //            Name = c.String(maxLength: 50),
            //            Money = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Point = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            OpType = c.Int(nullable: false),
            //            AdjustType = c.Int(nullable: false),
            //            Remark = c.String(),
            //            OrderPay_OrderId = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.OrderPay", t => t.OrderPay_OrderId)
            //    .Index(t => t.OrderPay_OrderId);
            
            //CreateTable(
            //    "dbo.Passenger",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            PassengerName = c.String(maxLength: 100),
            //            PassengerType = c.Int(nullable: false),
            //            CardNo = c.String(maxLength: 50),
            //            ABFee = c.Decimal(precision: 18, scale: 2),
            //            RQFee = c.Decimal(precision: 18, scale: 2),
            //            SeatPrice = c.Decimal(precision: 18, scale: 2),
            //            TicketStatus = c.Int(nullable: false),
            //            Mobile = c.String(maxLength: 20),
            //            TicketNumber = c.String(maxLength: 20),
            //            TravelNumber = c.String(maxLength: 20),
            //            PassengerTripStatus = c.Int(nullable: false),
            //            PayMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            RefundedServiceMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            BuyInsuranceCount = c.Int(nullable: false),
            //            BuyInsurancePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Order_OrderId = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Order", t => t.Order_OrderId)
            //    .Index(t => t.Order_OrderId);
            
            //CreateTable(
            //    "dbo.Policy",
            //    c => new
            //        {
            //            OrderId = c.String(nullable: false, maxLength: 20),
            //            Commission = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            AreaCity = c.String(maxLength: 10),
            //            PolicyId = c.String(maxLength: 50),
            //            PlatformCode = c.String(maxLength: 20),
            //            PolicyPoint = c.Decimal(precision: 18, scale: 2),
            //            DownPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PaidPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ReturnMoney = c.Decimal(precision: 18, scale: 2),
            //            IsChangePNRCP = c.Boolean(),
            //            EnumIssueTicketWay = c.Int(nullable: false),
            //            IsSp = c.Boolean(),
            //            PolicyType = c.String(maxLength: 10),
            //            WorkTime_StartTime = c.String(),
            //            WorkTime_EndTime = c.String(),
            //            ReturnTicketTime_StartTime = c.String(),
            //            ReturnTicketTime_EndTime = c.String(),
            //            AnnulTicketTime_StartTime = c.String(),
            //            AnnulTicketTime_EndTime = c.String(),
            //            CPOffice = c.String(maxLength: 20),
            //            IssueSpeed = c.String(maxLength: 20),
            //            Remark = c.String(maxLength: 500),
            //            PolicySourceType = c.Int(nullable: false),
            //            CarryCode = c.String(),
            //            Code = c.String(),
            //            Name = c.String(),
            //            CashbagCode = c.String(),
            //            Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            IsLow = c.Boolean(nullable: false),
            //            SeatPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ABFee = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            RQFee = c.Decimal(nullable: false, precision: 18, scale: 2),
            //        })
            //    .PrimaryKey(t => t.OrderId)
            //    .ForeignKey("dbo.Order", t => t.OrderId)
            //    .Index(t => t.OrderId);
            
            //CreateTable(
            //    "dbo.DeductionDetail",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            UnCode = c.String(),
            //            UnName = c.String(),
            //            Code = c.String(),
            //            Name = c.String(maxLength: 50),
            //            Point = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            AdjustType = c.Int(nullable: false),
            //            DeductionType = c.Int(nullable: false),
            //            Policy_OrderId = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.Policy", t => t.Policy_OrderId)
            //    .Index(t => t.Policy_OrderId);
            
            //CreateTable(
            //    "dbo.SkyWay",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            FromCityCode = c.String(maxLength: 10),
            //            ToCityCode = c.String(maxLength: 10),
            //            FlightNumber = c.String(maxLength: 10),
            //            StartDateTime = c.DateTime(nullable: false),
            //            ToDateTime = c.DateTime(nullable: false),
            //            CarrayCode = c.String(maxLength: 10),
            //            Seat = c.String(maxLength: 10),
            //            FromTerminal = c.String(maxLength: 10),
            //            ToTerminal = c.String(maxLength: 10),
            //            Discount = c.Decimal(precision: 18, scale: 2),
            //            FlightModel = c.String(maxLength: 10),
            //            Order_OrderId = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Order", t => t.Order_OrderId)
            //    .Index(t => t.Order_OrderId);
            
            //CreateTable(
            //    "dbo.AfterSalePassenger",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            PassengerId = c.Int(nullable: false),
            //            RetirementMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            RetirementPoundage = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            IsRefund = c.Boolean(nullable: false),
            //            Status = c.Int(nullable: false),
            //            AfterSaleOrder_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Passenger", t => t.PassengerId, cascadeDelete: true)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.AfterSaleOrder_Id)
            //    .Index(t => t.PassengerId)
            //    .Index(t => t.AfterSaleOrder_Id);
            
            //CreateTable(
            //    "dbo.AfterSaleOrder",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            ProcessStatus = c.Int(nullable: false),
            //            Reason = c.String(maxLength: 1000),
            //            Description = c.String(maxLength: 1000),
            //            CreateMan = c.String(maxLength: 20),
            //            CreateTime = c.DateTime(nullable: false),
            //            Money = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ProcessName = c.String(),
            //            ProcessDate = c.DateTime(),
            //            Remark = c.String(),
            //            Order_OrderId = c.String(maxLength: 20),
            //            LockCurrentAccount = c.String(),
            //            IsCoorCompleted = c.Boolean(),
            //            AnnulAttachmentUrl = c.String(),
            //            CashBagCode = c.String(),
            //            PayWay = c.Int(),
            //            PayTime = c.DateTime(),
            //            OutPayNo = c.String(),
            //            PayStatus = c.Int(),
            //            RefundMoney = c.Decimal(precision: 18, scale: 2),
            //            IsVoluntary = c.Boolean(),
            //            BounceAttachmentUrl = c.String(),
            //            Type = c.String(nullable: false, maxLength: 128),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Order", t => t.Order_OrderId)
            //    .Index(t => t.Order_OrderId);
            
            //CreateTable(
            //    "dbo.BounceLine",
            //    c => new
            //        {
            //            ID = c.String(nullable: false, maxLength: 128),
            //            PayMethod = c.Int(nullable: false),
            //            PassgenerName = c.String(),
            //            ChangeOrderID = c.Int(),
            //            PaySerialNumber = c.String(),
            //            OrderID = c.String(maxLength: 20),
            //            RefundMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            RefundTime = c.DateTime(),
            //            BusArgs = c.String(),
            //            Status = c.Int(nullable: false),
            //            RefundServiceMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            AnnulOrder_Id = c.Int(),
            //            BounceOrder_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.ChangeOrderID)
            //    .ForeignKey("dbo.Order", t => t.OrderID)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.AnnulOrder_Id)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.BounceOrder_Id)
            //    .Index(t => t.ChangeOrderID)
            //    .Index(t => t.OrderID)
            //    .Index(t => t.AnnulOrder_Id)
            //    .Index(t => t.BounceOrder_Id);
            
            //CreateTable(
            //    "dbo.AfterSaleSkyWay",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            SkyWayId = c.Int(nullable: false),
            //            FlyDate = c.DateTime(),
            //            ToDate = c.DateTime(),
            //            FlightNumber = c.String(),
            //            Seat = c.String(),
            //            ChangeOrder_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.SkyWay", t => t.SkyWayId, cascadeDelete: true)
            //    .ForeignKey("dbo.AfterSaleOrder", t => t.ChangeOrder_Id)
            //    .Index(t => t.SkyWayId)
            //    .Index(t => t.ChangeOrder_Id);
            
            //CreateTable(
            //    "dbo.DeductionGroup",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            CarrierCode = c.String(),
            //            Name = c.String(maxLength: 50),
            //            Description = c.String(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.DeductionRule",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            CarrCode = c.String(),
            //            DeductionType = c.Int(nullable: false),
            //            StartTime = c.DateTime(nullable: false),
            //            EndTime = c.DateTime(nullable: false),
            //            DeductionGroup_ID = c.Int(),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.DeductionGroup", t => t.DeductionGroup_ID)
            //    .Index(t => t.DeductionGroup_ID);
            
            //CreateTable(
            //    "dbo.FrePasser",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(maxLength: 20),
            //            PasserType = c.String(maxLength: 10),
            //            CertificateType = c.String(maxLength: 20),
            //            CertificateNo = c.String(maxLength: 50),
            //            Mobile = c.String(maxLength: 20),
            //            AirCardNo = c.String(maxLength: 50),
            //            Remark = c.String(maxLength: 200),
            //            BusinessmanCode = c.String(maxLength: 20),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Insurance",
            //    c => new
            //        {
            //            InsuranceNo = c.String(nullable: false, maxLength: 128),
            //            OrderId = c.String(maxLength: 20),
            //            BuyTime = c.DateTime(),
            //            InsuranceLimitStartTime = c.DateTime(),
            //            InsuranceLimitEndTime = c.DateTime(),
            //            InsuranceCompany = c.String(maxLength: 100),
            //            InsurancePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            EnumInsuranceStatus = c.Int(nullable: false),
            //            CarrierCode = c.String(maxLength: 20),
            //            CarrierName = c.String(maxLength: 50),
            //            BussinessmanCode = c.String(maxLength: 20),
            //            BussinessmanName = c.String(maxLength: 50),
            //            Passenger_Id = c.Int(),
            //            SkyWay_Id = c.Int(),
            //        })
            //    .PrimaryKey(t => t.InsuranceNo)
            //    .ForeignKey("dbo.Passenger", t => t.Passenger_Id)
            //    .ForeignKey("dbo.SkyWay", t => t.SkyWay_Id)
            //    .Index(t => t.Passenger_Id)
            //    .Index(t => t.SkyWay_Id);
            
            //CreateTable(
            //    "dbo.InsuranceConfig",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            LeaveCount = c.Int(nullable: false),
            //            SinglePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            IsOpen = c.Boolean(nullable: false),
            //            BusinessmanCode = c.String(maxLength: 20),
            //            BusinessmanName = c.String(maxLength: 50),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.InsuranceDepositLog",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            PayNo = c.String(maxLength: 50),
            //            OutTradeNo = c.String(maxLength: 50),
            //            BeforeLeaveCount = c.Int(nullable: false),
            //            AfterLeaveCount = c.Int(nullable: false),
            //            DepositCount = c.Int(nullable: false),
            //            BuyTime = c.DateTime(nullable: false),
            //            SinglePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            BusinessmanCode = c.String(maxLength: 20),
            //            BusinessmanName = c.String(maxLength: 50),
            //            PayWay = c.Int(nullable: false),
            //            BuyState = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.LocalPolicy",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Code = c.String(maxLength: 20),
            //            ReleaseType = c.Int(nullable: false),
            //            FromCityCodes = c.String(),
            //            ToCityCodes = c.String(),
            //            LocalPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Different = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            TravelType = c.Int(nullable: false),
            //            LocalPolicyType = c.String(),
            //            Low = c.Boolean(nullable: false),
            //            ChangeCode = c.Boolean(nullable: false),
            //            Office = c.String(),
            //            WeekLimit = c.String(),
            //            Share = c.Boolean(nullable: false),
            //            CarrayCode = c.String(),
            //            Seats = c.String(),
            //            PassengeDate_StartTime = c.DateTime(nullable: false),
            //            PassengeDate_EndTime = c.DateTime(nullable: false),
            //            IssueTicketSetting_IssueDate_StartTime = c.DateTime(nullable: false),
            //            IssueTicketSetting_IssueDate_EndTime = c.DateTime(nullable: false),
            //            IssueTicketSetting_EarlyDays = c.Int(nullable: false),
            //            IssueTicketSetting_IssueTicketWay = c.Int(nullable: false),
            //            Apply = c.Boolean(),
            //            ApplyFlights = c.String(),
            //            Remark = c.String(),
            //            Review = c.Boolean(nullable: false),
            //            HangUp = c.Boolean(nullable: false),
            //            CreateMan = c.String(),
            //            CreateDate = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.PlatformRefundOrder",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            RefundType = c.Int(nullable: false),
            //            RefundOrderId = c.String(maxLength: 20),
            //            RefundStatus = c.Int(nullable: false),
            //            RefundAmount = c.Decimal(precision: 18, scale: 2),
            //            RefundTime = c.DateTime(),
            //            Remark = c.String(maxLength: 500),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.TicketSum",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            SupplierCode = c.String(),
            //            CarrierCode = c.String(),
            //            Code = c.String(),
            //            OrderID = c.String(maxLength: 20),
            //            TransactionNumber = c.String(),
            //            CurrentOrderID = c.String(),
            //            TicketNum = c.String(),
            //            TicketState = c.String(maxLength: 10),
            //            StartTime = c.String(),
            //            FlightNum = c.String(maxLength: 50),
            //            Voyage = c.String(maxLength: 50),
            //            Seat = c.String(maxLength: 10),
            //            PassengerName = c.String(maxLength: 50),
            //            PNR = c.String(maxLength: 10),
            //            SeatPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ABFee = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            RQFee = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PolicyPoint = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            OrderMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PayMethod = c.String(maxLength: 10),
            //            TrvalNum = c.String(maxLength: 50),
            //            CreateDate = c.DateTime(nullable: false),
            //            PolicyFrom = c.String(),
            //            OutOrderID = c.String(),
            //            BigCode = c.String(),
            //            PaidMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            CarryCode = c.String(),
            //            PMFee = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            OldPolicy = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PolicyType = c.String(),
            //            CarrierMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            ParentMoney = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            PolicySourceType = c.String(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.TravelGrantRecord",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            BusinessmanCode = c.String(maxLength: 50),
            //            BusinessmanName = c.String(maxLength: 100),
            //            UseBusinessmanCode = c.String(maxLength: 50),
            //            UseBusinessmanName = c.String(maxLength: 100),
            //            TripScope = c.String(),
            //            Office = c.String(),
            //            TripCount = c.Int(nullable: false),
            //            TripRemark = c.String(),
            //            GrantTime = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.TravelPaperLog",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            OperationDatetime = c.DateTime(nullable: false),
            //            OperationPerson = c.String(maxLength: 20),
            //            OperationType = c.String(maxLength: 20),
            //            OperationContent = c.String(),
            //            TravelPaper_ID = c.Int(),
            //        })
            //    .PrimaryKey(t => t.ID)
            //    .ForeignKey("dbo.TravelPaper", t => t.TravelPaper_ID)
            //    .Index(t => t.TravelPaper_ID);
            
            //CreateTable(
            //    "dbo.TravelPaper",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            BusinessmanCode = c.String(),
            //            BusinessmanName = c.String(),
            //            UseBusinessmanCode = c.String(),
            //            UseBusinessmanName = c.String(),
            //            TicketNumber = c.String(maxLength: 15),
            //            TripNumber = c.String(maxLength: 10),
            //            TripStatus = c.Int(nullable: false),
            //            UseOffice = c.String(maxLength: 6),
            //            IataCode = c.String(maxLength: 8),
            //            TicketCompanyName = c.String(maxLength: 200),
            //            GrantTime = c.DateTime(nullable: false),
            //            PrintTime = c.DateTime(nullable: false),
            //            InvalidTime = c.DateTime(nullable: false),
            //            BlankRecoveryTime = c.DateTime(nullable: false),
            //            UseTripRemark = c.String(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TravelPaperLog", "TravelPaper_ID", "dbo.TravelPaper");
            DropForeignKey("dbo.OrderLog", "PlatformRefundOrder_Id", "dbo.PlatformRefundOrder");
            DropForeignKey("dbo.Insurance", "SkyWay_Id", "dbo.SkyWay");
            DropForeignKey("dbo.Insurance", "Passenger_Id", "dbo.Passenger");
            DropForeignKey("dbo.DeductionRule", "DeductionGroup_ID", "dbo.DeductionGroup");
            DropForeignKey("dbo.AdjustDetail", "DeductionRule_ID", "dbo.DeductionRule");
            DropForeignKey("dbo.BounceLine", "BounceOrder_Id", "dbo.AfterSaleOrder");
            DropForeignKey("dbo.BounceLine", "AnnulOrder_Id", "dbo.AfterSaleOrder");
            DropForeignKey("dbo.BounceLine", "OrderID", "dbo.Order");
            DropForeignKey("dbo.BounceLine", "ChangeOrderID", "dbo.AfterSaleOrder");
            DropForeignKey("dbo.AfterSaleSkyWay", "ChangeOrder_Id", "dbo.AfterSaleOrder");
            DropForeignKey("dbo.AfterSaleSkyWay", "SkyWayId", "dbo.SkyWay");
            DropForeignKey("dbo.AfterSalePassenger", "AfterSaleOrder_Id", "dbo.AfterSaleOrder");
            DropForeignKey("dbo.AfterSalePassenger", "PassengerId", "dbo.Passenger");
            DropForeignKey("dbo.AfterSaleOrder", "Order_OrderId", "dbo.Order");
            DropForeignKey("dbo.SkyWay", "Order_OrderId", "dbo.Order");
            DropForeignKey("dbo.Policy", "OrderId", "dbo.Order");
            DropForeignKey("dbo.DeductionDetail", "Policy_OrderId", "dbo.Policy");
            DropForeignKey("dbo.Passenger", "Order_OrderId", "dbo.Order");
            DropForeignKey("dbo.PayBillDetail", "OrderPay_OrderId", "dbo.OrderPay");
            DropForeignKey("dbo.OrderPay", "OrderId", "dbo.Order");
            DropForeignKey("dbo.OrderLog", "Order_OrderId", "dbo.Order");
            DropForeignKey("dbo.CoordinationLog", "Order_OrderId", "dbo.Order");
            DropForeignKey("dbo.OrderLog", "AfterSaleOrder_Id", "dbo.AfterSaleOrder");
            DropForeignKey("dbo.CoordinationLog", "AfterSaleOrder_Id", "dbo.AfterSaleOrder");
            DropIndex("dbo.TravelPaperLog", new[] { "TravelPaper_ID" });
            DropIndex("dbo.OrderLog", new[] { "PlatformRefundOrder_Id" });
            DropIndex("dbo.Insurance", new[] { "SkyWay_Id" });
            DropIndex("dbo.Insurance", new[] { "Passenger_Id" });
            DropIndex("dbo.DeductionRule", new[] { "DeductionGroup_ID" });
            DropIndex("dbo.AdjustDetail", new[] { "DeductionRule_ID" });
            DropIndex("dbo.BounceLine", new[] { "BounceOrder_Id" });
            DropIndex("dbo.BounceLine", new[] { "AnnulOrder_Id" });
            DropIndex("dbo.BounceLine", new[] { "OrderID" });
            DropIndex("dbo.BounceLine", new[] { "ChangeOrderID" });
            DropIndex("dbo.AfterSaleSkyWay", new[] { "ChangeOrder_Id" });
            DropIndex("dbo.AfterSaleSkyWay", new[] { "SkyWayId" });
            DropIndex("dbo.AfterSalePassenger", new[] { "AfterSaleOrder_Id" });
            DropIndex("dbo.AfterSalePassenger", new[] { "PassengerId" });
            DropIndex("dbo.AfterSaleOrder", new[] { "Order_OrderId" });
            DropIndex("dbo.SkyWay", new[] { "Order_OrderId" });
            DropIndex("dbo.Policy", new[] { "OrderId" });
            DropIndex("dbo.DeductionDetail", new[] { "Policy_OrderId" });
            DropIndex("dbo.Passenger", new[] { "Order_OrderId" });
            DropIndex("dbo.PayBillDetail", new[] { "OrderPay_OrderId" });
            DropIndex("dbo.OrderPay", new[] { "OrderId" });
            DropIndex("dbo.OrderLog", new[] { "Order_OrderId" });
            DropIndex("dbo.CoordinationLog", new[] { "Order_OrderId" });
            DropIndex("dbo.OrderLog", new[] { "AfterSaleOrder_Id" });
            DropIndex("dbo.CoordinationLog", new[] { "AfterSaleOrder_Id" });
            DropTable("dbo.TravelPaper");
            DropTable("dbo.TravelPaperLog");
            DropTable("dbo.TravelGrantRecord");
            DropTable("dbo.TicketSum");
            DropTable("dbo.PlatformRefundOrder");
            DropTable("dbo.LocalPolicy");
            DropTable("dbo.InsuranceDepositLog");
            DropTable("dbo.InsuranceConfig");
            DropTable("dbo.Insurance");
            DropTable("dbo.FrePasser");
            DropTable("dbo.DeductionRule");
            DropTable("dbo.DeductionGroup");
            DropTable("dbo.AfterSaleSkyWay");
            DropTable("dbo.BounceLine");
            DropTable("dbo.AfterSaleOrder");
            DropTable("dbo.AfterSalePassenger");
            DropTable("dbo.SkyWay");
            DropTable("dbo.DeductionDetail");
            DropTable("dbo.Policy");
            DropTable("dbo.Passenger");
            DropTable("dbo.PayBillDetail");
            DropTable("dbo.OrderPay");
            DropTable("dbo.Order");
            DropTable("dbo.OrderLog");
            DropTable("dbo.CoordinationLog");
            DropTable("dbo.AdjustDetail");
        }
    }
}
