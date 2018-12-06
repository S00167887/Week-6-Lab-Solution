using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Week6.DataDomain;
using Week6.MVC.Models;

namespace Week6.MVC.Controllers
{
    [Authorize(Roles ="ClubAdmin")]
    public class ApproveMembersController : Controller
    {
        ClubContext db = new ClubContext();
        // GET: ClubAdmin
        public ActionResult Approve_Members()
        {
            // Get Current Logged in details
            ApplicationUser user = getUserDetails();
            // Get the associated member
            Member AdminMember = getCurrentUserMember(user);
            // Get the club of the logged in user
            Club club = getClub(AdminMember);
            // Get members that are unapproved for that club
            // Assumes that the members have registered 
            // and applied to join that club. IE members should apply for membership to existing clubs
            // This would require a member controller Join action (left as an excercise)
            // Fill Member List Select List
            ViewBag.UnAssignedMembers = getUnapprovedClubMembersSelectList(AdminMember);

            return View(new ClubAssignMemberViewModel()
            {
                AssignedMember = 0,
                AssignAll = false,
                ClubName = club.ClubName,
                NoOfMembers = club.clubMembers.Where(m => m.approved == true).Count() - 1
            });
        }

        private SelectList getUnapprovedClubMembersSelectList(Member adminMember)
        {
            // return a Select List for the club of the current admin member
            return new SelectList(
                    db.ClubMembers
                    .Where(c => c.myClub.ClubId == adminMember.myClub.ClubId
                            && c.MemberID != adminMember.MemberID
                            && c.approved == false)
                    .Select(m => new { m.MemberID, MemberName = m.studentMember.FirstName + " " + m.studentMember.SecondName })
                    , "MemberID", "MemberName");
        }
        private List<Member> getUnapprovedClubMembers(Member adminMember)
        {
            // return member for the current admin member user without themselves 
            // and only tha unassigned ones
            return db.ClubMembers
                    .Where(c => c.myClub.ClubId == adminMember.myClub.ClubId
                            && c.MemberID != adminMember.MemberID
                            && c.approved == false)
                            .ToList();
        }

        private Member getCurrentUserMember(ApplicationUser user)
        {
            return db.ClubMembers
                     .FirstOrDefault(m => m.StudentID == user.EntityID);

        }

        private Club getClub(Member member)
        {
            return db.Clubs.FirstOrDefault(c => c.ClubId == member.myClub.ClubId);
        }

        public ApplicationUser getUserDetails()
        {
            ApplicationDbContext AppAuthDb = new ApplicationDbContext();
            return AppAuthDb.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
        }


        [HttpPost]
        public ActionResult Approve_Members(ClubAssignMemberViewModel model)
        {
            ApplicationUser user = getUserDetails();

            Member AdminMember = getCurrentUserMember(user);
            Club club = getClub(AdminMember);

            // Approve all
            if (model.AssignAll)
            {
                List<Member> UnApprovedMembers = getUnapprovedClubMembers(getCurrentUserMember(getUserDetails()));
                MakeMemberRoles(UnApprovedMembers);
                UnApprovedMembers.ForEach(m => m.approved = true);

            }
            // Approve individual
            else
            {
                // Using a list as existing method can be used. Only one element in the list
                List<Member> memberToapprove = club.clubMembers.Where(m => m.MemberID == model.AssignedMember).ToList();
                MakeMemberRoles(memberToapprove);
                memberToapprove.ForEach(m => m.approved = true);
            }
            db.SaveChanges();
            ViewBag.UnAssignedMembers = getUnapprovedClubMembersSelectList(AdminMember);

            return View(model);
        }

        private void MakeMemberRoles(List<Member> unApprovedMembers)
        {
            foreach (var member in unApprovedMembers)
            {
                AssignMemberRole(member);
            }
        }

        private void AssignMemberRole(Member member)
        {
            using (ApplicationDbContext authDb = new ApplicationDbContext())
            {
                var manager =
                    new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(authDb));
                ApplicationUser user = manager.FindByEmail(member.StudentID + "@mail.itsligo.ie");
                if (user != null)
                    manager.AddToRole(user.Id, "Member");
            }
        }

    }
}