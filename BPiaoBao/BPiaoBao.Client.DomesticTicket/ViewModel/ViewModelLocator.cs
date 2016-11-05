/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:BPiaoBao.Client.Account"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //共享的视图模型
            SimpleIoc.Default.Register<TicketBookingManagerViewModel>();
            SimpleIoc.Default.Register<TicketBookingQueryViewModel>();
          
        }

        public ViewModelLocator()
        {

        }

        /// <summary>
        /// 机票预订管理视图
        /// </summary>
        public static TicketBookingManagerViewModel TicketBookingManager
        {
            get { return GetViewModel<TicketBookingManagerViewModel>(); }
        }

        /// <summary>
        /// 机票预订查询结果视图
        /// </summary>
        public static TicketBookingQueryViewModel TicketBookingQuery
        {
            get { return GetViewModel<TicketBookingQueryViewModel>(); }
        }
       


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        /// <summary>
        /// 获取ViewModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T GetViewModel<T>() where T : new()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}