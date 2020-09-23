using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SuperVoyageInfini.Web.Controllers
{
    public class BaseController : Controller
    {

        private bool TryGetCultureInfo(string cultureCode)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo(cultureCode);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;

            }
        }

        public ActionResult ChangeLanguage(string id)
        {

            if (!TryGetCultureInfo(id)) return RedirectToAction("Index", "Voyages");

            Thread.CurrentThread.CurrentCulture = new CultureInfo(id);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            var cookie = new HttpCookie("_culture", id);
            cookie.Expires = DateTime.Today.AddYears(1);
            Response.SetCookie(cookie);

            return Redirect(Request.UrlReferrer.AbsolutePath);

        }
    }

}