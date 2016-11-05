using System;
using System.Globalization;
using System.Threading;
using BPiaoBao.Client.DomesticTicket.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace BPiaoBao.Client.DomesticTicket.View
{
    /// <summary>
    /// TicketBookingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TicketBookingWindow
    {
        public TicketBookingWindow()
        {
            InitializeComponent();
            // 在此点之下插入创建对象所需的代码。
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-Hans");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-Hans");

            Messenger.Default.Register<bool>(this, TicketBookingViewModel.CCloseWindow,
                p =>
                {
                    if (!p) return;
                    try
                    {
                        Messenger.Default.Unregister<bool>(this, TicketBookingViewModel.CCloseWindow); 
                        Close();
                    }
                    catch (Exception exx)
                    {
                        Console.WriteLine(exx.Message);
                    }
                });
        }

        
    }
}