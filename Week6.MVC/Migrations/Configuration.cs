namespace Week6.MVC.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Week6.DataDomain;
    using Week6.MVC.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Week6.MVC.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Week6.MVC.Models.ApplicationDbContext context)
        {
            var manager =
                new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole { Name = "Admin" }
                );
            context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole { Name = "ClubAdmin" }
                );
            context.Roles.AddOrUpdate(r => r.Name,
                new IdentityRole { Name = "Member" }
                );

            PasswordHasher ps = new PasswordHasher();

            context.Users.AddOrUpdate(u => u.UserName,
                new ApplicationUser
                {
                    UserName = "admin@itsligo.ie",
                    Email = "admin@itsligo.ie",
                    EntityID = "admin",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = ps.HashPassword("Ppowell$1")
                });
            context.SaveChanges();
            // We seed the Club admin and Members from this context as it is easier to create a 
            // a club context hrer from the Class library than it is to create a ApplicationDbContext
            // in the club context
            Seed_club(context, manager);

        }

        private void Seed_club(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            ClubContext ctx = new ClubContext();
            Club club = ctx.Clubs.First();
            SeedClubMembersApplicationUsers(club, manager, context);
            SeedAdminMember(club, manager, context);
            ctx.SaveChanges();
        }

        private void SeedClubMembersApplicationUsers(Club club, UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {
            List<Member> members;
            // Create Application Logins for all seeded members 

            members = club.clubMembers.ToList();
            PasswordHasher ps = new PasswordHasher();
            foreach (var member in members)
            {
                ApplicationUser user = manager.FindByEmail(member.StudentID + "@mail.itsligo.ie");
                if (user == null)
                {
                    context.Users.AddOrUpdate(u => u.UserName,
                        new ApplicationUser
                        {
                            EntityID = member.StudentID,
                            Email = member.StudentID + "@mail.itsligo.ie",
                            UserName = member.StudentID + "@mail.itsligo.ie",
                            EmailConfirmed = true,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = ps.HashPassword(member.StudentID + "s$1")
                        });
                    context.SaveChanges();
                    ApplicationUser saveduser = manager.FindByEmail(member.StudentID + "@mail.itsligo.ie");
                    manager.AddToRoles(saveduser.Id, new string[] { "Member" });
                    context.SaveChanges();
                }
            }
        }
        private void SeedAdminMember(Club club, UserManager<ApplicationUser> manager, ApplicationDbContext context)
        {
            PasswordHasher ps = new PasswordHasher();
            Member chosenMember = club.clubMembers.FirstOrDefault();
            if (chosenMember == null)
            {
                throw new Exception("No Club Member available for " + club.ClubName);
            }
            else club.adminID = chosenMember.MemberID;
            
            // Add the membership and role for this member
            if (chosenMember != null)
            {
                context.Users.AddOrUpdate(u => u.UserName,
                    new ApplicationUser
                    {
                        EntityID = chosenMember.StudentID,
                        Email = chosenMember.StudentID + "@mail.itsligo.ie",
                        UserName = chosenMember.StudentID + "@mail.itsligo.ie",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        PasswordHash = ps.HashPassword(chosenMember.StudentID + "s$1")
                    });
                context.SaveChanges();
                ApplicationUser ChosenClubAdmin = manager.FindByEmail(chosenMember.StudentID + "@mail.itsligo.ie");
                if (ChosenClubAdmin != null)
                {
                    manager.AddToRoles(ChosenClubAdmin.Id, new string[] { "ClubAdmin" });
                }
                context.SaveChanges();
            }
        }
    }
}
