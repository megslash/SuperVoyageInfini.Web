namespace SuperVoyageInfini.Database.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SuperVoyageInfini.Database.Models;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //Cr�ation des voyages
            List<Voyage> voyages = new List<Voyage>
            {
              new Voyage { Name = "Montr�al", Description = "Aller au Stade Olympique", IsPublic = true, IsPending = false, Image = "/Content/images/montreal.jpg" },
              new Voyage { Name = "Sherbrooke", Description = "Aller � l'universit�", IsPublic = true, IsPending = false, Image = "/Content/images/sherbrooke.jpg" }
            };

            voyages.ForEach(v => context.Voyages.AddOrUpdate(v));
            context.SaveChanges();

            //Cr�ation de l'utilisateur admin
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);        

            ApplicationUser user = new ApplicationUser();
            user.UserName = "admin";
            user.Email = "admin@test.com";

            if (user != null)
            {
                var result = userManager.Create(user, "admin123");
            }

            //Cr�ation des r�les
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            if (!roleManager.RoleExists("User")) roleManager.Create(new IdentityRole("User"));
            if (!roleManager.RoleExists("Admin")) roleManager.Create(new IdentityRole("Admin"));

            //Ajouter le r�le admin au newUser
            userManager.AddToRole(user.Id, "Admin");
        }
    }
}
