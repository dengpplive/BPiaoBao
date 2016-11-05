using BPiaoBao.Client.DomesticTicket.Properties;
using BPiaoBao.Client.Module;
using System.Collections.Generic;
using System.Windows;
namespace BPiaoBao.Client.DomesticTicket
{
    /// <summary>
    /// 国内机票模块 接口
    /// </summary>
    [Plugin(ProjectCode, "国内机票", "pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/module_logo.png", 1, TicketBookingCode)]
    public class Main : IPlugin
    {
        public const string ProjectCode = "ticket1";
        public const string OrderCode = "10012";


        public const string TicketBookingName = "机票预订";
        public const string TicketBookingCode = "10014";
        public const string TicketBookingDesc = "方便快捷机票预订";
        public const string TicketBookingQueryResultName = "航班查询结果";
        public const string TicketBookingQueryResultCode = "10015";
        public const string PnrControlCode = "10011";

        public const string OrderQuery = "订单查询";
        public const string OrderQueryCode = "100121";
        public const string AfterSaleOrder = "售后订单";
        public const string AfterSaleOrderCode = "100122";

        public const string TravelManageOrder = "行程单管理";
        public const string TravelManageOrderCode = "100123";
        public const string TravelManageOrderDesc = "为您提供行程查询";

        public const string InsuranceManage = "保险管理";
        public const string InsuranceManageCode = "100141";
        public const string InsuranceManageDesc = "为您提供全方位保障";
        public const string InsuranceOrder = "电子保险";
        public const string InsuranceOrderCode = "100142";

        public const string UsualPassenger = "常旅客管理";
        public const string UsualPassengerCode = "100140";
        public const string UsualPassengerDesc = "快速选择乘机人";



        static Main()
        {
            Application.Current.Resources.MergedDictionaries.Add(SharedDictionaryManager.SharedDictionary);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        internal static void Initlize()
        {
            //什么都不用做，只是为了触发静态构造函数，引用常量是不会触发的
        }

        /// <summary>
        /// 获取模块菜单
        /// </summary>
        public IEnumerable<MainMenuItem> Parts
        {
            get
            {
                return new List<MainMenuItem>
                { 
                    new MainMenuItem
                    {
                        Name=TicketBookingName,
                        MenuCode = TicketBookingCode,
                        Description = TicketBookingDesc,
                        Icon = "pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/bookicon.png"
                    },
                    new MainMenuItem{
                        Name=Resources.PNRName,
                        MenuCode=Resources.PNRCode,
                        Description=Resources.PNRDesc,
                        Icon="pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/pnr.png"},
                     new MainMenuItem{
                        Name=Resources.OrderName,
                        MenuCode=Resources.OrderCode,
                        Description=Resources.OrderDesc,
                        Icon="pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/order.png",
                        Children=new List<MainMenuItem>{
                                 new MainMenuItem{
                                     Name= OrderQuery,
                                     MenuCode= OrderQueryCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name= AfterSaleOrder,
                                    MenuCode= AfterSaleOrderCode,
                                    Description=null
                                 },
                            }
                     },
                    new MainMenuItem{
                        Name = TravelManageOrder,
                        MenuCode = TravelManageOrderCode,
                        Description = TravelManageOrderDesc,
                        Icon="pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/travel.png",
                     },
                    new MainMenuItem{
                        Name = InsuranceManage,
                        MenuCode = InsuranceManageCode,
                        Description = InsuranceManageDesc,
                        Icon="pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/insurance.png",
                        Children=new List<MainMenuItem>{
                                 new MainMenuItem{
                                    Name = InsuranceManage,
                                    MenuCode = InsuranceManageCode,
                                    Description = InsuranceManageDesc,
                                 } ,
                                 new MainMenuItem{
                                    Name= InsuranceOrder,
                                    MenuCode= InsuranceOrderCode,
                                    Description=null
                                 } ,
                            }
                     },
                     new MainMenuItem{
                        Name=Resources.ReportName,
                        MenuCode=Resources.ReportCode,
                        Description=Resources.ReportDesc,
                        Icon="pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/report.png",
                        Children=new List<MainMenuItem>{
                                 new MainMenuItem{
                                     Name=Resources.TicketTableName,
                                     MenuCode=Resources.TickTableCode,
                                     Description=null
                                 },
                                 new MainMenuItem{
                                    Name=Resources.ReportAnalysis,
                                    MenuCode=Resources.ReportAnalysisCode,
                                    Description=null
                                 }
                            }
                        },
                    new MainMenuItem{
                        Name = UsualPassenger,
                        MenuCode = UsualPassengerCode,
                        Description = UsualPassengerDesc,
                        Icon="pack://application:,,,/BPiaoBao.Client.DomesticTicket;component/Image/clk.png"},

                };
            }
        }

    }
}
