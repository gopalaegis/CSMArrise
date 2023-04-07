using BCInsight.BAL.Repository;
using BCInsight.BAL.Services;
using BCInsight.Resolver;
using Microsoft.Practices.Unity;
using System.Web.Http;

namespace BCInsight.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new Microsoft.Practices.Unity.UnityContainer();
            container.RegisterType<IUser, UserRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IAttendance, AttendanceRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IClockInRequest, ClockInRequestRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ILeaveRequest, LeaveRequestRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ITaskDepartment, TaskDepartmentRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IDailytaskstatus, DailyTaskStatusRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<ITask, TaskRepository>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}