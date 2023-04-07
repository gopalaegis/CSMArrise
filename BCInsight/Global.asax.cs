using BCInsight.App_Start;
using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BCInsight
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            FileInfo f = new FileInfo(MapPath("~/log4net.config"));
            if (f.Exists)
                log4net.Config.XmlConfigurator.ConfigureAndWatch(f);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
            DateTime dt = DateTime.Now;
            if (dt.Month == 4 && dt.Day < 10)
            {
                Start();
            }
        }

        private static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                return HostingEnvironment.MapPath(path);
            }
            else
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
                return Path.Combine(baseDirectory, path);
            }
        }

        public static void Start()
        {
        }
    }
}
