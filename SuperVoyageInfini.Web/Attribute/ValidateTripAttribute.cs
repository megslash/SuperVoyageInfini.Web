using SuperVoyageInfini.Database.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SuperVoyageInfini.Web.Attribute
{
    public class ValidateTripAttribute : AuthorizeAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //Chercher le voyage qu'on veut vérifier les données
            int voyageId = int.Parse(httpContext.Request.RequestContext.RouteData.Values["Id"].ToString());
            Voyage voyage = db.Voyages.Single(v => v.Id == voyageId);

            //Chercher le activeUser

            //Vérifications pour l'image
            if((voyage.Image != "" || voyage.Image != null) && voyage.User == httpContext.User) return true;           

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Publish", controller = "Voyages" }));
        }
    }
}