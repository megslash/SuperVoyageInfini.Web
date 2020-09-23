using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SuperVoyageInfini.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            var cookie = HttpContext.Current.Request.Cookies["_culture"];
            var cultureId = cookie?.Value;


            if (string.IsNullOrEmpty(cultureId))
            {
                return;
            }

            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {
                culture = CultureInfo.GetCultureInfo(cultureId);
            }
            catch (CultureNotFoundException)
            {
                return;

            }
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

    }
}
