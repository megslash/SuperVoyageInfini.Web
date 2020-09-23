using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SuperVoyageInfini.Web.Startup))]
namespace SuperVoyageInfini.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
