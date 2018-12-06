using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Week6.MVC.Models
{
    public class ClubViewModel
    {
        [Display(Name = "Club Name")]
        public string ClubName { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }
        [Required]
        [Display(Name = "Student ID")]
        [RegularExpression(@"^[S|s]\d{8}$", 
            ErrorMessage = "Student ID must start with an S and have 9 Characters in Total")]
        public string StudentID { get; set; }
        
    }

    public class ClubAssignMemberViewModel
    {

        [Display(Name = "Club Name")]
        public string ClubName { get; set; }


        [Display(Name = "Number of Current Assigned members")]
        public int NoOfMembers { get; set; }

        [Display(Name = "Assign All?")]
        public bool AssignAll { get; set; }

        public int AssignedMember { get; set; }

    }
}