using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SuperVoyageInfini.Database.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SuperVoyageInfini.Web.Controllers
{
    [RequireHttps]
    public class VoyagesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            //Si un User est connecté, récuper le user dans un ViewBag
            

            foreach (Voyage v in db.Voyages.ToList())
            {
                if (v.IsPublic)
                {
                    v.Color = "green";
                }
                else if (User.Identity.IsAuthenticated)
                {
                    UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
                    UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
                    ApplicationUser activeUser = userManager.FindByName(User.Identity.Name);
                    ViewBag.ActiveUser = activeUser;

                    if (v.Participants.Contains(activeUser))
                    {
                        v.Color = "red";
                    }
                    else if(v.User == activeUser)
                    {
                        v.Color = "deepskyblue";
                    }
                }
            }


            return View(db.Voyages.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voyage voyage = db.Voyages.Find(id);
            if (voyage == null)
            {
                return HttpNotFound();
            }

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
            ApplicationUser activeUser = userManager.FindByName(User.Identity.Name);
            ViewBag.ActiveUser = activeUser;

            return View(voyage);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Image")] Voyage voyage)
        {
            if (ModelState.IsValid)
            {
                UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
                ApplicationUser activeUser = userManager.FindByName(User.Identity.Name);

                voyage.IsPending = false;
                voyage.IsPublic = false;
                voyage.User = activeUser;
                activeUser.Voyages.Add(voyage);
                db.Voyages.Add(voyage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(voyage);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voyage voyage = db.Voyages.Find(id);
            if (voyage == null)
            {
                return HttpNotFound();
            }
            return View(voyage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Voyage voyage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voyage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(voyage);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voyage voyage = db.Voyages.Find(id);
            if (voyage == null)
            {
                return HttpNotFound();
            }
            return View(voyage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Voyage voyage = db.Voyages.Find(id);
            db.Voyages.Remove(voyage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
            List<ApplicationUser> users = db.Users.ToList();

            //On ajoute dans la liste userAdmins les user qui ont le rôle admin
            List<ApplicationUser> userAdmins = new List<ApplicationUser>();
            foreach (ApplicationUser user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                    userAdmins.Add(user);
            }

            ViewBag.UserAdmins = userAdmins;

            //On va chercher le nom des voyages que le paramètre IsPending est set à true pour avoir les voyages qui ont été publiés
            //par le propriétaire.
            ViewBag.VoyagesPending = db.Voyages.Where(v => v.IsPending == true).Select(v => v.Name).ToList();
            List<ApplicationUser> Users = db.Users.ToList();

            return View(Users);
        }

        //On récupère le user choisi et on lui enlève le rôle d'admin
        public ActionResult RemoveAdmin(string Id)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
            userManager.RemoveFromRole(Id, "Admin");

            return RedirectToAction("Admin", "Voyages");
        }

        //On récupère le user choisi et on lui applique le rôle d'admin
        public ActionResult AddAdmin(string Id)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);
            userManager.AddToRole(Id, "Admin");

            return RedirectToAction("Admin", "Voyages");
        }

        //On récupère le voyage et on le rend publique et on précise qu'il n'est plus en attente.
        public ActionResult Approuve(string voyageName)
        {
            Voyage currentVoyage = db.Voyages.Where(v => v.Name == voyageName).SingleOrDefault();
            currentVoyage.IsPublic = true;
            currentVoyage.IsPending = false;
            db.SaveChanges();
            return RedirectToAction("Admin", "Voyages");
        }

        //Quand on clique sur le bouton Publier dans la page de détails d'un voyage, nous allons chercher.
        //le voyage qu'on veut publier, on change la valeur de IsPending à true et on retourne à la page de détails du voyage.
        public ActionResult Publish(int Id)
        {
            Voyage pendingVoyage = db.Voyages.Find(Id);
            pendingVoyage.IsPending = true;
            db.SaveChanges();
            return RedirectToAction("Details", "Voyages", new { id = Id });
        }

        [HttpPost]
        public ActionResult AddParticipant(string userInfo, int? Id)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            ApplicationUser participant = userManager.FindByEmail(userInfo);
            Voyage voyage = db.Voyages.Find(Id);

            //On vérifie si le participant est déjà dans la liste des participants du voyage pour ne pas avoir de doublon
            if (!voyage.Participants.Contains(participant))
            {
                voyage.Participants.Add(participant);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Voyages", new { id = Id });
        }

        [HttpPost]
        public ActionResult RemoveParticipant(string userInfo, int? Id)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            ApplicationUser participant = userManager.FindByEmail(userInfo);
            Voyage voyage = db.Voyages.Find(Id);

            //On vérifie si le participant est déjà dans la liste des participants du voyage pour ne pas avoir de doublon
            if (voyage.Participants.Contains(participant))
            {
                voyage.Participants.Remove(participant);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Voyages", new { id = Id });
        }    
    }
}
