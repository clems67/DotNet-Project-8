using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using CalifornianHealthMonolithic.Controllers;

namespace CalifornianHealthMonolithic
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            //var rpcClientConsultantQueue = new RpcClientConsultantQueue();
            //var rpcClientAppointmentQueue = new RpcClientAppointmentQueue();
            //builder.RegisterType<RpcClientAppointmentQueue>().As<IRpcClientAppointmentQueue>().SingleInstance();
            //builder.RegisterType<RpcClientConsultantQueue>().As<IRpcClientConsultantQueue>().SingleInstance();

            DependencyResolver.SetResolver(new CustomDependencyResolver());

            //var hospitalContext = new CHEntities();
            //Database.SetInitializer(new ConsultantsInitializer());
            //Database.SetInitializer(new PatientsInitializer());
            //Database.SetInitializer(new ConsultantsCalendarInitializer());

            //hospitalContext.Database.Initialize(true);
        }
    }

    // Define your custom resolver
    public class CustomDependencyResolver : IDependencyResolver
    {
        private RpcClient rpcClientConsultant = new RpcClient("Consultant_queue");
        private RpcClient rpcClientAppointment = new RpcClient("Appointment_queue");
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(HomeController))
            {
                var controller = new HomeController();
                controller.rpcClient = rpcClientConsultant;
                return controller;
            }
            else if (serviceType == typeof(BookingController))
            {
                var controller = new BookingController();
                controller.rpcClient = rpcClientAppointment;
                return controller;
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
    }
}
